# Damage Calculation Pipeline

The damage calculation system uses a **pipeline pattern** where damage is calculated through a series of independent steps. Each step modifies the damage context, making the system modular, extensible, and testable.

## Architecture

The `DamagePipeline` executes a series of `IDamageStep` implementations in order. Each step receives a `DamageContext` and modifies it to calculate the final damage.

## Complete Step List (Execution Order)

| #   | Step                         | Purpose                                                 | Modifies                         |
| --- | ---------------------------- | ------------------------------------------------------- | -------------------------------- |
| 1   | **BaseDamageStep.cs**        | Calculates base damage from formula                     | BaseDamage                       |
| 2   | **CriticalHitStep.cs**       | Checks critical hit, applies 1.5x multiplier            | CurrentDamage, IsCritical        |
| 3   | **RandomFactorStep.cs**      | Applies random factor (0.85-1.0)                        | CurrentDamage                    |
| 4   | **StabStep.cs**              | Applies STAB bonus (1.5x if type matches)               | CurrentDamage                    |
| 5   | **AttackerAbilityStep.cs**   | Applies ability multipliers (Blaze, Adaptability, etc.) | CurrentDamage                    |
| 6   | **AttackerItemStep.cs**      | Applies item multipliers (Life Orb, Choice Band, etc.)  | CurrentDamage                    |
| 7   | **WeatherStep.cs**           | Applies weather multipliers (Sun/Rain, etc.)            | CurrentDamage                    |
| 8   | **TerrainStep.cs**           | Applies terrain multipliers (Electric/Grassy, etc.)     | CurrentDamage                    |
| 9   | **ScreenStep.cs**            | Applies screen reduction (Reflect/Light Screen, etc.)   | CurrentDamage                    |
| 10  | **TypeEffectivenessStep.cs** | Applies type effectiveness (0x-4x)                      | CurrentDamage, TypeEffectiveness |
| 11  | **BurnStep.cs**              | Applies burn penalty (0.5x for physical)                | CurrentDamage                    |

## Damage Context

The `DamageContext` flows through all steps and contains:

-   **Attacker**: The attacking Pokemon slot
-   **Defender**: The defending Pokemon slot
-   **Move**: The move being used
-   **Field**: The battlefield reference
-   **BaseDamage**: Calculated base damage
-   **CurrentDamage**: Current damage after all steps
-   **Multipliers**: Applied multipliers (for debugging)
-   **IsCritical**: Whether critical hit occurred
-   **TypeEffectiveness**: Type effectiveness multiplier

## Pipeline Interface

### IDamagePipeline.cs

Interface for damage pipeline implementations:

```csharp
DamageContext Calculate(
    BattleSlot attacker,
    BattleSlot defender,
    MoveData move,
    BattleField field,
    bool forceCritical = false,
    float? fixedRandomValue = null);
```

### IDamageStep.cs

Interface for damage calculation steps:

```csharp
void Process(DamageContext context);
```

## How to Add a New Damage Step

### Step 1: Create the Step Class

Create a new file in `Damage/Steps/` (e.g., `AbilityPowerStep.cs`):

```csharp
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Field;

namespace PokemonUltimate.Combat.Damage.Steps
{
    public class AbilityPowerStep : IDamageStep
    {
        public void Process(DamageContext context)
        {
            // Your step logic here
            // Read from context
            // Modify context.CurrentDamage
            // Add to context.Multipliers for debugging

            // Example: Apply ability power modifier
            if (context.Attacker.Pokemon?.Ability?.Id == "huge-power")
            {
                context.CurrentDamage = (int)(context.CurrentDamage * 2.0f);
                context.Multipliers.Add("Huge Power", 2.0f);
            }
        }
    }
}
```

### Step 2: Add Step to DamagePipeline

Open `Damage/DamagePipeline.cs` and add your step to the step list in the constructor:

