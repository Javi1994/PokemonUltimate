# Battle Mechanics Catalog

## Overview
This document catalogs **all BattleActions, IMoveEffects, and Data Types** needed to support authentic Pokémon combat mechanics. Each entry follows the "Everything is an Action" pattern.

---

## 1. Data Types & Enums

### Core Enums

#### PokemonType
```csharp
public enum PokemonType {
    Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison,
    Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy
}
// 18 types total
```

#### Stat
```csharp
public enum Stat {
    HP, Attack, Defense, SpAttack, SpDefense, Speed,
    Accuracy,  // For evasion/accuracy modifications
    Evasion
}
```

#### MoveCategory
```csharp
public enum MoveCategory {
    Physical,   // Uses Attack vs Defense
    Special,    // Uses SpAttack vs SpDefense
    Status      // No damage, only effects
}
```

#### TargetScope
```csharp
public enum TargetScope {
    Self,              // Swords Dance, Recover
    SingleEnemy,       // Tackle, Thunderbolt
    SingleAlly,        // Helping Hand
    AllEnemies,        // Earthquake, Surf
    AllAllies,         // Not common
    AllOthers,         // Explosion (hits everyone except user)
    Any,               // Telekinesis (can target anyone)
    RandomEnemy,       // Outrage, Petal Dance
    Field,             // Stealth Rock, Weather moves
    UserAndAllies      // Aromatherapy, Heal Bell
}
```

### Status Enums

#### PersistentStatus
```csharp
public enum PersistentStatus {
    None = 0, Burn = 1, Paralysis = 2, Sleep = 3,
    Poison = 4, BadlyPoisoned = 5, Freeze = 6
}
// Only one can be active at a time
```

#### VolatileStatus (Flags)
```csharp
[Flags]
public enum VolatileStatus {
    None = 0, Confusion = 1, Flinch = 2, LeechSeed = 4, Attract = 8,
    Curse = 16, Encore = 32, Taunt = 64, Torment = 128, Disable = 256,
    IsSemiInvulnerable = 512,  // Fly, Dig
    IsCharging = 1024           // Solar Beam, Skull Bash
}
// Multiple can be active simultaneously
```

#### SideStatus (Flags)
```csharp
[Flags]
public enum SideStatus {
    None = 0, Reflect = 1, LightScreen = 2, Mist = 4,
    Safeguard = 8, Tailwind = 16, AuroraVeil = 32
}
```

### Field Effect Enums

#### WeatherType
```csharp
public enum WeatherType {
    None, Rain, Sun, Sandstorm, Hail, HarshSunlight, HeavyRain
}
```

#### TerrainType
```csharp
public enum TerrainType {
    None, Electric, Grassy, Psychic, Misty
}
```

#### FieldEffect (Flags)
```csharp
[Flags]
public enum FieldEffect {
    None = 0, TrickRoom = 1, WonderRoom = 2, MagicRoom = 4,
    Gravity = 8, IonDeluge = 16
}
```

#### HazardType
```csharp
public enum HazardType {
    Spikes, ToxicSpikes, StealthRock, StickyWeb
}
```

### Battle Event Enums

#### BattleTrigger
```csharp
public enum BattleTrigger {
    OnSwitchIn, OnBeforeMove, OnAfterMove, OnDamageTaken,
    OnTurnEnd, OnWeatherChange, OnFaint, OnHit, OnMiss
}
```

---

## 2. Core BattleActions

### Damage & HP
| Action | Description | Example |
|--------|-------------|---------|
| `DamageAction` | Standard damage application | All damaging moves |
| `FixedDamageAction` | Fixed amount (ignores stats) | Seismic Toss, Dragon Rage |
| `PercentDamageAction` | % of target's max HP | Super Fang (50%) |
| `HealAction` | Restore HP | Synthesis, Moonlight |
| `RecoilDamageAction` | User takes % of damage dealt | Double-Edge, Brave Bird |
| `DrainAction` | Damage + heal user | Giga Drain, Drain Punch |

