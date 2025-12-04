# Sub-Feature 3.3: Item Expansion

> Adding more held items and consumables.

**Sub-Feature Number**: 3.3  
**Parent Feature**: Feature 3: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Item Expansion focuses on adding held items, berries, and consumables to the game catalog, ensuring variety and balance.

**Current Status**: ~23 items (held items + berries) implemented

## Current Status

- ✅ **Implemented**: ~23 items (held items + berries)
- ⏳ **Planned**: Expand item variety

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Item catalog system design |
| **[Use Cases](../../use_cases.md#uc-003-add-new-item)** | Item addition scenarios |
| **[Roadmap](../../roadmap.md#phase-33-item-expansion)** | Expansion phases and goals |
| **[Testing](../../testing.md)** | Content testing strategy |
| **[Code Location](../../code_location.md)** | Where Item catalog code lives |

## Related Sub-Features

- **[3.5: Status Effect Expansion](../3.5-status-effect-expansion/)** - Status effects catalog
- **[3.6: Field Conditions Expansion](../3.6-field-conditions-expansion/)** - Field conditions catalogs
- **[3.7: Content Validation](../3.7-content-validation/)** - Validates new item data
- **[3.8: Content Organization](../3.8-content-organization/)** - Organizes items in catalog

## Related Features

- **[Feature 2: Combat System](../../2-combat-system/)** - Items used in battles
- **[Feature 2.9: Abilities & Items](../../2-combat-system/2.9-abilities-items/)** - Item effects in combat

**⚠️ Always use numbered feature paths**: `../../[N]-[feature-name]/` instead of `../../feature-name/`

## Related Documents

- **[Parent Feature README](../README.md)** - Overview of Content Expansion
- **[Parent Architecture](../architecture.md)** - Item catalog system design
- **[Parent Use Cases](../use_cases.md#uc-003-add-new-item)** - Item addition scenarios
- **[Parent Roadmap](../roadmap.md#phase-33-item-expansion)** - Expansion phases and goals
- **[Parent Testing](../testing.md)** - Content testing strategy
- **[Parent Code Location](../code_location.md)** - Where Item catalog code lives

## Quality Standards

- Item effects match official behavior
- Held item stat modifiers correct
- Berry effects correct
- Item categories properly assigned

## Quick Links

- **Current Count**: ~23 Items
- **Key Classes**: `ItemCatalog`, `ItemBuilder`
- **Status**: ⏳ Planned

---

**Last Updated**: 2025-01-XX

