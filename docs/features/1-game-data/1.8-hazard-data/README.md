# Sub-Feature 1.8: Hazard Data

> Entry hazard blueprint for battle field entry hazards.

**Sub-Feature Number**: 1.8  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for entry hazards that activate when Pokemon switch in:
- **HazardData**: Entry hazard blueprint (Stealth Rock, Spikes, Toxic Spikes, Sticky Web)

Entry hazards activate when Pokemon enter the field and can:
- Deal damage based on layers and type effectiveness
- Apply status conditions based on layers
- Lower stats
- Support multiple layers (1-3 layers)

## Components

### HazardData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/HazardData.cs`

Immutable blueprint defining entry hazard behavior:
- Layer system (max layers: 1 for Stealth Rock, 3 for Spikes, 2 for Toxic Spikes)
- Targeting (affects Flying types, affects Levitate)
- Damage system (type effectiveness, damage by layer)
- Status system (status by layer, absorbed by Poison types)
- Stat changes (stat to lower, stages)
- Removal (moves that remove the hazard)

## Key Features

- **Layer System**: Multiple layers increase damage/effect (Spikes: 1/8, 1/6, 1/4 HP)
- **Type Effectiveness**: Stealth Rock uses Rock type effectiveness
- **Flying Immunity**: Most hazards don't affect Flying types (except Stealth Rock)
- **Levitate Immunity**: Most hazards don't affect Pokemon with Levitate
- **Status Application**: Toxic Spikes applies Poison/Badly Poisoned based on layers
- **Stat Lowering**: Sticky Web lowers Speed by 1 stage
- **Poison Absorption**: Toxic Spikes absorbed by Poison types (removes hazard)
- **Removal**: Can be removed by Rapid Spin, Defog, etc.

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that set or remove hazards
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - HazardBuilder for creating hazard data
- **[1.5: Status Effect Data](../1.5-status-effect-data/)** - Status conditions applied by hazards

## Documentation

- **[Parent Architecture](../architecture.md#18-hazard-data)** - Technical specification
- **[Parent Code Location](../code_location.md#18-hazard-data)** - Code organization

---

**Last Updated**: 2025-01-XX

