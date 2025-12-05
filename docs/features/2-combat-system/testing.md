# Feature 2: Combat System - Testing

> How to test the combat system.

**Feature Number**: 2  
**Feature Name**: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The combat system uses a comprehensive three-phase testing approach:

-   **Functional Tests** - Normal behavior and expected outcomes
-   **Edge Case Tests** - Boundary conditions, invalid inputs, special cases
-   **Integration Tests** - System interactions and cascading effects

**Coverage**: 2,528+ passing tests covering all combat phases (2.1-2.14, 2.16)

> **📋 Post-Refactoring Note (2024-12-05)**: After the comprehensive refactoring, all components now use Dependency Injection, making them highly testable. Mock implementations can be easily injected for testing (e.g., `NullBattleLogger`, `NullBattleView`). The refactoring improved testability significantly by removing static dependencies and direct object creation. See `PokemonUltimate.Combat/ANALISIS_COMPLETO_Y_PLAN_IMPLEMENTACION.md` for refactoring details.

## Test Structure

Following TDD workflow from `ai_workflow/docs/TDD_GUIDE.md`:

```
PokemonUltimate.Tests/
└── Systems/
    └── Combat/                          # Combat system tests
        ├── [Component]Tests.cs          # Functional tests
        ├── [Component]EdgeCasesTests.cs # Edge case tests
        └── Integration/                 # Integration tests
            ├── Actions/                 # Action integration
            ├── Damage/                  # Damage integration
            ├── Engine/                  # Engine integration
            └── System/                  # Full system integration
```

## Test Types

### Functional Tests

**Location**: `Tests/Systems/Combat/[Component]Tests.cs`
**Purpose**: Test normal behavior and expected outcomes
**Naming**: `MethodName_Scenario_ExpectedResult`

**Examples**:

-   `CombatEngine_Initialize_SetsUpFieldCorrectly`
-   `BattleQueue_Enqueue_AddsActionToQueue`
-   `DamagePipeline_CalculateDamage_ReturnsCorrectDamage`
-   `UseMoveAction_ExecuteLogic_AppliesDamage`

### Edge Case Tests

**Location**: `Tests/Systems/Combat/[Component]EdgeCasesTests.cs`
**Purpose**: Test boundary conditions, invalid inputs, special cases
**Naming**: `MethodName_EdgeCase_ExpectedResult`

**Examples**:

-   `BattleQueue_Enqueue_NullAction_ThrowsException`
-   `DamagePipeline_CalculateDamage_ZeroPowerMove_ReturnsZero`
-   `CombatEngine_RunTurn_EmptyQueue_CompletesWithoutError`
-   `UseMoveAction_ExecuteLogic_Miss_NoDamageApplied`

### Integration Tests

**Location**: `Tests/Systems/Combat/Integration/[Category]/`
**Purpose**: Test system interactions and cascading effects
**Naming**: `[System1]_[System2]_[ExpectedBehavior]`

**Categories**:

