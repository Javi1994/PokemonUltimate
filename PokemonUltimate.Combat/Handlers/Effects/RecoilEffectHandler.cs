using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    /// <summary>
    /// Handler para efectos de retroceso (recoil).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class RecoilEffectHandler : IMoveEffectHandler
    {
        private readonly DamageContextFactory _damageContextFactory;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(RecoilEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de retroceso.
        /// </summary>
        /// <param name="damageContextFactory">La factory para crear contextos de daño. No puede ser null.</param>
        /// <param name="handlerRegistry">El registry de handlers. No puede ser null.</param>
        public RecoilEffectHandler(
            DamageContextFactory damageContextFactory,
            CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory));
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Verifica si el efecto de retroceso puede aplicarse.
        /// El retroceso solo se aplica si se infligió daño.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is RecoilEffect))
                return false;

            // El retroceso solo se aplica si el usuario está activo y puede recibir daño
            return user != null && !user.IsEmpty && !user.Pokemon.IsFainted;
        }

        /// <summary>
        /// Procesa el efecto de retroceso y genera acciones.
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

            if (!(effect is RecoilEffect recoilEffect))
                return actions;

            // El retroceso solo se aplica si se infligió daño
            if (damageDealt <= 0)
                return actions;

            // Verificar si puede aplicarse
            if (!CanApply(effect, user, target, field))
                return actions;

            // Calcular daño de retroceso (usar valor modificado si aplica)
            int originalRecoil = (int)(damageDealt * recoilEffect.RecoilPercent / 100f);
            originalRecoil = Math.Max(1, originalRecoil); // Mínimo 1 HP

            // Modificar el valor si aplica (ej: Rock Head previene retroceso)
            int? modifiedRecoil = ModifyValue(effect, originalRecoil, "recoil", user, target);
            int finalRecoil = modifiedRecoil ?? originalRecoil;

            // Si el retroceso fue modificado a 0, no aplicar
            if (finalRecoil <= 0)
                return actions;

            // Crear contexto de daño para retroceso
            var recoilContext = _damageContextFactory.CreateForRecoil(user, finalRecoil, move, field);
            actions.Add(new DamageAction(user, user, recoilContext, _handlerRegistry));

            return actions;
        }

        /// <summary>
        /// Modifica el valor de retroceso basado en habilidades/items.
        /// Por ejemplo, Rock Head previene el retroceso.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            if (valueType != "recoil")
                return null;

            if (user?.Pokemon == null)
                return null;

            // Verificar si el usuario tiene Rock Head (previene retroceso)
            var rockHeadHandler = _handlerRegistry.GetAbilityHandler("rock-head");
            if (rockHeadHandler != null && rockHeadHandler.HasBehavior(user.Pokemon))
            {
                return 0; // Sin retroceso
            }

            // Verificar si el usuario tiene Magic Guard (previene daño indirecto)
            var magicGuardHandler = _handlerRegistry.GetAbilityHandler("magic-guard");
            if (magicGuardHandler != null && magicGuardHandler.HasBehavior(user.Pokemon))
            {
                return 0; // Sin retroceso (daño indirecto)
            }

            return null; // Usar valor original
        }
    }
}
