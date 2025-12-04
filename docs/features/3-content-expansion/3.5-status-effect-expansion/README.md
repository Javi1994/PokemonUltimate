# Sub-Feature 3.5: Status Effect Expansion

> Status effects catalog (15 statuses complete).

**Sub-Feature Number**: 3.5  
**Parent Feature**: Feature 3: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Status Effect Expansion covers all status conditions that can affect Pokemon in battle:
- **Persistent Status**: Burn, Paralysis, Sleep, Poison, Badly Poisoned, Freeze
- **Volatile Status**: Confusion, Attract, Flinch, Leech Seed, Curse, Encore, Taunt, Torment, Disable

**Status**: ✅ Complete (15 statuses)

## Current Content

### Persistent Status (6)
- **Burn** - Fire-type status, reduces Attack by 50%, deals damage each turn
- **Paralysis** - Electric-type status, reduces Speed by 50%, may prevent action
- **Sleep** - Prevents action for 1-3 turns
- **Poison** - Poison-type status, deals damage each turn
- **Badly Poisoned** - Poison-type status, deals increasing damage each turn
- **Freeze** - Ice-type status, prevents action (may thaw)

### Volatile Status (9)
- **Confusion** - May hit self instead of target
- **Attract** - May not attack opposite gender
- **Flinch** - Prevents action for one turn
- **Leech Seed** - Drains HP each turn
- **Curse** - Ghost-type curse effect
- **Encore** - Forces repeated move use
- **Taunt** - Prevents status moves
- **Torment** - Prevents repeated move use
- **Disable** - Prevents specific move use

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](../../architecture.md)** | Status effect catalog design |
| **[Use Cases](../../use_cases.md)** | Status effect scenarios |
| **[Roadmap](../../roadmap.md)** | Status effect expansion plan |
| **[Testing](../../testing.md)** | Status effect testing strategy |
| **[Code Location](../../code_location.md)** | Where status catalog code lives |

## Related Sub-Features

- **[3.1: Pokemon Expansion](../3.1-pokemon-expansion/)** - Pokemon can have status effects
- **[3.2: Move Expansion](../3.2-move-expansion/)** - Moves can inflict status effects
- **[3.4: Ability Expansion](../3.4-ability-expansion/)** - Abilities can prevent/remove status effects

## Related Documents

- **[Parent Feature README](../README.md)** - Overview of Content Expansion
- **[Feature 1.5: Status Effect Data](../../1-game-data/1.5-status-effect-data/README.md)** - Status effect data structure
- **[Feature 2.8: End-of-Turn Effects](../../2-combat-system/2.8-end-of-turn-effects/architecture.md)** - How status effects work in combat

## Quick Links

- **Key Classes**: `StatusCatalog`, `StatusEffectData`, `StatusEffectBuilder`
- **Status**: ✅ Complete (15 statuses)
- **Location**: `PokemonUltimate.Content/Catalogs/Status/StatusCatalog.cs`

---

**Last Updated**: 2025-01-XX

