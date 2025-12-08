# Battle Flow Steps

This directory contains all 8 steps that execute during the complete battle lifecycle. Steps are executed sequentially in the order defined in `CombatEngine.Initialize()`.

## Complete Step List

### Phase 1: Setup

| #   | Step                             | Purpose                                          |
| --- | -------------------------------- | ------------------------------------------------ |
| 1   | **CreateFieldStep.cs**           | Creates BattleField with both sides              |
| 2   | **AssignActionProvidersStep.cs** | Assigns action providers to battle sides         |
| 3   | **CreateQueueStep.cs**           | Creates BattleQueueService for action processing |
| 4   | **ValidateInitialStateStep.cs**  | Validates initial battle state (parties, rules)  |
| 5   | **CreateDependenciesStep.cs**    | Creates TurnEngine and runtime dependencies      |

### Phase 2: Execution

| #   | Step                         | Purpose                                          |
| --- | ---------------------------- | ------------------------------------------------ |
| 6   | **BattleStartFlowStep.cs**   | Executes battle start effects (abilities, items) |
| 7   | **ExecuteBattleLoopStep.cs** | Executes main battle loop until completion       |

### Phase 3: Cleanup

| #   | Step                     | Purpose                                          |
| --- | ------------------------ | ------------------------------------------------ |
| 8   | **BattleEndFlowStep.cs** | Handles battle end (statistics, cleanup, result) |

## Step Interface

All steps implement `IBattleFlowStep`:

```csharp
public interface IBattleFlowStep
{
    string StepName { get; }
    Task<bool> ExecuteAsync(BattleFlowContext context);
}
```

## Context Usage

Steps read from and write to `BattleFlowContext`:

**Input (set at initialization):**

-   Rules, PlayerParty, EnemyParty
-   PlayerProvider, EnemyProvider
-   View, StateValidator, Logger
-   Engine, TargetResolver, IsDebugMode

**Output (created/modified by steps):**

-   Field (created by CreateFieldStep)
-   QueueService (created by CreateQueueStep)
-   TurnEngine (created by CreateDependenciesStep)
-   Result (created by BattleEndFlowStep)
-   Outcome (updated by ExecuteBattleLoopStep)

## How to Add a New Battle Flow Step

### Step 1: Create the Step Class

Create a new file in `Engine/BattleFlow/Steps/` (e.g., `CustomSetupStep.cs`):

```csharp
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    public class CustomSetupStep : IBattleFlowStep
    {
        public string StepName => "Custom Setup";

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            // Your step logic here
            // Read from context
            // Write to context
            // Perform setup operations

            return await Task.FromResult(true);
        }
    }
}
```

### Step 2: Add Step to CombatEngine

Open `Engine/CombatEngine.cs` and add your step to the step list in the `Initialize()` method:

```csharp
public void Initialize(...)
{
    // ... create context ...

    // Create battle flow steps
    var steps = new List<IBattleFlowStep>
    {
        // === FASE 1: SETUP ===
        new CreateFieldStep(_battleFieldFactory),
        new AssignActionProvidersStep(),
        new CreateQueueStep(_battleQueueFactory),
        new ValidateInitialStateStep(),

        // Add your step in the appropriate phase
        new CustomSetupStep(), // Example: Add after validation

        new CreateDependenciesStep(...),

        // === FASE 2: EJECUCIÓN ===
        new BattleStartFlowStep(),
        new ExecuteBattleLoopStep(),

        // === FASE 3: CLEANUP ===
        new BattleEndFlowStep()
    };

    // ... create executor ...
}
```

**Important**: Place your step in the correct phase:

-   **Setup Phase**: Before battle starts (after validation, before dependencies)
-   **Execution Phase**: During battle (before or after battle loop)
-   **Cleanup Phase**: After battle ends (before BattleEndFlowStep)

### Step 3: Update BattleFlowContext (if needed)

If your step needs new context data, update `Engine/BattleFlow/BattleFlowContext.cs`:

```csharp
public class BattleFlowContext
{
    // ... existing properties ...

    // Add new property for your step
    public CustomData CustomProperty { get; set; }
}
```

### Step 4: Test Your Step

Create unit tests for your step:

```csharp
[Test]
public async Task CustomSetupStep_ExecutesSetup()
{
    // Arrange
    var step = new CustomSetupStep();
    var context = CreateTestBattleFlowContext();

    // Act
    var result = await step.ExecuteAsync(context);

    // Assert
    Assert.IsTrue(result);
    // Verify expected behavior
}
```

## Design Principles

1. **Separation of Concerns**: Each step handles one aspect of battle lifecycle
2. **Dependency Injection**: Steps receive dependencies through context
3. **Testability**: Each step can be tested independently
4. **Extensibility**: New steps can be added without modifying existing ones
5. **Validation**: State validation between critical steps
6. **Order Matters**: Step execution order is critical for correct battle flow

## Common Patterns

### Reading from Context

```csharp
public async Task<bool> ExecuteAsync(BattleFlowContext context)
{
    // Read input parameters
    var rules = context.Rules;
    var playerParty = context.PlayerParty;
    var enemyParty = context.EnemyParty;

    // Read created components
    var field = context.Field;
    var queueService = context.QueueService;

    // Your logic here

    return await Task.FromResult(true);
}
```

### Writing to Context

```csharp
public async Task<bool> ExecuteAsync(BattleFlowContext context)
{
    // Create or modify context properties
    context.Field = CreateField();
    context.Outcome = BattleOutcome.PlayerWin;

    // Your logic here

    return await Task.FromResult(true);
}
```

### Using Dependencies

```csharp
public class CustomStep : IBattleFlowStep
{
    private readonly ISomeService _service;

    public CustomStep(ISomeService service)
    {
        _service = service;
    }

    public async Task<bool> ExecuteAsync(BattleFlowContext context)
    {
        // Use dependency
        var result = _service.DoSomething(context);

        return await Task.FromResult(true);
    }
}
```

## Battle Lifecycle

```
Initialize()
  ↓
[Setup Phase]
  ├─ CreateFieldStep
  ├─ AssignActionProvidersStep
  ├─ CreateQueueStep
  ├─ ValidateInitialStateStep
  └─ CreateDependenciesStep
  ↓
[Execution Phase]
  ├─ BattleStartFlowStep
  └─ ExecuteBattleLoopStep
       └─ [Turn Loop]
            └─ TurnEngine.ExecuteTurn()
                 └─ [23 Turn Steps]
  ↓
[Cleanup Phase]
  └─ BattleEndFlowStep
  ↓
Result
```

## Related Documentation

-   `../BattleFlowContext.cs` - Battle flow context structure
-   `../BattleFlowExecutor.cs` - Step execution engine
-   `../../README.md` - Engine overview
-   `../../TurnFlow/Steps/README.md` - Turn step documentation
-   `../../../Actions/README.md` - Action system
-   `../../../Handlers/README.md` - Handler system
