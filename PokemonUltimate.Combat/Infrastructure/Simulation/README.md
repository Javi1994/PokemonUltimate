# Battle Simulation System

The simulation system provides tools for running multiple battles or move executions to collect aggregated statistics. Useful for testing strategies, analyzing move effectiveness, and performance testing.

## Architecture

The simulation system uses a **batch processing pattern**:

-   **BattleSimulator**: Simulates multiple complete battles
-   **MoveSimulator**: Simulates multiple move executions
-   **Statistics Aggregation**: Collects and aggregates statistics across simulations
-   **Configuration**: Flexible configuration for reproducible or random simulations

## Components

### BattleSimulator.cs

Simulates multiple battles with the same configuration and aggregates statistics.

**Key Classes:**

| Class                 | Purpose                                             |
| --------------------- | --------------------------------------------------- |
| **BattleSimulator**   | Main simulator class with static simulation methods |
| **SimulationResults** | Results container with aggregated statistics        |
| **SimulationConfig**  | Configuration for battle simulation                 |

**SimulationResults Properties:**

| Property                 | Type                 | Purpose                                |
| ------------------------ | -------------------- | -------------------------------------- |
| **TotalBattles**         | `int`                | Total number of battles simulated      |
| **PlayerWins**           | `int`                | Number of battles won by player        |
| **EnemyWins**            | `int`                | Number of battles won by enemy         |
| **Draws**                | `int`                | Number of draws                        |
| **AggregatedStatistics** | `BattleStatistics`   | Aggregated statistics from all battles |
| **IndividualResults**    | `List<BattleResult>` | Individual battle results              |
| **AverageTurns**         | `double`             | Average turns per battle               |
| **PlayerWinRate**        | `double`             | Player win rate (0.0 to 1.0)           |
| **EnemyWinRate**         | `double`             | Enemy win rate (0.0 to 1.0)            |

**SimulationConfig Properties:**

| Property                          | Type   | Default | Purpose                                    |
| --------------------------------- | ------ | ------- | ------------------------------------------ |
| **NumberOfBattles**               | `int`  | 100     | Number of battles to simulate              |
| **UseRandomSeeds**                | `bool` | true    | Use different random seeds per battle      |
| **FixedSeed**                     | `int?` | null    | Optional seed for reproducible simulations |
| **ResetStatisticsBetweenBattles** | `bool` | false   | Reset statistics between battles           |
| **CollectIndividualResults**      | `bool` | true    | Collect individual battle results          |

**Key Methods:**

-   `SimulateAsync(builder, config, progress)`: Simulates multiple battles
-   `CloneBuilder(original)`: Clones builder with fresh Pokemon instances
-   `AggregateStatistics(aggregated, battle)`: Aggregates statistics from one battle

### MoveSimulator.cs

Simulates single move executions without full battle simulation.

**Key Classes:**

| Class                       | Purpose                                             |
| --------------------------- | --------------------------------------------------- |
| **MoveSimulator**           | Main simulator class with static simulation methods |
| **MoveSimulationResults**   | Results container with aggregated statistics        |
| **MoveSimulationConfig**    | Configuration for move simulation                   |
| **FixedMoveActionProvider** | AI provider that always selects specific move       |
| **NoActionProvider**        | AI provider that never provides action              |

**MoveSimulationResults Properties:**

| Property                 | Type                     | Purpose                                   |
| ------------------------ | ------------------------ | ----------------------------------------- |
| **TotalTests**           | `int`                    | Total number of move executions simulated |
| **AggregatedStatistics** | `BattleStatistics`       | Aggregated statistics from all tests      |
| **IndividualResults**    | `List<BattleStatistics>` | Individual test statistics                |

**MoveSimulationConfig Properties:**

| Property                        | Type   | Default | Purpose                                    |
| ------------------------------- | ------ | ------- | ------------------------------------------ |
| **NumberOfTests**               | `int`  | 100     | Number of move executions to simulate      |
| **UseRandomSeeds**              | `bool` | true    | Use different random seeds per test        |
| **FixedSeed**                   | `int?` | null    | Optional seed for reproducible simulations |
| **ResetStatisticsBetweenTests** | `bool` | false   | Reset statistics between tests             |
| **CollectIndividualResults**    | `bool` | false   | Collect individual test results            |

**Key Methods:**

-   `SimulateAsync(moveToTest, attacker, target, config, progress)`: Simulates multiple move executions
-   `ExecuteSingleTurn(...)`: Executes single turn for move testing
-   `AggregateStatistics(aggregated, test)`: Aggregates statistics from one test

## Usage Examples

### Battle Simulation

