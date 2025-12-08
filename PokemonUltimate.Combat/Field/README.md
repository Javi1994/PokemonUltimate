# Battle Field

The field system represents the complete battlefield state, including both sides, weather, terrain, and battle rules.

## Architecture

The battlefield is organized hierarchically:

-   **BattleField**: Top-level battlefield container
-   **BattleSide**: Represents one side of the battle (player or enemy)
-   **BattleSlot**: Represents a single Pokemon position on a side

## Components

### BattleField.cs

The main battlefield class that contains both sides and global field conditions.

**Properties:**

-   `PlayerSide`: The player's side of the field
-   `EnemySide`: The enemy's side of the field
-   `Rules`: Battle configuration and rules
-   `Weather`: Current weather condition
-   `WeatherDuration`: Remaining weather duration (0 = infinite)
-   `WeatherData`: Weather data for current weather
-   `Terrain`: Current terrain condition
-   `TerrainDuration`: Remaining terrain duration (0 = infinite)
-   `TerrainData`: Terrain data for current terrain

**Key Methods:**

-   `Initialize()`: Initializes battlefield with rules and parties
-   `GetAllActiveSlots()`: Gets all active Pokemon slots
-   `GetAllSlots()`: Gets all slots (active and inactive)
-   `SetWeather()`: Sets weather condition with duration
-   `SetTerrain()`: Sets terrain condition with duration
-   `ClearWeather()`: Clears weather condition
-   `ClearTerrain()`: Clears terrain condition

### BattleSide.cs

Represents one side of the battlefield (player or enemy).

**Properties:**

-   `IsPlayer`: Whether this is the player's side
-   `Slots`: List of battle slots on this side
-   `ActiveSlots`: List of currently active slots
-   `SideConditions`: Dictionary of side-specific conditions (Reflect, Light Screen, etc.)

**Key Methods:**

-   `GetActiveSlot(int index)`: Gets active slot by index
-   `GetSlot(int index)`: Gets slot by index (active or inactive)
-   `HasActivePokemon()`: Checks if side has active Pokemon
-   `GetAllActiveSlots()`: Gets all active slots
-   `SetSideCondition()`: Sets a side condition
-   `HasSideCondition()`: Checks for side condition
-   `ClearSideCondition()`: Clears side condition

### BattleSlot.cs

Represents a single Pokemon position on a battle side.

**Properties:**

-   `Side`: The battle side this slot belongs to
-   `Index`: Slot index on the side
-   `Pokemon`: The Pokemon instance in this slot (null if empty)
-   `IsActive`: Whether this slot is currently active
-   `ActionProvider`: Provider for actions (player input, AI, etc.)
-   `StatStages`: Current stat stage modifiers
-   `StatusCondition`: Current status condition
-   `ProtectTracker`: Tracks protection move states
-   `SemiInvulnerableState`: Tracks semi-invulnerable move states
-   `ChargingMoveState`: Tracks charging move states
-   `MoveStateTracker`: Tracks move states (multi-turn, etc.)

**Key Methods:**

-   `SetPokemon()`: Sets Pokemon in slot
-   `ClearPokemon()`: Removes Pokemon from slot
-   `IsEmpty()`: Checks if slot is empty
-   `HasStatus()`: Checks for status condition
-   `ApplyStatus()`: Applies status condition
-   `ClearStatus()`: Clears status condition
-   `ModifyStatStage()`: Modifies stat stage
-   `GetStatModifier()`: Gets stat modifier from stages

### BattleRules.cs

Battle configuration and rules.

**Properties:**

-   `PlayerSlots`: Number of active slots for player
-   `EnemySlots`: Number of active slots for enemy
-   `BattleType`: Type of battle (Single, Double, Triple, etc.)
-   `LevelCap`: Maximum Pokemon level
-   `ItemUsage`: Whether items can be used
-   `MegaEvolution`: Whether Mega Evolution is allowed
-   `ZMove`: Whether Z-Moves are allowed
-   `Dynamax`: Whether Dynamax is allowed

## Field Files

-   **BattleField.cs** - Main battlefield class
-   **BattleSide.cs** - Battle side representation
-   **BattleSlot.cs** - Battle slot representation
-   **BattleRules.cs** - Battle configuration and rules

## Field State Management

### Weather State

Weather is managed through `WeatherState` value object:

-   Weather type (None, Sun, Rain, Hail, Sandstorm, etc.)
-   Duration in turns (0 = infinite)
-   Weather data reference

### Terrain State

Terrain is managed through `TerrainState` value object:

-   Terrain type (None, Electric, Grassy, Misty, Psychic)
-   Duration in turns (0 = infinite)
-   Terrain data reference

### Side Conditions

Side conditions are stored per side:

-   Reflect: Reduces physical damage
-   Light Screen: Reduces special damage
-   Aurora Veil: Reduces all damage (requires Hail)
-   Safeguard: Prevents status conditions
-   Mist: Prevents stat reductions
-   And more...

## Slot State Management

Each slot tracks:

-   **Pokemon**: The Pokemon instance
-   **Stat Stages**: Current stat modifiers (-6 to +6)
-   **Status Condition**: Current status (None, Burn, Poison, etc.)
-   **Protect Tracker**: Protection move states
-   **Semi-Invulnerable State**: Semi-invulnerable move states
-   **Charging Move State**: Charging move states
-   **Move State Tracker**: Multi-turn move states

## Usage Example

```csharp
// Create battlefield
var field = new BattleField();
field.Initialize(rules, playerParty, enemyParty);

// Access sides
var playerSide = field.PlayerSide;
var enemySide = field.EnemySide;

// Access slots
var playerSlot = playerSide.GetActiveSlot(0);
var enemySlot = enemySide.GetActiveSlot(0);

// Set weather
field.SetWeather(Weather.Sun, 5); // Sun for 5 turns

// Set terrain
field.SetTerrain(Terrain.Electric, 5); // Electric Terrain for 5 turns

// Set side condition
playerSide.SetSideCondition(SideCondition.Reflect, 5);

// Access Pokemon
var pokemon = playerSlot.Pokemon;
```

## Design Principles

1. **Hierarchical Structure**: Field → Side → Slot organization
2. **State Management**: Centralized state tracking
3. **Immutability**: Field state changes through methods, not direct property access
4. **Separation of Concerns**: Field manages structure, slots manage Pokemon state
5. **Testability**: Field can be tested independently

## Field Queries

Common field queries:

-   `GetAllActiveSlots()`: All active Pokemon slots
-   `GetAllSlots()`: All slots (active and inactive)
-   `HasActivePokemon(side)`: Check if side has active Pokemon
-   `GetSlot(side, index)`: Get specific slot
-   `IsWeatherActive(weather)`: Check if weather is active
-   `IsTerrainActive(terrain)`: Check if terrain is active
-   `HasSideCondition(side, condition)`: Check for side condition

## Related Documentation

-   `../Engine/README.md` - Engine overview
-   `../Infrastructure/ValueObjects/` - Value objects for state tracking
-   `../Actions/README.md` - Actions that modify field state
-   `../Handlers/README.md` - Handlers that read field state
