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
- **BattleTriggerProcessor**: Coordinates trigger processing

## Current Status

- ✅ **Implemented**: Event-driven system with basic triggers
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
- **[2.10: Pipeline Hooks](../2.10-pipeline-hooks/)** - Abilities/items hook into damage pipeline

## Quick Links

- **Key Classes**: `IBattleListener`, `BattleTrigger`, `AbilityListener`, `ItemListener`
- **Status**: ✅ Complete (Phase 2.9)

---

**Last Updated**: 2025-01-XX

