# Battle Events System

The events system provides a centralized event manager for battle-related events. It enables loose coupling between battle execution and observers (statistics, logging, debugging, etc.).

## Architecture

The events system uses a **static event manager pattern**:

-   **BattleEventManager**: Central static event manager
-   **BattleEventArgs**: Base event arguments with field reference
-   **Specific EventArgs**: Event-specific data containers
-   **Production vs Debug Events**: Separates production and debug events

## Components

### BattleEventManager.cs

Static event manager that provides centralized event handling.

**Key Properties:**

-   `IsDebugMode`: Whether debug events should be raised (default: false)

**Production Events (Always Available):**

| Event              | EventArgs                 | Purpose                     |
| ------------------ | ------------------------- | --------------------------- |
| **BattleStart**    | `BattleStartEventArgs`    | Raised when battle starts   |
| **BattleEnd**      | `BattleEndEventArgs`      | Raised when battle ends     |
| **TurnStart**      | `TurnEventArgs`           | Raised when turn starts     |
| **TurnEnd**        | `TurnEventArgs`           | Raised when turn ends       |
| **ActionExecuted** | `ActionExecutedEventArgs` | Raised when action executes |

**Debug Events (Only in Debug Mode):**

| Event                  | EventArgs                     | Purpose                          |
| ---------------------- | ----------------------------- | -------------------------------- |
| **StepExecuted**       | `StepExecutedEventArgs`       | Raised when step executes        |
| **StepStarted**        | `StepStartedEventArgs`        | Raised when step starts          |
| **StepFinished**       | `StepFinishedEventArgs`       | Raised when step finishes        |
| **BattleStateChanged** | `BattleStateChangedEventArgs` | Raised when battle state changes |

**Event Raising Methods:**

-   `RaiseBattleStart(field)`: Raises battle start event
-   `RaiseBattleEnd(outcome, field)`: Raises battle end event
-   `RaiseTurnStart(turnNumber, field)`: Raises turn start event
-   `RaiseTurnEnd(turnNumber, field)`: Raises turn end event
-   `RaiseActionExecuted(action, field, reactions)`: Raises action executed event
-   `RaiseStepExecuted(stepName, stepType, field, duration)`: Raises step executed (debug)
-   `RaiseStepStarted(stepName, stepType, field)`: Raises step started (debug)
-   `RaiseStepFinished(stepName, stepType, field, duration, shouldContinue)`: Raises step finished (debug)
-   `RaiseBattleStateChanged(field, changeDescription)`: Raises state changed (debug)
-   `ClearAll()`: Clears all event subscriptions

### BattleEvents.cs

Event argument classes for battle events.

| Class                           | Inherits          | Properties                                           | Purpose                          |
| ------------------------------- | ----------------- | ---------------------------------------------------- | -------------------------------- |
| **BattleEventArgs**             | `EventArgs`       | `Field`                                              | Base class for all battle events |
| **BattleStartEventArgs**        | `BattleEventArgs` | -                                                    | Battle start event data          |
| **BattleEndEventArgs**          | `BattleEventArgs` | `Outcome`                                            | Battle end event data            |
| **TurnEventArgs**               | `BattleEventArgs` | `TurnNumber`                                         | Turn start/end event data        |
| **ActionExecutedEventArgs**     | `BattleEventArgs` | `Action`, `Reactions`                                | Action execution event data      |
| **StepExecutedEventArgs**       | `BattleEventArgs` | `StepName`, `StepType`, `Duration`                   | Step execution event data        |
| **StepStartedEventArgs**        | `BattleEventArgs` | `StepName`, `StepType`                               | Step started event data          |
| **StepFinishedEventArgs**       | `BattleEventArgs` | `StepName`, `StepType`, `Duration`, `ShouldContinue` | Step finished event data         |
| **BattleStateChangedEventArgs** | `BattleEventArgs` | `ChangeDescription`                                  | Battle state change event data   |

## Usage Examples

### Subscribing to Production Events

```csharp
// Subscribe to battle start
BattleEventManager.BattleStart += (sender, e) =>
{
    Console.WriteLine($"Battle started on field: {e.Field}");
};

// Subscribe to action execution
BattleEventManager.ActionExecuted += (sender, e) =>
{
    Console.WriteLine($"Action executed: {e.Action.GetType().Name}");
    Console.WriteLine($"Reactions: {e.Reactions.Count()}");
};

// Subscribe to battle end
BattleEventManager.BattleEnd += (sender, e) =>
{
    Console.WriteLine($"Battle ended: {e.Outcome}");
};
```

### Subscribing to Debug Events

