using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Logging;

namespace PokemonUltimate.BattleSimulator.Logging
{
    /// <summary>
    /// Custom IBattleLogger implementation that captures logs for UI display.
    /// Provides real-time log capture and event notification for battle simulation UI.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.8: Interactive Battle Simulator
    /// **Documentation**: See `docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`
    /// </remarks>
    public class UIBattleLogger : IBattleLogger
    {
        private readonly List<LogEntry> _logs = new List<LogEntry>();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Event raised when a new log entry is added.
        /// </summary>
        public event EventHandler<LogEntry>? LogAdded;

        /// <summary>
        /// Gets all captured log entries.
        /// </summary>
        public IReadOnlyList<LogEntry> Logs
        {
            get
            {
                lock (_lockObject)
                {
                    return _logs.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Clears all captured logs.
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _logs.Clear();
            }
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public void LogDebug(string message)
        {
            AddLog(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public void LogInfo(string message)
        {
            AddLog(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public void LogWarning(string message)
        {
            AddLog(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public void LogError(string message)
        {
            AddLog(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs a battle event.
        /// </summary>
        public void LogBattleEvent(string eventType, string message)
        {
            AddLog(LogLevel.BattleEvent, message, eventType);
        }

        private void AddLog(LogLevel level, string message, string? eventType = null)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                EventType = eventType
            };

            lock (_lockObject)
            {
                _logs.Add(entry);
            }

            LogAdded?.Invoke(this, entry);
        }

        /// <summary>
        /// Represents a log entry with timestamp, level, and message.
        /// </summary>
        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public LogLevel Level { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? EventType { get; set; }

            public override string ToString()
            {
                var timestamp = Timestamp.ToString("HH:mm:ss.fff");
                var levelStr = Level switch
                {
                    LogLevel.Debug => "DEBUG",
                    LogLevel.Info => "INFO",
                    LogLevel.Warning => "WARNING",
                    LogLevel.Error => "ERROR",
                    LogLevel.BattleEvent => $"BATTLE[{EventType ?? "EVENT"}]",
                    _ => "UNKNOWN"
                };
                return $"[{timestamp}] {levelStr} {Message}";
            }
        }

        /// <summary>
        /// Log level enumeration.
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            BattleEvent
        }
    }
}

