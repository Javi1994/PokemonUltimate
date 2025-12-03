# Sub-Feature 1.13: Registry System - Architecture

> Complete technical specification for data registry system for storing and retrieving game data.

**Sub-Feature Number**: 1.13  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the registry system for data access:
- **IDataRegistry<T>**: Generic registry interface
- **GameDataRegistry<T>**: Generic registry implementation
- **Specialized Registries**: PokemonRegistry, MoveRegistry

## Design Principles

- **Generic Design**: Reusable for any IIdentifiable data type
- **Dictionary-Based**: Fast O(1) lookup by ID
- **Specialized Queries**: Domain-specific queries for Pokemon and Moves
- **Testability First**: Pure C# classes, no Unity dependencies

---

## 1. IDataRegistry<T> Interface

**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/IDataRegistry.cs`

Generic interface for data registries. All data types must implement `IIdentifiable` to be stored in registries.

```csharp
public interface IDataRegistry<T> where T : IIdentifiable
{
    // Retrieval
    T Get(string id);                              // Throws if not found
    T GetById(string id);                          // Returns null if not found
    bool TryGet(string id, out T item);            // Returns false if not found
    
    // Enumeration
    IEnumerable<T> GetAll();                       // Get all items
    IEnumerable<T> All { get; }                    // Property alias for GetAll
    
    // Existence checks
    bool Exists(string id);                        // Check if exists
    bool Contains(string id);                      // Alias for Exists
    
    // Count
    int Count { get; }                             // Number of registered items
    
    // Registration
    void Register(T item);                         // Register single item
    void RegisterAll(IEnumerable<T> items);       // Register multiple items
}
```

### IIdentifiable Interface

All data types stored in registries must implement `IIdentifiable`:

```csharp
public interface IIdentifiable
{
    string Id { get; }  // Unique identifier
}
```

---

## 2. GameDataRegistry<T> Implementation

**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/GameDataRegistry.cs`

Generic implementation using `Dictionary<string, T>` for fast O(1) lookup.

```csharp
public class GameDataRegistry<T> : IDataRegistry<T> where T : class, IIdentifiable
{
    private readonly Dictionary<string, T> _items = new Dictionary<string, T>();
    
    // Registration
    public void Register(T item)
    {
        _items[item.Id] = item;
    }
    
    public void RegisterAll(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Register(item);
        }
    }
    
    // Retrieval
    public T Get(string id)
    {
        if (_items.TryGetValue(id, out var item))
        {
            return item;
        }
        throw new KeyNotFoundException($"Item '{id}' not found");
    }
    
    public T GetById(string id)
    {
        _items.TryGetValue(id, out var item);
        return item;
    }
    
    public bool TryGet(string id, out T item)
    {
        return _items.TryGetValue(id, out item);
    }
    
    // Enumeration
    public IEnumerable<T> GetAll() => _items.Values;
    public IEnumerable<T> All => _items.Values;
    
    // Existence checks
    public bool Exists(string id) => _items.ContainsKey(id);
    public bool Contains(string id) => Exists(id);
    
    // Count
    public int Count => _items.Count;
}
```

### Usage Example

```csharp
// Create registry for any IIdentifiable type
var registry = new GameDataRegistry<AbilityData>();

// Register items
registry.Register(blazeAbility);
registry.RegisterAll(abilityList);

// Retrieve items
var ability = registry.Get("blaze");              // Throws if not found
var ability = registry.GetById("blaze");          // Returns null if not found
if (registry.TryGet("blaze", out var ability))   // Safe retrieval
{
    // Use ability
}

// Check existence
if (registry.Exists("blaze"))
{
    // Ability exists
}

// Enumerate all
foreach (var ability in registry.GetAll())
{
    // Process ability
}
```

---

## 3. PokemonRegistry

**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/PokemonRegistry.cs`

Specialized registry for Pokemon that supports lookup by Name, Pokedex Number, and Type.

```csharp
public class PokemonRegistry : GameDataRegistry<PokemonSpeciesData>, IPokemonRegistry
{
    private readonly Dictionary<int, PokemonSpeciesData> _byPokedexNumber = new Dictionary<int, PokemonSpeciesData>();
    
    // Override Register to maintain Pokedex number index
    public new void Register(PokemonSpeciesData item)
    {
        base.Register(item);
        if (item.PokedexNumber > 0)
        {
            _byPokedexNumber[item.PokedexNumber] = item;
        }
    }
    
