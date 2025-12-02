# 🤖 PokemonUltimate

> **An experiment in AI-assisted game development**: A complete Pokémon battle engine built entirely through structured AI pair programming.

[![Tests](https://img.shields.io/badge/tests-2,075%2B%20passing-brightgreen)](https://github.com)
[![AI Generated](https://img.shields.io/badge/AI%20Generated-100%25-blueviolet)](https://github.com)
[![Warnings](https://img.shields.io/badge/warnings-0-success)](https://github.com)

---

## 🎯 What Is This Project?

This project answers the question:

> **"Can AI build a complete, production-quality game engine from scratch?"**

**The answer: Yes, with the right methodology.**

| Role | Responsibility |
|------|----------------|
| **Human** | Direction, requirements, feedback, approval |
| **AI (Claude)** | All code, tests, documentation, architecture decisions |
| **Tools** | Cursor IDE with custom AI rules |

### Results

| Metric | Value |
|--------|-------|
| **Lines of Code** | ~8,000+ |
| **Test Cases** | 2,075+ passing |
| **Integration Tests** | 66 tests |
| **Test Pass Rate** | 100% |
| **Compiler Warnings** | 0 |
| **Architecture Docs** | 20+ files |
| **Workflow Guides** | 5+ comprehensive guides |
| **Coding Rules** | 24+ enforced |

---

## 🔄 The AI Development Workflow

The core innovation of this project is a **comprehensive, structured AI-assisted development workflow** that ensures quality, consistency, and maintainability across thousands of lines of code.

### 9-Step Mandatory Development Process

Every feature follows this exact workflow:

| Step | Action | Details |
|------|--------|---------|
| 1 | **Read Context & Specs** | Read `.ai/context.md`, architecture specs, complete incomplete specs |
| 2 | **Verify Spec Completeness** | Ensure all details documented (interfaces, classes, methods, examples) |
| 3 | **TDD: Write Functional Tests** | Create `[Feature]Tests.cs` with all main scenarios (red phase) |
| 4 | **Implement Feature** | Follow spec exactly, use existing patterns, make tests pass (green) |
| 5 | **Write Edge Case Tests** | Create `[Feature]EdgeCasesTests.cs` for boundaries, nulls, real-world |
| 6 | **Write Integration Tests** | Mandatory for system interactions (see integration guide) |
| 7 | **Validate Use Cases** | Check `docs/combat_use_cases.md` for combat features |
| 8 | **Verify Implementation** | Build (0 warnings), test (all pass), check checklists |
| 9 | **Update Documentation** | Update `.ai/context.md`, architecture docs, use cases |

### Problem-Solving Process

When issues arise during development:

| Situation | Action |
|-----------|--------|
| **Spec Incomplete** | Complete spec first, then implement |
| **Spec Incorrect** | Document discrepancy, fix spec or implementation |
| **Test Reveals Missing Feature** | Implement immediately (Test-Driven Discovery) |
| **Architectural Change Needed** | Document discovery, evaluate impact, update docs |

### Refactoring Process

Safe code improvement follows this pattern:

1. **Identify Scope** - What needs improvement? Check `docs/anti-patterns.md`
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
├── project_guidelines.md         # 24+ coding rules
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
├── testing/                      # Testing guides
│   └── integration_testing_guide.md  # Integration test patterns
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

| File | Purpose | Auto-loaded |
|------|---------|-------------|
| `.cursorrules` | Rules for every AI conversation | ✅ Yes |
| `.ai/context.md` | Current project state | ✅ AI reads first |
| `docs/project_guidelines.md` | 24+ coding rules | ✅ AI reads first |

---

## ⚡ Key Workflow Features

| Feature | Description |
|---------|-------------|
| **Three-Phase Testing** | Functional → Edge Cases → Integration Tests |
| **Test-Driven Discovery** | Tests reveal missing functionality → implement immediately |
| **Structured Problem-Solving** | Clear process for handling incomplete specs, errors, missing features |
| **Integration Test Standard** | Mandatory for system interactions, standardized patterns |
| **Living Documentation** | Architecture specs updated as features are implemented |
| **Quality Checklists** | Pre-implementation and feature-complete checklists |
| **Anti-Pattern Library** | What NOT to do, with examples |
| **Prompt Templates** | Reusable templates for common tasks |

---

## 📋 Workflow Guides

| Guide | Purpose | Link |
|-------|---------|------|
| **Pre-Implementation** | Checklist before coding | [`docs/checklists/pre_implementation.md`](docs/checklists/pre_implementation.md) |
| **Feature Complete** | Checklist before marking done | [`docs/checklists/feature_complete.md`](docs/checklists/feature_complete.md) |
| **Troubleshooting** | Common issues and solutions | [`docs/workflow/troubleshooting.md`](docs/workflow/troubleshooting.md) |
| **Refactoring** | Safe code improvement process | [`docs/workflow/refactoring_guide.md`](docs/workflow/refactoring_guide.md) |
| **Integration Testing** | System integration test patterns | [`docs/testing/integration_testing_guide.md`](docs/testing/integration_testing_guide.md) |
| **Project Guidelines** | 24+ mandatory coding rules | [`docs/project_guidelines.md`](docs/project_guidelines.md) |
| **Anti-Patterns** | What NOT to do | [`docs/anti-patterns.md`](docs/anti-patterns.md) |

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
2. `docs/project_guidelines.md` - Coding rules

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
| Aspect | Status |
|--------|--------|
| **Current Phase** | Phase X |
| **Tests** | N passing |
| **Warnings** | 0 |

## Completed Systems
- [x] Feature A
- [x] Feature B
- [ ] Feature C (in progress)

## Key Architectural Decisions
| Decision | Rationale |
|----------|-----------|
| Pattern X | Reason Y |
```

### 3. Document Architecture

Write specs before implementation in `docs/architecture/`:

```markdown
# Feature Specification

## Overview
What this feature does.

## API
- `ClassName.MethodName()` - Description

## Examples
```csharp
// Usage example
```

## Test Cases
1. Test scenario A
2. Test scenario B
```

### 4. Enforce TDD

Require tests before code in your rules:

```markdown
## TDD Mandate
- Write functional tests FIRST
- Write edge case tests after implementation
- Write integration tests for system interactions
- If test reveals missing functionality → implement it
```

### 5. Use Checklists

Create verification checklists:

**Pre-Implementation:**
- [ ] Read architecture spec
- [ ] Understand requirements
- [ ] Identify test cases

**Feature Complete:**
- [ ] All tests pass
- [ ] 0 warnings
- [ ] Documentation updated
- [ ] Use cases validated

### 6. Provide Examples

Show the AI what good code looks like in `docs/examples/`:

```markdown
# Good Code Examples

## Correct Pattern
```csharp
// This is the right way
```

## Anti-Pattern
```csharp
// Don't do this
```
```

---

## 📊 What We Learned

| Finding | Details |
|---------|---------|
| ✅ **Consistency is possible** | With proper documentation, AI maintains patterns across 8000+ lines |
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

PokemonUltimate is a battle-focused Pokémon game engine built with clean architecture principles. The core logic is completely testable without Unity, making it perfect for both game development and battle simulation.

### Key Features

- ✅ **Complete Combat System** - Full battle mechanics with actions, turn order, damage calculation
- ✅ **Test-Driven Development** - 2,075+ passing tests with comprehensive coverage
- ✅ **Modular Architecture** - Clean separation between Core, Combat, and Content
- ✅ **Accurate Game Mechanics** - Gen 6+ type chart, Gen 3+ stat formulas, official damage calculations
- ✅ **Integration Testing** - 66 integration tests verifying system interactions
- ✅ **AI vs AI Battles** - Built-in AI providers for automated testing and demos

---

## 📁 Project Structure

```
PokemonUltimate/
├── Core/              # Game logic (NO game data)
│   ├── Blueprints/    # Immutable data structures
│   ├── Instances/     # Mutable runtime state
│   ├── Effects/       # Move effects (IMoveEffect)
│   └── Constants/     # Centralized strings
│
├── Combat/            # Battle system
│   ├── Engine/       # CombatEngine, BattleQueue, TurnOrderResolver
│   ├── Actions/      # BattleAction implementations
│   ├── Damage/      # DamagePipeline with modular steps
│   ├── AI/          # RandomAI, AlwaysAttackAI
│   └── Providers/   # IActionProvider, PlayerInputProvider
│
├── Content/          # Game data definitions
│   └── Catalogs/     # Pokemon, Move, Ability, Item definitions
│
├── Tests/            # Comprehensive test suite
│   ├── [Module]/     # Mirrors source structure
│   └── Integration/  # System integration tests
│
└── BattleDemo/       # Visual AI vs AI battle simulator
```

---

## 🚀 Quick Start

### Prerequisites

- .NET SDK 8.0 or later
- IDE with C# support (Rider, Visual Studio, VS Code)

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
```

---

## 📊 Current Status

### ✅ Completed Systems

**Phase 1: Core Data**
- Pokemon species blueprints with stats, types, abilities
- Move system with composable effects (21 effect types)
- Type effectiveness (Gen 6+ chart)
- Stat calculation (Gen 3+ formulas)
- Evolution system (Level, Item, Trade, Friendship)

**Phase 2: Instances**
- PokemonInstance with battle state management
- MoveInstance with PP tracking
- Level up and move learning
- Ability and Item assignment

**Phase 3: Combat System**
- ✅ Battle Foundation (BattleField, Slots, Sides)
- ✅ Action Queue System (BattleQueue, BattleAction)
- ✅ Turn Order Resolution (Priority → Speed → Random)
- ✅ Damage Calculation (Modular pipeline with 6 steps)
- ✅ Combat Actions (UseMove, Switch, Damage, Status, etc.)
- ✅ Combat Engine (Full battle loop)
- ✅ Integration (AI providers, Player input, Full battles)
- ✅ End-of-Turn Effects (Status damage: Burn, Poison, Toxic)

### 🎯 Next Steps

- Phase 4: Abilities & Items (Event-driven system)
- Phase 5: Advanced Battle Mechanics (Weather, Terrain, Hazards)
- Phase 6: Unity Integration

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

- ✅ **No Magic Strings** - Centralized constants (`ErrorMessages`, `GameMessages`)
- ✅ **No Magic Numbers** - Named constants only
- ✅ **Fail-Fast** - Exceptions for invalid inputs
- ✅ **Guard Clauses** - Early validation
- ✅ **TDD Mandatory** - Tests before implementation
- ✅ **XML Documentation** - All public APIs documented

---

## 🎮 Game Mechanics

### Type Effectiveness
- Gen 6+ chart (Fairy type included)
- STAB = 1.5x multiplier
- Dual-type multipliers combine

### Stat Calculation
- Gen 3+ formulas
- HP formula differs from other stats
- Nature modifiers (0.9x, 1.0x, 1.1x)
- IVs (0-31) and EVs (0-252 per stat)

### Battle Mechanics
- Turn order: Priority → Speed → Random
- Damage pipeline: Base → Crit → Random → STAB → Type → Status
- Status effects: Burn, Poison, Toxic, Sleep, Freeze, Paralysis
- Stat stages: -6 to +6 with proper multipliers
- End-of-turn effects: Status damage processing

---

## 🧪 Testing

The project follows **Test-Driven Development (TDD)** with three-phase testing:

1. **Functional Tests** - Core behavior verification
2. **Edge Case Tests** - Boundary conditions and real-world scenarios
3. **Integration Tests** - System interactions and cascading effects

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

| Document | Purpose |
|----------|---------|
| [`docs/architecture/combat_system_spec.md`](docs/architecture/combat_system_spec.md) | Battle system design |
| [`docs/architecture/action_system_spec.md`](docs/architecture/action_system_spec.md) | BattleAction implementations |
| [`docs/architecture/damage_and_effect_system.md`](docs/architecture/damage_and_effect_system.md) | Modular damage calculation |
| [`docs/architecture/player_ai_spec.md`](docs/architecture/player_ai_spec.md) | Input provider system |
| [`docs/combat_use_cases.md`](docs/combat_use_cases.md) | All battle mechanics |
| [`docs/combat/actions_bible.md`](docs/combat/actions_bible.md) | Complete actions reference |

---

## 📝 License

This is a non-commercial fan project for educational purposes. Pokémon names and game mechanics are trademarks of Nintendo/Game Freak/The Pokémon Company.

## 🙏 Acknowledgments

- **Claude (Anthropic)** — AI that wrote this entire codebase
- Pokémon community for documenting game formulas
- Built with ❤️ as an experiment in AI-assisted development

---

<p align="center">
  <strong>🤖 100% AI-Generated Code | 2,075+ Tests | 0 Warnings</strong>
</p>
