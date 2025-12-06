# Feature 1: Game Data - Architecture

> Complete technical specification of all game data structures (blueprints) and supporting systems.

**Feature Number**: 1  
**Feature Name**: Game Data (formerly "Pokemon Data")  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document defines **all game data structures** (blueprints) and supporting systems, organized into logical groups:

-   **Core Entity Data**: Pokemon, Moves, Abilities, Items
-   **Field & Status Data**: Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects
-   **Supporting Systems**: Evolution System, Type Effectiveness Table
-   **Infrastructure**: Interfaces, Enums, Constants, Builders, Factories, Registries

It follows the **Testability First** guideline by separating the Data (POCO) from the Unity Asset (ScriptableObject).

## Structure Overview (Organized by Groups)

### Grupo A: Core Entity Data (1.1-1.4)

**Entidades principales del juego**

-   **1.1: Pokemon Data** - PokemonSpeciesData (Blueprint), PokemonInstance (Runtime), BaseStats, LearnableMove
-   **1.2: Move Data** - MoveData (Blueprint), MoveInstance (Runtime), Move Effects (22 implementations)
-   **1.3: Ability Data** - AbilityData (Blueprint)
-   **1.4: Item Data** - ItemData (Blueprint)

### Grupo B: Field & Status Data (1.5-1.10)

**Condiciones de campo y estado**

-   **1.5: Status Effect Data** - StatusEffectData (Blueprint)
-   **1.6: Weather Data** - WeatherData (Blueprint)
-   **1.7: Terrain Data** - TerrainData (Blueprint)
-   **1.8: Hazard Data** - HazardData (Blueprint)
-   **1.9: Side Condition Data** - SideConditionData (Blueprint)
-   **1.10: Field Effect Data** - FieldEffectData (Blueprint)

### Grupo C: Supporting Systems (1.11-1.12)

**Sistemas que soportan los datos**

-   **1.11: Evolution System** - Evolution, IEvolutionCondition, EvolutionConditions (6 classes)
-   **1.12: Type Effectiveness Table** - TypeEffectiveness (data table)

### Grupo D: Infrastructure (1.13-1.16)

**Infraestructura para crear y gestionar datos**

-   **1.13: Interfaces Base** - IIdentifiable
-   **1.14: Enums & Constants** - Enums (20 main + 7 in Effects), ErrorMessages, GameMessages, NatureData
-   ⚠️ **Builders**: Moved to **[Feature 3.9: Builders](../3-content-expansion/3.9-builders/)** - Used primarily for content creation
-   **1.16: Factories & Calculators** - StatCalculator, PokemonFactory, PokemonInstanceBuilder
-   **1.17: Registry System** - IDataRegistry<T>, GameDataRegistry<T>, PokemonRegistry, MoveRegistry

### Grupo E: Planned Features (1.18-1.19)

**Features futuras**

-   **1.18: Variants System** - Mega/Dinamax/Terracristalización (Planned)
-   **1.19: Pokedex Fields** - Description, Category, Height, Weight, Color, Shape, Habitat ✅ Complete

---

## 1. Core Entity Data

### 1.1: Pokemon Data ✅ IMPLEMENTED

## 2. Static Data (The "Species") ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Blueprints`_

This represents the "Blueprint" of a Pokemon (e.g., "Charizard"). It is immutable during gameplay.

### Current Implementation

```csharp
public class PokemonSpeciesData : IIdentifiable {
    // Identity
    public string Name { get; set; }        // "Charizard"
    public int PokedexNumber { get; set; }  // 6
    public string Id => Name;               // IIdentifiable

    // Types (Primary is required, Secondary is optional)
    public PokemonType PrimaryType { get; set; }   // Fire
    public PokemonType? SecondaryType { get; set; } // Flying (nullable)
    public bool IsDualType => SecondaryType.HasValue;
    public bool HasType(PokemonType type);  // Helper method

    // Base Stats
    public BaseStats BaseStats { get; set; }

    // Gender Ratio
    public float GenderRatio { get; set; }  // 0-100 = % male, -1 = genderless
    public bool IsGenderless { get; }       // GenderRatio < 0
    public bool IsMaleOnly { get; }         // GenderRatio >= 100
    public bool IsFemaleOnly { get; }       // GenderRatio == 0
    public bool HasBothGenders { get; }     // 0 < GenderRatio < 100

    // Learnset (moves this species can learn)
    public List<LearnableMove> Learnset { get; set; }

    // Evolution paths
    public List<Evolution> Evolutions { get; set; }
    public bool CanEvolve => Evolutions.Count > 0;

    // Learnset helpers
    IEnumerable<LearnableMove> GetStartingMoves();
    IEnumerable<LearnableMove> GetMovesAtLevel(int level);
    IEnumerable<LearnableMove> GetMovesUpToLevel(int level);
    bool CanLearn(MoveData move);
}

public class BaseStats {
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpAttack { get; set; }
    public int SpDefense { get; set; }
    public int Speed { get; set; }
    public int Total { get; }  // Sum of all stats (BST)

    public BaseStats(int hp, int attack, int defense, int spAttack, int spDefense, int speed);
}
```

