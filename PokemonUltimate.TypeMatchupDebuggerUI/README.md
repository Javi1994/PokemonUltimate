# PokemonUltimate.TypeMatchupDebuggerUI

Visual Windows Forms application for testing type effectiveness combinations and viewing the complete type chart.

## Purpose

This debugger focuses specifically on **Type Matchups**:
- Test single type vs single type effectiveness
- Test single type vs dual type effectiveness
- View complete type chart for any attacking type
- See effectiveness breakdowns for dual-type defenders
- Verify super effective combinations (2x, 4x)
- Verify not very effective combinations (0.5x, 0.25x)
- Verify type immunities (0x)

## Features

- **Visual Configuration**: Select attacking type, defender primary type, and optional secondary type using dropdowns
- **Instant Calculation**: Click "Calculate Effectiveness" to see results immediately
- **Effectiveness Breakdown**: For dual-type defenders, see how each type contributes to the final effectiveness
- **Complete Type Chart**: View effectiveness of the selected attacking type against all 18 Pokemon types
- **Color-Coded Results**: 
  - Red = Immune (0x)
  - Orange = Not Very Effective (< 1x)
  - Black = Normal (1x)
  - Green = Super Effective (> 1x)

## Usage

### Running the Application

```bash
dotnet run --project PokemonUltimate.TypeMatchupDebuggerUI
```

Or build and run the executable:
```bash
dotnet build PokemonUltimate.TypeMatchupDebuggerUI
# Then run the .exe from bin/Debug/net8.0-windows/
```

### Using the UI

1. **Select Attacking Type**: Choose the type of the attacking move from the dropdown
2. **Select Defender Primary Type**: Choose the primary type of the defending Pokemon
3. **Select Defender Secondary Type**: Choose the secondary type (or select "(None)" for single-type Pokemon)
4. **Click "Calculate Effectiveness"**: View the result and complete type chart

### Example Scenarios

**Test 1: Single Type Matchup**
- Attacking Type: Fire
- Defender Primary Type: Grass
- Defender Secondary Type: (None)
- Result: Super Effective (2x)

**Test 2: Dual Type Matchup**
- Attacking Type: Electric
- Defender Primary Type: Water
- Defender Secondary Type: Flying
- Result: Super Effective (4x)
- Breakdown: Electric vs Water (2x) × Electric vs Flying (2x) = 4x

**Test 3: Type Immunity**
- Attacking Type: Ground
- Defender Primary Type: Electric
- Defender Secondary Type: Flying
- Result: IMMUNE (0x)
- Breakdown: Ground vs Electric (2x) × Ground vs Flying (0x) = 0x

## Architecture

- **MainForm.cs**: Windows Forms UI with configuration controls and results display
- **Program.cs**: Application entry point
- Uses `TypeEffectiveness` from `PokemonUltimate.Core` for all calculations

## Differences from Console Version

- **Visual Interface**: Easy-to-use dropdowns instead of editing code
- **Instant Feedback**: See results immediately without recompiling
- **Complete Type Chart**: Always visible table showing effectiveness against all types
- **Better Visualization**: Color-coded results for quick understanding

