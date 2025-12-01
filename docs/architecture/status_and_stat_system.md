# Status & Stat Management System Specification

## 1. Overview
This system manages all **status conditions** and **stat modifications** in battle. It handles:
- **Persistent Status** (Burn, Paralysis, Sleep, Poison, Freeze)
- **Volatile Status** (Confusion, Flinch, Leech Seed)
- **Stat Stages** (-6 to +6 modifications)
- **Side Status** (Reflect, Light Screen, Spikes)

All effects integrate with the **Action Queue** and **Event Trigger** systems.

## 1.1 Current Implementation Status ✅

| Component | Status | Location |
|-----------|--------|----------|
| `PersistentStatus` enum | ✅ Complete | `Core/Enums/PersistentStatus.cs` |
| `VolatileStatus` enum | ✅ Complete | `Core/Enums/VolatileStatus.cs` |
| `StatusEffectData` | ✅ Complete | `Core/Blueprints/StatusEffectData.cs` |
| `StatusEffectBuilder` | ✅ Complete | `Core/Builders/StatusEffectBuilder.cs` |
| `StatusCatalog` | ✅ 15 statuses | `Content/Catalogs/Status/StatusCatalog.cs` |
| `BattleSlot.StatStages` | ✅ Complete | `Combat/BattleSlot.cs` |

**Runtime processors** (damage per turn, action prevention) are pending for Phase 2.5+.

## 1.2 StatusEffectData Blueprint (NEW ✅)

The `StatusEffectData` class provides **declarative definitions** for all status effects:

```csharp
public sealed class StatusEffectData {
    // Identity
    public string Id { get; }
    public string Name { get; }
    public PersistentStatus PersistentStatus { get; }
    public VolatileStatus VolatileStatus { get; }
    
    // Duration
    public int MinTurns { get; }
    public int MaxTurns { get; }
    public bool IsIndefinite => MinTurns == 0 && MaxTurns == 0;
    
    // End of Turn Effects
    public float EndOfTurnDamage { get; }      // 1/16 = 0.0625
    public bool DamageEscalates { get; }       // Toxic
    public bool DrainsToOpponent { get; }      // Leech Seed
    
    // Action Prevention
    public float MoveFailChance { get; }       // 0.25 for Paralysis
    public bool PreventsAction => MoveFailChance >= 1.0f;
    public float RecoveryChancePerTurn { get; }  // 0.20 for Freeze
    
    // Stat Modifiers
    public float SpeedMultiplier { get; }      // 0.5 for Paralysis
    public float AttackMultiplier { get; }     // 0.5 for Burn (physical only)
    
    // Self-Damage
    public float SelfHitChance { get; }        // 0.33 for Confusion
    public int SelfHitPower { get; }           // 40 for Confusion
    
    // Type Interactions
    public PokemonType[] ImmuneTypes { get; }
    public PokemonType[] CuredByMoveTypes { get; }
    
    // Helper Methods
    public bool IsTypeImmune(PokemonType type);
    public int GetRandomDuration(Random random);
    public float GetEscalatingDamage(int turnCount);
}
```

### StatusEffectBuilder (Fluent API)
```csharp
public static readonly StatusEffectData Burn = Status.Define("Burn")
    .Description("Takes damage each turn, halves physical Attack.")
    .Persistent(PersistentStatus.Burn)
    .Indefinite()
    .DealsDamagePerTurn(1f / 16f)
    .HalvesPhysicalAttack()
    .ImmuneTypes(PokemonType.Fire)
    .Build();

public static readonly StatusEffectData BadlyPoisoned = Status.Define("Badly Poisoned")
    .Persistent(PersistentStatus.BadlyPoisoned)
    .Indefinite()
    .DealsEscalatingDamage(1f / 16f, 1)  // 1/16, 2/16, 3/16...
    .ImmuneTypes(PokemonType.Poison, PokemonType.Steel)
    .Build();

public static readonly StatusEffectData Confusion = Status.Define("Confusion")
    .Volatile(VolatileStatus.Confusion)
    .LastsTurns(2, 5)
    .SelfHitChance(0.33f, 40)
    .Build();
```

