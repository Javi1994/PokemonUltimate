# Feature 2: Combat System

> Complete Pokemon battle engine implementation.

**Feature Number**: 2  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This feature implements the core Pokemon battle system, including:

-   Battle field management (slots, sides, field conditions)
-   Turn order resolution
-   Damage calculation pipeline
-   Action system (moves, status, healing, switching)
-   End-of-turn effects processing
-   Abilities and items integration

**Status**: Core phases complete (2.1-2.9, 2.11), extended phases implemented (2.12-2.14, 2.16)

> **📋 Refactoring Completed (2024-12-05)**: A comprehensive refactoring was completed following SOLID principles. All major components now use Dependency Injection, Value Objects, Strategy Pattern, Factory Pattern, Event System, Logging, and Validation. See `PokemonUltimate.Combat/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for details.

## Current Status

-   ✅ **Core Complete**: Battle foundation, actions, turn order, damage, engine
-   ✅ **Integration**: Abilities, items, stat/damage modifiers, recoil/drain
-   ✅ **Field Systems**: Weather, terrain, hazards, field conditions (screens, Tailwind, etc.)
-   ✅ **Advanced Moves**: Protect, Counter, Pursuit, Focus Punch, Semi-Invulnerable, Multi-Hit, Multi-Turn (2.15 Core Complete)
-   ✅ **Advanced Abilities**: Truant, Speed Boost, Static, Rough Skin, Swift Swim, Chlorophyll, Moxie (2.17 ~95% Complete)
-   ✅ **Advanced Items**: Life Orb, Focus Sash, Rocky Helmet, Black Sludge (2.18 Complete)
-   ⏳ **Planned**: Battle formats (2.19)

## Documentation

| Document                              | Purpose                                     |
| ------------------------------------- | ------------------------------------------- |
| **[Architecture](architecture.md)**   | Main combat system specification            |
| **[Use Cases](use_cases.md)**         | All battle scenarios and cases              |
| **[Roadmap](roadmap.md)**             | Implementation phases (2.1-2.19)            |
| **[Testing](testing.md)**             | Comprehensive testing strategy              |
| **[Code Location](code_location.md)** | Where the code lives and how it's organized |

## Sub-Features

### Core Complete (2.1-2.11)

-   **[2.1: Battle Foundation](2.1-battle-foundation/)** - BattleField, Slots, Sides, Rules ✅
-   **[2.2: Action Queue System](2.2-action-queue-system/)** - BattleQueue, BattleAction ✅
-   **[2.3: Turn Order Resolution](2.3-turn-order-resolution/)** - Priority, Speed, Random sorting ✅
-   **[2.4: Damage Calculation Pipeline](2.4-damage-calculation-pipeline/)** - Modular damage calculation ✅
-   **[2.5: Combat Actions](2.5-combat-actions/)** - UseMove, Damage, Status, Heal, Switch, Faint ✅
-   **[2.6: Combat Engine](2.6-combat-engine/)** - Battle loop, turn execution, outcome detection ✅
-   **[2.7: Integration](2.7-integration/)** - AI providers, Player input, Full battles ✅
-   **[2.8: End-of-Turn Effects](2.8-end-of-turn-effects/)** - Status damage, effects processing ✅
-   **[2.9: Abilities & Items](2.9-abilities-items/)** - Event-driven system, triggers ✅
-   **[2.11: Recoil & Drain](2.11-recoil-drain/)** - Recoil damage, HP drain effects ✅

### Implemented (2.12-2.16)

-   **[2.12: Weather System](2.12-weather-system/)** - Weather conditions and effects ✅
-   **[2.13: Terrain System](2.13-terrain-system/)** - Terrain conditions and effects ✅
-   **[2.14: Hazards System](2.14-hazards-system/)** - Stealth Rock, Spikes, etc. ✅ Core Complete
-   **[2.15: Advanced Move Mechanics](2.15-advanced-move-mechanics/)** - Protect, Counter, Pursuit, Focus Punch, Semi-Invulnerable, Multi-Hit, Multi-Turn ✅ Core Complete
-   **[2.16: Field Conditions](2.16-field-conditions/)** - Screens, Tailwind, protections ✅ Core Complete

### Planned (2.17-2.19)

-   **[2.17: Advanced Abilities](2.17-advanced-abilities/)** - Complex ability interactions ⏳
-   **[2.18: Advanced Items](2.18-advanced-items/)** - Complex item interactions ⏳
-   **[2.19: Battle Formats](2.19-battle-formats/)** - Doubles, Triples, Rotation ⏳

## Related Features

-   **[Feature 1: Game Data](../1-game-data/)** - Pokemon instances used in battles
-   **[Feature 3: Content Expansion](../3-content-expansion/)** - Moves, abilities, items
-   **[Feature 4: Unity Integration](../4-unity-integration/)** - Visual battle presentation

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Quick Links

-   **Phases Complete**: 2.1-2.9, 2.11, 2.12, 2.13, 2.14, 2.15, 2.16 (Core combat + Weather + Terrain + Hazards + Advanced Moves + Field Conditions - note: 2.10 consolidated into 2.4)
-   **Phases Planned**: 2.17-2.19 (Advanced Abilities, Advanced Items, Battle Formats)
-   **Tests**: 2,528+ passing tests (see [testing](testing.md))
-   **Key Classes**: See [code location](code_location.md) for implementation details
-   **Use Cases**: See [use_cases.md](use_cases.md) for all battle scenarios

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-05)
