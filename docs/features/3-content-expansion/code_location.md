# Feature 3: Content Expansion - Code Location

> Where the content code lives and how it's organized.

**Feature Number**: 3  
**Feature Name**: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Content expansion code is organized in the `PokemonUltimate.Content` project:
- **Catalogs** - Static content definitions (Pokemon, Moves, Items, Abilities)
- **Builders** - Fluent APIs for creating content

## Namespaces

### `PokemonUltimate.Content.Catalogs`
**Purpose**: Static catalogs of game content
**Key Classes**:
- `PokemonCatalog` - Pokemon species catalog (partial classes by generation)
- `MoveCatalog` - Moves catalog (partial classes by type)
- `AbilityCatalog` - Abilities catalog (partial classes by category)
- `ItemCatalog` - Items catalog (partial classes by category)
- `StatusCatalog` - Status effects catalog
- `WeatherCatalog` - Weather conditions catalog
- `TerrainCatalog` - Terrain conditions catalog
- `HazardCatalog` - Hazard catalog
- `SideConditionCatalog` - Side condition catalog
- `FieldEffectCatalog` - Field effect catalog

### `PokemonUltimate.Content.Builders`
**Purpose**: Fluent builders for creating content  
**Sub-Feature**: [3.9: Builders](3.9-builders/)  
**Key Classes**:
- `PokemonBuilder` - Builder for `PokemonSpeciesData`
- `MoveBuilder` - Builder for `MoveData`
- `AbilityBuilder` - Builder for `AbilityData`
- `ItemBuilder` - Builder for `ItemData`
- `StatusEffectBuilder` - Builder for `StatusEffectData`
- `SideConditionBuilder` - Builder for `SideConditionData`
- `FieldEffectBuilder` - Builder for `FieldEffectData`
- `HazardBuilder` - Builder for `HazardData`
- `WeatherBuilder` - Builder for `WeatherData`
- `TerrainBuilder` - Builder for `TerrainData`
- `EffectBuilder` - Builder for move effects
- `LearnsetBuilder` - Builder for Pokemon learnsets
- `EvolutionBuilder` - Builder for evolution conditions

## Project Structure

```
PokemonUltimate.Content/
├── Catalogs/
│   ├── Pokemon/
│   │   ├── PokemonCatalog.cs           # Main catalog (All, Count, RegisterAll)
│   │   └── PokemonCatalog.Gen1.cs      # Gen 1 Pokemon (26 implemented)
│   │
│   ├── Moves/
│   │   ├── MoveCatalog.cs              # Main catalog
│   │   ├── MoveCatalog.Normal.cs       # Normal-type moves
│   │   ├── MoveCatalog.Fire.cs         # Fire-type moves
│   │   ├── MoveCatalog.Water.cs        # Water-type moves
│   │   ├── MoveCatalog.Grass.cs        # Grass-type moves
│   │   ├── MoveCatalog.Electric.cs     # Electric-type moves
│   │   ├── MoveCatalog.Ground.cs       # Ground-type moves
│   │   ├── MoveCatalog.Psychic.cs      # Psychic-type moves
│   │   ├── MoveCatalog.Ghost.cs        # Ghost-type moves
│   │   ├── MoveCatalog.Rock.cs         # Rock-type moves
│   │   ├── MoveCatalog.Flying.cs       # Flying-type moves
│   │   ├── MoveCatalog.Poison.cs       # Poison-type moves
│   │   └── MoveCatalog.Dragon.cs       # Dragon-type moves
│   │
│   ├── Abilities/
│   │   ├── AbilityCatalog.cs           # Main catalog
│   │   ├── AbilityCatalog.Gen3.cs      # Gen 3 abilities (25)
│   │   └── AbilityCatalog.Additional.cs # Additional abilities (10)
│   │
│   ├── Items/
│   │   ├── ItemCatalog.cs               # Main catalog
│   │   ├── ItemCatalog.HeldItems.cs    # Held items (15)
│   │   └── ItemCatalog.Berries.cs      # Berries (8)
│   │
│   ├── Status/
│   │   └── StatusCatalog.cs            # Status effects (15)
│   │
│   ├── Weather/
│   │   └── WeatherCatalog.cs           # Weather conditions (9)
│   │
│   ├── Terrain/
│   │   └── TerrainCatalog.cs           # Terrain conditions (4)
│   │
│   ├── Field/
│   │   ├── HazardCatalog.cs            # Hazards (4)
│   │   ├── SideConditionCatalog.cs     # Side conditions (10)
│   │   └── FieldEffectCatalog.cs       # Field effects (8)
│   │
└── Builders/ (Sub-Feature 3.9)
    ├── PokemonBuilder.cs               # Pokemon.Define(...)
    ├── MoveBuilder.cs                  # Move.Define(...)
    ├── AbilityBuilder.cs               # Ability.Define(...)
    ├── ItemBuilder.cs                  # Item.Define(...)
    ├── StatusEffectBuilder.cs          # Status.Define(...)
    ├── SideConditionBuilder.cs         # Screen.Define(...)
    ├── FieldEffectBuilder.cs           # Room.Define(...)
    ├── HazardBuilder.cs                # Hazard.Define(...)
    ├── WeatherBuilder.cs               # WeatherEffect.Define(...)
    ├── TerrainBuilder.cs               # TerrainEffect.Define(...)
    ├── EffectBuilder.cs                # Effect builder
    ├── LearnsetBuilder.cs              # Learnset builder
    └── EvolutionBuilder.cs             # Evolution builder
```

