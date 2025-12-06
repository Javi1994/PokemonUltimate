# Feature 4: Unity Integration - Roadmap

> Step-by-step guide for integrating the Pokemon battle engine with Unity.

**Feature Number**: 4  
**Feature Name**: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This roadmap outlines the phases for systematically integrating the Pokemon Ultimate battle engine with Unity, creating a fully playable visual battle experience.

**Current Status**: Engine is complete and tested. Unity integration pending.

**Dependencies**: 
- ✅ Combat System (Phases 2.1-2.11 complete)
- ✅ Content System (Pokemon, Moves, Abilities, Items)
- ⏳ Unity Project Setup

---

## Phase 4.1: Unity Project Setup & DLL Integration

**Goal**: Set up Unity project and integrate the battle engine DLLs.

**Depends on**: Combat System complete, DLLs built.

### Components to Implement

- Unity project structure
- DLL import configuration
- Basic namespace setup
- Build automation script

### Specifications

- **Unity Version**: Unity 6 (or Unity 2021.3+)
- **Project Template**: 2D (URP) recommended
- **DLL Location**: `Assets/Plugins/PokemonUltimate.*.dll`
- **Build Script**: Automated DLL copy script for development workflow

### Workflow

1. **Create Unity Project**
   - Use 2D (URP) template
   - Configure project settings

2. **Build DLLs**
   ```powershell
   dotnet build -c Release
   ```

3. **Copy DLLs to Unity**
   - Create `Assets/Plugins/` folder
   - Copy `PokemonUltimate.Core.dll`
   - Copy `PokemonUltimate.Combat.dll`
   - Copy `PokemonUltimate.Content.dll`

4. **Verify Import**
   - Check DLLs appear without errors
   - Test namespace imports in C# scripts

5. **Create Build Script** (Optional but recommended)
   - Script to automate DLL rebuild and copy
   - Can be triggered from Unity or command line

### Testing Strategy

**Note**: Unity tests are not used. All tests are in C# project.

- Manual testing in Unity Editor
- C# unit tests in `PokemonUltimate.Tests`
- Smoke tests in `PokemonUltimate.SmokeTests`

### Completion Checklist

- [x] Unity project created and configured ✅
- [x] All DLLs imported successfully ✅
- [x] No import errors in Unity console ✅
- [x] Can create `PokemonInstance` from Unity script ✅
- [x] Build script created ✅
- [x] Basic smoke test passes (create Pokemon, log stats) ✅

**Estimated Effort**: 2-4 hours
**Estimated Tests**: Manual testing + C# tests

---

## Phase 4.2: Basic UI Foundation

**Goal**: Create core UI elements for battle display (HP bars, Pokemon sprites, text).

**Depends on**: Phase 4.1 (DLL Integration).

### Components to Implement

- Battle scene setup
- HP bar UI component
- Pokemon sprite display
- Text/dialog system
- Basic UI layout

### Specifications

- **Scene Structure**: Battle scene with UI canvas
- **HP Bar**: Visual representation of current/max HP
- **Pokemon Display**: Sprite renderer for Pokemon images
- **Text System**: Dialog box for battle messages
- **Layout**: Player side (bottom), Enemy side (top)

### Workflow

1. **Create Battle Scene**
   - New scene: `Scenes/BattleScene.unity`
   - Add Canvas (Screen Space - Overlay)
   - Configure UI scaling

2. **Create HP Bar Component**
   ```csharp
   public class HPBar : MonoBehaviour
   {
       [SerializeField] private Image fillImage;
       [SerializeField] private TextMeshProUGUI hpText;
       
       public void UpdateHP(int current, int max)
       {
           fillImage.fillAmount = (float)current / max;
           hpText.text = $"{current}/{max}";
       }
   }
   ```

