using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.Processors;

namespace PokemonUltimate.Combat.Engine.TurnSteps.Steps
{
    /// <summary>
    /// Step reutilizable que verifica y maneja Pokemon debilitados.
    /// </summary>
    public class FaintedPokemonCheckStep : ITurnStep
    {
        private readonly FaintedPokemonSwitchingProcessor _processor;

        public string StepName => "Fainted Pokemon Check";
        public bool ExecuteEvenIfFainted => true; // Always execute

        public FaintedPokemonCheckStep(FaintedPokemonSwitchingProcessor processor)
        {
            _processor = processor ?? throw new System.ArgumentNullException(nameof(processor));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var switchingActions = await _processor.ProcessAsync(context.Field);
            if (switchingActions.Count > 0)
            {
                context.Queue.EnqueueRange(switchingActions);
                await context.Queue.ProcessQueue(context.Field, context.View);
                context.HasFaintedPokemon = true;
            }
            else
            {
                context.HasFaintedPokemon = false;
            }

            return true; // Continue execution
        }
    }
}
