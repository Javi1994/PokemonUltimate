# Sub-Feature 1.1: Pokemon Data

> Pokemon species blueprints and runtime instances.

**Sub-Feature Number**: 1.1  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the complete data structure for Pokemon, including:
- **PokemonSpeciesData** (Blueprint): Immutable species data shared by all Pokemon of the same kind
- **PokemonInstance** (Runtime): Mutable instance data for individual Pokemon
- **BaseStats**: Base stat values structure
- **LearnableMove**: Move learning information structure

## Components

### PokemonSpeciesData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs`

Immutable blueprint for a Pokemon species. Contains:
- Identity: Name, PokedexNumber
- Types: PrimaryType, SecondaryType
- BaseStats: HP, Attack, Defense, SpAttack, SpDefense, Speed
- Abilities: Ability1, Ability2, HiddenAbility
- Learnset: List of LearnableMove
- Evolutions: List of Evolution paths

### PokemonInstance
**Namespace**: `PokemonUltimate.Core.Instances`  
**Files**: 
- `PokemonInstance.cs` (Core)
- `PokemonInstance.Battle.cs` (Battle state)
- `PokemonInstance.LevelUp.cs` (Level-up logic)
- `PokemonInstance.Evolution.cs` (Evolution tracking)

Mutable runtime instance of a Pokemon. Contains:
- Species reference
- Level, Experience, HP
- Moves (MoveInstance list)
- Status effects
- Stat stages
- Nature, Gender, Friendship

### BaseStats
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/BaseStats.cs`

Structure containing base stat values (HP, Attack, Defense, SpAttack, SpDefense, Speed).

### LearnableMove
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/LearnableMove.cs`

Structure defining how a Pokemon learns a move (Move reference, LearnMethod, Level).

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves referenced in Learnset
- **[1.3: Ability Data](../1.3-ability-data/)** - Abilities assigned to species
- **[1.7: Evolution System](../1.7-evolution-system/)** - Evolution paths
- **[1.12: Factories & Calculators](../1.12-factories-calculators/)** - StatCalculator, PokemonInstanceBuilder

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete technical specification |
| **[Parent Architecture](../architecture.md#11-pokemon-data)** | Feature-level technical specification |
| **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** | Code organization |

---

**Last Updated**: 2025-01-XX

