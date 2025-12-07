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
| **1** | **[Game Data](1-game-data/)** | ✅ Core Complete | Complete game data structure |
| **2** | **[Combat System](2-combat-system/)** | ✅ Core Complete | Battle engine (phases 2.1-2.11) |
| **3** | **[Content Expansion](3-content-expansion/)** | 🎯 In Progress | Adding Pokemon, Moves, Items |

### 🔧 Integration & Infrastructure

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| **4** | **[Unity Integration](4-unity-integration/)** | ⏳ Planned | Unity UI and integration |
| **5** | **[Game Features](5-game-features/)** | ⏳ Planned | Progression, roguelike, meta-game |

### 🛠️ Development Tools

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| **6** | **[Development Tools](6-development-tools/)** | 🎯 In Progress | Windows Forms debugger applications |

### 🎮 Game Implementation

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| **7** | **[Game Implementation](7-game-implementation/)** | ⏳ Planned | Text-based demo game to test all features before Unity |

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

**Feature Numbering**: All features are numbered (1, 2, 3, 4, 5, 6, 7) and sub-features use decimal notation (1.1, 1.2, 2.1, 2.2...). See [`../features_master_list.md`](../features_master_list.md) for complete reference.

**Testing**: Each feature has its own `testing.md` file.

## Development Process ⭐ **MANDATORY**

**⚠️ CRITICAL: Feature-Driven Development**

Before starting ANY development work:

1. **Review Existing Features** - Read [`../features_master_list.md`](../features_master_list.md) ⭐ **START HERE**
2. **Assign to Feature** - Determine if work fits existing feature or needs new one
3. **Read Feature Documentation** - If existing feature, read its complete documentation
4. **Create Feature Documentation** - If new feature, create complete documentation structure (README.md, architecture.md, roadmap.md, use_cases.md, testing.md, code_location.md)
5. **Proceed with Development** - Follow TDD workflow from `ai_workflow/`

**After completing work:**

- Update feature's `roadmap.md` - Mark completed phases
- Update feature's `architecture.md` - Reflect actual implementation
- Update feature's `use_cases.md` - Mark completed cases
- Update feature's `code_location.md` - Add new files
- Update feature's `testing.md` - Document tests
- Update `features_master_list.md` - Update status

**See**: [`../ai_workflow/docs/FDD_GUIDE.md`](../../ai_workflow/docs/FDD_GUIDE.md) for complete Feature-Driven Development process

## Quick Navigation

- **Starting development?** → Read [`../features_master_list.md`](../features_master_list.md) first ⭐
- **Starting a new feature?** → Read feature's README.md
- **Implementing?** → Check roadmap.md and architecture.md
- **Testing?** → See testing.md (each feature has its own)
- **Workflow system?** → See [`../ai_workflow/`](../../ai_workflow/)

---

**Last Updated**: 2025-01-XX
