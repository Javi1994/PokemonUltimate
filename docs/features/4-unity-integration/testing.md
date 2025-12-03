# Feature 4: Unity Integration - Testing

> How to test Unity integration.

**Feature Number**: 4  
**Feature Name**: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Unity integration testing focuses on:
- **DLL Integration** - Verify DLLs load and namespaces are accessible
- **UI Components** - Test UI components work correctly
- **IBattleView Implementation** - Verify interface implementation
- **Input Handling** - Test player input conversion
- **Animations & Audio** - Test visual and audio systems

**Status**: ⏳ Planned (no tests yet, structure defined)

## Test Structure

Unity uses Unity Test Framework (UTF) with two test modes:

```
Unity Project/
└── Tests/
    ├── EditMode/                       # Editor tests (no runtime)
    │   ├── DLLs_LoadWithoutErrors.cs
    │   ├── Namespaces_AreAccessible.cs
    │   └── CoreTypes_CanBeInstantiated.cs
    │
    └── PlayMode/                       # Runtime tests
        ├── UnityBattleView/
        │   ├── UnityBattleView_ShowsMessages.cs
        │   ├── UnityBattleView_UpdatesHPBars.cs
        │   └── UnityBattleView_ShowsStatusIcons.cs
        │
        ├── UI/
        │   ├── HPBar_UpdatesCorrectly.cs
        │   ├── PokemonDisplay_ShowsCorrectData.cs
        │   └── Dialog_DisplaysText.cs
        │
        ├── Input/
        │   ├── BattleInputHandler_ConvertsInputToActions.cs
        │   └── ActionMenu_HandlesSelection.cs
        │
        ├── Animation/
        │   └── MoveAnimationPlayer_PlaysAnimations.cs
        │
        └── Audio/
            └── BattleAudioController_PlaysSounds.cs
```

## Test Types

### EditMode Tests
**Location**: `Tests/EditMode/`
**Purpose**: Test DLL integration and static code
**When to Run**: In Unity Editor, no play mode required

**Examples**:
- DLL loading without errors
- Namespace accessibility
- Type instantiation
- Static method calls

### PlayMode Tests
**Location**: `Tests/PlayMode/`
**Purpose**: Test runtime behavior and UI interactions
**When to Run**: In Unity Editor play mode or CI/CD

**Examples**:
- UI component updates
- Input handling
- Animation playback
- Audio playback

## Coverage by Phase

### Phase 4.1: Unity Project Setup
**Tests**:
- ✅ `DLLs_LoadWithoutErrors` - Verify DLLs import
- ✅ `Namespaces_AreAccessible` - Verify namespaces accessible
- ✅ `CoreTypes_CanBeInstantiated` - Verify types can be created

### Phase 4.2: UI Foundation
**Tests**:
- ⏳ `HPBar_UpdatesCorrectly` - Test HP bar updates
- ⏳ `PokemonDisplay_ShowsCorrectData` - Test Pokemon display
- ⏳ `Dialog_DisplaysText` - Test dialog system
- ⏳ `Dialog_WaitsForInput` - Test dialog input waiting

### Phase 4.3: IBattleView Implementation
**Tests**:
- ⏳ `UnityBattleView_ShowsMessages` - Test message display
- ⏳ `UnityBattleView_UpdatesHPBars` - Test HP bar updates
- ⏳ `UnityBattleView_ShowsStatusIcons` - Test status display
- ⏳ `UnityBattleView_HandlesStatChanges` - Test stat change visuals
- ⏳ `Localization_TranslatesKeys` - Test localization

### Phase 4.4: Player Input System
**Tests**:
- ⏳ `BattleInputHandler_ConvertsInputToActions` - Test input conversion
- ⏳ `ActionMenu_HandlesSelection` - Test action menu
- ⏳ `MoveMenu_HandlesSelection` - Test move menu
- ⏳ `PokemonMenu_HandlesSelection` - Test Pokemon selection

