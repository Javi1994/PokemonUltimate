using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Processes ProtectEffect (applies Protect status with decreasing success rate).
    /// </summary>
    public class ProtectEffectProcessor : IMoveEffectProcessor
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new ProtectEffectProcessor.
        /// </summary>
        /// <param name="randomProvider">The random provider for chance checks. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public ProtectEffectProcessor(IRandomProvider randomProvider)
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
            if (effect is ProtectEffect protectEffect)
            {
                // Apply Protect: success rate halves with consecutive use (100%, 50%, 25%, 12.5%...)
                // Counter increments regardless of success (for tracking consecutive uses)
                int consecutiveUses = user.ProtectConsecutiveUses;
                int successRate = 100;
                for (int i = 0; i < consecutiveUses; i++)
                {
                    successRate /= 2; // Halve each time
                }

                // Increment counter before checking success (tracks consecutive uses)
                user.IncrementProtectUses();

                var provider = LocalizationService.Instance;
                if (_randomProvider.Next(100) < successRate)
                {
                    user.AddVolatileStatus(VolatileStatus.Protected);
                    actions.Add(new MessageAction(provider.GetString(LocalizationKey.BattleProtected, user.Pokemon.DisplayName)));
                }
                else
                {
                    actions.Add(new MessageAction(provider.GetString(LocalizationKey.MoveProtectFailed, user.Pokemon.DisplayName)));
                }
            }
        }
    }
}
