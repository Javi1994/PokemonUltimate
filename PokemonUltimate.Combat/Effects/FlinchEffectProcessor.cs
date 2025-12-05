using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Effects
{
    /// <summary>
    /// Processes FlinchEffect (applies flinch status to target).
    /// </summary>
    public class FlinchEffectProcessor : IMoveEffectProcessor
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new FlinchEffectProcessor.
        /// </summary>
        /// <param name="randomProvider">The random provider for chance checks. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public FlinchEffectProcessor(IRandomProvider randomProvider)
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
            if (effect is FlinchEffect flinchEffect)
            {
                // Apply flinch to target (if chance succeeds)
                if (_randomProvider.Next(100) < flinchEffect.ChancePercent)
                {
                    target.AddVolatileStatus(VolatileStatus.Flinch);
                }
            }
        }
    }
}
