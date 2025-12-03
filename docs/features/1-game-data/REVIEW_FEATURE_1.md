# Revisión Feature 1: Game Data

> ⚠️ **DOCUMENTO HISTÓRICO** - Este documento fue creado durante la reorganización de Feature 1. La reorganización ya está completa. Ver el README principal para la estructura actual.

> Análisis de qué datos de juego existen en el código y cómo deberían organizarse en Feature 1.

## Estado Actual

✅ **REORGANIZACIÓN COMPLETA** - Feature 1 ha sido renombrada a "Game Data" y reorganizada según el análisis de este documento. Ver [`README.md`](README.md) para la estructura actual.

## Problema Identificado (Histórico)

La **Feature 1** originalmente se llamaba "Pokemon Data" pero debería abarcar **TODOS los datos de juego**, no solo Pokemon. El código tenía muchos más datos implementados de los que estaban documentados en la feature.

## Datos Existentes en el Código

### ✅ Datos Correctamente Categorizados como Feature 1

1. **PokemonSpeciesData** - Blueprint de especie Pokemon
2. **PokemonInstance** - Instancia runtime de Pokemon
3. **MoveData** - Blueprint de movimientos
4. **MoveInstance** - Instancia runtime de movimientos (PP tracking)
5. **Move Effects** (IMoveEffect y todas las implementaciones) - **DATOS DE JUEGO**
   - DamageEffect, StatusEffect, StatChangeEffect
   - HealEffect, RecoilEffect, DrainEffect
   - ProtectionEffect, ForceSwitchEffect, SwitchAfterAttackEffect
   - MultiHitEffect, BindingEffect, ChargingEffect
   - DelayedDamageEffect, FieldConditionEffect, FlinchEffect
   - FixedDamageEffect, MoveRestrictionEffect, PriorityModifierEffect
   - RevengeEffect, SelfDestructEffect, VolatileStatusEffect
   - **Nota**: Actualmente marcados como "1.1: PokemonSpeciesData" pero deberían ser "1.2: Move Data"
6. **AbilityData** - Blueprint de habilidades
7. **ItemData** - Blueprint de items
8. **StatusEffectData** - Blueprint de efectos de estado
9. **Evolution System** - Sistema de evolución
   - Evolution (clase)
   - IEvolutionCondition (interfaz)
   - EvolutionConditionType (enum)
   - LevelCondition, ItemCondition, FriendshipCondition
   - TradeCondition, TimeOfDayCondition, KnowsMoveCondition
10. **Registry System** - Sistema de registro de datos
    - IDataRegistry<T> (interfaz genérica)
    - GameDataRegistry<T> (implementación genérica)
    - IPokemonRegistry, IMoveRegistry (interfaces especializadas)
    - PokemonRegistry, MoveRegistry (implementaciones especializadas)
11. **Builders** - Builders para crear datos
    - PokemonBuilder, MoveBuilder, AbilityBuilder, ItemBuilder
    - StatusEffectBuilder, SideConditionBuilder, FieldEffectBuilder
    - HazardBuilder, WeatherBuilder, TerrainBuilder
    - EffectBuilder, EvolutionBuilder, LearnsetBuilder
    - **Clases estáticas helper**: Pokemon, Move, Ability, Item, Status, Screen, Room, Hazard, WeatherEffect, TerrainEffect
12. **Enums** - Todos los enums del juego (20 enums principales)
    - PokemonType, Stat, Nature, Gender
    - MoveCategory, EffectType, PersistentStatus, VolatileStatus
    - AbilityTrigger, AbilityEffect, ItemTrigger, ItemCategory
    - LearnMethod, TimeOfDay, TargetScope
    - Weather, Terrain, HazardType, SideCondition, FieldEffect
    - EvolutionConditionType
    - **Enums dentro de Effects** (7 enums adicionales):
      - SemiInvulnerableState (ChargingEffect)
      - FieldConditionType (FieldConditionEffect)
      - MoveRestrictionType (MoveRestrictionEffect)
      - ProtectionType, ContactPenalty (ProtectionEffect)
      - PriorityCondition (PriorityModifierEffect)
      - SelfDestructType (SelfDestructEffect)
