# Sub-Feature 1.1: Pokemon Data - Architecture

> Complete technical specification for Pokemon species blueprints and runtime instances.

**Sub-Feature Number**: 1.1  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the complete data structure for Pokemon, including:
- **PokemonSpeciesData** (Blueprint): Immutable species data shared by all Pokemon of the same kind
- **PokemonInstance** (Runtime): Mutable instance data for individual Pokemon
- **BaseStats**: Base stat values structure
- **LearnableMove**: Move learning information structure

## Design Principles

- **Blueprint/Instance Separation**: Blueprints are immutable, instances are mutable
- **Composition over Inheritance**: Pokemon are composed of stats, moves, abilities, not subclassed
- **Testability First**: Pure C# classes, no Unity dependencies

---

## 1. PokemonSpeciesData (Blueprint)

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/PokemonSpeciesData.cs`

Immutable blueprint for a Pokemon species. This is the "Species" data - shared by all Pokemon of the same kind.

### Core Properties

```csharp
public class PokemonSpeciesData : IIdentifiable
{
    // Identity
    public string Name { get; set; }              // "Charizard"
    public int PokedexNumber { get; set; }        // 6
    public string Id => Name;                     // IIdentifiable implementation
    
    // Types
    public PokemonType PrimaryType { get; set; }   // Required: Fire
    public PokemonType? SecondaryType { get; set; } // Optional: Flying (null if mono-type)
    
    // Base Stats
    public BaseStats BaseStats { get; set; }      // HP, Attack, Defense, SpAttack, SpDefense, Speed
    
    // Gender Ratio
    public float GenderRatio { get; set; }        // 0-100 = % male, -1 = genderless
    
    // Abilities
    public AbilityData Ability1 { get; set; }      // Primary ability
    public AbilityData Ability2 { get; set; }     // Secondary ability (optional)
    public AbilityData HiddenAbility { get; set; } // Hidden ability (optional)
    
    // Learnset
    public List<LearnableMove> Learnset { get; set; } // Moves this species can learn
    
    // Evolution paths
    public List<Evolution> Evolutions { get; set; } // Possible evolutions
}
```

### Helper Properties

```csharp
// Type helpers
public bool IsDualType => SecondaryType.HasValue;
public bool HasType(PokemonType type);

// Gender helpers
public bool IsGenderless => GenderRatio < 0;
public bool IsMaleOnly => GenderRatio >= 100;
public bool IsFemaleOnly => GenderRatio == 0;
public bool HasBothGenders => 0 < GenderRatio && GenderRatio < 100;

// Evolution helpers
public bool CanEvolve => Evolutions.Count > 0;

// Learnset helpers
public IEnumerable<LearnableMove> GetStartingMoves();
public IEnumerable<LearnableMove> GetMovesAtLevel(int level);
public IEnumerable<LearnableMove> GetMovesUpToLevel(int level);
public bool CanLearn(MoveData move);
```

### Gender Ratio Values

- `50.0f` = 50% male, 50% female (most Pokemon)
- `87.5f` = 87.5% male (starters like Charmander)
- `100.0f` = male only (Tauros)
- `0.0f` = female only (Chansey)
- `-1.0f` = genderless (Magnemite, Ditto)

---

## 2. BaseStats

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/BaseStats.cs`

Structure containing base stat values used for calculating actual stats.

```csharp
public class BaseStats
{
    public int HP { get; set; }           // Hit Points
    public int Attack { get; set; }        // Physical attack
    public int Defense { get; set; }       // Physical defense
    public int SpAttack { get; set; }      // Special attack
    public int SpDefense { get; set; }     // Special defense
    public int Speed { get; set; }         // Speed
    
    public int Total { get; }              // Sum of all stats (BST - Base Stat Total)
    
    // Validation
    public void Validate();                // Ensures all stats are non-negative
    
    // Helpers
    public int GetStat(Stat stat);        // Get stat by enum
    public Stat HighestStat { get; }      // Highest base stat
    public Stat LowestStat { get; }       // Lowest base stat
}
```

---

## 3. LearnableMove

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/LearnableMove.cs`

Structure defining how a Pokemon learns a move.

```csharp
public class LearnableMove
{
    public MoveData Move { get; set; }      // Reference to the move blueprint
    public LearnMethod Method { get; set; } // How it's learned
    public int Level { get; set; }          // Level requirement (for LevelUp method)
}
```

### LearnMethod Enum

```csharp
public enum LearnMethod
{
    Start,      // Available at level 1
    LevelUp,    // Learned at a specific level
    Evolution,  // Learned upon evolving
    TM,         // Learned via TM/HM
    Egg,        // Inherited from parents
    Tutor       // Taught by move tutor
}
```

---

## 4. PokemonInstance (Runtime)

**Namespace**: `PokemonUltimate.Core.Instances`  
**Files**: 
- `PokemonInstance.cs` (Core)
- `PokemonInstance.Battle.cs` (Battle state)
- `PokemonInstance.LevelUp.cs` (Level-up logic)
- `PokemonInstance.Evolution.cs` (Evolution tracking)

Mutable runtime instance of a Pokemon. Created from a PokemonSpeciesData blueprint via PokemonFactory.

### Core Properties (PokemonInstance.cs)

```csharp
public partial class PokemonInstance
{
    // Identity
    public PokemonSpeciesData Species { get; private set; }  // Reference to blueprint
    public string InstanceId { get; }                        // Unique identifier
    public string Nickname { get; set; }                     // Optional nickname
    public string DisplayName => Nickname ?? Species.Name;   // Display name
    
