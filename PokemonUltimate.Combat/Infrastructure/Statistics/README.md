# Battle Statistics System

The statistics system collects detailed battle data by subscribing to battle events. It automatically tracks actions, damage, healing, moves, and more throughout the battle.

## Architecture

The statistics system uses an **event-driven architecture**:

-   **BattleStatistics**: Data container for all collected statistics
-   **BattleStatisticsCollector**: Subscribes to events and collects statistics
-   **BattleEventManager**: Provides events that the collector listens to

## Components

### BattleStatistics.cs

Data container that holds all collected battle statistics.

| Property                 | Type                                          | Purpose                                                   |
| ------------------------ | --------------------------------------------- | --------------------------------------------------------- |
| **TotalTurns**           | `int`                                         | Total number of turns in battle                           |
| **TotalActions**         | `int`                                         | Total number of actions executed                          |
| **ActionsByType**        | `Dictionary<string, int>`                     | Actions executed by type (DamageAction, HealAction, etc.) |
| **ActionsByPokemon**     | `Dictionary<string, Dictionary<string, int>>` | Actions by Pokemon (Pokemon name → Action type → count)   |
| **PlayerActionsByType**  | `Dictionary<string, int>`                     | Actions executed by player team                           |
| **EnemyActionsByType**   | `Dictionary<string, int>`                     | Actions executed by enemy team                            |
| **PlayerDamageDealt**    | `int`                                         | Total damage dealt by player                              |
| **EnemyDamageDealt**     | `int`                                         | Total damage dealt by enemy                               |
| **PlayerHealing**        | `int`                                         | Total healing by player                                   |
| **EnemyHealing**         | `int`                                         | Total healing by enemy                                    |
| **PlayerMoveUsage**      | `Dictionary<string, int>`                     | Moves used by player Pokemon                              |
| **EnemyMoveUsage**       | `Dictionary<string, int>`                     | Moves used by enemy Pokemon                               |
| **MoveUsageByPokemon**   | `Dictionary<string, Dictionary<string, int>>` | Detailed move usage (Pokemon → Move → count)              |
| **DamageByMove**         | `Dictionary<string, int>`                     | Total damage dealt by each move                           |
| **DamageValuesByMove**   | `Dictionary<string, List<int>>`               | Individual damage values per move (for min/max/avg)       |
| **ActionsPerTurn**       | `Dictionary<int, int>`                        | Actions executed per turn                                 |
| **PokemonSwitches**      | `Dictionary<string, int>`                     | Pokemon switches (Pokemon name → count)                   |
| **CriticalHits**         | `int`                                         | Total critical hits count                                 |
| **MissedMoves**          | `int`                                         | Total missed moves count                                  |
| **PlayerFainted**        | `List<string>`                                | Pokemon that fainted on player side                       |
| **EnemyFainted**         | `List<string>`                                | Pokemon that fainted on enemy side                        |
| **StatusEffectsApplied** | `Dictionary<string, int>`                     | Status effects applied (status name → count)              |
| **StatChanges**          | `Dictionary<string, Dictionary<string, int>>` | Stat changes (Pokemon → Stat → total change)              |
| **WeatherChanges**       | `List<string>`                                | Weather changes that occurred                             |
| **TerrainChanges**       | `List<string>`                                | Terrain changes that occurred                             |
| **StepExecutionTimes**   | `Dictionary<string, List<TimeSpan>>`          | Step execution times (step name → durations)              |
| **Outcome**              | `BattleOutcome`                               | Final battle outcome                                      |
| **FinalField**           | `BattleField`                                 | Final battlefield state                                   |

### BattleStatisticsCollector.cs

Collects statistics by subscribing to battle events. Automatically tracks statistics when events are raised.

**Key Methods:**

-   `Subscribe()`: Subscribes to battle events
-   `Unsubscribe()`: Unsubscribes from battle events
-   `GetStatistics()`: Gets collected statistics
-   `Reset()`: Resets all statistics
-   `AutoResetOnBattleStart`: Whether to reset on battle start (default: true)

**Subscribed Events:**

-   `BattleStart`: Resets statistics (if auto-reset enabled)
-   `BattleEnd`: Records outcome and final field
-   `TurnStart`: Tracks turn number and initializes turn action count
-   `TurnEnd`: Records turn end
-   `ActionExecuted`: Tracks all action types
-   `StepExecuted`: Tracks step execution times (debug mode)

**Tracked Action Types:**

