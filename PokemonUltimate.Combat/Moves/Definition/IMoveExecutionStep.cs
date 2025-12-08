namespace PokemonUltimate.Combat.Moves.Definition
{
    /// <summary>
    /// A single step in the move execution pipeline.
    /// Each step processes the execution context and may modify state or add actions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public interface IMoveExecutionStep
    {
        /// <summary>
        /// Gets the order in which this step should execute (lower numbers execute first).
        /// </summary>
        int ExecutionOrder { get; }

        /// <summary>
        /// Processes this step of the move execution.
        /// </summary>
        /// <param name="context">The move execution context to process.</param>
        /// <returns>The result of this step's execution, indicating whether to continue or stop.</returns>
        MoveExecutionStepResult Process(MoveExecutionContext context);
    }
}
