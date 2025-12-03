# Sub-Feature 3.2: Move Expansion - Architecture

> Complete technical specification of the move catalog system.

**Sub-Feature Number**: 3.2  
**Parent Feature**: Feature 3: Content Expansion  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 1. Overview
Moves are the primary interaction in combat. To support thousands of moves without creating thousands of classes, we use a **Composition Pattern**.
A Move is defined by its **Data** and a list of **Effects**. It is **NOT** a unique class (e.g., no `class Ember : Move`).

## 2. Static Data (The "Blueprint")
*Namespace: `PokemonUltimate.Core.Models`*

```csharp
public class MoveData {
    // Identity
    public string Name { get; set; }        // "Ember"
    public string Description { get; set; }
    
    // Core Rules
    public PokemonType Type { get; set; }
    public MoveCategory Category { get; set; } // Physical, Special, Status
    public int Power { get; set; }
    public int Accuracy { get; set; }
    public int MaxPP { get; set; }
    public int Priority { get; set; }
    public TargetScope TargetScope { get; set; } // Self, SingleEnemy, AllEnemies...

    // Logic Composition (The "What it does")
    public List<IMoveEffect> Effects { get; set; } = new List<IMoveEffect>();
}
```

## 3. MoveBuilder API

Use the fluent `MoveBuilder` to define moves in a readable, type-safe way.

### Basic Usage
```csharp
public static readonly MoveData Tackle = Move.Define("Tackle")
    .Description("A physical attack in which the user charges and slams into the target.")
    .Type(PokemonType.Normal)
    .Physical(40, 100, 35)  // power, accuracy, pp
    .WithEffects(e => e.Damage())
    .Build();
```

### Full API Reference

```csharp
Move.Define("Name")
    .Description("...")               // Optional description
    .Type(PokemonType.Fire)           // Element type
    
    // Category (choose one)
    .Physical(power, accuracy, pp)    // Physical attack
    .Special(power, accuracy, pp)     // Special attack
    .Status(accuracy, pp)             // Status move (power = 0)
    
    // Optional modifiers
    .Priority(1)                      // Default 0, range -7 to +5
    .Target(TargetScope.AllEnemies)   // Default SingleEnemy
    .TargetSelf()                     // Shorthand for Self scope
    .TargetAllEnemies()               // Shorthand for AllEnemies scope
    
    // Effects
    .WithEffects(e => e
        .Damage()
        .MayBurn(10))
    
    .Build();
```

### Complex Move Examples

```csharp
// Physical move with priority
public static readonly MoveData QuickAttack = Move.Define("Quick Attack")
    .Type(PokemonType.Normal)
    .Physical(40, 100, 30)
    .Priority(1)
    .WithEffects(e => e.Damage())
    .Build();

// Special move with secondary effect
public static readonly MoveData Thunderbolt = Move.Define("Thunderbolt")
    .Type(PokemonType.Electric)
    .Special(90, 100, 15)
    .WithEffects(e => e
        .Damage()
        .MayParalyze(10))
    .Build();

// Status move with stat changes
public static readonly MoveData SwordsDance = Move.Define("Swords Dance")
    .Type(PokemonType.Normal)
    .Status(0, 20)
    .TargetSelf()
    .WithEffects(e => e.RaiseAttack(2))
    .Build();
```

## 4. EffectBuilder API

The `EffectBuilder` provides a fluent way to compose move effects.

### Damage Effects
```csharp
.Damage()                    // Standard damage
.DamageHighCrit(stages)      // High crit ratio (default +1)
.FixedDamage(40)             // Fixed amount (Dragon Rage)
.LevelDamage()               // Damage = user level (Seismic Toss)
```

### Status Effects
```csharp
.MayBurn(chance)             // Burn (default 100%)
.MayParalyze(chance)         // Paralysis
.MayPoison(chance)           // Poison
.MayBadlyPoison(chance)      // Toxic poison
.MaySleep(chance)            // Sleep
.MayFreeze(chance)           // Freeze
.SelfStatus(status)          // Apply status to self (Rest)
```

