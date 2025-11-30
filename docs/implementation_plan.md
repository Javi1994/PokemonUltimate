# Implementation Plan: Pokemon Battle Engine

> **Last Updated**: November 2025 | **Tests**: 1,165 passing | **Warnings**: 0

## Goal

Build a functional **Combat Simulator** (1v1) running in a Console Environment (or Test Runner) before touching Unity.

**Philosophy**: Testability First. We build the engine in a pure C# Class Library (`PokemonUltimate.Core`).

---

## Current Status

| Phase | Status | Tests |
|-------|--------|-------|
| Step 0: Project Setup | ✅ Complete | - |
| Step 1: Data Foundation | ✅ Complete | ~1,165 |
| Step 2: Action Queue | 🎯 Next | - |
| Step 3: Battle Field | ⏳ Pending | - |
| Step 4: Turn Loop | ⏳ Pending | - |
| Step 5: First Attack | ⏳ Pending | - |
| Step 6: Integration | ⏳ Pending | - |

---

## Step 0: Project Setup ✅ COMPLETE

**Objective**: Create the workspace structure.

| Task | Status |
|------|--------|
| Solution `PokemonUltimate.sln` | ✅ |
| Class Library `PokemonUltimate.Core` (netstandard2.1) | ✅ |
| Class Library `PokemonUltimate.Content` (netstandard2.1) | ✅ |
| NUnit Project `PokemonUltimate.Tests` (net8.0) | ✅ |
| Console App `PokemonUltimate.Console` (net8.0) | ✅ |
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

## Step 2: Action Queue 🎯 NEXT

**Objective**: Verify the "Everything is an Action" pattern.

### Tests to Write First
```csharp
[Test]
public void ProcessQueue_SingleAction_ExecutesAction()
{
    // Arrange: MockAction that increments counter
    // Act: Enqueue and ProcessQueue
    // Assert: Counter == 1
}
```

### Components to Implement
- [ ] `BattleAction` (Abstract base)
- [ ] `BattleQueue` (Processor)
- [ ] `IBattleView` (Interface for presentation)

---

## Step 3: Battle Field ⏳

**Objective**: Verify we can place Pokemon in slots.

### Tests to Write
```csharp
[Test]
public void Initialize_1v1Battle_CreatesCorrectSlots()
{
    // Arrange: BattleField
    // Act: Initialize(1, 1), add Pokemon
    // Assert: PlayerSide.GetActivePokemon().Count == 1
}
```

### Components to Implement
- [ ] `BattleField`
- [ ] `BattleSide`
- [ ] `BattleSlot`
- [ ] `BattleRules` (Config)

---

## Step 4: Turn Loop ⏳

**Objective**: Verify Speed determines order.

### Tests to Write
```csharp
[Test]
public void RunTurn_DifferentSpeeds_FasterGoesFirst()
{
    // Arrange: FastMon (Speed 100), SlowMon (Speed 50)
    // Act: RunTurn()
    // Assert: FastMon action executed before SlowMon
}
```

### Components to Implement
- [ ] `CombatEngine` (Controller)
- [ ] `TurnOrderResolver` (Speed-based sorting, priority handling)
- [ ] `IActionProvider` (Interface)

---

## Step 5: First Attack ⏳

**Objective**: Verify moves deal damage.

### Tests to Write
```csharp
[Test]
public void UseMoveAction_Tackle_DealsDamage()
{
    // Arrange: Attacker, Defender, Tackle
    // Act: Execute UseMoveAction
    // Assert: Defender HP decreased
}
```

### Components to Implement
- [ ] `UseMoveAction`
- [ ] `DamageAction`
- [ ] `DamageCalculator` (Pipeline with modifiers)

---

## Step 6: Integration ⏳

**Objective**: Run a full battle without rendering.

### Tests to Write
```csharp
[Test]
public void FullBattle_TwoRandomAI_ProducesWinner()
{
    // Arrange: 1v1 with RandomAI
    // Act: RunBattle()
    // Assert: BattleResult.Winner is not null
}
```

### Components to Implement
- [ ] `RandomAI` (Simple AI)
- [ ] `BattleResult`
- [ ] Full battle loop

---

## Architecture Reference

```
PokemonUltimate/
├── Core/              # Game logic (NO data)
│   ├── Blueprints/    # Immutable definitions
│   ├── Instances/     # Mutable runtime state
│   ├── Factories/     # Creation & calculation
│   ├── Effects/       # Move effects
│   ├── Evolution/     # Evolution system
│   ├── Registry/      # Data access
│   ├── Enums/         # Type definitions
│   └── Constants/     # Centralized strings
│
├── Content/           # Game data
│   ├── Catalogs/      # Pokemon & Move definitions
│   └── Builders/      # Fluent APIs
│
└── Tests/             # 1,165 tests
```

---

## Why This Order?

1. **Data** is the atoms
2. **Queue** is the physics engine
3. **Field** is the world
4. **Turn** is time
5. **Attack** is interaction
6. **Integration** is the game

Once Step 6 passes, you have a working game engine. Only THEN do you open Unity to visualize it.

---

## Key Documents

| Need | Read |
|------|------|
| Current state | `.ai/context.md` |
| Coding rules | `docs/project_guidelines.md` |
| Combat design | `docs/architecture/combat_system_spec.md` |
| Damage formulas | `docs/architecture/damage_and_effect_system.md` |
| Turn order | `docs/architecture/turn_order_system.md` |
