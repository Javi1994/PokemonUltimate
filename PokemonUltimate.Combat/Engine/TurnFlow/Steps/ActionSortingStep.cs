using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que ordena las acciones por prioridad y velocidad.
    /// </summary>
    public class ActionSortingStep : ITurnStep
    {
        private readonly TurnOrderResolver _turnOrderResolver;

        public string StepName => "Action Sorting";
        public bool ExecuteEvenIfFainted => false;

        public ActionSortingStep(TurnOrderResolver turnOrderResolver)
        {
            _turnOrderResolver = turnOrderResolver ?? throw new ArgumentNullException(nameof(turnOrderResolver));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var sortedActions = _turnOrderResolver.SortActions(context.CollectedActions, context.Field);
            context.SortedActions = sortedActions;

            // Publish action collection events for debugging
            PublishActionCollectionEvent(context);

            return await Task.FromResult(true);
        }

        private void PublishActionCollectionEvent(TurnContext context)
        {
            if (context.SortedActions == null)
                return;

            int executionOrder = 1;
            foreach (var action in context.SortedActions)
            {
                if (action.User?.Pokemon == null)
                    continue;

                var speed = context.TurnOrderResolver?.GetEffectiveSpeed(action.User, null) ?? 0;
                var priority = action.Priority;
                executionOrder++;
            }

            if (context.SortedActions.Count > 0)
            {

            }
        }
    }
}
