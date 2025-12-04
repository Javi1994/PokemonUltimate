# Feature 3: Content Expansion - Testing

> How to test content expansion and validation.

**Feature Number**: 3  
**Feature Name**: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Content expansion testing focuses on:
- **Data Completeness** - All required fields present
- **Data Correctness** - Values match official game data
- **Data Consistency** - No duplicates, valid references
- **Catalog Organization** - Proper registration and queries

**Coverage**: 935+ tests covering all Pokemon and Moves (26 Pokemon, 36 Moves)

## Test Structure

Following TDD workflow from `ai_workflow/docs/TDD_GUIDE.md`:

```
PokemonUltimate.Tests/
└── Data/                              # Content-specific tests
    ├── Pokemon/                       # Pokemon data tests
    │   └── [Pokemon]Tests.cs          # One file per Pokemon
    ├── Moves/                         # Move data tests
    │   └── [Move]Tests.cs             # One file per Move
    ├── Catalogs/                      # Catalog validation tests
    │   ├── PokemonCatalogValidationTests.cs
    │   ├── MoveCatalogValidationTests.cs
    │   └── [Catalog]ValidationTests.cs
    └── Validation/                    # Validation against official data
        └── [Category]ValidationTests.cs
```

## Test Types

### Individual Content Tests
**Location**: `Tests/Data/[Category]/[Name]Tests.cs`
**Purpose**: Test individual Pokemon/Move/Item/Ability data completeness and correctness
**Naming**: `[Name]Tests.cs` (e.g., `PikachuTests.cs`, `FlamethrowerTests.cs`)

**Examples**:
- `PikachuTests.cs` - Tests Pikachu data (stats, types, abilities, learnset)
- `FlamethrowerTests.cs` - Tests Flamethrower data (power, accuracy, effects)

### Catalog Validation Tests
**Location**: `Tests/Data/Catalogs/[Catalog]ValidationTests.cs`
**Purpose**: Test catalog organization, registration, and queries
**Naming**: `[Catalog]ValidationTests.cs`

**Examples**:
- `PokemonCatalogValidationTests.cs` - Tests catalog queries, uniqueness, registration
- `MoveCatalogValidationTests.cs` - Tests catalog queries, type organization

### Data Consistency Tests
**Location**: `Tests/Data/Validation/[Category]ValidationTests.cs`
**Purpose**: Validate content against official game data
**Naming**: `[Category]ValidationTests.cs`

**Examples**:
- `PokemonDataValidationTests.cs` - Validates Pokemon stats/types against official data
- `MoveDataValidationTests.cs` - Validates move power/accuracy against official data

## Coverage by Category

### Pokemon Tests
**Location**: `Tests/Data/Pokemon/[Pokemon]Tests.cs`
**Current**: 26 test files (one per Pokemon)
**Coverage**:
- ✅ Base stats match official data
- ✅ Types match official data
- ✅ Abilities match official data
- ✅ Learnset includes signature moves
- ✅ Evolution conditions match official data
- ⏳ Pokedex fields (when implemented)

**Example Test Structure**:
```csharp
[TestFixture]
public class PikachuTests
{
    [Test]
    public void Pikachu_HasCorrectBaseStats()
    {
        var pikachu = PokemonCatalog.Pikachu;
        Assert.That(pikachu.BaseStats.HP, Is.EqualTo(35));
        Assert.That(pikachu.BaseStats.Attack, Is.EqualTo(55));
        // ... etc
    }
    
    [Test]
    public void Pikachu_HasCorrectTypes()
    {
        var pikachu = PokemonCatalog.Pikachu;
        Assert.That(pikachu.PrimaryType, Is.EqualTo(PokemonType.Electric));
        Assert.That(pikachu.SecondaryType, Is.Null);
    }
    
    [Test]
    public void Pikachu_HasCorrectAbilities()
    {
        var pikachu = PokemonCatalog.Pikachu;
        Assert.That(pikachu.Ability1, Is.EqualTo(AbilityCatalog.Static));
        // ... etc
    }
    
    [Test]
    public void Pikachu_HasSignatureMoves()
    {
        var pikachu = PokemonCatalog.Pikachu;
        var moveNames = pikachu.Learnset.Select(m => m.Move.Name).ToList();
        Assert.That(moveNames, Contains.Item("Thunder Shock"));
        Assert.That(moveNames, Contains.Item("Thunderbolt"));
    }
}
```

### Move Tests
**Location**: `Tests/Data/Moves/[Move]Tests.cs`
**Current**: 36 test files (one per Move)
**Coverage**:
- ✅ Power/Accuracy match official data (±5 tolerance)
- ✅ Type matches official data
- ✅ Category (Physical/Special) matches official data
- ✅ PP matches official data
- ✅ Effects match official behavior

**Example Test Structure**:
```csharp
[TestFixture]
public class FlamethrowerTests
{
    [Test]
    public void Flamethrower_HasCorrectStats()
    {
        var move = MoveCatalog.Flamethrower;
        Assert.That(move.Power, Is.EqualTo(90));
        Assert.That(move.Accuracy, Is.EqualTo(100));
        Assert.That(move.PP, Is.EqualTo(15));
    }
    
    [Test]
    public void Flamethrower_HasCorrectType()
    {
        var move = MoveCatalog.Flamethrower;
        Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
    }
    
    [Test]
    public void Flamethrower_HasCorrectEffects()
    {
        var move = MoveCatalog.Flamethrower;
        Assert.That(move.Effects, Has.Some.InstanceOf<DamageEffect>());
        Assert.That(move.Effects, Has.Some.InstanceOf<StatusEffect>());
    }
}
```

