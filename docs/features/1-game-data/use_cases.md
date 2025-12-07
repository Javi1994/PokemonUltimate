# Feature 1: Game Data - Use Cases

> All scenarios, behaviors, and edge cases for game data structures.

**Feature Number**: 1  
**Feature Name**: Game Data (formerly "Pokemon Data")  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document covers all use cases for **all game data structures** organized by sub-features:

-   **Grupo A: Core Entity Data** (1.1-1.4) - Pokemon, Moves, Abilities, Items
-   **Grupo B: Field & Status Data** (1.5-1.10) - Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects
-   **Grupo C: Supporting Systems** (1.11-1.12) - Evolution System, Type Effectiveness
-   **Grupo D: Infrastructure** (1.13-1.14, 1.16-1.17) - Interfaces, Enums, Constants, Factories, Registries (Note: Builders moved to Feature 3.9)
-   **Grupo E: Planned Features** (1.18-1.19) - Variants System, Pokedex Fields

**See**: [Sub-Features Overview](README.md) for complete sub-feature list.

## Core Use Cases by Sub-Feature

### Grupo A: Core Entity Data (Sub-Features 1.1-1.4)

#### Sub-Feature 1.1: Pokemon Data

### UC-001: Define Pokemon Species

**Description**: Create a new Pokemon species blueprint with all required data
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Pokemon.Define(name, pokedexNumber)` to start
2. Set types with `.Type()` or `.Types()`
3. Set base stats with `.Stats()`
4. Set abilities with `.Ability()` or `.Abilities()`
5. Add learnset with `.Learnset()`
6. Add evolutions with `.Evolves()`
7. Call `.Build()` to create `PokemonSpeciesData`
   **Expected Result**: `PokemonSpeciesData` instance with all specified data
   **Status**: ✅ Implemented

### UC-002: Create Pokemon Instance

**Description**: Create a Pokemon instance from a species blueprint
**Actor**: Game system (battle, catching, etc.)
**Preconditions**: `PokemonSpeciesData` exists
**Steps**:

1. Use `Pokemon.Create(species, level)` to start
2. Optionally set nature with `.WithNature()`
3. Optionally set nickname with `.Named()`
4. Optionally set moves with `.WithMoves()`
5. Call `.Build()` to create `PokemonInstance`
   **Expected Result**: `PokemonInstance` with calculated stats, HP, and moves
   **Status**: ✅ Implemented

### UC-003: Query Pokemon by Name

**Description**: Retrieve a Pokemon species by its name
**Actor**: Game system
**Preconditions**: Pokemon exists in catalog
**Steps**:

1. Access `PokemonCatalog.[PokemonName]` (e.g., `PokemonCatalog.Pikachu`)
   **Expected Result**: `PokemonSpeciesData` for the requested Pokemon
   **Status**: ✅ Implemented

### UC-004: Query Pokemon by Pokedex Number

**Description**: Retrieve a Pokemon species by its Pokedex number
**Actor**: Game system
**Preconditions**: Pokemon exists in catalog
**Steps**:

1. Use `PokemonCatalog.GetByPokedexNumber(number)`
   **Expected Result**: `PokemonSpeciesData` for the requested Pokemon
   **Status**: ✅ Implemented

### UC-005: Calculate Pokemon Stats

**Description**: Calculate actual stats from base stats, level, nature, and IVs
**Actor**: `PokemonInstance` creation or level-up
**Preconditions**: `PokemonSpeciesData` with base stats exists
**Steps**:

1. Instance is created or level changes
2. `StatCalculator` calculates stats using Gen 3+ formula
3. Nature modifier applied (0.9, 1.0, or 1.1)
   **Expected Result**: Accurate stat values for the Pokemon instance
   **Status**: ✅ Implemented

### UC-006: Learn Moves on Level Up

**Description**: Pokemon learns new moves when leveling up
**Actor**: Level-up system
**Preconditions**: Pokemon has moves in learnset at new level
**Steps**:

1. Pokemon levels up
2. System checks learnset for moves at new level
3. If moves available and slots free, Pokemon learns them
4. If no slots free, player chooses which move to replace
   **Expected Result**: Pokemon has new moves appropriate for its level
   **Status**: ✅ Implemented

### UC-007: Evolution Triggered by Level

**Description**: Pokemon evolves when reaching a specific level
**Actor**: Level-up system
**Preconditions**: Pokemon has evolution condition (Level) and meets requirements
**Steps**:

1. Pokemon levels up to required level
2. System checks evolution conditions
3. If condition met, evolution occurs
4. Pokemon transforms to evolved species
   **Expected Result**: Pokemon becomes evolved species with new stats/types
   **Status**: ✅ Implemented

### UC-008: Evolution Triggered by Item

**Description**: Pokemon evolves when using a specific item
**Actor**: Player action
**Preconditions**: Pokemon has evolution condition (Item) and player has item
**Steps**:

1. Player uses evolution item on Pokemon
2. System checks evolution conditions
3. If condition met, evolution occurs
   **Expected Result**: Pokemon becomes evolved species
   **Status**: ✅ Implemented

### UC-009: Evolution Triggered by Friendship

**Description**: Pokemon evolves when friendship reaches threshold
**Actor**: Friendship system
**Preconditions**: Pokemon has evolution condition (Friendship) and friendship >= threshold
**Steps**:

1. Pokemon's friendship increases
2. System checks if friendship >= threshold
3. If condition met, evolution occurs
   **Expected Result**: Pokemon becomes evolved species
   **Status**: ⏳ Planned (Friendship field not yet implemented)

### UC-010: Create Mega Evolution Variant

**Description**: Define a Mega Evolution as a separate Pokemon species
**Actor**: Content developer
**Preconditions**: Base Pokemon species exists
**Steps**:

1. Use `Pokemon.Define()` to create Mega variant
2. Set different stats/types for Mega form
3. Use `.AsMegaVariant(baseForm)` to link to base
4. Build the Mega variant
   **Expected Result**: Separate `PokemonSpeciesData` for Mega form linked to base
   **Status**: ⏳ Planned (Variants system not yet implemented)

### UC-011: Create Dinamax Variant

**Description**: Define a Dinamax form as a separate Pokemon species
**Actor**: Content developer
**Preconditions**: Base Pokemon species exists
**Steps**:

1. Use `Pokemon.Define()` to create Dinamax variant
2. Set HP to 2x base HP
3. Use `.AsDinamaxVariant(baseForm)` to link to base
4. Build the Dinamax variant
   **Expected Result**: Separate `PokemonSpeciesData` for Dinamax form
   **Status**: ⏳ Planned (Variants system not yet implemented)

### UC-012: Create Terracristalización Variant

**Description**: Define a Terracristalización form as a separate Pokemon species
**Actor**: Content developer
**Preconditions**: Base Pokemon species exists
**Steps**:

1. Use `Pokemon.Define()` to create Tera variant
2. Set type to mono-type (Tera type)
3. Use `.AsTeraVariant(baseForm, teraType)` to link to base
4. Build the Tera variant
   **Expected Result**: Separate `PokemonSpeciesData` for Tera form
   **Status**: ⏳ Planned (Variants system not yet implemented)

---

#### Sub-Feature 1.2: Move Data

### UC-013: Define Move

**Description**: Create a new move blueprint with type, category, power, accuracy, and effects
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Move.Define(name)` to start
2. Set type with `.Type(PokemonType)`
3. Set category with `.Category(MoveCategory)`
4. Set power and accuracy with `.Power(int)` and `.Accuracy(int)`
5. Add effects with `.Effect(IMoveEffect)` or `.Effects(...)`
6. Call `.Build()` to create `MoveData`
   **Expected Result**: `MoveData` instance with all specified properties
   **Status**: ✅ Implemented

