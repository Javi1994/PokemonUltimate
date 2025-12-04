# Sub-Feature 1.11: Evolution System - Architecture

> Complete technical specification for evolution paths and conditions.

**Sub-Feature Number**: 1.11  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the evolution system:
- **Evolution**: Evolution path definition
- **IEvolutionCondition**: Base interface for evolution conditions
- **EvolutionConditions** (6 classes): Level, Item, Trade, Friendship, TimeOfDay, KnowsMove

## Design Principles

- **Composition Pattern**: Evolutions are composed of multiple conditions, not subclassed
- **All Conditions Must Be Met**: All conditions in an evolution are AND-ed together
- **Testability First**: Pure C# classes, no Unity dependencies

---

## 1. Evolution Class

**Namespace**: `PokemonUltimate.Core.Evolution`  
**File**: `PokemonUltimate.Core/Evolution/Evolution.cs`

Represents a possible evolution for a Pokemon species. Contains the target species and the conditions required.

```csharp
public class Evolution
{
    /// <summary>
    /// The Pokemon species this evolution leads to.
    /// </summary>
    public PokemonSpeciesData Target { get; set; }
    
    /// <summary>
    /// All conditions that must be met for this evolution.
    /// All conditions are AND-ed together.
    /// </summary>
    public List<IEvolutionCondition> Conditions { get; set; } = new List<IEvolutionCondition>();
    
    /// <summary>
    /// Human-readable description of all conditions.
    /// </summary>
    public string Description => string.Join(" + ", Conditions.Select(c => c.Description));
    
    /// <summary>
    /// Check if this evolution has a specific condition type.
    /// </summary>
    public bool HasCondition<T>() where T : IEvolutionCondition;
    
    /// <summary>
    /// Get a specific condition type (or null if not found).
    /// </summary>
    public T GetCondition<T>() where T : class, IEvolutionCondition;
    
    /// <summary>
    /// Checks if a specific Pokemon instance meets all conditions for this evolution.
    /// </summary>
    public bool CanEvolve(PokemonInstance pokemon);
}
```

### Usage Example

```csharp
// Simple level evolution
var evolution = new Evolution
{
    Target = charizardSpecies,
    Conditions = new List<IEvolutionCondition>
    {
        new LevelCondition(36)
    }
};

// Complex evolution (Espeon: Friendship + Daytime)
var espeonEvolution = new Evolution
{
    Target = espeonSpecies,
    Conditions = new List<IEvolutionCondition>
    {
        new FriendshipCondition(220),
        new TimeOfDayCondition(TimeOfDay.Day)
    }
};
```

---

## 2. IEvolutionCondition Interface

**Namespace**: `PokemonUltimate.Core.Evolution`  
**File**: `PokemonUltimate.Core/Evolution/IEvolutionCondition.cs`

Base interface for all evolution conditions. Follows the Composition Pattern - evolutions are composed of multiple conditions.

```csharp
public interface IEvolutionCondition
{
    /// <summary>
    /// The type of this condition.
    /// </summary>
    EvolutionConditionType ConditionType { get; }
    
    /// <summary>
    /// Human-readable description of this condition.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Checks if this condition is met by the given Pokemon instance.
    /// </summary>
    bool IsMet(PokemonInstance pokemon);
}
```

---

## 3. EvolutionConditionType Enum

**Namespace**: `PokemonUltimate.Core.Evolution`  
**File**: `PokemonUltimate.Core/Evolution/EvolutionConditionType.cs`

Types of conditions that can trigger evolution.

```csharp
public enum EvolutionConditionType
{
    /// <summary>Reach a minimum level.</summary>
    Level,
    
    /// <summary>Use a specific item (stone, etc.).</summary>
    UseItem,
    
    /// <summary>Trade with another player.</summary>
    Trade,
    
    /// <summary>High friendship value.</summary>
    Friendship,
    
    /// <summary>Specific time of day.</summary>
    TimeOfDay,
    
    /// <summary>Must know a specific move.</summary>
    KnowsMove,
    
    /// <summary>Must be a specific gender.</summary>
    Gender,
    
    /// <summary>Must be in a specific location.</summary>
    Location,
    
    /// <summary>Must be holding a specific item.</summary>
    HeldItem
}
```

