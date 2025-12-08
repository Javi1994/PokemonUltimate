# Sub-Feature 2.9: Abilities & Items

> Event-driven system, triggers - Ability and item activation system.

**Sub-Feature Number**: 2.9  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Abilities & Items implements a **unified handler registry system** for processing abilities, items, and move effects:

-   **CombatEffectHandlerRegistry**: Centralized registry for all handlers (34 handlers total)
-   **Ability Handlers** (4): ContactAbilityHandler, IntimidateHandler, MoxieHandler, SpeedBoostHandler
-   **Item Handlers** (3): LeftoversHandler, LifeOrbHandler, RockyHelmetHandler
-   **Move Effect Handlers** (12): StatusEffectHandler, StatChangeEffectHandler, RecoilEffectHandler, DrainEffectHandler, etc.
-   **Checker Handlers** (15): DamageApplicationHandler, StatusApplicationHandler, etc.
-   **Handler Interfaces**: IAbilityHandler, IItemHandler, IMoveEffectHandler, ICheckerHandler
-   **BattleEventManager**: Event system for statistics and logging (not game logic)

## Current Status

-   ✅ **Implemented**: Unified handler registry system with 34 handlers (refactored to step-based architecture with Handler Registry, 2024-12-05)
-   ✅ **Tested**: Comprehensive test coverage
-   ✅ **Architecture**: Handler Registry Pattern - handlers generate actions in response to triggers during turn flow steps

## Documentation

| Document                                                  | Purpose                    |
| --------------------------------------------------------- | -------------------------- |
| **[Architecture](../../architecture.md#abilities-items)** | Technical specification    |
| **[Use Cases](../../use_cases.md#abilities-items)**       | Ability and item scenarios |
| **[Roadmap](../../roadmap.md#phase-29-abilities-items)**  | Implementation details     |
| **[Testing](../../testing.md)**                           | Testing strategy           |
| **[Code Location](../../code_location.md)**               | Where the code lives       |

## Related Sub-Features

-   **[2.6: Combat Engine](../2.6-combat-engine/)** - Engine triggers ability/item events
-   **[2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Abilities/items provide data for stat/damage modifiers

## Quick Links

-   **Key Classes**: `CombatEffectHandlerRegistry`, `IAbilityHandler`, `IItemHandler`, `IMoveEffectHandler`, `ICheckerHandler`
-   **Status**: ✅ Complete (Phase 2.9)
-   **Handlers**: 34 total (4 abilities + 3 items + 12 effects + 15 checkers)

---

**Last Updated**: January 2025 (Handler Registry architecture refactor: 2024-12-05)
