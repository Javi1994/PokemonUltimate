# Project Roadmap: Pokemon Combat Roguelike

## Phase 1: Core Systems 🔄 IN PROGRESS

### Data Layer (Step 1) 🔄
- [x] **Registry Architecture**
    - [x] `IIdentifiable` interface
    - [x] `IDataRegistry<T>` interface
    - [x] `GameDataRegistry<T>` implementation
- [x] **Pokemon Data**
    - [x] `PokemonSpeciesData` (Name, PokedexNumber, Types, BaseStats, Learnset, Evolutions)
    - [x] `BaseStats` class (HP, Attack, Defense, SpAttack, SpDefense, Speed, Total)
    - [x] `LearnableMove` class (Move reference, LearnMethod, Level)
    - [x] `Evolution` class (Target, Conditions)
    - [x] `IEvolutionCondition` interface + 6 condition classes
    - [x] `IPokemonRegistry` interface
    - [x] `PokemonRegistry` (with Pokedex lookup)
    - [ ] `PokemonInstance` (mutable runtime state)
    - [ ] `PokemonFactory` (create instances from blueprints)
- [x] **Move Data**
    - [x] `MoveData` (blueprint with Type, Category, Power, etc.)
    - [x] `IMoveRegistry` interface
    - [x] `MoveRegistry` (with Type/Category filters)
    - [ ] `MoveInstance` (PP tracking per Pokemon)
- [x] **Move Effects (Composable)**
    - [x] `IMoveEffect` interface (EffectType + Description)
    - [x] `EffectType` enum (Damage, Status, StatChange, etc.)
    - [x] 9 effect classes: DamageEffect, FixedDamageEffect, StatusEffect, StatChangeEffect, RecoilEffect, DrainEffect, HealEffect, FlinchEffect, MultiHitEffect
    - [x] Effects integrated into `MoveCatalog` entries
    - [ ] `GenerateActions()` method (pending combat system)
- [x] **Enums**
    - [x] `PokemonType` (18 types)
    - [x] `MoveCategory` (Physical, Special, Status)
    - [x] `TargetScope` (Self, SingleEnemy, AllEnemies, etc.)
    - [x] `Stat` (HP, Attack, Defense, SpAttack, SpDefense, Speed, Accuracy, Evasion)
    - [x] `PersistentStatus` (Burn, Paralysis, Sleep, Poison, BadlyPoisoned, Freeze)
    - [x] `VolatileStatus` (Confusion, Flinch, LeechSeed, etc.)
    - [x] `EffectType` (24 types: 9 implemented + 15 planned)
    - [x] `LearnMethod` (Start, LevelUp, Evolution, TM, Egg, Tutor)
    - [x] `TimeOfDay` (Morning, Day, Evening, Night)
    - [x] `EvolutionConditionType` (Level, UseItem, Trade, Friendship, etc.)
- [x] **Builders (Fluent API)**
    - [x] `PokemonBuilder` - fluent Pokemon definition
    - [x] `LearnsetBuilder` - learnset configuration
    - [x] `EvolutionBuilder` - evolution conditions
- [x] **Catalogs (Modular with Partial Classes)**
    - [x] `PokemonCatalog` - organized by generation
        - [x] `PokemonCatalog.cs` (orchestrator)
        - [x] `PokemonCatalog.Gen1.cs` (15 Pokemon with learnsets & evolutions)
    - [x] `MoveCatalog` - organized by type
        - [x] `MoveCatalog.cs` (orchestrator)
        - [x] `MoveCatalog.[Type].cs` (20 Moves across 7 types)
    - [x] `RegisterAll()` for bulk registration
    - [x] Lazy initialization for `All` enumeration
- [x] **Documentation**
    - [x] `project_structure.md` - Solution organization and folder conventions

### Combat Engine (Step 2-5) ⏳ PENDING
- [ ] **Action Queue Architecture**
    - [ ] `BattleAction` abstract class
    - [ ] `BattleQueue` processor
    - [ ] `IBattleView` interface
- [ ] **Battle Field & Slot System**
    - [ ] `BattleField`, `BattleSide`, `BattleSlot`
    - [ ] `BattleRules` configuration
- [ ] **Turn System**
    - [ ] `TurnOrderResolver`
    - [ ] `IActionProvider` interface
    - [ ] `CombatEngine` controller
- [ ] **Damage System**
    - [ ] `DamagePipeline` with `IDamageStep`
    - [ ] `UseMoveAction`, `DamageAction`

### Testing ✅
- [x] Pokemon Registry tests (16 tests)
- [x] Move Registry tests (9 tests)
- [x] Move Filter tests (9 tests)
- [x] Pokemon/Move Data model tests (22 tests)
- [x] Catalog tests (28 tests)
- [x] Move Effect tests (25 tests)
- [x] Effect Composition tests (12 tests)
- [x] Catalog Effects tests (12 tests)
- [x] Builder tests (45+ tests)
- [x] Evolution & Condition tests (18 tests)

---

- [ ] **Team Management**
    - [ ] Party system (6 Pokémon limit)
    - [ ] Pokemon capture mechanics
    - [ ] Catch rate formula (Ball types, HP, Status)
    - [ ] PC/Storage system
