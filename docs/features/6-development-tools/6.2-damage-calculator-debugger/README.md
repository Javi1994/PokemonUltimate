# Sub-Feature 6.2: Damage Calculator Debugger

> Debugger tab for step-by-step damage calculation pipeline visualization.

**Sub-Feature Number**: 6.2  
**Parent Feature**: Feature 6: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Damage Calculator Debugger visualizes the damage calculation pipeline step-by-step, showing how each multiplier and modifier affects the final damage value. This helps developers understand and debug damage calculations.

## Features

- Select attacker Pokemon
- Select defender Pokemon
- Select move
- Set level
- Display each step of damage pipeline:
  1. Base Damage
  2. Critical Hit (yes/no, multiplier)
  3. Random Factor (0.85-1.0)
  4. STAB multiplier
  5. Ability modifiers
  6. Item modifiers
  7. Weather modifiers
  8. Terrain modifiers
  9. Screen modifiers
  10. Type effectiveness
  11. Burn penalty
- Show final damage and HP percentage
- Show damage range (min/max with random factor)

## Dependencies

- **Feature 2.4**: Damage Calculation Pipeline

## Implementation

**Runner**: `DamageCalculatorRunner`  
**Tab**: `DamageCalculatorDebuggerTab`

## Use Cases

See parent feature's [use_cases.md](../use_cases.md#62-damage-calculator-debugger) for complete list.

## Related Documents

- **[Parent Feature](../README.md)** - Feature 6: Development Tools
- **[Architecture](../architecture.md)** - Technical specification
- **[Roadmap](../roadmap.md#phase-62-damage-calculator-debugger--planned)** - Implementation plan

---

**Last Updated**: January 2025

