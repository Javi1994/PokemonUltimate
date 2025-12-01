# Data Layer Compatibility Analysis

> **Purpose**: Verify all data systems are compatible with the action system before implementing Phase 2.5.
> **Last Updated**: December 2025

---

## 1. Executive Summary

### Overall Status: ✅ COMPATIBLE (Minor Gaps Identified)

All data layer blueprints are **well-designed** and **compatible** with the combat system architecture. A few minor gaps were identified that can be addressed incrementally.

| System | Compatibility | Gaps |
|--------|---------------|------|
| AbilityData | ✅ 95% | Missing terrain triggers |
| ItemData | ✅ 100% | None |
| StatusEffectData | ✅ 100% | None |
| WeatherData | ✅ 100% | None |
| TerrainData | ✅ 100% | None |

---

## 2. Trigger Mapping Analysis

### 2.1 AbilityTrigger → BattleTrigger

| AbilityTrigger | BattleTrigger | Status |
|----------------|---------------|--------|
| `OnSwitchIn` | `OnSwitchIn` | ✅ Match |
| `OnSwitchOut` | `OnSwitchOut` | ✅ Match |
| `OnTurnStart` | `OnTurnStart` | ✅ Match |
| `OnTurnEnd` | `OnTurnEnd` | ✅ Match |
| `OnBeforeMove` | `OnBeforeMove` | ✅ Match |
| `OnAfterMove` | `OnAfterMove` | ✅ Match |
| `OnMoveHit` | `OnMoveHit` | ✅ Match |
| `OnMoveMiss` | `OnMoveMiss` | ✅ Match |
| `OnDamageTaken` | `OnDamageTaken` | ✅ Match |
| `OnDamageDealt` | `OnDamageDealt` | ✅ Match |
| `OnWouldFaint` | `OnOHKO` | ✅ Same concept |
| `OnContactReceived` | `OnContactMade` | ✅ Context distinguishes |
| `OnContactMade` | `OnContactMade` | ✅ Match |
| `OnSuperEffectiveHit` | `OnSuperEffective` | ✅ Match |
| `OnCriticalHit` | `OnCriticalHit` | ✅ Match |
| `OnStatusAttempt` | `OnStatusAttempt` | ✅ Match |
| `OnStatusApplied` | `OnStatusApplied` | ✅ Match |
| `OnStatChangeAttempt` | `OnStatChangeAttempt` | ✅ Match |
| `OnStatChanged` | `OnStatChanged` | ✅ Match |
| `OnWeatherChange` | `OnWeatherStart` | ✅ Match |
| `OnWeatherTick` | `OnWeatherTick` | ✅ Match |
| ❌ Missing | `OnTerrainStart` | ⚠️ Gap |
| ❌ Missing | `OnTerrainEnd` | ⚠️ Gap |
| ❌ Missing | `OnTerrainTick` | ⚠️ Gap |

**Gap Resolution**: Add `OnTerrainChange` and `OnTerrainTick` to `AbilityTrigger` enum.

### 2.2 ItemTrigger → BattleTrigger

| ItemTrigger | BattleTrigger | Status |
|-------------|---------------|--------|
| `OnTurnEnd` | `OnTurnEnd` | ✅ Match |
| `OnLowHP` | N/A (derived) | ✅ Check in `OnDamageTaken` handler |
| `OnWouldFaint` | `OnOHKO` | ✅ Match |
| `OnStatusApplied` | `OnStatusApplied` | ✅ Match |
| `OnConfused` | `OnStatusApplied` | ✅ Subset |
| `OnMoveUsed` | `OnAfterMove` | ✅ Match |
| `OnDamageDealt` | `OnDamageDealt` | ✅ Match |
| `OnSuperEffectiveHit` | `OnSuperEffective` | ✅ Match |
| `OnStatLowered` | `OnStatChanged` | ✅ Context distinguishes |
| `OnContactReceived` | `OnContactMade` | ✅ Context distinguishes |

**Result**: ✅ All triggers mappable, no gaps.

---

## 3. Data Completeness for Listeners

### 3.1 AbilityData

| Required for Listener | Property | Status |
|-----------------------|----------|--------|
| When to activate | `Triggers`, `ListensTo()` | ✅ |
| What effect | `Effect` enum | ✅ |
| Target stat | `TargetStat`, `StatStages` | ✅ |
| Probability | `EffectChance` | ✅ |
| Status to apply | `StatusEffect` | ✅ |
| Affected types | `AffectedType`, `SecondaryAffectedType` | ✅ |
| Damage modifier | `Multiplier` | ✅ |
| HP threshold | `HPThreshold` | ✅ |
| Weather interaction | `WeatherCondition` | ✅ |
| Terrain interaction | ❌ Missing | ⚠️ Add `TerrainCondition?` |

### 3.2 ItemData

