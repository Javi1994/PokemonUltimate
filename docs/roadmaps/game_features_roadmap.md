# Game Features Roadmap

> Step-by-step guide for implementing game features beyond combat (progression, roguelike, meta-game).

## Overview

This roadmap outlines the phases for implementing game features that make Pokemon Ultimate a complete playable game, including progression systems, roguelike mechanics, and meta-game features.

**Current Status**: Combat engine complete. Game features pending.

**Dependencies**: 
- ✅ Combat System (Phases 2.1-2.11 complete)
- ✅ Content System (Pokemon, Moves, Abilities, Items)
- ⏳ Post-Battle System (EXP, rewards)

---

## Phase 5.1: Post-Battle Rewards System

**Goal**: Implement EXP gain, level ups, and basic rewards after battles.

**Depends on**: Combat System complete, Victory/Defeat system working.

### Components to Implement

- EXP calculation system
- Level up system
- Move learning on level up
- Basic item rewards
- Post-battle service

### Specifications

- **EXP Formula**: Gen 3+ EXP formula (base EXP, level difference, participation)
- **Level Up**: Stat increases, move learning, evolution checks
- **Rewards**: Items, money (if applicable)
- **Distribution**: EXP to all participating Pokemon

### Workflow

1. **Create EXP Calculator**
   ```csharp
   public static class ExpCalculator
   {
       public static int Calculate(PokemonInstance defeatedPokemon, bool isWild, int levelDifference)
       {
           // Gen 3+ formula: (Base EXP * Level * Wild multiplier) / (7 * Participants)
           // Apply level difference modifier
       }
   }
   ```

2. **Create Post-Battle Service**
   ```csharp
   public class PostBattleService
   {
       public async Task ProcessRewards(BattleResult result, BattleField field, IBattleView view)
       {
           // 1. Calculate EXP
           // 2. Distribute to party
           // 3. Handle level ups
           // 4. Check evolutions
           // 5. Award items
       }
   }
   ```

3. **Implement Level Up Logic**
   - Stat recalculation
   - Move learning checks
   - Evolution checks
   - Visual feedback

4. **Implement Item Rewards**
   - Random item drops
   - Guaranteed rewards
   - Item collection

### Tests to Write

```csharp
Tests/Systems/PostBattle/
├── ExpCalculator_CalculatesCorrectly
├── PostBattleService_DistributesEXP
├── LevelUp_IncreasesStats
├── LevelUp_LearnsMoves
└── Rewards_AwardItems
```

### Completion Checklist

- [ ] EXP calculator implemented
- [ ] Post-battle service created
- [ ] Level up system working
- [ ] Move learning on level up works
- [ ] Item rewards implemented
- [ ] All post-battle tests pass

**Estimated Effort**: 15-20 hours
**Estimated Tests**: ~20-25 tests

---

## Phase 5.2: Pokemon Management System

**Goal**: Implement party management, PC storage, and Pokemon catching.

**Depends on**: Phase 5.1 (Post-Battle Rewards).

### Components to Implement

- Party system (6 Pokemon max)
- PC storage system
- Pokemon catching mechanics
- Pokemon release/transfer
- Party switching

### Specifications

- **Party**: Max 6 Pokemon, can switch order
- **PC**: Unlimited storage, organized by boxes
- **Catching**: Poke Ball mechanics, catch rate calculation
- **Release**: Remove Pokemon from party/PC

### Workflow

1. **Create Party System**
   ```csharp
   public class PokemonParty
   {
       private readonly List<PokemonInstance> _pokemon = new();
       private const int MaxSize = 6;
       
       public bool CanAdd => _pokemon.Count < MaxSize;
       public void Add(PokemonInstance pokemon) { }
       public void Remove(PokemonInstance pokemon) { }
       public void Swap(int index1, int index2) { }
   }
   ```

2. **Create PC Storage System**
   ```csharp
   public class PCStorage
   {
       private readonly List<PokemonBox> _boxes = new();
       
       public void Deposit(PokemonInstance pokemon) { }
       public void Withdraw(PokemonInstance pokemon) { }
       public PokemonInstance Get(int boxIndex, int slotIndex) { }
   }
   ```

