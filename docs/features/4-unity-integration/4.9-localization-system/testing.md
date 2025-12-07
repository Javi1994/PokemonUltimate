# Sub-Feature 4.9: Localization System - Testing Strategy

> **Testing strategy for the localization system**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document outlines the testing strategy for the localization system, following TDD principles and the existing test structure.

## Test Structure

Following the project's test organization:

```
PokemonUltimate.Tests/
└── Systems/
    └── Localization/
        ├── LocalizationProviderTests.cs
        ├── LocalizationProviderEdgeCasesTests.cs
        ├── LocalizationDataProviderTests.cs
        ├── LocalizationDataProviderEdgeCasesTests.cs
        └── Integration/
            ├── BattleMessageFormatterLocalizationTests.cs
            └── CatalogLocalizationTests.cs
```

## Test Categories

### 1. Functional Tests

**Location**: `Systems/Localization/`

**Purpose**: Test normal behavior and expected results

**Test Files**:

-   `LocalizationProviderTests.cs` - Core functionality
-   `LocalizationDataProviderTests.cs` - Data provider functionality

**Key Tests**:

-   `GetString_ValidKey_ReturnsTranslatedString`
-   `GetString_ValidKeyWithArgs_ReturnsFormattedString`
-   `GetString_MissingLanguage_FallsBackToEnglish`
-   `HasString_ExistingKey_ReturnsTrue`
-   `HasString_NonExistentKey_ReturnsFalse`
-   `CurrentLanguage_SetValidLanguage_UpdatesLanguage`
-   `GetAvailableLanguages_ReturnsAllLanguages`

---

### 2. Edge Cases Tests

**Location**: `Systems/Localization/`

**Purpose**: Test boundary conditions and error handling

**Test Files**:

-   `LocalizationProviderEdgeCasesTests.cs` - Provider edge cases
-   `LocalizationDataProviderEdgeCasesTests.cs` - Data provider edge cases

**Key Tests**:

-   `GetString_NullKey_ReturnsEmptyString`
-   `GetString_EmptyKey_ReturnsEmptyString`
-   `GetString_MissingKey_ReturnsKey`
-   `GetString_MissingLanguageAndEnglish_ReturnsKey`
-   `GetString_InvalidFormatString_ReturnsUnformattedString`
-   `GetString_TooManyArgs_HandlesGracefully`
-   `GetString_TooFewArgs_HandlesGracefully`
-   `CurrentLanguage_SetNull_ThrowsArgumentException`
-   `CurrentLanguage_SetEmpty_ThrowsArgumentException`
-   `GetString_KeyWithSpecialCharacters_HandlesCorrectly`

---

### 3. Integration Tests

**Location**: `Systems/Localization/Integration/`

**Purpose**: Test system interactions

**Test Files**:

-   `BattleMessageFormatterLocalizationTests.cs` - Battle message integration
-   `CatalogLocalizationTests.cs` - Catalog integration

**Key Tests**:

-   `BattleMessageFormatter_UsesLocalizationProvider_FormatsCorrectly`
-   `BattleMessageFormatter_FormatMoveUsed_ReturnsLocalizedMessage`
-   `BattleMessageFormatter_FormatTypeEffectiveness_ReturnsLocalizedMessage`
-   `MoveCatalog_GetDescription_UsesLocalization`
-   `AbilityCatalog_GetDescription_UsesLocalization`
-   `ItemCatalog_GetDescription_UsesLocalization`

---

## Test Implementation Details

### Test Naming Convention

**Pattern**: `MethodName_Scenario_ExpectedResult`

**Examples**:

-   `GetString_ValidKey_ReturnsTranslatedString`
-   `GetString_MissingLanguage_FallsBackToEnglish`
-   `CurrentLanguage_SetNull_ThrowsArgumentException`

### Test Structure

```csharp
[Test]
public void GetString_ValidKey_ReturnsTranslatedString()
{
    // Arrange
    var provider = new LocalizationProvider();
    provider.CurrentLanguage = "en";
    var key = LocalizationKey.BattleUsedMove;

    // Act
    var result = provider.GetString(key, "Pikachu", "Thunderbolt");

    // Assert
    Assert.That(result, Is.EqualTo("Pikachu used Thunderbolt!"));
}
```

### Test Data Setup

**English Default**: All tests assume English ("en") as default language unless specified otherwise.

**Test Keys**: Use `LocalizationKey` constants, not magic strings.

**Test Languages**: Test with at least English ("en") and Spanish ("es") for coverage.