    // Name lookup (delegates to base Get)
    public PokemonSpeciesData GetByName(string name) => Get(name);
    
    // Pokedex number lookup
    public PokemonSpeciesData GetByPokedexNumber(int number)
    {
        if (_byPokedexNumber.TryGetValue(number, out var item))
        {
            return item;
        }
        throw new KeyNotFoundException($"Pokemon with Pokedex number {number} not found");
    }
    
    public bool ExistsByPokedexNumber(int number) => _byPokedexNumber.ContainsKey(number);
    
    // Type queries
    public IEnumerable<PokemonSpeciesData> GetByType(PokemonType type)
    {
        return GetAll().Where(p => p.HasType(type));
    }
    
    // Pokedex range queries
    public IEnumerable<PokemonSpeciesData> GetByPokedexRange(int start, int end)
    {
        if (start > end)
            return Enumerable.Empty<PokemonSpeciesData>();
            
        return GetAll().Where(p => p.PokedexNumber >= start && p.PokedexNumber <= end);
    }
    
    // Type composition queries
    public IEnumerable<PokemonSpeciesData> GetDualType() => GetAll().Where(p => p.IsDualType);
    public IEnumerable<PokemonSpeciesData> GetMonoType() => GetAll().Where(p => !p.IsDualType);
    
    // Evolution queries
    public IEnumerable<PokemonSpeciesData> GetEvolvable() => GetAll().Where(p => p.CanEvolve);
    public IEnumerable<PokemonSpeciesData> GetFinalForms() => GetAll().Where(p => !p.CanEvolve);
}
```

### Usage Example

```csharp
var pokemonRegistry = new PokemonRegistry();
pokemonRegistry.RegisterAll(pokemonList);

// Name lookup
var pikachu = pokemonRegistry.GetByName("Pikachu");
var pikachu = pokemonRegistry.Get("Pikachu");  // Same as above

// Pokedex number lookup
var pikachu = pokemonRegistry.GetByPokedexNumber(25);

// Type queries
var electricPokemon = pokemonRegistry.GetByType(PokemonType.Electric);

// Range queries
var gen1Pokemon = pokemonRegistry.GetByPokedexRange(1, 151);

// Composition queries
var dualTypePokemon = pokemonRegistry.GetDualType();
var monoTypePokemon = pokemonRegistry.GetMonoType();

// Evolution queries
var evolvablePokemon = pokemonRegistry.GetEvolvable();
var finalForms = pokemonRegistry.GetFinalForms();
```

---

## 4. MoveRegistry

**Namespace**: `PokemonUltimate.Core.Registry`  
**File**: `PokemonUltimate.Core/Registry/MoveRegistry.cs`

Specialized registry for Moves that supports lookup by Name and filtering by Type/Category/Power.

```csharp
public class MoveRegistry : GameDataRegistry<MoveData>, IMoveRegistry
{
    // Name lookup (delegates to base Get)
    public MoveData GetByName(string name) => Get(name);
    
    // Type queries
    public IEnumerable<MoveData> GetByType(PokemonType type)
    {
        return GetAll().Where(m => m.Type == type);
    }
    
    // Category queries
    public IEnumerable<MoveData> GetByCategory(MoveCategory category)
    {
        return GetAll().Where(m => m.Category == category);
    }
    
    public IEnumerable<MoveData> GetDamaging() => GetAll().Where(m => m.IsDamaging);
    public IEnumerable<MoveData> GetStatus() => GetAll().Where(m => m.IsStatus);
    
    // Power queries
    public IEnumerable<MoveData> GetByMinPower(int minPower)
    {
        return GetAll().Where(m => m.Power >= minPower);
    }
    
    public IEnumerable<MoveData> GetByMaxPower(int maxPower)
    {
        return GetAll().Where(m => m.Power <= maxPower);
    }
    
    public IEnumerable<MoveData> GetByPowerRange(int minPower, int maxPower)
    {
        return GetAll().Where(m => m.Power >= minPower && m.Power <= maxPower);
    }
    
    // Accuracy queries
    public IEnumerable<MoveData> GetByMinAccuracy(int minAccuracy)
    {
        return GetAll().Where(m => m.Accuracy >= minAccuracy);
    }
    
    public IEnumerable<MoveData> GetNeverMiss() => GetAll().Where(m => m.NeverMisses || m.Accuracy == 0);
    