### StatusCatalog (15 Statuses Defined)

**Persistent (6):**
| Status | Damage/Turn | Stat Effect | Immunities |
|--------|-------------|-------------|------------|
| Burn | 1/16 HP | -50% Atk (phys) | Fire |
| Paralysis | - | 25% fail, -50% Speed | Electric |
| Sleep | - | Cannot act (1-3 turns) | - |
| Poison | 1/8 HP | - | Poison, Steel |
| BadlyPoisoned | Escalating | - | Poison, Steel |
| Freeze | - | Cannot act, 20% thaw | Ice |

**Volatile (9):**
| Status | Duration | Effect |
|--------|----------|--------|
| Confusion | 2-5 turns | 33% self-hit |
| Attract | Indefinite | 50% fail |
| Flinch | 1 turn | Cannot act |
| LeechSeed | Indefinite | Drains 1/8 HP |
| Curse | Indefinite | 1/4 HP/turn |
| Encore | 3 turns | Locks move |
| Taunt | 3 turns | No Status moves |
| Torment | Indefinite | No repeat moves |
| Disable | 4 turns | Blocks one move |

### Usage in Combat
```csharp
// Get status data from catalog
var burnData = StatusCatalog.GetByStatus(PersistentStatus.Burn);

// Check immunity before applying
if (!burnData.IsTypeImmune(target.Species.PrimaryType)) {
    target.Status = PersistentStatus.Burn;
}

// Calculate end-of-turn damage
float damagePercent = burnData.EndOfTurnDamage; // 0.0625
int damage = (int)(target.MaxHP * damagePercent);

// For Toxic, escalating damage
var toxicData = StatusCatalog.BadlyPoisoned;
float toxicDamage = toxicData.GetEscalatingDamage(turnCount);
```

## 2. Persistent Status (Major Conditions)
*Namespace: `PokemonUltimate.Core.Enums`*

### Data Structure
```csharp
public enum PersistentStatus {
    None = 0,
    Burn = 1,
    Paralysis = 2,
    Sleep = 3,
    Poison = 4,
    BadlyPoisoned = 5,
    Freeze = 6
}

public class PokemonInstance {
    public PersistentStatus Status { get; set; }
    public int StatusTurnCounter { get; set; } // For Sleep duration, Toxic accumulation
}
```

### Status Effects

| Status | On Apply | Each Turn | Battle Impact |
|--------|----------|-----------|---------------|
| **Burn** | - | Lose 1/16 HP | Attack halved (for Physical moves) |
| **Paralysis** | - | 25% chance to skip turn | Speed halved |
| **Sleep** | Random 1-3 turns | Skip turn, decrement counter | Cannot act |
| **Poison** | - | Lose 1/8 HP | - |
| **Badly Poisoned** | Counter = 1 | Lose (1/16 * counter) HP, counter++ | - |
| **Freeze** | - | 20% chance to thaw | Cannot act |

### Application Service
```csharp
public static class StatusApplicationService {
    public static bool CanApply(PokemonInstance target, PersistentStatus status) {
        // 1. Already has status?
        if (target.Status != PersistentStatus.None) return false;

        // 2. Type Immunities
        if (status == PersistentStatus.Burn && target.HasType(PokemonType.Fire)) return false;
        if (status == PersistentStatus.Freeze && target.HasType(PokemonType.Ice)) return false;
        if (status == PersistentStatus.Poison && target.HasType(PokemonType.Poison)) return false;
        if (status == PersistentStatus.Poison && target.HasType(PokemonType.Steel)) return false;
        if (status == PersistentStatus.Paralysis && target.HasType(PokemonType.Electric)) return false;

        // 3. Ability Immunities (e.g., Water Veil prevents Burn)
        // if (target.Ability.Prevents(status)) return false;

        return true;
    }
}
```

