# Feature 5: Game Features

> Game features beyond combat: progression, roguelike, meta-game.

**Feature Number**: 5  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This feature covers game systems beyond the battle engine:
- Post-battle rewards (EXP, level ups)
- Pokemon management (party, PC, catching)
- Progression system (roguelike runs, meta-progression)
- Encounter system (wild, trainer, boss battles)
- Item and inventory system
- Save system

**Status**: Planning phase

## Quick Links

- **Phases Planned**: 5.1-5.6 (Game features)
- **Key Classes**: See [code location](code_location.md) for planned structure
- **Tests**: See [testing](testing.md) for test strategy

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](5.1-post-battle-rewards/architecture.md)** | Post-battle rewards specification (main architecture in sub-feature) |
| **[Use Cases](use_cases.md)** | All scenarios for game features |
| **[Roadmap](roadmap.md)** | Game features phases (5.1-5.6) |
| **[Testing](testing.md)** | Testing strategy for game features |
| **[Code Location](code_location.md)** | Where game feature code will be located |

## Sub-Features

- **[5.1: Post-Battle Rewards](5.1-post-battle-rewards/)** - EXP calculation, level ups, rewards ⏳
- **[5.2: Pokemon Management](5.2-pokemon-management/)** - Party, PC, catching system ⏳
- **[5.3: Encounter System](5.3-encounter-system/)** - Wild, trainer, boss battles ⏳
- **[5.4: Inventory System](5.4-inventory-system/)** - Item management and usage ⏳
- **[5.5: Save System](5.5-save-system/)** - Save/load game progress ⏳
- **[5.6: Progression System](5.6-progression-system/)** - Roguelike runs, meta-progression ⏳

## Related Features

- **[Feature 2: Combat System](../2-combat-system/)** - Battle system used in encounters
- **[Feature 1: Game Data](../1-game-data/)** - Game data for catching and management
- **[Feature 3: Content Expansion](../3-content-expansion/)** - Items and Pokemon for encounters
- **[Feature 4: Unity Integration](../4-unity-integration/)** - Post-battle UI and game UI

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Related Documents

- **[Use Cases](use_cases.md)** - All scenarios for game features
- **[Roadmap](roadmap.md)** - Implementation phases (5.1-5.6)
- **[Testing](testing.md)** - Testing strategy for game features
- **[Code Location](code_location.md)** - Where game feature code will be located
- **[5.1 Architecture](5.1-post-battle-rewards/architecture.md)** - Post-battle rewards specification

---

**Last Updated**: 2025-01-XX

