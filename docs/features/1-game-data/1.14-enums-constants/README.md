# Sub-Feature 1.14: Enums & Constants

> All game enums and constant messages.

**Sub-Feature Number**: 1.14  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines all enums and constants used throughout the game:

-   **Enums** (20 main + 7 in Effects): Type definitions
-   **Constants**: ErrorMessages, GameMessages, CoreConstants (post-refactor)
-   **Validators**: CoreValidators (post-refactor)
-   **Extensions**: LevelExtensions, FriendshipExtensions (post-refactor)
-   **Data Tables**: NatureData (nature modifier tables)

## Components

### Main Enums (20)

**Namespace**: `PokemonUltimate.Core.Enums`  
**Files**: `PokemonUltimate.Core/Enums/*.cs`

-   `PokemonType` - 18 Pokemon types
-   `Stat` - HP, Attack, Defense, SpAttack, SpDefense, Speed
-   `Nature` - 25 natures
-   `Gender` - Male, Female, Genderless
-   `MoveCategory` - Physical, Special, Status
-   `EffectType` - Move effect types
-   `PersistentStatus` - Burn, Paralysis, Poison, Sleep, Freeze, BadlyPoisoned
-   `VolatileStatus` - Confusion, Flinch, etc.
-   `AbilityTrigger` - When abilities activate
-   `AbilityEffect` - What abilities do
-   `ItemTrigger` - When items activate
-   `ItemCategory` - Held Item, Berry, Consumable, etc.
-   `LearnMethod` - Start, LevelUp, TM, Egg, Tutor, Evolution
-   `TimeOfDay` - Day, Night, Dawn, Dusk
-   `TargetScope` - Single, AllAdjacent, All, etc.
-   `Weather` - Rain, Sun, Hail, Sandstorm, Fog, etc.
-   `Terrain` - Grassy, Electric, Psychic, Misty
-   `HazardType` - Stealth Rock, Spikes, Toxic Spikes, Sticky Web
-   `SideCondition` - Reflect, Light Screen, Tailwind, etc.
-   `FieldEffect` - Trick Room, Wonder Room, etc.
-   `EvolutionConditionType` - Level, Item, Trade, Friendship, etc.

### Enums in Effects (7)

**Namespace**: `PokemonUltimate.Core.Effects`  
**Files**: Within effect classes

-   `SemiInvulnerableState` (ChargingEffect)
-   `FieldConditionType` (FieldConditionEffect)
-   `MoveRestrictionType` (MoveRestrictionEffect)
-   `ProtectionType` (ProtectionEffect)
-   `ContactPenalty` (ProtectionEffect)
-   `PriorityCondition` (PriorityModifierEffect)
-   `SelfDestructType` (SelfDestructEffect)

### Constants

**Namespace**: `PokemonUltimate.Core.Constants`  
**Files**: `PokemonUltimate.Core/Constants/*.cs`

-   **ErrorMessages**: Centralized error message strings (for exceptions/validation)
-   **GameMessages**: ⚠️ **DEPRECATED** - In-game message strings (only used in `TypeEffectiveness.GetEffectivenessDescription()` which is not used in production). All combat messages now use `LocalizationProvider` (see Feature 4.9)
-   **CoreConstants** (post-refactor): Core module constants
    -   ShinyOdds, Friendship values (DefaultWildFriendship, HatchedFriendship, HighFriendshipThreshold, MaxFriendship)
    -   IV/EV limits (MaxIV, MaxEV, MaxTotalEV)
    -   Stat stages (MinStatStage, MaxStatStage)
    -   Formula constants (StatFormulaBase, StatFormulaDivisor, StatFormulaBonus, HPFormulaBonus, EVBonusDivisor)

### Validators (Post-Refactor)

**Namespace**: `PokemonUltimate.Core.Constants`  
**File**: `PokemonUltimate.Core/Constants/CoreValidators.cs`

Centralized validation methods:

-   `ValidateLevel(int level)` - Validates level (1-100)
-   `ValidateFriendship(int friendship)` - Validates friendship (0-255)
-   `ValidateStatStage(int stage)` - Validates stat stage (-6 to +6)
-   `ValidateIV(int iv)` - Validates IV (0-31)
-   `ValidateEV(int ev)` - Validates EV (0-252)

### Extensions (Post-Refactor)

**Namespace**: `PokemonUltimate.Core.Extensions`  
**Files**: `PokemonUltimate.Core/Extensions/*.cs`

Extension methods for common operations:

-   **LevelExtensions**: `IsValidLevel()` - Validates level (1-100)
-   **FriendshipExtensions**: `ClampFriendship()` - Clamps friendship (0-255)

### Data Tables

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/NatureData.cs`

-   **NatureData**: Static class providing nature modifier tables
    -   `GetStatMultiplier()` - Get nature multiplier for stat (1.1, 0.9, or 1.0)
    -   `GetIncreasedStat()` / `GetDecreasedStat()` - Which stats are affected
    -   `IsNeutral()` - Check if nature has no stat changes
    -   Complements the `Nature` enum (25 natures)

## Related Sub-Features

-   All sub-features use these enums and constants

## Documentation

-   **[Parent Architecture](../architecture.md#114-enums--constants)** - Technical specification
-   **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** - Code organization

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