### Phase 4.5: Animations
**Tests**:
- ⏳ `MoveAnimationPlayer_PlaysAnimations` - Test move animations
- ⏳ `AnimationSystem_HandlesMissingAnimations` - Test fallback

### Phase 4.6: Audio
**Tests**:
- ⏳ `BattleAudioController_PlaysSounds` - Test sound effects
- ⏳ `AudioManager_ManagesMusic` - Test music system

### Phase 4.7: Post-Battle UI
**Tests**:
- ⏳ `PostBattleUI_ShowsResults` - Test results display
- ⏳ `PostBattleUI_ShowsRewards` - Test rewards display

### Phase 4.8: Transitions
**Tests**:
- ⏳ `SceneTransition_HandlesBattleStart` - Test battle start transition
- ⏳ `SceneTransition_HandlesBattleEnd` - Test battle end transition

## Coverage Requirements

### ✅ Planned Coverage
- [ ] DLL integration tests (Phase 4.1)
- [ ] UI component tests (Phase 4.2)
- [ ] IBattleView implementation tests (Phase 4.3)
- [ ] Input handling tests (Phase 4.4)
- [ ] Animation tests (Phase 4.5)
- [ ] Audio tests (Phase 4.6)
- [ ] Post-battle UI tests (Phase 4.7)
- [ ] Transition tests (Phase 4.8)

## Test Examples

### EditMode Test Example
```csharp
[TestFixture]
public class DLLs_LoadWithoutErrors
{
    [Test]
    public void CoreDLL_LoadsWithoutErrors()
    {
        // Arrange & Act
        var type = typeof(PokemonUltimate.Core.Blueprints.PokemonSpeciesData);
        
        // Assert
        Assert.That(type, Is.Not.Null);
    }
    
    [Test]
    public void CombatDLL_LoadsWithoutErrors()
    {
        // Arrange & Act
        var type = typeof(PokemonUltimate.Combat.CombatEngine);
        
        // Assert
        Assert.That(type, Is.Not.Null);
    }
}
```

### PlayMode Test Example
```csharp
[TestFixture]
public class UnityBattleView_ShowsMessages
{
    [UnityTest]
    public IEnumerator ShowMessage_DisplaysText()
    {
        // Arrange
        var view = new GameObject().AddComponent<UnityBattleView>();
        var dialog = new GameObject().AddComponent<BattleDialog>();
        view.Dialog = dialog;
        
        // Act
        view.ShowMessage("Test message");
        yield return null; // Wait one frame
        
        // Assert
        Assert.That(dialog.Text, Is.EqualTo("Test message"));
    }
}
```

## Test Execution

### Run All Tests
```bash
# In Unity Editor
Window > General > Test Runner
# Click "Run All"
```

### Run EditMode Tests Only
```bash
# In Unity Editor Test Runner
# Select "EditMode" tab
# Click "Run All"
```

### Run PlayMode Tests Only
```bash
# In Unity Editor Test Runner
# Select "PlayMode" tab
# Click "Run All"
```

### Run Specific Test
```bash
# In Unity Editor Test Runner
# Select test
# Click "Run Selected"
```

## CI/CD Integration

Unity tests can be run in CI/CD using:
- Unity Test Runner (command line)
- Unity Cloud Build
- GitHub Actions / GitLab CI

**Example CI Command**:
```bash
Unity -runTests -batchmode -projectPath . -testResults TestResults.xml
```

## Related Documents

- **[Feature README](README.md)** - Overview of Unity Integration
- **[Architecture](architecture.md)** - What to test
- **[Use Cases](use_cases.md)** - Scenarios to verify
- **[Roadmap](roadmap.md)** - Implementation phases
- **[Code Location](code_location.md)** - Where tests will be located
- **[Feature 2: Combat System](../2-combat-system/testing.md)** - Battle engine testing strategy

---

**Last Updated**: 2025-01-XX

