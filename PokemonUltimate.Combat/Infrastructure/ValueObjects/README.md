# Value Objects

Value objects represent immutable state in the battle system. They follow the Value Object pattern, ensuring immutability and providing methods that return new instances rather than modifying existing ones.

## Architecture

All value objects follow these principles:

-   **Immutability**: Properties are read-only, modifications return new instances
-   **Value Semantics**: Two instances with same values are considered equal
-   **Validation**: Constructors and methods validate input
-   **Fluent API**: Methods return new instances for chaining

## Complete Value Object List

| Class                        | Purpose                                                 | Key Properties                                           | Key Methods                                                                     |
| ---------------------------- | ------------------------------------------------------- | -------------------------------------------------------- | ------------------------------------------------------------------------------- |
| **StatStages.cs**            | Tracks stat stage modifiers (-6 to +6)                  | Stages dictionary                                        | `GetStage()`, `ModifyStage()`, `Reset()`, `Copy()`                              |
| **WeatherState.cs**          | Tracks battlefield weather                              | `Weather`, `Duration`, `WeatherData`                     | `SetWeather()`, `Clear()`, `DecrementDuration()`                                |
| **TerrainState.cs**          | Tracks battlefield terrain                              | `Terrain`, `Duration`, `TerrainData`                     | `SetTerrain()`, `Clear()`, `DecrementDuration()`                                |
| **ChargingMoveState.cs**     | Tracks charging moves (Solar Beam, etc.)                | `MoveName`                                               | `SetMove()`, `Clear()`                                                          |
| **SemiInvulnerableState.cs** | Tracks semi-invulnerable moves (Fly, Dig, etc.)         | `MoveName`, `IsCharging`                                 | `SetMove()`, `Clear()`, `SetReady()`                                            |
| **MoveStateTracker.cs**      | Tracks all move-related states                          | `SemiInvulnerableState`, `ChargingMoveState`             | `WithSemiInvulnerableState()`, `WithChargingMoveState()`, `Reset()`             |
| **ProtectTracker.cs**        | Tracks consecutive Protect uses                         | `ConsecutiveUses`                                        | `Increment()`, `Reset()`                                                        |
| **DamageTakenTracker.cs**    | Tracks damage taken for Counter/Mirror Coat/Focus Punch | `PhysicalDamage`, `SpecialDamage`, `WasHitWhileFocusing` | `AddPhysicalDamage()`, `AddSpecialDamage()`, `SetHitWhileFocusing()`, `Reset()` |

## Detailed Value Object Descriptions

### StatStages.cs

**Purpose**: Tracks stat stage modifiers for all stats (-6 to +6).

**Properties:**

-   Internal dictionary of `Stat` → `int` stages

**Key Methods:**

-   `GetStage(stat)`: Gets current stage for a stat
-   `ModifyStage(stat, change, out actualChange)`: Modifies stage with clamping, returns new instance
-   `Reset()`: Returns new instance with all stages at 0
-   `Copy()`: Creates copy of current instance

**Usage:**

```csharp
var stages = new StatStages();
var newStages = stages.ModifyStage(Stat.Attack, 2, out int actualChange);
// actualChange = 2 (if not clamped)
```

### WeatherState.cs

**Purpose**: Tracks battlefield weather condition with duration.

**Properties:**

-   `Weather`: Current weather type (None, Sun, Rain, etc.)
-   `Duration`: Remaining duration in turns (0 = infinite)
-   `WeatherData`: Weather data reference
-   `IsActive`: Whether weather is active
-   `IsInfinite`: Whether weather has infinite duration

**Key Methods:**

-   `SetWeather(weather, duration, weatherData)`: Sets weather, returns new instance
-   `Clear()`: Clears weather, returns new instance
-   `DecrementDuration()`: Decrements duration, clears if expired

**Usage:**

```csharp
var weather = new WeatherState();
var newWeather = weather.SetWeather(Weather.Sun, 5, sunData);
var decremented = newWeather.DecrementDuration();
```

### TerrainState.cs

**Purpose**: Tracks battlefield terrain condition with duration.

**Properties:**

-   `Terrain`: Current terrain type (None, Electric, Grassy, etc.)
-   `Duration`: Remaining duration in turns (0 = infinite)
-   `TerrainData`: Terrain data reference
-   `IsActive`: Whether terrain is active
-   `IsInfinite`: Whether terrain has infinite duration

**Key Methods:**

-   `SetTerrain(terrain, duration, terrainData)`: Sets terrain, returns new instance
-   `Clear()`: Clears terrain, returns new instance
-   `DecrementDuration()`: Decrements duration, clears if expired

**Usage:**

```csharp
var terrain = new TerrainState();
var newTerrain = terrain.SetTerrain(Terrain.Electric, 5, electricData);
var decremented = newTerrain.DecrementDuration();
```

### ChargingMoveState.cs

**Purpose**: Tracks charging moves that take multiple turns (Solar Beam, Sky Attack, etc.).

**Properties:**

-   `MoveName`: Name of charging move (null if none)
-   `IsActive`: Whether a charging move is active

**Key Methods:**

-   `SetMove(moveName)`: Sets charging move, returns new instance
-   `Clear()`: Clears charging move, returns new instance

**Usage:**

```csharp
var charging = new ChargingMoveState();
var newCharging = charging.SetMove("Solar Beam");
var cleared = newCharging.Clear();
```

### SemiInvulnerableState.cs

**Purpose**: Tracks semi-invulnerable moves (Fly, Dig, Dive, etc.).

**Properties:**

