# Combat System Implementation Plan

> Step-by-step guide for implementing the Pokemon battle engine.

## Overview

The Combat System is divided into **7 phases**, each building on the previous. Every phase must pass all tests before moving to the next.

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
│  Phase 2.7: Integration          ══════════════════════     │
└─────────────────────────────────────────────────────────────┘
```

---

## Current Status

| Phase | Status | Tests | Notes |
|-------|--------|-------|-------|
| 2.1 Battle Foundation | ✅ Complete | 133 | BattleSlot, BattleSide, BattleField |
| 2.2 Action Queue | ✅ Complete | 77 | BattleAction, BattleQueue |
| 2.3 Turn Order | ✅ Complete | 48 | TurnOrderResolver |
| 2.4 Damage Calculation | ✅ Complete | 65 | DamagePipeline |
| **Data Layer** | ✅ Complete | 170 | AbilityData, ItemData, StatusEffectData |
| 2.5 Combat Actions | ✅ Complete | 47 | All actions implemented |
| 2.6 Combat Engine | ✅ Complete | 30 | CombatEngine, BattleArbiter, IActionProvider |
| 2.7 Integration | ✅ Complete | 38 | RandomAI, AlwaysAttackAI, TargetResolver, Full battles |
| **Total** | **7/7** | **608** | Combat module only |

---

## Phase 2.1: Battle Foundation

**Goal**: Create the battlefield structure where Pokemon fight.

**Depends on**: Core (PokemonInstance, MoveInstance)

### Components

| Component | File | Description |
|-----------|------|-------------|
| `BattleSlot` | `Combat/BattleSlot.cs` | Container for active Pokemon + battle state |
| `BattleSide` | `Combat/BattleSide.cs` | One side of the field (player/enemy) |
| `BattleField` | `Combat/BattleField.cs` | Complete battlefield (2 sides) |
| `BattleRules` | `Combat/BattleRules.cs` | Configuration (1v1, 2v2, etc.) |
| `IBattleView` | `Combat/IBattleView.cs` | Interface for visual presentation |
| `BattleAction` | `Combat/Actions/BattleAction.cs` | Abstract base for all actions |

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

- [x] `BattleSlot` implemented with tests (27 tests)
- [x] `BattleSide` implemented with tests (26 tests)
- [x] `BattleField` implemented with tests (22 tests)
- [x] `BattleRules` implemented
- [x] `IBattleView` interface defined
- [x] `NullBattleView` for testing
- [x] `BattleAction` abstract class defined
- [x] `MessageAction` implemented with tests (6 tests)
- [x] **Edge case tests** (52 tests)
- [x] All tests pass (133 tests)
- [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**
- `BattleSlot` with SlotIndex, Pokemon, IsEmpty, HasFainted
- `BattleSide` with Slots, Party, IsPlayer, GetActiveSlots, GetAvailableSwitches
- `BattleField` with PlayerSide, EnemySide, GetAllActiveSlots, GetOppositeSide
- `BattleRules` with PlayerSlots, EnemySlots (simplified naming from spec)
- `BattleAction` with ExecuteLogic, ExecuteVisual, User, Priority

**API Changes from Spec:**
- `PlayerSideSlots` → `PlayerSlots` (simpler)
- `EnemySideSlots` → `EnemySlots` (simpler)
- `Index` → `SlotIndex` (clearer)
- `IsOccupied` → `IsEmpty` (inverted, clearer)
- Added `HasFainted` helper property

**Deferred to Later Phases:**
- `BattleSlot.Effects` (SlotEffects) → Phase 2.5+
- `BattleSlot.ActionProvider` → Phase 2.3 (Turn Order)
- `BattleSide.SideStatus` (Reflect, LightScreen) → Future
- `BattleSide.SpikesCount` → Future (Hazards)
- `BattleField.Weather` → Future (Weather system)
- `BattleField.Terrain` → Future (Terrain system)

**Added (not in spec):**
- Stat stages management in `BattleSlot` (needed for damage calc)
- Volatile status in `BattleSlot` (needed for turn order checks)

---

## Phase 2.2: Action Queue

**Goal**: Process battle actions in sequence with logic/visual separation.

**Depends on**: Phase 2.1

### Components

| Component | File | Description |
|-----------|------|-------------|
| `BattleQueue` | `Combat/BattleQueue.cs` | Action processor |
| `MessageAction` | `Combat/Actions/MessageAction.cs` | Simple message (for testing) |
| `NullBattleView` | `Combat/NullBattleView.cs` | No-op view for tests |

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

- [x] `BattleQueue` implemented with tests (19 tests)
- [x] `BattleAction` abstract class complete
- [x] `MessageAction` implemented with tests (6 tests - Phase 2.1)
- [x] `NullBattleView` for testing (Phase 2.1)
- [x] Safety limit for infinite loops (1000 max iterations)
- [x] **Edge case tests** (19 tests)
- [x] All tests pass (38 BattleQueue tests)
- [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**
- `BattleQueue` with Enqueue, EnqueueRange, InsertAtFront, Clear, ProcessQueue
- Reactions inserted at front (execute immediately after triggering action)
- Safety counter prevents infinite loops (1000 iterations max)
- Logic → Visual execution order

**API Additions:**
- `Count` property for queue size
- `IsEmpty` property for empty check
- Uses `LinkedList<T>` for O(1) InsertAtFront

---

## Phase 2.3: Turn Order

**Goal**: Sort actions by priority and speed.

**Depends on**: Phase 2.2

### Components

| Component | File | Description |
|-----------|------|-------------|
| `TurnOrderResolver` | `Combat/TurnOrderResolver.cs` | Sorts actions |
| `IActionProvider` | `Combat/IActionProvider.cs` | Interface for action selection |

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

| Action Type | Priority |
|-------------|----------|
| Switch | +6 (always first) |
| Quick Attack | +1 |
| Normal moves | 0 |
| Vital Throw | -1 |
| Trick Room active | Reverse speed |

### Speed Modifiers

| Modifier | Effect |
|----------|--------|
| Stat stages | ±6 stages formula |
| Paralysis | ×0.5 |
| Choice Scarf | ×1.5 (future) |
| Tailwind | ×2.0 (future) |

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

- [x] `TurnOrderResolver` implemented (21 functional tests)
- [x] Priority extraction working
- [x] Speed calculation with modifiers
- [x] Paralysis penalty applied (×0.5)
- [x] Stat stage modifiers applied (±6 stages)
- [x] Random tiebreaker for equal speeds
- [x] **Edge case tests** (27 tests)
- [x] All tests pass (48 TurnOrder tests)
- [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**
- `TurnOrderResolver.SortActions()` - sorts by priority DESC, then speed DESC
- `GetPriority()` - returns action.Priority
- `GetEffectiveSpeed()` - base speed × stage modifier × status modifier
- Stat stage formula: +stages = (2+n)/2, -stages = 2/(2+n)
- Paralysis: ×0.5 speed
- Random tiebreaker for equal speeds

**Deferred to Later Phases:**
- Switch priority (+6) - SwitchAction not yet implemented
- Choice Scarf (×1.5) - Item system
- Tailwind (×2.0) - Field effects
- Trick Room (reverse order) - Field effects
- Ability modifiers (Swift Swim, etc.)

---

## Phase 2.4: Damage Calculation

**Goal**: Calculate exact damage using the official formula.

**Depends on**: Phase 2.1 (can be done in parallel with 2.2-2.3)

### Components

| Component | File | Description |
|-----------|------|-------------|
| `DamageContext` | `Combat/Damage/DamageContext.cs` | Calculation state |
| `IDamageStep` | `Combat/Damage/IDamageStep.cs` | Pipeline step interface |
| `DamagePipeline` | `Combat/Damage/DamagePipeline.cs` | Executes steps |

### Pipeline Steps

| Step | Order | Description |
|------|-------|-------------|
| `BaseDamageStep` | 1 | Base formula: `((2*Level/5+2) * Power * A/D) / 50 + 2` |
| `CriticalHitStep` | 2 | 1.5x multiplier, ignore negative stages |
| `RandomFactorStep` | 3 | 0.85 to 1.00 random roll |
| `StabStep` | 4 | 1.5x if move type matches user type |
| `TypeEffectivenessStep` | 5 | 0x, 0.25x, 0.5x, 1x, 2x, 4x |
| `BurnStep` | 6 | 0.5x for physical moves if burned |
| `ScreenStep` | 7 | 0.5x if Reflect/Light Screen active (future) |

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

- [x] `DamageContext` implemented (immutable inputs, mutable state)
- [x] `IDamageStep` interface defined
- [x] `DamagePipeline` implemented (25 functional tests)
- [x] `BaseDamageStep` with Gen 3+ formula
- [x] `CriticalHitStep` implemented (1.5x, 1/24 base rate)
- [x] `RandomFactorStep` implemented (0.85-1.0)
- [x] `StabStep` implemented (1.5x)
- [x] `TypeEffectivenessStep` (uses existing TypeEffectiveness)
- [x] `BurnStep` implemented (0.5x for physical)
- [x] **Edge case tests** (25 tests)
- [x] **Real-world verification tests** (15 tests)
- [x] All tests pass (65 DamagePipeline tests total)
- [x] No compiler warnings

### Spec Compliance Notes

**Implemented (matches spec):**
- Pipeline pattern with 6 steps
- Gen 3+ base damage formula
- All damage modifiers in correct order
- Type effectiveness including immunity
- STAB for primary and secondary types
- Critical hit multiplier and base rate
- Burn penalty for physical moves only
- Status moves deal 0 damage
- Minimum 1 damage (unless immune or status)
- Fixed random support for deterministic testing

**Deferred to Later Phases:**
- Multi-target penalty (0.75x)
- Weather modifiers
- Screen modifiers (Reflect/Light Screen)
- Ability modifiers
- Item modifiers (Life Orb, etc.)
- Critical hit stage increases (Focus Energy)

---

## Phase 2.5: Combat Actions

**Goal**: Create actions for attacking, fainting, status effects.

**Depends on**: Phases 2.1-2.4

### Components

| Component | File | Description |
|-----------|------|-------------|
| `DamageAction` | `Combat/Actions/DamageAction.cs` | Apply damage to target |
| `UseMoveAction` | `Combat/Actions/UseMoveAction.cs` | Full move execution |
| `FaintAction` | `Combat/Actions/FaintAction.cs` | Handle KO |
| `ApplyStatusAction` | `Combat/Actions/ApplyStatusAction.cs` | Apply status condition |
| `StatChangeAction` | `Combat/Actions/StatChangeAction.cs` | Modify stat stages |
| `HealAction` | `Combat/Actions/HealAction.cs` | Restore HP |
| `SwitchAction` | `Combat/Actions/SwitchAction.cs` | Switch Pokemon |

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

- [x] `DamageAction` implemented with tests
- [x] `UseMoveAction` implemented with tests
- [x] `FaintAction` implemented with tests
- [x] `ApplyStatusAction` implemented with tests
- [x] `StatChangeAction` implemented with tests
- [x] `HealAction` implemented with tests
- [x] `SwitchAction` implemented with tests
- [x] `MessageAction` implemented with tests
- [x] PP deduction working
- [x] Accuracy checks working (`AccuracyChecker`)
- [x] Effect application working
- [x] All tests pass (1,885 total)

### Documentation Created

- [x] `docs/combat/action_use_cases.md` - 207 use cases documented
- [x] `docs/combat/actions_bible.md` - Complete technical reference

---

## Phase 2.6: Combat Engine

**Goal**: Orchestrate the full battle loop.

**Depends on**: Phases 2.1-2.5

### Components

| Component | File | Description |
|-----------|------|-------------|
| `CombatEngine` | `Combat/CombatEngine.cs` | Main controller |
| `BattleArbiter` | `Combat/BattleArbiter.cs` | Victory/defeat detection |
| `BattleOutcome` | `Combat/BattleOutcome.cs` | Battle result enum |
| `BattleResult` | `Combat/BattleResult.cs` | Detailed result data |

### CombatEngine Specification

```csharp
public class CombatEngine
{
    public BattleField Field { get; }
    public BattleQueue Queue { get; }
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

- [x] `CombatEngine` implemented with tests (9 functional + 12 edge cases)
- [x] `BattleArbiter` implemented with tests (6 functional + 8 edge cases)
- [x] `BattleOutcome` enum defined
- [x] `BattleResult` class defined
- [x] `IActionProvider` interface defined
- [x] `TestActionProvider` helper for tests
- [x] `RunTurn()` working
- [x] `RunBattle()` loop working
- [x] Victory detection working
- [x] Defeat detection working
- [x] All tests pass (30 tests total)

---

## Phase 2.7: Integration

**Goal**: Full battle simulation with AI opponents.

**Depends on**: Phase 2.6

### Components

| Component | File | Description |
|-----------|------|-------------|
| `RandomAI` | `Combat/AI/RandomAI.cs` | Random move selection |
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

- [x] `RandomAI` implemented with tests (12 tests)
- [x] `AlwaysAttackAI` implemented with tests (9 tests)
- [x] `TargetResolver` helper implemented
- [x] Full 1v1 battle simulation working
- [x] Multiple battles run without errors
- [x] Type advantages affect outcome
- [x] Status effects work in battle
- [x] PP depletion works
- [x] Integration tests pass (17 tests)
- [x] Smoke test updated with AI tests
- [x] All tests pass (38 new tests for Phase 2.7)

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

| Phase | Estimated | Actual |
|-------|-----------|--------|
| 2.1 Foundation | ~30 | 133 |
| 2.2 Queue | ~15 | 77 |
| 2.3 Turn Order | ~20 | 48 |
| 2.4 Damage | ~40 | 65 |
| 2.5 Actions | ~50 | 47 |
| 2.6 Engine | ~25 | 30 |
| 2.7 Integration | ~20 | 38 |
| **Total** | **~200** | **400** |

---

## Related Documents

| Document | Purpose |
|----------|---------|
| `architecture/combat_system_spec.md` | Technical design |
| `architecture/damage_and_effect_system.md` | Damage formula |
| `architecture/turn_order_system.md` | Speed/priority rules |
| `architecture/status_and_stat_system.md` | Status effects |
| `checklists/pre_combat.md` | Pre-implementation checklist |

## Tools & Demos

| Tool | Purpose |
|------|---------|
| `PokemonUltimate.BattleDemo` | Visual AI vs AI battle simulator with debug information. Demonstrates combat system with turn-by-turn display, damage calculations, stat changes, and action queue processing. |

---

## Version History

| Date | Phase | Notes |
|------|-------|-------|
| Dec 2025 | 2.1 | Battle Foundation - 133 tests (incl. edge cases) |
| TBD | 2.2 | Action Queue |
| TBD | 2.3 | Turn Order |
| TBD | 2.4 | Damage Calculation |
| TBD | 2.5 | Combat Actions |
| TBD | 2.6 | Combat Engine |
| TBD | 2.7 | Integration |

