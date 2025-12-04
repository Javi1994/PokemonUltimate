# Feature 2: Combat System - Code Location

> Where the combat system code lives and how it's organized.

**Feature Number**: 2  
**Feature Name**: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The combat system is organized into several key areas:
- **Engine** - Main battle controller and turn execution
- **Actions** - BattleAction implementations (moves, status, switching, etc.)
- **Damage** - Damage calculation pipeline
- **Field** - Battle field, slots, sides, rules
- **Events** - Ability and item trigger system
- **Helpers** - Utility classes (turn order, targeting, accuracy)
- **AI** - AI action providers
- **View** - Battle view interface for visual presentation

## Namespaces

### `PokemonUltimate.Combat`
**Purpose**: Main combat engine and core classes
**Key Classes**:
- `CombatEngine` - Main battle controller
- `BattleQueue` - Action queue processor
- `BattleArbiter` - Victory/defeat detection

### `PokemonUltimate.Combat.Actions`
**Purpose**: BattleAction implementations (base class from Sub-Feature 2.2)
**Key Classes**:
- `BattleAction` - Base action class (Sub-Feature 2.2)
- `UseMoveAction` - Move execution
- `DamageAction` - Damage application
- `ApplyStatusAction` - Status effect application
- `SetWeatherAction` - Weather condition changes (Sub-Feature 2.12)
- `SetTerrainAction` - Terrain condition changes (Sub-Feature 2.13)
- `HealAction` - HP restoration
- `SwitchAction` - Pokemon switching (includes entry hazard processing, Sub-Feature 2.14)
- `StatChangeAction` - Stat stage modifications
- `SwitchAction` - Pokemon switching
- `FaintAction` - Pokemon fainting
- `MessageAction` - Text messages
- `BattleActionType` - Action type enum

### `PokemonUltimate.Combat.Damage`
**Purpose**: Damage calculation pipeline (includes stat and damage modifiers)
**Key Classes**:
- `DamagePipeline` - Main damage calculator
- `DamageContext` - Damage calculation context
- `IDamageStep` - Pipeline step interface
- `IStatModifier` - Stat and damage modifier interface (abilities, items)
- `AbilityStatModifier` - Ability-based stat/damage modifier adapter
- `ItemStatModifier` - Item-based stat/damage modifier adapter

### `PokemonUltimate.Combat.Damage.Steps`
**Purpose**: Individual damage calculation steps
**Key Classes**:
- `BaseDamageStep` - Base damage calculation
- `CriticalHitStep` - Critical hit detection and multiplier
- `RandomFactorStep` - Random damage variation (0.85-1.0)
- `StabStep` - STAB (Same Type Attack Bonus) multiplier
- `AttackerAbilityStep` - Attacker ability effects
- `AttackerItemStep` - Attacker item effects
- `WeatherStep` - Weather damage modifiers (Sub-Feature 2.12)
- `TerrainStep` - Terrain damage modifiers (Sub-Feature 2.13)
- `TypeEffectivenessStep` - Type effectiveness calculation
- `BurnStep` - Burn status penalty for physical moves

### `PokemonUltimate.Combat.Field`
**Purpose**: Battle field management
**Key Classes**:
- `BattleField` - Main battlefield container (includes weather tracking - Sub-Feature 2.12, terrain tracking - Sub-Feature 2.13)
- `BattleSide` - Player or enemy side (includes entry hazard tracking - Sub-Feature 2.14)
- `BattleSlot` - Individual Pokemon slot
- `BattleRules` - Battle format rules

### `PokemonUltimate.Combat.Events`
**Purpose**: Event-driven ability and item system
**Key Classes**:
- `BattleTrigger` - Trigger type enum
- `IBattleListener` - Listener interface
- `AbilityListener` - Ability event handler
- `ItemListener` - Item event handler
- `BattleTriggerProcessor` - Trigger processor

### `PokemonUltimate.Combat.Engine`
**Purpose**: Engine components
**Key Classes**:
- `CombatEngine` - Main battle controller (includes weather duration decrement - Sub-Feature 2.12, terrain duration decrement - Sub-Feature 2.13)
- `EndOfTurnProcessor` - End-of-turn effects processor (includes weather damage - Sub-Feature 2.12, terrain healing - Sub-Feature 2.13)
- `EntryHazardProcessor` - Entry hazard processing on switch-in (Sub-Feature 2.14)

### `PokemonUltimate.Combat.Helpers`
**Purpose**: Utility classes
**Key Classes**:
- `TurnOrderResolver` - Turn order calculation
- `TargetResolver` - Target selection
- `AccuracyChecker` - Accuracy calculation (includes weather perfect accuracy - Sub-Feature 2.12)

### `PokemonUltimate.Combat.AI`
**Purpose**: AI action providers
**Key Classes**:
- `RandomAI` - Random action selection
- `AlwaysAttackAI` - Always attack AI

### `PokemonUltimate.Combat.Providers`
**Purpose**: Action provider interfaces
**Key Classes**:
- `IActionProvider` - Action provider interface
- `PlayerInputProvider` - Human player input provider

### `PokemonUltimate.Combat.View`
**Purpose**: Battle view interface
**Key Classes**:
- `IBattleView` - Battle view interface
- `NullBattleView` - Null implementation for testing

