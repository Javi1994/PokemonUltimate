# Feature 6: Development Tools - Code Location

> Where the development tools code lives and how it's organized.

**Feature Number**: 6  
**Feature Name**: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The development tools are organized in a single Windows Forms project: `PokemonUltimate.DeveloperTools`.

## Project Structure

```
PokemonUltimate.DeveloperTools/
├── MainForm.cs                    # Main form with TabControl
├── Program.cs                     # Application entry point
├── Runners/
│   ├── BattleRunner.cs           # Battle execution logic ✅
│   ├── MoveRunner.cs             # Move testing logic ✅
│   ├── StatCalculatorRunner.cs   # Stat calculation logic ✅ (6.1)
│   ├── DamageCalculatorRunner.cs # Damage pipeline visualization ✅ (6.2)
│   ├── StatusEffectRunner.cs     # Status effect testing logic (6.3)
│   └── TurnOrderRunner.cs        # Turn order calculation logic (6.4)
└── Tabs/
    ├── BattleDebuggerTab.cs      # Battle debugger UserControl ✅
    ├── MoveDebuggerTab.cs        # Move debugger UserControl ✅
    ├── TypeMatchupDebuggerTab.cs # Type matchup UserControl ✅
    ├── StatCalculatorDebuggerTab.cs # Stat calculator UserControl ✅ (6.1)
    ├── DamageCalculatorDebuggerTab.cs # Damage calculator UserControl ✅ (6.2)
    ├── StatusEffectDebuggerTab.cs # Status effect UserControl (6.3)
    └── TurnOrderDebuggerTab.cs   # Turn order UserControl (6.4)

PokemonUltimate.DataViewer/
├── Program.cs                     # Application entry point ✅ (6.7)
├── MainForm.cs                    # Main form with TabControl ✅ (6.7)
└── Tabs/
    ├── PokemonDataTab.cs         # Pokemon data display tab ✅ (6.7)
    ├── MoveDataTab.cs            # Move data display tab ✅ (6.7)
    ├── ItemDataTab.cs            # Item data display tab ✅ (6.7)
    ├── AbilityDataTab.cs         # Ability data display tab ✅ (6.7)
    ├── StatusDataTab.cs          # Status Effect data display tab ✅ (6.7)
    ├── WeatherDataTab.cs         # Weather data display tab ✅ (6.7)
    ├── TerrainDataTab.cs         # Terrain data display tab ✅ (6.7)
    ├── HazardDataTab.cs          # Hazard data display tab ✅ (6.7)
    ├── SideConditionDataTab.cs   # Side Condition data display tab ✅ (6.7)
    └── FieldEffectDataTab.cs     # Field Effect data display tab ✅ (6.7)
```

## Namespaces

### `PokemonUltimate.DeveloperTools`

**Purpose**: Main application namespace

**Key Classes**:
- `MainForm` - Main application window
- `Program` - Application entry point

### `PokemonUltimate.DeveloperTools.Runners`

**Purpose**: Reusable logic for executing tests and collecting statistics

**Key Classes**:
- `BattleRunner` - Executes battles and collects statistics ✅ (6.5)
- `MoveRunner` - Tests moves and collects statistics ✅ (6.6)
- `StatCalculatorRunner` - Calculates stats with different configurations ✅ (6.1)
- `DamageCalculatorRunner` - Visualizes damage pipeline step-by-step ✅ (6.2)
- `StatusEffectRunner` - Tests status effects and interactions ✅ (6.3)
- `TurnOrderRunner` - Calculates turn order with different configurations ✅ (6.4)

### `PokemonUltimate.DeveloperTools.Tabs`

**Purpose**: UserControl-based tabs for each debugger

**Key Classes**:
- `BattleDebuggerTab` - Battle debugger UI ✅ (6.5)
- `MoveDebuggerTab` - Move debugger UI ✅ (6.6)
- `TypeMatchupDebuggerTab` - Type matchup UI ✅
- `StatCalculatorDebuggerTab` - Stat calculator UI ✅ (6.1)
- `DamageCalculatorDebuggerTab` - Damage calculator UI ✅ (6.2)
- `StatusEffectDebuggerTab` - Status effect UI ✅ (6.3)
- `TurnOrderDebuggerTab` - Turn order UI ✅ (6.4)

## Key Classes

### MainForm

**Namespace**: `PokemonUltimate.DeveloperTools`
**File**: `PokemonUltimate.DeveloperTools/MainForm.cs`
**Purpose**: Main application window with tabbed interface

**Key Properties**:
- `mainTabControl` - TabControl containing all debugger tabs
- Tab pages for each debugger

**Key Methods**:
- `InitializeComponent()` - Sets up UI with all tabs

