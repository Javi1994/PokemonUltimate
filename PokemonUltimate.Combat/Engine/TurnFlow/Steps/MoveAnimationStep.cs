using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Infrastructure.Events;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que ejecuta las animaciones de los movimientos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveAnimationStep : ITurnStep
    {
        public string StepName => "Move Animation";
        public bool ExecuteEvenIfFainted => false;

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true &&
                             !context.ProtectionChecks?.GetValueOrDefault(ma, false) == true &&
                             context.AccuracyChecks?.GetValueOrDefault(ma, true) == true);

            foreach (var moveAction in moveActions)
            {
                context.Logger?.LogDebug(
                    $"Playing animation for {moveAction.Move.Name} " +
                    $"({moveAction.User.Pokemon?.DisplayName} → {moveAction.Target.Pokemon?.DisplayName})");

                // Note: ActionExecuted event for UseMoveAction is now raised in MoveValidationStep
                // to ensure correct chronological order (before damage actions)

                // Ejecutar animación del movimiento
                await moveAction.ExecuteVisual(context.View);
            }

            return await Task.FromResult(true);
        }
    }
}
