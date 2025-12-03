# Features Master List

> **Master reference for all project features with standardized naming and numbering.**

This document serves as the single source of truth for all features in the Pokemon Ultimate project. All documentation should reference features using these numbers and names.

**⚠️ MANDATORY: Feature Naming in Code** - All public classes MUST include feature references in XML documentation comments. See [`docs/ai/guidelines/feature_naming_in_code.md`](ai/guidelines/feature_naming_in_code.md) for complete guidelines.

---

## Feature Numbering System

Features are numbered sequentially (1, 2, 3...) and sub-features use decimal notation (1.1, 1.2, 2.1, 2.2...).

**Format**: `[Feature Number]: [Feature Name]`
- **Feature Number**: Unique identifier (1, 2, 3...)
- **Feature Name**: Standardized name (used in folder names and references)

---

## Core Features

### Feature 1: Game Data
**Folder**: `docs/features/1-game-data/`  
**Status**: ✅ Core Complete  
**Description**: All game data structures (blueprints) and supporting systems for Pokemon, Moves, Abilities, Items, Field Conditions, and infrastructure

**Sub-Features** (Organized by Groups):

#### Grupo A: Core Entity Data (Entidades Principales)
- **1.1**: Pokemon Data - PokemonSpeciesData (Blueprint), PokemonInstance (Runtime), BaseStats, LearnableMove
- **1.2**: Move Data - MoveData (Blueprint), MoveInstance (Runtime), Move Effects (22 implementations)
- **1.3**: Ability Data - AbilityData (Blueprint)
- **1.4**: Item Data - ItemData (Blueprint)

#### Grupo B: Field & Status Data (Condiciones de Campo)
- **1.5**: Status Effect Data - StatusEffectData (Blueprint)
- **1.6**: Field Conditions Data - WeatherData, TerrainData, HazardData, SideConditionData, FieldEffectData

#### Grupo C: Supporting Systems (Sistemas de Soporte)
- **1.7**: Evolution System - Evolution, IEvolutionCondition, EvolutionConditions (6 classes)
- **1.8**: Type Effectiveness Table - TypeEffectiveness (data table)

#### Grupo D: Infrastructure (Infraestructura)
- **1.9**: Interfaces Base - IIdentifiable
- **1.10**: Enums & Constants - Enums (20 main + 7 in Effects), ErrorMessages, GameMessages
- **1.11**: Builders - 13 builder classes + 10 static helper classes
- **1.12**: Factories & Calculators - StatCalculator, PokemonFactory, PokemonInstanceBuilder, NatureData
- **1.13**: Registry System - IDataRegistry<T>, GameDataRegistry<T>, PokemonRegistry, MoveRegistry

#### Grupo E: Planned Features
- **1.14**: Variants System - Mega/Dinamax/Terracristalización as separate species (Planned)
- **1.15**: Pokedex Fields - Description, Category, Height, Weight, Color, Shape, Habitat (Planned)

**Related Roadmap**: `docs/features/1-game-data/roadmap.md`

---

### Feature 2: Combat System
**Folder**: `docs/features/2-combat-system/`  
**Status**: ✅ Core Complete (Phases 2.1-2.11)  
**Description**: Complete Pokemon battle engine implementation

**Sub-Features**:
- **2.1**: Battle Foundation - BattleField, Slots, Sides, Rules
- **2.2**: Action Queue System - BattleQueue, BattleAction
- **2.3**: Turn Order Resolution - Priority, Speed, Random sorting
- **2.4**: Damage Calculation Pipeline - Modular damage calculation
- **2.5**: Combat Actions - UseMove, Damage, Status, Heal, Switch, Faint
- **2.6**: Combat Engine - Battle loop, turn execution, outcome detection
- **2.7**: Integration - AI providers, Player input, Full battles
- **2.8**: End-of-Turn Effects - Status damage, effects processing
- **2.9**: Abilities & Items - Event-driven system, triggers
- **2.10**: Pipeline Hooks - Stat modifiers, damage modifiers
- **2.11**: Recoil & Drain - Recoil damage, HP drain effects
- **2.12**: Weather System - Weather conditions and effects (Planned)
- **2.13**: Terrain System - Terrain conditions and effects (Planned)
- **2.14**: Hazards System - Stealth Rock, Spikes, etc. (Planned)
- **2.15**: Advanced Move Mechanics - Multi-hit, charging moves (Planned)
- **2.16**: Field Conditions - Screens, Tailwind, protections (Planned)
- **2.17**: Advanced Abilities - Complex ability interactions (Planned)
- **2.18**: Advanced Items - Complex item interactions (Planned)
- **2.19**: Battle Formats - Doubles, Triples, Rotation (Planned)

**Related Roadmap**: `docs/features/2-combat-system/roadmap.md`

---

