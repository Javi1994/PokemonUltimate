# Data Loading & Registry Specification

## 1. Overview
To ensure the game is **Testable** and **Modular**, the code never loads files directly (no `Resources.Load` inside game logic). Instead, it asks a **Registry** for data.
This allows us to swap the "Real Unity Database" for a "Fake Test Database" instantly.

## 2. The Registry Interfaces
*Namespace: `PokemonGame.Core.Interfaces`*

We define a generic contract for accessing data.

```csharp
public interface IDataRegistry<T> {
    T Get(string id);
    IEnumerable<T> GetAll();
    bool Exists(string id);
}

// Specific Registries (to keep dependencies clean)
public interface IPokemonRegistry : IDataRegistry<PokemonSpeciesData> { }
public interface IMoveRegistry : IDataRegistry<MoveData> { }
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

## 6. Testability (The "Mock Registry")
For Unit Tests, we don't use Unity or the Database. We use a Mock.

```csharp
public class MockPokemonRegistry : IPokemonRegistry {
    private List<PokemonSpeciesData> _fakeData = new List<PokemonSpeciesData>();

    public void Add(PokemonSpeciesData data) => _fakeData.Add(data);

    public PokemonSpeciesData Get(string id) => _fakeData.First(p => p.Id == id);
}

[Test]
public void Test_Combat_Start() {
    // Setup Fake Data
    var registry = new MockPokemonRegistry();
    registry.Add(new PokemonSpeciesData { Id = "pikachu", BaseStats = ... });
    
    // Inject
    GameServices.Register(registry, null);
    
    // Run Test
    var pokemon = PokemonFactory.Create(registry.Get("pikachu"), 5);
    Assert.IsNotNull(pokemon);
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
