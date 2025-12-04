# Feature 3: Content Expansion - Roadmap

> Step-by-step guide for expanding game content (Pokemon, Moves, Items, Abilities, etc.)

**Feature Number**: 3  
**Feature Name**: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This roadmap defines phases for expanding game content. Each phase adds new data while maintaining quality and consistency with existing patterns.

**Current Status**:
- **Pokemon**: 26/151 (Gen 1 only) 🎯
- **Moves**: 36 (12 types) 🎯
- **Items**: 23 (15 Held items + 8 Berries) 🎯
- **Abilities**: 35 (25 Gen 3 + 10 Additional) 🎯
- **Status Effects**: 15 (6 Persistent + 9 Volatile) ✅ Complete
- **Weather**: 9 ✅ Complete
- **Terrain**: 4 ✅ Complete
- **Hazards**: 4 ✅ Complete
- **Side Conditions**: 10 ✅ Complete
- **Field Effects**: 8 ✅ Complete

**Goal**: Expand content systematically to support full battle mechanics and diverse gameplay.

---

## Current Content Status

| Category | Current | Target | Priority |
|----------|---------|--------|----------|
| **Pokemon** | 26/151 (Gen 1) | 151+ (Gen 1-2) | High |
| **Moves** | 36 (12 types) | 100+ (all types) | High |
| **Items** | 23 | 50+ | Medium |
| **Abilities** | 35 | 50+ | Medium |
| **Status Effects** | 15 | 15 ✅ | Complete |
| **Weather** | 9 | 9 ✅ | Complete |
| **Terrain** | 4 | 4 ✅ | Complete |
| **Hazards** | 4 | 4 ✅ | Complete |
| **Side Conditions** | 10 | 10 ✅ | Complete |
| **Field Effects** | 8 | 8 ✅ | Complete |

---

## Content Expansion Phases

### Phase 3.1: Complete Gen 1 Pokemon ✅ IN PROGRESS

**Goal**: Add all remaining Gen 1 Pokemon (currently 26/151).

**Current**: 26 Pokemon (starters, evolutions, legendaries, key Pokemon)

**Remaining**: 125 Pokemon

**Priority**: High - Needed for diverse battles

**Approach**:
- Add Pokemon in batches of 20-30
- Focus on evolution lines first
- Add legendaries last
- Ensure all have abilities assigned

**Estimated Effort**: 4-6 hours per batch (20-30 Pokemon)

