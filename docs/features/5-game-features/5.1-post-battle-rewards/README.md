# Sub-Feature 5.1: Post-Battle Rewards

> EXP calculation, level ups, rewards - Post-battle reward system.

**Sub-Feature Number**: 5.1  
**Parent Feature**: Feature 5: Game Features  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Post-Battle Rewards handles all rewards and progression after battles:
- **EXP Calculation**: EXP gained based on defeated Pokemon
- **Level Ups**: Pokemon level up when EXP threshold reached
- **Stat Increases**: Stat increases on level up
- **Move Learning**: Moves learned on level up
- **Rewards**: Items, money, other rewards

## Current Status

- ⏳ **Planned**: Post-battle rewards not yet implemented

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Victory/defeat system specification |
| **[Use Cases](../../use_cases.md#uc-001-calculate-exp-after-battle)** | Reward scenarios |
| **[Roadmap](../../roadmap.md#phase-51-post-battle-rewards-system)** | Implementation plan |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where reward code will be located |

## Related Sub-Features

- **[5.2: Pokemon Management](../5.2-pokemon-management/)** - Manages Pokemon receiving rewards
- **[5.6: Progression System](../5.6-progression-system/)** - Part of progression system

## Related Features

- **[Feature 2: Combat System](../../2-combat-system/architecture.md)** - Battle system providing battle results
- **[Feature 1: Pokemon Data](../../1-pokemon-data/architecture.md)** - Pokemon data for EXP and level ups

**⚠️ Always use numbered feature paths**: `../../[N]-[feature-name]/` instead of `../../feature-name/`

## Related Documents

- **[Parent Feature README](../README.md)** - Overview of Game Features
- **[Architecture](architecture.md)** - Victory/defeat system specification
- **[Parent Use Cases](../use_cases.md#uc-001-calculate-exp-after-battle)** - Reward scenarios
- **[Parent Roadmap](../roadmap.md#phase-51-post-battle-rewards-system)** - Implementation plan
- **[Parent Testing](../testing.md)** - Testing strategy
- **[Parent Code Location](../code_location.md)** - Where reward code will be located

## Quick Links

- **Key Classes**: `PostBattleService`, `ExpCalculator`, `LevelUpService`
- **Status**: ⏳ Planned (Phase 5.1)

---

**Last Updated**: 2025-01-XX

