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

### Pokemon Catalog (by Generation)
```
Catalogs/Pokemon/
├── PokemonCatalog.cs           # Orquestador: All, Count, RegisterAll
├── PokemonCatalog.Gen1.cs      # Generation 1 (#001-151)
├── PokemonCatalog.Gen2.cs      # Generation 2 (#152-251) [future]
└── PokemonCatalog.Custom.cs    # Custom Pokemon [future]
```

### Move Catalog (by Type)
```
Catalogs/Moves/
├── MoveCatalog.cs              # Orquestador: All, Count, RegisterAll
├── MoveCatalog.Normal.cs       # Normal-type moves
├── MoveCatalog.Fire.cs         # Fire-type moves
├── MoveCatalog.Water.cs        # Water-type moves
├── MoveCatalog.Grass.cs        # Grass-type moves
├── MoveCatalog.Electric.cs     # Electric-type moves
├── MoveCatalog.Ground.cs       # Ground-type moves
└── MoveCatalog.Psychic.cs      # Psychic-type moves
```

## 4. Architecture

### PokemonCatalog (Orchestrator)
```csharp
public static partial class PokemonCatalog
{
    private static List<PokemonSpeciesData> _all;
    
    public static IEnumerable<PokemonSpeciesData> All
    {
        get
        {
            if (_all == null) InitializeAll();
            return _all;
        }
    }
    
    public static int Count => _all?.Count ?? 0;
    
    public static void RegisterAll(IPokemonRegistry registry);
    
    private static void InitializeAll()
    {
        _all = new List<PokemonSpeciesData>();
        RegisterGen1();  // Implemented in PokemonCatalog.Gen1.cs
        // RegisterGen2(); // Add when needed
    }
    
    static partial void RegisterGen1();
}
```

### PokemonCatalog.Gen1 (Partial)
```csharp
public static partial class PokemonCatalog
{
    public static readonly PokemonSpeciesData Pikachu = new PokemonSpeciesData
    {
        Name = "Pikachu",
        PokedexNumber = 25,
        PrimaryType = PokemonType.Electric,
        BaseStats = new BaseStats(35, 55, 40, 50, 50, 90)
    };
    
    static partial void RegisterGen1()
    {
        _all.Add(Pikachu);
        // ... add all Gen1 Pokemon
    }
}
```

## 5. Current Content

### PokemonCatalog (15 Pokemon - Gen 1)
| Pokemon | Pokedex # | Types | BST |
|---------|-----------|-------|-----|
| Bulbasaur, Ivysaur, Venusaur | 1-3 | Grass/Poison | 318→525 |
| Charmander, Charmeleon, Charizard | 4-6 | Fire→Fire/Flying | 309→534 |
| Squirtle, Wartortle, Blastoise | 7-9 | Water | 314→530 |
| Pikachu, Raichu | 25-26 | Electric | 320→485 |
| Eevee, Snorlax | 133, 143 | Normal | 325, 540 |
| Mewtwo, Mew | 150-151 | Psychic | 680, 600 |

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

1. **Create file** `PokemonCatalog.Gen2.cs`:
```csharp
public static partial class PokemonCatalog
{
    public static readonly PokemonSpeciesData Chikorita = new PokemonSpeciesData
    {
        Name = "Chikorita",
        PokedexNumber = 152,
        PrimaryType = PokemonType.Grass,
        BaseStats = new BaseStats(45, 49, 65, 49, 65, 45)
    };
    
    static partial void RegisterGen2()
    {
        _all.Add(Chikorita);
        // ... add all Gen2 Pokemon
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

1. **Create file** `MoveCatalog.Ice.cs`:
```csharp
public static partial class MoveCatalog
{
    public static readonly MoveData IceBeam = new MoveData
    {
        Name = "Ice Beam",
        Type = PokemonType.Ice,
        Category = MoveCategory.Special,
        Power = 90,
        Accuracy = 100,
        Effects = { new DamageEffect(), new StatusEffect(PersistentStatus.Freeze, 10) }
    };
    
    static partial void RegisterIce()
    {
        _all.Add(IceBeam);
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
```

### Pattern 2: Registry Population
```csharp
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

// All Status moves
var statusMoves = MoveCatalog.All.Where(m => m.Category == MoveCategory.Status);
```

## 8. Move Effects

Moves include composed Effects that define their behavior:

```csharp
public static readonly MoveData Flamethrower = new MoveData
{
    Name = "Flamethrower",
    Type = PokemonType.Fire,
    Power = 90,
    Effects = 
    { 
        new DamageEffect(),
        new StatusEffect(PersistentStatus.Burn, 10)
    }
};
```

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
│   └── PokemonCatalogGen1Tests.cs   # Gen1-specific tests
└── Moves/
    ├── MoveCatalogTests.cs          # General tests
    ├── MoveCatalogNormalTests.cs    # Normal moves tests
    ├── MoveCatalogFireTests.cs      # Fire moves tests
    └── ...
```

### What Tests Should Verify:
1. **General**: All enumeration, Count, RegisterAll, uniqueness
2. **Generation/Type specific**: Correct data for each Pokemon/Move
3. **Effects**: Correct effects attached to moves

## 10. Benefits of This Architecture

| Aspect | Benefit |
|--------|---------|
| **Scalability** | 1000 Pokemon = ~7 generation files, not 1 huge file |
| **Maintainability** | Each file is ~100-200 lines max |
| **Collaboration** | Different developers can work on different generations |
| **Testing** | Tests match source structure |
| **Discovery** | IDE shows all Pokemon/Moves via autocomplete |
