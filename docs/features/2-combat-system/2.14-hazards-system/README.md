# Sub-Feature 2.14: Hazards System

> Stealth Rock, Spikes, etc. - Entry hazards system.

**Sub-Feature Number**: 2.14  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Hazards System implements entry hazards that damage Pokemon when they switch in:
- **Hazard Types**: Stealth Rock, Spikes, Toxic Spikes, Sticky Web
- **Hazard Effects**: Damage on switch-in, type effectiveness
- **Hazard Stacking**: Multiple layers of hazards

## Current Status

- ✅ **Core Complete**: Hazards system fully implemented
- ✅ **Data Ready**: HazardData blueprint exists
- ✅ **Tracking**: BattleSide hazard tracking implemented
- ✅ **Processing**: EntryHazardProcessor processes all hazards on switch-in
- ✅ **Integration**: Integrated with SwitchAction for automatic processing
- ⏳ **Pending**: Hazard removal actions (Rapid Spin, Defog) - requires move-specific implementation

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Technical specification (when implemented) |
| **[Use Cases](../../use_cases.md)** | Hazard scenarios |
| **[Roadmap](../../roadmap.md#phase-214-hazards-system)** | Implementation plan |
| **[Testing](../../testing.md)** | Testing strategy |
| **[Code Location](../../code_location.md)** | Where code will live |

## Related Sub-Features

- **[2.5: Combat Actions](../2.5-combat-actions/)** - Hazards applied via actions
- **[2.6: Combat Engine](../2.6-combat-engine/)** - Hazards trigger on switch-in

## Implementation Details

### Hazard Types Implemented

1. **Spikes** - Deals percentage HP damage based on layers (1-3 layers)
   - 1 layer: 12.5% max HP
   - 2 layers: 16.67% max HP
   - 3 layers: 25% max HP
   - Immunity: Flying types and Levitate

2. **Stealth Rock** - Deals damage based on type effectiveness vs Rock
   - Base: 12.5% max HP
   - Modified by type effectiveness (0.25x to 4x)
   - Affects all Pokemon including Flying types

3. **Toxic Spikes** - Applies status based on layers (1-2 layers)
   - 1 layer: Poison status
   - 2 layers: Badly Poisoned status
   - Absorption: Poison types absorb and remove spikes
   - Immunity: Flying types and Levitate

4. **Sticky Web** - Lowers Speed by 1 stage on entry
   - Immunity: Flying types and Levitate
   - Contrary support: Reverses to +1 Speed

### Code Location

- **Tracking**: `PokemonUltimate.Combat.Field.BattleSide` - Hazard tracking methods
- **Processing**: `PokemonUltimate.Combat.Engine.EntryHazardProcessor` - Processes hazards on switch-in
- **Integration**: `PokemonUltimate.Combat.Actions.SwitchAction` - Automatic hazard processing
- **Tests**: `PokemonUltimate.Tests.Systems.Combat.Field.EntryHazardsTests` (12 tests)
- **Tests**: `PokemonUltimate.Tests.Systems.Combat.Engine.EntryHazardProcessorTests` (13 tests)

## Quick Links

- **Status**: ✅ Core Complete (hazard removal actions pending move-specific implementation)
- **Tests**: 25+ tests, all passing
- **Implementation**: Complete tracking, processing, and integration

---

**Last Updated**: 2025-01-XX

