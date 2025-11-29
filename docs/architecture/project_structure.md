# Project Structure

## Overview

This document describes the organization of the PokemonUltimate solution, its projects, and the folder structure within each project.

## Solution Structure

```
PokemonUltimate/
├── PokemonUltimate.sln              # Solution file
├── docs/                            # Documentation
│   ├── architecture/                # System specifications
│   ├── project_guidelines.md        # Core development rules
│   ├── implementation_plan.md       # Step-by-step roadmap
│   └── task.md                      # Project phases & progress
│
├── PokemonUltimate.Core/            # 🎯 Game Engine (netstandard2.1)
├── PokemonUltimate.Tests/           # 🧪 Unit Tests (net8.0)
└── PokemonUltimate.Console/         # 🖥️ Demo Application (net8.0)
```

---

## Projects

### 1. PokemonUltimate.Core
**The Game Engine** - Pure C# library with all game logic.

| Property | Value |
|----------|-------|
| Framework | `netstandard2.1` |
| C# Version | `7.3` |
| Nullable | `disable` |
| Purpose | Unity-compatible game logic |

**Why netstandard2.1?**
- Unity 2020+ uses this framework
- Ensures all code is compatible with Unity's Mono runtime
- No dependencies on Unity assemblies

### 2. PokemonUltimate.Tests
**Test Suite** - NUnit tests for all Core functionality.

| Property | Value |
|----------|-------|
| Framework | `net8.0` |
| Test Framework | NUnit 3 |
| Purpose | Verify Core logic works correctly |

### 3. PokemonUltimate.Console
**Runtime Smoke Test** - Console application for verifying all systems work correctly at runtime.

| Property | Value |
|----------|-------|
| Framework | `net8.0` |
| Purpose | Runtime verification of all data systems |

#### How to Run
```powershell
dotnet run --project PokemonUltimate.Console
```

#### What It Tests
The console application performs ~70 runtime tests across all systems:

| Section | What's Tested |
|---------|---------------|
| **Catalogs** | Count, enumeration, direct access (PokemonCatalog.Pikachu) |
| **Registries** | RegisterAll, GetByName, GetByPokedexNumber, type/category filters |
| **Pokemon Data** | Name, types, IsDualType, HasType, BaseStats |
| **Learnsets** | GetStartingMoves, GetMovesAtLevel, CanLearn |
| **Evolutions** | CanEvolve, conditions (Level, Item), target references |
| **Gender System** | GenderRatio, HasBothGenders, IsGenderless |
| **Nature System** | GetStatMultiplier, IsNeutral, increased/decreased stats |
| **Move Data** | Power, accuracy, effects composition |
| **Move Builder** | Create moves dynamically with effects |
| **Pokemon Builder** | Create Pokemon with stats, learnsets |
| **Effect Types** | All 9 effect classes (Damage, Status, etc.) |
| **Target Scopes** | SingleEnemy, AllEnemies, AllOthers |

#### Output Format
```
═══ SECTION NAME ═══
  ✓ Test that passes
  ✗ Test that fails [FAILED]
    → Informational message

╔═══════════════════════════════════════════════════════════════╗
║  ✓ ALL 70 TESTS PASSED - Systems Ready for Combat!           ║
╚═══════════════════════════════════════════════════════════════╝
```

#### When to Use
- After making changes to Core to verify nothing broke
- Before starting new features to confirm base systems work
- As a quick sanity check without running the full test suite
- To see a visual listing of all Pokemon and Moves in the catalog

---

## Core Project Structure

