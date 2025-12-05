# Feature 2: Combat System - Code Location

> Where the combat system code lives and how it's organized.

**Feature Number**: 2  
**Feature Name**: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The combat system is organized into several key areas:

-   **Engine** - Main battle controller and turn execution
-   **Actions** - BattleAction implementations (moves, status, switching, etc.)
-   **Damage** - Damage calculation pipeline
-   **Field** - Battle field, slots, sides, rules
-   **Events** - Ability and item trigger system
-   **Helpers** - Utility classes (turn order, targeting, accuracy)
-   **AI** - AI action providers
-   **View** - Battle view interface for visual presentation

## Namespaces

### `PokemonUltimate.Combat`

**Purpose**: Main combat engine and core classes
**Key Classes**:

-   `CombatEngine` - Main battle controller
-   `BattleQueue` - Action queue processor
-   `BattleArbiter` - Victory/defeat detection

### `PokemonUltimate.Combat.Actions`

**Purpose**: BattleAction implementations (base class from Sub-Feature 2.2)
**Key Classes**:

-   `BattleAction` - Base action class (Sub-Feature 2.2)
-   `UseMoveAction` - Move execution (includes advanced move mechanics - Sub-Feature 2.15)
-   `DamageAction` - Damage application
-   `ApplyStatusAction` - Status effect application
-   `SetWeatherAction` - Weather condition changes (Sub-Feature 2.12)
-   `SetTerrainAction` - Terrain condition changes (Sub-Feature 2.13)
-   `SetSideConditionAction` - Side condition changes (Sub-Feature 2.16)
-   `HealAction` - HP restoration
-   `SwitchAction` - Pokemon switching (includes entry hazard processing, Sub-Feature 2.14)
-   `StatChangeAction` - Stat stage modifications (includes Mist protection, Sub-Feature 2.16)
-   `FaintAction` - Pokemon fainting
-   `MessageAction` - Text messages
-   `BattleActionType` - Action type enum

**Note**: `UseMoveAction` handles advanced move mechanics (Sub-Feature 2.15) including:

-   Multi-hit moves (MultiHitEffect)
-   Counter/Mirror Coat (CounterEffect)
-   Protect/Detect (ProtectEffect)
-   Focus Punch (FocusPunchEffect)
-   Multi-turn moves (MultiTurnEffect)
-   Pursuit (PursuitEffect)
-   Semi-invulnerable moves (SemiInvulnerableEffect)

These effects are defined in `PokemonUltimate.Core.Effects` (see Feature 1.2: Move Data).

### `PokemonUltimate.Combat.Damage`

**Purpose**: Damage calculation pipeline (includes stat and damage modifiers)
**Key Classes**:

-   `DamagePipeline` - Main damage calculator
-   `IDamagePipeline` - Damage pipeline interface (post-refactor)
-   `DamageContext` - Damage calculation context
-   `IDamageStep` - Pipeline step interface
-   `IStatModifier` - Stat and damage modifier interface (abilities, items)
-   `AbilityStatModifier` - Ability-based stat/damage modifier adapter
-   `ItemStatModifier` - Item-based stat/damage modifier adapter

### `PokemonUltimate.Combat.Damage.Steps`

**Purpose**: Individual damage calculation steps
**Key Classes**:

-   `BaseDamageStep` - Base damage calculation
-   `CriticalHitStep` - Critical hit detection and multiplier
-   `RandomFactorStep` - Random damage variation (0.85-1.0)
-   `StabStep` - STAB (Same Type Attack Bonus) multiplier
-   `AttackerAbilityStep` - Attacker ability effects
-   `AttackerItemStep` - Attacker item effects
-   `WeatherStep` - Weather damage modifiers (Sub-Feature 2.12)
-   `TerrainStep` - Terrain damage modifiers (Sub-Feature 2.13)
-   `ScreenStep` - Screen damage reduction (Reflect, Light Screen, Aurora Veil) (Sub-Feature 2.16)
-   `TypeEffectivenessStep` - Type effectiveness calculation
-   `BurnStep` - Burn status penalty for physical moves

