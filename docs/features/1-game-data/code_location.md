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
-   **Grupo E: Planned Features** (1.18-1.19) - Variants System ✅ Complete, Pokedex Fields ✅ Complete
-   **Phase 4: Optional Enhancements** ✅ Complete - IVs/EVs System, Breeding System, Ownership/Tracking Fields
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

**Key Constants & Validators** (post-refactor):

-   `CoreConstants` - Core module constants (ShinyOdds, Friendship values, IV/EV limits, Stat stages, Formula constants)
-   `CoreValidators` - Centralized validation methods (ValidateLevel, ValidateFriendship, ValidateStatStage, ValidateIV, ValidateEV)
-   `ErrorMessages` - Error message constants
-   `GameMessages` - In-game message constants

**Key Extensions** (post-refactor):

-   `LevelExtensions` - Extension methods for level validation (`IsValidLevel()`)
-   `FriendshipExtensions` - Extension methods for friendship (`ClampFriendship()`)
-   `Weather`, `Terrain`, `HazardType`, `SideCondition`, `FieldEffect`
-   `EvolutionConditionType`

**Key Constants & Data Tables**:

-   `ErrorMessages` - Centralized error message strings
-   `GameMessages` - In-game message strings for UI
-   `NatureData` - Nature modifier tables (complements Nature enum)
-   **Enums in Effects**: `SemiInvulnerableState`, `FieldConditionType`, `MoveRestrictionType`, `ProtectionType`, `ContactPenalty`, `PriorityCondition`, `SelfDestructType`

**Key Classes**:

-   `CoreConstants` - Core module constants (ShinyOdds, Friendship values, IV/EV limits, Stat stages, Formula constants, post-refactor)
-   `CoreValidators` - Centralized validation methods (ValidateLevel, ValidateFriendship, ValidateStatStage, ValidateIV, ValidateEV, post-refactor)
-   `ErrorMessages` - Error message constants
-   `GameMessages` - In-game message constants
-   `LevelExtensions` - Extension methods for level validation (`IsValidLevel()`, post-refactor)
-   `FriendshipExtensions` - Extension methods for friendship (`ClampFriendship()`, post-refactor)

#### Builders (Moved to Feature 3.9)

**Sub-Feature**: **[3.9: Builders](../3-content-expansion/3.9-builders/)**  
**Namespace**: `PokemonUltimate.Content.Builders`  
**Note**: Builders moved to Feature 3 as they are primarily used for content creation

#### 1.16: Factories & Calculators

**Sub-Feature**: [1.16: Factories & Calculators](1.16-factories-calculators/)  
**Namespace**: `PokemonUltimate.Core.Factories`

**Key Classes**:

-   `IStatCalculator` - Stat calculation interface (post-refactor)
-   `StatCalculator` - Stat calculation formulas (Gen 3+, instance-based, post-refactor)
-   `ITypeEffectiveness` - Type effectiveness interface (post-refactor)
-   `TypeEffectiveness` - Type effectiveness calculator (instance-based, post-refactor)
-   `PokemonFactory` - Static factory for Pokemon creation
-   `PokemonInstanceBuilder` - Fluent builder for Pokemon instances (uses `IRandomProvider`, post-refactor)
-   `IMoveSelector` - Move selection interface (post-refactor)
-   `MoveSelector` - Move selection with strategies (post-refactor)
-   `MoveSelection/Strategies/` - Move selection strategies (RandomMoveStrategy, StabMoveStrategy, PowerMoveStrategy, OptimalMoveStrategy)

#### 1.17: Registry System

**Sub-Feature**: [1.17: Registry System](1.17-registry-system/)  
**Namespace**: `PokemonUltimate.Core.Registry`

**Key Classes**:

-   `IDataRegistry<T>` - Generic registry interface
-   `GameDataRegistry<T>` - Generic registry implementation
-   `IPokemonRegistry`, `IMoveRegistry` - Specialized interfaces
-   `PokemonRegistry`, `MoveRegistry` - Specialized implementations

