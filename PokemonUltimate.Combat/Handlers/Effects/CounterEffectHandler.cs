using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    /// <summary>
    /// Handler para efectos de contraataque (Counter, Mirror Coat).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class CounterEffectHandler : IMoveEffectHandler
    {
        private readonly DamageContextFactory _damageContextFactory;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(CounterEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de contraataque.
        /// </summary>
        /// <param name="damageContextFactory">La factory para crear contextos de daño. No puede ser null.</param>
        public CounterEffectHandler(DamageContextFactory damageContextFactory)
        {
            _damageContextFactory = damageContextFactory ?? throw new System.ArgumentNullException(nameof(damageContextFactory));
        }

        /// <summary>
        /// Verifica si el efecto de contraataque puede aplicarse.
        /// Solo funciona si el usuario recibió daño físico o especial este turno.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is CounterEffect counterEffect))
                return false;

            // El usuario debe estar activo
            if (user == null || user.IsEmpty || user.Pokemon.IsFainted)
                return false;

            // Verificar si recibió daño del tipo correspondiente este turno
            if (counterEffect.IsPhysicalCounter)
            {
                return user.PhysicalDamageTakenThisTurn > 0;
            }
            else
            {
                return user.SpecialDamageTakenThisTurn > 0;
            }
        }

        /// <summary>
        /// Procesa el efecto de contraataque y genera acciones.
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

            if (!(effect is CounterEffect counterEffect))
                return actions;

            // Verificar si puede aplicarse
            if (!CanApply(effect, user, target, field))
                return actions; // Counter falla silenciosamente si no hay daño

            // Calcular daño a devolver (2x el daño recibido)
            int damageToReturn = 0;
            if (counterEffect.IsPhysicalCounter)
            {
                damageToReturn = user.PhysicalDamageTakenThisTurn * 2;
            }
            else
            {
                damageToReturn = user.SpecialDamageTakenThisTurn * 2;
            }

            if (damageToReturn > 0)
            {
                // Crear contexto de daño para contraataque
                var counterContext = _damageContextFactory.CreateForCounter(user, target, damageToReturn, move, field);
                actions.Add(new DamageAction(user, target, counterContext));

                var provider = LocalizationService.Instance;
                actions.Add(new MessageAction(
                    provider.GetString(LocalizationKey.MoveCountered, user.Pokemon.DisplayName)));
            }

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento del efecto.
        /// Los efectos de contraataque no modifican valores directamente.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            // Los efectos de contraataque no modifican valores
            return null;
        }
    }
}
