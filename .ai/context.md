# 🤖 AI Context Summary

> **This file provides immediate context for the AI assistant.**
> Update this file after completing major features or making architectural decisions.

---

## 📍 Current Project State

| Aspect                  | Status                                                                                                                                                                                                                                                                                                    |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Current Phase**       | Phase 3: Combat System ✅                                                                                                                                                                                                                                                                                 |
| **Sub-Phase**           | 2.14 Hazards System ✅ Core Complete                                                                                                                                                                                                                                                                      |
| **Combat Refactoring**  | ✅ Complete (2024-12-05) - Phases 0-13 completed (42/44 tasks, 95.5%). DI, Value Objects, Strategy Pattern, Factory Pattern, Event System, Logging, Validation implemented.                                                                                                                               |
| **Core Refactoring**    | ✅ Complete (2024-12-XX) - Phases 0-8 completed (21/22 tasks, 95.5%). DI, Strategy Pattern, Constants centralization, Extension methods, Validation, Move selection, Stat stage management, Stats caching implemented. See `PokemonUltimate.Core/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for details. |
| **Tests**               | 3,210+ passing (includes Phase 4: IVs/EVs System, Breeding System, Ownership/Tracking tests)                                                                                                                                                                                                     |
| **Integration Tests**   | 90+ tests (system interactions, Advanced Abilities & Items)                                                                                                                                                                                                                                                                           |
| **Test Reorganization** | ✅ Complete - All phases finished (62 individual catalog files: 26 Pokemon 100%, 36 Moves 100%). Redundant grouped tests removed.                                                                                                                                                                         |
| **Warnings**            | 0                                                                                                                                                                                                                                                                                                         |
| **Pokemon Catalog**     | 26 Pokemon (Gen1)                                                                                                                                                                                                                                                                                         |
| **Move Catalog**        | 36 Moves (12 types)                                                                                                                                                                                                                                                                                       |
| **Last Updated**        | January 2025 (Advanced Abilities & Items Integration Complete)                                                                                                                                                                                                                                                                           |

---

## 🏗️ Architecture Overview

```
PokemonUltimate/
├── Core/           # Game logic (DO NOT add game data here) - ✅ Refactored (2024-12-XX)
│   ├── Blueprints/ # Immutable data structures (uses Strategy Pattern for stat getters, post-refactor)
│   ├── Instances/  # Mutable runtime state (uses StatsCache, StatStageManager, post-refactor)
│   ├── Factories/  # Object creation (DI-based: IStatCalculator, ITypeEffectiveness, IMoveSelector, post-refactor)
│   ├── Effects/    # Move effects (IMoveEffect, uses Strategy Pattern for descriptions, post-refactor)
│   ├── Evolution/  # Evolution conditions
│   ├── Registry/   # Data access layer
│   ├── Managers/   # StatStageManager (post-refactor)
│   ├── Providers/  # IRandomProvider (post-refactor)
│   ├── Extensions/ # LevelExtensions, FriendshipExtensions (post-refactor)
│   ├── Enums/      # Type definitions
│   └── Constants/  # CoreConstants, CoreValidators, ErrorMessages, GameMessages (post-refactor)
│
├── Combat/         # Battle system (depends on Core) - ✅ Refactored (2024-12-05)
│   ├── Field/      # BattleField, BattleSide, BattleSlot, BattleRules
│   ├── Engine/     # CombatEngine (DI-based), BattleArbiter, BattleQueue, EndOfTurnProcessor (instance-based)
│   ├── Events/     # BattleTrigger, IBattleListener, AbilityListener, ItemListener, BattleTriggerProcessor (instance-based), IBattleEventBus
│   ├── Damage/Steps/ # BaseDamageStep, AttackerAbilityStep, AttackerItemStep, etc.
│   ├── Results/    # BattleOutcome, BattleResult
│   ├── Providers/  # IActionProvider, PlayerInputProvider, IRandomProvider
│   ├── View/       # IBattleView, NullBattleView (with input methods)
│   ├── Actions/    # BattleAction implementations, BattleActionType
│   ├── Damage/     # DamagePipeline (IDamagePipeline), DamageContext, IStatModifier, AbilityStatModifier, ItemStatModifier
│   ├── AI/         # RandomAI, AlwaysAttackAI
│   ├── Helpers/    # AccuracyChecker (instance-based), TurnOrderResolver (instance-based), TargetResolver (instance-based), ITargetRedirectionResolver
│   ├── Factories/  # IBattleFieldFactory, IBattleQueueFactory, DamageContextFactory
│   ├── ValueObjects/ # StatStages, DamageTracker, ProtectTracker, MoveStateTracker, WeatherState, TerrainState
│   ├── Effects/    # IMoveEffectProcessor, MoveEffectProcessorRegistry, effect processors (Strategy Pattern)
│   ├── Logging/    # IBattleLogger, BattleLogger, NullBattleLogger
│   ├── Messages/   # IBattleMessageFormatter, BattleMessageFormatter
│   ├── Validation/ # IBattleStateValidator, BattleStateValidator
│   ├── Extensions/ # BattleSlotExtensions, DamageCalculationExtensions
│   └── Constants/  # BattleConstants, StatusConstants, ItemConstants, MoveConstants
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
    -   See `docs/features/2-combat-system/2.5-combat-actions/effects/architecture.md` for complete reference

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
-   [x] **Variants System Design** ← NEW
    -   Mega Evolutions, Dinamax, and Terracristalización as separate Pokemon species
    -   Each variant has own PokemonSpeciesData with different stats
    -   See `docs/features/1-game-data/1.18-variants-system/README.md` for specification
