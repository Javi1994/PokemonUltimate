# Sub-Feature 1.16: Factories & Calculators

> Factories for creating instances and calculators for stat/formula calculations.

**Sub-Feature Number**: 1.16  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines factories and calculators:
- **StatCalculator**: Stat calculation formulas (Gen 3+)
- **PokemonFactory**: Static factory for Pokemon creation
- **PokemonInstanceBuilder**: Fluent builder for Pokemon instances

## Components

### StatCalculator
**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/StatCalculator.cs`

Static class with Gen 3+ stat calculation formulas:
- `CalculateHP()` - HP formula (different from other stats)
- `CalculateStat()` - Other stats formula (with nature modifier)
- `GetStageMultiplier()` - Battle stat stage multipliers
- `GetExpForLevel()` - Experience calculations

### PokemonFactory
**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/PokemonFactory.cs`

Static factory for quick Pokemon creation:
- `Create(species, level)` - Simple creation
- Delegates to PokemonInstanceBuilder for complex cases

### PokemonInstanceBuilder
**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/PokemonInstanceBuilder.cs`

Fluent builder for creating Pokemon instances with full control:
- `Pokemon.Create()` - Entry point
- Methods for nature, gender, moves, HP, status, etc.

## Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Creates Pokemon instances
- **[1.14: Enums & Constants](../1.14-enums-constants/)** - Uses Stat, Nature enums and NatureData

## Documentation

- **[Parent Architecture](../architecture.md#116-factories--calculators)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** - Code organization

---

**Last Updated**: 2025-01-XX

