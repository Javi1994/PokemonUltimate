# PokemonUltimate.BattleDebuggerUI

Windows Forms application for running multiple battles and viewing statistics in a visual interface.

## Purpose

This is a **visual version** of the `BattleDebugger` console application. It provides:

- **Easy configuration** - Select Pokemon, level, and number of battles from dropdowns
- **Visual statistics** - View results in tables and formatted text
- **Progress tracking** - See progress bar during battle execution
- **Multiple result views** - Summary, Move Usage, and Status Effects tabs

## Features

### Configuration Panel (Left Side)

- **Player Pokemon** - Dropdown to select Pokemon (or Random)
- **Enemy Pokemon** - Dropdown to select Pokemon (or Random)
- **Level** - Numeric input for Pokemon level (1-100)
- **Number of Battles** - Numeric input for how many battles to run (1-10000)
- **Detailed Output** - Checkbox (currently not used in UI, kept for future)
- **Run Battles** - Button to start execution
- **Progress Bar** - Shows execution progress
- **Status Label** - Shows current status

### Results Panel (Right Side)

Three tabs with different views:

1. **Summary Tab**
   - Win/Loss/Draw statistics
   - Move usage summary
   - Status effect summary
   - Formatted text output

2. **Move Usage Tab**
   - DataGridView table showing:
     - Pokemon name
     - Move name
     - Usage count
     - Usage percentage

3. **Status Effects Tab**
   - DataGridView table showing:
     - Pokemon name
     - Status effect name
     - Times caused
     - Percentage

## Usage

### Run the Application

```bash
dotnet run --project PokemonUltimate.BattleDebuggerUI
```

Or from Rider/Visual Studio:
- Set `PokemonUltimate.BattleDebuggerUI` as startup project
- Press F5

### Configure and Run

1. Select Pokemon from dropdowns (or leave as "Random")
2. Set level (default: 50)
3. Set number of battles (default: 100)
4. Click "Run Battles"
5. Watch progress bar
6. View results in tabs

## Architecture

- **MainForm.cs** - Windows Forms UI with configuration and results display
- **BattleRunner.cs** - Reusable battle execution logic extracted from console debugger
- Uses `NullBattleView` for silent execution (no console output)
- Tracks statistics during battle execution
- Updates UI with results after completion

## Differences from Console BattleDebugger

| Feature | Console Debugger | UI Debugger |
|---------|------------------|-------------|
| Configuration | Edit code | Visual dropdowns |
| Output | Console text | Tables and formatted text |
| Progress | None | Progress bar |
| Results | Text summary | Multiple tabbed views |
| Execution | Synchronous | Async with UI updates |

## Notes

- Battles run asynchronously to keep UI responsive
- Progress updates during execution
- Results are displayed after all battles complete
- Random Pokemon selection uses same Pokemon for all battles (consistent statistics)
- Uses same battle logic as console debugger for consistency

## Future Enhancements

- Real-time results update during execution
- Export results to CSV/JSON
- Charts/graphs for statistics visualization
- Save/load configurations
- Compare multiple configurations side-by-side

