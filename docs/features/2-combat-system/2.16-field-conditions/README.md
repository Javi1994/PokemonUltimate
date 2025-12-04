# Sub-Feature 2.16: Field Conditions

> Screens, Tailwind, protections - Field condition system.

**Sub-Feature Number**: 2.16  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Field Conditions implements side-specific conditions:
- **Screens**: Reflect, Light Screen, Aurora Veil
- **Speed Modifiers**: Tailwind, Trick Room
- **Protections**: Safeguard, Mist
- **Other Conditions**: Various side effects

## Current Status

- ✅ **Core Complete**: Field conditions implemented
  - Side condition tracking in BattleSide
  - Screen damage reduction (Reflect, Light Screen, Aurora Veil)
  - Tailwind speed multiplier
  - Safeguard status protection
  - Mist stat reduction protection
  - Side condition duration management
  - SetSideConditionAction for applying conditions
- ✅ **Data Ready**: SideConditionData blueprint exists

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Technical specification (when implemented) |
| **[Use Cases](../../use_cases.md)** | Field condition scenarios |
| **[Roadmap](../../roadmap.md#phase-216-field-conditions)** | Implementation plan |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where code will live |

## Related Sub-Features

- **[2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Screens modify damage
- **[2.3: Turn Order Resolution](../2.3-turn-order-resolution/)** - Speed modifiers affect turn order

## Quick Links

- **Status**: ✅ Core Complete (Phase 2.16)
- **Tests**: 40+ tests covering all side condition mechanics
- **Key Classes**: 
  - `BattleSide` - Side condition tracking
  - `ScreenStep` - Screen damage reduction in DamagePipeline
  - `TurnOrderResolver` - Tailwind speed multiplier
  - `ApplyStatusAction` - Safeguard protection
  - `StatChangeAction` - Mist protection
  - `SetSideConditionAction` - Applying side conditions

---

**Last Updated**: 2025-01-XX

