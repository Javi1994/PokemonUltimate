# Project Guidelines: Pokemon Combat Game

## 1. Core Philosophy
*These rules are absolute. All code must adhere to them.*

1.  **Testability First (TDD Workflow)**:
    -   The core logic **MUST** be testable without Unity.
    -   Avoid `MonoBehaviour` for game logic. Use it only for View/Input.
    -   **Rule of Thumb**: If you can't write a unit test for it in a separate C# console project, it belongs in the View layer, not the Logic layer.
    -   **TDD Mandate**: Every implementation MUST be followed by its corresponding test.
        -   Write the test immediately after (or before) the implementation.
        -   No code is considered "done" until its test passes.
        -   Tests verify both the happy path and edge cases.
        -   Run `dotnet test` after each implementation to ensure all tests pass.

2.  **Everything is an Action (The Queue Pattern)**:
    -   Complex systems (Combat, Game Loop) must use an **Action Queue**.
    -   Do not write spaghetti code where Method A calls Method B calls Animation C.
    -   Instead, Method A should **Enqueue** an Action. The Queue processes it.
    -   **Action Structure**:
        -   `ExecuteLogic()`: Updates data (Instant).
        -   `ExecuteVisual()`: Updates UI/View (Async).

3.  **Input Symmetry (Engine Agnosticism)**:
    -   The Game Logic **MUST NOT** know if a participant is a Human or an AI.
    -   Never write `if (isPlayer)`.
    -   Use the **Provider Pattern** (`IActionProvider`).

4.  **Composition over Inheritance (Content Rule)**:
    -   **NEVER** create a class for a specific piece of content (e.g., `class Ember`).
    -   Use **Data Composition**. Define a generic `MoveData` and compose its behavior using a list of **Effects**.

5.  **Registry Pattern (No Direct Loading)**:
    -   Game Logic **MUST NOT** load files directly (no `Resources.Load`, no `Addressables.Load` inside logic).
    -   Use **Registries** (`IDataRegistry<T>`) to access data by ID.

6.  **Battle State Abstraction (Slot System)**:
    -   **NEVER** reference "PlayerPokemon" or "EnemyPokemon" directly in the engine.
    -   Always refer to **Slots** (`BattleSlot`).
    -   This ensures the game supports 1v1, 2v2, 1v3, and Horde modes natively without code changes.

7.  **Pipeline Pattern (Complex Math)**:
    -   For complex calculations (Damage, Catch Rate), **NEVER** write a single giant function.
    -   Use a **Pipeline** of small, independent steps (`IDamageStep`).
    -   This allows easy modification (e.g., adding "Life Orb" logic) without breaking the core formula.

8.  **Event-Driven Extensions (Abilities & Items)**:
    -   For passive effects (Abilities, Items), use the **Event Trigger** pattern (`IBattleListener`).
    -   Never check `if (hasLeftovers)` in the engine. Instead, the Item **listens** to `OnTurnEnd` and returns Actions.
    -   This keeps the engine clean and allows infinite Abilities/Items without modifying core code.

9.  **Simplicity & Readability**:
    -   Code should be self-explanatory.
    -   Prefer duplication over complex abstractions if the abstraction reduces readability.

10. **Modularity**:
    -   Systems should be loosely coupled.
    -   Use Interfaces to define boundaries.

11. **Documentation Comments**:
    -   Every class and interface **MUST** have a simple, direct comment at the top explaining its purpose.
    -   Keep it short. Example: `// Defines the blueprint for a Pokemon species (immutable data).`

## 2. Game Design Pillars
*Key constraints defining the game's scope.*

-   **Combat Focused**: The entire game revolves around the combat loop.
-   **Pure RNG Progression**:
    -   No map screen.
    -   No path selection.
    -   Biomes, encounters, and events are strictly random.
-   **Roguelike Structure**:
    -   Start with starter Pokemon.
    -   Progress through increasingly difficult fights.
    -   Run ends on defeat or final boss victory.

## 3. Technical Architecture
-   **Engine**: Unity (2022.3+ LTS recommended).
-   **Language**: C#.
-   **Data Layer (The Blueprint Pattern)**:
    -   **Blueprints (Immutable)**: `SpeciesData`, `MoveData`. Loaded from ScriptableObjects. Never modified at runtime.
    -   **Instances (Mutable)**: `PokemonInstance`, `MoveInstance`. Created from Blueprints. Holds state.
    -   **Factories**: Use `PokemonFactory.Create(species, level)` to bridge the two.
