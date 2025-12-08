using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler para procesamiento de aplicación de daño.
    /// Maneja la lógica compleja de aplicación de daño, incluyendo prevención de OHKO y registro de daño.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class DamageApplicationHandler
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Crea una nueva instancia del handler de aplicación de daño.
        /// </summary>
        /// <param name="handlerRegistry">El registry de handlers. Si es null, crea uno por defecto.</param>
        public DamageApplicationHandler(CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

        /// <summary>
        /// Procesa la aplicación de daño, incluyendo prevención de OHKO y generación de mensajes.
        /// </summary>
        /// <param name="target">El slot que recibe el daño. No puede ser null.</param>
        /// <param name="damage">El daño original a aplicar.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Resultado con el daño modificado y acciones de reacción generadas.</returns>
        public DamageApplicationResult ProcessDamageApplication(BattleSlot target, int damage, BattleField field)
        {
            var reactions = new List<BattleAction>();

            // No damage to apply
            if (damage == 0)
            {
                return new DamageApplicationResult(0, reactions);
            }

            // Check if damage would cause fainting BEFORE applying damage (for Focus Sash, Sturdy)
            bool wouldFaint = target.Pokemon.CurrentHP <= damage;
            bool wasAtFullHP = target.Pokemon.CurrentHP >= target.Pokemon.MaxHP;

            // Trigger OnWouldFaint BEFORE applying damage if damage would be fatal
            // This allows items/abilities to prevent fainting (Focus Sash, Sturdy)
            if (wouldFaint && wasAtFullHP)
            {
                // Obtener handler de OHKO prevention
                var ohkoHandler = _handlerRegistry.GetItemHandler("focus-sash") as OHKOPreventionHandler;
                if (ohkoHandler == null)
                {
                    // Intentar obtener por ability (Sturdy)
                    ohkoHandler = _handlerRegistry.GetAbilityHandler("sturdy") as OHKOPreventionHandler;
                }

                if (ohkoHandler != null && ohkoHandler.HasBehavior(target.Pokemon))
                {
                    // Calculate modified damage (leaves Pokemon at 1 HP)
                    damage = ohkoHandler.CalculateModifiedDamage(target.Pokemon, damage);

                    var preventionType = ohkoHandler.GetPreventionType(target.Pokemon);

                    // Generate reaction messages based on prevention type
                    if (preventionType == OHKOPreventionType.Item)
                    {
                        // Focus Sash - consume item and generate messages
                        var itemHandler = ohkoHandler as IItemEffectHandler;
                        if (itemHandler != null && target.Pokemon.HeldItem != null)
                        {
                            var itemActions = itemHandler.Process(target.Pokemon.HeldItem, target, field);
                            reactions.AddRange(itemActions);
                        }
                    }
                    else if (preventionType == OHKOPreventionType.Ability)
                    {
                        // Sturdy - ability doesn't get consumed, generate messages
                        if (target.Pokemon.Ability != null)
                        {
                            var abilityActions = ohkoHandler.Process(target.Pokemon.Ability, target, field);
                            reactions.AddRange(abilityActions);
                        }
                    }
                }
            }

            return new DamageApplicationResult(damage, reactions);
        }

        /// <summary>
        /// Registra el daño recibido para efectos como Counter/Mirror Coat y Focus Punch.
        /// </summary>
        /// <param name="target">El slot que recibió el daño. No puede ser null.</param>
        /// <param name="actualDamage">El daño real aplicado.</param>
        /// <param name="moveCategory">La categoría del movimiento que causó el daño.</param>
        public void RecordDamageTaken(BattleSlot target, int actualDamage, MoveCategory moveCategory)
        {
            if (target == null || actualDamage <= 0)
                return;

            // Record damage taken for Counter/Mirror Coat
            if (moveCategory == MoveCategory.Physical)
            {
                target.RecordPhysicalDamage(actualDamage);
            }
            else if (moveCategory == MoveCategory.Special)
            {
                target.RecordSpecialDamage(actualDamage);
            }

            // Mark if target was hit while focusing (for Focus Punch)
            target.MarkHitWhileFocusing();
        }
    }

    /// <summary>
    /// Resultado del procesamiento de aplicación de daño.
    /// </summary>
    public class DamageApplicationResult
    {
        /// <summary>
        /// El daño modificado (puede haber sido reducido por Focus Sash/Sturdy).
        /// </summary>
        public int ModifiedDamage { get; }

        /// <summary>
        /// Acciones de reacción generadas (mensajes, etc.).
        /// </summary>
        public IReadOnlyList<BattleAction> Reactions { get; }

        /// <summary>
        /// Crea un nuevo resultado de aplicación de daño.
        /// </summary>
        /// <param name="modifiedDamage">El daño modificado.</param>
        /// <param name="reactions">Las acciones de reacción generadas.</param>
        public DamageApplicationResult(int modifiedDamage, IReadOnlyList<BattleAction> reactions)
        {
            ModifiedDamage = modifiedDamage;
            Reactions = reactions ?? new List<BattleAction>();
        }
    }
}