13. **NatureData** - Datos de naturalezas (clase estática con tablas de modificadores)
14. **BaseStats** - Estadísticas base (estructura de datos)
15. **LearnableMove** - Movimientos aprendibles (estructura de datos)
16. **IIdentifiable** - Interfaz base para datos identificables
17. **StatCalculator** - Calculadora de estadísticas (clase estática con fórmulas)
18. **PokemonFactory** - Factory estático para crear Pokemon (clase estática)
19. **PokemonInstanceBuilder** - Builder fluido para crear instancias (clase + clase estática Pokemon)
20. **ErrorMessages** - Mensajes de error centralizados (clase estática)

### ❌ Datos Incorrectamente Categorizados como Feature 2

Estos datos están marcados como Feature 2 pero son **DATOS DE JUEGO**, no lógica de combate:

1. **WeatherData** - Actualmente marcado como Feature 2.12 (Weather System)
   - **Debería ser**: Feature 1 (Game Data)
   - **Razón**: Es un dato de juego (blueprint), no lógica de combate

2. **TerrainData** - Actualmente marcado como Feature 2.13 (Terrain System)
   - **Debería ser**: Feature 1 (Game Data)
   - **Razón**: Es un dato de juego (blueprint), no lógica de combate

3. **HazardData** - Actualmente marcado como Feature 2.14 (Hazards System)
   - **Debería ser**: Feature 1 (Game Data)
   - **Razón**: Es un dato de juego (blueprint), no lógica de combate

4. **SideConditionData** - Actualmente marcado como Feature 2.16 (Field Conditions)
   - **Debería ser**: Feature 1 (Game Data)
   - **Razón**: Es un dato de juego (blueprint), no lógica de combate

5. **FieldEffectData** - Actualmente marcado como Feature 2.16 (Field Conditions)
   - **Debería ser**: Feature 1 (Game Data)
   - **Razón**: Es un dato de juego (blueprint), no lógica de combate

6. **TypeEffectiveness** - Actualmente marcado como Feature 2.4 (Damage Calculation Pipeline)
   - **Debería ser**: Feature 1 (Game Data)
   - **Razón**: Contiene la tabla de efectividad de tipos (DATOS), no lógica de cálculo
   - **Nota**: La lógica de cálculo está en Feature 2, pero la tabla de datos debería estar en Feature 1

7. **GameMessages** - Actualmente marcado como Feature 2.5 (Combat Actions)
   - **Debería ser**: Feature 1 (Game Data) o Feature compartido
   - **Razón**: Son mensajes de texto del juego, no lógica de combate
   - **Nota**: Podría ser compartido entre Features, pero son datos de texto, no lógica

### 📝 Nota sobre Catálogos (Feature 3: Content Expansion)

Los **catálogos** en `PokemonUltimate.Content/Catalogs/` son parte de **Feature 3: Content Expansion**, no Feature 1.
- Feature 1 define las **estructuras de datos** (blueprints)
- Feature 3 contiene el **contenido específico** (instancias de datos)

**Catálogos existentes**:
- `PokemonCatalog` ✅ Feature 3 (correcto)
- `MoveCatalog` ✅ Feature 3 (correcto)
- `AbilityCatalog` ✅ Feature 3 (correcto)
- `ItemCatalog` ✅ Feature 3 (correcto)
- `StatusCatalog` ✅ Feature 1 (correcto - contiene definiciones de StatusEffectData)
- `WeatherCatalog` ❌ Feature 2.12 (debería ser Feature 3, pero WeatherData es Feature 1)
- `TerrainCatalog` ❌ Feature 2.13 (debería ser Feature 3, pero TerrainData es Feature 1)
- `HazardCatalog` ❌ Feature 2.14 (debería ser Feature 3, pero HazardData es Feature 1)
- `SideConditionCatalog` ❌ Feature 2.16 (debería ser Feature 3, pero SideConditionData es Feature 1)
- `FieldEffectCatalog` ❌ Feature 2.16 (debería ser Feature 3, pero FieldEffectData es Feature 1)

