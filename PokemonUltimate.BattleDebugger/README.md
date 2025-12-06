# PokemonUltimate.BattleDebugger

Battle debugger with detailed information display for debugging and verification.

## Overview

This project creates random battles and displays comprehensive debug information to help verify that battle mechanics are working correctly. It's perfect for:

- **Debugging battle mechanics** - See exactly how damage is calculated
- **Verifying type effectiveness** - Confirm super effective/not very effective calculations
- **Checking STAB** - Verify Same Type Attack Bonus is applied correctly
- **Validating stat stages** - See how stat modifications affect battles
- **Testing random scenarios** - Generate random battles to find edge cases

## Specialized Debuggers

This is the **general purpose** battle debugger. For specialized testing, see:

- **[PokemonDebugger](../PokemonUltimate.PokemonDebugger/)** - Test specific Pokemon (stats, types, levels)
- **[MoveDebugger](../PokemonUltimate.MoveDebugger/)** - Test specific moves (effectiveness, power, category)
- **[AbilityDebugger](../PokemonUltimate.AbilityDebugger/)** - Test abilities and their effects
- **[TypeMatchupDebugger](../PokemonUltimate.TypeMatchupDebugger/)** - Test type effectiveness combinations
- **[ItemDebugger](../PokemonUltimate.ItemDebugger/)** - Test held items and their effects

## Features

### Detailed Damage Calculation Display

For every damaging move, the debugger shows:

- **Move Information**: Name, type, power, category
- **Attacker/Defender Info**: Names, levels, types
- **Stats**: Attack/Defense or Special Attack/Special Defense used
- **Base Damage**: Calculated base damage before multipliers
- **Multipliers Breakdown**:
  - Critical Hit (1.5x)
  - STAB (1.5x)
  - Type Effectiveness (0x, 0.25x, 0.5x, 1x, 2x, 4x)
  - Random Factor (0.85x - 1.0x)
- **Total Multiplier**: Final multiplier applied
- **Final Damage**: Calculated damage and actual HP change

### Complete Battle State Display

Each turn shows:

- **Pokemon Information**: Name, level, HP bar, current/max HP
- **Types**: Primary and secondary types
- **Status Effects**: Current status conditions
- **Stats**: All 6 stats (HP, Attack, Defense, Special Attack, Special Defense, Speed)
- **Stat Stages**: Current stat stage modifications
- **Moves**: All moves with current/max PP
- **Field Conditions**: Weather and terrain

### Turn Order Information

Shows:

- Action order
- Priority values
- Speed values
- Move targets

## Configuration

Edit `Program.cs` to configure:

```csharp
// Pokemon del jugador (null = aleatorio, pero será el mismo en todas las batallas)
static PokemonSpeciesData? PlayerPokemon = PokemonCatalog.Pikachu;

// Pokemon enemigo (null = aleatorio, pero será el mismo en todas las batallas)
static PokemonSpeciesData? EnemyPokemon = PokemonCatalog.Charmander;

// Nivel de ambos Pokemon
static int Level = 50;

// Número de batallas a ejecutar
static int NumberOfBattles = 100;

// Modo detallado (true) o resumen (false)
static bool DetailedOutput = false;
```

**Note**: If `PlayerPokemon` or `EnemyPokemon` is `null`, a random Pokemon will be selected **once** at the start and used for all battles. This ensures consistent statistics.

## Usage

### Run Single Battle (Detailed)

Set `DetailedOutput = true` and `NumberOfBattles = 1`:

```bash
dotnet run --project PokemonUltimate.BattleDebugger
```

### Run Multiple Battles (Summary Mode)

Set `DetailedOutput = false` and `NumberOfBattles = 100`:

```bash
dotnet run --project PokemonUltimate.BattleDebugger
```

This will run 100 battles and show summary statistics at the end.

## Example Output

```
════════════════════════════════════════════════════════════════════════════════════════════════════════
BATTLE #1
════════════════════════════════════════════════════════════════════════════════════════════════════════

[BATTLE SETUP]
Player: Pikachu Lv.55 (Electric)
Enemy:  Charmander Lv.52 (Fire)

════════════════════════════════════════════════════════════════════════════════════════════════════════
TURN 1
════════════════════════════════════════════════════════════════════════════════════════════════════════

[PLAYER SIDE]
  Slot 1: Pikachu Lv.55 [██████████████████████████████] 150/150 HP | Types: Electric | Stats: HP=150/150 ATK=85 DEF=60 SPATK=95 SPDEF=85 SPD=110
    Moves: Thunderbolt (PP:15/15) | Quick Attack (PP:30/30) | Thunder Wave (PP:20/20) | Iron Tail (PP:15/15)

[ENEMY SIDE]
  Slot 1: Charmander Lv.52 [██████████████████████████████] 145/145 HP | Types: Fire | Stats: HP=145/145 ATK=75 DEF=65 SPATK=80 SPDEF=70 SPD=75
    Moves: Ember (PP:25/25) | Scratch (PP:35/35) | Growl (PP:40/40) | Smokescreen (PP:20/20)

[TURN ORDER]
  1. Pikachu [Speed: 110] → Thunderbolt
  2. Charmander [Speed: 75] → Ember

[ACTION]
Pikachu uses Thunderbolt → Charmander

    [DAMAGE CALCULATION]
      Move: Thunderbolt (Electric) | Power: 90 | Category: Special
      Attacker: Pikachu Lv.55 (Electric)
      Defender: Charmander Lv.52 (Fire)
      Stats: Attack=95 | Defense=70
      Base Damage: 45.23
      Multipliers: STAB (1.5x) × SUPER EFFECTIVE (2x) × RANDOM (0.92x)
      Total Multiplier: 2.76x
      Final Damage: 125 HP
      HP Change: 145 → 20 (125 damage)

    Charmander: [████░░░░░░░░░░░░░░░░░░░░░░░░] 20/145 HP
```

## Architecture

- **DebugBattleView**: Enhanced `IBattleView` implementation with detailed debug output
- **Program.cs**: Main entry point that generates random battles and orchestrates debug display
- Intercepts `DamageAction` reactions to display calculation details

## Differences from BattleDemo

| Feature | BattleDemo | BattleDebugger |
|---------|-----------|----------------|
| Purpose | Visual demonstration | Debugging and verification |
| Information Detail | Basic (HP, status) | Comprehensive (all stats, calculations) |
| Damage Display | Simple effectiveness | Full calculation breakdown |
| Battle Generation | Predefined scenarios | Random battles |
| Use Case | Showcase features | Debug mechanics |

## Statistics Collection

When running multiple battles (`NumberOfBattles > 1`), the debugger collects:

### Battle Outcomes
- Player wins / losses / draws
- Win rate percentages
- Total battles completed

### Move Usage Statistics
- Most used moves per Pokemon
- Usage count and percentage for each move
- Top 5 moves displayed per Pokemon

### Status Effect Statistics
- Status effects caused per Pokemon
- Persistent status effects (Burn, Paralysis, Sleep, Poison, etc.)
- Volatile status effects (Flinch, Confusion, LeechSeed, etc.)
- Count and percentage for each effect

## Notes

- Battles are fully automated (AI vs AI)
- Uses `RandomAI` for both sides
- If Pokemon are set to `null`, they are randomly selected **once** at the start and reused for all battles
- Level is configurable (default: 50)
- All debug information is displayed in real-time during battle execution (when `DetailedOutput = true`)
- Summary mode (`DetailedOutput = false`) shows only final statistics for faster analysis
