# 📖 Test Examples: The Right Way

> **Reference these examples when writing tests.**
> Follow these patterns for consistent, maintainable tests.

---

## 🧪 Functional Test Example

```csharp
[TestFixture]
public class TypeEffectivenessTests
{
    [Test]
    public void GetEffectiveness_FireVsGrass_ReturnsSuperEffective()
    {
        // Arrange
        var attackType = PokemonType.Fire;
        var defenseType = PokemonType.Grass;
        
        // Act
        var effectiveness = TypeEffectiveness.GetEffectiveness(attackType, defenseType);
        
        // Assert
        Assert.That(effectiveness, Is.EqualTo(2.0));
    }
    
    [Test]
    public void GetEffectiveness_FireVsWater_ReturnsNotVeryEffective()
    {
        var effectiveness = TypeEffectiveness.GetEffectiveness(
            PokemonType.Fire, 
            PokemonType.Water);
        
        Assert.That(effectiveness, Is.EqualTo(0.5));
    }
    
    [Test]
    public void GetEffectiveness_NormalVsGhost_ReturnsImmune()
    {
        var effectiveness = TypeEffectiveness.GetEffectiveness(
            PokemonType.Normal, 
            PokemonType.Ghost);
        
        Assert.That(effectiveness, Is.EqualTo(0.0));
    }
}
```

**Key Points:**
- One assertion per test (usually)
- Test name describes scenario
- AAA pattern clear

---

## 🔥 Edge Case Test Example

```csharp
[TestFixture]
public class StatCalculatorEdgeCasesTests
{
    // Boundary tests
    [Test]
    public void CalculateHP_Level1MinimumStats_ReturnsMinimumHP()
    {
        var hp = StatCalculator.CalculateHP(
            baseHP: 1,      // Minimum base
            level: 1,       // Minimum level
            iv: 0,          // Minimum IV
            ev: 0);         // Minimum EV
        
        Assert.That(hp, Is.GreaterThanOrEqualTo(1));
    }
    
    [Test]
    public void CalculateHP_Level100MaxStats_ReturnsMaximumHP()
    {
        var hp = StatCalculator.CalculateHP(
            baseHP: 255,    // Maximum base (Blissey)
            level: 100,     // Maximum level
            iv: 31,         // Maximum IV
            ev: 252);       // Maximum EV
        
        Assert.That(hp, Is.EqualTo(714)); // Blissey's max HP
    }
    
    // Invalid input tests
    [Test]
    public void CalculateHP_NegativeBaseStat_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            StatCalculator.CalculateHP(-1, 50, 31, 0));
    }
    
    [Test]
    public void CalculateHP_LevelZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            StatCalculator.CalculateHP(80, 0, 31, 0));
    }
    
    [Test]
    public void CalculateHP_Level101_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            StatCalculator.CalculateHP(80, 101, 31, 0));
    }
}
```

**Key Points:**
- Test boundaries (min/max values)
- Test invalid inputs
- Use named parameters for clarity
- Comments explain the test scenario

---

## 🌍 Real-World Verification Test

```csharp
[TestFixture]
public class RealPokemonStatsTests
{
    [Test]
    public void Pikachu_Level50_MaxIVs_NoEVs_MatchesOfficialStats()
    {
        // Pikachu base stats: HP 35, Atk 55, Def 40, SpA 50, SpD 50, Spe 90
        const int level = 50;
        const int iv = 31;
        const int ev = 0;
        
        // Expected stats from official games
        var expectedHP = 110;
        var expectedAttack = 75;
        var expectedSpeed = 110;
        
        // Calculate
        var actualHP = StatCalculator.CalculateHP(35, level, iv, ev);
        var actualAttack = StatCalculator.CalculateStat(55, level, iv, ev, 1.0);
        var actualSpeed = StatCalculator.CalculateStat(90, level, iv, ev, 1.0);
        
        // Verify
        Assert.Multiple(() =>
        {
            Assert.That(actualHP, Is.EqualTo(expectedHP), "HP mismatch");
            Assert.That(actualAttack, Is.EqualTo(expectedAttack), "Attack mismatch");
            Assert.That(actualSpeed, Is.EqualTo(expectedSpeed), "Speed mismatch");
        });
    }
}
```

**Key Points:**
- Comments with source data
- `Assert.Multiple` for related assertions
- Descriptive failure messages
- Verified against official game data

---

## 📋 Parameterized Tests

```csharp
[TestFixture]
public class TypeEffectivenessParameterizedTests
{
    [TestCase(PokemonType.Fire, PokemonType.Grass, 2.0)]
    [TestCase(PokemonType.Water, PokemonType.Fire, 2.0)]
    [TestCase(PokemonType.Grass, PokemonType.Water, 2.0)]
    [TestCase(PokemonType.Fire, PokemonType.Water, 0.5)]
    [TestCase(PokemonType.Normal, PokemonType.Ghost, 0.0)]
    [TestCase(PokemonType.Normal, PokemonType.Normal, 1.0)]
    public void GetEffectiveness_TypeMatchups_ReturnsCorrectMultiplier(
        PokemonType attack, 
        PokemonType defense, 
        double expected)
    {
        var actual = TypeEffectiveness.GetEffectiveness(attack, defense);
        Assert.That(actual, Is.EqualTo(expected));
    }
}
```

**Key Points:**
- Use `[TestCase]` for data-driven tests
- Cover multiple scenarios efficiently
- Keep test method simple

---

## 🔄 State Transition Tests

```csharp
[TestFixture]
public class PokemonInstanceEvolutionTests
{
    private PokemonInstance _pokemon;
    
    [SetUp]
    public void SetUp()
    {
        _pokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 16);
    }
    
    [Test]
    public void Evolve_ValidTarget_ChangesSpecies()
    {
        // Arrange
        var originalSpecies = _pokemon.Species;
        var targetSpecies = PokemonCatalog.Charmeleon;
        
        // Act
        _pokemon.Evolve(targetSpecies);
        
        // Assert
        Assert.That(_pokemon.Species, Is.EqualTo(targetSpecies));
        Assert.That(_pokemon.Species, Is.Not.EqualTo(originalSpecies));
    }
    
    [Test]
    public void Evolve_ValidTarget_PreservesLevel()
    {
        var levelBefore = _pokemon.Level;
        
        _pokemon.Evolve(PokemonCatalog.Charmeleon);
        
        Assert.That(_pokemon.Level, Is.EqualTo(levelBefore));
    }
    
    [Test]
    public void Evolve_ValidTarget_RecalculatesStats()
    {
        var hpBefore = _pokemon.MaxHP;
        
        _pokemon.Evolve(PokemonCatalog.Charmeleon);
        
        Assert.That(_pokemon.MaxHP, Is.GreaterThan(hpBefore));
    }
}
```

**Key Points:**
- `[SetUp]` for common initialization
- Test one state change per test
- Verify both changed and preserved state

