# Documentation

> Complete documentation for Pokemon Ultimate project.

## 🚀 AI Workflow System

**⭐ IMPORTANT: This project uses the AI Workflow system from `ai_workflow/`**

The `ai_workflow/` directory contains the complete workflow system optimized for AI-assisted development. This includes:

- **Game Definition** - Define the game and generate features automatically
- **TDD Workflow** - Test-Driven Development with decision trees
- **FDD Workflow** - Feature-Driven Development with automatic assignment
- **Validation Scripts** - Automatic compliance checking
- **Templates** - Complete templates for tests, features, and documentation

**Start Here**: Read [`ai_workflow/START_HERE.md`](../ai_workflow/START_HERE.md) for complete setup and workflow.

**Quick Reference**: See [`ai_workflow/AI_QUICK_REFERENCE.md`](../ai_workflow/AI_QUICK_REFERENCE.md) for AI execution patterns.

## Structure

```
docs/
├── features/                      # 📦 Feature-specific documentation
│   ├── 1-game-data/              # Game data structure
│   ├── 2-combat-system/          # Battle engine
│   ├── 3-content-expansion/      # Adding content
│   ├── 4-unity-integration/       # Unity integration
│   └── 5-game-features/           # Game systems
│
├── features_master_list.md        # ⭐ Master reference for all features
├── features_master_list_detailed.md  # Detailed list with all sub-features
├── GAME_DEFINITION.yaml          # Game definition
└── README.md                      # This file
```

## Quick Start

**New to the project?** → Start with [`ai_workflow/START_HERE.md`](../ai_workflow/START_HERE.md)

**Starting development?** → **MUST read [`features_master_list.md`](features_master_list.md) first** - All work must be assigned to a feature

**Working on a feature?** → Go to [`features/`](features/) and find your feature

**AI assistant?** → Start with [`ai_workflow/AI_QUICK_REFERENCE.md`](../ai_workflow/AI_QUICK_REFERENCE.md) and [`features_master_list.md`](features_master_list.md)

## Development Process ⭐ **MANDATORY**

**⚠️ CRITICAL: Feature-Driven Development**

Before starting ANY development work:

1. **Review Existing Features** - Read [`features_master_list.md`](features_master_list.md)
2. **Assign to Feature** - Determine if work fits existing feature or needs new one
3. **Read Feature Documentation** - If existing feature, read its complete documentation
4. **Create Feature Documentation** - If new feature, create complete documentation structure
5. **Proceed with Development** - Follow TDD workflow from `ai_workflow/`

**After completing work:**

- Update feature's `roadmap.md` - Mark completed phases
- Update feature's `architecture.md` - Reflect actual implementation
- Update feature's `use_cases.md` - Mark completed cases
- Update feature's `code_location.md` - Add new files
- Update feature's `testing.md` - Document tests
- Update `features_master_list.md` - Update status

**See**: [`ai_workflow/docs/FDD_GUIDE.md`](../ai_workflow/docs/FDD_GUIDE.md) for complete Feature-Driven Development process

## Features

See [`features/README.md`](features/README.md) for complete feature list.

### Core Features
- **[Game Data](features/1-game-data/)** - Complete game data structure
- **[Combat System](features/2-combat-system/)** - Battle engine
- **[Content Expansion](features/3-content-expansion/)** - Adding Pokemon, Moves, Items

### Integration & Infrastructure
- **[Unity Integration](features/4-unity-integration/)** - Unity UI and integration
- **[Game Features](features/5-game-features/)** - Progression, roguelike systems

**Testing**: Each feature has its own `testing.md` file. See feature documentation for testing strategy.

## AI Workflow Documentation

**Main Workflow System**: [`ai_workflow/`](../ai_workflow/) - Complete AI workflow system

**Key Documents**:
- [`ai_workflow/START_HERE.md`](../ai_workflow/START_HERE.md) ⭐ **For new projects - Read first**
- [`ai_workflow/AI_QUICK_REFERENCE.md`](../ai_workflow/AI_QUICK_REFERENCE.md) ⭐ **For AI assistants - Quick reference**
- [`ai_workflow/docs/FDD_GUIDE.md`](../ai_workflow/docs/FDD_GUIDE.md) - Feature-Driven Development
- [`ai_workflow/docs/TDD_GUIDE.md`](../ai_workflow/docs/TDD_GUIDE.md) - Test-Driven Development
- [`ai_workflow/docs/GAME_DEFINITION_GUIDE.md`](../ai_workflow/docs/GAME_DEFINITION_GUIDE.md) - Game Definition workflow

## Key Documents Reference

| Need | Document |
|------|----------|
| **Project state** | `.ai/context.md` |
| **Feature numbers/names** | `features_master_list.md` ⭐ **MASTER REFERENCE** |
| **Workflow system** | `ai_workflow/` ⭐ **COMPLETE WORKFLOW** |

## Navigation Tips

1. **Feature documentation** → Go to `features/[N]-[feature-name]/`
2. **AI workflow** → Go to `ai_workflow/` (complete workflow system)
3. **Quick start** → Read `ai_workflow/START_HERE.md`
4. **Feature reference** → Read `features_master_list.md`

---

**Last Updated**: 2025-01-XX
