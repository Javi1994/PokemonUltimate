# Sub-Feature 2.5: Combat Actions - Move Effects Execution

> How move effects generate BattleActions during combat execution.

**Sub-Feature Number**: 2.5  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

**Purpose**: Document how move effects are processed during combat and what BattleActions they generate.  
**Related**: For effect property definitions and configuration, see [`../../../../3-content-expansion/3.2-move-expansion/architecture.md`](../../../../3-content-expansion/3.2-move-expansion/architecture.md#move-effects).

---

## Table of Contents

1. [Effect Execution Overview](#effect-execution-overview)
2. [UseMoveAction Processing Flow](#usemoveaction-processing-flow)
3. [Effect-to-Action Mapping](#effect-to-action-mapping)
4. [Execution Order](#execution-order)
5. [Effect Processing Patterns](#effect-processing-patterns)
6. [Related Documents](#related-documents)

---

## Effect Execution Overview

Move effects are **data definitions** stored in `MoveData`. During combat, `UseMoveAction` reads these effects and generates `BattleAction` instances that are queued for execution.

### Key Concepts

- **Effects are Data**: Effects (`IMoveEffect`) define WHAT a move does (properties, parameters)
- **Actions are Logic**: `BattleAction` instances define HOW effects execute (state changes, reactions)
- **One-to-Many**: A single effect can generate multiple actions (e.g., `DamageEffect` → `DamageAction` → `FaintAction`)

### Execution Flow

```
MoveData.Effects (data)
    ↓
UseMoveAction.ProcessEffects()
    ↓
BattleAction instances (logic)
    ↓
BattleQueue.Enqueue()
    ↓
BattleAction.ExecuteLogic()
    ↓
Game state updated
```

---

## UseMoveAction Processing Flow

### Current Implementation

`UseMoveAction.ProcessEffects()` processes effects in two passes:

1. **First Pass**: Process damage effects to calculate `damageDealt`
2. **Second Pass**: Process all other effects (recoil, drain, status, etc.)

```csharp
private void ProcessEffects(BattleField field, List<BattleAction> actions)
{
    int damageDealt = 0;

    // First pass: Process damage effect
    foreach (var effect in Move.Effects)
    {
        if (effect is DamageEffect damageEffect)
        {
            var pipeline = new DamagePipeline();
            var context = pipeline.Calculate(User, Target, Move, field);
            
            if (context.FinalDamage > 0)
            {
                actions.Add(new DamageAction(User, Target, context));
                damageDealt = context.FinalDamage;
            }
            break; // Only process first DamageEffect
        }
    }

    // Second pass: Process other effects
    foreach (var effect in Move.Effects)
    {
        if (effect is DamageEffect)
            continue; // Already processed

        // Process based on effect type
        switch (effect)
        {
            case StatusEffect statusEffect:
                // Generate ApplyStatusAction
                break;
            case RecoilEffect recoilEffect:
                // Generate DamageAction (depends on damageDealt)
                break;
            // ... other effects
        }
    }
}
```

### Why Two Passes?

Some effects depend on the damage dealt:
- **RecoilEffect**: Needs `damageDealt` to calculate recoil damage
- **DrainEffect**: Needs `damageDealt` to calculate healing amount

By processing damage first, we ensure these effects have the correct value.

---

## Effect-to-Action Mapping

### Core Damage Effects

#### DamageEffect → DamageAction
- **Input**: `DamageEffect` (properties: `DamageMultiplier`, `CanCrit`, `CritStages`)
- **Process**: Calculate damage using `DamagePipeline`
- **Output**: `DamageAction` with calculated damage
- **Reactions**: May generate `FaintAction` if HP reaches 0

```csharp
// In ProcessEffects()
if (effect is DamageEffect damageEffect)
{
    var pipeline = new DamagePipeline();
    var context = pipeline.Calculate(User, Target, Move, field);
    
    if (context.FinalDamage > 0)
    {
        actions.Add(new DamageAction(User, Target, context));
        // DamageAction may generate FaintAction if HP = 0
    }
}
```

#### FixedDamageEffect → DamageAction
- **Input**: `FixedDamageEffect` (properties: `FixedAmount`, `LevelBased`, `FractionOfHP`)
- **Process**: Calculate fixed damage (ignores stats)
- **Output**: `DamageAction` with fixed damage
- **Reactions**: May generate `FaintAction`

#### RecoilEffect → DamageAction (to user)
- **Input**: `RecoilEffect` (property: `RecoilPercent`)
- **Dependency**: Requires `damageDealt` from previous damage effect
- **Process**: Calculate recoil as percentage of damage dealt
- **Output**: `DamageAction` targeting the user
- **Note**: Always deals at least 1 HP if damage was dealt

```csharp
case RecoilEffect recoilEffect:
    if (damageDealt > 0)
    {
        int recoilDamage = (int)(damageDealt * recoilEffect.RecoilPercent / 100f);
        recoilDamage = Math.Max(1, recoilDamage); // At least 1 HP
        
        var recoilContext = new DamageContext(User, User, Move, field)
        {
            BaseDamage = recoilDamage,
            Multiplier = 1.0f,
            TypeEffectiveness = 1.0f
        };
        actions.Add(new DamageAction(User, User, recoilContext));
    }
    break;
```

#### DrainEffect → HealAction (to user)
- **Input**: `DrainEffect` (property: `DrainPercent`)
- **Dependency**: Requires `damageDealt` from previous damage effect
- **Process**: Calculate healing as percentage of damage dealt
- **Output**: `HealAction` targeting the user
- **Note**: Always heals at least 1 HP if damage was dealt, cannot exceed MaxHP

```csharp
case DrainEffect drainEffect:
    if (damageDealt > 0)
    {
        int drainHealAmount = (int)(damageDealt * drainEffect.DrainPercent / 100f);
        drainHealAmount = Math.Max(1, drainHealAmount); // At least 1 HP
        
        actions.Add(new HealAction(User, User, drainHealAmount));
    }
    break;
```

#### MultiHitEffect → Multiple DamageActions
- **Input**: `MultiHitEffect` (properties: `MinHits`, `MaxHits`, `FixedHits`)
- **Process**: Repeat damage effect multiple times
- **Output**: Multiple `DamageAction` instances (one per hit)
- **Note**: Each hit is independent (can crit, can miss, etc.)

---

### Status Effects

#### StatusEffect → ApplyStatusAction
- **Input**: `StatusEffect` (properties: `Status`, `ChancePercent`, `TargetSelf`)
- **Process**: Roll chance, determine target (self or enemy)
- **Output**: `ApplyStatusAction` if chance succeeds
- **Reactions**: `ApplyStatusAction` may trigger status listeners

```csharp
case StatusEffect statusEffect:
    if (random.Next(100) < statusEffect.ChancePercent)
    {
        var targetSlot = statusEffect.TargetSelf ? User : Target;
        actions.Add(new ApplyStatusAction(User, targetSlot, statusEffect.Status));
    }
    break;
```

#### VolatileStatusEffect → ApplyVolatileAction
- **Input**: `VolatileStatusEffect` (properties: `Status`, `ChancePercent`, `Duration`)
- **Process**: Roll chance, apply volatile status
- **Output**: `ApplyVolatileAction` if chance succeeds
- **Note**: Volatile statuses clear on switch

#### FlinchEffect → AddVolatileStatus (Flinch)
- **Input**: `FlinchEffect` (property: `ChancePercent`)
- **Process**: Roll chance, add Flinch volatile status
- **Output**: Direct state change (no action needed)
- **Note**: Flinch is consumed at start of target's next turn

```csharp
case FlinchEffect flinchEffect:
    if (random.Next(100) < flinchEffect.ChancePercent)
    {
        Target.AddVolatileStatus(VolatileStatus.Flinch);
    }
    break;
```

---

### Stat Modification Effects

#### StatChangeEffect → StatChangeAction
- **Input**: `StatChangeEffect` (properties: `TargetStat`, `Stages`, `ChancePercent`, `TargetSelf`)
- **Process**: Roll chance, determine target, validate stat change
- **Output**: `StatChangeAction` if chance succeeds
- **Reactions**: May trigger `OnStatChangeAttempt` triggers (Clear Body, etc.)

```csharp
case StatChangeEffect statChangeEffect:
    if (random.Next(100) < statChangeEffect.ChancePercent)
    {
        var targetSlot = statChangeEffect.TargetSelf ? User : Target;
        actions.Add(new StatChangeAction(User, targetSlot, 
            statChangeEffect.TargetStat, statChangeEffect.Stages));
    }
    break;
```

---

### Healing Effects

#### HealEffect → HealAction
- **Input**: `HealEffect` (properties: `HealPercent`, `FixedAmount`, `WeatherModified`)
- **Process**: Calculate healing amount (percentage or fixed)
- **Output**: `HealAction` targeting the user
- **Note**: Cannot exceed MaxHP, cannot revive fainted Pokemon

```csharp
case HealEffect healEffect:
    var healAmount = (int)(User.Pokemon.MaxHP * healEffect.HealPercent / 100f);
    actions.Add(new HealAction(User, User, healAmount));
    break;
```

---

## Execution Order

Effects are processed in the order they appear in `Move.Effects`, with one exception:

1. **Damage effects** are processed first (to calculate `damageDealt`)
2. **All other effects** are processed in order

### Example: Flamethrower

```csharp
Move.Effects = [
    new DamageEffect(),
    new StatusEffect(PersistentStatus.Burn, chancePercent: 10)
]
```

**Execution Order**:
1. `DamageEffect` → `DamageAction` (damageDealt = 85)
2. `StatusEffect` → `ApplyStatusAction` (if 10% chance succeeds)

### Example: Close Combat

```csharp
Move.Effects = [
    new DamageEffect(),
    new StatChangeEffect(Stat.Defense, -1, targetSelf: true),
    new StatChangeEffect(Stat.SpDefense, -1, targetSelf: true)
]
```

**Execution Order**:
1. `DamageEffect` → `DamageAction`
2. `StatChangeEffect` (Defense) → `StatChangeAction`
3. `StatChangeEffect` (SpDefense) → `StatChangeAction`

### Example: Double-Edge

```csharp
Move.Effects = [
    new DamageEffect(),
    new RecoilEffect(recoilPercent: 33)
]
```

**Execution Order**:
1. `DamageEffect` → `DamageAction` (damageDealt = 120)
2. `RecoilEffect` → `DamageAction` (user takes 40 damage, based on damageDealt)

---

## Effect Processing Patterns

### Pattern 1: Chance-Based Effects

Many effects have a chance to apply:

```csharp
if (random.Next(100) < effect.ChancePercent)
{
    // Generate action
}
```

**Examples**: `StatusEffect`, `StatChangeEffect`, `FlinchEffect`

### Pattern 2: Damage-Dependent Effects

Some effects depend on damage dealt:

```csharp
// First pass: Calculate damage
int damageDealt = 0;
if (effect is DamageEffect)
{
    damageDealt = CalculateDamage();
}

// Second pass: Use damageDealt
if (effect is RecoilEffect && damageDealt > 0)
{
    ApplyRecoil(damageDealt);
}
```

**Examples**: `RecoilEffect`, `DrainEffect`

### Pattern 3: Target Selection

Effects can target self or enemy:

```csharp
var targetSlot = effect.TargetSelf ? User : Target;
actions.Add(new SomeAction(User, targetSlot, ...));
```

**Examples**: `StatusEffect`, `StatChangeEffect`, `HealEffect`

### Pattern 4: Conditional Execution

Some effects only execute under certain conditions:

```csharp
if (damageDealt > 0) // Recoil/Drain only if damage was dealt
{
    // Generate action
}
```

---

## Related Documents

- **[Sub-Feature README](../README.md)** - Overview of Combat Actions
- **[Sub-Feature Architecture](../architecture.md)** - Complete action system specification
- **[Sub-Feature Use Cases](../use_cases.md)** - All action use cases
- **[Parent Architecture](../../architecture.md)** - Overall combat system design
- **[Effect Property Definitions](../../../../3-content-expansion/3.2-move-expansion/architecture.md#move-effects)** - Effect data structure and properties
- **[Roadmap](../../roadmap.md#phase-25-combat-actions)** - Implementation details
- **[Testing](../../testing.md)** - Testing strategy
- **[Code Location](../../code_location.md)** - Where effects are implemented

---

*Last updated: 2025-01-XX*  
*Focus: Effect execution in combat, not effect data definitions*
