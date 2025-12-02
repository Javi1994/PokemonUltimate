# Implementation Plan: Pokemon Battle Engine

> **Last Updated**: December 2025 | **Tests**: 1,885 passing | **Warnings**: 0

## Goal

Build a functional **Combat Simulator** (1v1, 2v2, 1v3, etc.) running in a Console Environment (or Test Runner) before touching Unity.

**Philosophy**: Testability First. We build the engine in a pure C# Class Library (`PokemonUltimate.Core`, `PokemonUltimate.Combat`).

---

## Current Status

| Phase | Status | Tests |
|-------|--------|-------|
| Step 0: Project Setup | ✅ Complete | - |
| Step 1: Data Foundation | ✅ Complete | ~800 |
| Step 2: Combat Foundation | ✅ Complete | ~300 |
| Step 3: Damage Pipeline | ✅ Complete | ~200 |
| Step 4: Data Enhancement | ✅ Complete | ~300 |
| Step 5: Combat Actions | ✅ Complete | 47 |
| Step 6: Combat Engine | 🎯 Next | - |
| Step 7: Integration | ⏳ Pending | - |

---

## Step 0: Project Setup ✅ COMPLETE

**Objective**: Create the workspace structure.

| Task | Status |
|------|--------|
| Solution `PokemonUltimate.sln` | ✅ |
| Class Library `PokemonUltimate.Core` (netstandard2.1) | ✅ |
| Class Library `PokemonUltimate.Content` (netstandard2.1) | ✅ |
| NUnit Project `PokemonUltimate.Tests` (net8.0) | ✅ |
| Smoke Tests `PokemonUltimate.SmokeTests` (net8.0) | ✅ |
| Documentation structure (`docs/`) | ✅ |
| AI infrastructure (`.ai/`, `.cursorrules`) | ✅ |
| Unity Project | ⏳ Later |

---

## Step 1: Data Foundation ✅ COMPLETE

### 1.1 Registry System ✅

- `IIdentifiable` interface
- `IDataRegistry<T>` interface  
- `GameDataRegistry<T>` with `Get`, `GetById`, `TryGet`, `Contains`, `All`, `Count`
- `PokemonRegistry` with type/pokedex queries
- `MoveRegistry` with type/power/category queries

### 1.2 Pokemon Blueprints ✅

- `PokemonSpeciesData` (Name, PokedexNumber, Types, BaseStats, GenderRatio, Learnset, Evolutions)
- `BaseStats` with validation, `GetStat()`, `HighestStat`, `LowestStat`
- `LearnableMove` + `LearnMethod` enum
- `Evolution` + `IEvolutionCondition` (6 condition types)
- `Gender` enum + `GenderRatio`
- `Nature` enum (25) + `NatureData` (stat modifiers ±10%)
- `PokemonCatalog` (15 Gen 1 Pokemon with learnsets & evolutions)
- `PokemonBuilder`, `LearnsetBuilder`, `EvolutionBuilder`

### 1.3 Move Blueprints ✅

- `MoveData` with 15+ flags (MakesContact, IsSoundBased, NeverMisses, etc.)
- `PokemonType` (18), `MoveCategory` (3), `TargetScope` (10)
- `Stat` (8), `PersistentStatus` (7), `VolatileStatus` (11), `EffectType` (24)
- `MoveCatalog` (50+ moves across 7 types)
- `IMoveEffect` + 9 effect classes
- `MoveBuilder` + `EffectBuilder`

### 1.4 Instances ✅

- **MoveInstance**: PP tracking, PP Ups support (max 3)
- **PokemonInstance** (partial classes):
  - `PokemonInstance.cs` - Core properties
  - `PokemonInstance.Battle.cs` - Combat state, status effects on stats
  - `PokemonInstance.LevelUp.cs` - Experience, move learning
  - `PokemonInstance.Evolution.cs` - Evolution conditions & execution
- **PokemonInstanceBuilder**: Full fluent API
- **PokemonFactory**: Quick creation

### 1.5 Calculations ✅

- **StatCalculator**: Gen 3+ formulas with IVs, EVs, Nature
  - HP: `floor((2*Base + IV + floor(EV/4)) * Level / 100) + Level + 10`
  - Other: `floor((floor((2*Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)`
  - Verified against real Pokemon data
- **TypeEffectiveness**: Gen 6+ chart (18 types, Fairy included)
  - Single/dual type effectiveness
  - STAB calculation (1.5x)
  - Immunity handling

### 1.6 Constants ✅

- `ErrorMessages.cs` - Centralized error strings
- `GameMessages.cs` - Centralized game strings

---

## Step 2: Combat Foundation ✅ COMPLETE

