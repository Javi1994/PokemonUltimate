# Sub-Feature 6.6: Move Debugger

> Windows Forms debugger tab for testing moves multiple times and collecting comprehensive statistics.

**Feature**: 6: Development Tools  
**Sub-Feature**: 6.6: Move Debugger  
**Status**: ✅ Complete  
**See**: [`../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

The Move Debugger is a debugger tab in the `PokemonUltimate.DeveloperTools` application that allows developers to test moves multiple times and collect comprehensive statistics including damage ranges, critical hit rates, miss rates, and status effect application rates.

## Purpose

- Test moves multiple times automatically (1-10,000 tests)
- Analyze damage statistics (average, min, max, median)
- Track critical hit rates
- Track miss rates
- Monitor status effect application rates (persistent and volatile)
- Track action generation (what actions the move creates)

## Features

- ✅ Damage statistics (average, min, max, median)
- ✅ Critical hit rates
- ✅ Miss rates
- ✅ Status effect application rates (persistent and volatile)
- ✅ Action generation tracking (what actions the move creates)
- ✅ Visual progress tracking during execution
- ✅ Results displayed in tables and formatted text
- ✅ Uses Statistics System (Sub-Feature 2.20) for automatic tracking

## UI Components

### Configuration Panel

- **Move**: Dropdown to select move to test
- **Attacker Pokemon**: Dropdown to select attacker Pokemon species
- **Target Pokemon**: Dropdown to select target Pokemon species
- **Level**: Numeric input (1-100)
- **Number of Tests**: Numeric input (1-10,000)
- **Detailed Output**: Checkbox for verbose logging
- **Run Tests Button**: Starts move testing

### Results Tabs

#### Summary Tab
- Move information (name, type, power, accuracy)
- Type effectiveness calculation
- Damage statistics summary
- Status effects summary
- Actions generated summary
- Formatted text output

#### Damage Tab
- DataGridView table with damage metrics:
  - Total hits
  - Average damage
  - Minimum damage
  - Maximum damage
  - Median damage
  - Critical hits count and percentage
  - Misses count and percentage

#### Status Effects Tab
- DataGridView table showing:
  - Persistent status effects caused (Burn, Poison, etc.)
  - Volatile status effects caused (Confusion, Flinch, etc.)
  - Application counts and percentages

#### Actions Tab
- DataGridView table showing:
  - Action type (Damage, Status, Heal, etc.)
  - Count
  - Percentage

## Code Location

**Namespace**: `PokemonUltimate.DeveloperTools.Runners`  
**Runner**: `MoveRunner.cs`  
**Tab**: `PokemonUltimate.DeveloperTools.Tabs.MoveDebuggerTab.cs`

### Key Classes

#### MoveRunner

**Purpose**: Tests moves and collects statistics

**Key Methods**:
- `RunTests(...)` - Tests moves multiple times and collects statistics
- Returns `MoveResult` with comprehensive statistics

#### MoveDebuggerTab

**Purpose**: UserControl-based UI for move debugging

**Key Components**:
- Configuration panel with move and Pokemon selection
- Results tabs (Summary, Damage, Status Effects, Actions)
- Progress tracking display

## Use Cases

1. **Verifying Move Power Ranges**: Test a move many times to see damage distribution
2. **Testing Status Effect Chances**: Verify status effect application rates match expected values
3. **Analyzing Move Effects**: See what actions a move generates
4. **Comparing Damage Output**: Compare average damage across multiple tests
5. **Debugging Move Mechanics**: Verify that move effects work correctly

## Dependencies

- **Feature 2: Combat System** - Uses DamagePipeline, CombatEngine, BattleField
- **Feature 2.20: Statistics System** - Uses automatic statistics collection
- **Feature 1: Game Data** - Uses MoveData, PokemonSpeciesData, PokemonInstance

## Related Documentation

- **[Feature 6: Development Tools](../README.md)** - Parent feature overview
- **[Code Location](../code_location.md)** - Code organization details
- **[Architecture](../architecture.md)** - Technical design
- **[Use Cases](../use_cases.md)** - All debugger scenarios

---

**Last Updated**: January 2025

