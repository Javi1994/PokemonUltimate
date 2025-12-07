using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Foundation.ValueObjects
{
    /// <summary>
    /// Value Object representing stat stages for a Pokemon in battle.
    /// Encapsulates stat stage management with validation and immutability patterns.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class StatStages
    {
        private const int MinStatStage = -6;
        private const int MaxStatStage = 6;

        private readonly Dictionary<Stat, int> _stages;

        /// <summary>
        /// Creates a new StatStages instance with all stats at stage 0.
        /// </summary>
        public StatStages()
        {
            _stages = new Dictionary<Stat, int>
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
        /// Creates a new StatStages instance with the specified stages.
        /// </summary>
        /// <param name="stages">Dictionary of stat stages. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If stages is null.</exception>
        private StatStages(Dictionary<Stat, int> stages)
        {
            _stages = stages ?? throw new ArgumentNullException(nameof(stages));
        }

        /// <summary>
        /// Gets the current stat stage for a stat (-6 to +6).
        /// </summary>
        /// <param name="stat">The stat to query. Cannot be HP.</param>
        /// <returns>The current stage (-6 to +6). Returns 0 for HP or unknown stats.</returns>
        public int GetStage(Stat stat)
        {
            if (stat == Stat.HP)
                return 0;

            return _stages.TryGetValue(stat, out var stage) ? stage : 0;
        }

        /// <summary>
        /// Creates a new StatStages instance with the specified stat modified.
        /// </summary>
        /// <param name="stat">The stat to modify. Cannot be HP.</param>
        /// <param name="change">The amount to change (+/-).</param>
        /// <param name="actualChange">The actual change applied (may be less if clamped).</param>
        /// <returns>A new StatStages instance with the modification applied.</returns>
        /// <exception cref="ArgumentException">If stat is HP.</exception>
        public StatStages ModifyStage(Stat stat, int change, out int actualChange)
        {
            if (stat == Stat.HP)
                throw new ArgumentException(ErrorMessages.CannotModifyHPStatStage, nameof(stat));

            if (!_stages.ContainsKey(stat))
            {
                actualChange = 0;
                return this;
            }

            var oldStage = _stages[stat];
            var newStage = Math.Max(MinStatStage, Math.Min(MaxStatStage, oldStage + change));
            actualChange = newStage - oldStage;

            var newStages = new Dictionary<Stat, int>(_stages);
            newStages[stat] = newStage;

            return new StatStages(newStages);
        }

        /// <summary>
        /// Creates a new StatStages instance with all stats reset to 0.
        /// </summary>
        /// <returns>A new StatStages instance with all stats at stage 0.</returns>
        public StatStages Reset()
        {
            return new StatStages();
        }

        /// <summary>
        /// Creates a copy of this StatStages instance.
        /// </summary>
        /// <returns>A new StatStages instance with the same stages.</returns>
        public StatStages Copy()
        {
            return new StatStages(new Dictionary<Stat, int>(_stages));
        }
    }
}
