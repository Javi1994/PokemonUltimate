# Sub-Feature 2.13: Terrain System

> Terrain conditions and effects - Terrain system implementation.

**Sub-Feature Number**: 2.13  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Terrain System implements terrain conditions that affect battles:
- **Terrain Types**: Grassy Terrain, Electric Terrain, Psychic Terrain, Misty Terrain
- **Terrain Effects**: Type effectiveness changes, move restrictions
- **Terrain Duration**: Turn-based terrain persistence

## Current Status

- ✅ **Implemented**: Core terrain system complete
- ✅ **Data Ready**: TerrainData blueprint exists
- ✅ **Features**: Terrain tracking, damage modifiers, end-of-turn healing, terrain actions

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Technical specification (when implemented) |
| **[Use Cases](../../use_cases.md)** | Terrain scenarios |
| **[Roadmap](../../roadmap.md#phase-213-terrain-system)** | Implementation plan |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where code will live |

## Related Sub-Features

- **[2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Terrain modifies damage
- **[2.5: Combat Actions](../2.5-combat-actions/)** - Terrain affects move execution

## Quick Links

- **Status**: ✅ Implemented (Core features complete)
- **Tests**: 84+ terrain-related tests passing
- **Advanced Features Pending**: Status prevention, priority blocking, move-specific modifications (depend on other systems)

---

**Last Updated**: 2025-01-XX