### Grupo E: Planned Features (Sub-Features 1.18-1.19)

**Note**: Both 1.18 (Variants System) and 1.19 (Pokedex Fields) are now complete.

#### 1.18: Variants System ✅ COMPLETE

**Sub-Feature**: [1.18: Variants System](1.18-variants-system/) ✅ Complete  
**Namespace**: `PokemonUltimate.Core.Blueprints`, `PokemonUltimate.Core.Enums`, `PokemonUltimate.Core.Registry`, `PokemonUltimate.Content.Builders`

**Files**:

-   `PokemonUltimate.Core/Enums/PokemonVariantType.cs` - Variant type enum (Mega, Dinamax, Tera)
-   `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs` - Variant fields and computed properties
-   `PokemonUltimate.Core/Builders/PokemonBuilder.cs` - Variant builder methods
-   `PokemonUltimate.Core/Registry/PokemonRegistry.cs` - Variant query methods

**Fields** (in PokemonSpeciesData):

-   `BaseForm` - Reference to base Pokemon (null if base form)
-   `VariantType` - Type of variant (Mega, Dinamax, Tera, Regional, null if base form)
-   `TeraType` - Tera type for Terracristalización variants (null if not Tera variant)
-   `RegionalForm` - Regional identifier for Regional variants (e.g., "Alola", "Galar", "Hisui", empty if not Regional)
-   `Variants` - List of all variant forms

**Computed Properties**:

-   `IsVariant` - Returns true if this Pokemon is a variant form
-   `IsBaseForm` - Returns true if this Pokemon is a base form
-   `IsMegaVariant` - Returns true if this Pokemon is a Mega Evolution variant
-   `IsDinamaxVariant` - Returns true if this Pokemon is a Dinamax variant
-   `IsTeraVariant` - Returns true if this Pokemon is a Terracristalización variant
-   `IsRegionalVariant` - Returns true if this Pokemon is a Regional form variant
-   `HasGameplayChanges` - Returns true if variant has different stats/types/abilities from base form (detects purely visual variants)

**Builder Methods**:

-   `AsMegaVariant(PokemonSpeciesData baseForm, string variant = null)` - Mark as Mega variant
-   `AsDinamaxVariant(PokemonSpeciesData baseForm)` - Mark as Dinamax variant
-   `AsTeraVariant(PokemonSpeciesData baseForm, PokemonType teraType)` - Mark as Tera variant
-   `AsRegionalVariant(PokemonSpeciesData baseForm, string region)` - Mark as Regional variant
-   `AsCosmeticVariant(PokemonSpeciesData baseForm, string cosmeticFormIdentifier)` - Mark as Cosmetic variant

**Computed Properties** (on PokemonSpeciesData):

-   `HasVariants` - Returns true if Pokemon has any variant forms
-   `VariantCount` - Returns number of variant forms
-   `MegaVariants` - Gets all Mega Evolution variants
-   `DinamaxVariants` - Gets all Dinamax variants
-   `TeraVariants` - Gets all Terracristalización variants
-   `RegionalVariants` - Gets all Regional form variants
-   `CosmeticVariants` - Gets all Cosmetic variants
-   `VariantsWithGameplayChanges` - Gets variants with stat/type/ability changes
-   `VisualOnlyVariants` - Gets purely visual variants

**Registry Methods**:

-   `GetVariantsOf(PokemonSpeciesData baseForm)` - Get all variants of a base form
-   `GetMegaVariants()` - Get all Mega Evolution variants
-   `GetDinamaxVariants()` - Get all Dinamax variants
-   `GetTeraVariants()` - Get all Terracristalización variants
-   `GetRegionalVariants()` - Get all Regional form variants
-   `GetRegionalVariantsByRegion(string region)` - Get Regional variants from specific region
-   `GetVariantsWithGameplayChanges()` - Get variants with stat/type/ability changes
-   `GetVisualOnlyVariants()` - Get purely visual variants (no gameplay changes)
-   `GetBaseForms()` - Get all base forms (non-variant Pokemon)

