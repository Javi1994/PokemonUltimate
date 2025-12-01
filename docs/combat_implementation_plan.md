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
| 2.1 Battle Foundation | ⏳ Pending | 0 | Start here |
| 2.2 Action Queue | ⏳ Pending | 0 | |
| 2.3 Turn Order | ⏳ Pending | 0 | |
| 2.4 Damage Calculation | ⏳ Pending | 0 | |
| 2.5 Combat Actions | ⏳ Pending | 0 | |
| 2.6 Combat Engine | ⏳ Pending | 0 | |
| 2.7 Integration | ⏳ Pending | 0 | |
| **Total** | **0/7** | **0** | |

---

## Phase 2.1: Battle Foundation

**Goal**: Create the battlefield structure where Pokemon fight.

**Depends on**: Core (PokemonInstance, MoveInstance)

### Components

| Component | File | Description |
|-----------|------|-------------|
| `BattleSlot` | `Core/Combat/BattleSlot.cs` | Container for active Pokemon + battle state |
| `BattleSide` | `Core/Combat/BattleSide.cs` | One side of the field (player/enemy) |
| `BattleField` | `Core/Combat/BattleField.cs` | Complete battlefield (2 sides) |
| `BattleRules` | `Core/Combat/BattleRules.cs` | Configuration (1v1, 2v2, etc.) |
| `IBattleView` | `Core/Combat/IBattleView.cs` | Interface for visual presentation |
| `BattleAction` | `Core/Combat/Actions/BattleAction.cs` | Abstract base for all actions |

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

- [ ] `BattleSlot` implemented with tests
- [ ] `BattleSide` implemented with tests
- [ ] `BattleField` implemented with tests
- [ ] `BattleRules` implemented
- [ ] `IBattleView` interface defined
- [ ] `BattleAction` abstract class defined
- [ ] All tests pass
- [ ] No compiler warnings

---

## Phase 2.2: Action Queue

**Goal**: Process battle actions in sequence with logic/visual separation.

**Depends on**: Phase 2.1

### Components

| Component | File | Description |
|-----------|------|-------------|
| `BattleQueue` | `Core/Combat/BattleQueue.cs` | Action processor |
| `MessageAction` | `Core/Combat/Actions/MessageAction.cs` | Simple message (for testing) |
| `NullBattleView` | `Core/Combat/NullBattleView.cs` | No-op view for tests |

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

- [ ] `BattleQueue` implemented with tests
- [ ] `BattleAction` abstract class complete
- [ ] `MessageAction` implemented with tests
- [ ] `NullBattleView` for testing
- [ ] Safety limit for infinite loops
- [ ] All tests pass

---

## Phase 2.3: Turn Order

**Goal**: Sort actions by priority and speed.

**Depends on**: Phase 2.2

### Components

| Component | File | Description |
|-----------|------|-------------|
| `TurnOrderResolver` | `Core/Combat/TurnOrderResolver.cs` | Sorts actions |
| `IActionProvider` | `Core/Combat/IActionProvider.cs` | Interface for action selection |

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

- [ ] `TurnOrderResolver` implemented
- [ ] Priority extraction working
- [ ] Speed calculation with modifiers
- [ ] Paralysis penalty applied
- [ ] Stat stage modifiers applied
- [ ] Random tiebreaker for equal speeds
- [ ] All tests pass

---

## Phase 2.4: Damage Calculation

**Goal**: Calculate exact damage using the official formula.

**Depends on**: Phase 2.1 (can be done in parallel with 2.2-2.3)

### Components

| Component | File | Description |
|-----------|------|-------------|
| `DamageContext` | `Core/Combat/Damage/DamageContext.cs` | Calculation state |
| `IDamageStep` | `Core/Combat/Damage/IDamageStep.cs` | Pipeline step interface |
| `DamagePipeline` | `Core/Combat/Damage/DamagePipeline.cs` | Executes steps |

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

- [ ] `DamageContext` implemented
- [ ] `IDamageStep` interface defined
- [ ] `DamagePipeline` implemented
- [ ] `BaseDamageStep` with official formula
- [ ] `CriticalHitStep` implemented
- [ ] `RandomFactorStep` implemented
- [ ] `StabStep` implemented
- [ ] `TypeEffectivenessStep` (uses existing TypeEffectiveness)
- [ ] `BurnStep` implemented
- [ ] Integration tests with known values
- [ ] All tests pass

---

## Phase 2.5: Combat Actions

**Goal**: Create actions for attacking, fainting, status effects.

**Depends on**: Phases 2.1-2.4

### Components

