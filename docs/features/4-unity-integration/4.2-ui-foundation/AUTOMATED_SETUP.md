# Automated Scene Setup - Phase 4.2

> **Automated generation of battle scene with all UI components**

**Feature**: 4: Unity Integration  
**Sub-Feature**: 4.2: UI Foundation

## Overview

Instead of manually creating the battle scene in Unity Editor, you can use the automated scene generator script to create everything programmatically.

## Quick Start

### Method 1: Unity Editor Menu (Recommended)

1. **Open Unity Editor**
2. **Go to Menu**: `PokemonUltimate → Generate Battle Scene`
3. **Wait for completion**: The script will create the entire scene automatically
4. **Scene Location**: `Assets/Scenes/BattleScene.unity`

### Method 2: Programmatic Generation

You can also call the generator from code:

```csharp
using PokemonUltimate.Unity.Editor;

// Generate battle scene programmatically
BattleSceneGenerator.GenerateBattleScene();
```

## What Gets Created

The script automatically creates:

### ✅ Canvas Setup
- **BattleCanvas** with proper scaling configuration
- Canvas Scaler: Scale With Screen Size
- Reference Resolution: 1920x1080
- Match: 0.5 (width/height)

### ✅ Player Side UI
- **PlayerSide** panel (bottom left)
- **PlayerPokemonDisplay** with:
  - Sprite Image (placeholder)
  - Name Text (TextMeshPro)
  - Level Text (TextMeshPro)
- **PlayerHPBar** with:
  - Fill Image (green)
  - HP Text (TextMeshPro)
  - All references assigned

### ✅ Enemy Side UI
- **EnemySide** panel (top right)
- **EnemyPokemonDisplay** with:
  - Sprite Image (placeholder)
  - Name Text (TextMeshPro)
  - Level Text (TextMeshPro)
- **EnemyHPBar** with:
  - Fill Image (red)
  - HP Text (TextMeshPro)
  - All references assigned

### ✅ Dialog System
- **DialogBox** (bottom center)
- **DialogText** (TextMeshPro)
- Initially disabled
- All references assigned

### ✅ Battle Setup Script
- **BattleUISetup** GameObject
- All component references automatically assigned:
  - Player HP Bar
  - Enemy HP Bar
  - Player Display
  - Enemy Display
  - Dialog

## Component Configuration

All components are automatically configured:

- **HPBar**: Fill images, text components, references
- **PokemonDisplay**: Sprite images, name/level texts, references
- **BattleDialog**: Dialog box, text component, references
- **BattleUISetup**: All UI component references

## Layout Structure

```
BattleCanvas
├── PlayerSide (Bottom Left)
│   ├── PlayerPokemonDisplay
│   │   ├── SpriteImage
│   │   ├── NameText
│   │   └── LevelText
│   └── PlayerHPBar
│       ├── Background
│       ├── Fill Area
│       │   └── Fill
│       └── HPText
│
├── EnemySide (Top Right)
│   ├── EnemyPokemonDisplay
│   │   ├── SpriteImage
│   │   ├── NameText
│   │   └── LevelText
│   └── EnemyHPBar
│       ├── Background
│       ├── Fill Area
│       │   └── Fill
│       └── HPText
│
├── DialogBox (Bottom Center)
│   └── DialogText
│
└── BattleUISetup
```

## Testing the Generated Scene

After generation:

1. **Open the scene**: `Assets/Scenes/BattleScene.unity`
2. **Select BattleUISetup** in Hierarchy
3. **Check Inspector**: All references should be assigned
4. **Press Play**: The scene should work immediately
5. **Test Context Menu**: Right-click BattleUISetup → Test Player Damage / Test Enemy Damage

## Customization

After generation, you can customize:

- **Colors**: Change HP bar colors, dialog background
- **Positions**: Adjust UI element positions
- **Sizes**: Modify panel and text sizes
- **Sprites**: Replace placeholder sprites with actual Pokemon sprites

## Troubleshooting

### Scene Not Generated
- Check Unity Console for errors
- Ensure all UI scripts are compiled without errors
- Verify DLLs are imported correctly

### References Not Assigned
- The script uses reflection to assign private fields
- If references are missing, check Unity Console
- Manually assign in Inspector if needed

### UI Elements Not Visible
- Check Canvas is set to Screen Space Overlay
- Verify UI elements are within screen bounds
- Check if DialogBox is initially disabled (expected)

## Next Steps

After scene generation:

1. ✅ **Scene Created** - Battle scene ready
2. ➡️ **Phase 4.3** - Implement IBattleView interface
3. ➡️ **Phase 4.4** - Add player input system
4. ➡️ **Phase 4.5** - Add animations

---

**Last Updated**: 2025-01-XX

