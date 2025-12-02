# Combat Actions Bible

> **Purpose**: Complete technical reference for all combat actions in PokemonUltimate.  
> **Audience**: Developers implementing, testing, or extending the combat system.  
> **Status**: ✅ Phase 2.6 Actions Documented  
**Related Docs**: See `action_use_cases.md` for use case checklist, `coverage_verification.md` for test coverage

---

## Table of Contents

1. [Introduction](#introduction)
2. [Action System Architecture](#action-system-architecture)
3. [Base Action Class](#base-action-class)
4. [Core Actions](#core-actions)
   - [DamageAction](#damageaction)
   - [FaintAction](#faintaction)
   - [HealAction](#healaction)
   - [StatChangeAction](#statchangeaction)
   - [ApplyStatusAction](#applystatusaction)
   - [UseMoveAction](#usemoveaction)
   - [SwitchAction](#switchaction)
   - [MessageAction](#messageaction)
5. [Action Execution Flow](#action-execution-flow)
6. [Action Priority System](#action-priority-system)
7. [Action Blocking](#action-blocking)
8. [Best Practices](#best-practices)
9. [Common Patterns](#common-patterns)
10. [Troubleshooting](#troubleshooting)

---

## Introduction

The combat system in PokemonUltimate follows the **"Everything is an Action"** philosophy. Every event in battle—from using a move to displaying a message—is represented as a `BattleAction` that is processed sequentially by the `BattleQueue`.

### Key Principles

1. **Separation of Logic and Presentation**: Actions have two phases:
   - `ExecuteLogic()`: Updates game state instantly, returns reaction actions
   - `ExecuteVisual()`: Shows animations/messages to the player (async)

2. **Action Queue Pattern**: All actions are queued and processed in order, allowing for predictable execution and easy testing.

3. **Reaction Actions**: Actions can generate other actions (e.g., `DamageAction` → `FaintAction`), creating a chain of events.

4. **Fail-Fast**: Invalid inputs throw exceptions immediately, preventing invalid game states.

---

## Action System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    BattleQueue                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Action 1: UseMoveAction                            │   │
│  │    ├─ ExecuteLogic() → [DamageAction, MessageAction]│   │
│  │    └─ ExecuteVisual() → PlayMoveAnimation()         │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Action 2: DamageAction (from Action 1)             │   │
│  │    ├─ ExecuteLogic() → [FaintAction] (if HP = 0)    │   │
│  │    └─ ExecuteVisual() → PlayDamageAnimation()       │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Action 3: FaintAction (from Action 2)             │   │
│  │    ├─ ExecuteLogic() → []                           │   │
│  │    └─ ExecuteVisual() → PlayFaintAnimation()        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

---

## Base Action Class

### BattleAction

**Location**: `PokemonUltimate.Combat.Actions.BattleAction`

**Purpose**: Abstract base class for all battle actions.

**Key Properties**:
- `User`: The `BattleSlot` that initiated this action (null for system actions)
- `Priority`: Turn order modifier (default: 0)
- `CanBeBlocked`: Whether this action can be blocked by Protect, etc. (default: false)

**Key Methods**:
- `ExecuteLogic(BattleField field)`: Updates game state, returns reaction actions
- `ExecuteVisual(IBattleView view)`: Shows visual feedback to player

**Design Decisions**:
- Two-phase execution separates game logic from presentation (Logic → Visual)
- Reaction actions allow actions to trigger other actions
- Priority system ensures correct turn order

---

## Core Actions

### DamageAction

**Location**: `PokemonUltimate.Combat.Actions.DamageAction`

**Purpose**: Applies damage to a target Pokemon slot.

#### Constructor

```csharp
public DamageAction(BattleSlot user, BattleSlot target, DamageContext context)
```

**Parameters**:
- `user`: The slot that initiated this damage (attacker). Can be null for passive damage.
- `target`: The slot receiving damage. **Required**.
- `context`: The `DamageContext` containing calculated damage. **Required**.

**Throws**: `ArgumentNullException` if target or context is null.

#### ExecuteLogic Behavior

1. **Validation**: Checks if target slot is empty or already fainted → returns empty
2. **Zero Damage Check**: If `context.FinalDamage == 0` → returns empty
3. **Apply Damage**: Calls `target.Pokemon.TakeDamage(damage)`
4. **Faint Check**: If `target.Pokemon.IsFainted` → returns `FaintAction`
5. **Return**: Empty enumerable if no faint

**Reaction Actions**:
- Returns `FaintAction` if HP reaches 0
- Returns empty enumerable otherwise

#### ExecuteVisual Behavior

1. Plays damage animation via `view.PlayDamageAnimation(target)`
2. Updates HP bar via `view.UpdateHPBar(target)`
3. Skips visual if slot is empty or damage is 0

#### Key Features

- **Overkill Protection**: `TakeDamage()` prevents HP from going below 0
- **Faint Detection**: Automatically generates `FaintAction` when HP reaches 0
- **Zero Damage Handling**: Gracefully handles immune moves or status moves

#### Usage Examples

```csharp
// From UseMoveAction
var pipeline = new DamagePipeline();
var context = pipeline.Calculate(attacker, defender, move, field);
var damageAction = new DamageAction(attacker, defender, context);
actions.Add(damageAction);

// Recoil damage
var recoilContext = new DamageContext(user, user, move, field) {
    BaseDamage = recoilAmount,
    Multiplier = 1.0f
};
actions.Add(new DamageAction(user, user, recoilContext));
```

#### Edge Cases Handled

- Empty slot → No damage applied
- Already fainted → No damage applied
- Zero damage → No damage applied, no visual
- Overkill damage → HP clamped to 0, FaintAction still generated

---

### FaintAction

**Location**: `PokemonUltimate.Combat.Actions.FaintAction`

**Purpose**: Handles a Pokemon fainting (HP reaching zero).

#### Constructor

```csharp
public FaintAction(BattleSlot user, BattleSlot target)
```

**Parameters**:
- `user`: The slot that caused the faint (attacker). Can be null for system actions.
- `target`: The slot containing the fainted Pokemon. **Required**.

**Throws**: `ArgumentNullException` if target is null.

#### ExecuteLogic Behavior

1. **Validation**: Checks if target slot is empty → returns empty
2. **State Check**: Pokemon should already be at 0 HP (set by `DamageAction`)
3. **Battle End**: Battle end checking is deferred to `CombatEngine`
4. **Return**: Empty enumerable (no reactions)

**Note**: This action primarily handles visual feedback. The Pokemon is already marked as fainted by `DamageAction`.

#### ExecuteVisual Behavior

1. Plays faint animation via `view.PlayFaintAnimation(target)`
2. Skips animation if slot is empty

#### Key Features

- **Separation of Concerns**: Logic handled by `DamageAction`, visual handled here
- **Battle End Deferred**: CombatEngine checks for battle end after all actions
- **No State Changes**: Pokemon is already fainted, this is just cleanup/visual

#### Usage Examples

```csharp
// Generated by DamageAction
if (target.Pokemon.IsFainted)
{
    return new[] { new FaintAction(user, target) };
}

// System faint (weather damage, etc.)
var faintAction = new FaintAction(null, targetSlot);
```

#### Edge Cases Handled

- Empty slot → No action taken
- Already fainted → Action still executes (for visual consistency)

---

### HealAction

**Location**: `PokemonUltimate.Combat.Actions.HealAction`

**Purpose**: Restores HP to a target Pokemon.

#### Constructor

```csharp
public HealAction(BattleSlot user, BattleSlot target, int amount)
```

**Parameters**:
- `user`: The slot that initiated this healing. Can be null for system actions.
- `target`: The slot receiving healing. **Required**.
- `amount`: The amount of HP to restore. Must be non-negative.

**Throws**: 
- `ArgumentNullException` if target is null
- `ArgumentException` if amount is negative

#### ExecuteLogic Behavior

1. **Validation**: Checks if target slot is empty or amount is 0 → returns empty
2. **Apply Healing**: Calls `target.Pokemon.Heal(amount)`
3. **Overheal Protection**: `Heal()` method prevents HP from exceeding MaxHP
4. **Return**: Empty enumerable (no reactions)

**Reaction Actions**: None (healing doesn't trigger other actions directly)

#### ExecuteVisual Behavior

1. Updates HP bar via `view.UpdateHPBar(target)`
2. Skips visual if slot is empty or amount is 0

#### Key Features

- **Overheal Protection**: `Heal()` method clamps HP to MaxHP
- **Zero Amount Handling**: Gracefully handles zero healing
- **No Faint Prevention**: Cannot revive fainted Pokemon (validation needed in future)

#### Usage Examples

```csharp
// From HealEffect
var healAmount = (int)(user.Pokemon.MaxHP * healEffect.HealPercent / 100f);
actions.Add(new HealAction(user, user, healAmount));

// From item (Potion)
actions.Add(new HealAction(null, target, 20));

// Percentage-based healing
var healAmount = (int)(target.Pokemon.MaxHP * 0.5f); // 50%
actions.Add(new HealAction(user, target, healAmount));
```

#### Edge Cases Handled

- Empty slot → No healing applied
- Zero amount → No healing applied, no visual
- Negative amount → Validation error (exception)
- Overheal → HP clamped to MaxHP
- Fainted Pokemon → Cannot heal (validation needed)

---

### StatChangeAction

**Location**: `PokemonUltimate.Combat.Actions.StatChangeAction`

**Purpose**: Modifies a stat stage of a Pokemon (-6 to +6).

#### Constructor

```csharp
public StatChangeAction(BattleSlot user, BattleSlot target, Stat stat, int change)
```

**Parameters**:
- `user`: The slot that initiated this change. Can be null for system actions.
- `target`: The slot whose stat is being modified. **Required**.
- `stat`: The stat to modify. Cannot be HP.
- `change`: The amount to change (+/-). Can be any integer (clamped internally).

**Throws**: 
- `ArgumentNullException` if target is null
- `ArgumentException` if stat is HP

#### ExecuteLogic Behavior

1. **Validation**: Checks if target slot is empty or change is 0 → returns empty
2. **Apply Change**: Calls `target.ModifyStatStage(stat, change)`
3. **Clamping**: `ModifyStatStage()` automatically clamps to -6/+6 range
4. **Return**: Empty enumerable (no reactions)

**Reaction Actions**: None (stat changes don't trigger other actions directly)

#### ExecuteVisual Behavior

1. Shows stat change indicator via `view.ShowStatChange(target, statName, change)`
2. Skips visual if slot is empty or change is 0

#### Key Features

- **Automatic Clamping**: Stages are clamped to valid range (-6 to +6)
- **HP Protection**: Cannot modify HP stat (validation error)
- **Persistent**: Stat stages persist until battle ends or reset
- **Stacking**: Multiple changes stack correctly

#### Usage Examples

```csharp
// Swords Dance (+2 Attack)
actions.Add(new StatChangeAction(user, user, Stat.Attack, 2));

// Growl (-1 Attack)
actions.Add(new StatChangeAction(user, target, Stat.Attack, -1));

// Dragon Dance (+1 Attack, +1 Speed) - requires two actions
actions.Add(new StatChangeAction(user, user, Stat.Attack, 1));
actions.Add(new StatChangeAction(user, user, Stat.Speed, 1));
```

#### Edge Cases Handled

- Empty slot → No change applied
- Zero change → No change applied, no visual
- HP stat → Validation error (exception)
- Large changes → Clamped to -6/+6
- Fainted Pokemon → Stat changes still apply (for consistency)

---

### ApplyStatusAction

**Location**: `PokemonUltimate.Combat.Actions.ApplyStatusAction`

**Purpose**: Applies a persistent status condition to a Pokemon.

#### Constructor

```csharp
public ApplyStatusAction(BattleSlot user, BattleSlot target, PersistentStatus status)
```

**Parameters**:
- `user`: The slot that initiated this status application. Can be null for system actions.
- `target`: The slot receiving the status. **Required**.
- `status`: The status condition to apply. Can be `None` to clear status.

**Throws**: `ArgumentNullException` if target is null.

#### ExecuteLogic Behavior

1. **Validation**: Checks if target slot is empty → returns empty
2. **Apply Status**: Sets `target.Pokemon.Status = status`
3. **Status Replacement**: New status replaces old status (only one persistent status at a time)
4. **Return**: Empty enumerable (no reactions)

**Reaction Actions**: None (status application doesn't trigger other actions directly)

#### ExecuteVisual Behavior

1. Plays status animation via `view.PlayStatusAnimation(target, statusName)`
2. Skips animation if slot is empty or status is `None`

#### Key Features

- **Single Status Rule**: Only one persistent status at a time
- **Status Replacement**: New status replaces old status
- **Status Clearing**: Can clear status by applying `None`
- **Persistent**: Status persists until cured or battle ends

#### Usage Examples

```csharp
// Thunder Wave (Paralysis)
actions.Add(new ApplyStatusAction(user, target, PersistentStatus.Paralysis));

// Will-O-Wisp (Burn)
actions.Add(new ApplyStatusAction(user, target, PersistentStatus.Burn));

// Rest (Sleep on self)
actions.Add(new ApplyStatusAction(user, user, PersistentStatus.Sleep));

// Heal Bell (clear status)
actions.Add(new ApplyStatusAction(user, target, PersistentStatus.None));
```

#### Edge Cases Handled

- Empty slot → No status applied
- Status is None → Clears existing status, no animation
- Already statused → Replaces old status
- Fainted Pokemon → No status applied (validation needed)

---

### UseMoveAction

**Location**: `PokemonUltimate.Combat.Actions.UseMoveAction`

**Purpose**: Executes a move in battle. The most complex action, handling PP, status checks, accuracy, and effect processing.

#### Constructor

```csharp
public UseMoveAction(BattleSlot user, BattleSlot target, MoveInstance moveInstance)
```

**Parameters**:
- `user`: The slot using the move. **Required**.
- `target`: The target slot. **Required**.
- `moveInstance`: The move instance to use. **Required**.

**Throws**: `ArgumentNullException` if any parameter is null.

#### Properties

- `Target`: The target slot
- `MoveInstance`: The move instance being used
- `Move`: The move data blueprint (`MoveInstance.Move`)
- `Priority`: Override from move data (`Move.Priority`)
- `CanBeBlocked`: Returns `true` (moves can be blocked by Protect)

#### ExecuteLogic Behavior

**Phase 1: Pre-Execution Checks**
1. **PP Check**: If `!MoveInstance.HasPP` → returns `MessageAction("X has no PP left!")`
2. **Flinch Check**: If `User.HasVolatileStatus(VolatileStatus.Flinch)` → returns `MessageAction("X flinched!")`, removes flinch
3. **Status Checks**: Checks Sleep, Freeze, Paralysis (25% chance for paralysis)
   - Sleep → returns `MessageAction("X is fast asleep.")`
   - Freeze → returns `MessageAction("X is frozen solid!")`
   - Paralysis → 25% chance to return `MessageAction("X is paralyzed!")`

**Phase 2: Execution**
4. **Deduct PP**: Calls `MoveInstance.Use()`
5. **Generate Message**: Adds `MessageAction("X used Y!")`
6. **Accuracy Check**: Uses `AccuracyChecker.CheckHit(user, target, move)`
   - If miss → adds `MessageAction("The attack missed!")`, returns
7. **Process Effects**: Iterates through `Move.Effects`:
   - `DamageEffect` → Calculates damage via `DamagePipeline`, creates `DamageAction`
   - `StatusEffect` → Checks chance, creates `ApplyStatusAction`
   - `StatChangeEffect` → Checks chance, creates `StatChangeAction`
   - `HealEffect` → Calculates heal amount, creates `HealAction`
   - `RecoilEffect` → Calculates recoil damage, creates `DamageAction` (self-target)
   - `FlinchEffect` → Checks chance, adds volatile status to target

**Reaction Actions**: Returns list of generated actions (damage, status, stat changes, etc.)

#### ExecuteVisual Behavior

1. Plays move animation via `view.PlayMoveAnimation(user, target, move.Id)`
2. Skips visual if slot is empty

#### Key Features

- **Comprehensive Checks**: PP, flinch, status conditions all checked before execution
- **Effect Processing**: Handles multiple effect types with chance-based logic
- **Child Actions**: Generates appropriate actions for each effect
- **Accuracy System**: Integrates with `AccuracyChecker` for hit/miss determination

#### Usage Examples

```csharp
// Basic move usage
var moveInstance = user.Pokemon.Moves[0];
var action = new UseMoveAction(userSlot, targetSlot, moveInstance);
queue.Enqueue(action);

// From turn selection
var selectedMove = selectedMoveInstance;
var selectedTarget = selectedTargetSlot;
var moveAction = new UseMoveAction(activeSlot, selectedTarget, selectedMove);
```

#### Edge Cases Handled

- No PP → Fail message, no execution
- Flinched → Fail message, flinch consumed
- Asleep/Frozen → Fail message, no execution
- Paralyzed → 25% chance to fail
- Empty user/target → No execution
- Miss → Miss message, no effects applied
- Zero damage → No damage action generated

---

### SwitchAction

**Location**: `PokemonUltimate.Combat.Actions.SwitchAction`

**Purpose**: Switches a Pokemon in battle.

#### Constructor

```csharp
public SwitchAction(BattleSlot slot, PokemonInstance newPokemon)
```

**Parameters**:
- `slot`: The slot to switch. **Required**.
- `newPokemon`: The Pokemon to switch in. **Required**.

**Throws**: `ArgumentNullException` if slot or newPokemon is null.

#### Properties

- `Slot`: The slot being switched
- `NewPokemon`: The Pokemon to switch in
- `Priority`: Returns `6` (highest priority)
- `CanBeBlocked`: Returns `false` (switches cannot be blocked)

#### ExecuteLogic Behavior

1. **Validation**: Checks if slot is empty → returns empty
2. **Get Side**: Gets `slot.Side` (for party management)
3. **Switch Pokemon**: Calls `slot.SetPokemon(newPokemon)`
4. **Battle State Reset**: `SetPokemon()` automatically calls `ResetBattleState()`
   - Resets stat stages to 0
   - Clears volatile status
   - Preserves persistent status (handled by PokemonInstance)
5. **Return**: Empty enumerable (no reactions)

**Reaction Actions**: None (switching doesn't trigger other actions directly, but entry effects will be handled in future)

#### ExecuteVisual Behavior

1. Plays switch-out animation via `view.PlaySwitchOutAnimation(slot)`
2. Plays switch-in animation via `view.PlaySwitchInAnimation(slot)`
3. Skips animations if slot is empty

#### Key Features

- **Highest Priority**: Priority +6 ensures switches execute before all moves
- **Battle State Reset**: Automatically resets stat stages and volatile status
- **Cannot Be Blocked**: Switches always succeed
- **Entry Effects**: Future implementation will handle entry hazards and abilities

#### Usage Examples

```csharp
// Player switch
var benchPokemon = playerSide.Party[1];
var switchAction = new SwitchAction(activeSlot, benchPokemon);
queue.Enqueue(switchAction);

// Forced switch (Roar, Whirlwind)
var forcedSwitch = new SwitchAction(targetSlot, randomBenchPokemon);
queue.Enqueue(forcedSwitch);
```

#### Edge Cases Handled

- Empty slot → No switch performed
- Fainted Pokemon → Cannot switch to fainted Pokemon (validation needed)
- No bench available → Validation needed in CombatEngine

---

### MessageAction

**Location**: `PokemonUltimate.Combat.Actions.MessageAction`

**Purpose**: Displays a message to the player. Pure presentation action with no game logic.

#### Constructor

```csharp
public MessageAction(string message)
```

**Parameters**:
- `message`: The message to display. **Required**.

**Throws**: `ArgumentNullException` if message is null.

#### Properties

- `Message`: The message text
- `User`: Always `null` (system action)
- `Priority`: Returns `0` (default)
- `CanBeBlocked`: Returns `false` (messages cannot be blocked)

#### ExecuteLogic Behavior

1. **No Logic**: Returns empty enumerable (no state changes)

**Reaction Actions**: None

#### ExecuteVisual Behavior

1. Shows message via `view.ShowMessage(message)`

#### Key Features

- **Pure Presentation**: No game logic, only visual feedback
- **Flexible**: Can be inserted anywhere in action queue
- **Simple**: Minimal overhead, used frequently

#### Usage Examples

```csharp
// Move usage message
actions.Add(new MessageAction($"{user.Name} used {move.Name}!"));

// Status message
actions.Add(new MessageAction($"{target.Name} is paralyzed!"));

// Effectiveness message
actions.Add(new MessageAction("It's super effective!"));

// Failure message
actions.Add(new MessageAction($"{user.Name} has no PP left!"));
```

#### Edge Cases Handled

- Null message → Validation error (exception)
- Empty message → Displayed as-is (UI handles)

---

## Action Execution Flow

### Queue Processing

```
1. Dequeue next action
2. Check if blocked (if CanBeBlocked == true)
3. Fire PRE triggers (abilities, items)
4. ExecuteLogic() → Get reaction actions
5. Insert reaction actions at front of queue
6. ExecuteVisual() → Show to player
7. Fire POST triggers (abilities, items)
8. Repeat until queue is empty
```

### Reaction Action Insertion

Reaction actions are inserted at the **front** of the queue to ensure immediate execution:

```csharp
var reactions = action.ExecuteLogic(field);
foreach (var reaction in reactions)
{
    queue.InsertAtFront(reaction);
}
```

This ensures that:
- `DamageAction` → `FaintAction` executes immediately
- Status effects apply before next action
- Stat changes affect subsequent actions

---

## Action Priority System

### Priority Values

| Priority | Examples | Description |
|----------|----------|-------------|
| +6 | SwitchAction | Highest priority (switches) |
| +5 | Helping Hand | Very high priority |
| +4 | Protect, Detect | High priority |
| +3 | Fake Out | Above normal |
| +2 | Extreme Speed | Above normal |
| +1 | Quick Attack | Slightly above normal |
| 0 | Most moves | Normal priority |
| -1 | Vital Throw | Below normal |
| -3 | Focus Punch | Low priority |
| -5 | Avalanche | Very low priority |
| -6 | Counter | Lowest priority |
| -7 | Trick Room | Special case |

### Priority Resolution

1. Actions sorted by priority (highest first)
2. Within same priority, sorted by Speed
3. Speed ties resolved randomly (50/50)

---

## Action Blocking

### CanBeBlocked Property

- `true`: Action can be blocked by Protect, Detect, etc.
- `false`: Action always executes (switches, status damage, etc.)

### Blocking Logic

```csharp
if (action.CanBeBlocked && IsBlocked(action))
{
    queue.InsertAtFront(new BlockedMessageAction(action));
    continue;
}
```

### Blockable Actions

- `UseMoveAction`: CanBeBlocked = true
- `DamageAction`: CanBeBlocked = false (reaction action)
- `SwitchAction`: CanBeBlocked = false (always succeeds)

---

## Best Practices

### 1. Always Validate Inputs

```csharp
public DamageAction(BattleSlot user, BattleSlot target, DamageContext context)
{
    Target = target ?? throw new ArgumentNullException(nameof(target));
    Context = context ?? throw new ArgumentNullException(nameof(context));
}
```

### 2. Handle Edge Cases Gracefully

```csharp
if (Target.IsEmpty || Target.HasFainted)
    return Enumerable.Empty<BattleAction>();
```

### 3. Use Centralized Messages

```csharp
actions.Add(new MessageAction(string.Format(GameMessages.MoveNoPP, pokemon.Name)));
```

### 4. Return Appropriate Reaction Actions

```csharp
if (Target.Pokemon.IsFainted)
{
    return new[] { new FaintAction(User, Target) };
}
```

### 5. Separate Logic and Visual

```csharp
// Logic phase: Update state
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
{
    // Update game state
    return reactions;
}

// Visual phase: Show to player
public override Task ExecuteVisual(IBattleView view)
{
    return view.PlayAnimation();
}
```

---

## Common Patterns

### Pattern 1: Damage → Faint Chain

```csharp
// In DamageAction.ExecuteLogic()
int actualDamage = Target.Pokemon.TakeDamage(damage);
if (Target.Pokemon.IsFainted)
{
    return new[] { new FaintAction(User, Target) };
}
```

### Pattern 2: Move → Effects Chain

```csharp
// In UseMoveAction.ExecuteLogic()
foreach (var effect in Move.Effects)
{
    if (effect is DamageEffect damageEffect)
    {
        var context = pipeline.Calculate(User, Target, Move, field);
        actions.Add(new DamageAction(User, Target, context));
    }
    // ... other effects
}
```

### Pattern 3: Chance-Based Effects

```csharp
if (random.Next(100) < effect.ChancePercent)
{
    actions.Add(new ApplyStatusAction(User, Target, status));
}
```

### Pattern 4: Self-Targeting

```csharp
var targetSlot = effect.TargetSelf ? User : Target;
actions.Add(new StatChangeAction(User, targetSlot, stat, stages));
```

---

## Troubleshooting

### Issue: Action Not Executing

**Symptoms**: Action is queued but doesn't execute.

**Possible Causes**:
1. Action is blocked (check `CanBeBlocked` and blocking logic)
2. Queue is not processing (check `BattleQueue` implementation)
3. Action is being filtered out (check queue filtering logic)

**Solution**: Add logging to `BattleQueue` to track action execution.

### Issue: Reaction Actions Not Firing

**Symptoms**: `DamageAction` doesn't generate `FaintAction` when HP reaches 0.

**Possible Causes**:
1. `ExecuteLogic()` not returning reaction actions
2. Reaction actions not being inserted into queue
3. Queue processing stopped before reactions

**Solution**: Verify `ExecuteLogic()` return value and queue insertion logic.

### Issue: Visual Not Playing

**Symptoms**: Action executes but no animation/message shown.

**Possible Causes**:
1. `ExecuteVisual()` not being called
2. `IBattleView` implementation not handling action
3. Visual skipped due to empty slot check

**Solution**: Check `IBattleView` implementation and visual execution flow.

### Issue: Priority Not Working

**Symptoms**: Actions execute in wrong order.

**Possible Causes**:
1. Priority not set correctly on action
2. `TurnOrderResolver` not sorting by priority
3. Speed calculation incorrect

**Solution**: Verify priority values and sorting logic in `TurnOrderResolver`.

---

## Summary

This bible documents all combat actions in PokemonUltimate. Each action follows consistent patterns:

1. **Validation**: Check inputs and edge cases
2. **Logic**: Update game state, return reactions
3. **Visual**: Show feedback to player
4. **Integration**: Work with other actions seamlessly

Actions are the foundation of the combat system. Understanding them is essential for implementing new features and debugging issues.

---

**Last Updated**: Phase 2.5 Complete  
**Status**: ✅ All core actions documented

