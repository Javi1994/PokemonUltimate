using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Sets or changes a side condition on a battle side.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    public class SetSideConditionAction : BattleAction
    {
        /// <summary>
        /// The side to apply the condition to.
        /// </summary>
        public BattleSide TargetSide { get; }

        /// <summary>
        /// The side condition to set.
        /// </summary>
        public SideCondition Condition { get; }

        /// <summary>
        /// The duration of the condition in turns.
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// The side condition data for this condition.
        /// </summary>
        public SideConditionData ConditionData { get; }

        /// <summary>
        /// Creates a new set side condition action.
        /// </summary>
        /// <param name="user">The slot that initiated this condition change. Can be null for system actions.</param>
        /// <param name="targetSide">The side to apply the condition to. Cannot be null.</param>
        /// <param name="condition">The condition to set. Use SideCondition.None to clear.</param>
        /// <param name="duration">Duration in turns.</param>
        /// <param name="conditionData">The side condition data for this condition. Can be null if not available.</param>
        public SetSideConditionAction(BattleSlot user, BattleSide targetSide, SideCondition condition, int duration, SideConditionData conditionData = null) : base(user)
        {
            TargetSide = targetSide ?? throw new ArgumentNullException(nameof(targetSide));
            Condition = condition;
            Duration = duration;
            ConditionData = conditionData;
        }

        /// <summary>
        /// Sets the side condition on the target side.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Clear condition if None is specified
            if (Condition == SideCondition.None)
            {
                TargetSide.RemoveAllSideConditions();
                return Enumerable.Empty<BattleAction>();
            }

            // Validate condition can be set (e.g., Aurora Veil requires Hail/Snow)
            if (ConditionData != null)
            {
                if (ConditionData.RequiredWeather.HasValue)
                {
                    var currentWeather = field.Weather;
                    if (!ConditionData.CanBeSetInWeather(currentWeather))
                    {
                        // Condition cannot be set in current weather
                        return Enumerable.Empty<BattleAction>();
                    }
                }
            }

            // Set the condition
            if (ConditionData != null)
            {
                TargetSide.AddSideCondition(ConditionData, Duration);
            }

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Plays the side condition change animation.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            // Side condition animation not yet implemented in IBattleView
            // For now, just return completed task
            return Task.CompletedTask;
        }
    }
}

