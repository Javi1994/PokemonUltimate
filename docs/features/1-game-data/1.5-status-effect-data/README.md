# Sub-Feature 1.5: Status Effect Data

> Status effect blueprints for persistent and volatile status conditions.

**Sub-Feature Number**: 1.5  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for Status Effects:
- **StatusEffectData** (Blueprint): Immutable status effect data

## Components

### StatusEffectData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/StatusEffectData.cs`

Immutable blueprint for a status effect. Covers both:
- **Persistent Status**: Burn, Paralysis, Poison, Sleep, Freeze
- **Volatile Status**: Confusion, Flinch, Leech Seed, etc.

Contains:
- Identity: Name, Description
- Type: PersistentStatus or VolatileStatus enum
- Effects: Damage per turn, stat modifiers, immunities, etc.

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that apply status effects
- **[1.11: Builders](../1.11-builders/)** - StatusEffectBuilder for creating status effects

## Documentation

- **[Parent Architecture](../architecture.md#15-status-effect-data)** - Technical specification
- **[Parent Code Location](../code_location.md#grupo-b-field--status-data)** - Code organization

---

**Last Updated**: 2025-01-XX

