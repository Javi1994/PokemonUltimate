using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    /// <summary>
    /// Handler para efectos de drenaje (drain).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class DrainEffectHandler : IMoveEffectHandler
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(DrainEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de drenaje.
        /// </summary>
        /// <param name="handlerRegistry">El registry de handlers. No puede ser null.</param>
        public DrainEffectHandler(CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Verifica si el efecto de drenaje puede aplicarse.
        /// El drenaje solo se aplica si se infligió daño y el usuario puede ser curado.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is DrainEffect))
                return false;

            // El drenaje solo se aplica si el usuario está activo y puede recibir curación
            return user != null && !user.IsEmpty && !user.Pokemon.IsFainted;
        }

        /// <summary>
        /// Procesa el efecto de drenaje y genera acciones.
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

            if (!(effect is DrainEffect drainEffect))
                return actions;

            // El drenaje solo se aplica si se infligió daño
            if (damageDealt <= 0)
                return actions;

            // Verificar si puede aplicarse
            if (!CanApply(effect, user, target, field))
                return actions;

            // Calcular curación (usar valor modificado si aplica)
            int originalHealing = (int)(damageDealt * drainEffect.DrainPercent / 100f);

            // Modificar el valor si aplica
            int? modifiedHealing = ModifyValue(effect, originalHealing, "drain", user, target);
            int finalHealing = modifiedHealing ?? originalHealing;

            // Si la curación fue modificada a 0, no aplicar
            if (finalHealing <= 0)
                return actions;

            // Crear acción de curación
            actions.Add(new HealAction(user, user, finalHealing, _handlerRegistry));

            return actions;
        }

        /// <summary>
        /// Modifica el valor de drenaje basado en habilidades/items.
        /// Por ejemplo, Big Root aumenta el drenaje.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            if (valueType != "drain")
                return null;

            if (user?.Pokemon == null)
                return null;

            // Verificar si el usuario tiene Big Root (aumenta drenaje en 30%)
            var bigRootHandler = _handlerRegistry.GetItemHandler("big-root");
            if (bigRootHandler != null && bigRootHandler.HasBehavior(user.Pokemon))
            {
                return (int)(originalValue * 1.3f); // Aumento del 30%
            }

            return null; // Usar valor original
        }
    }
}
