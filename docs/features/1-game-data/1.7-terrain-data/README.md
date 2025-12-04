# Sub-Feature 1.7: Terrain Data

> Terrain condition blueprint for battle field terrain effects.

**Sub-Feature Number**: 1.7  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for terrain conditions that affect only "grounded" Pokemon:
- **TerrainData**: Terrain condition blueprint (Grassy, Electric, Psychic, Misty)

Terrains only affect Pokemon that are grounded (not Flying type, Levitate ability, or Air Balloon). They can:
- Boost type power (1.3x for matching type)
- Reduce damage from certain types
- Heal HP at end of turn
- Prevent status conditions
- Block priority moves
- Modify specific moves (Nature Power, Camouflage, Secret Power)

## Components

### TerrainData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/TerrainData.cs`

Immutable blueprint defining terrain condition behavior:
- Duration (default 5 turns, 8 with Terrain Extender)
- Type power modifiers (boosted type with multiplier)
- Damage reduction (reduced damage type with multiplier)
- End-of-turn healing (as fraction of max HP)
- Status prevention (prevented status conditions)
- Move modifications (priority blocking, halved damage moves)
- Ability interactions (setter abilities, benefiting abilities)

## Key Features

- **Grounded Check**: Only affects Pokemon without Flying type, Levitate, or Air Balloon
- **Type Boost**: 1.3x power multiplier for matching type (Gen 7+)
- **Damage Reduction**: 0.5x damage multiplier for certain types (e.g., Misty vs Dragon)
- **Healing**: End-of-turn HP restoration (1/16 max HP for Grassy Terrain)
- **Status Prevention**: Prevents status conditions for grounded Pokemon
- **Priority Blocking**: Blocks priority moves against grounded targets (Psychic Terrain)
- **Move Modifications**: Changes Nature Power, Camouflage, Secret Power effects

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that set terrain conditions
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - TerrainBuilder for creating terrain data
- **[1.3: Ability Data](../1.3-ability-data/)** - Abilities that set or benefit from terrain

## Documentation

- **[Parent Architecture](../architecture.md#17-terrain-data)** - Technical specification
- **[Parent Code Location](../code_location.md#17-terrain-data)** - Code organization

---

**Last Updated**: 2025-01-XX