### Feature 3: Content Expansion
**Folder**: `docs/features/3-content-expansion/`  
**Status**: 🎯 In Progress (26 Pokemon, 36 Moves)  
**Description**: Adding Pokemon, Moves, Items, and Abilities to the game

**Sub-Features**:
- **3.1**: Pokemon Expansion - Adding more Pokemon species
- **3.2**: Move Expansion - Adding more moves across all types
- **3.3**: Item Expansion - Adding more held items and consumables
- **3.4**: Ability Expansion - Adding more abilities
- **3.5**: Content Validation - Quality standards and validation
- **3.6**: Content Organization - Catalog organization and maintenance

**Related Roadmap**: `docs/features/3-content-expansion/roadmap.md`

---

## Integration Features

### Feature 4: Unity Integration
**Folder**: `docs/features/4-unity-integration/`  
**Status**: ⏳ Planned  
**Description**: Integrating the battle engine with Unity for visuals, input, and audio

**Sub-Features**:
- **4.1**: Unity Project Setup - DLL integration, project structure
- **4.2**: UI Foundation - HP bars, Pokemon display, dialog system
- **4.3**: IBattleView Implementation - Connecting engine to Unity UI
- **4.4**: Player Input System - Action selection, move selection, switching
- **4.5**: Animations System - Move animations, visual effects
- **4.6**: Audio System - Sound effects, battle music
- **4.7**: Post-Battle UI - Results, rewards, level ups display
- **4.8**: Transitions - Battle start/end transitions, scene management

**Related Roadmap**: `docs/features/4-unity-integration/roadmap.md`

---

## Game Features

### Feature 5: Game Features
**Folder**: `docs/features/5-game-features/`  
**Status**: ⏳ Planned  
**Description**: Game systems beyond combat (progression, roguelike, meta-game)

**Sub-Features**:
- **5.1**: Post-Battle Rewards - EXP calculation, level ups, rewards
- **5.2**: Pokemon Management - Party, PC, catching system
- **5.3**: Encounter System - Wild, trainer, boss battles
- **5.4**: Inventory System - Item management and usage
- **5.5**: Save System - Save/load game progress
- **5.6**: Progression System - Roguelike runs, meta-progression

**Related Roadmap**: `docs/features/5-game-features/roadmap.md`

---

## Feature Reference Table

| # | Feature Name | Folder | Status | Roadmap |
|---|--------------|--------|--------|---------|
| **1** | Game Data | `1-game-data/` | ✅ Core Complete | [roadmap.md](../features/1-game-data/roadmap.md) |
| **2** | Combat System | `2-combat-system/` | ✅ Core Complete | [roadmap.md](../features/2-combat-system/roadmap.md) |
| **3** | Content Expansion | `3-content-expansion/` | 🎯 In Progress | [roadmap.md](../features/3-content-expansion/roadmap.md) |
| **4** | Unity Integration | `4-unity-integration/` | ⏳ Planned | [roadmap.md](../features/4-unity-integration/roadmap.md) |
| **5** | Game Features | `5-game-features/` | ⏳ Planned | [roadmap.md](../features/5-game-features/roadmap.md) |

---

## Usage Guidelines

### Referencing Features in Documentation

When referencing features in documentation, use the format:

**Format**: `Feature [Number]: [Feature Name]` or `[Number]: [Feature Name]`

**Examples**:
- "Feature 2: Combat System" or "2: Combat System"
- "Sub-feature 2.4: Damage Calculation Pipeline"
- "See Feature 2 roadmap for implementation details"

### In Roadmaps

Roadmaps should reference features using their numbers:
- "Depends on: Feature 1: Game Data"
- "Related to: Feature 2.4: Damage Calculation Pipeline"
- "Integrates with: Feature 2: Combat System"

### In Architecture Documents

Architecture documents should reference related features:
- "Integrates with Feature 2: Combat System"
- "Uses Feature 1.1: PokemonSpeciesData (Blueprint)"
- "Provides data for Feature 2: Combat System"

### In Use Cases

Use cases should reference feature numbers:
- "UC-001: Feature 1: Game Data - Define Pokemon Species"
- "INT-001: Feature 1 ↔ Feature 2 - Game Data → Combat System"
- "EC-001: Feature 2.4: Damage Calculation Pipeline - Zero Power Move"

---

## Feature Status Legend

- ✅ **Complete** - Feature fully implemented and tested
- ✅ **Core Complete** - Core phases complete, extended phases planned (used for Feature 2)
- 🎯 **In Progress** - Feature currently being implemented
- ⏳ **Planned** - Feature planned but not started

---

## Testing Documentation

**Testing is NOT a feature** - Each feature has its own `testing.md` file documenting its testing strategy. The shared testing strategy document is:

- **Test Structure Standard**: `docs/ai/testing_structure_definition.md` - Standard test organization (Systems/, Blueprints/, Data/)

