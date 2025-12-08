using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que procesa triggers de fin de turno (habilidades e items).
    /// </summary>
    public class TurnEndStep : ITurnStep
    {
        private readonly TurnEndProcessor _processor;

        public string StepName => "Turn End";
        public bool ExecuteEvenIfFainted => false;

        public TurnEndStep(TurnEndProcessor processor)
        {
            _processor = processor ?? throw new System.ArgumentNullException(nameof(processor));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var actions = await _processor.ProcessAsync(context.Field);
            if (actions.Count > 0)
            {
                context.Queue.EnqueueRange(actions);
                await context.Queue.ProcessQueue(context.Field, context.View);
            }

            return true; // Continue execution
        }
    }
}
