# Sub-Feature 2.7: Integration - Architecture

> Technical specification of AI providers, Player input, and Full battles.

**Sub-Feature Number**: 2.7  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 1. Overview
To support **Manual Play**, **AI Enemies**, and **Autoplay** (AI playing for the user), we abstract the source of decisions. The Combat Engine does not care *who* makes the decision, only that an Action is provided.

## 2. Core Abstraction: `IActionProvider`

Every active Pokemon in battle is assigned a "Brain" (Provider).

```csharp
public interface IActionProvider {
    // Returns the Action the pokemon wants to perform this turn.
    // Async because Player input takes time (UI), while AI is instant.
    Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot);
}
```

## 3. Implementations

### A. `PlayerInputProvider` (Manual Control)
Connects the Logic to the View/UI.

**Location**: `PokemonUltimate.Combat/Providers/PlayerInputProvider.cs`

**Constructor**:
```csharp
public PlayerInputProvider(IBattleView view)
{
    _view = view ?? throw new ArgumentNullException(nameof(view));
}
```

**Complete Implementation**:
```csharp
public class PlayerInputProvider : IActionProvider {
    private readonly IBattleView _view;

    public PlayerInputProvider(IBattleView view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public async Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot) {
        if (field == null)
            throw new ArgumentNullException(nameof(field));
        if (mySlot == null)
            throw new ArgumentNullException(nameof(mySlot));
        if (mySlot.IsEmpty || mySlot.HasFainted)
            return null; // Can't act if fainted

        // 1. Show action menu (Fight/Switch/Item/Run)
        var actionType = await _view.SelectActionType(mySlot);

        // 2. Route based on selection
        switch (actionType) {
            case BattleActionType.Fight:
                return await HandleFightAction(field, mySlot);
            
            case BattleActionType.Switch:
                return await HandleSwitchAction(field, mySlot);
            
            case BattleActionType.Item:
                // Future: Item usage
                throw new NotImplementedException("Item usage not yet implemented");
            
            case BattleActionType.Run:
                // Future: Flee from battle
                throw new NotImplementedException("Run not yet implemented");
            
            default:
                throw new ArgumentException($"Unknown action type: {actionType}");
        }
    }

    private async Task<BattleAction> HandleFightAction(BattleField field, BattleSlot mySlot) {
        // 1. Get available moves (with PP > 0)
        var availableMoves = mySlot.Pokemon.Moves.Where(m => m.HasPP).ToList();
        if (availableMoves.Count == 0)
            return null; // No moves available

        // 2. Select move
        var moveInstance = await _view.SelectMove(availableMoves);
        if (moveInstance == null)
            return null; // Player cancelled

        // 3. Get valid targets
        var validTargets = TargetResolver.GetValidTargets(mySlot, moveInstance.Move, field);
        if (validTargets.Count == 0)
            return null; // No valid targets

        // 4. Select target (auto-select if only one)
        BattleSlot target;
        if (validTargets.Count == 1) {
            target = validTargets[0]; // Auto-select
        } else {
            target = await _view.SelectTarget(validTargets);
            if (target == null)
                return null; // Player cancelled
        }

        return new UseMoveAction(mySlot, target, moveInstance);
    }

    private async Task<BattleAction> HandleSwitchAction(BattleField field, BattleSlot mySlot) {
        // 1. Get available Pokemon to switch to
        var side = mySlot.Side;
        var availablePokemon = side.GetAvailableSwitches().ToList();
        if (availablePokemon.Count == 0)
            return null; // No Pokemon available to switch

        // 2. Select Pokemon to switch in
        var newPokemon = await _view.SelectSwitch(availablePokemon);
        if (newPokemon == null)
            return null; // Player cancelled

        return new SwitchAction(mySlot, newPokemon);
    }
}
```

**Key Behaviors**:
- Returns `null` if slot is empty/fainted (no action)
- Returns `null` if no moves available (should be handled by UI, but defensive)
- Returns `null` if player cancels selection
- Auto-selects target if only one valid option
- Validates all inputs before creating actions

### B. `AIActionProvider` (The Bot)
Pure logic decision maker.

```csharp
public class AIActionProvider : IActionProvider {
    private IAILogic _logic; // Strategy (Random, Aggressive, Smart)

    public async Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot) {
        // Optional: Fake thinking delay for realism
        await Task.Delay(500); 
        
        // Delegate decision to the strategy
        return _logic.DecideBestAction(field, mySlot);
    }
}
```

