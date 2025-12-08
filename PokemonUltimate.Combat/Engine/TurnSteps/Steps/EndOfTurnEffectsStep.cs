using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que procesa efectos de fin de turno (daño de status, clima, etc.).
    /// </summary>
    public class EndOfTurnEffectsStep : ITurnStep
    {
        private readonly EndOfTurnEffectsProcessor _processor;

        public string StepName => "End of Turn Effects";
        public bool ExecuteEvenIfFainted => false;

        public EndOfTurnEffectsStep(EndOfTurnEffectsProcessor processor)
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
