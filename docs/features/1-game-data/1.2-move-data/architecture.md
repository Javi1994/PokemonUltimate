# Sub-Feature 1.2: Move Data - Architecture

> Complete technical specification for move blueprints, runtime instances, and move effects.

**Sub-Feature Number**: 1.2  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the complete data structure for Moves, including:
- **MoveData** (Blueprint): Immutable move data
- **MoveInstance** (Runtime): Mutable instance with PP tracking
- **Move Effects** (22 implementations): Composition-based move behavior

## Design Principles

- **Composition Pattern**: Moves are composed of multiple effects, not subclassed
- **Blueprint/Instance Separation**: Blueprints are immutable, instances are mutable
- **Testability First**: Pure C# classes, no Unity dependencies

---

## 1. MoveData (Blueprint)

**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/MoveData.cs`

Immutable blueprint for a move. Contains all static data about a move.

### Core Properties

```csharp
public class MoveData : IIdentifiable
{
    // Identity
    public string Name { get; set; }              // "Thunderbolt"
    public string Id => Name;                     // IIdentifiable implementation
    
    // Type & Category
    public PokemonType Type { get; set; }         // Electric
    public MoveCategory Category { get; set; }     // Physical, Special, or Status
    
    // Power & Accuracy
    public int? Power { get; set; }               // Base power (null for status moves)
    public int? Accuracy { get; set; }           // Accuracy % (null = never misses)
    
    // PP (Power Points)
    public int MaxPP { get; set; }               // Maximum PP (can be increased with PP Up)
    
    // Priority
    public int Priority { get; set; }            // Priority bracket (-7 to +5)
    
    // Target Scope
    public TargetScope Target { get; set; }       // Who the move targets
    
    // Effects (Composition Pattern)
    public List<IMoveEffect> Effects { get; set; } // List of effects this move has
    
    // Move Flags (15+ boolean flags)
    public bool MakesContact { get; set; }         // Can trigger contact-based abilities/items
    public bool IsSoundBased { get; set; }       // Affected by Soundproof
    public bool NeverMisses { get; set; }        // Ignores accuracy checks
    public bool IsPunchMove { get; set; }        // Affected by Iron Fist
    public bool IsBiteMove { get; set; }         // Affected by Strong Jaw
    public bool IsPulseMove { get; set; }        // Affected by Mega Launcher
    public bool IsBombMove { get; set; }         // Affected by Bulletproof
    public bool IsDanceMove { get; set; }        // Affected by Dancer
    public bool IsPowderMove { get; set; }       // Affected by Overcoat/Leaf Guard
    public bool IsWindMove { get; set; }         // Affected by Wind Rider
    public bool IsRechargeMove { get; set; }     // Requires recharge turn
    public bool IsMultiTurnMove { get; set; }    // Multi-turn move (e.g., Fly)
    public bool IsHealingMove { get; set; }      // Restores HP
    public bool IsSnatchable { get; set; }       // Can be stolen by Snatch
    public bool IsMagicCoatable { get; set; }    // Can be reflected by Magic Coat
    // ... more flags
}
```

### MoveCategory Enum

```csharp
public enum MoveCategory
{
    Physical,  // Uses Attack and Defense stats
    Special,   // Uses SpAttack and SpDefense stats
    Status     // No damage, only effects
}
```

### TargetScope Enum

```csharp
public enum TargetScope
{
    Self,              // User only
    SingleTarget,      // One opponent
    AllOpponents,     // All opponents
    AllPokemon,        // All Pokemon on field
    Ally,              // One ally
    AllAllies,         // All allies
    RandomOpponent,    // Random opponent
    Field,             // Field effect
    // ... more
}
```

---

## 2. MoveInstance (Runtime)

**Namespace**: `PokemonUltimate.Core.Instances`  
**File**: `PokemonUltimate.Core/Instances/MoveInstance.cs`

Mutable runtime instance with PP tracking.

```csharp
public class MoveInstance
{
    public MoveData Move { get; }                 // Reference to blueprint
    public int CurrentPP { get; set; }            // Current PP
    public int MaxPP { get; set; }                // Maximum PP (can be increased)
    public int PPUps { get; set; }                // PP Up count (0-3)
    
