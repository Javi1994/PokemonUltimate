# Feature 3: Content Expansion - Architecture

> Complete technical specification of the catalogs system for game content.

**Feature Number**: 3  
**Feature Name**: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 1. Overview

The **Catalogs System** provides static, predefined game data (Pokemon and Moves) that can be:

1. Accessed directly in code (`PokemonCatalog.Pikachu`)
2. Bulk-registered into Registries (`PokemonCatalog.RegisterAll(registry)`)

This bridges the gap between static data definitions and the runtime Registry system.

## 2. Design Philosophy

### Why Catalogs?

-   **Direct Access**: IDE autocompletion shows all available Pokemon/Moves
-   **No Magic Strings**: Type-safe references instead of `registry.Get("Pikachu")`
-   **Testability**: Tests can use specific Pokemon without setting up full registries
-   **Bulk Registration**: One call to populate an entire registry
-   **Modular**: Organized by generation/type using partial classes

### Catalog vs Registry

| Aspect     | Catalog                  | Registry                        |
| ---------- | ------------------------ | ------------------------------- |
| Storage    | Static readonly fields   | Runtime Dictionary              |
| Access     | `PokemonCatalog.Pikachu` | `registry.GetByName("Pikachu")` |
| Lookup     | By field name only       | By Name, Number, Type, etc.     |
| Mutability | Immutable                | Can add at runtime              |
| Use Case   | Known data, compile-time | Dynamic lookup, runtime         |

## 3. File Structure

Catalogs and Builders live in the **`PokemonUltimate.Content`** project (separate from Core):

### Pokemon Catalog (by Generation)

```
PokemonUltimate.Content/Catalogs/Pokemon/
├── PokemonCatalog.cs           # Orchestrator: All, Count, RegisterAll
├── PokemonCatalog.Gen1.cs      # Generation 1 (#001-151) - with abilities!
├── PokemonCatalog.Gen2.cs      # Generation 2 (#152-251) [future]
└── PokemonCatalog.Custom.cs    # Custom Pokemon [future]
```

### Move Catalog (by Type)

```
PokemonUltimate.Content/Catalogs/Moves/
├── MoveCatalog.cs              # Orchestrator: All, Count, RegisterAll
├── MoveCatalog.Normal.cs       # Normal-type moves
├── MoveCatalog.Fire.cs         # Fire-type moves
├── MoveCatalog.Water.cs        # Water-type moves
├── MoveCatalog.Grass.cs        # Grass-type moves
├── MoveCatalog.Electric.cs     # Electric-type moves
├── MoveCatalog.Ground.cs       # Ground-type moves
├── MoveCatalog.Psychic.cs      # Psychic-type moves
├── MoveCatalog.Ghost.cs        # Ghost-type moves
├── MoveCatalog.Rock.cs         # Rock-type moves
├── MoveCatalog.Flying.cs       # Flying-type moves
├── MoveCatalog.Poison.cs       # Poison-type moves
└── MoveCatalog.Dragon.cs       # Dragon-type moves
```

### Ability Catalog (NEW ✅)

```
PokemonUltimate.Content/Catalogs/Abilities/
├── AbilityCatalog.cs           # Orchestrator: All, GetByName
├── AbilityCatalog.Gen3.cs      # Gen 3 abilities (25 abilities)
└── AbilityCatalog.Additional.cs # Additional abilities (10 abilities)
```

### Item Catalog (NEW ✅)

```
PokemonUltimate.Content/Catalogs/Items/
├── ItemCatalog.cs              # Orchestrator: All, GetByName
├── ItemCatalog.HeldItems.cs    # Held items (15 items)
└── ItemCatalog.Berries.cs      # Berries (8 items)
```

### Status Catalog (NEW ✅)

```
PokemonUltimate.Content/Catalogs/Status/
└── StatusCatalog.cs            # All status effects (15 statuses)
```

### Weather Catalog (NEW ✅)

```
PokemonUltimate.Content/Catalogs/Weather/
└── WeatherCatalog.cs           # All weather conditions (9 weathers)
```

### Terrain Catalog (NEW ✅)

```
PokemonUltimate.Content/Catalogs/Terrain/
└── TerrainCatalog.cs           # All terrain conditions (4 terrains)
```

### Builders (Sub-Feature 3.9)

**Namespace**: `PokemonUltimate.Content.Builders`  
**Files**: `PokemonUltimate.Core/Builders/*.cs` (Note: Files are physically in Core but namespace is Content.Builders)

