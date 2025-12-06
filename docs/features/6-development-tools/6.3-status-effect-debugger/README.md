# Sub-Feature 6.3: Status Effect Debugger

> Debugger tab for testing status effects and their interactions.

**Sub-Feature Number**: 6.3  
**Parent Feature**: Feature 6: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Status Effect Debugger allows developers to test status effects and their interactions, view stat modifications, and understand how statuses affect Pokemon in battle.

## Features

- Select Pokemon
- Apply persistent status (Burn, Paralysis, Poison, Sleep, Freeze)
- Apply volatile status (Confusion, Flinch, etc.)
- View stat modifications (Burn reduces Attack, Paralysis reduces Speed)
- Test interactions (cannot apply multiple persistent statuses)
- View damage per turn (Poison/Burn)
- Test status removal conditions

## Dependencies

- **Feature 1.5**: Status Effect Data
- **Feature 2.5**: Combat Actions

## Implementation

**Runner**: `StatusEffectRunner`  
**Tab**: `StatusEffectDebuggerTab`

## Use Cases

See parent feature's [use_cases.md](../use_cases.md#63-status-effect-debugger) for complete list.

## Related Documents

- **[Parent Feature](../README.md)** - Feature 6: Development Tools
- **[Architecture](../architecture.md)** - Technical specification
- **[Roadmap](../roadmap.md#phase-63-status-effect-debugger--planned)** - Implementation plan

---

**Last Updated**: January 2025

