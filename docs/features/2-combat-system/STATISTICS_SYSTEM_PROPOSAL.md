# Battle Statistics System - Design Proposal

> Elegant event-driven statistics collection system integrated into the combat SDK.

## Problem Statement

Currently, debuggers manually track statistics by:
- Inspecting actions after execution
- Using reflection to access private queue fields
- Manually iterating through reactions
- Duplicating tracking logic across multiple runners

This is error-prone, hard to maintain, and tightly coupled to implementation details.

## Proposed Solution

Create a **Battle Statistics System** integrated into the combat SDK that:
- ✅ Uses event-driven architecture (Observer Pattern)
- ✅ Automatically tracks statistics without modifying battle logic
- ✅ Is injectable and optional (doesn't affect normal battles)
- ✅ Provides structured data models for statistics
- ✅ Can be extended for different types of statistics

## Architecture

### 1. Core Interfaces

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

### 2. Statistics Data Models (Extensible Design)

```csharp
/// <summary>
/// Comprehensive battle statistics collected during battle execution.
/// Designed to be easily extensible - just add new properties as needed.
/// </summary>
public class BattleStatistics
{
    // ===== BATTLE OUTCOMES =====
    public int PlayerWins { get; set; }
    public int EnemyWins { get; set; }
    public int Draws { get; set; }
    
    // ===== MOVE STATISTICS =====
    // Move usage (Pokemon -> Move -> Count)
    public Dictionary<string, Dictionary<string, int>> MoveUsageStats { get; set; } = new();
    
    // ===== STATUS EFFECTS =====
    // Persistent status (Pokemon -> Status -> Count)
    public Dictionary<string, Dictionary<string, int>> StatusEffectStats { get; set; } = new();
    
    // Volatile status (Pokemon -> VolatileStatus -> Count)
    public Dictionary<string, Dictionary<string, int>> VolatileStatusStats { get; set; } = new();
    
    // ===== DAMAGE STATISTICS =====
    // Damage values (Pokemon -> List of damage values)
    public Dictionary<string, List<int>> DamageStats { get; set; } = new();
    
    // Critical hits
    public int CriticalHits { get; set; }
    
    // Misses
    public int Misses { get; set; }
    
    // ===== FIELD EFFECTS =====
    // Weather changes (Weather -> Count)
    public Dictionary<string, int> WeatherChanges { get; set; } = new();
    
    // Terrain changes (Terrain -> Count)
    public Dictionary<string, int> TerrainChanges { get; set; } = new();
    
    // Side conditions (Side -> Condition -> Count)
    public Dictionary<string, Dictionary<string, int>> SideConditionStats { get; set; } = new();
    
    // Hazards (Side -> Hazard -> Count)
    public Dictionary<string, Dictionary<string, int>> HazardStats { get; set; } = new();
    
    // ===== STAT CHANGES =====
    // Stat changes (Pokemon -> Stat -> List of stage changes)
    public Dictionary<string, Dictionary<string, List<int>>> StatChangeStats { get; set; } = new();
    
    // ===== HEALING =====
    // Healing (Pokemon -> List of heal amounts)
    public Dictionary<string, List<int>> HealingStats { get; set; } = new();
    
    // ===== ACTION STATISTICS =====
    // Action type counts (ActionType -> Count)
    public Dictionary<string, int> ActionTypeStats { get; set; } = new();
    
    // Effects generated (Move -> EffectType -> Count)
    public Dictionary<string, Dictionary<string, int>> EffectGenerationStats { get; set; } = new();
    
    // ===== TURN STATISTICS =====
    public int TotalTurns { get; set; }
    public List<int> TurnDurations { get; set; } = new();
    
    // ===== ABILITY & ITEM STATISTICS =====
    // Ability activations (Pokemon -> Ability -> Count)
    public Dictionary<string, Dictionary<string, int>> AbilityActivationStats { get; set; } = new();
    
    // Item activations (Pokemon -> Item -> Count)
    public Dictionary<string, Dictionary<string, int>> ItemActivationStats { get; set; } = new();
}
```

### 3. Implementation (Modular & Extensible)

#### 3.1 Core Collector with Modular Trackers

```csharp
/// <summary>
/// Collects battle statistics by observing action execution.
/// Uses modular tracker pattern for easy extensibility.
/// </summary>
public class BattleStatisticsCollector : IBattleActionObserver, IBattleStatisticsCollector
{
    private readonly BattleStatistics _statistics = new BattleStatistics();
    private readonly List<IStatisticsTracker> _trackers = new();
    
    public BattleStatisticsCollector()
    {
        // Register default trackers
        RegisterDefaultTrackers();
    }
    
    /// <summary>
    /// Registers a custom tracker for additional statistics.
    /// </summary>
    public void RegisterTracker(IStatisticsTracker tracker)
    {
        _trackers.Add(tracker);
    }
    
    public void OnActionExecuted(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions)
    {
        // Process through all registered trackers
        foreach (var tracker in _trackers)
        {
            tracker.TrackAction(action, field, reactions, _statistics);
        }
    }
    
    public BattleStatistics GetStatistics() => _statistics;
    
    public void Reset()
    {
        // Reset all statistics
        _statistics.PlayerWins = 0;
        _statistics.EnemyWins = 0;
        _statistics.Draws = 0;
        _statistics.MoveUsageStats.Clear();
        _statistics.StatusEffectStats.Clear();
        _statistics.VolatileStatusStats.Clear();
        _statistics.DamageStats.Clear();
        _statistics.CriticalHits = 0;
        _statistics.Misses = 0;
        _statistics.WeatherChanges.Clear();
        _statistics.TerrainChanges.Clear();
        _statistics.SideConditionStats.Clear();
        _statistics.HazardStats.Clear();
        _statistics.StatChangeStats.Clear();
        _statistics.HealingStats.Clear();
        _statistics.ActionTypeStats.Clear();
        _statistics.EffectGenerationStats.Clear();
        _statistics.AbilityActivationStats.Clear();
        _statistics.ItemActivationStats.Clear();
        _statistics.TotalTurns = 0;
        _statistics.TurnDurations.Clear();
    }
    
    private void RegisterDefaultTrackers()
    {
        _trackers.Add(new MoveUsageTracker());
        _trackers.Add(new DamageTracker());
        _trackers.Add(new StatusEffectTracker());
        _trackers.Add(new VolatileStatusTracker());
        _trackers.Add(new FieldEffectTracker());
        _trackers.Add(new StatChangeTracker());
        _trackers.Add(new HealingTracker());
        _trackers.Add(new ActionTypeTracker());
        _trackers.Add(new EffectGenerationTracker());
    }
}

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

#### 3.2 Example Trackers (Easy to Add More)

```csharp
/// <summary>
/// Tracks volatile status applications.
/// Example of how easy it is to add new tracking types.
/// </summary>
public class VolatileStatusTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Track volatile status from UseMoveAction
        if (action is UseMoveAction moveAction && moveAction.User?.Pokemon != null)
        {
            var pokemonName = moveAction.User.Pokemon.Species.Name;
            var target = moveAction.Target;
            
            if (target?.Pokemon != null)
            {
                // Check for new volatile statuses applied
                // (Compare before/after volatile status flags)
                // Implementation details...
            }
        }
        
        // Track volatile status from reactions
        foreach (var reaction in reactions)
        {
            // Check if reaction applies volatile status
            // Implementation details...
        }
    }
}

