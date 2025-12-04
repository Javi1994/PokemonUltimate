# 🚀 Getting Started with Pokemon Ultimate

> Quick guide for new developers and contributors.

## 📚 First Steps

### 1. Understand the Project Structure

```
PokemonUltimate/
├── PokemonUltimate.Core/          # Generic game engine (logic only)
├── PokemonUltimate.Combat/         # Battle system
├── PokemonUltimate.Content/        # Concrete game data (Pokemon, Moves, etc.)
├── PokemonUltimate.Tests/          # Test suite (2,460+ tests)
├── PokemonUltimate.SmokeTests/     # Runtime smoke tests
└── docs/                           # Documentation
```

### Project Structure Details

**Core** (`PokemonUltimate.Core`):
- **Target**: `netstandard2.1`
- **Purpose**: Generic, reusable game engine with zero concrete content
- **Contains**: Blueprints, Instances, Factories, Effects, Enums, Registry interfaces
- **Never contains**: Concrete Pokemon/Moves (no "Pikachu", no "Ember")

**Content** (`PokemonUltimate.Content`):
- **Target**: `netstandard2.1`
- **Purpose**: Concrete game content (specific Pokemon and Moves)
- **Dependencies**: `PokemonUltimate.Core`
- **Contains**: Builders (DSL), Catalogs (static content)
- **Never contains**: Game logic (no damage calculation, no turn order)

**Combat** (`PokemonUltimate.Combat`):
- **Purpose**: Battle engine, damage calculation, turn order
- **Dependencies**: `PokemonUltimate.Core`

**Tests** (`PokemonUltimate.Tests`):
- **Target**: `net8.0`
- **Purpose**: Unit tests for Core and Content
- **Dependencies**: `PokemonUltimate.Core`, `PokemonUltimate.Content`, `NUnit`

**SmokeTests** (`PokemonUltimate.SmokeTests`):
- **Target**: `net8.0`
- **Purpose**: Runtime smoke tests for validating all systems
- **Dependencies**: `PokemonUltimate.Core`, `PokemonUltimate.Content`, `PokemonUltimate.Combat`

### Dependency Rules

1. ✅ **Content → Core**: Content depends on Core
2. ✅ **Tests → Core + Content**: Tests can reference both
3. ✅ **SmokeTests → Core + Content + Combat**: SmokeTests can use all systems
4. ❌ **Core → Content**: Core must NEVER know about Content
5. ❌ **Core → Tests**: Core must NEVER depend on Tests

### 2. Read Essential Documentation

**Start here:**
1. `.ai/context.md` - Current project state and progress
2. `docs/ai/guidelines/project_guidelines.md` - Coding rules and standards
3. `docs/features_master_list.md` - **Feature numbering and naming standards**
4. `docs/ai/guidelines/feature_naming_in_code.md` - **MANDATORY: Feature references in code**
5. `docs/features/README.md` - Overview of all features
6. `docs/README.md` - Documentation structure overview

**Then read based on what you want to work on:**
- **Combat features** → `docs/features/2-combat-system/`
- **Content (Pokemon/Moves)** → `docs/features/3-content-expansion/`
- **Game data structure** → `docs/features/1-game-data/`
- **Variants system (Mega/Dinamax/Tera)** → `docs/features/1-game-data/1.18-variants-system/README.md`
- **Unity integration** → `docs/features/4-unity-integration/`
- **Game features** → `docs/features/5-game-features/`
- **Testing** → Each feature has `testing.md`. Shared strategy: `docs/ai/testing_structure_definition.md`
- **Game data testing** → `docs/features/1-game-data/testing.md`

### 3. Understand the Development Workflow

**⚠️ CRITICAL: Feature-Driven Development - ALWAYS start here**

**Before implementing ANY feature:**
0. ✅ **Feature Discovery & Assignment** ⭐ **MUST DO FIRST**
   - Read `docs/features_master_list.md` - Review all existing features
   - Determine if work fits existing feature or needs new one
   - If existing → Read feature's complete documentation
   - If new → Create feature folder and complete documentation structure
   - Update `docs/features_master_list.md` if creating new feature
1. ✅ Read `docs/ai/checklists/pre_implementation.md`
2. ✅ Check relevant roadmap for phase status and dependencies
3. ✅ Read relevant architecture spec in `docs/features/[N]-[feature-name]/architecture.md` or sub-feature `README.md` (always use numbered format)
4. ✅ **MANDATORY: Read existing code** - Review all related code files before writing (see `code_location.md`)
5. ✅ **MANDATORY: Understand existing patterns** - Match style, naming, structure of existing code
6. ✅ Follow TDD: Write tests first
7. ✅ **MANDATORY: Add feature references to XML docs** - ALL code must reference its feature (see `docs/ai/guidelines/feature_naming_in_code.md`)

**After implementing ANY feature:**
8. ✅ **Update Feature Documentation** ⭐ **MANDATORY**
   - Update `roadmap.md` - Mark completed phases
   - Update `architecture.md` - Reflect actual implementation
   - Update `use_cases.md` - Mark completed cases
   - Update `code_location.md` - Add new files
   - Update `testing.md` - Document tests
   - Update `docs/features_master_list.md` - Update status
   - Update `.ai/context.md` - Current state

