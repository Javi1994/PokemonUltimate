using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que ejecuta el inicio del turno.
    /// </summary>
    public class TurnStartStep : ITurnStep
    {
        private readonly TurnStartProcessor _processor;

        public string StepName => "Turn Start";
        public bool ExecuteEvenIfFainted => false;

        public TurnStartStep(TurnStartProcessor processor)
        {
            _processor = processor ?? throw new System.ArgumentNullException(nameof(processor));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            await _processor.ProcessAsync(context.Field, context.TurnNumber, context.Queue);
            return true; // Continue execution
        }
    }
}