3. **Create Catch Calculator**
   ```csharp
   public static class CatchCalculator
   {
       public static bool AttemptCatch(PokemonInstance pokemon, ItemData ball)
       {
           // Gen 3+ catch formula
           // Factor in HP, status, ball type
       }
   }
   ```

4. **Implement Catching Flow**
   - Throw ball action
   - Catch rate calculation
   - Success/failure handling
   - Add to party or PC

### Tests to Write

```csharp
Tests/Systems/PokemonManagement/
├── Party_CanAddUpTo6Pokemon
├── Party_CannotExceedMaxSize
├── PCStorage_StoresPokemon
├── CatchCalculator_CalculatesCorrectly
└── Catching_AddsToPartyOrPC
```

### Completion Checklist

- [ ] Party system implemented
- [ ] PC storage system implemented
- [ ] Catch calculator implemented
- [ ] Catching mechanics working
- [ ] Release/transfer working
- [ ] All management tests pass

**Estimated Effort**: 20-25 hours
**Estimated Tests**: ~25-30 tests

---

## Phase 5.3: Progression System (Roguelike Runs)

**Goal**: Implement roguelike run structure with progression and meta-progression.

**Depends on**: Phase 5.2 (Pokemon Management).

### Components to Implement

- Run manager
- Run state (current run data)
- Meta-progression (unlocks, upgrades)
- Run persistence
- Run end conditions

### Specifications

- **Run Structure**: Start → Battles → Boss → Next Area → Repeat
- **Meta-Progression**: Unlock new Pokemon, items, abilities between runs
- **Persistence**: Save run state, resume on restart
- **End Conditions**: Victory (final boss), Defeat (all Pokemon faint)

### Workflow

1. **Create Run Manager**
   ```csharp
   public class RunManager
   {
       private RunState _currentRun;
       private MetaProgression _metaProgression;
       
       public void StartNewRun() { }
       public void EndRun(BattleResult result) { }
       public void SaveRun() { }
       public void LoadRun() { }
   }
   ```

2. **Create Run State**
   ```csharp
   public class RunState
   {
       public int CurrentArea { get; set; }
       public int CurrentBattle { get; set; }
       public PokemonParty Party { get; set; }
       public List<ItemData> Inventory { get; set; }
       public Dictionary<string, object> Flags { get; set; }
   }
   ```

3. **Create Meta-Progression System**
   ```csharp
   public class MetaProgression
   {
       public HashSet<string> UnlockedPokemon { get; set; }
       public HashSet<string> UnlockedItems { get; set; }
       public HashSet<string> UnlockedAbilities { get; set; }
       public int TotalShards { get; set; }
       
       public void Unlock(string category, string id) { }
       public bool IsUnlocked(string category, string id) { }
   }
   ```

4. **Implement Run Flow**
   - Start run → Create party
   - Battle → Update run state
   - Boss battle → Check progression
   - Victory → Unlock rewards, save meta-progression
   - Defeat → End run, save meta-progression

### Tests to Write

```csharp
Tests/Systems/Progression/
├── RunManager_StartsNewRun
├── RunManager_EndsRunOnDefeat
├── RunState_PersistsCorrectly
├── MetaProgression_UnlocksItems
└── RunFlow_CompletesCorrectly
```

### Completion Checklist

- [ ] Run manager implemented
- [ ] Run state system working
- [ ] Meta-progression system implemented
- [ ] Run persistence working
- [ ] Run end conditions handled
- [ ] All progression tests pass

**Estimated Effort**: 25-35 hours
**Estimated Tests**: ~30-40 tests

---

## Phase 5.4: Encounter System

**Goal**: Implement Pokemon encounter generation and battle initiation.

**Depends on**: Phase 5.3 (Progression System).

### Components to Implement

- Encounter table system
- Encounter generation
- Wild Pokemon generation
- Trainer battle system
- Boss battle system

### Specifications

- **Encounter Tables**: Area-based encounter rates
- **Wild Encounters**: Random Pokemon based on area
- **Trainer Battles**: Fixed teams, scripted encounters
- **Boss Battles**: Special encounters with unique rewards