```
PokemonUltimate.Core/Builders/ (all use namespace PokemonUltimate.Content.Builders)
├── PokemonBuilder.cs           # Pokemon.Define("Pikachu", 25)
├── MoveBuilder.cs              # Move.Define("Ember")
├── AbilityBuilder.cs           # Ability.Define("Intimidate")
├── ItemBuilder.cs              # Item.Define("Leftovers")
├── StatusEffectBuilder.cs      # Status.Define("Burn")
├── SideConditionBuilder.cs     # Screen.Define("Reflect")
├── FieldEffectBuilder.cs       # Room.Define("Trick Room")
├── HazardBuilder.cs            # Hazard.Define("Stealth Rock")
├── WeatherBuilder.cs           # WeatherEffect.Define("Rain")
├── TerrainBuilder.cs           # TerrainEffect.Define("Grassy Terrain")
├── EffectBuilder.cs            # e => e.Damage().MayBurn(10)
├── LearnsetBuilder.cs          # m => m.StartsWith(...)
└── EvolutionBuilder.cs         # e => e.AtLevel(16)
```

**See**: [Sub-Feature 3.9: Builders](3.9-builders/) for complete documentation.

## 4. Pokemon Builder System ✅ NEW

Pokemon are now defined using a **Fluent Builder Pattern** for improved readability:

### Example: Defining a Pokemon (with Abilities ✅)

```csharp
// Final evolutions first (so they can be referenced)
public static readonly PokemonSpeciesData Charizard = Pokemon.Define("Charizard", 6)
    .Types(PokemonType.Fire, PokemonType.Flying)
    .Stats(78, 84, 78, 109, 85, 100)
    .Ability(AbilityCatalog.Blaze)              // Primary ability
    .HiddenAbility(AbilityCatalog.SolarPower)   // Hidden ability
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
        .AtLevel(46, MoveCatalog.Flamethrower)
        .ByTM(MoveCatalog.FireBlast, MoveCatalog.Earthquake))
    .Build();

// Pokemon with two normal abilities
public static readonly PokemonSpeciesData Snorlax = Pokemon.Define("Snorlax", 143)
    .Type(PokemonType.Normal)
    .Stats(160, 110, 65, 65, 110, 30)
    .Abilities(AbilityCatalog.Immunity, AbilityCatalog.ThickFat)  // Two options
    .HiddenAbility(AbilityCatalog.Gluttony)
    .Build();

// Evolution target is already defined, so we can reference it
public static readonly PokemonSpeciesData Charmander = Pokemon.Define("Charmander", 4)
    .Type(PokemonType.Fire)
    .Stats(39, 52, 43, 60, 50, 65)
    .Ability(AbilityCatalog.Blaze)
    .HiddenAbility(AbilityCatalog.SolarPower)
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
        .AtLevel(9, MoveCatalog.Ember))
    .EvolvesTo(Charmeleon, e => e.AtLevel(16))  // Type-safe reference!
    .Build();
```

### Builder Methods

| Method                                | Description               |
| ------------------------------------- | ------------------------- |
| `Pokemon.Define(name, dex)`           | Start definition          |
| `.Type(type)`                         | Set mono-type             |
| `.Types(primary, secondary)`          | Set dual-type             |
| `.Stats(hp, atk, def, spa, spd, spe)` | Set base stats            |
| `.Ability(ability)`                   | Set primary ability       |
| `.Abilities(primary, secondary)`      | Set both normal abilities |
| `.HiddenAbility(ability)`             | Set hidden ability        |
| `.Moves(m => ...)`                    | Configure learnset        |
| `.EvolvesTo(target, e => ...)`        | Add evolution             |
| `.Build()`                            | Finalize                  |

### Learnset Builder

| Method                      | Description     |
| --------------------------- | --------------- |
| `.StartsWith(moves...)`     | Level 1 moves   |
| `.AtLevel(level, moves...)` | Level-up moves  |
| `.ByTM(moves...)`           | TM/HM moves     |
| `.ByEgg(moves...)`          | Egg moves       |
| `.OnEvolution(moves...)`    | Evolution moves |
| `.ByTutor(moves...)`        | Tutor moves     |

### Evolution Builder

