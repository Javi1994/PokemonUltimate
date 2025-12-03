# Feature 2: Combat System - Architecture

> Complete technical specification of the Pokemon battle engine.

**Feature Number**: 2  
**Feature Name**: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

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

---

## 4. Battle Mechanics Catalog

This section catalogs **all BattleActions, IMoveEffects, and Data Types** needed to support authentic Pokémon combat mechanics. Each entry follows the "Everything is an Action" pattern.

### 4.1 Core BattleActions

#### Damage & HP
| Action | Description | Example |
|--------|-------------|---------|
| `DamageAction` | Standard damage application | All damaging moves |
| `FixedDamageAction` | Fixed amount (ignores stats) | Seismic Toss, Dragon Rage |
| `PercentDamageAction` | % of target's max HP | Super Fang (50%) |
| `HealAction` | Restore HP | Synthesis, Moonlight |
| `RecoilDamageAction` | User takes % of damage dealt | Double-Edge, Brave Bird |
| `DrainAction` | Damage + heal user | Giga Drain, Drain Punch |

#### Status & Stats
| Action | Description | Example |
|--------|-------------|---------|
| `ApplyStatusAction` | Apply major status | Burn, Paralysis, Sleep |
| `CureStatusAction` | Remove status | Heal Bell, Aromatherapy |
| `StatChangeAction` | Modify stat stages | Swords Dance (+2 Atk) |
| `StatResetAction` | Reset all stat changes | Haze, Clear Smog |
| `StatSwapAction` | Swap stat stages | Psych Up, Guard Swap |

#### Field & Environment
| Action | Description | Example |
|--------|-------------|---------|
| `SetWeatherAction` | Change weather | Rain Dance, Sunny Day |
| `SetTerrainAction` | Change terrain | Electric Terrain |
| `SetFieldEffectAction` | Room effects | Trick Room, Wonder Room |
| `SetHazardAction` | Entry hazards | Stealth Rock, Spikes |
| `SetScreenAction` | Defensive screens | Reflect, Light Screen |

#### Control & Utility
| Action | Description | Example |
|--------|-------------|---------|
| `FlinchAction` | Apply flinch status | Fake Out, Air Slash |
| `ConfuseAction` | Apply confusion | Confuse Ray, Swagger |
| `ProtectAction` | Block next move | Protect, Detect |
| `DisableAction` | Disable last used move | Disable |
| `TauntAction` | Can only use damaging moves | Taunt |

#### Special Mechanics
| Action | Description | Example |
|--------|-------------|---------|
| `FaintAction` | Mark as fainted, trigger death events | HP reaches 0 |
| `TransformAction` | Copy target's stats/moves | Transform (Ditto) |
| `SubstituteAction` | Create HP-based decoy | Substitute |
| `BatonPassAction` | Switch + keep stat changes | Baton Pass |
| `WishAction` | Delayed heal (2 turns) | Wish |
| `FutureSightAction` | Delayed damage (2 turns) | Future Sight |

### 4.2 IMoveEffect Catalog

#### Damage Effects
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `DamageEffect` | - | Standard damage formula |
| `FixedDamageEffect` | `int Amount` | Fixed damage (20, 40, 80) |
| `LevelDamageEffect` | - | Damage = User's Level |
| `PercentDamageEffect` | `float Percent` | % of target's HP (0.5 = 50%) |
| `MultiHitEffect` | `int MinHits, int MaxHits` | Hit 2-5 times |
| `CriticalRatioEffect` | `int Stages` | Increased crit chance |
| `OneHitKOEffect` | - | OHKO if hits (Fissure, Guillotine) |

#### HP Manipulation
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `RecoilEffect` | `float Percent` | User takes % of damage dealt |
| `DrainEffect` | `float Percent` | User heals % of damage dealt |
| `HealEffect` | `float Percent` | Heal % of max HP |
| `SacrificialEffect` | - | User faints (Explosion, Self-Destruct) |
| `PainSplitEffect` | - | Average HP of user and target |

#### Status Application
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `BurnEffect` | `float Chance` | Apply Burn |
| `ParalyzeEffect` | `float Chance` | Apply Paralysis |
| `PoisonEffect` | `float Chance, bool BadlyPoisoned` | Apply Poison/Badly Poisoned |
| `SleepEffect` | `float Chance` | Apply Sleep |
| `FreezeEffect` | `float Chance` | Apply Freeze |
| `ConfuseEffect` | `float Chance` | Apply Confusion |
| `FlinchEffect` | `float Chance` | Apply Flinch (only if slower) |

#### Stat Modification
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `StatChangeEffect` | `Stat, int Stages, bool Self` | Raise/lower stat |
| `MultiStatChangeEffect` | `Dictionary<Stat, int>` | Multiple stats at once |
| `SwaggerEffect` | - | Confuse + raise target's Attack |

#### Field Effects
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `WeatherEffect` | `WeatherType, int Duration` | Set weather |
| `TerrainEffect` | `TerrainType, int Duration` | Set terrain |
| `HazardEffect` | `HazardType, int Layers` | Set entry hazard |
| `ScreenEffect` | `ScreenType, int Duration` | Set defensive screen |
| `TrickRoomEffect` | `int Duration` | Reverse speed order |

#### Special Mechanics
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `ChargeEffect` | `bool Invulnerable, string Message` | 2-turn moves (Fly, Solar Beam) |
| `ProtectEffect` | - | Block incoming move |
| `CounterEffect` | `bool Physical` | Return 2x damage (Counter, Mirror Coat) |
| `BindEffect` | `int Turns` | Trap + damage (Wrap, Fire Spin) |
| `LeechSeedEffect` | - | Drain 1/8 HP each turn |
| `DestinyBondEffect` | - | If user faints, faint attacker too |

### 4.3 Implementation Priority

**Phase 1 (MVP)**: ✅ Complete
- DamageAction, HealAction, FaintAction
- ApplyStatusAction, StatChangeAction
- Basic IMoveEffects (Damage, Status, StatChange)

**Phase 2 (Core Mechanics)**: ✅ Complete
- RecoilEffect, DrainEffect, MultiHitEffect
- FlinchEffect, ConfuseEffect
- WeatherEffect, HazardEffect

**Phase 3 (Advanced)**: ⏳ Planned
- ChargeEffect (2-turn moves)
- ProtectEffect, SubstituteEffect
- TransformEffect, MetronomeEffect

**Phase 4 (Complex)**: ⏳ Planned
- BatonPassAction, WishEffect
- CounterEffect, DestinyBondEffect
- Field effects (Trick Room, Terrain)

---

## Related Documents

- **[Use Cases](use_cases.md)** - Scenarios this architecture supports
- **[Roadmap](roadmap.md)** - Implementation plan for all phases (2.1-2.19)
- **[Testing](testing.md)** - How to test this architecture
- **[Code Location](code_location.md)** - Where this is implemented
- **[Feature 1: Game Data](../1-game-data/architecture.md)** - Pokemon instances used in battles
- **[Feature 3: Content Expansion](../3-content-expansion/roadmap.md)** - Moves, abilities, items
- **[Feature 4: Unity Integration](../4-unity-integration/architecture.md)** - Visual battle presentation

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

---

**Last Updated**: 2025-01-XX