# Sub-Feature 2.9: Abilities & Items

> Event-driven system, triggers - Ability and item activation system.

**Sub-Feature Number**: 2.9  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Abilities & Items implements an event-driven system for ability and item triggers:
- **IBattleListener**: Interface for ability/item listeners
- **BattleTrigger**: Event types (OnSwitchIn, OnTurnEnd, etc.)
- **AbilityListener**: Ability trigger handling
- **ItemListener**: Item trigger handling
- **BattleTriggerProcessor**: Coordinates trigger processing (instance-based with DI, post-refactor)
- **IBattleTriggerProcessor**: Processor interface for dependency injection
- **IBattleEventBus**: Event bus interface for decoupled communication (post-refactor)
- **BattleEventBus**: Event bus implementation (post-refactor)

## Current Status

- ✅ **Implemented**: Event-driven system with basic triggers (refactored to instance-based with DI and Event Bus, 2024-12-05)
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md#abilities-items)** | Technical specification |
| **[Use Cases](../../use_cases.md#abilities-items)** | Ability and item scenarios |
| **[Roadmap](../../roadmap.md#phase-29-abilities-items)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.6: Combat Engine](../2.6-combat-engine/)** - Engine triggers ability/item events
- **[2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Abilities/items provide data for stat/damage modifiers

## Quick Links

- **Key Classes**: `IBattleListener`, `BattleTrigger`, `AbilityListener`, `ItemListener`
- **Status**: ✅ Complete (Phase 2.9)

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-05)

