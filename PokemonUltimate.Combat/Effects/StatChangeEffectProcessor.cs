using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes StatChangeEffect (modifies stat stages).
    /// </summary>
    public class StatChangeEffectProcessor : IMoveEffectProcessor
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new StatChangeEffectProcessor.
        /// </summary>
        /// <param name="randomProvider">The random provider for chance checks. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public StatChangeEffectProcessor(IRandomProvider randomProvider)
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
            if (effect is StatChangeEffect statChangeEffect)
            {
                // Check chance
                if (_randomProvider.Next(100) < statChangeEffect.ChancePercent)
                {
                    var targetSlot = statChangeEffect.TargetSelf ? user : target;
                    actions.Add(new StatChangeAction(user, targetSlot, statChangeEffect.TargetStat, statChangeEffect.Stages));
                }
            }
        }
    }
}
