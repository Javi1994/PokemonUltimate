# Feature 4: Unity Integration - Architecture

> Technical specification for integrating the Pokemon battle engine with Unity.

**Feature Number**: 4  
**Feature Name**: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 1. Overview

The **Presentation System** is responsible for visualizing AND sonifying the state of the battle.
Following the **Decoupling** guideline, the Core Logic **NEVER** accesses Unity UI or AudioSources directly.
Instead, it communicates via:
1.  **BattleActions**: For sequential events (Text, Animations, Sound Effects).
2.  **IBattleView Interface**: For direct updates (HP Bars, Status Icons, Audio triggers).

## 2. Presentation Actions
*Namespace: `PokemonUltimate.Combat.Actions`*

### MessageAction (Text)
```csharp
public class MessageAction : BattleAction {
    public string TextKey { get; private set; }
    public object[] Args { get; private set; }
    public bool WaitForInput { get; set; } = true;
    
    // ... implementation ...
}
```

### PlayAnimationAction (Visuals)
```csharp
public class PlayAnimationAction : BattleAction {
    public string AnimationId { get; private set; } // e.g., "move_tackle", "anim_faint"
    public BattleSlot User { get; private set; }
    public BattleSlot Target { get; private set; }
    
    public override async Task ExecuteVisual(IBattleView view) {
        await view.PlayAnimation(AnimationId, User, Target);
    }
}
```

### PlaySoundAction (Audio)
**Note**: Audio is handled via BattleActions, not directly through IBattleView interface.

```csharp
// Example: Audio would be triggered via custom BattleAction
public class PlaySoundAction : BattleAction {
    public string SoundId { get; private set; } // e.g., "sfx_hit_super_effective", "cry_pikachu"
    
    public override async Task ExecuteVisual(IBattleView view) {
        // Unity implementation would handle audio playback
        // This is a placeholder - actual implementation in Unity project
        await Task.CompletedTask;
    }
}
```

**Audio Strategy**: Audio playback is handled by Unity implementation of BattleActions, not through IBattleView interface methods. This keeps the interface focused on visual presentation.

## 3. The View Interface (`IBattleView`)
*Namespace: `PokemonUltimate.Combat`*

This interface defines **contracts** that the Unity View (UI + Audio) must fulfill.

**Note**: This is the actual interface implementation. See `PokemonUltimate.Combat/View/IBattleView.cs` for the source.

```csharp
public interface IBattleView {
    // 1. Text & Dialog
    Task ShowMessage(string message);
    
    // 2. Visual Updates (State)
    Task UpdateHPBar(BattleSlot slot);
    Task PlayDamageAnimation(BattleSlot slot);
    Task ShowStatChange(BattleSlot slot, string statName, int stages);
    
    // 3. Animations (FX)
    Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId);
    Task PlayFaintAnimation(BattleSlot slot);
    Task PlayStatusAnimation(BattleSlot slot, string statusName);
    Task PlaySwitchOutAnimation(BattleSlot slot);
    Task PlaySwitchInAnimation(BattleSlot slot);
    
    // 4. Input Requests (Player Choice)
    Task<BattleActionType> SelectActionType(BattleSlot slot);
    Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves);
    Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets);
    Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon);
}
```

**Key Design Decisions**:
- `ShowMessage(string message)` - Takes pre-formatted message string (localization handled by Unity)
- `UpdateHPBar(BattleSlot slot)` - Takes slot object, reads HP from slot.Pokemon.CurrentHP/MaxHP
- `ShowStatChange(BattleSlot slot, string statName, int stages)` - Uses string for stat name (not enum) for flexibility
- All input methods return `Task<T>` for async UI operations
- No audio methods in interface - audio handled via BattleActions (PlaySoundAction, etc.)
- No post-battle UI methods - handled separately via Feature 5 integration

## 4. Implementation in Unity (The "Adapter")
*Namespace: `PokemonGame.Unity.Views`*

The Unity implementation (`BattleView`) handles the specifics of TextMeshPro, DOTween, etc.

```csharp
public class UnityBattleView : MonoBehaviour, IBattleView {
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private HPBar _playerHPBar;
    [SerializeField] private HPBar _enemyHPBar;
    
    public async Task ShowMessage(string message) {
        _dialogBox.SetActive(true);
        
        // Typewriter Effect
        _dialogText.text = "";
        foreach (char c in message) {
            _dialogText.text += c;
            await Task.Delay(30); // 30ms per char
        }
        
        // Wait for input
        await WaitForKeyPress();
    }
    
    public async Task UpdateHPBar(BattleSlot slot) {
        var pokemon = slot.Pokemon;
        if (slot.IsPlayer) {
            await _playerHPBar.UpdateHP(pokemon.CurrentHP, pokemon.MaxHP);
        } else {
            await _enemyHPBar.UpdateHP(pokemon.CurrentHP, pokemon.MaxHP);
        }
    }
    
    // ... other methods ...
}
```