**Completion Checklist**:
- [ ] Batch 1: Early route Pokemon (#010-029) - 20 Pokemon
- [ ] Batch 2: Mid-game Pokemon (#030-059) - 30 Pokemon
- [ ] Batch 3: Late-game Pokemon (#060-099) - 40 Pokemon
- [ ] Batch 4: Final Pokemon (#100-149) - 50 Pokemon
- [ ] Batch 5: Legendaries (#150-151) - 2 Pokemon
- [ ] All Pokemon have abilities assigned
- [ ] All Pokemon have learnsets
- [ ] All evolution chains complete
- [ ] All Pokemon have Pokedex fields (Description, Category, Height, Weight, Color, Shape, Habitat)
- [ ] All Pokemon have gameplay fields (BaseExperienceYield, CatchRate, BaseFriendship, GrowthRate)
- [ ] Tests updated for new Pokemon

**Files to Create/Update**:
- `PokemonCatalog.Gen1.cs` (extend existing)

---

### Phase 3.2: Expand Move Coverage (All Types)

**Goal**: Add moves for all 18 types, focusing on commonly used moves.

**Current**: 36 moves (12 types covered)

**Target**: 100+ moves covering all types

**Priority**: High - Needed for diverse movepools

**Approach**:
- Add moves by type priority
- Focus on moves needed for current Pokemon learnsets
- Add signature moves for key Pokemon
- Ensure move effects are properly implemented

**Type Priority Order**:
1. **Normal** (expand) - Common moves, priority moves
2. **Fire** (expand) - More damage options
3. **Water** (expand) - More damage options
4. **Grass** (expand) - Status moves, healing
5. **Electric** (expand) - Paralysis moves
6. **Ground** (expand) - Entry hazard removal
7. **Psychic** (expand) - Status moves
8. **Ghost** (expand) - Status moves
9. **Rock** (expand) - Entry hazards
10. **Flying** (expand) - Priority moves
11. **Poison** (expand) - Status moves
12. **Dragon** (expand) - Damage moves
13. **Ice** (new) - Weather moves, damage
14. **Steel** (new) - Defensive moves
15. **Dark** (new) - Status moves
16. **Bug** (new) - Status moves
17. **Fighting** (new) - Damage moves
18. **Fairy** (new) - Status moves

**Move Categories to Add**:
- **Status Moves**: Protect, Detect, Substitute, Swords Dance, etc.
- **Priority Moves**: Extreme Speed, Sucker Punch, etc.
- **Entry Hazards**: Spikes, Stealth Rock, Toxic Spikes, Sticky Web
- **Weather Moves**: Sunny Day, Rain Dance, Sandstorm, Hail
- **Terrain Moves**: Electric Terrain, Grassy Terrain, etc.
- **Recovery Moves**: Recover, Rest, Wish, etc.
- **Multi-Hit Moves**: Double Slap, Bullet Seed, etc.
- **Multi-Turn Moves**: Solar Beam, Hyper Beam, Outrage, etc.

**Estimated Effort**: 2-3 hours per type (10-15 moves)

**Completion Checklist**:
- [ ] Normal type expanded (10+ moves)
- [ ] Fire type expanded (10+ moves)
- [ ] Water type expanded (10+ moves)
- [ ] Grass type expanded (10+ moves)
- [ ] Electric type expanded (5+ moves)
- [ ] Ground type expanded (5+ moves)
- [ ] Psychic type expanded (5+ moves)
- [ ] Ghost type expanded (5+ moves)
- [ ] Rock type expanded (5+ moves)
- [ ] Flying type expanded (5+ moves)
- [ ] Poison type expanded (5+ moves)
- [ ] Dragon type expanded (5+ moves)
- [ ] Ice type added (10+ moves)
- [ ] Steel type added (10+ moves)
- [ ] Dark type added (10+ moves)
- [ ] Bug type added (10+ moves)
- [ ] Fighting type added (10+ moves)
- [ ] Fairy type added (10+ moves)
- [ ] All moves have proper effects
- [ ] Tests updated for new moves

**Files to Create/Update**:
- `MoveCatalog.Normal.cs` (extend)
- `MoveCatalog.Fire.cs` (extend)
- `MoveCatalog.Water.cs` (extend)
- `MoveCatalog.Grass.cs` (extend)
- `MoveCatalog.Electric.cs` (extend)
- `MoveCatalog.Ground.cs` (extend)
- `MoveCatalog.Psychic.cs` (extend)
- `MoveCatalog.Ghost.cs` (extend)
- `MoveCatalog.Rock.cs` (extend)
- `MoveCatalog.Flying.cs` (extend)
- `MoveCatalog.Poison.cs` (extend)
- `MoveCatalog.Dragon.cs` (extend)
- `MoveCatalog.Ice.cs` (new)
- `MoveCatalog.Steel.cs` (new)
- `MoveCatalog.Dark.cs` (new)
- `MoveCatalog.Bug.cs` (new)
- `MoveCatalog.Fighting.cs` (new)
- `MoveCatalog.Fairy.cs` (new)
- `MoveCatalog.cs` (update InitializeAll)

---

### Phase 3.3: Expand Item Catalog

**Goal**: Add more held items, berries, and consumables.

**Current**: ~23 items (15 held items + 8 berries)

**Target**: 50+ items

**Priority**: Medium - Enhances battle variety

**Item Categories to Add**:

#### Held Items (Expand)
- **Type-Boosting Items**: All 18 types (currently 4)
- **Stat-Boosting Items**: Muscle Band, Wise Glasses, etc.
- **Recovery Items**: Shell Bell, Big Root, etc.
- **Defensive Items**: Light Clay, Smooth Rock, etc.
- **Status Items**: Flame Orb, Toxic Orb, etc.

#### Berries (Expand)
- **Status-Curing Berries**: All 7 status berries
- **HP Berries**: Oran, Sitrus, etc.
- **Stat Berries**: Pinch berries (Salac, Petaya, etc.)
- **Type-Resist Berries**: All 18 types

#### Consumables (New)
- **Battle Items**: X Attack, X Defense, etc.
- **Evolution Items**: Evolution stones, etc.

**Estimated Effort**: 1-2 hours per category

**Completion Checklist**:
- [ ] Type-boosting items (all 18 types)
- [ ] Stat-boosting items (5+ items)
- [ ] Recovery items (5+ items)
- [ ] Defensive items (5+ items)
- [ ] Status items (5+ items)
- [ ] Status-curing berries (7 berries)
- [ ] HP berries (5+ berries)
- [ ] Stat berries (5+ berries)
- [ ] Type-resist berries (18 berries)
- [ ] All items have proper effects
- [ ] Tests updated for new items

**Files to Create/Update**:
- `ItemCatalog.HeldItems.cs` (extend)
- `ItemCatalog.Berries.cs` (extend)
- `ItemCatalog.Consumables.cs` (new)

---

### Phase 3.4: Expand Ability Catalog

**Goal**: Add more abilities, especially Gen 4+ abilities.

**Current**: 35 abilities (Gen 3 + Additional)

**Target**: 50+ abilities

**Priority**: Medium - Enhances Pokemon uniqueness

**Ability Categories to Add**:

#### Gen 4 Abilities
- **Weather Abilities**: Snow Warning, Sand Force, etc.
- **Terrain Abilities**: Grassy Surge, Electric Surge, etc.
- **Stat Abilities**: Hustle, Guts, etc.

#### Gen 5+ Abilities
- **Complex Abilities**: Regenerator, Magic Guard, etc.
- **Type Abilities**: Adaptability, Technician, etc.

#### Battle-Relevant Abilities
- **Entry Abilities**: Intimidate ✅, Download, etc.
- **Damage Abilities**: Static ✅, Rough Skin, etc.
- **Status Abilities**: Immunity ✅, Limber ✅, etc.

**Estimated Effort**: 1-2 hours per generation

**Completion Checklist**:
- [ ] Gen 4 abilities (10+ abilities)
- [ ] Gen 5 abilities (10+ abilities)
- [ ] Gen 6+ abilities (10+ abilities)
- [ ] All abilities have proper triggers
- [ ] All abilities have proper effects
- [ ] Tests updated for new abilities

**Files to Create/Update**:
- `AbilityCatalog.Gen4.cs` (new)
- `AbilityCatalog.Gen5.cs` (new)
- `AbilityCatalog.Gen6.cs` (new)
- `AbilityCatalog.Additional.cs` (extend)

---

### Phase 3.5: Status Effect Expansion ✅ COMPLETE

**Goal**: Complete status effects catalog.

**Status**: ✅ Complete (15 statuses)

**Content**:
- **Persistent Status** (6): Burn, Paralysis, Sleep, Poison, Badly Poisoned, Freeze
- **Volatile Status** (9): Confusion, Attract, Flinch, Leech Seed, Curse, Encore, Taunt, Torment, Disable

**Files**:
- `StatusCatalog.cs` ✅

---

### Phase 3.6: Field Conditions Expansion ✅ COMPLETE

**Goal**: Complete field conditions catalogs.

**Status**: ✅ Complete (35 field conditions)

**Content**:
- **Weather** (9): Rain, Sun, Sandstorm, Hail, Snow, Heavy Rain, Extremely Harsh Sunlight, Strong Winds, Fog
- **Terrain** (4): Grassy Terrain, Electric Terrain, Psychic Terrain, Misty Terrain
- **Hazards** (4): Stealth Rock, Spikes, Toxic Spikes, Sticky Web
- **Side Conditions** (10): Reflect, Light Screen, Aurora Veil, Tailwind, Safeguard, Mist, Lucky Chant, Wide Guard, Quick Guard, Mat Block
- **Field Effects** (8): Trick Room, Magic Room, Wonder Room, Gravity, Ion Deluge, Fairy Lock, Mud Sport, Water Sport

**Files**:
- `WeatherCatalog.cs` ✅
- `TerrainCatalog.cs` ✅
- `HazardCatalog.cs` ✅
- `SideConditionCatalog.cs` ✅
- `FieldEffectCatalog.cs` ✅

---

### Phase 3.7: Gen 2 Pokemon

**Goal**: Add all Gen 2 Pokemon (Johto region, #152-251).

**Target**: 100 Pokemon

**Priority**: Medium - Expands Pokemon roster significantly

**Approach**:
- Add Pokemon in batches similar to Gen 1
- Focus on evolution lines
- Add new types (Dark, Steel)
- Add new abilities

**Estimated Effort**: 6-8 hours per batch (20-30 Pokemon)

**Completion Checklist**:
- [ ] Batch 1: Early Johto (#152-179) - 28 Pokemon
- [ ] Batch 2: Mid Johto (#180-209) - 30 Pokemon
- [ ] Batch 3: Late Johto (#210-239) - 30 Pokemon
- [ ] Batch 4: Final Johto (#240-251) - 12 Pokemon
- [ ] All Pokemon have abilities assigned
- [ ] All Pokemon have learnsets
- [ ] All evolution chains complete
- [ ] Tests updated for new Pokemon

**Files to Create/Update**:
- `PokemonCatalog.Gen2.cs` (new)
- `PokemonCatalog.cs` (update InitializeAll)

---

### Phase 3.8: Complete Move Coverage for Pokemon

**Goal**: Ensure all Pokemon have complete, accurate learnsets.

**Priority**: High - Needed for proper gameplay

**Approach**:
- Review each Pokemon's learnset
- Add missing level-up moves
- Add missing TM/HM moves (when TM system implemented)
- Add missing egg moves (when breeding implemented)
- Ensure move coverage (STAB, coverage, status)

**Estimated Effort**: 1-2 hours per Pokemon batch (20-30 Pokemon)

**Completion Checklist**:
- [ ] Gen 1 Pokemon learnsets complete
- [ ] Gen 2 Pokemon learnsets complete
- [ ] All Pokemon have STAB moves
- [ ] All Pokemon have coverage moves
- [ ] All Pokemon have status moves (where appropriate)
- [ ] Tests updated for learnsets

**Files to Update**:
- `PokemonCatalog.Gen1.cs` (update learnsets)
- `PokemonCatalog.Gen2.cs` (update learnsets)

---

### Phase 3.9: Builders ✅ COMPLETE

**Goal**: Consolidate all builder classes under Feature 3 for content creation.

**Status**: ✅ Complete

**Components**:
- **13 Builder Classes**: Fluent APIs for creating game content
- **10 Static Helper Classes**: Convenience entry points

**Implemented**:
- All builders use namespace `PokemonUltimate.Content.Builders`
- All XML comments reference Feature 3, Sub-Feature 3.9
- All catalogs and tests updated to use new namespace
- Documentation created in Feature 3

**Files**:
- `PokemonUltimate.Core/Builders/*.cs` (all 13 builder files)
- **Note**: Files are physically in Core but namespace is `PokemonUltimate.Content.Builders`

**Related**: [Sub-Feature 3.9 Documentation](3.9-builders/)

---

## Content Quality Standards

### Pokemon Standards
- ✅ Correct base stats (BST verified)
- ✅ Correct types (primary + secondary)
- ✅ Correct abilities (Ability1, Ability2, HiddenAbility)
- ✅ Complete learnsets (level-up moves)
- ✅ Correct evolution chains
- ✅ Correct gender ratios
- ✅ Correct Pokedex numbers
- ⚠️ **Pokedex fields** (Description, Category, Height, Weight, Color, Shape, Habitat) - See `pokemon_data_roadmap.md`
- ⚠️ **Gameplay fields** (BaseExperienceYield, CatchRate, BaseFriendship, GrowthRate) - See `pokemon_data_roadmap.md`

### Move Standards
- ✅ Correct base power
- ✅ Correct accuracy
- ✅ Correct PP
- ✅ Correct type
- ✅ Correct category (Physical/Special/Status)
- ✅ Correct target scope
- ✅ Proper effects implemented
- ✅ Correct priority (if applicable)

### Item Standards
- ✅ Correct category
- ✅ Correct price
- ✅ Proper effects implemented
- ✅ Correct triggers (if applicable)
- ✅ Correct stat modifiers (if applicable)

### Ability Standards
- ✅ Correct generation
- ✅ Correct triggers
- ✅ Proper effects implemented
- ✅ Correct stat modifiers (if applicable)

---

## Testing Requirements

### Content Validation Tests

Each new content addition should include:

1. **Data Validation Tests**
   - Verify all required fields are set
   - Verify data consistency (e.g., BST matches sum of stats)
   - Verify no duplicate IDs

2. **Integration Tests**
   - Verify Pokemon can be created from catalog
   - Verify moves can be used in battle
   - Verify items work correctly
   - Verify abilities trigger correctly

3. **Real-World Verification**
   - Compare against official Pokemon data
   - Verify stats match official sources
   - Verify learnsets match official sources

---

## Content Addition Workflow

### When Adding New Pokemon

1. **Choose Pokemon** from target generation/batch
2. **Gather Data** from official sources (Bulbapedia, Serebii)
   - **Required**: Base stats, types, abilities, learnset, evolution, gender ratio
   - **Gameplay Fields**: BaseExperienceYield, CatchRate, BaseFriendship, GrowthRate
   - **Pokedex Fields**: Description, Category, Height, Weight, Color, Shape, Habitat
   - See `docs/features/pokemon-data/roadmap.md` for complete field list
3. **Create Entry** in appropriate catalog file using `PokemonBuilder`
   - Use builder methods for all fields (`.Description()`, `.Category()`, `.Height()`, `.Weight()`, etc.)
4. **Assign Abilities** (Ability1, Ability2, HiddenAbility)
5. **Create Learnsets** (level-up moves)
6. **Set Evolution** (if applicable)
7. **Write Tests** (data validation + integration)
   - Verify all fields are set correctly (including Pokedex and gameplay fields)
8. **Verify** against official sources
9. **Update Documentation** (catalog counts)

### When Adding New Moves

1. **Choose Move** from target type/category
2. **Gather Data** from official sources
3. **Create Entry** in appropriate catalog file
4. **Implement Effects** (if new effect type needed)
5. **Write Tests** (data validation + battle integration)
6. **Verify** against official sources
7. **Update Documentation** (catalog counts)

### When Adding New Items/Abilities

1. **Choose Item/Ability** from target category
2. **Gather Data** from official sources
3. **Create Entry** in appropriate catalog file
4. **Implement Effects** (if new effect type needed)
5. **Write Tests** (data validation + battle integration)
6. **Verify** against official sources
7. **Update Documentation** (catalog counts)

---

## Priority Matrix

| Phase | Content Type | Current | Target | Priority | Effort | Dependencies |
|-------|--------------|---------|--------|----------|--------|--------------|
| 3.1 | Gen 1 Pokemon | 26/151 | 151 | High | 20-30h | None |
| 3.2 | Moves (All Types) | 36 | 100+ | High | 30-40h | None |
| 3.3 | Items | 23 | 50+ | Medium | 15-20h | None |
| 3.4 | Abilities | 35 | 50+ | Medium | 10-15h | None |
| 3.5 | Status Effects | 15 | 15 ✅ | Complete | - | None |
| 3.6 | Field Conditions | 35 | 35 ✅ | Complete | - | None |
| 3.7 | Content Validation | - | - | Medium | 10-15h | All content |
| 3.8 | Content Organization | - | - ✅ | Complete | - | None |
| 3.9 | Builders | 13 builders | 13 ✅ | Complete | - | None |
| 3.10 | Gen 2 Pokemon | 0/100 | 100 | Medium | 25-35h | 3.2 (moves) |
| 3.11 | Complete Learnsets | Partial | Complete | High | 20-30h | 3.1, 3.2, 3.10 |

**Total Estimated Effort**: 120-170 hours

---

## Quick Reference

### File Locations

```
PokemonUltimate.Content/Catalogs/
├── Pokemon/
│   ├── PokemonCatalog.cs
│   ├── PokemonCatalog.Gen1.cs
│   └── PokemonCatalog.Gen2.cs (future)
├── Moves/
│   ├── MoveCatalog.cs
│   ├── MoveCatalog.Normal.cs
│   ├── MoveCatalog.Fire.cs
│   └── ... (all types)
├── Items/
│   ├── ItemCatalog.cs
│   ├── ItemCatalog.HeldItems.cs
│   ├── ItemCatalog.Berries.cs
│   └── ItemCatalog.Consumables.cs (future)
└── Abilities/
    ├── AbilityCatalog.cs
    ├── AbilityCatalog.Gen3.cs
    ├── AbilityCatalog.Gen4.cs (future)
    └── AbilityCatalog.Additional.cs
```

### Data Sources

- **Bulbapedia**: Comprehensive Pokemon data
- **Serebii**: Move and ability data
- **Pokemon Showdown**: Competitive movepools
- **Official Games**: Primary source of truth

---

## Version History

| Date | Phase | Content Added | Notes |
|------|-------|---------------|-------|
| Dec 2025 | Initial | 26 Pokemon, 36 Moves, 23 Items, 35 Abilities | Foundation complete |
| Jan 2025 | 3.5 | Status Effect Expansion | 15 statuses complete |
| Jan 2025 | 3.6 | Field Conditions Expansion | 35 field conditions complete |
| Jan 2025 | 3.8 | Content Organization | Catalog organization complete |
| Jan 2025 | 3.9 | Builders Consolidation | Builders moved to Feature 3 |
| TBD | 3.1 | Gen 1 completion | |
| TBD | 3.2 | Move expansion | |
| TBD | 3.3 | Item expansion | |
| TBD | 3.4 | Ability expansion | |
| TBD | 3.7 | Content Validation | |
| TBD | 3.10 | Gen 2 Pokemon | |
| TBD | 3.11 | Complete learnsets | |

---

## Related Documents

- **[Feature README](README.md)** - Overview of Content Expansion
- **[Architecture](architecture.md)** - Catalog system design
- **[Use Cases](use_cases.md)** - All scenarios for adding content
- **[Testing](testing.md)** - Content testing strategy
- **[Code Location](code_location.md)** - Where content code is located
- **[Feature 1: Game Data](../1-game-data/architecture.md)** - Game data structure
- **[Feature 2: Combat System](../2-combat-system/roadmap.md)** - Combat system phases
- **[Feature 2.5: Combat Actions](../2-combat-system/2.5-combat-actions/architecture.md)** - Move system design
- **[Feature 2.9: Abilities & Items](../2-combat-system/2.9-abilities-items/architecture.md)** - Abilities & Items design

---

**Last Updated**: 2025-01-XX

