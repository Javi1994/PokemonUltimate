# Sub-Feature 1.9: Side Condition Data

> Side condition blueprint for battle field side-specific effects.

**Sub-Feature Number**: 1.9  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for side conditions that affect all Pokemon on one side of the field:
- **SideConditionData**: Side condition blueprint (Reflect, Light Screen, Tailwind, Aurora Veil, etc.)

Side conditions affect all Pokemon on one team's side and can:
- Reduce damage (screens)
- Boost Speed (Tailwind)
- Provide other team-wide effects

## Components

### SideConditionData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/SideConditionData.cs`

Immutable blueprint defining side condition behavior:
- Duration (turns remaining)
- Damage reduction (physical/special reduction multipliers)
- Speed multiplier (Tailwind doubles Speed)
- Other effects (Aurora Veil combines Reflect + Light Screen)

## Key Features

- **Side-Specific**: Affects only one team's side of the field
- **Damage Reduction**: Screens reduce damage by 50% (Reflect: Physical, Light Screen: Special)
- **Speed Boost**: Tailwind doubles Speed for 4 turns
- **Combined Effects**: Aurora Veil provides both Reflect and Light Screen effects
- **Duration**: Fixed duration (5 turns for screens, 4 for Tailwind)

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that set side conditions
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - SideConditionBuilder for creating side condition data

## Documentation

- **[Parent Architecture](../architecture.md#19-side-condition-data)** - Technical specification
- **[Parent Code Location](../code_location.md#19-side-condition-data)** - Code organization

---

**Last Updated**: 2025-01-XX

