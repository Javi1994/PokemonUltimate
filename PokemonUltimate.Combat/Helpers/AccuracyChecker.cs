using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Combat.Helpers
{
    /// <summary>
    /// Checks if a move hits its target based on accuracy and evasion stages.
    /// Centralized accuracy calculation system for combat actions.
    /// </summary>
    public static class AccuracyChecker
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Checks if a move hits its target.
        /// </summary>
        /// <param name="user">The slot using the move.</param>
        /// <param name="target">The target slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="fixedRandomValue">Fixed random value for testing (0.0 to 1.0).</param>
        /// <returns>True if the move hits, false if it misses.</returns>
        /// <exception cref="ArgumentNullException">If user, target, or move is null.</exception>
        public static bool CheckHit(BattleSlot user, BattleSlot target, MoveData move, float? fixedRandomValue = null)
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

            // Calculate effective accuracy
            float accuracyMultiplier = StatCalculator.GetAccuracyStageMultiplier(user.GetStatStage(Stat.Accuracy));
            float evasionMultiplier = StatCalculator.GetAccuracyStageMultiplier(target.GetStatStage(Stat.Evasion));
            
            float effectiveAccuracy = move.Accuracy * (accuracyMultiplier / evasionMultiplier);
            effectiveAccuracy = Math.Max(1f, Math.Min(100f, effectiveAccuracy)); // Clamp to 1-100%

            // Roll for hit
            float roll = fixedRandomValue ?? (float)_random.NextDouble() * 100f;
            return roll < effectiveAccuracy;
        }
    }
}

