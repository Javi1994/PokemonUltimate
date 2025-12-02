# Move System Specification

## 1. Overview
Moves are the primary interaction in combat. To support thousands of moves without creating thousands of classes, we use a **Composition Pattern**.
A Move is defined by its **Data** and a list of **Effects**. It is **NOT** a unique class (e.g., no `class Ember : Move`).

## 2. Static Data (The "Blueprint")
*Namespace: `PokemonUltimate.Core.Models`*

```csharp
public class MoveData {
    // Identity
    public string Name { get; set; }        // "Ember"
    public string Description { get; set; }
    
    // Core Rules
    public PokemonType Type { get; set; }
    public MoveCategory Category { get; set; } // Physical, Special, Status
    public int Power { get; set; }
    public int Accuracy { get; set; }
    public int MaxPP { get; set; }
    public int Priority { get; set; }
    public TargetScope TargetScope { get; set; } // Self, SingleEnemy, AllEnemies...

    // Logic Composition (The "What it does")
    public List<IMoveEffect> Effects { get; set; } = new List<IMoveEffect>();
}
```

## 3. MoveBuilder API

Use the fluent `MoveBuilder` to define moves in a readable, type-safe way.

### Basic Usage
```csharp
public static readonly MoveData Tackle = Move.Define("Tackle")
    .Description("A physical attack in which the user charges and slams into the target.")
    .Type(PokemonType.Normal)
    .Physical(40, 100, 35)  // power, accuracy, pp
    .WithEffects(e => e.Damage())
    .Build();
```

### Full API Reference

```csharp
Move.Define("Name")
    .Description("...")               // Optional description
    .Type(PokemonType.Fire)           // Element type
    
    // Category (choose one)
    .Physical(power, accuracy, pp)    // Physical attack
    .Special(power, accuracy, pp)     // Special attack
    .Status(accuracy, pp)             // Status move (power = 0)
    
    // Optional modifiers
    .Priority(1)                      // Default 0, range -7 to +5
    .Target(TargetScope.AllEnemies)   // Default SingleEnemy
    .TargetSelf()                     // Shorthand for Self scope
    .TargetAllEnemies()               // Shorthand for AllEnemies scope
    
    // Effects
    .WithEffects(e => e
        .Damage()
        .MayBurn(10))
    
    .Build();
```

### Complex Move Examples

```csharp
// Physical move with priority
public static readonly MoveData QuickAttack = Move.Define("Quick Attack")
    .Type(PokemonType.Normal)
    .Physical(40, 100, 30)
    .Priority(1)
    .WithEffects(e => e.Damage())
    .Build();

// Special move with secondary effect
public static readonly MoveData Thunderbolt = Move.Define("Thunderbolt")
    .Type(PokemonType.Electric)
    .Special(90, 100, 15)
    .WithEffects(e => e
        .Damage()
        .MayParalyze(10))
    .Build();

// Status move with stat changes
public static readonly MoveData SwordsDance = Move.Define("Swords Dance")
    .Type(PokemonType.Normal)
    .Status(0, 20)
    .TargetSelf()
    .WithEffects(e => e.RaiseAttack(2))
    .Build();
```

## 4. EffectBuilder API

The `EffectBuilder` provides a fluent way to compose move effects.

### Damage Effects
```csharp
.Damage()                    // Standard damage
.DamageHighCrit(stages)      // High crit ratio (default +1)
.FixedDamage(40)             // Fixed amount (Dragon Rage)
.LevelDamage()               // Damage = user level (Seismic Toss)
```

### Status Effects
```csharp
.MayBurn(chance)             // Burn (default 100%)
.MayParalyze(chance)         // Paralysis
.MayPoison(chance)           // Poison
.MayBadlyPoison(chance)      // Toxic poison
.MaySleep(chance)            // Sleep
.MayFreeze(chance)           // Freeze
.SelfStatus(status)          // Apply status to self (Rest)
```