**Note**: The actual implementation will be in the Unity project. This is a conceptual example showing how Unity would implement the interface.

## 5. Decoupling Logic: The Event System
Sometimes, UI needs to update **reactively** without an Action (e.g., HP bar changing color when low).
We use C# Events in the Data Layer, and the View subscribes.

**Rule**: The View observes the Data. The Data ignores the View.

```csharp
// In PokemonInstance
public event Action<int, int> OnHPChanged;

// In BattleSlotUI (Unity Component)
public void Bind(PokemonInstance pokemon) {
    pokemon.OnHPChanged += UpdateHPBar;
}
```

*Note: For the Action Queue, we prefer `ExecuteVisual` calls over events for synchronization. Events are better for static UI state.*

## 6. Localization Strategy
To support multiple languages, we never hardcode strings in Logic.

**Bad:**
`new MessageAction("Pikachu used Thunderbolt!")`

**Good:**
`new MessageAction("battle_used_move", user.Name, move.Name)`

The View handles the translation:
`"battle_used_move": "{0} used {1}!"` (English)
`"battle_used_move": "¡{0} usó {1}!"` (Spanish)

## 7. Integration with Combat Loop
The `CombatEngine` doesn't know about TextMeshPro. It just calls the Interface.

```csharp
// CombatEngine.cs
public async Task RunTurn() {
    // ...
    await _queue.ProcessQueue(_field, _view); // Passes the interface
}
```

## 8. Testability
We can mock `IBattleView` to test logic without Unity.

```csharp
// Example mock implementation (NullBattleView already exists in codebase)
public class MockBattleView : IBattleView {
    public List<string> MessagesShown = new List<string>();

    public Task ShowMessage(string message) {
        MessagesShown.Add(message); // Record for assertion
        return Task.CompletedTask;
    }
    
    public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;
    public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;
    public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;
    public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;
    public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;
    public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;
    public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;
    public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
    
    // Input methods return defaults
    public Task<BattleActionType> SelectActionType(BattleSlot slot) => 
        Task.FromResult(BattleActionType.Fight);
    public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) => 
        Task.FromResult(moves.FirstOrDefault());
    public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) => 
        Task.FromResult(validTargets.FirstOrDefault());
    public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) => 
        Task.FromResult(availablePokemon.FirstOrDefault());
}

[Test]
public void Test_Attack_Shows_Message() {
    var view = new MockBattleView();
    var engine = new CombatEngine(/* ... dependencies ... */);
    engine.Initialize(rules, playerParty, enemyParty, playerProvider, enemyProvider, view);
    
    // Run turn...
    
    Assert.Contains("used", view.MessagesShown[0]); // Check message content
}
```

**Note**: `NullBattleView` already exists in `PokemonUltimate.Combat/View/NullBattleView.cs` for testing purposes.

---

## Related Documents

- **[Feature README](README.md)** - Overview of Unity Integration
- **[Use Cases](use_cases.md)** - All scenarios for Unity integration
- **[Roadmap](roadmap.md)** - Integration phases (4.1-4.8)
- **[Testing](testing.md)** - Testing strategy for Unity integration
- **[Code Location](code_location.md)** - Where Unity code will be located
- **[Feature 2: Combat System](../2-combat-system/architecture.md)** - Battle engine being integrated
- **[Feature 2.5: Combat Actions](../2-combat-system/2.5-combat-actions/architecture.md)** - BattleActions used for presentation
- **[Feature 2.7: Integration](../2-combat-system/2.7-integration/architecture.md)** - IActionProvider and IBattleView interfaces

---

## Appendix: IBattleView Implementation Reference

**Actual Interface Location**: `PokemonUltimate.Combat/View/IBattleView.cs`

**Key Points**:
- Interface is in `PokemonUltimate.Combat` namespace (not `PokemonGame.Core.Interfaces`)
- `ShowMessage(string message)` - Takes pre-formatted message (no localization keys)
- `UpdateHPBar(BattleSlot slot)` - Reads HP from `slot.Pokemon.CurrentHP/MaxHP`
- `ShowStatChange(BattleSlot slot, string statName, int stages)` - Uses string for stat name
- All input methods return `Task<T>` for async operations
- No audio methods - audio handled via BattleActions
- No post-battle UI methods - handled via Feature 5 integration

**Testing**: Use `NullBattleView` from `PokemonUltimate.Combat/View/NullBattleView.cs` for testing.

---

**Last Updated**: 2025-01-XX
```