```csharp
// Enable debug mode
BattleEventManager.IsDebugMode = true;

// Subscribe to step execution
BattleEventManager.StepExecuted += (sender, e) =>
{
    Console.WriteLine($"Step {e.StepName} executed in {e.Duration.TotalMilliseconds}ms");
};

// Subscribe to battle state changes
BattleEventManager.BattleStateChanged += (sender, e) =>
{
    Console.WriteLine($"Battle state changed: {e.ChangeDescription}");
};
```

### Statistics Collection Example

```csharp
public class StatisticsCollector
{
    private int _totalActions = 0;

    public void Subscribe()
    {
        BattleEventManager.ActionExecuted += OnActionExecuted;
        BattleEventManager.BattleStart += OnBattleStart;
    }

    public void Unsubscribe()
    {
        BattleEventManager.ActionExecuted -= OnActionExecuted;
        BattleEventManager.BattleStart -= OnBattleStart;
    }

    private void OnActionExecuted(object sender, ActionExecutedEventArgs e)
    {
        _totalActions++;
    }

    private void OnBattleStart(object sender, BattleStartEventArgs e)
    {
        _totalActions = 0; // Reset on battle start
    }
}
```

### Logging Example

```csharp
public class BattleLogger
{
    public void Subscribe()
    {
        BattleEventManager.BattleStart += OnBattleStart;
        BattleEventManager.TurnStart += OnTurnStart;
        BattleEventManager.ActionExecuted += OnActionExecuted;
        BattleEventManager.BattleEnd += OnBattleEnd;
    }

    private void OnBattleStart(object sender, BattleStartEventArgs e)
    {
        Log.Info("Battle started");
    }

    private void OnTurnStart(object sender, TurnEventArgs e)
    {
        Log.Info($"Turn {e.TurnNumber} started");
    }

    private void OnActionExecuted(object sender, ActionExecutedEventArgs e)
    {
        Log.Debug($"Action: {e.Action.GetType().Name}");
    }

    private void OnBattleEnd(object sender, BattleEndEventArgs e)
    {
        Log.Info($"Battle ended: {e.Outcome}");
    }
}
```

## How to Add New Events

### Step 1: Create EventArgs Class

Create a new event args class in `Infrastructure/Events/BattleEvents.cs`:

```csharp
/// <summary>
/// Event arguments for your new event.
/// </summary>
public class YourNewEventArgs : BattleEventArgs
{
    public string YourProperty { get; set; }
    public int YourOtherProperty { get; set; }
}
```

### Step 2: Add Event to BattleEventManager

Open `Infrastructure/Events/BattleEventManager.cs` and add:

```csharp
public static class BattleEventManager
{
    // ... existing events ...

    /// <summary>
    /// Event raised when your event occurs.
    /// </summary>
    public static event EventHandler<YourNewEventArgs> YourNewEvent;

    // ... rest of class ...
}
```

### Step 3: Add Event Raiser Method

Add a method to raise the event:

```csharp
/// <summary>
/// Raises the YourNewEvent event.
/// </summary>
public static void RaiseYourNewEvent(BattleField field, string yourProperty, int yourOtherProperty)
{
    if (YourNewEvent != null)
    {
        YourNewEvent(null, new YourNewEventArgs
        {
            Field = field,
            YourProperty = yourProperty,
            YourOtherProperty = yourOtherProperty
        });
    }
}
```

### Step 4: Raise Event in Code

Raise the event where appropriate:

```csharp
// In your step or handler
BattleEventManager.RaiseYourNewEvent(field, "value", 123);
```

### Step 5: Subscribe to Event

Subscribe to the event where needed:

```csharp
BattleEventManager.YourNewEvent += (sender, e) =>
{
    Console.WriteLine($"Your event: {e.YourProperty}, {e.YourOtherProperty}");
};
```

## Design Principles

1. **Centralized**: Single point for all battle events
2. **Loose Coupling**: Observers don't need direct references to battle components
3. **Performance**: Only creates event args if subscribers exist
4. **Separation**: Production events always available, debug events optional
5. **Extensible**: Easy to add new events

## Performance Considerations

-   **Conditional Creation**: Event args only created if subscribers exist
-   **Debug Mode**: Debug events only raised when debug mode enabled
-   **Minimal Overhead**: Event system has minimal impact on battle performance
-   **Null Checks**: All event raisers check for null subscribers before creating args

## Event Subscription Best Practices

1. **Subscribe Early**: Subscribe before battle starts
2. **Unsubscribe**: Always unsubscribe when done to prevent memory leaks
3. **Exception Handling**: Handle exceptions in event handlers to prevent breaking battle
4. **Thread Safety**: Events are raised synchronously, handlers should be fast
5. **Multiple Subscribers**: Multiple subscribers can listen to same event

## Related Documentation

-   `../Statistics/README.md` - Statistics system (uses events)
-   `BattleEventManager.cs` - Event manager implementation
-   `BattleEvents.cs` - Event argument classes
-   `../../Engine/README.md` - Engine that raises events
