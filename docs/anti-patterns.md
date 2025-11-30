# ❌ Anti-Patterns: What NOT To Do

> **Avoid these patterns in the codebase.**
> If you see these in code review, fix them.

---

## 🔤 Magic Strings

### ❌ BAD
```csharp
throw new Exception("Pokemon not found");
throw new ArgumentException("Level must be between 1 and 100");
Console.WriteLine("It's super effective!");
```

### ✅ GOOD
```csharp
throw new KeyNotFoundException(ErrorMessages.PokemonNotFound);
throw new ArgumentOutOfRangeException(nameof(level), ErrorMessages.LevelMustBeBetween1And100);
Console.WriteLine(GameMessages.SuperEffective);
```

**Rule**: All user-facing strings and error messages go in `Constants/ErrorMessages.cs` or `Constants/GameMessages.cs`.

---

## 🎣 Unnecessary Try-Catch

### ❌ BAD
```csharp
public PokemonSpeciesData GetPokemon(string id)
{
    try
    {
        return _registry.Get(id);
    }
    catch (KeyNotFoundException)
    {
        return null; // Silent failure!
    }
}
```

### ✅ GOOD
```csharp
public PokemonSpeciesData GetPokemon(string id)
{
    return _registry.Get(id); // Let it throw if not found
}

// OR if null is valid:
public PokemonSpeciesData? GetPokemonOrDefault(string id)
{
    return _registry.TryGet(id, out var pokemon) ? pokemon : null;
}
```

**Rule**: Don't catch exceptions just to hide errors. Let them propagate or use explicit Try-pattern.

---

## 🔇 Silent Failures

### ❌ BAD
```csharp
public bool TakeDamage(int amount)
{
    if (amount < 0) return false; // Silent failure
    CurrentHP -= amount;
    return true;
}
```

### ✅ GOOD
```csharp
public void TakeDamage(int amount)
{
    if (amount < 0)
        throw new ArgumentException(ErrorMessages.AmountCannotBeNegative, nameof(amount));
    
    CurrentHP = Math.Max(0, CurrentHP - amount);
}
```

**Rule**: Invalid inputs should throw exceptions, not return false silently.

---

## 🧱 God Classes

### ❌ BAD
```csharp
public class PokemonInstance
{
    // 2000+ lines in one file
    // Battle logic, evolution, leveling, stats, moves, items, abilities...
}
```

### ✅ GOOD
```csharp
// Split into partial classes by responsibility:
// PokemonInstance.cs - Core properties
// PokemonInstance.Battle.cs - Battle methods
// PokemonInstance.LevelUp.cs - Level up logic
// PokemonInstance.Evolution.cs - Evolution logic
```

**Rule**: Use partial classes to split large classes by responsibility.

---

## 🔄 Mutable Blueprints

### ❌ BAD
```csharp
public class MoveData
{
    public int Power { get; set; } // Mutable!
    public int PP { get; set; }    // Can be changed!
}
```

### ✅ GOOD
```csharp
public class MoveData
{
    public int Power { get; }  // Immutable
    public int BasePP { get; } // Immutable
    
    public MoveData(int power, int pp)
    {
        Power = power;
        BasePP = pp;
    }
}
```

**Rule**: Blueprints (data definitions) should be immutable. Use Instances for mutable state.

---

## 🔢 Magic Numbers

### ❌ BAD
```csharp
if (level > 100) throw new Exception("Invalid level");
var stab = damage * 1.5;
if (iv > 31) throw new Exception("Invalid IV");
```

### ✅ GOOD
```csharp
private const int MaxLevel = 100;
private const int MaxIV = 31;
private const double StabMultiplier = 1.5;

if (level > MaxLevel) 
    throw new ArgumentOutOfRangeException(nameof(level));
var stab = damage * StabMultiplier;
```

**Rule**: Use named constants for numeric values that have meaning.

---

## 📛 Poor Naming

### ❌ BAD
```csharp
public class PM { }           // What is PM?
public int Calc(int x) { }    // Calculate what?
private int _d;               // What does d mean?
public void DoIt() { }        // Do what?
```

