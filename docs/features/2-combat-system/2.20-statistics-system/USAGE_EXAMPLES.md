# Statistics System - Usage Examples

> Practical examples of how to use the Statistics System.

**Sub-Feature**: 2.20: Statistics System  
**See**: [`architecture.md`](architecture.md) for technical details.

## Basic Usage

### Example 1: Collect Statistics During Battle

```csharp
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Statistics;

// Create statistics collector
var statisticsCollector = new BattleStatisticsCollector();

// Create battle engine
var engine = CombatEngineFactory.Create();
var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };

// Initialize engine
engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

// Register observer BEFORE running battle
engine.Queue.AddObserver(statisticsCollector);
statisticsCollector.OnBattleStart(engine.Field);

// Run battle normally
var result = await engine.RunBattle();

// Notify observer of battle end
statisticsCollector.OnBattleEnd(result.Outcome, engine.Field);

// Get statistics
var stats = statisticsCollector.GetStatistics();

// Access statistics
Console.WriteLine($"Player Wins: {stats.PlayerWins}");
Console.WriteLine($"Total Turns: {stats.TotalTurns}");
Console.WriteLine($"Critical Hits: {stats.CriticalHits}");

// Access move usage
foreach (var pokemon in stats.MoveUsageStats.Keys)
{
    Console.WriteLine($"{pokemon} used:");
    foreach (var move in stats.MoveUsageStats[pokemon])
    {
        Console.WriteLine($"  {move.Key}: {move.Value} times");
    }
}
```

### Example 2: Multiple Battles with Aggregated Statistics

```csharp
var statisticsCollector = new BattleStatisticsCollector();
var stats = statisticsCollector.GetStatistics();

for (int i = 0; i < 100; i++)
{
    var engine = CombatEngineFactory.Create();
    engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);
    
    engine.Queue.AddObserver(statisticsCollector);
    statisticsCollector.OnBattleStart(engine.Field);
    
    var result = await engine.RunBattle();
    statisticsCollector.OnBattleEnd(result.Outcome, engine.Field);
    
    // Statistics accumulate across battles
}

// Get aggregated statistics
var totalWins = stats.PlayerWins;
var totalCriticalHits = stats.CriticalHits;
```

### Example 3: Reset Statistics Between Battles

```csharp
var statisticsCollector = new BattleStatisticsCollector();

for (int i = 0; i < 100; i++)
{
    // Reset before each battle
    statisticsCollector.Reset();
    statisticsCollector.OnBattleStart(engine.Field);
    
    // Run battle...
    
    // Get statistics for this battle only
    var battleStats = statisticsCollector.GetStatistics();
    Console.WriteLine($"Battle {i}: {battleStats.PlayerWins} wins");
}
```

## Custom Trackers

### Example 4: Adding a Custom Tracker

```csharp
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Combat.Statistics.Trackers;

// Create collector
var statisticsCollector = new BattleStatisticsCollector();

// Create custom tracker
public class CustomAbilityTracker : IStatisticsTracker
{
    public void TrackAction(BattleAction action, BattleField field, 
                           IEnumerable<BattleAction> reactions, BattleStatistics stats)
    {
        // Your custom tracking logic
        if (action is UseMoveAction moveAction && moveAction.User?.Pokemon?.Ability != null)
        {
            var pokemonName = moveAction.User.Pokemon.Species.Name;
            var abilityName = moveAction.User.Pokemon.Ability.Name;
            
            if (!stats.AbilityActivationStats.ContainsKey(pokemonName))
                stats.AbilityActivationStats[pokemonName] = new Dictionary<string, int>();
            
            if (!stats.AbilityActivationStats[pokemonName].ContainsKey(abilityName))
                stats.AbilityActivationStats[pokemonName][abilityName] = 0;
            
            stats.AbilityActivationStats[pokemonName][abilityName]++;
        }
    }
}

// Register custom tracker
statisticsCollector.RegisterTracker(new CustomAbilityTracker());

// Use collector normally
engine.Queue.AddObserver(statisticsCollector);
```