| Method                       | Description            |
| ---------------------------- | ---------------------- |
| `.AtLevel(level)`            | Level requirement      |
| `.WithItem(itemName)`        | Use item (stone, etc.) |
| `.WithFriendship(min = 220)` | High friendship        |
| `.During(time)`              | Time of day            |
| `.ByTrade()`                 | Trade evolution        |
| `.KnowsMove(move)`           | Must know move         |

### Important: Definition Order

**Define Pokemon in reverse evolution order** so targets exist when referenced:

```csharp
// ✅ Correct: Final form first
public static readonly PokemonSpeciesData Venusaur = ...;  // No evolution
public static readonly PokemonSpeciesData Ivysaur = ...;   // → Venusaur
public static readonly PokemonSpeciesData Bulbasaur = ...; // → Ivysaur

// ❌ Wrong: Would fail because Ivysaur doesn't exist yet
public static readonly PokemonSpeciesData Bulbasaur = ...
    .EvolvesTo(Ivysaur, ...)  // Error: Ivysaur is null
```

## 5. Current Content

### PokemonCatalog (26 Pokemon - Gen 1)

| Pokemon    | Pokedex # | Types        | BST | Evolves To             |
| ---------- | --------- | ------------ | --- | ---------------------- |
| Bulbasaur  | 1         | Grass/Poison | 318 | Ivysaur @16            |
| Ivysaur    | 2         | Grass/Poison | 405 | Venusaur @32           |
| Venusaur   | 3         | Grass/Poison | 525 | -                      |
| Charmander | 4         | Fire         | 309 | Charmeleon @16         |
| Charmeleon | 5         | Fire         | 405 | Charizard @36          |
| Charizard  | 6         | Fire/Flying  | 534 | -                      |
| Squirtle   | 7         | Water        | 314 | Wartortle @16          |
| Wartortle  | 8         | Water        | 405 | Blastoise @36          |
| Blastoise  | 9         | Water        | 530 | -                      |
| Pikachu    | 25        | Electric     | 320 | Raichu (Thunder Stone) |
| Raichu     | 26        | Electric     | 485 | -                      |
| Geodude    | 74        | Rock/Ground  | 300 | Graveler @25           |
| Graveler   | 75        | Rock/Ground  | 390 | Golem (Trade)          |
| Golem      | 76        | Rock/Ground  | 495 | -                      |
| Abra       | 63        | Psychic      | 310 | Kadabra @16            |
| Kadabra    | 64        | Psychic      | 400 | Alakazam (Trade)       |
| Alakazam   | 65        | Psychic      | 500 | -                      |
| Gastly     | 92        | Ghost/Poison | 310 | Haunter @25            |
| Haunter    | 93        | Ghost/Poison | 405 | Gengar (Trade)         |
| Gengar     | 94        | Ghost/Poison | 500 | -                      |
| Magikarp   | 129       | Water        | 200 | Gyarados @20           |
| Gyarados   | 130       | Water/Flying | 540 | -                      |
| Eevee      | 133       | Normal       | 325 | (multiple)             |
| Snorlax    | 143       | Normal       | 540 | -                      |
| Mewtwo     | 150       | Psychic      | 680 | -                      |
| Mew        | 151       | Psychic      | 600 | -                      |

### MoveCatalog (36 Moves)

| Type     | Moves                                                                  |
| -------- | ---------------------------------------------------------------------- |
| Normal   | Tackle, Scratch, Quick Attack, Hyper Beam, Growl, Defense Curl, Splash |
| Fire     | Ember, Flamethrower, Fire Blast                                        |
| Water    | Water Gun, Surf, Hydro Pump, Waterfall                                 |
| Grass    | Vine Whip, Razor Leaf, Solar Beam                                      |
| Electric | Thunder Shock, Thunderbolt, Thunder, Thunder Wave                      |
| Ground   | Earthquake                                                             |
| Psychic  | Psychic, Teleport, Confusion, Psybeam, Hypnosis                        |
| Ghost    | Lick, Shadow Ball                                                      |
| Rock     | Rock Throw, Rock Slide                                                 |
| Flying   | Wing Attack, Fly                                                       |
| Poison   | Poison Sting, Sludge Bomb                                              |
| Dragon   | Dragon Rage                                                            |

### AbilityCatalog (35 Abilities) ✅ NEW

