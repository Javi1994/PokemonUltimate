using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Processes special behaviors (Multi-Turn moves, Focus Punch).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class SpecialBehaviorProcessingStep : IMoveExecutionStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new special behavior processing step.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. Cannot be null.</param>
        public SpecialBehaviorProcessingStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 50;

        /// <summary>
        /// Processes special behaviors.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            var multiTurnHandler = _handlerRegistry.GetMultiTurnHandler();
            var focusPunchHandler = _handlerRegistry.GetFocusPunchHandler();

            // Process Multi-Turn moves
            bool hasMultiTurnEffect = multiTurnHandler.HasMultiTurnBehavior(context.Move);
            context.HasMultiTurnEffect = hasMultiTurnEffect;

            if (hasMultiTurnEffect)
            {
                if (multiTurnHandler.ProcessMultiTurn(context.User, context.Move, context.MoveInstance, context.Actions))
                {
                    context.ShouldStop = true;
                    return MoveExecutionStepResult.Stop; // Charging turn, cancel execution
                }
            }

            // Process Focus Punch moves
            bool hasFocusPunchEffect = focusPunchHandler.HasFocusPunchBehavior(context.Move);
            context.HasFocusPunchEffect = hasFocusPunchEffect;

            if (hasFocusPunchEffect)
            {
                if (focusPunchHandler.ProcessFocusPunchStart(context.User, context.Actions, context.MoveInstance))
                {
                    context.ShouldStop = true;
                    return MoveExecutionStepResult.Stop; // Focus lost, cancel move
                }
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