-   [x] **Game Data Roadmap** ← NEW
    -   Complete field specification for PokemonSpeciesData and PokemonInstance
    -   Missing critical fields identified (BaseExperienceYield, CatchRate, BaseFriendship, GrowthRate)
    -   Pokedex fields specified (Description, Category, Height, Weight, Color, Shape, Habitat)
    -   Variants system fields specified (BaseForm, VariantType, TeraType, Variants)
    -   See `docs/features/1-game-data/roadmap.md` for complete specification
-   [x] **Phase 4: Optional Enhancements** ✅ Complete
    -   **IVs/EVs System**: IVSet and EVSet classes, integrated with StatCalculator, random IVs (0-31), max EVs (252/252/4/0/0/0)
    -   **Breeding System**: EggGroup enum (13 groups), EggGroups and EggCycles properties, breeding compatibility methods (CanBreedWith, IsInEggGroup, CannotBreed)
    -   **Ownership/Tracking Fields**: OriginalTrainer, TrainerId, MetLevel, MetLocation, MetDate properties in PokemonInstance

---

## 🎯 Combat System Progress

See `docs/features/2-combat-system/roadmap.md` for full details.

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
| 2.12 Weather System     | ✅ Complete | Weather tracking, damage modifiers, end-of-turn damage, perfect accuracy moves                                |
| 2.13 Terrain System     | ✅ Complete | Terrain tracking, damage modifiers, end-of-turn healing, terrain actions                                      |
| 2.14 Hazards System     | ✅ Complete | Entry hazard tracking, processing on switch-in, Spikes/Stealth Rock/Toxic Spikes/Sticky Web                   |
| 2.17 Advanced Abilities | ✅ ~95% Complete | Truant, Speed Boost, Static, Rough Skin, Swift Swim, Chlorophyll, Moxie (29 tests) |
| 2.18 Advanced Items    | ✅ Complete | Life Orb, Focus Sash, Rocky Helmet, Black Sludge (21 tests) |

Reference docs:

-   `docs/features/2-combat-system/roadmap.md` ← **Start here**
-   `docs/features/2-combat-system/2.5-combat-actions/architecture.md` ← **⭐ Action system**
-   `docs/features/2-combat-system/2.5-combat-actions/use_cases.md` ← **📋 Use cases (207 cases)**
-   `docs/features/2-combat-system/actions_bible.md` ← **📖 Actions reference**
-   `docs/features/2-combat-system/architecture.md`
-   `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`

---

## 📦 Content Expansion Progress

See `docs/features/3-content-expansion/roadmap.md` for full details.

| Sub-Feature                    | Status         | Description                                                             |
| ------------------------------ | -------------- | ----------------------------------------------------------------------- |
| 3.1 Pokemon Expansion          | 🎯 In Progress | 26/151 Gen 1 Pokemon                                                    |
| 3.2 Move Expansion             | 🎯 In Progress | 36 moves (12 types)                                                     |
| 3.3 Item Expansion             | 🎯 In Progress | 23 items (15 held + 8 berries)                                          |
| 3.4 Ability Expansion          | 🎯 In Progress | 35 abilities                                                            |
| 3.5 Status Effect Expansion    | ✅ Complete    | 15 statuses (6 persistent + 9 volatile)                                 |
| 3.6 Field Conditions Expansion | ✅ Complete    | 35 field conditions (9 weather, 4 terrain, 4 hazards, 10 side, 8 field) |
| 3.7 Content Validation         | ⏳ Planned     | Quality standards and validation                                        |
| 3.8 Content Organization       | ✅ Complete    | Catalog organization and maintenance                                    |

**Current Content**:

