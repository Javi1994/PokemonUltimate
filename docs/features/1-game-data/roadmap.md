# Feature 1: Game Data - Roadmap

> Comprehensive roadmap of all game data structures (blueprints) and supporting systems organized by sub-features.

**Feature Number**: 1  
**Feature Name**: Game Data (formerly "Pokemon Data")  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document defines the implementation roadmap for **all game data structures** and supporting systems, organized into logical groups:

-   **Core Entity Data** (1.1-1.4): Pokemon, Moves, Abilities, Items
-   **Field & Status Data** (1.5-1.10): Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects
-   **Supporting Systems** (1.11-1.12): Evolution System, Type Effectiveness Table
-   **Infrastructure** (1.13-1.14, 1.16-1.17): Interfaces, Enums, Constants, Factories, Registries (Note: Builders moved to Feature 3.9, was 1.15)
-   **Planned Features** (1.18-1.19): Variants System ✅ Complete, Pokedex Fields ✅ Complete
-   **Phase 4: Optional Enhancements**: IVs/EVs System, Breeding System, Ownership/Tracking Fields ⏳ Pending

**Goal**: Ensure all data structures are complete and well-organized before expanding content, preventing future refactoring.

---

## Current Status Summary

| Group                            | Sub-Features         | Status           | Completion |
| -------------------------------- | -------------------- | ---------------- | ---------- |
| **Grupo A: Core Entity Data**    | 1.1-1.4              | ✅ Core Complete | 100%       |
| **Grupo B: Field & Status Data** | 1.5-1.10             | ✅ Core Complete | 100%       |
| **Grupo C: Supporting Systems**  | 1.11-1.12            | ✅ Core Complete | 100%       |
| **Grupo D: Infrastructure**      | 1.13-1.14, 1.16-1.17 | ✅ Core Complete | 100%       |
| **Grupo E: Planned Features**    | 1.18-1.19            | ✅ Complete      | 100%       |
| **Phase 4: Optional Extensions** | 1.1 (extensions)     | ⏳ Planned       | 0%         |

---

## Sub-Feature Status by Group

### Grupo A: Core Entity Data (Entidades Principales)

#### 1.1: Pokemon Data ✅ CORE COMPLETE

**Status**: Core structure complete, some fields planned  
**Components**: PokemonSpeciesData, PokemonInstance, BaseStats, LearnableMove

**✅ Implemented**:

-   Core blueprint structure (Name, PokedexNumber, Types, BaseStats)
-   Instance structure (Level, HP, Stats, Moves, Status)
-   Abilities integration (Ability1, Ability2, HiddenAbility)
-   Learnset system (LearnableMove, LearnMethod)
-   Evolution integration

**⏳ Planned Fields** (See Phase 1-3 below):

-   BaseExperienceYield, CatchRate, BaseFriendship, GrowthRate
-   Pokedex fields (Description, Category, Height, Weight, Color, Shape, Habitat)
-   Variants system fields (BaseForm, VariantType, TeraType, Variants)

**Related**: [Sub-Feature 1.1 Documentation](1.1-pokemon-data/)

---

#### 1.2: Move Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: MoveData, MoveInstance, Move Effects (22 implementations)

**✅ Implemented**:

-   MoveData blueprint (Type, Category, Power, Accuracy, PP, Priority, Target)
-   MoveInstance runtime (PP tracking, PP Ups)
-   22 Move Effect classes (DamageEffect, StatusEffect, StatChangeEffect, etc.)
-   Composition pattern for move behavior

**Related**: [Sub-Feature 1.2 Documentation](1.2-move-data/)

---

#### 1.3: Ability Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: AbilityData

**✅ Implemented**:

-   AbilityData blueprint (Triggers, Effects, Modifiers)
-   AbilityBuilder fluent API
-   Ability catalog (35 abilities)

**Related**: [Sub-Feature 1.3 Documentation](1.3-ability-data/)

---

