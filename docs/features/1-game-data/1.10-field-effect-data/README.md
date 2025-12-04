# Sub-Feature 1.10: Field Effect Data

> Field effect blueprint for battle field-wide effects.

**Sub-Feature Number**: 1.10  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for field effects that affect the entire battlefield (both sides):
- **FieldEffectData**: Field effect blueprint (Trick Room, Wonder Room, Magic Room, Gravity, etc.)

Field effects affect all Pokemon on both sides and can:
- Modify Speed order (Trick Room reverses Speed)
- Modify stat calculations (Wonder Room swaps Defense/SpDef)
- Modify item effects (Magic Room disables held items)
- Modify move behavior (Gravity affects accuracy and Flying types)

## Components

### FieldEffectData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/FieldEffectData.cs`

Immutable blueprint defining field effect behavior:
- Duration (turns remaining)
- Speed modification (Trick Room reverses Speed order)
- Stat modifications (Wonder Room swaps stats)
- Item modifications (Magic Room disables items)
- Move modifications (Gravity affects accuracy, Flying types)

## Key Features

- **Field-Wide**: Affects all Pokemon on both sides
- **Speed Reversal**: Trick Room makes slower Pokemon move first
- **Stat Swapping**: Wonder Room swaps Defense and SpDef for all Pokemon
- **Item Disabling**: Magic Room disables all held items
- **Gravity Effects**: Gravity makes all moves 100% accurate and grounds Flying types
- **Duration**: Fixed duration (5 turns for rooms, 5 for Gravity)

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that set field effects
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - FieldEffectBuilder for creating field effect data

## Documentation

- **[Parent Architecture](../architecture.md#110-field-effect-data)** - Technical specification
- **[Parent Code Location](../code_location.md#110-field-effect-data)** - Code organization

---

**Last Updated**: 2025-01-XX

