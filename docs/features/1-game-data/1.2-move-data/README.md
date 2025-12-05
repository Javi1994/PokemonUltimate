# Sub-Feature 1.2: Move Data

> Move blueprints, runtime instances, and move effects.

**Sub-Feature Number**: 1.2  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the complete data structure for Moves, including:

-   **MoveData** (Blueprint): Immutable move data
-   **MoveInstance** (Runtime): Mutable instance with PP tracking
-   **Move Effects** (22 implementations): Composition-based move behavior

## Components

### MoveData

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/MoveData.cs`

Immutable blueprint for a move. Contains:

-   Identity: Name
-   Type: PokemonType
-   Category: MoveCategory (Physical, Special, Status)
-   Power, Accuracy, MaxPP
-   Priority
-   Effects: List of IMoveEffect (composition pattern)

### MoveInstance

**Namespace**: `PokemonUltimate.Core.Instances`  
**File**: `PokemonUltimate.Core/Instances/MoveInstance.cs`

Mutable runtime instance with PP tracking. Contains:

-   MoveData reference
-   CurrentPP, MaxPP
-   PP restoration methods

### Move Effects (22 Classes)

**Namespace**: `PokemonUltimate.Core.Effects`  
**Files**: `PokemonUltimate.Core/Effects/*.cs`

Move effects implementing `IMoveEffect`:

-   **Basic**: DamageEffect, StatusEffect, StatChangeEffect
-   **Healing**: HealEffect, RecoilEffect, DrainEffect
-   **Control**: ProtectionEffect, ForceSwitchEffect, SwitchAfterAttackEffect
-   **Multi-hit**: MultiHitEffect, BindingEffect, ChargingEffect
-   **Timing**: DelayedDamageEffect, FieldConditionEffect, FlinchEffect
-   **Special**: FixedDamageEffect, MoveRestrictionEffect, PriorityModifierEffect
-   **Other**: RevengeEffect, SelfDestructEffect, VolatileStatusEffect
-   **Advanced** (Sub-Feature 2.15): CounterEffect, FocusPunchEffect, MultiTurnEffect, ProtectEffect, PursuitEffect, SemiInvulnerableEffect

## Design Pattern

Moves use the **Composition Pattern**: behavior is defined by composing multiple effects, not by subclassing.

```csharp
var thunderbolt = Move.Define("Thunderbolt")
    .Type(PokemonType.Electric)
    .Category(MoveCategory.Special)
    .Power(90)
    .Accuracy(100)
    .MaxPP(15)
    .WithEffect(new DamageEffect())
    .WithEffect(new StatusEffect(PersistentStatus.Paralysis, 10))
    .Build();
```

## Related Sub-Features

-   **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Moves referenced in Learnset
-   **[1.5: Status Effect Data](../1.5-status-effect-data/)** - Status effects applied by moves
-   **[1.6: Weather Data](../1.6-weather-data/)** - Weather conditions set by moves
-   **[1.7: Terrain Data](../1.7-terrain-data/)** - Terrain conditions set by moves
-   **[1.8: Hazard Data](../1.8-hazard-data/)** - Entry hazards set by moves
-   **[1.9: Side Condition Data](../1.9-side-condition-data/)** - Side conditions set by moves
-   **[1.10: Field Effect Data](../1.10-field-effect-data/)** - Field effects set by moves
-   **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - MoveBuilder for creating moves

## Documentation

| Document                                                                 | Purpose                               |
| ------------------------------------------------------------------------ | ------------------------------------- |
| **[Architecture](architecture.md)**                                      | Complete technical specification      |
| **[Parent Architecture](../architecture.md#12-move-data)**               | Feature-level technical specification |
| **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** | Code organization                     |

---

**Last Updated**: 2025-01-XX
