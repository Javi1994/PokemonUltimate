# Documentation

> Complete documentation for Pokemon Ultimate project.

## Structure

Documentation is organized by **feature/area** for easy navigation:

```
docs/
├── features/              # 📦 Feature-specific documentation
│   ├── 1-pokemon-data/      # Pokemon data structure
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

**New to the project?** → Start with [`GETTING_STARTED.md`](GETTING_STARTED.md)

**Working on a feature?** → Go to [`features/`](features/) and find your feature

**AI assistant?** → Start with [`ai/GETTING_STARTED.md`](ai/GETTING_STARTED.md) and [`features_master_list.md`](features_master_list.md)

**Need shared resources?** → Check [`shared/`](shared/)

## Features

See [`features/README.md`](features/README.md) for complete feature list.

### Core Features
- **[Pokemon Data](features/1-pokemon-data/)** - Complete Pokemon data structure
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
| [`features_master_list.md`](features_master_list.md) | Feature numbering and naming |
| [`ai/guidelines/project_guidelines.md`](ai/guidelines/project_guidelines.md) | Coding rules and standards |
| [`ai/anti-patterns.md`](ai/anti-patterns.md) | What NOT to do |
| [`implementation_plan.md`](implementation_plan.md) | Overall technical roadmap |
| [`feature_documentation_standard.md`](feature_documentation_standard.md) | Documentation structure standard |

## Roadmaps

Roadmaps are now organized within each feature. See:
- [`features/1-pokemon-data/roadmap.md`](features/1-pokemon-data/roadmap.md)
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

