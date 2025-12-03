# Sub-Feature 2.3: Turn Order Resolution - Architecture

> Technical specification of Priority, Speed, and Random sorting.

**Sub-Feature Number**: 2.3  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 1. Overview
The **Turn Order System** determines **who acts first** each turn. It's responsible for sorting the `BattleQueue` before execution.

This system must handle:
- Base Speed comparison
- Priority moves (Quick Attack, Extremespeed)
- Speed modifiers (Tailwind, Paralysis, Choice Scarf)
- Speed ties (random)
- Field effects (Trick Room)

## 2. Core Service: `TurnOrderResolver`
*Namespace: `PokemonUltimate.Combat.TurnOrder`*

```csharp
public static class TurnOrderResolver {
    
    public static List<BattleAction> SortActions(List<BattleAction> actions, BattleField field) {
        return actions
            .OrderByDescending(a => GetPriority(a))      // 1. Priority first
            .ThenByDescending(a => GetEffectiveSpeed(a, field)) // 2. Then Speed
            .ThenBy(a => Random.Range(0f, 1f))           // 3. Ties: Random
            .ToList();
    }
    
    private static int GetPriority(BattleAction action) {
        if (action is UseMoveAction moveAction) {
            return moveAction.Move.Priority; // +2, +1, 0, -1...
        }
        return 0; // Default for non-move actions (Switch, Item)
    }
    
    private static float GetEffectiveSpeed(BattleAction action, BattleField field) {
        var slot = action.User; // Assuming all Actions expose their user
        float speed = slot.Pokemon.Stats[Stat.Speed];
        
        // Apply modifiers
        speed *= GetStatStageMultiplier(slot.Pokemon.StatStages[Stat.Speed]);
        speed *= GetStatusMultiplier(slot.Pokemon.Status);
        speed *= GetItemMultiplier(slot.Pokemon.Item);
        speed *= GetAbilityMultiplier(slot.Pokemon.Ability, field);
        speed *= GetFieldMultiplier(field);
        
        return speed;
    }
}
```

## 3. Speed Modifiers

### Stat Stages
```csharp
private static float GetStatStageMultiplier(int stages) {
    // Stages: -6 to +6
    // Formula: (2 + max(0, stages)) / (2 + max(0, -stages))
    if (stages >= 0) return (2 + stages) / 2f;
    else return 2f / (2 - stages);
    
    // +1 = 1.5x, +2 = 2.0x, -1 = 0.66x, -2 = 0.5x
}
```

### Status Conditions
```csharp
private static float GetStatusMultiplier(PersistentStatus status) {
    if (status == PersistentStatus.Paralysis) return 0.5f;
    return 1.0f;
}
```

### Items
```csharp
private static float GetItemMultiplier(IBattleListener item) {
    if (item is ChoiceScarfItem) return 1.5f;
    if (item is IronBallItem) return 0.5f;
    return 1.0f;
}
```

### Field Effects (Trick Room)
```csharp
private static float GetFieldMultiplier(BattleField field) {
    if (field.HasEffect(FieldEffect.TrickRoom)) return -1f; // Reverse!
    return 1.0f;
}
```

## 4. Integration with Combat Engine

The `CombatEngine` collects actions, sorts them, then enqueues.

```csharp
public async Task RunTurn() {
    // Phase 1: Collect Actions from all participants
    var pendingActions = new List<BattleAction>();
    
    foreach (var slot in _field.GetAllActiveSlots()) {
        var action = await slot.ActionProvider.GetAction(_field, slot);
        pendingActions.Add(action);
    }
    
    // Phase 2: Sort by Turn Order
    var sortedActions = TurnOrderResolver.SortActions(pendingActions, _field);
    
    // Phase 3: Enqueue in order
    foreach (var action in sortedActions) {
        _queue.Enqueue(action);
    }
    
    // Phase 4: Process the queue
    await _queue.ProcessQueue(_field, _view);
}
```

## 5. Special Cases

### Switching Always Goes First
Switching has implicit Priority = +6 (higher than any move).

```csharp
private static int GetPriority(BattleAction action) {
    if (action is SwitchAction) return 6;
    if (action is UseMoveAction moveAction) return moveAction.Move.Priority;
    return 0;
}
```

### Future Sight / Delayed Moves
These don't participate in turn order. They're queued at a specific trigger.

```csharp
// During UseMoveAction for Future Sight
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
    // Schedule damage for 2 turns later
    field.ScheduleDelayedAction(new FutureSightDamageAction(_target), turnsDelay: 2);
    
    yield return new MessageAction($"{_user.Name} foresaw an attack!");
}
```

## 6. Testability

```csharp
[Test]
public void Test_TurnOrder_PriorityBeatsSpeed() {
    // Setup
    var fast = CreateAction(priority: 0, speed: 100);
    var slow = CreateAction(priority: 1, speed: 50); // Slower, but Priority +1
    
    // Execute
    var sorted = TurnOrderResolver.SortActions(new[] { fast, slow }, field);
    
    // Assert
    Assert.AreEqual(slow, sorted[0]); // Priority wins
}

[Test]
public void Test_TurnOrder_ParalysisHalvesSpeed() {
    var paralyzed = CreateSlot(speed: 100, status: Paralysis);
    var normal = CreateSlot(speed: 60);
    
    var eff1 = GetEffectiveSpeed(paralyzed); // 100 * 0.5 = 50
    var eff2 = GetEffectiveSpeed(normal);    // 60
    
    Assert.IsTrue(eff2 > eff1);
}
```

---

## Related Documents

- **[Sub-Feature README](README.md)** - Overview of Turn Order Resolution
- **[Parent Architecture](../../architecture.md)** - Overall combat system design
- **[Use Cases](../../use_cases.md)** - Turn order scenarios
- **[Roadmap](../../roadmap.md#phase-23-turn-order-resolution)** - Implementation details
- **[Testing](../../testing.md)** - Testing strategy
- **[Code Location](../../code_location.md)** - Where this is implemented
- **[Sub-Feature 2.2: Action Queue](../2.2-action-queue-system/)** - Actions are sorted by turn order
- **[Sub-Feature 2.6: Combat Engine](../2.6-combat-engine/)** - Engine uses turn order resolver

---

**Last Updated**: 2025-01-XX