#### 1.4: Item Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: ItemData

**✅ Implemented**:

-   ItemData blueprint (Category, Triggers, Effects)
-   ItemBuilder fluent API
-   Item catalog (23 items: held items + berries)

**Related**: [Sub-Feature 1.4 Documentation](1.4-item-data/)

---

### Grupo B: Field & Status Data (Condiciones de Campo)

#### 1.5: Status Effect Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: StatusEffectData

**✅ Implemented**:

-   StatusEffectData blueprint (Persistent and Volatile statuses)
-   StatusEffectBuilder fluent API
-   Status catalog (15 statuses: 6 persistent + 9 volatile)

**Related**: [Sub-Feature 1.5 Documentation](1.5-status-effect-data/)

---

#### 1.6: Weather Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: WeatherData

**✅ Implemented**:

-   WeatherData blueprint (Weather conditions: Rain, Sun, Hail, Sandstorm, Fog, Primal weathers)
-   WeatherBuilder fluent API

**Related**: [Sub-Feature 1.6 Documentation](1.6-weather-data/)

---

#### 1.7: Terrain Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: TerrainData

**✅ Implemented**:

-   TerrainData blueprint (Terrain conditions: Grassy, Electric, Psychic, Misty)
-   TerrainBuilder fluent API

**Related**: [Sub-Feature 1.7 Documentation](1.7-terrain-data/)

---

#### 1.8: Hazard Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: HazardData

**✅ Implemented**:

-   HazardData blueprint (Entry hazards: Stealth Rock, Spikes, Toxic Spikes, Sticky Web)
-   HazardBuilder fluent API

**Related**: [Sub-Feature 1.8 Documentation](1.8-hazard-data/)

---

#### 1.9: Side Condition Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: SideConditionData

**✅ Implemented**:

-   SideConditionData blueprint (Screens, Tailwind, Aurora Veil, etc.)
-   SideConditionBuilder fluent API

**Related**: [Sub-Feature 1.9 Documentation](1.9-side-condition-data/)

---

#### 1.10: Field Effect Data ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: FieldEffectData

**✅ Implemented**:

-   FieldEffectData blueprint (Rooms, Gravity, etc.)
-   FieldEffectBuilder fluent API

**Related**: [Sub-Feature 1.10 Documentation](1.10-field-effect-data/)

---

### Grupo C: Supporting Systems (Sistemas de Soporte)

#### 1.11: Evolution System ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: Evolution, IEvolutionCondition, EvolutionConditions (6 classes)

**✅ Implemented**:

-   Evolution class (Target, Conditions)
-   IEvolutionCondition interface
-   6 Evolution Condition classes (Level, Item, Trade, Friendship, TimeOfDay, KnowsMove)
-   EvolutionBuilder fluent API

**Related**: [Sub-Feature 1.11 Documentation](1.11-evolution-system/)

---

#### 1.12: Type Effectiveness Table ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: TypeEffectiveness

**✅ Implemented**:

-   TypeEffectiveness static class
-   Gen 6+ type chart (18 types, Fairy included)
-   Single/dual type effectiveness calculation
-   STAB calculation (1.5x multiplier)
-   Immunity handling

**Related**: [Sub-Feature 1.12 Documentation](1.12-type-effectiveness-table/)

---

### Grupo D: Infrastructure (Infraestructura)

#### 1.13: Interfaces Base ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: IIdentifiable

**✅ Implemented**:

-   IIdentifiable interface (Id property)

**Related**: [Sub-Feature 1.13 Documentation](1.13-interfaces-base/)

---

#### 1.14: Enums & Constants ✅ CORE COMPLETE

**Status**: Core structure complete, refactored (2024-12-XX)  
**Components**: Enums (20 main + 7 in Effects), ErrorMessages, GameMessages, CoreConstants, CoreValidators, Extensions

**✅ Implemented**:

-   20 main enums (PokemonType, Stat, Nature, Gender, MoveCategory, etc.)
-   7 enums in Effects namespace
-   ErrorMessages static class
-   GameMessages static class
-   **CoreConstants** (post-refactor): ShinyOdds, Friendship values, IV/EV limits, Stat stages, Formula constants
-   **CoreValidators** (post-refactor): Centralized validation methods (ValidateLevel, ValidateFriendship, ValidateStatStage, ValidateIV, ValidateEV)
-   **Extensions** (post-refactor): LevelExtensions, FriendshipExtensions

**Related**: [Sub-Feature 1.14 Documentation](1.14-enums-constants/)

---

#### Builders (Moved to Feature 3.9)

**Status**: ✅ Moved to **[Feature 3.9: Builders](../3-content-expansion/3.9-builders/)**  
**Note**: Builders are primarily used for content creation, so they belong in Feature 3

---

#### 1.16: Factories & Calculators ✅ CORE COMPLETE

**Status**: Core structure complete, refactored (2024-12-XX)  
**Components**: IStatCalculator/StatCalculator, ITypeEffectiveness/TypeEffectiveness, PokemonFactory, PokemonInstanceBuilder, IMoveSelector/MoveSelector

**✅ Implemented**:

-   **IStatCalculator** / **StatCalculator** (Gen 3+ formulas with IVs, EVs, Nature, instance-based with interface, post-refactor)
-   **ITypeEffectiveness** / **TypeEffectiveness** (Type effectiveness calculator, instance-based with interface, post-refactor)
-   PokemonFactory (Quick instance creation)
-   **PokemonInstanceBuilder** (Fluent API for instances, uses IRandomProvider, MoveSelector, post-refactor)
-   **IMoveSelector** / **MoveSelector** (Move selection system with strategies, post-refactor)

**Post-Refactor Improvements**:

-   Dependency Injection implemented (IRandomProvider, IStatCalculator, ITypeEffectiveness)
-   Strategy Pattern for move selection (RandomMoveStrategy, StabMoveStrategy, PowerMoveStrategy, OptimalMoveStrategy)
-   Constants centralization (CoreConstants)
-   Validation centralization (CoreValidators)
-   Improved testability and maintainability

**Related**: [Sub-Feature 1.16 Documentation](1.16-factories-calculators/)

---

#### 1.17: Registry System ✅ CORE COMPLETE

**Status**: Core structure complete  
**Components**: IDataRegistry<T>, GameDataRegistry<T>, PokemonRegistry, MoveRegistry

**✅ Implemented**:

-   IDataRegistry<T> generic interface
-   GameDataRegistry<T> generic implementation
-   PokemonRegistry (specialized with Pokedex queries)
-   MoveRegistry (specialized with type/power queries)

**Related**: [Sub-Feature 1.17 Documentation](1.17-registry-system/)

---

### Grupo E: Planned Features

#### 1.18: Variants System ✅ COMPLETE

**Status**: Complete  
**Components**: Mega/Dinamax/Terracristalización as separate species

**✅ Implemented**:

-   PokemonVariantType enum (Mega, Dinamax, Tera)
-   BaseForm, VariantType, TeraType, Variants fields in PokemonSpeciesData
-   Computed properties (IsVariant, IsBaseForm, IsMegaVariant, IsDinamaxVariant, IsTeraVariant)
-   Builder methods (AsMegaVariant, AsDinamaxVariant, AsTeraVariant)
-   Registry query methods (GetVariantsOf, GetMegaVariants, GetDinamaxVariants, GetTeraVariants, GetBaseForms)
-   Variant relationship validation and automatic linking
-   25 tests passing (functional + edge cases)

**Related**: [Sub-Feature 1.18 Documentation](1.18-variants-system/) | [Architecture](1.18-variants-system/architecture.md)

---

#### 1.19: Pokedex Fields ✅ COMPLETE

**Status**: Complete  
**Components**: Description, Category, Height, Weight, Color, Shape, Habitat