## Key Classes

### PokemonCatalog
**Namespace**: `PokemonUltimate.Content.Catalogs`
**File**: `PokemonUltimate.Content/Catalogs/Pokemon/PokemonCatalog.cs` (+ partial classes)
**Purpose**: Static catalog of all Pokemon species
**Structure**: Partial classes organized by generation
- `PokemonCatalog.cs` - Main catalog class (All, Count, RegisterAll)
- `PokemonCatalog.Gen1.cs` - Gen 1 Pokemon (26 implemented)

**Usage**:
```csharp
var pikachu = PokemonCatalog.Pikachu;  // Returns PokemonSpeciesData
var allGen1 = PokemonCatalog.GetAllGen1();  // Returns all Gen 1 Pokemon
```

**Key Methods**:
- `GetAllGen1()` - Get all Gen 1 Pokemon
- `GetByPokedexNumber(int)` - Get Pokemon by Pokedex number
- `RegisterAll(IPokemonRegistry)` - Register all Pokemon to registry

### MoveCatalog
**Namespace**: `PokemonUltimate.Content.Catalogs`
**File**: `PokemonUltimate.Content/Catalogs/Moves/MoveCatalog.cs` (+ partial classes)
**Purpose**: Static catalog of all moves
**Structure**: Partial classes organized by type
- `MoveCatalog.cs` - Main catalog class
- `MoveCatalog.Fire.cs` - Fire-type moves
- `MoveCatalog.Water.cs` - Water-type moves
- etc.

**Usage**:
```csharp
var flamethrower = MoveCatalog.Flamethrower;  // Returns MoveData
var fireMoves = MoveCatalog.GetAllByType(PokemonType.Fire);  // Returns all Fire moves
```

**Key Methods**:
- `GetAllByType(PokemonType)` - Get all moves of a type
- `RegisterAll(IMoveRegistry)` - Register all moves to registry

### AbilityCatalog
**Namespace**: `PokemonUltimate.Content.Catalogs`
**File**: `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.cs` (+ partial classes)
**Purpose**: Static catalog of all abilities
**Structure**: Partial classes organized by category
- `AbilityCatalog.cs` - Main catalog class
- `AbilityCatalog.Gen3.cs` - Gen 3 abilities
- `AbilityCatalog.Additional.cs` - Additional abilities

**Usage**:
```csharp
var blaze = AbilityCatalog.Blaze;  // Returns AbilityData
var all = AbilityCatalog.GetAll();  // Returns all abilities
```

### ItemCatalog
**Namespace**: `PokemonUltimate.Content.Catalogs`
**File**: `PokemonUltimate.Content/Catalogs/Items/ItemCatalog.cs` (+ partial classes)
**Purpose**: Static catalog of all items
**Structure**: Partial classes organized by category
- `ItemCatalog.cs` - Main catalog class
- `ItemCatalog.HeldItems.cs` - Held items
- `ItemCatalog.Berries.cs` - Berries

**Usage**:
```csharp
var leftovers = ItemCatalog.Leftovers;  // Returns ItemData
```

## Factories & Builders

