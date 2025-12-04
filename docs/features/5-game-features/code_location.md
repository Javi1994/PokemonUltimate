# Feature 5: Game Features - Code Location

> Where the game features code will live and how it's organized.

**Feature Number**: 5  
**Feature Name**: Game Features  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Game features code will be organized in a new project:
- **PokemonUltimate.Game** - Game systems beyond combat
- **Services** - Post-battle, encounters, progression
- **Managers** - Party, PC, inventory, save

**Note**: This is planned structure. Code not yet implemented.

## Project Structure (Planned)

```
PokemonUltimate.Game/
├── Services/
│   ├── PostBattleService.cs          # Post-battle rewards and EXP
│   ├── EncounterService.cs           # Wild/trainer/boss encounters
│   ├── ProgressionService.cs         # Roguelike progression
│   └── MetaProgressionService.cs     # Meta-progression unlocks
│
├── Managers/
│   ├── PartyManager.cs                # Pokemon party management
│   ├── PCManager.cs                   # PC storage system
│   ├── InventoryManager.cs           # Item inventory
│   └── SaveManager.cs                 # Save/load system
│
├── Systems/
│   ├── ExpCalculator.cs               # EXP calculation
│   ├── CatchCalculator.cs             # Catch rate calculation
│   ├── EncounterTable.cs              # Encounter table data
│   └── RoguelikeRun.cs                # Roguelike run state
│
└── Data/
    ├── SaveData.cs                    # Save file structure
    ├── RunData.cs                     # Roguelike run data
    └── MetaProgressionData.cs         # Meta-progression data
```

## Key Classes (Planned)

### PostBattleService
**Namespace**: `PokemonUltimate.Game.Services`
**File**: `PokemonUltimate.Game/Services/PostBattleService.cs`
**Purpose**: Handles post-battle rewards and progression
**Key Methods**:
- `CalculateExp(BattleResult, PokemonInstance[])` - Calculate EXP gained
- `DistributeExp(PokemonInstance[], int)` - Distribute EXP to Pokemon
- `ProcessLevelUps(PokemonInstance[])` - Handle level ups
- `ProcessRewards(BattleResult)` - Process item/money rewards

### EncounterService
**Namespace**: `PokemonUltimate.Game.Services`
**File**: `PokemonUltimate.Game/Services/EncounterService.cs`
**Purpose**: Handles wild, trainer, and boss encounters
**Key Methods**:
- `RollWildEncounter(EncounterArea)` - Roll for wild encounter
- `GenerateWildPokemon(EncounterArea)` - Generate wild Pokemon
- `InitializeTrainerBattle(TrainerData)` - Set up trainer battle
- `InitializeBossBattle(BossData)` - Set up boss battle

### PartyManager
**Namespace**: `PokemonUltimate.Game.Managers`
**File**: `PokemonUltimate.Game/Managers/PartyManager.cs`
**Purpose**: Manages player's active Pokemon party
**Key Methods**:
- `AddPokemon(PokemonInstance)` - Add Pokemon to party
- `RemovePokemon(int index)` - Remove Pokemon from party
- `SwitchOrder(int from, int to)` - Reorder party
- `GetActivePokemon()` - Get first non-fainted Pokemon

### PCManager
**Namespace**: `PokemonUltimate.Game.Managers`
**File**: `PokemonUltimate.Game/Managers/PCManager.cs`
**Purpose**: Manages PC storage system
**Key Methods**:
- `StorePokemon(PokemonInstance, int box)` - Store Pokemon in PC
- `RetrievePokemon(int box, int slot)` - Retrieve Pokemon from PC
- `GetBox(int box)` - Get PC box contents
- `GetAllBoxes()` - Get all PC boxes

### InventoryManager
**Namespace**: `PokemonUltimate.Game.Managers`
**File**: `PokemonUltimate.Game/Managers/InventoryManager.cs`
**Purpose**: Manages item inventory
**Key Methods**:
- `AddItem(ItemData, int quantity)` - Add items to inventory
- `RemoveItem(ItemData, int quantity)` - Remove items
- `UseItem(ItemData, PokemonInstance)` - Use item on Pokemon
- `GetItemCount(ItemData)` - Get item quantity

### SaveManager
**Namespace**: `PokemonUltimate.Game.Managers`
**File**: `PokemonUltimate.Game/Managers/SaveManager.cs`
**Purpose**: Handles save/load system
**Key Methods**:
- `SaveGame(SaveData, string path)` - Save game to file
- `LoadGame(string path)` - Load game from file
- `GetSaveFiles()` - List available save files
- `DeleteSave(string path)` - Delete save file

### ExpCalculator
**Namespace**: `PokemonUltimate.Game.Systems`
**File**: `PokemonUltimate.Game/Systems/ExpCalculator.cs`
**Purpose**: Calculates EXP gained
**Key Methods**:
- `Calculate(PokemonInstance defeated, bool isWild, int levelDifference, int participants)` - Calculate EXP
- `ApplyLevelDifference(int baseExp, int levelDifference)` - Apply level modifier

### CatchCalculator
**Namespace**: `PokemonUltimate.Game.Systems`
**File**: `PokemonUltimate.Game/Systems/CatchCalculator.cs`
**Purpose**: Calculates catch rate
**Key Methods**:
- `CalculateCatchRate(PokemonSpeciesData, int currentHP, int maxHP, StatusEffect status, ItemData ball)` - Calculate catch rate
- `RollCatch(int catchRate)` - Roll for catch success

## Integration Points

### With Combat System
- Uses `BattleResult` from combat system
- Uses `PokemonInstance` for battle participants
- Uses `CombatEngine` for encounters

### With Pokemon Data
- Uses `PokemonSpeciesData` for encounters
- Uses `PokemonInstance` for party/PC
- Uses evolution system for level-ups

### With Content System
- Uses `PokemonCatalog` for encounters
- Uses `ItemCatalog` for inventory
- Uses `MoveCatalog` for move learning

### With Unity Integration
- Uses Unity UI for post-battle display
- Uses Unity UI for party/PC management
- Uses Unity UI for inventory

## Test Location (Planned)

**Tests**: `PokemonUltimate.Tests/Systems/Game/`
- **Services**: `Services/[Service]Tests.cs`
- **Managers**: `Managers/[Manager]Tests.cs`
- **Systems**: `Systems/[System]Tests.cs`

**Test Structure**:
```
Tests/Systems/Game/
├── Services/
│   ├── PostBattleServiceTests.cs
│   ├── EncounterServiceTests.cs
│   └── ProgressionServiceTests.cs
├── Managers/
│   ├── PartyManagerTests.cs
│   ├── PCManagerTests.cs
│   ├── InventoryManagerTests.cs
│   └── SaveManagerTests.cs
└── Systems/
    ├── ExpCalculatorTests.cs
    └── CatchCalculatorTests.cs
```

## Related Documents

- **[Feature README](README.md)** - Overview of Game Features
- **[5.1 Architecture](5.1-post-battle-rewards/architecture.md)** - Post-battle rewards specification
- **[Use Cases](use_cases.md)** - Scenarios for game features
- **[Roadmap](roadmap.md)** - Implementation phases (5.1-5.6)
- **[Testing](testing.md)** - Testing strategy for game features
- **[Feature 2: Combat System](../2-combat-system/code_location.md)** - Battle engine code location
- **[Feature 1: Game Data](../1-game-data/code_location.md)** - Game data code location
- **[Feature 3: Content Expansion](../3-content-expansion/code_location.md)** - Content system code location

---

**Last Updated**: 2025-01-XX

