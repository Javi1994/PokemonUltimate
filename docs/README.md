# Documentation

> Complete documentation for Pokemon Ultimate project.

## Structure

Documentation is organized by **feature/area** for easy navigation:

```
docs/
├── features/              # 📦 Feature-specific documentation
│   ├── 1-game-data/          # Game data structure
│   ├── 2-combat-system/     # Battle engine
│   ├── 3-content-expansion/ # Adding content
│   ├── 4-unity-integration/ # Unity integration
│   └── 5-game-features/     # Game systems
│
├── ai/                    # 🤖 AI-specific documentation
│   ├── guidelines/        # Coding rules and standards
│   ├── prompts/           # AI prompt templates
│   └── [standards]        # Feature standards, master list
│
│
└── [root files]           # Project-wide documentation
```

## Quick Start

**New to the project?** → Start with [`ai/GETTING_STARTED.md`](ai/GETTING_STARTED.md)

**Starting development?** → **MUST read [`features_master_list.md`](features_master_list.md) first** - All work must be assigned to a feature

**Working on a feature?** → Go to [`features/`](features/) and find your feature

**AI assistant?** → Start with [`ai/GETTING_STARTED.md`](ai/GETTING_STARTED.md) and [`features_master_list.md`](features_master_list.md)

**Need shared resources?** → Check [`ai/`](ai/)

## Development Process ⭐ **MANDATORY**

**⚠️ CRITICAL: Feature-Driven Development**

Before starting ANY development work:

1. **Review Existing Features** - Read [`features_master_list.md`](features_master_list.md)
2. **Assign to Feature** - Determine if work fits existing feature or needs new one
3. **Read Feature Documentation** - If existing feature, read its complete documentation
4. **Create Feature Documentation** - If new feature, create complete documentation structure
5. **Proceed with Development** - Follow standard workflow after feature assignment

**After completing work:**

- Update feature's `roadmap.md` - Mark completed phases
- Update feature's `architecture.md` - Reflect actual implementation
- Update feature's `use_cases.md` - Mark completed cases
- Update feature's `code_location.md` - Add new files
- Update feature's `testing.md` - Document tests
- Update `features_master_list.md` - Update status

**See**: [`ai/guidelines/feature_driven_development.md`](ai/guidelines/feature_driven_development.md) for complete process

## Features

See [`features/README.md`](features/README.md) for complete feature list.

### Core Features
- **[Game Data](features/1-game-data/)** - Complete game data structure
- **[Combat System](features/2-combat-system/)** - Battle engine
- **[Content Expansion](features/3-content-expansion/)** - Adding Pokemon, Moves, Items

### Integration & Infrastructure
- **[Unity Integration](features/4-unity-integration/)** - Unity UI and integration
- **[Game Features](features/5-game-features/)** - Progression, roguelike systems

**Testing**: Each feature has its own `testing.md` file. Shared testing strategy: [`ai/testing_structure_definition.md`](ai/testing_structure_definition.md)

## Shared Resources

- **[Testing Strategy](ai/testing_structure_definition.md)** - Test structure standard

**AI Resources** (workflow, checklists, examples, prompts): See [`ai/`](ai/)

## AI Documentation

| Document | Purpose |
|----------|---------|
| [`ai/GETTING_STARTED.md`](ai/GETTING_STARTED.md) | Quick start guide |
| [`features_master_list.md`](features_master_list.md) | Feature numbering and naming ⭐ **START HERE** |
| [`ai/guidelines/feature_driven_development.md`](ai/guidelines/feature_driven_development.md) | Feature-driven development process ⭐ **MANDATORY** |
| [`ai/guidelines/project_guidelines.md`](ai/guidelines/project_guidelines.md) | Coding rules and standards |
| [`ai/anti-patterns.md`](ai/anti-patterns.md) | What NOT to do |
| [`implementation_plan.md`](implementation_plan.md) | Overall technical roadmap |
| [`feature_documentation_standard.md`](feature_documentation_standard.md) | Documentation structure standard |

## Roadmaps

Roadmaps are now organized within each feature. See:
- [`features/1-game-data/roadmap.md`](features/1-game-data/roadmap.md)
- [`features/2-combat-system/roadmap.md`](features/2-combat-system/roadmap.md)
- [`features/3-content-expansion/roadmap.md`](features/3-content-expansion/roadmap.md)
- [`features/4-unity-integration/roadmap.md`](features/4-unity-integration/roadmap.md)
- [`features/5-game-features/roadmap.md`](features/5-game-features/roadmap.md)

## Navigation Tips

1. **Feature documentation** → Go to `features/[feature-name]/`
2. **AI documentation** → Go to `ai/` (standards, guidelines, prompts)
3. **Shared technical docs** → Go to `shared/` (architecture, workflow, examples)
4. **Quick start** → Read `ai/GETTING_STARTED.md`
5. **Coding rules** → Read `ai/guidelines/project_guidelines.md`

---

**Last Updated**: 2025-01-XX

