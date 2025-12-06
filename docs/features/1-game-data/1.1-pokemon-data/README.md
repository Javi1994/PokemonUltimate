# Sub-Feature 1.1: Pokemon Data

> Pokemon species blueprints and runtime instances.

**Sub-Feature Number**: 1.1  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the complete data structure for Pokemon, including:

-   **PokemonSpeciesData** (Blueprint): Immutable species data shared by all Pokemon of the same kind
-   **PokemonInstance** (Runtime): Mutable instance data for individual Pokemon
-   **BaseStats**: Base stat values structure
-   **LearnableMove**: Move learning information structure

## Components

### PokemonSpeciesData

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs`

Immutable blueprint for a Pokemon species. Contains:

-   Identity: Name, PokedexNumber
-   Types: PrimaryType, SecondaryType
-   BaseStats: HP, Attack, Defense, SpAttack, SpDefense, Speed
-   Abilities: Ability1, Ability2, HiddenAbility
-   Learnset: List of LearnableMove
-   Evolutions: List of Evolution paths

### PokemonInstance

**Namespace**: `PokemonUltimate.Core.Instances`  
**Files**:

-   `PokemonInstance.cs` (Core)
-   `PokemonInstance.Battle.cs` (Battle state)
-   `PokemonInstance.LevelUp.cs` (Level-up logic)
-   `PokemonInstance.Evolution.cs` (Evolution tracking)

Mutable runtime instance of a Pokemon. Contains:

-   Species reference
-   Level, Experience, HP
-   Moves (MoveInstance list)
-   Status effects
-   Stat stages
-   Nature, Gender, Friendship

### BaseStats

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/BaseStats.cs`

Structure containing base stat values (HP, Attack, Defense, SpAttack, SpDefense, Speed).

### LearnableMove

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/LearnableMove.cs`

Structure defining how a Pokemon learns a move (Move reference, LearnMethod, Level).

## Phase 4 Extensions ✅ COMPLETE

**Status**: ✅ **Complete** - All Phase 4 optional enhancements have been implemented.

### IVs/EVs System ✅

-   **IVSet** - Individual Values (0-31 per stat) stored per Pokemon instance
-   **EVSet** - Effort Values (0-252 per stat, 510 total) stored per Pokemon instance
-   **Integration** - StatCalculator uses IVs/EVs for stat calculation
-   **Default Behavior** - Random IVs (0-31), Maximum EVs (252/252/4/0/0/0)

### Breeding System ✅

-   **EggGroup Enum** - 13 egg groups (Monster, Water1, Bug, Flying, Field, Fairy, Grass, Human-Like, Mineral, Amorphous, Dragon, Ditto, Undiscovered)
-   **EggGroups Property** - List of egg groups per Pokemon species
-   **EggCycles Property** - Cycles required to hatch eggs (default: 20)
-   **Breeding Compatibility** - `CanBreedWith()`, `IsInEggGroup()`, `CannotBreed` methods

### Ownership/Tracking Fields ✅

-   **OriginalTrainer** (string?) - Original Trainer's name (OT)
-   **TrainerId** (int?) - Unique identifier for the original trainer
-   **MetLevel** (int?) - Level at which Pokemon was met/caught
-   **MetLocation** (string?) - Location where Pokemon was met/caught
-   **MetDate** (DateTime?) - Date when Pokemon was met/caught

**See**: [Parent Roadmap - Phase 4](../roadmap.md#phase-4-optional-enhancements-low-priority) for complete implementation details.

## Related Sub-Features

-   **[1.2: Move Data](../1.2-move-data/)** - Moves referenced in Learnset
-   **[1.3: Ability Data](../1.3-ability-data/)** - Abilities assigned to species
-   **[1.11: Evolution System](../1.11-evolution-system/)** - Evolution paths
-   **[1.16: Factories & Calculators](../1.16-factories-calculators/)** - StatCalculator, PokemonInstanceBuilder

## Documentation

| Document                                                                 | Purpose                               |
| ------------------------------------------------------------------------ | ------------------------------------- |
| **[Architecture](architecture.md)**                                      | Complete technical specification      |
| **[Parent Architecture](../architecture.md#11-pokemon-data)**            | Feature-level technical specification |
| **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** | Code organization                     |
| **[Parent Roadmap - Phase 4](../roadmap.md#phase-4-optional-enhancements-low-priority)** | Pending extensions (IVs/EVs, Breeding, Ownership) |

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