### 2.1 Action Queue ✅
- `BattleAction` - Abstract base with `ExecuteLogic()` and `ExecuteVisual()`
- `MessageAction` - Simple message display
- `BattleQueue` - Action processor with reaction handling
- `IBattleView` - Interface for presentation
- `NullBattleView` - No-op implementation for testing

### 2.2 Battle Field ✅
- `BattleField` - Contains both sides
- `BattleSide` - Player or Enemy side with slots and bench
- `BattleSlot` - Active Pokemon position with stat stages
- `BattleRules` - Configuration (1v1, 2v2, etc.)

### 2.3 Turn Order ✅
- `TurnOrderResolver` - Speed-based sorting with priority handling
- Stat stage modifiers
- Paralysis speed penalty

---

## Step 3: Damage Pipeline ✅ COMPLETE

- `DamageContext` - Calculation context
- `IDamageStep` - Pipeline step interface
- `DamagePipeline` - Orchestrates calculation
- **Steps**: BaseDamage, CriticalHit, RandomFactor, STAB, TypeEffectiveness, Burn

---

## Step 4: Data Enhancement ✅ COMPLETE (NEW)

### 4.1 Abilities ✅
- `AbilityData` blueprint with triggers and effects
- `AbilityBuilder` fluent API
- `AbilityCatalog` (35 abilities: Gen3 + additional)
- Species have Ability1, Ability2, HiddenAbility
- Instance has assigned Ability

### 4.2 Items ✅
- `ItemData` blueprint with categories and effects
- `ItemBuilder` fluent API
- `ItemCatalog` (23 items: held items + berries)
- Instance has HeldItem

### 4.3 Status Effects ✅
- `StatusEffectData` blueprint with full mechanics
- `StatusEffectBuilder` fluent API
- `StatusCatalog` (15 statuses: 6 persistent + 9 volatile)
- Damage per turn, stat modifiers, duration, immunities

---

## Step 5: Combat Actions ✅ COMPLETE

**Objective**: Implement core battle actions.

### Components Implemented
- [x] `UseMoveAction` - Execute a move (with PP, accuracy, effects)
- [x] `SwitchAction` - Switch Pokemon (priority +6)
- [x] `DamageAction` - Apply damage (with faint detection)
- [x] `FaintAction` - Handle Pokemon fainting
- [x] `HealAction` - Restore HP
- [x] `StatChangeAction` - Modify stat stages
- [x] `ApplyStatusAction` - Apply status conditions
- [x] `MessageAction` - Display battle messages
- [x] `AccuracyChecker` - Accuracy calculation system

### Documentation
- [x] `docs/combat/action_use_cases.md` - 207 use cases
- [x] `docs/combat/actions_bible.md` - Complete technical reference

---

## Step 6: Combat Engine ⏳

**Objective**: Full turn loop and battle flow.

### Components to Implement
- [ ] `CombatEngine` - Main controller
- [ ] `CombatArbiter` - Rules enforcement
- [ ] Turn phases (Selection, Execution, End)
- [ ] Victory/Defeat detection

---

## Step 7: Integration ⏳

**Objective**: Run a full battle without rendering.

### Components to Implement
- [ ] `RandomAI` - Simple AI
- [ ] `BattleResult`
- [ ] Full battle loop
- [ ] Console demo

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
├── Combat/            # Battle system (NEW)
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
│   ├── Catalogs/      # Pokemon, Move, Ability, Item, Status
│   └── Builders/      # Pokemon, Move builders
│
└── Tests/             # 1,600 tests
```

---

## Why This Order?

1. **Data** is the atoms (Pokemon, Moves, Types)
2. **Enhancement** adds complexity (Abilities, Items, Status)
3. **Queue** is the physics engine (Action processing)
4. **Field** is the world (BattleField, Slots)
5. **Turn Order** is time (Speed, Priority)
6. **Damage** is interaction (Pipeline)
7. **Actions** are events (UseMoveAction, etc.)
8. **Engine** is the game loop
9. **Integration** is the full game

Once Step 7 passes, you have a working game engine. Only THEN do you open Unity to visualize it.

---

## Key Documents

| Need | Read |
|------|------|
| Current state | `.ai/context.md` |
| Coding rules | `docs/project_guidelines.md` |
| **Combat phases** | `docs/combat_implementation_plan.md` |
| **Use cases** | `docs/combat_use_cases.md` |
| **Unity setup** | `docs/unity_integration.md` |
| Combat design | `docs/architecture/combat_system_spec.md` |
| Abilities/Items | `docs/architecture/abilities_items_system.md` |
| Status effects | `docs/architecture/status_and_stat_system.md` |
| Damage formulas | `docs/architecture/damage_and_effect_system.md` |
| Turn order | `docs/architecture/turn_order_system.md` |
