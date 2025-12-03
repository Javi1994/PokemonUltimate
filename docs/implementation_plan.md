# Implementation Plan: Pokemon Battle Engine

> **Last Updated**: January 2025 | **Tests**: 2,075+ passing | **Warnings**: 0

## Goal

Build a functional **Combat Simulator** (1v1, 2v2, 1v3, etc.) running in a Console Environment (or Test Runner) before touching Unity.

**Philosophy**: Testability First. We build the engine in a pure C# Class Library (`PokemonUltimate.Core`, `PokemonUltimate.Combat`).

---

## Current Status

| Feature | Status | Tests | Roadmap |
|---------|--------|-------|---------|
| **Feature 1: Game Data** | ✅ Core Complete | ~800 | [roadmap.md](features/1-game-data/roadmap.md) |
| **Feature 2: Combat System** | ✅ Core Complete (2.1-2.11) | ~1,200 | [roadmap.md](features/2-combat-system/roadmap.md) |
| **Feature 3: Content Expansion** | 🎯 In Progress | ~75 | [roadmap.md](features/3-content-expansion/roadmap.md) |
| **Feature 4: Unity Integration** | ⏳ Planned | - | [roadmap.md](features/4-unity-integration/roadmap.md) |
| **Feature 5: Game Features** | ⏳ Planned | - | [roadmap.md](features/5-game-features/roadmap.md) |

**⚠️ Feature Documentation**: Always use numbered paths: `docs/features/[N]-[feature-name]/`  
**📋 Master Reference**: See [`features_master_list.md`](features_master_list.md) for complete feature list

---

## Feature 1: Game Data ✅ COMPLETE

**Status**: ✅ Core Complete  
**Roadmap**: [`docs/features/1-game-data/roadmap.md`](features/1-game-data/roadmap.md)

### Completed Sub-Features

#### 1.1: Pokemon Data ✅
- `PokemonSpeciesData` (Name, PokedexNumber, Types, BaseStats, GenderRatio, Learnset, Evolutions, Variants support)
- `BaseStats` with validation, `GetStat()`, `HighestStat`, `LowestStat`
- `LearnableMove` + `LearnMethod` enum
- `PokemonInstance` (partial classes):
  - `PokemonInstance.cs` - Core properties
  - `PokemonInstance.Battle.cs` - Combat state, status effects on stats
  - `PokemonInstance.LevelUp.cs` - Experience, move learning
  - `PokemonInstance.Evolution.cs` - Evolution conditions & execution
- `PokemonInstanceBuilder`: Full fluent API
- `PokemonFactory`: Quick creation

#### 1.2: Move Data ✅
- `MoveData` with 15+ flags (MakesContact, IsSoundBased, NeverMisses, etc.)
- `PokemonType` (18), `MoveCategory` (3), `TargetScope` (10)
- `MoveInstance`: PP tracking, PP Ups support (max 3)
- `IMoveEffect` + 22 effect classes
- `MoveBuilder` + `EffectBuilder`

#### 1.3: Ability Data ✅
- `AbilityData` blueprint with triggers and effects
- `AbilityBuilder` fluent API
- Species have Ability1, Ability2, HiddenAbility
- Instance has assigned Ability

#### 1.4: Item Data ✅
- `ItemData` blueprint with categories and effects
- `ItemBuilder` fluent API
- Instance has HeldItem

#### 1.5: Status Effect Data ✅
- `StatusEffectData` blueprint with full mechanics
- `StatusEffectBuilder` fluent API
- Damage per turn, stat modifiers, duration, immunities

#### 1.6: Field Conditions Data ✅
- `WeatherData`, `TerrainData`, `HazardData`, `SideConditionData`, `FieldEffectData`
- Builders for all field condition types

#### 1.7: Evolution System ✅
- `Evolution` + `IEvolutionCondition` (6 condition types)
- `EvolutionBuilder`
- `Gender` enum + `GenderRatio`

#### 1.8: Type Effectiveness Table ✅
- Gen 6+ chart (18 types, Fairy included)
- Single/dual type effectiveness
- STAB calculation (1.5x)
- Immunity handling

#### 1.9: Interfaces Base ✅
- `IIdentifiable` interface

#### 1.10: Enums & Constants ✅
- Enums (20 main + 7 in Effects)
- `ErrorMessages.cs` - Centralized error strings
- `GameMessages.cs` - Centralized game strings
- `Nature` enum (25) + `NatureData` (stat modifiers ±10%)

#### 1.11: Builders ✅
- 13 builder classes + 10 static helper classes
- `PokemonBuilder`, `MoveBuilder`, `AbilityBuilder`, `ItemBuilder`, `StatusEffectBuilder`, etc.

