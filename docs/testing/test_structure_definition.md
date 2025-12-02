# Definición de Estructura de Tests

## Filosofía

Los tests deben organizarse en tres categorías principales:

1. **Systems/** - Tests del core del funcionamiento (CÓMO funcionan los sistemas)
2. **Blueprints/** - Tests de estructura de datos (CÓMO son los datos)
3. **Data/** - Tests de contenido específico (QUÉ contienen los datos)

## Tipos de Tests

### 1. Tests Funcionales (Normales)
- **Nomenclatura**: `*Tests.cs`
- **Propósito**: Testear el comportamiento normal y esperado
- **Cuándo usar**: Para verificar que la funcionalidad básica funciona correctamente
- **Ejemplos**: 
  - `DamagePipelineTests.cs` - Testea que el pipeline calcula daño correctamente
  - `PokemonFactoryTests.cs` - Testea que la factory crea Pokemon correctamente

### 2. Edge Cases Tests
- **Nomenclatura**: `*EdgeCasesTests.cs`
- **Propósito**: Testear casos límite, valores extremos, condiciones especiales
- **Cuándo usar**: Para verificar manejo de casos especiales, valores límite, errores esperados
- **Ejemplos**:
  - `DamagePipelineEdgeCasesTests.cs` - Testea valores mínimos/máximos, divisiones por cero
  - `PokemonInstanceEdgeCasesTests.cs` - Testea HP=0, stats negativos, etc.

### 3. Integration Tests
- **Nomenclatura**: `*IntegrationTests.cs`
- **Propósito**: Testear la integración entre múltiples sistemas o componentes
- **Cuándo usar**: Cuando se necesita verificar que varios sistemas trabajan juntos correctamente
- **Ubicación**: Siempre en `Systems/*/Integration/` o como parte del sistema si es integración simple
- **Ejemplos**:
  - `DamagePipelineIntegrationTests.cs` - Testea pipeline con UseMoveAction y DamageAction
  - `FullBattleTests.cs` - Testea integración completa del sistema de combate

## Estructura Completa

```
PokemonUltimate.Tests/
│
├── Systems/                         # Tests de sistemas (core del funcionamiento)
│   │                               # Cada sistema debe tener sus tests
│   │
│   ├── Combat/                     # Sistema de combate
│   │   │
│   │   ├── Field/                  # BattleField, BattleSlot, BattleSide
│   │   │   ├── BattleFieldTests.cs
│   │   │   ├── BattleFieldEdgeCasesTests.cs
│   │   │   ├── BattleSlotTests.cs
│   │   │   ├── BattleSlotEdgeCasesTests.cs
│   │   │   ├── BattleSideTests.cs
│   │   │   └── BattleSideEdgeCasesTests.cs
│   │   │
│   │   ├── Queue/                  # BattleQueue
│   │   │   ├── BattleQueueTests.cs
│   │   │   └── BattleQueueEdgeCasesTests.cs
│   │   │
│   │   ├── Actions/                # Implementaciones de BattleAction
│   │   │   ├── MessageActionTests.cs
│   │   │   ├── ApplyStatusActionTests.cs
│   │   │   ├── ApplyStatusActionEdgeCasesTests.cs
│   │   │   ├── DamageActionTests.cs
│   │   │   ├── DamageActionEdgeCasesTests.cs
│   │   │   ├── FaintActionTests.cs
│   │   │   ├── FaintActionEdgeCasesTests.cs
│   │   │   ├── HealActionTests.cs
│   │   │   ├── HealActionEdgeCasesTests.cs
│   │   │   ├── StatChangeActionTests.cs
│   │   │   ├── StatChangeActionEdgeCasesTests.cs
│   │   │   ├── SwitchActionTests.cs
│   │   │   ├── SwitchActionEdgeCasesTests.cs
│   │   │   ├── UseMoveActionTests.cs
│   │   │   └── UseMoveActionEdgeCasesTests.cs
│   │   │
│   │   ├── Effects/                # Efectos específicos de combate
│   │   │   ├── RecoilDrainEffectTests.cs
│   │   │   └── RecoilDrainEffectEdgeCasesTests.cs
│   │   │
│   │   ├── Damage/                 # Sistema de cálculo de daño
│   │   │   ├── DamagePipelineTests.cs
│   │   │   ├── DamagePipelineEdgeCasesTests.cs
│   │   │   ├── StatModifierTests.cs
│   │   │   └── StatModifierEdgeCasesTests.cs
│   │   │
│   │   ├── Engine/                 # CombatEngine, EndOfTurnProcessor
│   │   │   ├── CombatEngineTests.cs
│   │   │   ├── CombatEngineEdgeCasesTests.cs
│   │   │   ├── EndOfTurnProcessorTests.cs
│   │   │   └── EndOfTurnProcessorEdgeCasesTests.cs
│   │   │
│   │   ├── Helpers/                # Helpers del sistema de combate
│   │   │   ├── TurnOrderResolverTests.cs
│   │   │   └── TurnOrderResolverEdgeCasesTests.cs
│   │   │
│   │   ├── Arbiter/                # BattleArbiter
│   │   │   ├── BattleArbiterTests.cs
│   │   │   └── BattleArbiterEdgeCasesTests.cs
│   │   │
│   │   ├── AI/                     # Proveedores de AI
│   │   │   ├── AlwaysAttackAITests.cs
│   │   │   └── RandomAITests.cs
│   │   │
│   │   ├── Providers/              # Proveedores de input
│   │   │   ├── PlayerInputProviderTests.cs
│   │   │   └── PlayerInputProviderEdgeCasesTests.cs
│   │   │
│   │   ├── Events/                 # Sistema de eventos
│   │   │   └── BattleTriggerProcessorTests.cs
│   │   │
│   │   └── Integration/           # Tests de integración entre sistemas
│   │       │                       # Tests que verifican múltiples sistemas trabajando juntos
│   │       │
│   │       ├── Actions/            # Integración de acciones con otros sistemas
│   │       │   ├── ActionSystemIntegrationTests.cs
│   │       │   ├── HealActionIntegrationTests.cs
│   │       │   ├── RecoilDrainIntegrationTests.cs
│   │       │   ├── StatChangesIntegrationTests.cs
│   │       │   ├── StatusEffectsIntegrationTests.cs
│   │       │   └── SwitchActionIntegrationTests.cs
│   │       │
│   │       ├── Damage/             # Integración de daño con acciones y otros sistemas
│   │       │   ├── DamagePipelineIntegrationTests.cs
│   │       │   └── StatModifierIntegrationTests.cs
│   │       │
│   │       ├── Engine/             # Integración del engine con otros sistemas
│   │       │   ├── EndOfTurnIntegrationTests.cs
│   │       │   └── TurnOrderIntegrationTests.cs
│   │       │
│   │       └── System/             # Integración completa del sistema de combate
│   │           ├── AbilitiesItemsIntegrationTests.cs
│   │           ├── AccuracyIntegrationTests.cs
│   │           ├── BattleArbiterIntegrationTests.cs
│   │           ├── FullBattleTests.cs              # Tests funcionales de batallas completas
│   │           ├── FullBattleEdgeCasesTests.cs     # Edge cases de batallas completas
│   │           ├── PlayerInputIntegrationTests.cs
│   │           └── TargetResolverIntegrationTests.cs
│   │
│   ├── Core/                       # Sistemas core
│   │   │
│   │   ├── Factories/              # Sistema de factories
│   │   │   ├── PokemonFactoryTests.cs
│   │   │   ├── PokemonInstanceBuilderTests.cs
│   │   │   ├── StatCalculatorTests.cs
│   │   │   ├── TypeEffectivenessTests.cs
│   │   │   └── TypeEffectivenessEdgeCasesTests.cs
│   │   │
│   │   ├── Instances/              # Sistema de instancias
│   │   │   ├── PokemonInstanceTests.cs
│   │   │   ├── PokemonInstanceAbilityTests.cs
│   │   │   ├── PokemonInstanceAbilityEdgeCasesTests.cs
│   │   │   ├── MoveInstanceTests.cs
│   │   │   ├── LevelUpEvolutionEdgeCasesTests.cs
│   │   │   └── StatsAndMovesEdgeCasesTests.cs
│   │   │
│   │   ├── Evolution/              # Sistema de evolución
│   │   │   ├── EvolutionTests.cs
│   │   │   ├── EvolutionConditionTests.cs
│   │   │   └── EvolutionChainsEdgeCasesTests.cs
│   │   │
│   │   └── Registry/               # Sistema de registry
│   │       ├── PokemonRegistryTests.cs
│   │       ├── PokemonRegistryPokedexTests.cs
│   │       ├── MoveRegistryTests.cs
│   │       ├── MoveRegistryFilterTests.cs
│   │       └── RegistryEdgeCasesTests.cs
│   │
│   └── Effects/                    # Sistema de efectos
│       │                           # Tests de comportamiento de efectos
│       │
│       ├── Basic/                  # Efectos básicos (comportamiento)
│       │   ├── MoveEffectTests.cs
│       │   ├── EffectsEdgeCasesTests.cs
│       │   └── MoveEffectCompositionTests.cs
│       │
│       └── Advanced/               # Efectos avanzados (comportamiento)
│           ├── VolatileStatusEffectTests.cs
│           ├── ProtectionEffectTests.cs
│           ├── ChargingEffectTests.cs
│           ├── ForceSwitchEffectTests.cs
│           ├── BindingEffectTests.cs
│           ├── SwitchAfterAttackEffectTests.cs
│           ├── FieldConditionEffectTests.cs
│           ├── SelfDestructEffectTests.cs
│           ├── RevengeEffectTests.cs
│           ├── MoveRestrictionEffectTests.cs
│           ├── DelayedDamageEffectTests.cs
│           └── PriorityModifierEffectTests.cs
│
├── Blueprints/                      # Tests de estructura de datos
│   │                               # Tests de CÓMO son los datos (estructura)
│   │                               # Tests funcionales y edge cases de estructuras
│   │
│   ├── PokemonSpeciesDataTests.cs
│   ├── MoveDataTests.cs
│   ├── MoveDataFlagsTests.cs
│   ├── BaseStatsTests.cs
│   ├── BaseStatsEdgeCasesTests.cs
│   ├── AbilityDataTests.cs
│   ├── ItemDataTests.cs
│   ├── NatureDataTests.cs
│   ├── StatusEffectDataTests.cs
│   ├── StatusEffectDataEdgeCasesTests.cs
│   ├── WeatherDataTests.cs
│   ├── WeatherDataEdgeCasesTests.cs
│   ├── TerrainDataTests.cs
│   ├── TerrainDataEdgeCasesTests.cs
│   ├── FieldEffectDataTests.cs
│   ├── HazardDataTests.cs
│   └── SideConditionDataTests.cs
│
└── Data/                            # Tests de contenido específico
    │                               # Tests de QUÉ contienen los datos (contenido)
    │
    ├── Validation/                  # Validación contra datos oficiales
    │   │                           # Tests que comparan nuestros datos con juegos oficiales
    │   │
    │   ├── StatCalculatorValidationTests.cs      # Valida cálculos contra juegos oficiales
    │   └── DamageCalculationValidationTests.cs   # Valida daño contra juegos oficiales
    │
    ├── Pokemon/                     # Tests de Pokemon específicos
    │   │                           # Un archivo por cada Pokemon del catálogo
    │   │                           # Permite ver rápidamente los tests de cada Pokemon
    │   │
    │   ├── PikachuTests.cs                       # Tests específicos de Pikachu
    │   ├── CharizardTests.cs                     # Tests específicos de Charizard
    │   ├── BlastoiseTests.cs                     # Tests específicos de Blastoise
    │   ├── VenusaurTests.cs                      # Tests específicos de Venusaur
    │   ├── GengarTests.cs                        # Tests específicos de Gengar
    │   ├── GolemTests.cs                         # Tests específicos de Golem
    │   ├── GyaradosTests.cs                      # Tests específicos de Gyarados
    │   ├── AlakazamTests.cs                      # Tests específicos de Alakazam
    │   └── [un archivo por cada Pokemon]          # Un archivo por cada Pokemon del catálogo
    │
    ├── Moves/                       # Tests de Moves específicos
    │   │                           # Un archivo por cada Move del catálogo
    │   │                           # Permite ver rápidamente los tests de cada Move
    │   │
    │   ├── TackleTests.cs                        # Tests específicos de Tackle
    │   ├── FlamethrowerTests.cs                  # Tests específicos de Flamethrower
    │   ├── ThunderboltTests.cs                   # Tests específicos de Thunderbolt
    │   ├── WaterGunTests.cs                      # Tests específicos de Water Gun
    │   ├── DragonRageTests.cs                    # Tests específicos de Dragon Rage
    │   ├── ShadowBallTests.cs                    # Tests específicos de Shadow Ball
    │   └── [un archivo por cada Move]             # Un archivo por cada Move del catálogo
    │
    ├── Catalogs/                    # Tests generales de catálogos
    │   │                           # Tests que verifican el catálogo completo
    │   │
    │   ├── PokemonCatalogTests.cs                # Tests generales del catálogo de Pokemon
    │   ├── PokemonCatalogValidationTests.cs       # Validación de datos oficiales de Pokemon
    │   ├── MoveCatalogTests.cs                   # Tests generales del catálogo de Moves
    │   ├── Items/                                 # Tests de Items (cuando se implementen)
    │   │   └── ItemCatalogTests.cs               # Tests generales del catálogo de Items
    │   └── Abilities/                             # Tests de Abilities (cuando se implementen)
    │       └── AbilityCatalogTests.cs             # Tests generales del catálogo de Abilities
    │
    ├── Builders/                    # Tests de contenido creado por builders
    │   │                           # Tests de QUÉ crean los builders
    │   │                           # Tests funcionales y edge cases
    │   │
    │   ├── PokemonBuilderTests.cs
    │   ├── MoveBuilderTests.cs
    │   ├── EffectBuilderTests.cs
    │   ├── EvolutionBuilderTests.cs
    │   ├── LearnsetBuilderTests.cs
    │   └── BuilderEdgeCasesTests.cs
    │
    └── Models/                      # Tests de contenido de modelos
        ├── LearnableMoveTests.cs
        └── LearnsetEdgeCasesTests.cs
