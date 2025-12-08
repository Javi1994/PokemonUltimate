# Combat Effect Handlers

The handler system provides a unified registry for processing abilities, items, and move effects. Handlers generate `BattleAction` instances in response to triggers, making the system modular and extensible.

## Architecture

The `CombatEffectHandlerRegistry` serves as the central registry for all effect handlers. Handlers are organized by:

-   **Type**: Ability, Item, or Move Effect handlers
-   **Trigger**: When the handler should activate
-   **Effect**: What the handler does

## Complete Handler List

### Ability Handlers (`Abilities/`)

| Class                        | Purpose                                                        | Trigger             | Effect                   |
| ---------------------------- | -------------------------------------------------------------- | ------------------- | ------------------------ |
| **ContactAbilityHandler.cs** | Contact-based abilities (Static, Flame Body, Rough Skin, etc.) | `OnContactReceived` | Status/damage on contact |
| **IntimidateHandler.cs**     | Lowers opponent Attack on switch-in                            | `OnSwitchIn`        | Lower opponent stat      |
| **MoxieHandler.cs**          | Raises Attack after KO'ing opponent                            | `OnKO`              | Raise stat on KO         |
| **SpeedBoostHandler.cs**     | Raises Speed at end of turn                                    | `OnTurnEnd`         | Raise stat on turn end   |

### Item Handlers (`Items/`)

| Class                     | Purpose                              | Trigger             | Effect                |
| ------------------------- | ------------------------------------ | ------------------- | --------------------- |
| **LeftoversHandler.cs**   | Heals 1/16 max HP at end of turn     | `OnTurnEnd`         | Healing               |
| **LifeOrbHandler.cs**     | Increases damage 1.3x, causes recoil | `OnMoveUse`         | Damage boost + recoil |
| **RockyHelmetHandler.cs** | Deals 1/6 max HP damage on contact   | `OnContactReceived` | Contact damage        |

### Move Effect Handlers (`Effects/`)

| Class                          | Purpose                                        | Effect Type        | Generates           |
| ------------------------------ | ---------------------------------------------- | ------------------ | ------------------- |
| **StatusEffectHandler.cs**     | Applies status conditions (Burn, Poison, etc.) | `StatusEffect`     | `ApplyStatusAction` |
| **StatChangeEffectHandler.cs** | Changes stat stages (Swords Dance, etc.)       | `StatChangeEffect` | `StatChangeAction`  |
| **RecoilEffectHandler.cs**     | Recoil damage to user (Take Down, etc.)        | `RecoilEffect`     | `DamageAction`      |
| **DrainEffectHandler.cs**      | Drains HP from target (Giga Drain, etc.)       | `DrainEffect`      | `HealAction`        |
| **HealEffectHandler.cs**       | Heals user (Recover, etc.)                     | `HealEffect`       | `HealAction`        |
| **FlinchEffectHandler.cs**     | Causes flinching (Bite, etc.)                  | `FlinchEffect`     | Flinch status       |
| **ProtectEffectHandler.cs**    | Protection moves (Protect, Detect)             | `ProtectEffect`    | Protection state    |
| **CounterEffectHandler.cs**    | Counter damage (Counter, Mirror Coat)          | `CounterEffect`    | `DamageAction`      |
| **StatusDamageHandler.cs**     | Status damage at end of turn                   | End of turn        | `DamageAction`      |
| **WeatherDamageHandler.cs**    | Weather damage at end of turn                  | End of turn        | `DamageAction`      |
| **TerrainHealingHandler.cs**   | Terrain healing at end of turn                 | End of turn        | `HealAction`        |
| **EntryHazardHandler.cs**      | Entry hazard damage on switch-in               | Switch-in          | `DamageAction`      |

### Checker Handlers (`Checkers/`)

