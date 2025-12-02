# Test Organization Proposal

## Current State Analysis

**Total: 117 test files**

### Distribution by Module:
- **Combat**: 62 archivos (53% del total) - **Necesita más organización**
- **Blueprints**: 13 archivos
- **Builders**: 6 archivos
- **Catalogs**: 7 archivos
- **Effects**: 4 archivos
- **Evolution**: 3 archivos
- **Factories**: 6 archivos
- **Instances**: 6 archivos
- **Models**: 6 archivos
- **Registry**: 5 archivos
- **Pokemon**: 1 archivo (debería estar en Catalogs)

## Current Problems

### Combat Module (62 archivos - el más problemático):

1. **Archivos sueltos en raíz de Combat/**:
   - `BattleFieldTests.cs`, `BattleSlotTests.cs`, `BattleSideTests.cs`, `BattleQueueTests.cs`
   - `CombatEngineTests.cs`, `BattleArbiterTests.cs`
   - `TurnOrderResolverTests.cs`
   - Deberían estar en subdirectorios organizados

2. **RecoilDrainEffectTests** está en `Combat/Actions` pero testea efectos, no acciones específicas
   - Debería estar en `Combat/Effects/` o `Effects/`

3. **Integration/** tiene 16 archivos sin organización:
   - Mezcla tests de Actions, Damage, Engine, etc.
   - Debería agruparse por sistema

4. **Damage/** tiene estructura inconsistente:
   - `DamageVerificationTests.cs` tiene nombre poco claro
   - `Integration/StatModifierIntegrationTests.cs` está dentro de Damage pero debería estar en Integration/Damage/

5. **EdgeCaseTests.cs** en Integration es muy genérico

### Otros Módulos:

6. **Models/** tiene tests que podrían estar mejor organizados:
   - `BaseStatsTests.cs` podría estar en `Blueprints/`
   - `MoveDataTests.cs` podría estar en `Blueprints/`
   - `PokemonSpeciesDataTests.cs` podría estar en `Blueprints/`

7. **Pokemon/** tiene solo 1 archivo:
   - `PokemonCatalogValidationTests.cs` debería estar en `Catalogs/Pokemon/`

8. **Inconsistencia en EdgeCases**:
   - Algunos módulos tienen `*EdgeCasesTests.cs` separados
   - Otros tienen edge cases dentro del mismo archivo

## Proposed Complete Structure

### High-Level Organization

```
PokemonUltimate.Tests/
├── Blueprints/              # Tests for immutable data structures
│   ├── AbilityDataTests.cs
│   ├── BaseStatsTests.cs          # Moved from Models/
│   ├── BaseStatsEdgeCasesTests.cs
│   ├── FieldEffectDataTests.cs
│   ├── HazardDataTests.cs
│   ├── ItemDataTests.cs
│   ├── MoveDataTests.cs           # Moved from Models/
│   ├── MoveDataFlagsTests.cs
│   ├── PokemonSpeciesDataTests.cs # Moved from Models/
│   ├── NatureDataTests.cs         # Moved from Models/
│   ├── SideConditionDataTests.cs
│   ├── StatusEffectDataTests.cs
│   ├── StatusEffectDataEdgeCasesTests.cs
│   ├── TerrainDataTests.cs
│   ├── TerrainDataEdgeCasesTests.cs
│   ├── WeatherDataTests.cs
│   └── WeatherDataEdgeCasesTests.cs
│
├── Builders/                # Tests for fluent builders
│   ├── BuilderEdgeCasesTests.cs
│   ├── EffectBuilderTests.cs
│   ├── EvolutionBuilderTests.cs
│   ├── LearnsetBuilderTests.cs
│   ├── MoveBuilderTests.cs
│   └── PokemonBuilderTests.cs
│
├── Catalogs/               # Tests for content catalogs
│   ├── Moves/
│   │   ├── MoveCatalogTests.cs
│   │   ├── MoveCatalogElectricTests.cs
│   │   ├── MoveCatalogFireTests.cs
│   │   ├── MoveCatalogNormalTests.cs
│   │   └── MoveCatalogOtherTypesTests.cs
│   └── Pokemon/
│       ├── PokemonCatalogTests.cs
│       ├── PokemonCatalogGen1Tests.cs
│       └── PokemonCatalogValidationTests.cs  # Moved from Pokemon/
│
├── Effects/                # Tests for move effects
│   ├── MoveEffectTests.cs
│   ├── MoveEffectCompositionTests.cs
│   ├── EffectsEdgeCasesTests.cs
│   ├── NewEffectsTests.cs
│   ├── RecoilDrainEffectTests.cs        # Moved from Combat/Actions/
│   └── RecoilDrainEffectEdgeCasesTests.cs # Moved from Combat/Actions/
│
├── Evolution/             # Tests for evolution system
│   ├── EvolutionTests.cs
│   ├── EvolutionConditionTests.cs
│   └── EvolutionChainsEdgeCasesTests.cs
│
├── Factories/             # Tests for factory classes
│   ├── PokemonFactoryTests.cs
│   ├── PokemonInstanceBuilderTests.cs
│   ├── RealPokemonStatsTests.cs
│   ├── StatCalculatorTests.cs
│   ├── TypeEffectivenessTests.cs
│   └── TypeEffectivenessEdgeCasesTests.cs
│
├── Instances/             # Tests for runtime instances
│   ├── PokemonInstanceTests.cs
│   ├── PokemonInstanceAbilityTests.cs
│   ├── PokemonInstanceAbilityEdgeCasesTests.cs
│   ├── MoveInstanceTests.cs
│   ├── LevelUpEvolutionEdgeCasesTests.cs
│   └── StatsAndMovesEdgeCasesTests.cs
│
├── Models/                # Tests for data models (reduced)
│   ├── BaseStatsTests.cs         # Keep here OR move to Blueprints?
│   ├── LearnableMoveTests.cs
│   └── LearnsetEdgeCasesTests.cs
│
├── Registry/              # Tests for registry system
│   ├── PokemonRegistryTests.cs
│   ├── PokemonRegistryPokedexTests.cs
│   ├── MoveRegistryTests.cs
│   ├── MoveRegistryFilterTests.cs
│   └── RegistryEdgeCasesTests.cs
│
└── Combat/                # Tests for combat system (REORGANIZED)
    ├── Field/             # BattleField, BattleSlot, BattleSide (NEW)
    │   ├── BattleFieldTests.cs
    │   ├── BattleFieldEdgeCasesTests.cs
    │   ├── BattleSlotTests.cs
    │   ├── BattleSlotEdgeCasesTests.cs
    │   ├── BattleSideTests.cs
    │   └── BattleSideEdgeCasesTests.cs
    │
    ├── Queue/             # BattleQueue (NEW)
    │   ├── BattleQueueTests.cs
    │   └── BattleQueueEdgeCasesTests.cs
    │
    ├── Actions/           # BattleAction implementations
    │   ├── BattleActionTests.cs
    │   ├── ApplyStatusActionTests.cs
    │   ├── ApplyStatusActionEdgeCasesTests.cs
    │   ├── DamageActionTests.cs
    │   ├── DamageActionEdgeCasesTests.cs
    │   ├── FaintActionTests.cs
    │   ├── FaintActionEdgeCasesTests.cs
    │   ├── HealActionTests.cs
    │   ├── HealActionEdgeCasesTests.cs
    │   ├── StatChangeActionTests.cs
    │   ├── StatChangeActionEdgeCasesTests.cs
    │   ├── SwitchActionTests.cs
    │   ├── SwitchActionEdgeCasesTests.cs
    │   ├── UseMoveActionTests.cs
    │   └── UseMoveActionEdgeCasesTests.cs
    │
    ├── Effects/           # Combat-specific effect tests (NEW)
    │   ├── RecoilDrainEffectTests.cs        # Moved from Actions/
    │   └── RecoilDrainEffectEdgeCasesTests.cs
    │
    ├── Damage/            # Damage calculation tests
    │   ├── DamagePipelineTests.cs
    │   ├── DamagePipelineEdgeCasesTests.cs
    │   ├── DamageCalculationVerificationTests.cs  # Renamed
    │   ├── StatModifierTests.cs
    │   └── StatModifierEdgeCasesTests.cs
    │
    ├── Engine/            # CombatEngine, EndOfTurnProcessor
    │   ├── CombatEngineTests.cs
    │   ├── CombatEngineEdgeCasesTests.cs
    │   ├── EndOfTurnProcessorTests.cs
    │   └── EndOfTurnProcessorEdgeCasesTests.cs
    │
    ├── Helpers/           # Helper classes (NEW)
    │   ├── TurnOrderResolverTests.cs
    │   └── TurnOrderResolverEdgeCasesTests.cs
    │
    ├── Arbiter/           # BattleArbiter (NEW)
    │   ├── BattleArbiterTests.cs
    │   └── BattleArbiterEdgeCasesTests.cs
    │
    ├── AI/                # AI providers
    │   ├── AlwaysAttackAITests.cs
    │   └── RandomAITests.cs
    │
    ├── Providers/         # Input providers
    │   ├── PlayerInputProviderTests.cs
    │   └── PlayerInputProviderEdgeCasesTests.cs
    │
    ├── Events/            # Event system
    │   └── BattleTriggerProcessorTests.cs
    │
    └── Integration/       # Integration tests (REORGANIZED)
        ├── Actions/      # Action integration tests (NEW)
        │   ├── ActionSystemIntegrationTests.cs
        │   ├── RecoilDrainIntegrationTests.cs
        │   ├── StatusEffectsIntegrationTests.cs
        │   ├── StatChangesIntegrationTests.cs
        │   ├── SwitchActionIntegrationTests.cs
        │   └── HealActionIntegrationTests.cs
        │
        ├── Damage/       # Damage integration tests (NEW)
        │   ├── DamagePipelineIntegrationTests.cs
        │   └── StatModifierIntegrationTests.cs  # Moved from Damage/Integration/
        │
        ├── Engine/       # Engine integration tests (NEW)
        │   ├── EndOfTurnIntegrationTests.cs
        │   └── TurnOrderIntegrationTests.cs
        │
        └── System/       # Full system integration (NEW)
            ├── AbilitiesItemsIntegrationTests.cs
            ├── AccuracyIntegrationTests.cs
            ├── BattleArbiterIntegrationTests.cs
            ├── FullBattleTests.cs
            ├── PlayerInputIntegrationTests.cs
            ├── TargetResolverIntegrationTests.cs
            └── ComplexBattleScenariosTests.cs  # Renamed from EdgeCaseTests.cs
```

## Proposed Structure

```
PokemonUltimate.Tests/Combat/
├── Actions/                          # Tests for BattleAction implementations
│   ├── ApplyStatusActionTests.cs
│   ├── ApplyStatusActionEdgeCasesTests.cs
│   ├── DamageActionTests.cs
│   ├── DamageActionEdgeCasesTests.cs
│   ├── FaintActionTests.cs
│   ├── FaintActionEdgeCasesTests.cs
│   ├── HealActionTests.cs
│   ├── HealActionEdgeCasesTests.cs
│   ├── StatChangeActionTests.cs
│   ├── StatChangeActionEdgeCasesTests.cs
│   ├── SwitchActionTests.cs
│   ├── SwitchActionEdgeCasesTests.cs
│   ├── UseMoveActionTests.cs
│   └── UseMoveActionEdgeCasesTests.cs
│
├── Effects/                          # Tests for move effects (NEW)
│   ├── RecoilDrainEffectTests.cs    # Moved from Actions
│   ├── RecoilDrainEffectEdgeCasesTests.cs
│   └── [Future effect tests]
│
├── Damage/                           # Tests for damage calculation
│   ├── DamagePipelineTests.cs
│   ├── DamagePipelineEdgeCasesTests.cs
│   ├── DamageCalculationVerificationTests.cs  # Renamed from DamageVerificationTests
│   ├── StatModifierTests.cs
│   └── StatModifierEdgeCasesTests.cs
│
├── Integration/                      # Integration tests
│   ├── Actions/                     # Action integration tests (NEW)
│   │   ├── ActionSystemIntegrationTests.cs
│   │   ├── RecoilDrainIntegrationTests.cs
│   │   ├── StatusEffectsIntegrationTests.cs
│   │   └── StatChangesIntegrationTests.cs
│   │
│   ├── Damage/                     # Damage integration tests (NEW)
│   │   ├── DamagePipelineIntegrationTests.cs
│   │   └── StatModifierIntegrationTests.cs
│   │
│   ├── Engine/                     # Engine integration tests (NEW)
│   │   ├── CombatEngineIntegrationTests.cs
│   │   ├── EndOfTurnIntegrationTests.cs
│   │   └── TurnOrderIntegrationTests.cs
│   │
│   ├── AbilitiesItemsIntegrationTests.cs
│   ├── AccuracyIntegrationTests.cs
│   ├── BattleArbiterIntegrationTests.cs
│   ├── FullBattleTests.cs
│   ├── PlayerInputIntegrationTests.cs
│   ├── SwitchActionIntegrationTests.cs
│   └── TargetResolverIntegrationTests.cs
│
├── Field/                          # BattleField, BattleSlot, BattleSide tests
│   ├── BattleFieldTests.cs
│   ├── BattleFieldEdgeCasesTests.cs
│   ├── BattleSlotTests.cs
│   ├── BattleSlotEdgeCasesTests.cs
│   ├── BattleSideTests.cs
│   └── BattleSideEdgeCasesTests.cs
│
├── Queue/                          # BattleQueue tests
│   ├── BattleQueueTests.cs
│   └── BattleQueueEdgeCasesTests.cs
│
├── Engine/                         # CombatEngine, EndOfTurnProcessor tests
│   ├── CombatEngineTests.cs
│   ├── CombatEngineEdgeCasesTests.cs
│   ├── EndOfTurnProcessorTests.cs
│   └── EndOfTurnProcessorEdgeCasesTests.cs
│
├── Helpers/                        # Helper classes tests
│   ├── TurnOrderResolverTests.cs
│   ├── TurnOrderResolverEdgeCasesTests.cs
│   └── [Future helper tests]
│
├── AI/                            # AI provider tests
│   ├── AlwaysAttackAITests.cs
│   └── RandomAITests.cs
│
├── Providers/                     # Provider tests
│   ├── PlayerInputProviderTests.cs
│   └── PlayerInputProviderEdgeCasesTests.cs
│
├── Events/                        # Event system tests
│   └── BattleTriggerProcessorTests.cs
│
└── BattleArbiterTests.cs         # Top-level arbiter tests
└── BattleArbiterEdgeCasesTests.cs
```

## Key Changes

### 1. New `Effects/` Directory
- Move `RecoilDrainEffectTests.cs` and `RecoilDrainEffectEdgeCasesTests.cs` from `Actions/` to `Effects/`
- This separates effect testing from action testing
- Future effect tests (FlinchEffect, MultiHitEffect, etc.) can go here

### 2. Reorganize Integration Tests
- Group by system: `Actions/`, `Damage/`, `Engine/`
- Makes it easier to find related integration tests
- `RecoilDrainIntegrationTests.cs` → `Integration/Actions/RecoilDrainIntegrationTests.cs`
- `DamagePipelineIntegrationTests.cs` → `Integration/Damage/DamagePipelineIntegrationTests.cs`
- `StatModifierIntegrationTests.cs` → `Integration/Damage/StatModifierIntegrationTests.cs`

### 3. Rename Files
- `DamageVerificationTests.cs` → `DamageCalculationVerificationTests.cs` (more descriptive)
- `EdgeCaseTests.cs` → Split into specific integration test files or rename to `ComplexBattleScenariosTests.cs`

### 4. Group Related Tests
- `Field/` for BattleField, BattleSlot, BattleSide
- `Queue/` for BattleQueue
- `Engine/` for CombatEngine and EndOfTurnProcessor
- `Helpers/` for helper classes

## Benefits

1. **Clearer separation**: Effects vs Actions vs Damage
2. **Easier navigation**: Related tests grouped together
3. **Better scalability**: Easy to add new test categories
4. **Consistent structure**: All modules follow same pattern
5. **Easier maintenance**: Find tests faster

## Migration Plan

1. Create new directories
2. Move files (update namespaces)
3. Update any imports/references
4. Run tests to verify everything still works
5. Update documentation

## Alternative: Flatter Structure

If subdirectories feel too deep, we could use a flatter structure:

```
Combat/
├── Actions/
│   └── [action tests]
├── Effects/
│   └── [effect tests]
├── Damage/
│   └── [damage tests]
├── Integration/
│   └── [all integration tests flat]
└── [other modules]
```

This keeps it simpler but loses some organization benefits.

