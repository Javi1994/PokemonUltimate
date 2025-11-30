# Pokemon Data Structure & Factory Specification

## 1. Overview
This document defines exactly how a Pokemon is structured in data (Static) and in memory (Dynamic).
It follows the **Testability First** guideline by separating the Data (POCO) from the Unity Asset (ScriptableObject).

## 2. Static Data (The "Species") ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Models`*

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
*Namespace: `PokemonUltimate.Core.Enums`*

```csharp
public enum Gender {
    Male,
    Female,
    Genderless
}
```

**GenderRatio** in PokemonSpeciesData defines the probability:
- `50.0f` = 50% male, 50% female (most Pokemon)
- `87.5f` = 87.5% male (starters like Charmander)
- `100.0f` = male only (Tauros)
- `0.0f` = female only (Chansey)
- `-1.0f` = genderless (Magnemite, Ditto)

### Nature System ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Enums` and `PokemonUltimate.Core.Models`*

Natures affect stat calculation: +10% to one stat, -10% to another.

```csharp
public enum Nature {
    // Neutral (5): Hardy, Docile, Serious, Bashful, Quirky
    // Attack+: Lonely, Brave, Adamant, Naughty
    // Defense+: Bold, Relaxed, Impish, Lax
    // Speed+: Timid, Hasty, Jolly, Naive
    // SpAttack+: Modest, Mild, Quiet, Rash
    // SpDefense+: Calm, Gentle, Sassy, Careful
}

public static class NatureData {
    const float BoostMultiplier = 1.1f;
    const float ReduceMultiplier = 0.9f;
    
    Stat? GetIncreasedStat(Nature nature);
    Stat? GetDecreasedStat(Nature nature);
    bool IsNeutral(Nature nature);
    float GetStatMultiplier(Nature nature, Stat stat);
}
```

**Usage in stat calculation:**
```csharp
int baseStat = species.BaseStats.Attack;
float natureMultiplier = NatureData.GetStatMultiplier(nature, Stat.Attack);
int finalStat = (int)(calculatedStat * natureMultiplier);
```

### Learnset System ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Models`*

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
*Namespace: `PokemonUltimate.Core.Evolution` and `PokemonUltimate.Core.Evolution.Conditions`*

```csharp
public class Evolution {
    public PokemonSpeciesData Target { get; set; }  // What it evolves into
    public List<IEvolutionCondition> Conditions { get; set; }  // All must be met
    public string Description { get; }  // Human-readable
    
    bool HasCondition<T>() where T : IEvolutionCondition;
    T GetCondition<T>() where T : class, IEvolutionCondition;
}

public interface IEvolutionCondition {
    EvolutionConditionType ConditionType { get; }
    string Description { get; }
}

// Available conditions:
// - LevelCondition(minLevel)
// - ItemCondition(itemName)
// - FriendshipCondition(minFriendship = 220)
// - TimeOfDayCondition(timeOfDay)
// - TradeCondition()
// - KnowsMoveCondition(move)
```

## 3. Pokemon Builder (Fluent API) ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Content.Builders`*

The builder pattern provides a clean, readable way to define Pokemon data:

```csharp
// Example: Full Charmander definition
public static readonly PokemonSpeciesData Charmander = Pokemon.Define("Charmander", 4)
    .Type(PokemonType.Fire)
    .Stats(39, 52, 43, 60, 50, 65)
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

| Method | Description | Example |
|--------|-------------|---------|
| `Pokemon.Define(name, dex)` | Start definition | `Pokemon.Define("Pikachu", 25)` |
| `.Type(type)` | Set mono-type | `.Type(PokemonType.Fire)` |
| `.Types(primary, secondary)` | Set dual-type | `.Types(Fire, Flying)` |
| `.Stats(hp, atk, def, spa, spd, spe)` | Set base stats | `.Stats(39, 52, 43, 60, 50, 65)` |
| `.GenderRatio(percent)` | Set male percentage | `.GenderRatio(87.5f)` |
| `.Genderless()` | Mark as genderless | `.Genderless()` |
| `.MaleOnly()` | Mark as male only | `.MaleOnly()` |
| `.FemaleOnly()` | Mark as female only | `.FemaleOnly()` |
| `.Moves(m => ...)` | Configure learnset | See below |
| `.EvolvesTo(target, e => ...)` | Add evolution | See below |
| `.Build()` | Finalize | Required at end |

### Learnset Builder API

| Method | Description | Example |
|--------|-------------|---------|
| `.StartsWith(moves...)` | Level 1 moves | `.StartsWith(MoveCatalog.Tackle)` |
| `.AtLevel(level, moves...)` | Level-up moves | `.AtLevel(15, MoveCatalog.Ember)` |
| `.ByTM(moves...)` | TM/HM moves | `.ByTM(MoveCatalog.Earthquake)` |
| `.ByEgg(moves...)` | Egg moves | `.ByEgg(MoveCatalog.DragonRush)` |
| `.OnEvolution(moves...)` | Evolution moves | `.OnEvolution(MoveCatalog.HyperBeam)` |
| `.ByTutor(moves...)` | Tutor moves | `.ByTutor(MoveCatalog.Outrage)` |

### Evolution Builder API

| Method | Description | Example |
|--------|-------------|---------|
| `.AtLevel(level)` | Level requirement | `.AtLevel(16)` |
| `.WithItem(itemName)` | Use item | `.WithItem("Fire Stone")` |
| `.WithFriendship(min = 220)` | Friendship | `.WithFriendship()` |
| `.During(time)` | Time of day | `.During(TimeOfDay.Night)` |
| `.ByTrade()` | Trade evolution | `.ByTrade()` |
| `.KnowsMove(move)` | Must know move | `.KnowsMove(MoveCatalog.Rollout)` |

## 4. Dynamic Data (The "Instance") ⏳ PENDING
*Namespace: `PokemonUltimate.Core.Models`*

This represents a specific Pokemon in a specific battle. It is mutable.

### MoveInstance (PP Tracking)
```csharp
public class MoveInstance {
    // Reference to blueprint
    public MoveData Move { get; }
    
