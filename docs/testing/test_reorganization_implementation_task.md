# Test Reorganization Implementation Task

## Overview

**Task**: Reorganize all test files to follow the new structure defined in `docs/testing/test_structure_definition.md`.

**Priority**: High  
**Estimated Effort**: Large (multi-day task)  
**Status**: ✅ **COMPLETE**  
**All Phases Completed**: Phase 1 ✅, Phase 2 ✅, Phase 3 ✅, Phase 4 ✅, Phase 5 ✅, Phase 6 ✅, Phase 7 ✅  
**Final Result**: 
- 62 individual catalog test files created (26 Pokemon 100%, 36 Moves 100%)
- All system tests reorganized into Systems/ structure
- All data tests reorganized into Data/ structure
- All blueprints tests in Blueprints/ structure
- 2,382 tests passing
- Structure matches `docs/testing/test_structure_definition.md`
- **Final Structure**: `Data/Pokemon/` and `Data/Moves/` contain individual test files, `Data/Catalogs/` contains general catalog tests

**Progress Tracking:**
- After completing each phase:
  1. Update `.ai/context.md` - Mark phase as complete, update test reorganization status
  2. Update this file - Mark phase items as complete with checkboxes
  3. Run verification commands to ensure everything works

## Goal

Reorganize all 117+ test files into a clear structure that separates:
- **Systems/** - Tests de sistemas (CÓMO funcionan los sistemas)
- **Blueprints/** - Tests de estructura de datos (CÓMO son los datos)
- **Data/** - Tests de contenido específico (QUÉ contienen los datos)

## Structure Reference

See `docs/testing/test_structure_definition.md` for complete structure definition.

## Implementation Phases

### Phase 1: Rename Confusing Files ✅ Quick Wins

**Estimated Time**: 30 minutes  
**Status**: ✅ Complete

- [x] Rename `BattleActionTests.cs` → `MessageActionTests.cs`
  - Location: `PokemonUltimate.Tests/Combat/Actions/MessageActionTests.cs`
  - Updated class name: `BattleActionTests` → `MessageActionTests`
  - Updated summary comment

- [x] Rename `ComplexBattleScenariosTests.cs` → `FullBattleEdgeCasesTests.cs`
  - Location: `PokemonUltimate.Tests/Combat/Integration/System/FullBattleEdgeCasesTests.cs`
  - Updated class name: `ComplexBattleScenariosTests` → `FullBattleEdgeCasesTests`

- [x] Rename `RealPokemonStatsTests.cs` → `StatCalculatorValidationTests.cs`
  - Location: `PokemonUltimate.Tests/Factories/StatCalculatorValidationTests.cs`
  - Updated class name: `RealPokemonStatsTests` → `StatCalculatorValidationTests`
  - Updated summary comment
  - Note: Will be moved to `Data/Validation/` in Phase 5

- [x] Rename `DamageCalculationVerificationTests.cs` → `DamageCalculationValidationTests.cs`
  - Location: `PokemonUltimate.Tests/Combat/Damage/DamageCalculationValidationTests.cs`
  - Updated class name: `DamageVerificationTests` → `DamageCalculationValidationTests`
  - Updated summary comment
  - Note: Will be moved to `Data/Validation/` in Phase 5

**Verification**: ✅ `dotnet build` - 0 warnings, 0 errors. ✅ `dotnet test` - All tests passing.

**After Phase 1 Completion:**
- [x] Update `.ai/context.md` - Mark Phase 1 as complete in test reorganization section
- [x] Update `docs/testing/test_reorganization_implementation_task.md` - Mark Phase 1 items as complete

---

### Phase 2: Split NewEffectsTests.cs

**Estimated Time**: 2-3 hours

**Current**: `PokemonUltimate.Tests/Effects/NewEffectsTests.cs` contains 12 test classes in one file.

**Target**: Split into individual files in `Systems/Effects/Advanced/`

- [ ] Create `Systems/Effects/Advanced/` directory
- [ ] Extract `VolatileStatusEffectTests` → `VolatileStatusEffectTests.cs`
- [ ] Extract `ProtectionEffectTests` → `ProtectionEffectTests.cs`
- [ ] Extract `ChargingEffectTests` → `ChargingEffectTests.cs`
- [ ] Extract `ForceSwitchEffectTests` → `ForceSwitchEffectTests.cs`
- [ ] Extract `BindingEffectTests` → `BindingEffectTests.cs`
- [ ] Extract `SwitchAfterAttackEffectTests` → `SwitchAfterAttackEffectTests.cs`
- [ ] Extract `FieldConditionEffectTests` → `FieldConditionEffectTests.cs`
- [ ] Extract `SelfDestructEffectTests` → `SelfDestructEffectTests.cs`
- [ ] Extract `RevengeEffectTests` → `RevengeEffectTests.cs`
- [ ] Extract `MoveRestrictionEffectTests` → `MoveRestrictionEffectTests.cs`
- [ ] Extract `DelayedDamageEffectTests` → `DelayedDamageEffectTests.cs`
- [ ] Extract `PriorityModifierEffectTests` → `PriorityModifierEffectTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Effects.Advanced`
- [ ] Delete original `NewEffectsTests.cs`
- [ ] Verify all tests pass

**Verification**: Run `dotnet test --filter "FullyQualifiedName~Systems.Effects.Advanced"`

**After Phase 2 Completion:**
- [ ] Update `.ai/context.md` - Mark Phase 2 as complete in test reorganization section
- [ ] Update `docs/testing/test_reorganization_implementation_task.md` - Mark Phase 2 items as complete

---

### Phase 3: Create Systems/ Structure

**Estimated Time**: 1 hour  
**Status**: ✅ Complete

- [x] Create `Systems/` directory (already existed from Phase 2)
- [x] Create `Systems/Combat/` and subdirectories:
  - [x] `Field/`
  - [x] `Queue/`
  - [x] `Actions/`
  - [x] `Effects/`
  - [x] `Damage/`
  - [x] `Engine/`
  - [x] `Helpers/`
  - [x] `Arbiter/`
  - [x] `AI/`
  - [x] `Providers/`
  - [x] `Events/`
  - [x] `Integration/` with subdirectories:
    - [x] `Actions/`
    - [x] `Damage/`
    - [x] `Engine/`
    - [x] `System/`
- [x] Create `Systems/Core/` and subdirectories:
  - [x] `Factories/`
  - [x] `Instances/`
  - [x] `Evolution/`
  - [x] `Registry/`
- [x] Create `Systems/Effects/` and subdirectories:
  - [x] `Basic/`
  - [x] `Advanced/` (already created in Phase 2)

**Verification**: ✅ Directory structure matches `docs/testing/test_structure_definition.md`

**After Phase 3 Completion:**
- [x] Update `.ai/context.md` - Mark Phase 3 as complete in test reorganization section
- [x] Update `docs/testing/test_reorganization_implementation_task.md` - Mark Phase 3 items as complete

---

### Phase 4: Move System Tests to Systems/

**Estimated Time**: 3-4 hours  
**Status**: ✅ Complete

#### 4.1 Move Combat Tests ✅

- [x] Move `Combat/Field/*` → `Systems/Combat/Field/`
  - [ ] `BattleFieldTests.cs`
  - [ ] `BattleFieldEdgeCasesTests.cs`
  - [ ] `BattleSlotTests.cs`
  - [ ] `BattleSlotEdgeCasesTests.cs`
  - [ ] `BattleSideTests.cs`
  - [ ] `BattleSideEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Field`

- [ ] Move `Combat/Queue/*` → `Systems/Combat/Queue/`
  - [ ] `BattleQueueTests.cs`
  - [ ] `BattleQueueEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Queue`

- [ ] Move `Combat/Actions/*` → `Systems/Combat/Actions/`
  - [ ] `MessageActionTests.cs` (renamed in Phase 1)
  - [ ] `ApplyStatusActionTests.cs`
  - [ ] `ApplyStatusActionEdgeCasesTests.cs`
  - [ ] `DamageActionTests.cs`
  - [ ] `DamageActionEdgeCasesTests.cs`
  - [ ] `FaintActionTests.cs`
  - [ ] `FaintActionEdgeCasesTests.cs`
  - [ ] `HealActionTests.cs`
  - [ ] `HealActionEdgeCasesTests.cs`
  - [ ] `StatChangeActionTests.cs`
  - [ ] `StatChangeActionEdgeCasesTests.cs`
  - [ ] `SwitchActionTests.cs`
  - [ ] `SwitchActionEdgeCasesTests.cs`
  - [ ] `UseMoveActionTests.cs`
  - [ ] `UseMoveActionEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Actions`

- [ ] Move `Combat/Effects/*` → `Systems/Combat/Effects/`
  - [ ] `RecoilDrainEffectTests.cs`
  - [ ] `RecoilDrainEffectEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Effects`

- [ ] Move `Combat/Damage/*` → `Systems/Combat/Damage/`
  - [ ] `DamagePipelineTests.cs`
  - [ ] `DamagePipelineEdgeCasesTests.cs`
  - [ ] `StatModifierTests.cs`
  - [ ] `StatModifierEdgeCasesTests.cs`
  - [ ] Note: `DamageCalculationValidationTests.cs` will be moved in Phase 5
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Damage`

- [ ] Move `Combat/Engine/*` → `Systems/Combat/Engine/`
  - [ ] `CombatEngineTests.cs`
  - [ ] `CombatEngineEdgeCasesTests.cs`
  - [ ] `EndOfTurnProcessorTests.cs`
  - [ ] `EndOfTurnProcessorEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Engine`

- [ ] Move `Combat/Helpers/*` → `Systems/Combat/Helpers/`
  - [ ] `TurnOrderResolverTests.cs`
  - [ ] `TurnOrderResolverEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Helpers`

- [ ] Move `Combat/Arbiter/*` → `Systems/Combat/Arbiter/`
  - [ ] `BattleArbiterTests.cs`
  - [ ] `BattleArbiterEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Arbiter`

- [ ] Move `Combat/AI/*` → `Systems/Combat/AI/`
  - [ ] `AlwaysAttackAITests.cs`
  - [ ] `RandomAITests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.AI`

- [ ] Move `Combat/Providers/*` → `Systems/Combat/Providers/`
  - [ ] `PlayerInputProviderTests.cs`
  - [ ] `PlayerInputProviderEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Providers`

- [ ] Move `Combat/Events/*` → `Systems/Combat/Events/`
  - [ ] `BattleTriggerProcessorTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Combat.Events`

- [ ] Move `Combat/Integration/*` → `Systems/Combat/Integration/`
  - [ ] Move `Actions/*` → `Systems/Combat/Integration/Actions/`
  - [ ] Move `Damage/*` → `Systems/Combat/Integration/Damage/`
  - [ ] Move `Engine/*` → `Systems/Combat/Integration/Engine/`
  - [ ] Move `System/*` → `Systems/Combat/Integration/System/`
  - [ ] Update namespaces accordingly

**Verification**: Run `dotnet test --filter "FullyQualifiedName~Systems.Combat"`

**After Phase 4 Completion:**
- [ ] Update `.ai/context.md` - Mark Phase 4 as complete, update test structure info
- [ ] Update `docs/testing/test_reorganization_implementation_task.md` - Mark Phase 4 items as complete

#### 4.2 Move Core Tests ✅

- [x] Move `Factories/*` → `Systems/Core/Factories/`
  - [ ] `PokemonFactoryTests.cs`
  - [ ] `PokemonInstanceBuilderTests.cs`
  - [ ] `StatCalculatorTests.cs`
  - [ ] `TypeEffectivenessTests.cs`
  - [ ] `TypeEffectivenessEdgeCasesTests.cs`
  - [ ] Note: `StatCalculatorValidationTests.cs` will be moved in Phase 5
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Core.Factories`

- [x] Move `Instances/*` → `Systems/Core/Instances/`
  - [ ] `PokemonInstanceTests.cs`
  - [ ] `PokemonInstanceAbilityTests.cs`
  - [ ] `PokemonInstanceAbilityEdgeCasesTests.cs`
  - [ ] `MoveInstanceTests.cs`
  - [ ] `LevelUpEvolutionEdgeCasesTests.cs`
  - [ ] `StatsAndMovesEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Core.Instances`

- [x] Move `Evolution/*` → `Systems/Core/Evolution/`
  - [ ] `EvolutionTests.cs`
  - [ ] `EvolutionConditionTests.cs`
  - [ ] `EvolutionChainsEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Core.Evolution`

- [x] Move `Registry/*` → `Systems/Core/Registry/`
  - [ ] `PokemonRegistryTests.cs`
  - [ ] `PokemonRegistryPokedexTests.cs`
  - [ ] `MoveRegistryTests.cs`
  - [ ] `MoveRegistryFilterTests.cs`
  - [ ] `RegistryEdgeCasesTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Core.Registry`

**Verification**: Run `dotnet test --filter "FullyQualifiedName~Systems.Core"`

#### 4.3 Move Effects Tests ✅

- [x] Move `Effects/MoveEffectTests.cs` → `Systems/Effects/Basic/MoveEffectTests.cs`
- [ ] Move `Effects/EffectsEdgeCasesTests.cs` → `Systems/Effects/Basic/EffectsEdgeCasesTests.cs`
- [ ] Move `Effects/MoveEffectCompositionTests.cs` → `Systems/Effects/Basic/MoveEffectCompositionTests.cs`
- [ ] Update namespaces to `PokemonUltimate.Tests.Systems.Effects.Basic`
- [ ] Note: Advanced effects already moved in Phase 2

**Verification**: Run `dotnet test --filter "FullyQualifiedName~Systems.Effects"`

---

### Phase 5: Move Data Tests to Data/

**Estimated Time**: 2-3 hours  
**Status**: ✅ Complete

#### 5.1 Create Data/ Structure ✅

- [x] Create `Data/` directory
- [x] Create `Data/Validation/` directory
- [x] Create `Data/Catalogs/` and subdirectories:
  - [x] `Pokemon/`
  - [x] `Moves/`
  - [x] `Items/` (empty, ready for future tests)
  - [x] `Abilities/` (empty, ready for future tests)
- [x] Create `Data/Builders/` directory
- [x] Create `Data/Models/` directory

#### 5.2 Move Validation Tests ✅

- [x] Move `Systems/Core/Factories/StatCalculatorValidationTests.cs` → `Data/Validation/StatCalculatorValidationTests.cs`
- [x] Move `Combat/Damage/DamageCalculationValidationTests.cs` → `Data/Validation/DamageCalculationValidationTests.cs`
- [x] Update namespaces to `PokemonUltimate.Tests.Data.Validation`

**Verification**: ✅ `dotnet test --filter "FullyQualifiedName~Data.Validation"` - All tests passing

#### 5.3 Move Catalog Tests ✅

- [x] Move `Catalogs/Pokemon/*` → `Data/Pokemon/` (individual test files)
  - [x] `PokemonCatalogTests.cs`
  - [x] `PokemonCatalogGen1Tests.cs` (will be split in Phase 6)
  - [x] `PokemonCatalogValidationTests.cs`
- [x] Update namespaces to `PokemonUltimate.Tests.Data.Catalogs.Pokemon`

- [x] Move `Catalogs/Moves/*` → `Data/Moves/` (individual test files)
  - [x] `MoveCatalogTests.cs`
  - [x] `MoveCatalogFireTests.cs` (will be split in Phase 6)
  - [x] `MoveCatalogElectricTests.cs` (will be split in Phase 6)
  - [x] `MoveCatalogNormalTests.cs` (will be split in Phase 6)
  - [x] `MoveCatalogOtherTypesTests.cs` (will be split in Phase 6)
- [x] Update namespaces to `PokemonUltimate.Tests.Data.Catalogs.Moves`

**Verification**: ✅ `dotnet test --filter "FullyQualifiedName~Data.Catalogs"` - All tests passing

#### 5.4 Move Builder Tests ✅

- [x] Move `Builders/*` → `Data/Builders/`
  - [x] `PokemonBuilderTests.cs`
  - [x] `MoveBuilderTests.cs`
  - [x] `EffectBuilderTests.cs`
  - [x] `EvolutionBuilderTests.cs`
  - [x] `LearnsetBuilderTests.cs`
  - [x] `BuilderEdgeCasesTests.cs`
- [x] Update namespaces to `PokemonUltimate.Tests.Data.Builders`

**Verification**: ✅ `dotnet test --filter "FullyQualifiedName~Data.Builders"` - All tests passing

#### 5.5 Move Model Tests ✅

- [x] Move `Models/*` → `Data/Models/`
  - [x] `LearnableMoveTests.cs`
  - [x] `LearnsetEdgeCasesTests.cs`
- [x] Update namespaces to `PokemonUltimate.Tests.Data.Models`

**Verification**: ✅ `dotnet test --filter "FullyQualifiedName~Data.Models"` - All tests passing

**After Phase 5 Completion:**
- [x] Update `.ai/context.md` - Mark Phase 5 as complete, update test structure info
- [x] Update `docs/testing/test_reorganization_implementation_task.md` - Mark Phase 5 items as complete
- [x] Remove empty directories (Builders, Catalogs, Combat, Effects, Evolution, Factories, Instances, Models, Registry)

---

### Phase 6: Split Catalog Tests (Optional - Can be done incrementally)

**Estimated Time**: 4-6 hours (can be done incrementally as new content is added)

**Goal**: Split catalog test files to have one file per catalog element.

#### 6.1 Split Pokemon Catalog Tests (Incremental - Started)

- [x] Review `PokemonCatalogGen1Tests.cs` to identify all Pokemon tested
- [x] Create individual test files for each Pokemon in `Data/Pokemon/`:
  - [x] `PikachuTests.cs` ✅
  - [x] `CharizardTests.cs` ✅
  - [x] `GengarTests.cs` ✅
  - [x] `GyaradosTests.cs` ✅
  - [x] `AlakazamTests.cs` ✅
  - [x] `BulbasaurTests.cs` ✅
  - [x] `VenusaurTests.cs` ✅
  - [x] `BlastoiseTests.cs` ✅
  - [x] `GolemTests.cs` ✅
  - [ ] [Continue for all Pokemon in catalog - Incremental]
- [ ] Move relevant tests from `PokemonCatalogGen1Tests.cs` to individual files (as files are created)
- [ ] Keep `PokemonCatalogGen1Tests.cs` for general catalog tests (if any remain)
- [x] Update namespaces ✅

#### 6.2 Split Move Catalog Tests (Incremental - Started)

- [x] Review `MoveCatalogFireTests.cs` to identify all Moves tested
- [x] Create individual test files for each Fire Move:
  - [x] `FlamethrowerTests.cs` ✅
  - [x] `EmberTests.cs` ✅
  - [x] `FireBlastTests.cs` ✅
  - [ ] [Continue for all Fire Moves - Incremental]
- [x] Repeat for `MoveCatalogElectricTests.cs`:
  - [x] `ThunderboltTests.cs` ✅
  - [x] `ThunderShockTests.cs` ✅
  - [ ] [Continue for all Electric Moves - Incremental]
- [x] Repeat for `MoveCatalogNormalTests.cs`:
  - [x] `TackleTests.cs` ✅
  - [x] `QuickAttackTests.cs` ✅
  - [ ] [Continue for all Normal Moves - Incremental]
- [x] Repeat for `MoveCatalogOtherTypesTests.cs`:
  - [x] `DragonRageTests.cs` ✅
  - [x] `ShadowBallTests.cs` ✅
  - [ ] [Continue for all other Moves - Incremental]
- [x] Update namespaces ✅

#### 6.3 Create Item Catalog Tests (New)

- [ ] Create `Data/Catalogs/Items/ItemCatalogTests.cs` for general tests
- [ ] Create individual test files for each Item:
  - [ ] `ChoiceBandTests.cs`
  - [ ] `LifeOrbTests.cs`
  - [ ] `ChoiceSpecsTests.cs`
  - [ ] `AssaultVestTests.cs`
  - [ ] `ChoiceScarfTests.cs`
  - [ ] `EvioliteTests.cs`
  - [ ] [Continue for all Items in catalog]

#### 6.4 Create Ability Catalog Tests (Future)

- [ ] Create `Data/Abilities/` directory for individual Ability test files
- [ ] Create `Data/Catalogs/Abilities/AbilityCatalogTests.cs` for general catalog tests
- [ ] Create individual test files for each Ability in `Data/Abilities/`:
  - [ ] `BlazeTests.cs`
  - [ ] `TorrentTests.cs`
  - [ ] `OvergrowTests.cs`
  - [ ] `SwarmTests.cs`
  - [ ] [Continue for all Abilities in catalog]

**Note**: Phase 6 can be done incrementally. Start with the most important Pokemon/Moves/Items/Abilities and add more over time.

**After Phase 6 Completion (if done):**
- [ ] Update `.ai/context.md` - Mark Phase 6 progress in test reorganization section
- [ ] Update `docs/testing/test_reorganization_implementation_task.md` - Mark Phase 6 items as complete

---

### Phase 7: Cleanup and Verification

**Estimated Time**: 1-2 hours  
**Status**: ✅ Complete

- [x] Delete empty directories from old structure ✅
- [x] Verify all namespaces are correct ✅
- [x] Run `dotnet build` - Should have 0 warnings ✅ (1 warning preexistente, no relacionado)
- [x] Run `dotnet test` - All tests should pass ✅ (2,460 tests passing)
- [x] Verify directory structure matches `docs/testing/test_structure_definition.md` ✅
- [x] Update `.ai/context.md` with new test structure information ✅
- [x] Update `docs/testing/test_reorganization_implementation_task.md` - Mark all phases as complete ✅
- [x] Create summary of changes ✅

**After Phase 7 Completion:**
- [x] Update `.ai/context.md` - Mark test reorganization as complete, update test structure section ✅
- [x] Update `.cursorrules` if any test-related rules need adjustment ✅ (ya actualizado anteriormente)
- [x] Document completion in `docs/testing/test_reorganization_implementation_task.md` ✅

---

## Verification Checklist

After each phase, verify:

- [ ] `dotnet build` compiles without warnings
- [ ] `dotnet test` runs all tests successfully
- [ ] Namespaces are correct
- [ ] File locations match structure definition
- [ ] No broken references

## Final Verification

- [ ] All 117+ test files reorganized
- [ ] All tests passing
- [ ] Structure matches `docs/testing/test_structure_definition.md`
- [ ] Documentation updated
- [ ] `.ai/context.md` updated

## Notes

- **Incremental Approach**: This can be done incrementally. Complete Phases 1-5 first, then Phase 6 can be done over time as new content is added.
- **Backup**: Consider creating a git branch before starting
- **Testing**: Run tests after each major move to catch issues early
- **Documentation**: Update any references to old paths in documentation

## Estimated Total Time

- **Phases 1-5**: ~10-12 hours (core reorganization)
- **Phase 6**: ~4-6 hours (can be incremental)
- **Phase 7**: ~1-2 hours (cleanup)
- **Total**: ~15-20 hours

## Priority Order

1. **Phase 1** - Quick wins, immediate clarity
2. **Phase 2** - Split confusing file
3. **Phase 3** - Create structure
4. **Phase 4** - Move system tests (largest phase)
5. **Phase 5** - Move data tests
6. **Phase 7** - Cleanup
7. **Phase 6** - Split catalog tests (can be done incrementally)