---

## 4. Evolution Condition Classes (6 Implementations)

**Namespace**: `PokemonUltimate.Core.Evolution.Conditions`  
**Files**: `PokemonUltimate.Core/Evolution/Conditions/*.cs`

### 4.1 LevelCondition

**File**: `LevelCondition.cs`

Evolution condition: Pokemon must reach a minimum level.

```csharp
public class LevelCondition : IEvolutionCondition
{
    public EvolutionConditionType ConditionType => EvolutionConditionType.Level;
    public string Description => $"Reach level {MinLevel}";
    
    /// <summary>
    /// Minimum level required for evolution.
    /// </summary>
    public int MinLevel { get; set; }
    
    public bool IsMet(PokemonInstance pokemon)
    {
        return pokemon != null && pokemon.Level >= MinLevel;
    }
}
```

**Usage**: Most common evolution method (e.g., Charmander evolves at level 16).

---

### 4.2 ItemCondition

**File**: `ItemCondition.cs`

Evolution condition: Use a specific evolution item (stone, etc.). This condition returns false by default - items must be used explicitly.

```csharp
public class ItemCondition : IEvolutionCondition
{
    public EvolutionConditionType ConditionType => EvolutionConditionType.UseItem;
    public string Description => $"Use {ItemName}";
    
    /// <summary>
    /// Name of the required item.
    /// </summary>
    public string ItemName { get; set; } = string.Empty;
    
    /// <summary>
    /// Item conditions are never automatically met - they require explicit item use.
    /// </summary>
    public bool IsMet(PokemonInstance pokemon)
    {
        return false; // Requires explicit item use
    }
    
    /// <summary>
    /// Checks if the condition is met when a specific item is used.
    /// </summary>
    public bool IsMet(PokemonInstance pokemon, string itemUsed)
    {
        return pokemon != null && 
               !string.IsNullOrEmpty(itemUsed) && 
               itemUsed.Equals(ItemName, StringComparison.OrdinalIgnoreCase);
    }
}
```

**Usage**: Stone evolutions (e.g., Pikachu evolves with Thunder Stone).

---

### 4.3 TradeCondition

**File**: `TradeCondition.cs`

Evolution condition: Trade with another player. This condition returns false by default - trades must be performed explicitly.

```csharp
public class TradeCondition : IEvolutionCondition
{
    public EvolutionConditionType ConditionType => EvolutionConditionType.Trade;
    public string Description => "Trade";
    
    /// <summary>
    /// Trade conditions are never automatically met - they require explicit trade.
    /// </summary>
    public bool IsMet(PokemonInstance pokemon)
    {
        return false; // Requires explicit trade
    }
}
```

**Usage**: Trade evolutions (e.g., Kadabra evolves when traded).

---

### 4.4 FriendshipCondition

**File**: `FriendshipCondition.cs`

Evolution condition: Pokemon must have high friendship value.

```csharp
public class FriendshipCondition : IEvolutionCondition
{
    public EvolutionConditionType ConditionType => EvolutionConditionType.Friendship;
    public string Description => $"Friendship >= {MinFriendship}";
    
    /// <summary>
    /// Minimum friendship value required (default 220).
    /// </summary>
    public int MinFriendship { get; set; } = 220;
    
    public bool IsMet(PokemonInstance pokemon)
    {
        return pokemon != null && pokemon.Friendship >= MinFriendship;
    }
}
```

**Usage**: Friendship evolutions (e.g., Eevee evolves with high friendship).

---

### 4.5 TimeOfDayCondition

**File**: `TimeOfDayCondition.cs`

Evolution condition: Specific time of day. This condition returns false by default - time context must be provided explicitly.

```csharp
public class TimeOfDayCondition : IEvolutionCondition
{
    public EvolutionConditionType ConditionType => EvolutionConditionType.TimeOfDay;
    public string Description => $"During {TimeOfDay}";
    
    /// <summary>
    /// Required time of day.
    /// </summary>
    public TimeOfDay TimeOfDay { get; set; }
    
    /// <summary>
    /// Time conditions are never automatically met - they require time context.
    /// </summary>
    public bool IsMet(PokemonInstance pokemon)
    {
        return false; // Requires time context
    }
    
    /// <summary>
    /// Checks if the condition is met at a specific time of day.
    /// </summary>
    public bool IsMet(PokemonInstance pokemon, TimeOfDay currentTime)
    {
        return pokemon != null && currentTime == TimeOfDay;
    }
}
```

