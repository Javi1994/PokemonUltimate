# Feature 7: Game Implementation - Architecture

> Technical specification for text-based demo game implementation.

**Feature**: 7: Game Implementation  
**See**: [`README.md`](README.md) for feature overview.

## Overview

This document specifies the technical architecture for the text-based demo game implementation. The demo serves as a validation platform for all game features before Unity integration.

## Design Principles

### Core Principles

1. **Text-First Design** - Simple, text-based interface with minimal visual elements
2. **Feature Validation** - Tests all game features (1-5) in a playable format
3. **Complete Game Loop** - Full gameplay experience from start to finish
4. **Modular Architecture** - Clean separation between game systems
5. **SOLID Principles** - Follow SOLID, clean code, and code quality standards

### Visual Style

- **Primary**: Text-based interface
- **Minimal Graphics**: ASCII art, simple borders, text formatting
- **Color**: Optional console colors for readability
- **Layout**: Menu-driven, clear sections, readable text

## Architecture Overview

```
PokemonUltimate.Game/
├── Core/
│   ├── GameState.cs              # Main game state management
│   ├── GameLoop.cs               # Core game loop
│   ├── GameSession.cs            # Game session data
│   └── IGameState.cs             # Game state interface
├── World/
│   ├── WorldMap.cs               # World map representation
│   ├── Location.cs               # Location/area definition
│   ├── EncounterTable.cs         # Wild encounter tables
│   └── Trainer.cs                # Trainer definitions
├── Player/
│   ├── Player.cs                 # Player state
│   ├── Party.cs                  # Player's Pokemon party
│   ├── PC.cs                     # Pokemon storage system
│   └── Inventory.cs              # Item inventory
├── Encounters/
│   ├── EncounterSystem.cs        # Encounter management
│   ├── WildEncounter.cs          # Wild Pokemon encounters
│   ├── TrainerEncounter.cs       # Trainer battles
│   └── BossEncounter.cs          # Boss battles
├── Progression/
│   ├── ExpSystem.cs              # EXP calculation and distribution
│   ├── LevelUpSystem.cs          # Level up processing
│   ├── RewardSystem.cs           # Post-battle rewards
│   └── RoguelikeSystem.cs        # Roguelike progression
├── UI/
│   ├── IGameView.cs              # Game view interface
│   ├── ConsoleGameView.cs       # Console implementation
│   ├── MenuSystem.cs            # Menu navigation
│   ├── BattleDisplay.cs          # Battle UI display
│   ├── PartyDisplay.cs          # Party display
│   └── InventoryDisplay.cs      # Inventory display
└── Managers/
    ├── GameManager.cs            # Main game manager
    ├── SaveManager.cs            # Save/load system
    └── EventManager.cs           # Game events
```

## Core Components

### Game State Management

**GameState.cs** - Manages current game state (Menu, World, Battle, Party, Inventory, etc.)

```csharp
public enum GameStateType
{
    MainMenu,
    WorldMap,
    Battle,
    Party,
    Inventory,
    PC,
    Settings,
    GameOver
}

public class GameState
{
    public GameStateType CurrentState { get; set; }
    public Player Player { get; set; }
    public WorldMap World { get; set; }
    // ... state data
}
```

### Game Loop

**GameLoop.cs** - Core game loop that processes input and updates game state

```csharp
public class GameLoop
{
    private GameState _gameState;
    private IGameView _view;
    
    public void Run()
    {
        while (!_gameState.IsGameOver)
        {
            _view.Render(_gameState);
            var input = _view.GetInput();
            ProcessInput(input);
            Update(_gameState);
        }
    }
}
```

### Player System

**Player.cs** - Player state (name, party, inventory, position, stats)

**Party.cs** - Active Pokemon party (max 6 Pokemon)

**PC.cs** - Pokemon storage system (boxes, organization)

**Inventory.cs** - Item inventory management

### World System

**WorldMap.cs** - World map with locations and connections

**Location.cs** - Individual location/area (name, encounter table, trainers)

**EncounterTable.cs** - Wild Pokemon encounter probabilities

### Encounter System

**EncounterSystem.cs** - Manages all encounter types

**WildEncounter.cs** - Wild Pokemon encounters (uses Feature 2: Combat System)

**TrainerEncounter.cs** - Trainer battles (uses Feature 2: Combat System)

**BossEncounter.cs** - Boss battles (special encounters)

### Progression System

**ExpSystem.cs** - EXP calculation and distribution (uses Feature 5.1)

**LevelUpSystem.cs** - Level up processing (moves, stats, evolutions)

**RewardSystem.cs** - Post-battle rewards (money, items, EXP)

**RoguelikeSystem.cs** - Roguelike progression (runs, meta-progression)

### UI System

**IGameView.cs** - Interface for game view implementations

**ConsoleGameView.cs** - Console-based implementation (text-based)

**MenuSystem.cs** - Menu navigation and selection

**BattleDisplay.cs** - Battle UI (uses IBattleView from Feature 2)

**PartyDisplay.cs** - Party display and management UI

**InventoryDisplay.cs** - Inventory display and item usage UI

## Integration Points

### Feature 1: Game Data

- Uses `PokemonSpeciesData`, `MoveData`, `ItemData`, `AbilityData`
- Uses `PokemonRegistry`, `MoveRegistry`
- Uses `PokemonFactory` for creating instances

### Feature 2: Combat System

- Uses `CombatEngine` for battles
- Uses `IBattleView` for battle display
- Uses `IActionProvider` for player input
- Uses `RandomAI` for wild Pokemon and trainers

### Feature 3: Content Expansion

- Uses all catalog data (Pokemon, Moves, Items, Abilities)
- Uses builders for creating game content

### Feature 5: Game Features

- Uses post-battle rewards system
- Uses Pokemon management system
- Uses encounter system
- Uses inventory system
- Uses save system
- Uses progression system

## Data Flow

### Battle Flow

```
Player in World → Encounter Triggered → 
EncounterSystem creates battle → 
CombatEngine processes battle → 
RewardSystem distributes rewards → 
LevelUpSystem processes level ups → 
Return to World
```

### Game Flow

```
Main Menu → New Game → 
Create Player → 
World Map → 
Encounter → Battle → 
Rewards → Level Up → 
World Map → ...
```

## Technical Requirements

### Dependencies

- **Feature 1**: Game Data (complete)
- **Feature 2**: Combat System (complete)
- **Feature 3**: Content Expansion (in progress)
- **Feature 5**: Game Features (planned - will implement as needed)

### Platform

- **Target**: .NET Console Application
- **UI**: Console-based (text)
- **Input**: Keyboard input
- **Output**: Console text output

### Performance

- **Responsiveness**: Immediate input response
- **Rendering**: Fast text rendering
- **Memory**: Efficient state management

## Testing Strategy

See [testing.md](testing.md) for complete testing strategy.

### Test Types

1. **Unit Tests** - Individual components (GameState, Party, Inventory, etc.)
2. **Integration Tests** - System interactions (World → Encounter → Battle)
3. **End-to-End Tests** - Complete game scenarios
4. **Smoke Tests** - Critical paths (new game → battle → victory)

## Future Considerations

### Unity Migration

- Design with Unity migration in mind
- Separate UI from game logic
- Use interfaces for view implementations
- Keep game logic Unity-agnostic

### Extensibility

- Easy to add new locations
- Easy to add new encounter types
- Easy to add new UI elements
- Easy to add new game features

---

**Last Updated**: January 2025

