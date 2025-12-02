# Implementation Plan: Pipeline Hooks for Abilities & Items

**Spec Document**: `docs/architecture/abilities_items_system.md`

## Overview
Implement Pipeline Hooks system to allow abilities and items to modify stats and damage calculations passively (without generating Actions).

## Components to Implement

### Implementing Now:
- `IStatModifier` interface - For passive stat/damage modifiers
- `AbilityStatModifier` adapter - Converts `AbilityData` to `IStatModifier`
- `ItemStatModifier` adapter - Converts `ItemData` to `IStatModifier`
- Integration in `BaseDamageStep` - Apply stat modifiers during stat calculation
- `AttackerAbilityStep` - Apply ability damage modifiers
- `AttackerItemStep` - Apply item damage modifiers
- Choice Band implementation - +50% Attack stat
- Life Orb implementation - +30% damage, 10% recoil
- Blaze implementation - +50% Fire damage when HP < 33%

### Deferring to Later:
- `DefenderAbilityStep` / `DefenderItemStep` - Damage reduction modifiers (future)
- Weather/Terrain modifiers - Requires weather/terrain system integration
- More complex abilities (Adaptability, etc.) - Can be added incrementally
- Move locking for Choice items - Requires action system changes

## API Design

### IStatModifier Interface
```csharp
public interface IStatModifier
{
    /// <summary>
    /// Gets the stat multiplier for a specific stat.
    /// Returns 1.0f if no modification.
    /// </summary>
    float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field);
    
    /// <summary>
    /// Gets the damage multiplier for an attack.
    /// Returns 1.0f if no modification.
    /// </summary>
    float GetDamageMultiplier(DamageContext context);
}
```

### Integration Points
1. **BaseDamageStep**: Before calculating stat ratio, apply stat modifiers
2. **DamagePipeline**: Add `AttackerAbilityStep` and `AttackerItemStep` after STAB
3. **StatCalculator**: Not needed - modifiers applied in BaseDamageStep

## Test Plan

### Functional Tests:
- `IStatModifier_GetStatMultiplier_ReturnsCorrectMultiplier`
- `AbilityStatModifier_ChoiceBand_Returns1_5ForAttack`
- `ItemStatModifier_LifeOrb_Returns1_3ForDamage`
- `BaseDamageStep_WithChoiceBand_AppliesStatMultiplier`
- `AttackerItemStep_WithLifeOrb_AppliesDamageMultiplier`
- `AttackerAbilityStep_WithBlaze_AppliesDamageMultiplierWhenLowHP`

### Edge Cases:
- Null ability/item
- Stat multiplier = 1.0 (no effect)
- Multiple modifiers stacking
- HP threshold edge cases (exactly 33%, 32.9%, 33.1%)
- Move type matching for Blaze

### Integration Tests:
- Choice Band → BaseDamageStep → Final damage increased
- Life Orb → AttackerItemStep → Damage increased, recoil applied
- Blaze → AttackerAbilityStep → Fire moves boosted when low HP

## Implementation Order

1. Create `IStatModifier` interface
2. Create `AbilityStatModifier` adapter
3. Create `ItemStatModifier` adapter
4. Write functional tests (TDD)
5. Integrate in `BaseDamageStep` for stat modifiers
6. Create `AttackerAbilityStep` and `AttackerItemStep`
7. Integrate steps in `DamagePipeline`
8. Implement Choice Band, Life Orb, Blaze
9. Write edge case tests
10. Write integration tests
11. Update smoke tests
12. Update documentation

