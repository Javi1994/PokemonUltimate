# Target Resolution System

The target resolution system handles move targeting and target redirection effects. It determines which Pokemon a move will target, accounting for redirection abilities and moves.

## Architecture

The system consists of:

-   **TargetResolver**: Main resolver that determines move targets
-   **TargetRedirectionResolver**: Base class for redirection effects
-   **Specific Redirection Resolvers**: Handlers for specific redirection effects

## Components

### TargetResolver.cs

Main resolver that determines move targets and applies redirection effects.

**Key Methods:**

-   `ResolveTargets()`: Resolves targets for a move action
-   `ApplyRedirection()`: Applies target redirection effects
-   `GetValidTargets()`: Gets valid targets for a move
-   `IsValidTarget()`: Checks if a target is valid for a move

**Responsibilities:**

-   Determining move targets based on move targeting type
-   Applying redirection effects (Follow Me, Lightning Rod, etc.)
-   Validating target selection
-   Handling multi-target moves

### TargetRedirectionResolver.cs

Base class for target redirection resolvers. Provides common functionality for redirection effects.

**Key Methods:**

-   `CanRedirect()`: Checks if redirection can occur
-   `GetRedirectedTarget()`: Gets the redirected target
-   `ShouldRedirect()`: Determines if redirection should happen

### Specific Redirection Resolvers

#### FollowMeResolver.cs

Handles Follow Me and Rage Powder redirection effects.

-   Redirects all single-target moves to the user
-   Applies to moves that target opponents
-   Cannot redirect moves that target the user or allies

#### LightningRodResolver.cs

Handles Lightning Rod and Storm Drain redirection effects.

-   Redirects Electric-type moves (Lightning Rod) or Water-type moves (Storm Drain)
-   Only affects moves that target opponents
-   Provides immunity to redirected moves

## Target Resolution Files

-   **TargetResolver.cs** - Main target resolver
-   **TargetRedirectionResolver.cs** - Base redirection resolver
-   **TargetRedirectionResolvers/FollowMeResolver.cs** - Follow Me/Rage Powder resolver
-   **TargetRedirectionResolvers/LightningRodResolver.cs** - Lightning Rod/Storm Drain resolver

## Target Types

Moves can target:

-   **Single Target**: One specific Pokemon
-   **All Opponents**: All opposing Pokemon
-   **All Adjacent**: All adjacent Pokemon (including allies in doubles)
-   **Self**: The user
-   **Ally**: An ally Pokemon
-   **Field**: The battlefield (weather, terrain moves)
-   **Random**: Random target selection

## Redirection Priority

Redirection effects have priority order:

1. **Follow Me / Rage Powder**: Highest priority, redirects all single-target moves
2. **Lightning Rod / Storm Drain**: Redirects specific type moves
3. **Default Targeting**: Normal move targeting

## Usage Example

```csharp
// Create resolver
var targetResolver = new TargetResolver();

// Resolve targets for a move
var targets = targetResolver.ResolveTargets(
    moveAction: useMoveAction,
    field: battleField
);

// Apply redirection
var redirectedTargets = targetResolver.ApplyRedirection(
    moveAction: useMoveAction,
    targets: targets,
    field: battleField
);
```

## Design Principles

1. **Separation of Concerns**: Target resolution separate from move execution
2. **Extensibility**: New redirection effects can be added easily
3. **Priority System**: Clear priority order for redirection effects
4. **Validation**: Target validation before redirection
5. **Testability**: Resolvers can be tested independently

## Adding New Redirection Effects

To add a new redirection effect:

1. **Create Resolver**: Create a class inheriting from `TargetRedirectionResolver`
2. **Implement Logic**: Implement redirection logic
3. **Register**: Register resolver in `TargetResolver`
4. **Test**: Test redirection behavior
5. **Document**: Document redirection conditions and priority

## Related Documentation

-   `../Engine/TurnFlow/Steps/TargetResolutionStep.cs` - Step that uses target resolver
-   `../Actions/UseMoveAction.cs` - Move actions that need target resolution
-   `../Field/README.md` - Field structure
