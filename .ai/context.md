# 🤖 AI Context Summary

> **This file provides immediate context for the AI assistant.**
> Update this file after completing major features or making architectural decisions.

---

## 📍 Current Project State

| Aspect                  | Status                                                                                                                            |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| **Current Phase**       | Phase 3: Combat System ✅                                                                                                         |
| **Sub-Phase**           | 2.10 Pipeline Hooks ✅ Extended                                                                                                   |
| **Tests**               | 2,460 passing                                                                                                                     |
| **Integration Tests**   | 83+ tests (system interactions)                                                                                                   |
| **Test Reorganization** | ✅ Complete - All phases finished (62 individual catalog files: 26 Pokemon 100%, 36 Moves 100%). Redundant grouped tests removed. |
| **Warnings**            | 0                                                                                                                                 |
| **Pokemon Catalog**     | 26 Pokemon (Gen1)                                                                                                                 |
| **Move Catalog**        | 36 Moves (12 types)                                                                                                               |
| **Last Updated**        | December 2025                                                                                                                     |

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
│   ├── Engine/     # CombatEngine, BattleArbiter, BattleQueue, EndOfTurnProcessor
│   ├── Events/     # BattleTrigger, IBattleListener, AbilityListener, ItemListener, BattleTriggerProcessor
│   ├── Damage/Steps/ # BaseDamageStep, AttackerAbilityStep, AttackerItemStep, etc.
│   ├── Results/    # BattleOutcome, BattleResult
│   ├── Providers/  # IActionProvider, PlayerInputProvider
│   ├── View/       # IBattleView, NullBattleView (with input methods)
│   ├── Actions/    # BattleAction implementations, BattleActionType
│   ├── Damage/     # DamagePipeline, DamageContext, IStatModifier, AbilityStatModifier, ItemStatModifier
│   ├── AI/         # RandomAI, AlwaysAttackAI
│   └── Helpers/    # AccuracyChecker, TurnOrderResolver, TargetResolver
│
├── Content/        # Game data definitions
│   ├── Catalogs/   # Pokemon, Move, Ability, Item definitions
│   └── Builders/   # Fluent APIs
│
├── Tests/          # Organized by purpose: Systems/, Blueprints/, Data/
│   ├── Systems/    # Tests de sistemas (CÓMO funcionan)
│   ├── Blueprints/ # Tests de estructura de datos (CÓMO son)
│   └── Data/       # Tests de contenido específico (QUÉ contienen)
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

| Sub-Phase               | Status      | Description                                                                                                   |
| ----------------------- | ----------- | ------------------------------------------------------------------------------------------------------------- |
| 2.1 Battle Foundation   | ✅ Complete | BattleField, Slot, Side                                                                                       |
| 2.2 Action Queue        | ✅ Complete | BattleQueue, BattleAction                                                                                     |
| 2.3 Turn Order          | ✅ Complete | TurnOrderResolver                                                                                             |
| 2.4 Damage Calculation  | ✅ Complete | DamagePipeline                                                                                                |
| 2.5 Combat Actions      | ✅ Complete | All actions implemented                                                                                       |
| 2.6 Combat Engine       | ✅ Complete | CombatEngine, Arbiter                                                                                         |
| 2.7 Integration         | ✅ Complete | RandomAI, AlwaysAttackAI, TargetResolver, Full battles                                                        |
| 2.8 End-of-Turn Effects | ✅ Complete | EndOfTurnProcessor, Status damage (Burn/Poison/Toxic)                                                         |
| 2.9 Abilities & Items   | ✅ Complete | BattleTrigger system, AbilityListener, ItemListener, Leftovers, Intimidate                                    |
| 2.10 Pipeline Hooks     | ✅ Extended | IStatModifier system, Choice Band/Specs/Scarf, Life Orb, Assault Vest, Eviolite, Blaze/Torrent/Overgrow/Swarm |
| 2.11 Recoil & Drain     | ✅ Complete | RecoilEffect (25%, 33%, 50%), DrainEffect (50%, 75%)                                                          |

Reference docs:

-   `docs/combat_implementation_plan.md` ← **Start here**
-   `docs/architecture/action_system_spec.md` ← **⭐ Action system**
-   `docs/combat/action_use_cases.md` ← **📋 Use cases (207 cases)**
-   `docs/combat/actions_bible.md` ← **📖 Actions reference**
-   `docs/architecture/combat_system_spec.md`
-   `docs/architecture/damage_and_effect_system.md`

---

## 📐 Key Architectural Decisions

