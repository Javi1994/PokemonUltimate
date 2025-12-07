# Sub-Feature 4.9: Localization System - Roadmap

> **Implementation plan for the localization system**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This roadmap outlines the phased implementation of the localization system, following TDD principles and the existing codebase patterns.

## Implementation Phases

### Phase 1: Core Localization Infrastructure ✅ **COMPLETE**

**Goal**: Create core interfaces and data structures for localization.

**Location**: `PokemonUltimate.Core/Localization/`

**Tasks**:

1. ✅ Create `ILocalizationProvider` interface
2. ✅ Create `LocalizationKey` constants class
3. ✅ Create `LocalizationData` structure
4. ✅ Write tests for core infrastructure (TDD)

**Dependencies**: None

**Estimated Effort**: 4-6 hours  
**Estimated Tests**: 15-20 tests

**Completion Criteria**:

-   [x] `ILocalizationProvider` interface created and documented
-   [x] `LocalizationKey` constants for all `GameMessages` created
-   [x] `LocalizationData` structure created
-   [x] All tests passing
-   [x] Code follows SOLID principles
-   [x] Feature references added to all classes

---

### Phase 2: Content Localization Provider ✅ **COMPLETE**

**Goal**: Create data provider and default implementation.

**Location**: `PokemonUltimate.Content/Providers/`

**Tasks**:

1. ✅ Create `LocalizationDataProvider` static class
2. ✅ Create `LocalizationProvider` implementation
3. ✅ Migrate `GameMessages` strings to `LocalizationDataProvider`
4. ✅ Add English, Spanish, and French translations
5. ✅ Write tests for providers (TDD)

**Dependencies**: Phase 1

**Estimated Effort**: 8-12 hours  
**Estimated Tests**: 25-30 tests

**Completion Criteria**:

-   [x] `LocalizationDataProvider` created with all `GameMessages` migrated
-   [x] `LocalizationProvider` implements `ILocalizationProvider` correctly
-   [x] English, Spanish, and French translations added
-   [x] Fallback to English works correctly
-   [x] All tests passing
-   [x] Code follows SOLID principles
-   [x] Feature references added to all classes

---

### Phase 3: BattleMessageFormatter Integration ✅ **COMPLETE**

**Goal**: Integrate localization into `BattleMessageFormatter` and all Combat actions.

**Location**: `PokemonUltimate.Combat/Messages/` and all Combat action files

**Tasks**:

1. ✅ Update `BattleMessageFormatter` to use `ILocalizationProvider`
2. ✅ Update `IBattleMessageFormatter` if needed
3. ✅ Replace `GameMessages` usage with `LocalizationKey` constants
4. ✅ Update all call sites to pass `ILocalizationProvider`
5. ✅ Write tests for integration (TDD)
6. ✅ **Replace ALL `GameMessages` usage in Combat system**
7. ✅ **Replace hardcoded item/ability/hazard name comparisons with catalog lookups**
8. ✅ **Add localization for all generic damage messages**

**Dependencies**: Phase 2

**Estimated Effort**: 6-8 hours (actual: ~12 hours including full Combat migration)  
**Estimated Tests**: 20-25 tests

**Completion Criteria**:

-   [x] `BattleMessageFormatter` uses `ILocalizationProvider`
-   [x] **ALL `GameMessages` replaced with `LocalizationKey` constants in entire Combat system**
-   [x] **All Combat actions use `LocalizationProvider` directly**
-   [x] **All hardcoded name comparisons replaced with catalog lookups**
-   [x] **All generic damage messages localized**
-   [x] Backward compatibility maintained (constructor sin parámetros)
-   [x] All tests passing
-   [x] Code follows SOLID principles
-   [x] Feature references added to all classes

**Note**: ✅ **`GameMessages` class no longer used in Combat system**. Only exists in Core for `TypeEffectiveness.GetEffectivenessDescription()` which is not used in production code.

---

### Phase 4: Catalog Descriptions Migration ⏳ **PLANNED**

**Goal**: Migrate catalog descriptions to localization system.

**Location**: `PokemonUltimate.Content/Catalogs/`

**Status**: Extension methods created, translations pending

**Tasks**:

1. ✅ Add extension methods for descriptions (`GetDescription()`)
2. ⏳ Add translations for move descriptions (36 moves)
3. ⏳ Add translations for ability descriptions (43 abilities)
4. ⏳ Add translations for item descriptions (23 items)
5. ⏳ Write tests for description localization (TDD)

