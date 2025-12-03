# Sub-Feature 2.3: Turn Order Resolution

> Priority, Speed, Random sorting - Determines action execution order.

**Sub-Feature Number**: 2.3  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Turn Order Resolution determines the order in which actions execute during a turn, considering:
- **Priority**: Move priority (e.g., Quick Attack = +1)
- **Speed**: Pokemon speed stat
- **Random**: Random sorting for same priority/speed

## Current Status

- ✅ **Implemented**: TurnOrderResolver with priority, speed, and random sorting
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md#turn-order)** | Technical specification |
| **[Use Cases](../../use_cases.md#turn-flow)** | Turn order scenarios |
| **[Roadmap](../../roadmap.md#phase-23-turn-order-resolution)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.2: Action Queue System](../2.2-action-queue-system/)** - Actions are ordered before processing
- **[2.6: Combat Engine](../2.6-combat-engine/)** - Uses TurnOrderResolver for turn execution

## Quick Links

- **Key Classes**: `TurnOrderResolver`
- **Status**: ✅ Complete (Phase 2.3)

---

**Last Updated**: 2025-01-XX