### PokemonBuilder
**Namespace**: `PokemonUltimate.Content.Builders`
**File**: `PokemonUltimate.Content/Builders/PokemonBuilder.cs`
**Purpose**: Fluent builder for creating `PokemonSpeciesData`
**Usage**:
```csharp
public static readonly PokemonSpeciesData Pikachu = Pokemon.Define("Pikachu", 25)
    .Type(PokemonType.Electric)
    .Stats(35, 55, 30, 50, 40, 90)
    .Ability(AbilityCatalog.Static)
    .Learnset(
        new LearnableMove(MoveCatalog.ThunderShock, 1),
        new LearnableMove(MoveCatalog.QuickAttack, 5)
    )
    .Build();
```

**Key Methods**:
- `Define(string name, int pokedexNumber)` - Start defining a Pokemon
- `Type(PokemonType)` - Set mono-type
- `Types(PokemonType, PokemonType)` - Set dual-type
- `Stats(...)` - Set base stats
- `Ability(AbilityData)` - Set abilities
- `Learnset(...)` - Add learnable moves
- `Evolves(...)` - Add evolution paths
- `Build()` - Create the `PokemonSpeciesData`

### MoveBuilder
**Namespace**: `PokemonUltimate.Content.Builders`
**File**: `PokemonUltimate.Content/Builders/MoveBuilder.cs`
**Purpose**: Fluent builder for creating `MoveData`
**Usage**:
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

**Key Methods**:
- `Define(string name)` - Start defining a move
- `Type(PokemonType)` - Set move type
- `Physical(int power, int accuracy, int pp)` - Set as physical move
- `Special(int power, int accuracy, int pp)` - Set as special move
- `WithEffects(...)` - Add move effects
- `Build()` - Create the `MoveData`

## Current Content Status

### Pokemon
- **Gen 1**: 26/151 implemented
- **Location**: `Catalogs/Pokemon/PokemonCatalog.Gen1.cs`

### Moves
- **Total**: 36 moves across 12 types
- **Location**: `Catalogs/Moves/MoveCatalog.[Type].cs`

### Abilities
- **Total**: 35 abilities
- **Gen 3**: 25 abilities
- **Additional**: 10 abilities
- **Location**: `Catalogs/Abilities/AbilityCatalog.[Category].cs`

### Items
- **Total**: 23 items
- **Held Items**: 15 items
- **Berries**: 8 items
- **Location**: `Catalogs/Items/ItemCatalog.[Category].cs`

### Status Effects
- **Total**: 15 statuses
- **Persistent**: 6 statuses
- **Volatile**: 9 statuses
- **Location**: `Catalogs/Status/StatusCatalog.cs`

### Weather
- **Total**: 9 weather conditions
- **Location**: `Catalogs/Weather/WeatherCatalog.cs`

### Terrain
- **Total**: 4 terrain conditions
- **Location**: `Catalogs/Terrain/TerrainCatalog.cs`

### Hazards
- **Total**: 4 hazard types
- **Location**: `Catalogs/Field/HazardCatalog.cs`

### Side Conditions
- **Total**: 10 side conditions
- **Location**: `Catalogs/Field/SideConditionCatalog.cs`

### Field Effects
- **Total**: 8 field effects
- **Location**: `Catalogs/Field/FieldEffectCatalog.cs`

## Test Location

**Tests**: `PokemonUltimate.Tests/Data/`
- **Pokemon**: `Data/Pokemon/[Pokemon]Tests.cs` (one file per Pokemon)
- **Moves**: `Data/Moves/[Move]Tests.cs` (one file per Move)
- **Catalogs**: `Data/Catalogs/[Catalog]ValidationTests.cs` (general catalog tests)
- **Validation**: `Data/Validation/` (validation against official data)

See **[Testing](testing.md)** for complete test organization.

## Related Documents

- **[Feature README](README.md)** - Overview of Content Expansion
- **[Architecture](architecture.md)** - Technical design of catalog system
- **[Use Cases](use_cases.md)** - Scenarios for adding content
- **[Roadmap](roadmap.md)** - Content expansion phases
- **[Testing](testing.md)** - Testing strategy and test locations
- **[Feature 1: Game Data](../1-game-data/architecture.md)** - Data structure for content
- **[Feature 2: Combat System](../2-combat-system/code_location.md)** - How content is used in combat

---

**Last Updated**: 2025-01-XX