| Category          | Abilities                                                 |
| ----------------- | --------------------------------------------------------- |
| Stat Modification | Intimidate, Speed Boost, Clear Body, Huge Power           |
| Status-Related    | Static, Poison Point, Flame Body, Limber, Immunity        |
| Type Immunity     | Levitate, Flash Fire, Water Absorb, Volt Absorb           |
| Power Boost       | Blaze, Torrent, Overgrow, Swarm                           |
| Damage Reduction  | Thick Fat                                                 |
| Weather           | Drizzle, Drought, Sand Stream, Swift Swim, Chlorophyll    |
| Survival          | Sturdy                                                    |
| Contact Damage    | Rough Skin                                                |
| Additional        | Solar Power, Rain Dish, Lightning Rod, Adaptability, etc. |

### ItemCatalog (23 Items) ✅ NEW

| Category   | Items                                                                                                                                                                                      |
| ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Held Items | Leftovers, Choice Band, Choice Specs, Choice Scarf, Life Orb, Focus Sash, Expert Belt, Muscle Band, Wise Glasses, Assault Vest, Rocky Helmet, Eviolite, Black Sludge, Toxic Orb, Flame Orb |
| Berries    | Oran Berry, Sitrus Berry, Cheri Berry, Chesto Berry, Pecha Berry, Rawst Berry, Aspear Berry, Lum Berry                                                                                     |

### StatusCatalog (15 Statuses) ✅ COMPLETE

**Location**: `PokemonUltimate.Content/Catalogs/Status/StatusCatalog.cs`

| Type               | Statuses                                                                      |
| ------------------ | ----------------------------------------------------------------------------- |
| **Persistent** (6) | Burn, Paralysis, Sleep, Poison, BadlyPoisoned, Freeze                         |
| **Volatile** (9)   | Confusion, Attract, Flinch, LeechSeed, Curse, Encore, Taunt, Torment, Disable |

**Status**: ✅ Complete - All status effects implemented

### WeatherCatalog (9 Weathers) ✅ COMPLETE

**Location**: `PokemonUltimate.Content/Catalogs/Weather/WeatherCatalog.cs`

| Category                | Weather                                            |
| ----------------------- | -------------------------------------------------- |
| **Standard** (5 turns)  | Rain, Sun (Harsh Sunlight), Sandstorm, Hail, Snow  |
| **Primal** (indefinite) | Heavy Rain, Extremely Harsh Sunlight, Strong Winds |
| **Special**             | Fog                                                |

**Status**: ✅ Complete - All weather conditions implemented

### TerrainCatalog (4 Terrains) ✅ COMPLETE

**Location**: `PokemonUltimate.Content/Catalogs/Terrain/TerrainCatalog.cs`

| Terrain  | Boost         | Special Effect                   |
| -------- | ------------- | -------------------------------- |
| Grassy   | Grass 1.3x    | Heals 1/16 HP, halves Earthquake |
| Electric | Electric 1.3x | Prevents Sleep                   |
| Psychic  | Psychic 1.3x  | Blocks Priority                  |
| Misty    | -             | Dragon 0.5x, prevents all status |

**Status**: ✅ Complete - All terrain conditions implemented

### HazardCatalog (4 Hazards) ✅ COMPLETE

**Location**: `PokemonUltimate.Content/Catalogs/Field/HazardCatalog.cs`

| Hazard       | Layers | Effect                                 |
| ------------ | ------ | -------------------------------------- |
| Stealth Rock | 1      | Type-based damage (Rock effectiveness) |
| Spikes       | 1-3    | 12.5% / 16.7% / 25% HP                 |
| Toxic Spikes | 1-2    | Poison / Badly Poisoned                |
| Sticky Web   | 1      | -1 Speed                               |

**Status**: ✅ Complete - All hazard types implemented

### SideConditionCatalog (10 Conditions) ✅ COMPLETE

**Location**: `PokemonUltimate.Content/Catalogs/Field/SideConditionCatalog.cs`

| Category         | Conditions                             |
| ---------------- | -------------------------------------- |
| **Screens**      | Reflect, Light Screen, Aurora Veil     |
| **Speed/Status** | Tailwind, Safeguard, Mist, Lucky Chant |
| **Protection**   | Wide Guard, Quick Guard, Mat Block     |

**Status**: ✅ Complete - All side conditions implemented

### FieldEffectCatalog (8 Effects) ✅ COMPLETE

**Location**: `PokemonUltimate.Content/Catalogs/Field/FieldEffectCatalog.cs`

| Category   | Effects                             |
| ---------- | ----------------------------------- |
| **Rooms**  | Trick Room, Magic Room, Wonder Room |
| **Field**  | Gravity, Ion Deluge, Fairy Lock     |
| **Sports** | Mud Sport, Water Sport              |

