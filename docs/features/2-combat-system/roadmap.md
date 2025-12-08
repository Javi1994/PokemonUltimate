# Feature 2: Combat System - Roadmap

> Step-by-step guide for implementing the Pokemon battle engine.

**Feature Number**: 2  
**Feature Name**: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Combat System is divided into **multiple phases**, each building on the previous. Every phase must pass all tests before moving to the next.

**Core Phases (2.1-2.11)**: ✅ Complete  
**Extended Phases (2.12-2.19)**: ⏳ Planned

> **📋 Refactoring Completed (2024-12-05)**: A comprehensive refactoring was completed following SOLID principles and clean code practices. The system now uses a **step-based pipeline architecture** with:
>
> -   **Battle Flow**: 8 steps for battle lifecycle (setup → execution → cleanup)
> -   **Turn Flow**: 23 unique steps (34 total) for turn execution
> -   **Damage Pipeline**: 11 steps for damage calculation
> -   **Handler Registry**: 34 handlers (4 abilities + 3 items + 12 effects + 15 checkers)
> -   **Action System**: 13 action types implementing Command Pattern
> -   **AI System**: 6 AI implementations
> -   **Infrastructure**: Event system, Statistics collection, Simulation tools, Value Objects (8), Logging, Message formatting
>
> The refactoring included Dependency Injection, Value Objects, Strategy Pattern, Factory Pattern, Event System, Logging, Validation, and more. See `PokemonUltimate.Combat/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for complete details. All phases 0-13 of the refactoring plan were completed (42 of 44 tasks, 95.5%).

```
┌─────────────────────────────────────────────────────────────┐
│                    COMBAT SYSTEM                            │
├─────────────────────────────────────────────────────────────┤
│  Phase 2.1: Battle Foundation    ══════════════════════╗    │
│  Phase 2.2: Action Queue         ══════════════════════╣    │
│  Phase 2.3: Turn Order           ══════════════════════╝    │
│                        ↓                                    │
│  Phase 2.4: Damage Calculation   ══════════════════════╗    │
│  Phase 2.5: Combat Actions       ══════════════════════╣    │
│  Phase 2.6: Combat Engine        ══════════════════════╝    │
│                        ↓                                    │
│  Phase 2.7: Integration          ══════════════════════╗    │
│  Phase 2.8: End-of-Turn Effects ══════════════════════╣    │
│  Phase 2.9: Abilities & Items   ══════════════════════╝    │
│                        ↓                                    │
│  Phase 2.11: Recoil & Drain    ══════════════════════╗    │
│  (Note: 2.10 consolidated into 2.4)                 ══════════════════════╝    │
│                        ↓                                    │
│  Phase 2.12: Weather System     ══════════════════════╗    │
│  Phase 2.13: Terrain System     ══════════════════════╣    │
│  Phase 2.14: Hazards System      ══════════════════════╝    │
│                        ↓                                    │
│  Phase 2.15: Advanced Moves     ══════════════════════╗    │
│  Phase 2.16: Field Conditions   ══════════════════════╣    │
│  Phase 2.17: Advanced Abilities ══════════════════════╝    │
│                        ↓                                    │
│  Phase 2.18: Advanced Items     ══════════════════════╗    │
│  Phase 2.19: Battle Formats     ══════════════════════╝    │
└─────────────────────────────────────────────────────────────┘
```

---

## Current Status

| Phase                     | Status           | Tests    | Notes                                                                                                 |
| ------------------------- | ---------------- | -------- | ----------------------------------------------------------------------------------------------------- |
| 2.1 Battle Foundation     | ✅ Complete      | 133      | BattleSlot, BattleSide, BattleField                                                                   |
| 2.2 Action Queue          | ✅ Complete      | 77       | BattleAction, BattleQueue                                                                             |
| 2.3 Turn Order            | ✅ Complete      | 48       | TurnOrderResolver                                                                                     |
| 2.4 Damage Calculation    | ✅ Complete      | 65       | DamagePipeline                                                                                        |
| **Data Layer**            | ✅ Complete      | 170      | AbilityData, ItemData, StatusEffectData                                                               |
| 2.5 Combat Actions        | ✅ Complete      | 47       | All actions implemented                                                                               |
| 2.6 Combat Engine         | ✅ Complete      | 30       | CombatEngine (Battle Flow: 8 steps), TurnEngine (Turn Flow: 23 steps), BattleArbiter, IActionProvider |
| 2.7 Integration           | ✅ Complete      | 38       | RandomAI, AlwaysAttackAI, TargetResolver, Full battles                                                |
| 2.8 End-of-Turn Effects   | ✅ Complete      | 25       | EndOfTurnProcessor, Status damage                                                                     |
| 2.9 Abilities & Items     | ✅ Complete      | 12       | BattleTrigger system, AbilityListener, ItemListener                                                   |
| 2.11 Recoil & Drain       | ✅ Complete      | -        | RecoilEffect, DrainEffect                                                                             |
| **Core Total**            | **10/10**        | **645+** | Combat module only (Note: 2.10 consolidated into 2.4)                                                 |
| 2.12 Extended End-of-Turn | ⏳ Planned       | ~30      | Leech Seed, Wish, Perish Song, Binding                                                                |
| 2.13 Additional Triggers  | ⏳ Planned       | ~30      | OnBeforeMove, OnAfterMove, OnDamageTaken                                                              |
| 2.14 Volatile Status      | ⏳ Planned       | ~30      | Confusion, Infatuation, Taunt, Encore, Disable                                                        |
| 2.12 Weather System       | ✅ Complete      | 35+      | Sun, Rain, Sandstorm, Hail                                                                            |
| 2.13 Terrain System       | ✅ Complete      | 35+      | Electric, Grassy, Psychic, Misty                                                                      |
| 2.14 Entry Hazards        | ✅ Complete      | 25+      | Spikes, Stealth Rock, Toxic Spikes, Sticky Web                                                        |
| 2.15 Advanced Moves       | ✅ Core Complete | 35+      | Protect, Counter, Pursuit, Focus Punch, Semi-Invulnerable, Multi-Hit, Multi-Turn                      |
| 2.16 Field Conditions     | ✅ Complete      | 40+      | Screens, Tailwind, Safeguard, Mist                                                                    |
| 2.17 Advanced Abilities   | ✅ ~95% Complete | 29       | Truant, Speed Boost, Static, Rough Skin, Swift Swim, Chlorophyll, Moxie                               |
| 2.18 Advanced Items       | ✅ Complete      | 21       | Life Orb, Focus Sash, Rocky Helmet, Black Sludge                                                      |
| 2.19 Battle Formats       | ✅ Complete      | 48       | Doubles ✅, Horde ✅, Raid ✅, Triples ✅, Integration Tests ✅                                       |
| 2.20 Statistics System    | ✅ Complete      | -        | BattleStatistics, BattleStatisticsCollector, Event-driven collection                                  |
| **Future Total**          | **9/9**          | **~298** | Extended features (all complete)                                                                      |

---

## Phase 2.1: Battle Foundation

**Goal**: Create the battlefield structure where Pokemon fight.

**Depends on**: Core (PokemonInstance, MoveInstance)

### Components

| Component      | File                             | Description                                 |
| -------------- | -------------------------------- | ------------------------------------------- |
| `BattleSlot`   | `Combat/BattleSlot.cs`           | Container for active Pokemon + battle state |
| `BattleSide`   | `Combat/BattleSide.cs`           | One side of the field (player/enemy)        |
| `BattleField`  | `Combat/BattleField.cs`          | Complete battlefield (2 sides)              |
| `BattleRules`  | `Combat/BattleRules.cs`          | Configuration (1v1, 2v2, etc.)              |
| `IBattleView`  | `Combat/IBattleView.cs`          | Interface for visual presentation           |
| `BattleAction` | `Combat/Actions/BattleAction.cs` | Abstract base for all actions               |

### BattleSlot Specification

```csharp
public class BattleSlot
{
    public PokemonInstance Pokemon { get; }
    public BattleSide Side { get; }
    public int SlotIndex { get; }
    public bool IsEmpty { get; }
    public bool HasFainted { get; }

    // Battle-specific state (resets each battle)
    public Dictionary<Stat, int> StatStages { get; }  // -6 to +6
    public VolatileStatus VolatileStatus { get; set; }

    public void ResetBattleState();
    public void SetPokemon(PokemonInstance pokemon);
    public void ClearSlot();
}
```

### BattleSide Specification

```csharp
public class BattleSide
{
    public IReadOnlyList<BattleSlot> Slots { get; }
    public IReadOnlyList<PokemonInstance> Party { get; }  // Bench Pokemon
    public bool IsPlayer { get; }

    public IEnumerable<BattleSlot> GetActiveSlots();  // Non-empty slots
    public IEnumerable<PokemonInstance> GetAvailableSwitches();
    public bool HasActivePokemon();
    public bool IsDefeated();  // All fainted
}
```

### BattleField Specification

```csharp
public class BattleField
{
    public BattleSide PlayerSide { get; }
    public BattleSide EnemySide { get; }
    public BattleRules Rules { get; }

    public void Initialize(BattleRules rules,
                          IReadOnlyList<PokemonInstance> playerParty,
                          IReadOnlyList<PokemonInstance> enemyParty);

    public IEnumerable<BattleSlot> GetAllActiveSlots();
    public BattleSlot GetSlot(bool isPlayer, int index);
    public BattleSide GetOppositeSide(BattleSide side);
}
```

### Tests to Write

```
Tests/Combat/BattleSlotTests.cs
├── Constructor_WithPokemon_InitializesCorrectly
├── StatStages_Default_AllZero
├── StatStages_Modify_ClampsToRange
├── ResetBattleState_ClearsStatStages
├── ResetBattleState_ClearsVolatileStatus
├── IsEmpty_NoPokemon_ReturnsTrue
├── HasFainted_FaintedPokemon_ReturnsTrue
└── SetPokemon_ReplacesPokemon

Tests/Combat/BattleSideTests.cs
├── Constructor_CreatesCorrectSlots
├── GetActiveSlots_ReturnsNonEmpty
├── IsDefeated_AllFainted_ReturnsTrue
├── IsDefeated_OneAlive_ReturnsFalse
├── GetAvailableSwitches_ExcludesFainted
└── GetAvailableSwitches_ExcludesActive

