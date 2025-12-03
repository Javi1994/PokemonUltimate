# Sub-Feature 1.6: Field Conditions Data

> Field condition blueprints for weather, terrain, hazards, side conditions, and field effects.

**Sub-Feature Number**: 1.6  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structures for all field conditions:
- **WeatherData**: Weather conditions (Rain, Sun, Hail, Sandstorm, etc.)
- **TerrainData**: Terrain conditions (Grassy, Electric, Psychic, Misty)
- **HazardData**: Entry hazards (Stealth Rock, Spikes, Toxic Spikes, Sticky Web)
- **SideConditionData**: Side conditions (Screens, Tailwind, protections)
- **FieldEffectData**: Field effects (Rooms, Gravity, sports)

## Components

### WeatherData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/WeatherData.cs`

Weather condition blueprint (Rain, Sun, Hail, Sandstorm, Fog, Primal weathers).

### TerrainData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/TerrainData.cs`

Terrain condition blueprint (Grassy, Electric, Psychic, Misty). Only affects grounded Pokemon.

### HazardData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/HazardData.cs`

Entry hazard blueprint (Stealth Rock, Spikes, Toxic Spikes, Sticky Web).

### SideConditionData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/SideConditionData.cs`

Side condition blueprint (Reflect, Light Screen, Tailwind, Aurora Veil, etc.).

### FieldEffectData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/FieldEffectData.cs`

Field effect blueprint (Trick Room, Wonder Room, Magic Room, Gravity, etc.).

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that set field conditions
- **[1.11: Builders](../1.11-builders/)** - Builders for creating field conditions

## Documentation

- **[Parent Architecture](../architecture.md#16-field-conditions-data)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-b-field--status-data)** - Code organization

---

**Last Updated**: 2025-01-XX