-   **Actions/** - Action → Queue → Processing interactions
-   **Damage/** - Damage → Status → Effects interactions
-   **Engine/** - Engine → Turn Order → Queue interactions
-   **System/** - Full battle end-to-end scenarios

**Examples**:

-   `Actions_BattleQueue_DamageAction_TriggersFaintAction`
-   `Damage_StatusEffects_Burn_AppliesDamageAtEndOfTurn`
-   `Engine_TurnOrderResolver_Priority_SortsActionsCorrectly`
-   `System_CombatEngine_FullBattle_EndsWithVictory`

## Coverage by Component

### Battle Foundation (Phase 2.1)

-   ✅ `BattleFieldTests.cs` - Field initialization, slot management
-   ✅ `BattleSideTests.cs` - Side operations
-   ✅ `BattleSlotTests.cs` - Slot operations

### Action Queue (Phase 2.2)

-   ✅ `BattleQueueTests.cs` - Queue operations, processing
-   ✅ `BattleActionTests.cs` - Base action behavior

### Turn Order (Phase 2.3)

-   ✅ `TurnOrderResolverTests.cs` - Priority, speed, random sorting

### Damage Calculation (Phase 2.4)

-   ✅ `DamagePipelineTests.cs` - Damage calculation
-   ✅ `DamageContextTests.cs` - Context creation
-   ✅ `DamageStepsTests.cs` - Individual pipeline steps

### Combat Actions (Phase 2.5)

-   ✅ `UseMoveActionTests.cs` - Move execution
-   ✅ `DamageActionTests.cs` - Damage application
-   ✅ `ApplyStatusActionTests.cs` - Status effects
-   ✅ `HealActionTests.cs` - HP restoration
-   ✅ `StatChangeActionTests.cs` - Stat modifications
-   ✅ `SwitchActionTests.cs` - Pokemon switching
-   ✅ `FaintActionTests.cs` - Fainting

### Combat Engine (Phase 2.6)

-   ✅ `CombatEngineTests.cs` - Battle loop, turn execution
-   ✅ `BattleArbiterTests.cs` - Victory/defeat detection

### Integration (Phase 2.7)

-   ✅ `Integration/Actions/` - Action interactions
-   ✅ `Integration/Damage/` - Damage interactions
-   ✅ `Integration/Engine/` - Engine interactions
-   ✅ `Integration/System/` - Full battle scenarios

### End-of-Turn Effects (Phase 2.8)

-   ✅ `EndOfTurnProcessorTests.cs` - End-of-turn processing
-   ✅ `Integration/Damage/StatusDamageTests.cs` - Status damage

### Abilities & Items (Phase 2.9)

-   ✅ `AbilityListenerTests.cs` - Ability triggers
-   ✅ `ItemListenerTests.cs` - Item triggers
-   ✅ `Integration/System/AbilitiesItemsTests.cs` - Ability/item interactions

### Stat & Damage Modifiers (Phase 2.4 - consolidated from 2.10)

-   ✅ `IStatModifierTests.cs` - Stat modifier system
-   ✅ `AbilityStatModifierTests.cs` - Ability modifiers
-   ✅ `ItemStatModifierTests.cs` - Item modifiers
-   ✅ `Integration/Damage/StatModifiersTests.cs` - Modifier integration

### Recoil & Drain (Phase 2.11)

-   ✅ `RecoilEffectTests.cs` - Recoil damage
-   ✅ `DrainEffectTests.cs` - HP drain

## Coverage Requirements

### ✅ Completed Coverage

-   [x] All public APIs tested
-   [x] All action types tested
-   [x] All damage pipeline steps tested
-   [x] All edge cases covered (null inputs, invalid states, boundary conditions)
-   [x] Integration scenarios tested (83+ integration tests)
-   [x] Full battle end-to-end scenarios tested

### ✅ Weather System Tests (Sub-Feature 2.12)

**Location**: `Tests/Systems/Combat/`

-   `Field/WeatherTrackingTests.cs` - Weather tracking and duration (16 tests)
-   `Damage/WeatherStepTests.cs` - Weather damage modifiers (8 tests)
-   `Engine/WeatherDamageTests.cs` - Weather end-of-turn damage (9 tests)
-   `Actions/SetWeatherActionTests.cs` - Weather actions (9 tests)
-   `Helpers/WeatherAccuracyTests.cs` - Weather perfect accuracy (6 tests)

**Total**: 48 weather-related tests passing

### ✅ Terrain System Tests (Sub-Feature 2.13)

**Location**: `Tests/Systems/Combat/`

-   `Field/TerrainTrackingTests.cs` - Terrain tracking and duration (11 tests)
-   `Damage/TerrainStepTests.cs` - Terrain damage modifiers (7 tests)
-   `Engine/TerrainHealingTests.cs` - Terrain end-of-turn healing (8 tests)
-   `Actions/SetTerrainActionTests.cs` - Terrain actions (8 tests)

**Total**: 84+ terrain-related tests passing (includes integration with other systems)

### ✅ Hazards System Tests (Sub-Feature 2.14)

**Location**: `Tests/Systems/Combat/`

-   `Field/EntryHazardsTests.cs` - Entry hazard tracking and layers (12 tests)
-   `Engine/EntryHazardProcessorTests.cs` - Entry hazard processing on switch-in (13 tests)
    -   Spikes damage (1-3 layers)
    -   Stealth Rock damage (type-based effectiveness)
    -   Toxic Spikes status application (1-2 layers, Poison type absorption)
    -   Sticky Web speed reduction (Contrary support)
    -   Multiple hazards processing
    -   Immunity checks (Flying, Levitate)

### ✅ Field Conditions Tests (Sub-Feature 2.16)

**Location**: `Tests/Systems/Combat/`

-   `Field/SideConditionTrackingTests.cs` - Side condition tracking in BattleSide (16 tests)
-   `Damage/ScreenStepTests.cs` - Screen damage reduction in DamagePipeline (7 tests)
-   `Helpers/TailwindSpeedTests.cs` - Tailwind speed multiplier in TurnOrderResolver (4 tests)
-   `Actions/SafeguardProtectionTests.cs` - Safeguard status protection (4 tests)
-   `Actions/SetSideConditionActionTests.cs` - SetSideConditionAction (6 tests)
-   `Engine/SideConditionDurationTests.cs` - Side condition duration management (4 tests)
    -   Side condition tracking (add, remove, duration)
    -   Reflect/Light Screen/Aurora Veil damage reduction
    -   Tailwind speed multiplier (2.0x)
    -   Safeguard status protection (all status types)
    -   Mist stat reduction protection
    -   Side condition duration decrement
    -   Aurora Veil weather requirement (Hail/Snow)

**Total**: 25+ hazards-related tests passing

### ✅ Implemented Coverage (Phase 2.15)

-   `Actions/CounterTests.cs` - Counter/Mirror Coat mechanics (4 tests)
-   `Actions/FocusPunchTests.cs` - Focus Punch mechanics
-   `Actions/ProtectTests.cs` - Protect/Detect mechanics
-   `Actions/ProtectEdgeCasesTests.cs` - Protect edge cases
-   `Actions/MultiTurnTests.cs` - Multi-turn move mechanics
-   `Actions/MultiHitTests.cs` - Multi-hit move mechanics
-   `Actions/PursuitTests.cs` - Pursuit mechanics
-   `Actions/SemiInvulnerableTests.cs` - Semi-invulnerable move mechanics
-   `Actions/Integration/AdvancedMoveMechanicsIntegrationTests.cs` - Integration tests (6 tests)

**Total**: 20+ advanced move mechanics tests passing

### ⏳ Planned Coverage (Phases 2.16-2.19)

-   [ ] Multi-target move tests
-   [ ] Field condition interactions tests

## Test Examples

### Functional Test Example

```csharp
[Test]
public void DamagePipeline_CalculateDamage_PhysicalMove_ReturnsCorrectDamage()
{
    // Arrange
    var attacker = CreatePokemon(level: 50, attack: 100);
    var defender = CreatePokemon(level: 50, defense: 80);
    var move = CreateMove(power: 80, category: MoveCategory.Physical);
    var context = new DamageContext(attacker, defender, move);

    // Act
    var damage = DamagePipeline.CalculateDamage(context);

    // Assert
    Assert.That(damage, Is.GreaterThan(0));
    Assert.That(damage, Is.LessThanOrEqualTo(defender.CurrentHP));
}
```

### Edge Case Test Example

```csharp
[Test]
public void BattleQueue_Enqueue_NullAction_ThrowsArgumentNullException()
{
    // Arrange
    var queue = new BattleQueue();

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null));
}
```

### Integration Test Example

```csharp
[Test]
public void Actions_BattleQueue_DamageAction_TriggersFaintAction()
{
    // Arrange
    var field = CreateBattleField();
    var queue = new BattleQueue();
    var weakPokemon = CreatePokemon(hp: 1);
    var damageAction = new DamageAction(weakPokemon, 10);

    // Act
    queue.Enqueue(damageAction);
    await queue.ProcessQueue(field, new NullBattleView());

    // Assert
    Assert.That(weakPokemon.CurrentHP, Is.LessThanOrEqualTo(0));
    Assert.That(queue.Count, Is.GreaterThan(0)); // FaintAction should be enqueued
}
```

## Integration Testing Guide

See **[Integration Testing Guide](testing/integration_guide.md)** for:

-   When to create integration tests
-   Integration test patterns
-   System interaction examples
-   Cascading effect testing

## Test Execution

### Run All Combat Tests

```bash
dotnet test --filter "FullyQualifiedName~Combat"
```

### Run Functional Tests Only

```bash
dotnet test --filter "FullyQualifiedName~CombatTests&FullyQualifiedName!~EdgeCases&FullyQualifiedName!~Integration"
```

### Run Edge Case Tests Only

```bash
dotnet test --filter "FullyQualifiedName~CombatEdgeCasesTests"
```

### Run Integration Tests Only

```bash
dotnet test --filter "FullyQualifiedName~CombatIntegration"
```

### Run Specific Component Tests

```bash
dotnet test --filter "FullyQualifiedName~CombatEngineTests"
```

## Related Documents

-   **[Architecture](architecture.md)** - What to test (combat system specification)
-   **[Use Cases](use_cases.md)** - Scenarios to verify
-   **[Code Location](code_location.md)** - Where tests live
-   **[Integration Guide](testing/integration_guide.md)** - Integration test patterns
-   **[TDD Guide](../../../ai_workflow/docs/TDD_GUIDE.md)** - Test-Driven Development guide

---

**Last Updated**: 2025-01-XX
