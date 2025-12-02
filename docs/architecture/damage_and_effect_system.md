# Damage & Effect System Specification

## 1. Overview
Pokemon damage calculation is complex (Stats, Types, Crits, Items, Abilities, Weather).
To keep this **Robust** and **Testable**, we avoid a single giant function. Instead, we use a **Pipeline Pattern**.

## 2. Damage Calculation Pipeline
*Namespace: `PokemonUltimate.Combat.Damage`*

### `DamageContext`
A snapshot of the attack event. Mutable as it passes through the pipeline.

```csharp
public class DamageContext {
    // Inputs (Immutable)
    public BattleSlot Attacker { get; }
    public BattleSlot Defender { get; }
    public MoveData Move { get; }
    public BattleField Field { get; }

    // Calculation State (Mutable)
    public float RawDamage { get; set; }
    public float Multiplier { get; set; } = 1.0f;
    public bool IsCritical { get; set; }
    public float TypeEffectiveness { get; set; } = 1.0f;
    
    // Result
    public int FinalDamage => Mathf.FloorToInt(RawDamage * Multiplier);
}
```

### `IDamageStep` (Middleware)
A single step in the formula.

```csharp
public interface IDamageStep {
    void Process(DamageContext ctx);
}
```

### `DamagePipeline` (The Calculator)
Executes the steps in order.

```csharp
public class DamagePipeline {
    private List<IDamageStep> _steps;

    public DamagePipeline() {
        _steps = new List<IDamageStep> {
            new BaseDamageStep(),        // 1. Calculate base damage (includes stat modifiers, handles FixedDamageEffect)
            new CriticalHitStep(),       // 2. Check for critical hit (1.5x)
            new RandomFactorStep(),      // 3. Apply random factor (0.85-1.0)
            new StabStep(),              // 4. Apply STAB bonus (1.5x)
            new AttackerAbilityStep(),   // 5. Apply ability damage multipliers (Blaze, etc.)
            new AttackerItemStep(),      // 6. Apply item damage multipliers (Life Orb, etc.)
            new TypeEffectivenessStep(), // 7. Apply type effectiveness
            new BurnStep(),              // 8. Apply burn penalty (0.5x for physical)
        };
    }

    public DamageContext Calculate(BattleSlot attacker, BattleSlot defender, MoveData move, BattleField field) {
        var ctx = new DamageContext(attacker, defender, move, field);
        foreach (var step in _steps) {
            step.Process(ctx);
        }
        return ctx;
    }
}
```

## 3. Type Effectiveness System
*Namespace: `PokemonGame.Core.Data`*

We need a robust way to query "Fire vs Water".

```csharp
public static class TypeChart {
    // 2D Array or Dictionary lookup
    public static float GetEffectiveness(PokemonType attackType, PokemonType defenderType) {
        // Returns 0.0, 0.5, 1.0, or 2.0
    }

    public static float GetCombinedEffectiveness(PokemonType attackType, List<PokemonType> defenderTypes) {
        float multiplier = 1.0f;
        foreach (var type in defenderTypes) {
            multiplier *= GetEffectiveness(attackType, type);
        }
        return multiplier;
    }
}
```

## 4. Effect Application (Status & Volatile)
*Namespace: `PokemonUltimate.Combat.Effects`*

Applying a status isn't just `p.Status = Burn`. We need to check immunities.

### `StatusApplicationService`
Logic to verify if a status can be applied.

```csharp
public static bool CanApplyStatus(PokemonInstance target, PersistentStatus status) {
    // 1. Already has status?
    if (target.Status != PersistentStatus.None) return false;

    // 2. Type Immunities
    if (status == PersistentStatus.Burn && target.HasType(PokemonType.Fire)) return false;
    if (status == PersistentStatus.Poison && target.HasType(PokemonType.Poison)) return false;
    if (status == PersistentStatus.Poison && target.HasType(PokemonType.Steel)) return false;

    // 3. Ability Immunities (e.g. Immunity, Limber)
    // if (target.Ability.Prevents(status)) return false;

    return true;
}
```

### Integration with Action Queue
The `StatusRollAction` uses this service.

```csharp
public class ApplyStatusAction : BattleAction {
    // ...
    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        if (StatusApplicationService.CanApplyStatus(_target.Pokemon, _status)) {
            _target.Pokemon.Status = _status;
            return new List<BattleAction> { new MessageAction($"{_target.Pokemon.Name} was burned!") };
        } else {
            return new List<BattleAction> { new MessageAction("It doesn't affect " + _target.Pokemon.Name) };
        }
    }
}
```

## 5. Modularity & Extensibility
How do we handle **Items** (Choice Band) or **Abilities** (Blaze)?

We use an **Observer/Event** system within the Pipeline steps.

```csharp
public class StatRatioStep : IDamageStep {
    public void Process(DamageContext ctx) {
        float atk = ctx.Attacker.Pokemon.Stats[Stat.Attack];
        
        // Apply Stat Stages (see status_and_stat_system.md)
        int stage = ctx.Attacker.Pokemon.StatStages[Stat.Attack];
        atk *= StatStageCalculator.GetMultiplier(stage);
        
        // Apply Burn penalty (if physical move)
        if (ctx.Move.Category == MoveCategory.Physical &&
            ctx.Attacker.Pokemon.Status == PersistentStatus.Burn) {
            atk *= 0.5f;
        }
        
        // Event Hook: "OnGetStats"
        // Items/Abilities subscribe to this to modify 'atk'
        atk = StatModifierEvents.Invoke(ctx.Attacker, Stat.Attack, atk);
        
        // ... calculation
    }
}
```

## 6. Testability
This system is extremely testable.

```csharp
[Test]
public void Test_Damage_Pipeline_Fire_Vs_Water() {
    // Setup
    var pipeline = new DamagePipeline();
    var charizard = CreateDummy("charizard", Type.Fire);
    var blastoise = CreateDummy("blastoise", Type.Water);
    var ember = new MoveData { Type = Type.Fire, Power = 40 };

    // Execute
    var result = pipeline.Calculate(charizard, blastoise, ember, new BattleField());

    // Assert
    Assert.AreEqual(0.5f, result.TypeEffectiveness);
    Assert.IsTrue(result.FinalDamage > 0);
}
```
