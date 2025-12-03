# Sub-Feature 2.1: Battle Foundation

> BattleField, Slots, Sides, Rules - Core battle field management.

**Sub-Feature Number**: 2.1  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Battle Foundation provides the core data structures for managing the battle field, including:
- **BattleField**: Main battle container
- **BattleSide**: Player or opponent side
- **BattleSlot**: Individual Pokemon slot on the field
- **BattleRules**: Battle format rules (1v1, 2v2, etc.)

## Current Status

- ✅ **Implemented**: All core structures complete
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md#battle-foundation)** | Technical specification |
| **[Use Cases](../../use_cases.md#battle-formats)** | Battle format scenarios |
| **[Roadmap](../../roadmap.md#phase-21-battle-foundation)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.2: Action Queue System](../2.2-action-queue-system/)** - Uses BattleField for action processing
- **[2.6: Combat Engine](../2.6-combat-engine/)** - Uses BattleField for battle execution

## Quick Links

- **Key Classes**: `BattleField`, `BattleSide`, `BattleSlot`, `BattleRules`
- **Status**: ✅ Complete (Phase 2.1)

---

**Last Updated**: 2025-01-XX

