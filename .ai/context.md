# 🤖 AI Context Summary

> **This file provides immediate context for the AI assistant.**
> Update this file after completing major features or making architectural decisions.

---

## 📍 Current Project State

| Aspect            | Status                       |
| ----------------- | ---------------------------- |
| **Current Phase** | Phase 3: Combat System 🚧    |
| **Sub-Phase**     | 2.2 Action Queue ✅ Complete |
| **Tests**         | 1,340 passing                |
| **Warnings**      | 0                            |
| **Last Updated**  | December 2025                |

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
├── Content/        # Game data definitions
│   ├── Catalogs/   # Pokemon & Move definitions
│   └── Builders/   # Fluent APIs
│
└── Tests/          # Mirror structure of Core/Content
```

---

## ✅ Completed Systems

### Phase 1: Core Data

-   [x] PokemonSpeciesData (blueprints)
-   [x] MoveData with effects composition
-   [x] BaseStats with validation
-   [x] Type system (18 types)
-   [x] Nature system (25 natures)

### Phase 2: Instances

-   [x] PokemonInstance (partial classes: Core, Battle, LevelUp, Evolution)
-   [x] MoveInstance with PP tracking and PP Ups
-   [x] StatCalculator (Gen 3+ formulas)
-   [x] TypeEffectiveness (Gen 6+ chart, STAB)
-   [x] Level up system with move learning
-   [x] Evolution system (Level, Item, Trade, Friendship)
-   [x] Registries (Pokemon, Move) with query methods

---

## 🎯 Combat System Progress

See `docs/combat_implementation_plan.md` for full details.

| Sub-Phase              | Status      | Description               |
| ---------------------- | ----------- | ------------------------- |
| 2.1 Battle Foundation  | ✅ Complete | BattleField, Slot, Side   |
| 2.2 Action Queue       | ✅ Complete | BattleQueue, BattleAction |
| 2.3 Turn Order         | 🎯 Next     | TurnOrderResolver         |
| 2.4 Damage Calculation | ⏳ Pending  | DamagePipeline            |
| 2.5 Combat Actions     | ⏳ Pending  | UseMoveAction, etc.       |
| 2.6 Combat Engine      | ⏳ Pending  | CombatEngine, Arbiter     |
| 2.7 Integration        | ⏳ Pending  | AI, full battles          |

Reference docs:

-   `docs/combat_implementation_plan.md` ← **Start here**
-   `docs/architecture/action_system_spec.md` ← **⭐ Action system**
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
