# Data Loading & Registry Specification

## 1. Overview
To ensure the game is **Testable** and **Modular**, the code never loads files directly (no `Resources.Load` inside game logic). Instead, it asks a **Registry** for data.
This allows us to swap the "Real Unity Database" for a "Fake Test Database" instantly.

## 2. The Registry Interfaces
*Namespace: `PokemonUltimate.Core.Registry`*

We define a generic contract for accessing data.

```csharp
public interface IDataRegistry<T> where T : IIdentifiable {
    // Registration
    void Register(T item);
    void RegisterAll(IEnumerable<T> items);
    
    // Retrieval
    T Get(string id);           // Throws if not found
    T GetById(string id);       // Returns null if not found
    bool TryGet(string id, out T item);
    IEnumerable<T> GetAll();
    IEnumerable<T> All { get; } // Property alias
    
    // Queries
    bool Exists(string id);
    bool Contains(string id);   // Alias for Exists
    int Count { get; }
}
```

### PokemonRegistry (Extended Functionality)
```csharp
public class PokemonRegistry : GameDataRegistry<PokemonSpeciesData> {
    // By Pokedex
    PokemonSpeciesData GetByPokedexNumber(int number);
    bool ExistsByPokedexNumber(int number);
    IEnumerable<PokemonSpeciesData> GetByPokedexRange(int start, int end);
    
    // By Type
    IEnumerable<PokemonSpeciesData> GetByType(PokemonType type);
    
    // By Evolution Status
    IEnumerable<PokemonSpeciesData> GetDualType();
    IEnumerable<PokemonSpeciesData> GetMonoType();
    IEnumerable<PokemonSpeciesData> GetEvolvable();
    IEnumerable<PokemonSpeciesData> GetFinalForms();
}
```

### MoveRegistry (Extended Functionality)
```csharp
public class MoveRegistry : GameDataRegistry<MoveData> {
    // By Type/Category
    IEnumerable<MoveData> GetByType(PokemonType type);
    IEnumerable<MoveData> GetByCategory(MoveCategory category);
    IEnumerable<MoveData> GetDamaging();
    IEnumerable<MoveData> GetStatus();
    
    // By Power
    IEnumerable<MoveData> GetByMinPower(int minPower);
    IEnumerable<MoveData> GetByMaxPower(int maxPower);
    IEnumerable<MoveData> GetByPowerRange(int min, int max);
    
    // By Accuracy/Priority
    IEnumerable<MoveData> GetByMinAccuracy(int minAccuracy);
    IEnumerable<MoveData> GetNeverMiss();
    IEnumerable<MoveData> GetByPriority(int priority);
    IEnumerable<MoveData> GetPriorityMoves();
    
    // By Flags
    IEnumerable<MoveData> GetContactMoves();
    IEnumerable<MoveData> GetSoundMoves();
}
```

## 3. Unity Implementation: `GameDatabaseSO`
*Namespace: `PokemonGame.Unity.Data`*

We use a single **ScriptableObject** as the "Master Database". This object holds references to all Species and Moves.

```csharp
[CreateAssetMenu(fileName = "GameDatabase", menuName = "Pokemon/GameDatabase")]
public class GameDatabaseSO : ScriptableObject, IPokemonRegistry, IMoveRegistry {
    
    // Serialized Lists (The "Source of Truth" in Editor)
    [SerializeField] private List<PokemonSpeciesSO> _allSpecies;
    [SerializeField] private List<MoveSO> _allMoves;

    // Runtime Dictionaries (For O(1) Lookup)
    private Dictionary<string, PokemonSpeciesData> _speciesCache;
    private Dictionary<string, MoveData> _moveCache;

    public void Initialize() {
        // Convert SOs to POCOs and cache them
        _speciesCache = new Dictionary<string, PokemonSpeciesData>();
        foreach(var so in _allSpecies) {
            var poco = so.ToPOCO();
            _speciesCache[poco.Id] = poco;
        }
        
        // Do same for Moves...
    }

    // Interface Implementation
    public PokemonSpeciesData Get(string id) => _speciesCache[id];
    // ...
}
```

## 4. The "Easy to Add" Workflow (Auto-Population)
To avoid manually dragging 500 Pokemons into the list, we add an **Editor Button** to the `GameDatabaseSO`.

```csharp
#if UNITY_EDITOR
public void AutoPopulate() {
    // Finds all assets of type PokemonSpeciesSO in the project
    _allSpecies = AssetDatabase.FindAssets("t:PokemonSpeciesSO")
        .Select(guid => AssetDatabase.LoadAssetAtPath<PokemonSpeciesSO>(AssetDatabase.GUIDToAssetPath(guid)))
        .ToList();
        
    // Same for Moves...
    Debug.Log($"Database updated! Found {_allSpecies.Count} Pokemon.");
}
#endif
```

