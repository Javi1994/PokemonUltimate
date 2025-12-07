# Feature 7: Game Implementation - Code Location

> Where the game implementation code lives and how it's organized.

**Feature**: 7: Game Implementation  
**See**: [`README.md`](README.md) for feature overview.

## Overview

This document describes where the game implementation code is located and how it's organized within the project structure.

## Project Structure

### Main Project

**Project**: `PokemonUltimate.Game`  
**Location**: `PokemonUltimate.Game/`

```
PokemonUltimate.Game/
├── Core/
│   ├── GameState.cs
│   ├── GameLoop.cs
│   ├── GameSession.cs
│   └── IGameState.cs
├── World/
│   ├── WorldMap.cs
│   ├── Location.cs
│   ├── EncounterTable.cs
│   └── Trainer.cs
├── Player/
│   ├── Player.cs
│   ├── Party.cs
│   ├── PC.cs
│   └── Inventory.cs
├── Encounters/
│   ├── EncounterSystem.cs
│   ├── WildEncounter.cs
│   ├── TrainerEncounter.cs
│   └── BossEncounter.cs
├── Progression/
│   ├── ExpSystem.cs
│   ├── LevelUpSystem.cs
│   ├── RewardSystem.cs
│   └── RoguelikeSystem.cs
├── UI/
│   ├── IGameView.cs
│   ├── ConsoleGameView.cs
│   ├── MenuSystem.cs
│   ├── BattleDisplay.cs
│   ├── PartyDisplay.cs
│   └── InventoryDisplay.cs
├── Managers/
│   ├── GameManager.cs
│   ├── SaveManager.cs
│   └── EventManager.cs
└── Program.cs
```

---

## Code Organization

### Core

**Location**: `PokemonUltimate.Game/Core/`

**Purpose**: Core game loop and state management

**Classes**:
- `GameState.cs` - Main game state management
- `GameLoop.cs` - Core game loop
- `GameSession.cs` - Game session data
- `IGameState.cs` - Game state interface

---

### World

**Location**: `PokemonUltimate.Game/World/`

**Purpose**: World map and location system

**Classes**:
- `WorldMap.cs` - World map representation
- `Location.cs` - Location/area definition
- `EncounterTable.cs` - Wild encounter tables
- `Trainer.cs` - Trainer definitions

---

### Player

**Location**: `PokemonUltimate.Game/Player/`

**Purpose**: Player state and Pokemon management

**Classes**:
- `Player.cs` - Player state
- `Party.cs` - Player's Pokemon party
- `PC.cs` - Pokemon storage system
- `Inventory.cs` - Item inventory

---

### Encounters

**Location**: `PokemonUltimate.Game/Encounters/`

**Purpose**: Encounter system and types

**Classes**:
- `EncounterSystem.cs` - Encounter management
- `WildEncounter.cs` - Wild Pokemon encounters
- `TrainerEncounter.cs` - Trainer battles
- `BossEncounter.cs` - Boss battles

---

### Progression

**Location**: `PokemonUltimate.Game/Progression/`

**Purpose**: EXP, level ups, rewards, roguelike

**Classes**:
- `ExpSystem.cs` - EXP calculation and distribution
- `LevelUpSystem.cs` - Level up processing
- `RewardSystem.cs` - Post-battle rewards
- `RoguelikeSystem.cs` - Roguelike progression

---

### UI

**Location**: `PokemonUltimate.Game/UI/`

**Purpose**: User interface implementations

**Classes**:
- `IGameView.cs` - Game view interface
- `ConsoleGameView.cs` - Console implementation
- `MenuSystem.cs` - Menu navigation
- `BattleDisplay.cs` - Battle UI display
- `PartyDisplay.cs` - Party display
- `InventoryDisplay.cs` - Inventory display

---

### Managers

**Location**: `PokemonUltimate.Game/Managers/`

**Purpose**: High-level game management

**Classes**:
- `GameManager.cs` - Main game manager
- `SaveManager.cs` - Save/load system
- `EventManager.cs` - Game events

---

## Dependencies

### Internal Dependencies

- **PokemonUltimate.Core** - Game data (Pokemon, Moves, Items, Abilities)
- **PokemonUltimate.Combat** - Battle system
- **PokemonUltimate.Content** - Game content catalogs

### External Dependencies

- **.NET Standard/Core** - Base framework
- **System.IO** - File I/O for save/load
- **System.Text.Json** - JSON serialization (if used)

---

## Entry Point

**File**: `PokemonUltimate.Game/Program.cs`

**Purpose**: Application entry point

**Structure**:
```csharp
class Program
{
    static void Main(string[] args)
    {
        var gameManager = new GameManager();
        gameManager.Run();
    }
}
```

---

## Test Location

**Project**: `PokemonUltimate.Tests`  
**Location**: `PokemonUltimate.Tests/Systems/Game/`

```
Tests/
└── Systems/
    └── Game/
        ├── Core/
        ├── Player/
        ├── World/
        ├── Encounters/
        ├── Progression/
        ├── UI/
        └── Integration/
```

See [testing.md](testing.md) for test organization details.

---

## File Naming Conventions

### Classes

- **PascalCase**: `GameState.cs`, `Party.cs`, `WorldMap.cs`
- **One class per file**: Each class in its own file
- **Interface prefix**: `I` prefix for interfaces (`IGameView.cs`)

### Namespaces

- **Base namespace**: `PokemonUltimate.Game`
- **Sub-namespaces**: `PokemonUltimate.Game.Core`, `PokemonUltimate.Game.World`, etc.

---

## Code References

### Feature References

All classes must include feature references in XML documentation:

```csharp
/// <summary>
/// Main game state management.
/// </summary>
/// <remarks>
/// **Feature**: 7: Game Implementation
/// **Sub-Feature**: 7.1: Game Loop Foundation
/// **Documentation**: See `docs/features/7-game-implementation/7.1-game-loop-foundation/architecture.md`
/// </remarks>
public class GameState
{
    // ...
}
```

---

## Related Code Locations

### Feature 1: Game Data

- **PokemonUltimate.Core** - Game data structures
- **PokemonUltimate.Content** - Game content catalogs

### Feature 2: Combat System

- **PokemonUltimate.Combat** - Battle engine

### Feature 5: Game Features

- **PokemonUltimate.Game** - Game features implementation (this project)

---

**Last Updated**: January 2025