**Status**: ✅ Complete - All field effects implemented

## 6. Adding New Content

### Adding Generation 2 Pokemon

1. **Create file** `PokemonUltimate.Content/Catalogs/Pokemon/PokemonCatalog.Gen2.cs`:

```csharp
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    public static partial class PokemonCatalog
    {
        // Define in reverse evolution order!
        public static readonly PokemonSpeciesData Meganium = Builders.Pokemon.Define("Meganium", 154)
            .Type(PokemonType.Grass)
            .Stats(80, 82, 100, 83, 100, 80)
            .Build();

        public static readonly PokemonSpeciesData Bayleef = Builders.Pokemon.Define("Bayleef", 153)
            .Type(PokemonType.Grass)
            .Stats(60, 62, 80, 63, 80, 60)
            .EvolvesTo(Meganium, e => e.AtLevel(32))
            .Build();

        public static readonly PokemonSpeciesData Chikorita = Builders.Pokemon.Define("Chikorita", 152)
            .Type(PokemonType.Grass)
            .Stats(45, 49, 65, 49, 65, 45)
            .EvolvesTo(Bayleef, e => e.AtLevel(16))
            .Build();

        static partial void RegisterGen2()
        {
            _all.Add(Chikorita);
            _all.Add(Bayleef);
            _all.Add(Meganium);
        }
    }
}
```

2. **Update orchestrator** `PokemonCatalog.cs`:

```csharp
private static void InitializeAll()
{
    _all = new List<PokemonSpeciesData>();
    RegisterGen1();
    RegisterGen2();  // ← Add this line
}

static partial void RegisterGen2();  // ← Add declaration
```

### Adding Ice Moves

1. **Create file** `PokemonUltimate.Content/Catalogs/Moves/MoveCatalog.Ice.cs`:

```csharp
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    public static partial class MoveCatalog
    {
        public static readonly MoveData IceBeam = Move.Define("Ice Beam")
            .Type(PokemonType.Ice)
            .Special(90, 100, 10)
            .WithEffects(e => e
                .Damage()
                .MayFreeze(10))
            .Build();

        static partial void RegisterIce()
        {
            _all.Add(IceBeam);
        }
    }
}
```

2. **Update orchestrator** `MoveCatalog.cs`:

```csharp
private static void InitializeAll()
{
    // ... existing types
    RegisterIce();  // ← Add this line
}

static partial void RegisterIce();  // ← Add declaration
```

## 7. Usage Patterns

### Pattern 1: Direct Access

```csharp
var pikachu = PokemonCatalog.Pikachu;
var thunderbolt = MoveCatalog.Thunderbolt;

Assert.That(pikachu.BaseStats.Speed, Is.EqualTo(90));
Assert.That(pikachu.CanEvolve, Is.True);
Assert.That(pikachu.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Raichu));
```

### Pattern 2: Registry Population

```csharp
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Core.Registry;

var pokemonRegistry = new PokemonRegistry();
var moveRegistry = new MoveRegistry();

PokemonCatalog.RegisterAll(pokemonRegistry);
MoveCatalog.RegisterAll(moveRegistry);

var pokemon = pokemonRegistry.GetByPokedexNumber(25);
```

**Note**: `RegisterAll()` now validates that the registry is not null and throws `ArgumentNullException` if it is.

### Pattern 3: Query Methods (NEW ✅)

```csharp
// Get all Gen 1 Pokemon
var gen1Pokemon = PokemonCatalog.GetAllGen1();

// Get Pokemon by Pokedex number
var pikachu = PokemonCatalog.GetByPokedexNumber(25);

// Get all Fire-type Pokemon
var fireTypes = PokemonCatalog.GetAllByType(PokemonType.Fire);

// Get all Fire-type moves
var fireMoves = MoveCatalog.GetAllByType(PokemonType.Fire);

// Get move by name (case-insensitive)
var flamethrower = MoveCatalog.GetByName("Flamethrower");
var flamethrower2 = MoveCatalog.GetByName("FLAMETHROWER"); // Same result
```

### Pattern 4: Filtering

```csharp
// All Fire-type Pokemon (using query method)
var fireTypes = PokemonCatalog.GetAllByType(PokemonType.Fire);

// All Pokemon that can evolve
var evolving = PokemonCatalog.All.Where(p => p.CanEvolve);

// All Status moves
var statusMoves = MoveCatalog.All.Where(m => m.Category == MoveCategory.Status);
```