**✅ Implemented**:

-   Description field (Pokedex entry text)
-   Category field (Classification)
-   Height and Weight fields (Physical measurements)
-   Color, Shape, Habitat enums and fields
-   PokedexDataProvider system for centralized data management
-   Extension method `WithPokedexData()` for easy application

**Related**: [Sub-Feature 1.19 Documentation](1.19-pokedex-fields/)

---

## Implementation Phases

### Phase 1: Critical Pokemon Fields (HIGH Priority) ✅ COMPLETE

**Goal**: Add fields required for immediate future features (Post-Battle Rewards, Catching).

**Sub-Feature**: 1.1: Pokemon Data  
**Status**: ✅ Complete

#### 1.1 BaseExperienceYield

**Purpose**: Base EXP value used in Gen 3+ EXP formula:

```
EXP = (BaseExp * Level * WildMultiplier) / (7 * Participants)
```

**Implementation**:

```csharp
// In PokemonSpeciesData.cs
/// <summary>
/// Base experience yield when this Pokemon is defeated.
/// Used in Gen 3+ EXP formula: (BaseExp * Level * WildMultiplier) / (7 * Participants)
/// Typical range: 50-300 (legendaries can be 300+)
/// </summary>
public int BaseExperienceYield { get; set; } = 0;
```

**Builder Support**: Add `.BaseExp(int)` method to PokemonBuilder  
**Default Values**: Use official Pokemon data (e.g., Pikachu = 112, Charizard = 240)  
**Tests**: Verify EXP calculation uses this value correctly  
**Required For**: Feature 5.1 (Post-Battle Rewards)

---

#### 1.2 CatchRate

**Purpose**: Determines catch probability in Gen 3+ catch formula.

**Implementation**:

```csharp
// In PokemonSpeciesData.cs
/// <summary>
/// Catch rate (3-255). Lower values = harder to catch.
/// Typical values: 45 (common), 3 (legendaries), 255 (guaranteed catch)
/// </summary>
public int CatchRate { get; set; } = 45;
```

**Builder Support**: Add `.CatchRate(int)` method to PokemonBuilder  
**Default Values**: Use official Pokemon data (most = 45, legendaries = 3)  
**Tests**: Verify catch rate calculation uses this value correctly  
**Required For**: Feature 5.2 (Catching System)

---

#### 1.3 BaseFriendship

**Purpose**: Initial friendship value (some Pokemon start with different values).

**Implementation**:

```csharp
// In PokemonSpeciesData.cs
/// <summary>
/// Base friendship value (0-255) when Pokemon is first obtained.
/// Default: 70 (wild Pokemon), 120 (hatched from egg), 0 (some legendaries)
/// </summary>
public int BaseFriendship { get; set; } = 70;
```

**Builder Support**: Add `.BaseFriendship(int)` method to PokemonBuilder (Sub-Feature 3.9)  
**Update PokemonInstance**: Use `Species.BaseFriendship` instead of hardcoded `70`  
**Default Values**: Most = 70, Starters/Hatched = 120, Some legendaries = 0  
**Tests**: Verify PokemonInstance uses BaseFriendship correctly  
**Required For**: Evolution system, Friendship features

---

#### 1.4 GrowthRate

**Purpose**: Different EXP curves for different Pokemon (Fast, Medium Fast, Medium Slow, Slow, Erratic, Fluctuating).

**Implementation**:

```csharp
// New enum: PokemonUltimate.Core/Enums/GrowthRate.cs
public enum GrowthRate
{
    Fast,           // Level^3 * 0.8
    MediumFast,     // Level^3 (default, most Pokemon)
    MediumSlow,     // Complex formula
    Slow,           // Level^3 * 1.25
    Erratic,        // Complex formula
    Fluctuating     // Complex formula
}

// In PokemonSpeciesData.cs
public GrowthRate GrowthRate { get; set; } = GrowthRate.MediumFast;
```

