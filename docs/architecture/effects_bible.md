# 📖 Effects Bible - Complete Reference

> Comprehensive documentation of all move effects in the combat system.

## Table of Contents

1. [Core Damage Effects](#core-damage-effects)
2. [Status Effects](#status-effects)
3. [Stat Modification Effects](#stat-modification-effects)
4. [Healing Effects](#healing-effects)
5. [Two-Turn Effects](#two-turn-effects)
6. [Protection Effects](#protection-effects)
7. [Move Restriction Effects](#move-restriction-effects)
8. [Switching Effects](#switching-effects)
9. [Field Effects](#field-effects)
10. [Special Damage Effects](#special-damage-effects)
11. [Priority Effects](#priority-effects)
12. [Utility Effects](#utility-effects)
13. [Planned Effects](#planned-effects)
14. [Effect Combinations](#effect-combinations)
15. [Implementation Status](#implementation-status)

---

## Core Damage Effects

### DamageEffect
Standard damage using the Pokémon damage formula.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DamageMultiplier` | float | 1.0 | Multiplier to final damage |
| `CanCrit` | bool | true | Whether move can critical hit |
| `CritStages` | int | 0 | Additional crit stages (0=normal, 1=high) |

**Example Moves:**
- Tackle (basic damage)
- Slash (CritStages=1)
- Frost Breath (always crits)

```csharp
// Basic damage
new DamageEffect()

// High crit ratio
new DamageEffect { CritStages = 1 }

// Cannot crit (used with guaranteed crit abilities)
new DamageEffect { CanCrit = false }
```

---

### FixedDamageEffect
Deals a fixed amount of damage, ignoring stats.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FixedAmount` | int | 0 | Exact HP damage dealt |
| `LevelBased` | bool | false | If true, damage equals user's level |
| `FractionOfHP` | float | 0 | Damage as fraction of target's HP |
| `FractionOfMaxHP` | bool | true | Use max HP vs current HP |

**Example Moves:**
| Move | Configuration |
|------|---------------|
| Dragon Rage | FixedAmount = 40 |
| Seismic Toss | LevelBased = true |
| Night Shade | LevelBased = true |
| Super Fang | FractionOfHP = 0.5 |
| Nature's Madness | FractionOfHP = 0.5 |

---

### RecoilEffect
User takes damage based on damage dealt.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RecoilPercent` | float | 0.25 | Fraction of damage dealt |
| `RecoilOfMaxHP` | float | 0 | Alternative: fraction of max HP |
| `RecoilMinimum` | int | 1 | Minimum recoil damage |

**Example Moves:**
| Move | Recoil |
|------|--------|
| Take Down | 25% |
| Double-Edge | 33% |
| Brave Bird | 33% |
| Flare Blitz | 33% |
| Head Smash | 50% |

---

### DrainEffect
User heals based on damage dealt.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DrainPercent` | float | 0.5 | Fraction of damage to heal |

**Example Moves:**
| Move | Drain |
|------|-------|
| Absorb | 50% |
| Giga Drain | 50% |
| Drain Punch | 50% |
| Leech Life | 50% |
| Oblivion Wing | 75% |
| Draining Kiss | 75% |

---

### MultiHitEffect
Hits multiple times in one turn.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MinHits` | int | 2 | Minimum hits |
| `MaxHits` | int | 5 | Maximum hits |
| `FixedHits` | int | 0 | If >0, always hits this many times |

**Hit Distribution (2-5 hits):**
- 2 hits: 35%
- 3 hits: 35%
- 4 hits: 15%
- 5 hits: 15%

**Example Moves:**
| Move | Hits | Notes |
|------|------|-------|
| Bullet Seed | 2-5 | Standard multi-hit |
| Fury Attack | 2-5 | Standard multi-hit |
| Double Kick | 2 | FixedHits = 2 |
| Triple Kick | 3 | FixedHits = 3, escalating power |
| Population Bomb | 1-10 | MinHits=1, MaxHits=10 |

---

## Status Effects

### StatusEffect (Persistent)
Applies a persistent status condition.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Status` | PersistentStatus | - | Status to apply |
| `ChancePercent` | int | 100 | Chance to apply (0-100) |
| `TargetSelf` | bool | false | Apply to user (Rest) |

**Persistent Statuses:**
| Status | Effect |
|--------|--------|
| Burn | 1/16 HP/turn, halves physical Attack |
| Paralysis | 25% fail, halves Speed |
| Poison | 1/8 HP/turn |
| BadlyPoisoned | Escalating damage (1/16, 2/16, 3/16...) |
| Sleep | Cannot move (1-3 turns) |
| Freeze | Cannot move (random thaw) |

**Example Moves:**
| Move | Status | Chance |
|------|--------|--------|
| Thunder Wave | Paralysis | 100% |
| Will-O-Wisp | Burn | 100% |
| Toxic | BadlyPoisoned | 100% |
| Flamethrower | Burn | 10% |
| Rest | Sleep | 100% (self) |

---

### VolatileStatusEffect
Applies a volatile status (clears on switch).

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Status` | VolatileStatus | - | Status to apply |
| `ChancePercent` | int | 100 | Chance to apply |
| `Duration` | int | 0 | Duration in turns (0=until cured) |

**Volatile Statuses:**
| Status | Effect |
|--------|--------|
| Confusion | 33% self-hit, 1-4 turns |
| Attract | 50% fail if opposite gender |
| Flinch | Skip action (same turn only) |
| LeechSeed | 1/8 HP drained each turn |
| Curse | 1/4 HP lost each turn (Ghost version) |
| Trapped | Cannot switch |
| Drowsy | Falls asleep next turn (Yawn) |

---

### FlinchEffect
Causes target to skip their action.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ChancePercent` | int | - | Chance to flinch |

**Example Moves:**
| Move | Chance | Notes |
|------|--------|-------|
| Fake Out | 100% | Only works turn 1 |
| Iron Head | 30% | |
| Air Slash | 30% | |
| Bite | 30% | |
| Waterfall | 20% | |

---

## Stat Modification Effects

### StatChangeEffect
Modifies stat stages (-6 to +6).

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TargetStat` | Stat | - | Stat to modify |
| `Stages` | int | - | Stages to change (-6 to +6) |
| `ChancePercent` | int | 100 | Chance to apply |
| `TargetSelf` | bool | false | Apply to user |

**Stage Multipliers:**
| Stage | Multiplier |
|-------|------------|
| -6 | 2/8 (0.25x) |
| -5 | 2/7 |
| -4 | 2/6 |
| -3 | 2/5 |
| -2 | 2/4 (0.5x) |
| -1 | 2/3 |
| 0 | 1x |
| +1 | 3/2 |
| +2 | 4/2 (2x) |
| +3 | 5/2 |
| +4 | 6/2 (3x) |
| +5 | 7/2 |
| +6 | 8/2 (4x) |

**Example Moves:**
| Move | Effect |
|------|--------|
| Swords Dance | +2 Attack (self) |
| Growl | -1 Attack (target) |
| Shell Smash | +2 Atk/SpA/Spe, -1 Def/SpD (self) |
| Close Combat | -1 Def/SpD (self) |
| Psychic | 10% chance -1 SpD (target) |

---

## Healing Effects

### HealEffect
Direct HP recovery.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `HealPercent` | float | 0.5 | Fraction of max HP to heal |
| `FixedAmount` | int | 0 | Fixed HP to heal |
| `WeatherModified` | bool | false | Affected by weather |

**Weather-Modified Healing:**
| Weather | Modifier |
|---------|----------|
| Sunny | 2/3 (66%) |
| Rain | 1/4 (25%) |
| Sandstorm | 1/4 (25%) |
| Hail | 1/4 (25%) |

**Example Moves:**
| Move | Heal |
|------|------|
| Recover | 50% |
| Soft-Boiled | 50% |
| Slack Off | 50% |
| Roost | 50% (loses Flying type) |
| Moonlight | 25-66% (weather-modified) |
| Synthesis | 25-66% (weather-modified) |
| Shore Up | 50% (66% in Sandstorm) |

---

## Two-Turn Effects

### ChargingEffect
Two-turn moves with preparation phase.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ChargeMessage` | string | - | Message during charge |
| `SemiInvulnerable` | bool | false | User cannot be hit |
| `InvulnerableState` | enum | None | Location (Flying/Underground/etc) |
| `SkipChargeInWeather` | Weather? | null | Weather that skips charge |
| `SkipChargeWithItem` | string | "Power Herb" | Item that skips charge |
| `IsRechargeMove` | bool | false | Recharge AFTER attack |

**Semi-Invulnerable States:**
| State | Vulnerable To |
|-------|---------------|
| Flying | Thunder, Hurricane, Sky Uppercut, Smack Down |
| Underground | Earthquake, Magnitude, Fissure |
| Underwater | Surf, Whirlpool |
| Vanished | Nothing (Shadow Force) |

**Example Moves:**
| Move | Type | Notes |
|------|------|-------|
| Solar Beam | Charge | Skips in Sun |
| Fly | Semi-invulnerable (Flying) | |
| Dig | Semi-invulnerable (Underground) | |
| Dive | Semi-invulnerable (Underwater) | |
| Skull Bash | Charge | +1 Defense on charge |
| Hyper Beam | Recharge | Must rest after |
| Giga Impact | Recharge | Must rest after |

---

### DelayedDamageEffect
Damage or healing occurs after a delay.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TurnsDelay` | int | 2 | Turns until effect |
| `IsHealing` | bool | false | Heals instead of damages |
| `HealFraction` | float | 0.5 | Heal amount (if healing) |
| `TargetsSlot` | bool | true | Hits slot vs specific Pokémon |
| `UsesCasterStats` | bool | true | Use caster's stats for damage |
| `DamageType` | PokemonType? | null | Type for damage calc |
| `BypassesProtect` | bool | true | Ignores Protect |

**Example Moves:**
| Move | Delay | Effect |
|------|-------|--------|
| Future Sight | 2 turns | Psychic damage, uses caster SpA |
| Doom Desire | 2 turns | Steel damage, uses caster SpA |
| Wish | 1 turn | Heals 50% of WISHER's max HP |

---

## Protection Effects

### ProtectionEffect
Blocks incoming attacks.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Type` | ProtectionType | Full | Type of protection |
| `SingleTargetOnly` | bool | false | Only blocks single-target |
| `ContactEffect` | ContactPenalty | None | Effect on contact |
| `ContactStatDrop` | Stat? | null | Stat to lower on contact |
| `ContactStatStages` | int | -2 | Stages to lower |
| `ContactStatus` | PersistentStatus? | null | Status on contact |
| `ContactDamage` | float | 0 | Damage on contact (% max HP) |

**Protection Types:**
| Type | Blocks |
|------|--------|
| Full | All moves (Protect, Detect) |
| WideGuard | Spread moves only |
| QuickGuard | Priority moves only |
| CraftyShield | Status moves only |
| MatBlock | Damaging moves (turn 1 only) |
| SpikyShield | All + 1/8 damage on contact |
| KingsShield | All + -2 Attack on contact |
| BanefulBunker | All + Poison on contact |
| Obstruct | All + -2 Defense on contact |

**Success Rate:**
- First use: 100%
- Consecutive: Success × 1/3 each use
- Resets after 1 turn without using

---

## Move Restriction Effects

### MoveRestrictionEffect
Restricts move usage.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RestrictionType` | enum | - | Type of restriction |
| `Duration` | int | 3 | Duration in turns |
| `TargetSelf` | bool | false | Apply to user (Imprison) |

**Restriction Types:**
| Type | Effect | Duration |
|------|--------|----------|
| Encore | Repeat last move | 3 turns |
| Disable | Block one move | 4 turns |
| Taunt | Block status moves | 3 turns |
| Torment | Can't repeat same move | Until switch |
| Imprison | Block shared moves | Until user switches |
| HealBlock | Block healing | 5 turns |
| Embargo | Block items | 5 turns |
| ThroatChop | Block sound moves | 2 turns |

---

### BindingEffect
Traps and damages over time.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MinTurns` | int | 4 | Minimum duration |
| `MaxTurns` | int | 5 | Maximum duration |
| `ExtendedTurns` | int | 7 | Duration with Grip Claw |
| `DamagePerTurn` | float | 0.125 | Damage per turn (1/8) |
| `EnhancedDamagePerTurn` | float | 0.167 | With Binding Band (1/6) |
| `PreventsSwitch` | bool | true | Target can't switch |

**Example Moves:**
| Move | Type | Notes |
|------|------|-------|
| Wrap | Normal | |
| Bind | Normal | |
| Fire Spin | Fire | |
| Whirlpool | Water | Traps diving Pokémon |
| Sand Tomb | Ground | |
| Infestation | Bug | |
| Magma Storm | Fire | Most powerful (100 BP) |

---

## Switching Effects

### ForceSwitchEffect
Forces target to switch out.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DealsDamage` | bool | false | Also deals damage |
| `RandomReplacement` | bool | true | Replacement is random |
| `WorksInTrainerBattles` | bool | true | Works vs trainers |
| `EndsWildBattle` | bool | true | Ends wild battles |

**Example Moves:**
| Move | Deals Damage | Priority |
|------|--------------|----------|
| Roar | No | -6 |
| Whirlwind | No | -6 |
| Dragon Tail | Yes (60 BP) | -6 |
| Circle Throw | Yes (60 BP) | -6 |

---

### SwitchAfterAttackEffect
User switches after attacking.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DealsDamage` | bool | true | Deals damage before switch |
| `MandatorySwitch` | bool | true | Must switch if able |
| `StatChanges` | StatChangeEffect[] | null | Stat changes before switch |

**Example Moves:**
| Move | Type | Power | Notes |
|------|------|-------|-------|
| U-turn | Bug | 70 | |
| Volt Switch | Electric | 70 | |
| Flip Turn | Water | 60 | |
| Parting Shot | Dark | - | -1 Atk/SpA to target |
| Teleport | Psychic | - | -6 priority (Gen 8+) |

---

## Field Effects

### FieldConditionEffect
Sets or removes field conditions.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ConditionType` | FieldConditionType | - | Category of condition |
| `WeatherToSet` | Weather? | null | Weather to set |
| `TerrainToSet` | Terrain? | null | Terrain to set |
| `HazardToSet` | HazardType? | null | Hazard to set |
| `SideConditionToSet` | SideCondition? | null | Screen/barrier |
| `FieldEffectToSet` | FieldEffect? | null | Room effect |
| `TargetsUserSide` | bool | false | Which side |
| `RemovesCondition` | bool | false | Removal mode |

**Weather Moves:**
| Move | Weather | Duration |
|------|---------|----------|
| Rain Dance | Rain | 5 turns (8 with Damp Rock) |
| Sunny Day | Sun | 5 turns (8 with Heat Rock) |
| Sandstorm | Sandstorm | 5 turns (8 with Smooth Rock) |
| Hail | Hail | 5 turns (8 with Icy Rock) |
| Snowscape | Snow | 5 turns |

**Terrain Moves:**
| Move | Terrain | Duration |
|------|---------|----------|
| Electric Terrain | Electric | 5 turns |
| Grassy Terrain | Grassy | 5 turns |
| Psychic Terrain | Psychic | 5 turns |
| Misty Terrain | Misty | 5 turns |

**Hazard Moves:**
| Move | Hazard | Effect |
|------|--------|--------|
| Stealth Rock | StealthRock | Rock-type damage on entry |
| Spikes | Spikes | Stackable ground damage |
| Toxic Spikes | ToxicSpikes | Poison/BadlyPoison on entry |
| Sticky Web | StickyWeb | -1 Speed on entry |

**Screen Moves:**
| Move | Screen | Effect |
|------|--------|--------|
| Reflect | Reflect | Halves physical damage |
| Light Screen | LightScreen | Halves special damage |
| Aurora Veil | AuroraVeil | Both (Hail only) |

**Room Moves:**
| Move | Effect | Duration |
|------|--------|----------|
| Trick Room | Reverse speed order | 5 turns |
| Magic Room | Disable items | 5 turns |
| Wonder Room | Swap Def/SpD | 5 turns |
| Gravity | Ground all, disable jumping | 5 turns |

**Removal Moves:**
| Move | Removes |
|------|---------|
| Rapid Spin | Own side hazards |
| Defog | All hazards + screens |
| Court Change | Swaps hazards/screens |

---

## Special Damage Effects

### SelfDestructEffect
User faints after using.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Type` | SelfDestructType | Explosion | Type of self-destruct |
| `DealsDamage` | bool | true | Deals damage |
| `StatChanges` | StatChangeEffect[] | null | Stat changes first |
| `HealsReplacement` | bool | false | Heals next Pokémon |
| `RestoresPP` | bool | false | Restores PP |
| `DamageEqualsHP` | bool | false | Damage = user HP |

**Example Moves:**
| Move | Effect |
|------|--------|
| Explosion | 250 BP, user faints |
| Self-Destruct | 200 BP, user faints |
| Memento | -2 Atk/SpA, user faints |
| Final Gambit | Damage = user's HP, user faints |
| Healing Wish | User faints, replacement fully healed |
| Lunar Dance | User faints, replacement healed + PP restored |

---

### RevengeEffect
Returns damage based on damage taken.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CountersCategory` | MoveCategory? | null | Physical/Special/both |
| `DamageMultiplier` | float | 2.0 | Return multiplier |
| `Priority` | int | 0 | Move priority |
| `RequiresHit` | bool | true | Must be hit first |
| `AccumulationTurns` | int | 0 | Turns to accumulate (Bide) |

**Example Moves:**
| Move | Counters | Multiplier | Priority |
|------|----------|------------|----------|
| Counter | Physical | 2x | -5 |
| Mirror Coat | Special | 2x | -5 |
| Metal Burst | Both | 1.5x | 0 |
| Bide | Both | 2x | +1 (2 turns) |

---

## Priority Effects

### PriorityModifierEffect
Conditionally modifies priority.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PriorityChange` | int | 1 | Priority modifier |
| `Condition` | PriorityCondition | Always | Activation condition |
| `RequiredTerrain` | Terrain? | null | For terrain condition |
| `RequiredWeather` | Weather? | null | For weather condition |
| `HPThreshold` | float | 1.0 | For HP condition |

**Priority Brackets:**
| Priority | Examples |
|----------|----------|
| +5 | Helping Hand |
| +4 | Protect, Detect, Endure |
| +3 | Fake Out, Quick Guard, Wide Guard |
| +2 | Extreme Speed, Feint, Follow Me |
| +1 | Quick Attack, Mach Punch, Aqua Jet |
| 0 | Most moves |
| -1 | Vital Throw |
| -3 | Focus Punch |
| -5 | Counter, Mirror Coat |
| -6 | Roar, Whirlwind, Dragon Tail |
| -7 | Trick Room |

**Conditional Priority:**
| Move/Ability | Condition | Priority |
|--------------|-----------|----------|
| Grassy Glide | Grassy Terrain | +1 |
| Gale Wings | Full HP + Flying move | +1 |
| Prankster | Status move | +1 |
| Triage | Healing move | +3 |

---

## Utility Effects

### PowerUp (Planned)
Boosts next attack.

**Example Moves:**
| Move | Effect |
|------|--------|
| Focus Energy | +2 crit stages |
| Charge | 2x Electric power next turn |
| Laser Focus | Guaranteed crit next turn |

### CallMove (Planned)
Calls another move.

**Example Moves:**
| Move | Effect |
|------|--------|
| Metronome | Random move |
| Mirror Move | Copy last used move |
| Copycat | Copy last move in battle |
| Sleep Talk | Use random known move while asleep |
| Me First | Use target's move at 1.5x |

### Transform (Planned)
Copies target's form.

**Example Moves:**
| Move | Effect |
|------|--------|
| Transform | Copy species, stats, moves, types |

---

## Planned Effects

These effects are defined in EffectType but not yet implemented:

| Effect | Description | Priority |
|--------|-------------|----------|
| MagicBounce | Reflect status moves | Medium |
| ItemSwap | Exchange items (Trick) | Low |
| AbilityCopy | Copy ability (Role Play) | Low |
| AbilitySuppress | Disable ability (Gastro Acid) | Low |
| TypeChange | Change type (Soak) | Low |
| RoomEffect | Set room (Trick Room) | Implemented via FieldCondition |
| Substitute | Create substitute | High |
| BatonPass | Switch with transfer | Medium |

---

## Effect Combinations

Many moves combine multiple effects:

### Flamethrower (Fire, 90 BP)
```csharp
Effects = [
    new DamageEffect(),
    new StatusEffect(PersistentStatus.Burn, chancePercent: 10)
]
```

### Close Combat (Fighting, 120 BP)
```csharp
Effects = [
    new DamageEffect(),
    new StatChangeEffect(Stat.Defense, -1, targetSelf: true),
    new StatChangeEffect(Stat.SpDefense, -1, targetSelf: true)
]
```

### Volt Switch (Electric, 70 BP)
```csharp
Effects = [
    new DamageEffect(),
    new SwitchAfterAttackEffect()
]
```

### Shell Smash (Normal, Status)
```csharp
Effects = [
    new StatChangeEffect(Stat.Attack, +2, targetSelf: true),
    new StatChangeEffect(Stat.SpAttack, +2, targetSelf: true),
    new StatChangeEffect(Stat.Speed, +2, targetSelf: true),
    new StatChangeEffect(Stat.Defense, -1, targetSelf: true),
    new StatChangeEffect(Stat.SpDefense, -1, targetSelf: true)
]
```

### Stealth Rock (Rock, Status)
```csharp
Effects = [
    new FieldConditionEffect {
        ConditionType = FieldConditionType.Hazard,
        HazardToSet = HazardType.StealthRock,
        TargetsUserSide = false
    }
]
```

### Fly (Flying, 90 BP)
```csharp
Effects = [
    new ChargingEffect {
        ChargeMessage = "{0} flew up high!",
        SemiInvulnerable = true,
        InvulnerableState = SemiInvulnerableState.Flying
    },
    new DamageEffect()
]
```

---

## Implementation Status

### ✅ Fully Implemented (19)
| Effect | File | Tests |
|--------|------|-------|
| DamageEffect | `DamageEffect.cs` | ✓ |
| FixedDamageEffect | `FixedDamageEffect.cs` | ✓ |
| StatusEffect | `StatusEffect.cs` | ✓ |
| VolatileStatusEffect | `VolatileStatusEffect.cs` | Pending |
| StatChangeEffect | `StatChangeEffect.cs` | ✓ |
| RecoilEffect | `RecoilEffect.cs` | ✓ |
| DrainEffect | `DrainEffect.cs` | ✓ |
| HealEffect | `HealEffect.cs` | ✓ |
| FlinchEffect | `FlinchEffect.cs` | ✓ |
| MultiHitEffect | `MultiHitEffect.cs` | ✓ |
| ProtectionEffect | `ProtectionEffect.cs` | Pending |
| ChargingEffect | `ChargingEffect.cs` | Pending |
| DelayedDamageEffect | `DelayedDamageEffect.cs` | Pending |
| MoveRestrictionEffect | `MoveRestrictionEffect.cs` | Pending |
| BindingEffect | `BindingEffect.cs` | Pending |
| ForceSwitchEffect | `ForceSwitchEffect.cs` | Pending |
| SwitchAfterAttackEffect | `SwitchAfterAttackEffect.cs` | Pending |
| FieldConditionEffect | `FieldConditionEffect.cs` | Pending |
| SelfDestructEffect | `SelfDestructEffect.cs` | Pending |
| RevengeEffect | `RevengeEffect.cs` | Pending |
| PriorityModifierEffect | `PriorityModifierEffect.cs` | Pending |

### 📋 Planned (6)
- BatonPassEffect
- SubstituteEffect
- CallMoveEffect
- TransformEffect
- MagicBounceEffect
- TypeChangeEffect

---

## Quick Reference Card

```
┌─────────────────────────────────────────────────────────────┐
│                     EFFECT QUICK REFERENCE                   │
├─────────────────────────────────────────────────────────────┤
│ DAMAGE                                                       │
│   DamageEffect      - Standard formula damage               │
│   FixedDamageEffect - Fixed/level-based damage              │
│   RecoilEffect      - User takes % of damage dealt          │
│   DrainEffect       - User heals % of damage dealt          │
│   MultiHitEffect    - 2-5 hits per turn                     │
├─────────────────────────────────────────────────────────────┤
│ STATUS                                                       │
│   StatusEffect         - Burn/Para/Poison/Sleep/Freeze      │
│   VolatileStatusEffect - Confusion/Attract/LeechSeed        │
│   FlinchEffect         - Skip target's action               │
├─────────────────────────────────────────────────────────────┤
│ STATS                                                        │
│   StatChangeEffect  - Modify stat stages (-6 to +6)         │
├─────────────────────────────────────────────────────────────┤
│ HEALING                                                      │
│   HealEffect        - Direct HP recovery                    │
├─────────────────────────────────────────────────────────────┤
│ SPECIAL                                                      │
│   ProtectionEffect      - Block attacks                     │
│   ChargingEffect        - Two-turn moves                    │
│   DelayedDamageEffect   - Future Sight/Wish                 │
│   MoveRestrictionEffect - Encore/Taunt/Disable              │
│   BindingEffect         - Wrap/Fire Spin                    │
│   ForceSwitchEffect     - Roar/Dragon Tail                  │
│   SwitchAfterAttackEffect - U-turn/Volt Switch              │
│   FieldConditionEffect  - Weather/Terrain/Hazards/Screens   │
│   SelfDestructEffect    - Explosion/Memento                 │
│   RevengeEffect         - Counter/Mirror Coat               │
│   PriorityModifierEffect - Conditional priority             │
└─────────────────────────────────────────────────────────────┘
```

---

*Last updated: December 2025*
*Total Effects: 21 implemented + 6 planned = 27 total*