### Pattern 5: Learnset Queries

```csharp
var charmander = PokemonCatalog.Charmander;

// Get starting moves
var startMoves = charmander.GetStartingMoves(); // [Scratch, Growl]

// Get moves at level 9
var level9Moves = charmander.GetMovesAtLevel(9); // [Ember]

// Check if can learn a move
bool canLearn = charmander.CanLearn(MoveCatalog.Flamethrower); // true
```

## 8. Move Builder System ✅ NEW

Moves are now defined using a **Fluent Builder Pattern** for improved readability:

### Example: Defining Moves

```csharp
public static readonly MoveData Flamethrower = Move.Define("Flamethrower")
    .Description("The target is scorched with an intense blast of fire.")
    .Type(PokemonType.Fire)
    .Special(90, 100, 15)  // power, accuracy, pp
    .WithEffects(e => e
        .Damage()
        .MayBurn(10))
    .Build();

public static readonly MoveData SwordsDance = Move.Define("Swords Dance")
    .Type(PokemonType.Normal)
    .Status(0, 20)  // accuracy (0 = never miss), pp
    .TargetSelf()
    .WithEffects(e => e.RaiseAttack(2))
    .Build();
```

### MoveBuilder Methods

| Method                      | Description                    |
| --------------------------- | ------------------------------ |
| `Move.Define(name)`         | Start definition               |
| `.Description(text)`        | Optional description           |
| `.Type(type)`               | Element type                   |
| `.Physical(power, acc, pp)` | Physical attack                |
| `.Special(power, acc, pp)`  | Special attack                 |
| `.Status(acc, pp)`          | Status move (power = 0)        |
| `.Priority(n)`              | Priority (-7 to +5, default 0) |
| `.Target(scope)`            | Target scope                   |
| `.TargetSelf()`             | Shorthand: Self scope          |
| `.TargetAllEnemies()`       | Shorthand: AllEnemies scope    |
| `.WithEffects(e => ...)`    | Add effects                    |
| `.Build()`                  | Finalize                       |

### EffectBuilder Methods

**Damage:**
| Method | Description |
|--------|-------------|
| `.Damage()` | Standard damage |
| `.DamageHighCrit(stages)` | High crit ratio |
| `.FixedDamage(amount)` | Fixed damage (Dragon Rage) |
| `.LevelDamage()` | Damage = level (Seismic Toss) |

**Status:**
| Method | Description |
|--------|-------------|
| `.MayBurn(chance)` | May inflict burn |
| `.MayParalyze(chance)` | May inflict paralysis |
| `.MayPoison(chance)` | May inflict poison |
| `.MayBadlyPoison(chance)` | May inflict toxic |
| `.MaySleep(chance)` | May inflict sleep |
| `.MayFreeze(chance)` | May inflict freeze |

**Stat Changes:**
| Method | Description |
|--------|-------------|
| `.RaiseAttack/Defense/SpAttack/SpDefense/Speed/Evasion(stages, chance)` | Buff self |
| `.LowerAttack/Defense/SpAttack/SpDefense/Speed/Accuracy(stages, chance)` | Debuff target |

**Other:**
| Method | Description |
|--------|-------------|
| `.Recoil(percent)` | User takes recoil |
| `.Drain(percent)` | Heal from damage |
| `.Heal(percent)` | Heal % of max HP |
| `.MayFlinch(chance)` | May flinch target |
| `.MultiHit(min, max)` | Hit 2-5 times |
| `.HitsNTimes(n)` | Hit exactly N times |

### Available Effect Types

| EffectType    | Class               | Description                 |
| ------------- | ------------------- | --------------------------- |
| `Damage`      | `DamageEffect`      | Standard damage calculation |
| `FixedDamage` | `FixedDamageEffect` | Fixed HP damage             |
| `Status`      | `StatusEffect`      | Apply persistent status     |
| `StatChange`  | `StatChangeEffect`  | Modify stat stages          |
| `Recoil`      | `RecoilEffect`      | User takes recoil damage    |
| `Drain`       | `DrainEffect`       | User heals from damage      |
| `Heal`        | `HealEffect`        | Direct HP recovery          |
| `Flinch`      | `FlinchEffect`      | May cause flinch            |
| `MultiHit`    | `MultiHitEffect`    | Hits 2-5 times              |

