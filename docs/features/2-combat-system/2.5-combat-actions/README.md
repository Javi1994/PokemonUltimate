# Sub-Feature 2.5: Combat Actions

> UseMove, Damage, Status, Heal, Switch, Faint - All battle action implementations.

**Sub-Feature Number**: 2.5  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Combat Actions are the concrete implementations of `BattleAction` (base class from Sub-Feature 2.2), including:
- **UseMoveAction**: Execute a move (refactored with Strategy Pattern for effects, DI for dependencies, post-refactor)
- **DamageAction**: Apply damage
- **ApplyStatusAction**: Apply status conditions
- **HealAction**: Restore HP
- **SwitchAction**: Switch Pokemon (uses IEntryHazardProcessor, post-refactor)
- **FaintAction**: Handle fainting
- **StatChangeAction**: Modify stat stages
- **MessageAction**: Display battle messages
- **MoveEffectProcessorRegistry**: Strategy Pattern registry for processing move effects (post-refactor)

## Current Status

- ✅ **Implemented**: All core action types (refactored with Strategy Pattern and DI, 2024-12-05)
- ✅ **Tested**: Comprehensive test coverage
- ✅ **Refactored**: UseMoveAction uses MoveEffectProcessorRegistry (Strategy Pattern), all dependencies injected

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete action system specification and technical reference |
| **[Use Cases](use_cases.md)** | All action use cases and coverage verification |
| **[Move Effects Execution](effects/architecture.md)** | How effects generate BattleActions in combat |
| **[Roadmap](../../roadmap.md#phase-25-combat-actions)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.2: Action Queue System](../2.2-action-queue-system/)** - Actions are processed by queue
- **[2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Damage actions use pipeline
- **[2.8: End-of-Turn Effects](../2.8-end-of-turn-effects/)** - Status actions integrate with status system

## Quick Links

- **Key Classes**: `UseMoveAction`, `DamageAction`, `ApplyStatusAction`, `HealAction`, `SwitchAction`, `FaintAction`
- **Status**: ✅ Complete (Phase 2.5)

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-05)

