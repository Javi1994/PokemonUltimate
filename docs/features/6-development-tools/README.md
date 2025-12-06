# Feature 6: Development Tools

> Windows Forms debugger applications for testing and analyzing the Pokemon battle system.

**Feature Number**: 6  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This feature provides unified Windows Forms debugger applications for testing, analyzing, and understanding different aspects of the Pokemon battle system. These tools help developers verify mechanics, debug issues, and understand system behavior.

**Status**: 🎯 In Progress

## Current Status

- ✅ **Existing Debuggers**: Battle Debugger, Move Debugger, Type Matchup Debugger
- ✅ **Stat Calculator Debugger**: Complete (6.1)
- ✅ **Damage Calculator Debugger**: Complete (6.2)
- ✅ **Status Effect Debugger**: Complete (6.3)
- ⏳ **Planned**: Turn Order Debugger (6.4)

## Documentation

| Document                              | Purpose                                     |
| ------------------------------------- | ------------------------------------------- |
| **[Architecture](architecture.md)**   | Technical specification of debugger system |
| **[Use Cases](use_cases.md)**         | All debugger scenarios and use cases       |
| **[Roadmap](roadmap.md)**             | Implementation phases (6.1-6.4)            |
| **[Testing](testing.md)**             | Testing strategy for debuggers             |
| **[Code Location](code_location.md)** | Where the code lives and how it's organized |

## Sub-Features

### Existing (Implemented)

- **Battle Debugger** - Run multiple battles and analyze statistics ✅
- **Move Debugger** - Test moves multiple times and collect statistics ✅
- **Type Matchup Debugger** - Calculate type effectiveness combinations ✅

### Complete (6.1)

- **[6.1: Stat Calculator Debugger](6.1-stat-calculator-debugger/)** - Calculate and visualize Pokemon stats with different configurations ✅

### Complete (6.2)
- **[6.2: Damage Calculator Debugger](6.2-damage-calculator-debugger/)** - Step-by-step damage calculation pipeline visualization ✅

### Complete (6.3)
- **[6.3: Status Effect Debugger](6.3-status-effect-debugger/)** - Test status effects and their interactions ✅

### Planned (6.4)
- **[6.4: Turn Order Debugger](6.4-turn-order-debugger/)** - Visualize turn order determination with speed and priority 🎯

## Related Features

- **[Feature 1: Game Data](../1-game-data/)** - Pokemon data, stats, types used by debuggers
- **[Feature 2: Combat System](../2-combat-system/)** - Battle mechanics tested by debuggers

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Quick Links

- **Unified Debugger**: `PokemonUltimate.UnifiedDebuggerUI` - Single Windows Forms application
- **Existing Tabs**: Battle Debugger, Move Debugger, Type Matchup
- **New Tabs**: Stat Calculator, Damage Calculator, Status Effect, Turn Order (in progress)

---

**Last Updated**: January 2025