```
PokemonUltimate.Core/
│
├── Models/                          # 📦 Data Models (POCOs)
│   ├── IIdentifiable.cs             # Base interface for registry items
│   ├── PokemonSpeciesData.cs        # Pokemon blueprint (with GenderRatio)
│   ├── MoveData.cs                  # Move blueprint
│   ├── BaseStats.cs                 # HP, Attack, Defense, etc.
│   ├── LearnableMove.cs             # Move in a Pokemon's learnset
│   └── NatureData.cs                # Static: Nature stat modifiers (±10%)
│
├── Registry/                        # 🗄️ Data Storage & Retrieval
│   ├── IDataRegistry.cs             # Generic registry interface
│   ├── GameDataRegistry.cs          # Base implementation
│   ├── IPokemonRegistry.cs          # Pokemon-specific interface
│   ├── PokemonRegistry.cs           # Name + Pokedex lookup
│   ├── IMoveRegistry.cs             # Move-specific interface
│   └── MoveRegistry.cs              # Name + Type/Category filters
│
├── Enums/                           # 🏷️ Type Definitions
│   ├── PokemonType.cs               # 18 types (Fire, Water, etc.)
│   ├── MoveCategory.cs              # Physical, Special, Status
│   ├── TargetScope.cs               # Who can be targeted
│   ├── Stat.cs                      # HP, Attack, Speed, etc.
│   ├── PersistentStatus.cs          # Burn, Paralysis, etc.
│   ├── VolatileStatus.cs            # Confusion, Flinch, etc.
│   ├── EffectType.cs                # Types of move effects
│   ├── LearnMethod.cs               # How moves are learned
│   ├── TimeOfDay.cs                 # For evolution conditions
│   ├── Gender.cs                    # Male, Female, Genderless
│   └── Nature.cs                    # 25 natures (stat modifiers)
│
├── Effects/                         # ⚡ Move Effect System
│   ├── IMoveEffect.cs               # Effect interface
│   ├── DamageEffect.cs              # Standard damage
│   ├── FixedDamageEffect.cs         # Fixed HP damage
│   ├── StatusEffect.cs              # Apply status condition
│   ├── StatChangeEffect.cs          # Modify stat stages
│   ├── RecoilEffect.cs              # User takes damage
│   ├── DrainEffect.cs               # Heal from damage dealt
│   ├── HealEffect.cs                # Direct HP recovery
│   ├── FlinchEffect.cs              # May cause flinch
│   └── MultiHitEffect.cs            # Hits 2-5 times
│
├── Evolution/                       # 🔄 Evolution System
│   ├── Evolution.cs                 # Evolution definition
│   ├── IEvolutionCondition.cs       # Condition interface
│   ├── EvolutionConditionType.cs    # Condition types enum
│   └── Conditions/                  # Concrete conditions
│       ├── LevelCondition.cs
│       ├── ItemCondition.cs
│       ├── FriendshipCondition.cs
│       ├── TimeOfDayCondition.cs
│       ├── TradeCondition.cs
│       └── KnowsMoveCondition.cs
│
├── Builders/                        # 🏗️ Fluent Builders
│   ├── PokemonBuilder.cs            # Pokemon.Define(...).Build()
│   ├── LearnsetBuilder.cs           # .StartsWith(), .AtLevel()
│   ├── EvolutionBuilder.cs          # .AtLevel(), .WithItem()
│   ├── MoveBuilder.cs               # Move.Define(...).Build()
│   └── EffectBuilder.cs             # .Damage(), .MayBurn()
│
└── Catalogs/                        # 📚 Static Game Data
    ├── Pokemon/
    │   ├── PokemonCatalog.cs        # Orchestrator
    │   └── PokemonCatalog.Gen1.cs   # Generation 1 Pokemon
    └── Moves/
        ├── MoveCatalog.cs           # Orchestrator
        ├── MoveCatalog.Normal.cs    # Normal-type moves
        ├── MoveCatalog.Fire.cs      # Fire-type moves
        ├── MoveCatalog.Water.cs     # Water-type moves
        ├── MoveCatalog.Grass.cs     # Grass-type moves
        ├── MoveCatalog.Electric.cs  # Electric-type moves
        ├── MoveCatalog.Ground.cs    # Ground-type moves
        └── MoveCatalog.Psychic.cs   # Psychic-type moves
```

---

## Tests Project Structure

**Mirrors Core structure for easy navigation:**

```
PokemonUltimate.Tests/
│
├── Models/                          # Tests for data models
│   ├── BaseStatsTests.cs
│   ├── LearnableMoveTests.cs
│   ├── MoveDataTests.cs
│   ├── NatureDataTests.cs
│   └── PokemonSpeciesDataTests.cs
│
├── Registry/                        # Tests for registries
│   ├── MoveRegistryTests.cs
│   ├── MoveRegistryFilterTests.cs
│   ├── PokemonRegistryTests.cs
│   └── PokemonRegistryPokedexTests.cs
│
├── Effects/                         # Tests for move effects
│   ├── MoveEffectTests.cs
│   └── MoveEffectCompositionTests.cs
│
├── Evolution/                       # Tests for evolution system
│   ├── EvolutionTests.cs
│   └── EvolutionConditionTests.cs
│
├── Builders/                        # Tests for builders
│   ├── PokemonBuilderTests.cs
│   ├── LearnsetBuilderTests.cs
│   ├── EvolutionBuilderTests.cs
│   ├── MoveBuilderTests.cs
│   └── EffectBuilderTests.cs
│
└── Catalogs/                        # Tests for catalogs
    ├── Pokemon/
    │   ├── PokemonCatalogTests.cs
    │   └── PokemonCatalogGen1Tests.cs
    └── Moves/
        ├── MoveCatalogTests.cs
        ├── MoveCatalogNormalTests.cs
        ├── MoveCatalogFireTests.cs
        ├── MoveCatalogElectricTests.cs
        └── MoveCatalogOtherTypesTests.cs
```