### Workflow

1. **Create Encounter Table**
   ```csharp
   public class EncounterTable
   {
       public string AreaId { get; set; }
       public List<EncounterEntry> Encounters { get; set; }
       
       public PokemonInstance GenerateEncounter(Random random)
       {
           // Roll for encounter, generate Pokemon
       }
   }
   ```

2. **Create Encounter Generator**
   ```csharp
   public class EncounterGenerator
   {
       public PokemonInstance GenerateWildEncounter(string areaId, int levelRange)
       {
           // Get encounter table, generate Pokemon
       }
       
       public BattleField GenerateTrainerBattle(string trainerId)
       {
           // Load trainer data, create battle field
       }
   }
   ```

3. **Implement Encounter Types**
   - Wild Pokemon (random)
   - Trainer battles (fixed)
   - Boss battles (special)
   - Event encounters (story)

4. **Integrate with Run System**
   - Generate encounters based on area
   - Track encounter history
   - Handle special encounters

### Tests to Write

```csharp
Tests/Systems/Encounter/
├── EncounterTable_GeneratesCorrectly
├── EncounterGenerator_CreatesWildPokemon
├── TrainerBattle_HasCorrectTeam
└── BossBattle_HasSpecialRewards
```

### Completion Checklist

- [ ] Encounter table system implemented
- [ ] Wild encounter generation working
- [ ] Trainer battle system implemented
- [ ] Boss battle system implemented
- [ ] Encounter integration with runs working
- [ ] All encounter tests pass

**Estimated Effort**: 15-20 hours
**Estimated Tests**: ~20-25 tests

---

## Phase 5.5: Item & Inventory System

**Goal**: Implement inventory management, item usage, and shop system.

**Depends on**: Phase 5.4 (Encounter System).

### Components to Implement

- Inventory system
- Item usage (healing items, battle items)
- Shop system
- Item management UI

### Specifications

- **Inventory**: Item storage with quantities
- **Item Usage**: Consumable items, held items
- **Shop**: Buy/sell items with currency
- **Management**: Organize, sort, filter items

### Workflow

1. **Create Inventory System**
   ```csharp
   public class Inventory
   {
       private Dictionary<ItemData, int> _items = new();
       
       public void Add(ItemData item, int quantity) { }
       public void Remove(ItemData item, int quantity) { }
       public int GetQuantity(ItemData item) { }
       public bool HasItem(ItemData item) { }
   }
   ```

2. **Create Item Usage System**
   ```csharp
   public class ItemUsageService
   {
       public bool CanUse(ItemData item, PokemonInstance target)
       {
           // Check item type, target state
       }
       
       public void Use(ItemData item, PokemonInstance target)
       {
           // Apply item effect
       }
   }
   ```

3. **Create Shop System**
   ```csharp
   public class Shop
   {
       public List<ShopItem> AvailableItems { get; set; }
       
       public bool Buy(ItemData item, int quantity, int currency)
       {
           // Check currency, add to inventory
       }
       
       public int Sell(ItemData item, int quantity)
       {
           // Remove from inventory, return currency
       }
   }
   ```

4. **Implement Item Categories**
   - Healing items (Potion, Super Potion)
   - Status cure items (Antidote, Paralyze Heal)
   - Battle items (X Attack, X Speed)
   - Evolution items (Fire Stone, Water Stone)

### Tests to Write

```csharp
Tests/Systems/Inventory/
├── Inventory_StoresItems
├── ItemUsage_HealsPokemon
├── Shop_BuysItems
└── ItemManagement_WorksCorrectly
```

### Completion Checklist

- [ ] Inventory system implemented
- [ ] Item usage system working
- [ ] Shop system implemented
- [ ] Item management UI (if applicable)
- [ ] All inventory tests pass

**Estimated Effort**: 15-20 hours
**Estimated Tests**: ~20-25 tests

---

## Phase 5.6: Save System

**Goal**: Implement save/load functionality for runs and meta-progression.

**Depends on**: Phase 5.5 (Item & Inventory System).