### Stat Changes
```csharp
// Raise user's stats
.RaiseAttack(stages, chance)
.RaiseDefense(stages, chance)
.RaiseSpAttack(stages, chance)
.RaiseSpDefense(stages, chance)
.RaiseSpeed(stages, chance)
.RaiseEvasion(stages, chance)

// Lower target's stats
.LowerAttack(stages, chance)
.LowerDefense(stages, chance)
.LowerSpAttack(stages, chance)
.LowerSpDefense(stages, chance)
.LowerSpeed(stages, chance)
.LowerAccuracy(stages, chance)
```

### Recoil, Drain, Heal
```csharp
.Recoil(percent)             // User takes % of damage dealt
.Drain(percent)              // User heals % of damage dealt
.Heal(percent)               // Heal % of max HP
```

### Other Effects
```csharp
.MayFlinch(chance)           // Target may flinch
.MultiHit(min, max)          // Hit 2-5 times (default)
.HitsNTimes(n)               // Hit exactly N times
```

## 5. Dynamic Data (The "Instance")
*Namespace: `PokemonUltimate.Core.Models`*

Represents a move learned by a specific Pokemon.

```csharp
public class MoveInstance {
    public MoveData Data { get; private set; }
    public int CurrentPP { get; set; }
    public int PPUps { get; set; }

    public MoveInstance(MoveData data) {
        Data = data;
        CurrentPP = data.MaxPP;
    }
}
```

## 6. Move Effects (The Building Blocks)
*Namespace: `PokemonUltimate.Core.Effects`*

Effects are small, reusable logic blocks that define **what a move does**. They are data definitions stored in `MoveData`. During combat, `UseMoveAction` reads these effects and generates `BattleAction` instances.

> **Related**: For how effects execute in combat, see [`../../2-combat-system/2.5-combat-actions/effects/architecture.md`](../../2-combat-system/2.5-combat-actions/effects/architecture.md).

### Current Implementation (Data Definitions)
```csharp
public interface IMoveEffect {
    // Type-safe identifier for this effect
    EffectType EffectType { get; }
    
    // Human-readable description
    string Description { get; }
}
```

### Effect Property Reference

#### Core Damage Effects

**DamageEffect**
Standard damage using the Pokémon damage formula.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DamageMultiplier` | float | 1.0 | Multiplier to final damage |
| `CanCrit` | bool | true | Whether move can critical hit |
| `CritStages` | int | 0 | Additional crit stages (0=normal, 1=high) |

**Example Moves**: Tackle, Razor Leaf, Slash (CritStages=1), Frost Breath (always crits)

**FixedDamageEffect**
Deals a fixed amount of damage, ignoring stats.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FixedAmount` | int | 0 | Exact HP damage dealt |
| `LevelBased` | bool | false | If true, damage equals user's level |
| `FractionOfHP` | float | 0 | Damage as fraction of target's HP |
| `FractionOfMaxHP` | bool | true | Use max HP vs current HP |

**Example Moves**: Dragon Rage (FixedAmount=40), Seismic Toss (LevelBased=true), Super Fang (FractionOfHP=0.5)

**RecoilEffect**
User takes damage based on damage dealt.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RecoilPercent` | float | 0.25 | Fraction of damage dealt |
| `RecoilOfMaxHP` | float | 0 | Alternative: fraction of max HP |
| `RecoilMinimum` | int | 1 | Minimum recoil damage |

**Example Moves**: Take Down (25%), Double-Edge (33%), Brave Bird (33%), Head Smash (50%)

**DrainEffect**
User heals based on damage dealt.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DrainPercent` | float | 0.5 | Fraction of damage to heal |

**Example Moves**: Absorb (50%), Giga Drain (50%), Oblivion Wing (75%)

**MultiHitEffect**
Hits multiple times in one turn.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MinHits` | int | 2 | Minimum hits |
| `MaxHits` | int | 5 | Maximum hits |
| `FixedHits` | int | 0 | If >0, always hits this many times |

**Hit Distribution (2-5 hits)**: 2 hits: 35%, 3 hits: 35%, 4 hits: 15%, 5 hits: 15%

**Example Moves**: Bullet Seed (2-5), Double Kick (FixedHits=2), Triple Kick (FixedHits=3)

---

#### Status Effects

**StatusEffect (Persistent)**
Applies a persistent status condition.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Status` | PersistentStatus | - | Status to apply |
| `ChancePercent` | int | 100 | Chance to apply (0-100) |
| `TargetSelf` | bool | false | Apply to user (Rest) |

**Persistent Statuses**: Burn, Paralysis, Poison, BadlyPoisoned, Sleep, Freeze

