# 🤖 PokemonUltimate

> **An experiment in AI-assisted game development**: A complete Pokémon battle engine built entirely through structured AI pair programming.

[![Tests](https://img.shields.io/badge/tests-3,210%2B%20passing-brightgreen)](https://github.com)
[![AI Generated](https://img.shields.io/badge/AI%20Generated-100%25-blueviolet)](https://github.com)
[![Warnings](https://img.shields.io/badge/warnings-0-success)](https://github.com)
[![Unity Integration](https://img.shields.io/badge/Unity-Basic%20Complete-green)](https://github.com)

---

## 🎯 What Is This Project?

This project answers the question:

> **"Can AI build a complete, production-quality game engine from scratch?"**

**The answer: Yes, with the right methodology.**

> **New to the project?** Start with [`docs/ai/GETTING_STARTED.md`](docs/ai/GETTING_STARTED.md) for a quick guide.

| Role            | Responsibility                                         |
| --------------- | ------------------------------------------------------ |
| **Human**       | Direction, requirements, feedback, approval            |
| **AI (Claude)** | All code, tests, documentation, architecture decisions |
| **Tools**       | Cursor IDE with custom AI rules                        |

### Results

| Metric                | Value                           |
| --------------------- | ------------------------------- |
| **Lines of Code**     | ~15,000+                        |
| **Test Cases**        | 3,210+ passing                  |
| **Integration Tests** | 90+ tests                       |
| **Test Pass Rate**    | 100%                            |
| **Compiler Warnings** | 0                               |
| **Architecture Docs** | 30+ files                       |
| **Workflow Guides**   | 8+ comprehensive guides         |
| **Coding Rules**      | 24+ enforced                    |
| **Unity Integration** | Basic Complete (Phases 4.1-4.3) |
| **Pokemon Catalog**   | 26 Pokemon (Gen 1)              |
| **Move Catalog**      | 36 Moves (12 types)             |

---

## 🔄 The AI Development Workflow

The core innovation of this project is a **comprehensive, structured AI-assisted development workflow** that ensures quality, consistency, and maintainability across thousands of lines of code.

### 9-Step Mandatory Development Process

Every feature follows this exact workflow:

| Step | Action                          | Details                                                                |
| ---- | ------------------------------- | ---------------------------------------------------------------------- |
| 1    | **Read Context & Specs**        | Read `.ai/context.md`, architecture specs, complete incomplete specs   |
| 2    | **Verify Spec Completeness**    | Ensure all details documented (interfaces, classes, methods, examples) |
| 3    | **TDD: Write Functional Tests** | Create `[Feature]Tests.cs` with all main scenarios (red phase)         |
| 4    | **Implement Feature**           | Follow spec exactly, use existing patterns, make tests pass (green)    |
| 5    | **Write Edge Case Tests**       | Create `[Feature]EdgeCasesTests.cs` for boundaries, nulls, real-world  |
| 6    | **Write Integration Tests**     | Mandatory for system interactions (see integration guide)              |
| 7    | **Validate Use Cases**          | Check `docs/combat_use_cases.md` for combat features                   |
| 8    | **Verify Implementation**       | Build (0 warnings), test (all pass), check checklists                  |
| 9    | **Update Documentation**        | Update `.ai/context.md`, architecture docs, use cases                  |

### Problem-Solving Process

When issues arise during development:

| Situation                        | Action                                           |
| -------------------------------- | ------------------------------------------------ |
| **Spec Incomplete**              | Complete spec first, then implement              |
| **Spec Incorrect**               | Document discrepancy, fix spec or implementation |
| **Test Reveals Missing Feature** | Implement immediately (Test-Driven Discovery)    |
| **Architectural Change Needed**  | Document discovery, evaluate impact, update docs |

### Refactoring Process

Safe code improvement follows this pattern:

1. **Identify Scope** - What needs improvement? Check `docs/ai/anti-patterns.md`
2. **Verify Tests Exist** - If not, write tests first using TDD
3. **Refactor Safely** - Small incremental changes, run tests after each
4. **Maintain API Compatibility** - Or document breaking changes
5. **Update Documentation** - Architecture docs if pattern changed

---

## 📂 AI Infrastructure

The project includes a complete AI guidance system:

```
.ai/
└── context.md                    # Live project state (AI reads first)

.cursorrules                      # Auto-loaded rules for Cursor IDE

docs/
├── anti-patterns.md              # What NOT to do (with examples)
│
├── checklists/                   # Quality verification
│   ├── pre_implementation.md     # Before coding checklist
│   └── feature_complete.md       # Completion checklist
│
├── workflow/                     # Process guides
│   ├── troubleshooting.md        # Problem-solving guide
│   └── refactoring_guide.md      # Safe refactoring process
│
├── features/                     # Feature-specific documentation
│   ├── 1-game-data/             # Game data structure
│   ├── 2-combat-system/         # Battle engine
│   ├── 3-content-expansion/      # Adding content
│   ├── 4-unity-integration/      # Unity integration
│   ├── 5-game-features/         # Game systems
│   └── features_master_list.md  # Master feature reference
└── shared/                      # Shared documentation
    ├── architecture/            # Shared technical specs
    ├── workflow/                # Process guides
    ├── checklists/              # Reusable checklists
    ├── examples/                # Code examples
    └── prompts/                 # Prompt templates
│
├── examples/                     # Code reference
│   ├── good_code.md              # Correct patterns to follow
│   └── good_tests.md             # Test patterns (functional, edge, integration)
│
├── prompts/                      # Reusable prompt templates
│   ├── new_feature.md            # Template for implementing features
│   ├── code_review.md            # Template for code reviews
│   └── edge_cases.md             # Template for finding edge cases
│
└── architecture/                 # System specifications (20+ docs)
    ├── combat_system_spec.md
    ├── action_system_spec.md
    └── ...
```

### Configuration Files

| File             | Purpose                         | Auto-loaded       |
| ---------------- | ------------------------------- | ----------------- |
| `.cursorrules`   | Rules for every AI conversation | ✅ Yes            |
| `.ai/context.md` | Current project state           | ✅ AI reads first |

---

## ⚡ Key Workflow Features

| Feature                        | Description                                                           |
| ------------------------------ | --------------------------------------------------------------------- |
| **Three-Phase Testing**        | Functional → Edge Cases → Integration Tests                           |
| **Test-Driven Discovery**      | Tests reveal missing functionality → implement immediately            |
| **Structured Problem-Solving** | Clear process for handling incomplete specs, errors, missing features |
| **Integration Test Standard**  | Mandatory for system interactions, standardized patterns              |
| **Living Documentation**       | Architecture specs updated as features are implemented                |
| **Quality Checklists**         | Pre-implementation and feature-complete checklists                    |
| **Anti-Pattern Library**       | What NOT to do, with examples                                         |
| **Prompt Templates**           | Reusable templates for common tasks                                   |

---

## 🗺️ Roadmaps

| Roadmap                          | Purpose                                        | Status                            |
| -------------------------------- | ---------------------------------------------- | --------------------------------- | --------------------------- |
| **Feature 1: Game Data**         | `docs/features/1-game-data/roadmap.md`         | Complete data structure fields    | ✅ Core Complete            |
| **Feature 2: Combat System**     | `docs/features/2-combat-system/roadmap.md`     | Core combat phases (2.1-2.19)     | ✅ Core Complete            |
| **Feature 3: Content Expansion** | `docs/features/3-content-expansion/roadmap.md` | Pokemon, Moves, Items expansion   | 🎯 In Progress              |
| **Feature 4: Unity Integration** | `docs/features/4-unity-integration/roadmap.md` | Unity UI and integration          | ✅ Basic Complete (4.1-4.3) |
| **Feature 5: Game Features**     | `docs/features/5-game-features/roadmap.md`     | Progression, roguelike, meta-game | ⏳ Planned                  |

See `docs/features/README.md` for overview of all features.

## 📋 Workflow Guides

| Guide                   | Purpose                                  | Link                                                                                                                       |
| ----------------------- | ---------------------------------------- | -------------------------------------------------------------------------------------------------------------------------- |
| **Pre-Implementation**  | Checklist before coding                  | [`docs/ai/checklists/pre_implementation.md`](docs/ai/checklists/pre_implementation.md)                                     |
| **Feature Complete**    | Checklist before marking done            | [`docs/ai/checklists/feature_complete.md`](docs/ai/checklists/feature_complete.md)                                         |
| **Troubleshooting**     | Common issues and solutions              | [`docs/ai/workflow/troubleshooting.md`](docs/ai/workflow/troubleshooting.md)                                               |
| **Refactoring**         | Safe code improvement process            | [`docs/ai/workflow/refactoring_guide.md`](docs/ai/workflow/refactoring_guide.md)                                           |
| **Integration Testing** | System integration test patterns         | [`docs/features/2-combat-system/testing/integration_guide.md`](docs/features/2-combat-system/testing/integration_guide.md) |
| **Game Data Testing**   | Comprehensive game data testing strategy | [`docs/features/1-game-data/testing.md`](docs/features/1-game-data/testing.md)                                             |
| **Anti-Patterns**       | What NOT to do                           | [`docs/ai/anti-patterns.md`](docs/ai/anti-patterns.md)                                                                     |

---

## 🔄 How to Use This Methodology

Want to apply this AI workflow to your own project?

### 1. Set Up Rules

Create `.cursorrules` with your coding standards:

```markdown
# Project Rules

## Automatic Context Loading

ALWAYS read these files at the start of any task:

1. `.ai/context.md` - Current project state
2. `.cursorrules` - Coding rules and guidelines

## Development Workflow

When implementing a feature:

1. Read context and specs
2. Verify spec completeness
3. Write tests first (TDD)
4. Implement feature
5. Write edge case tests
6. Write integration tests
7. Validate against use cases
8. Verify implementation
9. Update documentation
```

### 2. Create Context File

Maintain `.ai/context.md` with project state:

```markdown
# AI Context Summary

## Current Project State

| Aspect            | Status    |
| ----------------- | --------- |
| **Current Phase** | Phase X   |
| **Tests**         | N passing |
| **Warnings**      | 0         |

## Completed Systems

-   [x] Feature A
-   [x] Feature B
-   [ ] Feature C (in progress)

## Key Architectural Decisions

| Decision  | Rationale |
| --------- | --------- |
| Pattern X | Reason Y  |
```

### 3. Document Architecture

Write specs before implementation in `docs/features/[feature-name]/architecture.md` or `docs/shared/architecture/`:

````markdown
# Feature Specification

## Overview

What this feature does.

## API

-   `ClassName.MethodName()` - Description

## Examples

```csharp
// Usage example
```
````

## Test Cases

1. Test scenario A
2. Test scenario B

````

### 4. Enforce TDD

Require tests before code in your rules:

```markdown
## TDD Mandate
- Write functional tests FIRST
- Write edge case tests after implementation
- Write integration tests for system interactions
- If test reveals missing functionality → implement it
````

### 5. Use Checklists

Create verification checklists:

**Pre-Implementation:**

-   [ ] Read architecture spec
-   [ ] Understand requirements
-   [ ] Identify test cases

**Feature Complete:**

-   [ ] All tests pass
-   [ ] 0 warnings
-   [ ] Documentation updated
-   [ ] Use cases validated

### 6. Provide Examples

Show the AI what good code looks like in `docs/ai/examples/`:

````markdown
# Good Code Examples

## Correct Pattern

```csharp
// This is the right way
```
````

## Anti-Pattern

```csharp
// Don't do this
```

```

---

## 📊 What We Learned

| Finding | Details |
|---------|---------|
| ✅ **Consistency is possible** | With proper documentation, AI maintains patterns across 15,000+ lines |
| ✅ **TDD works** | AI follows test-first development when explicitly instructed |
| ✅ **Edge cases are thorough** | AI-generated edge case tests often reveal missing functionality |
| ✅ **Documentation stays current** | AI updates docs as it implements features |
| ✅ **Complex domains work** | AI understands game mechanics (stat formulas, type charts, etc.) |
| ✅ **Self-review helps** | AI can review and improve its own code when asked |
| ✅ **Structure prevents drift** | Checklists and rules prevent quality degradation over time |
| ✅ **Integration tests catch bugs** | System interaction tests reveal issues unit tests miss |

---

---

# 🎮 The Game Engine

Everything below this line describes the actual Pokémon battle engine that was built using the AI workflow above.

---

## 🎯 Game Overview

PokemonUltimate is a battle-focused Pokémon game engine built with clean architecture principles. The core logic is completely testable without Unity, making it perfect for both game development and battle simulation. The engine is now integrated with Unity, providing a visual battle experience with UI components and IBattleView implementation.

### Key Features

- ✅ **Complete Combat System** - Full battle mechanics with actions, turn order, damage calculation, abilities, items, weather, terrain, hazards
- ✅ **Test-Driven Development** - 3,210+ passing tests with comprehensive coverage
- ✅ **Modular Architecture** - Clean separation between Core, Combat, and Content
- ✅ **Accurate Game Mechanics** - Gen 6+ type chart, Gen 3+ stat formulas, official damage calculations
- ✅ **Integration Testing** - 90+ integration tests verifying system interactions
- ✅ **AI vs AI Battles** - Built-in AI providers for automated testing and demos
- ✅ **Unity Integration** - Basic UI foundation and IBattleView implementation complete
- ✅ **Developer Tools** - Unified Windows Forms application with 7 debugger tabs (Battle, Move, Type Matchup, Stat Calculator, Damage Calculator, Status Effect, Turn Order)
- ✅ **Data Viewer** - Windows Forms application for visually browsing all game data (10 data types: Pokemon, Moves, Items, Abilities, Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects)
- ✅ **Battle Simulator** - Interactive battle simulator with real-time logs, automatic log saving, and batch simulation
- ✅ **Content System** - 26 Pokemon, 36 Moves, 35 Abilities, 23 Items cataloged

---

## 📁 Project Structure

```

PokemonUltimate/
├── Core/ # Game logic (NO game data)
│ ├── Blueprints/ # Immutable data structures
│ ├── Instances/ # Mutable runtime state
│ ├── Effects/ # Move effects (IMoveEffect)
│ ├── Factories/ # Object creation (DI-based)
│ ├── Registry/ # Data access layer
│ └── Constants/ # Centralized strings
│
├── Combat/ # Battle system
│ ├── Engine/ # CombatEngine, BattleQueue, TurnOrderResolver
│ ├── Actions/ # BattleAction implementations
│ ├── Damage/ # DamagePipeline with modular steps
│ ├── Field/ # BattleField, BattleSide, BattleSlot
│ ├── Events/ # BattleTrigger system, IBattleListener
│ ├── AI/ # RandomAI, AlwaysAttackAI
│ └── Providers/ # IActionProvider, PlayerInputProvider
│
├── Content/ # Game data definitions
│ └── Catalogs/ # Pokemon, Move, Ability, Item definitions
│
├── Tests/ # Comprehensive test suite
│ ├── Systems/ # System tests (HOW systems work)
│ ├── Blueprints/ # Data structure tests (HOW data is structured)
│ └── Data/ # Content tests (WHAT data contains)
│
├── BattleDemo/ # Visual AI vs AI battle simulator
│
├── DeveloperTools/ # Unified debugger application with 7 tabs (Windows Forms)
│ └── README.md # Complete documentation
│
├── DataViewer/ # Data browser application with 10 data tabs (Windows Forms)
│ └── README.md # Complete documentation
│
├── BattleSimulator/ # Interactive battle simulator with automatic log saving (Windows Forms)
│ ├── README.md # Complete documentation
│ └── Logs/ # Automatically saved battle logs (for AI debugging)
│
└── PokemonUltimateUnity/ # Unity project
├── Assets/
│ ├── Plugins/ # Battle engine DLLs
│ ├── Scripts/ # Unity C# scripts
│ │ ├── Battle/ # UnityBattleView, BattleManager
│ │ └── UI/ # HPBar, PokemonDisplay, BattleDialog
│ └── Scenes/ # BattleScene

````

---

## 🚀 Quick Start

### Prerequisites

- .NET SDK 8.0 or later
- IDE with C# support (Rider, Visual Studio, VS Code)
- Unity 6 (or Unity 2021.3+) - For Unity integration (optional)

### Building

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/PokemonUltimate.git
cd PokemonUltimate

# Build
dotnet build

# Run tests
dotnet test

# Run battle demo (AI vs AI battles)
dotnet run --project PokemonUltimate.BattleDemo

# Run development tools (Windows Forms application with all debuggers)
dotnet run --project PokemonUltimate.DeveloperTools

# Run data viewer (Windows Forms application for browsing game data)
dotnet run --project PokemonUltimate.DataViewer

# Run battle simulator (Interactive battle simulator with automatic log saving)
dotnet run --project PokemonUltimate.BattleSimulator

# Build DLLs for Unity (optional)
dotnet build -c Release
# DLLs will be in: PokemonUltimate.Core/bin/Release/netstandard2.1/
#                   PokemonUltimate.Combat/bin/Release/netstandard2.1/
#                   PokemonUltimate.Content/bin/Release/netstandard2.1/
````

---

## 📊 Current Status

### ✅ Completed Systems

**Phase 1: Core Data**

-   Pokemon species blueprints with stats, types, abilities
-   Move system with composable effects (21 effect types)
-   Type effectiveness (Gen 6+ chart)
-   Stat calculation (Gen 3+ formulas)
-   Evolution system (Level, Item, Trade, Friendship)

**Phase 2: Instances**

-   PokemonInstance with battle state management
-   MoveInstance with PP tracking
-   Level up and move learning
-   Ability and Item assignment

**Phase 3: Combat System**

-   ✅ Battle Foundation (BattleField, Slots, Sides)
-   ✅ Action Queue System (BattleQueue, BattleAction)
-   ✅ Turn Order Resolution (Priority → Speed → Random)
-   ✅ Damage Calculation (Modular pipeline with 6 steps)
-   ✅ Combat Actions (UseMove, Switch, Damage, Status, etc.)
-   ✅ Combat Engine (Full battle loop)
-   ✅ Integration (AI providers, Player input, Full battles)
-   ✅ End-of-Turn Effects (Status damage: Burn, Poison, Toxic)
-   ✅ Abilities & Items (BattleTrigger system, AbilityListener, ItemListener)
-   ✅ Weather System (9 weather conditions with damage modifiers)
-   ✅ Terrain System (4 terrains with damage modifiers and healing)
-   ✅ Hazards System (Stealth Rock, Spikes, Toxic Spikes, Sticky Web)
-   ✅ Advanced Abilities & Items (29+ abilities, 21+ items tested)

**Phase 4: Unity Integration** ✅ Basic Complete

-   ✅ Unity Project Setup (DLL integration, project structure)
-   ✅ UI Foundation (HPBar, PokemonDisplay, BattleDialog, scene generator)
-   ✅ IBattleView Implementation (UnityBattleView, BattleManager, UnityBattleLogger)
-   ⏳ Player Input System (Phase 4.4 - Planned)
-   ⏳ Animations & Visual Effects (Phase 4.5 - Planned)
-   ⏳ Audio System (Phase 4.6 - Planned)

### 🎯 Next Steps

See detailed roadmaps for implementation plans:

-   **Feature 1: Game Data**: `docs/features/1-game-data/roadmap.md` ✅ Core Complete (Optional: IVs/EVs, Breeding, Ownership tracking)
-   **Feature 2: Combat System**: `docs/features/2-combat-system/roadmap.md` ✅ Core Complete (Optional: Advanced moves, Battle formats)
-   **Feature 3: Content Expansion**: `docs/features/3-content-expansion/roadmap.md` 🎯 In Progress (26/151 Gen 1 Pokemon, 36 moves, expanding)
-   **Feature 4: Unity Integration**: `docs/features/4-unity-integration/roadmap.md` ✅ Basic Complete (Next: Player Input, Animations, Audio)
-   **Feature 5: Game Features**: `docs/features/5-game-features/roadmap.md` ⏳ Planned (Progression, roguelike, meta-game)
-   **Testing**: Each feature has `testing.md`. Shared strategy: `docs/ai/testing_structure_definition.md`

---

## 🔧 Development Tools

The project includes three specialized Windows Forms applications for testing, debugging, and analyzing the battle system:

### 1. Developer Tools (`PokemonUltimate.DeveloperTools`)

**Unified Windows Forms application** with tabbed interface integrating all debuggers:

| Tab                         | Purpose                         | Features                                                       |
| --------------------------- | ------------------------------- | -------------------------------------------------------------- |
| **Battle Debugger** (6.5)   | Battle statistics and analysis  | Move usage stats, status effects, win/loss/draw rates          |
| **Move Debugger** (6.6)     | Move testing and statistics     | Damage stats, critical hits, status effects, action generation |
| **Type Matchup**            | Type effectiveness testing      | Type chart, dual types, immunities, effectiveness breakdown    |
| **Stat Calculator** (6.1)   | Pokemon stat calculation        | Visualize stats with different levels, natures, IVs, EVs       |
| **Damage Calculator** (6.2) | Step-by-step damage calculation | Complete pipeline visualization with all multipliers           |
| **Status Effect** (6.3)     | Status effect testing           | Test application, duration, and interactions                   |
| **Turn Order** (6.4)        | Turn order visualization        | Speed and priority-based action ordering                       |

**Usage**:

```bash
# Run development tools (opens with all debugger tabs)
dotnet run --project PokemonUltimate.DeveloperTools
```

**Features**:

-   ✅ **Tabbed Interface** - All debuggers in one application
-   ✅ **Visual Windows Forms UI** - Easy configuration with dropdowns
-   ✅ **Real-time progress tracking** - See progress during execution
-   ✅ **Comprehensive statistics** - Detailed tables and summaries
-   ✅ **No code editing required** - Configure everything through the UI

**Documentation**: See [`PokemonUltimate.DeveloperTools/README.md`](PokemonUltimate.DeveloperTools/README.md) and [`docs/features/6-development-tools/README.md`](docs/features/6-development-tools/README.md)

### 2. Data Viewer (`PokemonUltimate.DataViewer`)

**Windows Forms application** for visually browsing and exploring all game data:

| Tab                 | Data Type          | Description                                    |
| ------------------- | ------------------ | ---------------------------------------------- |
| **Pokemon**         | Species data       | Stats, types, abilities, learnsets, evolution  |
| **Moves**           | Move data          | Type, power, accuracy, PP, effects             |
| **Items**           | Item data          | Effects, properties, categories                |
| **Abilities**       | Ability data       | Effects, triggers, conditions                  |
| **Status Effects**  | Status data        | Burn, Paralysis, Sleep, Poison, etc.           |
| **Weather**         | Weather conditions | Rain, Sun, Hail, Sandstorm, etc.               |
| **Terrain**         | Terrain conditions | Grassy, Electric, Psychic, Misty               |
| **Hazards**         | Entry hazards      | Stealth Rock, Spikes, Toxic Spikes, Sticky Web |
| **Side Conditions** | Side conditions    | Reflect, Light Screen, Safeguard, etc.         |
| **Field Effects**   | Field effects      | Trick Room, Gravity, etc.                      |

**Usage**:

```bash
# Run data viewer
dotnet run --project PokemonUltimate.DataViewer
```

**Features**:

-   ✅ **Interactive Data Grids** - Click rows to see detailed information
-   ✅ **Details Panel** - Complete information for selected items
-   ✅ **Consistent Interface** - Same layout across all tabs
-   ✅ **Complete Coverage** - All game data types in one application

**Documentation**: See [`PokemonUltimate.DataViewer/README.md`](PokemonUltimate.DataViewer/README.md) and [`docs/features/6-development-tools/6.7-data-viewer/README.md`](docs/features/6-development-tools/6.7-data-viewer/README.md)

### 3. Battle Simulator (`PokemonUltimate.BattleSimulator`)

**Interactive battle simulator** with real-time logs and automatic log saving:

**Features**:

-   ✅ **Multiple Battle Modes** - Singles (1v1), Doubles (2v2), Triples (3v3), Horde (1v3/1v5), Custom
-   ✅ **Team Configuration** - Manual selection or random generation with level, nature, IVs
-   ✅ **Batch Simulation** - Run multiple battles consecutively with statistics
-   ✅ **Real-time Logs** - Color-coded logs with filtering (Debug, Info, Warning, Error, Battle Events)
-   ✅ **Automatic Log Saving** - All battles automatically saved to `Logs/` folder
-   ✅ **Status Effect Logging** - Complete logging including when effects prevent movement
-   ✅ **Battle Statistics** - Detailed results per team with kill history
-   ✅ **AI vs AI Battles** - Automated battles with configurable AI

**Usage**:

```bash
# Run battle simulator
dotnet run --project PokemonUltimate.BattleSimulator
```

**Log Files**:

-   Automatically saved to `PokemonUltimate.BattleSimulator/Logs/`
-   Format: `battle_logs_YYYYMMDD_HHMMSS.txt` (single battles)
-   Format: `battle_logs_YYYYMMDD_HHMMSS_battleXofY.txt` (batch battles)
-   **Purpose**: Logs are designed for rapid AI debugging - complete battle traceability

**Documentation**: See [`PokemonUltimate.BattleSimulator/README.md`](PokemonUltimate.BattleSimulator/README.md) and [`docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`](docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md)

### Quick Reference

| Tool                 | Purpose                  | When to Use                                                   |
| -------------------- | ------------------------ | ------------------------------------------------------------- |
| **Developer Tools**  | Debug specific mechanics | Testing damage calculations, stat formulas, status effects    |
| **Data Viewer**      | Browse game data         | Quick reference, verify data, explore relationships           |
| **Battle Simulator** | Full battle simulation   | Test complete battles, analyze AI behavior, debug battle flow |

See [`docs/features/6-development-tools/README.md`](docs/features/6-development-tools/README.md) for complete documentation on all development tools.

---

## 🎮 Unity Integration

The engine is integrated with Unity for visual battles. Basic implementation includes:

### ✅ Completed (Phases 4.1-4.3)

-   **DLL Integration**: Battle engine DLLs imported as Unity plugins
-   **UI Foundation**:
    -   `HPBar` - Visual HP representation
    -   `PokemonDisplay` - Pokemon sprite, name, and level display
    -   `BattleDialog` - Battle message system with typewriter effect
    -   `BattleSceneGenerator` - Automated scene creation tool
-   **IBattleView Implementation**:
    -   `UnityBattleView` - Full IBattleView interface implementation
    -   `BattleManager` - Battle orchestration and lifecycle
    -   `UnityBattleLogger` - Unity-specific logging

### 🎯 Using Unity Integration

1. **Open Unity Project**: Open `PokemonUltimateUnity/` in Unity Editor
2. **Build DLLs**: Run `dotnet build -c Release` to generate DLLs
3. **Copy DLLs**: Copy DLLs to `PokemonUltimateUnity/Assets/Plugins/`
4. **Generate Scene**: Use `PokemonUltimate > Generate Battle Scene` menu
5. **Run Battle**: Attach `BattleManager` to a GameObject and start a battle

See [`docs/features/4-unity-integration/README.md`](docs/features/4-unity-integration/README.md) for complete documentation.

---

---

## 🏗️ Architecture Principles

### Core Philosophy

1. **Testability First** - All logic testable without Unity
2. **Action Queue Pattern** - Complex systems use action queues
3. **Input Symmetry** - Logic doesn't know Human vs AI
4. **Composition over Inheritance** - Moves use effects, not subclasses
5. **Registry Pattern** - No direct file loading in logic
6. **Slot System** - Supports 1v1, 2v2, 1v3, Horde modes
7. **Pipeline Pattern** - Complex math uses modular steps
8. **Event-Driven Extensions** - Abilities/Items use listeners

### Code Quality Standards

-   ✅ **No Magic Strings** - Centralized constants (`ErrorMessages`, `GameMessages`)
-   ✅ **No Magic Numbers** - Named constants only
-   ✅ **Fail-Fast** - Exceptions for invalid inputs
-   ✅ **Guard Clauses** - Early validation
-   ✅ **TDD Mandatory** - Tests before implementation
-   ✅ **XML Documentation** - All public APIs documented

---

## 🎮 Game Mechanics

### Type Effectiveness

-   Gen 6+ chart (Fairy type included)
-   STAB = 1.5x multiplier
-   Dual-type multipliers combine

### Stat Calculation

-   Gen 3+ formulas
-   HP formula differs from other stats
-   Nature modifiers (0.9x, 1.0x, 1.1x)
-   IVs (0-31) and EVs (0-252 per stat)

### Battle Mechanics

-   Turn order: Priority → Speed → Random
-   Damage pipeline: Base → Crit → Random → STAB → Type → Status (modular 6-step pipeline)
-   Status effects: Burn, Poison, Toxic, Sleep, Freeze, Paralysis (6 persistent + 9 volatile)
-   Stat stages: -6 to +6 with proper multipliers
-   End-of-turn effects: Status damage processing, weather damage, terrain healing
-   Abilities & Items: Event-driven system with BattleTrigger (29+ abilities, 21+ items)
-   Weather: 9 weather conditions (5 standard + 3 primal + fog) with damage modifiers
-   Terrain: 4 terrains (Grassy, Electric, Psychic, Misty) with damage modifiers and healing
-   Hazards: 4 entry hazards (Stealth Rock, Spikes, Toxic Spikes, Sticky Web)

---

## 🧪 Testing

The project follows **Test-Driven Development (TDD)** with three-phase testing:

1. **Functional Tests** - Core behavior verification (Systems/ folder)
2. **Edge Case Tests** - Boundary conditions and real-world scenarios (Systems/ folder)
3. **Integration Tests** - System interactions and cascading effects (Systems/[Feature]/Integration/)

**Test Organization**:

-   `Systems/` - Tests de sistemas (CÓMO funcionan los sistemas)
-   `Blueprints/` - Tests de estructura de datos (CÓMO son los datos)
-   `Data/` - Tests de contenido específico (QUÉ contienen los datos)

### Running Tests

```bash
# All tests
dotnet test

# Specific test category
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Single test file
dotnet test --filter "FullyQualifiedName~CombatEngineTests"
```

---

## 📖 Architecture Documents

| Document                                                                                                                                                         | Purpose                      |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| [`docs/features_master_list.md`](docs/features_master_list.md)                                                                                                   | Master feature reference ⭐  |
| [`docs/features/1-game-data/architecture.md`](docs/features/1-game-data/architecture.md)                                                                         | Game data structure design   |
| [`docs/features/2-combat-system/architecture.md`](docs/features/2-combat-system/architecture.md)                                                                 | Battle system design         |
| [`docs/features/2-combat-system/2.5-combat-actions/architecture.md`](docs/features/2-combat-system/2.5-combat-actions/architecture.md)                           | BattleAction implementations |
| [`docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`](docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md) | Modular damage calculation   |
| [`docs/features/2-combat-system/use_cases.md`](docs/features/2-combat-system/use_cases.md)                                                                       | All battle mechanics         |

---

## 📝 License

This is a non-commercial fan project for educational purposes. Pokémon names and game mechanics are trademarks of Nintendo/Game Freak/The Pokémon Company.

## 🙏 Acknowledgments

-   **Claude (Anthropic)** — AI that wrote this entire codebase
-   Pokémon community for documenting game formulas
-   Built with ❤️ as an experiment in AI-assisted development

---

<p align="center">
  <strong>🤖 100% AI-Generated Code | 3,210+ Tests | 0 Warnings | Unity Integration Basic Complete</strong>
</p>