Tests/Combat/BattleFieldTests.cs
├── Initialize_1v1_CreatesSingleSlots
├── Initialize_2v2_CreatesDoubleSlots
├── GetAllActiveSlots_ReturnsBothSides
├── GetOppositeSide_ReturnsCorrectSide
└── GetSlot_ValidIndex_ReturnsSlot
```

### Completion Checklist

-   [x] `BattleSlot` implemented with tests (27 tests)
-   [x] `BattleSide` implemented with tests (26 tests)
-   [x] `BattleField` implemented with tests (22 tests)
-   [x] `BattleRules` implemented
-   [x] `IBattleView` interface defined
-   [x] `NullBattleView` for testing
-   [x] `BattleAction` abstract class defined
-   [x] `MessageAction` implemented with tests (6 tests)
-   [x] **Edge case tests** (52 tests)
-   [x] All tests pass (133 tests)
-   [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**

-   `BattleSlot` with SlotIndex, Pokemon, IsEmpty, HasFainted
-   `BattleSide` with Slots, Party, IsPlayer, GetActiveSlots, GetAvailableSwitches
-   `BattleField` with PlayerSide, EnemySide, GetAllActiveSlots, GetOppositeSide
-   `BattleRules` with PlayerSlots, EnemySlots (simplified naming from spec)
-   `BattleAction` with ExecuteLogic, ExecuteVisual, User, Priority

**API Changes from Spec:**

-   `PlayerSideSlots` → `PlayerSlots` (simpler)
-   `EnemySideSlots` → `EnemySlots` (simpler)
-   `Index` → `SlotIndex` (clearer)
-   `IsOccupied` → `IsEmpty` (inverted, clearer)
-   Added `HasFainted` helper property

**Deferred to Later Phases:**

-   `BattleSlot.Effects` (SlotEffects) → Phase 2.5+
-   `BattleSlot.ActionProvider` → Phase 2.3 (Turn Order)
-   `BattleSide.SideStatus` (Reflect, LightScreen) → Future
-   `BattleSide.SpikesCount` → Future (Hazards)
-   `BattleField.Weather` → Future (Weather system)
-   `BattleField.Terrain` → Future (Terrain system)

**Added (not in spec):**

-   Stat stages management in `BattleSlot` (needed for damage calc)
-   Volatile status in `BattleSlot` (needed for turn order checks)

---

## Phase 2.2: Action Queue

**Goal**: Process battle actions in sequence with logic/visual separation.

**Depends on**: Phase 2.1

### Components

| Component        | File                              | Description                                       |
| ---------------- | --------------------------------- | ------------------------------------------------- |
| `BattleQueue`    | `Combat/Engine/BattleQueue.cs`    | Action processor                                  |
| `BattleAction`   | `Combat/Actions/BattleAction.cs`  | Base action class (all actions inherit from this) |
| `MessageAction`  | `Combat/Actions/MessageAction.cs` | Simple message (for testing)                      |
| `NullBattleView` | `Combat/View/NullBattleView.cs`   | No-op view for tests                              |

### BattleQueue Specification

```csharp
public class BattleQueue
{
    public int Count { get; }
    public bool IsEmpty { get; }

    public void Enqueue(BattleAction action);
    public void EnqueueRange(IEnumerable<BattleAction> actions);
    public void InsertAtFront(IEnumerable<BattleAction> actions);  // For reactions
    public void Clear();

    public async Task ProcessQueue(BattleField field, IBattleView view);
}
```

### BattleAction Specification

```csharp
public abstract class BattleAction
{
    public BattleSlot User { get; }  // Who initiated (null for system actions)

    // Phase 1: Update game state (instant)
    public abstract IEnumerable<BattleAction> ExecuteLogic(BattleField field);

    // Phase 2: Show to player (async, can be skipped in tests)
    public abstract Task ExecuteVisual(IBattleView view);
}
```

### Tests to Write

```
Tests/Combat/BattleQueueTests.cs
├── Enqueue_SingleAction_IncreasesCount
├── ProcessQueue_SingleAction_ExecutesLogic
├── ProcessQueue_SingleAction_ExecutesVisual
├── ProcessQueue_ReactionActions_ExecutedImmediately
├── ProcessQueue_Empty_DoesNothing
├── ProcessQueue_MultipleActions_ExecutesInOrder
├── ProcessQueue_InfiniteLoop_ThrowsAfterLimit
├── InsertAtFront_AddsBeforeExisting
└── Clear_RemovesAllActions

Tests/Combat/Actions/MessageActionTests.cs
├── ExecuteLogic_ReturnsEmpty
├── ExecuteVisual_CallsShowMessage
└── Constructor_StoresMessage
```

### Completion Checklist

-   [x] `BattleQueue` implemented with tests (19 tests)
-   [x] `BattleAction` abstract class complete
-   [x] `MessageAction` implemented with tests (6 tests - Phase 2.1)
-   [x] `NullBattleView` for testing (Phase 2.1)
-   [x] Safety limit for infinite loops (1000 max iterations)
-   [x] **Edge case tests** (19 tests)
-   [x] All tests pass (38 BattleQueue tests)
-   [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**

-   `BattleQueue` with Enqueue, EnqueueRange, InsertAtFront, Clear, ProcessQueue
-   Reactions inserted at front (execute immediately after triggering action)
-   Safety counter prevents infinite loops (1000 iterations max)
-   Logic → Visual execution order

**API Additions:**

-   `Count` property for queue size
-   `IsEmpty` property for empty check
-   Uses `LinkedList<T>` for O(1) InsertAtFront

---

## Phase 2.3: Turn Order

**Goal**: Sort actions by priority and speed.

**Depends on**: Phase 2.2

### Components

| Component           | File                          | Description                    |
| ------------------- | ----------------------------- | ------------------------------ |
| `TurnOrderResolver` | `Combat/TurnOrderResolver.cs` | Sorts actions                  |
| `IActionProvider`   | `Combat/IActionProvider.cs`   | Interface for action selection |

### TurnOrderResolver Specification

```csharp
public static class TurnOrderResolver
{
    public static List<BattleAction> SortActions(
        IEnumerable<BattleAction> actions,
        BattleField field);

    // Exposed for testing
    public static int GetPriority(BattleAction action);
    public static float GetEffectiveSpeed(BattleSlot slot, BattleField field);
}
```

### Priority Rules

| Action Type       | Priority          |
| ----------------- | ----------------- |
| Switch            | +6 (always first) |
| Quick Attack      | +1                |
| Normal moves      | 0                 |
| Vital Throw       | -1                |
| Trick Room active | Reverse speed     |

### Speed Modifiers

| Modifier     | Effect            |
| ------------ | ----------------- |
| Stat stages  | ±6 stages formula |
| Paralysis    | ×0.5              |
| Choice Scarf | ×1.5 (future)     |
| Tailwind     | ×2.0 (future)     |

### Tests to Write

```
Tests/Combat/TurnOrderResolverTests.cs
├── SortActions_HigherPriority_GoesFirst
├── SortActions_SamePriority_FasterGoesFirst
├── SortActions_SameSpeed_RandomOrder
├── SortActions_SwitchAction_AlwaysFirst
├── GetEffectiveSpeed_NoModifiers_ReturnsBaseStat
├── GetEffectiveSpeed_Paralyzed_HalvesSpeed
├── GetEffectiveSpeed_PositiveStages_IncreasesSpeed
├── GetEffectiveSpeed_NegativeStages_DecreasesSpeed
├── GetPriority_QuickAttack_ReturnsOne
└── GetPriority_NormalMove_ReturnsZero
```

### Completion Checklist

-   [x] `TurnOrderResolver` implemented (21 functional tests)
-   [x] Priority extraction working
-   [x] Speed calculation with modifiers
-   [x] Paralysis penalty applied (×0.5)
-   [x] Stat stage modifiers applied (±6 stages)
-   [x] Random tiebreaker for equal speeds
-   [x] **Edge case tests** (27 tests)
-   [x] All tests pass (48 TurnOrder tests)
-   [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**

-   `TurnOrderResolver.SortActions()` - sorts by priority DESC, then speed DESC
-   `GetPriority()` - returns action.Priority
-   `GetEffectiveSpeed()` - base speed × stage modifier × status modifier
-   Stat stage formula: +stages = (2+n)/2, -stages = 2/(2+n)
-   Paralysis: ×0.5 speed
-   Random tiebreaker for equal speeds

**Deferred to Later Phases:**

-   Switch priority (+6) - SwitchAction not yet implemented
-   Choice Scarf (×1.5) - Item system
-   Tailwind (×2.0) - Field effects
-   Trick Room (reverse order) - Field effects
-   Ability modifiers (Swift Swim, etc.)

---

## Phase 2.4: Damage Calculation

**Goal**: Calculate exact damage using the official formula.

**Depends on**: Phase 2.1 (can be done in parallel with 2.2-2.3)

### Components

| Component        | File                              | Description             |
| ---------------- | --------------------------------- | ----------------------- |
| `DamageContext`  | `Combat/Damage/DamageContext.cs`  | Calculation state       |
| `IDamageStep`    | `Combat/Damage/IDamageStep.cs`    | Pipeline step interface |
| `DamagePipeline` | `Combat/Damage/DamagePipeline.cs` | Executes steps          |

### Pipeline Steps

| Step                    | Order | Description                                            |
| ----------------------- | ----- | ------------------------------------------------------ |
| `BaseDamageStep`        | 1     | Base formula: `((2*Level/5+2) * Power * A/D) / 50 + 2` |
| `CriticalHitStep`       | 2     | 1.5x multiplier, ignore negative stages                |
| `RandomFactorStep`      | 3     | 0.85 to 1.00 random roll                               |
| `StabStep`              | 4     | 1.5x if move type matches user type                    |
| `TypeEffectivenessStep` | 5     | 0x, 0.25x, 0.5x, 1x, 2x, 4x                            |
| `BurnStep`              | 6     | 0.5x for physical moves if burned                      |
| `ScreenStep`            | 7     | 0.5x if Reflect/Light Screen active (future)           |

### DamageContext Specification

```csharp
public class DamageContext
{
    // Inputs (readonly)
    public BattleSlot Attacker { get; }
    public BattleSlot Defender { get; }
    public MoveData Move { get; }
    public BattleField Field { get; }

    // Calculation state (mutable)
    public float BaseDamage { get; set; }
    public float Multiplier { get; set; } = 1.0f;
    public bool IsCritical { get; set; }
    public float TypeEffectiveness { get; set; } = 1.0f;
    public bool IsStab { get; set; }

    // Result
    public int FinalDamage => Math.Max(1, (int)(BaseDamage * Multiplier));
}
```

### Tests to Write

```
Tests/Combat/Damage/DamagePipelineTests.cs
├── Calculate_BasicAttack_ReturnsPositiveDamage
├── Calculate_SuperEffective_DoublesMultiplier
├── Calculate_NotEffective_HalvesMultiplier
├── Calculate_Immune_ReturnsZero
├── Calculate_STAB_Adds50Percent
├── Calculate_Critical_Adds50Percent
├── Calculate_Burned_HalvesPhysical
├── Calculate_Burned_DoesNotAffectSpecial
├── Calculate_MinimumDamage_IsOne
└── Calculate_VerifyAgainstKnownValues

Tests/Combat/Damage/Steps/BaseDamageStepTests.cs
├── Process_Level50_CalculatesCorrectly
├── Process_HighAttackLowDefense_HighDamage
├── Process_LowAttackHighDefense_LowDamage
└── Process_Power0_ReturnsZero

