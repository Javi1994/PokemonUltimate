# Feature 1: Game Data - Code Location

> Where all game data code lives and how it's organized.

**Feature Number**: 1  
**Feature Name**: Game Data (formerly "Pokemon Data")  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Game data is organized into logical groups matching the sub-feature structure:

-   **Grupo A: Core Entity Data** (1.1-1.4) - Pokemon, Moves, Abilities, Items (blueprints and instances)
-   **Grupo B: Field & Status Data** (1.5-1.10) - Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects
-   **Grupo C: Supporting Systems** (1.11-1.12) - Evolution System, Type Effectiveness Table
-   **Grupo D: Infrastructure** (1.13-1.14, 1.16-1.17) - Interfaces, Enums, Constants, Factories, Registries (Note: Builders moved to Feature 3.9)
-   **Grupo E: Planned Features** (1.18-1.19) - Variants System, Pokedex Fields
-   **Content** - Data definitions in `PokemonUltimate.Content` (Feature 3: Content Expansion)

**See**: [Sub-Features Overview](../1-game-data/README.md) for complete sub-feature list.

## Namespaces (Organized by Groups)

### Grupo A: Core Entity Data (Sub-Features 1.1-1.4)

#### 1.1: Pokemon Data

**Sub-Feature**: [1.1: Pokemon Data](1.1-pokemon-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`, `PokemonUltimate.Core.Instances`

**Key Classes**:

-   `PokemonSpeciesData` - Pokemon species blueprint (types, stats, abilities, learnset)
-   `PokemonInstance` - Individual Pokemon instance (level, HP, status, moves)
-   `BaseStats` - Base stat values (HP, Attack, Defense, SpAttack, SpDefense, Speed)
-   `LearnableMove` - Move learning information (move, method, level)

#### 1.2: Move Data

**Sub-Feature**: [1.2: Move Data](1.2-move-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`, `PokemonUltimate.Core.Instances`, `PokemonUltimate.Core.Effects`

**Key Classes**:

-   `MoveData` - Move blueprint (type, category, power, accuracy, effects)
-   `MoveInstance` - Move instance with PP tracking
-   `IMoveEffect` - Base interface for all move effects
-   22 Move Effect classes (DamageEffect, StatusEffect, StatChangeEffect, etc.)

#### 1.3: Ability Data

**Sub-Feature**: [1.3: Ability Data](1.3-ability-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `AbilityData` - Ability blueprint (triggers, effects, modifiers)

#### 1.4: Item Data

**Sub-Feature**: [1.4: Item Data](1.4-item-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `ItemData` - Item blueprint (category, triggers, effects)

**PokemonInstance Partial Classes** (Sub-Feature 1.1):

-   `PokemonInstance.Core.cs` - Core instance data (partial class)
-   `PokemonInstance.Battle.cs` - Battle-specific state (partial class)
-   `PokemonInstance.LevelUp.cs` - Level-up and evolution logic (partial class)
-   `PokemonInstance.Evolution.cs` - Evolution tracking (partial class)

### Grupo B: Field & Status Data (Sub-Features 1.5-1.10)

#### 1.5: Status Effect Data

**Sub-Feature**: [1.5: Status Effect Data](1.5-status-effect-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `StatusEffectData` - Status effect blueprint (persistent and volatile)

#### 1.6: Weather Data

**Sub-Feature**: [1.6: Weather Data](1.6-weather-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `WeatherData` - Weather condition blueprint

#### 1.7: Terrain Data

**Sub-Feature**: [1.7: Terrain Data](1.7-terrain-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `TerrainData` - Terrain condition blueprint

#### 1.8: Hazard Data

**Sub-Feature**: [1.8: Hazard Data](1.8-hazard-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `HazardData` - Entry hazard blueprint

#### 1.9: Side Condition Data

**Sub-Feature**: [1.9: Side Condition Data](1.9-side-condition-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `SideConditionData` - Side condition blueprint (screens, Tailwind, etc.)

#### 1.10: Field Effect Data

**Sub-Feature**: [1.10: Field Effect Data](1.10-field-effect-data/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `FieldEffectData` - Field effect blueprint (rooms, Gravity, etc.)

### Grupo C: Supporting Systems (Sub-Features 1.11-1.12)

#### 1.11: Evolution System

**Sub-Feature**: [1.11: Evolution System](1.11-evolution-system/)  
**Namespace**: `PokemonUltimate.Core.Evolution`

**Key Classes**:

-   `Evolution` - Evolution path definition
-   `IEvolutionCondition` - Base interface for evolution conditions
-   `EvolutionConditionType` - Enum of condition types
-   `LevelCondition`, `ItemCondition`, `FriendshipCondition`
-   `TradeCondition`, `TimeOfDayCondition`, `KnowsMoveCondition`

#### 1.12: Type Effectiveness Table

**Sub-Feature**: [1.12: Type Effectiveness Table](1.12-type-effectiveness-table/)  
**Namespace**: `PokemonUltimate.Core.Factories`

**Key Classes**:

-   `TypeEffectiveness` - Type effectiveness table (Gen 6+ chart)

### Grupo D: Infrastructure (Sub-Features 1.13-1.17)

#### 1.13: Interfaces Base

**Sub-Feature**: [1.13: Interfaces Base](1.13-interfaces-base/)  
**Namespace**: `PokemonUltimate.Core.Blueprints`

**Key Classes**:

-   `IIdentifiable` - Base interface for identifiable data

#### 1.14: Enums & Constants

**Sub-Feature**: [1.14: Enums & Constants](1.14-enums-constants/)  
**Namespace**: `PokemonUltimate.Core.Enums`, `PokemonUltimate.Core.Constants`

**Key Enums** (20 main + 7 in Effects):

-   `PokemonType`, `Stat`, `Nature`, `Gender`
-   `MoveCategory`, `EffectType`, `PersistentStatus`, `VolatileStatus`
-   `AbilityTrigger`, `AbilityEffect`, `ItemTrigger`, `ItemCategory`
-   `LearnMethod`, `TimeOfDay`, `TargetScope`
-   `Weather`, `Terrain`, `HazardType`, `SideCondition`, `FieldEffect`
-   `EvolutionConditionType`

**Key Constants & Data Tables**:

-   `ErrorMessages` - Centralized error message strings
-   `GameMessages` - In-game message strings for UI
-   `NatureData` - Nature modifier tables (complements Nature enum)
-   **Enums in Effects**: `SemiInvulnerableState`, `FieldConditionType`, `MoveRestrictionType`, `ProtectionType`, `ContactPenalty`, `PriorityCondition`, `SelfDestructType`

**Key Classes**:

-   `ErrorMessages` - Error message constants
-   `GameMessages` - In-game message constants

#### Builders (Moved to Feature 3.9)

**Sub-Feature**: **[3.9: Builders](../3-content-expansion/3.9-builders/)**  
**Namespace**: `PokemonUltimate.Content.Builders`  
**Note**: Builders moved to Feature 3 as they are primarily used for content creation

#### 1.15: Factories & Calculators

**Sub-Feature**: [1.15: Factories & Calculators](1.15-factories-calculators/)  
**Namespace**: `PokemonUltimate.Core.Factories`

**Key Classes**:

-   `StatCalculator` - Stat calculation formulas (Gen 3+)
-   `PokemonFactory` - Static factory for Pokemon creation
-   `PokemonInstanceBuilder` - Fluent builder for Pokemon instances

#### 1.16: Registry System

**Sub-Feature**: [1.16: Registry System](1.16-registry-system/)  
**Namespace**: `PokemonUltimate.Core.Registry`

**Key Classes**:

-   `IDataRegistry<T>` - Generic registry interface
-   `GameDataRegistry<T>` - Generic registry implementation
-   `IPokemonRegistry`, `IMoveRegistry` - Specialized interfaces
-   `PokemonRegistry`, `MoveRegistry` - Specialized implementations

### Grupo E: Planned Features (Sub-Features 1.18-1.19)

#### 1.18: Variants System

**Sub-Feature**: [1.18: Variants System](1.18-variants-system/) ⏳ Planned  
**Namespace**: `PokemonUltimate.Core.Blueprints` (extensions to PokemonSpeciesData)

**Planned Fields**:

-   `BaseForm` - Reference to base Pokemon
-   `VariantType` - Type of variant (Mega, Dinamax, Tera)
-   `TeraType` - Tera type for Terracristalización variants
-   `Variants` - List of all variant forms

#### 1.19: Pokedex Fields

**Sub-Feature**: [1.19: Pokedex Fields](1.19-pokedex-fields/) ⏳ Planned  
**Namespace**: `PokemonUltimate.Core.Blueprints` (extensions to PokemonSpeciesData), `PokemonUltimate.Core.Enums`

**Planned Fields**:

-   `Description` - Pokedex entry text
-   `Category` - Classification
-   `Height`, `Weight` - Physical measurements
-   `Color`, `Shape`, `Habitat` - Classification enums

---

### Content (Feature 3: Content Expansion)

#### `PokemonUltimate.Content.Catalogs`

**Purpose**: Static catalogs of game data (Feature 3, not Feature 1)  
**Note**: These contain specific data instances, not data structures

**Key Classes**:

-   `PokemonCatalog` - Pokemon species catalog (26 Gen 1 implemented)
-   `MoveCatalog` - Move catalog
-   `AbilityCatalog` - Ability catalog (35 abilities)
-   `ItemCatalog` - Item catalog (23 items)
-   `StatusCatalog` - Status effect catalog (15 statuses)
-   `WeatherCatalog` - Weather catalog
-   `TerrainCatalog` - Terrain catalog
-   `HazardCatalog` - Hazard catalog
-   `SideConditionCatalog` - Side condition catalog
-   `FieldEffectCatalog` - Field effect catalog

## Project Structure

```
PokemonUltimate.Core/
├── Blueprints/
│   └── PokemonSpeciesData.cs          # Species blueprint class
├── Instances/
│   ├── PokemonInstance.cs             # Main instance class
│   ├── PokemonInstance.Core.cs        # Core data (partial)
│   ├── PokemonInstance.Battle.cs      # Battle state (partial)
│   ├── PokemonInstance.LevelUp.cs    # Level-up logic (partial)
│   └── PokemonInstance.Evolution.cs  # Evolution tracking (partial)
├── Factories/
│   └── PokemonInstanceBuilder.cs     # Instance builder
└── Enums/
    ├── PokemonType.cs                 # Type enum
    ├── PokemonColor.cs                # Pokedex color enum
    ├── PokemonShape.cs                # Pokedex shape enum
    └── PokemonHabitat.cs             # Pokedex habitat enum

PokemonUltimate.Content/
├── Builders/
│   └── PokemonBuilder.cs              # Species builder
└── Catalogs/
    └── Pokemon/
        ├── PokemonCatalog.cs          # Main catalog (partial)
        ├── PokemonCatalog.Gen1.cs     # Gen 1 Pokemon (partial)
        └── PokemonCatalog.Gen2.cs     # Gen 2+ Pokemon (partial)
```

## Key Classes

### PokemonSpeciesData

**Namespace**: `PokemonUltimate.Core.Blueprints`
**File**: `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs`
**Purpose**: Immutable blueprint for a Pokemon species
**Key Properties**:

-   `Name` - Unique identifier
-   `PokedexNumber` - National Pokedex number
-   `PrimaryType`, `SecondaryType` - Pokemon types
-   `BaseStats` - Base stat values
-   `Ability1`, `Ability2`, `HiddenAbility` - Abilities
-   `Learnset` - Moves this species can learn
-   `Evolutions` - Evolution paths

### PokemonInstance

**Namespace**: `PokemonUltimate.Core.Instances`
**File**: `PokemonUltimate.Core/Instances/PokemonInstance.cs` (+ partial classes)
**Purpose**: Mutable runtime instance of a Pokemon
**Key Properties**:

-   `Species` - Reference to `PokemonSpeciesData`
-   `Level` - Current level
-   `CurrentHP`, `MaxHP` - HP values
-   `Status` - Current status effects
-   `Moves` - Current move set
-   `Nature` - Nature modifier
-   `Gender` - Gender
-   `Experience` - Current experience points

**Partial Classes**:

-   `PokemonInstance.Core.cs` - Core instance data
-   `PokemonInstance.Battle.cs` - Battle state (stat stages, volatile status)
-   `PokemonInstance.LevelUp.cs` - Level-up and evolution logic
-   `PokemonInstance.Evolution.cs` - Evolution tracking

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

-   `Define(string name, int pokedexNumber)` - Start defining a Pokemon
-   `Type(PokemonType)` - Set mono-type
-   `Types(PokemonType, PokemonType)` - Set dual-type
-   `Stats(...)` - Set base stats
-   `Ability(AbilityData)` - Set abilities
-   `Learnset(...)` - Add learnable moves
-   `Evolves(...)` - Add evolution paths
-   `Build()` - Create the `PokemonSpeciesData`

### PokemonInstanceBuilder

**Namespace**: `PokemonUltimate.Core.Factories`
**File**: `PokemonUltimate.Core/Factories/PokemonInstanceBuilder.cs`
**Purpose**: Fluent builder for creating `PokemonInstance`
**Usage**:

```csharp
var pikachu = Pokemon.Create(PokemonCatalog.Pikachu, 25)
    .WithNature(Nature.Jolly)
    .Named("Sparky")
    .Build();
```

**Key Methods**:

-   `Create(PokemonSpeciesData species, int level)` - Start creating an instance
-   `WithNature(Nature)` - Set nature
-   `Named(string)` - Set nickname
-   `WithMoves(...)` - Set specific moves
-   `Build()` - Create the `PokemonInstance`

## Catalogs & Registries

### PokemonCatalog

**Namespace**: `PokemonUltimate.Content.Catalogs`
**File**: `PokemonUltimate.Content/Catalogs/Pokemon/PokemonCatalog.cs` (+ partial classes)
**Purpose**: Static catalog of all Pokemon species
**Structure**: Partial classes organized by generation

-   `PokemonCatalog.cs` - Main catalog class
-   `PokemonCatalog.Gen1.cs` - Gen 1 Pokemon (26 implemented)
-   `PokemonCatalog.Gen2.cs` - Gen 2+ Pokemon (planned)

**Usage**:

```csharp
var pikachu = PokemonCatalog.Pikachu;  // Returns PokemonSpeciesData
var allGen1 = PokemonCatalog.GetAllGen1();  // Returns all Gen 1 Pokemon
```

## Test Location

**Tests**: `PokemonUltimate.Tests/`

### By Sub-Feature

**1.1: Pokemon Data**:

-   **Blueprints**: `Tests/Blueprints/PokemonSpeciesDataTests.cs`
-   **Instances**: `Tests/Systems/Core/Instances/PokemonInstanceTests.cs`
-   **Builders**: `Tests/Data/Builders/PokemonBuilderTests.cs`
-   **Data**: `Tests/Data/Pokemon/[Pokemon]Tests.cs` (one file per Pokemon)
-   **Catalogs**: `Tests/Data/Catalogs/PokemonCatalogValidationTests.cs`

**1.2: Move Data**:

-   **Blueprints**: `Tests/Blueprints/MoveDataTests.cs`
-   **Instances**: `Tests/Systems/Core/Instances/MoveInstanceTests.cs`
-   **Effects**: `Tests/Systems/Effects/[Effect]Tests.cs`
-   **Builders**: `Tests/Data/Builders/MoveBuilderTests.cs`

**1.3-1.4: Ability & Item Data**:

-   **Blueprints**: `Tests/Blueprints/[Ability/Item]DataTests.cs`
-   **Builders**: `Tests/Data/Builders/[Ability/Item]BuilderTests.cs`

**1.5-1.10: Field & Status Data**:

-   **Blueprints**: `Tests/Blueprints/[Status/Weather/Terrain/etc]DataTests.cs`
-   **Builders**: `Tests/Data/Builders/[Status/Weather/etc]BuilderTests.cs`

**1.11: Evolution System**:

-   **Evolution**: `Tests/Systems/Core/Evolution/EvolutionTests.cs`
-   **Conditions**: `Tests/Systems/Core/Evolution/[Condition]Tests.cs`

**1.12: Type Effectiveness**:

-   **Type Effectiveness**: `Tests/Systems/Core/Factories/TypeEffectivenessTests.cs`

**1.13-1.17: Infrastructure**:

-   **Interfaces**: `Tests/Blueprints/IIdentifiableTests.cs`
-   **Enums**: `Tests/Data/Enums/[Enum]Tests.cs`
-   **Constants**: `Tests/Data/Constants/[Constants]Tests.cs`
-   **Builders**: `Tests/Data/Builders/[Builder]Tests.cs`
-   **Factories**: `Tests/Systems/Core/Factories/[Factory]Tests.cs`
-   **Registries**: `Tests/Systems/Core/Registry/[Registry]Tests.cs`

See **[Testing Strategy](testing.md)** for complete test organization by sub-feature.

## Related Documents

-   **[Architecture](architecture.md)** - Complete technical specification of all game data structures
-   **[Testing](testing.md)** - Comprehensive testing strategy for all sub-features
-   **[Roadmap](roadmap.md)** - Implementation plan organized by sub-features
-   **[Use Cases](use_cases.md)** - All scenarios and behaviors for game data
-   **[Sub-Features Overview](README.md)** - Complete list of all 19 sub-features organized by groups

---

**Last Updated**: 2025-01-XX
