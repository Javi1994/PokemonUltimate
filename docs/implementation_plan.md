# Implementation Plan: Combat MVP (Vertical Slice)

## Goal
Build a functional **Combat Simulator** (1v1) running in a Console Environment (or Test Runner) before touching Unity.
**Philosophy**: Testability First. We build the engine in a pure C# Class Library (`PokemonUltimate.Core`).

---

## Step 0: Project Setup ✅ COMPLETE
**Objective**: Create the workspace structure.

1.  ✅ Create Solution `PokemonUltimate.sln`.
2.  ✅ Create Class Library `PokemonUltimate.Core` (The Engine) - netstandard2.1 for Unity compatibility.
3.  ✅ Create NUnit Project `PokemonUltimate.Tests` (The Verifier) - net8.0.
4.  ⏳ Create Unity Project `PokemonUltimate.Unity` (The Viewer) - *Leave empty for now*.

---

## Step 1: The Data Foundation & Registry (TDD) 🔄 IN PROGRESS
**Objective**: Verify we can store, retrieve, and instantiate Pokemon and Moves.

### 1.1 Registry System ✅ COMPLETE
1.  ✅ **Interfaces**: `IIdentifiable`, `IDataRegistry<T>`
2.  ✅ **Implementation**: `GameDataRegistry<T>` (Dictionary-based)
3.  ✅ **Tests**: Register, Get, Exists, GetAll, edge cases

### 1.2 Pokemon Data ✅ COMPLETE (Basic)
1.  ✅ **Blueprint**: `PokemonSpeciesData` (Name, PokedexNumber)
2.  ✅ **Registry**: `IPokemonRegistry`, `PokemonRegistry` (dual lookup: Name + PokedexNumber)
3.  ✅ **Catalog**: `PokemonCatalog` (15 Pokemon: starters, Pikachu, legendaries)
4.  ✅ **Tests**: 16 tests for registry + 10 tests for model

### 1.3 Move Data ✅ COMPLETE (Basic)
1.  ✅ **Blueprint**: `MoveData` (Name, Type, Category, Power, Accuracy, PP, Priority, TargetScope, Effects)
2.  ✅ **Enums**: `PokemonType` (18), `MoveCategory` (3), `TargetScope` (10), `Stat` (8), `PersistentStatus` (7), `VolatileStatus` (11), `EffectType` (9)
3.  ✅ **Registry**: `IMoveRegistry`, `MoveRegistry` (with Type/Category filters)
4.  ✅ **Catalog**: `MoveCatalog` (20 Moves: Normal, Fire, Water, Grass, Electric, Ground, Psychic)
5.  ✅ **Effects**: `IMoveEffect` interface + 9 concrete effects (DamageEffect, FixedDamageEffect, StatusEffect, StatChangeEffect, RecoilEffect, DrainEffect, HealEffect, FlinchEffect, MultiHitEffect)
6.  ✅ **Tests**: 9 registry + 9 filter + 12 model + 25 effect + 12 composition + 12 catalog effects tests

### 1.4 Instance & Factory ⏳ PENDING
1.  ⏳ **Expand**: Add BaseStats, Types, MovePool to `PokemonSpeciesData`
2.  ⏳ **Instance**: `PokemonInstance` (Level, CurrentHP, Stats, Moves, Status)
3.  ⏳ **Factory**: `PokemonFactory.Create(species, level)` with stat calculation
4.  ⏳ **Builder**: `PokemonBuilder` for fluent instance creation
5.  ⏳ **Tests**: Factory creates valid instances, stats calculated correctly

---

## Step 2: The Action Queue (TDD)
**Objective**: Verify the "Everything is an Action" pattern.

1.  **Write Test**: `Test_Queue_Processing`
    *   Create a `MockAction` that increments a counter when `ExecuteLogic` is called.
    *   Create `BattleQueue`.
    *   Enqueue the action.
    *   Call `ProcessQueue`.
    *   **Assert**: Counter == 1.
2.  **Implement**:
    *   `BattleAction` (Abstract).
    *   `BattleQueue` (Class).
    *   `IBattleView` (Interface - Mocked in test).

---

## Step 3: The Battle Field (TDD)
**Objective**: Verify we can place Pokemon in slots.

1.  **Write Test**: `Test_Field_Initialization`
    *   Create `BattleField`.
    *   Call `Initialize(1, 1)` (1v1 battle).
    *   Add a Pokemon to Player Slot 0.
    *   **Assert**: `PlayerSide.GetActivePokemon().Count` is 1.
2.  **Implement**:
    *   `BattleField`, `BattleSide`, `BattleSlot`.
    *   `BattleRules` (Config).

---

## Step 4: The Turn Loop (TDD)
**Objective**: Verify Speed determines order.

1.  **Write Test**: `Test_Turn_Order_Speed`
    *   Create "FastMon" (Speed 100) and "SlowMon" (Speed 50).
    *   Mock `IActionProvider` to return a `WaitAction` for both.
    *   Run `CombatEngine.RunTurn()`.
    *   **Assert**: "FastMon" action executed before "SlowMon".
2.  **Implement**:
    *   `CombatEngine` (The Controller).
    *   `TurnOrderResolver` (The Sorter).
    *   `IActionProvider` (Interface).

---

## Step 5: The First Attack (TDD)
**Objective**: Verify "Tackle" deals damage.

1.  **Write Test**: `Test_Move_Damage`
    *   Create "Attacker" (Atk 100) and "Defender" (Def 100).
    *   Create Move "Tackle" (Power 40).
    *   Enqueue `UseMoveAction`.
    *   **Assert**: Defender HP decreased by approx calculation.
    *   **Assert**: `IBattleView.ShowMessage` was called with "Used Tackle".
2.  **Implement**:
    *   `MoveData`, `MoveInstance`.
    *   `UseMoveAction`.
    *   `DamageAction`.
    *   `DamageCalculator` (Pipeline).

---

## Step 6: Integration (The "Bot Battle")
**Objective**: Run a full battle without rendering.

1.  **Write Test**: `Test_Full_Battle_Simulation`
    *   Setup 1v1 Battle.
    *   Assign `RandomAI` to both sides.
    *   Call `RunBattle()` loop.
    *   **Assert**: One side eventually faints (Winner is declared).
    *   **Assert**: `BattleResult` is not null.

---

## Why this order?
1.  **Data** is the atoms.
2.  **Queue** is the physics engine.
3.  **Field** is the world.
4.  **Turn** is time.
5.  **Attack** is interaction.
6.  **Integration** is the game.

Once Step 6 passes, you have a working game engine. Only THEN do you open Unity to visualize it.
