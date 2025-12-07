# Sub-Feature 2.6: Combat Engine

> Battle loop, turn execution, outcome detection - Main battle controller.

**Sub-Feature Number**: 2.6  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Combat Engine orchestrates the entire battle, including:

-   **CombatEngine**: Main battle loop controller (orchestrates battle loop and turn execution)
-   **TurnExecutor**: Orchestrates single turn execution using phase-based processors
-   **BattleArbiter**: Victory/defeat detection
-   **Phase Processors**: Specialized processors for each battle phase/moment
-   **ActionProcessorObserver**: Observer pattern for reactive processors (damage, contact, weather, switch-in)
-   **Battle Outcome**: Determines battle result
-   **Dependencies**: Uses factories for BattleField and BattleQueue creation, injects processors and helpers

## Architecture

The engine uses a **phase-based processor system** where each battle phase/moment has a dedicated processor:

-   **Turn Phases**: `TurnStartProcessor`, `ActionCollectionProcessor`, `ActionSortingProcessor`, `FaintedPokemonSwitchingProcessor`, `EndOfTurnEffectsProcessor`, `TurnEndProcessor`, `DurationDecrementProcessor`
-   **Reactive Phases**: `SwitchInProcessor`, `BeforeMoveProcessor`, `AfterMoveProcessor`, `ContactReceivedProcessor`, `DamageTakenProcessor`, `WeatherChangeProcessor`
-   **Battle Phases**: `BattleStartProcessor`, `BattleEndProcessor`

Processors are organized by the `BattlePhase` enum and implement interfaces:

-   `IBattlePhaseProcessor` - Base interface
-   `IActionGeneratingPhaseProcessor` - For processors that generate actions
-   `IStateModifyingPhaseProcessor` - For processors that modify state directly

The `ActionProcessorObserver` detects specific action types (`DamageAction`, `SwitchAction`, `SetWeatherAction`) and delegates to appropriate reactive processors, decoupling actions from processors.

## Current Status

-   ✅ **Implemented**: Complete battle engine with phase-based processors (refactored 2024-12-07)
-   ✅ **Tested**: Comprehensive test coverage
-   ✅ **Refactored**: Phase-based architecture, improved separation of concerns and maintainability

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

-   **Key Classes**: `CombatEngine`, `BattleArbiter`
-   **Status**: ✅ Complete (Phase 2.6)

---

**Last Updated**: 2024-12-07 (Phase-based processors refactor)