## 9. Thread Safety and Performance

### Thread-Safe Initialization ✅ NEW

Catalogs use **double-check locking** pattern for thread-safe lazy initialization:

```csharp
private static readonly object _lockObject = new object();
private static volatile List<PokemonSpeciesData> _all;

public static IReadOnlyList<PokemonSpeciesData> All
{
    get
    {
        if (_all == null)
        {
            lock (_lockObject)
            {
                if (_all == null)
                {
                    InitializeAll();
                }
            }
        }
        return _all.AsReadOnly();
    }
}
```

**Benefits**:

-   Thread-safe initialization without performance penalty after first access
-   Prevents race conditions in multi-threaded environments
-   Uses `volatile` keyword for proper memory visibility

### Performance Optimizations ✅ NEW

-   **`IReadOnlyList<T>` return type**: More specific than `IEnumerable<T>`, enables indexed access and better performance
-   **`AsReadOnly()`**: Prevents accidental modifications while maintaining performance
-   **Constants for generation ranges**: Eliminates magic numbers, improves maintainability

## 10. Validation and Error Handling ✅ NEW

### Catalog Integrity Validation

During initialization, catalogs validate:

1. **Duplicate Pokedex Numbers**: Ensures no two Pokemon share the same Pokedex number
2. **Duplicate Names**: Ensures no two Pokemon share the same name (case-insensitive)
3. **Evolution References**: Validates that all evolution targets exist in the catalog
4. **Learnset References**: Validates that all moves in learnsets exist in MoveCatalog

**Error Handling**:

-   `InitializeAll()` wraps registration in try-catch
-   Throws `InvalidOperationException` with descriptive messages
-   Fail-fast approach: errors detected immediately during initialization

### Input Validation

-   `RegisterAll()`: Validates registry is not null (`ArgumentNullException`)
-   `GetByPokedexNumber()`: Validates number is greater than 0 (`ArgumentException`)
-   `GetByName()`: Validates name is not null/empty/whitespace (`ArgumentException`)

## 11. Testing Strategy

Tests are organized to match the catalog structure:

```
Tests/Data/Catalogs/
├── Pokemon/
│   ├── PokemonCatalogTests.cs              # General: All, Count, RegisterAll
│   └── PokemonCatalogValidationTests.cs    # Validation: integrity, queries, edge cases
└── Moves/
    └── MoveCatalogTests.cs                 # General tests + query methods + validation
```

### What Tests Verify:

1. **General**: All enumeration, Count, RegisterAll, uniqueness
2. **Query Methods**: `GetAllGen1()`, `GetByPokedexNumber()`, `GetAllByType()`, `GetByName()`
3. **Validation**: Null checks, duplicate detection, reference validation
4. **Thread Safety**: Concurrent access (if applicable)
5. **Generation/Type specific**: Correct data for each Pokemon/Move
6. **Effects**: Correct effects attached to moves
7. **Learnsets**: Moves are correctly defined with proper methods/levels
8. **Evolutions**: Evolution chains are correctly linked

## 10. Benefits of This Architecture

| Aspect              | Benefit                                                |
| ------------------- | ------------------------------------------------------ |
| **Scalability**     | 1000 Pokemon = ~7 generation files, not 1 huge file    |
| **Maintainability** | Each file is ~100-200 lines max                        |
| **Collaboration**   | Different developers can work on different generations |
| **Testing**         | Tests match source structure                           |
| **Discovery**       | IDE shows all Pokemon/Moves via autocomplete           |
| **Type Safety**     | Evolution targets are compile-time references          |
| **Readability**     | Builder pattern makes data definitions clear           |

---

## Related Documents

-   **[Feature README](README.md)** - Overview of Content Expansion
-   **[Use Cases](use_cases.md)** - All scenarios for adding content
-   **[Roadmap](roadmap.md)** - Content expansion phases and implementation status
-   **[Testing](testing.md)** - Testing strategy for content validation
-   **[Code Location](code_location.md)** - Where catalogs and builders are implemented
-   **[Feature 1: Game Data](../1-game-data/architecture.md)** - Data structure for Pokemon species
-   **[Feature 2: Combat System](../2-combat-system/architecture.md)** - How moves and abilities are used in combat
-   **[Feature 2.5: Combat Actions - Move Effects Execution](../2-combat-system/2.5-combat-actions/effects/architecture.md)** - How move effects execute in combat

---

**Last Updated**: 2025-01-XX