**Nota**: Los catálogos de Weather, Terrain, Hazard, SideCondition y FieldEffect están marcados como Feature 2, pero deberían ser Feature 3 (Content Expansion) ya que contienen instancias específicas de datos, no las estructuras de datos en sí.

## Propuesta de Reorganización

### Opción 1: Renombrar Feature 1 a "Game Data" (RECOMENDADA)

**Feature 1: Game Data** (antes "Pokemon Data")
- **Descripción**: Todos los datos de juego (blueprints) y estructuras de datos

#### Grupo A: Core Entity Data (Entidades Principales del Juego)
- **1.1**: Pokemon Data
  - PokemonSpeciesData (Blueprint)
  - PokemonInstance (Runtime)
  - BaseStats, LearnableMove (estructuras de soporte)
- **1.2**: Move Data
  - MoveData (Blueprint)
  - MoveInstance (Runtime con PP tracking)
  - **Move Effects** (IMoveEffect y todas las implementaciones - 22 clases)
- **1.3**: Ability Data
  - AbilityData (Blueprint)
- **1.4**: Item Data
  - ItemData (Blueprint)

#### Grupo B: Field & Status Data (Condiciones de Campo y Estado)
- **1.5**: Status Effect Data
  - StatusEffectData (Blueprint)
- **1.6**: Field Conditions Data
  - WeatherData, TerrainData, HazardData, SideConditionData, FieldEffectData

#### Grupo C: Supporting Systems (Sistemas de Soporte)
- **1.7**: Evolution System
  - Evolution, IEvolutionCondition, EvolutionConditionType
  - LevelCondition, ItemCondition, FriendshipCondition, TradeCondition, TimeOfDayCondition, KnowsMoveCondition
- **1.8**: Type Effectiveness Table
  - TypeEffectiveness (tabla de efectividad de tipos - DATOS)

#### Grupo D: Infrastructure (Infraestructura para Crear y Gestionar Datos)
- **1.9**: Interfaces Base
  - IIdentifiable (interfaz base para datos identificables)
- **1.10**: Enums & Constants
  - Enums principales (20): PokemonType, Stat, Nature, Gender, MoveCategory, etc.
  - Enums dentro de Effects (7): SemiInvulnerableState, ProtectionType, etc.
  - Constants: ErrorMessages, GameMessages
- **1.11**: Builders
  - PokemonBuilder, MoveBuilder, AbilityBuilder, ItemBuilder
  - StatusEffectBuilder, SideConditionBuilder, FieldEffectBuilder
  - HazardBuilder, WeatherBuilder, TerrainBuilder
  - EffectBuilder, EvolutionBuilder, LearnsetBuilder
  - Clases estáticas helper (10): Pokemon, Move, Ability, Item, Status, Screen, Room, Hazard, WeatherEffect, TerrainEffect
- **1.12**: Factories & Calculators
  - StatCalculator (fórmulas de cálculo de stats)
  - PokemonFactory (factory estático)
  - PokemonInstanceBuilder (builder fluido)
  - NatureData (tablas de modificadores de naturalezas)
- **1.13**: Registry System
  - IDataRegistry<T>, GameDataRegistry<T>
  - IPokemonRegistry, IMoveRegistry
  - PokemonRegistry, MoveRegistry

#### Grupo E: Planned Features (Futuro)
- **1.14**: Variants System (Mega/Dinamax/Tera) - Planned
- **1.15**: Pokedex Fields (Description, Height, Weight, etc.) - Planned

### Opción 2: Mantener Feature 1 como "Pokemon Data" y crear Feature 1.X para otros datos

