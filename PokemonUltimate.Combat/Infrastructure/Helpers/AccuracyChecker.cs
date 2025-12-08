using System;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Providers;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Services;

namespace PokemonUltimate.Combat.Infrastructure.Helpers
{
    /// <summary>
    /// Checks if a move hits its target based on accuracy and evasion stages.
    /// Centralized accuracy calculation system for combat actions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class AccuracyChecker
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new AccuracyChecker with a random provider.
        /// </summary>
        /// <param name="randomProvider">The random provider for accuracy rolls. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public AccuracyChecker(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider), ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Checks if a move hits its target.
        /// </summary>
        /// <param name="user">The slot using the move.</param>
        /// <param name="target">The target slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="fixedRandomValue">Fixed random value for testing (0.0 to 1.0).</param>
        /// <returns>True if the move hits, false if it misses.</returns>
        /// <exception cref="ArgumentNullException">If user, target, or move is null.</exception>
        public bool CheckHit(BattleSlot user, BattleSlot target, MoveData move, float? fixedRandomValue = null)
        {
            return CheckHit(user, target, move, field: null, fixedRandomValue);
        }

        /// <summary>
        /// Checks if a move hits its target, considering weather accuracy modifiers.
        /// </summary>
        /// <param name="user">The slot using the move.</param>
        /// <param name="target">The target slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="field">The battlefield. Used for weather accuracy modifiers.</param>
        /// <param name="fixedRandomValue">Fixed random value for testing (0.0 to 1.0).</param>
        /// <returns>True if the move hits, false if it misses.</returns>
        /// <exception cref="ArgumentNullException">If user, target, or move is null.</exception>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.12: Weather System (Perfect Accuracy)
        /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
        /// </remarks>
        public bool CheckHit(BattleSlot user, BattleSlot target, MoveData move, BattleField field, float? fixedRandomValue = null)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            // Always-hit moves never miss
            if (move.NeverMisses)
                return true;

            // Status moves with 0 accuracy always hit (like status moves)
            if (move.Accuracy == 0)
                return true;

            // Check weather perfect accuracy (Thunder/Hurricane in Rain, Blizzard in Hail)
            if (field != null && field.WeatherData != null)
            {
                if (field.WeatherData.HasPerfectAccuracy(move.Name))
                {
                    // Weather grants perfect accuracy - always hit
                    return true;
                }
            }

            // Calculate effective accuracy
            float accuracyMultiplier = StatCalculatorService.GetAccuracyStageMultiplier(user.GetStatStage(Stat.Accuracy));
            float evasionMultiplier = StatCalculatorService.GetAccuracyStageMultiplier(target.GetStatStage(Stat.Evasion));

            float effectiveAccuracy = move.Accuracy * (accuracyMultiplier / evasionMultiplier);
            effectiveAccuracy = Math.Max(1f, Math.Min(100f, effectiveAccuracy)); // Clamp to 1-100%

            // Roll for hit
            float roll = fixedRandomValue ?? _randomProvider.NextFloat() * 100f;
            return roll < effectiveAccuracy;
        }
    }
}

