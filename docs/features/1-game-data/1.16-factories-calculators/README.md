# Sub-Feature 1.16: Factories & Calculators

> Factories for creating instances and calculators for stat/formula calculations.

**Sub-Feature Number**: 1.16  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines factories and calculators:

-   **IStatCalculator** / **StatCalculator**: Stat calculation formulas (Gen 3+, instance-based with interface, post-refactor)
-   **ITypeEffectiveness** / **TypeEffectiveness**: Type effectiveness calculator (instance-based with interface, post-refactor)
-   **PokemonFactory**: Static factory for Pokemon creation
-   **PokemonInstanceBuilder**: Fluent builder for Pokemon instances (uses IRandomProvider, MoveSelector, post-refactor)
-   **IMoveSelector** / **MoveSelector**: Move selection system with strategies (post-refactor)

## Components

### IStatCalculator / StatCalculator

**Namespace**: `PokemonUltimate.Core.Factories`  
**Files**: `PokemonUltimate.Core/Factories/IStatCalculator.cs`, `StatCalculator.cs`

Instance-based calculator with interface (post-refactor). Implements Gen 3+ stat calculation formulas:

-   `CalculateHP()` - HP formula (different from other stats)
-   `CalculateStat()` - Other stats formula (with nature modifier)
-   `GetStageMultiplier()` - Battle stat stage multipliers
-   `GetExpForLevel()` - Experience calculations

**Post-Refactor Improvements**:

-   Converted from static class to instance-based with `IStatCalculator` interface
-   Uses `CoreConstants` for formula constants
-   Uses `CoreValidators` for validation
-   Static methods maintained as wrappers for backward compatibility

### PokemonFactory

**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/PokemonFactory.cs`

Static factory for quick Pokemon creation:

-   `Create(species, level)` - Simple creation
-   Delegates to PokemonInstanceBuilder for complex cases

### PokemonInstanceBuilder

**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/PokemonInstanceBuilder.cs`

Fluent builder for creating Pokemon instances with full control (refactored with DI and Strategy Pattern, post-refactor):

-   `Pokemon.Create()` - Entry point (now requires `IRandomProvider`, post-refactor)
-   Methods for nature, gender, moves, HP, status, etc.
-   Uses `MoveSelector` with strategies for move selection (post-refactor)
-   Uses `CoreConstants` instead of magic numbers (post-refactor)
-   Uses `CoreValidators` for validation (post-refactor)

**Post-Refactor Improvements**:

-   Uses `IRandomProvider` instead of static `Random` for testability
-   Uses `MoveSelector` with strategies (RandomMoveStrategy, StabMoveStrategy, PowerMoveStrategy, OptimalMoveStrategy)
-   Methods extracted for better SRP (CalculateStats, DetermineShiny, DetermineAbility, etc.)
-   Complete validation in `Build()` method

### ITypeEffectiveness / TypeEffectiveness

**Namespace**: `PokemonUltimate.Core.Factories`  
**Files**: `PokemonUltimate.Core/Factories/ITypeEffectiveness.cs`, `TypeEffectiveness.cs`

Instance-based type effectiveness calculator with interface (post-refactor):

-   Type effectiveness multipliers (0x, 0.5x, 1x, 2x)
-   Gen 6+ type chart (includes Fairy type)
-   STAB (Same Type Attack Bonus) multiplier
-   Methods: `GetEffectiveness()`, `GetSTABMultiplier()`

**Post-Refactor Improvements**:

-   Converted from static class to instance-based with `ITypeEffectiveness` interface
-   Static methods maintained as wrappers for backward compatibility

## Related Sub-Features

-   **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Creates Pokemon instances
-   **[1.14: Enums & Constants](../1.14-enums-constants/)** - Uses Stat, Nature enums and NatureData

## Documentation

-   **[Parent Architecture](../architecture.md#116-factories--calculators)** - Technical specification
-   **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** - Code organization

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