**Example Moves**: Thunder Wave (Paralysis, 100%), Will-O-Wisp (Burn, 100%), Flamethrower (Burn, 10%), Rest (Sleep, 100%, self)

**VolatileStatusEffect**
Applies a volatile status (clears on switch).

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Status` | VolatileStatus | - | Status to apply |
| `ChancePercent` | int | 100 | Chance to apply |
| `Duration` | int | 0 | Duration in turns (0=until cured) |

**Volatile Statuses**: Confusion, Attract, Flinch, LeechSeed, Curse, Trapped, Drowsy

**FlinchEffect**
Causes target to skip their action.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ChancePercent` | int | - | Chance to flinch |

**Example Moves**: Fake Out (100%), Iron Head (30%), Air Slash (30%), Waterfall (20%)

---

#### Stat Modification Effects

**StatChangeEffect**
Modifies stat stages (-6 to +6).

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TargetStat` | Stat | - | Stat to modify |
| `Stages` | int | - | Stages to change (-6 to +6) |
| `ChancePercent` | int | 100 | Chance to apply |
| `TargetSelf` | bool | false | Apply to user |

**Stage Multipliers**: -6 (0.25x), -2 (0.5x), 0 (1x), +2 (2x), +4 (3x), +6 (4x)

**Example Moves**: Swords Dance (+2 Attack, self), Growl (-1 Attack, target), Shell Smash (+2 Atk/SpA/Spe, -1 Def/SpD, self)

---

#### Healing Effects

**HealEffect**
Direct HP recovery.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `HealPercent` | float | 0.5 | Fraction of max HP to heal |
| `FixedAmount` | int | 0 | Fixed HP to heal |
| `WeatherModified` | bool | false | Affected by weather |

**Weather-Modified Healing**: Sunny (66%), Rain/Sandstorm/Hail (25%)

**Example Moves**: Recover (50%), Moonlight (25-66%, weather-modified), Synthesis (25-66%, weather-modified)

---

#### Two-Turn Effects

**ChargingEffect**
Two-turn moves with preparation phase.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ChargeMessage` | string | - | Message during charge |
| `SemiInvulnerable` | bool | false | User cannot be hit |
| `InvulnerableState` | enum | None | Location (Flying/Underground/etc) |
| `SkipChargeInWeather` | Weather? | null | Weather that skips charge |
| `SkipChargeWithItem` | string | "Power Herb" | Item that skips charge |
| `IsRechargeMove` | bool | false | Recharge AFTER attack |

**Example Moves**: Solar Beam (skips in Sun), Fly (Semi-invulnerable, Flying), Hyper Beam (Recharge)

**DelayedDamageEffect**
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

**Example Moves**: Future Sight (2 turns, Psychic damage), Wish (1 turn, heals 50%)

---

#### Protection Effects

**ProtectionEffect**
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

**Protection Types**: Full, WideGuard, QuickGuard, CraftyShield, SpikyShield, KingsShield, BanefulBunker

**Success Rate**: First use: 100%, Consecutive: Success × 1/3 each use

---

#### Move Restriction Effects

**MoveRestrictionEffect**
Restricts move usage.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RestrictionType` | enum | - | Type of restriction |
| `Duration` | int | 3 | Duration in turns |
| `TargetSelf` | bool | false | Apply to user (Imprison) |

**Restriction Types**: Encore (3 turns), Disable (4 turns), Taunt (3 turns), Torment (until switch), Imprison (until user switches)

**BindingEffect**
Traps and damages over time.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MinTurns` | int | 4 | Minimum duration |
| `MaxTurns` | int | 5 | Maximum duration |
| `DamagePerTurn` | float | 0.125 | Damage per turn (1/8) |
| `PreventsSwitch` | bool | true | Target can't switch |

**Example Moves**: Wrap, Bind, Fire Spin, Whirlpool, Infestation

---

#### Switching Effects

**ForceSwitchEffect**
Forces target to switch out.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DealsDamage` | bool | false | Also deals damage |
| `RandomReplacement` | bool | true | Replacement is random |
| `WorksInTrainerBattles` | bool | true | Works vs trainers |
| `EndsWildBattle` | bool | true | Ends wild battles |

**Example Moves**: Roar (no damage), Dragon Tail (60 BP damage)

**SwitchAfterAttackEffect**
User switches after attacking.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DealsDamage` | bool | true | Deals damage before switch |
| `MandatorySwitch` | bool | true | Must switch if able |
| `StatChanges` | StatChangeEffect[] | null | Stat changes before switch |