## 4. The "Autoplay" Feature
Implementing Autoplay is trivial with this pattern. We simply swap the provider at runtime.

```csharp
public class BattleManager {
    public void ToggleAutoplay(bool enabled) {
        foreach (var slot in PlayerSide.ActiveSlots) {
            if (enabled) {
                // Replace Player Input with AI
                slot.ActionProvider = new AIActionProvider(new SmartAIStrategy());
            } else {
                // Restore Manual Control
                slot.ActionProvider = new PlayerInputProvider(_view);
            }
        }
    }
}
```

## 5. AI Logic Strategies (`IAILogic`)
We can have different "brains" for different difficulty levels.

### `RandomAI`
-   Picks a random valid move.

### `SmartAI` (Standard Enemy / Autoplay)
1.  **Kill Check**: Does any move kill the target? -> Use it.
2.  **Type Matchup**: Filter moves that are Super Effective.
3.  **Status Check**: If target is not burned/poisoned, prioritize Status moves.
4.  **Healing**: If HP < 30%, use Potion or Heal move.

```csharp
public BattleAction DecideBestAction(BattleField field, BattleSlot me) {
    // We evaluate every possible Move + Target combination
    var bestOption = new { Score = -999f, Action = (BattleAction)null };

    foreach (var move in me.Pokemon.Moves) {
        var targets = TargetResolver.GetValidTargets(me, move.Data, field);
        
        foreach (var target in targets) {
            float score = Evaluate(move, target);
            if (score > bestOption.Score) {
                bestOption = new { Score = score, Action = new UseMoveAction(me, move.Data, target) };
            }
        }
    }
        
    return bestOption.Action ?? new WaitAction(me);
}
```

## 6. Integration with Action Queue
The flow remains perfectly consistent with our "Everything is an Action" rule.

1.  **PhaseActionSelection**:
    ```csharp
    foreach (var slot in AllActiveSlots) {
        // Engine doesn't know if this is Player or AI
        var action = await slot.ActionProvider.GetAction(field, slot);
        _queue.Enqueue(action);
    }
    ```
2.  **PhaseExecution**:
    The Queue processes the actions. The `UseMoveAction` generated by the AI is identical to the one generated by the Player.

## 7. Critical Use Cases

### 7.1 Autoplay (AI Controlling Player)
**Scenario**: Player enables autoplay, AI takes control of player's Pokemon.

**Implementation**:
```csharp
public class CombatEngine {
    public void ToggleAutoplay(bool enabled) {
        foreach (var slot in Field.PlayerSide.GetActiveSlots()) {
            if (enabled) {
                // Store original provider for restoration
                slot.OriginalProvider = slot.ActionProvider;
                slot.ActionProvider = new RandomAI(); // or SmartAI
            } else {
                // Restore manual control
                slot.ActionProvider = slot.OriginalProvider ?? new PlayerInputProvider(_view);
            }
        }
    }
}
```

**Key Points**:
- Each slot can have its provider swapped independently
- Original provider should be stored for restoration
- Works seamlessly with existing `CombatEngine` - no changes needed to battle loop

### 7.2 AI vs AI (Observer Mode)
**Scenario**: Both sides controlled by AI, player watches the battle unfold.

**Implementation**:
```csharp
// Initialize battle with AI on both sides
var playerAI = new RandomAI();
var enemyAI = new AlwaysAttackAI();

engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

// Run battle - no player input needed
var result = await engine.RunBattle();
```

**Key Points**:
- Both `playerProvider` and `enemyProvider` can be AI implementations
- Battle runs completely autonomously
- Useful for testing, AI training, or spectator mode

### 7.3 Mixed Control (Partial Autoplay)
**Scenario**: Player controls some Pokemon, AI controls others (e.g., doubles battle).

**Implementation**:
```csharp
// Player controls slot 0, AI controls slot 1
var playerSide = Field.PlayerSide;
playerSide.GetSlot(0).ActionProvider = new PlayerInputProvider(_view);
playerSide.GetSlot(1).ActionProvider = new RandomAI();

// Enemy side: both AI
Field.EnemySide.GetSlot(0).ActionProvider = new AlwaysAttackAI();
Field.EnemySide.GetSlot(1).ActionProvider = new AlwaysAttackAI();
```

