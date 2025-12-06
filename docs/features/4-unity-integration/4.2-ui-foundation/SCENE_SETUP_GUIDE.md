# Scene Setup Guide - Phase 4.2

> **Step-by-step guide for creating the battle scene with UI components**

**Feature**: 4: Unity Integration  
**Sub-Feature**: 4.2: UI Foundation

## Overview

This guide walks through creating a battle scene in Unity with all UI components properly set up.

## Prerequisites

- ✅ Phase 4.1 complete (DLLs integrated)
- ✅ UI components created (`HPBar`, `PokemonDisplay`, `BattleDialog`)
- ✅ Unity Editor open

## Step-by-Step Setup

### Step 1: Create Battle Scene

1. **Create New Scene**
   - File → New Scene
   - Choose "2D" template
   - Save as `Assets/Scenes/BattleScene.unity`

### Step 2: Create Canvas

1. **Add Canvas**
   - Right-click in Hierarchy → UI → Canvas
   - Name it "BattleCanvas"
   - Set Canvas Scaler:
     - UI Scale Mode: "Scale With Screen Size"
     - Reference Resolution: 1920x1080
     - Match: 0.5 (width/height)

### Step 3: Create Player Side UI

1. **Create Player Container**
   - Right-click Canvas → UI → Panel
   - Name: "PlayerSide"
   - Anchor: Bottom Left
   - Position: (50, 50, 0)
   - Size: (400, 200)

2. **Add Player Pokemon Display**
   - Right-click PlayerSide → Create Empty
   - Name: "PlayerPokemonDisplay"
   - Add Component: `PokemonDisplay`
   - Create child objects:
     - Image (for sprite) → Name: "SpriteImage"
     - TextMeshPro - Text → Name: "NameText"
     - TextMeshPro - Text → Name: "LevelText"
   - Assign references in Inspector:
     - Sprite Image → SpriteImage
     - Name Text → NameText
     - Level Text → LevelText

3. **Add Player HP Bar**
   - Right-click PlayerSide → UI → Slider
   - Name: "PlayerHPBar"
   - Remove Handle Slide Area
   - Configure Fill Area/Fill:
     - Set Fill color (e.g., green)
   - Add Component: `HPBar`
   - Create TextMeshPro child → Name: "HPText"
   - Assign references:
     - Fill Image → Fill (from Slider)
     - HP Text → HPText

### Step 4: Create Enemy Side UI

1. **Create Enemy Container**
   - Right-click Canvas → UI → Panel
   - Name: "EnemySide"
   - Anchor: Top Right
   - Position: (-50, -50, 0)
   - Size: (400, 200)

2. **Add Enemy Pokemon Display**
   - Same as Player Pokemon Display
   - Name: "EnemyPokemonDisplay"

3. **Add Enemy HP Bar**
   - Same as Player HP Bar
   - Name: "EnemyHPBar"

### Step 5: Create Dialog Box

1. **Create Dialog Container**
   - Right-click Canvas → UI → Panel
   - Name: "DialogBox"
   - Anchor: Bottom Center
   - Position: (0, 100, 0)
   - Size: (800, 150)
   - Initially disabled (uncheck in Inspector)

2. **Add Dialog Text**
   - Right-click DialogBox → UI → TextMeshPro - Text
   - Name: "DialogText"
   - Set font size: 24
   - Set alignment: Left, Middle
   - Set padding: (20, 20, 20, 20)

3. **Add BattleDialog Component**
   - Select DialogBox
   - Add Component: `BattleDialog`
   - Assign references:
     - Dialog Text → DialogText
     - Dialog Box → DialogBox (self)

### Step 6: Create Battle UI Setup Script

1. **Create Setup GameObject**
   - Right-click Hierarchy → Create Empty
   - Name: "BattleUISetup"
   - Add Component: `BattleUISetup`

2. **Assign References**
   - Player HP Bar → PlayerHPBar
   - Enemy HP Bar → EnemyHPBar
   - Player Display → PlayerPokemonDisplay
   - Enemy Display → EnemyPokemonDisplay
   - Dialog → DialogBox

### Step 7: Test Setup

1. **Press Play**
   - Should see Pokemon displays and HP bars
   - Dialog should show welcome message

2. **Test Context Menu**
   - Right-click BattleUISetup component
   - Select "Test Player Damage" or "Test Enemy Damage"
   - Verify HP bars update correctly

## Layout Reference

```
BattleCanvas
├── PlayerSide (Bottom Left)
│   ├── PlayerPokemonDisplay
│   │   ├── SpriteImage
│   │   ├── NameText
│   │   └── LevelText
│   └── PlayerHPBar
│       ├── Fill
│       └── HPText
│
├── EnemySide (Top Right)
│   ├── EnemyPokemonDisplay
│   │   ├── SpriteImage
│   │   ├── NameText
│   │   └── LevelText
│   └── EnemyHPBar
│       ├── Fill
│       └── HPText
│
├── DialogBox (Bottom Center)
│   └── DialogText
│
└── BattleUISetup (Script)
```

## Troubleshooting

### HP Bar Not Updating
- Verify Fill Image reference is assigned
- Check HPBar component is on the correct GameObject
- Ensure UpdateHP is being called

### Pokemon Display Not Showing
- Verify all text references are assigned
- Check PokemonInstance is not null
- Ensure Display() method is being called

### Dialog Not Appearing
- Verify DialogBox GameObject is assigned
- Check DialogText reference is assigned
- Ensure ShowMessage() is being called

## Next Steps

Once scene is set up:
- ✅ **Phase 4.2 Complete** - UI Foundation ready
- ➡️ **Phase 4.3** - Implement IBattleView interface

---

**Last Updated**: 2025-01-XX