### `PokemonUltimate.Combat.Field`

**Purpose**: Battle field management
**Key Classes**:

-   `BattleField` - Main battlefield container (includes weather tracking - Sub-Feature 2.12, terrain tracking - Sub-Feature 2.13)
-   `BattleSide` - Player or enemy side (includes entry hazard tracking - Sub-Feature 2.14, side condition tracking - Sub-Feature 2.16)
-   `BattleSlot` - Individual Pokemon slot
-   `BattleRules` - Battle format rules

### `PokemonUltimate.Combat.Events`

**Purpose**: Event-driven ability and item system
**Key Classes**:

-   `BattleTrigger` - Trigger type enum
-   `IBattleListener` - Listener interface
-   `AbilityListener` - Ability event handler
-   `ItemListener` - Item event handler
-   `BattleTriggerProcessor` - Trigger processor (instance-based, post-refactor)
-   `IBattleTriggerProcessor` - Trigger processor interface
-   `IBattleEventBus` - Event bus interface for decoupled communication
-   `BattleEventBus` - Event bus implementation

### `PokemonUltimate.Combat.Engine`

**Purpose**: Engine components
**Key Classes**:

-   `CombatEngine` - Main battle controller (includes weather duration decrement - Sub-Feature 2.12, terrain duration decrement - Sub-Feature 2.13, side condition duration decrement - Sub-Feature 2.16)
-   `BattleQueue` - Action queue processor (uses `LinkedList<BattleAction>` internally)
-   `BattleArbiter` - Victory/defeat detection
-   `EndOfTurnProcessor` - End-of-turn effects processor (instance-based, post-refactor)
-   `IEndOfTurnProcessor` - End-of-turn processor interface
-   `EntryHazardProcessor` - Entry hazard processing on switch-in (instance-based, post-refactor)
-   `IEntryHazardProcessor` - Entry hazard processor interface

### `PokemonUltimate.Combat.Helpers`

**Purpose**: Utility classes
**Key Classes**:

-   `TurnOrderResolver` - Turn order calculation (instance-based, post-refactor, includes Tailwind speed multiplier - Sub-Feature 2.16)
-   `TargetResolver` - Target selection (instance-based, post-refactor)
-   `ITargetResolver` - Target resolver interface
-   `AccuracyChecker` - Accuracy calculation (instance-based, post-refactor, includes weather perfect accuracy - Sub-Feature 2.12)
-   `ITargetRedirectionResolver` - Target redirection resolver interface
-   `TargetRedirectionResolver` - Target redirection coordinator
-   `FollowMeResolver` - Follow Me redirection resolver
-   `LightningRodResolver` - Lightning Rod redirection resolver

### `PokemonUltimate.Combat.AI`

**Purpose**: AI action providers
**Key Classes**:

-   `RandomAI` - Random action selection
-   `AlwaysAttackAI` - Always attack AI

### `PokemonUltimate.Combat.Providers`

**Purpose**: Action provider interfaces and random number generation
**Key Classes**:

-   `IActionProvider` - Action provider interface
-   `PlayerInputProvider` - Human player input provider
-   `IRandomProvider` - Random number generation interface (replaces static Random)
-   `RandomProvider` - Random number generation implementation

### `PokemonUltimate.Combat.View`

**Purpose**: Battle view interface
**Key Classes**:

-   `IBattleView` - Battle view interface
-   `NullBattleView` - Null implementation for testing

### `PokemonUltimate.Combat.Results`

**Purpose**: Battle results
**Key Classes**:

-   `BattleOutcome` - Outcome enum
-   `BattleResult` - Detailed battle result

### `PokemonUltimate.Combat.Factories`

**Purpose**: Factory pattern for object creation (post-refactor)
**Key Classes**:

-   `IBattleFieldFactory` - BattleField factory interface
-   `BattleFieldFactory` - BattleField factory implementation
-   `IBattleQueueFactory` - BattleQueue factory interface
-   `BattleQueueFactory` - BattleQueue factory implementation
-   `DamageContextFactory` - DamageContext factory for creating contexts for different scenarios

### `PokemonUltimate.Combat.ValueObjects`

