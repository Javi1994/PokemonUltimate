# Feature 5: Game Features - Use Cases

> All scenarios, behaviors, and edge cases for game features beyond combat.

**Feature Number**: 5  
**Feature Name**: Game Features  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document covers all use cases for game features beyond the battle engine, including:
- Post-battle rewards and progression
- Pokemon management (party, PC, catching)
- Encounter system (wild, trainer, boss battles)
- Item and inventory management
- Save system
- Roguelike progression

## Core Use Cases

### UC-001: Calculate EXP After Battle
**Description**: Calculate EXP gained by participating Pokemon after battle
**Actor**: Post-battle system
**Preconditions**: Battle completed, Pokemon participated
**Steps**:
1. Get defeated Pokemon's base EXP yield
2. Apply level difference modifier
3. Apply wild/trainer multiplier
4. Divide by number of participants
5. Distribute EXP to participating Pokemon
**Expected Result**: Each Pokemon gains appropriate EXP
**Status**: ⏳ Planned (Phase 5.1)

### UC-002: Level Up Pokemon
**Description**: Pokemon levels up when EXP threshold reached
**Actor**: EXP system
**Preconditions**: Pokemon has enough EXP
**Steps**:
1. Check if EXP >= level threshold
2. Increase level by 1
3. Recalculate stats
4. Check for move learning
5. Check for evolution
6. Show level up animation/UI
**Expected Result**: Pokemon levels up with stat increases
**Status**: ⏳ Planned (Phase 5.1)

### UC-003: Learn Move on Level Up
**Description**: Pokemon learns new moves when leveling up
**Actor**: Level-up system
**Preconditions**: Pokemon levels up, has moves in learnset at new level
**Steps**:
1. Check learnset for moves at new level
2. If moves available and slots free, learn automatically
3. If no slots free, prompt player to choose move to replace
4. Replace old move with new move
**Expected Result**: Pokemon learns appropriate moves for level
**Status**: ⏳ Planned (Phase 5.1)

### UC-004: Catch Wild Pokemon
**Description**: Player catches wild Pokemon after battle
**Actor**: Player
**Preconditions**: Wild Pokemon battle won, player has Pokeballs
**Steps**:
1. Player uses Pokeball item
2. Calculate catch rate based on Pokemon HP, status, catch rate
3. Roll for catch success
4. If successful, add Pokemon to party or PC
5. If party full, send to PC
**Expected Result**: Pokemon added to player's collection
**Status**: ⏳ Planned (Phase 5.2)

### UC-005: Manage Pokemon Party
**Description**: Player manages active Pokemon party
**Actor**: Player
**Preconditions**: Player has Pokemon
**Steps**:
1. View current party (up to 6 Pokemon)
2. Switch Pokemon order
3. Move Pokemon to PC
4. Move Pokemon from PC to party
5. View Pokemon details
**Expected Result**: Party management works correctly
**Status**: ⏳ Planned (Phase 5.2)

### UC-006: Store Pokemon in PC
**Description**: Store Pokemon in PC storage system
**Actor**: Player
**Preconditions**: PC system exists
**Steps**:
1. Access PC
2. Select Pokemon from party
3. Move to PC box
4. Organize boxes
5. Retrieve Pokemon from PC
**Expected Result**: Pokemon stored and retrievable
**Status**: ⏳ Planned (Phase 5.2)

### UC-007: Encounter Wild Pokemon
**Description**: Random wild Pokemon encounter in overworld
**Actor**: Encounter system
**Preconditions**: Player in encounter area
**Steps**:
1. Check encounter rate for area
2. Roll for encounter
3. Select Pokemon from area's encounter table
4. Select level (based on area level range)
5. Initialize battle with wild Pokemon
**Expected Result**: Wild Pokemon battle starts
**Status**: ⏳ Planned (Phase 5.3)

### UC-008: Battle Trainer
**Description**: Battle NPC trainer
**Actor**: Encounter system
**Preconditions**: Player triggers trainer battle
**Steps**:
1. Initialize trainer's party
2. Start battle
3. Trainer uses AI to select actions
4. Battle proceeds normally
5. On victory, receive rewards
**Expected Result**: Trainer battle works correctly
**Status**: ⏳ Planned (Phase 5.3)

### UC-009: Battle Boss
**Description**: Battle special boss Pokemon
**Actor**: Encounter system
**Preconditions**: Boss encounter triggered
**Steps**:
1. Initialize boss Pokemon (higher level, better stats)
2. Start battle
3. Boss may have special mechanics
4. On victory, receive special rewards
**Expected Result**: Boss battle works correctly
**Status**: ⏳ Planned (Phase 5.3)

### UC-010: Use Items
**Description**: Player uses consumable items
**Actor**: Player
**Preconditions**: Player has items
**Steps**:
1. Open inventory
2. Select item
3. Choose target (Pokemon or self)
4. Apply item effects
5. Remove item from inventory
**Expected Result**: Item effects applied
**Status**: ⏳ Planned (Phase 5.4)

