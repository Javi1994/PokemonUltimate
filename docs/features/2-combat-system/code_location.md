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
**Purpose**: BattleAction implementations
**Key Classes**:
- `BattleAction` - Base action class
- `UseMoveAction` - Move execution
- `DamageAction` - Damage application
- `ApplyStatusAction` - Status effect application
- `HealAction` - HP restoration
- `StatChangeAction` - Stat stage modifications
- `SwitchAction` - Pokemon switching
- `FaintAction` - Pokemon fainting
- `MessageAction` - Text messages
- `BattleActionType` - Action type enum

### `PokemonUltimate.Combat.Damage`
**Purpose**: Damage calculation pipeline
**Key Classes**:
- `DamagePipeline` - Main damage calculator
- `DamageContext` - Damage calculation context
- `IDamageStep` - Pipeline step interface
- `IStatModifier` - Stat modifier interface (abilities, items)
- `AbilityStatModifier` - Ability-based stat modifier
- `ItemStatModifier` - Item-based stat modifier

### `PokemonUltimate.Combat.Damage.Steps`
**Purpose**: Individual damage calculation steps
**Key Classes**:
- `BaseDamageStep` - Base damage calculation
- `AttackerAbilityStep` - Attacker ability effects
- `AttackerItemStep` - Attacker item effects
- `DefenderAbilityStep` - Defender ability effects
- `DefenderItemStep` - Defender item effects
- `TypeEffectivenessStep` - Type effectiveness calculation
- `RandomStep` - Random damage variation

### `PokemonUltimate.Combat.Field`
**Purpose**: Battle field management
**Key Classes**:
- `BattleField` - Main battlefield container
- `BattleSide` - Player or enemy side
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
- `EndOfTurnProcessor` - End-of-turn effects processor

### `PokemonUltimate.Combat.Helpers`
**Purpose**: Utility classes
**Key Classes**:
- `TurnOrderResolver` - Turn order calculation
- `TargetResolver` - Target selection
- `AccuracyChecker` - Accuracy calculation

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
│       ├── AttackerAbilityStep.cs      # Attacker abilities
│       ├── AttackerItemStep.cs         # Attacker items
│       ├── DefenderAbilityStep.cs      # Defender abilities
│       ├── DefenderItemStep.cs         # Defender items
│       ├── TypeEffectivenessStep.cs    # Type effectiveness
│       └── RandomStep.cs               # Random variation
│
├── Engine/
│   ├── CombatEngine.cs                 # Main battle controller
│   ├── BattleQueue.cs                  # Action queue
│   ├── BattleArbiter.cs                # Victory/defeat detection
│   └── EndOfTurnProcessor.cs          # End-of-turn effects
│
├── Events/
│   ├── BattleTrigger.cs                # Trigger enum
│   ├── IBattleListener.cs              # Listener interface
│   ├── AbilityListener.cs              # Ability handler
│   ├── ItemListener.cs                 # Item handler
│   └── BattleTriggerProcessor.cs       # Trigger processor
│
├── Field/
│   ├── BattleField.cs                  # Main battlefield
│   ├── BattleSide.cs                   # Player/enemy side
│   ├── BattleSlot.cs                   # Pokemon slot
│   └── BattleRules.cs                 # Battle rules
│
├── Helpers/
│   ├── TurnOrderResolver.cs           # Turn order calculation
│   ├── TargetResolver.cs              # Target selection
│   └── AccuracyChecker.cs             # Accuracy calculation
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
1. BaseDamageStep - Base damage
2. AttackerAbilityStep - Attacker abilities
3. AttackerItemStep - Attacker items
4. DefenderAbilityStep - Defender abilities
5. DefenderItemStep - Defender items
6. TypeEffectivenessStep - Type effectiveness
7. RandomStep - Random variation

### BattleField
**Namespace**: `PokemonUltimate.Combat.Field`
**File**: `PokemonUltimate.Combat/Field/BattleField.cs`
**Purpose**: Main battlefield container
**Key Properties**:
- `PlayerSide` - Player's side
- `EnemySide` - Enemy's side
- `Rules` - Battle rules

**Key Methods**:
- `Initialize(BattleRules, parties...)` - Set up battlefield

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

### With Pokemon Data
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
- **[Feature 1: Pokemon Data](../1-pokemon-data/code_location.md)** - Pokemon data code organization

---

**Last Updated**: 2025-01-XX

