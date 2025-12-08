using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Cancels conflicting move states using Move State Handler.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class CancelConflictingStatesStep : IMoveExecutionStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new cancel conflicting states step.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. Cannot be null.</param>
        public CancelConflictingStatesStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
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
            var moveStateHandler = _handlerRegistry.GetMoveStateHandler();
            moveStateHandler.CancelConflictingMoveStates(context.User, context.Move);
            return MoveExecutionStepResult.Continue;
        }
    }
}
