using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.Processors;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Processes before-move effects (abilities and items).
    /// This happens BEFORE validation so abilities can block the move.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class BeforeMoveProcessingStep : IMoveExecutionStep
    {
        private readonly BeforeMoveProcessor _beforeMoveProcessor;

        /// <summary>
        /// Creates a new before move processing step.
        /// </summary>
        /// <param name="beforeMoveProcessor">The before move processor. Cannot be null.</param>
        public BeforeMoveProcessingStep(BeforeMoveProcessor beforeMoveProcessor)
        {
            _beforeMoveProcessor = beforeMoveProcessor ?? throw new System.ArgumentNullException(nameof(beforeMoveProcessor));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 30;

        /// <summary>
        /// Processes before-move effects.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            var beforeMoveActions = _beforeMoveProcessor.ProcessBeforeMove(context.User, context.Field);
            context.Actions.AddRange(beforeMoveActions);

            // Check if any ability blocked the move (e.g., Truant)
            // Truant returns a TruantLoafing message - if present, block the move
            bool moveBlocked = beforeMoveActions.Any(action =>
                action is MessageAction msg &&
                msg.Message.Contains("loafing around"));

            if (moveBlocked)
            {
                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop; // Block move execution (PP not consumed)
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
