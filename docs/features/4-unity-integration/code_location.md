# Feature 4: Unity Integration - Code Location

> Where the Unity integration code will live and how it's organized.

**Feature Number**: 4  
**Feature Name**: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Unity integration code will be organized in a separate Unity project:
- **Unity Project** - Visual presentation, input, audio
- **DLLs** - Battle engine DLLs imported as plugins
- **Unity Scripts** - C# scripts implementing Unity-specific functionality

**Note**: Basic implementation complete (Phases 4.1-4.3). Code structure matches planned design.

## Project Structure

```
Unity Project/
├── Assets/
│   ├── Plugins/                        # Battle engine DLLs
│   │   ├── PokemonUltimate.Core.dll
│   │   ├── PokemonUltimate.Combat.dll
│   │   └── PokemonUltimate.Content.dll
│   │
│   ├── Scripts/                        # Unity C# scripts
│   │   ├── Battle/
│   │   │   ├── UnityBattleView.cs      # IBattleView implementation ✅
│   │   │   ├── BattleManager.cs        # Battle orchestration ✅
│   │   │   ├── UnityBattleLogger.cs    # Unity logger implementation ✅
│   │   │   └── BattleInputHandler.cs   # Input handling ⏳ Planned
│   │   │
│   │   ├── UI/
│   │   │   ├── HPBar.cs                # HP bar component ✅
│   │   │   ├── PokemonDisplay.cs        # Pokemon sprite/name display ✅
│   │   │   ├── BattleDialog.cs         # Dialog system ✅
│   │   │   ├── BattleUISetup.cs        # UI setup helper ✅
│   │   │   ├── ActionMenu.cs           # Action selection menu ⏳ Planned
│   │   │   ├── MoveMenu.cs             # Move selection menu ⏳ Planned
│   │   │   └── PostBattleUI.cs         # Post-battle UI ⏳ Planned
│   │   │
│   │   ├── Editor/
│   │   │   └── BattleSceneGenerator.cs  # Automated scene generator ✅
│   │   │
│   │   ├── Animation/
│   │   │   ├── BattleAnimator.cs       # Animation controller
│   │   │   └── MoveAnimationPlayer.cs  # Move animation player
│   │   │
│   │   ├── Audio/
│   │   │   ├── AudioManager.cs         # Audio manager
│   │   │   └── BattleAudioController.cs # Battle audio controller
│   │   │
│   │   └── Utils/
│   │       ├── LocalizationManager.cs   # Localization system
│   │       └── SceneTransition.cs      # Scene transition handler
│   │
│   ├── Scenes/
│   │   ├── Battle.unity                 # Battle scene
│   │   └── PostBattle.unity            # Post-battle scene
│   │
│   ├── Sprites/
│   │   ├── Pokemon/                     # Pokemon sprites
│   │   └── UI/                          # UI sprites
│   │
│   ├── Animations/
│   │   ├── Moves/                       # Move animations
│   │   └── Effects/                     # Effect animations
│   │
│   └── Audio/
│       ├── SFX/                         # Sound effects
│       └── Music/                       # Battle music
│
└── (No Tests/)                          # No Unity tests - all tests in C# project
    ├── EditMode/
    │   ├── DLLs_LoadWithoutErrors.cs
    │   └── Namespaces_AreAccessible.cs
    └── PlayMode/
        ├── UnityBattleView/
        └── BattleInputHandler/
```

## Key Classes (Planned)

### UnityBattleView
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/Battle/UnityBattleView.cs` ✅ **Implemented**
**Purpose**: Implements `IBattleView` interface for Unity
**Key Methods**:
- `ShowMessage(string)` - Display battle messages ✅
- `UpdateHPBar(BattleSlot)` - Update HP bar ✅
- `PlaySwitchInAnimation(BattleSlot)` - Update Pokemon display ✅
- `SelectActionType(BattleSlot)` - Player input (placeholder) ⏳
- `SelectMove(IReadOnlyList<MoveInstance>)` - Player input (placeholder) ⏳
- `SelectTarget(IReadOnlyList<BattleSlot>)` - Player input (placeholder) ⏳
- `SelectSwitch(IReadOnlyList<PokemonInstance>)` - Player input (placeholder) ⏳

### BattleManager
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/Battle/BattleManager.cs` ✅ **Implemented**
**Purpose**: Orchestrates battle in Unity
**Key Methods**:
- `StartBattle(IReadOnlyList<PokemonInstance>, IReadOnlyList<PokemonInstance>)` - Initialize battle ✅
- `CreateCombatEngine()` - Create engine with dependencies ✅
- `BindSlotsToUI()` - Connect BattleSlots to UI components ✅
- `HandleBattleEnd(BattleResult)` - Handle battle conclusion ✅

