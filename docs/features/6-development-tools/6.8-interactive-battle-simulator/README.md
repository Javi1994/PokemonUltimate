# Sub-Feature 6.8: Interactive Battle Simulator

> Windows Forms application with tabbed interface: battle configuration and real-time battle logs in a single window.

**Feature**: 6: Development Tools  
**Sub-Feature**: 6.8: Interactive Battle Simulator  
**Status**: ✅ Complete  
**See**: [`../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

The Interactive Battle Simulator is a Windows Forms application that provides a realistic battle simulation environment with a tabbed interface:

1. **Battle Mode Tab**: Configure battle format (Singles, Doubles, Triples, Horde, Custom) and number of slots
2. **Pokemon Tab**: Configure player and enemy teams with sub-tabs for each team, including Pokemon selection, level, Nature, and IVs
3. **Logs Tab**: Real-time display of all battle events, actions, and logs with filtering and auto-scroll

This tool allows developers to simulate battles step-by-step and observe all internal mechanics through comprehensive logging.

## Purpose

-   Simulate realistic Pokemon battles with full control over parameters
-   View all battle logs in real-time as the battle progresses
-   Debug battle mechanics by observing detailed event logs
-   Test specific Pokemon matchups and scenarios
-   Understand battle flow and action execution order

## Features

### Battle Mode Tab

-   **Battle Format Selection**:
    -   Singles (1v1)
    -   Doubles (2v2)
    -   Triples (3v3)
    -   Horde (1v3 or 1v5)
    -   Custom (configurable slots)
-   **Custom Slots Configuration**:
    -   Player slots: 1-6
    -   Enemy slots: 1-6
    -   Enabled only when "Custom" mode is selected

### Pokemon Tab

-   **Player Team Sub-Tab**:
    -   Dynamic Pokemon slot configuration (based on battle mode)
    -   Select Pokemon from catalog
    -   Configure level (1-100)
    -   Select Nature (or Random)
    -   Perfect IVs checkbox
-   **Enemy Team Sub-Tab**:
    -   Dynamic Pokemon slot configuration (based on battle mode)
    -   Select Pokemon from catalog
    -   Configure level (1-100)
    -   Select Nature (or Random)
    -   Perfect IVs checkbox

### Logs Tab

-   **Real-time Log Display**:
    -   RichTextBox with formatted log messages
    -   Color-coded log levels (Debug=Gray, Info=LightGreen, Warning=Yellow, Error=Red, Battle Events=Cyan)
    -   Timestamp for each log entry
    -   Auto-scroll to latest log entry (configurable)
-   **Log Filtering**:
    -   Filter by log level (All, Debug, Info, Warning, Error, Battle Events)
-   **Log Controls**:
    -   Clear logs button
    -   Copy to clipboard button
    -   Auto-scroll checkbox
    -   Filter dropdown

### Results Tabs

-   **Player Results Tab**:
    -   Battle outcome and statistics
    -   Alive and fainted Pokemon list
    -   Kill history
    -   Total turns
-   **Enemy Results Tab**:
    -   Battle outcome and statistics
    -   Alive and fainted Pokemon list
    -   Kill history
    -   Total turns

### Control Buttons (Bottom Panel)

-   **Start Battle**: Initiates battle simulation
-   **Stop Battle**: Stops running battle
-   **Status Label**: Shows current battle state

## UI Components

### Main Form (Tabbed Interface)

**Tab 1: Battle Mode**

-   Battle mode selector (ComboBox)
-   Custom slots configuration (GroupBox with NumericUpDown controls)
-   Information label explaining battle modes

**Tab 2: Pokemon**

-   Inner TabControl with two sub-tabs:
    -   **Player Team**: Dynamic Pokemon slot controls
    -   **Enemy Team**: Dynamic Pokemon slot controls
-   Each slot includes: Pokemon selector, Level, Nature, Perfect IVs checkbox

**Tab 3: Logs**

-   Header panel with controls (Clear Logs button, Copy to Clipboard button, Auto-scroll checkbox, Filter dropdown)
-   RichTextBox for log display (full height, scrollable)

**Tab 4: Player Results**

-   Battle outcome and statistics
-   Alive and fainted Pokemon lists
-   Kill history
-   Refresh and Copy buttons

**Tab 5: Enemy Results**

-   Battle outcome and statistics
-   Alive and fainted Pokemon lists
-   Kill history
-   Refresh and Copy buttons

**Bottom Panel (Always Visible)**

-   Start Battle button
-   Stop Battle button
-   Status label

## Code Location

**Project**: `PokemonUltimate.BattleSimulator` (separate Windows Forms project)  
**Namespace**: `PokemonUltimate.BattleSimulator.Forms`  
**Main Form**: `Forms/InteractiveBattleSimulatorForm.cs`  
**Logger**: `Logging/UIBattleLogger.cs` (implements IBattleLogger)  
**Entry Point**: `Program.cs`

### Key Classes

#### InteractiveBattleSimulatorForm

**Purpose**: Main form with tabbed interface for battle configuration and logs

**Key Components**:

-   TabControl with three tabs: Battle Mode, Pokemon, Logs
-   Battle Mode tab: Battle format selection and custom slots configuration
-   Pokemon tab: Inner TabControl with Player Team and Enemy Team sub-tabs
-   Logs tab: Real-time log display with filtering
-   Bottom panel: Battle control buttons (Start, Stop) and status label
-   Dynamic Pokemon slot generation based on battle mode selection

#### UIBattleLogger

**Purpose**: Custom IBattleLogger implementation that captures logs and displays them in UI

**Key Methods**:

-   `LogDebug(string message)` - Logs debug messages
-   `LogInfo(string message)` - Logs info messages
-   `LogWarning(string message)` - Logs warnings
-   `LogError(string message)` - Logs errors
-   `LogBattleEvent(string eventType, string message)` - Logs battle events
-   `OnLogAdded` event - Notifies UI when new log is added

## Automatic Log Saving

The simulator **automatically saves all logs** from each battle to the `Logs` folder in the project directory (`PokemonUltimate.BattleSimulator/Logs/`).

### Log File Format

-   **Single battles**: `battle_logs_YYYYMMDD_HHMMSS.txt`
-   **Batch battles**: `battle_logs_YYYYMMDD_HHMMSS_battleXofY.txt`
-   **Consolidated export** (optional): `battle_logs_batch_YYYYMMDD_HHMMSS_Xbattles.txt`

### Log Content

Each log file contains:

-   Battle timestamp and outcome
-   Complete chronological event log
-   Detailed action information (moves, damage, status effects)
-   Status effect application and prevention messages (paralysis preventing movement, sleep, etc.)
-   Pokemon state at start/end of each turn
-   Complete battle statistics

### Purpose for AI Debugging

The automatically saved logs are designed to facilitate **rapid debugging by AI**. When unexpected behavior or bugs are detected, the logs provide:

1. **Complete traceability**: Every battle action is recorded with precise timestamps
2. **Full context**: State information for all Pokemon at each moment
3. **Rapid analysis**: AI can quickly identify:
    - Damage calculation errors
    - Status effect problems
    - Turn logic bugs
    - Anomalous AI behavior
    - Stat change issues
4. **Bug reproduction**: Logs allow understanding exactly what happened in a problematic battle

## Use Cases

1. **Debug Battle Mechanics**: Run a battle and observe all internal events through logs
2. **Test Specific Scenarios**: Configure exact Pokemon teams and levels to test specific matchups
3. **Understand Action Flow**: See the order of actions and reactions in the battle queue
4. **Verify Damage Calculations**: Observe damage calculation steps in logs
5. **Test Status Effects**: See when status effects are applied and when they prevent movement
6. **Analyze AI Behavior**: Observe AI decision-making through logs
7. **Batch Testing**: Run multiple battles to gather statistics and identify patterns
8. **Log Analysis**: Review saved logs to debug issues reported by users or AI

## Dependencies

-   **Feature 2: Combat System** - Uses CombatEngine, BattleField, IBattleLogger
-   **Feature 1: Game Data** - Uses PokemonSpeciesData, PokemonInstance
-   **Feature 2.6: Combat Engine** - Uses IBattleLogger interface

## Implementation Notes

-   Uses custom `UIBattleLogger` that implements `IBattleLogger` and captures logs for UI display
-   Battle runs asynchronously to keep UI responsive
-   Logs are displayed in real-time as battle progresses
-   **Automatic log saving**: All battles are automatically saved to `Logs/` folder in project directory
-   **Status effect logging**: Complete logging of status effects including when they prevent movement
-   **Event-driven logging**: Uses `EventBasedBattleLogger` and `DetailedBattleLoggerObserver` for comprehensive event capture
-   Standalone application (separate from DeveloperTools)

## Related Documentation

-   **[README](../../PokemonUltimate.BattleSimulator/README.md)** - Module README with complete documentation
-   **[Feature 6: Development Tools](../README.md)** - Parent feature overview
-   **[Code Location](../code_location.md)** - Code organization details
-   **[Architecture](../architecture.md)** - Technical design
-   **[Use Cases](../use_cases.md)** - All debugger scenarios
-   **[Feature 2: Combat System](../../2-combat-system/)** - Battle system documentation

---

**Last Updated**: December 2025
