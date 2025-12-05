# Sub-Feature 2.4: Damage Calculation Pipeline

> Modular damage calculation using pipeline pattern.

**Sub-Feature Number**: 2.4  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Damage Calculation Pipeline uses a modular pipeline pattern to calculate damage, avoiding a single giant function. Damage passes through multiple steps, each modifying the damage context.

**Key Components**:
- **DamagePipeline**: Orchestrates damage calculation (implements `IDamagePipeline`, post-refactor)
- **IDamagePipeline**: Pipeline interface for dependency injection (post-refactor)
- **DamageContext**: Mutable snapshot of attack event
- **DamageContextFactory**: Factory for creating DamageContext instances (post-refactor)
- **IDamageStep**: Individual pipeline steps (ability, item, type effectiveness, etc.)
- **IStatModifier**: Interface for stat and damage modifications from abilities/items
- **AbilityStatModifier**: Adapter for ability-based stat/damage modifiers
- **ItemStatModifier**: Adapter for item-based stat/damage modifiers

## Current Status

- ✅ **Implemented**: Complete pipeline with all core steps (refactored with DI and Factory Pattern, 2024-12-05)
- ✅ **Tested**: Comprehensive test coverage

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete technical specification |
| **[Use Cases](../../use_cases.md#damage-calculation)** | Damage calculation scenarios |
| **[Roadmap](../../roadmap.md#phase-24-damage-calculation-pipeline)** | Implementation details |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where the code lives |

## Related Sub-Features

- **[2.5: Combat Actions](../2.5-combat-actions/)** - Actions trigger damage calculation
- **[2.9: Abilities & Items](../2.9-abilities-items/)** - Abilities and items provide data for modifiers
- **[2.3: Turn Order Resolution](../2.3-turn-order-resolution/)** - Uses stat modifiers for speed calculation

## Quick Links

- **Key Classes**: `DamagePipeline`, `DamageContext`, `IDamageStep`, `IStatModifier`, `AbilityStatModifier`, `ItemStatModifier`
- **Status**: ✅ Complete (Phase 2.4)

---

**Last Updated**: January 2025 (Post-Refactoring: 2024-12-05)

