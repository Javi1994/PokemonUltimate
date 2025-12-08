using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Events.Definition;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que ordena las acciones por prioridad y velocidad.
    /// </summary>
    public class ActionSortingStep : ITurnStep
    {
        private readonly ActionSortingProcessor _processor;

        public string StepName => "Action Sorting";
        public bool ExecuteEvenIfFainted => false;

        public ActionSortingStep(ActionSortingProcessor processor)
        {
            _processor = processor ?? throw new System.ArgumentNullException(nameof(processor));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var sortedActions = _processor.SortActions(context.CollectedActions, context.Field);
            context.SortedActions = sortedActions;

            // Publish action collection events for debugging
            PublishActionCollectionEvent(context);

            return await Task.FromResult(true); // Continue execution
        }

        private void PublishActionCollectionEvent(TurnContext context)
        {
            if (context.EventBus == null || context.SortedActions == null)
                return;

            int executionOrder = 1;
            foreach (var action in context.SortedActions)
            {
                if (action.User?.Pokemon == null)
                    continue;

                var speed = context.TurnOrderResolver?.GetEffectiveSpeed(action.User, null) ?? 0;
                var priority = action.Priority;

                context.EventBus.PublishEvent(new BattleEvent(
                    BattleEventType.ActionCollected,
                    turnNumber: context.TurnNumber,
                    isPlayerSide: action.User.Side.IsPlayer,
                    pokemon: action.User.Pokemon,
                    data: new BattleEventData
                    {
                        ActionType = action.GetType().Name,
                        Priority = priority,
                        Speed = speed,
                        SlotIndex = action.User.SlotIndex,
                        ActionCount = executionOrder
                    }));

                executionOrder++;
            }

            if (context.SortedActions.Count > 0)
            {
                context.EventBus.PublishEvent(new BattleEvent(
                    BattleEventType.ActionsSorted,
                    turnNumber: context.TurnNumber,
                    isPlayerSide: false,
                    data: new BattleEventData
                    {
                        ActionCount = context.SortedActions.Count
                    }));
            }
        }
    }
}