### UC-014: Create Move Instance

**Description**: Create a move instance from a move blueprint with PP tracking
**Actor**: Game system (battle, Pokemon instance creation)
**Preconditions**: `MoveData` exists
**Steps**:

1. Use `MoveInstance.Create(moveData)` or access from `PokemonInstance.Moves`
2. PP is initialized from `MoveData.MaxPP`
3. PP Ups can be applied (up to 3)
   **Expected Result**: `MoveInstance` with current PP tracking
   **Status**: ✅ Implemented

### UC-015: Apply Move Effect

**Description**: Execute a move's effect during battle
**Actor**: Combat system
**Preconditions**: Move instance exists, battle context available
**Steps**:

1. Move is used in battle
2. Each effect in `MoveData.Effects` is executed in order
3. Effects modify battle state (damage, status, stat changes, etc.)
   **Expected Result**: Move effects applied correctly
   **Status**: ✅ Implemented

---

#### Sub-Feature 1.3: Ability Data

### UC-016: Define Ability

**Description**: Create a new ability blueprint with triggers and effects
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Ability.Define(name)` to start
2. Set triggers with `.Trigger(AbilityTrigger)`
3. Set effects with `.Effect(AbilityEffect)`
4. Add modifiers if needed
5. Call `.Build()` to create `AbilityData`
   **Expected Result**: `AbilityData` instance with triggers and effects
   **Status**: ✅ Implemented

### UC-017: Ability Activates

**Description**: Ability triggers during battle based on its trigger conditions
**Actor**: Combat system
**Preconditions**: Pokemon has ability, trigger condition met
**Steps**:

1. Trigger condition occurs (e.g., weather change, stat drop, entry)
2. System checks Pokemon's ability trigger
3. If trigger matches, ability effect activates
4. Ability modifies battle state
   **Expected Result**: Ability activates correctly when trigger conditions are met
   **Status**: ✅ Implemented

---

#### Sub-Feature 1.4: Item Data

### UC-018: Define Item

**Description**: Create a new item blueprint with category, triggers, and effects
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Item.Define(name)` to start
2. Set category with `.Category(ItemCategory)`
3. Set triggers with `.Trigger(ItemTrigger)`
4. Set effects with `.Effect(...)`
5. Call `.Build()` to create `ItemData`
   **Expected Result**: `ItemData` instance with category, triggers, and effects
   **Status**: ✅ Implemented