    // PP tracking
    public int MaxPP { get; }        // From MoveData.PP
    public int CurrentPP { get; set; }
    
    // Helpers
    public bool HasPP => CurrentPP > 0;
    
    // Methods
    public void Use();           // CurrentPP-- (if > 0)
    public void Restore(int amount);
    public void RestoreFully();  // CurrentPP = MaxPP
    
    public MoveInstance(MoveData move) {
        Move = move;
        MaxPP = move.PP;
        CurrentPP = MaxPP;
    }
}
```

### PokemonInstance (Runtime State)
```csharp
public class PokemonInstance {
    // Reference to source blueprint
    public PokemonSpeciesData Species { get; }
    
    // Unique ID for this specific instance
    public string InstanceId { get; }  // GUID
    public string Nickname { get; set; } // Optional, defaults to Species.Name

    // Level & Experience
    public int Level { get; private set; }
    public int CurrentExp { get; set; }
    
    // Calculated Stats (set by Factory, affected by Nature)
    public int MaxHP { get; }
    public int CurrentHP { get; set; }
    public int Attack { get; }
    public int Defense { get; }
    public int SpAttack { get; }
    public int SpDefense { get; }
    public int Speed { get; }
    
    // Personal characteristics
    public Gender Gender { get; }
    public Nature Nature { get; }
    
    // Combat State
    public List<MoveInstance> Moves { get; } // Max 4
    public PersistentStatus Status { get; set; } // Burn, Paralysis (Persists between battles)
    public VolatileStatus VolatileStatus { get; set; } // Flinch, Confusion (Reset after battle)
    public int StatusTurnCounter { get; set; } // For Sleep duration, Toxic accumulation
    
    // Stat Stages (-6 to +6, reset after battle)
    public Dictionary<Stat, int> StatStages { get; }
    
    // Future: Abilities & Items (IBattleListener)
    // public IBattleListener Ability { get; set; }
    // public IBattleListener Item { get; set; }

    // Helpers
    public bool IsFainted => CurrentHP <= 0;
    public float HPPercentage => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0;
    public string DisplayName => Nickname ?? Species.Name;
    
    // Get effective stat considering stages
    public int GetEffectiveStat(Stat stat);
    
    // Reset volatile state (after battle)
    public void ResetBattleState();
}
```

### Stat Stages Multiplier Table
| Stage | Multiplier |
|-------|------------|
| -6 | 2/8 = 0.25 |
| -5 | 2/7 = 0.29 |
| -4 | 2/6 = 0.33 |
| -3 | 2/5 = 0.40 |
| -2 | 2/4 = 0.50 |
| -1 | 2/3 = 0.67 |
| 0 | 2/2 = 1.00 |
| +1 | 3/2 = 1.50 |
| +2 | 4/2 = 2.00 |
| +3 | 5/2 = 2.50 |
| +4 | 6/2 = 3.00 |
| +5 | 7/2 = 3.50 |
| +6 | 8/2 = 4.00 |

Formula: `(2 + stage) / 2` for positive, `2 / (2 + |stage|)` for negative

## 5. The Factory (The Converter) ⏳ PENDING
*Namespace: `PokemonUltimate.Core.Factories`*

We use a Factory to handle the complex logic of creating a Pokemon (calculating stats, picking moves, determining gender).

### StatCalculator (Stat Formulas)
```csharp
public static class StatCalculator {
    // Gen3+ simplified formula (no IVs/EVs for now):
    // Other Stats: ((Base * 2) * Level / 100) + 5
    // HP: ((Base * 2) * Level / 100) + Level + 10
    
