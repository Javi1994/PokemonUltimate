# Features Master List

> **Master reference for all project features with standardized naming and numbering.**

This document serves as the single source of truth for all features in the Pokemon Ultimate project. All documentation should reference features using these numbers and names.

**⚠️ MANDATORY: Feature-Driven Development** - Before starting ANY development work, review this document to determine if work fits an existing feature or needs a new one. See [`ai_workflow/docs/FDD_GUIDE.md`](../../ai_workflow/docs/FDD_GUIDE.md) for complete process.

**⚠️ MANDATORY: Feature Naming in Code** - All public classes MUST include feature references in XML documentation comments. See feature documentation for guidelines.

---

## Feature Numbering System

Features are numbered sequentially (1, 2, 3...) and sub-features use decimal notation (1.1, 1.2, 2.1, 2.2...).

**Format**: `[Feature Number]: [Feature Name]`

-   **Feature Number**: Unique identifier (1, 2, 3...)
-   **Feature Name**: Standardized name (used in folder names and references)

---

## Core Features

### Feature 1: Game Data

**Folder**: `docs/features/1-game-data/`  
**Status**: ✅ Core Complete  
**Description**: All game data structures (blueprints) and supporting systems for Pokemon, Moves, Abilities, Items, Field Conditions, and infrastructure

> **📋 Refactoring Completed (2024-12-XX)**: Comprehensive refactoring completed following SOLID principles. All major components now use Dependency Injection, Strategy Pattern, Constants centralization, Extension methods, Validation, Move selection, Stat stage management, and Stats caching. See `PokemonUltimate.Core/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for details. Phases 0-8 completed (21/22 tasks, 95.5%).

**Sub-Features** (Organized by Groups):

#### Grupo A: Core Entity Data (Entidades Principales)

-   **1.1**: Pokemon Data - PokemonSpeciesData (Blueprint), PokemonInstance (Runtime), BaseStats, LearnableMove
-   **1.2**: Move Data - MoveData (Blueprint), MoveInstance (Runtime), Move Effects (22 implementations)
-   **1.3**: Ability Data - AbilityData (Blueprint)
-   **1.4**: Item Data - ItemData (Blueprint)

#### Grupo B: Field & Status Data (Condiciones de Campo)

-   **1.5**: Status Effect Data - StatusEffectData (Blueprint)
-   **1.6**: Weather Data - WeatherData (Blueprint)
-   **1.7**: Terrain Data - TerrainData (Blueprint)
-   **1.8**: Hazard Data - HazardData (Blueprint)
-   **1.9**: Side Condition Data - SideConditionData (Blueprint)
-   **1.10**: Field Effect Data - FieldEffectData (Blueprint)

#### Grupo C: Supporting Systems (Sistemas de Soporte)

-   **1.11**: Evolution System - Evolution, IEvolutionCondition, EvolutionConditions (6 classes)
-   **1.12**: Type Effectiveness Table - ITypeEffectiveness/TypeEffectiveness (instance-based with interface, post-refactor)

#### Grupo D: Infrastructure (Infraestructura)

-   **1.13**: Interfaces Base - IIdentifiable
-   **1.14**: Enums & Constants - Enums (20 main + 7 in Effects), ErrorMessages, GameMessages, CoreConstants, CoreValidators, Extensions, NatureData (post-refactor)
-   ⚠️ **Builders**: Moved to Feature 3.9 (see Feature 3 below)
-   **1.15**: Factories & Calculators - IStatCalculator/StatCalculator, ITypeEffectiveness/TypeEffectiveness, PokemonFactory, PokemonInstanceBuilder, IMoveSelector/MoveSelector (post-refactor)
-   **1.16**: Registry System - IDataRegistry<T>, GameDataRegistry<T>, PokemonRegistry, MoveRegistry

#### Grupo E: Planned Features

-   **1.18**: Variants System - Mega/Dinamax/Terracristalización as separate species (Planned)
-   **1.19**: Pokedex Fields - Description, Category, Height, Weight, Color, Shape, Habitat (Planned)

**Related Roadmap**: `docs/features/1-game-data/roadmap.md`

---

### Feature 2: Combat System

**Folder**: `docs/features/2-combat-system/`  
**Status**: ✅ Core Complete (Phases 2.1-2.11, 2.12-2.14, 2.16)  
**Description**: Complete Pokemon battle engine implementation

> **📋 Refactoring Completed (2024-12-05)**: Comprehensive refactoring completed following SOLID principles. All major components now use Dependency Injection, Value Objects, Strategy Pattern, Factory Pattern, Event System, Logging, and Validation. See `PokemonUltimate.Combat/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for details.

**Sub-Features**:

