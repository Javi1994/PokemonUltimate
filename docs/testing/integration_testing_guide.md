# Integration Testing Guide

## Overview

Integration tests verify that multiple systems work together correctly. They test interactions between components, not individual unit behavior.

## When to Create Integration Tests

Create integration tests when implementing features that:
- **Interact with multiple systems** (e.g., Action → Queue → Processing)
- **Have system boundaries** (e.g., Status → End-of-Turn → Damage)
- **Require coordination** (e.g., CombatEngine → TurnOrderResolver → BattleQueue)
- **Generate cascading effects** (e.g., DamageAction → FaintAction → BattleArbiter)

## Test Structure

### Directory Organization

```
Tests/
├── [Module]/
│   ├── [Feature]Tests.cs          # Functional tests
│   ├── [Feature]EdgeCasesTests.cs  # Edge case tests
│   └── Integration/                # Integration tests
│       ├── [Feature]IntegrationTests.cs
│       └── [System]IntegrationTests.cs
```

### Naming Convention

- **File**: `[Feature]IntegrationTests.cs` or `[System]IntegrationTests.cs`
- **Class**: `[Feature]IntegrationTests` or `[System]IntegrationTests`
- **Methods**: `[System1]_[System2]_[ExpectedBehavior]`

### Examples

- `EndOfTurnIntegrationTests.cs` - Tests end-of-turn effects with CombatEngine
- `ActionSystemIntegrationTests.cs` - Tests action interactions (Damage → Faint, Move → Effects)
- `TurnOrderIntegrationTests.cs` - Tests turn order resolution with CombatEngine

## Test Patterns

### 1. Action Chain Testing

Test that actions generate correct reactions:

```csharp
[Test]
public void DamageAction_CausesFaint_GeneratesFaintAction()
{
    // Arrange
    var damageAction = new DamageAction(...);
    
    // Act
    var reactions = damageAction.ExecuteLogic(_field).ToList();
    
    // Assert
    Assert.That(reactions, Has.Count.EqualTo(1));
    Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
}
```

### 2. System Integration Testing

Test that systems work together:

```csharp
[Test]
public async Task CombatEngine_RunTurn_ProcessesEndOfTurnEffects()
{
    // Arrange
    var engine = new CombatEngine();
    engine.Initialize(...);
    _pokemon.Status = PersistentStatus.Burn;
    
    // Act
    await engine.RunTurn();
    
    // Assert
    Assert.That(_pokemon.CurrentHP, Is.LessThan(initialHP));
}
```

### 3. State Transition Testing

Test that state changes propagate correctly:

```csharp
[Test]
public void SwitchAction_ResetsBattleState_ClearsStatStages()
{
    // Arrange
    _slot.ModifyStatStage(Stat.Attack, 2);
    
    // Act
    var switchAction = new SwitchAction(_slot, newPokemon);
    switchAction.ExecuteLogic(_field);
    
    // Assert
    Assert.That(_slot.GetStatStage(Stat.Attack), Is.EqualTo(0));
}
```

## Best Practices

### ✅ Do

- **Test real interactions** - Use actual system components, not mocks
- **Test boundaries** - Focus on where systems meet
- **Test cascading effects** - Verify reactions and side effects
- **Use realistic data** - Create Pokemon with proper stats, moves, etc.
- **Test error propagation** - Verify errors flow correctly through systems

### ❌ Don't

- **Don't test unit behavior** - That's what functional tests are for
- **Don't mock everything** - Integration tests should use real components
- **Don't test implementation details** - Focus on observable behavior
- **Don't create fragile tests** - Avoid tests that break with minor changes

## Integration Test Checklist

When creating integration tests, verify:

- [ ] Tests multiple systems working together
- [ ] Uses real components (not excessive mocking)
- [ ] Tests observable behavior (not implementation)
- [ ] Verifies state transitions
- [ ] Tests error propagation
- [ ] Uses realistic test data
- [ ] Follows naming conventions
- [ ] Located in `Tests/[Module]/Integration/`

## Examples from Codebase

### EndOfTurnIntegrationTests.cs

Tests end-of-turn effects integrated with CombatEngine:
- `RunTurn_WithBurnStatus_ProcessesEndOfTurnDamage`
- `RunTurn_StatusDamageCausesFaint_GeneratesFaintAction`
- `RunTurn_MultiplePokemonWithStatus_ProcessesAll`

### ActionSystemIntegrationTests.cs

Tests action system interactions:
- `DamageAction_CausesFaint_GeneratesFaintAction`
- `UseMoveAction_WithStatusEffect_GeneratesDamageAndStatusActions`
- `SwitchAction_ResetsBattleState_ClearsStatStages`

### TurnOrderIntegrationTests.cs

Tests turn order resolution:
- `TurnOrderResolver_SwitchAction_HighestPriority`
- `CombatEngine_RunTurn_SortsActionsByTurnOrder`

## Running Integration Tests

```bash
# Run all integration tests
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Run specific integration test file
dotnet test --filter "FullyQualifiedName~EndOfTurnIntegrationTests"
```

## Maintenance

- **Keep tests focused** - Each test should verify one integration point
- **Update when systems change** - Integration tests may need updates when APIs change
- **Remove obsolete tests** - Delete tests for removed features
- **Document complex scenarios** - Add comments explaining why the integration matters

---

**Last Updated**: Phase 2.8  
**Status**: ✅ Standardized