/// <summary>
/// Tracks field effects (weather, terrain, hazards, side conditions).
/// </summary>
public class FieldEffectTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Track weather changes
        if (action is SetWeatherAction weatherAction && weatherAction.Weather != Weather.None)
        {
            var weatherName = weatherAction.Weather.ToString();
            stats.WeatherChanges.TryGetValue(weatherName, out var count);
            stats.WeatherChanges[weatherName] = count + 1;
        }
        
        // Track terrain changes
        if (action is SetTerrainAction terrainAction && terrainAction.Terrain != Terrain.None)
        {
            var terrainName = terrainAction.Terrain.ToString();
            stats.TerrainChanges.TryGetValue(terrainName, out var count);
            stats.TerrainChanges[terrainName] = count + 1;
        }
        
        // Track side conditions
        if (action is SetSideConditionAction sideConditionAction)
        {
            var side = sideConditionAction.Target.IsPlayer ? "Player" : "Enemy";
            var conditionName = sideConditionAction.Condition.ToString();
            
            if (!stats.SideConditionStats.ContainsKey(side))
                stats.SideConditionStats[side] = new Dictionary<string, int>();
            
            stats.SideConditionStats[side].TryGetValue(conditionName, out var count);
            stats.SideConditionStats[side][conditionName] = count + 1;
        }
        
        // Track hazards (similar pattern)
        // Implementation...
    }
}

/// <summary>
/// Tracks move effects generated.
/// </summary>
public class EffectGenerationTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        if (action is UseMoveAction moveAction && moveAction.Move?.Effects != null)
        {
            var moveName = moveAction.Move.Name;
            
            if (!stats.EffectGenerationStats.ContainsKey(moveName))
                stats.EffectGenerationStats[moveName] = new Dictionary<string, int>();
            
            foreach (var effect in moveAction.Move.Effects)
            {
                var effectType = effect.GetType().Name.Replace("Effect", "");
                stats.EffectGenerationStats[moveName].TryGetValue(effectType, out var count);
                stats.EffectGenerationStats[moveName][effectType] = count + 1;
            }
        }
    }
}

