# PokemonUltimate.TypeMatchupDebugger

Specialized debugger for testing type effectiveness combinations and the complete type chart.

## Purpose

This debugger focuses specifically on **Type Matchups**:
- Complete type chart verification
- Single type vs single type effectiveness
- Single type vs dual type effectiveness
- Super effective combinations (2x, 4x)
- Not very effective combinations (0.5x, 0.25x)
- Type immunities (0x)

## Usage

```bash
dotnet run --project PokemonUltimate.TypeMatchupDebugger
```

## Test Scenarios

### Test 1: Type Chart
Shows effectiveness of one attacking type against all defending types.
Example: Fire type attacks against all 18 types.

### Test 2: Dual Type Effectiveness
Tests how single attacking types interact with dual-type defenders:
- Super effective combinations (e.g., Electric vs Water/Flying = 4x)
- Normal effectiveness (e.g., Water vs Water/Flying = 1x)
- Immunities (e.g., Ground vs Electric/Flying = 0x)
- Not very effective (e.g., Grass vs Fire/Flying = 0.5x)

### Test 3: Super Effective Combinations
Lists common super effective matchups (2x and 4x).

### Test 4: Type Immunities
Lists all type immunities (0x effectiveness).

## Example Output

```
════════════════════════════════════════════════════════════════════════════════════════════════════════
TEST 2: Dual Type Effectiveness
════════════════════════════════════════════════════════════════════════════════════════════════════════

Electric vs Water/Flying (4x Super Effective)
  Effectiveness: 4.00x (Super Effective (4x))

Fire vs Grass/Poison (2x Super Effective)
  Effectiveness: 2.00x (Super Effective (2x))
```

## Customization

Edit `Program.cs` to:
- Test specific type combinations
- Add more test cases
- Verify type chart completeness
- Test edge cases

