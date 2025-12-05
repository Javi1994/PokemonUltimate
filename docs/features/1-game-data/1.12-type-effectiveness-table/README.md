# Sub-Feature 1.12: Type Effectiveness Table

> Type effectiveness data table for damage calculations.

**Sub-Feature Number**: 1.12  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the type effectiveness table:

-   **ITypeEffectiveness** / **TypeEffectiveness**: Instance-based class with interface containing Gen 6+ type chart data (post-refactor)

## Components

### ITypeEffectiveness / TypeEffectiveness

**Namespace**: `PokemonUltimate.Core.Factories`  
**Files**: `PokemonUltimate.Core/Factories/ITypeEffectiveness.cs`, `TypeEffectiveness.cs`

Instance-based calculator with interface (post-refactor) containing:

-   Type effectiveness multipliers (0x, 0.5x, 1x, 2x)
-   Gen 6+ type chart (includes Fairy type)
-   STAB (Same Type Attack Bonus) multiplier
-   Methods: `GetEffectiveness()`, `GetSTABMultiplier()`, `GetEffectivenessDescription()`

**Post-Refactor Improvements**:

-   Converted from static class to instance-based with `ITypeEffectiveness` interface
-   Static methods maintained as wrappers for backward compatibility
-   Improved testability through dependency injection

**Note**: This contains the **data table**. The logic for using this data in damage calculation is in Feature 2: Combat System.

## Related Sub-Features

-   **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Pokemon types used in effectiveness calculations
-   **[1.2: Move Data](../1.2-move-data/)** - Move types used in effectiveness calculations

## Documentation

-   **[Parent Architecture](../architecture.md#112-type-effectiveness-table)** - Technical specification
-   **[Parent Code Location](../code_location.md#grupo-c-supporting-systems)** - Code organization

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