| Decision                            | Rationale                                                                               |
| ----------------------------------- | --------------------------------------------------------------------------------------- |
| Blueprint/Instance pattern          | Immutable data vs mutable runtime state                                                 |
| Partial classes for PokemonInstance | File size management, separation of concerns                                            |
| Nullable disabled in Tests/Content  | Practical for testing patterns, Unity compatibility                                     |
| Centralized constants               | No magic strings, easy maintenance                                                      |
| Fail-fast exceptions                | Clear error detection, no silent failures                                               |
| IMoveEffect composition             | Moves can have multiple effects                                                         |
| Three-Phase Testing                 | Functional → Edge Cases → Integration ensures complete coverage                         |
| Integration Test Standard           | Mandatory for system interactions, ensures components work together                     |
| Structured Workflow                 | Clear process for implementation, troubleshooting, and refactoring                      |
| Event-Driven Abilities & Items      | IBattleListener pattern for reactive effects, keeps engine clean                        |
| Pipeline Hooks for Modifiers        | IStatModifier pattern for passive stat/damage modifiers, integrates with DamagePipeline |
| Test Structure Organization         | Systems/Blueprints/Data separation for clear test organization and easy navigation      |

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

-   **Test Structure**: All tests MUST follow structure in `docs/testing/test_structure_definition.md`
-   **Systems/** - Tests de sistemas (CÓMO funcionan los sistemas)
-   **Blueprints/** - Tests de estructura de datos (CÓMO son los datos)
-   **Data/** - Tests de contenido específico (QUÉ contienen los datos)
-   **Three-Phase Testing**: Functional tests → Edge cases → Integration tests
-   **Test Types**:
-   Functional: `*Tests.cs` - Comportamiento normal y esperado
-   Edge Cases: `*EdgeCasesTests.cs` - Casos límite y condiciones especiales
-   Integration: `*IntegrationTests.cs` - Integración entre sistemas (en `Systems/*/Integration/`)
-   **Data Organization**:
    -   Un archivo por cada elemento en `Data/Pokemon/`, `Data/Moves/`, etc.
    -   Tests generales de catálogos en `Data/Catalogs/` (PokemonCatalogTests.cs, MoveCatalogTests.cs)
-   Use descriptive test names: `MethodName_Scenario_ExpectedResult`
-   **Integration Tests**: 83 tests covering system interactions
-   Status Effects ↔ DamagePipeline
-   Stat Changes ↔ DamagePipeline/TurnOrderResolver
-   Actions ↔ BattleQueue ↔ CombatEngine
-   Abilities & Items ↔ CombatEngine (OnSwitchIn, OnTurnEnd triggers)
-   Stat Modifiers ↔ DamagePipeline (Choice Band, Life Orb, Blaze)
-   Full battle end-to-end scenarios
-   **Test Reorganization**: ✅ **COMPLETE** - All 7 phases finished:
    -   Phase 1: Renamed confusing files ✅
    -   Phase 2: Split NewEffectsTests.cs ✅
    -   Phase 3: Created Systems/ structure ✅
    -   Phase 4: Moved all system tests to Systems/ ✅
    -   Phase 5: Moved all data tests to Data/ ✅
    -   Phase 6: Split catalog tests into individual files ✅ (62 files: 26 Pokemon, 36 Moves)
    -   Phase 7: Cleanup and verification ✅
    -   **Final Structure**: Systems/ (1,497+ tests), Blueprints/ (28 tests), Data/ (935+ tests)
    -   **Total**: 2,460 tests passing

---

## 📚 Key Reference Documents

| Document                                                  | Purpose                        |
| --------------------------------------------------------- | ------------------------------ |
| `docs/project_guidelines.md`                              | 24+ coding rules               |
| `.cursorrules`                                            | **AI workflow rules**          |
| `docs/implementation_plan.md`                             | Technical roadmap              |
| `docs/combat_implementation_plan.md`                      | **Combat phases**              |
| `docs/combat_use_cases.md`                                | **All battle cases**           |
| `docs/architecture/action_system_spec.md`                 | **⭐ Action system**           |
| `docs/architecture/effects_bible.md`                      | **📖 Effects guide**           |
| `docs/unity_integration.md`                               | Unity setup guide              |
| `CONTRIBUTING.md`                                         | Git workflow & rules           |
| `docs/architecture/combat_system_spec.md`                 | Combat system design           |
| `docs/checklists/pre_implementation.md`                   | **Before coding**              |
| `docs/checklists/feature_complete.md`                     | After coding                   |
| `docs/workflow/troubleshooting.md`                        | **Problem-solving guide**      |
| `docs/workflow/refactoring_guide.md`                      | **Safe refactoring process**   |
| `docs/testing/integration_testing_guide.md`               | **Integration test patterns**  |
| `docs/testing/test_structure_definition.md`               | **⭐ Test structure standard** |
| `docs/testing/test_reorganization_implementation_task.md` | **Test reorganization task**   |
| `docs/anti-patterns.md`                                   | What NOT to do                 |

---

## 🔄 How to Update This File

After completing a major feature:

1. Update "Current Project State" section
2. Move items from "Next" to "Completed"
3. Add any new architectural decisions
4. Update test count
5. Update "Last Updated" date

**During Test Reorganization:**

-   After completing each phase, update test reorganization status
-   Mark completed phases in test structure section
-   Update test count if tests are added/removed during reorganization
