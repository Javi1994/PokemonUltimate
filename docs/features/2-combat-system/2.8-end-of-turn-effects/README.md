# Sub-Feature 2.8: End-of-Turn Effects

> Status damage, effects processing - End-of-turn effect system.

**Sub-Feature Number**: 2.8  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

End-of-Turn Effects processes all effects that occur at the end of each turn:
- **Status Damage**: Burn, Poison, Leech Seed damage
- **Status Processing**: Status effect updates
- **Stat Modifications**: Stat stage management
- **EndOfTurnProcessor**: Coordinates end-of-turn processing (instance-based with DI, post-refactor)
- **IEndOfTurnProcessor**: Processor interface for dependency injection

## Current Status

- ✅ **Implemented**: Status damage and processing (refactored to instance-based with DI, 2024-12-05)
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Status & stat management specification |
| **[Use Cases](../../use_cases.md#status-conditions)** | Status effect scenarios |
| **[Roadmap](../../roadmap.md#phase-28-end-of-turn-effects)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.5: Combat Actions](../2.5-combat-actions/)** - Status actions apply status conditions
- **[2.6: Combat Engine](../2.6-combat-engine/)** - Engine triggers end-of-turn processing

## Quick Links

- **Key Classes**: `EndOfTurnProcessor`, status management classes
- **Status**: ✅ Complete (Phase 2.8)

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-05)

