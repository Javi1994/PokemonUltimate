# Sub-Feature 1.3: Ability Data

> Ability blueprints defining Pokemon abilities.

**Sub-Feature Number**: 1.3  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for Pokemon Abilities:
- **AbilityData** (Blueprint): Immutable ability data with triggers and effects

## Components

### AbilityData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/AbilityData.cs`

Immutable blueprint for a Pokemon ability. Contains:
- Identity: Id, Name, Description
- Triggers: AbilityTrigger flags (when ability activates)
- Effects: AbilityEffect flags (what ability does)
- Modifiers: Stat modifiers, damage multipliers, etc.

## Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Abilities assigned to Pokemon species
- **[1.11: Builders](../1.11-builders/)** - AbilityBuilder for creating abilities

## Documentation

- **[Parent Architecture](../architecture.md#13-ability-data)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** - Code organization

---

**Last Updated**: 2025-01-XX