### Status & Stats
| Action | Description | Example |
|--------|-------------|---------|
| `ApplyStatusAction` | Apply major status | Burn, Paralysis, Sleep |
| `CureStatusAction` | Remove status | Heal Bell, Aromatherapy |
| `StatChangeAction` | Modify stat stages | Swords Dance (+2 Atk) |
| `StatResetAction` | Reset all stat changes | Haze, Clear Smog |
| `StatSwapAction` | Swap stat stages | Psych Up, Guard Swap |
| `StatCopyAction` | Copy target's stat stages | Psych Up |

### Field & Environment
| Action | Description | Example |
|--------|-------------|---------|
| `SetWeatherAction` | Change weather | Rain Dance, Sunny Day |
| `SetTerrainAction` | Change terrain | Electric Terrain |
| `SetFieldEffectAction` | Room effects | Trick Room, Wonder Room |
| `SetHazardAction` | Entry hazards | Stealth Rock, Spikes |
| `RemoveHazardAction` | Clear hazards | Rapid Spin, Defog |
| `SetScreenAction` | Defensive screens | Reflect, Light Screen |

### Control & Utility
| Action | Description | Example |
|--------|-------------|---------|
| `FlinchAction` | Apply flinch status | Fake Out, Air Slash |
| `ConfuseAction` | Apply confusion | Confuse Ray, Swagger |
| `TrapAction` | Prevent switching | Wrap, Fire Spin |
| `ForceSwithAction` | Force target to switch | Roar, Whirlwind |
| `ProtectAction` | Block next move | Protect, Detect |
| `DisableAction` | Disable last used move | Disable |
| `TauntAction` | Can only use damaging moves | Taunt |

### Special Mechanics
| Action | Description | Example |
|--------|-------------|---------|
| `FaintAction` | Mark as fainted, trigger death events | HP reaches 0 |
| `ReviveAction` | Restore fainted Pokémon | Revive item |
| `TransformAction` | Copy target's stats/moves | Transform (Ditto) |
| `SubstituteAction` | Create HP-based decoy | Substitute |
| `BatonPassAction` | Switch + keep stat changes | Baton Pass |
| `WishAction` | Delayed heal (2 turns) | Wish |
| `FutureSightAction` | Delayed damage (2 turns) | Future Sight |

### Presentation Actions (UI & Audio)
| Action | Description | Example |
|--------|-------------|---------|
| `MessageAction` | Show text in battle log/dialog | "Pikachu used Thunderbolt!" |
| `PlayAnimationAction` | Trigger visual FX | Move animations, Faint anim |
| `PlaySoundAction` | Trigger SFX/Cry | "sfx_hit", "cry_bulbasaur" |

---

## 3. IMoveEffect Catalog

### Damage Effects
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `DamageEffect` | - | Standard damage formula |
| `FixedDamageEffect` | `int Amount` | Fixed damage (20, 40, 80) |
| `LevelDamageEffect` | - | Damage = User's Level |
| `PercentDamageEffect` | `float Percent` | % of target's HP (0.5 = 50%) |
| `MultiHitEffect` | `int MinHits, int MaxHits` | Hit 2-5 times |
| `CriticalRatioEffect` | `int Stages` | Increased crit chance |
| `HighCritEffect` | - | Always high crit ratio |
| `OneHitKOEffect` | - | OHKO if hits (Fissure, Guillotine) |

### HP Manipulation
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `RecoilEffect` | `float Percent` | User takes % of damage dealt |
| `DrainEffect` | `float Percent` | User heals % of damage dealt |
| `HealEffect` | `float Percent` | Heal % of max HP |
| `SacrificialEffect` | - | User faints (Explosion, Self-Destruct) |
| `PainSplitEffect` | - | Average HP of user and target |

### Status Application
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `BurnEffect` | `float Chance` | Apply Burn |
| `ParalyzeEffect` | `float Chance` | Apply Paralysis |
| `PoisonEffect` | `float Chance, bool BadlyPoisoned` | Apply Poison/Badly Poisoned |
| `SleepEffect` | `float Chance` | Apply Sleep |
| `FreezeEffect` | `float Chance` | Apply Freeze |
| `ConfuseEffect` | `float Chance` | Apply Confusion |
| `FlinchEffect` | `float Chance` | Apply Flinch (only if slower) |

