using System;
using System.Diagnostics;

namespace PokemonUltimate.Combat.Logging
{
    /// <summary>
    /// Default implementation of IBattleLogger using System.Diagnostics.Debug.
    /// Logs to debug output in development, can be extended to use other logging frameworks.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleLogger : IBattleLogger
    {
        private readonly string _context;

        /// <summary>
        /// Creates a new battle logger.
        /// </summary>
        /// <param name="context">Optional context identifier for the logger (e.g., "CombatEngine").</param>
        public BattleLogger(string context = null)
        {
            _context = context;
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public void LogDebug(string message)
        {
            Log("DEBUG", message);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public void LogError(string message)
        {
            Log("ERROR", message);
        }

        /// <summary>
        /// Logs a battle event.
        /// </summary>
        public void LogBattleEvent(string eventType, string message)
        {
            Log($"BATTLE[{eventType}]", message);
        }

        /// <summary>
        /// Internal logging method that formats and outputs the message.
        /// </summary>
        private void Log(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string contextPrefix = string.IsNullOrEmpty(_context) ? "" : $"[{_context}] ";
            string logMessage = $"[{timestamp}] {level} {contextPrefix}{message}";

            Debug.WriteLine(logMessage);

            // In production, this could be extended to write to a file, database, or logging service
            // For now, Debug.WriteLine is sufficient for development
        }
    }
}