#### 1.12: Factories & Calculators ✅
- **StatCalculator**: Gen 3+ formulas with IVs, EVs, Nature
  - HP: `floor((2*Base + IV + floor(EV/4)) * Level / 100) + Level + 10`
  - Other: `floor((floor((2*Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)`
  - Verified against real Pokemon data
- `PokemonFactory`: Quick creation

#### 1.13: Registry System ✅
- `IDataRegistry<T>` interface  
- `GameDataRegistry<T>` with `Get`, `GetById`, `TryGet`, `Contains`, `All`, `Count`
- `PokemonRegistry` with type/pokedex queries
- `MoveRegistry` with type/power/category queries

### Planned Sub-Features

#### 1.14: Variants System ⏳
- Mega/Dinamax/Terracristalización as separate species

#### 1.15: Pokedex Fields ⏳
- Description, Category, Height, Weight, Color, Shape, Habitat

---

## Feature 2: Combat System ✅ CORE COMPLETE

**Status**: ✅ Core Complete (Phases 2.1-2.11)  
**Roadmap**: [`docs/features/2-combat-system/roadmap.md`](features/2-combat-system/roadmap.md)

### Completed Sub-Features

#### 2.1: Battle Foundation ✅
- `BattleField` - Contains both sides
- `BattleSide` - Player or Enemy side with slots and bench
- `BattleSlot` - Active Pokemon position with stat stages
- `BattleRules` - Configuration (1v1, 2v2, etc.)

#### 2.2: Action Queue System ✅
- `BattleAction` - Abstract base with `ExecuteLogic()` and `ExecuteVisual()`
- `MessageAction` - Simple message display
- `BattleQueue` - Action processor with reaction handling
- `IBattleView` - Interface for presentation
- `NullBattleView` - No-op implementation for testing

#### 2.3: Turn Order Resolution ✅
- `TurnOrderResolver` - Speed-based sorting with priority handling
- Stat stage modifiers
- Paralysis speed penalty

#### 2.4: Damage Calculation Pipeline ✅
- `DamageContext` - Calculation context
- `IDamageStep` - Pipeline step interface
- `DamagePipeline` - Orchestrates calculation
- **Steps**: BaseDamage, CriticalHit, RandomFactor, STAB, AttackerAbility, AttackerItem, TypeEffectiveness, Burn

#### 2.5: Combat Actions ✅
- `UseMoveAction` - Execute a move (with PP, accuracy, effects)
- `SwitchAction` - Switch Pokemon (priority +6)
- `DamageAction` - Apply damage (with faint detection)
- `FaintAction` - Handle Pokemon fainting
- `HealAction` - Restore HP
- `StatChangeAction` - Modify stat stages
- `ApplyStatusAction` - Apply status conditions
- `MessageAction` - Display battle messages
- `AccuracyChecker` - Accuracy calculation system

#### 2.6: Combat Engine ✅
- `CombatEngine` - Main controller
- `BattleArbiter` - Victory/defeat detection
- Turn phases (Selection, Execution, End)
- Victory/Defeat detection

#### 2.7: Integration ✅
- `RandomAI` - Simple AI
- `AlwaysAttackAI` - Always attack AI
- `PlayerInputProvider` - Human player input provider
- `IActionProvider` - Action provider interface
- `BattleResult` - Detailed battle result
- Full battle loop

#### 2.8: End-of-Turn Effects ✅
- `EndOfTurnProcessor` - End-of-turn effects processor
- Status damage: Burn, Poison, Toxic

#### 2.9: Abilities & Items ✅
- `BattleTrigger` - Trigger type enum
- `IBattleListener` - Listener interface
- `AbilityListener` - Ability event handler
- `ItemListener` - Item event handler
- `BattleTriggerProcessor` - Trigger processor

#### 2.10: Pipeline Hooks ✅
- `IStatModifier` - Stat modifier interface
- `AbilityStatModifier` - Ability-based stat modifier
- `ItemStatModifier` - Item-based stat modifier

#### 2.11: Recoil & Drain ✅
- Recoil damage effects
- HP drain effects

### Planned Sub-Features

#### 2.12-2.19: Extended Features ⏳
- Weather System, Terrain System, Hazards System
- Advanced Move Mechanics, Field Conditions
- Advanced Abilities, Advanced Items
- Battle Formats (Doubles, Triples, Rotation)

---

## Feature 3: Content Expansion 🎯 IN PROGRESS

**Status**: 🎯 In Progress (26 Pokemon, 36 Moves)  
**Roadmap**: [`docs/features/3-content-expansion/roadmap.md`](features/3-content-expansion/roadmap.md)

### Current Status