-   **Core**: 26 Pokemon, 36 Moves, 35 Abilities, 23 Items
-   **Supporting**: 15 Status Effects, 9 Weather, 4 Terrain, 4 Hazards, 10 Side Conditions, 8 Field Effects

---

## 📐 Key Architectural Decisions

| Decision                                 | Rationale                                                                               |
| ---------------------------------------- | --------------------------------------------------------------------------------------- |
| Blueprint/Instance pattern               | Immutable data vs mutable runtime state                                                 |
| Partial classes for PokemonInstance      | File size management, separation of concerns                                            |
| Nullable disabled in Tests/Content       | Practical for testing patterns, Unity compatibility                                     |
| Centralized constants                    | No magic strings, easy maintenance                                                      |
| Fail-fast exceptions                     | Clear error detection, no silent failures                                               |
| IMoveEffect composition                  | Moves can have multiple effects                                                         |
| Three-Phase Testing                      | Functional → Edge Cases → Integration ensures complete coverage                         |
| Integration Test Standard                | Mandatory for system interactions, ensures components work together                     |
| Structured Workflow                      | Clear process for implementation, troubleshooting, and refactoring                      |
| Event-Driven Abilities & Items           | IBattleListener pattern for reactive effects, keeps engine clean                        |
| Pipeline Hooks for Modifiers             | IStatModifier pattern for passive stat/damage modifiers, integrates with DamagePipeline |
| Test Structure Organization              | Systems/Blueprints/Data separation for clear test organization and easy navigation      |
| **Dependency Injection (Post-Refactor)** | All major components use DI for improved testability and flexibility                    |
| **Value Objects (Post-Refactor)**        | Complex state encapsulated in Value Objects (StatStages, MoveStateTracker, etc.)        |
| **Strategy Pattern (Post-Refactor)**     | Move effects processed using Strategy Pattern for extensibility                         |
| **Factory Pattern (Post-Refactor)**      | Object creation centralized using Factory Pattern                                       |
| **Event Bus (Post-Refactor)**            | Decoupled communication using Event Bus pattern                                         |
| **Structured Logging (Post-Refactor)**   | IBattleLogger for debugging and monitoring                                              |
| **State Validation (Post-Refactor)**     | IBattleStateValidator ensures battle state consistency                                  |

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

-   **Test Structure**: All tests MUST follow structure in `docs/ai/testing_structure_definition.md`
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

| Document                                                                   | Purpose                                    |
| -------------------------------------------------------------------------- | ------------------------------------------ |
| `docs/features_master_list.md`                                             | **📋 Feature numbering and naming**        |
| `.cursorrules`                                                             | Coding rules and guidelines                 |
| `.cursorrules`                                                             | **AI workflow rules**                      |
| `docs/implementation_plan.md`                                              | Technical roadmap                          |
| `docs/features/2-combat-system/roadmap.md`                                 | **Combat phases**                          |
| `docs/features/3-content-expansion/roadmap.md`                             | **📦 Content expansion phases**            |
| `docs/features/1-game-data/roadmap.md`                                     | **📊 Game data fields spec**               |
| `docs/features/1-game-data/1.18-variants-system/README.md`                 | **🔀 Variants system (Mega/Dinamax/Tera)** |
| `docs/features/4-unity-integration/roadmap.md`                             | **🎮 Unity integration phases**            |
| `docs/features/5-game-features/roadmap.md`                                 | **🎯 Game features phases**                |
| `docs/features/2-combat-system/use_cases.md`                               | **All battle cases**                       |
| `docs/features/2-combat-system/2.5-combat-actions/architecture.md`         | **⭐ Action system**                       |
| `docs/features/2-combat-system/2.5-combat-actions/effects/architecture.md` | **📖 Effects guide**                       |
| `docs/features/4-unity-integration/architecture.md`                        | Unity setup guide                          |
| `CONTRIBUTING.md`                                                          | Git workflow & rules                       |
| `docs/features/2-combat-system/architecture.md`                            | Combat system design                       |
| `docs/ai/checklists/pre_implementation.md`                                 | **Before coding**                          |
| `docs/ai/checklists/feature_complete.md`                                   | After coding                               |
| `docs/ai/workflow/troubleshooting.md`                                      | **Problem-solving guide**                  |
| `docs/ai/workflow/refactoring_guide.md`                                    | **Safe refactoring process**               |
| `docs/features/2-combat-system/testing/integration_guide.md`               | **Integration test patterns**              |
| `docs/ai/testing_structure_definition.md`                                  | **⭐ Test structure standard**             |
| `docs/features/1-game-data/testing.md`                                     | **📊 Game data testing strategy**          |
| `docs/features/README.md`                                                  | **Features overview**                      |
| `docs/ai/anti-patterns.md`                                                 | What NOT to do                             |

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
