# Combat System Specification (Action Queue Architecture)

## 1. Core Philosophy: "Everything is an Action"
Instead of complex methods calling each other, the entire battle is a linear sequence of **Actions** processed by a **Queue**.
Logic and View are unified in these Actions, but executed in phases.

## 2. The Action Queue System

### `BattleAction` (Abstract Class)
The base unit of work.
```csharp
public abstract class BattleAction {
    // Phase 1: Logic (Instant)
    // Updates the data model (HP, Status, etc.)
    // Returns new Actions triggered by this one (e.g., Damage -> Faint)
    public abstract IEnumerable<BattleAction> ExecuteLogic(BattleField field);

    // Phase 2: Visual (Async)
    // Shows the animation, text, or UI update.
    public abstract Task ExecuteVisual(IBattleView view);
}
```

### `BattleQueue` (Controller)
Manages the flow.
```csharp
public class BattleQueue {
    private Queue<BattleAction> _queue = new Queue<BattleAction>();

    public async Task ProcessQueue(BattleField field, IBattleView view) {
        int safetyCounter = 0;
        while (_queue.Count > 0) {
            if (safetyCounter++ > 1000) throw new Exception("Infinite Loop in Battle Queue!");
            
            var action = _queue.Dequeue();
            
            // 1. Run Logic
            var reactions = action.ExecuteLogic(field);
            
            // 2. Run Visual (Wait for it)
            // This handles Animations, Text, and Audio via IBattleView
            await action.ExecuteVisual(view);
            
            // 3. Enqueue reactions (Priority: Immediate)
            InsertAtFront(reactions); 
        }
    }
}
```

> [!NOTE]
> Actions can be purely visual/audio (e.g., `PlaySoundAction`, `MessageAction`). These actions return no logic reactions but are critical for player feedback. See `ui_presentation_system.md`.

### Event Triggers (Abilities & Items)
After processing actions, the engine fires event triggers for Abilities and Items.

```csharp
// Example: At end of turn
await ProcessTriggers(BattleTrigger.OnTurnEnd);

private async Task ProcessTriggers(BattleTrigger trigger) {
    foreach (var slot in field.GetAllActiveSlots()) {
        if (slot.Pokemon.Item != null) {
            var actions = slot.Pokemon.Item.OnTrigger(trigger, slot, field);
            foreach (var action in actions) _queue.Enqueue(action);
        }
        if (slot.Pokemon.Ability != null) {
            var actions = slot.Pokemon.Ability.OnTrigger(trigger, slot, field);
            foreach (var action in actions) _queue.Enqueue(action);
        }
    }
    await _queue.ProcessQueue(field, view);
}
```

## 3. Battle Configuration (Modes: 1v1, 2v2, 1v3)
To support any battle type, we pass a `BattleRules` object when starting combat.

```csharp
public class BattleRules {
    public int PlayerSideSlots { get; set; } = 1; // Standard: 1
    public int EnemySideSlots { get; set; } = 1;  // Standard: 1
    // 1v3 Horde: Player=1, Enemy=3
    // 2v2 Double: Player=2, Enemy=2
    // Boss: Player=1, Enemy=1 (but Enemy has Boss Stats)
}

public void InitializeBattle(BattleRules rules, Party playerParty, Party enemyParty) {
    // Setup slots dynamically based on rules
    _field.SetupSides(rules.PlayerSideSlots, rules.EnemySideSlots);
    // Spawn pokemon into slots...
}
```

### Turn Flow with Turn Order
```csharp
### Battle Loop (Victory Check)
The engine runs turns in a loop until a Victory/Defeat condition is met.

```csharp
public async Task RunBattle() {
    while (true) {
        await RunTurn();
        
        // Check Outcome via Arbiter (See victory_defeat_system.md)
        var outcome = BattleArbiter.CheckOutcome(_field);
        if (outcome != BattleOutcome.Ongoing) {
            await EndBattle(outcome);
            return;
        }
    }
}

