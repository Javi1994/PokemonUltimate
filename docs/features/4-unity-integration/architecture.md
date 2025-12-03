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
```csharp
public class PlaySoundAction : BattleAction {
    public string SoundId { get; private set; } // e.g., "sfx_hit_super_effective", "cry_pikachu"
    
    public override async Task ExecuteVisual(IBattleView view) {
        view.PlaySound(SoundId); // Fire and forget, usually doesn't wait
        await Task.CompletedTask;
    }
}
```

## 3. The View Interface (`IBattleView`)
*Namespace: `PokemonGame.Core.Interfaces`*

This interface defines **contracts** that the Unity View (UI + Audio) must fulfill.

```csharp
public interface IBattleView {
    // 1. Text & Dialog
    Task ShowMessage(string key, object[] args, bool waitForInput);
    
    // 2. Visual Updates (State)
    Task UpdateHPBar(BattleSlot slot, float current, float max);
    Task UpdateExpBar(BattleSlot slot, float current, float max);
    Task ShowStatusIcon(BattleSlot slot, PersistentStatus status);
    Task ShowStatChangeEffect(BattleSlot slot, Stat stat, int stages);
    
    // 3. Post-Battle UI
    Task ShowExpGain(Dictionary<PokemonInstance, int> expGains);
    Task ShowLootScreen(List<ItemData> loot);
    
    // 4. Animations (FX)
    Task PlayAnimation(string animId, BattleSlot user, BattleSlot target);
    Task PlaySendOutAnimation(BattleSlot slot);
    
    // 4. Audio
    void PlaySound(string soundId);
    void PlayMusic(string musicId, bool loop = true);
    
    // 5. Input Requests (Player Choice)
    Task<MoveInstance> SelectMove(List<MoveInstance> moves);
    Task<BattleSlot> SelectTarget(List<BattleSlot> validTargets);
    Task<BattleAction> WaitForInput(BattleSlot slot);
}
```

## 4. Implementation in Unity (The "Adapter")
*Namespace: `PokemonGame.Unity.Views`*

The Unity implementation (`BattleView`) handles the specifics of TextMeshPro, DOTween, etc.

```csharp
public class BattleView : MonoBehaviour, IBattleView {
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private GameObject _dialogBox;
    
    public async Task ShowMessage(string key, object[] args, bool waitForInput) {
        _dialogBox.SetActive(true);
        
        // 1. Localize
        string finalText = LocalizationManager.Get(key, args);
        
        // 2. Typewriter Effect
        _dialogText.text = "";
        foreach (char c in finalText) {
            _dialogText.text += c;
            await Task.Delay(30); // 30ms per char
        }
        
        // 3. Wait
        if (waitForInput) {
            await WaitForKeyPress();
        } else {
            await Task.Delay(1000); // Auto-advance
        }
    }
}
```

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
public class MockBattleView : IBattleView {
    public List<string> MessagesShown = new List<string>();

    public Task ShowMessage(string key, object[] args, bool wait) {
        MessagesShown.Add(key); // Record for assertion
        return Task.CompletedTask;
    }
    // ...
}

[Test]
public void Test_Attack_Shows_Message() {
    var view = new MockBattleView();
    var engine = new CombatEngine(field, view);
    
    // Run turn...
    
    Assert.Contains("battle_used_move", view.MessagesShown);
}
```

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

**Last Updated**: 2025-01-XX
```