Tests/Combat/Damage/Steps/TypeEffectivenessStepTests.cs
├── Process_SuperEffective_SetsMultiplier2
├── Process_DualType4x_SetsMultiplier4
├── Process_Immune_SetsMultiplier0
└── Process_DualTypeWithImmunity_SetsMultiplier0
```

### Completion Checklist

-   [x] `DamageContext` implemented (immutable inputs, mutable state)
-   [x] `IDamageStep` interface defined
-   [x] `DamagePipeline` implemented (25 functional tests)
-   [x] `BaseDamageStep` with Gen 3+ formula
-   [x] `CriticalHitStep` implemented (1.5x, 1/24 base rate)
-   [x] `RandomFactorStep` implemented (0.85-1.0)
-   [x] `StabStep` implemented (1.5x)
-   [x] `TypeEffectivenessStep` (uses existing TypeEffectiveness)
-   [x] `BurnStep` implemented (0.5x for physical)
-   [x] **Edge case tests** (25 tests)
-   [x] **Real-world verification tests** (15 tests)
-   [x] All tests pass (65 DamagePipeline tests total)
-   [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**

-   Pipeline pattern with 6 steps
-   Gen 3+ base damage formula
-   All damage modifiers in correct order
-   Type effectiveness including immunity
-   STAB for primary and secondary types
-   Critical hit multiplier and base rate
-   Burn penalty for physical moves only
-   Status moves deal 0 damage
-   Minimum 1 damage (unless immune or status)
-   Fixed random support for deterministic testing

**Implemented in This Phase:**

-   Stat modifiers (Choice Band, Choice Specs, Assault Vest, Eviolite) via `IStatModifier`
-   Ability damage multipliers (Blaze, Torrent, Overgrow, Swarm) via `AttackerAbilityStep`
-   Item damage multipliers (Life Orb) via `AttackerItemStep`
-   Speed modifiers (Choice Scarf) via `TurnOrderResolver`

**Deferred to Later Phases:**

-   Multi-target penalty (0.75x)
-   Weather modifiers
-   Screen modifiers (Reflect/Light Screen)
-   Critical hit stage increases (Focus Energy)

---

## Phase 2.5: Combat Actions

**Goal**: Create actions for attacking, fainting, status effects.

**Depends on**: Phases 2.1-2.4

### Components

| Component           | File                                  | Description            |
| ------------------- | ------------------------------------- | ---------------------- |
| `DamageAction`      | `Combat/Actions/DamageAction.cs`      | Apply damage to target |
| `UseMoveAction`     | `Combat/Actions/UseMoveAction.cs`     | Full move execution    |
| `FaintAction`       | `Combat/Actions/FaintAction.cs`       | Handle KO              |
| `ApplyStatusAction` | `Combat/Actions/ApplyStatusAction.cs` | Apply status condition |
| `StatChangeAction`  | `Combat/Actions/StatChangeAction.cs`  | Modify stat stages     |
| `HealAction`        | `Combat/Actions/HealAction.cs`        | Restore HP             |
| `SwitchAction`      | `Combat/Actions/SwitchAction.cs`      | Switch Pokemon         |

### UseMoveAction Flow

```
UseMoveAction
├── Check PP (fail if 0)
├── Check volatile status (Flinch, Sleep, Paralysis roll)
├── Deduct PP
├── Generate child actions:
│   ├── MessageAction ("X used Y!")
│   ├── Accuracy check (may miss)
│   ├── DamageAction (if hits)
│   ├── Effect actions (status, stat changes)
│   └── Recoil/Drain actions
└── Return child actions for queue
```

### Tests to Write

```
Tests/Combat/Actions/DamageActionTests.cs
├── ExecuteLogic_DealsDamage
├── ExecuteLogic_TriggersFaint_WhenHPReachesZero
├── ExecuteLogic_DoesNotOverkill
├── ExecuteVisual_CallsPlayDamageAnimation
└── ExecuteVisual_CallsUpdateHPBar

Tests/Combat/Actions/UseMoveActionTests.cs
├── ExecuteLogic_NoPP_ReturnsFailMessage
├── ExecuteLogic_Flinched_ReturnsFailMessage
├── ExecuteLogic_Success_DeductsPP
├── ExecuteLogic_Success_ReturnsDamageAction
├── ExecuteLogic_WithStatusEffect_ReturnsStatusAction
├── ExecuteLogic_Misses_ReturnsOnlyMissMessage
└── ExecuteLogic_MultiHit_ReturnsMultipleDamageActions

Tests/Combat/Actions/FaintActionTests.cs
├── ExecuteLogic_MarksPokemonFainted
├── ExecuteLogic_ChecksForBattleEnd
└── ExecuteVisual_CallsFaintAnimation

Tests/Combat/Actions/SwitchActionTests.cs
├── ExecuteLogic_SwapsPokemon
├── ExecuteLogic_ResetsBattleState
└── ExecuteLogic_TriggersEntryEffects
```

### Completion Checklist

-   [x] `DamageAction` implemented with tests
-   [x] `UseMoveAction` implemented with tests
-   [x] `FaintAction` implemented with tests
-   [x] `ApplyStatusAction` implemented with tests
-   [x] `StatChangeAction` implemented with tests
-   [x] `HealAction` implemented with tests
-   [x] `SwitchAction` implemented with tests
-   [x] `MessageAction` implemented with tests
-   [x] PP deduction working
-   [x] Accuracy checks working (`AccuracyChecker`)
-   [x] Effect application working
-   [x] All tests pass (1,885 total)

### Documentation Created

-   [x] `docs/features/2-combat-system/2.5-combat-actions/use_cases.md` - 207 use cases documented
-   [x] `docs/features/2-combat-system/2.5-combat-actions/architecture.md` - Complete technical reference

---

## Phase 2.6: Combat Engine

**Goal**: Orchestrate the full battle loop using step-based pipeline architecture.

**Depends on**: Phases 2.1-2.5

### Components

| Component            | File                              | Description                                              |
| -------------------- | --------------------------------- | -------------------------------------------------------- |
| `CombatEngine`       | `Combat/Engine/CombatEngine.cs`   | Main controller (orchestrates Battle Flow: 8 steps)      |
| `TurnEngine`         | `Combat/Engine/TurnEngine.cs`     | Turn execution engine (orchestrates Turn Flow: 23 steps) |
| `BattleFlowExecutor` | `Combat/Engine/BattleFlow/`       | Executes battle flow steps                               |
| `TurnStepExecutor`   | `Combat/Engine/TurnFlow/`         | Executes turn flow steps                                 |
| `BattleArbiter`      | `Combat/Engine/BattleArbiter.cs`  | Victory/defeat detection                                 |
| `BattleOutcome`      | `Combat/Results/BattleOutcome.cs` | Battle result enum                                       |
| `BattleResult`       | `Combat/Results/BattleResult.cs`  | Detailed result data                                     |

### CombatEngine Specification

The engine uses a **step-based pipeline architecture**:

**Battle Flow** (8 steps):

1. CreateFieldStep - Creates BattleField
2. AssignActionProvidersStep - Assigns providers
3. CreateQueueStep - Creates BattleQueueService
4. ValidateInitialStateStep - Validates state
5. CreateDependenciesStep - Creates TurnEngine
6. BattleStartFlowStep - Battle start effects
7. ExecuteBattleLoopStep - Main battle loop
8. BattleEndFlowStep - Battle end handling

**Turn Flow** (23 unique steps, 34 total):

-   Preparation: TurnStart, ActionCollection, TargetResolution, ActionSorting
-   Move Validation: MoveValidation, MoveProtectionCheck, MoveAccuracyCheck
-   Before Move Effects: BeforeMoveEffects, ProcessGeneratedActions
-   Damage: MoveDamageCalculation, MoveDamageApplication, ProcessGeneratedActions
-   Animations: MoveAnimation
-   Reactive Effects: DamageTakenEffects, ContactReceivedEffects, ProcessGeneratedActions
-   Move Effects: MoveEffectProcessing, ProcessGeneratedActions
-   After Move Effects: AfterMoveEffects, ProcessGeneratedActions
-   Other Actions: SwitchActions, SwitchInEffects, StatusActions, ProcessGeneratedActions
-   Fainted Check: FaintedPokemonCheck (appears 3 times)
-   End of Turn: EndOfTurnEffects, DurationDecrement, TurnEnd, ProcessGeneratedActions

```csharp
public class CombatEngine
{
    public BattleField Field { get; }
    public BattleQueueService Queue { get; }
    public BattleOutcome Outcome { get; private set; }

    public void Initialize(BattleRules rules,
                          IReadOnlyList<PokemonInstance> playerParty,
                          IReadOnlyList<PokemonInstance> enemyParty,
                          IActionProvider playerProvider,
                          IActionProvider enemyProvider,
                          IBattleView view);

    public async Task<BattleResult> RunBattle();
    public async Task RunTurn();
}
```

### Battle Loop

```
RunBattle()
├── Initialize field
├── While (Outcome == Ongoing)
│   ├── RunTurn()
│   │   ├── Collect actions from providers
│   │   ├── Sort by turn order
│   │   ├── Enqueue all
│   │   ├── Process queue
│   │   └── End-of-turn effects
│   └── Check outcome
└── Return result
```

### Tests to Write

```
Tests/Combat/CombatEngineTests.cs
├── Initialize_SetsUpFieldCorrectly
├── RunTurn_CollectsActionsFromBothSides
├── RunTurn_SortsActionsBySpeed
├── RunTurn_ProcessesAllActions
├── RunBattle_EndsWhenOneSideDefeated
├── RunBattle_PlayerWins_ReturnsVictory
├── RunBattle_EnemyWins_ReturnsDefeat
└── RunBattle_MaxTurns_ReturnsTimeout

Tests/Combat/BattleArbiterTests.cs
├── CheckOutcome_BothAlive_ReturnsOngoing
├── CheckOutcome_EnemyDefeated_ReturnsVictory
├── CheckOutcome_PlayerDefeated_ReturnsDefeat
└── CheckOutcome_BothDefeated_ReturnsDraw
```

### Completion Checklist

-   [x] `CombatEngine` implemented with tests (9 functional + 12 edge cases)
-   [x] `BattleArbiter` implemented with tests (6 functional + 8 edge cases)
-   [x] `BattleOutcome` enum defined
-   [x] `BattleResult` class defined
-   [x] `IActionProvider` interface defined
-   [x] `TestActionProvider` helper for tests
-   [x] `RunTurn()` working
-   [x] `RunBattle()` loop working
-   [x] Victory detection working
-   [x] Defeat detection working
-   [x] All tests pass (30 tests total)

---

## Phase 2.7: Integration

**Goal**: Full battle simulation with AI opponents.

**Depends on**: Phase 2.6

### Components

| Component        | File                          | Description            |
| ---------------- | ----------------------------- | ---------------------- |
| `RandomAI`       | `Combat/AI/RandomAI.cs`       | Random move selection  |
| `AlwaysAttackAI` | `Combat/AI/AlwaysAttackAI.cs` | Always uses first move |

### Integration Tests

```
Tests/Combat/Integration/FullBattleTests.cs
├── FullBattle_1v1_ProducesWinner
├── FullBattle_TypeAdvantage_FavoredSideWins
├── FullBattle_HigherLevel_FavoredSideWins
├── FullBattle_ManyTurns_CompletesWithoutError
├── FullBattle_AllMovesUsed_PPDepletes
└── FullBattle_FaintAndSwitch_ContinuesBattle

Tests/Combat/Integration/EdgeCaseTests.cs
├── Battle_StatusMoves_ApplyCorrectly
├── Battle_StatChanges_AffectDamage
├── Battle_CriticalHits_IgnoreDefenseStages
├── Battle_TypeImmunity_DealsNoDamage
└── Battle_Flinch_SkipsTurn
```

### Unity Integration

```csharp
// Example Unity implementation
public class UnityBattleView : MonoBehaviour, IBattleView
{
    public async Task ShowMessage(string message) { /* UI */ }
    public async Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) { /* Anim */ }
    public async Task UpdateHPBar(BattleSlot slot) { /* UI */ }
    public async Task PlayFaintAnimation(BattleSlot slot) { /* Anim */ }
}