### Gender System ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Enums`_

```csharp
public enum Gender {
    Male,
    Female,
    Genderless
}
```

**GenderRatio** in PokemonSpeciesData defines the probability:

-   `50.0f` = 50% male, 50% female (most Pokemon)
-   `87.5f` = 87.5% male (starters like Charmander)
-   `100.0f` = male only (Tauros)
-   `0.0f` = female only (Chansey)
-   `-1.0f` = genderless (Magnemite, Ditto)

### Learnset System ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Blueprints`_

```csharp
public class LearnableMove {
    public MoveData Move { get; set; }      // Reference to the move
    public LearnMethod Method { get; set; } // How it's learned
    public int Level { get; set; }          // Level (for LevelUp)
}

public enum LearnMethod {
    Start,      // Available at level 1
    LevelUp,    // Learned at a specific level
    Evolution,  // Learned upon evolving
    TM,         // Learned via TM/HM
    Egg,        // Inherited from parents
    Tutor       // Taught by move tutor
}
```

### Evolution System ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Evolution` and `PokemonUltimate.Core.Evolution.Conditions`_

```csharp
public class Evolution {
    public PokemonSpeciesData Target { get; set; }  // What it evolves into
    public List<IEvolutionCondition> Conditions { get; set; }  // All must be met
    public string Description { get; }  // Human-readable

    bool HasCondition<T>() where T : IEvolutionCondition;
    T GetCondition<T>() where T : class, IEvolutionCondition;
    bool CanEvolve(PokemonInstance pokemon);  // Check if Pokemon can evolve
}

public interface IEvolutionCondition {
    EvolutionConditionType ConditionType { get; }
    string Description { get; }
    bool IsMet(PokemonInstance pokemon);  // Check if condition is met
}

// Available conditions:
// - LevelCondition(minLevel) - checks pokemon.Level >= minLevel
// - ItemCondition(itemName) - always returns false (needs explicit item use)
// - FriendshipCondition(minFriendship = 220) - checks pokemon.Friendship >= minFriendship
// - TimeOfDayCondition(timeOfDay) - always returns false (needs time context)
// - TradeCondition() - always returns false (needs explicit trade)
// - KnowsMoveCondition(move) - checks if pokemon knows the move
```

## 3. Pokemon Builder (Fluent API) ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Content.Builders`_

The builder pattern provides a clean, readable way to define Pokemon data:

```csharp
// Example: Full Charmander definition
public static readonly PokemonSpeciesData Charmander = Pokemon.Define("Charmander", 4)
    .Type(PokemonType.Fire)
    .Stats(39, 52, 43, 60, 50, 65)
    .GenderRatio(87.5f)
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
        .AtLevel(9, MoveCatalog.Ember)
        .AtLevel(38, MoveCatalog.Flamethrower)
        .ByTM(MoveCatalog.FireBlast))
    .EvolvesTo(Charmeleon, e => e.AtLevel(16))
    .Build();

// Dual-type Pokemon
public static readonly PokemonSpeciesData Charizard = Pokemon.Define("Charizard", 6)
    .Types(PokemonType.Fire, PokemonType.Flying)
    .Stats(78, 84, 78, 109, 85, 100)
    .Build();

// Stone evolution
public static readonly PokemonSpeciesData Pikachu = Pokemon.Define("Pikachu", 25)
    .Type(PokemonType.Electric)
    .Stats(35, 55, 40, 50, 50, 90)
    .EvolvesTo(Raichu, e => e.WithItem("Thunder Stone"))
    .Build();

// Complex evolution (Espeon: Friendship + Daytime)
public static readonly PokemonSpeciesData Eevee = Pokemon.Define("Eevee", 133)
    .EvolvesTo(Espeon, e => e
        .WithFriendship()
        .During(TimeOfDay.Day))
    .Build();
```

### Builder API Reference

| Method                                | Description         | Example                          |
| ------------------------------------- | ------------------- | -------------------------------- |
| `Pokemon.Define(name, dex)`           | Start definition    | `Pokemon.Define("Pikachu", 25)`  |
| `.Type(type)`                         | Set mono-type       | `.Type(PokemonType.Fire)`        |
| `.Types(primary, secondary)`          | Set dual-type       | `.Types(Fire, Flying)`           |
| `.Stats(hp, atk, def, spa, spd, spe)` | Set base stats      | `.Stats(39, 52, 43, 60, 50, 65)` |
| `.GenderRatio(percent)`               | Set male percentage | `.GenderRatio(87.5f)`            |
| `.Genderless()`                       | Mark as genderless  | `.Genderless()`                  |
| `.MaleOnly()`                         | Mark as male only   | `.MaleOnly()`                    |
| `.FemaleOnly()`                       | Mark as female only | `.FemaleOnly()`                  |
| `.Moves(m => ...)`                    | Configure learnset  | See below                        |
| `.EvolvesTo(target, e => ...)`        | Add evolution       | See below                        |
| `.Build()`                            | Finalize            | Required at end                  |

