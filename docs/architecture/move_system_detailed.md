# Move System Specification

## 1. Overview
Moves are the primary interaction in combat. To support thousands of moves without creating thousands of classes, we use a **Composition Pattern**.
A Move is defined by its **Data** and a list of **Effects**. It is **NOT** a unique class (e.g., no `class Ember : Move`).

## 2. Static Data (The "Blueprint")
*Namespace: `PokemonGame.Core.Data`*

```csharp
public class MoveData {
    // Identity
    public string Id { get; set; }          // "ember"
    public string Name { get; set; }        // "Ember"
    public string Description { get; set; }
    
    // Core Rules
    public PokemonType Type { get; set; }
    public MoveCategory Category { get; set; } // Physical, Special, Status
    public int Power { get; set; }
    public int Accuracy { get; set; }
    public int MaxPP { get; set; }
    public int Priority { get; set; }
    public int Priority { get; set; }
    public TargetScope TargetScope { get; set; } // Self, SingleEnemy, AllEnemies...

    // Logic Composition (The "What it does")
    public List<IMoveEffect> Effects { get; set; } = new List<IMoveEffect>();

    // Visuals (Separated)
    public string VisualId { get; set; } // Points to a MoveVisualData asset
}
```

## 3. Dynamic Data (The "Instance")
*Namespace: `PokemonGame.Core.Models`*

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

## 4. Move Effects (The Building Blocks)
*Namespace: `PokemonGame.Core.Effects`*

Effects are small, reusable logic blocks. They implement a method to generate **Actions**.

```csharp
public interface IMoveEffect {
    // Returns the Actions this effect causes.
    // Context includes UserSlot, TargetSlot, Field, etc.
    IEnumerable<BattleAction> GenerateActions(CombatContext ctx);
}

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
5.  **`RecoilEffect`**: User takes % of damage dealt.
    *   *Generates*: `DamageAction` targeting the user.

> [!NOTE]
> For a complete list of all possible effects and actions, see `battle_mechanics_catalog.md`.

## 5. Execution Flow: Generating Actions
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

## 6. Unity Integration (ScriptableObjects)
*Namespace: `PokemonGame.Unity.Data`*

We use **SerializeReference** (Odin Inspector or standard Unity 2021+) to show the list of Effects polymorphically in the Inspector.

### `MoveSO`
-   `Name`, `Power`, `Accuracy`...
-   `[SerializeReference] List<IMoveEffect> Effects;`
    -   In Inspector, you click "+" and choose "DamageEffect", "StatusEffect", etc.

## 7. Testability (The "Ember" Test)
We can test a move without running the game.

```csharp
[Test]
public void Test_Ember_Generates_Damage_And_Burn() {
    // 1. Setup
    var ember = new MoveData {
        Name = "Ember",
        Power = 40,
        Effects = { new DamageEffect(), new StatusEffect(Status.Burn, 0.1f) }
    };
    
    // Create Dummy Slots for testing
    var field = new BattleField(); 
    // (Assuming BattleField has a helper to create dummy slots or we mock them)
    var userSlot = new BattleSlot(0, new BattleSide(1, true));
    userSlot.Pokemon = new PokemonInstance(charmanderData, 5);
    
    var targetSlot = new BattleSlot(0, new BattleSide(1, false));
    targetSlot.Pokemon = new PokemonInstance(bulbasaurData, 5);

    // 2. Execute
    var context = new CombatContext(userSlot, targetSlot, field);
    var actions = new List<BattleAction>();
    foreach(var eff in ember.Effects) actions.AddRange(eff.GenerateActions(context));

    // 3. Assert
    Assert.IsTrue(actions.Any(a => a is DamageAction));
    Assert.IsTrue(actions.Any(a => a is StatusRollAction));
}
```