**Purpose**: Value Objects for complex state management (post-refactor)
**Key Classes**:

-   `StatStages` - Manages stat stage modifications (-6 to +6)
-   `DamageTracker` - Tracks damage for Counter/Mirror Coat calculations
-   `ProtectTracker` - Tracks Protect/Detect state
-   `SemiInvulnerableState` - Tracks semi-invulnerable move state
-   `ChargingMoveState` - Tracks charging move state
-   `MoveStateTracker` - Unified tracker for all move states
-   `TerrainState` - Terrain condition and duration
-   `WeatherState` - Weather condition and duration

### `PokemonUltimate.Combat.Effects`

**Purpose**: Move effect processing using Strategy Pattern (post-refactor)
**Key Classes**:

-   `IMoveEffectProcessor` - Move effect processor interface
-   `MoveEffectProcessorRegistry` - Registry for effect processors
-   `StatusEffectProcessor` - Processes status effects
-   `StatChangeEffectProcessor` - Processes stat change effects
-   `RecoilEffectProcessor` - Processes recoil effects
-   `DrainEffectProcessor` - Processes drain effects
-   `FlinchEffectProcessor` - Processes flinch effects
-   `ProtectEffectProcessor` - Processes protect effects
-   `CounterEffectProcessor` - Processes counter effects
-   `HealEffectProcessor` - Processes heal effects
-   `IMoveModifier` - Move modifier interface for temporary move modifications
-   `MoveModifier` - Move modifier implementation
-   `IAccumulativeEffect` - Accumulative effect interface
-   `AccumulativeEffectTracker` - Tracks accumulative effects (e.g., Badly Poisoned)

### `PokemonUltimate.Combat.Logging`

**Purpose**: Structured logging system (post-refactor)
**Key Classes**:

-   `IBattleLogger` - Logger interface
-   `BattleLogger` - Logger implementation
-   `NullBattleLogger` - Null implementation for tests

### `PokemonUltimate.Combat.Messages`

**Purpose**: Centralized message formatting (post-refactor)
**Key Classes**:

-   `IBattleMessageFormatter` - Message formatter interface
-   `BattleMessageFormatter` - Message formatter implementation

### `PokemonUltimate.Combat.Validation`

**Purpose**: Battle state validation (post-refactor)
**Key Classes**:

-   `IBattleStateValidator` - Battle state validator interface
-   `BattleStateValidator` - Battle state validator implementation

### `PokemonUltimate.Combat.Extensions`

**Purpose**: Extension methods for common operations (post-refactor)
**Key Classes**:

-   `BattleSlotExtensions` - Extension methods for BattleSlot (e.g., `IsActive()`)
-   `DamageCalculationExtensions` - Extension methods for damage calculations (e.g., `EnsureMinimumDamage()`)

### `PokemonUltimate.Combat.Constants`

**Purpose**: Centralized constants (post-refactor)
**Key Classes**:

-   `BattleConstants` - Battle system limits (`MaxTurns`, `MaxQueueIterations`)
-   `StatusConstants` - Status effect constants (`ParalysisSpeedMultiplier`, `ParalysisFullParalysisChance`)
-   `ItemConstants` - Item effect constants (`LeftoversHealDivisor`)
-   `MoveConstants` - Move-related constants (semi-invulnerable move names)

## Project Structure