**Example Moves**: U-turn (70 BP), Volt Switch (70 BP), Parting Shot (-1 Atk/SpA to target)

---

#### Field Effects

**FieldConditionEffect**
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

**Weather Moves**: Rain Dance, Sunny Day, Sandstorm, Hail (5 turns, 8 with rock items)

**Terrain Moves**: Electric Terrain, Grassy Terrain, Psychic Terrain, Misty Terrain (5 turns)

**Hazard Moves**: Stealth Rock, Spikes, Toxic Spikes, Sticky Web

**Screen Moves**: Reflect, Light Screen, Aurora Veil

**Room Moves**: Trick Room, Magic Room, Wonder Room, Gravity (5 turns)

---

#### Special Damage Effects

**SelfDestructEffect**
User faints after using.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Type` | SelfDestructType | Explosion | Type of self-destruct |
| `DealsDamage` | bool | true | Deals damage |
| `StatChanges` | StatChangeEffect[] | null | Stat changes first |
| `HealsReplacement` | bool | false | Heals next Pokémon |
| `RestoresPP` | bool | false | Restores PP |
| `DamageEqualsHP` | bool | false | Damage = user HP |

**Example Moves**: Explosion (250 BP), Self-Destruct (200 BP), Memento (-2 Atk/SpA), Final Gambit (damage = HP), Healing Wish (heals replacement)

**RevengeEffect**
Returns damage based on damage taken.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CountersCategory` | MoveCategory? | null | Physical/Special/both |
| `DamageMultiplier` | float | 2.0 | Return multiplier |
| `Priority` | int | 0 | Move priority |
| `RequiresHit` | bool | true | Must be hit first |
| `AccumulationTurns` | int | 0 | Turns to accumulate (Bide) |

**Example Moves**: Counter (Physical, 2x, -5 priority), Mirror Coat (Special, 2x, -5 priority), Metal Burst (Both, 1.5x), Bide (Both, 2x, +1 priority, 2 turns)

---

#### Priority Effects

