namespace PokemonUltimate.Combat.Infrastructure.Logging.Definition
{
    /// <summary>
    /// Interface for logging battle events and debug information.
    /// Provides structured logging for combat system debugging and analysis.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IBattleLogger
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogDebug(string message);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogInfo(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogError(string message);

        /// <summary>
        /// Logs a battle event (turn start, move used, damage dealt, etc.).
        /// </summary>
        /// <param name="eventType">The type of battle event.</param>
        /// <param name="message">The event message.</param>
        void LogBattleEvent(string eventType, string message);
    }
}
