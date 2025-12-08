using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que ejecuta el inicio del turno.
    /// </summary>
    public class TurnStartStep : ITurnStep
    {
        public string StepName => "Turn Start";
        public bool ExecuteEvenIfFainted => false;

        public TurnStartStep()
        {

        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            return await Task.FromResult(true);
        }
    }
}