**PriorityModifierEffect**
Conditionally modifies priority.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PriorityChange` | int | 1 | Priority modifier |
| `Condition` | PriorityCondition | Always | Activation condition |
| `RequiredTerrain` | Terrain? | null | For terrain condition |
| `RequiredWeather` | Weather? | null | For weather condition |
| `HPThreshold` | float | 1.0 | For HP condition |

**Priority Brackets**: +5 (Helping Hand), +4 (Protect), +3 (Fake Out), +2 (Extreme Speed), +1 (Quick Attack), 0 (Most moves), -1 (Vital Throw), -5 (Counter), -6 (Roar)

**Conditional Priority**: Grassy Glide (Grassy Terrain), Gale Wings (Full HP + Flying), Prankster (Status move), Triage (Healing move)

---

### Effect Combinations

Many moves combine multiple effects:

**Flamethrower** (Fire, 90 BP):
```csharp
Effects = [
    new DamageEffect(),
    new StatusEffect(PersistentStatus.Burn, chancePercent: 10)
]
```

**Close Combat** (Fighting, 120 BP):
```csharp
Effects = [
    new DamageEffect(),
    new StatChangeEffect(Stat.Defense, -1, targetSelf: true),
    new StatChangeEffect(Stat.SpDefense, -1, targetSelf: true)
]
```

**Volt Switch** (Electric, 70 BP):
```csharp
Effects = [
    new DamageEffect(),
    new SwitchAfterAttackEffect()
]
```

**Shell Smash** (Normal, Status):
```csharp
Effects = [
    new StatChangeEffect(Stat.Attack, +2, targetSelf: true),
    new StatChangeEffect(Stat.SpAttack, +2, targetSelf: true),
    new StatChangeEffect(Stat.Speed, +2, targetSelf: true),
    new StatChangeEffect(Stat.Defense, -1, targetSelf: true),
    new StatChangeEffect(Stat.SpDefense, -1, targetSelf: true)
]
```

---

### Implementation Status

**✅ Fully Implemented (19)**
- DamageEffect, FixedDamageEffect, StatusEffect, VolatileStatusEffect
- StatChangeEffect, RecoilEffect, DrainEffect, HealEffect
- FlinchEffect, MultiHitEffect, ProtectionEffect, ChargingEffect
- DelayedDamageEffect, MoveRestrictionEffect, BindingEffect
- ForceSwitchEffect, SwitchAfterAttackEffect, FieldConditionEffect
- SelfDestructEffect, RevengeEffect, PriorityModifierEffect

**📋 Planned (6)**
- BatonPassEffect, SubstituteEffect, CallMoveEffect
- TransformEffect, MagicBounceEffect, TypeChangeEffect

---

> **Related**: For how these effects execute in combat and generate BattleActions, see [`../../2-combat-system/2.5-combat-actions/effects/architecture.md`](../../2-combat-system/2.5-combat-actions/effects/architecture.md).

## 8. Unity Integration (ScriptableObjects)
*Namespace: `PokemonGame.Unity.Data`*

We use **SerializeReference** (Odin Inspector or standard Unity 2021+) to show the list of Effects polymorphically in the Inspector.

### `MoveSO`
-   `Name`, `Power`, `Accuracy`...
-   `[SerializeReference] List<IMoveEffect> Effects;`
    -   In Inspector, you click "+" and choose "DamageEffect", "StatusEffect", etc.

## 9. Testability (The "Ember" Test)
We can test a move without running the game. Current tests verify **composition** (what effects are attached), while future tests will verify **execution** (what actions are generated).

### Current: Composition Tests
```csharp
[Test]
public void Test_Ember_Has_Damage_And_Burn_Effects() {
    var ember = MoveCatalog.Ember;

    Assert.Multiple(() => {
        // Verify effect composition
        Assert.That(ember.HasEffect<DamageEffect>(), Is.True);
        Assert.That(ember.HasEffect<StatusEffect>(), Is.True);
        
        // Verify effect parameters
        var status = ember.GetEffect<StatusEffect>();
        Assert.That(status.EffectType, Is.EqualTo(EffectType.Status));
        Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
        Assert.That(status.ChancePercent, Is.EqualTo(10));
    });
}
```

### Builder Tests
```csharp
[Test]
public void Full_Special_Move_With_Secondary_Effect()
{
    var move = Move.Define("Ice Beam")
        .Type(PokemonType.Ice)
        .Special(90, 100, 10)
        .WithEffects(e => e
            .Damage()
            .MayFreeze(10))
        .Build();

    Assert.Multiple(() =>
    {
        Assert.That(move.Name, Is.EqualTo("Ice Beam"));
        Assert.That(move.Effects, Has.Count.EqualTo(2));
        
        var freezeEffect = move.GetEffect<StatusEffect>();
        Assert.That(freezeEffect.Status, Is.EqualTo(PersistentStatus.Freeze));
        Assert.That(freezeEffect.ChancePercent, Is.EqualTo(10));
    });
}
```

### Future: Execution Tests (When Combat System is Ready)
```csharp
[Test]
public void Test_Ember_Generates_Damage_And_Burn_Actions() {
    var ember = MoveCatalog.Ember;
    var context = new CombatContext(userSlot, targetSlot, field);
    
    var actions = new List<BattleAction>();
    foreach(var eff in ember.Effects) 
        actions.AddRange(eff.GenerateActions(context));

    Assert.IsTrue(actions.Any(a => a is DamageAction));
    Assert.IsTrue(actions.Any(a => a is StatusRollAction));
}
```

---

## Related Documents

- **[Sub-Feature README](README.md)** - Overview of Move Expansion
- **[Parent Feature README](../README.md)** - Overview of Content Expansion
- **[Parent Architecture](../architecture.md)** - Catalog system design
- **[Parent Use Cases](../use_cases.md#uc-002-add-new-move)** - Move addition scenarios
- **[Parent Roadmap](../roadmap.md#phase-32-expand-move-coverage-all-types)** - Expansion phases and goals
- **[Parent Testing](../testing.md)** - Content testing strategy
- **[Parent Code Location](../code_location.md)** - Where Move catalog code lives
- **[Feature 2: Combat System](../../2-combat-system/architecture.md)** - How moves are used in combat
- **[Feature 2.5: Combat Actions - Move Effects Execution](../../2-combat-system/2.5-combat-actions/effects/architecture.md)** - How move effects execute in combat

---

**Last Updated**: 2025-01-XX