3. **Create Pokemon Display Component**
   ```csharp
   public class PokemonDisplay : MonoBehaviour
   {
       [SerializeField] private Image spriteRenderer;
       [SerializeField] private TextMeshProUGUI nameText;
       [SerializeField] private TextMeshProUGUI levelText;
       
       public void Display(PokemonInstance pokemon)
       {
           // Load sprite, set name/level
       }
   }
   ```

4. **Create Dialog System**
   ```csharp
   public class BattleDialog : MonoBehaviour
   {
       [SerializeField] private TextMeshProUGUI dialogText;
       [SerializeField] private GameObject dialogBox;
       
       public async Task ShowMessage(string text, bool waitForInput = true)
       {
           // Display text with typewriter effect
           // Wait for input if needed
       }
   }
   ```

5. **Layout UI Elements**
   - Player Pokemon (bottom left)
   - Enemy Pokemon (top right)
   - HP bars for both
   - Dialog box (bottom center)

### Testing Strategy

**Note**: Unity tests are not used. UI components are tested manually in Unity Editor.

### Completion Checklist

- [x] Battle scene created (automated generator available) ✅
- [x] HP bar component implemented ✅
- [x] Pokemon display component implemented ✅
- [x] Dialog system implemented ✅
- [x] UI layout complete (automated generation script) ✅
- [x] Can display Pokemon data visually ✅
- [x] Manual testing completed ✅

**Estimated Effort**: 8-12 hours
**Estimated Tests**: Manual testing in Unity Editor

---

## Phase 4.3: IBattleView Implementation

**Goal**: Implement `IBattleView` interface to connect engine with Unity UI.

**Depends on**: Phase 4.2 (Basic UI Foundation).

### Components to Implement

- `UnityBattleView` class implementing `IBattleView`
- Connection between engine and UI components
- Message localization system
- State update handlers

### Specifications

- **Interface**: Full `IBattleView` implementation
- **Localization**: Message key → translated text system
- **Async Support**: Proper async/await for UI operations
- **State Binding**: Connect `BattleSlot` data to UI components

### Workflow

1. **Create UnityBattleView Class**
   ```csharp
   public class UnityBattleView : MonoBehaviour, IBattleView
   {
       [SerializeField] private BattleDialog dialog;
       [SerializeField] private HPBar playerHPBar;
       [SerializeField] private HPBar enemyHPBar;
       [SerializeField] private PokemonDisplay playerDisplay;
       [SerializeField] private PokemonDisplay enemyDisplay;
       
       // Implement all IBattleView methods
   }
   ```

2. **Implement Core Methods**
   - `ShowMessage(string message)` - Use dialog system (message is pre-formatted)
   - `UpdateHPBar(BattleSlot slot)` - Update HP bar component (reads HP from slot.Pokemon)
   - `PlayStatusAnimation(BattleSlot slot, string statusName)` - Display status effects
   - `ShowStatChange(BattleSlot slot, string statName, int stages)` - Visual feedback for stat changes

3. **Create Localization System** (Optional)
   ```csharp
   public static class LocalizationManager
   {
       private static Dictionary<string, string> _strings;
       
       public static string Get(string key, params object[] args)
       {
           // Load from JSON/CSV, format with args
           // Note: IBattleView.ShowMessage() receives pre-formatted strings
           // Localization can be done in Unity before calling ShowMessage()
       }
   }
   ```
   
   **Note**: `IBattleView.ShowMessage()` takes a pre-formatted string. Localization can be handled:
   - In Unity before calling `ShowMessage()`
   - Via BattleActions that format messages before creating MessageAction
   - Or Unity can implement its own localization layer

4. **Bind Battle State to UI**
   - Subscribe to `BattleSlot` events
   - Update UI when state changes
   - Handle Pokemon switching

### Testing Strategy

**Note**: Unity tests are not used. IBattleView is tested via C# tests with mocks.

### Completion Checklist