**Feature 1: Pokemon Data**
- **1.1**: PokemonSpeciesData (Blueprint)
- **1.2**: PokemonInstance (Runtime)
- **1.3**: Variants System - Planned
- **1.4**: Pokedex Fields - Planned

**Feature 1.5: Move Data** (nueva sub-feature)
- MoveData (Blueprint)
- MoveInstance (Runtime)
- Move Effects (IMoveEffect y todas las implementaciones)

**Feature 1.6: Ability Data** (nueva sub-feature)
- AbilityData (Blueprint)

**Feature 1.7: Item Data** (nueva sub-feature)
- ItemData (Blueprint)

**Feature 1.8: Status Effect Data** (nueva sub-feature)
- StatusEffectData (Blueprint)

**Feature 1.9: Field Condition Data** (nueva sub-feature)
- WeatherData
- TerrainData
- HazardData
- SideConditionData
- FieldEffectData

**Feature 1.10: Evolution System** (nueva sub-feature)
- Evolution
- EvolutionConditions

**Feature 1.11: Registry System** (nueva sub-feature)
- IDataRegistry
- PokemonRegistry
- MoveRegistry
- GameDataRegistry

**Feature 1.12: Builders** (nueva sub-feature)
- PokemonBuilder
- MoveBuilder
- AbilityBuilder
- ItemBuilder
- etc.

**Feature 1.13: Enums** (nueva sub-feature)
- PokemonType
- Stat
- Nature
- Gender
- etc.

## Recomendación

**Opción 1** es mejor porque:
1. Es más claro: "Game Data" describe mejor lo que contiene
2. Es más consistente: todos los datos de juego están en un solo lugar
3. Es más fácil de mantener: no hay confusión sobre qué va dónde
4. Feature 2 puede enfocarse solo en lógica de combate, no en datos
5. **La organización por grupos lógicos** (Core Entities, Field/Status, Supporting Systems, Infrastructure) hace más fácil navegar y entender la estructura

## Análisis de Organización de Sub-Features

### Problemas con la Organización Actual Propuesta

1. **Field Conditions dispersas**: Weather, Terrain, Hazard, SideCondition y FieldEffect están separadas (1.6-1.10) cuando conceptualmente son todas "condiciones de campo"
2. **Infraestructura mezclada con datos**: Builders, Registries, Factories están mezclados con los datos principales, dificultando distinguir entre "qué son los datos" vs "cómo se crean/gestionan"
3. **No hay agrupación lógica**: No hay separación clara entre entidades principales, condiciones de campo, sistemas de soporte e infraestructura

### Ventajas de la Nueva Organización por Grupos

1. **Grupo A: Core Entity Data** - Las entidades principales del juego (Pokemon, Moves, Abilities, Items)
2. **Grupo B: Field & Status Data** - Condiciones que afectan el campo de batalla y estado de Pokemon
3. **Grupo C: Supporting Systems** - Sistemas que soportan los datos pero no son datos en sí (Evolution, Type Effectiveness)
4. **Grupo D: Infrastructure** - Herramientas para crear, gestionar y acceder a los datos (Interfaces, Enums, Builders, Factories, Registries)
5. **Grupo E: Planned** - Features futuras

Esta organización:
- **Separa claramente** datos de infraestructura
- **Agrupa conceptos relacionados** (Field Conditions juntas)
- **Facilita la navegación** al saber en qué grupo buscar
- **Escala mejor** cuando se añadan nuevos tipos de datos

## Acciones Necesarias

1. ✅ Renombrar Feature 1 de "Pokemon Data" a "Game Data"
2. ✅ Actualizar features_master_list.md con nueva estructura organizada por grupos
3. ✅ Mover WeatherData, TerrainData, HazardData, SideConditionData, FieldEffectData de Feature 2 a Feature 1
4. ✅ **Corregir categorización de Move Effects**: Cambiar de "1.1: PokemonSpeciesData" a "1.2: Move Data"
5. ✅ **Mover TypeEffectiveness**: De Feature 2.4 → Feature 1.8 (Type Effectiveness Table)
   - **Nota**: La tabla de datos va a Feature 1, pero la lógica de cálculo puede quedarse en Feature 2
