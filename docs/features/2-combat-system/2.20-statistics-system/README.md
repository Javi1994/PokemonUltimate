# Sub-Feature 2.20: Statistics System

> Event-driven statistics collection system integrated into the combat SDK.

**Sub-Feature Number**: 2.20  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Statistics System provides an elegant, extensible way to collect battle statistics without modifying core battle logic. It uses an observer pattern to automatically track:

- Move usage statistics
- Damage statistics (values, critical hits, misses)
- Status effects (persistent and volatile)
- Field effects (weather, terrain, hazards, side conditions)
- Stat changes
- Healing amounts
- Effect generation
- Action type counts
- And more...

## Current Status

- ✅ **Complete**: Fully implemented and integrated
- ✅ **Design**: Complete (see `STATISTICS_SYSTEM_PROPOSAL.md`)
- ✅ **Tests**: 33 tests passing
- ✅ **Integration**: BattleRunner and MoveRunner refactored to use new system

## Architecture

The system uses a modular tracker pattern:

- **IBattleActionObserver**: Interface for observing battle events
- **IStatisticsTracker**: Interface for modular statistics trackers
- **BattleStatisticsCollector**: Main collector that aggregates statistics
- **BattleStatistics**: Data model containing all statistics
- **Default Trackers**: Pre-built trackers for common statistics

## Key Components

| Component | Purpose |
|-----------|---------|
| `IBattleActionObserver` | Interface for observing action execution |
| `IStatisticsTracker` | Interface for modular statistics trackers |
| `BattleStatisticsCollector` | Main statistics collector |
| `BattleStatistics` | Statistics data model |
| `BattleQueue` | Extended to support observers |

## Integration

The system integrates into `BattleQueue` via observer pattern:

```csharp
var collector = new BattleStatisticsCollector();
var queue = new BattleQueue();
queue.AddObserver(collector);

// Run battle normally
await engine.RunBattle();

// Get statistics
var stats = collector.GetStatistics();
```

## Extensibility

Easy to add new statistics types:

1. Add property to `BattleStatistics`
2. Create new `IStatisticsTracker` implementation
3. Register tracker: `collector.RegisterTracker(new MyTracker())`

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Technical specification |
| **[Usage Examples](USAGE_EXAMPLES.md)** | Practical examples and patterns |
| **[Proposal](../../STATISTICS_SYSTEM_PROPOSAL.md)** | Design proposal with examples |
| **[Code Location](../../code_location.md#statistics-system)** | Where the code lives |
| **[Testing](../../testing.md)** | Testing strategy |

## Related Sub-Features

- **[2.2: Action Queue System](../2.2-action-queue-system/)** - Statistics system observes actions from queue
- **[2.5: Combat Actions](../2.5-combat-actions/)** - Statistics track all action types
- **[2.6: Combat Engine](../2.6-combat-engine/)** - Statistics collected during battle execution

## Quick Links

- **Key Classes**: `IBattleActionObserver`, `BattleStatisticsCollector`, `BattleStatistics`
- **Status**: ✅ Complete
- **Tests**: 33 tests passing
- **Trackers**: 7 default trackers implemented (ActionType, MoveUsage, Damage, StatusEffect, FieldEffect, Healing, StatChange)

---

**Last Updated**: January 2025