### Status Actions
```csharp
public class ApplyStatusAction : BattleAction {
    private BattleSlot _target;
    private PersistentStatus _status;
    
    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        if (!StatusApplicationService.CanApply(_target.Pokemon, _status)) {
            yield return new MessageAction($"It doesn't affect {_target.Pokemon.Name}!");
            yield break;
        }
        
        _target.Pokemon.Status = _status;
        
        // Set duration for Sleep
        if (_status == PersistentStatus.Sleep) {
            _target.Pokemon.StatusTurnCounter = Random.Range(1, 4); // 1-3 turns
        }
        
        yield return new MessageAction($"{_target.Pokemon.Name} was {GetStatusName(_status)}!");
    }
    
    public override Task ExecuteVisual(IBattleView view) {
        return view.ShowStatusIcon(_target, _status);
    }
}
```

### Turn Processing (Event Trigger)
```csharp
// In CombatEngine, at end of turn
await ProcessTriggers(BattleTrigger.OnTurnEnd);

// Status damage is handled automatically
public class BurnStatusListener : IBattleListener {
    public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field) {
        if (trigger == BattleTrigger.OnTurnEnd && holder.Pokemon.Status == PersistentStatus.Burn) {
            int damage = holder.Pokemon.Stats[Stat.HP] / 16;
            yield return new MessageAction($"{holder.Pokemon.Name} is hurt by its burn!");
            yield return new DamageAction(holder, damage);
        }
    }
}
```

## 3. Volatile Status (Temporary)
*Reset when Pokemon switches out or battle ends.*

```csharp
[Flags]
public enum VolatileStatus {
    None = 0,
    Confusion = 1,
    Flinch = 2,
    LeechSeed = 4,
    Attract = 8,
    Curse = 16,
    Encore = 32
}

public class PokemonInstance {
    public VolatileStatus VolatileStatus { get; set; }
    public Dictionary<string, int> VolatileCounters { get; set; } // Confusion turns remaining
}
```

### Confusion Mechanic
```csharp
// In UseMoveAction.ExecuteLogic()
if (_user.Pokemon.VolatileStatus.HasFlag(VolatileStatus.Confusion)) {
    // 50% chance to hurt self
    if (Random.value < 0.5f) {
        int selfDamage = CalculateConfusionDamage(_user);
        yield return new MessageAction($"{_user.Pokemon.Name} is confused!");
        yield return new DamageAction(_user, selfDamage);
        yield break; // Move fails
    }
    
    // Decrement counter, remove if expired
    _user.Pokemon.VolatileCounters["Confusion"]--;
    if (_user.Pokemon.VolatileCounters["Confusion"] <= 0) {
        _user.Pokemon.VolatileStatus &= ~VolatileStatus.Confusion;
        yield return new MessageAction($"{_user.Pokemon.Name} snapped out of confusion!");
    }
}
```

## 4. Stat Stages System

### Data Structure
```csharp
public class PokemonInstance {
    // Each stat has a stage from -6 to +6
    public Dictionary<Stat, int> StatStages { get; set; } = new Dictionary<Stat, int> {
        { Stat.Attack, 0 },
        { Stat.Defense, 0 },
        { Stat.SpAttack, 0 },
        { Stat.SpDefense, 0 },
        { Stat.Speed, 0 },
        { Stat.Evasion, 0 },
        { Stat.Accuracy, 0 }
    };
}
```

### Stage Multipliers
```csharp
public static class StatStageCalculator {
    public static float GetMultiplier(int stage) {
        // Clamp to -6 to +6
        stage = Mathf.Clamp(stage, -6, 6);
        
        // Formula: (2 + max(0, stage)) / (2 + max(0, -stage))
        if (stage >= 0) {
            return (2f + stage) / 2f;
        } else {
            return 2f / (2f - stage);
        }
    }
}

// Stage | Multiplier
// +6    | 4.0x
// +2    | 2.0x
// +1    | 1.5x
//  0    | 1.0x
// -1    | 0.66x
// -2    | 0.5x
// -6    | 0.25x
```

