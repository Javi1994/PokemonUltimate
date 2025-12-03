# Sub-Feature 1.7: Evolution System

> Evolution paths and conditions for Pokemon evolution.

**Sub-Feature Number**: 1.7  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the evolution system:
- **Evolution**: Evolution path definition
- **IEvolutionCondition**: Base interface for evolution conditions
- **EvolutionConditions** (6 classes): Level, Item, Trade, Friendship, TimeOfDay, KnowsMove

## Components

### Evolution
**Namespace**: `PokemonUltimate.Core.Evolution`  
**File**: `PokemonUltimate.Core/Evolution/Evolution.cs`

Evolution path definition containing:
- Target: PokemonSpeciesData to evolve into
- Conditions: List of IEvolutionCondition (all must be met)

### IEvolutionCondition
**Namespace**: `PokemonUltimate.Core.Evolution`  
**File**: `PokemonUltimate.Core/Evolution/IEvolutionCondition.cs`

Base interface for evolution conditions with:
- ConditionType: EvolutionConditionType enum
- Description: Human-readable description
- IsMet(): Check if condition is met

### Evolution Conditions (6 Classes)
**Namespace**: `PokemonUltimate.Core.Evolution.Conditions`  
**Files**: `PokemonUltimate.Core/Evolution/Conditions/*.cs`

- **LevelCondition**: Minimum level requirement
- **ItemCondition**: Item use requirement
- **TradeCondition**: Trade requirement
- **FriendshipCondition**: Minimum friendship requirement
- **TimeOfDayCondition**: Time of day requirement
- **KnowsMoveCondition**: Must know specific move

## Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Evolution paths in PokemonSpeciesData
- **[1.11: Builders](../1.11-builders/)** - EvolutionBuilder for creating evolution paths

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete technical specification |
| **[Parent Architecture](../architecture.md#17-evolution-system)** | Feature-level technical specification |
| **[Parent Code Location](../code_location.md#grupo-c-supporting-systems)** | Code organization |

---

**Last Updated**: 2025-01-XX

