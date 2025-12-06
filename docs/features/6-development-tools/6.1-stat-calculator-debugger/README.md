# Sub-Feature 6.1: Stat Calculator Debugger

> Debugger tab for calculating and visualizing Pokemon stats with different configurations.

**Sub-Feature Number**: 6.1  
**Parent Feature**: Feature 6: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Stat Calculator Debugger allows developers to calculate and visualize Pokemon stats with different configurations (level, Nature, IVs, EVs). It provides detailed breakdowns of how each component contributes to the final stat values.

## Features

- Select Pokemon species
- Set level (1-100)
- Configure Nature
- Configure IVs (0-31 per stat)
- Configure EVs (0-252 per stat, 510 total)
- Display all calculated stats (HP, Attack, Defense, SpAttack, SpDefense, Speed)
- Show breakdown of calculation (base, IV, EV, Nature multiplier)
- Compare different configurations side-by-side

## Dependencies

- **Feature 1.16**: Factories & Calculators (StatCalculator)

## Implementation

**Runner**: `StatCalculatorRunner`  
**Tab**: `StatCalculatorDebuggerTab`

## Use Cases

See parent feature's [use_cases.md](../use_cases.md#61-stat-calculator-debugger) for complete list.

## Related Documents

- **[Parent Feature](../README.md)** - Feature 6: Development Tools
- **[Architecture](../architecture.md)** - Technical specification
- **[Roadmap](../roadmap.md#phase-61-stat-calculator-debugger--in-progress)** - Implementation plan

---

**Last Updated**: January 2025

