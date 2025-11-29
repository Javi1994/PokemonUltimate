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
    public BaseStats BaseStats { get; set; } // See below
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

### Example Usage
```csharp
var charizard = new PokemonSpeciesData {
    Name = "Charizard",
    PokedexNumber = 6,
    PrimaryType = PokemonType.Fire,
    SecondaryType = PokemonType.Flying,
    BaseStats = new BaseStats(78, 84, 78, 109, 85, 100) // BST: 534
};

// Or use the catalog
var pikachu = PokemonCatalog.Pikachu;
Console.WriteLine(pikachu.BaseStats.Speed); // 90
```

### Future Additions (Not Yet Implemented)
```csharp
// These will be added when needed:
public string SpritePath { get; set; }      // Visual reference
public string CryAudioPath { get; set; }    // Audio reference
public List<LearnableMove> MovePool { get; set; }  // Level-up moves
public string EvolutionId { get; set; }     // Evolution target
public int EvolutionLevel { get; set; }     // Evolution level
```

## 3. Dynamic Data (The "Instance")
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

## 4. The Factory (The Converter)
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

        // 2. Generate Moves
        // Get last 4 moves available at this level
        var validMoves = species.MovePool
            .Where(m => m.Level <= level)
            .OrderByDescending(m => m.Level)
            .Take(4);
            
        foreach (var move in validMoves) {
            // We need a MoveRepository to look up the MoveData
            // In a real implementation, we'd pass the repo to the Create method
            // pokemon.Moves.Add(new MoveInstance(moveData));
        }

        return pokemon;
    }
}
```

## 5. Unity Integration (ScriptableObjects)
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
        BaseStats = new Dictionary<Stat, int> {
            { Stat.HP, this.hp },
            { Stat.Attack, this.atk },
            // ...
        },
        // ...
    };
}
```

## 6. Workflow: Adding a New Pokemon
1.  Right Click in Project -> `Create > Pokemon > New Species`.
2.  Name it "Bulbasaur".
3.  Fill in Stats (45, 49, 49...).
4.  Drag and Drop Move assets into the "Moves" list.
5.  **Done.** The game automatically loads it via `GameDatabaseSO`.