```
PokemonUltimate.Combat/
├── Actions/
│   ├── BattleAction.cs                 # Base action class
│   ├── BattleActionType.cs             # Action type enum
│   ├── UseMoveAction.cs                # Move execution
│   ├── DamageAction.cs                 # Damage application
│   ├── ApplyStatusAction.cs            # Status effects
│   ├── HealAction.cs                   # HP restoration
│   ├── StatChangeAction.cs             # Stat modifications
│   ├── SwitchAction.cs                  # Pokemon switching
│   ├── FaintAction.cs                  # Fainting
│   └── MessageAction.cs                # Text messages
│
├── AI/
│   ├── RandomAI.cs                     # Random AI
│   └── AlwaysAttackAI.cs               # Always attack AI
│
├── Damage/
│   ├── DamagePipeline.cs               # Main damage calculator
│   ├── IDamagePipeline.cs               # Damage pipeline interface
│   ├── DamageContext.cs                # Damage context
│   ├── IDamageStep.cs                  # Pipeline step interface
│   ├── IStatModifier.cs                # Stat modifier interface
│   ├── AbilityStatModifier.cs          # Ability stat modifier
│   ├── ItemStatModifier.cs             # Item stat modifier
│   └── Steps/
│       ├── BaseDamageStep.cs           # Base damage
│       ├── CriticalHitStep.cs          # Critical hit detection
│       ├── RandomFactorStep.cs         # Random variation (0.85-1.0)
│       ├── StabStep.cs                 # STAB multiplier
│       ├── AttackerAbilityStep.cs      # Attacker abilities
│       ├── AttackerItemStep.cs         # Attacker items
│       ├── WeatherStep.cs              # Weather damage modifiers (Sub-Feature 2.12)
│       ├── TypeEffectivenessStep.cs    # Type effectiveness
│       └── BurnStep.cs                 # Burn penalty
│
├── Engine/
│   ├── CombatEngine.cs                 # Main battle controller (DI-based, post-refactor)
│   ├── BattleQueue.cs                  # Action queue (uses LinkedList internally)
│   ├── BattleArbiter.cs                # Victory/defeat detection
│   ├── EndOfTurnProcessor.cs          # End-of-turn effects (instance-based, post-refactor)
│   ├── IEndOfTurnProcessor.cs         # End-of-turn processor interface
│   ├── EntryHazardProcessor.cs        # Entry hazard processing (instance-based, post-refactor)
│   └── IEntryHazardProcessor.cs       # Entry hazard processor interface
│
├── Events/
│   ├── BattleTrigger.cs                # Trigger enum
│   ├── IBattleListener.cs              # Listener interface
│   ├── AbilityListener.cs              # Ability handler
│   ├── ItemListener.cs                 # Item handler
│   ├── BattleTriggerProcessor.cs       # Trigger processor (instance-based, post-refactor)
│   ├── IBattleTriggerProcessor.cs     # Trigger processor interface
│   ├── IBattleEventBus.cs              # Event bus interface
│   └── BattleEventBus.cs               # Event bus implementation
│
├── Field/
│   ├── BattleField.cs                  # Main battlefield (weather tracking - Sub-Feature 2.12)
│   ├── BattleSide.cs                   # Player/enemy side
│   ├── BattleSlot.cs                   # Pokemon slot
│   └── BattleRules.cs                 # Battle rules
│
├── Helpers/
│   ├── TurnOrderResolver.cs           # Turn order calculation (instance-based, post-refactor)
│   ├── TargetResolver.cs              # Target selection (instance-based, post-refactor)
│   ├── ITargetResolver.cs             # Target resolver interface
│   ├── AccuracyChecker.cs             # Accuracy calculation (instance-based, post-refactor)
│   ├── ITargetRedirectionResolver.cs  # Target redirection resolver interface
│   ├── TargetRedirectionResolver.cs   # Target redirection coordinator
│   └── TargetRedirectionResolvers/
│       ├── FollowMeResolver.cs        # Follow Me resolver
│       └── LightningRodResolver.cs    # Lightning Rod resolver
│
├── Providers/
│   ├── IActionProvider.cs              # Action provider interface
│   ├── PlayerInputProvider.cs          # Human input provider
│   ├── IRandomProvider.cs              # Random number generation interface
│   └── RandomProvider.cs               # Random number generation implementation
│
├── Results/
│   ├── BattleOutcome.cs                # Outcome enum
│   └── BattleResult.cs                 # Result data
│
├── Factories/
│   ├── IBattleFieldFactory.cs         # BattleField factory interface
│   ├── BattleFieldFactory.cs           # BattleField factory implementation
│   ├── IBattleQueueFactory.cs         # BattleQueue factory interface
│   ├── BattleQueueFactory.cs          # BattleQueue factory implementation
│   └── DamageContextFactory.cs        # DamageContext factory
│
├── ValueObjects/
│   ├── StatStages.cs                   # Stat stage management
│   ├── DamageTracker.cs                # Damage tracking for Counter/Mirror Coat
│   ├── ProtectTracker.cs               # Protect/Detect state tracking
│   ├── SemiInvulnerableState.cs        # Semi-invulnerable move state
│   ├── ChargingMoveState.cs            # Charging move state
│   ├── MoveStateTracker.cs             # Unified move state tracker
│   ├── TerrainState.cs                 # Terrain state and duration
│   └── WeatherState.cs                 # Weather state and duration
│
├── Effects/
│   ├── IMoveEffectProcessor.cs        # Move effect processor interface
│   ├── MoveEffectProcessorRegistry.cs  # Effect processor registry
│   ├── StatusEffectProcessor.cs        # Status effect processor
│   ├── StatChangeEffectProcessor.cs    # Stat change effect processor
│   ├── RecoilEffectProcessor.cs       # Recoil effect processor
│   ├── DrainEffectProcessor.cs         # Drain effect processor
│   ├── FlinchEffectProcessor.cs        # Flinch effect processor
│   ├── ProtectEffectProcessor.cs       # Protect effect processor
│   ├── CounterEffectProcessor.cs       # Counter effect processor
│   ├── HealEffectProcessor.cs         # Heal effect processor
│   ├── IMoveModifier.cs                # Move modifier interface
│   ├── MoveModifier.cs                 # Move modifier implementation
│   ├── IAccumulativeEffect.cs         # Accumulative effect interface
│   └── AccumulativeEffectTracker.cs    # Accumulative effect tracker
│
├── Logging/
│   ├── IBattleLogger.cs                # Logger interface
│   ├── BattleLogger.cs                 # Logger implementation
│   └── NullBattleLogger.cs             # Null logger for tests
│
├── Messages/
│   ├── IBattleMessageFormatter.cs      # Message formatter interface
│   └── BattleMessageFormatter.cs       # Message formatter implementation
│
├── Validation/
│   ├── IBattleStateValidator.cs        # Battle state validator interface
│   └── BattleStateValidator.cs          # Battle state validator implementation
│
├── Extensions/
│   ├── BattleSlotExtensions.cs         # BattleSlot extension methods
│   └── DamageCalculationExtensions.cs   # Damage calculation extension methods
│
├── Constants/
│   ├── BattleConstants.cs               # Battle system constants
│   ├── StatusConstants.cs               # Status effect constants
│   ├── ItemConstants.cs                 # Item effect constants
│   └── MoveConstants.cs                 # Move-related constants
│
└── View/
    ├── IBattleView.cs                  # View interface
    └── NullBattleView.cs              # Null implementation
```