### Learnset Builder API

| Method                      | Description     | Example                               |
| --------------------------- | --------------- | ------------------------------------- |
| `.StartsWith(moves...)`     | Level 1 moves   | `.StartsWith(MoveCatalog.Tackle)`     |
| `.AtLevel(level, moves...)` | Level-up moves  | `.AtLevel(15, MoveCatalog.Ember)`     |
| `.ByTM(moves...)`           | TM/HM moves     | `.ByTM(MoveCatalog.Earthquake)`       |
| `.ByEgg(moves...)`          | Egg moves       | `.ByEgg(MoveCatalog.DragonRush)`      |
| `.OnEvolution(moves...)`    | Evolution moves | `.OnEvolution(MoveCatalog.HyperBeam)` |
| `.ByTutor(moves...)`        | Tutor moves     | `.ByTutor(MoveCatalog.Outrage)`       |

### Evolution Builder API

| Method                       | Description       | Example                           |
| ---------------------------- | ----------------- | --------------------------------- |
| `.AtLevel(level)`            | Level requirement | `.AtLevel(16)`                    |
| `.WithItem(itemName)`        | Use item          | `.WithItem("Fire Stone")`         |
| `.WithFriendship(min = 220)` | Friendship        | `.WithFriendship()`               |
| `.During(time)`              | Time of day       | `.During(TimeOfDay.Night)`        |
| `.ByTrade()`                 | Trade evolution   | `.ByTrade()`                      |
| `.KnowsMove(move)`           | Must know move    | `.KnowsMove(MoveCatalog.Rollout)` |

## 4. Dynamic Data (The "Instance") ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Instances`_

This represents a specific Pokemon in a specific battle. It is mutable.

### File Organization (Partial Classes)

PokemonInstance is split into multiple files for maintainability:

```
PokemonInstance.cs           → Core properties, constructor (277 lines)
PokemonInstance.Battle.cs    → Battle methods, HP, status, stats (199 lines)
PokemonInstance.LevelUp.cs   → Level up, experience, move learning (293 lines)
PokemonInstance.Evolution.cs → Evolution queries and execution (227 lines)
```

**Total: ~996 lines** (down from 915 in single file due to added documentation)

### MoveInstance (PP Tracking)

```csharp
public class MoveInstance {
    // Reference to blueprint
    public MoveData Move { get; }

    // PP tracking
    public int MaxPP { get; }
    public int CurrentPP { get; private set; }

    // Helpers
    public bool HasPP => CurrentPP > 0;

    // Methods
    public bool Use();           // Returns false if no PP
    public void Restore(int amount);
    public void RestoreFully();
}
```

### PokemonInstance (Runtime State)

```csharp
public class PokemonInstance {
    // Identity
    public PokemonSpeciesData Species { get; private set; }  // Mutable for evolution
    public string InstanceId { get; }        // GUID
    public string Nickname { get; set; }
    public string DisplayName { get; }       // Nickname ?? Species.Name

    // Level & Experience
    public int Level { get; private set; }   // Mutable for level up
    public int CurrentExp { get; set; }

    // Calculated Stats (recalculated on level up/evolution)
    public int MaxHP { get; private set; }
    public int CurrentHP { get; set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int SpAttack { get; private set; }
    public int SpDefense { get; private set; }
    public int Speed { get; private set; }

    // Personal characteristics
    public Gender Gender { get; }
    public Nature Nature { get; }
    public int Friendship { get; set; }      // 0-255, default 70
    public bool IsShiny { get; }             // 1/4096 natural odds

    // Combat State
    public List<MoveInstance> Moves { get; }
    public PersistentStatus Status { get; set; }
    public VolatileStatus VolatileStatus { get; set; }
    public int StatusTurnCounter { get; set; }
    public Dictionary<Stat, int> StatStages { get; }  // -6 to +6

    // Computed Properties
    public bool IsFainted { get; }
    public float HPPercentage { get; }
    public bool HasStatus { get; }
    public bool HasHighFriendship { get; }   // >= 220
    public bool HasMaxFriendship { get; }    // >= 255

    // Battle Methods
    public int GetEffectiveStat(Stat stat);
    public int ModifyStatStage(Stat stat, int change);
    public void ResetBattleState();
    public void FullHeal();
    public int TakeDamage(int amount);
    public int Heal(int amount);
    public int IncreaseFriendship(int amount);
    public int DecreaseFriendship(int amount);

    // Level Up Methods ✅ NEW
    public bool CanLevelUp();                // Checks exp vs next level
    public int GetExpForNextLevel();         // Exp needed for next level
    public int GetExpToNextLevel();          // Remaining exp needed
    public int AddExperience(int amount);    // Returns levels gained
    public bool LevelUp();                   // Force level up
    public int LevelUpTo(int targetLevel);   // Level up to target
    public List<MoveData> TryLearnLevelUpMoves();  // Learn available moves
    public bool TryLearnMove(MoveData move); // Learn single move
    public bool ReplaceMove(int index, MoveData newMove);
    public bool ForgetMove(int index);

    // Evolution Methods ✅ NEW
    public bool CanEvolve();                 // Checks all evolution paths
    public Evolution GetAvailableEvolution();        // First available
    public List<Evolution> GetAvailableEvolutions(); // All available
    public bool Evolve(PokemonSpeciesData target);   // Execute evolution
    public PokemonSpeciesData TryEvolve();   // Auto-evolve if possible
}
```