public class PlayerActionProvider : IActionProvider
{
    public async Task<BattleAction> GetAction(BattleField field, BattleSlot slot)
    {
        // Show UI, wait for player input
        // Return selected action
    }
}
```

### Completion Checklist

-   [x] `RandomAI` implemented with tests (12 tests)
-   [x] `AlwaysAttackAI` implemented with tests (9 tests)
-   [x] `TargetResolver` helper implemented
-   [x] Full 1v1 battle simulation working
-   [x] Multiple battles run without errors
-   [x] Type advantages affect outcome
-   [x] Status effects work in battle
-   [x] PP depletion works
-   [x] Integration tests pass (17 tests)
-   [x] Smoke test updated with AI tests
-   [x] All tests pass (38 new tests for Phase 2.7)

---

## Phase 2.9: Abilities & Items Battle Integration

**Goal**: Integrate abilities and items into battle flow using event-driven triggers.

**Depends on**: Phase 2.8 (End-of-Turn Effects)

### Components

| Component                | File                                      | Description                               |
| ------------------------ | ----------------------------------------- | ----------------------------------------- |
| `BattleTrigger`          | `Combat/Events/BattleTrigger.cs`          | Enum for battle event triggers            |
| `IBattleListener`        | `Combat/Events/IBattleListener.cs`        | Interface for reactive effects            |
| `AbilityListener`        | `Combat/Events/AbilityListener.cs`        | Adapts AbilityData to IBattleListener     |
| `ItemListener`           | `Combat/Events/ItemListener.cs`           | Adapts ItemData to IBattleListener        |
| `BattleTriggerProcessor` | `Combat/Events/BattleTriggerProcessor.cs` | Processes triggers for all active Pokemon |

### BattleTrigger Enum

```csharp
public enum BattleTrigger
{
    OnSwitchIn,      // Intimidate
    OnBeforeMove,    // Truant (future)
    OnAfterMove,     // Life Orb recoil (future)
    OnDamageTaken,   // Static, Rough Skin (future)
    OnTurnEnd,       // Leftovers, Speed Boost
    OnWeatherChange  // Swift Swim (future)
}
```

### IBattleListener Interface

```csharp
public interface IBattleListener
{
    IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field);
}
```

### Integration Points

1. **OnSwitchIn**: Triggered in `SwitchAction.ExecuteLogic()` after Pokemon switches in
2. **OnTurnEnd**: Triggered in `CombatEngine.RunTurn()` after end-of-turn status damage

### Implemented Effects

#### Items

-   **Leftovers**: Heals 1/16 Max HP at end of turn

#### Abilities

-   **Intimidate**: Lowers opponent Attack by 1 stage on switch-in

### Tests

-   **Functional Tests**: 8 tests (`BattleTriggerProcessorTests`)
-   **Integration Tests**: 4 tests (`AbilitiesItemsIntegrationTests`)
-   **Total**: 12 new tests

### Completion Checklist

-   [x] `BattleTrigger` enum defined
-   [x] `IBattleListener` interface defined
-   [x] `AbilityListener` implemented
-   [x] `ItemListener` implemented
-   [x] `BattleTriggerProcessor` implemented
-   [x] OnSwitchIn trigger integrated in `SwitchAction`
-   [x] OnTurnEnd trigger integrated in `CombatEngine`
-   [x] Leftovers item effect working
-   [x] Intimidate ability effect working
-   [x] Functional tests passing (8 tests)
-   [x] Integration tests passing (4 tests)
-   [x] All existing tests still passing (2,165 total)
-   [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**

-   Event-driven trigger system (`IBattleListener`)
-   Ability and Item adapters (`AbilityListener`, `ItemListener`)
-   Centralized trigger processing (`BattleTriggerProcessor`)
-   Integration with `CombatEngine` and `SwitchAction`

**Deferred to Later Phases:**

-   OnBeforeMove triggers (Truant, etc.)
-   OnAfterMove triggers (Life Orb recoil - damage multiplier ✅, recoil deferred)
-   OnDamageTaken triggers (Static, Rough Skin)
-   OnWeatherChange triggers (Swift Swim, etc.)
-   ~~Passive stat modifiers (Choice Band, etc.)~~ ✅ **COMPLETE** - IStatModifier system implemented
-   ~~Blaze ability~~ ✅ **COMPLETE** - HP threshold damage multiplier implemented
-   ~~Torrent, Overgrow, Swarm~~ ✅ **COMPLETE** - HP threshold damage multipliers implemented
-   ~~Choice Specs, Choice Scarf, Assault Vest, Eviolite~~ ✅ **COMPLETE** - Stat modifiers implemented
-   More ability effects (Speed Boost, etc.)
-   More item effects (Black Sludge, etc.)

**API Additions:**

-   `BattleTrigger` enum for battle events
-   `IBattleListener` interface for reactive effects
-   `BattleTriggerProcessor.ProcessTrigger()` static method

---

## Phase 2.10: Pipeline Hooks (Stat Modifiers) ✅ CONSOLIDATED INTO 2.4

**Note**: This phase has been consolidated into **Phase 2.4: Damage Calculation Pipeline**. The stat and damage modifier system (`IStatModifier`, `AbilityStatModifier`, `ItemStatModifier`) is an integral part of the damage pipeline and is implemented within Sub-Feature 2.4.

**Status**: ✅ **Complete** - Stat modifier system fully implemented as part of 2.4

See **[Phase 2.4: Damage Calculation Pipeline](#phase-24-damage-calculation-pipeline)** for complete implementation details.

---

## Phase 2.11: Recoil & Drain Effects ✅ COMPLETE

**Goal**: Implement recoil damage and drain healing for moves.

**Depends on**: Phase 2.5 (Combat Actions)

### Components

| Component      | File                           | Description                          |
| -------------- | ------------------------------ | ------------------------------------ |
| `RecoilEffect` | `Core/Effects/RecoilEffect.cs` | Recoil damage effect (25%, 33%, 50%) |
| `DrainEffect`  | `Core/Effects/DrainEffect.cs`  | Drain healing effect (50%, 75%)      |

### Completion Checklist

-   [x] `RecoilEffect` implemented (25%, 33%, 50% variants)
-   [x] `DrainEffect` implemented (50%, 75% variants)
-   [x] Recoil damage applied after move execution
-   [x] Drain healing applied after move execution
-   [x] All tests passing
-   [x] No compiler warnings

**Status**: ✅ **Complete** - Recoil and drain effects implemented

---

## Future Phases: Detailed Implementation Plan

The following phases extend the combat system with advanced features. Each phase builds on previous phases and must pass all tests before moving to the next.

**⚠️ Note**: The roadmap below contains detailed implementation phases. These phases are organized into Sub-Features 2.12-2.19 as defined in `features_master_list.md`:

-   **2.12**: Weather System
-   **2.13**: Terrain System
-   **2.14**: Hazards System
-   **2.15**: Advanced Move Mechanics
-   **2.16**: Field Conditions
-   **2.17**: Advanced Abilities
-   **2.18**: Advanced Items
-   **2.19**: Battle Formats

The detailed phases below may include intermediate implementation steps that are part of these sub-features.

---

## Phase 2.12: Extended End-of-Turn Effects (Part of 2.15: Advanced Move Mechanics)

**Goal**: Implement additional end-of-turn effects (Leech Seed, Wish, Perish Song, Binding damage).

**Depends on**: Phase 2.8 (End-of-Turn Effects), Phase 2.5 (Combat Actions)

### Components

| Component                     | File                                  | Description                           |
| ----------------------------- | ------------------------------------- | ------------------------------------- |
| `LeechSeedEffect`             | `Core/Effects/LeechSeedEffect.cs`     | Leech Seed volatile status effect     |
| `WishEffect`                  | `Core/Effects/WishEffect.cs`          | Wish delayed healing effect           |
| `PerishSongEffect`            | `Core/Effects/PerishSongEffect.cs`    | Perish Song countdown effect          |
| `BindingEffect`               | `Core/Effects/BindingEffect.cs`       | Binding move effect (Wrap, Fire Spin) |
| Extended `EndOfTurnProcessor` | `Combat/Engine/EndOfTurnProcessor.cs` | Process new effects                   |

### Leech Seed Specification

-   **Drain**: 1/8 of Max HP per turn
-   **Healing**: Drains to opponent (heals opponent by same amount)
-   **Duration**: Until Pokemon switches out or faints
-   **Immunity**: Grass types immune
-   **Message**: `"{pokemon.Name} is sapped by Leech Seed!"`

### Wish Specification

-   **Healing**: 50% of user's Max HP
-   **Delay**: 2 turns (heals at end of turn 2 turns after use)
-   **Tracking**: Track Wish healing amount and turn count
-   **Message**: `"{pokemon.Name} restored HP using its Wish!"`
-   **Multiple Wishes**: Can stack (heals all pending wishes)

### Perish Song Specification

-   **Effect**: Pokemon faints in 3 turns
-   **Counter**: Starts at 3, decrements each turn
-   **Tracking**: Store counter in volatile status or separate tracking
-   **Message**: `"{pokemon.Name} will faint in {counter} turns!"`
-   **Immunity**: Soundproof ability immune
-   **Switch Reset**: Switching resets counter for that Pokemon

### Binding Moves Specification

-   **Moves**: Wrap, Fire Spin, Bind, Clamp, Whirlpool, Sand Tomb
-   **Duration**: 2-5 turns (random)
-   **Damage**: 1/8 Max HP per turn
-   **Effect**: Cannot switch while bound
-   **Message**: `"{pokemon.Name} is hurt by {moveName}!"`

### Tests to Write

```
Tests/Combat/Engine/EndOfTurnProcessorExtendedTests.cs
├── ProcessEffects_LeechSeed_DrainsHP
├── ProcessEffects_LeechSeed_HealsOpponent
├── ProcessEffects_LeechSeed_GrassTypeImmune
├── ProcessEffects_Wish_HealsAfterTwoTurns
├── ProcessEffects_Wish_MultipleWishesStack
├── ProcessEffects_PerishSong_DecrementsCounter
├── ProcessEffects_PerishSong_FaintsAtZero
├── ProcessEffects_PerishSong_SwitchResetsCounter
├── ProcessEffects_Binding_DamagesPerTurn
├── ProcessEffects_Binding_PreventsSwitch
└── ProcessEffects_AllEffects_ProcessesInOrder

Tests/Combat/Engine/EndOfTurnProcessorExtendedEdgeCasesTests.cs
├── ProcessEffects_LeechSeed_FaintedPokemon_Skips
├── ProcessEffects_Wish_NoPendingWish_NoHealing
├── ProcessEffects_PerishSong_SoundproofImmune
└── ProcessEffects_Binding_ExpiredDuration_RemovesEffect
```

### Completion Checklist

-   [ ] `LeechSeedEffect` implemented
-   [ ] `WishEffect` implemented
-   [ ] `PerishSongEffect` implemented
-   [ ] `BindingEffect` implemented
-   [ ] Extended `EndOfTurnProcessor` processes new effects
-   [ ] Leech Seed drain and healing working
-   [ ] Wish delayed healing working
-   [ ] Perish Song countdown working
-   [ ] Binding damage working
-   [ ] Functional tests passing (~15 tests)
-   [ ] Edge case tests passing (~10 tests)
-   [ ] Integration tests passing (~5 tests)
-   [ ] All existing tests still passing
-   [ ] No compiler warnings

**Estimated Tests**: ~30 new tests

---

## Phase 2.13a: Additional Ability Triggers (Implementation Detail)

**Note**: This is a detailed implementation phase that is part of **Sub-Feature 2.17: Advanced Abilities**. The official Sub-Feature 2.13 is Terrain System (see below).

**Goal**: Implement remaining ability triggers (OnBeforeMove, OnAfterMove, OnDamageTaken, OnWeatherChange).

**Depends on**: Phase 2.9 (Abilities & Items), Phase 2.15 (Weather System for OnWeatherChange)

### Components

| Component                     | File                              | Description                   |
| ----------------------------- | --------------------------------- | ----------------------------- |
| Extended `BattleTrigger`      | `Combat/Events/BattleTrigger.cs`  | Add new trigger types         |
| `OnBeforeMove` integration    | `Combat/Actions/UseMoveAction.cs` | Trigger before move execution |
| `OnAfterMove` integration     | `Combat/Actions/UseMoveAction.cs` | Trigger after move execution  |
| `OnDamageTaken` integration   | `Combat/Actions/DamageAction.cs`  | Trigger when damage received  |
| `OnWeatherChange` integration | `Combat/Field/BattleField.cs`     | Trigger when weather changes  |

### OnBeforeMove Triggers

**Examples:**

-   **Truant**: Skip every other turn
-   **Slow Start**: Halve Attack/Speed for 5 turns
-   **Defeatist**: Halve Attack/SpAttack when HP < 50%

**Integration Point**: In `UseMoveAction.ExecuteLogic()` before PP check

### OnAfterMove Triggers

**Examples:**

-   **Life Orb Recoil**: 10% HP loss after damaging move
-   **Shell Bell**: Heal 1/8 of damage dealt
-   **Moxie**: +1 Attack after KO

**Integration Point**: In `UseMoveAction.ExecuteLogic()` after damage dealt

### OnDamageTaken Triggers

**Examples:**

-   **Static**: 30% chance to paralyze attacker on contact
-   **Rough Skin**: Attacker takes 1/16 HP damage on contact
-   **Iron Barbs**: Attacker takes 1/8 HP damage on contact
-   **Cursed Body**: 30% chance to disable move

**Integration Point**: In `DamageAction.ExecuteLogic()` after damage applied

### OnWeatherChange Triggers

**Examples:**

-   **Swift Swim**: 2x Speed in Rain
-   **Chlorophyll**: 2x Speed in Sun
-   **Sand Rush**: 2x Speed in Sandstorm
-   **Slush Rush**: 2x Speed in Hail

**Integration Point**: In `BattleField.SetWeather()` after weather changes

### Tests to Write

```
Tests/Combat/Events/BattleTriggerExtendedTests.cs
├── ProcessTrigger_OnBeforeMove_Truant_SkipsTurn
├── ProcessTrigger_OnBeforeMove_SlowStart_HalvesStats
├── ProcessTrigger_OnAfterMove_LifeOrb_RecoilDamage
├── ProcessTrigger_OnAfterMove_ShellBell_Heals
├── ProcessTrigger_OnDamageTaken_Static_ParalyzesAttacker
├── ProcessTrigger_OnDamageTaken_RoughSkin_DamagesAttacker
└── ProcessTrigger_OnWeatherChange_SwiftSwim_DoublesSpeed