-   `DamageAction`: Damage dealt, critical hits, damage by move
-   `HealAction`: Healing by team
-   `UseMoveAction`: Move usage by Pokemon and team
-   `FaintAction`: Fainted Pokemon tracking
-   `ApplyStatusAction`: Status effects applied
-   `StatChangeAction`: Stat changes by Pokemon
-   `SetWeatherAction`: Weather changes
-   `SetTerrainAction`: Terrain changes
-   `SwitchAction`: Pokemon switches

### IBattleStatisticsCollector.cs

Interface for statistics collectors.

```csharp
void Subscribe();
void Unsubscribe();
BattleStatistics GetStatistics();
void Reset();
bool AutoResetOnBattleStart { get; set; }
```

## Usage Example

### Basic Usage

```csharp
// Create collector
var collector = new BattleStatisticsCollector();

// Subscribe to events
collector.Subscribe();

// Run battle
var engine = new CombatEngine(...);
engine.Initialize(...);
await engine.RunBattle();

// Get statistics
var stats = collector.GetStatistics();

// Access statistics
Console.WriteLine($"Total turns: {stats.TotalTurns}");
Console.WriteLine($"Total actions: {stats.TotalActions}");
Console.WriteLine($"Player damage: {stats.PlayerDamageDealt}");
Console.WriteLine($"Enemy damage: {stats.EnemyDamageDealt}");

// Unsubscribe when done
collector.Unsubscribe();
```

### Accumulating Statistics Across Multiple Battles

```csharp
// Disable auto-reset to accumulate statistics
collector.AutoResetOnBattleStart = false;

// Run multiple battles
for (int i = 0; i < 100; i++)
{
    var engine = new CombatEngine(...);
    engine.Initialize(...);
    await engine.RunBattle();
}

// Get accumulated statistics
var stats = collector.GetStatistics();
Console.WriteLine($"Total battles: {stats.TotalTurns / averageTurnsPerBattle}");

// Reset manually when done
collector.Reset();
```

### Accessing Detailed Statistics

```csharp
var stats = collector.GetStatistics();

// Actions by type
foreach (var kvp in stats.ActionsByType)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}

// Move usage by Pokemon
foreach (var pokemon in stats.MoveUsageByPokemon)
{
    Console.WriteLine($"{pokemon.Key}:");
    foreach (var move in pokemon.Value)
    {
        Console.WriteLine($"  {move.Key}: {move.Value} times");
    }
}

// Damage statistics by move
foreach (var move in stats.DamageByMove)
{
    var values = stats.DamageValuesByMove[move.Key];
    var avg = values.Average();
    var min = values.Min();
    var max = values.Max();
    Console.WriteLine($"{move.Key}: Total={move.Value}, Avg={avg:F1}, Min={min}, Max={max}");
}
```

## How to Add New Statistics

### Step 1: Add Property to BattleStatistics

Open `Infrastructure/Statistics/BattleStatistics.cs` and add your property:

```csharp
public class BattleStatistics
{
    // ... existing properties ...

    /// <summary>
    /// Your new statistic.
    /// </summary>
    public int YourNewStatistic { get; set; }
}
```

### Step 2: Track in BattleStatisticsCollector

Open `Infrastructure/Statistics/BattleStatisticsCollector.cs` and add tracking logic:

```csharp
private void OnActionExecuted(object sender, ActionExecutedEventArgs e)
{
    // ... existing tracking ...

    // Track your new statistic
    if (e.Action is YourActionType yourAction)
    {
        TrackYourStatistic(yourAction, e.Field);
    }
}

private void TrackYourStatistic(YourActionType action, BattleField field)
{
    // Your tracking logic here
    _statistics.YourNewStatistic++;
}
```

### Step 3: Reset in Reset Method

Add reset logic to the `Reset()` method:

```csharp
public void Reset()
{
    // ... existing resets ...
    _statistics.YourNewStatistic = 0;
}
```

## Design Principles

1. **Event-Driven**: Statistics collected automatically via events
2. **Non-Intrusive**: Doesn't modify battle logic
3. **Efficient**: Only tracks when subscribed, optimized lookups
4. **Flexible**: Can accumulate across battles or reset per battle
5. **Comprehensive**: Tracks all major battle aspects

## Performance Considerations

-   **Optimized Lookups**: Uses `TryGetValue` for dictionary operations
-   **Conditional Creation**: Only creates event args if subscribers exist
-   **Cached Checks**: Caches player/enemy slot checks
-   **Minimal Overhead**: Statistics collection has minimal impact on battle performance

## Related Documentation

-   `../Events/README.md` - Event system documentation
-   `../Events/BattleEventManager.cs` - Event manager implementation
-   `BattleStatistics.cs` - Statistics data container
-   `BattleStatisticsCollector.cs` - Statistics collector implementation
