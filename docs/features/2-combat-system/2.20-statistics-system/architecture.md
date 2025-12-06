# Sub-Feature 2.20: Statistics System - Architecture

> Complete technical specification for the battle statistics collection system.

**Sub-Feature Number**: 2.20  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Statistics System provides an event-driven, extensible architecture for collecting battle statistics without modifying core battle logic. It uses the Observer Pattern to automatically track all battle events.

## Design Principles

1. **Separation of Concerns**: Statistics collection is separate from battle logic
2. **Extensibility**: Easy to add new statistics types via modular trackers
3. **Performance**: Optional - only active when observers are registered
4. **Type Safety**: Strongly typed statistics models
5. **No Reflection**: Direct access to actions and events

## Architecture

### Core Interfaces

#### IBattleActionObserver

```csharp
/// <summary>
/// Observer for battle actions - receives notifications when actions execute.
/// Used for statistics collection, logging, debugging, etc.
/// </summary>
public interface IBattleActionObserver
{
    /// <summary>
    /// Called when an action's logic is executed.
    /// </summary>
    void OnActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions);
    
    /// <summary>
    /// Called when a battle turn starts.
    /// </summary>
    void OnTurnStart(int turnNumber, BattleField field);
    
    /// <summary>
    /// Called when a battle turn ends.
    /// </summary>
    void OnTurnEnd(int turnNumber, BattleField field);
    
    /// <summary>
    /// Called when a battle starts.
    /// </summary>
    void OnBattleStart(BattleField field);
    
    /// <summary>
    /// Called when a battle ends.
    /// </summary>
    void OnBattleEnd(BattleOutcome outcome, BattleField field);
}
```

#### IStatisticsTracker

```csharp
/// <summary>
/// Interface for modular statistics trackers.
/// Makes it easy to add new tracking types without modifying core collector.
/// </summary>
public interface IStatisticsTracker
{
    /// <summary>
    /// Tracks statistics for the given action and reactions.
    /// </summary>
    void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats);
}
```

#### IBattleStatisticsCollector

```csharp
/// <summary>
/// Interface for collecting battle statistics.
/// </summary>
public interface IBattleStatisticsCollector
{
    /// <summary>
    /// Gets collected statistics.
    /// </summary>
    BattleStatistics GetStatistics();
    
    /// <summary>
    /// Resets all statistics.
    /// </summary>
    void Reset();
}
```

### Data Models

#### BattleStatistics

Comprehensive statistics data model with all tracked metrics:

```csharp
public class BattleStatistics
{
    // Battle outcomes
    public int PlayerWins { get; set; }
    public int EnemyWins { get; set; }
    public int Draws { get; set; }
    
    // Move statistics
    public Dictionary<string, Dictionary<string, int>> MoveUsageStats { get; set; } = new();
    
    // Status effects
    public Dictionary<string, Dictionary<string, int>> StatusEffectStats { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> VolatileStatusStats { get; set; } = new();
    
    // Damage statistics
    public Dictionary<string, List<int>> DamageStats { get; set; } = new();
    public int CriticalHits { get; set; }
    public int Misses { get; set; }
    
    // Field effects
    public Dictionary<string, int> WeatherChanges { get; set; } = new();
    public Dictionary<string, int> TerrainChanges { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> SideConditionStats { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> HazardStats { get; set; } = new();
    
    // Stat changes
    public Dictionary<string, Dictionary<string, List<int>>> StatChangeStats { get; set; } = new();
    
    // Healing
    public Dictionary<string, List<int>> HealingStats { get; set; } = new();
    
    // Action statistics
    public Dictionary<string, int> ActionTypeStats { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> EffectGenerationStats { get; set; } = new();
    
    // Turn statistics
    public int TotalTurns { get; set; }
    public List<int> TurnDurations { get; set; } = new();
    
    // Ability & Item statistics
    public Dictionary<string, Dictionary<string, int>> AbilityActivationStats { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> ItemActivationStats { get; set; } = new();
}
```

