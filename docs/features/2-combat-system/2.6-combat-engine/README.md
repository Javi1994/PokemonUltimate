# Sub-Feature 2.6: Combat Engine

> Battle loop, turn execution, outcome detection - Main battle controller.

**Sub-Feature Number**: 2.6  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Combat Engine orchestrates the entire battle, including:
- **CombatEngine**: Main battle loop controller
- **BattleArbiter**: Victory/defeat detection
- **Turn Execution**: Coordinates turn processing
- **Battle Outcome**: Determines battle result

## Current Status

- ✅ **Implemented**: Complete battle engine with turn execution
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md#combat-engine)** | Technical specification |
| **[Use Cases](../../use_cases.md#turn-flow)** | Battle flow scenarios |
| **[Roadmap](../../roadmap.md#phase-26-combat-engine)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.2: Action Queue System](../2.2-action-queue-system/)** - Engine uses queue for action processing
- **[2.3: Turn Order Resolution](../2.3-turn-order-resolution/)** - Engine uses turn order resolver
- **[2.5: Combat Actions](../2.5-combat-actions/)** - Engine executes actions

## Quick Links

- **Key Classes**: `CombatEngine`, `BattleArbiter`
- **Status**: ✅ Complete (Phase 2.6)

---

**Last Updated**: 2025-01-XX