- [ ] **Progression**
    - [ ] Level Up system
    - [ ] Stat recalculation on level up
    - [ ] Evolution triggers & logic
    - [ ] Move learning (forget old moves)

## Phase 3: Game Loop & Run Structure 🎯 PRIORITY
- [ ] **Run Manager**
    - [ ] Starter selection screen
    - [ ] Encounter generation (RNG-based)
    - [ ] Biome/Zone progression
    - [ ] Final Boss encounter
- [ ] **Difficulty Scaling**
    - [ ] Enemy level curve formula
    - [ ] Boss stat multipliers
    - [ ] Horde encounter scaling (1v3)
- [ ] **Events & Choices**
    - [ ] Random events (Heal, Shop, Blessing)
    - [ ] Risk/Reward decisions
    - [ ] Branching paths (choose biome type)
- [ ] **Persistence**
    - [ ] Save/Load system (mid-run)
    - [ ] Run state serialization
    - [ ] Auto-save on quit

## Phase 4: Meta-Progression & Replayability 🔄
- [ ] **Unlockables**
    - [ ] Unlock new starter Pokémon
    - [ ] Unlock rare items/held items
    - [ ] Unlock challenge modes
- [ ] **Permanent Upgrades**
    - [ ] Meta-currency system (e.g., "Shards")
    - [ ] Permanent party buffs (start with +1 Pokémon)
    - [ ] Unlock starting items
- [ ] **Challenge Modes**
    - [ ] Nuzlocke mode (permadeath)
    - [ ] Randomizer (random types)
    - [ ] Hard mode (no items, higher difficulty)
- [ ] **Achievements & Tracking**
    - [ ] Win streak tracking
    - [ ] Fastest clear time
    - [ ] Pokédex completion %

## Phase 5: Content Expansion 📦
- [ ] **Weather System**
    - [ ] Rain, Sun, Sandstorm, Hail
    - [ ] Weather-triggered abilities (Swift Swim, Chlorophyll)
    - [ ] Weather-boosted moves
- [ ] **Terrain System**
    - [ ] Electric, Grassy, Psychic, Misty Terrain
    - [ ] Terrain effects on moves and abilities
- [ ] **Field Effects**
    - [ ] Trick Room, Wonder Room, Magic Room
    - [ ] Gravity, Tailwind
- [ ] **Advanced Moves**
    - [ ] 2-turn moves (Fly, Dig, Solar Beam)
    - [ ] Charge moves
    - [ ] Delayed moves (Future Sight)
- [ ] **Special Battle Types**
    - [ ] Double Battles (2v2)
    - [ ] Horde Battles (1v3)
    - [ ] Boss battles (custom AI, multi-phase)

## Phase 6: Economy & Items 💰
- [ ] **Currency System**
    - [ ] Gold/Money acquisition
    - [ ] Item selling
- [ ] **Shop/Vendor**
    - [ ] Random shop encounters
    - [ ] Item purchasing
    - [ ] Limited stock/refresh mechanic
- [ ] **Consumable Items**
    - [ ] Potions (HP restore)
    - [ ] Status healers (Antidote, Paralyze Heal)
    - [ ] Battle items (X Attack, Guard Spec)
- [ ] **Visual Feedback**
    - [ ] Damage numbers
    - [ ] Type effectiveness indicators
    - [ ] Stat change arrows (↑↓)
- [ ] **Battle Log**
    - [ ] Scrollable text log
    - [ ] Turn history
- [ ] **QoL Features**
    - [ ] Fast-forward/Speed-up toggle
    - [ ] Skip animations option
    - [ ] Auto-battle mode

## Phase 8: Audio & Polish 🎵
- [ ] **Sound Effects**
    - [ ] Move SFX (categorized by type)
    - [ ] UI interaction sounds
    - [ ] Status effect sounds
- [ ] **Music System**
    - [ ] Battle themes (normal, boss, victory)
    - [ ] Menu music
    - [ ] Dynamic music (intensity changes)
- [ ] **Particle Effects**
    - [ ] Move animations (Fire, Water, Electric)
    - [ ] Status effect particles (Burn, Poison)
    - [ ] Hit/Miss indicators
- [ ] **Screen Shake & Camera**
    - [ ] Impact shake on super-effective hits
    - [ ] Camera zoom for critical moments

## Phase 9: Testing & Balance 🧪
- [ ] **Unit Tests**
    - [ ] Damage calculation tests
    - [ ] Status application tests
    - [ ] Turn order tests
- [ ] **Integration Tests**
    - [ ] Full battle simulation tests
    - [ ] Save/Load integrity tests
- [ ] **Balancing**
    - [ ] Difficulty curve tuning
    - [ ] Pokémon stat balancing
    - [ ] Move power/accuracy adjustments
    - [ ] Item effectiveness tuning

## Phase 10: Deployment & Post-Launch 🚀
- [ ] **Build Pipeline**
    - [ ] Windows build
    - [ ] WebGL build (optional)
- [ ] **Analytics**
    - [ ] Win rate tracking
    - [ ] Popular Pokémon/Moves
    - [ ] Average run length
- [ ] **Post-Launch Content**
    - [ ] New Pokémon packs
    - [ ] New biomes
    - [ ] Seasonal events
    - [ ] Community challenges