6. ✅ **Mover GameMessages**: De Feature 2.5 → Feature 1.10 (Constants)
   - **Nota**: Son mensajes de texto del juego, no lógica de combate
7. ✅ Actualizar todos los comentarios de código para reflejar nueva estructura
   - Move Effects (22 archivos): De "1.1: PokemonSpeciesData" → "1.2: Move Data"
   - WeatherData, TerrainData, etc. (5 archivos): De Feature 2 → Feature 1.6 (Field Conditions Data)
   - TypeEffectiveness: De Feature 2.4 → Feature 1.8
   - GameMessages: De Feature 2.5 → Feature 1.10
8. ✅ Actualizar architecture.md de Feature 1 para incluir todos los datos organizados por grupos
9. ✅ Actualizar code_location.md para reflejar nueva estructura
10. ✅ Crear sub-features documentadas para cada tipo de dato
11. ✅ Documentar enums dentro de Effects (7 enums adicionales)
12. ✅ Documentar clases estáticas helper en Builders (10 clases)

## Estructura Propuesta Final (Organizada por Grupos)

```
Feature 1: Game Data
│
├── Grupo A: Core Entity Data (Entidades Principales)
│   ├── 1.1: Pokemon Data
│   │   ├── PokemonSpeciesData (Blueprint)
│   │   ├── PokemonInstance (Runtime)
│   │   ├── BaseStats (estructura)
│   │   └── LearnableMove (estructura)
│   ├── 1.2: Move Data
│   │   ├── MoveData (Blueprint)
│   │   ├── MoveInstance (Runtime con PP tracking)
│   │   └── Move Effects (IMoveEffect y 22 implementaciones)
│   ├── 1.3: Ability Data
│   │   └── AbilityData (Blueprint)
│   └── 1.4: Item Data
│       └── ItemData (Blueprint)
│
├── Grupo B: Field & Status Data (Condiciones de Campo)
│   ├── 1.5: Status Effect Data
│   │   └── StatusEffectData (Blueprint)
│   └── 1.6: Field Conditions Data
│       ├── WeatherData
│       ├── TerrainData
│       ├── HazardData
│       ├── SideConditionData
│       └── FieldEffectData
│
├── Grupo C: Supporting Systems (Sistemas de Soporte)
│   ├── 1.7: Evolution System
│   │   ├── Evolution
│   │   ├── IEvolutionCondition
│   │   └── EvolutionConditions (6 clases)
│   └── 1.8: Type Effectiveness Table
│       └── TypeEffectiveness (tabla de datos)
│
├── Grupo D: Infrastructure (Infraestructura)
│   ├── 1.9: Interfaces Base
│   │   └── IIdentifiable
│   ├── 1.10: Enums & Constants
│   │   ├── Enums principales (20)
│   │   ├── Enums dentro de Effects (7)
│   │   └── Constants (ErrorMessages, GameMessages)
│   ├── 1.11: Builders
│   │   ├── Builders (13 clases)
│   │   └── Clases estáticas helper (10)
│   ├── 1.12: Factories & Calculators
│   │   ├── StatCalculator
│   │   ├── PokemonFactory
│   │   ├── PokemonInstanceBuilder
│   │   └── NatureData
│   └── 1.13: Registry System
│       ├── IDataRegistry<T>
│       ├── GameDataRegistry<T>
│       └── Registries especializados (PokemonRegistry, MoveRegistry)
│
└── Grupo E: Planned Features
    ├── 1.14: Variants System (Planned)
    └── 1.15: Pokedex Fields (Planned)
```

---

**Fecha**: 2025-01-XX
**Autor**: Revisión de código y documentación
