using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Moves.Definition;

namespace PokemonUltimate.Combat.Moves.Steps
{
    /// <summary>
    /// Validates initial conditions before move execution (user active, target valid).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class InitialValidationStep : IMoveExecutionStep
    {
        /// <summary>
        /// Gets the execution order (first step).
        /// </summary>
        public int ExecutionOrder => 10;

        /// <summary>
        /// Validates initial conditions.
        /// </summary>
        public MoveExecutionStepResult Process(MoveExecutionContext context)
        {
            ActionValidators.ValidateField(context.Field);

            // User must be active to use a move
            // This check prevents fainted Pokemon from executing moves that were queued before they fainted
            if (!ActionValidators.ValidateUserActive(context.User))
            {
                context.ShouldStop = true;
                // Don't generate any actions - the move should not execute at all
                return MoveExecutionStepResult.Stop;
            }

            // Target slot must not be empty (but can be fainted - move still executes, consumes PP, but deals no damage)
            if (!ActionValidators.ValidateTarget(context.Target))
            {
                context.ShouldStop = true;
                return MoveExecutionStepResult.Stop;
            }

            return MoveExecutionStepResult.Continue;
        }
    }
}
