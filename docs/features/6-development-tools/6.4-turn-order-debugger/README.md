# Sub-Feature 6.4: Turn Order Debugger

> Debugger tab for visualizing turn order determination with speed and priority.

**Sub-Feature Number**: 6.4  
**Parent Feature**: Feature 6: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Turn Order Debugger visualizes how turn order is determined, showing the interaction between priority, speed, and modifiers. This helps developers understand and debug turn order mechanics.

## Features

- Configure multiple Pokemon with different speeds
- Set move priorities
- Apply modifiers (Paralysis, Tailwind, stat stages)
- Display effective speed calculation
- Show final turn order
- Visualize priority vs speed sorting

## Dependencies

- **Feature 2.3**: Turn Order Resolution

## Implementation

**Runner**: `TurnOrderRunner`  
**Tab**: `TurnOrderDebuggerTab`

## Use Cases

See parent feature's [use_cases.md](../use_cases.md#64-turn-order-debugger) for complete list.

## Related Documents

- **[Parent Feature](../README.md)** - Feature 6: Development Tools
- **[Architecture](../architecture.md)** - Technical specification
- **[Roadmap](../roadmap.md#phase-64-turn-order-debugger--planned)** - Implementation plan

---

**Last Updated**: January 2025