-   `MoveName`: Name of semi-invulnerable move (null if none)
-   `IsCharging`: True if charging (turn 1), false if attacking (turn 2)
-   `IsActive`: Whether a semi-invulnerable move is active

**Key Methods:**

-   `SetMove(moveName, isCharging)`: Sets semi-invulnerable move, returns new instance
-   `Clear()`: Clears semi-invulnerable move, returns new instance
-   `SetReady()`: Sets IsCharging to false (ready to attack), returns new instance

**Usage:**

```csharp
var semiInvulnerable = new SemiInvulnerableState();
var charging = semiInvulnerable.SetMove("Fly", isCharging: true);
var ready = charging.SetReady(); // Ready to attack
```

### MoveStateTracker.cs

**Purpose**: Tracks all move-related states for a Pokemon.

**Properties:**

-   `SemiInvulnerableState`: Semi-invulnerable move state
-   `ChargingMoveState`: Charging move state

**Key Methods:**

-   `WithSemiInvulnerableState(state)`: Updates semi-invulnerable state, returns new instance
-   `WithChargingMoveState(state)`: Updates charging move state, returns new instance
-   `Reset()`: Resets all states, returns new instance

**Usage:**

```csharp
var tracker = new MoveStateTracker();
var newTracker = tracker.WithChargingMoveState(chargingState);
var reset = newTracker.Reset();
```

### ProtectTracker.cs

**Purpose**: Tracks consecutive Protect uses for success rate calculation.

**Properties:**

-   `ConsecutiveUses`: Number of consecutive Protect uses

**Key Methods:**

-   `Increment()`: Increments consecutive uses, returns new instance
-   `Reset()`: Resets to zero, returns new instance

**Usage:**

```csharp
var tracker = new ProtectTracker();
var incremented = tracker.Increment();
var reset = incremented.Reset();
```

### DamageTakenTracker.cs

**Purpose**: Tracks damage taken during a turn for Counter/Mirror Coat/Focus Punch.

**Properties:**

-   `PhysicalDamage`: Physical damage taken this turn
-   `SpecialDamage`: Special damage taken this turn
-   `WasHitWhileFocusing`: Whether hit while focusing (Focus Punch)

**Key Methods:**

-   `AddPhysicalDamage(damage)`: Adds physical damage, returns new instance
-   `AddSpecialDamage(damage)`: Adds special damage, returns new instance
-   `SetHitWhileFocusing(wasHit)`: Sets focus hit flag, returns new instance
-   `Reset()`: Resets all values to zero, returns new instance

**Usage:**

```csharp
var tracker = new DamageTakenTracker();
var withPhysical = tracker.AddPhysicalDamage(50);
var withSpecial = withPhysical.AddSpecialDamage(30);
var reset = withSpecial.Reset();
```

## Value Object Pattern

All value objects follow this pattern:

```csharp
public class YourValueObject
{
    // Read-only properties
    public string Property { get; }

    // Private constructor for internal creation
    private YourValueObject(string property)
    {
        Property = property;
    }

    // Public constructor for initial state
    public YourValueObject()
    {
        Property = null; // or default value
    }

    // Methods return new instances
    public YourValueObject SetProperty(string value)
    {
        return new YourValueObject(value);
    }

    public YourValueObject Reset()
    {
        return new YourValueObject();
    }
}
```

## How to Add a New Value Object

### Step 1: Create Value Object Class

Create a new file in `Infrastructure/ValueObjects/` (e.g., `CustomState.cs`):

```csharp
namespace PokemonUltimate.Combat.Infrastructure.ValueObjects
{
    public class CustomState
    {
        public string Value { get; }

        public CustomState()
        {
            Value = null;
        }

        private CustomState(string value)
        {
            Value = value;
        }

        public CustomState SetValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            return new CustomState(value);
        }

        public CustomState Clear()
        {
            return new CustomState();
        }

        public bool IsActive => Value != null;
    }
}
```

### Step 2: Use in BattleSlot or Field

Update `Field/BattleSlot.cs` or `Field/BattleField.cs` to use your value object:

```csharp
public class BattleSlot
{
    private CustomState _customState;

    public CustomState CustomState => _customState;

    public void SetCustomState(CustomState state)
    {
        _customState = state ?? new CustomState();
    }
}
```

### Step 3: Test Your Value Object

Create unit tests:

```csharp
[Test]
public void CustomState_SetValue_CreatesNewInstance()
{
    // Arrange
    var state = new CustomState();

    // Act
    var newState = state.SetValue("test");

    // Assert
    Assert.AreNotSame(state, newState);
    Assert.AreEqual("test", newState.Value);
    Assert.IsNull(state.Value); // Original unchanged
}
```

## Design Principles

1. **Immutability**: All properties are read-only
2. **Value Semantics**: Two instances with same values are equivalent
3. **New Instances**: Methods return new instances, never modify existing
4. **Validation**: Constructors and methods validate input
5. **Fluent API**: Methods can be chained for convenience

## Usage Patterns

### Updating State

```csharp
// Always create new instance
var oldState = slot.StatStages;
var newState = oldState.ModifyStage(Stat.Attack, 2, out int change);
slot.SetStatStages(newState);
```

### Chaining Operations

```csharp
var state = new StatStages()
    .ModifyStage(Stat.Attack, 2, out _)
    .ModifyStage(Stat.Speed, 1, out _);
```

### Checking State

```csharp
if (slot.ChargingMoveState.IsActive)
{
    // Handle charging move
}
```

## Related Documentation

-   `../Field/README.md` - Field structure that uses value objects
-   `../Field/BattleSlot.cs` - Battle slot that contains value objects
-   `../Field/BattleField.cs` - Battle field that uses WeatherState and TerrainState
