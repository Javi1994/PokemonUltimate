using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que valida movimientos antes de su ejecución (PP, status, condiciones).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveValidationStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly IRandomProvider _randomProvider;

        public string StepName => "Move Validation";
        public bool ExecuteEvenIfFainted => false;

        public MoveValidationStep(CombatEffectHandlerRegistry handlerRegistry, IRandomProvider randomProvider)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.MoveValidations == null)
                context.MoveValidations = new System.Collections.Generic.Dictionary<UseMoveAction, bool>();

            var moveActions = context.SortedActions.OfType<UseMoveAction>();

            foreach (var moveAction in moveActions)
            {
                context.Logger?.LogDebug(
                    $"Validating move {moveAction.Move.Name} for {moveAction.User.Pokemon?.DisplayName}");

                // Validar usando Move Execution Handler
                var moveExecutionHandler = _handlerRegistry.GetMoveExecutionHandler(_randomProvider);
                var validationResult = moveExecutionHandler.ValidateExecution(
                    moveAction.MoveInstance,
                    moveAction.User.Pokemon,
                    moveAction.User);

                bool isValid = validationResult.CanExecute;
                context.MoveValidations[moveAction] = isValid;

                // Raise ActionExecuted event for UseMoveAction BEFORE damage is calculated/applied
                // This ensures the event is raised in the correct chronological order
                BattleEventManager.RaiseActionExecuted(moveAction, context.Field, Enumerable.Empty<BattleAction>());

                if (!isValid)
                {
                    context.Logger?.LogDebug(
                        $"Move {moveAction.Move.Name} failed validation: {validationResult.FailureMessage}");

                    // Generar acción de fallo
                    context.GeneratedActions.Add(
                        new MessageAction(validationResult.FailureMessage, validationResult.UserSlot));
                }
            }

            return await Task.FromResult(true);
        }
    }
}