Tests/Combat/Events/BattleTriggerExtendedEdgeCasesTests.cs
├── ProcessTrigger_OnBeforeMove_NoTrigger_DoesNothing
├── ProcessTrigger_OnAfterMove_NonDamagingMove_NoRecoil
├── ProcessTrigger_OnDamageTaken_NonContact_NoEffect
└── ProcessTrigger_OnWeatherChange_SameWeather_NoTrigger
```

### Completion Checklist

-   [ ] `OnBeforeMove` trigger added to `BattleTrigger` enum
-   [ ] `OnAfterMove` trigger added to `BattleTrigger` enum
-   [ ] `OnDamageTaken` trigger added to `BattleTrigger` enum
-   [ ] `OnWeatherChange` trigger added to `BattleTrigger` enum
-   [ ] `OnBeforeMove` integrated in `UseMoveAction`
-   [ ] `OnAfterMove` integrated in `UseMoveAction`
-   [ ] `OnDamageTaken` integrated in `DamageAction`
-   [ ] `OnWeatherChange` integrated in `BattleField.SetWeather()`
-   [ ] Truant ability working
-   [ ] Life Orb recoil working
-   [ ] Static ability working
-   [ ] Swift Swim ability working (requires Phase 2.15)
-   [ ] Functional tests passing (~15 tests)
-   [ ] Edge case tests passing (~10 tests)
-   [ ] Integration tests passing (~5 tests)
-   [ ] All existing tests still passing
-   [ ] No compiler warnings

**Estimated Tests**: ~30 new tests

**Note**: OnWeatherChange requires Phase 2.15 (Weather System) to be implemented first.

---

## Phase 2.14a: Volatile Status Effects (Implementation Detail)

**Note**: This is a detailed implementation phase that is part of **Sub-Feature 2.15: Advanced Move Mechanics**. The official Sub-Feature 2.14 is Hazards System (see below).

**Goal**: Implement remaining volatile status effects (Confusion, Infatuation, Taunt, Encore, Disable).

**Depends on**: Phase 2.5 (Combat Actions)

### Components

| Component                 | File                                | Description                 |
| ------------------------- | ----------------------------------- | --------------------------- |
| `ConfusionEffect`         | `Core/Effects/ConfusionEffect.cs`   | Confusion volatile status   |
| `InfatuationEffect`       | `Core/Effects/InfatuationEffect.cs` | Infatuation volatile status |
| `TauntEffect`             | `Core/Effects/TauntEffect.cs`       | Taunt volatile status       |
| `EncoreEffect`            | `Core/Effects/EncoreEffect.cs`      | Encore volatile status      |
| `DisableEffect`           | `Core/Effects/DisableEffect.cs`     | Disable volatile status     |
| Extended `VolatileStatus` | `Core/Enums/VolatileStatus.cs`      | Add new flags if needed     |
| Extended `UseMoveAction`  | `Combat/Actions/UseMoveAction.cs`   | Check volatile status       |

### Confusion Specification

-   **Effect**: 33% chance to hit self instead of target (Gen 7+)
-   **Self-Damage**: Uses 40 power physical move of same type
-   **Duration**: 2-5 turns (random)
-   **Message**: `"{pokemon.Name} is confused!"` / `"{pokemon.Name} hurt itself in confusion!"`
-   **Cure**: Persim Berry, switching out
-   **Immunity**: Own Tempo ability prevents

### Infatuation Specification

-   **Effect**: 50% chance to not act
-   **Gender Requirement**: Only works on opposite gender
-   **Duration**: Until Pokemon switches out
-   **Message**: `"{pokemon.Name} is in love with {target.Name}!"` / `"{pokemon.Name} is immobilized by love!"`
-   **Cure**: Mental Herb, switching out
-   **Immunity**: Oblivious ability prevents

### Taunt Specification

-   **Effect**: Cannot use status moves
-   **Duration**: 3 turns
-   **Message**: `"{pokemon.Name} fell for the taunt!"` / `"{pokemon.Name} can't use {moveName} after the taunt!"`
-   **Cure**: Switching out
-   **Immunity**: Dark types immune to Prankster-boosted Taunt

### Encore Specification

-   **Effect**: Forced to use last move used
-   **Duration**: 3 turns
-   **Message**: `"{pokemon.Name} received an encore!"` / `"{pokemon.Name} must use {moveName}!"`
-   **Failure**: Fails if target hasn't moved yet
-   **Cure**: Switching out

### Disable Specification

-   **Effect**: One specific move cannot be used
-   **Duration**: 4 turns
-   **Message**: `"{pokemon.Name}'s {moveName} was disabled!"` / `"{pokemon.Name} can't use {moveName}!"`
-   **Target**: Only disables the move that was used
-   **Cure**: Switching out

### Tests to Write

```
Tests/Combat/Actions/VolatileStatusTests.cs
├── UseMoveAction_Confused_ChanceToHitSelf
├── UseMoveAction_Confused_SelfDamageCalculated
├── UseMoveAction_Infatuated_ChanceToFail
├── UseMoveAction_Infatuated_SameGender_NoEffect
├── UseMoveAction_Taunted_StatusMoveFails
├── UseMoveAction_Taunted_DamageMoveWorks
├── UseMoveAction_Encore_ForcedToUseLastMove
├── UseMoveAction_Encore_NoPreviousMove_Fails
├── UseMoveAction_Disabled_MoveCannotBeUsed
└── UseMoveAction_Disabled_OtherMovesWork

Tests/Combat/Actions/VolatileStatusEdgeCasesTests.cs
├── UseMoveAction_Confused_OwnTempoImmune
├── UseMoveAction_Infatuated_ObliviousImmune
├── UseMoveAction_Taunt_DarkTypeImmune
├── UseMoveAction_Switch_ClearsAllVolatile
└── UseMoveAction_MultipleVolatile_AllApply
```

### Completion Checklist

-   [ ] `ConfusionEffect` implemented
-   [ ] `InfatuationEffect` implemented
-   [ ] `TauntEffect` implemented
-   [ ] `EncoreEffect` implemented
-   [ ] `DisableEffect` implemented
-   [ ] Confusion self-damage working
-   [ ] Infatuation chance to fail working
-   [ ] Taunt blocks status moves working
-   [ ] Encore forces last move working
-   [ ] Disable blocks specific move working
-   [ ] Functional tests passing (~15 tests)
-   [ ] Edge case tests passing (~10 tests)
-   [ ] Integration tests passing (~5 tests)
-   [ ] All existing tests still passing
-   [ ] No compiler warnings

**Estimated Tests**: ~30 new tests

---

## Phase 2.12: Weather System (Sub-Feature 2.12)

**Goal**: Implement weather effects (Sun, Rain, Sandstorm, Hail) with damage, modifiers, and ability interactions.

**Depends on**: Phase 2.1 (Battle Foundation), Phase 2.4 (Damage Pipeline)

### Components

| Component             | File                                  | Description                    |
| --------------------- | ------------------------------------- | ------------------------------ |
| `Weather` enum        | `Core/Enums/Weather.cs`               | Weather types (already exists) |
| `BattleField.Weather` | `Combat/Field/BattleField.cs`         | Current weather state          |
| `WeatherDuration`     | `Combat/Field/BattleField.cs`         | Weather turn counter           |
| `WeatherStep`         | `Combat/Damage/Steps/WeatherStep.cs`  | Weather damage modifiers       |
| `WeatherDamageStep`   | `Combat/Engine/EndOfTurnProcessor.cs` | Weather damage at end of turn  |

### Weather Types

#### Sun (Harsh Sunlight)

-   **Fire moves**: 1.5x damage
-   **Water moves**: 0.5x damage
-   **Solar Beam**: No charge turn
-   **Thunder**: 50% accuracy
-   **Moonlight/Morning Sun/Synthesis**: Heal 2/3 HP
-   **Growth**: +2 instead of +1
-   **Chlorophyll**: 2x Speed
-   **Duration**: 5 turns (or infinite with item)

#### Rain

-   **Water moves**: 1.5x damage
-   **Fire moves**: 0.5x damage
-   **Thunder**: 100% accuracy, bypasses Protect
-   **Hurricane**: 100% accuracy
-   **Solar Beam**: 1-turn but half power
-   **Moonlight/etc**: Heal 1/4 HP
-   **Swift Swim**: 2x Speed
-   **Duration**: 5 turns (or infinite with item)

#### Sandstorm

-   **Damage**: 1/16 HP to non-Rock/Ground/Steel types
-   **Rock types**: +50% SpDefense
-   **Sand Rush**: 2x Speed
-   **Sand Veil**: +20% Evasion
-   **Duration**: 5 turns (or infinite with item)

#### Hail/Snow

-   **Damage**: 1/16 HP to non-Ice types
-   **Ice types**: +50% Defense (Gen 9 Snow)
-   **Blizzard**: 100% accuracy
-   **Aurora Veil**: Can be set
-   **Slush Rush**: 2x Speed
-   **Snow Cloak**: +20% Evasion
-   **Duration**: 5 turns (or infinite with item)

### Tests to Write

```
Tests/Combat/Field/WeatherSystemTests.cs
├── SetWeather_Sun_SetsCorrectly
├── SetWeather_Rain_SetsCorrectly
├── SetWeather_Sandstorm_SetsCorrectly
├── SetWeather_Hail_SetsCorrectly
├── WeatherDuration_DecrementsEachTurn
├── WeatherDuration_Expires_RemovesWeather
├── WeatherDuration_InfiniteItem_DoesNotExpire
└── WeatherChange_TriggersOnWeatherChange

