# Sub-Feature 1.13: Registry System

> Data registry system for storing and retrieving game data.

**Sub-Feature Number**: 1.13  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the registry system for data access:
- **IDataRegistry<T>**: Generic registry interface
- **GameDataRegistry<T>**: Generic registry implementation
- **Specialized Registries**: PokemonRegistry, MoveRegistry

## Components

### IDataRegistry<T>
**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/IDataRegistry.cs`

Generic interface for data registries:
- `Register()` / `RegisterAll()` - Register data
- `Get()` / `GetById()` / `TryGet()` - Retrieve data
- `GetAll()` - Get all registered data
- `Exists()` / `Contains()` - Check existence
- `Count` - Get count

### GameDataRegistry<T>
**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/GameDataRegistry.cs`

Generic implementation using Dictionary<string, T>:
- Stores data by Id (from IIdentifiable)
- Provides all IDataRegistry<T> methods

### PokemonRegistry
**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/PokemonRegistry.cs`

Specialized registry for Pokemon with additional queries:
- `GetByPokedexNumber()` - Get by Pokedex number
- `GetByType()` - Get by Pokemon type
- `GetByPokedexRange()` - Get by Pokedex range
- `GetFinalForms()` - Get final evolution forms

### MoveRegistry
**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/MoveRegistry.cs`

Specialized registry for Moves with additional queries:
- `GetByType()` - Get by move type
- `GetByCategory()` - Get by move category
- `GetByMinPower()` - Get by minimum power
- `GetByPriority()` - Get by priority
- `GetContactMoves()` - Get contact moves

## Related Sub-Features

- **[1.9: Interfaces Base](../1.9-interfaces-base/)** - Uses IIdentifiable
- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - PokemonRegistry stores PokemonSpeciesData
- **[1.2: Move Data](../1.2-move-data/)** - MoveRegistry stores MoveData

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete technical specification |
| **[Parent Architecture](../architecture.md#113-registry-system)** | Feature-level technical specification |
| **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** | Code organization |

---

**Last Updated**: 2025-01-XX

