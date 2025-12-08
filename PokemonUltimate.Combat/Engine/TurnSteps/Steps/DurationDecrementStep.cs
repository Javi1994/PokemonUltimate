using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que decrementa duraciones (clima, terreno, condiciones de lado).
    /// </summary>
    public class DurationDecrementStep : ITurnStep
    {
        private readonly DurationDecrementProcessor _processor;

        public string StepName => "Duration Decrement";
        public bool ExecuteEvenIfFainted => false;

        public DurationDecrementStep(DurationDecrementProcessor processor)
        {
            _processor = processor ?? throw new System.ArgumentNullException(nameof(processor));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            _processor.Process(context.Field);
            return true; // Continue execution
        }
    }
}