public async Task RunTurn() {
    // 1. Collect Actions from all participants
    var pendingActions = new List<BattleAction>();
    foreach (var slot in _field.GetAllActiveSlots()) {
        var action = await slot.ActionProvider.GetAction(_field, slot);
        pendingActions.Add(action);
    }
    
    // 2. Sort by Turn Order (Priority, Speed)
    var sortedActions = TurnOrderResolver.SortActions(pendingActions, _field);
    
    // 3. Enqueue in sorted order
    foreach (var action in sortedActions) {
        _queue.Enqueue(action);
    }
    
    // 4. Process the queue
    await _queue.ProcessQueue(_field, _view);
    
    // 5. End of turn triggers (Status damage, Leftovers)
    await ProcessTriggers(BattleTrigger.OnTurnEnd);
}
```

## 4. Examples of Actions

### `DamageAction` (Core Logic)
```csharp
public class DamageAction : BattleAction {
    private BattleSlot _target;
    private int _amount;

    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        _target.Pokemon.CurrentHP -= _amount;
        
        // Check for Faint reaction
        if (_target.Pokemon.CurrentHP <= 0) {
            return new List<BattleAction> { new FaintAction(_target) };
        }
        return Enumerable.Empty<BattleAction>();
    }

    public override async Task ExecuteVisual(IBattleView view) {
        await view.PlayDamageAnimation(_target);
        await view.UpdateHPBar(_target); // Smooth drain
    }
}
```

### `UseMoveAction` (The Trigger & Conditional Logic)
```csharp
public class UseMoveAction : BattleAction {
    private BattleSlot _user;
    private MoveData _move;
    private BattleSlot _target;

    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        // 1. Check Volatile Status (Flinch, Confusion, Sleep)
        if (_user.Pokemon.VolatileStatus.HasFlag(VolatileStatus.Flinch)) {
             _user.Pokemon.RemoveVolatile(VolatileStatus.Flinch); // Consume it
             return new List<BattleAction> { new MessageAction($"{_user.Pokemon.Name} flinched!") };
        }

        // 2. If we passed checks, generate the move actions
        var actions = new List<BattleAction>();
        actions.Add(new MessageAction($"{_user.Pokemon.Name} used {_move.Name}!"));
        actions.Add(new MoveAnimationAction(_user, _target, _move.VisualId));
        
        int damage = DamageCalculator.Calculate(_user, _target, _move);
        actions.Add(new DamageAction(_target, damage));
        
        if (_move.HasStatusEffect) {
             actions.Add(new StatusRollAction(_target, _move.StatusEffect, _move.StatusChance));
        }
        
        return actions; 
    }

    public override Task ExecuteVisual(IBattleView view) {
        return Task.CompletedTask; 
    }
}
```

## 5. Trace Example: Flinch Interaction (Retroceso)
Scenario: Pokemon A uses "Headbutt" (Causes Flinch). Pokemon B is slower.

1.  **`UseMoveAction(A)`** executes.
    *   Spawns `DamageAction` + `ApplyVolatileAction(B, Flinch)`.
2.  **`DamageAction`** executes. B takes damage.
3.  **`ApplyVolatileAction`** executes.
    *   *Logic*: Sets `B.VolatileStatus |= Flinch`.
    *   *Visual*: None (or small icon).
4.  **`UseMoveAction(B)`** (which was already in the queue) finally executes.
    *   *Logic*: Checks `if (B.HasFlag(Flinch))`. **TRUE**.
    *   *Result*: Returns ONLY `[MessageAction("B flinched!")]`.
    *   *Note*: The Damage/Animation for B's move are NEVER created. The turn ends.

## 6. Verification against Pillars

### Is it Simple?
*   **Yes.** The flow is linear. No complex state machines jumping around. Just a list of things to do.
*   **Safety**: Added a loop counter to prevent infinite reaction chains.

### Is it Readable?
*   **Yes.** `UseMoveAction` clearly lists what happens: Message -> Animation -> Damage. Anyone can read it and understand the move's sequence.

### Is it Modular?
*   **Yes.**
    *   Want to add **Weather**? Create `WeatherDamageAction`.
    *   Want to add **Dialogue** mid-fight? Create `DialogueAction`.
    *   Want to add **Tutorial Popups**? Create `TutorialAction`.
    *   None of these require changing the `CombatEngine` code.

### Is it Testable?
*   **Yes.**
    *   **Logic Tests**: Enqueue `DamageAction`, call `ExecuteLogic`, assert HP.
    *   **Visual Tests**: Mock `IBattleView`, run queue, assert that `PlayAnimation` was called.
    *   **Scenario Tests**: Setup a 1v3 battle (via `BattleRules`), enqueue an Area of Effect move, verify 3 enemies took damage.
