using System.Threading.Tasks;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que ejecuta todas las acciones en la cola.
    /// </summary>
    public class ActionExecutionStep : ITurnStep
    {
        public string StepName => "Action Execution";
        public bool ExecuteEvenIfFainted => false;

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            // Enqueue all actions in sorted order
            context.Queue.EnqueueRange(context.SortedActions);

            // Process the queue
            await context.Queue.ProcessQueue(context.Field, context.View);

            return true; // Continue execution
        }
    }
}