```csharp
public DamagePipeline(IRandomProvider randomProvider = null)
{
    var random = randomProvider ?? new RandomProvider();

    _steps = new List<IDamageStep>
    {
        new BaseDamageStep(),
        new CriticalHitStep(random),
        new RandomFactorStep(random),
        new StabStep(),
        new AttackerAbilityStep(),
        new AttackerItemStep(),
        new WeatherStep(),
        new TerrainStep(),
        new ScreenStep(),
        new TypeEffectivenessStep(),
        new BurnStep(),

        // Add your step in the appropriate position
        new AbilityPowerStep(), // Example: Add after AttackerAbilityStep

        // Or insert at specific position:
        // InsertStep(5, new AbilityPowerStep());
    };
}
```

**Important**: Place your step in the correct position based on when it should modify damage. Consider:

-   **Before type effectiveness**: Modifiers that affect base damage
-   **After type effectiveness**: Modifiers that affect final damage
-   **Order matters**: Later steps multiply/divide already modified damage

### Step 3: Test Your Step

Create unit tests for your step:

```csharp
[Test]
public void AbilityPowerStep_DoublesDamage()
{
    // Arrange
    var step = new AbilityPowerStep();
    var context = CreateTestDamageContext();
    context.Attacker.Pokemon.Ability = GetAbility("huge-power");
    context.CurrentDamage = 50;

    // Act
    step.Process(context);

    // Assert
    Assert.AreEqual(100, context.CurrentDamage);
    Assert.Contains("Huge Power", context.Multipliers.Keys);
}
```

### Step 4: Document Your Step

Update this README to document your new step:

-   Add to the step list
-   Document purpose and behavior
-   Note any dependencies or special considerations

## Step Dependencies

Steps are designed to be independent, but some have dependencies:

-   **Random Steps**: Require `IRandomProvider` (CriticalHitStep, RandomFactorStep)
-   **Ability Steps**: Require handler registry access (AttackerAbilityStep)
-   **Item Steps**: Require handler registry access (AttackerItemStep)

## Common Step Patterns

### Applying a Multiplier

```csharp
public void Process(DamageContext context)
{
    float multiplier = 1.5f; // Your multiplier
    context.CurrentDamage = (int)(context.CurrentDamage * multiplier);
    context.Multipliers.Add("Your Modifier", multiplier);
}
```

### Conditional Modification

```csharp
public void Process(DamageContext context)
{
    if (ShouldApplyModifier(context))
    {
        float multiplier = CalculateMultiplier(context);
        context.CurrentDamage = (int)(context.CurrentDamage * multiplier);
        context.Multipliers.Add("Conditional Modifier", multiplier);
    }
}
```

### Reading from Context

```csharp
public void Process(DamageContext context)
{
    // Read current damage
    int currentDamage = context.CurrentDamage;

    // Read attacker/defender
    var attacker = context.Attacker;
    var defender = context.Defender;

    // Read move
    var move = context.Move;

    // Read field
    var field = context.Field;

    // Read multipliers (for debugging)
    var multipliers = context.Multipliers;
}
```

## Design Principles

1. **Pipeline Pattern**: Sequential step execution
2. **Single Responsibility**: Each step handles one aspect
3. **Modularity**: Steps are independent and testable
4. **Extensibility**: New steps can be added without modifying existing ones
5. **Testability**: Each step can be tested with mock contexts
6. **Order Matters**: Step execution order affects final damage

## Usage Example

```csharp
// Create pipeline
var pipeline = new DamagePipeline(randomProvider);

// Calculate damage
var context = pipeline.Calculate(
    attacker: attackerSlot,
    defender: defenderSlot,
    move: moveData,
    field: battleField
);

// Use calculated damage
var damage = context.FinalDamage;
```

## Testing

Each step can be tested independently:

-   Create a `DamageContext` with known values
-   Execute the step
-   Verify context modifications
-   Test edge cases and special conditions

## Related Documentation

-   `../Engine/README.md` - Engine overview
-   `../Engine/TurnFlow/Steps/MoveDamageCalculationStep.cs` - Step that uses pipeline
-   `DamageContext.cs` - Damage context structure
-   `DamagePipeline.cs` - Pipeline implementation