**Builder Support**: Add `.GrowthRate(GrowthRate)` method to PokemonBuilder  
**Update StatCalculator**: Add methods for each growth rate formula (Sub-Feature 1.16)  
**Default Values**: Most Pokemon use `MediumFast`  
**Tests**: Verify EXP calculation uses correct growth rate  
**Required For**: Different EXP curves per Pokemon

---

### Phase 2: Pokedex Fields (MEDIUM Priority) ✅ COMPLETE

**Goal**: Add fields required for Pokedex display.

**Sub-Feature**: 1.19: Pokedex Fields  
**Status**: ✅ Complete

#### 2.1 Description and Category

**Purpose**: Text content for Pokedex entries.

**Implementation**:

```csharp
// In PokemonSpeciesData.cs
public string Description { get; set; } = string.Empty;  // Pokedex entry text
public string Category { get; set; } = string.Empty;     // Classification (e.g., "Flame Pokemon")
```

**Builder Support**: Add `.Description(string)` and `.Category(string)` methods (Sub-Feature 3.9)  
**Default Values**: Use official Pokemon data from Bulbapedia

---

#### 2.2 Physical Attributes (Height, Weight)

**Purpose**: Physical measurements for Pokedex display.

**Implementation**:

```csharp
// In PokemonSpeciesData.cs
public float Height { get; set; } = 0f;   // Height in meters
public float Weight { get; set; } = 0f;   // Weight in kilograms
```

**Builder Support**: Add `.Height(float)` and `.Weight(float)` methods (Sub-Feature 3.9)  
**Default Values**: Use official Pokemon data

---

#### 2.3 Classification Fields (Color, Shape, Habitat)

**Purpose**: Categorization for Pokedex filtering and display.

**Implementation**:

```csharp
// New enums: PokemonUltimate.Core/Enums/
public enum PokemonColor { Unknown, Red, Orange, Yellow, Green, Blue, Indigo, Violet, Pink, Brown, Black, Gray, White }
public enum PokemonShape { Unknown, Ball, Squiggle, Fish, Arms, Blob, Upright, Legs, Quadruped, Wings, Tentacles, Heads, Humanoid, BugWings, Armor }
public enum PokemonHabitat { Unknown, Cave, Forest, Grassland, Mountain, Rare, RoughTerrain, Sea, Urban, WatersEdge }

// In PokemonSpeciesData.cs
public PokemonColor Color { get; set; } = PokemonColor.Unknown;
public PokemonShape Shape { get; set; } = PokemonShape.Unknown;
public PokemonHabitat Habitat { get; set; } = PokemonHabitat.Unknown;
```

**Builder Support**: Add `.Color()`, `.Shape()`, `.Habitat()` methods (Sub-Feature 3.9)  
**Default Values**: Use official Pokemon data

---

### Phase 3: Variants System (MEDIUM Priority) ✅ COMPLETE

**Goal**: Implement Mega Evolutions, Dinamax, and Terracristalización as separate species.

**Sub-Feature**: 1.18: Variants System  
**Status**: ✅ **COMPLETE**

**See**: [`1.18-variants-system/architecture.md`](1.18-variants-system/architecture.md) for complete specification.

#### Implementation Summary

**Purpose**: Support for variant forms as separate Pokemon species (Mega, Dinamax, Tera, Regional, Cosmetic).

**Implementation**:

```csharp
// In PokemonSpeciesData.cs
public PokemonSpeciesData BaseForm { get; set; }  // Reference to base Pokemon (null if base form)
public PokemonVariantType? VariantType { get; set; }  // Mega, Dinamax, Tera, Regional, Cosmetic
public PokemonType? TeraType { get; set; }  // Tera type for Terracristalización variants
public string RegionalForm { get; set; }  // Regional identifier for Regional/Cosmetic variants
public List<PokemonSpeciesData> Variants { get; set; } = new List<PokemonSpeciesData>();  // All variant forms

// Helper properties
public bool IsVariant => BaseForm != null;
public bool IsBaseForm => BaseForm == null;
public bool HasVariants => Variants.Count > 0;
public IEnumerable<PokemonSpeciesData> MegaVariants => Variants.Where(v => v.IsMegaVariant);
// ... more computed properties
```