### PokemonDisplay
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/UI/PokemonDisplay.cs` ✅ **Implemented**
**Purpose**: Displays Pokemon sprite, name, and level
**Key Methods**:
- `Display(PokemonInstance)` - Display Pokemon data ✅

### BattleUISetup
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/UI/BattleUISetup.cs` ✅ **Implemented**
**Purpose**: Helper script for setting up and testing UI components
**Key Methods**:
- `SetupTestBattle()` - Create test Pokemon and display them ✅
- `UpdateHPBars()` - Update HP bars with current values ✅

### BattleSceneGenerator
**Namespace**: (Global namespace - Unity Editor scripts)
**File**: `Assets/Scripts/Editor/BattleSceneGenerator.cs` ✅ **Implemented**
**Purpose**: Automated editor script to generate battle scene with all UI components
**Key Methods**:
- `GenerateBattleScene()` - Create complete battle scene ✅

### BattleInputHandler
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/Battle/BattleInputHandler.cs` ⏳ **Planned**
**Purpose**: Handles player input and converts to actions
**Key Methods**:
- `OnActionSelected(ActionType)` - Handle action selection ⏳
- `OnMoveSelected(MoveData)` - Handle move selection ⏳
- `OnPokemonSelected(PokemonInstance)` - Handle Pokemon selection ⏳

### HPBar
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/UI/HPBar.cs` ✅ **Implemented**
**Purpose**: Displays Pokemon HP bar
**Key Methods**:
- `UpdateHP(int current, int max)` - Update HP display ✅

### BattleDialog
**Namespace**: (Global namespace - Unity scripts)
**File**: `Assets/Scripts/UI/BattleDialog.cs` ✅ **Implemented**
**Purpose**: Displays battle messages
**Key Methods**:
- `ShowMessage(string, bool)` - Display message with typewriter effect ✅
- `Hide()` - Hide dialog box ✅

## DLL Integration

### DLL Location
**Path**: `Assets/Plugins/PokemonUltimate.*.dll`
**Source**: Built from `PokemonUltimate.sln`
**Build Command**: `dotnet build -c Release`

### DLLs Required
- `PokemonUltimate.Core.dll` - Core game logic
- `PokemonUltimate.Combat.dll` - Battle engine
- `PokemonUltimate.Content.dll` - Game content

### Import Configuration
- **API Compatibility**: .NET Standard 2.1
- **Platform**: Any Platform
- **Auto Reference**: Enabled

## Test Location (Planned)

**Tests**: No Unity tests - all tests in `PokemonUltimate.Tests`
- **Manual Testing**: Test UI components visually in Unity Editor
- **C# Tests**: Test battle engine logic in C# project
- **Smoke Tests**: Use `PokemonUltimate.SmokeTests` for integration validation

**Test Categories**:
- DLL integration tests
- UI component tests
- IBattleView implementation tests
- Input handling tests
- Animation tests
- Audio tests

## Related Documents

- **[Feature README](README.md)** - Overview of Unity Integration
- **[Architecture](architecture.md)** - Technical design of Unity integration
- **[Use Cases](use_cases.md)** - Scenarios for Unity integration
- **[Roadmap](roadmap.md)** - Implementation phases
- **[Testing](testing.md)** - Testing strategy for Unity code
- **[Feature 2: Combat System](../2-combat-system/code_location.md)** - Battle engine code location
- **[Feature 2.7: Integration](../2-combat-system/2.7-integration/architecture.md)** - IActionProvider and IBattleView interfaces

---

**Last Updated**: 2025-01-XX (Basic Implementation Complete - Phases 4.1-4.3)