### Stat Stages Multiplier Table

| Stage | Multiplier |
| ----- | ---------- |
| -6    | 2/8 = 0.25 |
| -5    | 2/7 = 0.29 |
| -4    | 2/6 = 0.33 |
| -3    | 2/5 = 0.40 |
| -2    | 2/4 = 0.50 |
| -1    | 2/3 = 0.67 |
| 0     | 2/2 = 1.00 |
| +1    | 3/2 = 1.50 |
| +2    | 4/2 = 2.00 |
| +3    | 5/2 = 2.50 |
| +4    | 6/2 = 3.00 |
| +5    | 7/2 = 3.50 |
| +6    | 8/2 = 4.00 |

Formula: `(2 + stage) / 2` for positive, `2 / (2 + |stage|)` for negative

## 4.5. Enums, Constants & Data Tables ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Enums`, `PokemonUltimate.Core.Constants`, `PokemonUltimate.Core.Blueprints`_

### NatureData (Nature Modifier Tables) ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Blueprints`_

Static class providing nature modifier tables for stat calculations. Natures affect stat calculation: +10% to one stat, -10% to another.

```csharp
public static class NatureData {
    const float BoostMultiplier = 1.1f;      // +10% for boosted stat
    const float ReduceMultiplier = 0.9f;     // -10% for reduced stat
    const float NeutralMultiplier = 1.0f;     // No change

    Stat? GetIncreasedStat(Nature nature);   // Which stat is boosted
    Stat? GetDecreasedStat(Nature nature);   // Which stat is reduced
    bool IsNeutral(Nature nature);           // True if no stat changes
    float GetStatMultiplier(Nature nature, Stat stat);  // Get multiplier for stat
}
```

**Nature Enum** (from Sub-Feature 1.14: Enums & Constants):

```csharp
public enum Nature {
    // Neutral (5): Hardy, Docile, Serious, Bashful, Quirky
    // Attack+: Lonely, Brave, Adamant, Naughty
    // Defense+: Bold, Relaxed, Impish, Lax
    // Speed+: Timid, Hasty, Jolly, Naive
    // SpAttack+: Modest, Mild, Quiet, Rash
    // SpDefense+: Calm, Gentle, Sassy, Careful
}
```

**Usage in stat calculation:**

```csharp
int baseStat = species.BaseStats.Attack;
float natureMultiplier = NatureData.GetStatMultiplier(nature, Stat.Attack);
int finalStat = (int)(calculatedStat * natureMultiplier);
```

---

## 5. The Factory (Fluent Builder) ✅ IMPLEMENTED

_Namespace: `PokemonUltimate.Core.Factories`_

Pokemon instances are created using a fluent builder pattern for maximum flexibility.

### StatCalculator (Stat Formulas) ✅ UPDATED

```csharp
public static class StatCalculator {
    // Constants for IVs/EVs (roguelike always uses max)
    public const int MaxIV = 31;
    public const int MaxEV = 252;
    public const int DefaultIV = MaxIV;
    public const int DefaultEV = MaxEV;

    // Full Gen3+ formula with max IVs/EVs:
    // HP: floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
    // Other: floor((floor((2 * Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)

    public static int CalculateHP(int baseHP, int level);
    public static int CalculateHP(int baseHP, int level, int iv, int ev);  // For testing

    public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat);
    public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat, int iv, int ev);

    // Battle calculations
    public static float GetStageMultiplier(int stage);          // Battle stat stages
    public static float GetAccuracyStageMultiplier(int stage);  // Accuracy/Evasion
    public static int GetEffectiveStat(int calculatedStat, int stage);

    // Experience calculations ✅ NEW
    public static int GetExpForLevel(int level);      // Total exp at level
    public static int GetExpToNextLevel(int level);   // Exp to next level
    public static int GetLevelForExp(int totalExp);   // Level from total exp
}
```

**Stat Comparison with IVs/EVs:**
| Base 100, Level 50 | Without (IV=0, EV=0) | With Max (IV=31, EV=252) |
|--------------------|----------------------|--------------------------|
| HP | 160 | 207 |
| Other Stat | 105 | 152 |

**Note**: StatCalculator uses `NatureData` (from Sub-Feature 1.14: Enums & Constants) to apply nature modifiers.

### PokemonInstanceBuilder (Fluent API)

