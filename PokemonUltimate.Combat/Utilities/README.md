# Combat Utilities

Utility classes that provide helper functionality for the combat system.

## Components

### TurnOrderResolver.cs

Resolves turn order for battle actions based on priority and speed.

**Key Methods:**

-   `ResolveTurnOrder()`: Resolves turn order for a list of actions
-   `CompareActions()`: Compares two actions for turn order
-   `GetSpeed()`: Gets effective speed for a Pokemon
-   `CalculateSpeed()`: Calculates speed with modifiers

**Responsibilities:**

-   Sorting actions by priority (higher priority goes first)
-   Sorting actions by speed when priority is equal
-   Handling speed ties with random selection
-   Accounting for speed modifiers (stat stages, abilities, items)

**Sorting Logic:**

1. **Priority**: Actions with higher priority execute first
2. **Speed**: When priority is equal, faster Pokemon go first
3. **Random**: Speed ties are resolved randomly

### AccuracyChecker.cs

Checks move accuracy and determines if moves hit.

**Key Methods:**

-   `CheckAccuracy()`: Checks if a move hits its target
-   `CalculateAccuracy()`: Calculates effective accuracy
-   `IsHit()`: Determines if move hits (accounts for accuracy and evasion)

**Responsibilities:**

-   Calculating effective accuracy (move accuracy + accuracy modifiers)
-   Calculating effective evasion (target evasion + evasion modifiers)
-   Determining hit/miss based on accuracy check
-   Handling accuracy-related abilities and items

**Accuracy Calculation:**

-   Base accuracy from move data
-   Accuracy modifiers from stat stages
-   Ability modifiers (Compound Eyes, Hustle, etc.)
-   Item modifiers (Wide Lens, etc.)
-   Evasion modifiers from target

### BattleSlotExtensions.cs

Extension methods for BattleSlot to provide convenient access to battle-related data.

**Key Methods:**

-   `GetEffectiveStat()`: Gets effective stat with modifiers
-   `HasAbility()`: Checks if Pokemon has specific ability
-   `HasItem()`: Checks if Pokemon has specific item
-   `CanUseMove()`: Checks if Pokemon can use a move
-   And more utility methods...

**Responsibilities:**

-   Providing convenient access to Pokemon data
-   Simplifying common checks and queries
-   Reducing code duplication

## Utility Files

-   **TurnOrderResolver.cs** - Turn order resolution
-   **AccuracyChecker.cs** - Move accuracy checking
-   **BattleSlotExtensions.cs** - Battle slot extension methods

## Usage Examples

### Turn Order Resolution

```csharp
// Create resolver
var resolver = new TurnOrderResolver(randomProvider);

// Resolve turn order
var sortedActions = resolver.ResolveTurnOrder(
    actions: collectedActions,
    field: battleField
);
```

### Accuracy Checking

```csharp
// Create checker
var checker = new AccuracyChecker(randomProvider);

// Check if move hits
var hits = checker.CheckAccuracy(
    move: moveData,
    user: attackerSlot,
    target: defenderSlot,
    field: battleField
);
```

### Slot Extensions

```csharp
// Use extension methods
if (slot.HasAbility("intimidate"))
{
    // Handle Intimidate
}

var effectiveAttack = slot.GetEffectiveStat(StatType.Attack);
```

## Design Principles

1. **Single Responsibility**: Each utility handles one specific concern
2. **Stateless**: Utilities are stateless and can be reused
3. **Testability**: Utilities can be tested independently
4. **Convenience**: Extension methods provide convenient access
5. **Performance**: Utilities are optimized for performance

## Related Documentation

-   `../Engine/TurnFlow/Steps/ActionSortingStep.cs` - Uses TurnOrderResolver
-   `../Engine/TurnFlow/Steps/MoveAccuracyCheckStep.cs` - Uses AccuracyChecker
-   `../Field/BattleSlot.cs` - Extended by BattleSlotExtensions
