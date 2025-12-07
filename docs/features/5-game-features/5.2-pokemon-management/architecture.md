# Sub-Feature 5.2: Pokemon Management - Architecture

> Complete technical specification for Pokemon Party management system.

**Sub-Feature Number**: 5.2  
**Parent Feature**: [Feature 5: Game Features](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the Pokemon Party management system, which handles:

-   **Party Management**: Active Pokemon party (max 6 Pokemon)
-   **PC Storage**: Pokemon storage system (future)
-   **Catching System**: Catching wild Pokemon (future)
-   **Pokemon Organization**: Sorting, filtering, searching (future)

**Current Focus**: Party Management - Core class for managing active Pokemon teams used in battles.

## Design Principles

-   **Encapsulation**: Party encapsulates list of Pokemon with validation
-   **Combat Integration**: Compatible with `IReadOnlyList<PokemonInstance>` for battle system
-   **Validation First**: Enforces party rules (max 6, min 1 for battles)
-   **Testability First**: Pure C# classes, no Unity dependencies

---

## 1. PokemonParty (Core Class)

**Namespace**: `PokemonUltimate.Game.Managers` (planned) or `PokemonUltimate.Core.Instances` (temporary)  
**File**: `PokemonUltimate.Game/Managers/PokemonParty.cs` (planned)

Encapsulates a collection of Pokemon instances with validation and management methods.

### Core Properties

```csharp
public class PokemonParty : IReadOnlyList<PokemonInstance>
{
    private readonly List<PokemonInstance> _pokemon;

    /// <summary>
    /// Maximum party size (standard Pokemon limit).
    /// </summary>
    public const int MaxPartySize = 6;

    /// <summary>
    /// Minimum party size required for battles.
    /// </summary>
    public const int MinBattlePartySize = 1;

    /// <summary>
    /// Current number of Pokemon in party.
    /// </summary>
    public int Count => _pokemon.Count;

    /// <summary>
    /// True if party is full (at max capacity).
    /// </summary>
    public bool IsFull => Count >= MaxPartySize;

    /// <summary>
    /// True if party is empty.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// True if party has at least one non-fainted Pokemon (valid for battle).
    /// </summary>
    public bool HasActivePokemon => _pokemon.Any(p => !p.IsFainted);

    /// <summary>
    /// Gets Pokemon at specified index.
    /// </summary>
    public PokemonInstance this[int index] => _pokemon[index];
}
```

### IReadOnlyList Implementation

```csharp
// IReadOnlyList<PokemonInstance> implementation for combat system compatibility
public IEnumerator<PokemonInstance> GetEnumerator() => _pokemon.GetEnumerator();
IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
```

### Core Methods

#### Construction

```csharp
/// <summary>
/// Creates an empty party.
/// </summary>
public PokemonParty()

/// <summary>
/// Creates a party with initial Pokemon.
/// </summary>
/// <param name="pokemon">Initial Pokemon. Cannot be null.</param>
/// <exception cref="ArgumentNullException">If pokemon is null.</exception>
/// <exception cref="ArgumentException">If party exceeds MaxPartySize.</exception>
public PokemonParty(IEnumerable<PokemonInstance> pokemon)
```

#### Adding Pokemon

```csharp
/// <summary>
/// Adds a Pokemon to the party.
/// </summary>
/// <param name="pokemon">Pokemon to add. Cannot be null.</param>
/// <returns>True if added successfully, false if party is full.</returns>
/// <exception cref="ArgumentNullException">If pokemon is null.</exception>
public bool TryAdd(PokemonInstance pokemon)

/// <summary>
/// Adds a Pokemon to the party.
/// </summary>
/// <param name="pokemon">Pokemon to add. Cannot be null.</param>
/// <exception cref="ArgumentNullException">If pokemon is null.</exception>
/// <exception cref="InvalidOperationException">If party is full.</exception>
public void Add(PokemonInstance pokemon)
```

#### Removing Pokemon

```csharp
/// <summary>
/// Removes Pokemon at specified index.
/// </summary>
/// <param name="index">Index of Pokemon to remove.</param>
/// <returns>True if removed successfully, false if index invalid.</returns>
public bool TryRemoveAt(int index)

/// <summary>
/// Removes Pokemon at specified index.
/// </summary>
/// <param name="index">Index of Pokemon to remove.</param>
/// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
/// <exception cref="InvalidOperationException">If removing would leave party empty and battle is ongoing.</exception>
public void RemoveAt(int index)

/// <summary>
/// Removes specified Pokemon from party.
/// </summary>
/// <param name="pokemon">Pokemon to remove.</param>
/// <returns>True if removed successfully, false if Pokemon not found.</returns>
public bool Remove(PokemonInstance pokemon)
```

#### Reordering Pokemon

```csharp
/// <summary>
/// Swaps positions of two Pokemon in party.
/// </summary>
/// <param name="index1">First Pokemon index.</param>
/// <param name="index2">Second Pokemon index.</param>
/// <exception cref="ArgumentOutOfRangeException">If either index is invalid.</exception>
public void Swap(int index1, int index2)

/// <summary>
/// Moves Pokemon from one position to another.
/// </summary>
/// <param name="fromIndex">Source index.</param>
/// <param name="toIndex">Destination index.</param>
/// <exception cref="ArgumentOutOfRangeException">If either index is invalid.</exception>
public void Move(int fromIndex, int toIndex)
```

#### Query Methods

```csharp
/// <summary>
/// Gets the first non-fainted Pokemon (for battle lead).
/// </summary>
/// <returns>First active Pokemon, or null if all fainted.</returns>
public PokemonInstance GetFirstActivePokemon()

/// <summary>
/// Gets all non-fainted Pokemon.
/// </summary>
/// <returns>List of active Pokemon.</returns>
public IReadOnlyList<PokemonInstance> GetActivePokemon()

/// <summary>
/// Gets Pokemon at index if valid.
/// </summary>
/// <param name="index">Index to check.</param>
/// <param name="pokemon">Pokemon at index if found.</param>
/// <returns>True if index is valid.</returns>
public bool TryGetAt(int index, out PokemonInstance pokemon)

/// <summary>
/// Checks if party contains specified Pokemon.
/// </summary>
/// <param name="pokemon">Pokemon to check.</param>
/// <returns>True if Pokemon is in party.</returns>
public bool Contains(PokemonInstance pokemon)

/// <summary>
/// Gets index of specified Pokemon.
/// </summary>
/// <param name="pokemon">Pokemon to find.</param>
/// <returns>Index if found, -1 otherwise.</returns>
public int IndexOf(PokemonInstance pokemon)
```

#### Validation Methods

```csharp
/// <summary>
/// Validates party is valid for battle (at least one active Pokemon).
/// </summary>
/// <returns>True if party can participate in battle.</returns>
public bool IsValidForBattle()

/// <summary>
/// Validates party is valid for battle.
/// </summary>
/// <param name="errorMessage">Error message if invalid.</param>
/// <returns>True if valid.</returns>
public bool IsValidForBattle(out string errorMessage)
```

---

## 2. Integration with Combat System

### BattleField Integration

The `BattleField.Initialize()` method accepts `IReadOnlyList<PokemonInstance>`, which `PokemonParty` implements:

```csharp
// Current usage (direct list)
var playerParty = new List<PokemonInstance> { ... };
var enemyParty = new List<PokemonInstance> { ... };
field.Initialize(rules, playerParty, enemyParty);

// Future usage (PokemonParty)
var playerParty = new PokemonParty { ... };
var enemyParty = new PokemonParty { ... };
field.Initialize(rules, playerParty, enemyParty); // Works seamlessly!
```

### CombatEngine Integration

```csharp
// CombatEngine.Initialize() also accepts IReadOnlyList<PokemonInstance>
var playerParty = new PokemonParty();
playerParty.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 50));
playerParty.Add(PokemonFactory.Create(PokemonCatalog.Charmander, 50));

var enemyParty = new PokemonParty();
enemyParty.Add(PokemonFactory.Create(PokemonCatalog.Squirtle, 50));

engine.Initialize(BattleRules.Singles, playerParty, enemyParty, ...);
```

---

## 3. Validation Rules

### Party Size Limits

-   **Maximum**: 6 Pokemon (standard limit)
-   **Minimum for Battle**: 1 active (non-fainted) Pokemon
-   **Empty Party**: Allowed outside of battle (e.g., after releasing all Pokemon)

### Battle Validation

A party is valid for battle if:

1. Contains at least 1 Pokemon (`Count >= MinBattlePartySize`)
2. Has at least 1 non-fainted Pokemon (`HasActivePokemon == true`)

### Adding Pokemon

-   Cannot add `null` Pokemon
-   Cannot add if party is full (`Count >= MaxPartySize`)
-   Can add duplicate Pokemon instances (same PokemonInstance object can be in multiple parties if needed)

### Removing Pokemon

-   Cannot remove if index is out of range
-   Cannot remove if it would leave party empty **during an active battle** (validation context-dependent)
-   Can remove outside of battle context

---

## 4. Error Messages

New constants to add to `ErrorMessages.cs`:

```csharp
public const string PartyIsFull = "Party is full (maximum {0} Pokemon)";
public const string PartyTooSmallForBattle = "Party must have at least {0} active Pokemon for battle";
public const string CannotRemoveLastActivePokemon = "Cannot remove last active Pokemon during battle";
public const string InvalidPartyIndex = "Party index {0} is invalid (party size: {1})";
```

---

## 5. Usage Examples

### Creating and Managing Party

```csharp
// Create empty party
var party = new PokemonParty();

// Add Pokemon
party.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 25));
party.Add(PokemonFactory.Create(PokemonCatalog.Charmander, 20));

// Check if full
if (!party.IsFull)
{
    party.Add(PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15));
}

// Reorder Pokemon
party.Swap(0, 1); // Swap first two Pokemon

// Get first active Pokemon for battle
var lead = party.GetFirstActivePokemon();
```

### Battle Integration

```csharp
// Create parties
var playerParty = new PokemonParty();
playerParty.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 50));
playerParty.Add(PokemonFactory.Create(PokemonCatalog.Charmander, 50));

var enemyParty = new PokemonParty();
enemyParty.Add(PokemonFactory.Create(PokemonCatalog.Squirtle, 50));

// Validate parties
if (!playerParty.IsValidForBattle())
    throw new InvalidOperationException("Player party invalid for battle");

// Initialize battle (PokemonParty implements IReadOnlyList<PokemonInstance>)
var field = new BattleField();
field.Initialize(BattleRules.Singles, playerParty, enemyParty);
```

---

## 6. Future Extensions

### PC Storage Integration

```csharp
// Future: Move Pokemon to PC when party is full
if (party.IsFull)
{
    pcManager.StorePokemon(pokemon, box: 1);
}
```

### Party Organization

```csharp
// Future: Sort by level, name, etc.
party.SortByLevel();
party.SortByName();
party.FilterByType(PokemonType.Fire);
```

---

## 7. Related Classes

-   **PokemonInstance**: Individual Pokemon instances stored in party
-   **BattleField**: Uses `IReadOnlyList<PokemonInstance>` for battle initialization
-   **CombatEngine**: Uses `IReadOnlyList<PokemonInstance>` for battle initialization
-   **PCManager** (future): Will handle Pokemon storage when party is full

---

## 8. Testing Strategy

### Unit Tests

-   **Construction**: Empty party, party with initial Pokemon
-   **Adding**: Add to empty party, add to full party, add null Pokemon
-   **Removing**: Remove at index, remove Pokemon, remove from empty party
-   **Reordering**: Swap positions, move Pokemon
-   **Query**: Get active Pokemon, find Pokemon, check validity
-   **Validation**: Battle validation, size limits

### Integration Tests

-   **Combat Integration**: Use PokemonParty with BattleField.Initialize()
-   **Combat Integration**: Use PokemonParty with CombatEngine.Initialize()

---

**Last Updated**: 2025-01-XX