    public static int CalculateHP(int baseHP, int level) {
        return ((baseHP * 2) * level / 100) + level + 10;
    }
    
    public static int CalculateStat(int baseStat, int level, Nature nature, Stat stat) {
        int raw = ((baseStat * 2) * level / 100) + 5;
        float multiplier = NatureData.GetStatMultiplier(nature, stat);
        return (int)(raw * multiplier);
    }
    
    // Stat stage multiplier for battle
    public static float GetStageMultiplier(int stage) {
        if (stage >= 0) return (2 + stage) / 2f;
        return 2f / (2 + Math.Abs(stage));
    }
}
```

### PokemonFactory
```csharp
public static class PokemonFactory {
    // Random Nature and Gender
    public static PokemonInstance Create(PokemonSpeciesData species, int level);
    
    // Specific Nature, random Gender
    public static PokemonInstance Create(PokemonSpeciesData species, int level, Nature nature);
    
    // Full control
    public static PokemonInstance Create(PokemonSpeciesData species, int level, Nature nature, Gender gender);
    
    // Internal: Determine gender based on GenderRatio
    private static Gender DetermineGender(PokemonSpeciesData species, Random rng);
    
    // Internal: Select up to 4 moves from learnset
    private static List<MoveInstance> SelectMoves(PokemonSpeciesData species, int level);
}
```

### Factory Logic
```csharp
public static PokemonInstance Create(PokemonSpeciesData species, int level, Nature nature, Gender gender) {
    // 1. Calculate all stats
    int hp = StatCalculator.CalculateHP(species.BaseStats.HP, level);
    int atk = StatCalculator.CalculateStat(species.BaseStats.Attack, level, nature, Stat.Attack);
    int def = StatCalculator.CalculateStat(species.BaseStats.Defense, level, nature, Stat.Defense);
    int spa = StatCalculator.CalculateStat(species.BaseStats.SpAttack, level, nature, Stat.SpAttack);
    int spd = StatCalculator.CalculateStat(species.BaseStats.SpDefense, level, nature, Stat.SpDefense);
    int spe = StatCalculator.CalculateStat(species.BaseStats.Speed, level, nature, Stat.Speed);
    
    // 2. Select moves (highest level first, max 4)
    var moves = SelectMoves(species, level);
    
    // 3. Create instance with all calculated values
    return new PokemonInstance(species, level, hp, atk, def, spa, spd, spe, nature, gender, moves);
}

private static List<MoveInstance> SelectMoves(PokemonSpeciesData species, int level) {
    return species.GetMovesUpToLevel(level)
        .OrderByDescending(m => m.Level)  // Highest level first
        .ThenBy(m => m.Move.Name)         // Alphabetical for consistency
        .Take(4)                          // Max 4 moves
        .Select(m => new MoveInstance(m.Move))
        .ToList();
}

private static Gender DetermineGender(PokemonSpeciesData species, Random rng) {
    if (species.IsGenderless) return Gender.Genderless;
    if (species.IsMaleOnly) return Gender.Male;
    if (species.IsFemaleOnly) return Gender.Female;
    
    // Roll based on GenderRatio (% male)
    return rng.NextDouble() * 100 < species.GenderRatio ? Gender.Male : Gender.Female;
}
```

### Usage Examples
```csharp
// Quick create (random nature/gender)
var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);

// Specific nature
var adamantCharizard = PokemonFactory.Create(PokemonCatalog.Charizard, 50, Nature.Adamant);

// Full control (for tests or special Pokemon)
var mewtwo = PokemonFactory.Create(PokemonCatalog.Mewtwo, 70, Nature.Modest, Gender.Genderless);

// Access instance data
Console.WriteLine($"{pikachu.DisplayName} Lv.{pikachu.Level}");
Console.WriteLine($"HP: {pikachu.CurrentHP}/{pikachu.MaxHP}");
Console.WriteLine($"Moves: {string.Join(", ", pikachu.Moves.Select(m => m.Move.Name))}");
```

## 6. Unity Integration (ScriptableObjects) ⏳ FUTURE
*Namespace: `PokemonGame.Unity.Data`*

To make it "easy to add more", we use Unity's Inspector.

### `PokemonSpeciesSO`
-   **Menu**: `Create > Pokemon > New Species`
-   **Inspector Fields**:
    -   `Name` (String)
    -   `Types` (List<Type>)
    -   `Base Stats` (Struct with 6 int fields)
    -   `Sprite` (Sprite) -> *Note: The `ToPOCO` method will ignore this or convert to path ID*
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
