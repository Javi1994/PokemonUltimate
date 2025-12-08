using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

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
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

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
        /// <param name="behaviorRegistry">The behavior checker registry. If null, creates a default one.</param>
        public SetSideConditionAction(BattleSlot user, BattleSide targetSide, SideCondition condition, int duration, SideConditionData conditionData = null, BehaviorCheckerRegistry behaviorRegistry = null) : base(user)
        {
            ActionValidators.ValidateBattleSide(targetSide, nameof(targetSide));
            TargetSide = targetSide;
            Condition = condition;
            Duration = duration;
            ConditionData = conditionData;
            _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();
        }

        /// <summary>
        /// Sets the side condition on the target side.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            ActionValidators.ValidateField(field);

            // Clear condition if None is specified
            if (Condition == SideCondition.None)
            {
                TargetSide.RemoveAllSideConditions();
                return Enumerable.Empty<BattleAction>();
            }

            // Use Field Condition Checker to validate side condition can be set (eliminates complex validation logic)
            var fieldChecker = _behaviorRegistry.GetFieldConditionChecker();
            if (!fieldChecker.CanSetSideCondition(field, ConditionData))
            {
                // Condition cannot be set (e.g., Aurora Veil requires Hail/Snow)
                return Enumerable.Empty<BattleAction>();
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
            ActionValidators.ValidateView(view);

            // Side condition animation not yet implemented in IBattleView
            // For now, just return completed task
            return Task.CompletedTask;
        }
    }
}