### `PokemonUltimate.Combat.Results`
**Purpose**: Battle results
**Key Classes**:
- `BattleOutcome` - Outcome enum
- `BattleResult` - Detailed battle result

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
│   ├── CombatEngine.cs                 # Main battle controller (weather duration - Sub-Feature 2.12)
│   ├── BattleQueue.cs                  # Action queue
│   ├── BattleArbiter.cs                # Victory/defeat detection
│   └── EndOfTurnProcessor.cs          # End-of-turn effects (weather damage - Sub-Feature 2.12)
│
├── Events/
│   ├── BattleTrigger.cs                # Trigger enum
│   ├── IBattleListener.cs              # Listener interface
│   ├── AbilityListener.cs              # Ability handler
│   ├── ItemListener.cs                 # Item handler
│   └── BattleTriggerProcessor.cs       # Trigger processor
│
├── Field/
│   ├── BattleField.cs                  # Main battlefield (weather tracking - Sub-Feature 2.12)
│   ├── BattleSide.cs                   # Player/enemy side
│   ├── BattleSlot.cs                   # Pokemon slot
│   └── BattleRules.cs                 # Battle rules
│
├── Helpers/
│   ├── TurnOrderResolver.cs           # Turn order calculation
│   ├── TargetResolver.cs              # Target selection
│   └── AccuracyChecker.cs             # Accuracy calculation (weather perfect accuracy - Sub-Feature 2.12)
│
├── Providers/
│   ├── IActionProvider.cs              # Action provider interface
│   └── PlayerInputProvider.cs          # Human input provider
│
├── Results/
│   ├── BattleOutcome.cs                # Outcome enum
│   └── BattleResult.cs                 # Result data
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
- `Field` - The battlefield
- `Queue` - Action queue
- `Outcome` - Current battle outcome

**Key Methods**:
- `Initialize(...)` - Set up battle with parties and providers
- `RunBattle()` - Run complete battle until conclusion
- `RunTurn()` - Execute a single turn

### BattleQueue
**Namespace**: `PokemonUltimate.Combat`
**File**: `PokemonUltimate.Combat/Engine/BattleQueue.cs`
**Purpose**: Manages sequential processing of battle actions
**Key Methods**:
- `Enqueue(BattleAction)` - Add action to queue
- `EnqueueRange(IEnumerable<BattleAction>)` - Add multiple actions
- `ProcessQueue(BattleField, IBattleView)` - Process all actions

### BattleAction
**Namespace**: `PokemonUltimate.Combat.Actions`
**File**: `PokemonUltimate.Combat/Actions/BattleAction.cs`
**Purpose**: Base class for all battle actions
**Key Properties**:
- `User` - Slot that initiated the action
- `Priority` - Turn order priority modifier
- `CanBeBlocked` - Whether action can be blocked

**Key Methods**:
- `ExecuteLogic(BattleField)` - Execute game logic (instant)
- `ExecuteVisual(IBattleView)` - Execute visual presentation (async)

### DamagePipeline
**Namespace**: `PokemonUltimate.Combat.Damage`
**File**: `PokemonUltimate.Combat/Damage/DamagePipeline.cs`
**Purpose**: Calculates damage using modular pipeline
**Key Methods**:
- `CalculateDamage(DamageContext)` - Calculate final damage
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
- `PlayerSide` - Player's side
- `EnemySide` - Enemy's side
- `Rules` - Battle rules
- `Weather` - Current weather condition (Sub-Feature 2.12)
- `WeatherDuration` - Weather turn counter (Sub-Feature 2.12)
- `WeatherData` - Weather data blueprint (Sub-Feature 2.12)

**Key Methods**:
- `Initialize(BattleRules, parties...)` - Set up battlefield
- `SetWeather(Weather, duration, WeatherData)` - Set weather condition (Sub-Feature 2.12)
- `ClearWeather()` - Clear weather (Sub-Feature 2.12)
- `DecrementWeatherDuration()` - Decrement weather duration (Sub-Feature 2.12)

### BattleSlot
**Namespace**: `PokemonUltimate.Combat.Field`
**File**: `PokemonUltimate.Combat/Field/BattleSlot.cs`
**Purpose**: Individual Pokemon slot on battlefield
**Key Properties**:
- `Pokemon` - Pokemon instance in this slot
- `ActionProvider` - Provider for actions (player/AI)
- `IsActive` - Whether slot is active

## Factories & Builders

No factories/builders in combat system - uses direct instantiation and action providers.

## Integration Points

### With Game Data
- Uses `PokemonInstance` from `PokemonUltimate.Core.Instances`
- Uses `PokemonSpeciesData` from `PokemonUltimate.Core.Blueprints`
- Uses `MoveData` from `PokemonUltimate.Core.Blueprints`

### With Content
- Uses `MoveCatalog`, `AbilityCatalog`, `ItemCatalog` from `PokemonUltimate.Content.Catalogs`

### With Unity (Future)
- `IBattleView` interface for visual presentation
- `IActionProvider` interface for player input

## Test Location

**Tests**: `PokemonUltimate.Tests/Systems/Combat/`
- **Functional**: `[Component]Tests.cs` - Normal behavior
- **Edge Cases**: `[Component]EdgeCasesTests.cs` - Edge cases
- **Integration**: `Integration/[Category]/` - System interactions
  - `Actions/` - Action integration tests
  - `Damage/` - Damage integration tests
  - `Engine/` - Engine integration tests
  - `System/` - Full system integration tests

See **[Testing](testing.md)** for complete test organization.

## Related Documents

- **[Architecture](architecture.md)** - Technical design of combat system
- **[Testing](testing.md)** - Testing strategy and test locations
- **[Roadmap](roadmap.md)** - Implementation phases
- **[Use Cases](use_cases.md)** - Scenarios implemented in this code
- **[Feature 1: Game Data](../1-game-data/code_location.md)** - Game data code organization

---

**Last Updated**: 2025-01-XX

