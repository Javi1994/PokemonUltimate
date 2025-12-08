using PokemonUltimate.Combat.Infrastructure.Logging.Definition;

namespace PokemonUltimate.Combat.Infrastructure.Logging
{
    /// <summary>
    /// Null object implementation of IBattleLogger that discards all log messages.
    /// Useful for tests and production scenarios where logging is not needed.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class NullBattleLogger : IBattleLogger
    {
        /// <summary>
        /// Logs a debug message (no-op).
        /// </summary>
        public void LogDebug(string message)
        {
            // No-op: discard message
        }

        /// <summary>
        /// Logs an informational message (no-op).
        /// </summary>
        public void LogInfo(string message)
        {
            // No-op: discard message
        }

        /// <summary>
        /// Logs a warning message (no-op).
        /// </summary>
        public void LogWarning(string message)
        {
            // No-op: discard message
        }

        /// <summary>
        /// Logs an error message (no-op).
        /// </summary>
        public void LogError(string message)
        {
            // No-op: discard message
        }

        /// <summary>
        /// Logs a battle event (no-op).
        /// </summary>
        public void LogBattleEvent(string eventType, string message)
        {
            // No-op: discard message
        }
    }
}