### Catalog Validation Tests
**Location**: `Tests/Data/Catalogs/[Catalog]ValidationTests.cs`
**Coverage**:
- ✅ All Pokemon/Moves registered in catalog
- ✅ No duplicate names
- ✅ No duplicate Pokedex numbers
- ✅ Catalog queries work correctly
- ✅ Registration to registry works

**Example Tests**:
```csharp
[TestFixture]
public class PokemonCatalogValidationTests
{
    [Test]
    public void PokemonCatalog_AllPokemon_HaveUniqueNames()
    {
        var allPokemon = PokemonCatalog.GetAll();
        var names = allPokemon.Select(p => p.Name).ToList();
        Assert.That(names, Is.Unique);
    }
    
    [Test]
    public void PokemonCatalog_AllPokemon_HaveUniquePokedexNumbers()
    {
        var allPokemon = PokemonCatalog.GetAll();
        var numbers = allPokemon.Select(p => p.PokedexNumber).ToList();
        Assert.That(numbers, Is.Unique);
    }
    
    [Test]
    public void PokemonCatalog_RegisterAll_PopulatesRegistry()
    {
        var registry = new PokemonRegistry();
        PokemonCatalog.RegisterAll(registry);
        Assert.That(registry.Count, Is.EqualTo(PokemonCatalog.Count));
    }
}
```

## Coverage Requirements

### ✅ Completed Coverage
- [x] All Pokemon have individual test files (26/26)
- [x] All Moves have individual test files (36/36)
- [x] Catalog validation tests exist
- [x] Data completeness tests (required fields)
- [x] Data correctness tests (stats, types, abilities)
- [x] Uniqueness tests (names, numbers)

### ⏳ Planned Coverage
- [ ] Item tests (when items are expanded)
- [ ] Ability tests (when abilities are expanded)
- [ ] Pokedex field tests (when implemented)
- [ ] Validation against official data (comprehensive)
- [ ] Performance tests (catalog queries)

## Test Examples

### Pokemon Test Example
```csharp
[TestFixture]
public class CharizardTests
{
    [Test]
    public void Charizard_HasCorrectBaseStats()
    {
        var charizard = PokemonCatalog.Charizard;
        Assert.That(charizard.BaseStats.HP, Is.EqualTo(78));
        Assert.That(charizard.BaseStats.Attack, Is.EqualTo(84));
        Assert.That(charizard.BaseStats.Defense, Is.EqualTo(78));
        Assert.That(charizard.BaseStats.SpAttack, Is.EqualTo(109));
        Assert.That(charizard.BaseStats.SpDefense, Is.EqualTo(85));
        Assert.That(charizard.BaseStats.Speed, Is.EqualTo(100));
    }
    
    [Test]
    public void Charizard_HasCorrectTypes()
    {
        var charizard = PokemonCatalog.Charizard;
        Assert.That(charizard.PrimaryType, Is.EqualTo(PokemonType.Fire));
        Assert.That(charizard.SecondaryType, Is.EqualTo(PokemonType.Flying));
    }
}
```

### Catalog Test Example
```csharp
[TestFixture]
public class MoveCatalogValidationTests
{
    [Test]
    public void MoveCatalog_GetAllByType_ReturnsCorrectMoves()
    {
        var fireMoves = MoveCatalog.GetAllByType(PokemonType.Fire);
        Assert.That(fireMoves, Is.Not.Empty);
        Assert.That(fireMoves.All(m => m.Type == PokemonType.Fire), Is.True);
    }
    
    [Test]
    public void MoveCatalog_AllMoves_HaveValidPower()
    {
        var allMoves = MoveCatalog.GetAll();
        Assert.That(allMoves.All(m => m.Power >= 0 && m.Power <= 200), Is.True);
    }
}
```

## Test Execution

### Run All Content Tests
```bash
dotnet test --filter "FullyQualifiedName~Data"
```

### Run Pokemon Tests Only
```bash
dotnet test --filter "FullyQualifiedName~Data/Pokemon"
```

### Run Move Tests Only
```bash
dotnet test --filter "FullyQualifiedName~Data/Moves"
```

### Run Catalog Validation Tests
```bash
dotnet test --filter "FullyQualifiedName~Data/Catalogs"
```

### Run Specific Pokemon Test
```bash
dotnet test --filter "FullyQualifiedName~PikachuTests"
```

## Related Documents

- **[Feature README](README.md)** - Overview of Content Expansion
- **[Architecture](architecture.md)** - What to test
- **[Use Cases](use_cases.md)** - Scenarios to verify
- **[Roadmap](roadmap.md)** - Content expansion phases
- **[Code Location](code_location.md)** - Where tests are located
- **[Feature 1: Game Data Testing](../1-game-data/testing.md)** - Game data testing strategy
- **[TDD Guide](../../../ai_workflow/docs/TDD_GUIDE.md)** - Test-Driven Development guide

---

**Last Updated**: 2025-01-XX