---

## Phase-by-Phase Testing

### Phase 1: Core Infrastructure Tests

**Test Files**:

-   `LocalizationProviderTests.cs` (interface compliance)
-   `LocalizationKeyTests.cs` (constants exist)

**Key Tests**:

-   Interface compliance tests
-   Constants existence tests
-   Data structure tests

**Estimated Tests**: 15-20 tests

---

### Phase 2: Provider Tests

**Test Files**:

-   `LocalizationProviderTests.cs` (implementation)
-   `LocalizationProviderEdgeCasesTests.cs` (edge cases)
-   `LocalizationDataProviderTests.cs` (data provider)

**Key Tests**:

-   `GetString` with valid keys
-   `GetString` with formatting
-   Fallback behavior
-   Language switching
-   Missing key handling
-   Invalid input handling

**Estimated Tests**: 25-30 tests

---

### Phase 3: Integration Tests

**Test Files**:

-   `Integration/BattleMessageFormatterLocalizationTests.cs`

**Key Tests**:

-   `BattleMessageFormatter` uses `ILocalizationProvider`
-   ✅ All `GameMessages` replaced correctly in Combat system
-   ✅ All hardcoded name comparisons replaced with catalog lookups
-   ✅ All generic damage messages localized
-   Backward compatibility maintained

**Estimated Tests**: 20-25 tests

---

### Phase 4: Catalog Tests

**Test Files**:

-   `Integration/CatalogLocalizationTests.cs`

**Key Tests**:

-   Move descriptions use localization
-   Ability descriptions use localization
-   Item descriptions use localization

**Estimated Tests**: 30-40 tests

---

## Test Coverage Goals

### Minimum Coverage

-   **Core Infrastructure**: 100% (interfaces, constants)
-   **Provider Implementation**: 95%+ (all paths covered)
-   **Integration**: 90%+ (all integration points)

### Critical Paths

-   ✅ GetString with valid key
-   ✅ GetString with formatting
-   ✅ Fallback to English
-   ✅ Missing key handling
-   ✅ Language switching
-   ✅ Error handling

---

## Mock Objects

### MockLocalizationProvider

For testing components that depend on `ILocalizationProvider`:

```csharp
public class MockLocalizationProvider : ILocalizationProvider
{
    private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
    public string CurrentLanguage { get; set; } = "en";

    public void SetupTranslation(string key, string translation)
    {
        _translations[key] = translation;
    }

    public string GetString(string key, params object[] args)
    {
        if (!_translations.TryGetValue(key, out var translation))
            return key;

        if (args == null || args.Length == 0)
            return translation;

        return string.Format(translation, args);
    }

    public bool HasString(string key) => _translations.ContainsKey(key);

    public IEnumerable<string> GetAvailableLanguages() => new[] { "en" };
}
```

---

## Test Data

### Sample Translations

**English (en)**:

-   `battle_used_move`: "{0} used {1}!"
-   `battle_missed`: "The attack missed!"
-   `type_super_effective`: "It's super effective!"

**Spanish (es)**:

-   `battle_used_move`: "¡{0} usó {1}!"
-   `battle_missed`: "¡El ataque falló!"
-   `type_super_effective`: "¡Es súper efectivo!"

**French (fr)**:

-   `battle_used_move`: "{0} a utilisé {1}!"
-   `battle_missed`: "L'attaque a échoué!"
-   `type_super_effective`: "C'est super efficace!"

---

## Test Execution

### Running Tests

```powershell
# Run all localization tests
dotnet test --filter "FullyQualifiedName~Localization"

# Run specific test file
dotnet test --filter "FullyQualifiedName~LocalizationProviderTests"

# Run integration tests
dotnet test --filter "FullyQualifiedName~Integration"
```

### Test Validation

After implementing tests:

1. ✅ All tests pass (`dotnet test`)
2. ✅ No warnings (`dotnet build`)
3. ✅ Test structure validation passes (`validate-test-structure.ps1`)
4. ✅ FDD compliance validation passes (`validate-fdd-compliance.ps1`)

---

## Related Documents

-   **[Architecture.md](architecture.md)** - Technical specification
-   **[Roadmap.md](roadmap.md)** - Implementation plan
-   **[Use Cases.md](use_cases.md)** - All scenarios
-   **[Code Location.md](code_location.md)** - Where code lives
-   **[TDD Guide](../../../../ai_workflow/docs/TDD_GUIDE.md)** - TDD workflow

---

**Last Updated**: 2025-01-XX