### UC-019: Item Activates

**Description**: Held item triggers during battle based on its trigger conditions
**Actor**: Combat system
**Preconditions**: Pokemon holds item, trigger condition met
**Steps**:

1. Trigger condition occurs (e.g., HP drops below threshold, stat drop)
2. System checks held item's trigger
3. If trigger matches, item effect activates
4. Item modifies battle state or Pokemon state
   **Expected Result**: Item activates correctly when trigger conditions are met
   **Status**: ✅ Implemented

---

### Grupo B: Field & Status Data (Sub-Features 1.5-1.10)

#### Sub-Feature 1.5: Status Effect Data

### UC-020: Define Status Effect

**Description**: Create a new status effect blueprint (persistent or volatile)
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Status.Define(name)` to start
2. Set type (persistent or volatile)
3. Set effects and modifiers
4. Call `.Build()` to create `StatusEffectData`
   **Expected Result**: `StatusEffectData` instance
   **Status**: ✅ Implemented

### UC-021: Apply Status Effect

**Description**: Apply status effect to Pokemon during battle
**Actor**: Combat system
**Preconditions**: Status effect exists, Pokemon can receive status
**Steps**:

1. Status effect is applied (from move, ability, or item)
2. Status is set on Pokemon instance
3. Status effects modify Pokemon behavior or stats
   **Expected Result**: Status effect applied and active
   **Status**: ✅ Implemented

---

#### Sub-Feature 1.6: Weather Data

### UC-022: Define Weather Condition

**Description**: Create a new weather condition blueprint
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `WeatherEffect.Define(name)` to start
2. Set weather type and effects
3. Call `.Build()` to create `WeatherData`
   **Expected Result**: `WeatherData` instance
   **Status**: ✅ Implemented

#### Sub-Feature 1.7: Terrain Data

### UC-023: Define Terrain Condition

**Description**: Create a new terrain condition blueprint
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `TerrainEffect.Define(name)` to start
2. Set terrain type and effects
3. Call `.Build()` to create `TerrainData`
   **Expected Result**: `TerrainData` instance
   **Status**: ✅ Implemented

#### Sub-Feature 1.8: Hazard Data

### UC-024: Define Hazard

**Description**: Create a new entry hazard blueprint
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Hazard.Define(name)` to start
2. Set hazard type and damage/effects
3. Call `.Build()` to create `HazardData`
   **Expected Result**: `HazardData` instance
   **Status**: ✅ Implemented

