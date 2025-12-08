using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step que recolecta acciones de todos los slots activos.
    /// </summary>
    public class ActionCollectionStep : ITurnStep
    {
        private readonly ActionCollectionProcessor _processor;

        public string StepName => "Action Collection";
        public bool ExecuteEvenIfFainted => false;

        public ActionCollectionStep(ActionCollectionProcessor processor)
        {
            _processor = processor ?? throw new System.ArgumentNullException(nameof(processor));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var actions = await _processor.ProcessAsync(context.Field);
            context.CollectedActions = actions;
            return true; // Continue execution
        }
    }
}
