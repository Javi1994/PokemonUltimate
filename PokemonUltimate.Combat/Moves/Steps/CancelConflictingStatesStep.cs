using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Cancels conflicting move states using Move State Checker.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class CancelConflictingStatesStep : IMoveExecutionStep
    {
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Creates a new cancel conflicting states step.
        /// </summary>
        /// <param name="behaviorRegistry">The behavior checker registry. Cannot be null.</param>
        public CancelConflictingStatesStep(BehaviorCheckerRegistry behaviorRegistry)
        {
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 20;

        /// <summary>
        /// Cancels conflicting move states.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            var moveStateChecker = _behaviorRegistry.GetMoveStateChecker();
            moveStateChecker.CancelConflictingMoveStates(context.User, context.Move);
            return MoveExecutionStepResult.Continue;
        }
    }
}