Tests/Combat/Damage/WeatherStepTests.cs
├── Process_Sun_FireMoves_1_5xDamage
├── Process_Sun_WaterMoves_0_5xDamage
├── Process_Rain_WaterMoves_1_5xDamage
├── Process_Rain_FireMoves_0_5xDamage
└── Process_NoWeather_NoModifier

Tests/Combat/Engine/WeatherDamageTests.cs
├── ProcessEffects_Sandstorm_DamagesNonImmune
├── ProcessEffects_Sandstorm_RockTypeImmune
├── ProcessEffects_Hail_DamagesNonImmune
├── ProcessEffects_Hail_IceTypeImmune
└── ProcessEffects_NoWeather_NoDamage
```

### Completion Checklist

-   [x] `BattleField.Weather` property added
-   [x] `WeatherDuration` tracking implemented
-   [x] `WeatherStep` integrated into DamagePipeline
-   [x] Weather damage in `EndOfTurnProcessor`
-   [x] Sun modifiers working
-   [x] Rain modifiers working
-   [x] Sandstorm damage working
-   [x] Hail damage working
-   [x] Weather duration tracking working
-   [x] Weather expiration working
-   [x] Perfect accuracy moves (Thunder/Hurricane in Rain, Blizzard in Hail)
-   [x] Functional tests passing (48+ tests)
-   [x] Edge case tests passing
-   [x] Integration tests passing
-   [x] All existing tests still passing
-   [x] No compiler warnings

**Status**: ✅ **Core Weather System Complete**

**Advanced Features Pending** (depend on other systems):

-   Solar Beam: No charge turn (requires charging moves system)
-   Moonlight/Morning Sun/Synthesis: Heal modifiers (requires healing moves system)
-   Growth: +2 instead of +1 (requires stat boost moves system)
-   Chlorophyll/Swift Swim/Sand Rush/Slush Rush: 2x Speed (requires ability stat modifiers)
-   Rock types: +50% SpDefense in Sandstorm (requires type-based stat modifiers)
-   Ice types: +50% Defense in Snow (requires type-based stat modifiers)
-   Sand Veil/Snow Cloak: +20% Evasion (requires ability evasion modifiers)

---

## Phase 2.13: Terrain System (Sub-Feature 2.13)

**Goal**: Implement terrain effects (Electric, Grassy, Psychic, Misty) with modifiers and interactions.

**Depends on**: Phase 2.1 (Battle Foundation), Phase 2.4 (Damage Pipeline), Phase 2.15 (Weather System)

### Components

| Component             | File                                  | Description                    |
| --------------------- | ------------------------------------- | ------------------------------ |
| `Terrain` enum        | `Core/Enums/Terrain.cs`               | Terrain types (already exists) |
| `BattleField.Terrain` | `Combat/Field/BattleField.cs`         | Current terrain state          |
| `TerrainDuration`     | `Combat/Field/BattleField.cs`         | Terrain turn counter           |
| `TerrainStep`         | `Combat/Damage/Steps/TerrainStep.cs`  | Terrain damage modifiers       |
| `TerrainHealingStep`  | `Combat/Engine/EndOfTurnProcessor.cs` | Terrain healing at end of turn |

### Terrain Types

#### Electric Terrain

-   **Electric moves**: 1.3x damage
-   **Prevents Sleep**: Grounded Pokemon cannot be put to sleep
-   **Rising Voltage**: 2x power
-   **Duration**: 5 turns
-   **Affects**: Only grounded Pokemon

#### Grassy Terrain

-   **Grass moves**: 1.3x damage
-   **Healing**: 1/16 HP at end of turn
-   **Earthquake/Bulldoze/Magnitude**: 0.5x damage
-   **Grassy Glide**: +1 priority
-   **Duration**: 5 turns
-   **Affects**: Only grounded Pokemon

#### Psychic Terrain

-   **Psychic moves**: 1.3x damage
-   **Blocks Priority**: Priority moves blocked against grounded Pokemon
-   **Expanding Force**: 1.5x power, hits all
-   **Duration**: 5 turns
-   **Affects**: Only grounded Pokemon

#### Misty Terrain

-   **Dragon moves**: 0.5x damage
-   **Prevents Status**: Grounded Pokemon cannot be statused
-   **Misty Explosion**: 1.5x power
-   **Duration**: 5 turns
-   **Affects**: Only grounded Pokemon

### Tests to Write

```
Tests/Combat/Field/TerrainSystemTests.cs
├── SetTerrain_Electric_SetsCorrectly
├── SetTerrain_Grassy_SetsCorrectly
├── SetTerrain_Psychic_SetsCorrectly
├── SetTerrain_Misty_SetsCorrectly
├── TerrainDuration_DecrementsEachTurn
├── TerrainDuration_Expires_RemovesTerrain
└── TerrainChange_TriggersOnTerrainChange

Tests/Combat/Damage/TerrainStepTests.cs
├── Process_ElectricTerrain_ElectricMoves_1_3xDamage
├── Process_GrassyTerrain_GrassMoves_1_3xDamage
├── Process_PsychicTerrain_PsychicMoves_1_3xDamage
├── Process_MistyTerrain_DragonMoves_0_5xDamage
└── Process_NoTerrain_NoModifier

Tests/Combat/Engine/TerrainEffectsTests.cs
├── ProcessEffects_GrassyTerrain_HealsGrounded
├── ProcessEffects_GrassyTerrain_FlyingImmune
├── ProcessEffects_ElectricTerrain_PreventsSleep
└── ProcessEffects_PsychicTerrain_BlocksPriority
```

### Completion Checklist

-   [x] `BattleField.Terrain` property added
-   [x] `TerrainDuration` tracking implemented
-   [x] `TerrainStep` integrated into DamagePipeline
-   [x] Terrain healing in `EndOfTurnProcessor`
-   [x] Electric Terrain modifiers working
-   [x] Grassy Terrain modifiers and healing working
-   [x] Psychic Terrain modifiers working
-   [x] Misty Terrain modifiers working
-   [x] Grounded Pokemon detection working
-   [x] Terrain duration tracking working
-   [x] Terrain expiration working
-   [x] SetTerrainAction implemented
-   [x] Terrain duration decrement in CombatEngine
-   [x] Functional tests passing (84+ tests)
-   [x] Edge case tests passing
-   [x] Integration tests passing
-   [x] All existing tests still passing
-   [x] No compiler warnings

**Status**: ✅ **Core Terrain System Complete**

**Advanced Features Pending** (depend on other systems):

-   Status prevention (Electric prevents Sleep, Misty prevents all status) - requires status application system modifications
-   Priority blocking (Psychic Terrain blocks priority moves) - requires priority move system
-   Move-specific modifications (Earthquake halved, Grassy Glide priority, Rising Voltage 2x, etc.) - requires move-specific effect system

---

## Phase 2.14: Hazards System (Sub-Feature 2.14)

**Goal**: Implement entry hazards (Spikes, Stealth Rock, Toxic Spikes, Sticky Web) that activate when Pokemon switch in.

**Depends on**: Phase 2.5 (Combat Actions), Phase 2.7 (SwitchAction)

### Components

| Component              | File                                    | Description                       |
| ---------------------- | --------------------------------------- | --------------------------------- |
| `HazardData`           | `Core/Blueprints/HazardData.cs`         | Hazard blueprint (already exists) |
| `BattleSide.Hazards`   | `Combat/Field/BattleSide.cs`            | Track hazards on side             |
| `SpikesLayer`          | `Combat/Field/BattleSide.cs`            | Track Spikes layers (1-3)         |
| `ToxicSpikesLayer`     | `Combat/Field/BattleSide.cs`            | Track Toxic Spikes layers (1-2)   |
| `EntryHazardProcessor` | `Combat/Engine/EntryHazardProcessor.cs` | Process hazards on switch-in      |

### Spikes Specification

-   **Layers**: 1-3 layers maximum
-   **Damage**:
    -   1 layer: 12.5% max HP
    -   2 layers: 16.67% max HP
    -   3 layers: 25% max HP
-   **Immunity**: Flying types and Levitate immune
-   **Removal**: Rapid Spin, Defog
-   **Message**: `"{pokemon.Name} is hurt by Spikes!"`

### Stealth Rock Specification

-   **Damage**: Based on type effectiveness vs Rock
    -   0.25x: 3.125% HP
    -   0.5x: 6.25% HP
    -   1x: 12.5% HP
    -   2x: 25% HP
    -   4x: 50% HP (Charizard, Volcarona)
-   **Immunity**: None (all Pokemon affected)
-   **Removal**: Rapid Spin, Defog
-   **Message**: `"{pokemon.Name} is hurt by Stealth Rock!"`

### Toxic Spikes Specification

-   **Layers**: 1-2 layers maximum
-   **Effect**:
    -   1 layer: Poison status
    -   2 layers: Badly Poisoned status
-   **Absorption**: Poison types absorb on entry (removes spikes)
-   **Immunity**: Flying types and Levitate immune
-   **Removal**: Rapid Spin, Defog
-   **Message**: `"{pokemon.Name} was poisoned by Toxic Spikes!"`

### Sticky Web Specification

-   **Effect**: -1 Speed on entry
-   **Immunity**: Flying types and Levitate immune
-   **Removal**: Rapid Spin, Defog
-   **Message**: `"{pokemon.Name} was caught in a Sticky Web!"`
-   **Contrary**: Reverses to +1 Speed

### Tests to Write

```
Tests/Combat/Field/EntryHazardsTests.cs
├── AddHazard_Spikes_AddsLayer
├── AddHazard_Spikes_MaxLayers_ClampsToThree
├── AddHazard_ToxicSpikes_MaxLayers_ClampsToTwo
├── RemoveHazard_RapidSpin_RemovesAll
└── RemoveHazard_Defog_RemovesAll

Tests/Combat/Engine/EntryHazardProcessorTests.cs
├── ProcessHazards_Spikes_DamagesOnEntry
├── ProcessHazards_Spikes_FlyingImmune
├── ProcessHazards_StealthRock_DamageByType
├── ProcessHazards_ToxicSpikes_OneLayer_Poisons
├── ProcessHazards_ToxicSpikes_TwoLayers_BadlyPoisons
├── ProcessHazards_ToxicSpikes_PoisonType_Absorbs
├── ProcessHazards_StickyWeb_LowersSpeed
└── ProcessHazards_StickyWeb_Contrary_RaisesSpeed