### ✅ GOOD
```csharp
public class PokemonInstance { }
public int CalculateDamage(int basePower) { }
private int _currentDamage;
public void ApplyStatusEffect() { }
```

**Rule**: Names should be descriptive and self-documenting.

---

## 🏗️ Wrong Layer

### ❌ BAD
```csharp
// In PokemonUltimate.Core:
public static class PokemonCatalog
{
    public static PokemonSpeciesData Pikachu = new(...);  // Game data in Core!
}
```

### ✅ GOOD
```csharp
// Game data belongs in PokemonUltimate.Content:
namespace PokemonUltimate.Content.Catalogs
{
    public static class PokemonCatalog
    {
        public static PokemonSpeciesData Pikachu = Pokemon.Create(...);
    }
}
```

**Rule**: Core = logic, Content = data, Tests = tests. Don't mix.

---

## 🔀 Missing Validation

### ❌ BAD
```csharp
public void SetLevel(int level)
{
    Level = level; // No validation!
}
```

### ✅ GOOD
```csharp
public void SetLevel(int level)
{
    if (level < 1 || level > 100)
        throw new ArgumentOutOfRangeException(nameof(level), 
            ErrorMessages.LevelMustBeBetween1And100);
    
    Level = level;
}
```

**Rule**: Always validate inputs at public API boundaries.

---

## 🧪 Poor Test Names

### ❌ BAD
```csharp
[Test]
public void Test1() { }

[Test]
public void TestCalculate() { }

[Test]
public void ItWorks() { }
```

### ✅ GOOD
```csharp
[Test]
public void CalculateHP_Level50MaxIV_Returns145() { }

[Test]
public void GetEffectiveness_FireVsGrass_ReturnsSuperEffective() { }

[Test]
public void Evolve_InvalidTarget_ThrowsArgumentException() { }
```

**Rule**: Test names follow `Method_Scenario_ExpectedResult` pattern.

---

## 📝 Missing XML Documentation

### ❌ BAD
```csharp
public class StatCalculator
{
    public static int CalculateHP(int baseHP, int level, int iv, int ev)
    {
        // No documentation!
    }
}
```

### ✅ GOOD
```csharp
/// <summary>
/// Calculates HP using the Gen 3+ formula.
/// </summary>
/// <param name="baseHP">Base HP stat of the species (1-255).</param>
/// <param name="level">Pokemon level (1-100).</param>
/// <param name="iv">Individual Value (0-31).</param>
/// <param name="ev">Effort Value (0-252).</param>
/// <returns>Calculated HP value.</returns>
/// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are out of valid range.</exception>
public static int CalculateHP(int baseHP, int level, int iv, int ev)
{
    // Implementation
}
```

**Rule**: All public APIs must have XML documentation.

---

## 🔁 Copy-Paste Code

### ❌ BAD
```csharp
public void ValidateAttack(int value)
{
    if (value < 0) throw new ArgumentException("Cannot be negative");
    if (value > 255) throw new ArgumentException("Cannot exceed 255");
}

public void ValidateDefense(int value)
{
    if (value < 0) throw new ArgumentException("Cannot be negative");
    if (value > 255) throw new ArgumentException("Cannot exceed 255");
}
```

### ✅ GOOD
```csharp
private void ValidateBaseStat(int value, string paramName)
{
    if (value < 0)
        throw new ArgumentException(ErrorMessages.BaseStatCannotBeNegative, paramName);
    if (value > MaxBaseStat)
        throw new ArgumentOutOfRangeException(paramName, ErrorMessages.BaseStatTooHigh);
}
```

**Rule**: DRY - Don't Repeat Yourself. Extract common logic.

---

## Summary Checklist

Before committing, verify:

- [ ] No magic strings (use `ErrorMessages`/`GameMessages`)
- [ ] No unnecessary try-catch
- [ ] No silent failures (throw exceptions)
- [ ] No god classes (use partial classes)
- [ ] Blueprints are immutable
- [ ] No magic numbers
- [ ] Descriptive naming
- [ ] Code in correct layer
- [ ] Inputs validated
- [ ] Test names descriptive
- [ ] Public APIs documented
- [ ] No duplicated code