**VariantProvider** (`PokemonUltimate.Content/Providers/VariantProvider.cs`):

-   `GetVariants(PokemonSpeciesData baseForm)` - Get all variants for a Pokemon
-   `GetVariants(string basePokemonName)` - Get all variants by Pokemon name
-   `HasVariants(PokemonSpeciesData baseForm)` - Check if Pokemon has variants
-   `GetMegaVariants(PokemonSpeciesData baseForm)` - Get Mega variants for a Pokemon
-   `GetRegionalVariants(PokemonSpeciesData baseForm)` - Get Regional variants for a Pokemon
-   `GetTeraVariants(PokemonSpeciesData baseForm)` - Get Tera variants for a Pokemon

**Extension Methods** (`PokemonUltimate.Content/Extensions/PokemonSpeciesDataExtensions.cs`):

-   `GetVariants(this PokemonSpeciesData pokemon)` - Extension method to get variants
-   `HasVariantsAvailable(this PokemonSpeciesData pokemon)` - Extension method to check for variants

**Robustness Features**:

-   ✅ Supports variants with gameplay changes (Mega, Dinamax, Tera, Regional with changes)
-   ✅ Supports purely visual variants (Cosmetic variants with same stats/types/abilities)
-   ✅ Automatic detection of gameplay changes via `HasGameplayChanges` property
-   ✅ Flexible regional form system (Alola, Galar, Hisui, Paldea, etc.)
-   ✅ Centralized variant management via VariantProvider
-   ✅ Easy querying via computed properties and extension methods

**Tests**: 63+ tests passing (functional + edge cases + VariantProvider tests + registry tests)

---

#### Phase 4: Optional Enhancements ✅ COMPLETE

**Sub-Feature**: 1.1: Pokemon Data (extensions)  
**Status**: ✅ **Complete** - All Phase 4 optional enhancements implemented

**Implemented Features**:

**1. IVs/EVs System** ✅ Complete:
-   **Files**: `PokemonUltimate.Core/Instances/IVSet.cs`, `PokemonUltimate.Core/Instances/EVSet.cs`
-   **Properties**: `PokemonInstance.IVs` (IVSet), `PokemonInstance.EVs` (EVSet)
-   **Integration**: `StatCalculator` uses IVs/EVs for stat calculation
-   **Default Behavior**: Random IVs (0-31), Maximum EVs (252/252/4/0/0/0)
-   **Builder Methods**: `.WithPerfectIVs()`, `.WithZeroIVs()`, `.WithIVs(...)`, `.WithMaximumEVs()`, `.WithZeroEVs()`, `.WithEVs(...)`

**2. Breeding System** ✅ Complete:
-   **Files**: `PokemonUltimate.Core/Enums/EggGroup.cs`
-   **Properties**: `PokemonSpeciesData.EggGroups` (List<EggGroup>), `PokemonSpeciesData.EggCycles` (int, default: 20)
-   **Methods**: `CanBreedWith(PokemonSpeciesData other)`, `IsInEggGroup(EggGroup eggGroup)`, `CannotBreed` (property)
-   **Egg Groups**: Monster, Water1, Bug, Flying, Field, Fairy, Grass, Human-Like, Mineral, Amorphous, Dragon, Ditto, Undiscovered
-   **Note**: IV inheritance and egg move inheritance are future enhancements

**3. Ownership/Tracking Fields** ✅ Complete:
-   **Properties**: `PokemonInstance.OriginalTrainer` (string?), `PokemonInstance.TrainerId` (int?), `PokemonInstance.MetLevel` (int?), `PokemonInstance.MetLocation` (string?), `PokemonInstance.MetDate` (DateTime?)
-   **Usage**: All fields are nullable and can be set/get for tracking Pokemon origin and ownership

