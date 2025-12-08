using System.Collections.Generic;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador y procesador de aplicación de daño.
    /// Maneja la lógica compleja de aplicación de daño, incluyendo prevención de OHKO y registro de daño.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class DamageApplicationChecker
    {
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Crea una nueva instancia del verificador de aplicación de daño.
        /// </summary>
        /// <param name="behaviorRegistry">El registry de behavior checkers. Si es null, crea uno por defecto.</param>
        public DamageApplicationChecker(BehaviorCheckerRegistry behaviorRegistry = null)
        {
            _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();
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
                var ohkoChecker = _behaviorRegistry.GetOHKOPreventionChecker();

                if (ohkoChecker.HasBehavior(target.Pokemon))
                {
                    // Calculate modified damage (leaves Pokemon at 1 HP)
                    damage = ohkoChecker.CalculateModifiedDamage(target.Pokemon, damage);

                    var preventionType = ohkoChecker.GetPreventionType(target.Pokemon);
                    var provider = LocalizationService.Instance;

                    // Generate reaction messages based on prevention type
                    if (preventionType == OHKOPreventionType.Item)
                    {
                        // Focus Sash - consume item and generate messages
                        target.Pokemon.HeldItem = null;
                        var itemName = GameContentReferences.FocusSash.GetDisplayName(provider) ?? "Focus Sash";
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.ItemActivated, target.Pokemon.DisplayName, itemName)));
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.HeldOnUsingItem, target.Pokemon.DisplayName, itemName)));
                    }
                    else if (preventionType == OHKOPreventionType.Ability)
                    {
                        // Sturdy - ability doesn't get consumed, generate messages
                        var abilityName = GameContentReferences.Sturdy.GetDisplayName(provider) ?? "Sturdy";
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, target.Pokemon.DisplayName, abilityName)));
                        reactions.Add(new MessageAction(provider.GetString(LocalizationKey.EnduredHit, target.Pokemon.DisplayName)));
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
