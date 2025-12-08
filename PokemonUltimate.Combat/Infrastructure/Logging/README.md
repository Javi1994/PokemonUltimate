# Battle Logging System

The logging system provides structured logging for battle events, debug information, and battle analysis. Supports multiple logging implementations including a null logger for production.

## Architecture

The logging system uses an **interface-based design**:

-   **IBattleLogger**: Interface for all loggers
-   **BattleLogger**: Default implementation using Debug.WriteLine
-   **NullBattleLogger**: Null object pattern for no-op logging

## Components

### IBattleLogger.cs

Interface for battle logging implementations.

**Methods:**

| Method                               | Purpose                     |
| ------------------------------------ | --------------------------- |
| `LogDebug(message)`                  | Logs debug messages         |
| `LogInfo(message)`                   | Logs informational messages |
| `LogWarning(message)`                | Logs warning messages       |
| `LogError(message)`                  | Logs error messages         |
| `LogBattleEvent(eventType, message)` | Logs battle-specific events |

### BattleLogger.cs

Default implementation using `System.Diagnostics.Debug`.

**Features:**

-   Timestamp formatting (HH:mm:ss.fff)
-   Context prefix support (e.g., "[CombatEngine]")
-   Log level prefixes (DEBUG, INFO, WARNING, ERROR, BATTLE)
-   Outputs to Debug.WriteLine (can be extended)

**Configuration:**

-   `context`: Optional context identifier for logger

**Usage:**

```csharp
var logger = new BattleLogger("CombatEngine");
logger.LogInfo("Battle started");
logger.LogDebug("Turn 1 executing");
logger.LogBattleEvent("MoveUsed", "Pikachu used Thunderbolt");
```

### NullBattleLogger.cs

Null object implementation that discards all log messages.

**Use Cases:**

-   Production scenarios where logging is not needed
-   Tests where logging output is unwanted
-   Performance-critical scenarios

**Usage:**

```csharp
var logger = new NullBattleLogger();
logger.LogInfo("This message is discarded"); // No-op
```

## Usage Examples

### Basic Logging

```csharp
var logger = new BattleLogger("MyComponent");

logger.LogInfo("Component initialized");
logger.LogDebug("Processing action");
logger.LogWarning("Low HP detected");
logger.LogError("Invalid state detected");
```

### Battle Event Logging

```csharp
logger.LogBattleEvent("TurnStart", $"Turn {turnNumber} started");
logger.LogBattleEvent("MoveUsed", $"{pokemon.Name} used {move.Name}");
logger.LogBattleEvent("DamageDealt", $"Dealt {damage} damage");
logger.LogBattleEvent("PokemonFainted", $"{pokemon.Name} fainted");
```

### Conditional Logging

```csharp
if (isDebugMode)
{
    logger.LogDebug($"Detailed state: {state}");
}
else
{
    logger.LogInfo($"Summary: {summary}");
}
```

### Null Logger for Production

```csharp
// In production, use null logger to avoid overhead
var logger = isDebugMode
    ? new BattleLogger("CombatEngine")
    : new NullBattleLogger();
```

## How to Add a New Logger Implementation

### Step 1: Create Logger Class

Create a new file in `Infrastructure/Logging/` (e.g., `FileBattleLogger.cs`):

```csharp
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using System.IO;

namespace PokemonUltimate.Combat.Infrastructure.Logging
{
    public class FileBattleLogger : IBattleLogger
    {
        private readonly string _filePath;
        private readonly string _context;

        public FileBattleLogger(string filePath, string context = null)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _context = context;
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        public void LogBattleEvent(string eventType, string message)
        {
            WriteLog($"BATTLE[{eventType}]", message);
        }

        private void WriteLog(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var contextPrefix = string.IsNullOrEmpty(_context) ? "" : $"[{_context}] ";
            var logMessage = $"[{timestamp}] {level} {contextPrefix}{message}";

            File.AppendAllText(_filePath, logMessage + Environment.NewLine);
        }
    }
}
```

### Step 2: Use Your Logger

Use your logger in the combat engine:

```csharp
var logger = new FileBattleLogger("battle.log", "CombatEngine");
var engine = new CombatEngine(..., logger);
```

## Design Principles

1. **Interface-Based**: All loggers implement IBattleLogger
2. **Null Object Pattern**: NullBattleLogger for no-op scenarios
3. **Structured Logging**: Separate methods for different log levels
4. **Context Support**: Optional context identifiers for filtering
5. **Extensibility**: Easy to add new logger implementations

## Log Levels

-   **Debug**: Detailed information for debugging
-   **Info**: General informational messages
-   **Warning**: Warning messages for potential issues
-   **Error**: Error messages for failures
-   **BattleEvent**: Battle-specific events (turn start, move used, etc.)

## Related Documentation

-   `../Events/README.md` - Event system (alternative to logging)
-   `IBattleLogger.cs` - Logger interface
-   `BattleLogger.cs` - Default logger implementation
-   `NullBattleLogger.cs` - Null logger implementation
