# Battle Debuggers - Specialized Testing Tools

> Collection of specialized debuggers for testing different aspects of the Pokemon battle system.

## Overview

The debugger suite consists of multiple specialized projects, each focused on testing a specific aspect of the game:

| Debugger | Purpose | Focus Area |
|----------|---------|------------|
| **[BattleDebuggerUI](../PokemonUltimate.BattleDebuggerUI/)** | Windows Forms | Visual interface for battle statistics (move usage, status effects) |
| **[MoveDebuggerUI](../PokemonUltimate.MoveDebuggerUI/)** | Windows Forms | Visual interface for move testing statistics (damage, effects, actions) |
| **[TypeMatchupDebugger](../PokemonUltimate.TypeMatchupDebugger/)** | Console | Type chart, dual types, immunities |

## Quick Start

### Run All Debuggers

```bash
# Visual battle debugging with statistics
dotnet run --project PokemonUltimate.BattleDebuggerUI

# Visual move testing with statistics
dotnet run --project PokemonUltimate.MoveDebuggerUI

# Test type effectiveness (console)
dotnet run --project PokemonUltimate.TypeMatchupDebugger
```

## When to Use Each Debugger

### Use BattleDebuggerUI When:
- ✅ You want to run multiple battles and see statistics
- ✅ You need move usage statistics (most used moves per Pokemon)
- ✅ You want status effect statistics (what effects are caused)
- ✅ You're debugging general battle mechanics
- ✅ You want to compare Pokemon matchups over many battles
- ✅ You need win/loss statistics
- ✅ You prefer visual interfaces with easy configuration

### Use MoveDebuggerUI When:
- ✅ Testing specific moves multiple times
- ✅ Verifying move power averages and ranges
- ✅ Testing status effect chance percentages
- ✅ Verifying move effects and action generation
- ✅ Comparing damage output across multiple tests
- ✅ Analyzing what actions a move generates
- ✅ You prefer visual interfaces with easy configuration

### Use TypeMatchupDebugger When:
- ✅ Verifying type chart correctness
- ✅ Testing dual-type effectiveness
- ✅ Checking immunities
- ✅ Validating super effective combinations

## Shared Components

UI debuggers use:
- **BattleRunner** / **MoveRunner**: Reusable execution logic extracted from console debuggers
- Common battle execution patterns
- Statistics collection and tracking

## Future Debuggers

Planned specialized debuggers:

- **AbilityDebugger** - Test abilities and their effects
- **ItemDebugger** - Test held items and their effects
- **StatusDebugger** - Test status effects and their interactions
- **WeatherDebugger** - Test weather effects
- **TerrainDebugger** - Test terrain effects

## Architecture

```
PokemonUltimate.BattleDebuggerUI/
├── MainForm.cs              # Windows Forms UI
├── BattleRunner.cs          # Reusable battle execution logic
└── README.md

PokemonUltimate.MoveDebuggerUI/
├── MainForm.cs              # Windows Forms UI
├── MoveRunner.cs            # Reusable move testing logic
└── README.md

PokemonUltimate.TypeMatchupDebugger/
├── DebugBattleView.cs      # Shared debug view
├── Program.cs               # Type effectiveness tests
└── README.md
```

## Common Features

All debuggers provide:

1. **Detailed Damage Calculations**
   - Base damage
   - Multipliers breakdown (CRIT, STAB, Type Effectiveness, Random)
   - Final damage

2. **Complete Battle State**
   - Pokemon stats
   - Stat stages
   - Status effects
   - Moves and PP

3. **Turn-by-Turn Information**
   - Turn order
   - Action details
   - Reaction chains

4. **Statistics Collection** (BattleDebuggerUI & MoveDebuggerUI)
   - Move usage statistics
   - Status effect statistics
   - Action generation statistics
   - Win/loss/draw rates
   - Damage averages and ranges
   - Visual progress tracking

## Contributing

When adding a new debugger:

1. Create new Windows Forms project: `PokemonUltimate.[Name]DebuggerUI`
2. Create `MainForm.cs` with configuration controls and result tabs
3. Create `[Name]Runner.cs` with reusable execution logic
4. Add to solution: `dotnet sln add PokemonUltimate.[Name]DebuggerUI`
5. Create `README.md` documenting purpose and usage
6. Update this document with new debugger entry

## Notes

- UI debuggers use Windows Forms for visual interfaces
- Battles/tests are fully automated (AI vs AI)
- Progress tracking during execution
- Results displayed in tables and formatted text
- Each debugger can be run independently
- Debuggers are designed for development/debugging, not gameplay

---

**Last Updated**: 2025-01-XX