-   **2.1**: Battle Foundation - BattleField, Slots, Sides, Rules
-   **2.2**: Action Queue System - BattleQueue, BattleAction
-   **2.3**: Turn Order Resolution - Priority, Speed, Random sorting
-   **2.4**: Damage Calculation Pipeline - Modular damage calculation (includes stat/damage modifiers)
-   **2.5**: Combat Actions - UseMove, Damage, Status, Heal, Switch, Faint
-   **2.6**: Combat Engine - Battle loop, turn execution, outcome detection
-   **2.7**: Integration - AI providers, Player input, Full battles
-   **2.8**: End-of-Turn Effects - Status damage, effects processing
-   **2.9**: Abilities & Items - Event-driven system, triggers
-   **2.11**: Recoil & Drain - Recoil damage, HP drain effects
-   **2.12**: Weather System - Weather conditions and effects ✅ Core Complete (advanced features pending dependent systems)
-   **2.13**: Terrain System - Terrain conditions and effects ✅ Core Complete (advanced features pending dependent systems)
-   **2.14**: Hazards System - Stealth Rock, Spikes, etc. ✅ Core Complete (hazard removal actions pending move-specific implementation)
-   **2.15**: Advanced Move Mechanics - Multi-hit, charging moves (Planned)
-   **2.16**: Field Conditions - Screens, Tailwind, protections ✅ Core Complete
-   **2.17**: Advanced Abilities - Complex ability interactions (Planned)
-   **2.18**: Advanced Items - Complex item interactions (Planned)
-   **2.19**: Battle Formats - Doubles, Triples, Rotation (Planned)

**Related Roadmap**: `docs/features/2-combat-system/roadmap.md`

---

### Feature 3: Content Expansion

**Folder**: `docs/features/3-content-expansion/`  
**Status**: 🎯 In Progress  
**Description**: Adding all game content: Pokemon, Moves, Items, Abilities, Status Effects, Field Conditions, and supporting catalogs

**Sub-Features**:

-   **3.1**: Pokemon Expansion - Adding more Pokemon species (26/151 Gen 1)
-   **3.2**: Move Expansion - Adding more moves across all types (36 moves, 12 types)
-   **3.3**: Item Expansion - Adding more held items and consumables (23 items)
-   **3.4**: Ability Expansion - Adding more abilities (35 abilities)
-   **3.5**: Status Effect Expansion - Status effects catalog (15 statuses) ✅ Complete
-   **3.6**: Field Conditions Expansion - Weather, Terrain, Hazards, Side Conditions, Field Effects (35 total) ✅ Complete
-   **3.7**: Content Validation - Quality standards and validation
-   **3.8**: Content Organization - Catalog organization and maintenance ✅ Complete
-   **3.9**: Builders - Fluent builder APIs for creating game content ✅ Complete

**Related Roadmap**: `docs/features/3-content-expansion/roadmap.md`

---

## Integration Features

### Feature 4: Unity Integration

**Folder**: `docs/features/4-unity-integration/`  
**Status**: ⏳ Planned  
**Description**: Integrating the battle engine with Unity for visuals, input, and audio

**Sub-Features**:

-   **4.1**: Unity Project Setup - DLL integration, project structure
-   **4.2**: UI Foundation - HP bars, Pokemon display, dialog system
-   **4.3**: IBattleView Implementation - Connecting engine to Unity UI
-   **4.4**: Player Input System - Action selection, move selection, switching
-   **4.5**: Animations System - Move animations, visual effects
-   **4.6**: Audio System - Sound effects, battle music
-   **4.7**: Post-Battle UI - Results, rewards, level ups display
-   **4.8**: Transitions - Battle start/end transitions, scene management

**Related Roadmap**: `docs/features/4-unity-integration/roadmap.md`

---

## Game Features

### Feature 5: Game Features

**Folder**: `docs/features/5-game-features/`  
**Status**: ⏳ Planned  
**Description**: Game systems beyond combat (progression, roguelike, meta-game)

**Sub-Features**:

-   **5.1**: Post-Battle Rewards - EXP calculation, level ups, rewards
-   **5.2**: Pokemon Management - Party, PC, catching system
-   **5.3**: Encounter System - Wild, trainer, boss battles
-   **5.4**: Inventory System - Item management and usage
-   **5.5**: Save System - Save/load game progress
-   **5.6**: Progression System - Roguelike runs, meta-progression

**Related Roadmap**: `docs/features/5-game-features/roadmap.md`

---

## Feature Reference Table