- ✅ **Pokemon**: 26 Gen 1 Pokemon
- ✅ **Moves**: 36 moves (12 types)
- ✅ **Items**: ~23 items (held items + berries)
- ✅ **Abilities**: 35 abilities
- ✅ **Status Effects**: 15 statuses
- ✅ **Field Conditions**: Weather, Terrain, Hazards, Side Conditions, Field Effects

### Sub-Features

#### 3.1: Pokemon Expansion 🎯
- Adding more Pokemon species

#### 3.2: Move Expansion 🎯
- Adding more moves across all types

#### 3.3: Item Expansion ⏳
- Adding more held items and consumables

#### 3.4: Ability Expansion ⏳
- Adding more abilities

#### 3.5: Content Validation ⏳
- Quality standards and validation

#### 3.6: Content Organization ✅
- Catalog organization and maintenance

---

## Feature 4: Unity Integration ⏳ PLANNED

**Status**: ⏳ Planned  
**Roadmap**: [`docs/features/4-unity-integration/roadmap.md`](features/4-unity-integration/roadmap.md)

### Planned Sub-Features

- 4.1: Unity Project Setup
- 4.2: UI Foundation
- 4.3: IBattleView Implementation
- 4.4: Player Input System
- 4.5: Animations System
- 4.6: Audio System
- 4.7: Post-Battle UI
- 4.8: Transitions

---

## Feature 5: Game Features ⏳ PLANNED

**Status**: ⏳ Planned  
**Roadmap**: [`docs/features/5-game-features/roadmap.md`](features/5-game-features/roadmap.md)

### Planned Sub-Features

- 5.1: Post-Battle Rewards
- 5.2: Pokemon Management
- 5.3: Encounter System
- 5.4: Inventory System
- 5.5: Save System
- 5.6: Progression System

---

## Architecture Reference

```
PokemonUltimate/
├── Core/              # Game logic (NO data)
│   ├── Blueprints/    # Immutable definitions (Pokemon, Move, Ability, Item, Status)
│   ├── Instances/     # Mutable runtime state
│   ├── Factories/     # Creation & calculation
│   ├── Builders/      # Core builders (Ability, Item, Status)
│   ├── Effects/       # Move effects
│   ├── Evolution/     # Evolution system
│   ├── Registry/      # Data access
│   ├── Enums/         # Type definitions
│   └── Constants/     # Centralized strings
│
├── Combat/            # Battle system
│   ├── Field/         # BattleField, BattleSide, BattleSlot, BattleRules
│   ├── Engine/        # CombatEngine, BattleArbiter, BattleQueue
│   ├── Results/       # BattleOutcome, BattleResult
│   ├── Providers/     # IActionProvider
│   ├── View/          # IBattleView, NullBattleView
│   ├── Actions/       # BattleAction implementations
│   ├── Damage/        # DamagePipeline, IDamageStep
│   └── Helpers/       # AccuracyChecker, TurnOrderResolver
│
├── Content/           # Game data
│   └── Catalogs/      # Pokemon, Move, Ability, Item, Status, Weather, Terrain, etc.
│
└── Tests/             # 2,075+ tests
```

---

## Key Documents

| Need | Read |
|------|------|
| Current state | `.ai/context.md` |
| Coding rules | `docs/ai/guidelines/project_guidelines.md` |
| **Feature master list** | `docs/features_master_list.md` ⭐ **MASTER REFERENCE** |
| **Feature documentation standard** | `docs/feature_documentation_standard.md` |
| **Feature 1: Game Data** | `docs/features/1-game-data/roadmap.md` |
| **Feature 2: Combat System** | `docs/features/2-combat-system/roadmap.md` |
| **Feature 3: Content Expansion** | `docs/features/3-content-expansion/roadmap.md` |
| **Feature 4: Unity Integration** | `docs/features/4-unity-integration/roadmap.md` |
| **Feature 5: Game Features** | `docs/features/5-game-features/roadmap.md` |
| **Test structure** | `docs/ai/testing_structure_definition.md` |
| **Use cases** | `docs/features/2-combat-system/use_cases.md` |
| Combat design | `docs/features/2-combat-system/architecture.md` |
| Abilities/Items | `docs/features/2-combat-system/2.9-abilities-items/architecture.md` |
| Status effects | `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md` |
| Damage formulas | `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md` |
| Turn order | `docs/features/2-combat-system/2.3-turn-order-resolution/architecture.md` |
| Game data structure | `docs/features/1-game-data/architecture.md` |

**⚠️ Feature Documentation**: Always use numbered paths:
- ✅ `docs/features/[N]-[feature-name]/` (e.g., `docs/features/2-combat-system/`)
- ❌ `docs/features/feature-name/` (wrong - missing number)

---

**Last Updated**: 2025-01-XX
