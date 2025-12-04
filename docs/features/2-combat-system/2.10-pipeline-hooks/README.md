# Sub-Feature 2.10: Pipeline Hooks (CONSOLIDATED)

> ⚠️ **CONSOLIDATED INTO 2.4** - This sub-feature has been consolidated into Sub-Feature 2.4: Damage Calculation Pipeline.

**Sub-Feature Number**: 2.10 (Historical - now part of 2.4)  
**Parent Feature**: Feature 2: Combat System  
**Status**: ✅ Consolidated into 2.4

## Consolidation Notice

The stat and damage modifier system (`IStatModifier`, `AbilityStatModifier`, `ItemStatModifier`) has been consolidated into **Sub-Feature 2.4: Damage Calculation Pipeline** because:

1. The modifiers are an **integral part** of the damage pipeline, not external hooks
2. They are implemented directly within the pipeline steps (`BaseDamageStep`, `AttackerAbilityStep`, `AttackerItemStep`)
3. This consolidation eliminates confusion about where the system is implemented

## Current Location

All stat and damage modifier functionality is now documented and implemented in:

- **[Sub-Feature 2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** - Complete implementation and documentation

## What Was Consolidated

The following components are part of Sub-Feature 2.4:

- `IStatModifier` - Interface for stat and damage modifications
- `AbilityStatModifier` - Adapter for ability-based modifiers
- `ItemStatModifier` - Adapter for item-based modifiers
- Stat modifier integration in `BaseDamageStep`
- Damage modifier integration in `AttackerAbilityStep` and `AttackerItemStep`
- Speed modifier integration in `TurnOrderResolver`

## Implemented Features

✅ Choice Band (+50% Attack)  
✅ Choice Specs (+50% SpAttack)  
✅ Choice Scarf (+50% Speed)  
✅ Life Orb (+30% damage)  
✅ Assault Vest (+50% SpDefense)  
✅ Eviolite (+50% Def/SpDef if can evolve)  
✅ Blaze/Torrent/Overgrow/Swarm (HP threshold damage multipliers)

---

**See**: **[Sub-Feature 2.4: Damage Calculation Pipeline](../2.4-damage-calculation-pipeline/)** for complete documentation.

**Last Updated**: 2025-01-XX (Consolidated)