**Workflow:**
1.  Create new Pokemon assets anywhere in the project folder.
2.  Go to `GameDatabase` asset.
3.  Click "Refresh Database".
4.  Done.

## 5. Service Locator / Dependency Injection
How does the `CombatEngine` get the Registry? We use a simple **Service Locator** pattern for the Game Scope.

```csharp
public static class GameServices {
    public static IPokemonRegistry Pokemon { get; private set; }
    public static IMoveRegistry Moves { get; private set; }

    public static void Register(IPokemonRegistry pokemon, IMoveRegistry moves) {
        Pokemon = pokemon;
        Moves = moves;
    }
}
```

**Initialization Flow (Game Start):**
1.  `GameBootstrapper` (MonoBehaviour) wakes up.
2.  It holds a reference to `GameDatabaseSO`.
3.  It calls `database.Initialize()`.
4.  It calls `GameServices.Register(database, database)`.
5.  It loads the Main Menu.

## 6. Testability (Registry + Catalogs)
For Unit Tests, we don't use Unity or the Database. We use the built-in `GameDataRegistry` and `Catalogs`.

### Option A: Direct Catalog Access (Preferred for Tests)
```csharp
[Test]
public void Test_Pokemon_Stats() {
    // Direct access - no setup needed
    var pikachu = PokemonCatalog.Pikachu;
    
    Assert.That(pikachu.BaseStats.Speed, Is.EqualTo(90));
    Assert.That(pikachu.HasType(PokemonType.Electric), Is.True);
}
```

### Option B: Registry with Catalog Data
```csharp
[Test]
public void Test_Combat_Start() {
    // Setup Registry with catalog data
    var registry = new PokemonRegistry();
    registry.RegisterAll(PokemonCatalog.All);
    
    // Use registry for dynamic lookup
    var pikachu = registry.GetByPokedexNumber(25);
    
    Assert.That(pikachu.Name, Is.EqualTo("Pikachu"));
}
```

### Option C: Custom Test Data
```csharp
[Test]
public void Test_With_Custom_Pokemon() {
    // Create custom test data using builder
    var testMon = Pokemon.Define("TestMon", 999)
        .Type(PokemonType.Normal)
        .Stats(100, 100, 100, 100, 100, 100)
        .Build();
    
    var registry = new PokemonRegistry();
    registry.Register(testMon);
    
    Assert.That(registry.Contains("TestMon"), Is.True);
}
```

### Option D: Using Registry Query Methods
```csharp
[Test]
public void Test_Registry_Queries() {
    var pokemonRegistry = new PokemonRegistry();
    pokemonRegistry.RegisterAll(PokemonCatalog.All);
    
    var moveRegistry = new MoveRegistry();
    moveRegistry.RegisterAll(MoveCatalog.All);
    
    // Query Pokemon by type
    var fireTypes = pokemonRegistry.GetByType(PokemonType.Fire);
    Assert.That(fireTypes.Any(), Is.True);
    
    // Query moves by power
    var powerfulMoves = moveRegistry.GetByMinPower(100);
    Assert.That(powerfulMoves.All(m => m.Power >= 100), Is.True);
    
    // Query by Pokedex range
    var starters = pokemonRegistry.GetByPokedexRange(1, 9);
    Assert.That(starters.Count(), Is.EqualTo(9));
}
```

## 7. Data Validation Strategy
To ensure content integrity (as per Project Guidelines), we implement **Validators** that run on the Registry data.

```csharp
[Test]
public void Validate_All_Moves() {
    var registry = new MoveRegistry();
    registry.RegisterAll(MoveCatalog.All);
    
    foreach(var move in registry.All) {
        Assert.IsNotEmpty(move.Name, $"Move {move.Id} has no name");
        Assert.GreaterOrEqual(move.Power, 0, $"Move {move.Name} has negative power");
    }
}

[Test]
public void Validate_All_Pokemon() {
    var registry = new PokemonRegistry();
    registry.RegisterAll(PokemonCatalog.All);
    
    // Check all Pokemon have positive stats
    foreach(var pokemon in registry.All) {
        Assert.That(pokemon.BaseStats.Total, Is.GreaterThan(0));
    }
    
    // Check no duplicate Pokedex numbers
    var pokedexNumbers = registry.All.Select(p => p.PokedexNumber).ToList();
    Assert.That(pokedexNumbers.Distinct().Count(), Is.EqualTo(pokedexNumbers.Count));
}
```