```csharp
// Basic creation
var pokemon = Pokemon.Create(species, level).Build();
var pokemon = Pokemon.Random(species, level);  // Shortcut

// Full example with all options
var pokemon = Pokemon.Create(PokemonCatalog.Pikachu, 50)
    // Nature
    .WithNature(Nature.Jolly)           // Specific nature
    .WithNatureBoosting(Stat.Speed)     // Auto-select (Jolly)
    .WithNeutralNature()                // Hardy

    // Gender
    .WithGender(Gender.Female)          // Specific
    .Male()                             // Shortcut
    .Female()                           // Shortcut

    // Nickname
    .Named("Sparky")

    // Moves
    .WithMoves(move1, move2, move3)     // Specific moves
    .WithLearnsetMoves()                // Auto-select from learnset (default)
    .WithRandomMoves()                  // Random from learnset
    .WithStabMoves()                    // Prioritize STAB moves ✅ NEW
    .WithStrongMoves()                  // Prioritize high-power moves ✅ NEW
    .WithOptimalMoves()                 // STAB + power combined ✅ NEW
    .WithMoveCount(2)                   // Limit count
    .WithSingleMove()                   // Only 1 move
    .WithNoMoves()                      // Empty moveset

    // HP
    .AtFullHealth()                     // 100% (default)
    .AtHealth(50)                       // Specific HP
    .AtHealthPercent(0.5f)              // Percentage
    .AtHalfHealth()                     // 50%
    .AtOneHP()                          // Clutch scenario
    .Fainted()                          // 0 HP

    // Status
    .WithStatus(PersistentStatus.Burn)
    .Burned() / .Paralyzed() / .Poisoned() / .BadlyPoisoned() / .Asleep() / .Frozen()

    // Friendship
    .WithFriendship(150)                // Specific value (0-255)
    .WithHighFriendship()               // 220 (evolution threshold)
    .WithMaxFriendship()                // 255
    .WithLowFriendship()                // 0
    .AsHatched()                        // 120 (hatched from egg)

    // Shiny
    .Shiny()                            // Force shiny
    .NotShiny()                         // Force not shiny
    .WithShinyChance()                  // Roll 1/4096

    // Experience
    .WithExperience(5000)

    // Stat overrides (testing)
    .WithStats(200, 150, 120, 130, 110, 140)
    .WithMaxHP(500)
    .WithAttack(999)
    .WithSpeed(200)

    .Build();
```

### Move Selection Presets ✅ NEW

The builder offers intelligent move selection:

| Method                 | Description                  | Priority                |
| ---------------------- | ---------------------------- | ----------------------- |
| `.WithLearnsetMoves()` | Default: highest level first | Level descending        |
| `.WithRandomMoves()`   | Random selection             | Random                  |
| `.WithStabMoves()`     | Same Type Attack Bonus       | Type match (+100 score) |
| `.WithStrongMoves()`   | High base power              | Power value             |
| `.WithOptimalMoves()`  | Best competitive set         | STAB + Power combined   |

**Score Calculation for Smart Moves:**

-   STAB bonus: +100 if move type matches Pokemon type
-   Power: Direct power value (0-150+)
-   Accuracy: +Accuracy/10 (minor bonus)
-   Damaging bonus: +50 for non-Status moves

### Quick Factory Methods

```csharp
// Simple creation (delegates to builder)
var pokemon = PokemonFactory.Create(species, level);
var pokemon = PokemonFactory.Create(species, level, nature);
var pokemon = PokemonFactory.Create(species, level, nature, gender);

// Level range (for wild encounters)
var wild = Pokemon.CreateInLevelRange(species, 15, 20).Build();
```

### Usage Examples

```csharp
// Competitive Pokemon with optimal moveset
var pikachu = Pokemon.Create(PokemonCatalog.Pikachu, 50)
    .WithNatureBoosting(Stat.Speed)
    .WithOptimalMoves()
    .Named("Bolt")
    .Build();

// Wild encounter with random moves
var wildPokemon = Pokemon.CreateInLevelRange(species, 15, 20)
    .WithRandomMoves()
    .AtHealthPercent(0.7f)
    .Build();

// Shiny hunting result
var shiny = Pokemon.Create(species, 25)
    .Shiny()
    .Named("★Lucky★")
    .Build();

// Friendship evolution ready
var eevee = Pokemon.Create(PokemonCatalog.Eevee, 20)
    .WithHighFriendship()
    .Build();
// eevee.HasHighFriendship == true
// eevee.CanEvolve() == true (if Espeon/Umbreon defined with friendship condition)

// Level up example
pokemon.AddExperience(1000);      // Returns levels gained
pokemon.TryLearnLevelUpMoves();   // Learn available moves

// Evolution example
if (pokemon.CanEvolve()) {
    var evolved = pokemon.TryEvolve();  // Returns new species
}

// For testing
var testMon = Pokemon.Create(species, 50)
    .WithNeutralNature()
    .WithStats(200, 100, 100, 100, 100, 100)
    .AtHalfHealth()
    .Burned()
    .Build();
```

## 6. Unity Integration (ScriptableObjects) ⏳ FUTURE

_Namespace: `PokemonGame.Unity.Data`_

To make it "easy to add more", we use Unity's Inspector.