- [x] `UnityBattleView` class created ✅
- [x] All `IBattleView` methods implemented ✅
- [ ] Localization system working (optional, messages are pre-formatted)
- [x] UI updates correctly from engine state ✅
- [x] Can run a battle with visual feedback ✅
- [x] Manual testing completed ✅
- [x] `BattleManager` created for battle execution ✅

**Estimated Effort**: 10-15 hours
**Estimated Tests**: Manual testing + C# tests with mocks

---

## Phase 4.4: Player Input System

**Goal**: Implement player input handling for battle actions (move selection, switching).

**Depends on**: Phase 4.3 (IBattleView Implementation).

### Components to Implement

- Action selection menu (Fight/Switch/Item/Run)
- Move selection UI
- Target selection UI
- Switch Pokemon UI
- Input handling (keyboard/mouse/touch)

### Specifications

- **Action Menu**: Main menu with 4 options
- **Move Selection**: Grid/list of available moves with PP display
- **Target Selection**: Visual target selection for multi-target moves
- **Switch Menu**: List of available Pokemon with HP/status
- **Input Methods**: Support keyboard, mouse, and touch

### Workflow

1. **Create Action Menu UI**
   ```csharp
   public class ActionMenu : MonoBehaviour
   {
       [SerializeField] private Button fightButton;
       [SerializeField] private Button switchButton;
       [SerializeField] private Button itemButton;
       [SerializeField] private Button runButton;
       
       public async Task<BattleActionType> WaitForSelection()
       {
           // Show menu, wait for button click
       }
   }
   ```

2. **Create Move Selection UI**
   ```csharp
   public class MoveSelectionMenu : MonoBehaviour
   {
       [SerializeField] private Transform moveButtonContainer;
       [SerializeField] private GameObject moveButtonPrefab;
       
       public async Task<MoveInstance> WaitForMoveSelection(IReadOnlyList<MoveInstance> moves)
       {
           // Create buttons for each move, wait for selection
       }
   }
   ```

3. **Create Target Selection System**
   ```csharp
   public class TargetSelector : MonoBehaviour
   {
       public async Task<BattleSlot> WaitForTargetSelection(IReadOnlyList<BattleSlot> validTargets)
       {
           // Highlight valid targets, wait for click
       }
   }
   ```

4. **Create Switch Pokemon UI**
   ```csharp
   public class SwitchMenu : MonoBehaviour
   {
       public async Task<PokemonInstance> WaitForSwitchSelection(IReadOnlyList<PokemonInstance> available)
       {
           // Show party, wait for selection
       }
   }
   ```

5. **Integrate with IBattleView**
   - Implement `SelectActionType(BattleSlot slot)` - Returns `Task<BattleActionType>`
   - Implement `SelectMove(IReadOnlyList<MoveInstance> moves)` - Returns `Task<MoveInstance>`
   - Implement `SelectTarget(IReadOnlyList<BattleSlot> validTargets)` - Returns `Task<BattleSlot>`
   - Implement `SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)` - Returns `Task<PokemonInstance>`

### Tests to Write

```csharp
Tests/Unity/Input/
├── ActionMenu_ReturnsCorrectActionType
├── MoveSelection_ReturnsSelectedMove
├── TargetSelection_ReturnsSelectedTarget
├── SwitchMenu_ReturnsSelectedPokemon
└── Input_HandlesKeyboardAndMouse
```

### Completion Checklist

- [ ] Action menu implemented
- [ ] Move selection UI implemented
- [ ] Target selection system implemented
- [ ] Switch Pokemon UI implemented
- [ ] Input handling works (keyboard/mouse)
- [ ] Can play a full battle with player input
- [ ] All input tests pass

**Estimated Effort**: 12-18 hours
**Estimated Tests**: ~10-15 Unity tests

---

## Phase 4.5: Animations & Visual Effects

**Goal**: Add animations for moves, damage, status effects, and transitions.

**Depends on**: Phase 4.4 (Player Input System).

### Components to Implement