#### Sub-Feature 1.9: Side Condition Data

### UC-025: Define Side Condition

**Description**: Create a new side condition blueprint (screens, Tailwind, etc.)
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Screen.Define(name)` or similar to start
2. Set side condition type and effects
3. Call `.Build()` to create `SideConditionData`
   **Expected Result**: `SideConditionData` instance
   **Status**: ✅ Implemented

#### Sub-Feature 1.10: Field Effect Data

### UC-026: Define Field Effect

**Description**: Create a new field effect blueprint (rooms, Gravity, etc.)
**Actor**: Content developer
**Preconditions**: None
**Steps**:

1. Use `Room.Define(name)` or similar to start
2. Set field effect type and effects
3. Call `.Build()` to create `FieldEffectData`
   **Expected Result**: `FieldEffectData` instance
   **Status**: ✅ Implemented

---

### Grupo C: Supporting Systems (Sub-Features 1.11-1.12)

#### Sub-Feature 1.11: Evolution System

### UC-027: Define Evolution Path

**Description**: Create an evolution path with conditions
**Actor**: Content developer
**Preconditions**: Base and target Pokemon species exist
**Steps**:

1. Use `EvolutionBuilder.Create(targetSpecies)` to start
2. Add conditions with `.WithCondition(IEvolutionCondition)`
3. Call `.Build()` to create `Evolution`
   **Expected Result**: `Evolution` instance with target and conditions
   **Status**: ✅ Implemented

### UC-028: Check Evolution Conditions

**Description**: Check if Pokemon can evolve based on its evolution conditions
**Actor**: Evolution system
**Preconditions**: Pokemon has evolution paths
**Steps**:

1. System checks all conditions in evolution path
2. Each condition checks if it's met (level, item, friendship, etc.)
3. If all conditions met, evolution can occur
   **Expected Result**: Boolean indicating if evolution is possible
   **Status**: ✅ Implemented

---

#### Sub-Feature 1.12: Type Effectiveness Table

### UC-029: Calculate Type Effectiveness

**Description**: Calculate damage multiplier based on attacker and defender types
**Actor**: Combat system
**Preconditions**: Attacker and defender types known
**Steps**:

1. System calls `TypeEffectiveness.GetMultiplier(attackType, defenderPrimaryType, defenderSecondaryType)`
2. Type chart is consulted (Gen 6+ chart with Fairy)
3. Multiplier calculated (0x, 0.5x, 1x, 2x, 4x)
   **Expected Result**: Correct damage multiplier
   **Status**: ✅ Implemented

### UC-030: Calculate STAB (Same Type Attack Bonus)

**Description**: Apply STAB multiplier if move type matches Pokemon type
**Actor**: Combat system
**Preconditions**: Move type and Pokemon types known
**Steps**:

1. System checks if move type matches Pokemon's primary or secondary type
2. If match, apply 1.5x STAB multiplier
   **Expected Result**: STAB multiplier applied correctly
   **Status**: ✅ Implemented

---

### Grupo D: Infrastructure (Sub-Features 1.13-1.17)

#### Sub-Feature 1.13: Interfaces Base

### UC-031: Identify Data by ID

**Description**: Access data using IIdentifiable.Id property
**Actor**: Game system
**Preconditions**: Data implements IIdentifiable
**Steps**:

1. Access `item.Id` property
2. Use ID to look up data in registries
   **Expected Result**: Unique identifier for data access
   **Status**: ✅ Implemented

---

#### Sub-Feature 1.14: Enums & Constants

### UC-032: Use Error Messages

**Description**: Access centralized error message constants
**Actor**: Game system
**Preconditions**: None
**Steps**:

1. Access `ErrorMessages.[MessageName]` constant
2. Use in exception or error handling
   **Expected Result**: Consistent error messages across codebase
   **Status**: ✅ Implemented

### UC-033: Use Game Messages

**Description**: Access centralized game message constants
**Actor**: Game system
**Preconditions**: None
**Steps**:

1. ~~Access `GameMessages.[MessageName]` constant~~ ⚠️ **DEPRECATED**
2. ✅ Use `LocalizationProvider` with `LocalizationKey` constants (see Feature 4.9)
3. Use in game UI or logs
   **Expected Result**: Consistent, localized game messages across codebase
   **Status**: ✅ **Migrated to LocalizationProvider** - All combat messages now use Feature 4.9 localization system

---

#### Builders (Moved to Feature 3.9)

**See**: **[Feature 3.9: Builders](../3-content-expansion/3.9-builders/)** for builder use cases

---

#### Sub-Feature 1.16: Factories & Calculators

### UC-035: Calculate Pokemon Stats

**Description**: Calculate Pokemon stats using Gen 3+ formula with IVs, EVs, and Nature
**Actor**: PokemonInstance creation or level-up
**Preconditions**: PokemonSpeciesData with base stats exists
**Steps**:

1. Call `StatCalculator.CalculateStat(baseStat, level, iv, ev, nature)`
2. Formula applied: `((2 * baseStat + iv + ev/4) * level / 100) + 5` (HP different)
3. Nature modifier applied (0.9, 1.0, or 1.1)
   **Expected Result**: Accurate stat value
   **Status**: ✅ Implemented

### UC-036: Create Pokemon Instance Quickly

**Description**: Create Pokemon instance using factory method
**Actor**: Game system
**Preconditions**: PokemonSpeciesData exists
**Steps**:

1. Call `PokemonFactory.CreateInstance(species, level)`
2. Factory creates instance with default values
3. Stats calculated automatically
   **Expected Result**: PokemonInstance created quickly
   **Status**: ✅ Implemented

---

#### Sub-Feature 1.17: Registry System

### UC-037: Register Data in Registry

**Description**: Register data instance in registry for lookup
**Actor**: Game system (initialization)
**Preconditions**: Data implements IIdentifiable
**Steps**:

1. Call `registry.Register(dataInstance)`
2. Data stored by ID
3. Available for lookup
   **Expected Result**: Data registered and accessible
   **Status**: ✅ Implemented

### UC-038: Query Data from Registry

**Description**: Retrieve data from registry by ID
**Actor**: Game system
**Preconditions**: Data registered in registry
**Steps**:

1. Call `registry.Get(id)` or `registry.TryGet(id, out item)`
2. Registry looks up data by ID
3. Data returned if found
   **Expected Result**: Data retrieved from registry
   **Status**: ✅ Implemented

### UC-039: Query Pokemon by Pokedex Number

**Description**: Retrieve Pokemon from specialized PokemonRegistry by Pokedex number
**Actor**: Game system
**Preconditions**: Pokemon registered in PokemonRegistry
**Steps**:

1. Call `pokemonRegistry.GetByPokedexNumber(number)`
2. Registry looks up Pokemon by Pokedex number
3. Pokemon returned if found
   **Expected Result**: Pokemon retrieved by Pokedex number
   **Status**: ✅ Implemented

---

### Grupo E: Planned Features (Sub-Features 1.18-1.19)

#### Sub-Feature 1.18: Variants System ⏳ Planned

### UC-040: Create Mega Evolution Variant

**Description**: Define Mega Evolution as separate Pokemon species (see UC-010)
**Status**: ⏳ Planned

### UC-041: Create Dinamax Variant

**Description**: Define Dinamax form as separate Pokemon species (see UC-011)
**Status**: ⏳ Planned

### UC-042: Create Terracristalización Variant

**Description**: Define Terracristalización form as separate Pokemon species (see UC-012)
**Status**: ⏳ Planned

---

#### Sub-Feature 1.19: Pokedex Fields ✅ Complete

### UC-043: Display Pokedex Entry

**Description**: Display Pokemon Pokedex entry with description, category, height, weight, etc.
**Actor**: Player
**Preconditions**: Pokedex fields implemented
**Steps**:

1. Player accesses Pokedex
2. System displays Pokemon data (Description, Category, Height, Weight, Color, Shape, Habitat)
   **Expected Result**: Complete Pokedex entry displayed
   **Status**: ⏳ Planned

---

## Edge Cases

### EC-001: Pokemon with No Secondary Type

**Description**: Mono-type Pokemon (e.g., Pikachu is Electric only)
**Behavior**: `SecondaryType` is `null`, `IsDualType` returns `false`
**Status**: ✅ Implemented

### EC-002: Genderless Pokemon

**Description**: Pokemon that cannot have a gender (e.g., Magnemite)
**Behavior**: `GenderRatio` is `-1`, `IsGenderless` returns `true`
**Status**: ✅ Implemented

### EC-003: Pokemon with No Evolutions

**Description**: Pokemon that cannot evolve (e.g., Tauros)
**Behavior**: `Evolutions` list is empty
**Status**: ✅ Implemented

### EC-004: Pokemon with Multiple Evolution Paths

**Description**: Pokemon that can evolve into different species (e.g., Eevee)
**Behavior**: `Evolutions` list contains multiple evolution entries
**Status**: ✅ Implemented

### EC-005: Pokemon with No Moves in Learnset

**Description**: Pokemon species with empty learnset (should not happen in practice)
**Behavior**: `Learnset` is empty list, instance creation may fail or use default moves
**Status**: ✅ Implemented (handled gracefully)

### EC-006: Instance Creation with Level 0

**Description**: Attempting to create Pokemon instance at level 0
**Behavior**: Should throw exception (invalid level)
**Status**: ✅ Implemented (validation exists)

### EC-007: Instance Creation with Level > 100

**Description**: Attempting to create Pokemon instance above level 100
**Behavior**: Should throw exception (invalid level)
**Status**: ✅ Implemented (validation exists)

### EC-008: Evolution with Full Party

**Description**: Pokemon tries to evolve but party is full
**Behavior**: Evolution is deferred or cancelled (depends on game design)
**Status**: ⏳ Planned (Game Features phase)

### EC-009: Missing Required Fields

**Description**: Creating Pokemon species without required fields (name, type, stats)
**Behavior**: Builder should throw exception or validation error
**Status**: ✅ Implemented (builder validation)

## Integration Scenarios

### INT-001: Game Data → Combat System

**Description**: Using game data structures in battles
**Steps**:

1. Create Pokemon instances for battle (Sub-Feature 1.1)
2. Combat system accesses instance stats, moves (Sub-Feature 1.2), abilities (Sub-Feature 1.3)
3. Battle applies move effects (Sub-Feature 1.2), ability effects (Sub-Feature 1.3), item effects (Sub-Feature 1.4)
4. Battle modifies instance state (HP, status from Sub-Feature 1.5)
5. Field conditions affect battle (Sub-Features 1.6-1.10)
6. Type effectiveness calculated (Sub-Feature 1.12)
   **Status**: ✅ Implemented

### INT-002: Game Data → Content Expansion

**Description**: Adding new content using the data structures
**Steps**:

1. Define new Pokemon species using `PokemonBuilder` (Sub-Feature 3.9)
2. Define new moves using `MoveBuilder` (Sub-Feature 3.9)
3. Define new abilities using `AbilityBuilder` (Sub-Feature 3.9)
4. Define new items using `ItemBuilder` (Sub-Feature 3.9)
5. Add to catalogs as partial classes (Feature 3)
6. New content available throughout game
   **Status**: ✅ Implemented (26 Gen 1 Pokemon, moves, abilities, items)

### INT-003: Pokemon Data → Catching System

**Description**: Catching wild Pokemon creates new instances
**Steps**:

1. Wild Pokemon encounter uses `PokemonSpeciesData` (Sub-Feature 1.1)
2. Catching uses `CatchRate` field (Sub-Feature 1.1, Phase 1)
3. Catching creates new `PokemonInstance` from species (Sub-Feature 1.1)
4. Instance added to player's party or PC
   **Status**: ⏳ Planned (Game Features phase, requires CatchRate field)

### INT-004: Pokemon Data → Evolution System

**Description**: Evolution transforms Pokemon instance
**Steps**:

1. Pokemon instance meets evolution condition (Sub-Feature 1.11)
2. System checks evolution conditions (Sub-Feature 1.11)
3. System creates new instance from evolved species (Sub-Feature 1.1)
4. Original instance replaced with evolved instance
   **Status**: ✅ Implemented (Level and Item evolutions)

### INT-005: Pokemon Data → Pokedex System

**Description**: Pokedex displays Pokemon species data
**Steps**:

1. Player encounters or catches Pokemon
2. Pokedex entry unlocked for that species
3. Pokedex displays species data (Sub-Feature 1.19: Description, Category, Height, Weight, Color, Shape, Habitat)
   **Status**: ⏳ Planned (Sub-Feature 1.19: Pokedex fields not yet implemented)

### INT-006: Move Effects → Combat System

**Description**: Move effects modify battle state
**Steps**:

1. Move is used in battle (Sub-Feature 1.2)
2. Move effects execute (Sub-Feature 1.2: 22 effect classes)
3. Effects modify battle state (damage, status, stat changes, field conditions)
   **Status**: ✅ Implemented

### INT-007: Abilities → Combat System

**Description**: Abilities activate during battle
**Steps**:

1. Ability trigger condition occurs (Sub-Feature 1.3)
2. Ability effect activates (Sub-Feature 1.3)
3. Ability modifies battle state or Pokemon state
   **Status**: ✅ Implemented

### INT-008: Items → Combat System

**Description**: Held items activate during battle
**Steps**:

1. Item trigger condition occurs (Sub-Feature 1.4)
2. Item effect activates (Sub-Feature 1.4)
3. Item modifies battle state or Pokemon state
   **Status**: ✅ Implemented

### INT-009: Field Conditions → Combat System

**Description**: Field conditions affect battle
**Steps**:

1. Weather condition active (Sub-Feature 1.6)
2. Terrain condition active (Sub-Feature 1.7)
3. Hazards on field (Sub-Feature 1.8)
4. Side conditions active (Sub-Feature 1.9)
5. Field effects active (Sub-Feature 1.10)
6. All conditions modify battle mechanics
   **Status**: ✅ Implemented

### INT-010: Type Effectiveness → Damage Calculation

**Description**: Type effectiveness affects move damage
**Steps**:

1. Move type and defender types known (Sub-Feature 1.12)
2. Type effectiveness calculated (Sub-Feature 1.12)
3. STAB calculated if applicable (Sub-Feature 1.12)
4. Multipliers applied to damage calculation
   **Status**: ✅ Implemented

### INT-011: Builders → Content Creation

**Description**: Builders used to create all game data
**Steps**:

1. Use builders (Sub-Feature 3.9) to create Pokemon, Moves, Abilities, Items, Field Conditions
2. Builders provide fluent API for data creation
3. All data created consistently
   **Status**: ✅ Implemented

### INT-012: Registries → Data Access

**Description**: Registries provide centralized data access
**Steps**:

1. Data registered in registries (Sub-Feature 1.17)
2. Game systems query registries for data
3. Data accessed by ID or specialized queries (Pokedex number, type, etc.)
   **Status**: ✅ Implemented

## Related Documents

-   **[Architecture](architecture.md)** - Technical design these use cases support
-   **[Roadmap](roadmap.md)** - Implementation status of use cases
-   **[Testing](testing.md)** - Tests covering these use cases
-   **[Code Location](code_location.md)** - Where use cases are implemented
-   **[Feature 2: Combat System](../2-combat-system/use_cases.md)** - Combat scenarios using Pokemon instances
-   **[Feature 3: Content Expansion](../3-content-expansion/roadmap.md)** - Adding Pokemon content

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
