# Catalogs System Specification

## 1. Overview
The **Catalogs System** provides static, predefined game data (Pokemon and Moves) that can be:
1. Accessed directly in code (`PokemonCatalog.Pikachu`)
2. Bulk-registered into Registries (`PokemonCatalog.RegisterAll(registry)`)

This bridges the gap between static data definitions and the runtime Registry system.

## 2. Design Philosophy

### Why Catalogs?
- **Direct Access**: IDE autocompletion shows all available Pokemon/Moves
- **No Magic Strings**: Type-safe references instead of `registry.Get("Pikachu")`
- **Testability**: Tests can use specific Pokemon without setting up full registries
- **Bulk Registration**: One call to populate an entire registry
- **Modular**: Organized by generation/type using partial classes

### Catalog vs Registry
| Aspect | Catalog | Registry |
|--------|---------|----------|
| Storage | Static readonly fields | Runtime Dictionary |
| Access | `PokemonCatalog.Pikachu` | `registry.GetByName("Pikachu")` |
| Lookup | By field name only | By Name, Number, Type, etc. |
| Mutability | Immutable | Can add at runtime |
| Use Case | Known data, compile-time | Dynamic lookup, runtime |

## 3. File Structure

Catalogs and Builders live in the **`PokemonUltimate.Content`** project (separate from Core):

### Pokemon Catalog (by Generation)
```
PokemonUltimate.Content/Catalogs/Pokemon/
├── PokemonCatalog.cs           # Orchestrator: All, Count, RegisterAll
├── PokemonCatalog.Gen1.cs      # Generation 1 (#001-151)
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
└── MoveCatalog.Psychic.cs      # Psychic-type moves
```

### Builders
```
PokemonUltimate.Content/Builders/
├── PokemonBuilder.cs           # Pokemon.Define("Pikachu", 25)
├── MoveBuilder.cs              # Move.Define("Ember")
├── EffectBuilder.cs            # e => e.Damage().MayBurn(10)
├── LearnsetBuilder.cs          # m => m.StartsWith(...)
└── EvolutionBuilder.cs         # e => e.AtLevel(16)
```

## 4. Pokemon Builder System ✅ NEW

Pokemon are now defined using a **Fluent Builder Pattern** for improved readability:

### Example: Defining a Pokemon
```csharp
// Final evolutions first (so they can be referenced)
public static readonly PokemonSpeciesData Charizard = Pokemon.Define("Charizard", 6)
    .Types(PokemonType.Fire, PokemonType.Flying)
    .Stats(78, 84, 78, 109, 85, 100)
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
        .AtLevel(46, MoveCatalog.Flamethrower)
        .ByTM(MoveCatalog.FireBlast, MoveCatalog.Earthquake))
    .Build();

// Evolution target is already defined, so we can reference it
public static readonly PokemonSpeciesData Charmander = Pokemon.Define("Charmander", 4)
    .Type(PokemonType.Fire)
    .Stats(39, 52, 43, 60, 50, 65)
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
        .AtLevel(9, MoveCatalog.Ember))
    .EvolvesTo(Charmeleon, e => e.AtLevel(16))  // Type-safe reference!
    .Build();
```

### Builder Methods
| Method | Description |
|--------|-------------|
| `Pokemon.Define(name, dex)` | Start definition |
| `.Type(type)` | Set mono-type |
| `.Types(primary, secondary)` | Set dual-type |
| `.Stats(hp, atk, def, spa, spd, spe)` | Set base stats |
| `.Moves(m => ...)` | Configure learnset |
| `.EvolvesTo(target, e => ...)` | Add evolution |
| `.Build()` | Finalize |

### Learnset Builder
| Method | Description |
|--------|-------------|
| `.StartsWith(moves...)` | Level 1 moves |
| `.AtLevel(level, moves...)` | Level-up moves |
| `.ByTM(moves...)` | TM/HM moves |
| `.ByEgg(moves...)` | Egg moves |
| `.OnEvolution(moves...)` | Evolution moves |
| `.ByTutor(moves...)` | Tutor moves |

### Evolution Builder
| Method | Description |
|--------|-------------|
| `.AtLevel(level)` | Level requirement |
| `.WithItem(itemName)` | Use item (stone, etc.) |
| `.WithFriendship(min = 220)` | High friendship |
| `.During(time)` | Time of day |
| `.ByTrade()` | Trade evolution |
| `.KnowsMove(move)` | Must know move |

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

