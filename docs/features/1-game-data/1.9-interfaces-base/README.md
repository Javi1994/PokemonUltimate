# Sub-Feature 1.9: Interfaces Base

> Base interfaces for identifiable game data.

**Sub-Feature Number**: 1.9  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines base interfaces for game data:
- **IIdentifiable**: Base interface for all identifiable data structures

## Components

### IIdentifiable
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/IIdentifiable.cs`

Base interface requiring:
- `Id` property (string): Unique identifier for the data

Used by:
- PokemonSpeciesData
- MoveData
- All registry systems

## Related Sub-Features

- **[1.13: Registry System](../1.13-registry-system/)** - Uses IIdentifiable for generic registries

## Documentation

- **[Parent Architecture](../architecture.md#19-interfaces-base)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** - Code organization

---

**Last Updated**: 2025-01-XX