/// <summary>
/// Tracks stat changes.
/// </summary>
public class StatChangeTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        if (action is StatChangeAction statAction && statAction.Target?.Pokemon != null)
        {
            var pokemonName = statAction.Target.Pokemon.Species.Name;
            var statName = statAction.Stat.ToString();
            var stageChange = statAction.StageChange;
            
            if (!stats.StatChangeStats.ContainsKey(pokemonName))
                stats.StatChangeStats[pokemonName] = new Dictionary<string, List<int>>();
            
            if (!stats.StatChangeStats[pokemonName].ContainsKey(statName))
                stats.StatChangeStats[pokemonName][statName] = new List<int>();
            
            stats.StatChangeStats[pokemonName][statName].Add(stageChange);
        }
    }
}

/// <summary>
/// Tracks healing actions.
/// </summary>
public class HealingTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        if (action is HealAction healAction && healAction.Target?.Pokemon != null)
        {
            var pokemonName = healAction.Target.Pokemon.Species.Name;
            var healAmount = healAction.Amount;
            
            if (!stats.HealingStats.ContainsKey(pokemonName))
                stats.HealingStats[pokemonName] = new List<int>();
            
            stats.HealingStats[pokemonName].Add(healAmount);
        }
    }
}
```

### 4. Integration Points

#### Option A: Modify BattleQueue (Recommended)

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

#### Option B: Wrapper Pattern (Less invasive)

```csharp
/// <summary>
/// Wraps BattleQueue and adds observer notifications.
/// </summary>
public class ObservableBattleQueue : BattleQueue
{
    private readonly List<IBattleActionObserver> _observers = new();
    private readonly BattleQueue _innerQueue;
    
    public void AddObserver(IBattleActionObserver observer)
    {
        _observers.Add(observer);
    }
    
    // Override ProcessQueue to add notifications
}
```

## Benefits

1. **Separation of Concerns**: Statistics collection is separate from battle logic
2. **Reusability**: Same collector can be used in debuggers, analytics, replays
3. **Extensibility**: 
   - ✅ **Easy to add new statistics types** - Just create a new `IStatisticsTracker`
   - ✅ **Modular design** - Each tracker handles one concern
   - ✅ **No core code changes** - Add trackers via `RegisterTracker()`
   - ✅ **Examples included** - VolatileStatus, FieldEffects, StatChanges, Healing, Effects
4. **Testability**: Can test statistics collection independently
5. **Performance**: Optional - only active when observers are registered
6. **Clean Code**: No reflection, no manual tracking, no duplication
7. **Type Safety**: Strongly typed statistics models
8. **Comprehensive**: Tracks all action types automatically

## Extensibility Examples

### Adding Volatile Status Tracking
```csharp
// Already included! Just use:
var stats = collector.GetStatistics();
var volatileStats = stats.VolatileStatusStats; // Pokemon -> VolatileStatus -> Count
```

### Adding Weather Tracking
```csharp
// Already included! Just use:
var stats = collector.GetStatistics();
var weatherChanges = stats.WeatherChanges; // Weather -> Count
```

### Adding Custom Tracking
```csharp
// Create your tracker
public class MyCustomTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Your custom tracking logic
        // Access stats object to store your data
    }
}

// Register it
collector.RegisterTracker(new MyCustomTracker());
```

### Adding New Statistics Properties
```csharp
// Just add to BattleStatistics class:
public class BattleStatistics
{
    // ... existing properties ...
    
    // NEW: Track switch-ins
    public Dictionary<string, int> SwitchInStats { get; set; } = new();
    
    // NEW: Track protect uses
    public Dictionary<string, int> ProtectStats { get; set; } = new();
    
    // NEW: Track type effectiveness
    public Dictionary<string, Dictionary<string, float>> TypeEffectivenessStats { get; set; } = new();
}
```

## Migration Path

1. **Phase 1**: Create interfaces and base implementation
2. **Phase 2**: Integrate into BattleQueue (optional observers)
3. **Phase 3**: Update debuggers to use new system
4. **Phase 4**: Remove old manual tracking code

## Usage Examples

### Basic Usage

```csharp
// In debugger
var statisticsCollector = new BattleStatisticsCollector();
var queue = new BattleQueue();
queue.AddObserver(statisticsCollector);

// Run battle normally
await engine.RunBattle();

// Get statistics
var stats = statisticsCollector.GetStatistics();
// Use stats.MoveUsageStats, stats.StatusEffectStats, stats.VolatileStatusStats, etc.
```

### Adding Custom Tracking

```csharp
// Create collector
var statisticsCollector = new BattleStatisticsCollector();