### UC-011: Manage Inventory
**Description**: Player manages item inventory
**Actor**: Player
**Preconditions**: Inventory system exists
**Steps**:
1. View inventory
2. Sort items by category
3. Use items
4. Discard items
5. Organize inventory
**Expected Result**: Inventory management works correctly
**Status**: ⏳ Planned (Phase 5.4)

### UC-012: Save Game
**Description**: Save game progress to file
**Actor**: Player
**Preconditions**: Save system exists
**Steps**:
1. Player triggers save
2. Serialize game state (party, PC, inventory, progress)
3. Write to save file
4. Confirm save success
**Expected Result**: Game state saved
**Status**: ⏳ Planned (Phase 5.5)

### UC-013: Load Game
**Description**: Load saved game progress
**Actor**: Player
**Preconditions**: Save file exists
**Steps**:
1. Player selects save file
2. Deserialize game state
3. Restore party, PC, inventory, progress
4. Resume from saved location
**Expected Result**: Game state restored
**Status**: ⏳ Planned (Phase 5.5)

### UC-014: Roguelike Run Progression
**Description**: Player progresses through roguelike run
**Actor**: Player
**Preconditions**: Roguelike system exists
**Steps**:
1. Start new run
2. Progress through encounters
3. Build party and collect items
4. Face boss encounters
5. On defeat, run ends, meta-progression unlocks
**Expected Result**: Roguelike progression works
**Status**: ⏳ Planned (Phase 5.6)

### UC-015: Meta-Progression Unlocks
**Description**: Unlock permanent upgrades between runs
**Actor**: Meta-progression system
**Preconditions**: Run completed
**Steps**:
1. Calculate run score/progress
2. Award meta-progression currency
3. Unlock new Pokemon, items, abilities
4. Apply permanent upgrades
5. Show unlock UI
**Expected Result**: Meta-progression unlocks applied
**Status**: ⏳ Planned (Phase 5.6)

## Edge Cases

### EC-001: EXP Overflow
**Description**: EXP calculation exceeds maximum value
**Behavior**: Cap at maximum EXP for level 100
**Status**: ⏳ Planned (handled in Phase 5.1)

### EC-002: Party Full When Catching
**Description**: Player catches Pokemon but party is full
**Behavior**: Automatically send to PC
**Status**: ⏳ Planned (handled in Phase 5.2)

### EC-003: PC Full
**Description**: All PC boxes are full
**Behavior**: Prevent catch or expand PC capacity
**Status**: ⏳ Planned (handled in Phase 5.2)

### EC-004: Multiple Moves at Same Level
**Description**: Pokemon learns multiple moves at same level
**Behavior**: Learn all moves, prompt to replace if slots full
**Status**: ⏳ Planned (handled in Phase 5.1)

### EC-005: Evolution During Battle
**Description**: Pokemon evolves during battle (level up)
**Behavior**: Evolution happens after battle
**Status**: ⏳ Planned (handled in Phase 5.1)

### EC-006: Save File Corruption
**Description**: Save file is corrupted or invalid
**Behavior**: Detect corruption, offer to create new save or restore backup
**Status**: ⏳ Planned (handled in Phase 5.5)

### EC-007: Encounter Rate Too High
**Description**: Player encounters Pokemon too frequently
**Behavior**: Implement encounter cooldown or reduce rate
**Status**: ⏳ Planned (handled in Phase 5.3)

## Integration Scenarios

### INT-001: Game Features → Combat System
**Description**: Post-battle rewards use battle results
**Steps**:
1. Battle completes
2. Post-battle system reads battle result
3. Calculates EXP and rewards
4. Applies to Pokemon
**Status**: ⏳ Planned (Phase 5.1)

### INT-002: Game Features → Pokemon Data
**Description**: Catching uses Pokemon data structure
**Steps**:
1. Wild Pokemon encounter uses PokemonSpeciesData
2. Catching creates PokemonInstance
3. Instance added to party/PC
**Status**: ⏳ Planned (Phase 5.2)

### INT-003: Game Features → Content Expansion
**Description**: Encounters use content catalogs
**Steps**:
1. Encounter system queries PokemonCatalog
2. Selects Pokemon from area's encounter table
3. Creates instance for battle
**Status**: ⏳ Planned (Phase 5.3)

### INT-004: Game Features → Unity Integration
**Description**: Post-battle UI shows rewards
**Steps**:
1. Post-battle system calculates rewards
2. Unity displays post-battle UI
3. Shows EXP, level ups, rewards
**Status**: ⏳ Planned (Phase 5.1, 4.7)

## Related Documents

- **[Feature README](README.md)** - Overview of Game Features
- **[5.1 Architecture](5.1-post-battle-rewards/architecture.md)** - Post-battle rewards specification
- **[Roadmap](roadmap.md)** - Implementation phases (5.1-5.6)
- **[Testing](testing.md)** - Testing strategy for game features
- **[Code Location](code_location.md)** - Where game feature code will be located
- **[Feature 2: Combat System](../2-combat-system/architecture.md)** - Battle system used in encounters
- **[Feature 1: Pokemon Data](../1-pokemon-data/architecture.md)** - Pokemon data for catching and management
- **[Feature 3: Content Expansion](../3-content-expansion/architecture.md)** - Items and Pokemon for encounters

---

**Last Updated**: 2025-01-XX

