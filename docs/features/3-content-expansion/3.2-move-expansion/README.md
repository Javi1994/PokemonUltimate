# Sub-Feature 3.2: Move Expansion

> Adding more moves across all types.

**Sub-Feature Number**: 3.2  
**Parent Feature**: Feature 3: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Move Expansion focuses on systematically adding moves to the game catalog, ensuring coverage across all types and maintaining quality standards.

**Current Status**: 36 moves (12 types) implemented

## Current Status

- ✅ **Implemented**: 36 moves (12 types)
- 🎯 **In Progress**: Expanding move coverage
- ⏳ **Planned**: Complete move coverage for all types

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Move catalog system design |
| **[Use Cases](../../use_cases.md#uc-002-add-new-move)** | Move addition scenarios |
| **[Roadmap](../../roadmap.md#phase-32-move-expansion)** | Expansion phases and goals |
| **[Testing](../../testing.md)** | Content testing strategy |
| **[Code Location](../../code_location.md)** | Where Move catalog code lives |

## Related Sub-Features

- **[3.5: Status Effect Expansion](../3.5-status-effect-expansion/)** - Status effects catalog
- **[3.6: Field Conditions Expansion](../3.6-field-conditions-expansion/)** - Field conditions catalogs
- **[3.7: Content Validation](../3.7-content-validation/)** - Validates new move data
- **[3.8: Content Organization](../3.8-content-organization/)** - Organizes moves in catalog

## Related Features

- **[Feature 2: Combat System](../../2-combat-system/)** - Moves used in battles
- **[Feature 2.5: Combat Actions - Move Effects Execution](../../2-combat-system/2.5-combat-actions/effects/architecture.md)** - How move effects execute

**⚠️ Always use numbered feature paths**: `../../[N]-[feature-name]/` instead of `../../feature-name/`

## Related Documents

- **[Architecture](architecture.md)** - Move catalog system design and effect properties
- **[Parent Feature README](../README.md)** - Overview of Content Expansion
- **[Parent Architecture](../architecture.md)** - Catalog system design
- **[Parent Use Cases](../use_cases.md#uc-002-add-new-move)** - Move addition scenarios
- **[Parent Roadmap](../roadmap.md#phase-32-expand-move-coverage-all-types)** - Expansion phases and goals
- **[Parent Testing](../testing.md)** - Content testing strategy
- **[Parent Code Location](../code_location.md)** - Where Move catalog code lives

## Quality Standards

- Power/Accuracy match official data (±5 tolerance)
- Type matches official data
- Category (Physical/Special) matches official data
- PP matches official data
- Effects match official behavior

## Quick Links

- **Current Count**: 36 Moves (12 types)
- **Key Classes**: `MoveCatalog`, `MoveBuilder`
- **Status**: 🎯 In Progress

---

**Last Updated**: 2025-01-XX

