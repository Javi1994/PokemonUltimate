using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Checks semi-invulnerable status using Semi-Invulnerable Handler.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class SemiInvulnerableCheckStep : IMoveExecutionStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// Creates a new semi-invulnerable check step.
        /// </summary>
        /// <param name="handlerRegistry">The handler registry. Cannot be null.</param>
        public SemiInvulnerableCheckStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
        }

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        public int ExecutionOrder => 80;

        /// <summary>
        /// Checks semi-invulnerable status.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            var semiInvulnerableHandler = _handlerRegistry.GetSemiInvulnerableHandler();
            var semiInvulnerableResult = semiInvulnerableHandler.CheckSemiInvulnerable(context.Target, context.Move);

            if (semiInvulnerableResult.MissedDueToSemiInvulnerable)
            {
                context.Actions.Add(new MessageAction(semiInvulnerableResult.MissMessage));
                var accuracyChecker = new Utilities.AccuracyChecker(new Infrastructure.Providers.RandomProvider());
                var moveAccuracyHandler = _handlerRegistry.GetMoveAccuracyHandler(accuracyChecker);
                moveAccuracyHandler.CleanupOnFailure(context.User, context.HasFocusPunchEffect, context.HasMultiTurnEffect);
                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop;
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
