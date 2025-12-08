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
    /// Handler para efectos de curación directa.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class HealEffectHandler : IMoveEffectHandler
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(HealEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de curación.
        /// </summary>
        /// <param name="handlerRegistry">El registry de handlers. No puede ser null.</param>
        public HealEffectHandler(CombatEffectHandlerRegistry handlerRegistry = null)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Verifica si el efecto de curación puede aplicarse.
        /// HealEffect siempre cura al usuario.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is HealEffect))
                return false;

            // HealEffect siempre cura al usuario
            if (user == null || user.IsEmpty || user.Pokemon.IsFainted)
                return false;

            // Usar el handler de aplicación de curación para verificar
            var healingHandler = _handlerRegistry.GetHealingApplicationHandler();
            return healingHandler.CanHeal(user.Pokemon, 1); // Verificar si puede recibir cualquier curación
        }

        /// <summary>
        /// Procesa el efecto de curación y genera acciones.
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

            if (!(effect is HealEffect healEffect))
                return actions;

            // Verificar si puede aplicarse
            if (!CanApply(effect, user, target, field))
                return actions;

            // HealEffect siempre cura al usuario
            var targetSlot = user;

            // Calcular cantidad de curación basada en porcentaje
            int healAmount = (int)(targetSlot.Pokemon.MaxHP * healEffect.HealPercent / 100f);

            // Modificar el valor si aplica
            int? modifiedHealing = ModifyValue(effect, healAmount, "heal", user, target);
            int finalHealing = modifiedHealing ?? healAmount;

            // Si la curación fue modificada a 0, no aplicar
            if (finalHealing <= 0)
                return actions;

            // Crear acción de curación
            actions.Add(new HealAction(user, targetSlot, finalHealing, _handlerRegistry));

            return actions;
        }

        /// <summary>
        /// Modifica el valor de curación basado en habilidades/items o clima.
        /// Por ejemplo, clima afecta la curación de movimientos como Moonlight.
        /// Nota: HealEffect actualmente no tiene soporte para modificadores de clima,
        /// pero este método está preparado para futuras extensiones.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            if (valueType != "heal")
                return null;

            if (!(effect is HealEffect))
                return null;

            // HealEffect actualmente no tiene propiedad WeatherModified.
            // Si en el futuro se agrega soporte para modificadores de clima,
            // aquí se podría verificar el clima del campo y aplicar modificadores.
            // Por ahora, no se modifica el valor.

            return null; // Usar valor original
        }
    }
}