---

## Namespace Convention

All namespaces follow the folder structure:

| Folder | Namespace |
|--------|-----------|
| `Core/Models/` | `PokemonUltimate.Core.Models` |
| `Core/Registry/` | `PokemonUltimate.Core.Registry` |
| `Core/Effects/` | `PokemonUltimate.Core.Effects` |
| `Core/Evolution/` | `PokemonUltimate.Core.Evolution` |
| `Core/Evolution/Conditions/` | `PokemonUltimate.Core.Evolution.Conditions` |
| `Core/Builders/` | `PokemonUltimate.Core.Builders` |
| `Core/Catalogs/` | `PokemonUltimate.Core.Catalogs` |
| `Core/Enums/` | `PokemonUltimate.Core.Enums` |

---

## Dependency Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    PokemonUltimate.Tests                     │
│                         (net8.0)                             │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    PokemonUltimate.Core                      │
│                     (netstandard2.1)                         │
│                                                              │
│  ┌─────────┐    ┌──────────┐    ┌──────────┐                │
│  │ Models  │◄───│ Registry │◄───│ Catalogs │                │
│  └─────────┘    └──────────┘    └──────────┘                │
│       ▲              ▲               │                       │
│       │              │               │                       │
│  ┌─────────┐    ┌──────────┐         │                       │
│  │  Enums  │    │ Builders │─────────┘                       │
│  └─────────┘    └──────────┘                                 │
│       ▲                                                      │
│       │                                                      │
│  ┌─────────┐    ┌───────────┐                               │
│  │ Effects │    │ Evolution │                               │
│  └─────────┘    └───────────┘                               │
└─────────────────────────────────────────────────────────────┘
```

**Key Rules:**
- `Models` has no dependencies (pure data)
- `Enums` has no dependencies (pure types)
- `Effects` depends on `Enums`
- `Registry` depends on `Models`
- `Catalogs` depends on `Models`, `Builders`, `Effects`
- `Builders` depends on `Models`, `Enums`, `Evolution`

---

## File Organization Guidelines

### 1. One Class Per File
Each public class/interface should have its own file with matching name.

### 2. Partial Classes for Large Content
Use partial classes to split large catalogs:
```
PokemonCatalog.cs       # Orchestrator (All, Count, RegisterAll)
PokemonCatalog.Gen1.cs  # Gen 1 Pokemon definitions
PokemonCatalog.Gen2.cs  # Gen 2 Pokemon definitions (future)
```

### 3. File Size Guidelines
- **~50-150 lines** per file
- If a file grows beyond 200 lines, consider splitting

### 4. Test File Naming
Test files mirror source files with `Tests` suffix:
- `BaseStats.cs` → `BaseStatsTests.cs`
- `PokemonCatalog.Gen1.cs` → `PokemonCatalogGen1Tests.cs`

---

## Adding New Content

### Adding a New Pokemon
1. Open `Catalogs/Pokemon/PokemonCatalog.Gen1.cs` (or appropriate generation)
2. Define in **reverse evolution order** (final form first)
3. Add to `RegisterGen1()` method
4. Add tests in `Tests/Catalogs/Pokemon/PokemonCatalogGen1Tests.cs`

### Adding a New Move
1. Open `Catalogs/Moves/MoveCatalog.[Type].cs`
2. Define the move with its effects
3. Add to `Register[Type]()` method
4. Add tests in appropriate test file

### Adding a New Effect
1. Create class in `Effects/` implementing `IMoveEffect`
2. Add to `EffectType` enum
3. Add tests in `Tests/Effects/MoveEffectTests.cs`

### Adding a New Evolution Condition
1. Create class in `Evolution/Conditions/` implementing `IEvolutionCondition`
2. Add to `EvolutionConditionType` enum
3. Add method to `EvolutionBuilder`
4. Add tests in `Tests/Evolution/EvolutionConditionTests.cs`

