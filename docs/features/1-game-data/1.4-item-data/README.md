# Sub-Feature 1.4: Item Data

> Item blueprints defining held items and consumables.

**Sub-Feature Number**: 1.4  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for Items:
- **ItemData** (Blueprint): Immutable item data with triggers and effects

## Components

### ItemData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/ItemData.cs`

Immutable blueprint for an item. Contains:
- Identity: Id, Name, Description
- Category: ItemCategory (Held Item, Berry, Consumable, etc.)
- Price: PokeDollars value
- Triggers: ItemTrigger flags (when item activates)
- Effects: Stat modifiers, damage multipliers, etc.
- Properties: IsHoldable, IsConsumable

## Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Items held by Pokemon instances
- **[1.11: Builders](../1.11-builders/)** - ItemBuilder for creating items

## Documentation

- **[Parent Architecture](../architecture.md#14-item-data)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** - Code organization

---

**Last Updated**: 2025-01-XX

