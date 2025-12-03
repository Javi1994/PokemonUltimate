# Feature 5: Game Features - Testing

> How to test game features.

**Feature Number**: 5  
**Feature Name**: Game Features  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Game features testing focuses on:
- **Post-Battle System** - EXP calculation, level ups, rewards
- **Pokemon Management** - Party, PC, catching
- **Encounter System** - Wild, trainer, boss encounters
- **Inventory System** - Item management and usage
- **Save System** - Save/load functionality
- **Progression System** - Roguelike and meta-progression

**Status**: ⏳ Planned (no tests yet, structure defined)

## Test Structure

Following `docs/ai/testing_structure_definition.md`:

```
PokemonUltimate.Tests/
└── Systems/
    └── Game/                          # Game features tests
        ├── Services/
        │   ├── [Service]Tests.cs      # Functional tests
        │   └── [Service]EdgeCasesTests.cs # Edge case tests
        ├── Managers/
        │   ├── [Manager]Tests.cs      # Functional tests
        │   └── [Manager]EdgeCasesTests.cs # Edge case tests
        └── Systems/
            ├── [System]Tests.cs       # Functional tests
            └── [System]EdgeCasesTests.cs # Edge case tests
```

## Test Types

### Functional Tests
**Location**: `Tests/Systems/Game/[Component]Tests.cs`
**Purpose**: Test normal behavior and expected outcomes
**Naming**: `MethodName_Scenario_ExpectedResult`

**Examples**:
- `ExpCalculator_Calculate_WildPokemon_ReturnsCorrectExp`
- `PartyManager_AddPokemon_PartyNotFull_AddsPokemon`
- `CatchCalculator_CalculateCatchRate_LowHP_ReturnsHigherRate`

### Edge Case Tests
**Location**: `Tests/Systems/Game/[Component]EdgeCasesTests.cs`
**Purpose**: Test boundary conditions, invalid inputs, special cases
**Naming**: `MethodName_EdgeCase_ExpectedResult`

**Examples**:
- `ExpCalculator_Calculate_Level100_ReturnsZero`
- `PartyManager_AddPokemon_PartyFull_ThrowsException`
- `SaveManager_LoadGame_InvalidFile_ThrowsException`

## Coverage by Component

### Post-Battle System (Phase 5.1)
**Tests**:
- ⏳ `PostBattleServiceTests.cs` - EXP calculation, distribution
- ⏳ `ExpCalculatorTests.cs` - EXP formula accuracy
- ⏳ `LevelUpTests.cs` - Level up logic, stat increases
- ⏳ `MoveLearningTests.cs` - Move learning on level up

### Pokemon Management (Phase 5.2)
**Tests**:
- ⏳ `PartyManagerTests.cs` - Party operations
- ⏳ `PCManagerTests.cs` - PC storage operations
- ⏳ `CatchCalculatorTests.cs` - Catch rate calculation
- ⏳ `CatchingTests.cs` - Catching flow

### Encounter System (Phase 5.3)
**Tests**:
- ⏳ `EncounterServiceTests.cs` - Encounter generation
- ⏳ `WildEncounterTests.cs` - Wild Pokemon encounters
- ⏳ `TrainerEncounterTests.cs` - Trainer battles
- ⏳ `BossEncounterTests.cs` - Boss battles

### Inventory System (Phase 5.4)
**Tests**:
- ⏳ `InventoryManagerTests.cs` - Inventory operations
- ⏳ `ItemUsageTests.cs` - Item usage logic

### Save System (Phase 5.5)
**Tests**:
- ⏳ `SaveManagerTests.cs` - Save/load operations
- ⏳ `SaveDataTests.cs` - Save data serialization

### Progression System (Phase 5.6)
**Tests**:
- ⏳ `ProgressionServiceTests.cs` - Roguelike progression
- ⏳ `MetaProgressionServiceTests.cs` - Meta-progression unlocks

## Coverage Requirements

### ✅ Planned Coverage
- [ ] Post-battle system tests (Phase 5.1)
- [ ] Pokemon management tests (Phase 5.2)
- [ ] Encounter system tests (Phase 5.3)
- [ ] Inventory system tests (Phase 5.4)
- [ ] Save system tests (Phase 5.5)
- [ ] Progression system tests (Phase 5.6)

## Test Examples

### Post-Battle Test Example
```csharp
[TestFixture]
public class ExpCalculatorTests
{
    [Test]
    public void Calculate_WildPokemon_ReturnsCorrectExp()
    {
        // Arrange
        var defeatedPokemon = Pokemon.Create(PokemonCatalog.Pikachu, 10).Build();
        var isWild = true;
        var levelDifference = 0;
        var participants = 1;
        
        // Act
        var exp = ExpCalculator.Calculate(defeatedPokemon, isWild, levelDifference, participants);
        
        // Assert
        Assert.That(exp, Is.GreaterThan(0));
        // Verify against Gen 3+ formula
    }
    
    [Test]
    public void Calculate_Level100_ReturnsZero()
    {
        // Arrange
        var pokemon = Pokemon.Create(PokemonCatalog.Pikachu, 100).Build();
        
        // Act
        var exp = ExpCalculator.Calculate(pokemon, true, 0, 1);
        
        // Assert
        Assert.That(exp, Is.EqualTo(0));
    }
}
```

### Party Management Test Example
```csharp
[TestFixture]
public class PartyManagerTests
{
    [Test]
    public void AddPokemon_PartyNotFull_AddsPokemon()
    {
        // Arrange
        var manager = new PartyManager();
        var pokemon = Pokemon.Create(PokemonCatalog.Pikachu, 5).Build();
        
        // Act
        manager.AddPokemon(pokemon);
        
        // Assert
        Assert.That(manager.Count, Is.EqualTo(1));
        Assert.That(manager.GetPokemon(0), Is.EqualTo(pokemon));
    }
    
    [Test]
    public void AddPokemon_PartyFull_ThrowsException()
    {
        // Arrange
        var manager = new PartyManager();
        for (int i = 0; i < 6; i++)
        {
            manager.AddPokemon(Pokemon.Create(PokemonCatalog.Pikachu, 5).Build());
        }
        var newPokemon = Pokemon.Create(PokemonCatalog.Charizard, 5).Build();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => manager.AddPokemon(newPokemon));
    }
}
```

## Test Execution

### Run All Game Feature Tests
```bash
dotnet test --filter "FullyQualifiedName~Game"
```

### Run Post-Battle Tests Only
```bash
dotnet test --filter "FullyQualifiedName~PostBattle"
```

### Run Specific Component Tests
```bash
dotnet test --filter "FullyQualifiedName~PartyManagerTests"
```

## Related Documents

- **[Feature README](README.md)** - Overview of Game Features
- **[5.1 Architecture](5.1-post-battle-rewards/architecture.md)** - Post-battle rewards specification
- **[Use Cases](use_cases.md)** - Scenarios to verify
- **[Roadmap](roadmap.md)** - Implementation phases
- **[Code Location](code_location.md)** - Where tests will be located
- **[Feature 2: Combat System](../2-combat-system/testing.md)** - Battle engine testing strategy

---

**Last Updated**: 2025-01-XX

