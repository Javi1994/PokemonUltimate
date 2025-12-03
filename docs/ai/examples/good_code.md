# 📖 Code Examples: The Right Way

> **Reference these examples when writing new code.**
> These patterns are extracted from our codebase and represent our standards.

---

## 🏗️ Blueprint Pattern (Immutable Data)

```csharp
/// <summary>
/// Immutable blueprint for Pokemon species data.
/// </summary>
public class PokemonSpeciesData : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public int PokedexNumber { get; }
    public PokemonType PrimaryType { get; }
    public PokemonType? SecondaryType { get; }
    public BaseStats BaseStats { get; }
    
    // All properties are read-only
    // No setters, no mutation methods
    // Created once via builder, never modified
}
```

**Key Points:**
- All properties are `{ get; }` only
- No public setters
- Immutable after creation
- Use builder pattern for construction

---

## 🔄 Instance Pattern (Mutable Runtime State)

```csharp
/// <summary>
/// Mutable runtime instance of a Pokemon.
/// </summary>
public partial class PokemonInstance
{
    // Reference to immutable blueprint
    public PokemonSpeciesData Species { get; private set; }
    
    // Mutable state
    public int CurrentHP { get; private set; }
    public int Level { get; private set; }
    
    // Methods that modify state
    public void TakeDamage(int amount)
    {
        if (amount < 0)
            throw new ArgumentException(ErrorMessages.AmountCannotBeNegative, nameof(amount));
        
        CurrentHP = Math.Max(0, CurrentHP - amount);
    }
}
```

**Key Points:**
- References immutable blueprint via `Species`
- Mutable properties have `private set`
- State changes through explicit methods
- Methods validate inputs

---

## 🎯 Effect Composition (IMoveEffect)

```csharp
/// <summary>
/// Drain effect that restores HP based on damage dealt.
/// </summary>
public class DrainEffect : IMoveEffect
{
    public EffectType Type => EffectType.Drain;
    
    /// <summary>
    /// Percentage of damage to drain (e.g., 50 for Giga Drain).
    /// </summary>
    public int DrainPercent { get; }
    
    // Alias for clarity
    public int Percent => DrainPercent;
    
    public DrainEffect(int drainPercent)
    {
        if (drainPercent <= 0 || drainPercent > 100)
            throw new ArgumentOutOfRangeException(nameof(drainPercent));
        
        DrainPercent = drainPercent;
    }
}
```

**Key Points:**
- Implements `IMoveEffect`
- Single responsibility
- Validation in constructor
- Descriptive property names with aliases

---

## 🏭 Factory with Builder

```csharp
public static class PokemonFactory
{
    /// <summary>
    /// Quick creation with defaults.
    /// </summary>
    public static PokemonInstance Create(PokemonSpeciesData species, int level)
    {
        return CreateBuilder(species)
            .WithLevel(level)
            .Build();
    }
    
    /// <summary>
    /// Returns builder for full customization.
    /// </summary>
    public static PokemonInstanceBuilder CreateBuilder(PokemonSpeciesData species)
    {
        if (species == null)
            throw new ArgumentNullException(nameof(species));
        
        return new PokemonInstanceBuilder(species);
    }
}
```

**Key Points:**
- Simple factory method for common cases
- Builder method for complex configuration
- Validation at entry point

---

## 🔍 Registry Query Methods

```csharp
public class PokemonRegistry : GameDataRegistry<PokemonSpeciesData>, IPokemonRegistry
{
    /// <summary>
    /// Gets Pokemon by type. Returns empty if none found.
    /// </summary>
    public IEnumerable<PokemonSpeciesData> GetByType(PokemonType type)
    {
        return All.Where(p => p.PrimaryType == type || p.SecondaryType == type);
    }
    
    /// <summary>
    /// Gets Pokemon by Pokedex number. Throws if not found.
    /// </summary>
    public PokemonSpeciesData GetByPokedexNumber(int number)
    {
        return All.FirstOrDefault(p => p.PokedexNumber == number)
            ?? throw new KeyNotFoundException(
                ErrorMessages.PokemonWithPokedexNotFound.Format(number));
    }
}
```

**Key Points:**
- Query methods return `IEnumerable<T>` (can be empty)
- Lookup methods throw if not found
- Use `ErrorMessages` for exception messages

---

## ✅ Test Structure

```csharp
[TestFixture]
public class StatCalculatorTests
{
    // Descriptive name: Method_Scenario_Expected
    [Test]
    public void CalculateHP_Level50With31IV_ReturnsCorrectValue()
    {
        // Arrange
        const int baseHP = 80;
        const int level = 50;
        const int iv = 31;
        const int ev = 0;
        
        // Act
        var result = StatCalculator.CalculateHP(baseHP, level, iv, ev);
        
        // Assert
        Assert.That(result, Is.EqualTo(145));
    }
    
    [Test]
    public void CalculateHP_NegativeBaseStat_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            StatCalculator.CalculateHP(-1, 50, 31, 0));
    }
}
```

**Key Points:**
- `[TestFixture]` and `[Test]` attributes
- Descriptive test names
- AAA pattern (Arrange, Act, Assert)
- Test both success and failure cases

---

## 🚨 Validation Pattern

```csharp
public static int CalculateStat(int baseStat, int level, int iv, int ev, double natureModifier)
{
    // Guard clauses at the start
    if (baseStat < 0)
        throw new ArgumentException(ErrorMessages.BaseStatCannotBeNegative, nameof(baseStat));
    
    if (level < 1 || level > 100)
        throw new ArgumentOutOfRangeException(nameof(level), ErrorMessages.LevelMustBeBetween1And100);
    
    if (iv < 0 || iv > 31)
        throw new ArgumentOutOfRangeException(nameof(iv), ErrorMessages.IVMustBeBetween0And31);
    
    if (ev < 0 || ev > 252)
        throw new ArgumentOutOfRangeException(nameof(ev), ErrorMessages.EVMustBeBetween0And252);
    
    // Main logic after validation
    var numerator = (2 * baseStat + iv + ev / 4) * level;
    var result = numerator / 100 + 5;
    return (int)(result * natureModifier);
}
```

**Key Points:**
- Guard clauses first
- Use `ErrorMessages` constants
- Throw specific exception types
- Clear parameter names in exceptions