**Feature Testing Documents** (each feature has its own):
- `docs/features/1-game-data/testing.md` - Game data testing strategy
- `docs/features/2-combat-system/testing.md` - Combat system testing strategy
- `docs/features/3-content-expansion/testing.md` - Content expansion testing strategy
- `docs/features/4-unity-integration/testing.md` - Unity integration testing strategy
- `docs/features/5-game-features/testing.md` - Game features testing strategy

**Additional Testing Resources**:
- `docs/features/2-combat-system/testing/integration_guide.md` - Integration test patterns
- `docs/ai/examples/good_tests.md` - Test code examples

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

- **Feature 2**: `docs/features/2-combat-system/README.md`
- **Sub-feature 2.5**: `docs/features/2-combat-system/2.5-combat-actions/README.md`
- **Sub-feature 3.1**: `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`

**⚠️ Always use numbered paths** - Never use unnumbered paths like `pokemon-data/` or `combat-system/`

---

## ➕ Creating New Features

### Process

1. **Determine Number**: Check this document for next available number (currently: 5)
2. **Choose Name**: Use kebab-case, check for duplicates
3. **Create Folder**: `docs/features/[N]-[feature-name]/`
4. **Create Documents**: Use templates from `docs/feature_documentation_standard.md`
5. **Update This Document**: Add feature entry with number, name, folder, status, description
6. **Update Features Overview**: Add to `docs/features/README.md`
7. **Use Numbered Format**: Always reference as `Feature [N]: [Name]` or `[N]: [Name]`

### Required Information

When adding a new feature, provide:
- **Feature Number**: Next sequential number
- **Feature Name**: Title Case display name
- **Folder**: `[N]-[kebab-case-name]`
- **Status**: ⏳ Planned / 🎯 In Progress / ✅ Complete
- **Description**: One-line description
- **Sub-Features**: List if applicable (numbered as `[N].[M]`)

### Example Entry

```markdown
### Feature 6: New Feature
**Folder**: `docs/features/6-new-feature/`  
**Status**: ⏳ Planned  
**Description**: Brief description of what this feature does

**Sub-Features**:
- **6.1**: First Sub-Feature - Description
- **6.2**: Second Sub-Feature - Description
```

---

## ➕ Creating New Sub-Features

### Process

1. **Determine Number**: Check parent feature's sub-features for next number
2. **Choose Name**: Use kebab-case, descriptive name
3. **Create Folder**: `docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/`
4. **Create Documents**: Minimum `README.md`, add others if needed
5. **Update This Document**: Add sub-feature entry under parent feature
6. **Update Parent README**: Add sub-feature link to parent's `README.md`
7. **Use Numbered Format**: Always reference as `Sub-Feature [N.M]: [Name]` or `[N.M]: [Name]`

### Required Information

When adding a new sub-feature, provide:
- **Sub-Feature Number**: `[N].[M]` format
- **Sub-Feature Name**: Title Case display name
- **Folder**: `[N.M]-[kebab-case-name]`
- **Description**: Brief description

### Example Entry

```markdown
**Sub-Features**:
- **[N.M]**: Sub-Feature Name - Description
```

---

## 📝 Naming Convention Rules

### ⚠️ CRITICAL: Always Use Numbered Format

**In Documentation**:
- ✅ **Correct**: "Feature 2: Combat System" or "2: Combat System"
- ❌ **Wrong**: "Combat System" (without number)

**In File Paths**:
- ✅ **Correct**: `docs/features/2-combat-system/`
- ❌ **Wrong**: `docs/features/combat-system/`

**In References**:
- ✅ **Correct**: `[Feature 2: Combat System](../2-combat-system/)`
- ❌ **Wrong**: `[Combat System](../combat-system/)`

**In Code Comments**:
- ✅ **Correct**: `// See Feature 2: Combat System architecture`
- ❌ **Wrong**: `// See Combat System architecture`

### Folder Naming

- **Features**: `[N]-[kebab-case-name]` (e.g., `1-game-data`)
- **Sub-Features**: `[N.M]-[kebab-case-name]` (e.g., `2.5-combat-actions`)
- **Always use kebab-case**: lowercase with hyphens
- **Always include number**: Never create unnumbered folders

### Display Naming

- **Features**: `Feature [N]: [Feature Name]` or `[N]: [Feature Name]`
- **Sub-Features**: `Sub-Feature [N.M]: [Sub-Feature Name]` or `[N.M]: [Sub-Feature Name]`
- **Use Title Case**: Capitalize first letter of each word

## Related Documents

- **[Features Overview](../features/README.md)** - Overview of all features
- **[Feature Documentation Standard](feature_documentation_standard.md)** - Standard structure for feature documentation
- **[Project Guidelines](../ai/guidelines/project_guidelines.md)** - Coding rules and standards

---

**Last Updated**: 2025-01-XX  
**Maintained By**: Project maintainers