**Builder Support**: ✅ `.AsMegaVariant()`, `.AsDinamaxVariant()`, `.AsTeraVariant()`, `.AsRegionalVariant()`, `.AsCosmeticVariant()`  
**VariantProvider**: ✅ Centralized variant management (`PokemonUltimate.Content/Providers/VariantProvider.cs`)  
**Extension Methods**: ✅ `.GetVariants()`, `.HasVariantsAvailable()`  
**Registry Methods**: ✅ `GetVariantsOf()`, `GetMegaVariants()`, `GetRegionalVariants()`, etc.  
**Tests**: ✅ 63+ tests passing (functional + edge cases + VariantProvider tests)  
**Related**: [Sub-Feature 1.18 Architecture](1.18-variants-system/architecture.md)

---

### Phase 4: Optional Enhancements (LOW Priority)

**Sub-Feature**: 1.1: Pokemon Data (extensions)

#### 4.1 IVs and EVs (For Competitive/Breeding)

**Purpose**: Individual Values and Effort Values for stat variation.

**Implementation**: Create `IVSet` and `EVSet` classes, add to `PokemonInstance`.

**Note**: Current implementation uses max IVs/EVs (roguelike). This is for future competitive features.

---

#### 4.2 Breeding Fields

**Purpose**: Breeding compatibility and egg hatching.

**Implementation**: Add `EggGroups` and `EggCycles` to `PokemonSpeciesData`, create `EggGroup` enum.

---

#### 4.3 Ownership Fields

**Purpose**: Track Pokemon origin and ownership.

**Implementation**: Add to `PokemonInstance`:

-   `OriginalTrainer` (string?)
-   `TrainerId` (int?)
-   `MetLevel` (int?)
-   `MetLocation` (string?)
-   `MetDate` (DateTime?)

---

## Implementation Checklist

### Phase 1: Critical Fields (HIGH Priority) ✅ COMPLETE

**Sub-Feature 1.1: Pokemon Data**

-   [x] **1.1.1**: `BaseExperienceYield` added to `PokemonSpeciesData` ✅
-   [x] **1.1.2**: `CatchRate` added to `PokemonSpeciesData` ✅
-   [x] **1.1.3**: `BaseFriendship` added to `PokemonSpeciesData` and used in `PokemonInstance` ✅
-   [x] **1.1.4**: `GrowthRate` enum created and added to `PokemonSpeciesData` ✅
-   [x] Builder methods added for all new fields ✅
-   [x] Default values set for example Pokemon (Pikachu, Bulbasaur, Charmander) ✅
-   [x] Tests written for new fields ✅
-   [x] Documentation updated ✅

---

### Phase 2: Pokedex Fields (MEDIUM Priority) ✅ COMPLETE

**Sub-Feature 1.19: Pokedex Fields**

-   [x] **2.1**: `Description` and `Category` added to `PokemonSpeciesData` ✅
-   [x] **2.2**: `Height` and `Weight` added to `PokemonSpeciesData` ✅
-   [x] **2.3**: `Color`, `Shape`, `Habitat` enums created and added to `PokemonSpeciesData` ✅
-   [x] Builder methods added for all new fields ✅
-   [x] Default values set for example Pokemon ✅
-   [x] Tests written for new fields ✅
-   [x] Documentation updated ✅

---

### Phase 3: Variants System (MEDIUM Priority)

**Sub-Feature 1.18: Variants System**

