# Features Documentation

> Documentation organized by feature/area.

## Overview

This directory contains all feature-specific documentation, organized by feature/area. Each feature has its own folder with:
- **README.md** - Feature overview and quick links
- **roadmap.md** - Implementation plan
- **architecture.md** - Technical specification
- **use_cases.md** - All scenarios and behaviors
- **testing.md** - Testing strategy
- **code_location.md** - Where the code lives
- **Sub-features/** - Related sub-features

**📋 Master Reference**: See [`../features_master_list.md`](../features_master_list.md) for complete feature numbering and naming standards.

## Features

### 🎮 Core Features

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| **1** | **[Pokemon Data](1-pokemon-data/)** | ⏳ Planned | Complete Pokemon data structure |
| **2** | **[Combat System](2-combat-system/)** | ✅ Core Complete | Battle engine (phases 2.1-2.11) |
| **3** | **[Content Expansion](3-content-expansion/)** | 🎯 In Progress | Adding Pokemon, Moves, Items |

### 🔧 Integration & Infrastructure

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| **4** | **[Unity Integration](4-unity-integration/)** | ⏳ Planned | Unity UI and integration |
| **5** | **[Game Features](5-game-features/)** | ⏳ Planned | Progression, roguelike, meta-game |

## Structure

Each feature follows this structure:

```
feature-name/
├── README.md              # Overview and quick links
├── roadmap.md             # Implementation plan (phases numbered)
├── architecture.md        # Technical specification
├── use_cases.md           # All scenarios and behaviors
├── testing.md             # Testing strategy
├── code_location.md       # Where the code lives
└── sub-features/          # Related sub-features
```

**Feature Numbering**: All features are numbered (1, 2, 3, 4, 5) and sub-features use decimal notation (1.1, 1.2, 2.1, 2.2...). See [`../features_master_list.md`](../features_master_list.md) for complete reference.

**Testing**: Each feature has its own `testing.md` file. Shared testing strategy: [`../ai/testing_structure_definition.md`](../ai/testing_structure_definition.md)

## Shared Documentation

Documentation shared across features:
- **Testing strategy**: [`../ai/testing_structure_definition.md`](../ai/testing_structure_definition.md) - Test structure standard
- **AI resources**: [`../ai/`](../ai/) - Workflow, checklists, examples, prompts, guidelines

## Quick Navigation

- **Starting a new feature?** → Read feature's README.md
- **Implementing?** → Check roadmap.md and architecture.md
- **Testing?** → See testing.md (each feature has its own)
- **Need shared resources?** → See [`../ai/`](../ai/)

---

**Last Updated**: 2025-01-XX

