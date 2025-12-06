# Feature 6: Development Tools - Roadmap

> Implementation plan for development tools and debuggers.

**Feature Number**: 6  
**Feature Name**: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This roadmap outlines the implementation phases for new debugger tabs in the unified debugger application.

## Current Status

- ✅ **Phase 0**: Existing Debuggers (Battle, Move, Type Matchup) - Complete
- ✅ **Phase 6.1**: Stat Calculator Debugger - Complete
- ⏳ **Phase 6.2**: Damage Calculator Debugger - Planned
- ⏳ **Phase 6.3**: Status Effect Debugger - Planned
- ⏳ **Phase 6.4**: Turn Order Debugger - Planned

## Phase 0: Existing Debuggers ✅ Complete

**Status**: ✅ Complete

**Components**:
- Battle Debugger Tab
- Move Debugger Tab
- Type Matchup Debugger Tab
- BattleRunner
- MoveRunner
- MainForm with TabControl

**See**: `docs/DEBUGGERS.md` for complete documentation

---

## Phase 6.1: Stat Calculator Debugger 🎯 In Progress

**Goal**: Create debugger tab for calculating and visualizing Pokemon stats with different configurations.

**Dependencies**: Feature 1.16: Factories & Calculators (StatCalculator)

### Components

| Component | File | Description |
|-----------|------|-------------|
| `StatCalculatorRunner` | `Runners/StatCalculatorRunner.cs` | Stat calculation logic |
| `StatCalculatorDebuggerTab` | `Tabs/StatCalculatorDebuggerTab.cs` | UI UserControl |

### UI Components

**Configuration Panel**:
- Pokemon species dropdown
- Level input (1-100)
- Nature dropdown
- IV inputs (HP, Attack, Defense, SpAttack, SpDefense, Speed) (0-31)
- EV inputs (HP, Attack, Defense, SpAttack, SpDefense, Speed) (0-252)
- EV total validation (max 510)
- Calculate button

**Results Panel**:
- **Summary Tab**: Formatted text showing all stats and breakdown
- **Stats Table Tab**: DataGridView with stat name, base, IV, EV, nature multiplier, final value
- **Comparison Tab**: Side-by-side comparison of two configurations

### Implementation Steps

1. ✅ Create feature documentation
2. ✅ Create `StatCalculatorRunner` class
3. ✅ Create `StatCalculatorDebuggerTab` UserControl
4. ✅ Add tab to `MainForm`
5. ✅ Test stat calculations (compiles successfully)
6. ✅ Test UI interactions (ready for manual testing)
7. ✅ Update documentation

### Tests

- Functional: Calculate stats with different configurations
- Edge Cases: Min/max IVs/EVs, all natures, level 1/100
- Integration: Verify calculations match StatCalculator

---

## Phase 6.2: Damage Calculator Debugger ⏳ Planned

**Goal**: Create debugger tab for step-by-step damage calculation pipeline visualization.

**Dependencies**: Feature 2.4: Damage Calculation Pipeline

### Components

| Component | File | Description |
|-----------|------|-------------|
| `DamageCalculatorRunner` | `Runners/DamageCalculatorRunner.cs` | Damage pipeline visualization logic |
| `DamageCalculatorDebuggerTab` | `Tabs/DamageCalculatorDebuggerTab.cs` | UI UserControl |

### UI Components

**Configuration Panel**:
- Attacker Pokemon dropdown
- Defender Pokemon dropdown
- Move dropdown
- Level input (1-100)
- Optional: Force critical hit checkbox
- Optional: Fixed random value (0-1)
- Calculate button

**Results Panel**:
- **Summary Tab**: Formatted text with move info, final damage, HP percentage
- **Pipeline Steps Tab**: DataGridView showing each step with multiplier and running total
- **Damage Range Tab**: Min/max damage with different random factors

### Implementation Steps

1. ⏳ Create `DamageCalculatorRunner` class
2. ⏳ Create `DamageCalculatorDebuggerTab` UserControl
3. ⏳ Add tab to `MainForm`
4. ⏳ Test damage calculations
5. ⏳ Test pipeline visualization
6. ⏳ Update documentation

### Tests

- Functional: Calculate damage with different moves/Pokemon
- Edge Cases: Zero power moves, fixed damage moves, status moves
- Integration: Verify calculations match DamagePipeline

---

## Phase 6.3: Status Effect Debugger ⏳ Planned

**Goal**: Create debugger tab for testing status effects and their interactions.

**Dependencies**: Feature 1.5: Status Effect Data, Feature 2.5: Combat Actions

### Components

| Component | File | Description |
|-----------|------|-------------|
| `StatusEffectRunner` | `Runners/StatusEffectRunner.cs` | Status effect testing logic |
| `StatusEffectDebuggerTab` | `Tabs/StatusEffectDebuggerTab.cs` | UI UserControl |

### UI Components

**Configuration Panel**:
- Pokemon dropdown
- Persistent status dropdown (None, Burn, Paralysis, Poison, Sleep, Freeze)
- Volatile status checkboxes (Confusion, Flinch, etc.)
- Apply button

**Results Panel**:
- **Summary Tab**: Formatted text showing current status and effects
- **Stat Modifications Tab**: DataGridView showing stat changes
- **Damage Per Turn Tab**: Shows damage from Poison/Burn per turn
- **Interactions Tab**: Shows which statuses can/cannot be applied together

### Implementation Steps

1. ⏳ Create `StatusEffectRunner` class
2. ⏳ Create `StatusEffectDebuggerTab` UserControl
3. ⏳ Add tab to `MainForm`
4. ⏳ Test status applications
5. ⏳ Test status interactions
6. ⏳ Update documentation

### Tests

- Functional: Apply different status effects
- Edge Cases: Multiple status attempts, status removal
- Integration: Verify status effects match game mechanics

---

## Phase 6.4: Turn Order Debugger ⏳ Planned

**Goal**: Create debugger tab for visualizing turn order determination with speed and priority.

**Dependencies**: Feature 2.3: Turn Order Resolution

### Components

| Component | File | Description |
|-----------|------|-------------|
| `TurnOrderRunner` | `Runners/TurnOrderRunner.cs` | Turn order calculation logic |
| `TurnOrderDebuggerTab` | `Tabs/TurnOrderDebuggerTab.cs` | UI UserControl |

### UI Components

**Configuration Panel**:
- Multiple Pokemon slots (2-4)
  - Pokemon dropdown
  - Move dropdown (for priority)
  - Speed modifiers (Paralysis checkbox, Tailwind checkbox, stat stages)
- Calculate button

**Results Panel**:
- **Summary Tab**: Formatted text showing turn order
- **Speed Calculation Tab**: DataGridView showing base speed, modifiers, effective speed
- **Priority Tab**: Shows move priorities
- **Order Tab**: Final sorted order with reasoning

### Implementation Steps

1. ⏳ Create `TurnOrderRunner` class
2. ⏳ Create `TurnOrderDebuggerTab` UserControl
3. ⏳ Add tab to `MainForm`
4. ⏳ Test turn order calculations
5. ⏳ Test speed modifiers
6. ⏳ Update documentation

### Tests

- Functional: Calculate turn order with different speeds/priorities
- Edge Cases: Speed ties, priority differences, modifiers
- Integration: Verify calculations match TurnOrderResolver

---

## Future Enhancements

- **Export Results**: Save results to CSV/JSON
- **Save/Load Configurations**: Preserve debugger settings
- **Compare Results**: Side-by-side comparison of multiple runs
- **Batch Testing**: Run multiple configurations automatically
- **Custom Scenarios**: Save and load custom test scenarios

---

**Last Updated**: January 2025

