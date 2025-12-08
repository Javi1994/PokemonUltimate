# Code Organization Strategy

> **Estrategia de organización de código para proyectos grandes**

This document defines how code is organized in Pokemon Ultimate project. With 5+ features and 20+ sub-features, maintaining clear organization is critical for scalability and navigation.

## Principles

### 1. Code by Technical System, Docs by Feature

**Key Principle**: Code is organized by **technical systems** (Core, Combat, Content), while documentation is organized by **features** (Game Data, Combat System, Content Expansion).

**Why**:

-   Code systems are shared across multiple features
-   Features are logical groupings for development and documentation
-   This separation allows code reuse while maintaining clear feature boundaries

### 2. Separation of Concerns

-   **Core/** → Generic game logic only, NO concrete game data
-   **Content/** → Concrete game data (Pokemon, Moves, Items)
-   **Combat/** → Battle engine and combat systems
-   **Tests/** → Organized by system/feature for easy navigation

### 3. Feature → Code Mapping

Each feature maps to specific namespaces and folders, but code is organized by technical system, not by feature folder.

---

## Code Structure

### Project Organization

```
PokemonUltimate/
├── PokemonUltimate.Core/          # Generic game engine (logic only)
│   ├── Blueprints/                # Immutable data structures
│   ├── Instances/                 # Mutable runtime state
│   ├── Effects/                   # Move effects (IMoveEffect)
│   ├── Enums/                     # Enumerations
│   ├── Evolution/                 # Evolution system
│   ├── Factories/                 # Factory classes
│   ├── Registry/                  # Data registries
│   ├── Builders/                  # Builder APIs
│   └── Constants/                 # Centralized strings
│
├── PokemonUltimate.Combat/        # Battle engine
│   ├── Engine/                    # Combat engine, battle loop
│   │   ├── CombatEngine.cs       # Main battle controller
│   │   ├── TurnEngine.cs         # Turn execution engine
│   │   ├── BattleFlow/           # Battle lifecycle (8 steps)
│   │   │   ├── BattleFlowContext.cs
│   │   │   ├── BattleFlowExecutor.cs
│   │   │   └── Steps/            # 8 battle flow steps
│   │   └── TurnFlow/             # Turn execution (23 unique steps, 34 total)
│   │       ├── TurnContext.cs
│   │       ├── TurnStepExecutor.cs
│   │       └── Steps/            # 23 turn flow steps
│   ├── Actions/                   # Battle actions (13 types)
│   │   ├── BattleAction.cs       # Base class
│   │   ├── UseMoveAction.cs
│   │   ├── DamageAction.cs
│   │   ├── HealAction.cs
│   │   └── ... (10 more action types)
│   ├── Damage/                   # Damage calculation pipeline
│   │   ├── DamagePipeline.cs    # Pipeline executor
│   │   ├── DamageContext.cs      # Damage calculation context
│   │   └── Steps/                # 11 damage calculation steps
│   ├── Handlers/                 # Effect handlers (34 handlers)
│   │   ├── Registry/            # CombatEffectHandlerRegistry
│   │   ├── Abilities/           # 4 ability handlers
│   │   ├── Items/               # 3 item handlers
│   │   ├── Effects/             # 12 move effect handlers
│   │   └── Checkers/           # 15 checker handlers
│   ├── Field/                    # Battle field, slots, sides
│   │   ├── BattleField.cs
│   │   ├── BattleSide.cs
│   │   ├── BattleSlot.cs
│   │   └── BattleRules.cs
│   ├── Target/                   # Target resolution system
│   │   ├── TargetResolver.cs
│   │   └── TargetRedirectionResolvers/
│   ├── Utilities/                # Helper utilities
│   │   ├── TurnOrderResolver.cs
│   │   ├── AccuracyChecker.cs
│   │   └── BattleSlotExtensions.cs
│   ├── AI/                       # AI implementations (6 AIs)
│   │   ├── RandomAI.cs
│   │   ├── SmartAI.cs
│   │   ├── TeamBattleAI.cs
│   │   └── ... (3 more AIs)
│   ├── Infrastructure/           # Supporting systems
│   │   ├── Events/              # BattleEventManager, BattleEvents
│   │   ├── Statistics/          # BattleStatistics, BattleStatisticsCollector
│   │   ├── Simulation/         # BattleSimulator, MoveSimulator
│   │   ├── ValueObjects/        # 8 value objects (StatStages, WeatherState, etc.)
│   │   ├── Logging/             # BattleLogger, NullBattleLogger
│   │   ├── Messages/            # BattleMessageFormatter
│   │   ├── Factories/           # BattleFieldFactory, BattleQueueFactory
│   │   └── Service/             # BattleQueueService, BattleArbiterService
│   └── View/                     # View interfaces
│       └── IBattleView.cs
│
├── PokemonUltimate.Content/       # Concrete game data
│   └── Catalogs/                  # Content catalogs (Pokemon, Moves, etc.)
│
└── PokemonUltimate.Tests/         # Test suite
    ├── Systems/                   # System tests (by feature/system)
    ├── Blueprints/                # Blueprint tests
    └── Data/                      # Content-specific tests
```

---

## Feature → Code Mapping

### Feature 1: Game Data

**Maps to**: `PokemonUltimate.Core/`

| Sub-Feature                    | Namespace                                           | Key Folders                             | Key Classes                                                               |
| ------------------------------ | --------------------------------------------------- | --------------------------------------- | ------------------------------------------------------------------------- |
| **1.1**: Pokemon Data          | `Core.Blueprints`, `Core.Instances`                 | `Blueprints/`, `Instances/`             | `PokemonSpeciesData`, `PokemonInstance`                                   |
| **1.2**: Move Data             | `Core.Blueprints`, `Core.Instances`, `Core.Effects` | `Blueprints/`, `Instances/`, `Effects/` | `MoveData`, `MoveInstance`, `IMoveEffect`                                 |
| **1.3**: Ability Data          | `Core.Blueprints`                                   | `Blueprints/`                           | `AbilityData`                                                             |
| **1.4**: Item Data             | `Core.Blueprints`                                   | `Blueprints/`                           | `ItemData`                                                                |
| **1.5-1.10**: Field Conditions | `Core.Blueprints`                                   | `Blueprints/`                           | `StatusEffectData`, `WeatherData`, `TerrainData`, etc.                    |
| **1.11**: Evolution            | `Core.Evolution`                                    | `Evolution/`                            | `Evolution`, `IEvolutionCondition`                                        |
| **1.12**: Type Effectiveness   | `Core.Factories`                                    | `Factories/`                            | `TypeEffectiveness`                                                       |
| **1.13**: Interfaces           | `Core.Blueprints`                                   | `Blueprints/`                           | `IIdentifiable`                                                           |
| **1.14**: Enums & Constants    | `Core.Enums`, `Core.Constants`                      | `Enums/`, `Constants/`                  | All enums, `ErrorMessages`, `GameMessages` (deprecated - see Feature 4.9) |
| **1.15**: Factories            | `Core.Factories`                                    | `Factories/`                            | `StatCalculator`, `PokemonFactory`                                        |
| **1.16**: Registry             | `Core.Registry`                                     | `Registry/`                             | `IDataRegistry<T>`, `PokemonRegistry`                                     |
| **1.17**: Builders             | `Core.Builders`                                     | `Builders/`                             | `PokemonBuilder`, `MoveBuilder`, etc.                                     |

**Test Location**: `Tests/Systems/GameData/`, `Tests/Blueprints/`

---

### Feature 2: Combat System

**Maps to**: `PokemonUltimate.Combat/`

| Sub-Feature                      | Namespace                          | Key Folders                                         | Key Classes                                                                                                      |
| -------------------------------- | ---------------------------------- | --------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------- |
| **2.1**: Battle Foundation       | `Combat.Field`                     | `Field/`                                            | `BattleField`, `BattleSlot`, `BattleSide`, `BattleRules`                                                         |
| **2.2**: Action Queue            | `Combat.Engine.Service`            | `Engine/Service/`                                   | `BattleQueueService`, `BattleAction` (13 action types)                                                           |
| **2.3**: Turn Order              | `Combat.Utilities`                 | `Utilities/`                                        | `TurnOrderResolver`                                                                                              |
| **2.4**: Damage Pipeline         | `Combat.Damage`                    | `Damage/`, `Damage/Steps/`                          | `DamagePipeline`, `IDamageStep` (11 steps)                                                                       |
| **2.5**: Combat Actions          | `Combat.Actions`                   | `Actions/`                                          | `BattleAction`, `UseMoveAction`, `DamageAction`, etc. (13 action types)                                          |
| **2.6**: Combat Engine           | `Combat.Engine`                    | `Engine/`, `Engine/BattleFlow/`, `Engine/TurnFlow/` | `CombatEngine`, `TurnEngine`, `BattleFlowExecutor`, `TurnStepExecutor` (8 battle flow steps, 23 turn flow steps) |
| **2.7**: Integration             | `Combat.AI`, `Combat.View`         | `AI/`, `View/`                                      | `IActionProvider`, `IBattleView` (6 AI implementations)                                                          |
| **2.8**: End-of-Turn             | `Combat.Engine.TurnFlow.Steps`     | `Engine/TurnFlow/Steps/`                            | `EndOfTurnEffectsStep`, `DurationDecrementStep`                                                                  |
| **2.9**: Abilities & Items       | `Combat.Handlers`                  | `Handlers/`, `Handlers/Registry/`                   | `CombatEffectHandlerRegistry` (34 handlers: 4 abilities + 3 items + 12 effects + 15 checkers)                    |
| **2.10**: Pipeline Hooks         | `Combat.Damage.Steps`              | `Damage/Steps/`                                     | `AttackerAbilityStep`, `AttackerItemStep`                                                                        |
| **2.11**: Recoil & Drain         | `Combat.Handlers.Effects`          | `Handlers/Effects/`                                 | `RecoilEffectHandler`, `DrainEffectHandler`                                                                      |
| **2.12-2.16**: Field Systems     | `Combat.Field`, `Combat.Handlers`  | `Field/`, `Handlers/Effects/`                       | Field condition handlers, weather/terrain/hazard handlers                                                        |
| **2.17-2.19**: Advanced Features | `Combat.Handlers`                  | `Handlers/`                                         | Advanced ability/item handlers (extensible via registry)                                                         |
| **2.20**: Statistics System      | `Combat.Infrastructure.Statistics` | `Infrastructure/Statistics/`                        | `BattleStatistics`, `BattleStatisticsCollector`                                                                  |

**Test Location**: `Tests/Systems/Combat/`

---

### Feature 3: Content Expansion

**Maps to**: `PokemonUltimate.Content/` and `PokemonUltimate.Core.Builders/`

| Sub-Feature                    | Namespace          | Key Folders | Key Classes                           |
| ------------------------------ | ------------------ | ----------- | ------------------------------------- |
| **3.1-3.6**: Content Expansion | `Content.Catalogs` | `Catalogs/` | `PokemonCatalog`, `MoveCatalog`, etc. |
| **3.7**: Content Validation    | `Content.Catalogs` | `Catalogs/` | Validation logic                      |
| **3.8**: Content Organization  | `Content.Catalogs` | `Catalogs/` | Catalog organization                  |
| **3.9**: Builders              | `Core.Builders`    | `Builders/` | Fluent builder APIs                   |

**Test Location**: `Tests/Data/` (content-specific tests)

---

### Feature 4: Unity Integration

**Maps to**: Unity project (future)

| Sub-Feature                    | Namespace        | Key Folders             | Key Classes                    |
| ------------------------------ | ---------------- | ----------------------- | ------------------------------ |
| **4.1-4.8**: Unity Integration | Unity namespaces | Unity project structure | Unity-specific implementations |

**Test Location**: Unity test project (future)

---

### Feature 5: Game Features

**Maps to**: Future projects/modules

| Sub-Feature                | Namespace         | Key Folders      | Key Classes                  |
| -------------------------- | ----------------- | ---------------- | ---------------------------- |
| **5.1-5.6**: Game Features | Future namespaces | Future structure | Game feature implementations |

**Test Location**: Future test projects

---

## Test Organization

Tests follow the same technical system organization:

```
PokemonUltimate.Tests/
├── Systems/                       # System tests (how systems work)
│   ├── GameData/                 # Feature 1 tests
│   │   ├── PokemonTests.cs
│   │   ├── MoveTests.cs
│   │   └── ...
│   ├── Combat/                   # Feature 2 tests
│   │   ├── BattleEngineTests.cs
│   │   ├── DamagePipelineTests.cs
│   │   └── Integration/          # Integration tests
│   └── ContentExpansion/         # Feature 3 tests
│
├── Blueprints/                    # Blueprint structure tests
│   ├── PokemonSpeciesDataTests.cs
│   ├── MoveDataTests.cs
│   └── ...
│
└── Data/                          # Content-specific tests
    ├── Pokemon/                   # One file per Pokemon
    │   ├── PikachuTests.cs
    │   └── ...
    ├── Moves/                     # One file per Move
    │   ├── FlamethrowerTests.cs
    │   └── ...
    └── Catalogs/                  # Catalog tests
        ├── PokemonCatalogTests.cs
        └── ...
```

**Test Structure Standard**: See `ai_workflow/docs/TDD_GUIDE.md` for complete test structure guidelines.

---

## Examples

### Example 1: Adding a New Pokemon

**Feature**: 3.1 (Pokemon Expansion)

**Code Location**:

-   **Blueprint**: `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs` (already exists, just use it)
-   **Content**: `PokemonUltimate.Content/Catalogs/PokemonCatalog.cs` (add new Pokemon)
-   **Builder**: `PokemonUltimate.Core/Builders/PokemonBuilder.cs` (use builder API)

**Test Location**:

-   **Content Test**: `PokemonUltimate.Tests/Data/Pokemon/[NewPokemon]Tests.cs`
-   **Catalog Test**: `PokemonUltimate.Tests/Data/Catalogs/PokemonCatalogTests.cs`

**Documentation**: `docs/features/3-content-expansion/3.1-pokemon-expansion/`

---

### Example 2: Adding a New Combat Action

**Feature**: 2.5 (Combat Actions)

**Code Location**:

-   **Action Class**: `PokemonUltimate.Combat/Actions/[NewAction].cs`
-   **Integration**: `PokemonUltimate.Combat/Engine/CombatEngine.cs` (use action)

**Test Location**:

-   **Functional**: `PokemonUltimate.Tests/Systems/Combat/[NewAction]Tests.cs`
-   **Integration**: `PokemonUltimate.Tests/Systems/Combat/Integration/[NewAction]IntegrationTests.cs`

**Documentation**: `docs/features/2-combat-system/2.5-combat-actions/`

---

### Example 3: Adding a New Move Effect

**Feature**: 1.2 (Move Data)

**Code Location**:

-   **Effect Class**: `PokemonUltimate.Core/Effects/[NewEffect].cs`
-   **Interface**: `PokemonUltimate.Core/Effects/IMoveEffect.cs` (implement)

**Test Location**:

-   **Blueprint Test**: `PokemonUltimate.Tests/Blueprints/[NewEffect]Tests.cs`
-   **System Test**: `PokemonUltimate.Tests/Systems/GameData/MoveEffectTests.cs`

**Documentation**: `docs/features/1-game-data/1.2-move-data/`

---

## Navigation Tips

### Finding Code for a Feature

1. **Check Feature's `code_location.md`** - Each feature documents where its code lives
2. **Use Feature → Code Mapping** - See table above
3. **Search by Namespace** - Code uses namespaces matching technical systems

### Finding Tests for Code

1. **Check Feature's `testing.md`** - Each feature documents its test strategy
2. **Follow Test Structure** - Tests mirror code structure (`Systems/`, `Blueprints/`, `Data/`)
3. **Use Feature Number** - Test files reference feature numbers in comments

### Adding New Code

1. **Assign to Feature First** - Always assign work to a feature before coding
2. **Follow Existing Patterns** - Match the organization of similar code
3. **Update `code_location.md`** - Document where new code lives
4. **Update Tests** - Add tests following the test structure standard

---

## Key Principles Summary

1. **Code by System**: Organize code by technical systems (Core, Combat, Content)
2. **Docs by Feature**: Organize documentation by features (Game Data, Combat System, etc.)
3. **Clear Mapping**: Each feature maps to specific namespaces and folders
4. **Test Mirror**: Tests mirror code structure for easy navigation
5. **Feature References**: All code includes feature references in XML docs

---

## Related Documents

-   **Feature Documentation**: `docs/features/[N]-[feature-name]/code_location.md` - Detailed code locations per feature
-   **Test Structure**: `ai_workflow/docs/TDD_GUIDE.md` - Complete test structure guidelines
-   **Feature Master List**: `docs/features_master_list.md` - Complete feature reference

---

**Last Updated**: 2025-01-XX