| #     | Feature Name      | Folder                 | Status           | Roadmap                                                  |
| ----- | ----------------- | ---------------------- | ---------------- | -------------------------------------------------------- |
| **1** | Game Data         | `1-game-data/`         | ✅ Core Complete | [roadmap.md](../features/1-game-data/roadmap.md)         |
| **2** | Combat System     | `2-combat-system/`     | ✅ Core Complete | [roadmap.md](../features/2-combat-system/roadmap.md)     |
| **3** | Content Expansion | `3-content-expansion/` | 🎯 In Progress   | [roadmap.md](../features/3-content-expansion/roadmap.md) |
| **4** | Unity Integration | `4-unity-integration/` | ⏳ Planned       | [roadmap.md](../features/4-unity-integration/roadmap.md) |
| **5** | Game Features     | `5-game-features/`     | ⏳ Planned       | [roadmap.md](../features/5-game-features/roadmap.md)     |

---

## Usage Guidelines

### Referencing Features in Documentation

When referencing features in documentation, use the format:

**Format**: `Feature [Number]: [Feature Name]` or `[Number]: [Feature Name]`

**Examples**:

-   "Feature 2: Combat System" or "2: Combat System"
-   "Sub-feature 2.4: Damage Calculation Pipeline"
-   "See Feature 2 roadmap for implementation details"

### In Roadmaps

Roadmaps should reference features using their numbers:

-   "Depends on: Feature 1: Game Data"
-   "Related to: Feature 2.4: Damage Calculation Pipeline"
-   "Integrates with: Feature 2: Combat System"

### In Architecture Documents

Architecture documents should reference related features:

-   "Integrates with Feature 2: Combat System"
-   "Uses Feature 1.1: PokemonSpeciesData (Blueprint)"
-   "Provides data for Feature 2: Combat System"

### In Use Cases

Use cases should reference feature numbers:

-   "UC-001: Feature 1: Game Data - Define Pokemon Species"
-   "INT-001: Feature 1 ↔ Feature 2 - Game Data → Combat System"
-   "EC-001: Feature 2.4: Damage Calculation Pipeline - Zero Power Move"

---

## Feature Status Legend

-   ✅ **Complete** - Feature fully implemented and tested
-   ✅ **Core Complete** - Core phases complete, extended phases planned (used for Feature 2)
-   🎯 **In Progress** - Feature currently being implemented
-   ⏳ **Planned** - Feature planned but not started

---

## Testing Documentation

**Testing is NOT a feature** - Each feature has its own `testing.md` file documenting its testing strategy.

**Feature Testing Documents** (each feature has its own):

-   `docs/features/1-game-data/testing.md` - Game data testing strategy
-   `docs/features/2-combat-system/testing.md` - Combat system testing strategy
-   `docs/features/3-content-expansion/testing.md` - Content expansion testing strategy
-   `docs/features/4-unity-integration/testing.md` - Unity integration testing strategy
-   `docs/features/5-game-features/testing.md` - Game features testing strategy

**Additional Testing Resources**:

-   `docs/features/2-combat-system/testing/integration_guide.md` - Integration test patterns
-   `ai_workflow/docs/TDD_GUIDE.md` - Test-Driven Development guide

---

## 📍 Locating Feature Documentation

### Quick Reference

**Feature Documentation Location**: `docs/features/[N]-[feature-name]/`

**Sub-Feature Documentation Location**: `docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/`

### Finding Documentation

1. **By Feature Number**: Check this document (`features_master_list.md`) for the feature number
2. **Navigate**: Go to `docs/features/[N]-[feature-name]/`
3. **Start Here**: Read `README.md` for overview and links to all documents
4. **Specific Info**: Use the Documentation table in `README.md` to find:
    - Technical specs → `architecture.md`
    - Scenarios → `use_cases.md`
    - Implementation plan → `roadmap.md`
    - Testing strategy → `testing.md`
    - Code location → `code_location.md`

### Examples

-   **Feature 2**: `docs/features/2-combat-system/README.md`
-   **Sub-feature 2.5**: `docs/features/2-combat-system/2.5-combat-actions/README.md`
-   **Sub-feature 3.1**: `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`

**⚠️ Always use numbered paths** - Never use unnumbered paths like `pokemon-data/` or `combat-system/`

---

## ➕ Creating New Features

**⚠️ CRITICAL: Before creating a new feature, review existing features in this document to ensure work doesn't fit an existing feature.**

### Process

1. **Review Existing Features** ⭐ **MUST DO FIRST**

    - Read this document (`features_master_list.md`) completely
    - Check if work fits any existing feature or sub-feature
    - If fits existing → Use that feature (create sub-feature if needed)
    - If doesn't fit → Proceed with creating new feature

