# Pokemon Data Structure & Factory Specification

## 1. Overview
This document defines exactly how a Pokemon is structured in data (Static) and in memory (Dynamic).
It follows the **Testability First** guideline by separating the Data (POCO) from the Unity Asset (ScriptableObject).

## 2. Static Data (The "Species") ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Blueprints`*

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
*Namespace: `PokemonUltimate.Core.Enums` and `PokemonUltimate.Core.Blueprints`*

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
*Namespace: `PokemonUltimate.Core.Blueprints`*

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
*Namespace: `PokemonUltimate.Content.Builders`*

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

## 4. Dynamic Data (The "Instance") ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Instances`*

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

## 5. The Factory (Fluent Builder) ✅ IMPLEMENTED
*Namespace: `PokemonUltimate.Core.Factories`*

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

| Method | Description | Priority |
|--------|-------------|----------|
| `.WithLearnsetMoves()` | Default: highest level first | Level descending |
| `.WithRandomMoves()` | Random selection | Random |
| `.WithStabMoves()` | Same Type Attack Bonus | Type match (+100 score) |
| `.WithStrongMoves()` | High base power | Power value |
| `.WithOptimalMoves()` | Best competitive set | STAB + Power combined |

**Score Calculation for Smart Moves:**
- STAB bonus: +100 if move type matches Pokemon type
- Power: Direct power value (0-150+)
- Accuracy: +Accuracy/10 (minor bonus)
- Damaging bonus: +50 for non-Status moves

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
