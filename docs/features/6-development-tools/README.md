# Feature 6: Development Tools

> Windows Forms debugger applications for testing and analyzing the Pokemon battle system.

**Feature Number**: 6  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This feature provides unified Windows Forms debugger applications for testing, analyzing, and understanding different aspects of the Pokemon battle system. These tools help developers verify mechanics, debug issues, and understand system behavior.

**Status**: 🎯 In Progress

## Current Status

- ✅ **Battle Debugger**: Complete (6.5)
- ✅ **Move Debugger**: Complete (6.6)
- ✅ **Type Matchup Debugger**: Complete (existing)
- ✅ **Stat Calculator Debugger**: Complete (6.1)
- ✅ **Damage Calculator Debugger**: Complete (6.2)
- ✅ **Status Effect Debugger**: Complete (6.3)
- ✅ **Turn Order Debugger**: Complete (6.4)

## Documentation

| Document                              | Purpose                                     |
| ------------------------------------- | ------------------------------------------- |
| **[Architecture](architecture.md)**   | Technical specification of debugger system |
| **[Use Cases](use_cases.md)**         | All debugger scenarios and use cases       |
| **[Roadmap](roadmap.md)**             | Implementation phases (6.1-6.4)            |
| **[Testing](testing.md)**             | Testing strategy for debuggers             |
| **[Code Location](code_location.md)** | Where the code lives and how it's organized |

## Sub-Features

### Complete (6.5)

- **[6.5: Battle Debugger](6.5-battle-debugger/)** - Run multiple battles and analyze statistics ✅

### Complete (6.6)

- **[6.6: Move Debugger](6.6-move-debugger/)** - Test moves multiple times and collect statistics ✅

### Existing (Implemented)

- **Type Matchup Debugger** - Calculate type effectiveness combinations ✅

### Complete (6.1)

- **[6.1: Stat Calculator Debugger](6.1-stat-calculator-debugger/)** - Calculate and visualize Pokemon stats with different configurations ✅

### Complete (6.2)
- **[6.2: Damage Calculator Debugger](6.2-damage-calculator-debugger/)** - Step-by-step damage calculation pipeline visualization ✅

### Complete (6.3)
- **[6.3: Status Effect Debugger](6.3-status-effect-debugger/)** - Test status effects and their interactions ✅

### Complete (6.4)
- **[6.4: Turn Order Debugger](6.4-turn-order-debugger/)** - Visualize turn order determination with speed and priority ✅

## Related Features

- **[Feature 1: Game Data](../1-game-data/)** - Pokemon data, stats, types used by debuggers
- **[Feature 2: Combat System](../2-combat-system/)** - Battle mechanics tested by debuggers

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Quick Links

- **Develop Tools**: `PokemonUltimate.DevelopTools` - Single Windows Forms application
- **Complete Tabs**: Battle Debugger (6.5), Move Debugger (6.6), Type Matchup, Stat Calculator (6.1), Damage Calculator (6.2), Status Effect (6.3), Turn Order (6.4)

---

**Last Updated**: January 2025

