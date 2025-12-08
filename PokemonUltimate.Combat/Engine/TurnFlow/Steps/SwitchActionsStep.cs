using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que ejecuta cambios de Pokemon explícitamente.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class SwitchActionsStep : ITurnStep
    {
        public string StepName => "Switch Actions";
        public bool ExecuteEvenIfFainted => false;

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var switchActions = context.SortedActions.OfType<SwitchAction>();

            foreach (var switchAction in switchActions)
            {
                var sideName = switchAction.Slot.Side.IsPlayer ? "Player" : "Enemy";
                context.Logger?.LogDebug(
                    $"Queueing switch action: {switchAction.NewPokemon?.DisplayName} " +
                    $"into {sideName} side slot");

                // Agregar acción a la cola para procesamiento consistente
                // Esto asegura que todas las acciones pasen por observers y estadísticas
                context.GeneratedActions.Add(switchAction);
            }

            return await Task.FromResult(true);
        }
    }
}