**After implementing:**
1. ✅ Run `dotnet build` (must have 0 warnings)
2. ✅ Run `dotnet test` (all tests must pass)
3. ✅ Check `docs/ai/checklists/feature_complete.md`
4. ✅ Update roadmap if phase completed
5. ✅ Update `.ai/context.md` with new state

## 🗺️ Roadmaps Overview

| Roadmap | Purpose | Current Status |
|---------|---------|---------------|
| **Feature 1: Pokemon Data** | Complete data structure fields | ⏳ Planned |
| **Feature 2: Combat System** | Battle engine phases (2.1-2.19) | ✅ Core Complete (2.1-2.11) |
| **Feature 3: Content Expansion** | Add more Pokemon, Moves, Items | 🎯 In Progress (26 Pokemon, 36 Moves) |
| **Feature 4: Unity Integration** | UI and visual integration | ⏳ Planned (4.1-4.8) |
| **Feature 5: Game Features** | Progression, roguelike, meta-game | ⏳ Planned (5.1-5.6) |

See `docs/features/README.md` for detailed information.

## 🎯 What Can I Work On?

### ✅ Ready to Implement (Core Complete)

- **Extended Combat Features** (Phase 2.12-2.19)
  - Weather system
  - Terrain system
  - Entry hazards
  - Special move mechanics
  - Multi-hit and multi-turn moves

### ⏳ Planned (Check Roadmaps)

- **Content Expansion** (Phase 3.1-3.6)
  - Complete Gen 1 Pokemon
  - Expand move coverage
  - Add more items and abilities

- **Unity Integration** (Phase 4.1-4.8)
  - UI foundation
  - Player input
  - Animations and effects
  - Audio system

- **Game Features** (Phase 5.1-5.6)
  - Post-battle rewards
  - Pokemon management
  - Progression system
  - Save system

## 📋 Quick Reference

### Key Commands

```bash
# Build project
dotnet build

# Run all tests
dotnet test

# Run tests with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Key Documents

| Need | Document |
|------|----------|
| **Coding rules** | `docs/ai/guidelines/project_guidelines.md` |
| **What NOT to do** | `docs/ai/anti-patterns.md` |
| **Before coding** | `docs/ai/checklists/pre_implementation.md` |
| **After coding** | `docs/ai/checklists/feature_complete.md` |
| **Troubleshooting** | `docs/ai/workflow/troubleshooting.md` |
| **Refactoring** | `docs/ai/workflow/refactoring_guide.md` |
| **Test structure** | `docs/ai/testing_structure_definition.md` |
| **Integration tests** | `docs/features/2-combat-system/testing/integration_guide.md` |
| **Game data testing** | `docs/features/1-game-data/testing.md` |

### Architecture Specs

| System | Document |
|--------|----------|
| Combat System | `docs/features/2-combat-system/architecture.md` |
| Action System | `docs/features/2-combat-system/2.5-combat-actions/architecture.md` |
| Damage System | `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md` |
| Status Effects | `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md` |
| Abilities/Items | `docs/features/2-combat-system/2.9-abilities-items/architecture.md` |

## 🧪 Testing Standards

**Three-Phase Testing Approach:**
1. **Functional Tests** (`*Tests.cs`) - Core behavior
2. **Edge Cases** (`*EdgeCasesTests.cs`) - Boundaries and invalid inputs
3. **Integration Tests** (`*IntegrationTests.cs`) - System interactions

**Test Organization:**
- `Systems/` - How systems work
- `Blueprints/` - Data structure tests
- `Data/` - Content-specific tests

See `docs/ai/testing_structure_definition.md` for details.

## 🚨 Common Mistakes to Avoid

1. ❌ **Don't skip the pre-implementation checklist**
2. ❌ **Don't implement without checking the roadmap**
3. ❌ **Don't write code without tests first (TDD)**
4. ❌ **Don't use magic strings** - Use `ErrorMessages`/`GameMessages`
5. ❌ **Don't skip integration tests** - Required for system interactions
6. ❌ **Don't forget to update roadmaps** - Mark phases as complete

## 💡 Tips

- **Always check `.ai/context.md` first** - It has the current project state
- **Read the roadmap** before starting work - Understand dependencies
- **Follow TDD** - Tests reveal missing functionality
- **Use existing patterns** - Don't reinvent the wheel
- **Update documentation** - Keep it current

## 🆘 Need Help?

1. **Check troubleshooting guide**: `docs/ai/workflow/troubleshooting.md`
2. **Review examples**: `docs/ai/examples/good_code.md` and `docs/ai/examples/good_tests.md`
3. **Check architecture docs**: `docs/features/[N]-[feature-name]/architecture.md` or sub-feature `architecture.md` (always use numbered format)
4. **Review features**: `docs/features/README.md`

## 🎉 Ready to Contribute?

1. ✅ Read `.ai/context.md` and `docs/ai/guidelines/project_guidelines.md`
2. ✅ Pick a feature from a roadmap
3. ✅ Follow `docs/ai/checklists/pre_implementation.md`
4. ✅ Write tests first (TDD)
5. ✅ Implement following the spec
6. ✅ Complete `docs/ai/checklists/feature_complete.md`
7. ✅ Update roadmap and `.ai/context.md`

**Welcome to Pokemon Ultimate! 🎮**

