using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa las acciones generadas por otros steps y las ejecuta en la cola.
    /// También marca las acciones como procesadas para steps reactivos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class ProcessGeneratedActionsStep : ITurnStep
    {
        public string StepName => "Process Generated Actions";
        public bool ExecuteEvenIfFainted => false;

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.GeneratedActions == null || context.GeneratedActions.Count == 0)
                return await Task.FromResult(true);

            context.Logger?.LogDebug(
                $"Processing {context.GeneratedActions.Count} generated actions");

            // Encolar acciones generadas
            context.QueueService.EnqueueRange(context.GeneratedActions);

            // Procesar la cola (esto ejecutará las acciones)
            await context.QueueService.ProcessQueue(context.Field, context.View);

            // Marcar acciones como procesadas para steps reactivos
            if (context.ProcessedActions == null)
                context.ProcessedActions = new System.Collections.Generic.List<BattleAction>();

            context.ProcessedActions.AddRange(context.GeneratedActions);

            // Limpiar acciones generadas para el siguiente ciclo
            context.GeneratedActions.Clear();

            return true;
        }
    }
}
