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

### Catalog vs Registry
| Aspect | Catalog | Registry |
|--------|---------|----------|
| Storage | Static readonly fields | Runtime Dictionary |
| Access | `PokemonCatalog.Pikachu` | `registry.GetByName("Pikachu")` |
| Lookup | By field name only | By Name, Number, Type, etc. |
| Mutability | Immutable | Can add at runtime |
| Use Case | Known data, compile-time | Dynamic lookup, runtime |

## 3. Structure

### `PokemonCatalog`
*Namespace: `PokemonUltimate.Core.Catalogs`*

```csharp
public static class PokemonCatalog
{
    // Direct access to Pokemon data
    public static readonly PokemonSpeciesData Pikachu = new PokemonSpeciesData
    {
        Name = "Pikachu",
        PokedexNumber = 25
    };
    
    // Enumerate all defined Pokemon
    public static IEnumerable<PokemonSpeciesData> All { get; }
    
    // Bulk register into a registry
    public static void RegisterAll(IPokemonRegistry registry);
    
    // Count of defined Pokemon
    public static int Count { get; }
}
```

### `MoveCatalog`
*Namespace: `PokemonUltimate.Core.Catalogs`*

```csharp
public static class MoveCatalog
{
    // Direct access to Move data
    public static readonly MoveData Thunderbolt = new MoveData
    {
        Name = "Thunderbolt",
        Type = PokemonType.Electric,
        Category = MoveCategory.Special,
        Power = 90,
        Accuracy = 100,
        MaxPP = 15,
        Priority = 0,
        TargetScope = TargetScope.SingleEnemy
    };
    
    // Enumerate all defined Moves
    public static IEnumerable<MoveData> All { get; }
    
    // Bulk register into a registry
    public static void RegisterAll(IMoveRegistry registry);
    
    // Count of defined Moves
    public static int Count { get; }
}
```

## 4. Current Content

### PokemonCatalog (15 Pokemon)
| Pokemon | Pokedex # | Category |
|---------|-----------|----------|
| Bulbasaur, Ivysaur, Venusaur | 1, 2, 3 | Grass Starter Line |
| Charmander, Charmeleon, Charizard | 4, 5, 6 | Fire Starter Line |
| Squirtle, Wartortle, Blastoise | 7, 8, 9 | Water Starter Line |
| Pikachu, Raichu | 25, 26 | Electric |
| Eevee | 133 | Normal |
| Snorlax | 143 | Normal |
| Mewtwo, Mew | 150, 151 | Legendary/Mythical |

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

## 5. Usage Patterns

### Pattern 1: Direct Access
```csharp
// In tests or when you know exactly what you need
var pikachu = PokemonCatalog.Pikachu;
var thunderbolt = MoveCatalog.Thunderbolt;

Assert.That(pikachu.PokedexNumber, Is.EqualTo(25));
Assert.That(thunderbolt.Power, Is.EqualTo(90));
```

### Pattern 2: Registry Population
```csharp
// At game startup
var pokemonRegistry = new PokemonRegistry();
var moveRegistry = new MoveRegistry();

PokemonCatalog.RegisterAll(pokemonRegistry);
MoveCatalog.RegisterAll(moveRegistry);

// Now use registry for dynamic lookups
var pokemon = pokemonRegistry.GetByPokedexNumber(25);
var fireMoves = moveRegistry.GetByType(PokemonType.Fire);
```

### Pattern 3: Iteration
```csharp
// List all available Pokemon
foreach (var pokemon in PokemonCatalog.All)
{
    Console.WriteLine($"#{pokemon.PokedexNumber}: {pokemon.Name}");
}

// Filter moves by category
var statusMoves = MoveCatalog.All.Where(m => m.Category == MoveCategory.Status);
```

## 6. Adding New Content

### Adding a Pokemon
```csharp
// 1. Add the static field in PokemonCatalog
public static readonly PokemonSpeciesData Gengar = new PokemonSpeciesData
{
    Name = "Gengar",
    PokedexNumber = 94
};

// 2. Add to the All property's yield returns
yield return Gengar;

// 3. Update Count
public static int Count => 16; // was 15
```

### Adding a Move
```csharp
// 1. Add the static field in MoveCatalog
public static readonly MoveData ShadowBall = new MoveData
{
    Name = "Shadow Ball",
    Type = PokemonType.Ghost,
    Category = MoveCategory.Special,
    Power = 80,
    Accuracy = 100,
    MaxPP = 15,
    Priority = 0,
    TargetScope = TargetScope.SingleEnemy
};

// 2. Add to the All property's yield returns
yield return ShadowBall;

// 3. Update Count
public static int Count => 21; // was 20
```

## 7. Testing Strategy

### Catalog Tests Should Verify:
1. **Direct Access**: Fields return correct data
2. **All Enumeration**: Returns correct count
3. **Uniqueness**: All names/numbers are unique
4. **RegisterAll**: Populates registry correctly
5. **Consistency**: Catalog data matches registry data after registration

### Example Test
```csharp
[Test]
public void Test_RegisterAll_Pokemon_Are_Retrievable()
{
    var registry = new PokemonRegistry();
    PokemonCatalog.RegisterAll(registry);
    
    Assert.Multiple(() =>
    {
        Assert.That(registry.GetByName("Pikachu"), Is.SameAs(PokemonCatalog.Pikachu));
        Assert.That(registry.GetByPokedexNumber(25), Is.SameAs(PokemonCatalog.Pikachu));
    });
}
```

## 8. Future Enhancements

### Planned
- Add BaseStats, Types, Abilities to Pokemon in catalog
- Add Effects list to Moves in catalog
- Consider grouping by generation or region

### Considered (Not Planned)
- **JSON/YAML Loading**: Would lose compile-time safety and IDE support
- **ScriptableObjects**: Only needed when Unity integration happens
- **Database**: Overkill for static game data

