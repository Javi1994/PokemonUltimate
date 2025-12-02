# 🤖 AI Context Summary

> **This file provides immediate context for the AI assistant.**
> Update this file after completing major features or making architectural decisions.

---

## 📍 Current Project State

| Aspect            | Status                      |
| ----------------- | --------------------------- |
| **Current Phase** | Phase 3: Combat System ✅   |
| **Sub-Phase**     | 2.7 Integration ✅ Complete |
| **Tests**         | 2,038 passing               |
| **Warnings**      | 0                           |
| **Last Updated**  | December 2025               |

---

## 🏗️ Architecture Overview

```
PokemonUltimate/
├── Core/           # Game logic (DO NOT add game data here)
│   ├── Blueprints/ # Immutable data structures
│   ├── Instances/  # Mutable runtime state
│   ├── Factories/  # Object creation
│   ├── Effects/    # Move effects (IMoveEffect)
│   ├── Evolution/  # Evolution conditions
│   ├── Registry/   # Data access layer
│   ├── Enums/      # Type definitions
│   └── Constants/  # Centralized strings
│
├── Combat/         # Battle system (depends on Core)
│   ├── Field/      # BattleField, BattleSide, BattleSlot, BattleRules
│   ├── Engine/     # CombatEngine, BattleArbiter, BattleQueue
│   ├── Results/    # BattleOutcome, BattleResult
│   ├── Providers/  # IActionProvider
│   ├── View/       # IBattleView, NullBattleView
│   ├── Actions/    # BattleAction implementations
│   ├── Damage/     # DamagePipeline, DamageContext
│   ├── AI/         # RandomAI, AlwaysAttackAI
│   └── Helpers/    # AccuracyChecker, TurnOrderResolver, TargetResolver
│
├── Content/        # Game data definitions
│   ├── Catalogs/   # Pokemon, Move, Ability, Item definitions
│   └── Builders/   # Fluent APIs
│
├── Tests/          # Mirror structure of Core/Combat/Content
│
└── BattleDemo/     # Visual AI vs AI battle simulator
    ├── ConsoleBattleView.cs  # Console implementation of IBattleView
    └── Program.cs            # Battle scenarios and debug display
```

---

## ✅ Completed Systems

### Phase 1: Core Data

-   [x] PokemonSpeciesData (blueprints)
-   [x] MoveData with effects composition
-   [x] BaseStats with validation
-   [x] Type system (18 types)
-   [x] Nature system (25 natures)
-   [x] AbilityData (25 Gen3 abilities + additional)
-   [x] ItemData (23 items: held items, berries)
-   [x] StatusEffectData (15 status effects: 6 persistent + 9 volatile)
-   [x] WeatherData (9 weather conditions: 5 standard + 3 primal + fog)
-   [x] TerrainData (4 terrains: Grassy, Electric, Psychic, Misty)
-   [x] HazardData (4 hazards: Stealth Rock, Spikes, Toxic Spikes, Sticky Web)
-   [x] SideConditionData (10 conditions: screens, Tailwind, protections)
-   [x] FieldEffectData (8 effects: rooms, Gravity, sports)
-   [x] **Move Effects System** (21 effect types: damage, status, field, etc.)
    -   See `docs/architecture/effects_bible.md` for complete reference

### Phase 2: Instances

-   [x] PokemonInstance (partial classes: Core, Battle, LevelUp, Evolution)
-   [x] MoveInstance with PP tracking and PP Ups
-   [x] StatCalculator (Gen 3+ formulas)
-   [x] TypeEffectiveness (Gen 6+ chart, STAB)
-   [x] Level up system with move learning
-   [x] Evolution system (Level, Item, Trade, Friendship)
-   [x] Registries (Pokemon, Move) with query methods
-   [x] **Ability & Item linked to PokemonInstance** ← NEW
    -   Species define Ability1, Ability2, HiddenAbility
    -   Instance has assigned Ability (random or specified)
    -   Instance can hold ItemData
    -   All Gen1 Pokemon in catalog have abilities assigned

---

## 🎯 Combat System Progress

See `docs/combat_implementation_plan.md` for full details.

| Sub-Phase              | Status      | Description                                            |
| ---------------------- | ----------- | ------------------------------------------------------ |
| 2.1 Battle Foundation  | ✅ Complete | BattleField, Slot, Side                                |
| 2.2 Action Queue       | ✅ Complete | BattleQueue, BattleAction                              |
| 2.3 Turn Order         | ✅ Complete | TurnOrderResolver                                      |
| 2.4 Damage Calculation | ✅ Complete | DamagePipeline                                         |
| 2.5 Combat Actions     | ✅ Complete | All actions implemented                                |
| 2.6 Combat Engine      | ✅ Complete | CombatEngine, Arbiter                                  |
| 2.7 Integration        | ✅ Complete | RandomAI, AlwaysAttackAI, TargetResolver, Full battles |

Reference docs:

-   `docs/combat_implementation_plan.md` ← **Start here**
-   `docs/architecture/action_system_spec.md` ← **⭐ Action system**
-   `docs/combat/action_use_cases.md` ← **📋 Use cases (207 cases)**
-   `docs/combat/actions_bible.md` ← **📖 Actions reference**
-   `docs/architecture/combat_system_spec.md`
-   `docs/architecture/damage_and_effect_system.md`

---

## 📐 Key Architectural Decisions

| Decision                            | Rationale                                           |
| ----------------------------------- | --------------------------------------------------- |
| Blueprint/Instance pattern          | Immutable data vs mutable runtime state             |
| Partial classes for PokemonInstance | File size management, separation of concerns        |
| Nullable disabled in Tests/Content  | Practical for testing patterns, Unity compatibility |
| Centralized constants               | No magic strings, easy maintenance                  |
| Fail-fast exceptions                | Clear error detection, no silent failures           |
| IMoveEffect composition             | Moves can have multiple effects                     |

---

## ⚠️ Important Conventions

### Naming

-   Private fields: `_camelCase`
-   Properties/Methods: `PascalCase`
-   Constants: `PascalCase`
-   Interfaces: `IName`

### Error Handling

-   Use `ErrorMessages` constants for exceptions
-   Throw exceptions for invalid states (no try-catch unless necessary)
-   Validate at public API boundaries

### Testing

-   Functional tests first, then edge cases
-   Test file mirrors source file location
-   Use descriptive test names: `MethodName_Scenario_ExpectedResult`

---

## 📚 Key Reference Documents

| Document                                  | Purpose              |
| ----------------------------------------- | -------------------- |
| `docs/project_guidelines.md`              | 24+ coding rules     |
| `docs/implementation_plan.md`             | Technical roadmap    |
| `docs/combat_implementation_plan.md`      | **Combat phases**    |
| `docs/combat_use_cases.md`                | **All battle cases** |
| `docs/architecture/action_system_spec.md` | **⭐ Action system** |
| `docs/architecture/effects_bible.md`      | **📖 Effects guide** |
| `docs/unity_integration.md`               | Unity setup guide    |
| `CONTRIBUTING.md`                         | Git workflow & rules |
| `docs/architecture/combat_system_spec.md` | Combat system design |
| `docs/checklists/pre_implementation.md`   | **Before coding**    |
| `docs/checklists/feature_complete.md`     | After coding         |
| `docs/anti-patterns.md`                   | What NOT to do       |

---

## 🔄 How to Update This File

After completing a major feature:

1. Update "Current Project State" section
2. Move items from "Next" to "Completed"
3. Add any new architectural decisions
4. Update test count
5. Update "Last Updated" date