| Required for Listener | Property | Status |
|-----------------------|----------|--------|
| When to activate | `Triggers`, `ListensTo()` | ✅ |
| Stat boost | `TargetStat`, `StatMultiplier` | ✅ |
| Healing | `HealAmount`, `HPThreshold` | ✅ |
| Status cure | `CuresStatus` | ✅ |
| Type boost | `BoostsType` | ✅ |
| Damage boost | `DamageMultiplier` | ✅ |
| Recoil | `RecoilPercent` | ✅ |
| Move lock | `LocksMove` | ✅ |
| Consumable | `IsConsumable` | ✅ |

**Result**: ✅ Complete, no gaps.

### 3.3 StatusEffectData

| Required for Listener | Property | Status |
|-----------------------|----------|--------|
| Duration | `MinTurns`, `MaxTurns`, `GetRandomDuration()` | ✅ |
| End-of-turn damage | `EndOfTurnDamage`, `DamageEscalates`, `GetEscalatingDamage()` | ✅ |
| Move prevention | `MoveFailChance`, `PreventsAction` | ✅ |
| Recovery chance | `RecoveryChancePerTurn` | ✅ |
| Stat modifiers | `SpeedMultiplier`, `AttackMultiplier` | ✅ |
| Self-damage | `SelfHitChance`, `SelfHitPower` | ✅ |
| Type immunity | `ImmuneTypes`, `IsTypeImmune()` | ✅ |
| Move cure | `CuredByMoveTypes`, `CanBeCuredByMoveType()` | ✅ |
| Action restriction | `PreventsSwitching`, `ForcesMove`, `RestrictedMoveCategory` | ✅ |

**Result**: ✅ Complete, no gaps. Excellent coverage.

### 3.4 WeatherData

| Required for Listener | Property | Status |
|-----------------------|----------|--------|
| Duration | `DefaultDuration` | ✅ |
| Primal check | `IsPrimal`, `CanBeOverwritten` | ✅ |
| End-of-turn damage | `EndOfTurnDamage`, `DealsDamage` | ✅ |
| Type immunity | `DamageImmuneTypes`, `IsTypeImmuneToDamage()` | ✅ |
| Type power modifiers | `GetTypePowerMultiplier()` | ✅ |
| Stat boost | `BoostedStat`, `StatBoostMultiplier`, `TypeGetsStatBoost()` | ✅ |
| Move accuracy | `HasPerfectAccuracy()` | ✅ |
| Move charge skip | `ChargesInstantly()` | ✅ |
| Ability interactions | `DamageImmunityAbilities`, `SpeedBoostAbilities`, `HealingAbilities` | ✅ |

**Result**: ✅ Complete, excellent design.

### 3.5 TerrainData

| Required for Listener | Property | Status |
|-----------------------|----------|--------|
| Duration | `DefaultDuration` | ✅ |
| Type power modifier | `GetTypePowerMultiplier()` | ✅ |
| Damage reduction | `GetDamageReceivedMultiplier()` | ✅ |
| End-of-turn healing | `EndOfTurnHealing`, `HealsEachTurn` | ✅ |
| Status prevention | `PreventedStatuses`, `PreventsStatus()` | ✅ |
| Priority blocking | `BlocksPriorityMoves` | ✅ |
| Move damage halving | `HalvesMoveDamage()` | ✅ |
| Grounded check | `IsGrounded()` (static) | ✅ |
| Move transformations | `NaturePowerMove`, `CamouflageType`, `SecretPowerEffect` | ✅ |
| Ability interactions | `SetterAbilities`, `BenefitingAbilities` | ✅ |

**Result**: ✅ Complete, excellent design.

---

## 4. DamagePipeline Integration

Current steps: `BaseDamage → Critical → Random → STAB → TypeEffectiveness → Burn`

### 4.1 Required New Steps

| Source | Step Name | Position | Priority |
|--------|-----------|----------|----------|
| WeatherData | `WeatherModifierStep` | After STAB | High |
| TerrainData | `TerrainModifierStep` | After Weather | High |
| AbilityData (attacker) | `AttackerAbilityStep` | After Terrain | Medium |
| AbilityData (defender) | `DefenderAbilityStep` | After Attacker | Medium |
| ItemData (attacker) | `AttackerItemStep` | After Abilities | Medium |
| ItemData (defender) | `DefenderItemStep` | After Attacker Item | Medium |
| Status (paralysis speed) | Already in `TurnOrderResolver` | N/A | ✅ Done |
| Status (burn attack) | `BurnStep` exists | N/A | ✅ Done |

### 4.2 Step Implementation Sketch