-   **Editor Tooling**:
    -   Prefer **Auto-Population** over manual list management.
    -   Create Editor scripts to find and register assets automatically.

## 4. Documentation Standard
-   Every major system must have a corresponding Specification Document in `docs/architecture/`.
-   Every major flow must have a Mermaid diagram.

## 5. Core Systems Reference
*Quick reference to the main combat systems. See individual specs for details.*

### Data & Loading
-   **Pokemon Data** (`pokemon_data_detailed.md`): Blueprints (Species) vs Instances, Factory pattern.
-   **Move System** (`move_system_detailed.md`): Composable effects (`IMoveEffect`), no content-specific classes.
-   **Data Loading** (`data_loading_system.md`): Registry pattern (`IDataRegistry`), auto-population.
-   **Catalogs** (`catalogs_system.md`): Static data definitions (`PokemonCatalog`, `MoveCatalog`), bulk registration.

### Combat Flow
-   **Action Queue** (`combat_system_spec.md`): Sequential processing of `BattleAction`s (Logic + Visual).
-   **Turn Order** (`turn_order_system.md`): Priority → Speed → Random. Handles stat stages, paralysis, Trick Room.
-   **Battle Field** (`battle_field_system.md`): Slot-based architecture supporting N-vs-M battles.
-   **Victory/Defeat** (`victory_defeat_system.md`): Arbiter service, BattleResult data, Post-battle rewards (EXP/Loot).

### Calculations & Effects
-   **Damage Pipeline** (`damage_and_effect_system.md`): Modular `IDamageStep` chain for complex formulas.
-   **Status & Stats** (`status_and_stat_system.md`): Persistent/Volatile status, Stat stages (-6 to +6), Side effects.
-   **Targeting** (`targeting_system.md`): `TargetScope` + `TargetResolver`, handles redirection (Follow Me).

### Extensions & AI
-   **Abilities & Items** (`abilities_items_system.md`): Event-driven (`IBattleListener`), trigger-based actions.
-   **Player/AI Input** (`player_ai_spec.md`): Symmetrical via `IActionProvider`, supports Autoplay.
-   **UI Presentation** (`ui_presentation_system.md`): Decoupled View (`IBattleView`) and Logic (`MessageAction`), localization support.

### Reference Documents
-   **Battle Mechanics Catalog** (`battle_mechanics_catalog.md`): Comprehensive list of all BattleActions and IMoveEffects (40+ effects, 25+ actions) with implementation examples and priority ordering.

## 6. Testing Strategy
*How to ensure correctness for both Logic and Content.*

### 1. Logic Tests (TDD Required)
Test the **Systems**, not the instances.
-   **Unit Tests**: Verify `DamagePipeline`, `TurnOrderResolver`, and individual `IMoveEffect` classes.
    -   *Example*: "Does `BurnEffect` reduce HP by 1/16?" (Test the class, not the move "Will-O-Wisp").
-   **Mocking**: Use `MockBattleView` and `MockActionProvider` to isolate the engine.

### 2. Data Validation (Content Integrity)
Do not write unit tests for every Pokémon. Instead, write **Validators** that run on all data.
-   **Asset Validation**: Iterate all `SpeciesData` and `MoveData` in the project.
    -   *Check*: "Does every Move have a valid Description?"
    -   *Check*: "Does every Pokemon have Base Stats > 0?"

### 3. Content State Verification (Specific Cases)
For critical content (e.g., "Ember"), verify the **Composition**, not the Execution.
Since `DamageEffect` and `BurnEffect` are already tested via TDD (Logic Tests), you only need to prove that "Ember" *has* those effects.
-   *Test*: Load "Ember". Assert `Type == Fire`. Assert `Effects` contains `DamageEffect`. Assert `Effects` contains `BurnEffect`.
-   **Benefit**: This proves "Ember works" without simulating a full battle engine for every single move.

### 4. Integration Simulations (Bot Battles)
To catch edge cases, run automated battles between random AI bots.
-   **Headless Mode**: Run the `CombatEngine` at 100x speed without rendering.
-   **Fuzz Testing**: Run 10,000 random turns. If an exception is thrown, log the state (Seed, Actions) to reproduce it.