### Stat Modification
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `StatChangeEffect` | `Stat, int Stages, bool Self` | Raise/lower stat |
| `MultiStatChangeEffect` | `Dictionary<Stat, int>` | Multiple stats at once |
| `SwaggerEffect` | - | Confuse + raise target's Attack |
| `FlatterEffect` | - | Confuse + raise target's Sp.Atk |

### Field Effects
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `WeatherEffect` | `WeatherType, int Duration` | Set weather |
| `TerrainEffect` | `TerrainType, int Duration` | Set terrain |
| `HazardEffect` | `HazardType, int Layers` | Set entry hazard |
| `ScreenEffect` | `ScreenType, int Duration` | Set defensive screen |
| `TrickRoomEffect` | `int Duration` | Reverse speed order |

### Special Mechanics
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `ChargeEffect` | `bool Invulnerable, string Message` | 2-turn moves (Fly, Solar Beam) |
| `ProtectEffect` | - | Block incoming move |
| `CounterEffect` | `bool Physical` | Return 2x damage (Counter, Mirror Coat) |
| `BindEffect` | `int Turns` | Trap + damage (Wrap, Fire Spin) |
| `LeechSeedEffect` | - | Drain 1/8 HP each turn |
| `CurseEffect` | - | Ghost type: Sacrifice HP, curse target |
| `DestinyBondEffect` | - | If user faints, faint attacker too |
| `GrudgeEffect` | - | If user faints, remove attacker's PP |

### Utility Effects
| Effect | Parameters | Description |
|--------|-----------|-------------|
| `HealingWishEffect` | - | User faints, next Pokémon fully healed |
| `WishEffect` | - | Heal next turn |
| `SubstituteEffect` | - | Create decoy with 25% HP |
| `TransformEffect` | - | Copy target |
| `MetronomeEffect` | - | Random move |
| `MirrorMoveEffect` | - | Copy last move used by target |

---

## 4. Conditional Effects (Require Context)

### Damage Modifiers
| Effect | Condition | Multiplier |
|--------|-----------|------------|
| `LowHPBoostEffect` | User HP < 33% | 1.5x (Reversal, Flail) |
| `WeatherBoostEffect` | Specific weather active | 1.5x (Thunder in Rain) |
| `TerrainBoostEffect` | Specific terrain active | 1.3x (Electric moves in E.Terrain) |
| `StatusPunishEffect` | Target has status | 2x damage (Hex, Venoshock) |
| `WeightBasedEffect` | Based on target weight | Variable (Low Kick, Grass Knot) |

### Accuracy Modifiers
| Effect | Condition | Accuracy |
|--------|-----------|----------|
| `NoMissInWeatherEffect` | Specific weather | Always hit (Thunder in Rain) |
| `BypassAccuracyEffect` | - | Ignore accuracy/evasion (Aerial Ace) |
| `MinimizeBypassEffect` | Target used Minimize | Always hit + 2x damage |

---

## 5. Implementation Priority

### Phase 1 (MVP)
- ✅ DamageAction, HealAction, FaintAction
- ✅ ApplyStatusAction, StatChangeAction
- ✅ Basic IMoveEffects (Damage, Status, StatChange)

### Phase 2 (Core Mechanics)
- [ ] RecoilEffect, DrainEffect, MultiHitEffect
- [ ] FlinchEffect, ConfuseEffect
- [ ] WeatherEffect, HazardEffect

### Phase 3 (Advanced)
- [ ] ChargeEffect (2-turn moves)
- [ ] ProtectEffect, SubstituteEffect
- [ ] TransformEffect, MetronomeEffect

### Phase 4 (Complex)
- [ ] BatonPassAction, WishEffect
- [ ] CounterEffect, DestinyBondEffect
- [ ] Field effects (Trick Room, Terrain)
