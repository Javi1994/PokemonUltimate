namespace PokemonUltimate.Combat.Moves
{
    /// <summary>
    /// Result of a move execution step.
    /// Indicates whether execution should continue or stop.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public enum MoveExecutionStepResult
    {
        /// <summary>
        /// Continue to the next step.
        /// </summary>
        Continue,

        /// <summary>
        /// Stop execution (move was blocked, failed, or completed early).
        /// </summary>
        Stop
    }
}