**Dependencies**: Phase 3

**Estimated Effort**: 12-16 hours  
**Estimated Tests**: 30-40 tests

**Completion Criteria**:

-   [x] Extension methods created (`GetDescription()` for moves, abilities, items)
-   [ ] Move descriptions translated (EN, ES, FR)
-   [ ] Ability descriptions translated (EN, ES, FR)
-   [ ] Item descriptions translated (EN, ES, FR)
-   [ ] All tests passing
-   [ ] Code follows SOLID principles
-   [ ] Feature references added to all classes

**Note**: Extension methods are ready. Only need to add translation data to `LocalizationDataProvider`.

---

### Phase 5: Unity Integration ⏳ **PLANNED**

**Goal**: Create Unity-specific localization manager.

**Location**: `PokemonUltimate.Unity/Localization/` (Future)

**Tasks**:

1. ⏳ Create `UnityLocalizationManager` class
2. ⏳ Implement loading from JSON/CSV/Unity Localization Package
3. ⏳ Implement runtime language switching
4. ⏳ Integrate with Unity UI
5. ⏳ Write Unity tests (if applicable)

**Dependencies**: Phase 4

**Estimated Effort**: 10-14 hours  
**Estimated Tests**: Manual testing + Unity tests

**Completion Criteria**:

-   [ ] Unity localization manager created
-   [ ] Can load translations from external files
-   [ ] Runtime language switching works
-   [ ] Unity UI uses localization
-   [ ] All tests passing

**Note**: This phase is for future Unity integration. Core SDK localization (Phases 1-4) is independent of Unity.

---

## Migration Strategy

### Gradual Migration Approach ✅ **COMPLETE**

**Phase 1-2**: ✅ Create infrastructure (no breaking changes)  
**Phase 3**: ✅ Integrate with `BattleMessageFormatter` and migrate all Combat actions  
**Phase 4**: ⏳ Migrate catalogs (extension methods ready, translations pending)  
**Phase 5**: ⏳ Unity-specific (optional, future work)

### Migration Status ✅

-   ✅ **Phase 3**: All `GameMessages` usage removed from Combat system
-   ✅ **Combat System**: Fully migrated to `LocalizationProvider`
-   ⏳ **Phase 4**: Catalog descriptions - extension methods ready, translations pending
-   ⚠️ **GameMessages**: Only exists in Core for `TypeEffectiveness.GetEffectivenessDescription()` (not used in production)

## Testing Strategy

### Phase 1: Core Infrastructure Tests

-   `ILocalizationProvider` interface compliance
-   `LocalizationKey` constants exist
-   `LocalizationData` structure works correctly

### Phase 2: Provider Tests

-   `LocalizationDataProvider` data integrity
-   `LocalizationProvider` implementation correctness
-   Fallback behavior (missing language → English)
-   Formatting with placeholders
-   Language switching

### Phase 3: Integration Tests

-   `BattleMessageFormatter` uses localization correctly
-   All `GameMessages` replaced correctly
-   Backward compatibility maintained

### Phase 4: Catalog Tests

-   Move descriptions use localization
-   Ability descriptions use localization
-   Item descriptions use localization
-   Dynamic key generation works

## Validation Checklist

After each phase:

-   [ ] **Validation scripts passed** (exit code 0):
    -   [ ] `validate-test-structure.ps1` passed
    -   [ ] `validate-fdd-compliance.ps1` passed
-   [ ] All tests pass (`dotnet test`)
-   [ ] No warnings (`dotnet build`)
-   [ ] `.ai/context.md` updated
-   [ ] `docs/features_master_list_index.md` updated (if feature status changed)
-   [ ] Documentation updated (roadmap, architecture, use_cases, code_location, testing)
-   [ ] **Feature references added** - All code references its feature
-   [ ] **Existing code reviewed** - All related code was read before implementation
-   [ ] **Decision trees followed** - Feature Discovery and TDD workflows executed

## Related Documents

-   **[Architecture.md](architecture.md)** - Technical specification
-   **[Use Cases.md](use_cases.md)** - All scenarios
-   **[Testing.md](testing.md)** - Testing strategy
-   **[Code Location.md](code_location.md)** - Where code lives

---

**Last Updated**: 2025-01-XX  
**Phase 3 Complete**: ✅ All Combat system messages migrated to `LocalizationProvider`
