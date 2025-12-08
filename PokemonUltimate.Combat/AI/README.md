# AI System

The AI system provides action providers that make decisions for Pokemon during battle. These implementations of `IActionProvider` can be used for enemy AI, testing, or automated battle simulations.

## Architecture

All AI implementations inherit from `ActionProviderBase` and implement the `GetAction()` method. They receive the current battlefield state and their slot, then return a `BattleAction` (or null to skip the turn).

## Complete AI List

| Class                 | Purpose                            | Strategy                                  | Use Case                      |
| --------------------- | ---------------------------------- | ----------------------------------------- | ----------------------------- |
| **RandomAI.cs**       | Selects random valid move          | Random move selection                     | Testing, basic enemy behavior |
| **AlwaysAttackAI.cs** | Always uses first available move   | First move with PP                        | Testing, predictable behavior |
| **FixedMoveAI.cs**    | Always uses specific move          | Fixed move selection                      | Move testing, debugging       |
| **NoActionAI.cs**     | Never provides action              | Always returns null                       | Testing single-side scenarios |
| **SmartAI.cs**        | Strategic decisions with switching | HP-based switching, type effectiveness    | Advanced enemy AI             |
| **TeamBattleAI.cs**   | Team battle AI with auto-switching | Auto-switch on faint, strategic switching | Team battles, simulations     |

## Detailed AI Descriptions

### RandomAI.cs

**Purpose**: Simple AI that selects a random valid move.

**Strategy:**

-   Gets all moves with PP > 0
-   Randomly selects one move
-   Randomly selects target from valid targets
-   Returns `UseMoveAction`

**Configuration:**

-   `TargetResolver`: Required for target resolution
-   `seed`: Optional seed for reproducible randomness

**Usage:**

```csharp
var ai = new RandomAI(targetResolver, seed: 12345);
var action = await ai.GetAction(field, slot);
```

### AlwaysAttackAI.cs

**Purpose**: Always uses the first available move.

**Strategy:**

-   Gets first move with PP > 0
-   Uses first valid target
-   Returns `UseMoveAction`

**Configuration:**

-   `TargetResolver`: Required for target resolution

**Usage:**

```csharp
var ai = new AlwaysAttackAI(targetResolver);
var action = await ai.GetAction(field, slot);
```

### FixedMoveAI.cs

**Purpose**: Always uses a specific move (for testing).

**Strategy:**

-   Checks if Pokemon has the specified move with PP
-   Uses first valid target
-   Returns `UseMoveAction` or null if move unavailable

**Configuration:**

-   `moveToUse`: The move instance to always use

**Usage:**

```csharp
var move = new MoveInstance(GetMoveData("thunderbolt"));
var ai = new FixedMoveAI(move);
var action = await ai.GetAction(field, slot);
```

### NoActionAI.cs

**Purpose**: Never provides an action (always returns null).

**Strategy:**

-   Always returns null
-   Pokemon skips its turn

**Use Cases:**

-   Testing single-side scenarios
-   Move testing where only one side acts
-   Debugging specific battle situations

**Usage:**

```csharp
var ai = new NoActionAI();
var action = await ai.GetAction(field, slot); // Always null
```

### SmartAI.cs

**Purpose**: Strategic AI that considers switching and type effectiveness.

**Strategy:**

-   **Switching Logic**:
    -   Considers switching when HP < threshold (default 30%)
    -   Switches with probability (default 50%)
    -   Prefers switching to low HP Pokemon
-   **Move Selection**:
    -   Evaluates moves based on type effectiveness
    -   Prefers super-effective moves
    -   Falls back to random move if no good options

**Configuration:**

-   `TargetResolver`: Required for target resolution
-   `switchThreshold`: HP percentage threshold for switching (default: 0.3)
-   `switchChance`: Probability of switching when conditions met (default: 0.5)
-   `seed`: Optional seed for reproducible randomness

**Usage:**

```csharp
var ai = new SmartAI(
    targetResolver,
    switchThreshold: 0.3,
    switchChance: 0.5,
    seed: 12345
);
var action = await ai.GetAction(field, slot);
```

