using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Validates move execution (PP, Flinch, Status) using Move Execution Checker.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class MoveExecutionValidationStep : IMoveExecutionStep
    {
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Creates a new move execution validation step.
        /// </summary>
        /// <param name="behaviorRegistry">The behavior checker registry. Cannot be null.</param>
        public MoveExecutionValidationStep(BehaviorCheckerRegistry behaviorRegistry)
        {
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 40;

        /// <summary>
        /// Validates move execution.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            var moveExecutionChecker = _behaviorRegistry.GetMoveExecutionChecker();
            var validationResult = moveExecutionChecker.ValidateExecution(context.MoveInstance, context.User.Pokemon, context.User);

            if (!validationResult.CanExecute)
            {
                context.Actions.Add(new MessageAction(validationResult.FailureMessage, validationResult.UserSlot));
                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop;
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
