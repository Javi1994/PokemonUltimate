# Battle Message System

The message system provides formatted battle messages for display to players. Supports localization and centralized message formatting logic.

## Architecture

The message system uses an **interface-based design**:

-   **IBattleMessageFormatter**: Interface for message formatters
-   **BattleMessageFormatter**: Default implementation using localization
-   **Localization Integration**: Uses ILocalizationProvider for multi-language support

## Components

### IBattleMessageFormatter.cs

Interface for battle message formatting.

**Methods:**

| Method                                                                 | Purpose                                       |
| ---------------------------------------------------------------------- | --------------------------------------------- |
| `FormatMoveUsed(pokemon, move)`                                        | Formats "Pokemon used Move" message           |
| `FormatMoveUsedDuringSwitch(switchingPokemon, attackingPokemon, move)` | Formats move used during switch message       |
| `Format(template, args)`                                               | Formats message using template with arguments |

### BattleMessageFormatter.cs

Default implementation using `ILocalizationProvider`.

**Features:**

-   Localized message formatting
-   Pokemon name localization
-   Move name localization
-   Template-based message formatting

**Configuration:**

-   `localizationProvider`: Localization provider (default: LocalizationProvider)

**Usage:**

```csharp
var formatter = new BattleMessageFormatter();
var message = formatter.FormatMoveUsed(pikachu, thunderbolt);
// Output: "Pikachu used Thunderbolt!"
```

## Message Types

### Move Used Messages

```csharp
// Standard move usage
var message = formatter.FormatMoveUsed(pokemon, move);
// Example: "Pikachu used Thunderbolt!"

// Move used during switch
var message = formatter.FormatMoveUsedDuringSwitch(
    switchingPokemon,
    attackingPokemon,
    move
);
// Example: "Pikachu is switching out! Charizard used Flamethrower!"
```

### Template-Based Messages

```csharp
// Format using template
var message = formatter.Format(
    "{0} dealt {1} damage to {2}!",
    attacker.Name,
    damage,
    defender.Name
);
// Example: "Pikachu dealt 50 damage to Charizard!"
```

## Usage Examples

### Basic Message Formatting

```csharp
var formatter = new BattleMessageFormatter();

// Format move used message
var moveMessage = formatter.FormatMoveUsed(pikachu, thunderbolt);

// Format switch message
var switchMessage = formatter.FormatMoveUsedDuringSwitch(
    switchingPokemon,
    attackingPokemon,
    move
);

// Format custom message
var customMessage = formatter.Format(
    "{0} fainted!",
    pokemon.Name
);
```

### Using in Actions

```csharp
public class UseMoveAction : BattleAction
{
    public override async Task ExecuteVisual(IBattleView view)
    {
        var formatter = new BattleMessageFormatter();
        var message = formatter.FormatMoveUsed(User.Pokemon, Move);
        await view.ShowMessage(message);
    }
}
```

### Custom Localization Provider

```csharp
var customProvider = new CustomLocalizationProvider();
var formatter = new BattleMessageFormatter(customProvider);
var message = formatter.FormatMoveUsed(pokemon, move);
```

## How to Add New Message Types

### Step 1: Add Method to Interface

Open `Infrastructure/Messages/Definition/IBattleMessageFormatter.cs`:

```csharp
public interface IBattleMessageFormatter
{
    // ... existing methods ...

    /// <summary>
    /// Formats a message for your new event.
    /// </summary>
    string FormatYourNewEvent(PokemonInstance pokemon, string data);
}
```

### Step 2: Implement in BattleMessageFormatter

Open `Infrastructure/Messages/BattleMessageFormatter.cs`:

```csharp
public class BattleMessageFormatter : IBattleMessageFormatter
{
    // ... existing methods ...

    public string FormatYourNewEvent(PokemonInstance pokemon, string data)
    {
        if (pokemon == null)
            throw new ArgumentNullException(nameof(pokemon));

        return _localizationProvider.GetString(
            LocalizationKey.YourNewEventKey,
            pokemon.Species.GetDisplayName(_localizationProvider),
            data
        );
    }
}
```

### Step 3: Use Your New Message Type

```csharp
var formatter = new BattleMessageFormatter();
var message = formatter.FormatYourNewEvent(pokemon, "data");
```

## Design Principles

1. **Interface-Based**: All formatters implement IBattleMessageFormatter
2. **Localization**: Uses ILocalizationProvider for multi-language support
3. **Centralized**: All message formatting in one place
4. **Type-Safe**: Methods for specific message types
5. **Extensible**: Easy to add new message types

## Related Documentation

-   `IBattleMessageFormatter.cs` - Message formatter interface
-   `BattleMessageFormatter.cs` - Default formatter implementation
-   `../../Actions/MessageAction.cs` - Message action that uses formatter
-   `../../../Localization/README.md` - Localization system documentation