    // PP Management
    public void UsePP(int amount = 1);
    public void RestorePP(int amount);
    public void RestoreAllPP();
    public void IncreaseMaxPP(int ppUps);        // Apply PP Ups
    
    // Helpers
    public bool HasPP => CurrentPP > 0;
    public bool IsOutOfPP => CurrentPP <= 0;
}
```

---

## 3. Move Effects (Composition Pattern)

**Namespace**: `PokemonUltimate.Core.Effects`  
**Files**: `PokemonUltimate.Core/Effects/*.cs`

Moves use the **Composition Pattern**: behavior is defined by composing multiple effects, not by subclassing.

### IMoveEffect Interface

```csharp
public interface IMoveEffect
{
    EffectType EffectType { get; }                // Type identifier
    string Description { get; }                    // Human-readable description
}
```

### Effect Classes (22 Implementations)

#### Basic Effects

1. **DamageEffect** - Standard damage using Pokemon damage formula
   - `DamageMultiplier` - Multiplier applied to final damage
   - `CanCrit` - Whether move can score critical hits
   - `CritStages` - Additional crit stages

2. **StatusEffect** - Applies persistent status condition
   - `Status` - Status to apply (Burn, Paralysis, etc.)
   - `ChancePercent` - Chance to apply (0-100)
   - `TargetSelf` - Apply to user instead of target

3. **StatChangeEffect** - Modifies stat stages
   - `Stat` - Stat to modify
   - `Stages` - Number of stages (-6 to +6)
   - `ChancePercent` - Chance to apply
   - `TargetSelf` - Apply to user instead of target

#### Healing Effects

4. **HealEffect** - Restores HP
   - `HealAmount` - Fixed HP amount (or percentage)
   - `IsPercentage` - Whether amount is percentage
   - `TargetSelf` - Heal user instead of target

5. **RecoilEffect** - User takes recoil damage
   - `RecoilPercent` - Percentage of damage dealt as recoil
   - `FixedRecoil` - Fixed recoil amount

6. **DrainEffect** - User drains HP from target
   - `DrainPercent` - Percentage of damage dealt as healing

#### Control Effects

7. **ProtectionEffect** - Protects user from attacks
   - `ProtectionType` - Type of protection (Protect, Detect, etc.)
   - `SuccessChance` - Chance to succeed (decreases with repeated use)

8. **ForceSwitchEffect** - Forces target to switch
   - `SwitchType` - Type of switch (Roar, Whirlwind, etc.)

9. **SwitchAfterAttackEffect** - User switches after attacking
   - `SwitchType` - Type of switch (U-turn, Volt Switch, etc.)

#### Multi-Hit Effects

10. **MultiHitEffect** - Hits multiple times
    - `MinHits` - Minimum number of hits
    - `MaxHits` - Maximum number of hits
    - `IsFixedHits` - Whether hit count is fixed

11. **BindingEffect** - Binds target for multiple turns
    - `Duration` - Number of turns
    - `DamagePerTurn` - Damage dealt each turn

12. **ChargingEffect** - Requires charge turn before attacking
    - `ChargeType` - Type of charge (Solar Beam, Sky Attack, etc.)
    - `CanSkipCharge` - Whether charge can be skipped (e.g., Sunny Day for Solar Beam)

#### Timing Effects

13. **DelayedDamageEffect** - Damage dealt after delay
    - `Turns` - Number of turns until damage
    - `DamagePercent` - Percentage of user's max HP as damage

14. **FieldConditionEffect** - Sets field condition
    - `ConditionType` - Type of condition (Weather, Terrain, etc.)
    - `ConditionData` - Condition data

15. **FlinchEffect** - Causes target to flinch
    - `ChancePercent` - Chance to flinch

#### Special Effects

16. **FixedDamageEffect** - Deals fixed damage
    - `Damage` - Fixed damage amount
    - `IsPercentage` - Whether damage is percentage of target's HP

17. **MoveRestrictionEffect** - Restricts move usage
    - `RestrictionType` - Type of restriction (Encore, Disable, Taunt, etc.)
    - `Duration` - Duration in turns
    - `TargetSelf` - Apply to user instead of target

18. **PriorityModifierEffect** - Modifies move priority
    - `PriorityChange` - Change to priority bracket

19. **RevengeEffect** - Damage increases based on damage taken
    - `Multiplier` - Multiplier based on damage taken

20. **SelfDestructEffect** - User faints after using move
    - `Type` - Type of self-destruct (Explosion, Memento, etc.)
    - `DealsDamage` - Whether move deals damage
    - `HalvesTargetDefense` - Whether target's Defense is halved

21. **VolatileStatusEffect** - Applies volatile status
    - `Status` - Volatile status to apply
    - `ChancePercent` - Chance to apply

22. **Other Effects** - Additional specialized effects as needed

---

## 4. Usage Examples

### Creating a Simple Damage Move

```csharp
var thunderbolt = Move.Define("Thunderbolt")
    .Type(PokemonType.Electric)
    .Category(MoveCategory.Special)
    .Power(90)
    .Accuracy(100)
    .MaxPP(15)
    .Priority(0)
    .Target(TargetScope.SingleTarget)
    .WithEffect(new DamageEffect())
    .Build();
```

### Creating a Move with Multiple Effects

```csharp
var thunderbolt = Move.Define("Thunderbolt")
    .Type(PokemonType.Electric)
    .Category(MoveCategory.Special)
    .Power(90)
    .Accuracy(100)
    .MaxPP(15)
    .WithEffect(new DamageEffect())
    .WithEffect(new StatusEffect(PersistentStatus.Paralysis, chancePercent: 10))
    .Build();
```

### Creating a Status Move

```csharp
var thunderWave = Move.Define("Thunder Wave")
    .Type(PokemonType.Electric)
    .Category(MoveCategory.Status)
    .Accuracy(90)
    .MaxPP(20)
    .WithEffect(new StatusEffect(PersistentStatus.Paralysis, chancePercent: 100))
    .Build();
```

### Creating a Multi-Hit Move

```csharp
var bulletSeed = Move.Define("Bullet Seed")
    .Type(PokemonType.Grass)
    .Category(MoveCategory.Physical)
    .Power(25)
    .Accuracy(100)
    .MaxPP(30)
    .WithEffect(new MultiHitEffect(minHits: 2, maxHits: 5))
    .WithEffect(new DamageEffect())
    .Build();
```

---

## 5. Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Moves referenced in Learnset
- **[1.5: Status Effect Data](../1.5-status-effect-data/)** - Status effects applied by moves
- **[1.6: Weather Data](../1.6-weather-data/)** - Weather conditions set by moves
- **[1.7: Terrain Data](../1.7-terrain-data/)** - Terrain conditions set by moves
- **[1.8: Hazard Data](../1.8-hazard-data/)** - Entry hazards set by moves
- **[1.9: Side Condition Data](../1.9-side-condition-data/)** - Side conditions set by moves
- **[1.10: Field Effect Data](../1.10-field-effect-data/)** - Field effects set by moves
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - MoveBuilder for creating moves

---

## Related Documents

- **[Parent Architecture](../architecture.md#12-move-data)** - Feature-level technical specification
- **[Parent Code Location](../code_location.md#grupo-a-core-entity-data)** - Code organization
- **[Sub-Feature README](README.md)** - Overview and quick navigation

---

**Last Updated**: 2025-01-XX

