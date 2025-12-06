# Sub-Feature 2.19: Battle Formats

> Doubles, Triples, Horde, Raid - Multiple battle format support.

**Sub-Feature Number**: 2.19  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Battle Formats extends the battle system to support multiple formats:
- **Doubles**: 2v2 battles
- **Triples**: 3v3 battles
- **Horde**: 1vs2, 1vs3, 1vs5 battles
- **Raid**: 1vsBoss, 2vsBoss battles
- **Format-Specific Rules**: Format-specific mechanics

## Current Status

- ✅ **Complete**: All battle formats implemented and tested
  - ✅ Doubles (2v2) - 8 tests passing
  - ✅ Triples (3v3) - 8 tests passing
  - ✅ Horde (1vs2, 1vs3, 1vs5) - 8 tests passing
  - ✅ Raid (1vsBoss, 2vsBoss) - 6 tests passing
  - ✅ Integration Tests - 48 tests passing

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Technical specification (when implemented) |
| **[Use Cases](../../use_cases.md)** | Battle format scenarios |
| **[Roadmap](../../roadmap.md#phase-219-battle-formats)** | Implementation plan |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where code will live |

## Related Sub-Features

- **[2.1: Battle Foundation](../2.1-battle-foundation/)** - Formats build on battle foundation
- **[2.5: Combat Actions](../2.5-combat-actions/)** - Format-specific actions

## Quick Links

- **Status**: ✅ Complete (Phase 2.19)
- **Tests**: 48 integration tests passing
- **Implementation**: All formats fully functional with targeting, spread moves, screens, abilities, items, and field conditions

---

**Last Updated**: 2025-12-06

