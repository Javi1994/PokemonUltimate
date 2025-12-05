using System.Collections.Generic;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Managers
{
    /// <summary>
    /// Interface for managing stat stages in battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    public interface IStatStageManager
    {
        /// <summary>
        /// Creates a new dictionary with all stat stages initialized to 0.
        /// </summary>
        Dictionary<Stat, int> CreateInitialStages();

        /// <summary>
        /// Modifies a stat stage by the given amount, clamped to -6/+6.
        /// </summary>
        /// <param name="stages">The stat stages dictionary to modify</param>
        /// <param name="stat">The stat to modify</param>
        /// <param name="change">The amount to change (+/-)</param>
        /// <returns>The actual change applied (may be less if clamped)</returns>
        int ModifyStage(Dictionary<Stat, int> stages, Stat stat, int change);

        /// <summary>
        /// Resets all stat stages to 0.
        /// </summary>
        /// <param name="stages">The stat stages dictionary to reset</param>
        void ResetStages(Dictionary<Stat, int> stages);

        /// <summary>
        /// Gets the current stat stage for a stat, defaulting to 0 if not found.
        /// </summary>
        int GetStage(Dictionary<Stat, int> stages, Stat stat);

        /// <summary>
        /// Validates that a stat stage is within valid range (-6 to +6).
        /// </summary>
        bool IsValidStage(int stage);
    }
}
