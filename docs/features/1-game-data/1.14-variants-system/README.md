# Sub-Feature 1.14: Variants System

> Mega Evolutions, Dinamax, and Terracristalización as separate Pokemon species.

**Sub-Feature Number**: 1.14  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

Instead of temporary transformations during battle, **Mega Evolutions**, **Dinamax**, and **Terracristalización** are implemented as **separate Pokemon species** with their own `PokemonSpeciesData` entries.

**Key Characteristics**:
- Each variant is a distinct `PokemonSpeciesData` entry
- Variants have different base stats, types, and abilities
- Variants are linked to base form via `BaseForm` field
- Variants can be obtained/encountered separately

## Current Status

- ⏳ **Planned**: Variant system fields (BaseForm, VariantType, TeraType, Variants)
- ⏳ **Planned**: Variant species definitions
- ⏳ **Planned**: Variant relationship validation

## Variant Types

- **Mega Evolutions**: Permanent stat changes, type changes, ability changes
- **Dinamax**: Massive HP increase, special moves
- **Terracristalización**: Type change, stat boost

## Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Variants are PokemonSpeciesData entries

## Documentation

- **[Architecture](architecture.md)** - Complete specification of variants system
- **[Parent Architecture](../architecture.md#114-variants-system)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-e-planned-features)** - Code organization

---

**Last Updated**: 2025-01-XX