### `PokemonSpeciesSO`

-   **Menu**: `Create > Pokemon > New Species`
-   **Inspector Fields**:
    -   `Name` (String)
    -   `Types` (List<Type>)
    -   `Base Stats` (Struct with 6 int fields)
    -   `Sprite` (Sprite) -> _Note: The `ToPOCO` method will ignore this or convert to path ID_
    -   `Moves` (List of MoveSO + Level)

### `ToPOCO()` Implementation

```csharp
public PokemonSpeciesData ToPOCO() {
    return new PokemonSpeciesData {
        Id = this.name.ToLower(), // Use asset filename as ID
        Name = this.speciesName,
        BaseStats = new BaseStats(this.hp, this.atk, this.def, this.spa, this.spd, this.spe),
        // ...
    };
}
```

## 7. Workflow: Adding a New Pokemon

### Option A: Code (Recommended for now)

1. Open `PokemonCatalog.Gen1.cs` (or appropriate generation file).
2. Add the Pokemon using the builder pattern.
3. **Important**: Define evolutions in reverse order (final form first).
4. Register in `RegisterGen1()` method.
5. Run tests to verify.

```csharp
// Example: Adding Pidgey line
public static readonly PokemonSpeciesData Pidgeot = Pokemon.Define("Pidgeot", 18)
    .Types(PokemonType.Normal, PokemonType.Flying)
    .Stats(83, 80, 75, 70, 70, 101)
    .Build();

public static readonly PokemonSpeciesData Pidgeotto = Pokemon.Define("Pidgeotto", 17)
    .Types(PokemonType.Normal, PokemonType.Flying)
    .Stats(63, 60, 55, 50, 50, 71)
    .EvolvesTo(Pidgeot, e => e.AtLevel(36))
    .Build();

public static readonly PokemonSpeciesData Pidgey = Pokemon.Define("Pidgey", 16)
    .Types(PokemonType.Normal, PokemonType.Flying)
    .Stats(40, 45, 40, 35, 35, 56)
    .EvolvesTo(Pidgeotto, e => e.AtLevel(18))
    .Build();
```

### Option B: Unity Inspector (Future)

1. Right Click in Project -> `Create > Pokemon > New Species`.
2. Name it "Bulbasaur".
3. Fill in Stats (45, 49, 49...).
4. Drag and Drop Move assets into the "Moves" list.
5. **Done.** The game automatically loads it via `GameDatabaseSO`.

---

## 8. Variants System: Mega, Dinamax, and Terracristalización ⏳ PLANNED

**See**: [`1.3-variants-system/architecture.md`](1.3-variants-system/architecture.md) for complete specification.

### Overview

Mega Evolutions, Dinamax, and Terracristalización are implemented as **separate Pokemon species** with their own `PokemonSpeciesData` entries, not as temporary transformations during battle.

### Key Design Decisions

-   **Variants as Separate Species**: Each variant (Mega, Dinamax, Tera) is a distinct Pokemon with its own stats, types, and abilities
-   **Shared Pokedex Numbers**: Variants share the same Pokedex number as their base form (unique identification via `Name`)
-   **Learnset Inheritance**: Variants inherit moves from base form by default, can override specific moves
-   **Builder Support**: Special builder methods (`.AsMegaVariant()`, `.AsDinamaxVariant()`, `.AsTeraVariant()`)

### Variant Types

1. **Mega Evolutions**: Higher BST, may change types, unique abilities

    - Example: `Mega Charizard X` (Fire/Dragon, Tough Claws)
    - Example: `Mega Charizard Y` (Fire/Flying, Drought)

2. **Dinamax**: 2x HP, same stats otherwise

    - Example: `Charizard Dinamax` (156 HP vs 78 base)
    - Example: `Charizard Gigantamax` (special appearance + unique Max Move)

3. **Terracristalización**: Changes to mono-type (Tera type), same stats
    - Example: `Charizard Tera Fire` (mono-Fire)
    - Example: `Charizard Tera Dragon` (mono-Dragon)

### Required Fields

-   `BaseForm` (PokemonSpeciesData?): Reference to base Pokemon
-   `VariantType` (PokemonVariantType): Type of variant (Mega, Dinamax, Tera)
-   `TeraType` (PokemonType?): Tera type for Terracristalización variants
-   `Variants` (List<PokemonSpeciesData>): List of all variant forms

### Example Usage

```csharp
// Base form
var Charizard = Pokemon.Define("Charizard", 6)
    .Types(PokemonType.Fire, PokemonType.Flying)
    .Stats(78, 84, 78, 109, 85, 100)
    .Build();

// Mega Charizard X
var MegaCharizardX = Pokemon.Define("Mega Charizard X", 6)
    .Types(PokemonType.Fire, PokemonType.Dragon)  // Type change!
    .Stats(78, 130, 111, 130, 85, 100)  // Higher BST
    .Ability(AbilityCatalog.ToughClaws)
    .AsMegaVariant(Charizard, "X")
    .Build();

// Charizard Dinamax
var CharizardDinamax = Pokemon.Define("Charizard Dinamax", 6)
    .Stats(156, 84, 78, 109, 85, 100)  // HP doubled
    .AsDinamaxVariant(Charizard)
    .Build();

// Charizard Tera Fire
var CharizardTeraFire = Pokemon.Define("Charizard Tera Fire", 6)
    .Type(PokemonType.Fire)  // Mono-type
    .Stats(78, 84, 78, 109, 85, 100)  // Same stats
    .AsTeraVariant(Charizard, PokemonType.Fire)
    .Build();
```