Tests/Combat/Engine/EntryHazardProcessorEdgeCasesTests.cs
├── ProcessHazards_NoHazards_NoEffect
├── ProcessHazards_FaintedPokemon_Skips
└── ProcessHazards_MultipleHazards_AllProcess
```

### Completion Checklist

-   [x] `BattleSide.Hazards` tracking implemented
-   [x] `SpikesLayer` tracking implemented
-   [x] `ToxicSpikesLayer` tracking implemented
-   [x] `EntryHazardProcessor` implemented
-   [x] Spikes damage working (1-3 layers)
-   [x] Stealth Rock damage working (type-based)
-   [x] Toxic Spikes status application working (1-2 layers)
-   [x] Sticky Web speed reduction working
-   [ ] Hazard removal working (Rapid Spin, Defog) - **Pending move-specific implementation**
-   [x] Immunity checks working (Flying, Levitate)
-   [x] Functional tests passing (25+ tests)
-   [x] Edge case tests passing (included in functional tests)
-   [x] Integration tests passing (SwitchAction integration)
-   [x] All existing tests still passing (2488 total)
-   [x] No compiler warnings

**Tests Implemented**: 25+ new tests (12 EntryHazardsTests + 13 EntryHazardProcessorTests)

**Status**: ✅ Core Hazards System Complete (hazard removal actions pending move-specific implementation)

---

## Phase 2.16: Field Conditions (Sub-Feature 2.16)

**Goal**: Implement side-specific field conditions (Screens, Tailwind, Safeguard, Mist, etc.).

**Depends on**: Phase 2.4 (Damage Calculation Pipeline), Phase 2.3 (Turn Order Resolution), Phase 2.5 (Combat Actions)

### Components

| Component                      | File                                       | Description                       |
| ------------------------------ | ------------------------------------------ | --------------------------------- |
| `BattleSide` (extended)        | `Combat/Field/BattleSide.cs`               | Side condition tracking           |
| `ScreenStep`                   | `Combat/Damage/Steps/ScreenStep.cs`        | Screen damage reduction           |
| `TurnOrderResolver` (extended) | `Combat/Helpers/TurnOrderResolver.cs`      | Tailwind speed multiplier         |
| `ApplyStatusAction` (extended) | `Combat/Actions/ApplyStatusAction.cs`      | Safeguard protection              |
| `StatChangeAction` (extended)  | `Combat/Actions/StatChangeAction.cs`       | Mist protection                   |
| `SetSideConditionAction`       | `Combat/Actions/SetSideConditionAction.cs` | Apply side conditions             |
| `CombatEngine` (extended)      | `Combat/Engine/CombatEngine.cs`            | Side condition duration decrement |

### Screens Specification

-   **Reflect**: Reduces physical damage by 50% (Singles) or 33% (Doubles)
-   **Light Screen**: Reduces special damage by 50% (Singles) or 33% (Doubles)
-   **Aurora Veil**: Reduces all damage by 50% (Singles) or 33% (Doubles), requires Hail/Snow
-   **Duration**: 5 turns (8 with Light Clay)
-   **Removed by**: Brick Break, Psychic Fangs, Defog

### Tailwind Specification

-   **Effect**: Doubles Speed for the side
-   **Duration**: 4 turns
-   **Applied to**: All Pokemon on the side

### Safeguard Specification

-   **Effect**: Prevents status conditions
-   **Duration**: 5 turns
-   **Protection**: All status types (Poison, Burn, Paralysis, Sleep, Freeze)

### Mist Specification

-   **Effect**: Prevents stat reductions from opponents
-   **Duration**: 5 turns
-   **Protection**: Only stat reductions from opponents (allows self-inflicted reductions)

### Completion Checklist

-   [x] Side condition tracking in BattleSide
-   [x] Screen damage reduction in DamagePipeline (Reflect, Light Screen, Aurora Veil)
-   [x] Tailwind speed multiplier in TurnOrderResolver
-   [x] Safeguard status protection in ApplyStatusAction
-   [x] Mist stat reduction protection in StatChangeAction
-   [x] Side condition duration management in CombatEngine
-   [x] SetSideConditionAction for applying conditions
-   [x] Aurora Veil weather requirement check
-   [x] Functional tests passing (40+ tests)
-   [x] All existing tests still passing
-   [x] No compiler warnings

**Tests Implemented**: 40+ new tests (16 SideConditionTrackingTests + 7 ScreenStepTests + 4 TailwindSpeedTests + 4 SafeguardProtectionTests + 6 SetSideConditionActionTests + 4 SideConditionDurationTests)

**Status**: ✅ Core Field Conditions System Complete

---

## Phase 2.15: Advanced Move Mechanics (Sub-Feature 2.15) ✅ CORE COMPLETE

**Goal**: Implement special move mechanics (Protect, Counter, Pursuit, Focus Punch, Semi-Invulnerable moves).

**Depends on**: Phase 2.5 (Combat Actions)

**Status**: ✅ **Core Complete** - All basic special move mechanics implemented. Advanced variants (Wide Guard, Quick Guard, King's Shield, etc.) pending.

### Components

| Component                | File                                     | Description                |
| ------------------------ | ---------------------------------------- | -------------------------- |
| `ProtectEffect`          | `Core/Effects/ProtectEffect.cs`          | Protect/Detect effect      |
| `CounterEffect`          | `Core/Effects/CounterEffect.cs`          | Counter/Mirror Coat effect |
| `PursuitEffect`          | `Core/Effects/PursuitEffect.cs`          | Pursuit effect             |
| `FocusPunchEffect`       | `Core/Effects/FocusPunchEffect.cs`       | Focus Punch effect         |
| `SemiInvulnerableEffect` | `Core/Effects/SemiInvulnerableEffect.cs` | Fly/Dig/Dive effect        |
| Extended `UseMoveAction` | `Combat/Actions/UseMoveAction.cs`        | Handle special mechanics   |

### Protect/Detect Specification

-   **Effect**: Blocks most moves
-   **Success Rate**: Starts at 100%, halves with consecutive use (50%, 25%, 12.5%...)
-   **Priority**: +4
-   **Bypass**: Feint, Shadow Force, Phantom Force, Perish Song
-   **Message**: `"{pokemon.Name} protected itself!"` / `"{pokemon.Name} avoided the attack!"`

### Counter/Mirror Coat Specification

-   **Counter**: Returns 2x physical damage taken
-   **Mirror Coat**: Returns 2x special damage taken
-   **Priority**: -5 (moves last)
-   **Failure**: Fails if not hit by appropriate damage type
-   **Uses**: Damage from last hit that turn
-   **Message**: `"{pokemon.Name} countered the attack!"`

### Pursuit Specification

-   **Effect**: 2x power if target switches
-   **Timing**: Hits before switch completes
-   **Priority**: Normal (0)
-   **Message**: `"{pokemon.Name} is switching out! {attacker.Name} used Pursuit!"`

### Focus Punch Specification

-   **Effect**: User "tightens focus" at start of turn
-   **Priority**: -3
-   **Failure**: If hit before moving, move fails
-   **PP**: Still deducted even if fails
-   **Message**: `"{pokemon.Name} is tightening its focus!"` / `"{pokemon.Name} lost its focus!"`

### Semi-Invulnerable Moves Specification

-   **Moves**: Fly, Dig, Dive, Bounce, Shadow Force, Phantom Force
-   **Effect**: User becomes semi-invulnerable (most moves miss)
-   **Duration**: 2 turns (charge turn, attack turn)
-   **Exceptions**:
    -   Earthquake hits Dig users
    -   Surf hits Dive users
    -   Thunder hits Fly users (in rain)
    -   No Guard hits all
-   **Message**: `"{pokemon.Name} flew up high!"` / `"{pokemon.Name} used {moveName}!"`

### Tests to Write

```
Tests/Combat/Actions/ProtectTests.cs
├── UseMoveAction_Protect_BlocksMove
├── UseMoveAction_Protect_ConsecutiveUse_HalvesChance
├── UseMoveAction_Protect_Feint_Bypasses
└── UseMoveAction_Protect_Priority_Plus4

Tests/Combat/Actions/CounterTests.cs
├── UseMoveAction_Counter_Returns2xPhysicalDamage
├── UseMoveAction_Counter_SpecialDamage_Fails
├── UseMoveAction_MirrorCoat_Returns2xSpecialDamage
└── UseMoveAction_Counter_Priority_Minus5

Tests/Combat/Actions/PursuitTests.cs
├── UseMoveAction_Pursuit_TargetSwitches_2xPower
├── UseMoveAction_Pursuit_TargetDoesNotSwitch_NormalPower
└── UseMoveAction_Pursuit_HitsBeforeSwitch

Tests/Combat/Actions/FocusPunchTests.cs
├── UseMoveAction_FocusPunch_NotHit_Succeeds
├── UseMoveAction_FocusPunch_HitBeforeMoving_Fails
└── UseMoveAction_FocusPunch_Fails_StillDeductsPP

Tests/Combat/Actions/SemiInvulnerableTests.cs
├── UseMoveAction_Fly_ChargeTurn_SemiInvulnerable
├── UseMoveAction_Fly_AttackTurn_Hits
├── UseMoveAction_Fly_Earthquake_Hits
└── UseMoveAction_Dig_Surf_Hits
```

### Completion Checklist

-   [x] `ProtectEffect` implemented ✅
-   [x] `CounterEffect` implemented ✅
-   [x] `PursuitEffect` implemented ✅
-   [x] `FocusPunchEffect` implemented ✅
-   [x] `SemiInvulnerableEffect` implemented ✅
-   [x] Protect blocking working ✅
-   [x] Counter/Mirror Coat damage return working ✅
-   [x] Pursuit 2x power on switch working ✅
-   [x] Focus Punch focus/fail logic working ✅
-   [x] Semi-invulnerable moves working ✅
-   [x] Functional tests passing (35+ tests) ✅
-   [x] Edge case tests passing (included in functional tests) ✅
-   [x] Integration tests passing (AdvancedMoveMechanicsIntegrationTests) ✅
-   [x] All existing tests still passing ✅
-   [x] No compiler warnings ✅

**Tests Implemented**: 35+ tests (Protect: 13, Counter: 4, Pursuit: 3, FocusPunch: 3, SemiInvulnerable: 6, MultiHit: 3, MultiTurn: 3, Integration: multiple)

**Status**: ✅ **Core Advanced Move Mechanics Complete** - All basic special move mechanics implemented and tested. Advanced variants (Wide Guard, Quick Guard, etc.) pending.

---

## Phase 2.15b: Multi-Hit and Multi-Turn Moves (Part of Sub-Feature 2.15) ✅ CORE COMPLETE

**Goal**: Implement multi-hit moves (Double Slap, Bullet Seed) and multi-turn moves (Solar Beam, Hyper Beam, Outrage).

**Depends on**: Phase 2.5 (Combat Actions)

**Status**: ✅ **Core Complete** - Basic multi-hit and multi-turn mechanics implemented. Advanced features (Substitute interaction, move-specific behaviors) pending.

### Components

| Component                | File                              | Description                                   |
| ------------------------ | --------------------------------- | --------------------------------------------- |
| `MultiHitEffect`         | `Core/Effects/MultiHitEffect.cs`  | Multi-hit move effect                         |
| `MultiTurnEffect`        | `Core/Effects/MultiTurnEffect.cs` | Multi-turn move effect                        |
| `MoveState`              | `Combat/Actions/MoveState.cs`     | Track move state (charging, recharging, etc.) |
| Extended `UseMoveAction` | `Combat/Actions/UseMoveAction.cs` | Handle multi-hit/turn logic                   |

### Multi-Hit Moves Specification

-   **2 hits guaranteed**: Double Slap (base 2 hits)
-   **2-5 hits random**: 35% for 2, 35% for 3, 15% for 4, 15% for 5
-   **Fixed hits**: Triple Kick (3), Population Bomb (1-10)
-   **Each hit**: Can crit independently
-   **Substitute**: Multi-hit breaks Substitute then continues
-   **Message**: `"{pokemon.Name} hit {target.Name} {count} times!"`

### Multi-Turn Moves Specification

#### Charging Moves (Solar Beam, Skull Bash)

-   **Turn 1**: Charge turn (user charges, can be interrupted)
-   **Turn 2**: Attack turn (deals damage)
-   **Sun**: Solar Beam skips charge turn

#### Recharging Moves (Hyper Beam, Giga Impact)

-   **Turn 1**: Attack turn (deals damage)
-   **Turn 2**: Recharge turn (user must recharge, cannot act)

#### Continuous Moves (Outrage, Petal Dance)

-   **Duration**: 2-3 turns (random)
-   **Effect**: Deals damage each turn
-   **After**: User becomes confused

#### Binding Moves (Wrap, Fire Spin)

-   **Duration**: 2-5 turns (random)
-   **Effect**: Deals damage each turn, prevents switch
-   **Already implemented**: See Phase 2.12

### Tests to Write

```
Tests/Combat/Actions/MultiHitTests.cs
├── UseMoveAction_MultiHit_2Hits_DealsDamageTwice
├── UseMoveAction_MultiHit_2to5Hits_RandomCount
├── UseMoveAction_MultiHit_EachHitCanCrit
├── UseMoveAction_MultiHit_BreaksSubstitute
└── UseMoveAction_MultiHit_MessageShowsCount

