# Unified Debugger - Testing Tools

> Single Windows Forms application integrating all debuggers for testing different aspects of the Pokemon battle system.

## Overview

The **PokemonUltimate.DevelopTools** is a unified Windows Forms application that combines all debuggers into a single interface with tabbed navigation. This provides a more consistent experience and easier maintenance compared to separate applications.

## Quick Start

### Run the Unified Debugger

```bash
# Run the unified debugger application
dotnet run --project PokemonUltimate.DevelopTools
```

The application opens with four tabs:
- **Battle Debugger** - Run multiple battles and see statistics
- **Move Debugger** - Test moves multiple times and see statistics
- **Type Matchup** - Calculate type effectiveness
- **Stat Calculator** - Calculate and visualize Pokemon stats with different configurations

## Debugger Tabs

### Battle Debugger Tab (6.5)

**Purpose**: Run multiple battles and analyze statistics

**Features**:
- ✅ Move usage statistics (most used moves per Pokemon)
- ✅ Status effect statistics (effects caused per Pokemon)
- ✅ Win/loss/draw rates
- ✅ Visual progress tracking during execution
- ✅ Results displayed in tables and formatted text
- ✅ Uses Statistics System (Sub-Feature 2.20) for automatic tracking

**See**: [`docs/features/6-development-tools/6.5-battle-debugger/README.md`](docs/features/6-development-tools/6.5-battle-debugger/README.md) for complete documentation

### Move Debugger Tab (6.6)

**Purpose**: Test moves multiple times and collect comprehensive statistics

**Features**:
- ✅ Damage statistics (average, min, max, median)
- ✅ Critical hit rates
- ✅ Miss rates
- ✅ Status effect application rates (persistent and volatile)
- ✅ Action generation tracking (what actions the move creates)
- ✅ Visual progress tracking during execution
- ✅ Results displayed in tables and formatted text
- ✅ Uses Statistics System (Sub-Feature 2.20) for automatic tracking

**See**: [`docs/features/6-development-tools/6.6-move-debugger/README.md`](docs/features/6-development-tools/6.6-move-debugger/README.md) for complete documentation

### Type Matchup Tab

**Purpose**: Calculate and visualize type effectiveness combinations

**Features**:
- ✅ Single type vs single type effectiveness
- ✅ Single type vs dual type effectiveness
- ✅ Complete type chart table for any attacking type
- ✅ Effectiveness breakdown for dual-type defenders
- ✅ Color-coded results (Red=Immune, Orange=Not Very Effective, Green=Super Effective)

**Configuration**:
- Select attacking type
- Select defender primary type
- Select defender secondary type (or None for single-type)

**Results**:
- **Effectiveness Result**: Shows calculated effectiveness with color coding
- **Breakdown**: For dual-type defenders, shows how each type contributes
- **Complete Type Chart**: Table showing effectiveness against all 18 Pokemon types

**Use Cases**:
- Verifying type chart correctness
- Testing dual-type effectiveness
- Checking immunities
- Validating super effective combinations
- Understanding type interactions

## Architecture

```
PokemonUltimate.DevelopTools/
├── MainForm.cs                    # Main form with TabControl
├── Program.cs                     # Application entry point
├── Runners/
│   ├── BattleRunner.cs           # Reusable battle execution logic
│   └── MoveRunner.cs             # Reusable move testing logic
└── Tabs/
    ├── BattleDebuggerTab.cs      # Battle debugger UserControl
    ├── MoveDebuggerTab.cs        # Move debugger UserControl
    └── TypeMatchupDebuggerTab.cs # Type matchup UserControl
```

## Shared Components

The unified debugger uses:
- **BattleRunner**: Reusable battle execution logic with statistics tracking
- **MoveRunner**: Reusable move testing logic with statistics tracking
- Common Windows Forms patterns and UI components
- Shared statistics collection and tracking mechanisms

## Common Features

All debugger tabs provide:

1. **Visual Configuration**
   - Dropdown menus for easy selection
   - Numeric inputs with validation
   - Checkboxes for optional features
   - Clear, organized layout

2. **Real-time Progress Tracking**
   - Progress bars during execution
   - Status labels with current operation
   - Non-blocking UI (async execution)

3. **Comprehensive Results**
   - Formatted text summaries
   - DataGridView tables for structured data
   - Multiple result views (tabs)
   - Easy-to-read statistics

4. **Error Handling**
   - Clear error messages
   - Graceful failure handling
   - UI remains responsive

## Benefits of Unified Application

✅ **Single Entry Point** - One application instead of three separate ones  
✅ **Consistent UI** - Same look and feel across all debuggers  
✅ **Easier Maintenance** - Shared code and components  
✅ **Better Organization** - All debuggers in one place  
✅ **Tabbed Navigation** - Easy switching between debuggers  
✅ **Shared Resources** - Common runners and utilities  

### Stat Calculator Debugger Tab

**Purpose**: Calculate and visualize Pokemon stats with different configurations

**Features**:
- ✅ Select Pokemon species and level
- ✅ Configure Nature (all 25 natures)
- ✅ Configure IVs (0-31 per stat)
- ✅ Configure EVs (0-252 per stat, 510 total)
- ✅ Display all calculated stats (HP, Attack, Defense, SpAttack, SpDefense, Speed)
- ✅ Show breakdown of calculation (base, IV, EV, Nature multiplier)
- ✅ Visual highlighting of Nature-affected stats

**Configuration**:
- Select Pokemon species
- Set level (1-100)
- Select Nature
- Set IVs for each stat (0-31)
- Set EVs for each stat (0-252)
- Real-time EV total validation (max 510)

**Results Tabs**:
- **Summary**: Formatted text with Pokemon info, final stats, Nature effects
- **Stats Breakdown**: DataGridView showing detailed calculation breakdown for each stat

**Use Cases**:
- Understanding how stats are calculated
- Testing different builds (IVs/EVs/Nature combinations)
- Validating stat calculations
- Comparing stat values at different levels

## Future Enhancements

Planned additions to the unified debugger:

- **Damage Calculator Debugger Tab** - Step-by-step damage pipeline visualization
- **Status Effect Debugger Tab** - Test status effects and their interactions
- **Turn Order Debugger Tab** - Visualize turn order determination
- **Ability Debugger Tab** - Test abilities and their effects
- **Item Debugger Tab** - Test held items and their effects
- **Weather Debugger Tab** - Test weather effects
- **Terrain Debugger Tab** - Test terrain effects
- **Export Results** - Save results to CSV/JSON
- **Save/Load Configurations** - Preserve debugger settings
- **Compare Results** - Side-by-side comparison of multiple runs

## Contributing

When adding a new debugger tab:

1. Create new UserControl: `Tabs/[Name]DebuggerTab.cs`
2. Implement `InitializeComponent()` for UI setup
3. Create runner class if needed: `Runners/[Name]Runner.cs`
4. Add tab to `MainForm.cs` TabControl
5. Update this document with new tab entry

## Notes

- The unified debugger uses Windows Forms for visual interfaces
- Battles/tests are fully automated (AI vs AI)
- Progress tracking during execution
- Results displayed in tables and formatted text
- All debuggers accessible from single application
- Debuggers are designed for development/debugging, not gameplay

---

**Last Updated**: 2025-01-XX
