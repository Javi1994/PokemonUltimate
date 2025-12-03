# Sub-Feature 2.10: Pipeline Hooks

> Stat modifiers, damage modifiers - Hooks into damage pipeline.

**Sub-Feature Number**: 2.10  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Pipeline Hooks allow abilities and items to modify damage and stats through the damage pipeline:
- **IStatModifier**: Interface for stat modifications
- **AbilityStatModifier**: Ability-based stat modifiers
- **ItemStatModifier**: Item-based stat modifiers
- **Damage Modifiers**: Modify damage through pipeline steps

## Current Status

- ✅ **Implemented**: Stat modifier system integrated with damage pipeline
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md#pipeline-hooks)** | Technical specification |
| **[Use Cases](../../use_cases.md)** | Modifier scenarios |
| **[Roadmap](../../roadmap.md#phase-210-pipeline-hooks)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Hooks integrate with pipeline
- **[2.9: Abilities & Items](../2.9-abilities-items/)** - Abilities/items use modifiers

## Quick Links

- **Key Classes**: `IStatModifier`, `AbilityStatModifier`, `ItemStatModifier`
- **Status**: ✅ Complete (Phase 2.10)

---

**Last Updated**: 2025-01-XX

