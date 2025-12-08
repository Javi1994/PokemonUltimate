using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Validates move execution (PP, Flinch, Status) using Move Execution Handler.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class MoveExecutionValidationStep : IMoveExecutionStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new move execution validation step.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. Cannot be null.</param>
        /// <param name="randomProvider">The random provider. If null, creates a temporary one.</param>
        public MoveExecutionValidationStep(CombatEffectHandlerRegistry handlerRegistry, IRandomProvider randomProvider = null)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
            _randomProvider = randomProvider ?? new Infrastructure.Providers.RandomProvider();
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
            var moveExecutionHandler = _handlerRegistry.GetMoveExecutionHandler(_randomProvider);
            var validationResult = moveExecutionHandler.ValidateExecution(context.MoveInstance, context.User.Pokemon, context.User);

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
