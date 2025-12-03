# Feature 2: Combat System - Use Cases

> Comprehensive list of ALL Pokemon battle mechanics.

**Feature Number**: 2  
**Feature Name**: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

**Purpose**: Validate implementation against these cases before completing each phase.

---

## Table of Contents

1. [Battle Formats](#1-battle-formats)
2. [Turn Flow](#2-turn-flow)
3. [Move Execution](#3-move-execution)
4. [Damage Calculation](#4-damage-calculation)
5. [Status Conditions](#5-status-conditions)
6. [Stat Modifications](#6-stat-modifications)
7. [Switching](#7-switching)
8. [Entry Effects](#8-entry-effects)
9. [Field Effects](#9-field-effects)
10. [Targeting](#10-targeting)
11. [Special Move Mechanics](#11-special-move-mechanics)
12. [Victory/Defeat](#12-victorydefeat)
13. [Abilities & Items](#13-abilities--items)

---

## 1. Battle Formats

### 1.1 Singles (1v1)
- [ ] One Pokemon per side active
- [ ] Standard targeting (only one enemy)
- [ ] No ally targeting

### 1.2 Doubles (2v2)
- [ ] Two Pokemon per side active
- [ ] Can target ally (Heal Pulse, Helping Hand)
- [ ] Can target any enemy
- [ ] Spread moves hit multiple targets (reduced damage 0.75x)
- [ ] Ally protection (Wide Guard, Quick Guard)

### 1.3 Triples (3v3)
- [ ] Three Pokemon per side active
- [ ] Adjacency matters (center can hit all, sides limited)
- [ ] Position swapping (Shift command)

### 1.4 Horde (1v5)
- [ ] Player has 1, enemy has 5
- [ ] All enemies share level penalty
- [ ] Spread moves hit all 5

### 1.5 Rotation (3v3 rotation)
- [ ] Three Pokemon, one active at a time
- [ ] Can rotate without using turn
- [ ] Hidden rotation adds mind games

### 1.6 Multi Battle (2v2 with 2 trainers per side)
- [ ] Each trainer controls 1 Pokemon
- [ ] Limited communication between partners

---

## 2. Turn Flow

### 2.1 Action Selection Phase
- [ ] All participants select actions simultaneously
- [ ] Options: Fight, Switch, Item, Run
- [ ] Mega Evolution choice (before move)
- [ ] Z-Move choice (before move)
- [ ] Dynamax choice (before move)

### 2.2 Turn Order Resolution
- [ ] **Priority brackets** (-7 to +5)
  - +5: Helping Hand
  - +4: Protect, Detect, Endure
  - +3: Fake Out, Quick Guard, Wide Guard
  - +2: Extreme Speed, Feint
  - +1: Aqua Jet, Mach Punch, Quick Attack, Shadow Sneak, Sucker Punch
  - 0: Most moves
  - -1: Vital Throw
  - -3: Focus Punch
  - -5: Avalanche, Revenge
  - -6: Counter, Mirror Coat, Roar, Whirlwind
  - -7: Trick Room
- [ ] **Within same priority**: Speed determines order
- [ ] **Speed ties**: Random 50/50
- [ ] **Switching always first** (priority +6 implicitly)
- [ ] **Items before moves** (player choice in some games)

### 2.3 Speed Modifiers
- [ ] Base Speed stat
- [ ] IV/EV contribution
- [ ] Nature modifier (±10%)
- [ ] Stat stages (-6 to +6)
- [ ] Paralysis (0.5x speed)
- [ ] Choice Scarf (1.5x speed)
- [ ] Tailwind (2x speed for side)
- [ ] Trick Room (reverse order)
- [ ] Abilities (Swift Swim, Chlorophyll, Sand Rush, etc.)

### 2.4 Action Execution
- [ ] Execute in order determined above
- [ ] Check if user can still act (not fainted, not flinched)
- [ ] Check if target is still valid
- [ ] Re-target if original target fainted (Singles: fail, Doubles: redirect)

### 2.5 End of Turn Phase
- [ ] Weather damage (Sandstorm, Hail)
- [x] Status damage (Burn, Poison, Badly Poisoned) ✅ Phase 2.8
- [ ] Binding damage (Wrap, Fire Spin)
- [ ] Leech Seed drain
- [ ] Ingrain heal
- [ ] Leftovers/Black Sludge
- [ ] Wish heal
- [ ] Future Sight/Doom Desire damage
- [ ] Perish Song countdown
- [ ] Disable/Encore/Taunt duration
- [ ] Weather/Terrain duration
- [ ] Screen duration (Reflect, Light Screen)

---

## 3. Move Execution

### 3.1 Pre-Move Checks
- [ ] **PP check**: Move fails if 0 PP
- [ ] **Status check**: Sleep, Freeze prevent action
- [ ] **Paralysis check**: 25% chance to fail
- [ ] **Confusion check**: 33% chance to self-damage
- [ ] **Infatuation check**: 50% chance to fail
- [ ] **Flinch check**: Move fails if flinched
- [ ] **Taunt check**: Status moves fail
- [ ] **Disable check**: Specific move fails
- [ ] **Encore check**: Forced to use same move
- [ ] **Choice lock**: Must use same move (Choice items)

### 3.2 Accuracy Check
- [ ] Base accuracy from move data
- [ ] Accuracy stages (+6 to -6)
- [ ] Evasion stages (+6 to -6)
- [ ] Formula: `BaseAccuracy * (AccStages / EvasionStages)`
- [ ] Always-hit moves (Aerial Ace, Swift, Aura Sphere)
- [ ] One-hit KO accuracy (30%, fails if target faster/higher level)
- [ ] Semi-invulnerable state dodges (Fly, Dig, Dive)

### 3.3 Critical Hits
- [ ] Base rate: 1/24 (Gen 6+), 1/16 (Gen 2-5), 1/256 (Gen 1)
- [ ] High crit moves: +1 stage (1/8)
- [ ] Focus Energy: +2 stages (1/2)
- [ ] Crit ignores negative stat stages on attacker
- [ ] Crit ignores positive stat stages on defender
- [ ] Crit ignores Reflect/Light Screen
- [ ] 1.5x damage multiplier (Gen 6+)

### 3.4 Move Categories
- [ ] **Physical**: Uses Attack vs Defense
- [ ] **Special**: Uses SpAttack vs SpDefense
- [ ] **Status**: No damage, applies effect

### 3.5 Multi-Hit Moves
- [ ] 2 hits: Double Slap base (2 guaranteed)
- [ ] 2-5 hits: 35% for 2, 35% for 3, 15% for 4, 15% for 5
- [ ] Fixed hits: Triple Kick (3), Population Bomb (1-10)
- [ ] Each hit can crit independently
- [ ] Multi-hit breaks Substitute then continues

### 3.6 Multi-Turn Moves
- [ ] **Charging**: Solar Beam, Skull Bash (charge turn, attack turn)
- [ ] **Recharging**: Hyper Beam, Giga Impact (attack turn, recharge turn)
- [ ] **Continuous**: Outrage, Petal Dance (2-3 turns, then confusion)
- [ ] **Binding**: Wrap, Fire Spin (2-5 turns damage)

---

## 4. Damage Calculation

### 4.1 Base Damage Formula
```
Damage = ((2*Level/5+2) * Power * A/D) / 50 + 2
```
Where:
- Level = Attacker's level
- Power = Move's base power
- A = Attack or SpAttack (modified)
- D = Defense or SpDefense (modified)

### 4.2 Damage Modifiers (In Order)
1. **Targets**: 0.75x if hitting multiple targets
2. **Weather**: 1.5x/0.5x for boosted/weakened types
3. **Critical Hit**: 1.5x (Gen 6+)
4. **Random**: 0.85 to 1.00
5. **STAB**: 1.5x (or 2.0x with Adaptability)
6. **Type Effectiveness**: 0x, 0.25x, 0.5x, 1x, 2x, 4x
7. **Burn**: 0.5x for physical moves
8. **Other**: Screens, abilities, items

### 4.3 Type Effectiveness Special Cases
- [ ] Immunity overrides weakness (Ground immune to Electric, even if dual-type)
- [ ] Abilities grant immunity (Levitate, Flash Fire, etc.)
- [ ] Items affect type chart (Ring Target removes immunity)
- [ ] Moves pierce immunity (Thousand Arrows hits Flying)
- [ ] Freeze-Dry is super effective vs Water

### 4.4 Fixed Damage
- [x] Dragon Rage: Always 40 damage ✅
- [ ] Sonic Boom: Always 20 damage
- [ ] Seismic Toss: Damage = User's level
- [ ] Night Shade: Damage = User's level
- [ ] Endeavor: Reduces target HP to user's HP
- [ ] Super Fang: Halves target's current HP
- [ ] OHKO moves: Fissure, Horn Drill, Guillotine, Sheer Cold

### 4.5 Recoil & Drain
- [x] **Recoil**: User takes % of damage dealt ✅
  - 1/4: Take Down, Double-Edge
  - 1/3: Brave Bird, Flare Blitz, Wood Hammer
  - 1/2: Head Smash
- [x] **Drain**: User heals % of damage dealt ✅
  - 1/2: Absorb, Mega Drain, Giga Drain
  - 3/4: Drain Punch, Horn Leech
- [ ] **Crash damage**: If move misses (High Jump Kick, Jump Kick) (Deferred)

---

## 5. Status Conditions

### 5.1 Persistent Status (One at a time)

#### Burn
- [x] 1/16 HP damage at end of turn ✅ Phase 2.8
- [ ] Attack halved for physical moves
- [ ] Fire types immune
- [ ] Can be inflicted by: Will-O-Wisp, Flamethrower (10%), etc.
- [ ] Cured by: Rawst Berry, Heal Bell, switching (with Natural Cure)

#### Paralysis
- [ ] Speed halved
- [ ] 25% chance to be fully paralyzed each turn
- [ ] Electric types immune (Gen 6+)
- [ ] Ground types can be paralyzed (it's not Electric damage)
- [ ] Cured by: Cheri Berry, Full Heal

#### Sleep
- [ ] Cannot act (except Sleep Talk, Snore)
- [ ] Duration: 1-3 turns (Gen 5+)
- [ ] Wake up at start of turn, can act immediately
- [ ] Grass types immune to powder moves that cause sleep
- [ ] Cured by: Awakening, being attacked by Wake-Up Slap

#### Poison
- [x] 1/8 HP damage at end of turn ✅ Phase 2.8
- [ ] Poison and Steel types immune (handled in ApplyStatusAction)
- [x] Badly Poisoned: 1/16, 2/16, 3/16... (increases each turn) ✅ Phase 2.8
- [ ] Badly Poisoned counter resets on switch (handled in SwitchAction)
- [ ] Cured by: Pecha Berry, Antidote

#### Freeze
- [ ] Cannot act
- [ ] 20% chance to thaw at start of each turn
- [ ] Fire moves used by/against frozen Pokemon thaw
- [ ] Ice types immune (Gen 2+)
- [ ] Cured by: Ice Heal, being hit by Fire move

### 5.2 Volatile Status (Cleared on switch)

#### Confusion
- [ ] 33% chance to hit self (Gen 7+)
- [ ] Self-damage uses 40 power physical move
- [ ] Duration: 2-5 turns
- [ ] Cured by: Persim Berry, switching out

#### Infatuation
- [ ] 50% chance to not act
- [ ] Only works on opposite gender
- [ ] Cured by: Mental Herb, switching out
- [ ] Oblivious ability prevents

#### Flinch
- [ ] Skip turn if hit by flinching move before acting
- [ ] Only works if flincher moves first
- [ ] Inner Focus, Shield Dust prevent

#### Taunt
- [ ] Cannot use status moves
- [ ] Duration: 3 turns
- [ ] Prankster-boosted Taunt can be blocked by Dark types

#### Encore
- [ ] Forced to use last move used
- [ ] Duration: 3 turns
- [ ] Fails if target hasn't moved yet

#### Disable
- [ ] One move cannot be used
- [ ] Duration: 4 turns
- [ ] Only disables one specific move

#### Leech Seed
- [ ] 1/8 HP drained to opponent at end of turn
- [ ] Grass types immune
- [ ] Continues if seeded Pokemon switches

#### Curse (Ghost)
- [ ] User loses 1/2 HP, target loses 1/4 HP each turn
- [ ] Not cleared by switching

#### Perish Song
- [ ] All Pokemon on field faint in 3 turns
- [ ] Switching resets counter for that Pokemon
- [ ] Soundproof immune

---

## 6. Stat Modifications

### 6.1 Stat Stages
- [ ] Range: -6 to +6
- [ ] Multipliers:
  - +6 = 4.0x
  - +5 = 3.5x
  - +4 = 3.0x
  - +3 = 2.5x
  - +2 = 2.0x
  - +1 = 1.5x
  - 0 = 1.0x
  - -1 = 0.66x
  - -2 = 0.5x
  - -3 = 0.4x
  - -4 = 0.33x
  - -5 = 0.28x
  - -6 = 0.25x

### 6.2 Affected Stats
- [ ] Attack
- [ ] Defense
- [ ] Special Attack
- [ ] Special Defense
- [ ] Speed
- [ ] Accuracy
- [ ] Evasion

### 6.3 Stat Change Mechanics
- [ ] Cannot exceed +6 or -6 ("won't go any higher/lower!")
- [ ] Cleared on switch out
- [ ] Critical hits ignore:
  - Negative stages on attacker
  - Positive stages on defender
- [ ] Abilities affect (Simple doubles, Contrary reverses)

### 6.4 Common Stat Moves
- [ ] +1 Atk: Howl, Meditate
- [ ] +2 Atk: Swords Dance
- [ ] +1 Def: Harden, Withdraw
- [ ] +2 Def: Iron Defense, Acid Armor
- [ ] +1 SpA: Growth (in sun +2)
- [ ] +2 SpA: Nasty Plot
- [ ] +2 SpD: Amnesia
- [ ] +2 Spe: Agility, Rock Polish
- [ ] +1 Acc: Hone Claws
- [ ] -1/-2 to opponent via moves/abilities

---

## 7. Switching

### 7.1 Manual Switch
- [ ] Switching has highest priority (+6)
- [ ] Resets all stat stages to 0
- [ ] Clears all volatile status
- [ ] Persistent status remains
- [ ] HP remains unchanged

### 7.2 Forced Switch
- [ ] Roar, Whirlwind: Random switch to bench
- [ ] Dragon Tail, Circle Throw: Damage then switch
- [ ] Priority -6 (moves last)
- [ ] Fails if no valid bench Pokemon
- [ ] Blocked by Suction Cups, Ingrain

### 7.3 Trapping
- [ ] Mean Look, Block: Cannot switch
- [ ] Binding moves: Cannot switch
- [ ] Shadow Tag, Arena Trap: Cannot switch
- [ ] Ghost types immune to trapping (Gen 6+)
- [ ] Shed Shell item allows escape

### 7.4 Baton Pass
- [ ] Switch while passing:
  - Stat stages
  - Substitute
  - Some volatile status (Ingrain, Aqua Ring)
- [ ] Does NOT pass:
  - Confusion
  - Leech Seed
  - Perish Song counter

### 7.5 U-turn / Volt Switch / Flip Turn
- [ ] Deal damage then switch
- [ ] If move fails/misses, no switch
- [ ] If opponent faints, switch is optional

---

## 8. Entry Effects

### 8.1 Entry Hazards

#### Spikes (3 layers)
- [ ] 1 layer: 12.5% max HP
- [ ] 2 layers: 16.67% max HP
- [ ] 3 layers: 25% max HP
- [ ] Flying types and Levitate immune
- [ ] Removed by Rapid Spin, Defog

#### Stealth Rock
- [ ] Damage based on type effectiveness vs Rock
- [ ] 0.25x: 3.125% HP
- [ ] 0.5x: 6.25% HP
- [ ] 1x: 12.5% HP
- [ ] 2x: 25% HP
- [ ] 4x: 50% HP (Charizard, Volcarona)

#### Toxic Spikes (2 layers)
- [ ] 1 layer: Poison
- [ ] 2 layers: Badly Poisoned
- [ ] Poison types absorb on entry (removes spikes)
- [ ] Flying types and Levitate immune
- [ ] Steel types immune to poison

#### Sticky Web
- [ ] -1 Speed on entry
- [ ] Flying types and Levitate immune
- [ ] Contrary reverses to +1 Speed

### 8.2 Entry Abilities
- [ ] Intimidate: -1 Attack to all opponents
- [ ] Drizzle/Drought/Sand Stream/Snow Warning: Set weather
- [ ] Electric/Grassy/Psychic/Misty Surge: Set terrain
- [ ] Download: +1 Atk or SpA based on opponent's lower defense
- [ ] Trace: Copy opponent's ability
- [ ] Pressure: Announced on entry

---

## 9. Field Effects

### 9.1 Weather (5 turns, or infinite with item)

#### Sun (Harsh Sunlight)
- [ ] Fire moves: 1.5x damage
- [ ] Water moves: 0.5x damage
- [ ] Solar Beam: No charge turn
- [ ] Thunder: 50% accuracy
- [ ] Moonlight/Morning Sun/Synthesis: Heal 2/3 HP
- [ ] Growth: +2 instead of +1
- [ ] Chlorophyll: 2x Speed
- [ ] Abilities: Drought sets, Solar Power boosts

#### Rain
- [ ] Water moves: 1.5x damage
- [ ] Fire moves: 0.5x damage
- [ ] Thunder: 100% accuracy, bypasses Protect
- [ ] Hurricane: 100% accuracy
- [ ] Solar Beam: 1-turn but half power
- [ ] Moonlight/etc: Heal 1/4 HP
- [ ] Swift Swim: 2x Speed
- [ ] Abilities: Drizzle sets, Rain Dish heals

#### Sandstorm
- [ ] 1/16 HP damage to non-Rock/Ground/Steel types
- [ ] Rock types: +50% SpDef
- [ ] Sand Rush: 2x Speed
- [ ] Sand Veil: +20% Evasion
- [ ] Abilities: Sand Stream sets

#### Hail/Snow
- [ ] 1/16 HP damage to non-Ice types
- [ ] Ice types: +50% Def (Gen 9 Snow)
- [ ] Blizzard: 100% accuracy
- [ ] Aurora Veil: Can be set
- [ ] Slush Rush: 2x Speed
- [ ] Snow Cloak: +20% Evasion

### 9.2 Terrain (5 turns, affects grounded Pokemon)

#### Electric Terrain
- [ ] Electric moves: 1.3x damage
- [ ] Prevents Sleep
- [ ] Rising Voltage: 2x power

#### Grassy Terrain
- [ ] Grass moves: 1.3x damage
- [ ] Heals 1/16 HP at end of turn
- [ ] Earthquake/Bulldoze/Magnitude: 0.5x damage
- [ ] Grassy Glide: +1 priority

#### Psychic Terrain
- [ ] Psychic moves: 1.3x damage
- [ ] Blocks priority moves against grounded Pokemon
- [ ] Expanding Force: 1.5x power, hits all

#### Misty Terrain
- [ ] Dragon moves: 0.5x damage
- [ ] Prevents status conditions
- [ ] Misty Explosion: 1.5x power

### 9.3 Side Conditions (5-8 turns)

#### Reflect
- [ ] Physical damage: 0.5x (Singles), 0.66x (Doubles)
- [ ] Brick Break removes
- [ ] Light Clay extends to 8 turns

#### Light Screen
- [ ] Special damage: 0.5x (Singles), 0.66x (Doubles)
- [ ] Brick Break removes
- [ ] Light Clay extends to 8 turns

#### Aurora Veil
- [ ] Both physical and special: 0.5x/0.66x
- [ ] Only in Hail/Snow
- [ ] Brick Break removes

#### Tailwind
- [ ] 2x Speed for side
- [ ] 4 turns

---

## 10. Targeting

### 10.1 Target Scopes
- [ ] Self
- [ ] Single adjacent ally
- [ ] Single adjacent opponent
- [ ] Single any (ally or opponent)
- [ ] All adjacent (spread)
- [ ] All opponents
- [ ] All allies
- [ ] All Pokemon on field
- [ ] Entire side (hazards, screens)
- [ ] Random opponent

### 10.2 Adjacency (Triples)
```
[L] [C] [R]    - Your side
[R] [C] [L]    - Opponent side

L can hit: Own C, Opp R, Opp C
C can hit: Own L, Own R, Opp L, Opp C, Opp R
R can hit: Own C, Opp C, Opp L
```

### 10.3 Target Redirection
- [ ] Follow Me: Draws single-target moves
- [ ] Rage Powder: Same but only affects Pokemon affected by powder
- [ ] Ally Switch: Swaps positions
- [ ] Lightning Rod: Draws Electric moves
- [ ] Storm Drain: Draws Water moves

### 10.4 Target Validation
- [ ] Fainted targets: Re-target (Doubles) or fail (Singles)
- [ ] Protected targets: Move fails/reduced
- [ ] Semi-invulnerable: Miss (unless exception)
- [ ] Switched out: Move fails (unless Pursuit)

---

## 11. Special Move Mechanics

### 11.1 Pursuit
- [ ] If target switches, hits with 2x power before switch
- [ ] Uses the "switching" target, not the incoming Pokemon
- [ ] If target doesn't switch, normal priority move

### 11.2 Future Sight / Doom Desire
- [ ] Damage delayed 2 turns
- [ ] Hits the Pokemon in that slot (not original target)
- [ ] Uses user's SpA at time of use
- [ ] Uses target's SpD at time of hit
- [ ] Does not check accuracy on hit turn

### 11.3 Counter / Mirror Coat
- [ ] Counter: Returns 2x physical damage taken
- [ ] Mirror Coat: Returns 2x special damage taken
- [ ] Priority -5
- [ ] Fails if not hit by appropriate damage type
- [ ] Uses damage from last hit that turn

### 11.4 Protect / Detect
- [ ] Blocks most moves
- [ ] Success rate halves with consecutive use
- [ ] Does not block:
  - Feint
  - Shadow Force, Phantom Force
  - Status moves that bypass (Perish Song, Transform)
- [ ] Protect variants:
  - Wide Guard (blocks spread)
  - Quick Guard (blocks priority)
  - King's Shield (protects, -2 Atk on contact)
  - Spiky Shield (protects, damages on contact)
  - Baneful Bunker (protects, poisons on contact)

### 11.5 Focus Punch
- [ ] Priority -3
- [ ] User "tightens focus" at start of turn
- [ ] If hit before moving, move fails
- [ ] Still deducts PP even if fails

### 11.6 Semi-Invulnerable Moves
- [ ] Fly: Flying state
- [ ] Dig: Underground
- [ ] Dive: Underwater
- [ ] Bounce: In air
- [ ] Shadow Force: Vanished
- [ ] Phantom Force: Vanished
- [ ] Sky Drop: In air with target

Most moves miss, except:
- [ ] Earthquake hits Dig users
- [ ] Surf hits Dive users
- [ ] Thunder hits Fly users (in rain)
- [ ] No Guard hits all

---

## 12. Victory/Defeat

### 12.1 Win Conditions
- [ ] All opponent's Pokemon fainted
- [ ] Opponent runs out of time (competitive)
- [ ] Opponent forfeits

### 12.2 Lose Conditions
- [ ] All your Pokemon fainted
- [ ] Run out of time
- [ ] Forfeit

### 12.3 Draw Conditions
- [ ] Both last Pokemon faint simultaneously (Explosion, recoil, etc.)
- [ ] Timeout with equal remaining Pokemon

### 12.4 End-of-Battle Effects
- [ ] Experience gain
- [ ] EVs from defeated Pokemon
- [ ] Friendship changes
- [ ] Item consumption finalized

---

## 13. Abilities & Items

### 13.1 Ability Triggers
- [x] On switch-in (Intimidate implemented)
- [ ] On being hit (Static, Rough Skin - deferred)
- [ ] On hitting opponent (deferred)
- [ ] On status change (deferred)
- [ ] On stat change (deferred)
- [x] End of turn (Speed Boost - deferred, system ready)
- [ ] Weather change (Swift Swim - deferred)
- [ ] Terrain change (deferred)
- [x] HP threshold (Blaze, Torrent, Overgrow) ✅

### 13.2 Common Battle Items
- [x] Choice Band: +50% Attack stat ✅
- [x] Choice Specs: +50% SpAttack stat ✅
- [x] Choice Scarf: +50% Speed stat ✅ (integrated in TurnOrderResolver)
- [x] Life Orb: +30% damage multiplier ✅
- [x] Assault Vest: +50% SpDefense stat ✅
- [x] Eviolite: +50% Defense/SpDefense if can evolve ✅
- [ ] Life Orb recoil: 10% HP loss after move (requires OnAfterMove trigger - deferred)
- [x] Leftovers: Heal 1/16 HP per turn ✅
- [ ] Focus Sash: Survive one hit at 1 HP (requires OnWouldFaint trigger)
- [ ] Assault Vest: +50% SpDef, no status moves (requires passive stat modifier)
- [ ] Rocky Helmet: Attacker takes 1/6 HP on contact (requires OnContactReceived trigger)
- [ ] Eviolite: +50% Def/SpDef for non-fully evolved (requires passive stat modifier)

---

## Validation Checklist by Phase

### Phase 2.1: Battle Foundation ✅
- [x] 1.1-1.4: Battle formats (1v1, 2v2, 3v3, horde)
- [x] 6.1-6.3: Stat stages structure
- [x] 5.2: Volatile status flags
- [x] 7.1: Switch resets state

### Phase 2.2: Action Queue ✅
- [x] 2.4: Action execution order (BattleQueue FIFO + InsertAtFront for reactions)
- [x] 2.5: End of turn processing structure (queue empty = turn complete)

### Phase 2.3: Turn Order ✅
- [x] 2.2: Priority brackets (BattleAction.Priority, TurnOrderResolver sorts by priority)
- [x] 2.3: Speed modifiers (stat stages ±6, paralysis ×0.5)
- [x] 7.1: Switch priority (+6 priority in `SwitchAction`)

### Phase 2.4: Damage Calculation ✅
- [x] 4.1: Base damage formula (BaseDamageStep with Gen 3+ formula)
- [x] 4.2: Damage modifiers (Pipeline with STAB, Burn, Random, Crit steps)
- [x] 4.3: Type effectiveness (TypeEffectivenessStep using existing TypeEffectiveness)
- [x] 3.3: Critical hits (CriticalHitStep with 1.5x multiplier, base 1/24 rate)
- [x] 3.4: Move categories (Physical/Special/Status routing)
- [x] Status moves: Zero damage for power 0 moves

### Phase 2.5: Combat Actions ✅
- [x] 3.1: Pre-move checks (PP, Sleep, Freeze, Paralysis, Flinch)
- [x] 3.2: Accuracy check (`AccuracyChecker` with stages)
- [x] 5.1: Persistent status effects (`ApplyStatusAction`)
- [x] 5.2: Volatile status effects (Flinch implemented, others pending)
- [x] 6.4: Stat change moves (`StatChangeAction`)

**Status**: ✅ **Core Complete** - Basic status application works, end-of-turn effects deferred

### Phase 2.6: Combat Engine ✅
- [x] 2.1-2.5: Complete turn flow (`CombatEngine.RunTurn()`)
- [x] 12.1-12.3: Victory/defeat conditions (`BattleArbiter`)
- [ ] 11.2: Delayed moves (Future Sight) ⏳ Deferred to future phase

**Status**: ✅ **Core Complete** - Turn flow and victory detection working

### Phase 2.7: Integration ✅
- [x] Full battle simulation (`CombatEngine.RunBattle()`)
- [x] AI vs AI battles (`RandomAI`, `AlwaysAttackAI`)
- [x] All basic mechanics working together
- [x] TargetResolver for move targeting

**Status**: ✅ **100% Complete**

### Phase 2.8: End-of-Turn Effects ✅
- [x] Status damage processing (`EndOfTurnProcessor`)
- [x] Burn damage (1/16 Max HP per turn)
- [x] Poison damage (1/8 Max HP per turn)
- [x] Badly Poisoned escalating damage (counter-based)
- [x] Integration into `CombatEngine.RunTurn()`

**Status**: ✅ **Core Complete** - Status damage implemented

### Phase 2.9: Abilities & Items Battle Integration ✅
- [x] BattleTrigger enum (`OnSwitchIn`, `OnTurnEnd`, etc.)
- [x] IBattleListener interface
- [x] AbilityListener adapter (converts AbilityData to IBattleListener)
- [x] ItemListener adapter (converts ItemData to IBattleListener)
- [x] BattleTriggerProcessor (processes triggers for all active Pokemon)
- [x] OnSwitchIn trigger integration (in `SwitchAction`)
- [x] OnTurnEnd trigger integration (in `CombatEngine`)
- [x] Leftovers item effect (heals 1/16 Max HP per turn)
- [x] Intimidate ability effect (lowers opponent Attack on switch-in)

**Status**: ✅ **Core Complete** - Event-driven system implemented, basic effects working

### Future Phases
- [ ] 8: Entry effects (hazards)
- [ ] 9: Field effects (weather, terrain)
- [ ] 10.2-10.3: Advanced targeting
- [ ] 11.1: Pursuit
- [ ] 11.3-11.6: Special move mechanics
- [ ] 13.1: More ability triggers (OnDamageTaken, OnBeforeMove, etc.)
- [ ] 13.2: More item effects (Choice items, Life Orb, etc.)

---

## Usage Instructions

1. **Before implementing**: Read relevant sections
2. **During implementation**: Use cases as test scenarios
3. **After implementing**: Validate against checklist
4. **Update this document**: Check off completed items

---

## Related Documents

- **[Architecture](architecture.md)** - Technical design these use cases support
- **[Roadmap](roadmap.md)** - Implementation status of use cases
- **[Testing](testing.md)** - Tests covering these use cases
- **[Code Location](code_location.md)** - Where use cases are implemented
- **[Feature 1: Game Data](../1-game-data/use_cases.md)** - Game data scenarios
- **[Sub-Feature 2.5: Combat Actions](2.5-combat-actions/use_cases.md)** - Detailed action use cases

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

---

**Last Updated**: 2025-01-XX

