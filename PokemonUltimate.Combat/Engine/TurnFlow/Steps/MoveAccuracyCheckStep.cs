using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que verifica la precisión de movimientos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveAccuracyCheckStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;
        private readonly AccuracyChecker _accuracyChecker;

        public string StepName => "Move Accuracy Check";
        public bool ExecuteEvenIfFainted => false;

        public MoveAccuracyCheckStep(CombatEffectHandlerRegistry handlerRegistry, AccuracyChecker accuracyChecker)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
            _accuracyChecker = accuracyChecker ?? throw new ArgumentNullException(nameof(accuracyChecker));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.AccuracyChecks == null)
                context.AccuracyChecks = new System.Collections.Generic.Dictionary<UseMoveAction, bool>();

            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true &&
                             !context.ProtectionChecks?.GetValueOrDefault(ma, false) == true);

            foreach (var moveAction in moveActions)
            {
                // Skip accuracy check if target is fainted (move still executes but deals no damage)
                if (!ActionValidators.ValidateActiveTarget(moveAction.Target))
                {
                    context.AccuracyChecks[moveAction] = true; // Consider it "hit" but no damage
                    continue;
                }

                context.Logger?.LogDebug(
                    $"Checking accuracy for {moveAction.Move.Name} " +
                    $"({moveAction.User.Pokemon?.DisplayName} → {moveAction.Target.Pokemon?.DisplayName})");

                var moveAccuracyHandler = _handlerRegistry.GetMoveAccuracyHandler(_accuracyChecker);
                var accuracyResult = moveAccuracyHandler.CheckAccuracy(
                    moveAction.User,
                    moveAction.Target,
                    moveAction.Move,
                    context.Field);

                bool hit = accuracyResult.Hit;
                context.AccuracyChecks[moveAction] = hit;

                if (!hit)
                {
                    context.Logger?.LogDebug(
                        $"Move {moveAction.Move.Name} missed: {accuracyResult.MissMessage}");

                    context.GeneratedActions.Add(
                        new MessageAction(accuracyResult.MissMessage));
                }
            }

            return await Task.FromResult(true);
        }
    }
}