| Class                                   | Purpose                                 | Validates/Checks               |
| --------------------------------------- | --------------------------------------- | ------------------------------ |
| **DamageApplicationHandler.cs**         | Damage application with OHKO prevention | Damage, Focus Sash, Sturdy     |
| **StatusApplicationHandler.cs**         | Status application validation           | Immunities, existing status    |
| **StatChangeApplicationHandler.cs**     | Stat change application                 | Stage limits, Contrary         |
| **HealingApplicationHandler.cs**        | Healing application                     | Healing limits, restrictions   |
| **SwitchApplicationHandler.cs**         | Switch application                      | Switch restrictions, cooldowns |
| **FieldConditionApplicationHandler.cs** | Field condition application             | Duration, interactions         |
| **MoveStateHandler.cs**                 | Move state tracking                     | Charging, multi-turn moves     |
| **MoveExecutionHandler.cs**             | Move execution validation               | PP, status, conditions         |
| **MoveAccuracyHandler.cs**              | Move accuracy checking                  | Accuracy modifiers, evasion    |
| **ProtectionHandler.cs**                | Protection effects                      | Success chance, stacking       |
| **SemiInvulnerableHandler.cs**          | Semi-invulnerable states                | Fly, Dig states, counters      |
| **OHKOPreventionHandler.cs**            | OHKO prevention                         | Focus Sash, Sturdy             |
| **StatChangeReversalHandler.cs**        | Stat change reversal                    | Contrary ability               |
| **FocusPunchHandler.cs**                | Focus Punch behavior                    | Charging, interruption         |
| **MultiTurnHandler.cs**                 | Multi-turn moves                        | Outrage, Petal Dance states    |

## Handler Registry

### CombatEffectHandlerRegistry.cs

Central registry for all handlers. Provides:

-   Handler registration by type and trigger
-   Handler lookup by ID or trigger
-   Handler processing methods
-   Default handler initialization

**Key Methods:**

-   `RegisterAbilityHandler()`: Register ability handler
-   `RegisterItemHandler()`: Register item handler
-   `RegisterMoveEffectHandler()`: Register move effect handler
-   `ProcessAbility()`: Process ability with trigger
-   `ProcessItem()`: Process item with trigger
-   `ProcessMoveEffect()`: Process move effect
-   `CreateDefault()`: Create registry with all default handlers

## Handler Interfaces

### Definition/

#### IAbilityEffectHandler.cs

Interface for ability effect handlers:

```csharp
AbilityTrigger Trigger { get; }
AbilityEffect Effect { get; }
List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field);
```

#### IItemEffectHandler.cs

Interface for item effect handlers:

```csharp
ItemTrigger Trigger { get; }
bool CanHandle(ItemData item);
List<BattleAction> Process(ItemData item, BattleSlot slot, BattleField field);
```

#### IMoveEffectHandler.cs

Interface for move effect handlers:

```csharp
Type EffectType { get; }
List<BattleAction> Process(IMoveEffect effect, BattleSlot user, BattleSlot target, MoveData move, BattleField field, int damageDealt);
```

#### IContactAbilityHandler.cs

Interface for contact-based ability handlers:

```csharp
List<BattleAction> ProcessWithAttacker(AbilityData ability, BattleSlot slot, BattleField field, BattleSlot attacker);
```

#### IContactItemHandler.cs

Interface for contact-based item handlers:

```csharp
List<BattleAction> ProcessWithAttacker(ItemData item, BattleSlot slot, BattleField field, BattleSlot attacker);
```

#### IApplicationHandler.cs

Interface for application handlers (damage, status, etc.):

```csharp
bool CanApply(...);
void Apply(...);
```

#### ICheckerHandler.cs

Interface for checker handlers (validation, checks):

```csharp
bool Check(...);
ValidationResult Validate(...);
```

## How to Add New Handlers

### Adding a New Ability Handler

**Step 1: Create the Handler Class**

Create a new file in `Handlers/Abilities/` (e.g., `AdaptabilityHandler.cs`):

```csharp
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Abilities
{
    public class AdaptabilityHandler : IAbilityEffectHandler
    {
        public AbilityTrigger Trigger => AbilityTrigger.OnMoveUse;
        public AbilityEffect Effect => AbilityEffect.ModifyDamage;

        public List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            // Your logic here
            return actions;
        }
    }
}
```

**Step 2: Register the Handler**

Open `Handlers/Registry/CombatEffectHandlerRegistry.cs` and add to the `Initialize()` method:

```csharp
public void Initialize(...)
{
    // ... existing registrations ...

    // Register new ability handler
    RegisterAbilityHandler(new AdaptabilityHandler());

    // If handler needs ID-based lookup:
    RegisterAbilityHandlerById("adaptability", new AdaptabilityHandler());
}
```

**Step 3: Test the Handler**

Create unit tests to verify the handler works correctly:

```csharp
[Test]
public void AdaptabilityHandler_IncreasesSTABDamage()
{
    // Arrange
    var handler = new AdaptabilityHandler();
    var ability = GetAbilityData("adaptability");
    var slot = CreateTestSlot();
    var field = CreateTestField();

    // Act
    var actions = handler.Process(ability, slot, field);

    // Assert
    // Verify expected behavior
}
```

### Adding a New Item Handler

**Step 1: Create the Handler Class**