-   [ ] **3.1**: `BaseForm`, `VariantType`, `TeraType`, `Variants` fields added to `PokemonSpeciesData`
-   [ ] `PokemonVariantType` enum created
-   [x] Builder methods added (`.AsMegaVariant()`, `.AsDinamaxVariant()`, `.AsTeraVariant()`, `.AsRegionalVariant()`, `.AsCosmeticVariant()`) ✅
-   [x] Variant relationship validation implemented ✅
-   [x] VariantProvider implemented ✅
-   [x] Extension methods implemented ✅
-   [x] Tests written for variants system (63+ tests) ✅
-   [x] Documentation updated (see [1.18-variants-system/architecture.md](1.18-variants-system/architecture.md)) ✅

---

### Phase 4: Optional Enhancements (LOW Priority) ⏳ PENDING

**Sub-Feature 1.1: Pokemon Data (extensions)**

**Status**: ⏳ **Not Implemented** - These are optional extensions for advanced features (competitive play, breeding, ownership tracking).

**Pending Items**:

-   [ ] **IVs/EVs System** - Individual Values and Effort Values tracking per Pokemon instance
    -   Create `IVSet` class to store IVs per stat (HP, Attack, Defense, SpAttack, SpDefense, Speed)
    -   Create `EVSet` class to store EVs per stat (0-252 per stat, 510 total)
    -   Add `IVs` and `EVs` properties to `PokemonInstance`
    -   Update `StatCalculator` to use stored IVs/EVs instead of defaults
    -   Add methods to gain EVs from battles
    -   **Note**: Currently uses max IVs/EVs (31/252) by default for roguelike experience

-   [ ] **Breeding System** - Breeding compatibility and egg hatching
    -   Create `EggGroup` enum (Monster, Water1, Bug, Flying, Field, Fairy, Grass, Human-Like, Mineral, Amorphous, Dragon, Ditto, Undiscovered)
    -   Add `EggGroups` property to `PokemonSpeciesData` (list of EggGroup)
    -   Add `EggCycles` property to `PokemonSpeciesData` (cycles to hatch egg)
    -   Implement breeding compatibility logic
    -   Implement IV inheritance from parents
    -   Implement egg move inheritance
    -   **Note**: Currently only `BaseFriendship` has default 120 for "hatched from egg"

-   [ ] **Ownership/Tracking Fields** - Track Pokemon origin and ownership
    -   Add `OriginalTrainer` (string?) to `PokemonInstance`
    -   Add `TrainerId` (int?) to `PokemonInstance`
    -   Add `MetLevel` (int?) to `PokemonInstance`
    -   Add `MetLocation` (string?) to `PokemonInstance`
    -   Add `MetDate` (DateTime?) to `PokemonInstance`
    -   **Note**: Useful for tracking Pokemon origin, trading, and ownership

-   [ ] **Advanced Pokedex Features** (if needed)
    -   Multiple Pokedex entries by generation
    -   Regional Pokedex support

---

## Migration Plan for Existing Pokemon

### Step 1: Add Fields to PokemonSpeciesData

-   Add properties with default values
-   Ensure backward compatibility

### Step 2: Update Builder

-   Add builder methods for new fields
-   Set sensible defaults

### Step 3: Update Existing Pokemon (26 Gen 1)

-   Add BaseExperienceYield (lookup official data)
-   Add CatchRate (lookup official data)
-   Add BaseFriendship (lookup official data)
-   Add GrowthRate (lookup official data)
-   Add Description (lookup official Pokedex entries)
-   Add Category (lookup official classification)
-   Add Height and Weight (lookup official data)
-   Add Color, Shape, Habitat (lookup official data)

### Step 4: Update PokemonInstance

-   Use `Species.BaseFriendship` instead of hardcoded `70`
-   Update factory to use new fields

### Step 5: Write Tests

-   Test EXP calculation with BaseExperienceYield
-   Test catch rate calculation with CatchRate
-   Test friendship initialization with BaseFriendship
-   Test EXP curves with GrowthRate
-   **See**: [testing.md](testing.md) for comprehensive testing strategy

### Step 6: Update Documentation

