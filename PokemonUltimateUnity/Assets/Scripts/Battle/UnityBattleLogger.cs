using UnityEngine;
using PokemonUltimate.Combat.Logging;

/// <summary>
/// Unity implementation of IBattleLogger that logs to Unity Console.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.3: IBattleView Implementation
/// **Documentation**: See `docs/features/4-unity-integration/4.3-ibattleview-implementation/README.md`
/// </summary>
public class UnityBattleLogger : IBattleLogger
{
    private readonly string _context;
    private readonly bool _logDebug;
    private readonly bool _logInfo;
    private readonly bool _logWarnings;
    private readonly bool _logErrors;
    private readonly bool _logBattleEvents;

    /// <summary>
    /// Creates a new Unity battle logger.
    /// </summary>
    /// <param name="context">Optional context identifier (e.g., "CombatEngine").</param>
    /// <param name="logDebug">Whether to log debug messages.</param>
    /// <param name="logInfo">Whether to log info messages.</param>
    /// <param name="logWarnings">Whether to log warnings.</param>
    /// <param name="logErrors">Whether to log errors.</param>
    /// <param name="logBattleEvents">Whether to log battle events.</param>
    public UnityBattleLogger(string context = null, bool logDebug = true, bool logInfo = true, bool logWarnings = true, bool logErrors = true, bool logBattleEvents = true)
    {
        _context = context;
        _logDebug = logDebug;
        _logInfo = logInfo;
        _logWarnings = logWarnings;
        _logErrors = logErrors;
        _logBattleEvents = logBattleEvents;
    }

    public void LogDebug(string message)
    {
        if (_logDebug)
        {
            Log("DEBUG", message, LogType.Log);
        }
    }

    public void LogInfo(string message)
    {
        if (_logInfo)
        {
            Log("INFO", message, LogType.Log);
        }
    }

    public void LogWarning(string message)
    {
        if (_logWarnings)
        {
            Log("WARNING", message, LogType.Warning);
        }
    }

    public void LogError(string message)
    {
        if (_logErrors)
        {
            Log("ERROR", message, LogType.Error);
        }
    }

    public void LogBattleEvent(string eventType, string message)
    {
        if (_logBattleEvents)
        {
            Log($"BATTLE[{eventType}]", message, LogType.Log);
        }
    }

    private void Log(string level, string message, LogType logType)
    {
        string contextPrefix = string.IsNullOrEmpty(_context) ? "" : $"[{_context}] ";
        string logMessage = $"{level} {contextPrefix}{message}";

        switch (logType)
        {
            case LogType.Warning:
                Debug.LogWarning(logMessage);
                break;
            case LogType.Error:
                Debug.LogError(logMessage);
                break;
            default:
                Debug.Log(logMessage);
                break;
        }
    }
}