## Key Classes

### CombatEngine

**Namespace**: `PokemonUltimate.Combat`
**File**: `PokemonUltimate.Combat/Engine/CombatEngine.cs`
**Purpose**: Main battle controller orchestrating the full battle loop
**Key Properties**:

-   `Field` - The battlefield
-   `Queue` - Action queue
-   `Outcome` - Current battle outcome

**Key Methods**:

-   `Initialize(...)` - Set up battle with parties and providers
-   `RunBattle()` - Run complete battle until conclusion
-   `RunTurn()` - Execute a single turn

### BattleQueue

**Namespace**: `PokemonUltimate.Combat`
**File**: `PokemonUltimate.Combat/Engine/BattleQueue.cs`
**Purpose**: Manages sequential processing of battle actions
**Key Methods**:

-   `Enqueue(BattleAction)` - Add action to queue
-   `EnqueueRange(IEnumerable<BattleAction>)` - Add multiple actions
-   `ProcessQueue(BattleField, IBattleView)` - Process all actions

### BattleAction

**Namespace**: `PokemonUltimate.Combat.Actions`
**File**: `PokemonUltimate.Combat/Actions/BattleAction.cs`
**Purpose**: Base class for all battle actions
**Key Properties**:

-   `User` - Slot that initiated the action
-   `Priority` - Turn order priority modifier
-   `CanBeBlocked` - Whether action can be blocked

