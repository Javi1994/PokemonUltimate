using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Checks semi-invulnerable status using Semi-Invulnerable Checker.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class SemiInvulnerableCheckStep : IMoveExecutionStep
    {
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Creates a new semi-invulnerable check step.
        /// </summary>
        /// <param name="behaviorRegistry">The behavior checker registry. Cannot be null.</param>
        public SemiInvulnerableCheckStep(BehaviorCheckerRegistry behaviorRegistry)
        {
            _behaviorRegistry = behaviorRegistry ?? throw new System.ArgumentNullException(nameof(behaviorRegistry));
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
            var semiInvulnerableChecker = _behaviorRegistry.GetSemiInvulnerableChecker();
            var semiInvulnerableResult = semiInvulnerableChecker.CheckSemiInvulnerable(context.Target, context.Move);

            if (semiInvulnerableResult.MissedDueToSemiInvulnerable)
            {
                context.Actions.Add(new MessageAction(semiInvulnerableResult.MissMessage));
                var moveAccuracyChecker = _behaviorRegistry.GetMoveAccuracyChecker();
                moveAccuracyChecker.CleanupOnFailure(context.User, context.HasFocusPunchEffect, context.HasMultiTurnEffect);
                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop;
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
