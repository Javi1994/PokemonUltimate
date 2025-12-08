using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    /// <summary>
    /// Handler para efectos de status (Burn, Poison, Paralysis, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class StatusEffectHandler : IMoveEffectHandler
    {
        private readonly IRandomProvider _randomProvider;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(StatusEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de status.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        /// <param name="handlerRegistry">El registry de handlers. No puede ser null.</param>
        public StatusEffectHandler(
            Infrastructure.Providers.Definition.IRandomProvider randomProvider = null,
            CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _randomProvider = randomProvider ?? new Infrastructure.Providers.RandomProvider();
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Verifica si el efecto de status puede aplicarse.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is StatusEffect statusEffect))
                return false;

            // Verificar probabilidad
            if (_randomProvider.Next(100) >= statusEffect.ChancePercent)
                return false;

            // Determinar el slot objetivo
            var targetSlot = statusEffect.TargetSelf ? user : target;

            // Usar el handler de aplicación de status para verificar si puede aplicarse
            var statusApplicationHandler = _handlerRegistry.GetStatusApplicationHandler();
            var result = statusApplicationHandler.CanApplyStatus(targetSlot, statusEffect.Status, field);

            return result.CanApply;
        }

        /// <summary>
        /// Procesa el efecto de status y genera acciones.
        /// </summary>
        public List<BattleAction> Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt)
        {
            var actions = new List<BattleAction>();

            if (!(effect is StatusEffect statusEffect))
                return actions;

            // Verificar si puede aplicarse (incluye verificación de probabilidad)
            if (!CanApply(effect, user, target, field))
                return actions;

            // Determinar el slot objetivo
            var targetSlot = statusEffect.TargetSelf ? user : target;

            // Crear acción de aplicación de status
            actions.Add(new ApplyStatusAction(user, targetSlot, statusEffect.Status, _handlerRegistry));

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento del efecto.
        /// Los efectos de status no modifican valores directamente.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            // Los efectos de status no modifican valores
            return null;
        }
    }
}
