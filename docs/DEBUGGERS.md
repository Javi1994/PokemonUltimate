# Battle Debuggers - Specialized Testing Tools

> Collection of specialized debuggers for testing different aspects of the Pokemon battle system.

## Overview

The debugger suite consists of multiple specialized projects, each focused on testing a specific aspect of the game:

| Debugger | Purpose | Focus Area |
|----------|---------|------------|
| **[BattleDebugger](../PokemonUltimate.BattleDebugger/)** | General purpose | Multiple battles with statistics (move usage, status effects) |
| **[MoveDebugger](../PokemonUltimate.MoveDebugger/)** | Move testing | Multiple move tests with statistics (damage, effects, actions) |
| **[TypeMatchupDebugger](../PokemonUltimate.TypeMatchupDebugger/)** | Type effectiveness | Type chart, dual types, immunities |

## Quick Start

### Run All Debuggers

```bash
# General battle debugging
dotnet run --project PokemonUltimate.BattleDebugger

# Test Pokemon configurations
dotnet run --project PokemonUltimate.PokemonDebugger

# Test specific moves
dotnet run --project PokemonUltimate.MoveDebugger

# Test type effectiveness
dotnet run --project PokemonUltimate.TypeMatchupDebugger
```

## When to Use Each Debugger

### Use BattleDebugger When:
- ✅ You want to run multiple battles and see statistics
- ✅ You need move usage statistics (most used moves per Pokemon)
- ✅ You want status effect statistics (what effects are caused)
- ✅ You're debugging general battle mechanics
- ✅ You want to compare Pokemon matchups over many battles
- ✅ You need win/loss statistics

### Use MoveDebugger When:
- ✅ Testing specific moves multiple times
- ✅ Verifying move power averages and ranges
- ✅ Testing status effect chance percentages
- ✅ Verifying move effects and action generation
- ✅ Comparing damage output across multiple tests
- ✅ Analyzing what actions a move generates

### Use TypeMatchupDebugger When:
- ✅ Verifying type chart correctness
- ✅ Testing dual-type effectiveness
- ✅ Checking immunities
- ✅ Validating super effective combinations

## Shared Components

All debuggers share:
- **DebugBattleView**: Enhanced `IBattleView` with detailed debug output
- Common battle execution patterns
- Detailed damage calculation display

## Future Debuggers

Planned specialized debuggers:

- **AbilityDebugger** - Test abilities and their effects
- **ItemDebugger** - Test held items and their effects
- **StatusDebugger** - Test status effects and their interactions
- **WeatherDebugger** - Test weather effects
- **TerrainDebugger** - Test terrain effects

## Architecture

```
PokemonUltimate.BattleDebugger/
├── DebugBattleView.cs      # Shared debug view
├── Program.cs               # Random battle generator
└── README.md

PokemonUltimate.MoveDebugger/
├── DebugBattleView.cs      # Shared debug view
├── Program.cs               # Move-specific tests
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

4. **Statistics Collection** (BattleDebugger & MoveDebugger)
   - Move usage statistics
   - Status effect statistics
   - Action generation statistics
   - Win/loss/draw rates
   - Damage averages and ranges

## Contributing

When adding a new debugger:

1. Create new console project: `PokemonUltimate.[Name]Debugger`
2. Copy `DebugBattleView.cs` from `BattleDebugger`
3. Create specialized `Program.cs` for your focus area
4. Add to solution: `dotnet sln add PokemonUltimate.[Name]Debugger`
5. Create `README.md` documenting purpose and usage
6. Update this document with new debugger entry

## Notes

- All debuggers are console applications
- Battles are fully automated (AI vs AI)
- Debug information is displayed in real-time
- Each debugger can be run independently
- Debuggers are designed for development/debugging, not gameplay

---

**Last Updated**: 2025-01-XX

