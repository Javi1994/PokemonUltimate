# Sub-Feature 2.2: Action Queue System

> BattleQueue, BattleAction - Linear action processing system.

**Sub-Feature Number**: 2.2  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Action Queue System implements the core philosophy: "Everything is an Action". All battle events are processed as a linear sequence of actions through a queue.

**Key Components**:

-   **BattleQueueService**: Processes actions sequentially (uses `LinkedList<BattleAction>` internally)
-   **BattleAction**: Base class for all battle events (13 action types)
-   **Action Processing**: Two-phase execution (Logic + Visual)
-   **IBattleQueueFactory**: Factory interface for BattleQueueService creation

## Current Status

-   ✅ **Implemented**: BattleQueue and BattleAction base system with observer support
-   ✅ **Tested**: Comprehensive test coverage
-   ✅ **Extended**: Observer pattern support for statistics collection (Sub-Feature 2.20)

## Documentation

| Document                                                            | Purpose                     |
| ------------------------------------------------------------------- | --------------------------- |
| **[Architecture](../../architecture.md#2-the-action-queue-system)** | Technical specification     |
| **[Use Cases](../../use_cases.md)**                                 | Action processing scenarios |
| **[Roadmap](../../roadmap.md#phase-22-action-queue-system)**        | Implementation details      |
| **[Testing](../../testing.md)**                                     | Testing strategy            |
| **[Code Location](../../code_location.md)**                         | Where the code lives        |

## Related Sub-Features

-   **[2.5: Combat Actions](../2.5-combat-actions/)** - Action implementations
-   **[2.6: Combat Engine](../2.6-combat-engine/)** - Uses BattleQueue for battle execution
-   **[2.20: Statistics System](../2.20-statistics-system/)** - Uses BattleQueue observers for statistics collection

## Quick Links

-   **Key Classes**: `BattleQueueService`, `BattleAction` (13 action types)
-   **Status**: ✅ Complete (Phase 2.2)
-   **Action Types**: UseMoveAction, DamageAction, HealAction, ApplyStatusAction, FaintAction, StatChangeAction, SetWeatherAction, SetTerrainAction, SetSideConditionAction, SwitchAction, MessageAction, ContactDamageAction, BattleAction (base)

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-05)