| Component | File | Description |
|-----------|------|-------------|
| `DamageAction` | `Core/Combat/Actions/DamageAction.cs` | Apply damage to target |
| `UseMoveAction` | `Core/Combat/Actions/UseMoveAction.cs` | Full move execution |
| `FaintAction` | `Core/Combat/Actions/FaintAction.cs` | Handle KO |
| `ApplyStatusAction` | `Core/Combat/Actions/ApplyStatusAction.cs` | Apply status condition |
| `StatChangeAction` | `Core/Combat/Actions/StatChangeAction.cs` | Modify stat stages |
| `HealAction` | `Core/Combat/Actions/HealAction.cs` | Restore HP |
| `SwitchAction` | `Core/Combat/Actions/SwitchAction.cs` | Switch Pokemon |

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

- [ ] `DamageAction` implemented with tests
- [ ] `UseMoveAction` implemented with tests
- [ ] `FaintAction` implemented with tests
- [ ] `ApplyStatusAction` implemented with tests
- [ ] `StatChangeAction` implemented with tests
- [ ] `HealAction` implemented with tests
- [ ] `SwitchAction` implemented with tests
- [ ] PP deduction working
- [ ] Accuracy checks working
- [ ] Effect application working
- [ ] All tests pass

---

## Phase 2.6: Combat Engine

**Goal**: Orchestrate the full battle loop.

**Depends on**: Phases 2.1-2.5

### Components

| Component | File | Description |
|-----------|------|-------------|
| `CombatEngine` | `Core/Combat/CombatEngine.cs` | Main controller |
| `BattleArbiter` | `Core/Combat/BattleArbiter.cs` | Victory/defeat detection |
| `BattleOutcome` | `Core/Combat/BattleOutcome.cs` | Battle result enum |
| `BattleResult` | `Core/Combat/BattleResult.cs` | Detailed result data |

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

- [ ] `CombatEngine` implemented with tests
- [ ] `BattleArbiter` implemented with tests
- [ ] `BattleOutcome` enum defined
- [ ] `BattleResult` class defined
- [ ] `RunTurn()` working
- [ ] `RunBattle()` loop working
- [ ] Victory detection working
- [ ] Defeat detection working
- [ ] All tests pass

---

## Phase 2.7: Integration

**Goal**: Full battle simulation with AI opponents.

**Depends on**: Phase 2.6

### Components

| Component | File | Description |
|-----------|------|-------------|
| `RandomAI` | `Core/Combat/AI/RandomAI.cs` | Random move selection |
| `AlwaysAttackAI` | `Core/Combat/AI/AlwaysAttackAI.cs` | Always uses first move |

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

- [ ] `RandomAI` implemented with tests
- [ ] `AlwaysAttackAI` implemented with tests
- [ ] Full 1v1 battle simulation working
- [ ] Multiple battles run without errors
- [ ] Type advantages affect outcome
- [ ] Status effects work in battle
- [ ] PP depletion works
- [ ] Faint and switch flow works
- [ ] All integration tests pass
- [ ] Unity example documented
- [ ] All tests pass

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
PokemonUltimate.Core/
└── Combat/
    ├── BattleSlot.cs
    ├── BattleSide.cs
    ├── BattleField.cs
    ├── BattleRules.cs
    ├── BattleQueue.cs
    ├── CombatEngine.cs
    ├── BattleArbiter.cs
    ├── TurnOrderResolver.cs
    ├── IBattleView.cs
    ├── IActionProvider.cs
    ├── Actions/
    │   ├── BattleAction.cs
    │   ├── MessageAction.cs
    │   ├── DamageAction.cs
    │   ├── UseMoveAction.cs
    │   ├── FaintAction.cs
    │   ├── ApplyStatusAction.cs
    │   ├── StatChangeAction.cs
    │   ├── HealAction.cs
    │   └── SwitchAction.cs
    ├── Damage/
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
    └── AI/
        ├── RandomAI.cs
        └── AlwaysAttackAI.cs
```

### Estimated Tests per Phase

| Phase | Tests |
|-------|-------|
| 2.1 Foundation | ~30 |
| 2.2 Queue | ~15 |
| 2.3 Turn Order | ~20 |
| 2.4 Damage | ~40 |
| 2.5 Actions | ~50 |
| 2.6 Engine | ~25 |
| 2.7 Integration | ~20 |
| **Total** | **~200** |

---

## Related Documents

| Document | Purpose |
|----------|---------|
| `architecture/combat_system_spec.md` | Technical design |
| `architecture/damage_and_effect_system.md` | Damage formula |
| `architecture/turn_order_system.md` | Speed/priority rules |
| `architecture/status_and_stat_system.md` | Status effects |
| `checklists/pre_combat.md` | Pre-implementation checklist |

---

## Version History

| Date | Phase | Notes |
|------|-------|-------|
| TBD | 2.1 | Battle Foundation |
| TBD | 2.2 | Action Queue |
| TBD | 2.3 | Turn Order |
| TBD | 2.4 | Damage Calculation |
| TBD | 2.5 | Combat Actions |
| TBD | 2.6 | Combat Engine |
| TBD | 2.7 | Integration |