// Add custom tracker for specific needs
statisticsCollector.RegisterTracker(new CustomAbilityTracker());
statisticsCollector.RegisterTracker(new CustomItemTracker());

// Use as observer
queue.AddObserver(statisticsCollector);
```

### Creating New Trackers (Extensibility Example)

```csharp
/// <summary>
/// Custom tracker for ability activations.
/// Shows how easy it is to add new tracking types.
/// </summary>
public class CustomAbilityTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Track when abilities trigger (via BattleTrigger events)
        // Implementation...
    }
}

/// <summary>
/// Custom tracker for item activations.
/// </summary>
public class CustomItemTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Track when items trigger
        // Implementation...
    }
}
```

## Statistics Types Covered

### ✅ Already Included (Default Trackers)

| Category | Statistics | Data Structure | Tracker |
|----------|------------|----------------|---------|
| **Moves** | Move usage | `Dictionary<Pokemon, Dictionary<Move, Count>>` | MoveUsageTracker |
| **Damage** | Damage values, critical hits, misses | `Dictionary<Pokemon, List<Damage>>` | DamageTracker |
| **Status** | Persistent status (Burn, Poison, etc.) | `Dictionary<Pokemon, Dictionary<Status, Count>>` | StatusEffectTracker |
| **Volatiles** | Volatile status (Confusion, Flinch, etc.) | `Dictionary<Pokemon, Dictionary<VolatileStatus, Count>>` | VolatileStatusTracker |
| **Weather** | Weather changes | `Dictionary<Weather, Count>` | FieldEffectTracker |
| **Terrain** | Terrain changes | `Dictionary<Terrain, Count>` | FieldEffectTracker |
| **Side Conditions** | Screens, Tailwind, etc. | `Dictionary<Side, Dictionary<Condition, Count>>` | FieldEffectTracker |
| **Hazards** | Stealth Rock, Spikes, etc. | `Dictionary<Side, Dictionary<Hazard, Count>>` | FieldEffectTracker |
| **Stat Changes** | Stat stage modifications | `Dictionary<Pokemon, Dictionary<Stat, List<Changes>>>` | StatChangeTracker |
| **Healing** | HP recovery amounts | `Dictionary<Pokemon, List<HealAmount>>` | HealingTracker |
| **Effects** | Move effects generated | `Dictionary<Move, Dictionary<EffectType, Count>>` | EffectGenerationTracker |
| **Actions** | Action type counts | `Dictionary<ActionType, Count>` | ActionTypeTracker |

### 🔧 Easy to Add (Examples)

| Category | How to Add | Example |
|----------|------------|---------|
| **Abilities** | Create `AbilityActivationTracker` | Track when Intimidate, Speed Boost, etc. activate |
| **Items** | Create `ItemActivationTracker` | Track when Leftovers, Life Orb, etc. activate |
| **Switches** | Add to existing tracker | Track switch-in/switch-out counts |
| **Protect** | Add to existing tracker | Track Protect usage and success rates |
| **Type Effectiveness** | Create `TypeEffectivenessTracker` | Track super effective/not very effective hits |
| **Recoil** | Add to DamageTracker | Track recoil damage taken |
| **Drain** | Add to HealingTracker | Track drain healing amounts |
| **Multi-hit** | Add to MoveUsageTracker | Track multi-hit move hit counts |

### 📝 Adding New Statistics - Step by Step

**Step 1**: Add property to `BattleStatistics`
```csharp
public class BattleStatistics
{
    // ... existing properties ...
    
    // NEW: Your custom statistics
    public Dictionary<string, int> MyCustomStats { get; set; } = new();
}
```

**Step 2**: Create tracker (or extend existing)
```csharp
public class MyCustomTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Your tracking logic here
        if (action is MyActionType myAction)
        {
            // Track it
            stats.MyCustomStats[myAction.Key] = stats.MyCustomStats.GetValueOrDefault(myAction.Key, 0) + 1;
        }
    }
}
```

**Step 3**: Register tracker (optional - can be added to default)
```csharp
var collector = new BattleStatisticsCollector();
collector.RegisterTracker(new MyCustomTracker());
```

**That's it!** No need to modify core battle logic.

## Future Enhancements

- **Real-time Statistics**: Update UI as battle progresses
- **Export Formats**: JSON, CSV, XML
- **Statistics Aggregation**: Combine statistics from multiple battles
- **Custom Metrics**: Allow users to define custom statistics via configuration
- **Performance Metrics**: Track execution time, memory usage
- **Replay System**: Use statistics for battle replay generation
- **Analytics Dashboard**: Visualize statistics in real-time

