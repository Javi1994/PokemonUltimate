# Sub-Feature 3.6: Field Conditions Expansion

> Field conditions catalog (Weather, Terrain, Hazards, Side Conditions, Field Effects - 35 total).

**Sub-Feature Number**: 3.6  
**Parent Feature**: Feature 3: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Field Conditions Expansion covers all field conditions that affect battles:
- **Weather**: Rain, Sun, Sandstorm, Hail, Snow, Heavy Rain, Extremely Harsh Sunlight, Strong Winds, Fog (9 total)
- **Terrain**: Grassy Terrain, Electric Terrain, Psychic Terrain, Misty Terrain (4 total)
- **Hazards**: Stealth Rock, Spikes, Toxic Spikes, Sticky Web (4 total)
- **Side Conditions**: Reflect, Light Screen, Aurora Veil, Tailwind, Safeguard, Mist, Lucky Chant, Wide Guard, Quick Guard, Mat Block (10 total)
- **Field Effects**: Trick Room, Magic Room, Wonder Room, Gravity, Ion Deluge, Fairy Lock, Mud Sport, Water Sport (8 total)

**Status**: ✅ Complete (35 field conditions)

## Current Content

### Weather (9)
- **Standard Weather** (5 turns): Rain, Harsh Sunlight, Sandstorm, Hail, Snow
- **Primal Weather** (indefinite): Heavy Rain, Extremely Harsh Sunlight, Strong Winds
- **Special**: Fog

### Terrain (4)
- **Grassy Terrain** - Grass moves 1.3x, heals 1/16 HP, halves Earthquake
- **Electric Terrain** - Electric moves 1.3x, prevents Sleep
- **Psychic Terrain** - Psychic moves 1.3x, blocks Priority
- **Misty Terrain** - Dragon moves 0.5x, prevents all status

### Hazards (4)
- **Stealth Rock** - Type-based damage on switch-in (Rock effectiveness)
- **Spikes** - 12.5% / 16.7% / 25% HP damage (1-3 layers)
- **Toxic Spikes** - Poison / Badly Poisoned on switch-in (1-2 layers)
- **Sticky Web** - -1 Speed on switch-in

### Side Conditions (10)
- **Screens**: Reflect, Light Screen, Aurora Veil
- **Speed/Status**: Tailwind, Safeguard, Mist, Lucky Chant
- **Protection**: Wide Guard, Quick Guard, Mat Block

### Field Effects (8)
- **Rooms**: Trick Room, Magic Room, Wonder Room
- **Field**: Gravity, Ion Deluge, Fairy Lock
- **Sports**: Mud Sport, Water Sport

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Field conditions catalog design |
| **[Use Cases](../../use_cases.md)** | Field condition scenarios |
| **[Roadmap](../../roadmap.md)** | Field conditions expansion plan |
| **[Testing](../../testing.md)** | Field conditions testing strategy |
| **[Code Location](../../code_location.md)** | Where field condition catalogs code lives |

## Related Sub-Features

- **[3.1: Pokemon Expansion](../3.1-pokemon-expansion/)** - Pokemon can use weather/terrain moves
- **[3.2: Move Expansion](../3.2-move-expansion/)** - Moves can set field conditions
- **[3.4: Ability Expansion](../3.4-ability-expansion/)** - Abilities can set weather/terrain

## Related Documents

- **[Parent Feature README](../README.md)** - Overview of Content Expansion
- **[Feature 1.6: Weather Data](../../1-game-data/1.6-weather-data/README.md)** - Weather data structures
- **[Feature 1.7: Terrain Data](../../1-game-data/1.7-terrain-data/README.md)** - Terrain data structures
- **[Feature 1.8: Hazard Data](../../1-game-data/1.8-hazard-data/README.md)** - Hazard data structures
- **[Feature 1.9: Side Condition Data](../../1-game-data/1.9-side-condition-data/README.md)** - Side condition data structures
- **[Feature 1.10: Field Effect Data](../../1-game-data/1.10-field-effect-data/README.md)** - Field effect data structures
- **[Feature 2.12-2.16: Field Conditions Systems](../../2-combat-system/roadmap.md)** - How field conditions work in combat

## Quick Links

- **Key Classes**: 
  - `WeatherCatalog`, `TerrainCatalog`
  - `HazardCatalog`, `SideConditionCatalog`, `FieldEffectCatalog`
- **Status**: ✅ Complete (35 field conditions)
- **Location**: `PokemonUltimate.Content/Catalogs/Weather/`, `Terrain/`, `Field/`

---

**Last Updated**: 2025-01-XX

