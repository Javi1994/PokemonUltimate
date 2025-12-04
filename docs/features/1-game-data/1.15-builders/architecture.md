# Sub-Feature 1.15: Builders - Architecture

> Complete technical specification for fluent builder APIs for creating game data.

**Sub-Feature Number**: 1.15  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines all builder classes for creating game data:
- **13 Builder Classes**: Fluent APIs for creating data blueprints
- **10 Static Helper Classes**: Convenience entry points

## Design Principles

- **Fluent API**: Method chaining for readable code
- **Immutable Builders**: Builders create immutable blueprints
- **Testability First**: Pure C# classes, no Unity dependencies

---

## 1. Builder Classes (13)

**Namespace**: `PokemonUltimate.Core.Builders`  
**Files**: `PokemonUltimate.Core/Builders/*.cs`

### 1.1 PokemonBuilder

**File**: `PokemonBuilder.cs`  
**Static Helper**: `Pokemon`

Fluent builder for creating PokemonSpeciesData instances.

```csharp
var pikachu = Pokemon.Define("Pikachu", 25)
    .Type(PokemonType.Electric)
    .Stats(35, 55, 40, 50, 50, 90)
    .GenderRatio(50f)
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch)
        .AtLevel(9, MoveCatalog.ThunderShock)
        .ByTM(MoveCatalog.Thunderbolt))
    .EvolvesTo(Raichu, e => e.WithItem("Thunder Stone"))
    .Build();
```

**Key Methods**:
- `Define(name, pokedexNumber)` - Start definition
- `Type(type)` - Set mono-type
- `Types(primary, secondary)` - Set dual-type
- `Stats(hp, atk, def, spa, spd, spe)` - Set base stats
- `GenderRatio(percent)` - Set gender ratio
- `Moves(config)` - Configure learnset
- `EvolvesTo(target, config)` - Add evolution
- `Build()` - Finalize

---

### 1.2 MoveBuilder

**File**: `MoveBuilder.cs`  
**Static Helper**: `Move`

Fluent builder for creating MoveData instances.

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
    .WithEffect(new StatusEffect(PersistentStatus.Paralysis, chancePercent: 10))
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Type(type)` - Set move type
- `Category(category)` - Set move category
- `Power(power)` - Set base power
- `Accuracy(accuracy)` - Set accuracy
- `MaxPP(pp)` - Set maximum PP
- `Priority(priority)` - Set priority bracket
- `Target(scope)` - Set target scope
- `WithEffect(effect)` - Add move effect
- `Build()` - Finalize

---

### 1.3 AbilityBuilder

**File**: `AbilityBuilder.cs`  
**Static Helper**: `Ability`

Fluent builder for creating AbilityData instances.

```csharp
var blaze = Ability.Define("Blaze")
    .Description("Powers up Fire-type moves when HP is low")
    .Gen(3)
    .OnLowHP(threshold: 0.33f)
    .BoostsType(PokemonType.Fire, multiplier: 1.5f)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `Gen(generation)` - Set generation
- `OnTrigger(trigger)` - Set ability trigger
- `WithEffect(effect)` - Set ability effect
- `Build()` - Finalize

---

### 1.4 ItemBuilder

**File**: `ItemBuilder.cs`  
**Static Helper**: `Item`

Fluent builder for creating ItemData instances.

```csharp
var leftovers = Item.Define("Leftovers")
    .Description("Restores HP at end of turn")
    .Category(ItemCategory.HeldItem)
    .OnEndOfTurn()
    .HealsHP(percent: 0.0625f)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `Category(category)` - Set item category
- `OnTrigger(trigger)` - Set item trigger
- `WithEffect(effect)` - Set item effect
- `Build()` - Finalize

---

### 1.5 StatusEffectBuilder

**File**: `StatusEffectBuilder.cs`  
**Static Helper**: `Status`

Fluent builder for creating StatusEffectData instances.

```csharp
var burn = Status.Define("Burn")
    .Description("Causes damage each turn and halves Attack")
    .PersistentStatus(PersistentStatus.Burn)
    .DamagePerTurn(percent: 0.0625f)
    .ModifiesStat(Stat.Attack, multiplier: 0.5f)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `PersistentStatus(status)` - Set persistent status
- `VolatileStatus(status)` - Set volatile status
- `DamagePerTurn(percent)` - Set damage per turn
- `ModifiesStat(stat, multiplier)` - Set stat modifier
- `Build()` - Finalize

---

### 1.6 SideConditionBuilder