    // Priority queries
    public IEnumerable<MoveData> GetByPriority(int priority)
    {
        return GetAll().Where(m => m.Priority == priority);
    }
    
    public IEnumerable<MoveData> GetPriorityMoves() => GetAll().Where(m => m.Priority > 0);
    
    // Flag queries
    public IEnumerable<MoveData> GetContactMoves() => GetAll().Where(m => m.MakesContact);
    public IEnumerable<MoveData> GetSoundMoves() => GetAll().Where(m => m.IsSoundBased);
}
```

### Usage Example

```csharp
var moveRegistry = new MoveRegistry();
moveRegistry.RegisterAll(moveList);

// Name lookup
var thunderbolt = moveRegistry.GetByName("Thunderbolt");
var thunderbolt = moveRegistry.Get("Thunderbolt");  // Same as above

// Type queries
var electricMoves = moveRegistry.GetByType(PokemonType.Electric);

// Category queries
var physicalMoves = moveRegistry.GetByCategory(MoveCategory.Physical);
var specialMoves = moveRegistry.GetByCategory(MoveCategory.Special);
var statusMoves = moveRegistry.GetByCategory(MoveCategory.Status);
var damagingMoves = moveRegistry.GetDamaging();

// Power queries
var strongMoves = moveRegistry.GetByMinPower(80);
var weakMoves = moveRegistry.GetByMaxPower(50);
var mediumMoves = moveRegistry.GetByPowerRange(50, 80);

// Accuracy queries
var accurateMoves = moveRegistry.GetByMinAccuracy(90);
var neverMissMoves = moveRegistry.GetNeverMiss();

// Priority queries
var priorityMoves = moveRegistry.GetPriorityMoves();
var quickAttack = moveRegistry.GetByPriority(1);

// Flag queries
var contactMoves = moveRegistry.GetContactMoves();
var soundMoves = moveRegistry.GetSoundMoves();
```

---

## 5. Specialized Interfaces

### IPokemonRegistry

**File**: `PokemonUltimate.Core/Registry/IPokemonRegistry.cs`

Interface for Pokemon-specific queries.

```csharp
public interface IPokemonRegistry : IDataRegistry<PokemonSpeciesData>
{
    PokemonSpeciesData GetByName(string name);
    PokemonSpeciesData GetByPokedexNumber(int number);
    bool ExistsByPokedexNumber(int number);
    IEnumerable<PokemonSpeciesData> GetByType(PokemonType type);
    IEnumerable<PokemonSpeciesData> GetByPokedexRange(int start, int end);
    IEnumerable<PokemonSpeciesData> GetDualType();
    IEnumerable<PokemonSpeciesData> GetMonoType();
    IEnumerable<PokemonSpeciesData> GetEvolvable();
    IEnumerable<PokemonSpeciesData> GetFinalForms();
}
```

### IMoveRegistry

**File**: `PokemonUltimate.Core/Registry/IMoveRegistry.cs`

Interface for Move-specific queries.

```csharp
public interface IMoveRegistry : IDataRegistry<MoveData>
{
    MoveData GetByName(string name);
    IEnumerable<MoveData> GetByType(PokemonType type);
    IEnumerable<MoveData> GetByCategory(MoveCategory category);
    IEnumerable<MoveData> GetDamaging();
    IEnumerable<MoveData> GetStatus();
    IEnumerable<MoveData> GetByMinPower(int minPower);
    IEnumerable<MoveData> GetByMaxPower(int maxPower);
    IEnumerable<MoveData> GetByPowerRange(int minPower, int maxPower);
    IEnumerable<MoveData> GetByMinAccuracy(int minAccuracy);
    IEnumerable<MoveData> GetNeverMiss();
    IEnumerable<MoveData> GetByPriority(int priority);
    IEnumerable<MoveData> GetPriorityMoves();
    IEnumerable<MoveData> GetContactMoves();
    IEnumerable<MoveData> GetSoundMoves();
}
```

---

## 6. Related Sub-Features

- **[1.9: Interfaces Base](../1.9-interfaces-base/)** - Uses IIdentifiable
- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - PokemonRegistry stores PokemonSpeciesData
- **[1.2: Move Data](../1.2-move-data/)** - MoveRegistry stores MoveData

---

## Related Documents

- **[Parent Architecture](../architecture.md#113-registry-system)** - Feature-level technical specification
- **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** - Code organization
- **[Sub-Feature README](README.md)** - Overview and quick navigation

---

**Last Updated**: 2025-01-XX