2. **Determine Number**: Check this document for next available number (currently: 5)
3. **Choose Name**: Use kebab-case, check for duplicates
4. **Create Folder**: `docs/features/[N]-[feature-name]/`
5. **Create Complete Documentation Structure**: Follow standard structure (README.md, architecture.md, roadmap.md, use_cases.md, testing.md, code_location.md)
    - All 6 required documents: README.md, architecture.md, roadmap.md, use_cases.md, testing.md, code_location.md
6. **Update This Document**: Add feature entry with number, name, folder, status, description
7. **Update Features Overview**: Add to `docs/features/README.md`
8. **Use Numbered Format**: Always reference as `Feature [N]: [Name]` or `[N]: [Name]`

**See**: [`ai_workflow/docs/FDD_GUIDE.md`](../../ai_workflow/docs/FDD_GUIDE.md) for complete process.

### Required Information

When adding a new feature, provide:

-   **Feature Number**: Next sequential number
-   **Feature Name**: Title Case display name
-   **Folder**: `[N]-[kebab-case-name]`
-   **Status**: ⏳ Planned / 🎯 In Progress / ✅ Complete
-   **Description**: One-line description
-   **Sub-Features**: List if applicable (numbered as `[N].[M]`)

### Example Entry

```markdown
### Feature 6: New Feature

**Folder**: `docs/features/6-new-feature/`  
**Status**: ⏳ Planned  
**Description**: Brief description of what this feature does

**Sub-Features**:

-   **6.1**: First Sub-Feature - Description
-   **6.2**: Second Sub-Feature - Description
```

---

## ➕ Creating New Sub-Features

### Process

1. **Review Parent Feature's Sub-Features** ⭐ **MUST DO FIRST**

    - Read parent feature's documentation
    - Check if work fits any existing sub-feature
    - If fits existing → Use that sub-feature
    - If doesn't fit → Proceed with creating new sub-feature

2. **Determine Number**: Check parent feature's sub-features for next number
3. **Choose Name**: Use kebab-case, descriptive name
4. **Create Folder**: `docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/`
5. **Create Documents**: Minimum `README.md`, add `architecture.md` if complex sub-feature
6. **Update This Document**: Add sub-feature entry under parent feature
7. **Update Parent README**: Add sub-feature link to parent's `README.md`
8. **Update Parent Documentation**: Update parent's `roadmap.md` and `code_location.md` if needed
9. **Use Numbered Format**: Always reference as `Sub-Feature [N.M]: [Name]` or `[N.M]: [Name]`

**See**: [`ai_workflow/docs/FDD_GUIDE.md`](../../ai_workflow/docs/FDD_GUIDE.md) for complete process.

### Required Information

When adding a new sub-feature, provide:

-   **Sub-Feature Number**: `[N].[M]` format
-   **Sub-Feature Name**: Title Case display name
-   **Folder**: `[N.M]-[kebab-case-name]`
-   **Description**: Brief description

### Example Entry

```markdown
**Sub-Features**:

-   **[N.M]**: Sub-Feature Name - Description
```

---

## 📝 Naming Convention Rules

### ⚠️ CRITICAL: Always Use Numbered Format

**In Documentation**:

-   ✅ **Correct**: "Feature 2: Combat System" or "2: Combat System"
-   ❌ **Wrong**: "Combat System" (without number)

**In File Paths**:

-   ✅ **Correct**: `docs/features/2-combat-system/`
-   ❌ **Wrong**: `docs/features/combat-system/`

**In References**:

-   ✅ **Correct**: `[Feature 2: Combat System](../2-combat-system/)`
-   ❌ **Wrong**: `[Combat System](../combat-system/)`

**In Code Comments**:

-   ✅ **Correct**: `// See Feature 2: Combat System architecture`
-   ❌ **Wrong**: `// See Combat System architecture`

### Folder Naming

-   **Features**: `[N]-[kebab-case-name]` (e.g., `1-game-data`)
-   **Sub-Features**: `[N.M]-[kebab-case-name]` (e.g., `2.5-combat-actions`)
-   **Always use kebab-case**: lowercase with hyphens
-   **Always include number**: Never create unnumbered folders

### Display Naming

-   **Features**: `Feature [N]: [Feature Name]` or `[N]: [Feature Name]`
-   **Sub-Features**: `Sub-Feature [N.M]: [Sub-Feature Name]` or `[N.M]: [Sub-Feature Name]`
-   **Use Title Case**: Capitalize first letter of each word

## Related Documents

-   **[Features Overview](../features/README.md)** - Overview of all features
-   **[Feature-Driven Development](../../ai_workflow/docs/FDD_GUIDE.md)** ⭐ **MANDATORY** - Complete process for feature-driven development
-   **[TDD Guide](../../ai_workflow/docs/TDD_GUIDE.md)** - Test-Driven Development guide

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)  
**Maintained By**: Project maintainers