```csharp
// Create battle builder
var builder = BattleBuilder.Create()
    .WithPlayerPokemon(pikachu)
    .WithEnemyPokemon(charmander)
    .Singles()
    .WithRandomAI();

// Configure simulation
var config = new BattleSimulator.SimulationConfig
{
    NumberOfBattles = 1000,
    UseRandomSeeds = true,
    CollectIndividualResults = false
};

// Simulate battles
var results = await BattleSimulator.SimulateAsync(builder, config);

// Access results
Console.WriteLine($"Player wins: {results.PlayerWins}");
Console.WriteLine($"Enemy wins: {results.EnemyWins}");
Console.WriteLine($"Win rate: {results.PlayerWinRate:P2}");
Console.WriteLine($"Average turns: {results.AverageTurns:F2}");

// Access aggregated statistics
var stats = results.AggregatedStatistics;
Console.WriteLine($"Total damage: {stats.PlayerDamageDealt}");
Console.WriteLine($"Critical hits: {stats.CriticalHits}");
```

### Move Simulation

```csharp
// Create move and Pokemon
var thunderbolt = new MoveInstance(GetMoveData("thunderbolt"));
var pikachu = PokemonFactory.Create("pikachu", 50);
var charizard = PokemonFactory.Create("charizard", 50);

// Configure simulation
var config = new MoveSimulator.MoveSimulationConfig
{
    NumberOfTests = 1000,
    UseRandomSeeds = true
};

// Simulate move executions
var results = await MoveSimulator.SimulateAsync(
    thunderbolt,
    pikachu,
    charizard,
    config
);

// Access results
var stats = results.AggregatedStatistics;
var damageValues = stats.DamageValuesByMove["Thunderbolt"];

Console.WriteLine($"Average damage: {damageValues.Average():F1}");
Console.WriteLine($"Min damage: {damageValues.Min()}");
Console.WriteLine($"Max damage: {damageValues.Max()}");
Console.WriteLine($"Critical hits: {stats.CriticalHits}");
```

### Reproducible Simulations

```csharp
// Use fixed seed for reproducible results
var config = new BattleSimulator.SimulationConfig
{
    NumberOfBattles = 100,
    UseRandomSeeds = false,
    FixedSeed = 12345 // Same seed for all battles
};

var results = await BattleSimulator.SimulateAsync(builder, config);
// Results will be identical each run
```

### Progress Reporting

```csharp
var progress = new Progress<int>(percent =>
{
    Console.WriteLine($"Progress: {percent}%");
});

var results = await BattleSimulator.SimulateAsync(
    builder,
    config,
    progress
);
```

## How to Add New Simulation Types

### Step 1: Create Simulator Class

Create a new file in `Infrastructure/Simulation/` (e.g., `AbilitySimulator.cs`):

```csharp
namespace PokemonUltimate.Combat.Infrastructure.Simulation
{
    public class AbilitySimulator
    {
        public class AbilitySimulationConfig
        {
            public int NumberOfTests { get; set; } = 100;
            public bool UseRandomSeeds { get; set; } = true;
            public int? FixedSeed { get; set; }
        }

        public class AbilitySimulationResults
        {
            public int TotalTests { get; set; }
            public BattleStatistics AggregatedStatistics { get; set; }
        }

        public static async Task<AbilitySimulationResults> SimulateAsync(
            AbilityData ability,
            PokemonInstance pokemon,
            AbilitySimulationConfig config = null)
        {
            // Your simulation logic here
            return new AbilitySimulationResults();
        }
    }
}
```

### Step 2: Implement Simulation Logic

Implement the simulation logic following the same pattern as `BattleSimulator`:

1. Create fresh Pokemon instances for each test
2. Set up battle configuration
3. Run simulation
4. Collect statistics
5. Aggregate results

### Step 3: Test Your Simulator

Create unit tests for your simulator:

```csharp
[Test]
public async Task AbilitySimulator_SimulatesAbility()
{
    // Arrange
    var ability = GetAbilityData("intimidate");
    var pokemon = CreateTestPokemon();
    var config = new AbilitySimulator.AbilitySimulationConfig
    {
        NumberOfTests = 100
    };

    // Act
    var results = await AbilitySimulator.SimulateAsync(ability, pokemon, config);

    // Assert
    Assert.IsNotNull(results);
    Assert.AreEqual(100, results.TotalTests);
}
```

## Design Principles

1. **Batch Processing**: Runs multiple simulations and aggregates results
2. **Fresh Instances**: Creates new Pokemon instances for each simulation
3. **Statistics Aggregation**: Combines statistics from all simulations
4. **Reproducibility**: Supports fixed seeds for reproducible results
5. **Flexibility**: Configurable for different simulation needs

## Performance Considerations

-   **Memory Management**: Disposes engines after each simulation to prevent leaks
-   **Event Cleanup**: Unsubscribes statistics collectors properly
-   **Fresh Instances**: Creates new Pokemon instances to ensure independence
-   **Progress Reporting**: Optional progress reporting for long simulations

## Related Documentation

-   `../Statistics/README.md` - Statistics system documentation
-   `../Events/README.md` - Event system documentation
-   `BattleSimulator.cs` - Battle simulator implementation
-   `MoveSimulator.cs` - Move simulator implementation