### Implementation

#### BattleStatisticsCollector

Main collector that aggregates statistics from multiple trackers:

```csharp
public class BattleStatisticsCollector : IBattleActionObserver, IBattleStatisticsCollector
{
    private readonly BattleStatistics _statistics = new BattleStatistics();
    private readonly List<IStatisticsTracker> _trackers = new();
    
    public BattleStatisticsCollector()
    {
        RegisterDefaultTrackers();
    }
    
    public void RegisterTracker(IStatisticsTracker tracker)
    {
        _trackers.Add(tracker);
    }
    
    public void OnActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions)
    {
        foreach (var tracker in _trackers)
        {
            tracker.TrackAction(action, field, reactions, _statistics);
        }
    }
    
    // ... other observer methods ...
    
    public BattleStatistics GetStatistics() => _statistics;
    public void Reset() { /* Reset all statistics */ }
}
```

### Default Trackers

Pre-built trackers for common statistics:

1. **MoveUsageTracker**: Tracks move usage per Pokemon
2. **DamageTracker**: Tracks damage values, critical hits, misses
3. **StatusEffectTracker**: Tracks persistent status applications
4. **VolatileStatusTracker**: Tracks volatile status applications
5. **FieldEffectTracker**: Tracks weather, terrain, hazards, side conditions
6. **StatChangeTracker**: Tracks stat stage modifications
7. **HealingTracker**: Tracks HP recovery amounts
8. **EffectGenerationTracker**: Tracks move effects generated
9. **ActionTypeTracker**: Tracks action type counts

### Integration Points

#### BattleQueue Integration

`BattleQueue` is extended to support observers:

```csharp
public class BattleQueue
{
    private readonly List<IBattleActionObserver> _observers = new();
    
    public void AddObserver(IBattleActionObserver observer)
    {
        _observers.Add(observer);
    }
    
    public async Task ProcessQueue(BattleField field, IBattleView view)
    {
        while (_queue.Count > 0)
        {
            var action = _queue.First.Value;
            _queue.RemoveFirst();
            
            var reactions = action.ExecuteLogic(field);
            
            // Notify observers
            foreach (var observer in _observers)
            {
                observer.OnActionExecuted(action, field, reactions);
            }
            
            await action.ExecuteVisual(view);
            
            if (reactions != null)
            {
                InsertAtFront(reactions);
            }
        }
    }
}
```

## Usage Example

```csharp
// Create collector
var statisticsCollector = new BattleStatisticsCollector();

// Add custom tracker (optional)
statisticsCollector.RegisterTracker(new CustomAbilityTracker());

// Integrate with battle queue
var queue = new BattleQueue();
queue.AddObserver(statisticsCollector);

// Run battle normally
await engine.RunBattle();

// Get statistics
var stats = statisticsCollector.GetStatistics();
var moveUsage = stats.MoveUsageStats;
var damageStats = stats.DamageStats;
```

## Extensibility

### Adding New Statistics

1. **Add property to BattleStatistics**:
```csharp
public Dictionary<string, int> MyCustomStats { get; set; } = new();
```

2. **Create tracker**:
```csharp
public class MyCustomTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Your tracking logic
    }
}
```

3. **Register tracker**:
```csharp
collector.RegisterTracker(new MyCustomTracker());
```

## Code Location

- **Interfaces**: `PokemonUltimate.Combat.Statistics/`
- **Implementations**: `PokemonUltimate.Combat.Statistics/`
- **Data Models**: `PokemonUltimate.Combat.Statistics/`
- **Trackers**: `PokemonUltimate.Combat.Statistics/Trackers/`

## Related Documentation

- **[Proposal](../../STATISTICS_SYSTEM_PROPOSAL.md)**: Complete design proposal with examples
- **[Feature 2.2: Action Queue](../2.2-action-queue-system/)**: Queue system that triggers observers
- **[Feature 2.5: Combat Actions](../2.5-combat-actions/)**: Actions being tracked

---

**Last Updated**: January 2025

