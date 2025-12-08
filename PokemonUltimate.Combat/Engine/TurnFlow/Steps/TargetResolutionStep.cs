using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que resuelve targets y aplica redirecciones (Follow Me, Lightning Rod, etc.).
    /// Centraliza la lógica de resolución de targets para hacerla más visible y simple.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class TargetResolutionStep : ITurnStep
    {
        public string StepName => "Target Resolution";
        public bool ExecuteEvenIfFainted => false;

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.CollectedActions == null || context.CollectedActions.Count == 0)
                return await Task.FromResult(true);

            if (context.TargetResolver == null)
                throw new InvalidOperationException("TargetResolver must be set in TurnContext before executing TargetResolutionStep.");

            var resolvedActions = new List<BattleAction>();

            foreach (var action in context.CollectedActions)
            {
                if (action is UseMoveAction moveAction)
                {
                    // Resolve target and apply redirections
                    var resolvedAction = ResolveMoveTarget(moveAction, context.Field, context.TargetResolver, context.Logger);
                    resolvedActions.Add(resolvedAction);
                }
                else
                {
                    // Non-move actions don't need target resolution
                    resolvedActions.Add(action);
                }
            }

            // Replace collected actions with resolved ones
            context.CollectedActions = resolvedActions;

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Resolves the target for a move action, applying redirections if necessary.
        /// </summary>
        private UseMoveAction ResolveMoveTarget(UseMoveAction originalAction, Field.BattleField field, TargetResolver targetResolver, IBattleLogger logger)
        {
            if (originalAction == null || field == null)
                return originalAction;

            // Get valid targets for the move (this already applies redirections internally)
            var resolvedTargets = targetResolver.GetValidTargets(
                originalAction.User,
                originalAction.Move,
                field);

            // If no valid targets, return original action (will be handled by validation step)
            if (resolvedTargets.Count == 0)
                return originalAction;

            var currentTarget = originalAction.Target;

            // For single-target moves, redirections result in exactly one target
            // If that target differs from the current one, apply the redirection
            if (resolvedTargets.Count == 1)
            {
                var resolvedTarget = resolvedTargets[0];
                if (resolvedTarget != currentTarget)
                {
                    // Target was redirected (Follow Me, Lightning Rod, etc.)
                    logger?.LogDebug(
                        $"Target redirected: {originalAction.Move.Name} " +
                        $"({currentTarget.Pokemon?.DisplayName} → {resolvedTarget.Pokemon?.DisplayName})");

                    return new UseMoveAction(
                        originalAction.User,
                        resolvedTarget,
                        originalAction.MoveInstance);
                }
            }
            // For multi-target moves, check if current target is still valid
            else if (!resolvedTargets.Contains(currentTarget))
            {
                // Current target is not valid anymore (e.g., fainted), use first valid target
                logger?.LogDebug(
                    $"Target invalid, using first valid target: {originalAction.Move.Name} " +
                    $"({currentTarget.Pokemon?.DisplayName} → {resolvedTargets[0].Pokemon?.DisplayName})");

                return new UseMoveAction(
                    originalAction.User,
                    resolvedTargets[0],
                    originalAction.MoveInstance);
            }

            // No changes needed - target is valid and no redirection occurred
            return originalAction;
        }
    }
}