## Statistics Access Patterns

### Example 5: Accessing Different Statistics Types

```csharp
var stats = statisticsCollector.GetStatistics();

// Battle outcomes
int playerWins = stats.PlayerWins;
int enemyWins = stats.EnemyWins;
int draws = stats.Draws;

// Move usage (Pokemon -> Move -> Count)
foreach (var pokemon in stats.MoveUsageStats.Keys)
{
    foreach (var move in stats.MoveUsageStats[pokemon])
    {
        Console.WriteLine($"{pokemon} used {move.Key} {move.Value} times");
    }
}

// Damage statistics (Pokemon -> List of damage values)
foreach (var pokemon in stats.DamageStats.Keys)
{
    var damageList = stats.DamageStats[pokemon];
    var avgDamage = damageList.Count > 0 ? damageList.Average() : 0;
    Console.WriteLine($"{pokemon} average damage: {avgDamage}");
}

// Status effects (Pokemon -> Status -> Count)
foreach (var pokemon in stats.StatusEffectStats.Keys)
{
    foreach (var status in stats.StatusEffectStats[pokemon])
    {
        Console.WriteLine($"{pokemon} had {status.Key} applied {status.Value} times");
    }
}

// Field effects
foreach (var weather in stats.WeatherChanges.Keys)
{
    Console.WriteLine($"Weather {weather} changed {stats.WeatherChanges[weather]} times");
}

// Stat changes (Pokemon -> Stat -> List of changes)
foreach (var pokemon in stats.StatChangeStats.Keys)
{
    foreach (var stat in stats.StatChangeStats[pokemon].Keys)
    {
        var changes = stats.StatChangeStats[pokemon][stat];
        var totalChange = changes.Sum();
        Console.WriteLine($"{pokemon} {stat} changed by {totalChange} total");
    }
}
```

## Integration with Debuggers

### Example 6: Using in Battle Debugger

```csharp
// In BattleRunner (already implemented)
var statisticsCollector = new BattleStatisticsCollector();
var internalStats = statisticsCollector.GetStatistics();
var stats = new BattleStatistics(internalStats); // Wrapper for UI compatibility

// Register observer
engine.Queue.AddObserver(statisticsCollector);
statisticsCollector.OnBattleStart(engine.Field);

// Run battles
for (int i = 0; i < numberOfBattles; i++)
{
    var result = await engine.RunBattle();
    statisticsCollector.OnBattleEnd(result.Outcome, engine.Field);
}

// Statistics are automatically collected
return stats;
```

## Performance Considerations

### Example 7: Conditional Statistics Collection

```csharp
// Only collect statistics when needed
if (shouldCollectStatistics)
{
    var statisticsCollector = new BattleStatisticsCollector();
    engine.Queue.AddObserver(statisticsCollector);
    
    // Run battle...
    
    // Remove observer when done
    engine.Queue.RemoveObserver(statisticsCollector);
}
else
{
    // Run battle without statistics collection
    await engine.RunBattle();
}
```

## Best Practices

1. **Register observer before battle starts**: Always call `AddObserver()` before `RunBattle()`
2. **Notify battle events**: Call `OnBattleStart()` and `OnBattleEnd()` for accurate statistics
3. **Reset between battles**: Use `Reset()` if you want per-battle statistics instead of aggregated
4. **Remove observer when done**: Use `RemoveObserver()` to clean up if needed
5. **Use wrapper classes**: For UI compatibility, create wrapper classes that expose only needed properties

## See Also

- [`architecture.md`](architecture.md) - Technical specification
- [`README.md`](README.md) - Overview and quick start
- [`../../STATISTICS_SYSTEM_PROPOSAL.md`](../../STATISTICS_SYSTEM_PROPOSAL.md) - Design proposal

