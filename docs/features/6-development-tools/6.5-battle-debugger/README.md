# Sub-Feature 6.5: Battle Debugger

> Windows Forms debugger tab for running multiple battles and analyzing statistics.

**Feature**: 6: Development Tools  
**Sub-Feature**: 6.5: Battle Debugger  
**Status**: ✅ Complete  
**See**: [`../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

The Battle Debugger is a debugger tab in the `PokemonUltimate.DevelopTools` application that allows developers to run multiple battles and analyze comprehensive statistics including move usage, status effects, and win/loss/draw rates.

## Purpose

- Run multiple battles automatically (1-10,000 battles)
- Analyze move usage patterns per Pokemon
- Track status effect application rates
- Calculate win/loss/draw statistics
- Debug battle mechanics and verify system behavior

## Features

- ✅ Move usage statistics (most used moves per Pokemon)
- ✅ Status effect statistics (effects caused per Pokemon)
- ✅ Win/loss/draw rates
- ✅ Visual progress tracking during execution
- ✅ Results displayed in tables and formatted text
- ✅ Uses Statistics System (Sub-Feature 2.20) for automatic tracking

## UI Components

### Configuration Panel

- **Player Pokemon**: Dropdown to select Pokemon species (or Random)
- **Enemy Pokemon**: Dropdown to select Pokemon species (or Random)
- **Level**: Numeric input (1-100)
- **Number of Battles**: Numeric input (1-10,000)
- **Detailed Output**: Checkbox for verbose logging
- **Run Battles Button**: Starts battle execution

### Results Tabs

#### Summary Tab
- Win/loss/draw statistics
- Move usage summary
- Status effect summary
- Formatted text output

#### Move Usage Tab
- DataGridView table showing:
  - Pokemon name
  - Move name
  - Usage count
  - Usage percentage

#### Status Effects Tab
- DataGridView table showing:
  - Pokemon name
  - Effect name
  - Times caused
  - Percentage

## Code Location

**Namespace**: `PokemonUltimate.DevelopTools.Runners`  
**Runner**: `BattleRunner.cs`  
**Tab**: `PokemonUltimate.DevelopTools.Tabs.BattleDebuggerTab.cs`

### Key Classes

#### BattleRunner

**Purpose**: Executes battles and collects statistics

**Key Methods**:
- `RunBattles(...)` - Runs multiple battles and collects statistics
- Returns `BattleResult` with comprehensive statistics

#### BattleDebuggerTab

**Purpose**: UserControl-based UI for battle debugging

**Key Components**:
- Configuration panel with Pokemon selection
- Results tabs (Summary, Move Usage, Status Effects)
- Progress tracking display

## Use Cases

1. **Testing Pokemon Matchups**: Run many battles between two Pokemon to see which wins more often
2. **Verifying Move Usage Patterns**: See which moves Pokemon use most frequently
3. **Analyzing Status Effect Rates**: Track how often status effects are applied
4. **Debugging Battle Mechanics**: Verify that battle system works correctly
5. **Comparing Pokemon Performance**: Compare win rates between different Pokemon

## Dependencies

- **Feature 2: Combat System** - Uses CombatEngine, BattleField, AI providers
- **Feature 2.20: Statistics System** - Uses automatic statistics collection
- **Feature 1: Game Data** - Uses PokemonSpeciesData, PokemonInstance

## Related Documentation

- **[Feature 6: Development Tools](../README.md)** - Parent feature overview
- **[Code Location](../code_location.md)** - Code organization details
- **[Architecture](../architecture.md)** - Technical design
- **[Use Cases](../use_cases.md)** - All debugger scenarios

---

**Last Updated**: January 2025

