# Data Loading & Registry Specification

## 1. Overview
To ensure the game is **Testable** and **Modular**, the code never loads files directly (no `Resources.Load` inside game logic). Instead, it asks a **Registry** for data.
This allows us to swap the "Real Unity Database" for a "Fake Test Database" instantly.

## 2. The Registry Interfaces
*Namespace: `PokemonUltimate.Core.Registry`*

We define a generic contract for accessing data.

```csharp
public interface IDataRegistry<T> where T : IIdentifiable {
    void Register(T item);
    T GetByName(string name);
    IEnumerable<T> GetAll();
    bool Exists(string name);
    int Count { get; }
}

// Specific Registries (extended functionality)
public interface IPokemonRegistry : IDataRegistry<PokemonSpeciesData> {
    PokemonSpeciesData GetByPokedexNumber(int number);
    bool ExistsByPokedexNumber(int number);
}

public interface IMoveRegistry : IDataRegistry<MoveData> {
    IEnumerable<MoveData> GetByType(PokemonType type);
    IEnumerable<MoveData> GetByCategory(MoveCategory category);
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
    PokemonCatalog.RegisterAll(registry);
    
    // Use registry for dynamic lookup
    var pikachu = registry.GetByPokedexNumber(25);
    
    // Future: PokemonFactory.Create(pikachu, level: 5)
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
    
    Assert.That(registry.Exists("TestMon"), Is.True);
}
```

## 7. Data Validation Strategy
To ensure content integrity (as per Project Guidelines), we implement **Validators** that run on the Registry data.

```csharp
[Test]
public void Validate_All_Moves() {
    var registry = new GameDatabaseSO(); // Load real data
    registry.Initialize();
    
    foreach(var move in registry.GetAllMoves()) {
        Assert.IsNotEmpty(move.Name, $"Move {move.Id} has no name");
        Assert.GreaterOrEqual(move.Power, 0, $"Move {move.Name} has negative power");
        // Verify Effects composition
        if (move.TargetScope == TargetScope.Self) {
            Assert.IsTrue(move.Effects.Any(e => e.TargetSelf), $"Self-move {move.Name} has no self-targeting effects");
        }
    }
}
```
