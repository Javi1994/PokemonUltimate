using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Systems.Effects
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
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
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