### Components to Implement

- Save data structure
- Save service
- Load service
- Save file management
- Auto-save system

### Specifications

- **Save Data**: Run state, meta-progression, settings
- **Save Format**: JSON (human-readable, debuggable)
- **Save Location**: Platform-specific (Windows, Mac, Linux)
- **Auto-Save**: Save after important events

### Workflow

1. **Create Save Data Structure**
   ```csharp
   public class SaveData
   {
       public RunState CurrentRun { get; set; }
       public MetaProgression MetaProgression { get; set; }
       public GameSettings Settings { get; set; }
       public DateTime SaveTime { get; set; }
   }
   ```

2. **Create Save Service**
   ```csharp
   public class SaveService
   {
       public void Save(SaveData data, string slotName)
       {
           // Serialize to JSON, write to file
       }
       
       public SaveData Load(string slotName)
       {
           // Read file, deserialize JSON
       }
       
       public bool SaveExists(string slotName)
       {
           // Check if save file exists
       }
   }
   ```

3. **Implement Auto-Save**
   - Save after battles
   - Save after important events
   - Save on application quit

4. **Implement Save Slots**
   - Multiple save slots
   - Save slot selection
   - Save slot deletion

### Tests to Write

```csharp
Tests/Systems/Save/
├── SaveService_SavesData
├── SaveService_LoadsData
├── SaveData_SerializesCorrectly
└── AutoSave_WorksCorrectly
```

### Completion Checklist

- [ ] Save data structure created
- [ ] Save service implemented
- [ ] Load service implemented
- [ ] Auto-save system working
- [ ] Save slots implemented
- [ ] All save tests pass

**Estimated Effort**: 10-15 hours
**Estimated Tests**: ~15-20 tests

---

## Quality Standards

- **Gameplay**: Fun, engaging, balanced
- **Progression**: Clear sense of advancement
- **Persistence**: Reliable save/load
- **Performance**: Smooth gameplay experience
- **Testability**: All systems testable

## Workflow for Game Features

1. **Design**: Plan feature mechanics and interactions
2. **Implement**: Build core functionality
3. **Test**: Write comprehensive tests
4. **Integrate**: Connect with existing systems
5. **Polish**: Refine and balance
6. **Document**: Update documentation

## Testing Requirements

- **Unit Tests**: Test individual systems
- **Integration Tests**: Test system interactions
- **Gameplay Tests**: Test full game flow
- **Balance Tests**: Verify progression feels good

---

## Priority Matrix

| Phase | Effort (Hours) | Dependencies | Priority |
|-------|----------------|--------------|----------|
| 5.1 Post-Battle Rewards | 15-20 | Combat System | High |
| 5.2 Pokemon Management | 20-25 | 5.1 | High |
| 5.3 Progression System | 25-35 | 5.2 | High |
| 5.4 Encounter System | 15-20 | 5.3 | Medium |
| 5.5 Item & Inventory | 15-20 | 5.4 | Medium |
| 5.6 Save System | 10-15 | 5.5 | High |

---

## Quick Reference

### File Structure

```
PokemonUltimate.Core/
├── Progression/
│   ├── RunManager.cs
│   ├── RunState.cs
│   └── MetaProgression.cs
├── Management/
│   ├── PokemonParty.cs
│   ├── PCStorage.cs
│   └── CatchCalculator.cs
├── Encounter/
│   ├── EncounterTable.cs
│   └── EncounterGenerator.cs
├── Inventory/
│   ├── Inventory.cs
│   ├── ItemUsageService.cs
│   └── Shop.cs
└── Save/
    ├── SaveData.cs
    └── SaveService.cs
```

### Related Documents

| Document | Purpose |
|----------|---------|
| `docs/architecture/victory_defeat_system.md` | Post-battle system spec |
| `docs/roadmaps/combat_roadmap.md` | Combat system roadmap |
| `docs/roadmaps/content_expansion_roadmap.md` | Content expansion roadmap |

---

## Version History

| Date | Phase | Notes |
|------|-------|-------|
| [Current Date] | 5.1-5.6 | Initial game features roadmap created. |