**File**: `SideConditionBuilder.cs`  
**Static Helper**: `Screen`

Fluent builder for creating SideConditionData instances.

```csharp
var lightScreen = Screen.Define("Light Screen")
    .Description("Reduces Special damage for 5 turns")
    .SideCondition(SideCondition.LightScreen)
    .Duration(turns: 5)
    .ReducesDamage(MoveCategory.Special, multiplier: 0.5f)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `SideCondition(condition)` - Set side condition type
- `Duration(turns)` - Set duration
- `ReducesDamage(category, multiplier)` - Set damage reduction
- `Build()` - Finalize

---

### 1.7 FieldEffectBuilder

**File**: `FieldEffectBuilder.cs`  
**Static Helper**: `Room`

Fluent builder for creating FieldEffectData instances.

```csharp
var trickRoom = Room.Define("Trick Room")
    .Description("Slower Pokemon move first for 5 turns")
    .FieldEffect(FieldEffect.TrickRoom)
    .Duration(turns: 5)
    .ReversesSpeed()
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `FieldEffect(effect)` - Set field effect type
- `Duration(turns)` - Set duration
- `Build()` - Finalize

---

### 1.8 HazardBuilder

**File**: `HazardBuilder.cs`  
**Static Helper**: `Hazard`

Fluent builder for creating HazardData instances.

```csharp
var stealthRock = Hazard.Define("Stealth Rock")
    .Description("Damages Pokemon switching in")
    .Type(HazardType.StealthRock)
    .MaxLayers(1)
    .DamageByType(PokemonType.Rock, percent: 0.125f)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `Type(type)` - Set hazard type
- `MaxLayers(layers)` - Set maximum layers
- `DamageByType(type, percent)` - Set damage by type
- `Build()` - Finalize

---

### 1.9 WeatherBuilder

**File**: `WeatherBuilder.cs`  
**Static Helper**: `WeatherEffect`

Fluent builder for creating WeatherData instances.

```csharp
var sunnyDay = WeatherEffect.Define("Sunny Day")
    .Description("Fire moves boosted, Water moves weakened")
    .Weather(Weather.Sunny)
    .DefaultDuration(turns: 5)
    .BoostsType(PokemonType.Fire, multiplier: 1.5f)
    .WeakensType(PokemonType.Water, multiplier: 0.5f)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `Weather(weather)` - Set weather type
- `DefaultDuration(turns)` - Set default duration
- `BoostsType(type, multiplier)` - Set type boost
- `Build()` - Finalize

---

### 1.10 TerrainBuilder

**File**: `TerrainBuilder.cs`  
**Static Helper**: `TerrainEffect`

Fluent builder for creating TerrainData instances.

```csharp
var electricTerrain = TerrainEffect.Define("Electric Terrain")
    .Description("Electric moves boosted, prevents Sleep")
    .Terrain(Terrain.Electric)
    .DefaultDuration(turns: 5)
    .BoostsType(PokemonType.Electric, multiplier: 1.3f)
    .PreventsStatus(PersistentStatus.Sleep)
    .Build();
```

**Key Methods**:
- `Define(name)` - Start definition
- `Description(description)` - Set description
- `Terrain(terrain)` - Set terrain type
- `DefaultDuration(turns)` - Set default duration
- `BoostsType(type, multiplier)` - Set type boost
- `Build()` - Finalize

---

### 1.11 EffectBuilder

**File**: `EffectBuilder.cs`

Fluent builder for creating move effects.

```csharp
var damageEffect = EffectBuilder.Damage()
    .Multiplier(1.0f)
    .CanCrit(true)
    .Build();

var statusEffect = EffectBuilder.Status(PersistentStatus.Paralysis)
    .Chance(10)
    .Build();
```

**Key Methods**:
- `Damage()` - Create damage effect
- `Status(status)` - Create status effect
- `StatChange(stat, stages)` - Create stat change effect
- `Heal(amount)` - Create heal effect
- `Build()` - Finalize

---

### 1.15 EvolutionBuilder

**File**: `EvolutionBuilder.cs`

Fluent builder for creating evolution paths.

```csharp
var evolution = EvolutionBuilder.Create(charizardSpecies)
    .AtLevel(36)
    .Build();

var complexEvolution = EvolutionBuilder.Create(espeonSpecies)
    .WithFriendship(220)
    .During(TimeOfDay.Day)
    .Build();
```

