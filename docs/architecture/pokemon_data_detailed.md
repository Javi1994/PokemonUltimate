# Pokemon Data Structure & Factory Specification

## 1. Overview
This document defines exactly how a Pokemon is structured in data (Static) and in memory (Dynamic).
It follows the **Testability First** guideline by separating the Data (POCO) from the Unity Asset (ScriptableObject).

## 2. Static Data (The "Species")
*Namespace: `PokemonGame.Core.Data`*

This represents the "Blueprint" of a Pokemon (e.g., "Charizard"). It is immutable during gameplay.

```csharp
public class PokemonSpeciesData {
    // Identity
    public string Id { get; set; }          // "charizard"
    public string Name { get; set; }        // "Charizard"
    
    // Visuals (Paths/Keys only, no Unity types)
    public string SpritePath { get; set; }  
    public string CryAudioPath { get; set; }

    // Core Stats
    public List<PokemonType> Types { get; set; } // [Fire, Flying]
    public Dictionary<Stat, int> BaseStats { get; set; } 
    // { HP:78, Atk:84, Def:78, SpAtk:109, SpDef:85, Spd:100 }

    // Progression
    public List<LearnableMove> MovePool { get; set; }
    // [{Level:1, MoveId:"scratch"}, {Level:36, MoveId:"flamethrower"}]
    
    // Evolution (Optional for now, but good to have)
    public string EvolutionId { get; set; }
    public int EvolutionLevel { get; set; }
}

public enum Stat { HP, Attack, Defense, SpAttack, SpDefense, Speed }
public enum PokemonType { Normal, Fire, Water, Grass, Electric, ... }
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