    // Level & Experience
    public int Level { get; private set; }                   // 1-100
    public int CurrentExp { get; set; }                      // Current experience points
    
    // Stats (calculated from base stats, level, IVs, EVs, nature)
    public int MaxHP { get; private set; }
    public int CurrentHP { get; set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int SpAttack { get; private set; }
    public int SpDefense { get; private set; }
    public int Speed { get; private set; }
    
    // Personal Characteristics
    public Gender Gender { get; }
    public Nature Nature { get; }
    public int Friendship { get; set; }                       // 0-255
    public bool IsShiny { get; }
    
    // Abilities & Items
    public AbilityData Ability { get; set; }                 // Assigned ability
    public ItemData HeldItem { get; set; }                   // Held item
    
    // Moves
    public List<MoveInstance> Moves { get; set; }           // Known moves (max 4)
}
```

### Battle State (PokemonInstance.Battle.cs)

```csharp
public partial class PokemonInstance
{
    // Status Effects
    public PersistentStatus Status { get; set; }             // Persistent status (Burn, Paralysis, etc.)
    public HashSet<VolatileStatus> VolatileStatuses { get; set; } // Volatile statuses
    
    // Stat Stages (-6 to +6)
    public int AttackStage { get; set; }
    public int DefenseStage { get; set; }
    public int SpAttackStage { get; set; }
    public int SpDefenseStage { get; set; }
    public int SpeedStage { get; set; }
    public int AccuracyStage { get; set; }
    public int EvasionStage { get; set; }
    
    // Battle Helpers
    public bool IsFainted => CurrentHP <= 0;
    public float HPPercentage => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0f;
    public bool HasStatus => Status != PersistentStatus.None;
    
    // Stat calculation with stages
    public int GetEffectiveStat(Stat stat);
    public int GetEffectiveSpeed();
}
```

### Level-Up Logic (PokemonInstance.LevelUp.cs)

```csharp
public partial class PokemonInstance
{
    // Experience & Leveling
    public void AddExperience(int exp);
    public void LevelUp();
    public void LearnMovesAtLevel(int level);
    
    // Move Learning
    public bool CanLearnMove(MoveData move);
    public void LearnMove(MoveData move, int slot = -1);  // -1 = auto-find slot
    public void ForgetMove(int slot);
}
```

### Evolution Logic (PokemonInstance.Evolution.cs)

```csharp
public partial class PokemonInstance
{
    // Evolution
    public bool CanEvolve();
    public PokemonInstance Evolve(PokemonSpeciesData targetSpecies);
    public List<PokemonSpeciesData> GetAvailableEvolutions();
    
    // Friendship helpers
    public bool HasHighFriendship => Friendship >= 220;
    public bool HasMaxFriendship => Friendship >= 255;
}
```

---

## 5. Stat Calculation

Stats are calculated using Gen 3+ formulas with IVs, EVs, and Nature modifiers.

**HP Formula**:
```
HP = floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
```

**Other Stats Formula**:
```
Stat = floor((floor((2 * Base + IV + floor(EV/4)) * Level / 100) + 5) * Nature)
```

Where:
- **Base**: Base stat from PokemonSpeciesData
- **IV**: Individual Value (0-31, randomly generated)
- **EV**: Effort Value (0-255 per stat, 510 total)
- **Level**: Pokemon's level (1-100)
- **Nature**: Multiplier (0.9, 1.0, or 1.1)

See **[Sub-Feature 1.12: Factories & Calculators](../1.12-factories-calculators/)** for `StatCalculator` implementation.

---

## 6. Creation & Factory Methods

### PokemonFactory

**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/PokemonFactory.cs`

Static factory for creating Pokemon instances:

```csharp
public static class PokemonFactory
{
    public static PokemonInstance Create(
        PokemonSpeciesData species,
        int level,
        Nature? nature = null,
        Gender? gender = null,
        AbilityData ability = null);
}
```

### PokemonInstanceBuilder

**Namespace**: `PokemonUltimate.Core.Factories`  
**File**: `PokemonUltimate.Core/Factories/PokemonInstanceBuilder.cs`

Fluent builder for creating Pokemon instances with full control:

```csharp
var pikachu = new PokemonInstanceBuilder(pikachuSpecies, 50)
    .WithNature(Nature.Timid)
    .WithGender(Gender.Male)
    .WithAbility(pikachuSpecies.Ability1)
    .WithMoves(thunderbolt, quickAttack, ironTail, voltTackle)
    .WithIVs(31, 31, 31, 31, 31, 31)  // Perfect IVs
    .WithEVs(0, 0, 0, 252, 4, 252)    // SpAttack and Speed EVs
    .Build();
```

---

## 7. Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves referenced in Learnset
- **[1.3: Ability Data](../1.3-ability-data/)** - Abilities assigned to species
- **[1.7: Evolution System](../1.7-evolution-system/)** - Evolution paths
- **[1.12: Factories & Calculators](../1.12-factories-calculators/)** - StatCalculator, PokemonFactory, PokemonInstanceBuilder
- **[1.13: Registry System](../1.13-registry-system/)** - PokemonRegistry for storing/retrieving species

---

## Related Documents

- **[Parent Architecture](../architecture.md#11-pokemon-data)** - Feature-level technical specification
- **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** - Code organization
- **[Sub-Feature README](README.md)** - Overview and quick navigation

---

**Last Updated**: 2025-01-XX

