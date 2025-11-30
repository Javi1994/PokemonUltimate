# Implementation Plan: Combat MVP (Vertical Slice)

## Goal
Build a functional **Combat Simulator** (1v1) running in a Console Environment (or Test Runner) before touching Unity.
**Philosophy**: Testability First. We build the engine in a pure C# Class Library (`PokemonUltimate.Core`).

---

## Step 0: Project Setup ✅ COMPLETE
**Objective**: Create the workspace structure.

1.  ✅ Create Solution `PokemonUltimate.sln`.
2.  ✅ Create Class Library `PokemonUltimate.Core` (The Engine) - netstandard2.1 for Unity compatibility.
3.  ✅ Create Class Library `PokemonUltimate.Content` (The Data) - netstandard2.1.
    - Contains: Catalogs (Pokemon, Moves) and Builders (fluent API)
    - Separated from Core for clean architecture (engine vs data)
4.  ✅ Create NUnit Project `PokemonUltimate.Tests` (The Verifier) - net8.0.
5.  ✅ Create Console App `PokemonUltimate.Console` (Smoke Test) - net8.0.
    - Runtime verification of all data systems (~70 tests)
    - Tests: Catalogs, Registries, Builders, Effects, Evolutions, Nature/Gender
    - Run with: `dotnet run --project PokemonUltimate.Console`
6.  ✅ Document project structure (`docs/architecture/project_structure.md`).
7.  ⏳ Create Unity Project `PokemonUltimate.Unity` (The Viewer) - *Leave empty for now*.

---

## Step 1: The Data Foundation & Registry (TDD) ✅ COMPLETE
**Objective**: Verify we can store, retrieve, and instantiate Pokemon and Moves.

### 1.1 Registry System ✅ COMPLETE
1.  ✅ **Interfaces**: `IIdentifiable`, `IDataRegistry<T>`
2.  ✅ **Implementation**: `GameDataRegistry<T>` (Dictionary-based)
3.  ✅ **Tests**: Register, Get, Exists, GetAll, edge cases

### 1.2 Pokemon Data ✅ COMPLETE
1.  ✅ **Blueprint**: `PokemonSpeciesData` (Name, PokedexNumber, Types, BaseStats, GenderRatio, Learnset, Evolutions)
2.  ✅ **BaseStats**: Class with HP, Attack, Defense, SpAttack, SpDefense, Speed, Total
3.  ✅ **Learnset**: `LearnableMove` class + `LearnMethod` enum
4.  ✅ **Evolution**: `Evolution` class + `IEvolutionCondition` interface + 6 condition types
5.  ✅ **Gender**: `Gender` enum + GenderRatio in PokemonSpeciesData + helper properties
6.  ✅ **Nature**: `Nature` enum (25) + `NatureData` static class (stat modifiers ±10%)
7.  ✅ **Registry**: `IPokemonRegistry`, `PokemonRegistry` (dual lookup: Name + PokedexNumber)
8.  ✅ **Catalog**: `PokemonCatalog` (15 Pokemon with official Gen 1 stats, types, learnsets, evolutions)
9.  ✅ **Builder**: `PokemonBuilder`, `LearnsetBuilder`, `EvolutionBuilder` (fluent API with gender methods)
10. ✅ **Tests**: 100+ tests covering registry, model, stats, catalog, builders, evolution, gender, nature

### 1.3 Move Data ✅ COMPLETE
1.  ✅ **Blueprint**: `MoveData` (Name, Type, Category, Power, Accuracy, PP, Priority, TargetScope, Effects)
2.  ✅ **Enums**: `PokemonType` (18), `MoveCategory` (3), `TargetScope` (10), `Stat` (8), `PersistentStatus` (7), `VolatileStatus` (11), `EffectType` (24)
3.  ✅ **Registry**: `IMoveRegistry`, `MoveRegistry` (with Type/Category filters)
4.  ✅ **Catalog**: `MoveCatalog` (20 Moves: Normal, Fire, Water, Grass, Electric, Ground, Psychic)
5.  ✅ **Effects**: `IMoveEffect` interface + 9 concrete effects (DamageEffect, FixedDamageEffect, StatusEffect, StatChangeEffect, RecoilEffect, DrainEffect, HealEffect, FlinchEffect, MultiHitEffect)
6.  ✅ **Builder**: `MoveBuilder` + `EffectBuilder` (fluent API for composing moves and effects)
7.  ✅ **Tests**: 100+ tests covering registry, filter, model, effect, composition, catalog effects, builders

### 1.4 Instance & Factory ✅ COMPLETE
**Objective**: Transform static blueprints into mutable runtime instances.

1.  ✅ **MoveInstance** (`Core/Instances/MoveInstance.cs`)
    - Reference to `MoveData` blueprint
    - `CurrentPP`, `MaxPP` tracking
    - `HasPP`, `Use()`, `Restore()`, `RestoreFully()` methods
    - 19 tests

2.  ✅ **StatCalculator** (`Core/Factories/StatCalculator.cs`)
    - Gen3+ simplified formula (no IVs/EVs)
    - HP formula: `((Base * 2) * Level / 100) + Level + 10`
    - Stat formula: `(((Base * 2) * Level / 100) + 5) * NatureModifier`
    - Stat stage multipliers, accuracy stage multipliers
    - 31 tests

3.  ✅ **PokemonInstance** (`Core/Instances/PokemonInstance.cs`)
    - Identity: `Species`, `InstanceId` (GUID), `Nickname`, `DisplayName`
    - Level: `Level`, `CurrentExp`
    - Stats: `MaxHP`, `CurrentHP`, `Attack`, `Defense`, `SpAttack`, `SpDefense`, `Speed`
    - Personal: `Gender`, `Nature`, `Friendship` (0-255), `IsShiny`
    - Combat: `Moves`, `Status`, `VolatileStatus`, `StatStages` (-6 to +6)
    - Helpers: `IsFainted`, `HPPercentage`, `HasHighFriendship`, `HasMaxFriendship`
    - Methods: `GetEffectiveStat`, `ModifyStatStage`, `TakeDamage`, `Heal`, `FullHeal`, etc.
    - 55 tests

4.  ✅ **PokemonInstanceBuilder** (`Core/Factories/PokemonInstanceBuilder.cs`)
    - Fluent builder pattern for full control
    - Nature: `WithNature()`, `WithNatureBoosting()`, `WithNeutralNature()`
    - Gender: `WithGender()`, `Male()`, `Female()`
    - Moves: `WithMoves()`, `WithRandomMoves()`, `WithSingleMove()`, `WithNoMoves()`
    - HP: `AtFullHealth()`, `AtHealth()`, `AtHealthPercent()`, `AtHalfHealth()`, `Fainted()`
    - Status: `Burned()`, `Paralyzed()`, `Poisoned()`, `Asleep()`, `Frozen()`
    - Friendship: `WithFriendship()`, `WithHighFriendship()`, `WithMaxFriendship()`, `AsHatched()`
    - Shiny: `Shiny()`, `NotShiny()`, `WithShinyChance()`
    - Stats override: `WithStats()`, `WithMaxHP()`, `WithAttack()`, `WithSpeed()`
    - 70 tests

5.  ✅ **PokemonFactory** (`Core/Factories/PokemonFactory.cs`)
    - Quick creation methods (delegate to builder)
    - `Create(species, level)`, `Create(species, level, nature)`, `Create(species, level, nature, gender)`
    - 29 tests

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
