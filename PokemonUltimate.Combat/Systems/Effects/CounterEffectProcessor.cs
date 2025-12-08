using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Systems.Effects.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Processes CounterEffect (returns 2x damage taken this turn).
    /// </summary>
    public class CounterEffectProcessor : IMoveEffectProcessor
    {
        private readonly DamageContextFactory _damageContextFactory;

        /// <summary>
        /// Creates a new CounterEffectProcessor.
        /// </summary>
        /// <param name="damageContextFactory">The factory for creating damage contexts. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If damageContextFactory is null.</exception>
        public CounterEffectProcessor(DamageContextFactory damageContextFactory)
        {
            _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory));
        }

        public void Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt,
            List<BattleAction> actions)
        {
            if (effect is CounterEffect counterEffect)
            {
                // Counter/Mirror Coat: returns 2x damage taken this turn
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
                    // Create damage context for counter damage
                    var counterContext = _damageContextFactory.CreateForCounter(user, target, damageToReturn, move, field);
                    actions.Add(new DamageAction(user, target, counterContext));
                    var provider = LocalizationService.Instance;
                    actions.Add(new MessageAction(provider.GetString(LocalizationKey.MoveCountered, user.Pokemon.DisplayName)));
                }
                // If no damage taken, Counter fails silently (no message)
            }
        }
    }
}
