# Sub-Feature 3.9: Builders

> Fluent builder APIs for creating game content.

**Sub-Feature Number**: 3.9  
**Parent Feature**: Feature 3: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Builders provide fluent APIs for creating game content (Pokemon, Moves, Items, Abilities, Status Effects, Field Conditions, etc.). They enable readable, type-safe content definitions in catalogs.

**Status**: ✅ Complete

## Components

### Builder Classes (13)

**Namespace**: `PokemonUltimate.Content.Builders`  
**Files**: `PokemonUltimate.Core/Builders/*.cs` (Note: Files are physically in Core but namespace is Content.Builders)

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

**Namespace**: `PokemonUltimate.Content.Builders`  
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

## Usage Examples

### Pokemon Builder

```csharp
public static readonly PokemonSpeciesData Pikachu = Pokemon.Define("Pikachu", 25)
    .Type(PokemonType.Electric)
    .Stats(35, 55, 40, 50, 50, 90)
    .Ability(AbilityCatalog.Static)
    .Moves(m => m
        .StartsWith(MoveCatalog.ThunderShock)
        .AtLevel(9, MoveCatalog.QuickAttack))
    .EvolvesTo(Raichu, e => e.WithItem("Thunder Stone"))
    .Build();
```

### Move Builder

```csharp
public static readonly MoveData Flamethrower = Move.Define("Flamethrower")
    .Description("The target is scorched with an intense blast of fire.")
    .Type(PokemonType.Fire)
    .Special(90, 100, 15)
    .WithEffects(e => e
        .Damage()
        .MayBurn(10))
    .Build();
```

### Status Effect Builder

```csharp
public static readonly StatusEffectData Burn = Status.Define("Burn")
    .Description("The Pokémon is burned. It takes damage each turn and its Attack is halved.")
    .Persistent(PersistentStatus.Burn)
    .Indefinite()
    .DealsDamagePerTurn(1f / 16f)
    .HalvesPhysicalAttack()
    .ImmuneTypes(PokemonType.Fire)
    .Build();
```

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Catalog system design (includes builder examples) |
| **[Use Cases](../../use_cases.md)** | Content addition scenarios using builders |
| **[Roadmap](../../roadmap.md)** | Content expansion phases |
| **[Testing](../../testing.md)** | Content testing strategy |
| **[Code Location](../../code_location.md)** | Where builder code lives |

## Related Sub-Features

- **[3.1: Pokemon Expansion](../3.1-pokemon-expansion/)** - Uses PokemonBuilder
- **[3.2: Move Expansion](../3.2-move-expansion/)** - Uses MoveBuilder and EffectBuilder
- **[3.3: Item Expansion](../3.3-item-expansion/)** - Uses ItemBuilder
- **[3.4: Ability Expansion](../3.4-ability-expansion/)** - Uses AbilityBuilder
- **[3.5: Status Effect Expansion](../3.5-status-effect-expansion/)** - Uses StatusEffectBuilder
- **[3.6: Field Conditions Expansion](../3.6-field-conditions-expansion/)** - Uses WeatherBuilder, TerrainBuilder, HazardBuilder, SideConditionBuilder, FieldEffectBuilder

## Related Features

- **[Feature 1: Game Data](../../1-game-data/)** - Builders create data structures defined in Feature 1

**⚠️ Always use numbered feature paths**: `../../[N]-[feature-name]/` instead of `../../feature-name/`

## Quick Links

- **Key Classes**: All builder classes in `PokemonUltimate.Content.Builders` namespace
- **Status**: ✅ Complete
- **Location**: `PokemonUltimate.Core/Builders/*.cs` (namespace: `PokemonUltimate.Content.Builders`)

---

**Last Updated**: 2025-01-XX

