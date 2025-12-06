# Feature 6: Development Tools - Testing

> Testing strategy for development tools and debuggers.

**Feature Number**: 6  
**Feature Name**: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Development tools are Windows Forms applications for testing and debugging. Testing focuses on:
- **UI Functionality** - Controls work correctly, data displays properly
- **Calculation Accuracy** - Results match underlying game systems
- **Integration** - Debuggers correctly use game data and combat systems

**Status**: ⏳ Planned (no automated tests yet, manual testing only)

## Testing Strategy

### Manual Testing

Development tools are primarily tested manually:
- Run debugger application
- Configure options
- Execute tests
- Verify results match expected values
- Compare with game system outputs

### Integration Testing

Verify debuggers correctly integrate with:
- **Feature 1: Game Data** - Pokemon data, stats, types
- **Feature 2: Combat System** - Damage pipeline, turn order, status effects

### Validation Testing

Verify calculations match:
- **StatCalculator** - Stat calculations match StatCalculator output
- **DamagePipeline** - Damage calculations match DamagePipeline output
- **TurnOrderResolver** - Turn order matches TurnOrderResolver output
- **Status Effects** - Status effects match game mechanics

## Test Structure

```
PokemonUltimate.Tests/
└── Systems/
    └── DevelopmentTools/              # Development tools tests (if automated)
        ├── StatCalculatorDebuggerTests.cs
        ├── DamageCalculatorDebuggerTests.cs
        ├── StatusEffectDebuggerTests.cs
        └── TurnOrderDebuggerTests.cs
```

## Test Types

### Functional Tests

**Purpose**: Test normal behavior and expected outcomes

**Examples**:
- `StatCalculatorDebugger_Calculate_DefaultConfiguration_ReturnsCorrectStats`
- `DamageCalculatorDebugger_Calculate_Move_ReturnsCorrectDamage`
- `StatusEffectDebugger_Apply_Burn_AppliesCorrectStatModifiers`
- `TurnOrderDebugger_Calculate_DifferentSpeeds_ReturnsCorrectOrder`

### Edge Case Tests

**Purpose**: Test boundary conditions, invalid inputs, special cases

**Examples**:
- `StatCalculatorDebugger_Calculate_Level1_ReturnsMinimumStats`
- `StatCalculatorDebugger_Calculate_Level100_ReturnsMaximumStats`
- `StatCalculatorDebugger_Calculate_MaxIVsEVs_ReturnsMaximumStats`
- `DamageCalculatorDebugger_Calculate_ZeroPowerMove_ReturnsZeroDamage`
- `StatusEffectDebugger_Apply_MultiplePersistentStatuses_OnlyFirstApplied`
- `TurnOrderDebugger_Calculate_SpeedTie_ResolvesRandomly`

### Integration Tests

**Purpose**: Test integration with game systems

**Examples**:
- `StatCalculatorDebugger_Calculate_MatchesStatCalculator`
- `DamageCalculatorDebugger_Calculate_MatchesDamagePipeline`
- `TurnOrderDebugger_Calculate_MatchesTurnOrderResolver`
- `StatusEffectDebugger_Apply_MatchesGameMechanics`

## Manual Testing Checklist

### Stat Calculator Debugger (6.1)

- [ ] Select Pokemon species
- [ ] Set level (1-100)
- [ ] Select Nature
- [ ] Set IVs (0-31)
- [ ] Set EVs (0-252, max 510 total)
- [ ] Calculate stats
- [ ] Verify stats match StatCalculator output
- [ ] View stat breakdown
- [ ] Compare two configurations

### Damage Calculator Debugger (6.2)

- [ ] Select attacker Pokemon
- [ ] Select defender Pokemon
- [ ] Select move
- [ ] Set level
- [ ] Calculate damage
- [ ] Verify damage matches DamagePipeline output
- [ ] View pipeline steps
- [ ] View damage range
- [ ] Force critical hit
- [ ] Use fixed random value

### Status Effect Debugger (6.3)

- [ ] Select Pokemon
- [ ] Apply persistent status
- [ ] Apply volatile status
- [ ] View stat modifications
- [ ] View damage per turn
- [ ] Test status interactions
- [ ] Test status removal

### Turn Order Debugger (6.4)

- [ ] Configure multiple Pokemon
- [ ] Set different speeds
- [ ] Set move priorities
- [ ] Apply speed modifiers
- [ ] Calculate turn order
- [ ] Verify order matches TurnOrderResolver output
- [ ] View speed calculation breakdown

## Validation Against Game Systems

### StatCalculator Validation

Compare debugger output with:
```csharp
var calculator = StatCalculator.Default;
int hp = calculator.CalculateHP(baseHP, level, iv, ev);
int attack = calculator.CalculateStat(baseAttack, level, nature, Stat.Attack, iv, ev);
// ... etc
```

### DamagePipeline Validation

Compare debugger output with:
```csharp
var pipeline = new DamagePipeline();
var context = pipeline.Calculate(attacker, defender, move, field);
int damage = context.FinalDamage;
```

### TurnOrderResolver Validation

Compare debugger output with:
```csharp
var resolver = new TurnOrderResolver(randomProvider);
var sortedActions = resolver.SortActions(actions, field);
```

## Future: Automated Testing

If automated testing is added:
- Unit tests for Runner classes
- Integration tests with game systems
- UI tests for UserControl classes
- Validation tests comparing outputs

---

**Last Updated**: January 2025