### Stat Changes
```csharp
// Raise user's stats
.RaiseAttack(stages, chance)
.RaiseDefense(stages, chance)
.RaiseSpAttack(stages, chance)
.RaiseSpDefense(stages, chance)
.RaiseSpeed(stages, chance)
.RaiseEvasion(stages, chance)

// Lower target's stats
.LowerAttack(stages, chance)
.LowerDefense(stages, chance)
.LowerSpAttack(stages, chance)
.LowerSpDefense(stages, chance)
.LowerSpeed(stages, chance)
.LowerAccuracy(stages, chance)
```

### Recoil, Drain, Heal
```csharp
.Recoil(percent)             // User takes % of damage dealt
.Drain(percent)              // User heals % of damage dealt
.Heal(percent)               // Heal % of max HP
```

### Other Effects
```csharp
.MayFlinch(chance)           // Target may flinch
.MultiHit(min, max)          // Hit 2-5 times (default)
.HitsNTimes(n)               // Hit exactly N times
```

## 5. Dynamic Data (The "Instance")
*Namespace: `PokemonUltimate.Core.Models`*

Represents a move learned by a specific Pokemon.

```csharp
public class MoveInstance {
    public MoveData Data { get; private set; }
    public int CurrentPP { get; set; }
    public int PPUps { get; set; }

    public MoveInstance(MoveData data) {
        Data = data;
        CurrentPP = data.MaxPP;
    }
}
```

## 6. Move Effects (The Building Blocks)
*Namespace: `PokemonUltimate.Core.Effects`*

Effects are small, reusable logic blocks. They are currently **data definitions** that describe what a move does. The execution logic (`GenerateActions`) will be added when the combat system is implemented.

### Current Implementation (Data Definitions)
```csharp
public interface IMoveEffect {
    // Type-safe identifier for this effect
    EffectType EffectType { get; }
    
    // Human-readable description
    string Description { get; }
    
    // TODO: Will be added when combat system is ready
    // IEnumerable<BattleAction> GenerateActions(CombatContext ctx);
}

public enum EffectType {
    Damage, FixedDamage, Status, StatChange, 
    Recoil, Drain, Heal, Flinch, MultiHit
}
```

### Implemented Effect Classes
| Effect | Parameters | Example |
|--------|------------|---------|
| `DamageEffect` | Multiplier, CanCrit, CritStages | Tackle, Razor Leaf |
| `FixedDamageEffect` | Amount, UseLevelAsDamage | Dragon Rage, Seismic Toss |
| `StatusEffect` | Status, ChancePercent, TargetSelf | Thunder Wave, Ember |
| `StatChangeEffect` | Stat, Stages, ChancePercent, TargetSelf | Growl, Swords Dance |
| `RecoilEffect` | RecoilPercent | Double-Edge |
| `DrainEffect` | DrainPercent | Giga Drain |
| `HealEffect` | HealPercent | Recover |
| `FlinchEffect` | ChancePercent | Air Slash |
| `MultiHitEffect` | MinHits, MaxHits | Fury Attack |

### Future Implementation (Execution Logic)
```csharp
public class CombatContext {
    public BattleSlot User { get; private set; }
    public BattleSlot Target { get; private set; }
    public BattleField Field { get; private set; }
    
    public CombatContext(BattleSlot user, BattleSlot target, BattleField field) {
        User = user;
        Target = target;
        Field = field;
    }
}
```

### Common Effects
1.  **`DamageEffect`**: Calculates damage using the standard formula.
    *   *Generates*: `DamageAction`.
2.  **`StatusEffect`**: Applies Burn, Paralysis, Sleep.
    *   *Fields*: `StatusCondition`, `float Chance`.
    *   *Generates*: `ApplyStatusAction` (see status_and_stat_system.md).
3.  **`StatChangeEffect`**: Modifies Attack, Defense, etc.
    *   *Fields*: `Stat`, `int Stages`, `float Chance`, `bool OnSelf`.
    *   *Generates*: `StatChangeAction` (see status_and_stat_system.md).
4.  **`MultiHitEffect`**: Repeats the other effects 2-5 times.
    *   *Generates*: Multiple sequences of Actions.