### TeamBattleAI.cs

**Purpose**: AI designed for team battles with automatic switching.

**Strategy:**

-   **Auto-Switching**:
    -   Automatically switches if slot is empty or Pokemon is fainted
    -   Switches to first available Pokemon in party
-   **Strategic Switching**:
    -   Considers switching when HP < threshold (default 30%)
    -   Switches with probability (default 50%)
-   **Move Selection**:
    -   Evaluates moves based on type effectiveness
    -   Prefers super-effective moves
    -   Falls back to random move

**Configuration:**

-   `TargetResolver`: Required for target resolution
-   `switchThreshold`: HP percentage threshold for switching (default: 0.3)
-   `switchChance`: Probability of switching when conditions met (default: 0.5)
-   `seed`: Optional seed for reproducible randomness

**Usage:**

```csharp
var ai = new TeamBattleAI(
    targetResolver,
    switchThreshold: 0.3,
    switchChance: 0.5,
    seed: 12345
);
var action = await ai.GetAction(field, slot);
```

## Action Provider Interface

All AI classes implement `IActionProvider`:

```csharp
public interface IActionProvider
{
    Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot);
}
```

## How to Add a New AI

### Step 1: Create AI Class

Create a new file in `AI/` (e.g., `CustomAI.cs`):

```csharp
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.AI
{
    public class CustomAI : ActionProviderBase
    {
        private readonly TargetResolver _targetResolver;

        public CustomAI(TargetResolver targetResolver)
        {
            _targetResolver = targetResolver ?? throw new ArgumentNullException(nameof(targetResolver));
        }

        public override Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            // Your AI logic here
            // Return UseMoveAction, SwitchAction, or null

            return Task.FromResult<BattleAction>(null);
        }
    }
}
```

### Step 2: Implement Decision Logic

Implement your AI's decision-making logic:

```csharp
public override Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
{
    // Check if slot is active
    if (!mySlot.IsActive())
        return Task.FromResult<BattleAction>(null);

    // Your decision logic:
    // 1. Evaluate battlefield state
    // 2. Consider switching if needed
    // 3. Select best move
    // 4. Select best target
    // 5. Return action

    var selectedMove = SelectBestMove(mySlot, field);
    var target = SelectBestTarget(mySlot, selectedMove, field);

    return Task.FromResult<BattleAction>(
        new UseMoveAction(mySlot, target, selectedMove)
    );
}
```

### Step 3: Test Your AI

Create unit tests for your AI:

```csharp
[Test]
public async Task CustomAI_SelectsAction()
{
    // Arrange
    var ai = new CustomAI(targetResolver);
    var field = CreateTestField();
    var slot = CreateTestSlot();

    // Act
    var action = await ai.GetAction(field, slot);

    // Assert
    Assert.IsNotNull(action);
    // Verify expected behavior
}
```

## Common Patterns

### Getting Available Moves

```csharp
var availableMoves = mySlot.Pokemon.Moves
    .Where(m => m.HasPP)
    .ToList();
```

### Getting Valid Targets

```csharp
var validTargets = _targetResolver.GetBasicTargets(
    mySlot,
    selectedMove.Move,
    field
);
```

### Creating Move Action

```csharp
var action = new UseMoveAction(mySlot, target, moveInstance);
return Task.FromResult<BattleAction>(action);
```

### Creating Switch Action

```csharp
var newPokemon = GetSwitchTarget(side);
var action = new SwitchAction(mySlot, newPokemon, mySlot);
return Task.FromResult<BattleAction>(action);
```

## Design Principles

1. **Strategy Pattern**: Different AI implementations for different strategies
2. **Dependency Injection**: AI receives dependencies (TargetResolver, etc.)
3. **Async/Await**: All AI methods are async for future extensibility
4. **Null Safety**: Returns null when no action available
5. **Testability**: AI can be tested independently

## Related Documentation

-   `../Infrastructure/Providers/README.md` - Action provider base classes
-   `../Target/README.md` - Target resolution system
-   `../Field/README.md` - Field structure
-   `../Actions/README.md` - Action system