**Key Methods**:

-   `ExecuteLogic(BattleField)` - Execute game logic (instant)
-   `ExecuteVisual(IBattleView)` - Execute visual presentation (async)

### DamagePipeline

**Namespace**: `PokemonUltimate.Combat.Damage`
**File**: `PokemonUltimate.Combat/Damage/DamagePipeline.cs`
**Purpose**: Calculates damage using modular pipeline
**Key Methods**:

-   `CalculateDamage(DamageContext)` - Calculate final damage
    **Pipeline Steps**:

1. BaseDamageStep - Base damage calculation
2. CriticalHitStep - Critical hit detection (1.5x multiplier)
3. RandomFactorStep - Random damage variation (0.85-1.0)
4. StabStep - STAB bonus (1.5x for same type)
5. AttackerAbilityStep - Attacker ability multipliers
6. AttackerItemStep - Attacker item multipliers
7. WeatherStep - Weather damage modifiers (Sub-Feature 2.12)
8. TypeEffectivenessStep - Type effectiveness
9. BurnStep - Burn penalty (0.5x for physical moves)

### BattleField

**Namespace**: `PokemonUltimate.Combat.Field`
**File**: `PokemonUltimate.Combat/Field/BattleField.cs`
**Purpose**: Main battlefield container
**Key Properties**:

-   `PlayerSide` - Player's side
-   `EnemySide` - Enemy's side
-   `Rules` - Battle rules
-   `Weather` - Current weather condition (Sub-Feature 2.12)
-   `WeatherDuration` - Weather turn counter (Sub-Feature 2.12)
-   `WeatherData` - Weather data blueprint (Sub-Feature 2.12)

**Key Methods**:

-   `Initialize(BattleRules, parties...)` - Set up battlefield
-   `SetWeather(Weather, duration, WeatherData)` - Set weather condition (Sub-Feature 2.12)
-   `ClearWeather()` - Clear weather (Sub-Feature 2.12)
-   `DecrementWeatherDuration()` - Decrement weather duration (Sub-Feature 2.12)

### BattleSlot

**Namespace**: `PokemonUltimate.Combat.Field`
**File**: `PokemonUltimate.Combat/Field/BattleSlot.cs`
**Purpose**: Individual Pokemon slot on battlefield
**Key Properties**:

-   `Pokemon` - Pokemon instance in this slot
-   `ActionProvider` - Provider for actions (player/AI)
-   `IsActive` - Whether slot is active

## Factories & Builders

No factories/builders in combat system - uses direct instantiation and action providers.

## Integration Points

### With Game Data

-   Uses `PokemonInstance` from `PokemonUltimate.Core.Instances`
-   Uses `PokemonSpeciesData` from `PokemonUltimate.Core.Blueprints`
-   Uses `MoveData` from `PokemonUltimate.Core.Blueprints`

### With Content

-   Uses `MoveCatalog`, `AbilityCatalog`, `ItemCatalog` from `PokemonUltimate.Content.Catalogs`

### With Unity (Future)

-   `IBattleView` interface for visual presentation
-   `IActionProvider` interface for player input

## Test Location

**Tests**: `PokemonUltimate.Tests/Systems/Combat/`

-   **Functional**: `[Component]Tests.cs` - Normal behavior
-   **Edge Cases**: `[Component]EdgeCasesTests.cs` - Edge cases
-   **Integration**: `Integration/[Category]/` - System interactions
    -   `Actions/` - Action integration tests
    -   `Damage/` - Damage integration tests
    -   `Engine/` - Engine integration tests
    -   `System/` - Full system integration tests

See **[Testing](testing.md)** for complete test organization.

## Related Documents

-   **[Architecture](architecture.md)** - Technical design of combat system
-   **[Testing](testing.md)** - Testing strategy and test locations
-   **[Roadmap](roadmap.md)** - Implementation phases
-   **[Use Cases](use_cases.md)** - Scenarios implemented in this code
-   **[Feature 1: Game Data](../1-game-data/code_location.md)** - Game data code organization

---

**Last Updated**: 2025-01-XX
