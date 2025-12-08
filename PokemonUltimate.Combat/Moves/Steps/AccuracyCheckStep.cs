using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Moves.Definition;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Checks accuracy using Move Accuracy Handler (skip if target is fainted).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class AccuracyCheckStep : IMoveExecutionStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly AccuracyChecker _accuracyChecker;

        /// <summary>
        /// Creates a new accuracy check step.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. Cannot be null.</param>
        /// <param name="accuracyChecker">The accuracy checker. If null, creates a temporary one.</param>
        public AccuracyCheckStep(CombatEffectHandlerRegistry handlerRegistry, AccuracyChecker accuracyChecker = null)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
            _accuracyChecker = accuracyChecker ?? new AccuracyChecker(new Infrastructure.Providers.RandomProvider());
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
            // Check accuracy using Move Accuracy Handler (skip if target is fainted - move still executes but deals no damage)
            if (ActionValidators.ValidateActiveTarget(context.Target))
            {
                var moveAccuracyHandler = _handlerRegistry.GetMoveAccuracyHandler(_accuracyChecker);
                var accuracyResult = moveAccuracyHandler.CheckAccuracy(context.User, context.Target, context.Move, context.Field);

                if (!accuracyResult.Hit)
                {
                    context.Actions.Add(new MessageAction(accuracyResult.MissMessage));
                    moveAccuracyHandler.CleanupOnFailure(context.User, context.HasFocusPunchEffect, context.HasMultiTurnEffect);
                    context.ShouldStop = true;
                    return MoveExecutionStepResult.Stop;
                }
            }

            // Remove focusing status if Focus Punch succeeds
            if (context.HasFocusPunchEffect)
            {
                var focusPunchHandler = _handlerRegistry.GetFocusPunchHandler();
                focusPunchHandler.CleanupOnSuccess(context.User);
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
