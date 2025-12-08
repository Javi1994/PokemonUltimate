using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que ejecuta acciones de status explícitamente.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class StatusActionsStep : ITurnStep
    {
        public string StepName => "Status Actions";
        public bool ExecuteEvenIfFainted => false;

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            // Procesar acciones de status (StatChangeAction, StatusApplicationAction, etc.)
            // Excluir UseMoveAction, SwitchAction, DamageAction que ya tienen sus propios steps
            var statusActions = context.SortedActions
                .Where(a => !(a is UseMoveAction) &&
                            !(a is SwitchAction) &&
                            !(a is DamageAction));

            foreach (var action in statusActions)
            {
                context.Logger?.LogDebug(
                    $"Queueing status action: {action.GetType().Name}");

                // Agregar acción a la cola para procesamiento consistente
                // Esto asegura que todas las acciones pasen por observers y estadísticas
                context.GeneratedActions.Add(action);
            }

            return await Task.FromResult(true);
        }
    }
}