```

## Reglas de Organización

### 1. Tests Funcionales (Normales)
- **Ubicación**: Junto al componente que testean
- **Nomenclatura**: `[Component]Tests.cs`
- **Ejemplos**:
  - `Systems/Combat/Damage/DamagePipelineTests.cs`
  - `Systems/Core/Factories/PokemonFactoryTests.cs`
  - `Blueprints/PokemonSpeciesDataTests.cs`

### 2. Edge Cases Tests
- **Ubicación**: Junto a los tests funcionales del mismo componente
- **Nomenclatura**: `[Component]EdgeCasesTests.cs`
- **Cuándo crear**: Cuando hay casos límite significativos que merecen archivo separado
- **Ejemplos**:
  - `Systems/Combat/Damage/DamagePipelineEdgeCasesTests.cs`
  - `Blueprints/BaseStatsEdgeCasesTests.cs`
  - `Data/Builders/BuilderEdgeCasesTests.cs`

### 3. Integration Tests
- **Ubicación**: Siempre en `Systems/[Module]/Integration/`
- **Nomenclatura**: `[Component]IntegrationTests.cs` o `[System]IntegrationTests.cs`
- **Cuándo crear**: Cuando se testea la integración entre múltiples sistemas/componentes
- **Estructura**: Organizados por tipo de integración (Actions/, Damage/, Engine/, System/)
- **Ejemplos**:
  - `Systems/Combat/Integration/Damage/DamagePipelineIntegrationTests.cs`
  - `Systems/Combat/Integration/Actions/HealActionIntegrationTests.cs`
  - `Systems/Combat/Integration/System/FullBattleTests.cs`

## Principios de Organización

### Systems/ - Tests de Sistemas
- **Propósito**: Testear CÓMO funcionan los sistemas
- **Criterio**: Cada sistema debe tener sus tests
- **Estructura**: 
  - Tests funcionales y edge cases en el directorio del sistema
  - Integration tests en `Integration/` subdirectorio
- **Ejemplos**: 
  - `Combat/Damage/DamagePipelineTests.cs` - Testea cómo funciona el pipeline
  - `Combat/Damage/DamagePipelineEdgeCasesTests.cs` - Testea casos límite
  - `Combat/Integration/Damage/DamagePipelineIntegrationTests.cs` - Testea integración

### Blueprints/ - Tests de Estructura
- **Propósito**: Testear CÓMO son los datos (estructura)
- **Criterio**: Un test por tipo de blueprint
- **Estructura**: Archivos planos, tests funcionales y edge cases
- **Ejemplos**:
  - `PokemonSpeciesDataTests.cs` - Testea estructura de PokemonSpeciesData
  - `BaseStatsEdgeCasesTests.cs` - Testea casos límite de BaseStats

### Data/ - Tests de Contenido
- **Propósito**: Testear QUÉ contienen los datos (contenido específico)
- **Criterio**: Un archivo por cada elemento del catálogo (Pokemon, Move, Item, Ability)
- **Estructura**: 
  - Un archivo por cada elemento para ver rápidamente sus tests
  - Tests funcionales de contenido específico
  - Edge cases cuando sea necesario
  - Validation tests separados
- **Ejemplos**:
  - `Data/Pokemon/PikachuTests.cs` - Tests específicos de Pikachu
  - `Data/Moves/FlamethrowerTests.cs` - Tests específicos de Flamethrower
  - `Data/Catalogs/PokemonCatalogTests.cs` - Tests generales del catálogo de Pokemon
  - `Data/Validation/StatCalculatorValidationTests.cs` - Valida contra datos oficiales

## Cuándo Usar Cada Tipo

### Tests Funcionales (Normales)
✅ **Usar cuando**:
- Testeas comportamiento básico y esperado
- Verificas que la funcionalidad funciona correctamente
- Testeas casos comunes y típicos

❌ **NO usar cuando**:
- Testeas integración entre sistemas (usa Integration)
- Testeas casos límite complejos (usa EdgeCases)

### Edge Cases Tests
✅ **Usar cuando**:
- Hay casos límite significativos (valores mínimos/máximos)
- Hay condiciones especiales que merecen tests separados
- Hay manejo de errores o validaciones importantes

❌ **NO usar cuando**:
- Los casos límite son simples (inclúyelos en tests funcionales)
- Testeas integración (usa Integration)

### Integration Tests
✅ **Usar cuando**:
- Testeas interacción entre múltiples sistemas
- Testeas flujos completos que involucran varios componentes
- Verificas que sistemas trabajan juntos correctamente

❌ **NO usar cuando**:
- Testeas un solo componente aislado (usa tests funcionales)
- Testeas casos límite de un componente (usa EdgeCases)

## Ventajas de esta Estructura

1. **Claridad**: Separación clara entre sistemas, estructura y contenido
2. **Tipos claros**: Fácil identificar tests funcionales, edge cases e integration
3. **Cobertura**: Fácil verificar qué está testeado
4. **Escalabilidad**: Fácil añadir nuevos tests siguiendo el patrón
5. **Mantenibilidad**: Cada tipo de test tiene su lugar claro
6. **Sistemático**: Permite testear sistemáticamente:
   - ✅ Cada sistema (Systems/)
   - ✅ Cada estructura de datos (Blueprints/)
   - ✅ Cada tipo de contenido importante (Data/)