- Move animation system
- Damage visual effects
- Status effect animations
- Pokemon switching animations
- Faint animations
- Camera effects

### Specifications

- **Animation System**: Support for move-specific animations
- **Visual Effects**: Particle effects for damage, status, etc.
- **Transitions**: Smooth transitions between states
- **Camera**: Dynamic camera for move animations
- **Performance**: Optimized for 60 FPS

### Workflow

1. **Create Animation Manager**
   ```csharp
   public class AnimationManager : MonoBehaviour
   {
       private Dictionary<string, AnimationClip> _animations;
       
       public async Task PlayAnimation(string animId, BattleSlot user, BattleSlot target)
       {
           // Load and play animation
       }
   }
   ```

2. **Create Visual Effects System**
   ```csharp
   public class VFXManager : MonoBehaviour
   {
       public void PlayDamageEffect(BattleSlot target, float damage)
       {
           // Particle effect, screen shake, etc.
       }
       
       public void PlayStatusEffect(BattleSlot target, PersistentStatus status)
       {
           // Status-specific visual effect
       }
   }
   ```

3. **Implement Move Animations**
   - Create animation clips for moves
   - Map move IDs to animations
   - Play animations during `UseMoveAction`

4. **Implement Damage Effects**
   - Damage number popup
   - Screen shake on critical hits
   - Type effectiveness color coding

5. **Implement Status Animations**
   - Burn effect overlay
   - Paralysis sparkles
   - Poison purple aura
   - Freeze ice particles

6. **Implement Switching Animations**
   - Pokemon send-out animation
   - Pokemon return animation
   - Smooth transitions

### Tests to Write

```csharp
Tests/Unity/Animations/
├── AnimationManager_PlaysAnimations
├── VFXManager_ShowsDamageEffects
├── StatusEffects_DisplayCorrectly
└── Switching_AnimatesSmoothly
```

### Completion Checklist

- [ ] Animation system implemented
- [ ] Move animations play correctly
- [ ] Damage effects display
- [ ] Status effects animate
- [ ] Switching animations smooth
- [ ] Camera effects work
- [ ] Performance is acceptable (60 FPS)
- [ ] All animation tests pass

**Estimated Effort**: 20-30 hours
**Estimated Tests**: ~8-12 Unity tests

---

## Phase 4.6: Audio System

**Goal**: Add sound effects and music to battles.

**Depends on**: Phase 4.5 (Animations & Visual Effects).

### Components to Implement

- Audio manager
- Sound effect system
- Music system
- Pokemon cry system
- Move sound effects

### Specifications

- **Audio Manager**: Centralized audio playback
- **Sound Effects**: Move sounds, damage sounds, status sounds
- **Music**: Battle music, victory music, defeat music
- **Pokemon Cries**: Species-specific cry sounds
- **Volume Control**: Master, music, and SFX volume sliders

### Workflow

1. **Create Audio Manager**
   ```csharp
   public class AudioManager : MonoBehaviour
   {
       [SerializeField] private AudioSource musicSource;
       [SerializeField] private AudioSource sfxSource;
       
       public void PlaySound(string soundId)
       {
           // Load and play sound effect
       }
       
       public void PlayMusic(string musicId, bool loop = true)
       {
           // Load and play music
       }
   }
   ```

2. **Implement Sound Effect System**
   - Load sound effect assets
   - Map sound IDs to audio clips
   - Play sounds for moves, damage, status

3. **Implement Music System**
   - Battle theme music
   - Victory music
   - Defeat music
   - Music transitions

4. **Implement Pokemon Cries**
   - Load cry audio files
   - Play on Pokemon send-out
   - Play on fainting

5. **Add Volume Controls**
   - Settings menu
   - Master volume slider
   - Music volume slider
   - SFX volume slider