Tests/Combat/Actions/MultiTurnTests.cs
├── UseMoveAction_SolarBeam_ChargeTurn_NoDamage
├── UseMoveAction_SolarBeam_AttackTurn_DealsDamage
├── UseMoveAction_SolarBeam_Sun_SkipsCharge
├── UseMoveAction_HyperBeam_AttackTurn_DealsDamage
├── UseMoveAction_HyperBeam_RechargeTurn_CannotAct
├── UseMoveAction_Outrage_2to3Turns_DealsDamageEachTurn
└── UseMoveAction_Outrage_Ends_ConfusesUser
```

### Completion Checklist

-   [x] `MultiHitEffect` implemented ✅
-   [x] `MultiTurnEffect` implemented ✅
-   [x] `MoveState` tracking implemented ✅ (via MoveStateTracker ValueObject)
-   [x] Multi-hit damage working (2-5 hits) ✅
-   [x] Multi-hit crits working (each hit independent) ✅
-   [x] Charging moves working (Solar Beam, Skull Bash) ✅ (via ChargingEffect)
-   [x] Recharging moves working (Hyper Beam, Giga Impact) ✅ (via MultiTurnEffect)
-   [x] Continuous moves working (Outrage, Petal Dance) ✅ (via MultiTurnEffect)
-   [x] Move state persistence working ✅
-   [x] Functional tests passing (6+ tests) ✅
-   [x] Edge case tests passing (included in functional tests) ✅
-   [x] Integration tests passing (AdvancedMoveMechanicsIntegrationTests) ✅
-   [x] All existing tests still passing ✅
-   [x] No compiler warnings ✅

**Tests Implemented**: 6+ tests (MultiHit: 3, MultiTurn: 3, Integration: multiple)

**Status**: ✅ **Core Multi-Hit/Turn Moves Complete** - Basic multi-hit and multi-turn mechanics implemented. Advanced features (Substitute interaction, move-specific behaviors) pending.

---

## Updated Dependencies Diagram

```
                    Core Systems (Complete)
                           │
                           ▼
              ┌────────────────────────┐
              │  Phase 2.1: Foundation │
              │  BattleField, Slot,    │
              │  Side, Rules, Action   │
              └───────────┬────────────┘
                          │
           ┌──────────────┼──────────────┐
           │              │              │
           ▼              ▼              ▼
    ┌─────────────┐ ┌───────────┐ ┌─────────────┐
    │ Phase 2.2:  │ │Phase 2.3: │ │ Phase 2.4:  │
    │ ActionQueue │ │TurnOrder  │ │ DamagePipe  │
    └──────┬──────┘ └─────┬─────┘ └──────┬──────┘
           │              │              │
           └──────────────┼──────────────┘
                          │
                          ▼
              ┌────────────────────────┐
              │  Phase 2.5: Actions    │
              │  UseMoveAction,        │
              │  DamageAction, etc.    │
              └───────────┬────────────┘
                          │
                          ▼
              ┌────────────────────────┐
              │  Phase 2.6: Engine     │
              │  CombatEngine,         │
              │  BattleArbiter         │
              └───────────┬────────────┘
                          │
                          ▼
              ┌────────────────────────┐
              │  Phase 2.7: Integration│
              │  AI, Full Battles      │
              └───────────┬────────────┘
                          │
           ┌──────────────┼──────────────┐
           │              │              │
           ▼              ▼              ▼
    ┌─────────────┐ ┌───────────┐ ┌─────────────┐
    │ Phase 2.8:  │ │Phase 2.9: │ │ Phase 2.10: │
    │ EndOfTurn  │ │Abilities  │ │ Pipeline    │
    │ Effects    │ │& Items    │ │ Hooks       │
    └──────┬──────┘ └─────┬─────┘ └──────┬──────┘
           │              │              │
           └──────────────┼──────────────┘
                          │
           ┌──────────────┼──────────────┐
           │              │              │
           ▼              ▼              ▼
    ┌─────────────┐ ┌───────────┐ ┌─────────────┐
    │ Phase 2.12: │ │Phase 2.13:│ │ Phase 2.14: │
    │ Extended    │ │Additional │ │ Volatile    │
    │ EndOfTurn   │ │Triggers   │ │ Status      │
    └──────┬──────┘ └─────┬─────┘ └──────┬──────┘
           │              │              │
           └──────────────┼──────────────┘
                          │
           ┌──────────────┼──────────────┐
           │              │              │
           ▼              ▼              ▼
    ┌─────────────┐ ┌───────────┐ ┌─────────────┐
    │ Phase 2.15: │ │Phase 2.16:│ │ Phase 2.17: │
    │ Weather     │ │Terrain    │ │ Entry       │
    │ System      │ │System     │ │ Hazards     │
    └──────┬──────┘ └─────┬─────┘ └──────┬──────┘
           │              │              │
           └──────────────┼──────────────┘
                          │
           ┌──────────────┼──────────────┐
           │              │              │
           ▼              ▼              ▼
    ┌─────────────┐ ┌───────────┐ ┌─────────────┐
    │ Phase 2.18: │ │Phase 2.19:│ │   Future    │
    │ Special     │ │Multi-Hit  │ │   Phases    │
    │ Moves       │ │& Multi-   │ │             │
    │             │ │Turn       │ │             │
    └─────────────┘ └───────────┘ └─────────────┘
```

---

## Updated Test Estimates

| Phase                     | Estimated Tests | Priority |
| ------------------------- | --------------- | -------- |
| 2.12 Extended End-of-Turn | ~30             | High     |
| 2.13 Additional Triggers  | ~30             | High     |
| 2.14 Volatile Status      | ~30             | High     |
| 2.15 Weather System       | ~35             | Medium   |
| 2.16 Terrain System       | ~35             | Medium   |
| 2.17 Entry Hazards        | ~35             | Medium   |
| 2.18 Special Moves        | ~50             | Low      |
| 2.19 Multi-Hit/Turn       | ~35             | Low      |
| **Total Future**          | **~280**        |          |

---

## Dependencies Diagram

```
                    Core Systems (Complete)
                           │
                           ▼
              ┌────────────────────────┐
              │  Phase 2.1: Foundation │
              │  BattleField, Slot,    │
              │  Side, Rules, Action   │
              └───────────┬────────────┘
                          │
           ┌──────────────┼──────────────┐
           │              │              │
           ▼              ▼              ▼
    ┌─────────────┐ ┌───────────┐ ┌─────────────┐
    │ Phase 2.2:  │ │Phase 2.3: │ │ Phase 2.4:  │
    │ ActionQueue │ │TurnOrder  │ │ DamagePipe  │
    └──────┬──────┘ └─────┬─────┘ └──────┬──────┘
           │              │              │
           └──────────────┼──────────────┘
                          │
                          ▼
              ┌────────────────────────┐
              │  Phase 2.5: Actions    │
              │  UseMoveAction,        │
              │  DamageAction, etc.    │
              └───────────┬────────────┘
                          │
                          ▼
              ┌────────────────────────┐
              │  Phase 2.6: Engine     │
              │  CombatEngine,         │
              │  BattleArbiter         │
              └───────────┬────────────┘
                          │
                          ▼
              ┌────────────────────────┐
              │  Phase 2.7: Integration│
              │  AI, Full Battles      │
              └────────────────────────┘
```

---

## Quick Reference

### File Structure

```
PokemonUltimate.Combat/
├── Field/                    # Battlefield components
│   ├── BattleSlot.cs
│   ├── BattleSide.cs
│   ├── BattleField.cs
│   └── BattleRules.cs
├── Engine/                    # Battle engine and queue
│   ├── CombatEngine.cs
│   ├── BattleArbiter.cs
│   └── BattleQueue.cs
├── Results/                   # Battle outcomes
│   ├── BattleOutcome.cs
│   └── BattleResult.cs
├── Providers/                 # Action providers
│   └── IActionProvider.cs
├── View/                      # Visual interface
│   ├── IBattleView.cs
│   └── NullBattleView.cs
├── Actions/                   # Battle actions
│   ├── BattleAction.cs
│   ├── MessageAction.cs
│   ├── DamageAction.cs
│   ├── UseMoveAction.cs
│   ├── FaintAction.cs
│   ├── ApplyStatusAction.cs
│   ├── StatChangeAction.cs
│   ├── HealAction.cs
│   └── SwitchAction.cs
├── Damage/                    # Damage calculation
│   ├── DamageContext.cs
│   ├── DamagePipeline.cs
│   ├── IDamageStep.cs
│   └── Steps/
│       ├── BaseDamageStep.cs
│       ├── CriticalHitStep.cs
│       ├── RandomFactorStep.cs
│       ├── StabStep.cs
│       ├── TypeEffectivenessStep.cs
│       └── BurnStep.cs
├── Helpers/                   # Utility helpers
│   ├── AccuracyChecker.cs
│   └── TurnOrderResolver.cs
└── AI/                        # AI implementations (Phase 2.7)
    ├── RandomAI.cs
    └── AlwaysAttackAI.cs
```

### Estimated Tests per Phase

| Phase           | Estimated | Actual  |
| --------------- | --------- | ------- |
| 2.1 Foundation  | ~30       | 133     |
| 2.2 Queue       | ~15       | 77      |
| 2.3 Turn Order  | ~20       | 48      |
| 2.4 Damage      | ~40       | 65      |
| 2.5 Actions     | ~50       | 47      |
| 2.6 Engine      | ~25       | 30      |
| 2.7 Integration | ~20       | 38      |
| **Total**       | **~200**  | **400** |

---

## Related Documents

-   **[Architecture](architecture.md)** - Technical design of combat system
-   **[Use Cases](use_cases.md)** - All battle scenarios
-   **[Testing](testing.md)** - Testing strategy
-   **[Code Location](code_location.md)** - Where code is implemented
-   **[Sub-Feature 2.4: Damage Pipeline](2.4-damage-calculation-pipeline/architecture.md)** - Damage formula specification
-   **[Sub-Feature 2.3: Turn Order](2.3-turn-order-resolution/architecture.md)** - Speed/priority rules
-   **[Sub-Feature 2.8: End-of-Turn Effects](2.8-end-of-turn-effects/architecture.md)** - Status effects specification
-   **[Feature 1: Game Data](../1-game-data/architecture.md)** - Pokemon instances used in battles
-   **[Feature 3: Content Expansion](../3-content-expansion/roadmap.md)** - Moves, abilities, items
-   **[TDD Guide](../../../ai_workflow/docs/TDD_GUIDE.md)** - Test-Driven Development guide

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Tools & Demos

| Tool                         | Purpose                                                                                                                                                                        |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `PokemonUltimate.BattleDemo` | Visual AI vs AI battle simulator with debug information. Demonstrates combat system with turn-by-turn display, damage calculations, stat changes, and action queue processing. |

---

## Version History

| Date     | Phase | Notes                                            |
| -------- | ----- | ------------------------------------------------ |
| Dec 2025 | 2.1   | Battle Foundation - 133 tests (incl. edge cases) |
| TBD      | 2.2   | Action Queue                                     |
| TBD      | 2.3   | Turn Order                                       |
| TBD      | 2.4   | Damage Calculation                               |
| TBD      | 2.5   | Combat Actions                                   |
| TBD      | 2.6   | Combat Engine                                    |
| TBD      | 2.7   | Integration                                      |