**Key Methods**:
- `Create(target)` - Start definition
- `AtLevel(level)` - Add level condition
- `WithItem(itemName)` - Add item condition
- `WithTrade()` - Add trade condition
- `WithFriendship(minFriendship)` - Add friendship condition
- `During(timeOfDay)` - Add time of day condition
- `KnowsMove(moveName)` - Add move condition
- `Build()` - Finalize

---

### 1.13 LearnsetBuilder

**File**: `LearnsetBuilder.cs`

Fluent builder for creating learnset lists.

```csharp
var learnset = LearnsetBuilder.Create()
    .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
    .AtLevel(9, MoveCatalog.Ember)
    .AtLevel(15, MoveCatalog.FireSpin)
    .ByTM(MoveCatalog.FireBlast)
    .ByEgg(MoveCatalog.DragonRush)
    .Build();
```

**Key Methods**:
- `Create()` - Start definition
- `StartsWith(moves...)` - Add starting moves
- `AtLevel(level, moves...)` - Add level-up moves
- `ByTM(moves...)` - Add TM moves
- `ByEgg(moves...)` - Add egg moves
- `OnEvolution(moves...)` - Add evolution moves
- `ByTutor(moves...)` - Add tutor moves
- `Build()` - Finalize

---

## 2. Static Helper Classes (10)

Static entry points for convenience. These provide shorter syntax for starting builder chains.

| Helper Class | Builder Class | Usage |
|--------------|---------------|-------|
| `Pokemon` | `PokemonBuilder` | `Pokemon.Define("Pikachu", 25)` |
| `Move` | `MoveBuilder` | `Move.Define("Thunderbolt")` |
| `Ability` | `AbilityBuilder` | `Ability.Define("Blaze")` |
| `Item` | `ItemBuilder` | `Item.Define("Leftovers")` |
| `Status` | `StatusEffectBuilder` | `Status.Define("Burn")` |
| `Screen` | `SideConditionBuilder` | `Screen.Define("Light Screen")` |
| `Room` | `FieldEffectBuilder` | `Room.Define("Trick Room")` |
| `Hazard` | `HazardBuilder` | `Hazard.Define("Stealth Rock")` |
| `WeatherEffect` | `WeatherBuilder` | `WeatherEffect.Define("Sunny Day")` |
| `TerrainEffect` | `TerrainBuilder` | `TerrainEffect.Define("Electric Terrain")` |

---

## 3. Usage Patterns

### Basic Pattern

```csharp
var data = Helper.Define("Name")
    .Property(value)
    .AnotherProperty(value)
    .Build();
```

### Nested Builders

```csharp
var pokemon = Pokemon.Define("Pikachu", 25)
    .Moves(m => m
        .StartsWith(MoveCatalog.Scratch)
        .AtLevel(9, MoveCatalog.ThunderShock))
    .EvolvesTo(Raichu, e => e
        .WithItem("Thunder Stone"))
    .Build();
```

### Multiple Effects

```csharp
var move = Move.Define("Thunderbolt")
    .WithEffect(new DamageEffect())
    .WithEffect(new StatusEffect(PersistentStatus.Paralysis, chancePercent: 10))
    .Build();
```

---

## 4. Related Sub-Features

All sub-features use builders to create their data structures:
- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Uses PokemonBuilder
- **[1.2: Move Data](../1.2-move-data/)** - Uses MoveBuilder, EffectBuilder
- **[1.3: Ability Data](../1.3-ability-data/)** - Uses AbilityBuilder
- **[1.4: Item Data](../1.4-item-data/)** - Uses ItemBuilder
- **[1.5: Status Effect Data](../1.5-status-effect-data/)** - Uses StatusEffectBuilder
- **[1.6: Weather Data](../1.6-weather-data/)** - Uses WeatherBuilder
- **[1.7: Terrain Data](../1.7-terrain-data/)** - Uses TerrainBuilder
- **[1.8: Hazard Data](../1.8-hazard-data/)** - Uses HazardBuilder
- **[1.9: Side Condition Data](../1.9-side-condition-data/)** - Uses SideConditionBuilder
- **[1.10: Field Effect Data](../1.10-field-effect-data/)** - Uses FieldEffectBuilder
- **[1.11: Evolution System](../1.11-evolution-system/)** - Uses EvolutionBuilder, LearnsetBuilder

---

## Related Documents

- **[Parent Architecture](../architecture.md#111-builders)** - Feature-level technical specification
- **[Parent Code Location](../code_location.md#grupo-d-infrastructure)** - Code organization
- **[Sub-Feature README](README.md)** - Overview and quick navigation

---

**Last Updated**: 2025-01-XX

