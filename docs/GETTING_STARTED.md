# 🚀 Getting Started with Pokemon Ultimate

> Quick guide for new developers and contributors.

## 📚 First Steps

### 1. Understand the Project Structure

```
PokemonUltimate/
├── PokemonUltimate.Core/      # Game logic (no Unity dependencies)
├── PokemonUltimate.Combat/     # Battle system
├── PokemonUltimate.Content/    # Game data (Pokemon, Moves, etc.)
├── PokemonUltimate.Tests/     # Test suite (2,460+ tests)
└── docs/                       # Documentation
```

### 2. Read Essential Documentation

**Start here:**
1. `.ai/context.md` - Current project state and progress
2. `docs/project_guidelines.md` - Coding rules and standards
3. `docs/roadmaps/README.md` - Overview of all roadmaps

**Then read based on what you want to work on:**
- **Combat features** → `docs/roadmaps/combat_roadmap.md`
- **Content (Pokemon/Moves)** → `docs/roadmaps/content_expansion_roadmap.md`
- **Unity integration** → `docs/roadmaps/unity_integration_roadmap.md`
- **Game features** → `docs/roadmaps/game_features_roadmap.md`
- **Testing** → `docs/roadmaps/testing_roadmap.md`

### 3. Understand the Development Workflow

**Before implementing ANY feature:**
1. ✅ Read `docs/checklists/pre_implementation.md`
2. ✅ Check relevant roadmap for phase status and dependencies
3. ✅ Read relevant architecture spec in `docs/architecture/`
4. ✅ Follow TDD: Write tests first

**After implementing:**
1. ✅ Run `dotnet build` (must have 0 warnings)
2. ✅ Run `dotnet test` (all tests must pass)
3. ✅ Check `docs/checklists/feature_complete.md`
4. ✅ Update roadmap if phase completed
5. ✅ Update `.ai/context.md` with new state

## 🗺️ Roadmaps Overview

| Roadmap | Purpose | Current Status |
|---------|---------|---------------|
| **Combat System** | Battle engine phases (2.1-2.19) | ✅ Core Complete (2.1-2.11) |
| **Content Expansion** | Add more Pokemon, Moves, Items | ⏳ Planned (3.1-3.6) |
| **Unity Integration** | UI and visual integration | ⏳ Planned (4.1-4.8) |
| **Game Features** | Progression, roguelike, meta-game | ⏳ Planned (5.1-5.6) |
| **Testing** | Test coverage and quality | ⏳ Planned (6.1-6.6) |

See `docs/roadmaps/README.md` for detailed information.

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
| **Coding rules** | `docs/project_guidelines.md` |
| **What NOT to do** | `docs/anti-patterns.md` |
| **Before coding** | `docs/checklists/pre_implementation.md` |
| **After coding** | `docs/checklists/feature_complete.md` |
| **Troubleshooting** | `docs/workflow/troubleshooting.md` |
| **Refactoring** | `docs/workflow/refactoring_guide.md` |
| **Test structure** | `docs/testing/test_structure_definition.md` |
| **Integration tests** | `docs/testing/integration_testing_guide.md` |

### Architecture Specs

| System | Document |
|--------|----------|
| Combat System | `docs/architecture/combat_system_spec.md` |
| Action System | `docs/architecture/action_system_spec.md` |
| Damage System | `docs/architecture/damage_and_effect_system.md` |
| Status Effects | `docs/architecture/status_and_stat_system.md` |
| Abilities/Items | `docs/architecture/abilities_items_system.md` |

## 🧪 Testing Standards

**Three-Phase Testing Approach:**
1. **Functional Tests** (`*Tests.cs`) - Core behavior
2. **Edge Cases** (`*EdgeCasesTests.cs`) - Boundaries and invalid inputs
3. **Integration Tests** (`*IntegrationTests.cs`) - System interactions

**Test Organization:**
- `Systems/` - How systems work
- `Blueprints/` - Data structure tests
- `Data/` - Content-specific tests

See `docs/testing/test_structure_definition.md` for details.

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

1. **Check troubleshooting guide**: `docs/workflow/troubleshooting.md`
2. **Review examples**: `docs/examples/good_code.md` and `docs/examples/good_tests.md`
3. **Check architecture docs**: `docs/architecture/`
4. **Review roadmaps**: `docs/roadmaps/README.md`

## 🎉 Ready to Contribute?

1. ✅ Read `.ai/context.md` and `docs/project_guidelines.md`
2. ✅ Pick a feature from a roadmap
3. ✅ Follow `docs/checklists/pre_implementation.md`
4. ✅ Write tests first (TDD)
5. ✅ Implement following the spec
6. ✅ Complete `docs/checklists/feature_complete.md`
7. ✅ Update roadmap and `.ai/context.md`

**Welcome to Pokemon Ultimate! 🎮**

