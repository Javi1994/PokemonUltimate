using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Checks accuracy using Move Accuracy Checker (skip if target is fainted).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class AccuracyCheckStep : IMoveExecutionStep
    {
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Creates a new accuracy check step.
        /// </summary>
        /// <param name="behaviorRegistry">The behavior checker registry. Cannot be null.</param>
        public AccuracyCheckStep(BehaviorCheckerRegistry behaviorRegistry)
        {
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 90;

        /// <summary>
        /// Checks accuracy.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            // Check accuracy using Move Accuracy Checker (skip if target is fainted - move still executes but deals no damage)
            if (ActionValidators.ValidateActiveTarget(context.Target))
            {
                var moveAccuracyChecker = _behaviorRegistry.GetMoveAccuracyChecker();
                var accuracyResult = moveAccuracyChecker.CheckAccuracy(context.User, context.Target, context.Move, context.Field);

                if (!accuracyResult.Hit)
                {
                    context.Actions.Add(new MessageAction(accuracyResult.MissMessage));
                    moveAccuracyChecker.CleanupOnFailure(context.User, context.HasFocusPunchEffect, context.HasMultiTurnEffect);
                    context.ShouldStop = true;
                    return MoveExecutionStepResult.Stop;
                }
            }

            // Remove focusing status if Focus Punch succeeds (using Behavior Checker)
            if (context.HasFocusPunchEffect)
            {
                var focusPunchChecker = _behaviorRegistry.GetFocusPunchChecker();
                focusPunchChecker.CleanupOnSuccess(context.User);
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
