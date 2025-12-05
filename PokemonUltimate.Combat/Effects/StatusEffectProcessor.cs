using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes StatusEffect (applies status conditions like Burn, Poison, etc.).
    /// </summary>
    public class StatusEffectProcessor : IMoveEffectProcessor
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new StatusEffectProcessor.
        /// </summary>
        /// <param name="randomProvider">The random provider for chance checks. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public StatusEffectProcessor(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        }

        public void Process(
            Core.Effects.IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            Core.Blueprints.MoveData move,
            BattleField field,
            int damageDealt,
            List<BattleAction> actions)
        {
            if (effect is StatusEffect statusEffect)
            {
                // Check chance
                if (_randomProvider.Next(100) < statusEffect.ChancePercent)
                {
                    var targetSlot = statusEffect.TargetSelf ? user : target;
                    actions.Add(new ApplyStatusAction(user, targetSlot, statusEffect.Status));
                }
            }
        }
    }
}