**Key Points**:
- Each slot has independent `ActionProvider`
- Can mix player and AI on same side
- Useful for:
  - Co-op mode (player + AI teammate)
  - Testing specific scenarios
  - Tutorial mode (AI demonstrates while player controls one Pokemon)

### 7.4 Dynamic Provider Switching
**Scenario**: Change control during battle (e.g., player gets disconnected, AI takes over).

**Implementation**:
```csharp
// During battle, swap provider for a specific slot
public void SwitchSlotToAI(BattleSlot slot, IActionProvider aiProvider) {
    if (slot == null)
        throw new ArgumentNullException(nameof(slot));
    if (aiProvider == null)
        throw new ArgumentNullException(nameof(aiProvider));
    
    slot.ActionProvider = aiProvider;
    // Next turn, this slot will use AI
}
```

**Key Points**:
- Provider can be changed at any time
- Change takes effect on next turn
- No need to restart battle or reinitialize

### 7.5 Provider Per Slot (Not Per Side)
**Critical Design**: `ActionProvider` is assigned **per slot**, not per side.

**Why This Matters**:
- In doubles/triples, each Pokemon can have different control
- Allows fine-grained control over battle participants
- Supports complex scenarios (e.g., player + AI teammate vs 2 AI enemies)

**Example**:
```csharp
// Doubles battle setup
var playerSlot0 = Field.PlayerSide.GetSlot(0);
var playerSlot1 = Field.PlayerSide.GetSlot(1);

playerSlot0.ActionProvider = new PlayerInputProvider(_view); // Player controls
playerSlot1.ActionProvider = new RandomAI(); // AI teammate

var enemySlot0 = Field.EnemySide.GetSlot(0);
var enemySlot1 = Field.EnemySide.GetSlot(1);

enemySlot0.ActionProvider = new AlwaysAttackAI();
enemySlot1.ActionProvider = new AlwaysAttackAI();
```

## 8. AI Implementations (Phase 2.7)

### 8.1 `RandomAI`
**Purpose**: Simple AI that picks random valid moves. Used for testing and basic enemies.

**Behavior**:
1. Get all moves with PP > 0
2. Pick random move
3. Get valid targets for that move
4. Pick random target (or use self if Self-targeting)
5. Return `UseMoveAction`

**Use Cases**:
- Testing battle mechanics
- Basic wild Pokemon behavior
- Autoplay when player wants random decisions

### 8.2 `AlwaysAttackAI`
**Purpose**: Always uses first available move. Simplest possible AI.

**Behavior**:
1. Get first move with PP > 0
2. Get valid targets
3. Pick first target (or self if Self-targeting)
4. Return `UseMoveAction`

**Use Cases**:
- Testing predictable behavior
- Simplest enemy AI
- Debugging specific scenarios

### 8.3 Future: `SmartAI` (Phase 3+)
**Purpose**: Strategic AI that evaluates moves based on type effectiveness, HP, status, etc.

**Behavior** (Planned):
1. **Kill Check**: If any move can KO target → use it
2. **Type Matchup**: Prioritize super-effective moves
3. **Status**: Apply status if target doesn't have one
4. **Healing**: Use healing moves if HP < 30%
5. **Stat Boosts**: Use stat-boosting moves if safe

**Use Cases**:
- Trainer battles
- Autoplay with strategy
- AI tournaments

## 9. Integration with Action Queue

The flow remains perfectly consistent with our "Everything is an Action" rule.

1.  **PhaseActionSelection**:
    ```csharp
    foreach (var slot in AllActiveSlots) {
        // Engine doesn't know if this is Player or AI
        var action = await slot.ActionProvider.GetAction(field, slot);
        _queue.Enqueue(action);
    }
    ```
2.  **PhaseExecution**:
    The Queue processes the actions. The `UseMoveAction` generated by the AI is identical to the one generated by the Player.

## 10. Testability

-   **Test AI**: Create a `BattleField`, run `RandomAI.GetAction`, assert it returns a valid `UseMoveAction`.
-   **Test Player Flow**: Mock `IBattleView` to return a specific input immediately, assert `PlayerInputProvider` returns the correct Action.
-   **Test Autoplay**: Initialize with player provider, swap to AI, verify AI controls next turn.
-   **Test Mixed Control**: Set different providers per slot, verify each slot uses correct provider.
-   **Test AI vs AI**: Initialize both sides with AI, run full battle, verify completion.

## 11. Architecture Benefits