**Status**: ⏳ **PLANNED** - See [`1.18-variants-system/architecture.md`](1.18-variants-system/architecture.md) for full specification.

---

## 9. Pokedex Fields ✅ COMPLETE

**See**: `docs/features/1-game-data/roadmap.md` Phase 2 for complete specification.

### Overview

Fields required for displaying Pokemon information in the Pokedex feature.

### Required Fields

-   **Description** (`string`): Pokedex entry text describing the Pokemon
-   **Category** (`string`): Classification (e.g., "Flame Pokemon", "Seed Pokemon")
-   **Height** (`float`): Height in meters
-   **Weight** (`float`): Weight in kilograms
-   **Color** (`PokemonColor` enum): Pokedex color category
-   **Shape** (`PokemonShape` enum): Body shape category
-   **Habitat** (`PokemonHabitat` enum): Preferred habitat/biome

### Example Usage

```csharp
var Charizard = Pokemon.Define("Charizard", 6)
    .Types(PokemonType.Fire, PokemonType.Flying)
    .Stats(78, 84, 78, 109, 85, 100)
    .Description("It can melt boulders. It causes forest fires by blowing flames.")
    .Category("Flame Pokemon")
    .Height(1.7f)  // 1.7 meters
    .Weight(90.5f)  // 90.5 kilograms
    .Color(PokemonColor.Red)
    .Shape(PokemonShape.Wings)
    .Habitat(PokemonHabitat.Mountain)
    .Build();
```

**Status**: ✅ **COMPLETE** - See `docs/features/1-game-data/roadmap.md` Phase 2 for implementation details. All fields implemented with PokedexDataProvider system.

---

## 5. Data Loading & Registry System

To ensure the game is **Testable** and **Modular**, the code never loads files directly (no `Resources.Load` inside game logic). Instead, it asks a **Registry** for data. This allows us to swap the "Real Unity Database" for a "Fake Test Database" instantly.

### 5.1 Registry Interfaces

_Namespace: `PokemonUltimate.Core.Registry`_

```csharp
public interface IDataRegistry<T> where T : IIdentifiable {
    // Registration
    void Register(T item);
    void RegisterAll(IEnumerable<T> items);

    // Retrieval
    T Get(string id);           // Throws if not found
    T GetById(string id);       // Returns null if not found
    bool TryGet(string id, out T item);
    IEnumerable<T> GetAll();
    IEnumerable<T> All { get; }

    // Queries
    bool Exists(string id);
    bool Contains(string id);
    int Count { get; }
}
```

### 5.5.2 Extended Registries

#### PokemonRegistry

```csharp
public class PokemonRegistry : GameDataRegistry<PokemonSpeciesData> {
    PokemonSpeciesData GetByPokedexNumber(int number);
    IEnumerable<PokemonSpeciesData> GetByType(PokemonType type);
    IEnumerable<PokemonSpeciesData> GetByPokedexRange(int start, int end);
    IEnumerable<PokemonSpeciesData> GetFinalForms();
}
```

#### MoveRegistry

```csharp
public class MoveRegistry : GameDataRegistry<MoveData> {
    IEnumerable<MoveData> GetByType(PokemonType type);
    IEnumerable<MoveData> GetByCategory(MoveCategory category);
    IEnumerable<MoveData> GetByMinPower(int minPower);
    IEnumerable<MoveData> GetByPriority(int priority);
    IEnumerable<MoveData> GetContactMoves();
}
```

### 5.5.3 Testability

For Unit Tests, we use direct Catalog access:

```csharp
[Test]
public void Test_Pokemon_Stats() {
    var pikachu = PokemonCatalog.Pikachu;
    Assert.That(pikachu.BaseStats.Speed, Is.EqualTo(90));
}
```

Or Registry with Catalog data:

```csharp
[Test]
public void Test_Combat_Start() {
    var registry = new PokemonRegistry();
    registry.RegisterAll(PokemonCatalog.All);
    var pikachu = registry.GetByPokedexNumber(25);
    Assert.That(pikachu.Name, Is.EqualTo("Pikachu"));
}
```

---

## 5.6. Data Layer Compatibility

### Overall Status: ✅ COMPATIBLE

All data layer blueprints are **well-designed** and **compatible** with the combat system architecture.

