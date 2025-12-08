using PokemonUltimate.Combat.Actions.Registry;
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
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Creates a new special behavior processing step.
        /// </summary>
        /// <param name="behaviorRegistry">The behavior checker registry. Cannot be null.</param>
        public SpecialBehaviorProcessingStep(BehaviorCheckerRegistry behaviorRegistry)
        {
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
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
            var multiTurnChecker = _behaviorRegistry.GetMultiTurnChecker();
            var focusPunchChecker = _behaviorRegistry.GetFocusPunchChecker();

            // Process Multi-Turn moves
            bool hasMultiTurnEffect = multiTurnChecker.HasMultiTurnBehavior(context.Move);
            context.HasMultiTurnEffect = hasMultiTurnEffect;

            if (hasMultiTurnEffect)
            {
                if (multiTurnChecker.ProcessMultiTurn(context.User, context.Move, context.MoveInstance, context.Actions))
                {
                    context.ShouldStop = true;
                    return MoveExecutionStepResult.Stop; // Charging turn, cancel execution
                }
            }

            // Process Focus Punch moves
            bool hasFocusPunchEffect = focusPunchChecker.HasFocusPunchBehavior(context.Move);
            context.HasFocusPunchEffect = hasFocusPunchEffect;

            if (hasFocusPunchEffect)
            {
                if (focusPunchChecker.ProcessFocusPunchStart(context.User, context.Actions, context.MoveInstance))
                {
                    context.ShouldStop = true;
                    return MoveExecutionStepResult.Stop; // Focus lost, cancel move
                }
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
