# Sub-Feature 2.6: Combat Engine

> Battle loop, turn execution, outcome detection - Main battle controller.

**Sub-Feature Number**: 2.6  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Combat Engine orchestrates the entire battle using a **step-based pipeline architecture**:

-   **CombatEngine**: Main battle controller (orchestrates Battle Flow: 8 steps)
-   **TurnEngine**: Turn execution engine (orchestrates Turn Flow: 23 unique steps, 34 total)
-   **BattleFlowExecutor**: Executes battle flow steps
-   **TurnStepExecutor**: Executes turn flow steps
-   **BattleArbiter**: Victory/defeat detection
-   **Battle Outcome**: Determines battle result
-   **Dependencies**: Uses factories for BattleField and BattleQueueService creation, injects handlers, validators, and helpers

## Architecture

The engine uses a **step-based pipeline architecture** with two levels:

### Battle Flow (8 Steps)

High-level battle lifecycle managed by `CombatEngine`:

1. **CreateFieldStep** - Creates BattleField with both sides
2. **AssignActionProvidersStep** - Assigns action providers to battle sides
3. **CreateQueueStep** - Creates BattleQueueService
4. **ValidateInitialStateStep** - Validates initial battle state
5. **CreateDependenciesStep** - Creates TurnEngine and dependencies
6. **BattleStartFlowStep** - Executes battle start effects
7. **ExecuteBattleLoopStep** - Executes main battle loop
8. **BattleEndFlowStep** - Handles battle end

### Turn Flow (23 Unique Steps, 34 Total)

Detailed turn execution managed by `TurnEngine`:

-   **Preparation**: TurnStart, ActionCollection, TargetResolution, ActionSorting
-   **Move Validation**: MoveValidation, MoveProtectionCheck, MoveAccuracyCheck
-   **Before Move Effects**: BeforeMoveEffects, ProcessGeneratedActions
-   **Damage**: MoveDamageCalculation, MoveDamageApplication, ProcessGeneratedActions
-   **Animations**: MoveAnimation
-   **Reactive Effects**: DamageTakenEffects, ContactReceivedEffects, ProcessGeneratedActions
-   **Move Effects**: MoveEffectProcessing, ProcessGeneratedActions
-   **After Move Effects**: AfterMoveEffects, ProcessGeneratedActions
-   **Other Actions**: SwitchActions, SwitchInEffects, StatusActions, ProcessGeneratedActions
-   **Fainted Check**: FaintedPokemonCheck (appears 3 times)
-   **End of Turn**: EndOfTurnEffects, DurationDecrement, TurnEnd, ProcessGeneratedActions

Steps implement interfaces:

-   `IBattleFlowStep` - Base interface for battle flow steps
-   `ITurnStep` - Base interface for turn flow steps

## Current Status

-   ✅ **Implemented**: Complete battle engine with step-based pipeline architecture (refactored 2024-12-05)
-   ✅ **Tested**: Comprehensive test coverage
-   ✅ **Refactored**: Step-based pipeline architecture (Battle Flow + Turn Flow), improved separation of concerns and maintainability

## Documentation

| Document                                                | Purpose                 |
| ------------------------------------------------------- | ----------------------- |
| **[Architecture](../../architecture.md#combat-engine)** | Technical specification |
| **[Use Cases](../../use_cases.md#turn-flow)**           | Battle flow scenarios   |
| **[Roadmap](../../roadmap.md#phase-26-combat-engine)**  | Implementation details  |
| **[Testing](../../testing.md)**                         | Testing strategy        |
| **[Code Location](../../code_location.md)**             | Where the code lives    |

## Related Sub-Features

-   **[2.2: Action Queue System](../2.2-action-queue-system/)** - Engine uses queue for action processing
-   **[2.3: Turn Order Resolution](../2.3-turn-order-resolution/)** - Engine uses turn order resolver
-   **[2.5: Combat Actions](../2.5-combat-actions/)** - Engine executes actions

## Quick Links

-   **Key Classes**: `CombatEngine`, `TurnEngine`, `BattleFlowExecutor`, `TurnStepExecutor`, `BattleArbiter`
-   **Status**: ✅ Complete (Phase 2.6)
-   **Architecture**: Step-based pipeline (Battle Flow: 8 steps, Turn Flow: 23 unique steps, 34 total)

---

**Last Updated**: January 2025 (Step-based pipeline architecture refactor: 2024-12-05)
