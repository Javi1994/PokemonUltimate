# Feature 1: Game Data

> All game data structures (blueprints) and supporting systems for Pokemon, Moves, Abilities, Items, Field Conditions, and infrastructure.

**Feature Number**: 1  
**Feature Name**: Game Data  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This feature defines **all game data structures** (blueprints) and supporting systems, organized into logical groups:
- **Core Entity Data**: Pokemon, Moves, Abilities, Items
- **Field & Status Data**: Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects
- **Supporting Systems**: Evolution System, Type Effectiveness Table
- **Infrastructure**: Interfaces, Enums, Constants, Builders, Factories, Registries

**Goal**: Provide a complete, well-organized foundation for all game data structures before expanding content, preventing future refactoring.

## Current Status

- ✅ **Core Complete**: All main data structures implemented (Pokemon, Moves, Abilities, Items, Field Conditions)
- ✅ **Infrastructure Complete**: Factories, Registries, Enums, Constants
- ⚠️ **Note**: Builders moved to Feature 3 (Sub-Feature 3.9) as they are primarily used for content creation
- ⏳ **Planned**: Variants System (Mega, Dinamax, Terracristalización)
- ⏳ **Planned**: Pokedex Fields (Description, Category, Height, Weight, Color, Shape, Habitat)

## Documentation

| Document | Purpose |
|----------|---------|
| **[Architecture](architecture.md)** | Complete technical specification of all data structures |
| **[Use Cases](use_cases.md)** | All scenarios and behaviors |
| **[Roadmap](roadmap.md)** | Implementation plan with phases and priorities |
| **[Testing](testing.md)** | Comprehensive testing strategy for game data |
| **[Code Location](code_location.md)** | Where the code lives and how it's organized |

## Sub-Features (Organized by Groups)

### Grupo A: Core Entity Data (Entidades Principales)
- **[1.1: Pokemon Data](1.1-pokemon-data/)** - PokemonSpeciesData (Blueprint), PokemonInstance (Runtime), BaseStats, LearnableMove
- **[1.2: Move Data](1.2-move-data/)** - MoveData (Blueprint), MoveInstance (Runtime), Move Effects (22 implementations)
- **[1.3: Ability Data](1.3-ability-data/)** - AbilityData (Blueprint)
- **[1.4: Item Data](1.4-item-data/)** - ItemData (Blueprint)

### Grupo B: Field & Status Data (Condiciones de Campo)
- **[1.5: Status Effect Data](1.5-status-effect-data/)** - StatusEffectData (Blueprint)
- **[1.6: Weather Data](1.6-weather-data/)** - WeatherData (Blueprint)
- **[1.7: Terrain Data](1.7-terrain-data/)** - TerrainData (Blueprint)
- **[1.8: Hazard Data](1.8-hazard-data/)** - HazardData (Blueprint)
- **[1.9: Side Condition Data](1.9-side-condition-data/)** - SideConditionData (Blueprint)
- **[1.10: Field Effect Data](1.10-field-effect-data/)** - FieldEffectData (Blueprint)

### Grupo C: Supporting Systems (Sistemas de Soporte)
- **[1.11: Evolution System](1.11-evolution-system/)** - Evolution, IEvolutionCondition, EvolutionConditions (6 classes)
- **[1.12: Type Effectiveness Table](1.12-type-effectiveness-table/)** - TypeEffectiveness (data table)

### Grupo D: Infrastructure (Infraestructura)
- **[1.13: Interfaces Base](1.13-interfaces-base/)** - IIdentifiable
- **[1.14: Enums & Constants](1.14-enums-constants/)** - Enums (20 main + 7 in Effects), ErrorMessages, GameMessages, NatureData
- ⚠️ **Builders**: Moved to **[Feature 3.9: Builders](../3-content-expansion/3.9-builders/)** - Used primarily for content creation
- **[1.16: Factories & Calculators](1.16-factories-calculators/)** - StatCalculator, PokemonFactory, PokemonInstanceBuilder
- **[1.17: Registry System](1.17-registry-system/)** - IDataRegistry<T>, GameDataRegistry<T>, PokemonRegistry, MoveRegistry

### Grupo E: Planned Features
- **[1.18: Variants System](1.18-variants-system/)** - Mega/Dinamax/Terracristalización as separate species (Planned)
- **[1.19: Pokedex Fields](1.19-pokedex-fields/)** - Description, Category, Height, Weight, Color, Shape, Habitat (Planned)

## Related Features

- **[Feature 2: Combat System](../2-combat-system/)** - Uses game data structures in battles
- **[Feature 3: Content Expansion](../3-content-expansion/)** - Adding more content using these data structures
- **[Feature 5: Game Features](../5-game-features/)** - Catching, evolution, friendship systems

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Quick Links

- **Current Status**: Core data structures complete, infrastructure complete
- **Key Classes**: See [code location](code_location.md) for implementation details
- **Tests**: See [testing](testing.md) for test organization
- **Sub-Feature Architecture**: Detailed technical specs available for complex sub-features (1.1, 1.2, 1.11, 1.15, 1.17, 1.18)

---

**Last Updated**: 2025-01-XX