6. **Integrate Audio with Battle Actions**
   - Audio handled via custom BattleActions (not IBattleView methods)
   - Create `PlaySoundAction` and `PlayMusicAction` BattleActions
   - Unity implementation handles audio playback in action execution

### Tests to Write

```csharp
Tests/Unity/Audio/
├── AudioManager_PlaysSounds
├── Music_PlaysCorrectly
├── PokemonCries_PlayOnSendOut
└── VolumeControls_WorkCorrectly
```

### Completion Checklist

- [ ] Audio manager implemented
- [ ] Sound effects play for moves
- [ ] Music plays during battles
- [ ] Pokemon cries implemented
- [ ] Volume controls work
- [ ] Audio doesn't cause performance issues
- [ ] All audio tests pass

**Estimated Effort**: 8-12 hours
**Estimated Tests**: ~6-8 Unity tests

---

## Phase 4.7: Post-Battle UI

**Goal**: Implement UI for post-battle rewards (EXP, level ups, loot).

**Depends on**: Phase 4.6 (Audio System).

### Components to Implement

- EXP gain display
- Level up screen
- Evolution screen
- Loot screen
- Victory/defeat screens

### Specifications

- **EXP Display**: Animated EXP bar filling
- **Level Up**: Screen showing stat increases
- **Evolution**: Evolution animation and confirmation
- **Loot**: Item received screen
- **Victory/Defeat**: End-of-battle screens

### Workflow

1. **Create EXP Display System**
   ```csharp
   public class EXPDisplay : MonoBehaviour
   {
       public async Task ShowExpGain(Dictionary<PokemonInstance, int> expGains)
       {
           // Animate EXP bars filling
       }
   }
   ```

2. **Create Level Up Screen**
   ```csharp
   public class LevelUpScreen : MonoBehaviour
   {
       public async Task ShowLevelUp(PokemonInstance pokemon, int newLevel)
       {
           // Display stat increases, new moves learned
       }
   }
   ```

3. **Create Evolution Screen**
   ```csharp
   public class EvolutionScreen : MonoBehaviour
   {
       public async Task<bool> ShowEvolution(PokemonInstance pokemon, PokemonSpeciesData evolvedForm)
       {
           // Play evolution animation, ask for confirmation
       }
   }
   ```

4. **Create Loot Screen**
   ```csharp
   public class LootScreen : MonoBehaviour
   {
       public async Task ShowLoot(List<ItemData> loot)
       {
           // Display items received
       }
   }
   ```

5. **Create Victory/Defeat Screens**
   - Victory screen with rewards summary
   - Defeat screen with retry option

### Tests to Write

```csharp
Tests/Unity/PostBattle/
├── EXPDisplay_ShowsExpGain
├── LevelUpScreen_DisplaysCorrectly
├── EvolutionScreen_PlaysAnimation
└── LootScreen_ShowsItems
```

### Completion Checklist

- [ ] EXP display implemented
- [ ] Level up screen implemented
- [ ] Evolution screen implemented
- [ ] Loot screen implemented
- [ ] Victory/defeat screens implemented
- [ ] All post-battle flows work correctly
- [ ] All post-battle tests pass

**Estimated Effort**: 10-15 hours
**Estimated Tests**: ~8-10 Unity tests

---

## Phase 4.8: Polish & Optimization

**Goal**: Polish the battle experience and optimize performance.

**Depends on**: Phase 4.7 (Post-Battle UI).

### Components to Implement

- Performance optimization
- UI polish
- Bug fixes
- Accessibility features
- Settings menu

### Specifications

- **Performance**: Maintain 60 FPS on target hardware
- **UI Polish**: Smooth animations, proper feedback
- **Accessibility**: Colorblind support, text scaling
- **Settings**: Graphics options, audio options, controls

### Workflow

1. **Performance Optimization**
   - Profile battle performance
   - Optimize animations
   - Reduce draw calls
   - Pool objects

2. **UI Polish**
   - Add button hover effects
   - Improve transitions
   - Add loading indicators
   - Improve feedback

