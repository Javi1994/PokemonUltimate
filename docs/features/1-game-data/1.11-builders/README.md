# Sub-Feature 1.11: Builders

> Fluent builder APIs for creating game data.

**Sub-Feature Number**: 1.11  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines all builder classes for creating game data:
- **13 Builder Classes**: Fluent APIs for creating data blueprints
- **10 Static Helper Classes**: Convenience entry points

## Components

### Builder Classes (13)
**Namespace**: `PokemonUltimate.Core.Builders`  
**Files**: `PokemonUltimate.Core/Builders/*.cs`

- `PokemonBuilder` - Create PokemonSpeciesData
- `MoveBuilder` - Create MoveData
- `AbilityBuilder` - Create AbilityData
- `ItemBuilder` - Create ItemData
- `StatusEffectBuilder` - Create StatusEffectData
- `SideConditionBuilder` - Create SideConditionData
- `FieldEffectBuilder` - Create FieldEffectData
- `HazardBuilder` - Create HazardData
- `WeatherBuilder` - Create WeatherData
- `TerrainBuilder` - Create TerrainData
- `EffectBuilder` - Create Move Effects
- `EvolutionBuilder` - Create Evolution paths
- `LearnsetBuilder` - Create LearnableMove lists

### Static Helper Classes (10)
**Namespace**: `PokemonUltimate.Core.Builders`  
**Files**: Within builder classes

- `Pokemon` - Entry point for PokemonBuilder
- `Move` - Entry point for MoveBuilder
- `Ability` - Entry point for AbilityBuilder
- `Item` - Entry point for ItemBuilder
- `Status` - Entry point for StatusEffectBuilder
- `Screen` - Entry point for SideConditionBuilder
- `Room` - Entry point for FieldEffectBuilder
- `Hazard` - Entry point for HazardBuilder
- `WeatherEffect` - Entry point for WeatherBuilder
- `TerrainEffect` - Entry point for TerrainBuilder

## Usage Example

```csharp
var pikachu = Pokemon.Define("Pikachu", 25)
    .Type(PokemonType.Electric)
    .Stats(35, 55, 40, 50, 50, 90)
    .Build();

var thunderbolt = Move.Define("Thunderbolt")
    .Type(PokemonType.Electric)
    .Category(MoveCategory.Special)
    .Power(90)
    .Accuracy(100)
    .Build();
```

## Related Sub-Features

- All sub-features use builders to create their data structures

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete technical specification |
| **[Parent Architecture](../architecture.md#111-builders)** | Feature-level technical specification |
| **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** | Code organization |

---

**Last Updated**: 2025-01-XX