**Usage**: Time-based evolutions (e.g., Espeon evolves during day, Umbreon at night).

---

### 4.6 KnowsMoveCondition

**File**: `KnowsMoveCondition.cs`

Evolution condition: Pokemon must know a specific move.

```csharp
public class KnowsMoveCondition : IEvolutionCondition
{
    public EvolutionConditionType ConditionType => EvolutionConditionType.KnowsMove;
    public string Description => $"Knows {MoveName}";
    
    /// <summary>
    /// Name of the required move.
    /// </summary>
    public string MoveName { get; set; } = string.Empty;
    
    public bool IsMet(PokemonInstance pokemon)
    {
        return pokemon != null && 
               pokemon.Moves.Any(m => m.Move.Name.Equals(MoveName, StringComparison.OrdinalIgnoreCase));
    }
}
```

**Usage**: Move-based evolutions (e.g., some Pokemon evolve when they know a specific move).

---

## 5. Evolution Builder

**Namespace**: `PokemonUltimate.Core.Builders`  
**File**: `PokemonUltimate.Core/Builders/EvolutionBuilder.cs`

Fluent builder for creating evolution paths.

```csharp
public class EvolutionBuilder
{
    private readonly Evolution _evolution = new Evolution();
    
    public static EvolutionBuilder Create(PokemonSpeciesData target)
    {
        return new EvolutionBuilder { _evolution.Target = target };
    }
    
    // Condition methods
    public EvolutionBuilder AtLevel(int level);
    public EvolutionBuilder WithItem(string itemName);
    public EvolutionBuilder WithTrade();
    public EvolutionBuilder WithFriendship(int minFriendship = 220);
    public EvolutionBuilder During(TimeOfDay timeOfDay);
    public EvolutionBuilder KnowsMove(string moveName);
    
    public Evolution Build() => _evolution;
}
```

### Usage Example

```csharp
// Simple level evolution
var evolution = EvolutionBuilder.Create(charizardSpecies)
    .AtLevel(36)
    .Build();

// Complex evolution (Espeon)
var espeonEvolution = EvolutionBuilder.Create(espeonSpecies)
    .WithFriendship(220)
    .During(TimeOfDay.Day)
    .Build();
```

---

## 6. Evolution Examples

### Simple Level Evolution

```csharp
// Charmander evolves at level 16
var charmeleonEvolution = new Evolution
{
    Target = charmeleonSpecies,
    Conditions = new List<IEvolutionCondition>
    {
        new LevelCondition(16)
    }
};
```

### Stone Evolution

```csharp
// Pikachu evolves with Thunder Stone
var raichuEvolution = new Evolution
{
    Target = raichuSpecies,
    Conditions = new List<IEvolutionCondition>
    {
        new ItemCondition("Thunder Stone")
    }
};
```

### Complex Evolution (Multiple Conditions)

```csharp
// Espeon: High friendship + Daytime
var espeonEvolution = new Evolution
{
    Target = espeonSpecies,
    Conditions = new List<IEvolutionCondition>
    {
        new FriendshipCondition(220),
        new TimeOfDayCondition(TimeOfDay.Day)
    }
};
```

### Trade Evolution

```csharp
// Kadabra evolves when traded
var alakazamEvolution = new Evolution
{
    Target = alakazamSpecies,
    Conditions = new List<IEvolutionCondition>
    {
        new TradeCondition()
    }
};
```

---

## 7. Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Evolution paths in PokemonSpeciesData
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - EvolutionBuilder for creating evolution paths

---

## Related Documents

- **[Parent Architecture](../architecture.md#17-evolution-system)** - Feature-level technical specification
- **[Parent Code Location](../code_location.md#grupo-c-supporting-systems)** - Code organization
- **[Sub-Feature README](README.md)** - Overview and quick navigation

---

**Last Updated**: 2025-01-XX