### 11.1 Decoupling
- `CombatEngine` doesn't know or care who makes decisions
- Same battle loop works for player, AI, or mixed control
- Easy to add new AI strategies without changing engine

### 11.2 Flexibility
- Can swap providers at runtime
- Each slot independently controlled
- Supports any combination of player/AI control

### 11.3 Testability
- Easy to test AI logic in isolation
- Can test battle mechanics without player input
- Can simulate any control scenario

### 11.4 Extensibility
- New AI strategies: implement `IActionProvider`
- New input methods: implement `IActionProvider`
- Future: Network multiplayer (remote `IActionProvider`)

---

## 12. Player Input API Specification

### 12.1 `IBattleView` Input Methods

**Required Methods**:

```csharp
/// <summary>
/// Prompts the player to select an action type (Fight/Switch/Item/Run).
/// </summary>
/// <param name="slot">The slot requesting input.</param>
/// <returns>The selected action type.</returns>
Task<BattleActionType> SelectActionType(BattleSlot slot);

/// <summary>
/// Prompts the player to select a move from available moves.
/// </summary>
/// <param name="moves">List of available moves (with PP > 0).</param>
/// <returns>The selected move instance, or null if cancelled.</returns>
Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves);

/// <summary>
/// Prompts the player to select a target slot.
/// </summary>
/// <param name="validTargets">List of valid target slots.</param>
/// <returns>The selected target slot, or null if cancelled.</returns>
Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets);

/// <summary>
/// Prompts the player to select a Pokemon to switch in.
/// </summary>
/// <param name="availablePokemon">List of available Pokemon (not fainted, not active).</param>
/// <returns>The selected Pokemon instance, or null if cancelled.</returns>
Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon);
```

### 12.2 `BattleActionType` Enum

**Location**: `PokemonUltimate.Combat/Actions/BattleActionType.cs`

```csharp
/// <summary>
/// Types of actions a player can choose during battle.
/// </summary>
public enum BattleActionType
{
    /// <summary>
    /// Use a move to attack or apply an effect.
    /// </summary>
    Fight,

    /// <summary>
    /// Switch to a different Pokemon.
    /// </summary>
    Switch,

    /// <summary>
    /// Use an item (Potion, Poke Ball, etc.).
    /// </summary>
    Item, // Future feature

    /// <summary>
    /// Attempt to flee from battle.
    /// </summary>
    Run // Future feature
}
```

### 12.3 Use Cases

**UC-PI-1: Player Selects Move**
- Player chooses Fight → SelectMove shows 4 moves → Player picks Thunderbolt → Auto-selects target (1v1) → Returns UseMoveAction

**UC-PI-2: Player Selects Move with Multiple Targets**
- Player chooses Fight → SelectMove → Player picks Earthquake → SelectTarget shows 2 enemies → Player picks target → Returns UseMoveAction

**UC-PI-3: Player Switches Pokemon**
- Player chooses Switch → SelectSwitch shows available Pokemon → Player picks Bulbasaur → Returns SwitchAction

**UC-PI-4: No Moves Available**
- Player chooses Fight → No moves with PP → Returns null (should be handled by UI, but defensive)

**UC-PI-5: No Pokemon Available to Switch**
- Player chooses Switch → No available Pokemon → Returns null

**UC-PI-6: Player Cancels Selection**
- Player chooses Fight → SelectMove → Player cancels → Returns null

**UC-PI-7: Fainted Pokemon**
- Slot is fainted → GetAction returns null immediately (no input requested)

---

## Related Documents

- **[Sub-Feature README](README.md)** - Overview of Integration
- **[Parent Architecture](../../architecture.md)** - Overall combat system design
- **[Use Cases](../../use_cases.md)** - Integration scenarios
- **[Roadmap](../../roadmap.md#phase-27-integration)** - Implementation details
- **[Testing](../../testing.md)** - Testing strategy
- **[Code Location](../../code_location.md)** - Where this is implemented
- **[Sub-Feature 2.6: Combat Engine](../2.6-combat-engine/)** - Engine uses action providers
- **[Feature 4: Unity Integration](../../../4-unity-integration/architecture.md)** - Unity will implement IActionProvider

**⚠️ Always use numbered feature paths**: `../../../[N]-[feature-name]/` instead of `../../../feature-name/`

---

**Status**: ✅ Complete (Phase 2.7)  
**Last Updated**: 2025-01-XX
