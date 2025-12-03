# 📋 Prompt Template: Edge Case Discovery

> Use this template to discover and test edge cases for a feature.

---

## Template

```
## Edge Case Analysis: [FEATURE NAME]

### Context
Feature location: [file path]
Related tests: [test file path]

### Discovery Mode
Act as a QA engineer specialized in finding bugs. Analyze [FEATURE] for:

1. **Boundary Values**
   - Minimum/maximum values
   - Zero, negative, overflow
   - Empty collections

2. **Invalid Inputs**
   - Null references
   - Wrong types
   - Malformed data

3. **State Transitions**
   - Invalid state combinations
   - Race conditions
   - Incomplete operations

4. **Real-World Scenarios**
   - Actual game situations
   - Edge cases from official games
   - Community-reported bugs

### Expected Output
1. List of edge cases discovered
2. Test implementations for each
3. Any missing functionality revealed by tests
4. Implementation of missing functionality
```

---

## Example: Stat Calculator Edge Cases

```
## Edge Case Analysis: StatCalculator

### Context
Feature location: PokemonUltimate.Core/Factories/StatCalculator.cs
Related tests: PokemonUltimate.Tests/Factories/StatCalculatorTests.cs

### Discovery Mode
Analyze StatCalculator for edge cases:

1. **Boundary Values**
   - Level 1 and Level 100
   - IV 0 and IV 31
   - EV 0 and EV 252
   - Base stat 1 (Shedinja HP) and base stat 255

2. **Real-World Scenarios**
   - Verify Pikachu Lv50 stats match official games
   - Verify Shedinja always has 1 HP
   - Verify Blissey max HP at level 100

3. **Invalid Inputs**
   - Negative base stats
   - Level 0 or Level 101
   - IV > 31, EV > 252
```

---

## Quick Edge Case Prompt

```
Find edge cases for [FEATURE] that could cause bugs.
Focus on: boundaries, nulls, real Pokemon examples.
Write tests. If tests reveal missing functionality, implement it.
```