-   Update architecture.md
-   Update `.ai/context.md`

---

## Default Values Reference

### BaseExperienceYield (Gen 1 Examples)

| Pokemon   | BaseExp | Notes           |
| --------- | ------- | --------------- |
| Bulbasaur | 64      | Starter         |
| Pikachu   | 112     | Common          |
| Charizard | 240     | Final evolution |
| Mewtwo    | 306     | Legendary       |

### CatchRate (Gen 1 Examples)

| Pokemon      | CatchRate | Notes      |
| ------------ | --------- | ---------- |
| Most Pokemon | 45        | Standard   |
| Starters     | 45        | Standard   |
| Legendaries  | 3         | Very hard  |
| Master Ball  | 255       | Guaranteed |

### BaseFriendship (Gen 1 Examples)

| Pokemon      | BaseFriendship | Notes        |
| ------------ | -------------- | ------------ |
| Most Pokemon | 70             | Wild Pokemon |
| Starters     | 120            | Gift Pokemon |
| Mewtwo       | 0              | Legendary    |
| Hatched      | 120            | From egg     |

### GrowthRate (Gen 1 Examples)

| GrowthRate  | Pokemon Examples                           |
| ----------- | ------------------------------------------ |
| Fast        | Pikachu, Clefairy                          |
| Medium Fast | Most Pokemon (default)                     |
| Medium Slow | Starters (Bulbasaur, Charmander, Squirtle) |
| Slow        | Legendaries (Mewtwo)                       |

---

## Related Documents

| Document                                                              | Purpose                                                 |
| --------------------------------------------------------------------- | ------------------------------------------------------- |
| **[Architecture](architecture.md)**                                   | Complete technical specification of all data structures |
| **[Use Cases](use_cases.md)**                                         | All scenarios and behaviors                             |
| **[Testing](testing.md)**                                             | Comprehensive testing strategy                          |
| **[Code Location](code_location.md)**                                 | Where the code lives                                    |
| **[Sub-Feature 1.1: Pokemon Data](1.1-pokemon-data/)**                | Pokemon data structure                                  |
| **[Sub-Feature 1.18: Variants System](1.18-variants-system/)**        | Variants system specification                           |
| **[Sub-Feature 1.19: Pokedex Fields](1.19-pokedex-fields/)**          | Pokedex fields specification                            |
| **[Feature 3: Content Expansion](../3-content-expansion/roadmap.md)** | Content expansion phases                                |
| **[Feature 5: Game Features](../5-game-features/roadmap.md)**         | Game features (EXP, catching)                           |

---

## Version History

| Date       | Phase      | Notes                                                                                                                                                                           |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2025-01-XX | Initial    | Roadmap created with new Feature 1: Game Data structure                                                                                                                         |
| 2025-01-XX | Updated    | Reorganized to reflect all 19 sub-features by groups                                                                                                                            |
| 2024-12-XX | Refactored | Core module refactored following SOLID principles. Phases 0-8 completed (21/22 tasks, 95.5%). See `PokemonUltimate.Core/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for details |

---

## Next Steps

1. **Review this roadmap** and confirm which fields are needed
2. **Implement Phase 1** (Critical fields: BaseExperienceYield, CatchRate, BaseFriendship, GrowthRate)
3. **Implement Phase 2** (Pokedex fields: Description, Category, Height, Weight, Color, Shape, Habitat)
4. **Implement Phase 3** (Variants System: BaseForm, VariantType, TeraType, Variants) - See [1.18-variants-system/architecture.md](1.18-variants-system/architecture.md)
5. **Update existing 26 Pokemon** with new field values
6. **Write tests** for new fields (see [testing.md](testing.md) for comprehensive testing strategy)
7. **Update documentation**
8. **Proceed with content expansion** (Feature 3: Content Expansion)

---

**Status**: ✅ **Core Complete** - All main data structures implemented. Planned enhancements ready for implementation.