Create a new file in `Handlers/Items/` (e.g., `ChoiceBandHandler.cs`):

```csharp
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Items
{
    public class ChoiceBandHandler : IItemEffectHandler
    {
        public ItemTrigger Trigger => ItemTrigger.OnMoveUse;

        public bool CanHandle(ItemData item)
        {
            return item?.Id == "choice-band";
        }

        public List<BattleAction> Process(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            // Your logic here
            return actions;
        }
    }
}
```

**Step 2: Register the Handler**

Open `Handlers/Registry/CombatEffectHandlerRegistry.cs` and add to the `Initialize()` method:

```csharp
public void Initialize(...)
{
    // ... existing registrations ...

    // Register new item handler
    var choiceBandHandler = new ChoiceBandHandler();
    RegisterItemHandler(choiceBandHandler);
    RegisterItemHandlerById("choice-band", choiceBandHandler);
}
```

**Step 3: Test the Handler**

Create unit tests similar to ability handler tests.

### Adding a New Move Effect Handler

**Step 1: Create the Handler Class**

Create a new file in `Handlers/Effects/` (e.g., `ConfusionEffectHandler.cs`):

```csharp
using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    public class ConfusionEffectHandler : IMoveEffectHandler
    {
        public Type EffectType => typeof(ConfusionEffect);

        public List<BattleAction> Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt)
        {
            var actions = new List<BattleAction>();

            if (effect is ConfusionEffect confusionEffect)
            {
                // Your logic here
            }

            return actions;
        }
    }
}
```

**Step 2: Register the Handler**

Open `Handlers/Registry/CombatEffectHandlerRegistry.cs` and add to the `Initialize()` method:

```csharp
public void Initialize(...)
{
    // ... existing registrations ...

    // Register new move effect handler
    RegisterMoveEffectHandler(new ConfusionEffectHandler());
}
```

**Step 3: Test the Handler**

Create unit tests to verify the handler processes the effect correctly.

### Adding a New Checker Handler

**Step 1: Create the Handler Class**

Create a new file in `Handlers/Checkers/` (e.g., `TypeImmunityHandler.cs`):

```csharp
namespace PokemonUltimate.Combat.Handlers.Checkers
{
    public class TypeImmunityHandler
    {
        public bool IsImmune(BattleSlot target, Type moveType)
        {
            // Your logic here
            return false;
        }
    }
}
```

**Step 2: Add Getter Method to Registry**

Open `Handlers/Registry/CombatEffectHandlerRegistry.cs` and add a getter method:

```csharp
public TypeImmunityHandler GetTypeImmunityHandler()
{
    return new TypeImmunityHandler();
}
```

**Step 3: Use the Handler**

Use the handler in steps or other handlers that need type immunity checking.

## Handler Trigger Reference

### Ability Triggers

-   `OnSwitchIn`: When Pokemon switches in
-   `OnTurnStart`: At start of turn
-   `OnMoveUse`: When using a move
-   `OnContactReceived`: When receiving contact damage
-   `OnDamageTaken`: When taking damage
-   `OnKO`: When KO'ing an opponent
-   `OnTurnEnd`: At end of turn
-   `None`: No specific trigger (ID-based lookup)

### Item Triggers

-   `OnTurnStart`: At start of turn
-   `OnMoveUse`: When using a move
-   `OnContactReceived`: When receiving contact damage
-   `OnDamageTaken`: When taking damage
-   `OnTurnEnd`: At end of turn
-   `None`: No specific trigger (ID-based lookup)

## Design Principles

1. **Registry Pattern**: Central registry for all handlers
2. **Strategy Pattern**: Different handlers for different effects
3. **Single Responsibility**: Each handler handles one effect
4. **Action Generation**: Handlers generate actions, don't modify state directly
5. **Testability**: Handlers can be tested independently

## Usage Example

```csharp
// Create registry
var registry = CombatEffectHandlerRegistry.CreateDefault();

// Process ability
var actions = registry.ProcessAbility(
    ability: abilityData,
    slot: pokemonSlot,
    field: battleField,
    trigger: AbilityTrigger.OnSwitchIn
);

// Process actions
foreach (var action in actions)
{
    var reactions = action.ExecuteLogic(field);
    await action.ExecuteVisual(view);
}
```

## Related Documentation

-   `../Actions/README.md` - Action system
-   `../Engine/README.md` - Engine overview
-   `../Engine/TurnFlow/Steps/README.md` - Steps that use handlers
-   `Registry/CombatEffectHandlerRegistry.cs` - Registry implementation