### PokemonCatalog (15 Pokemon - Gen 1)
| Pokemon | Pokedex # | Types | BST | Evolves To |
|---------|-----------|-------|-----|------------|
| Bulbasaur | 1 | Grass/Poison | 318 | Ivysaur @16 |
| Ivysaur | 2 | Grass/Poison | 405 | Venusaur @32 |
| Venusaur | 3 | Grass/Poison | 525 | - |
| Charmander | 4 | Fire | 309 | Charmeleon @16 |
| Charmeleon | 5 | Fire | 405 | Charizard @36 |
| Charizard | 6 | Fire/Flying | 534 | - |
| Squirtle | 7 | Water | 314 | Wartortle @16 |
| Wartortle | 8 | Water | 405 | Blastoise @36 |
| Blastoise | 9 | Water | 530 | - |
| Pikachu | 25 | Electric | 320 | Raichu (Thunder Stone) |
| Raichu | 26 | Electric | 485 | - |
| Eevee | 133 | Normal | 325 | (multiple) |
| Snorlax | 143 | Normal | 540 | - |
| Mewtwo | 150 | Psychic | 680 | - |
| Mew | 151 | Psychic | 600 | - |

### MoveCatalog (20 Moves)
| Type | Moves |
|------|-------|
| Normal | Tackle, Scratch, Quick Attack, Hyper Beam, Growl |
| Fire | Ember, Flamethrower, Fire Blast |
| Water | Water Gun, Surf, Hydro Pump |
| Grass | Vine Whip, Razor Leaf, Solar Beam |
| Electric | Thunder Shock, Thunderbolt, Thunder, Thunder Wave |
| Ground | Earthquake |
| Psychic | Psychic |

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

### Pattern 3: Filtering
```csharp
// All Fire-type Pokemon
var fireTypes = PokemonCatalog.All.Where(p => p.HasType(PokemonType.Fire));

// All Pokemon that can evolve
var evolving = PokemonCatalog.All.Where(p => p.CanEvolve);

// All Status moves
var statusMoves = MoveCatalog.All.Where(m => m.Category == MoveCategory.Status);
```

### Pattern 4: Learnset Queries
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
| Method | Description |
|--------|-------------|
| `Move.Define(name)` | Start definition |
| `.Description(text)` | Optional description |
| `.Type(type)` | Element type |
| `.Physical(power, acc, pp)` | Physical attack |
| `.Special(power, acc, pp)` | Special attack |
| `.Status(acc, pp)` | Status move (power = 0) |
| `.Priority(n)` | Priority (-7 to +5, default 0) |
| `.Target(scope)` | Target scope |
| `.TargetSelf()` | Shorthand: Self scope |
| `.TargetAllEnemies()` | Shorthand: AllEnemies scope |
| `.WithEffects(e => ...)` | Add effects |
| `.Build()` | Finalize |

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
| EffectType | Class | Description |
|------------|-------|-------------|
| `Damage` | `DamageEffect` | Standard damage calculation |
| `FixedDamage` | `FixedDamageEffect` | Fixed HP damage |
| `Status` | `StatusEffect` | Apply persistent status |
| `StatChange` | `StatChangeEffect` | Modify stat stages |
| `Recoil` | `RecoilEffect` | User takes recoil damage |
| `Drain` | `DrainEffect` | User heals from damage |
| `Heal` | `HealEffect` | Direct HP recovery |
| `Flinch` | `FlinchEffect` | May cause flinch |
| `MultiHit` | `MultiHitEffect` | Hits 2-5 times |

## 9. Testing Strategy

Tests are organized to match the catalog structure:

```
Tests/Catalogs/
├── Pokemon/
│   ├── PokemonCatalogTests.cs       # General: All, RegisterAll, validations
│   └── PokemonCatalogGen1Tests.cs   # Gen1-specific tests (with learnset/evolution)
└── Moves/
    ├── MoveCatalogTests.cs          # General tests
    ├── MoveCatalogNormalTests.cs    # Normal moves tests
    ├── MoveCatalogFireTests.cs      # Fire moves tests
    └── ...

Tests/Builders/
├── PokemonBuilderTests.cs       # Pokemon builder fluent API tests
├── LearnsetBuilderTests.cs      # Learnset builder tests
├── EvolutionBuilderTests.cs     # Evolution builder tests
├── MoveBuilderTests.cs          # Move builder fluent API tests
└── EffectBuilderTests.cs        # Effect builder tests

Tests/Evolution/
├── EvolutionConditionTests.cs   # Condition classes tests
└── EvolutionTests.cs            # Evolution class tests
```

### What Tests Should Verify:
1. **General**: All enumeration, Count, RegisterAll, uniqueness
2. **Generation/Type specific**: Correct data for each Pokemon/Move
3. **Effects**: Correct effects attached to moves
4. **Learnsets**: Moves are correctly defined with proper methods/levels
5. **Evolutions**: Evolution chains are correctly linked

## 10. Benefits of This Architecture

| Aspect | Benefit |
|--------|---------|
| **Scalability** | 1000 Pokemon = ~7 generation files, not 1 huge file |
| **Maintainability** | Each file is ~100-200 lines max |
| **Collaboration** | Different developers can work on different generations |
| **Testing** | Tests match source structure |
| **Discovery** | IDE shows all Pokemon/Moves via autocomplete |
| **Type Safety** | Evolution targets are compile-time references |
| **Readability** | Builder pattern makes data definitions clear |