**See**: [`roadmap.md`](roadmap.md#phase-4-optional-enhancements-low-priority) for complete implementation details.

---

#### 1.19: Pokedex Fields ✅ COMPLETE

**Sub-Feature**: [1.19: Pokedex Fields](1.19-pokedex-fields/) ✅ Complete  
**Namespace**: `PokemonUltimate.Core.Blueprints`, `PokemonUltimate.Content.Providers`, `PokemonUltimate.Content.Extensions`, `PokemonUltimate.Core.Enums`

**Files**:

-   `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs` - Fields in PokemonSpeciesData
-   `PokemonUltimate.Content/Providers/PokedexDataProvider.cs` - Centralized Pokedex data provider
-   `PokemonUltimate.Content/Providers/PokedexData.cs` - Pokedex data structure
-   `PokemonUltimate.Content/Extensions/PokemonSpeciesDataExtensions.cs` - Extension method `WithPokedexData()`
-   `PokemonUltimate.Core/Enums/PokemonColor.cs` - Color enum
-   `PokemonUltimate.Core/Enums/PokemonShape.cs` - Shape enum
-   `PokemonUltimate.Core/Enums/PokemonHabitat.cs` - Habitat enum

**Fields** (in PokemonSpeciesData):

-   `Description` - Pokedex entry text
-   `Category` - Classification (e.g., "Seed Pokemon", "Flame Pokemon")
-   `Height` - Height in meters
-   `Weight` - Weight in kilograms
-   `Color` - Pokedex color category (enum)
-   `Shape` - Body shape category (enum)
-   `Habitat` - Preferred habitat/biome (enum)

**Usage**:

```csharp
// Apply Pokedex data automatically
var pikachu = Pokemon.Define("Pikachu", 25)
    .Type(PokemonType.Electric)
    .Stats(35, 55, 40, 50, 50, 90)
    .Build()
    .WithPokedexData(); // Applies data from PokedexDataProvider
```

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
│   ├── PokemonSpeciesData.cs          # Species blueprint class
│   ├── BaseStats.cs                   # Base stats (uses StatGetterRegistry, post-refactor)
│   └── Strategies/                    # Strategy Pattern for stat getters (post-refactor)
│       ├── IStatGetterStrategy.cs
│       ├── StatGetterRegistry.cs
│       └── [Stat]StatGetterStrategy.cs
├── Instances/
│   ├── PokemonInstance.cs             # Main instance class (uses StatsCache, post-refactor)
│   ├── PokemonInstance.Core.cs        # Core data (partial)
│   ├── PokemonInstance.Battle.cs      # Battle state (partial, uses StatStageManager, post-refactor)
│   ├── PokemonInstance.LevelUp.cs    # Level-up logic (partial, uses StatsCache, post-refactor)
│   ├── PokemonInstance.Evolution.cs  # Evolution tracking (partial)
│   ├── StatsCache.cs                  # Stats caching system (post-refactor)
│   └── Strategies/                    # Strategy Pattern for PokemonInstance stat getters (post-refactor)
│       ├── IPokemonStatGetterStrategy.cs
│       ├── PokemonStatGetterRegistry.cs
│       └── [Stat]StatGetterStrategy.cs
├── Factories/
│   ├── IStatCalculator.cs             # Stat calculator interface (post-refactor)
│   ├── StatCalculator.cs              # Stat calculation (instance-based, post-refactor)
│   ├── ITypeEffectiveness.cs          # Type effectiveness interface (post-refactor)
│   ├── TypeEffectiveness.cs           # Type effectiveness (instance-based, post-refactor)
│   ├── PokemonFactory.cs              # Static factory wrapper
│   ├── PokemonInstanceBuilder.cs     # Instance builder (uses IRandomProvider, MoveSelector, post-refactor)
│   ├── MoveSelection/                 # Move selection system (post-refactor)
│   │   ├── IMoveSelector.cs
│   │   ├── MoveSelector.cs
│   │   └── Strategies/
│   │       ├── IMoveSelectionStrategy.cs
│   │       ├── RandomMoveStrategy.cs
│   │       ├── StabMoveStrategy.cs
│   │       ├── PowerMoveStrategy.cs
│   │       └── OptimalMoveStrategy.cs
│   └── Strategies/                    # Strategy Pattern for nature boosting (post-refactor)
│       └── NatureBoosting/
│           ├── INatureBoostingStrategy.cs
│           ├── NatureBoostingRegistry.cs
│           └── [Stat]NatureBoostingStrategy.cs
├── Managers/
│   ├── IStatStageManager.cs           # Stat stage manager interface (post-refactor)
│   └── StatStageManager.cs            # Stat stage manager (post-refactor)
├── Providers/
│   ├── IRandomProvider.cs             # Random provider interface (post-refactor)
│   └── RandomProvider.cs              # Random provider implementation (post-refactor)
├── Constants/
│   ├── CoreConstants.cs               # Core module constants (post-refactor)
│   ├── CoreValidators.cs              # Centralized validators (post-refactor)
│   ├── ErrorMessages.cs               # Error messages
│   └── GameMessages.cs                # Game messages
├── Extensions/
│   ├── LevelExtensions.cs             # Level validation extensions (post-refactor)
│   └── FriendshipExtensions.cs        # Friendship extensions (post-refactor)
├── Effects/
│   └── Strategies/                    # Strategy Pattern for effect descriptions (post-refactor)
│       └── [Effect]DescriptionStrategy.cs
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
**Purpose**: Fluent builder for creating `PokemonInstance` (refactored with DI and Strategy Pattern, post-refactor)
**Usage**:

```csharp
var randomProvider = new RandomProvider();
var pikachu = Pokemon.Create(PokemonCatalog.Pikachu, 25, randomProvider)
    .WithNature(Nature.Jolly)
    .Named("Sparky")
    .Build();
```

**Key Methods**:

-   `Create(PokemonSpeciesData species, int level, IRandomProvider randomProvider)` - Start creating an instance (requires IRandomProvider, post-refactor)
-   `WithNature(Nature)` - Set nature
-   `Named(string)` - Set nickname
-   `WithMoves(...)` - Set specific moves (uses MoveSelector internally, post-refactor)
-   `Build()` - Create the `PokemonInstance` (includes validation, post-refactor)

**Post-Refactor Improvements**:

-   Uses `IRandomProvider` instead of static `Random` for testability
-   Uses `CoreConstants` instead of magic numbers
-   Uses `MoveSelector` with strategies for move selection
-   Uses `CoreValidators` for validation
-   Methods extracted for better SRP (CalculateStats, DetermineShiny, DetermineAbility, etc.)

### StatCalculator

**Namespace**: `PokemonUltimate.Core.Factories`
**File**: `PokemonUltimate.Core/Factories/StatCalculator.cs`
**Purpose**: Calculates Pokemon stats using official formulas (instance-based, post-refactor)
**Interface**: `IStatCalculator` (post-refactor)
**Usage**:

```csharp
var calculator = new StatCalculator();
var hp = calculator.CalculateHP(baseHP, level, iv, ev);
var attack = calculator.CalculateStat(baseAttack, level, nature, Stat.Attack, iv, ev);
```

**Post-Refactor Improvements**:

-   Converted from static class to instance-based with `IStatCalculator` interface
-   Uses `CoreConstants` for formula constants
-   Uses `CoreValidators` for validation
-   Static methods maintained as wrappers for backward compatibility

### TypeEffectiveness

**Namespace**: `PokemonUltimate.Core.Factories`
**File**: `PokemonUltimate.Core/Factories/TypeEffectiveness.cs`
**Purpose**: Calculates type effectiveness for damage calculations (instance-based, post-refactor)
**Interface**: `ITypeEffectiveness` (post-refactor)
**Usage**:

```csharp
var typeEffectiveness = new TypeEffectiveness();
var multiplier = typeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass);
var stab = typeEffectiveness.GetSTABMultiplier(PokemonType.Fire, PokemonType.Fire, null);
```

**Post-Refactor Improvements**:

-   Converted from static class to instance-based with `ITypeEffectiveness` interface
-   Static methods maintained as wrappers for backward compatibility

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

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
