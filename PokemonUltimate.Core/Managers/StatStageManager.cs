using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Managers
{
    /// <summary>
    /// Manages stat stages for Pokemon in battle.
    /// Encapsulates initialization, modification, and validation logic.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    public class StatStageManager : IStatStageManager
    {
        private static readonly IStatStageManager _defaultInstance = new StatStageManager();
        public static IStatStageManager Default => _defaultInstance;

        /// <summary>
        /// Creates a new dictionary with all stat stages initialized to 0.
        /// </summary>
        public Dictionary<Stat, int> CreateInitialStages()
        {
            return new Dictionary<Stat, int>
            {
                { Stat.Attack, 0 },
                { Stat.Defense, 0 },
                { Stat.SpAttack, 0 },
                { Stat.SpDefense, 0 },
                { Stat.Speed, 0 },
                { Stat.Accuracy, 0 },
                { Stat.Evasion, 0 }
            };
        }

        /// <summary>
        /// Modifies a stat stage by the given amount, clamped to -6/+6.
        /// </summary>
        public int ModifyStage(Dictionary<Stat, int> stages, Stat stat, int change)
        {
            if (stages == null)
                throw new ArgumentNullException(nameof(stages));

            if (stat == Stat.HP)
                throw new ArgumentException(ErrorMessages.CannotModifyHPStatStage, nameof(stat));

            if (!stages.ContainsKey(stat))
                return 0;

            int oldStage = stages[stat];
            int newStage = Math.Max(CoreConstants.MinStatStage,
                Math.Min(CoreConstants.MaxStatStage, oldStage + change));

            stages[stat] = newStage;
            return newStage - oldStage;
        }

        /// <summary>
        /// Resets all stat stages to 0.
        /// </summary>
        public void ResetStages(Dictionary<Stat, int> stages)
        {
            if (stages == null)
                throw new ArgumentNullException(nameof(stages));

            stages[Stat.Attack] = 0;
            stages[Stat.Defense] = 0;
            stages[Stat.SpAttack] = 0;
            stages[Stat.SpDefense] = 0;
            stages[Stat.Speed] = 0;
            stages[Stat.Accuracy] = 0;
            stages[Stat.Evasion] = 0;
        }

        /// <summary>
        /// Gets the current stat stage for a stat, defaulting to 0 if not found.
        /// </summary>
        public int GetStage(Dictionary<Stat, int> stages, Stat stat)
        {
            if (stages == null)
                return 0;

            return stages.TryGetValue(stat, out var stage) ? stage : 0;
        }

        /// <summary>
        /// Validates that a stat stage is within valid range (-6 to +6).
        /// </summary>
        public bool IsValidStage(int stage)
        {
            return stage >= CoreConstants.MinStatStage && stage <= CoreConstants.MaxStatStage;
        }
    }
}
