# PokemonUltimate.BattleDemo

Visual AI vs AI battle simulator for testing and demonstrating the combat system.

## Overview

This is a simple console application that demonstrates the Pokemon battle system by running AI vs AI battles with visual output. It's perfect for:

- Testing battle mechanics
- Observing AI behavior
- Demonstrating the combat system
- Debugging battle flow

## Features

- **Visual Battle Display**: Shows HP bars, status effects, and turn-by-turn progress
- **Multiple Scenarios**: Pre-configured battle scenarios
- **AI Comparison**: Compare different AI strategies (Random vs Always Attack)
- **User-Controlled Pace**: Press ENTER to advance each turn at your own speed
- **Skip Option**: Press 'q' during a battle to skip to the next scenario

## Running the Demo

```bash
dotnet run --project PokemonUltimate.BattleDemo
```

## Scenarios

The demo includes 4 pre-configured scenarios:

1. **Basic 1v1**: Pikachu vs Charmander (Random AI vs Random AI)
2. **Type Advantage**: Squirtle vs Charmander (Water vs Fire)
3. **Multiple Pokemon**: 3 vs 3 battle with party switching
4. **Strategy Comparison**: Random AI vs Always Attack AI

## Customization

You can easily modify `Program.cs` to:

- Change Pokemon species and levels
- Use different AI implementations
- Adjust battle rules (slots, max turns)
- Add new scenarios

## Example Output

```
════════════════════════════════════════════════════════════════════════════════
TURN 1
════════════════════════════════════════════════════════════════════════════════

[PLAYER]
  Pikachu Lv.50 [██████████████████████████████] 150/150

[ENEMY]
  Charmander Lv.50 [██████████████████████████████] 150/150

  → Pikachu used Thunderbolt!
  → Charmander: [████████████████░░░░░░░░░░░░] 90/150 HP
```

## Architecture

- **ConsoleBattleView**: Implements `IBattleView` to display battles in the console
- **Program.cs**: Contains battle scenarios and orchestration
- Uses the same `CombatEngine` and `IActionProvider` system as the main game

## Controls

- **ENTER**: Execute the next turn
- **'q'**: Skip current battle and move to next scenario
- **Any key**: Continue to next scenario after battle ends

## Notes

- Battles wait for user input before executing each turn
- You control the pace - take your time to observe each turn
- The demo uses `RandomAI` and `AlwaysAttackAI` implementations