3. **Accessibility**
   - Colorblind-friendly colors
   - Text scaling options
   - Screen reader support (if applicable)

4. **Settings Menu**
   - Graphics quality settings
   - Audio volume controls
   - Control remapping
   - Language selection

5. **Bug Fixes**
   - Fix any discovered bugs
   - Improve error handling
   - Add logging for debugging

### Tests to Write

```csharp
Tests/Unity/Polish/
├── Performance_Maintains60FPS
├── UI_IsResponsive
├── Settings_SaveCorrectly
└── Accessibility_WorksCorrectly
```

### Completion Checklist

- [ ] Performance optimized (60 FPS)
- [ ] UI polished and responsive
- [ ] Accessibility features implemented
- [ ] Settings menu complete
- [ ] All bugs fixed
- [ ] Battle experience is smooth and enjoyable
- [ ] All polish tests pass

**Estimated Effort**: 15-25 hours
**Estimated Tests**: ~6-10 Unity tests

---

## Quality Standards

- **Performance**: 60 FPS on target hardware
- **Code Quality**: Follow Unity best practices, clean architecture
- **User Experience**: Smooth, responsive, intuitive
- **Testability**: All systems testable via Unity Test Framework
- **Maintainability**: Clean code, proper documentation

## Workflow for Unity Integration

1. **Setup**: Create Unity project, import DLLs
2. **Foundation**: Build basic UI components
3. **Integration**: Connect engine to UI via `IBattleView`
4. **Input**: Implement player input system
5. **Polish**: Add animations, audio, effects
6. **Testing**: Test all systems thoroughly
7. **Optimization**: Profile and optimize performance

## Testing Requirements

- **Unity Editor Tests**: Test UI components in isolation
- **Integration Tests**: Test full battle flow
- **Performance Tests**: Verify 60 FPS target
- **User Testing**: Get feedback on UX

---

## Priority Matrix

| Phase | Effort (Hours) | Dependencies | Priority |
|-------|----------------|--------------|----------|
| 4.1 Unity Setup | 2-4 | None | High |
| 4.2 Basic UI | 8-12 | 4.1 | High |
| 4.3 IBattleView | 10-15 | 4.2 | High |
| 4.4 Player Input | 12-18 | 4.3 | High |
| 4.5 Animations | 20-30 | 4.4 | Medium |
| 4.6 Audio | 8-12 | 4.5 | Medium |
| 4.7 Post-Battle | 10-15 | 4.6 | Medium |
| 4.8 Polish | 15-25 | 4.7 | Low |

---

## Quick Reference

### File Structure

```
Unity Project/
├── Assets/
│   ├── Plugins/
│   │   ├── PokemonUltimate.Core.dll
│   │   ├── PokemonUltimate.Combat.dll
│   │   └── PokemonUltimate.Content.dll
│   ├── Scripts/
│   │   ├── Battle/
│   │   │   ├── UnityBattleView.cs
│   │   │   ├── BattleManager.cs
│   │   │   └── ...
│   │   ├── UI/
│   │   │   ├── HPBar.cs
│   │   │   ├── ActionMenu.cs
│   │   │   └── ...
│   │   └── ...
│   └── ...
└── ...
```

## Related Documents

- **[Feature README](README.md)** - Overview of Unity Integration
- **[Architecture](architecture.md)** - Technical integration guide and UI system specification
- **[Use Cases](use_cases.md)** - All scenarios for Unity integration
- **[Testing](testing.md)** - Testing strategy for Unity integration
- **[Code Location](code_location.md)** - Where Unity code will be located
- **[Feature 2: Combat System](../2-combat-system/roadmap.md)** - Combat system roadmap
- **[Feature 2.7: Integration](../2-combat-system/2.7-integration/architecture.md)** - Input system specification (IActionProvider, IBattleView)

---

**Last Updated**: 2025-01-XX