### Stat Change Actions
```csharp
public class StatChangeAction : BattleAction {
    private BattleSlot _target;
    private Stat _stat;
    private int _stages; // e.g., +2 for Swords Dance
    
    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        int current = _target.Pokemon.StatStages[_stat];
        int newStage = Mathf.Clamp(current + _stages, -6, 6);
        int actualChange = newStage - current;
        
        if (actualChange == 0) {
            yield return new MessageAction($"{_target.Pokemon.Name}'s {_stat} won't go any higher!");
            yield break;
        }
        
        _target.Pokemon.StatStages[_stat] = newStage;
        
        string magnitude = actualChange == 1 ? "" : " sharply";
        string direction = actualChange > 0 ? "rose" : "fell";
        yield return new MessageAction($"{_target.Pokemon.Name}'s {_stat}{magnitude} {direction}!");
    }
    
    public override Task ExecuteVisual(IBattleView view) {
        return view.ShowStatChangeEffect(_target, _stat, _stages);
    }
}
```

### Reset on Switch
```csharp
// In SwitchAction.ExecuteLogic()
if (_slot.IsOccupied) {
    // Reset stat stages
    foreach (var stat in _slot.Pokemon.StatStages.Keys.ToList()) {
        _slot.Pokemon.StatStages[stat] = 0;
    }
    
    // Reset volatile status
    _slot.Pokemon.VolatileStatus = VolatileStatus.None;
}
```

## 5. Side Status (Field Hazards & Screens)
*Stored in `BattleSide`, affects all Pokemon on that side.*

```csharp
[Flags]
public enum SideStatus {
    None = 0,
    Reflect = 1,      // Physical damage halved
    LightScreen = 2,  // Special damage halved
    Mist = 4,         // Stat drops prevented
    Safeguard = 8,    // Status conditions prevented
    Tailwind = 16     // Speed doubled
}

public class BattleSide {
    public SideStatus SideStatus { get; set; }
    public int SpikesLayers { get; set; } // 0-3
    public int ToxicSpikesLayers { get; set; } // 0-2
    public bool StealthRockActive { get; set; }
}
```

### Entry Hazards
```csharp
// In SwitchAction.ExecuteLogic()
// After placing new Pokemon in slot

var actions = new List<BattleAction>();

// Spikes
if (_slot.Side.SpikesLayers > 0) {
    float damagePercent = _slot.Side.SpikesLayers * 0.125f; // 12.5% per layer
    int damage = (int)(_newPokemon.Stats[Stat.HP] * damagePercent);
    actions.Add(new MessageAction($"{_newPokemon.Name} is hurt by spikes!"));
    actions.Add(new DamageAction(_slot, damage));
}

// Stealth Rock (type-dependent)
if (_slot.Side.StealthRockActive) {
    float effectiveness = TypeChart.GetCombinedEffectiveness(PokemonType.Rock, _newPokemon.Types);
    int damage = (int)(_newPokemon.Stats[Stat.HP] * 0.125f * effectiveness);
    actions.Add(new DamageAction(_slot, damage));
}

return actions;
```

## 6. Integration Examples

### Using in Damage Calculation
```csharp
// In StatRatioStep.Process()
float atk = ctx.Attacker.Pokemon.Stats[Stat.Attack];

// Apply stat stage
int stage = ctx.Attacker.Pokemon.StatStages[Stat.Attack];
atk *= StatStageCalculator.GetMultiplier(stage);

// Apply Burn penalty (if physical move)
if (ctx.Move.Category == MoveCategory.Physical && 
    ctx.Attacker.Pokemon.Status == PersistentStatus.Burn) {
    atk *= 0.5f;
}
```

### Testability
```csharp
[Test]
public void Test_Burn_Halves_Physical_Attack() {
    var attacker = CreatePokemon(attack: 100);
    attacker.Status = PersistentStatus.Burn;
    
    var move = new MoveData { Category = MoveCategory.Physical, Power = 50 };
    var damage = DamagePipeline.Calculate(attacker, defender, move);
    
    // Burn should halve the attack stat
    Assert.IsTrue(damage.FinalDamage < expectedUnburnedDamage / 2);
}
```