### BattleRunner (6.5)

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`
**File**: `PokemonUltimate.DeveloperTools/Runners/BattleRunner.cs`
**Purpose**: Executes battles and collects statistics

**Key Methods**:
- `RunBattles(...)` - Runs multiple battles and collects statistics

**See**: [`6.5-battle-debugger/README.md`](6.5-battle-debugger/README.md) for complete documentation

### MoveRunner (6.6)

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`
**File**: `PokemonUltimate.DeveloperTools/Runners/MoveRunner.cs`
**Purpose**: Tests moves and collects statistics

**Key Methods**:
- `RunTests(...)` - Tests moves multiple times and collects statistics

**See**: [`6.6-move-debugger/README.md`](6.6-move-debugger/README.md) for complete documentation

### StatCalculatorRunner (6.1)

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`
**File**: `PokemonUltimate.DeveloperTools/Runners/StatCalculatorRunner.cs`
**Purpose**: Calculates stats with different configurations

**Key Methods**:
- `CalculateStats(...)` - Calculates stats for given configuration
- `CompareConfigurations(...)` - Compares two configurations

### DamageCalculatorRunner (6.2) ✅

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`
**File**: `PokemonUltimate.DeveloperTools/Runners/DamageCalculatorRunner.cs`
**Purpose**: Visualizes damage pipeline step-by-step

**Key Methods**:
- `CalculateDamage(...)` - Calculates damage and returns pipeline steps
- Returns `DamageCalculationResult` with step-by-step pipeline visualization

### StatusEffectRunner (6.3)

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`
**File**: `PokemonUltimate.DeveloperTools/Runners/StatusEffectRunner.cs`
**Purpose**: Tests status effects and interactions

**Key Methods**:
- `ApplyStatus(...)` - Applies status effect to Pokemon
- `GetStatModifications(...)` - Gets stat modifications from status
- `GetDamagePerTurn(...)` - Gets damage per turn from status

### TurnOrderRunner (6.4) ✅

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`
**File**: `PokemonUltimate.DeveloperTools/Runners/TurnOrderRunner.cs`
**Purpose**: Calculates turn order with different configurations

**Key Methods**:
- `CalculateTurnOrder(...)` - Calculates turn order for given Pokemon configurations
- Returns `TurnOrderResult` with speed calculations, priority info, and final turn order

### Data Viewer (6.7) ✅

**Namespace**: `PokemonUltimate.DataViewer`
**File**: `PokemonUltimate.DataViewer/Program.cs`, `MainForm.cs`
**Purpose**: Windows Forms application for viewing game data visually

**Key Classes**:
- `Program` - Application entry point
- `MainForm` - Main form with TabControl containing all data tabs
- `PokemonDataTab` - UserControl tab displaying Pokemon data in DataGridView
- `MoveDataTab` - UserControl tab displaying Move data in DataGridView
- `ItemDataTab` - UserControl tab displaying Item data in DataGridView
- `AbilityDataTab` - UserControl tab displaying Ability data in DataGridView
- `StatusDataTab` - UserControl tab displaying Status Effect data in DataGridView
- `WeatherDataTab` - UserControl tab displaying Weather data in DataGridView
- `TerrainDataTab` - UserControl tab displaying Terrain data in DataGridView
- `HazardDataTab` - UserControl tab displaying Hazard data in DataGridView
- `SideConditionDataTab` - UserControl tab displaying Side Condition data in DataGridView
- `FieldEffectDataTab` - UserControl tab displaying Field Effect data in DataGridView

**Key Features**:
- Visual Windows Forms interface with tabs
- DataGridView controls for tabular data display
- RichTextBox for detailed information display
- Interactive selection - click rows to see details

**See**: [`6.7-data-viewer/README.md`](6.7-data-viewer/README.md) for complete documentation

## Integration Points

### With Game Data (Feature 1)

- Uses `PokemonSpeciesData` from `PokemonUltimate.Core.Blueprints`
- Uses `PokemonInstance` from `PokemonUltimate.Core.Instances`
- Uses `StatCalculator` from `PokemonUltimate.Core.Factories`
- Uses `TypeEffectiveness` from `PokemonUltimate.Core.Factories`

### With Combat System (Feature 2)

- Uses `DamagePipeline` from `PokemonUltimate.Combat.Damage`
- Uses `TurnOrderResolver` from `PokemonUltimate.Combat.Helpers`
- Uses `BattleField` from `PokemonUltimate.Combat.Field`
- Uses `CombatEngine` from `PokemonUltimate.Combat.Engine`

### With Content

- Uses `PokemonCatalog`, `MoveCatalog` from `PokemonUltimate.Content.Catalogs`

## Related Documents

- **[Architecture](architecture.md)** - Technical design of debugger system
- **[Testing](testing.md)** - Testing strategy
- **[Roadmap](roadmap.md)** - Implementation phases
- **[Use Cases](use_cases.md)** - Scenarios implemented in this code
- **[Feature 1: Game Data](../1-game-data/code_location.md)** - Game data code organization
- **[Feature 2: Combat System](../2-combat-system/code_location.md)** - Combat system code organization

---

**Last Updated**: January 2025

