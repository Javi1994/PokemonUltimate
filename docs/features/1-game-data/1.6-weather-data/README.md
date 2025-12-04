# Sub-Feature 1.6: Weather Data

> Weather condition blueprint for battle field weather effects.

**Sub-Feature Number**: 1.6  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

This sub-feature defines the data structure for weather conditions that affect the entire battlefield:
- **WeatherData**: Weather condition blueprint (Rain, Sun, Hail, Sandstorm, Fog, Primal weathers)

Weather affects all Pokemon on the field and can:
- Modify type power (boost/weaken/nullify)
- Deal end-of-turn damage
- Grant stat boosts to certain types
- Modify move accuracy and charge times
- Interact with abilities

## Components

### WeatherData
**Namespace**: `PokemonUltimate.Core.Blueprints`  
**File**: `PokemonUltimate.Core/Blueprints/WeatherData.cs`

Immutable blueprint defining weather condition behavior:
- Duration (default 5 turns, 0 = indefinite)
- Type power modifiers (boosted/weakened/nullified types)
- End-of-turn damage (with type immunities)
- Stat modifiers (e.g., SpDef boost for Rock in Sandstorm)
- Move effects (perfect accuracy moves, instant charge moves)
- Ability interactions (immunities, speed boosts, healing)

## Key Features

- **Duration System**: Default duration, can be overwritten (except primal weathers)
- **Type Modifiers**: Boost (1.5x), weaken (0.5x), or nullify (0x) move power
- **Damage System**: End-of-turn damage as fraction of max HP (1/16 = 0.0625)
- **Stat Boosts**: Type-specific stat multipliers (e.g., Rock +50% SpDef in Sandstorm)
- **Move Modifications**: Perfect accuracy and instant charge for specific moves
- **Ability Integration**: Damage immunities, speed boosts, healing abilities

## Related Sub-Features

- **[1.2: Move Data](../1.2-move-data/)** - Moves that set weather conditions
- **[3.9: Builders](../../3-content-expansion/3.9-builders/)** - WeatherBuilder for creating weather data
- **[1.3: Ability Data](../1.3-ability-data/)** - Abilities that interact with weather

## Documentation

- **[Parent Architecture](../architecture.md#16-weather-data)** - Technical specification
- **[Parent Code Location](../code_location.md#16-weather-data)** - Code organization

---

**Last Updated**: 2025-01-XX

