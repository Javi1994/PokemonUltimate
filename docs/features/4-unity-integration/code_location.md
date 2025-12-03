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

**Note**: This is planned structure. Code not yet implemented.

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
│   │   │   ├── UnityBattleView.cs      # IBattleView implementation
│   │   │   ├── BattleManager.cs        # Battle orchestration
│   │   │   └── BattleInputHandler.cs   # Input handling
│   │   │
│   │   ├── UI/
│   │   │   ├── HPBar.cs                # HP bar component
│   │   │   ├── PokemonDisplay.cs        # Pokemon sprite/name display
│   │   │   ├── BattleDialog.cs         # Dialog system
│   │   │   ├── ActionMenu.cs           # Action selection menu
│   │   │   ├── MoveMenu.cs             # Move selection menu
│   │   │   └── PostBattleUI.cs         # Post-battle UI
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
└── Tests/                               # Unity tests
    ├── EditMode/
    │   ├── DLLs_LoadWithoutErrors.cs
    │   └── Namespaces_AreAccessible.cs
    └── PlayMode/
        ├── UnityBattleView/
        └── BattleInputHandler/
```

## Key Classes (Planned)

### UnityBattleView
**Namespace**: `PokemonUltimate.Unity.Battle`
**File**: `Assets/Scripts/Battle/UnityBattleView.cs`
**Purpose**: Implements `IBattleView` interface for Unity
**Key Methods**:
- `ShowMessage(string)` - Display battle messages
- `UpdateHPBar(BattleSlot, int, int)` - Update HP bar
- `ShowStatusIcon(BattleSlot, StatusEffect)` - Display status effects
- `ShowStatChangeEffect(BattleSlot, Stat, int)` - Show stat change visuals
- `PlayAnimation(string)` - Play move animations

### BattleManager
**Namespace**: `PokemonUltimate.Unity.Battle`
**File**: `Assets/Scripts/Battle/BattleManager.cs`
**Purpose**: Orchestrates battle in Unity
**Key Methods**:
- `StartBattle(PokemonInstance[], PokemonInstance[])` - Initialize battle
- `RunBattle()` - Run battle loop
- `HandleBattleEnd(BattleResult)` - Handle battle conclusion

### BattleInputHandler
**Namespace**: `PokemonUltimate.Unity.Battle`
**File**: `Assets/Scripts/Battle/BattleInputHandler.cs`
**Purpose**: Handles player input and converts to actions
**Key Methods**:
- `OnActionSelected(ActionType)` - Handle action selection
- `OnMoveSelected(MoveData)` - Handle move selection
- `OnPokemonSelected(PokemonInstance)` - Handle Pokemon selection

### HPBar
**Namespace**: `PokemonUltimate.Unity.UI`
**File**: `Assets/Scripts/UI/HPBar.cs`
**Purpose**: Displays Pokemon HP bar
**Key Methods**:
- `UpdateHP(int current, int max)` - Update HP display
- `AnimateHPChange(int from, int to)` - Animate HP change

### BattleDialog
**Namespace**: `PokemonUltimate.Unity.UI`
**File**: `Assets/Scripts/UI/BattleDialog.cs`
**Purpose**: Displays battle messages
**Key Methods**:
- `ShowMessage(string, bool)` - Display message
- `ShowMessageAsync(string)` - Display message async

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

**Tests**: `Tests/` (Unity Test Framework)
- **EditMode**: `Tests/EditMode/` - Editor tests
- **PlayMode**: `Tests/PlayMode/` - Runtime tests

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

**Last Updated**: 2025-01-XX

