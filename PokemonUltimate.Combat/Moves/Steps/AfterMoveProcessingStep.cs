using PokemonUltimate.Combat.Engine.Processors;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Processes after-move effects (abilities and items, e.g., Moxie, Life Orb recoil).
    /// This happens AFTER all effects are processed.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class AfterMoveProcessingStep : IMoveExecutionStep
    {
        private readonly AfterMoveProcessor _afterMoveProcessor;

        /// <summary>
        /// Creates a new after move processing step.
        /// </summary>
        /// <param name="afterMoveProcessor">The after move processor. Cannot be null.</param>
        public AfterMoveProcessingStep(AfterMoveProcessor afterMoveProcessor)
        {
            _afterMoveProcessor = afterMoveProcessor ?? throw new System.ArgumentNullException(nameof(afterMoveProcessor));
        }

        /// <summary>
        /// Gets the execution order (last step).
        /// </summary>
        public int ExecutionOrder => 110;

        /// <summary>
        /// Processes after-move effects.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            // Process after-move effects (abilities and items, e.g., Moxie, Life Orb recoil)
            // This happens AFTER all effects are processed
            // Note: DamageActions are in the actions list but haven't been executed yet.
            // Life Orb and Moxie will check damage trackers which are updated when DamageActions execute.
            var afterMoveActions = _afterMoveProcessor.ProcessAfterMove(context.User, context.Field);
            context.Actions.AddRange(afterMoveActions);

            return MoveExecutionStepResult.Continue;
        }
    }
}
