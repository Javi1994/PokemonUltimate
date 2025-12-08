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
    /// Handler para efectos de cambio de estadísticas.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class StatChangeEffectHandler : IMoveEffectHandler
    {
        private readonly IRandomProvider _randomProvider;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(StatChangeEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de cambio de stats.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        /// <param name="handlerRegistry">El registry de handlers. No puede ser null.</param>
        public StatChangeEffectHandler(
            Infrastructure.Providers.Definition.IRandomProvider randomProvider = null,
            CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _randomProvider = randomProvider ?? new Infrastructure.Providers.RandomProvider();
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Verifica si el efecto de cambio de stats puede aplicarse.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is StatChangeEffect statChangeEffect))
                return false;

            // Verificar probabilidad
            if (_randomProvider.Next(100) >= statChangeEffect.ChancePercent)
                return false;

            // Determinar el slot objetivo
            var targetSlot = statChangeEffect.TargetSelf ? user : target;

            // Usar el handler de aplicación de cambios de stats para verificar si puede aplicarse
            var statChangeApplicationHandler = _handlerRegistry.GetStatChangeApplicationHandler();
            bool canApply = statChangeApplicationHandler.CanApplyStatChange(
                targetSlot, statChangeEffect.TargetSelf ? user : null, statChangeEffect.Stages, field);

            return canApply;
        }

        /// <summary>
        /// Procesa el efecto de cambio de stats y genera acciones.
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

            if (!(effect is StatChangeEffect statChangeEffect))
                return actions;

            // Verificar si puede aplicarse (incluye verificación de probabilidad)
            if (!CanApply(effect, user, target, field))
                return actions;

            // Determinar el slot objetivo
            var targetSlot = statChangeEffect.TargetSelf ? user : target;

            // Aplicar inversión de stats si el Pokemon tiene Contrary
            int stages = statChangeEffect.Stages;
            if (targetSlot.Pokemon != null)
            {
                var reversalHandler = _handlerRegistry.GetStatChangeReversalHandler();
                stages = reversalHandler.ReverseStatChange(targetSlot.Pokemon, stages);
            }

            // Crear acción de cambio de stats
            actions.Add(new StatChangeAction(user, targetSlot, statChangeEffect.TargetStat, stages, _handlerRegistry));

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento del efecto.
        /// Puede invertir los cambios de stats si el Pokemon tiene Contrary.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            if (valueType != "statChange")
                return null;

            if (!(effect is StatChangeEffect statChangeEffect))
                return null;

            // Determinar el slot objetivo
            var targetSlot = statChangeEffect.TargetSelf ? user : target;
            if (targetSlot?.Pokemon == null)
                return null;

            // Aplicar inversión si tiene Contrary
            var reversalHandler = _handlerRegistry.GetStatChangeReversalHandler();
            int modifiedStages = reversalHandler.ReverseStatChange(targetSlot.Pokemon, originalValue);

            return modifiedStages != originalValue ? modifiedStages : (int?)null;
        }
    }
}