```csharp
// Combat/Damage/Steps/WeatherModifierStep.cs
public class WeatherModifierStep : IDamageStep
{
    public void Process(DamageContext ctx)
    {
        var weatherData = ctx.Field?.CurrentWeatherData;
        if (weatherData == null) return;
        
        var moveType = ctx.Move.Type;
        ctx.Multiplier *= weatherData.GetTypePowerMultiplier(moveType);
    }
}

// Combat/Damage/Steps/TerrainModifierStep.cs
public class TerrainModifierStep : IDamageStep
{
    public void Process(DamageContext ctx)
    {
        var terrainData = ctx.Field?.CurrentTerrainData;
        if (terrainData == null) return;
        
        // Boost attacker's move
        if (IsGrounded(ctx.Attacker))
        {
            ctx.Multiplier *= terrainData.GetTypePowerMultiplier(ctx.Move.Type);
        }
        
        // Reduce damage to grounded defender
        if (IsGrounded(ctx.Defender))
        {
            ctx.Multiplier *= terrainData.GetDamageReceivedMultiplier(ctx.Move.Type);
            
            // Halve specific moves (Earthquake in Grassy)
            if (terrainData.HalvesMoveDamage(ctx.Move.Name))
                ctx.Multiplier *= 0.5f;
        }
    }
}
```

---

## 5. Identified Gaps & Resolutions

### 5.1 Gap: AbilityTrigger Missing Terrain Events

**Problem**: `AbilityTrigger` enum lacks terrain-related triggers.

**Resolution**: Add to `AbilityTrigger.cs`:
```csharp
// === TERRAIN EVENTS ===
/// <summary>When terrain changes.</summary>
OnTerrainChange = 1 << 21,
/// <summary>Each turn during terrain.</summary>
OnTerrainTick = 1 << 22,
```

**Impact**: Low - Only affects future terrain-related abilities (Grassy Surge, etc.)

### 5.2 Gap: AbilityData Missing Terrain Property

**Problem**: `AbilityData` has `WeatherCondition` but no `TerrainCondition`.

**Resolution**: Add to `AbilityData.cs`:
```csharp
/// <summary>
/// Terrain type for terrain-related abilities (Grassy Surge, etc.).
/// </summary>
public Terrain? TerrainCondition { get; }
```

Also update `AbilityBuilder` with:
```csharp
public AbilityBuilder SummonsTerrain(Terrain terrain)
{
    _effect = AbilityEffect.SummonsTerrain;
    _triggers |= AbilityTrigger.OnSwitchIn;
    _terrainCondition = terrain;
    return this;
}
```

**Impact**: Low - Only affects terrain-summoning abilities

### 5.3 Gap: AbilityEffect Missing SummonsTerrain

**Problem**: `AbilityEffect` enum lacks `SummonsTerrain`.

**Resolution**: Add to `AbilityEffect.cs`:
```csharp
/// <summary>Summons terrain on switch-in (Grassy Surge, etc.).</summary>
SummonsTerrain,
```

---

## 6. Cross-System Compatibility Matrix

| System A | System B | Compatible? | Notes |
|----------|----------|-------------|-------|
| AbilityData | BattleAction | ✅ | Triggers map to BattleTrigger |
| ItemData | BattleAction | ✅ | Triggers map to BattleTrigger |
| StatusEffectData | BattleAction | ✅ | Has all action parameters |
| WeatherData | DamagePipeline | ✅ | `GetTypePowerMultiplier()` ready |
| TerrainData | DamagePipeline | ✅ | All methods ready |
| AbilityData | DamagePipeline | ✅ | `Multiplier` property |
| ItemData | DamagePipeline | ✅ | `DamageMultiplier` property |
| StatusEffectData | TurnOrderResolver | ✅ | `SpeedMultiplier` used |
| StatusEffectData | DamagePipeline | ✅ | `BurnStep` exists |
| WeatherData | TerrainData | ✅ | Independent, can coexist |
| All | IBattleListener | ✅ | All have sufficient data |

---

## 7. Recommendations

### Immediate (Before Phase 2.5)
1. ✅ Add `OnTerrainChange`, `OnTerrainTick` to `AbilityTrigger`
2. ✅ Add `TerrainCondition` to `AbilityData`
3. ✅ Add `SummonsTerrain` to `AbilityEffect`

### Phase 2.5 (Combat Actions)
1. Create `BattleTrigger` enum (mirrors spec)
2. Create `IBattleListener` interface
3. Create `TriggerContext` class
4. Implement listener mapping from `AbilityTrigger`/`ItemTrigger` to `BattleTrigger`

### Phase 2.6 (Combat Engine)
1. Add `WeatherModifierStep` to pipeline
2. Add `TerrainModifierStep` to pipeline
3. Add `AbilityModifierStep` to pipeline
4. Add `ItemModifierStep` to pipeline

---

## 8. Conclusion

The data layer is **well-designed and nearly complete**. The three minor gaps identified are easily resolvable with small additions to existing enums and classes. The architecture supports:

- ✅ Declarative data definitions
- ✅ Clean separation of data vs behavior
- ✅ Extensible trigger system
- ✅ Modular damage calculation
- ✅ Full combat use case coverage

**Verdict**: Ready to proceed to Phase 2.5 after addressing the 3 minor gaps.

