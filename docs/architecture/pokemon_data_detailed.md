# Pokemon Data Structure & Factory Specification

## 1. Overview
This document defines exactly how a Pokemon is structured in data (Static) and in memory (Dynamic).
It follows the **Testability First** guideline by separating the Data (POCO) from the Unity Asset (ScriptableObject).

## 2. Static Data (The "Species") ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Data`*

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

### Learnset System ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Data`*

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
*Namespace: `PokemonUltimate.Core.Evolution`*

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
*Namespace: `PokemonUltimate.Core.Builders`*

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
*Namespace: `PokemonGame.Core.Models`*

This represents a specific Pokemon in a specific battle. It is mutable.

```csharp
public class PokemonInstance {
    // Reference to source
    public PokemonSpeciesData Species { get; private set; }
    
    // Unique ID for this specific run/battle
    public string InstanceId { get; private set; } 

    // Mutable State
    public int Level { get; private set; }
    public int CurrentHP { get; set; }
    public int CurrentExp { get; set; }
    
    // Calculated Stats (Base + Level + IVs/EVs)
    public Dictionary<Stat, int> Stats { get; private set; }
    
    // Combat State
    public List<MoveInstance> Moves { get; private set; } // Max 4
    public VolatileStatus VolatileStatus { get; set; } // Flinch, Confusion (Reset after battle)
    public PersistentStatus Status { get; set; } // Burn, Paralysis (Persists)
    public int StatusTurnCounter { get; set; } // For Sleep duration, Toxic accumulation
    
    // Stat Stages (-6 to +6)
    public Dictionary<Stat, int> StatStages { get; private set; }
    
    // Abilities & Items
    public IBattleListener Ability { get; set; } // e.g., Intimidate, Blaze
    public IBattleListener Item { get; set; } // e.g., Leftovers, Choice Band

    public PokemonInstance(PokemonSpeciesData species, int level) {
        Species = species;
        Level = level;
        InstanceId = System.Guid.NewGuid().ToString();
        // Stats are calculated by the Factory, not here, to keep this class dumb/data-only
    }
}
```

## 5. The Factory (The Converter) ⏳ PENDING
*Namespace: `PokemonGame.Core.Factories`*

We use a Factory to handle the complex logic of creating a Pokemon (calculating stats, rolling IVs, picking moves).

```csharp
public static class PokemonFactory {
    public static PokemonInstance Create(PokemonSpeciesData species, int level) {
        var pokemon = new PokemonInstance(species, level);
        
        // 1. Calculate Stats
        // Formula: ((Base * 2 * Level) / 100) + 5
        foreach (var stat in species.BaseStats) {
            int val = CalculateStat(stat.Key, stat.Value, level);
            pokemon.Stats[stat.Key] = val;
        }
        pokemon.CurrentHP = pokemon.Stats[Stat.HP]; // Start full HP

        // 2. Generate Moves from Learnset
        var validMoves = species.GetMovesUpToLevel(level)
            .OrderByDescending(m => m.Level)
            .Take(4);
            
        foreach (var move in validMoves) {
            pokemon.Moves.Add(new MoveInstance(move.Move));
        }

        return pokemon;
    }
}
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
