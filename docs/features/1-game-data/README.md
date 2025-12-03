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
- ✅ **Infrastructure Complete**: Builders, Factories, Registries, Enums, Constants
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
- **[1.6: Field Conditions Data](1.6-field-conditions-data/)** - WeatherData, TerrainData, HazardData, SideConditionData, FieldEffectData

### Grupo C: Supporting Systems (Sistemas de Soporte)
- **[1.7: Evolution System](1.7-evolution-system/)** - Evolution, IEvolutionCondition, EvolutionConditions (6 classes)
- **[1.8: Type Effectiveness Table](1.8-type-effectiveness-table/)** - TypeEffectiveness (data table)

### Grupo D: Infrastructure (Infraestructura)
- **[1.9: Interfaces Base](1.9-interfaces-base/)** - IIdentifiable
- **[1.10: Enums & Constants](1.10-enums-constants/)** - Enums (20 main + 7 in Effects), ErrorMessages, GameMessages
- **[1.11: Builders](1.11-builders/)** - 13 builder classes + 10 static helper classes
- **[1.12: Factories & Calculators](1.12-factories-calculators/)** - StatCalculator, PokemonFactory, PokemonInstanceBuilder, NatureData
- **[1.13: Registry System](1.13-registry-system/)** - IDataRegistry<T>, GameDataRegistry<T>, PokemonRegistry, MoveRegistry

### Grupo E: Planned Features
- **[1.14: Variants System](1.14-variants-system/)** - Mega/Dinamax/Terracristalización as separate species (Planned)
- **[1.15: Pokedex Fields](1.15-pokedex-fields/)** - Description, Category, Height, Weight, Color, Shape, Habitat (Planned)

## Related Features

- **[Feature 2: Combat System](../2-combat-system/)** - Uses game data structures in battles
- **[Feature 3: Content Expansion](../3-content-expansion/)** - Adding more content using these data structures
- **[Feature 5: Game Features](../5-game-features/)** - Catching, evolution, friendship systems

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Quick Links

- **Current Status**: Core data structures complete, infrastructure complete
- **Key Classes**: See [code location](code_location.md) for implementation details
- **Tests**: See [testing](testing.md) for test organization
- **Sub-Feature Architecture**: Detailed technical specs available for complex sub-features (1.1, 1.2, 1.7, 1.11, 1.13, 1.14)

---

**Last Updated**: 2025-01-XX