5.  **`RecoilEffect`**: User takes % of damage dealt. ✅ Implemented
    *   *Fields*: `RecoilPercent` (25%, 33%, 50%)
    *   *Generates*: `DamageAction` targeting the user (based on final damage from pipeline)
    *   *Note*: Always deals at least 1 HP if damage was dealt
6.  **`DrainEffect`**: User heals % of damage dealt. ✅ Implemented
    *   *Fields*: `DrainPercent` (50%, 75%)
    *   *Generates*: `HealAction` for the user (based on final damage from pipeline)
    *   *Note*: Always heals at least 1 HP if damage was dealt, cannot exceed MaxHP

> [!NOTE]
> For a complete list of all possible effects and actions, see `battle_mechanics_catalog.md`.

## 7. Execution Flow: Generating Actions
When `UseMoveAction` executes, it asks the `MoveData` to generate the sub-actions.

```csharp
// Inside UseMoveAction.ExecuteLogic()
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
    var actions = new List<BattleAction>();
    
    // 1. Standard Message & Animation
    actions.Add(new MessageAction($"{_user.Name} used {_move.Name}!"));
    actions.Add(new MoveAnimationAction(_user, _target, _move.VisualId));

    // 2. Accuracy Check
    if (!AccuracyCheck.Roll(_user, _target, _move)) {
        actions.Add(new MessageAction("But it missed!"));
        return actions;
    }

    // 3. Apply Effects
    var context = new CombatContext(_user, _target, field);
    foreach (var effect in _move.Effects) {
        actions.AddRange(effect.GenerateActions(context));
    }

    return actions;
}
```

## 8. Unity Integration (ScriptableObjects)
*Namespace: `PokemonGame.Unity.Data`*

We use **SerializeReference** (Odin Inspector or standard Unity 2021+) to show the list of Effects polymorphically in the Inspector.

### `MoveSO`
-   `Name`, `Power`, `Accuracy`...
-   `[SerializeReference] List<IMoveEffect> Effects;`
    -   In Inspector, you click "+" and choose "DamageEffect", "StatusEffect", etc.

## 9. Testability (The "Ember" Test)
We can test a move without running the game. Current tests verify **composition** (what effects are attached), while future tests will verify **execution** (what actions are generated).

### Current: Composition Tests
```csharp
[Test]
public void Test_Ember_Has_Damage_And_Burn_Effects() {
    var ember = MoveCatalog.Ember;

    Assert.Multiple(() => {
        // Verify effect composition
        Assert.That(ember.HasEffect<DamageEffect>(), Is.True);
        Assert.That(ember.HasEffect<StatusEffect>(), Is.True);
        
        // Verify effect parameters
        var status = ember.GetEffect<StatusEffect>();
        Assert.That(status.EffectType, Is.EqualTo(EffectType.Status));
        Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
        Assert.That(status.ChancePercent, Is.EqualTo(10));
    });
}
```

### Builder Tests
```csharp
[Test]
public void Full_Special_Move_With_Secondary_Effect()
{
    var move = Move.Define("Ice Beam")
        .Type(PokemonType.Ice)
        .Special(90, 100, 10)
        .WithEffects(e => e
            .Damage()
            .MayFreeze(10))
        .Build();

    Assert.Multiple(() =>
    {
        Assert.That(move.Name, Is.EqualTo("Ice Beam"));
        Assert.That(move.Effects, Has.Count.EqualTo(2));
        
        var freezeEffect = move.GetEffect<StatusEffect>();
        Assert.That(freezeEffect.Status, Is.EqualTo(PersistentStatus.Freeze));
        Assert.That(freezeEffect.ChancePercent, Is.EqualTo(10));
    });
}
```

### Future: Execution Tests (When Combat System is Ready)
```csharp
[Test]
public void Test_Ember_Generates_Damage_And_Burn_Actions() {
    var ember = MoveCatalog.Ember;
    var context = new CombatContext(userSlot, targetSlot, field);
    
    var actions = new List<BattleAction>();
    foreach(var eff in ember.Effects) 
        actions.AddRange(eff.GenerateActions(context));

    Assert.IsTrue(actions.Any(a => a is DamageAction));
    Assert.IsTrue(actions.Any(a => a is StatusRollAction));
}
```