| System           | Compatibility | Notes                                |
| ---------------- | ------------- | ------------------------------------ |
| AbilityData      | ✅ 95%        | Missing terrain triggers (minor gap) |
| ItemData         | ✅ 100%       | Complete                             |
| StatusEffectData | ✅ 100%       | Complete                             |
| WeatherData      | ✅ 100%       | Complete                             |
| TerrainData      | ✅ 100%       | Complete                             |

### Trigger Mapping

All `AbilityTrigger` and `ItemTrigger` values map correctly to `BattleTrigger` enum. Minor gap: terrain-related triggers (`OnTerrainChange`, `OnTerrainTick`) should be added to `AbilityTrigger` for future terrain abilities.

### DamagePipeline Integration

All data systems integrate cleanly with the damage pipeline:

-   WeatherData: `GetTypePowerMultiplier()` ready
-   TerrainData: All methods ready
-   AbilityData: `Multiplier` property available
-   ItemData: `DamageMultiplier` property available

**Verdict**: Data layer is ready for combat system integration.

---

## 7. Architectural Patterns & Refactoring

> **Note**: A comprehensive refactoring was completed (2024-12-XX) following SOLID principles and clean code practices. See `PokemonUltimate.Core/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for complete details.

### 7.1 Dependency Injection

All major components now use dependency injection for improved testability and flexibility:

**Key Interfaces**:

-   `IRandomProvider` - Random number generation (replaces static `Random`)
-   `IStatCalculator` - Stat calculation interface
-   `ITypeEffectiveness` - Type effectiveness interface
-   `IMoveSelector` - Move selection interface
-   `IStatStageManager` - Stat stage management interface

**Example**:

```csharp
public class PokemonInstanceBuilder
{
    private readonly IRandomProvider _randomProvider;
    private readonly IStatCalculator _statCalculator;
    private readonly IMoveSelector _moveSelector;

    public PokemonInstanceBuilder(
        IRandomProvider randomProvider,
        IStatCalculator statCalculator,
        IMoveSelector moveSelector)
    {
        // Dependencies injected via constructor
    }
}
```

### 7.2 Strategy Pattern

Multiple areas use Strategy Pattern for extensibility:

**Strategy Implementations**:

-   **Nature Boosting**: `INatureBoostingStrategy` with implementations for each stat
-   **Stat Getters**: `IStatGetterStrategy` for `BaseStats` and `IPokemonStatGetterStrategy` for `PokemonInstance`
-   **Move Selection**: `IMoveSelectionStrategy` with RandomMoveStrategy, StabMoveStrategy, PowerMoveStrategy, OptimalMoveStrategy
-   **Effect Descriptions**: Strategy Pattern for effect description generation

**Example**:

```csharp
public class MoveSelector
{
    private readonly Dictionary<MoveSelectionType, IMoveSelectionStrategy> _strategies;

    public MoveInstance SelectMove(PokemonInstance pokemon, MoveSelectionType type)
    {
        var strategy = _strategies[type];
        return strategy.SelectMove(pokemon);
    }
}
```

### 7.3 Constants Centralization

Magic numbers and strings replaced with constants:

**Constant Classes**:

-   `CoreConstants` - Core module constants (ShinyOdds, Friendship values, IV/EV limits, Stat stages, Formula constants)
-   `CoreValidators` - Centralized validation methods
-   `ErrorMessages` - Error message constants
-   `GameMessages` - Game message constants

### 7.4 Extension Methods

Utility methods for common operations:

**Extensions**:

-   `LevelExtensions.IsValidLevel()` - Validates level (1-100)
-   `FriendshipExtensions.ClampFriendship()` - Clamps friendship (0-255)

### 7.5 Value Objects & Caching

Complex state encapsulated and cached for performance:

**Value Objects & Cache**:

-   `StatsCache` - Caches calculated stats for `PokemonInstance`
-   `StatStageManager` - Manages stat stage modifications

### 7.6 Refactoring Summary

**Completed Phases (0-8)**:

-   ✅ Dependency Injection throughout
-   ✅ Strategy Pattern for extensibility
-   ✅ Constants centralization
-   ✅ Extension methods
-   ✅ Validation centralization
-   ✅ Move selection system
-   ✅ Stat stage management
-   ✅ Stats caching

**Benefits**:

-   Improved testability (all dependencies injectable)
-   Better maintainability (clear separation of concerns)
-   Enhanced extensibility (Strategy Pattern)
-   Reduced coupling (DI)
-   Better code organization (Constants, Extensions, Managers)
-   Performance improvements (Stats caching)

---

## Related Documents

-   **[Use Cases](use_cases.md)** - Scenarios this architecture supports
-   **[Roadmap](roadmap.md)** - Implementation plan for all fields
-   **[Testing](testing.md)** - How to test this architecture
-   **[Code Location](code_location.md)** - Where this is implemented
-   **[Feature 2: Combat System](../2-combat-system/architecture.md)** - Uses Pokemon instances in battles
-   **[Feature 3: Content Expansion](../3-content-expansion/roadmap.md)** - Adding more Pokemon using this structure
-   **[Feature 5: Game Features](../5-game-features/roadmap.md)** - Catching, evolution, friendship systems

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-XX)
