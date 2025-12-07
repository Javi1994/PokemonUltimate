# Sub-Feature 4.9: Localization System - Use Cases

> **All scenarios for the localization system**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Use Cases Overview

This document describes all use cases for the localization system, organized by category.

## Category 1: Core Localization Operations

### UC-001: Get Localized String

**Description**: Get a localized string by key  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized, key exists  
**Steps**:

1. System calls `GetString(key)` with a valid key
2. Provider looks up translation for current language
3. Provider returns translated string

**Expected Result**: Translated string in current language  
**Status**: ⏳ Planned

---

### UC-002: Get Localized String with Formatting

**Description**: Get a localized string with placeholders replaced  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized, key exists, format string has placeholders  
**Steps**:

1. System calls `GetString(key, arg1, arg2, ...)` with a valid key and arguments
2. Provider looks up translation for current language
3. Provider formats string with arguments using `string.Format`
4. Provider returns formatted translated string

**Expected Result**: Formatted translated string  
**Status**: ⏳ Planned

**Example**:

```csharp
// Key: "battle_used_move" = "{0} used {1}!"
// Args: "Pikachu", "Thunderbolt"
// Result (en): "Pikachu used Thunderbolt!"
// Result (es): "¡Pikachu usó Thunderbolt!"
```

---

### UC-003: Check if Key Exists

**Description**: Check if a localization key exists  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized  
**Steps**:

1. System calls `HasString(key)` with a key
2. Provider checks if key exists in data
3. Provider returns boolean result

**Expected Result**: True if key exists, false otherwise  
**Status**: ⏳ Planned

---

### UC-004: Get String with Missing Key

**Description**: Get a localized string when key doesn't exist  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized, key doesn't exist  
**Steps**:

1. System calls `GetString(key)` with a non-existent key
2. Provider looks up key in data
3. Provider doesn't find key
4. Provider returns the key itself (fail-safe)

**Expected Result**: Key string returned (fail-safe behavior)  
**Status**: ⏳ Planned

---

### UC-005: Get String with Missing Language

**Description**: Get a localized string when current language not available  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized, key exists, current language not available  
**Steps**:

1. System calls `GetString(key)` with valid key
2. Provider looks up translation for current language
3. Provider doesn't find translation for current language
4. Provider falls back to English ("en")
5. Provider returns English translation

**Expected Result**: English translation (fallback behavior)  
**Status**: ⏳ Planned

---

### UC-006: Get String with Missing Language and English

**Description**: Get a localized string when neither current language nor English available  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized, key exists, neither current language nor English available  
**Steps**:

1. System calls `GetString(key)` with valid key
2. Provider looks up translation for current language
3. Provider doesn't find translation for current language
4. Provider falls back to English ("en")
5. Provider doesn't find English translation
6. Provider returns the key itself (fail-safe)

**Expected Result**: Key string returned (fail-safe behavior)  
**Status**: ⏳ Planned

---

### UC-007: Switch Language

**Description**: Change the current language  
**Actor**: Game system or player  
**Preconditions**: `ILocalizationProvider` initialized, language code valid  
**Steps**:

1. System sets `CurrentLanguage` property to a valid language code
2. Provider validates language code
3. Provider updates current language
4. Subsequent `GetString` calls use new language

**Expected Result**: Current language changed, subsequent calls use new language  
**Status**: ⏳ Planned

---

### UC-008: Switch Language with Invalid Code

**Description**: Attempt to set invalid language code  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized  
**Steps**:

1. System sets `CurrentLanguage` property to null or empty string
2. Provider validates language code
3. Provider throws `ArgumentException`

**Expected Result**: `ArgumentException` thrown  
**Status**: ⏳ Planned

---

### UC-009: Get Available Languages

**Description**: Get list of all available languages  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized  
**Steps**:

1. System calls `GetAvailableLanguages()`
2. Provider collects all language codes from registered data
3. Provider returns collection of language codes

**Expected Result**: Collection of available language codes (e.g., ["en", "es", "fr"])  
**Status**: ⏳ Planned

---

## Category 2: Battle Message Localization

### UC-010: Format Battle Move Used Message

**Description**: Format message when Pokemon uses a move  
**Actor**: Battle system  
**Preconditions**: `BattleMessageFormatter` uses `ILocalizationProvider`  
**Steps**:

1. Battle system calls `FormatMoveUsed(pokemon, move)`
2. Formatter uses `ILocalizationProvider.GetString(LocalizationKey.BattleUsedMove, pokemon.DisplayName, move.Name)`
3. Provider returns localized formatted string

**Expected Result**: Localized message (e.g., "Pikachu used Thunderbolt!" or "¡Pikachu usó Thunderbolt!")  
**Status**: ⏳ Planned

---

### UC-011: Format Battle Missed Message

**Description**: Format message when attack misses  
**Actor**: Battle system  
**Preconditions**: `BattleMessageFormatter` uses `ILocalizationProvider`  
**Steps**:

1. Battle system calls `Format(LocalizationKey.BattleMissed)`
2. Formatter uses `ILocalizationProvider.GetString(LocalizationKey.BattleMissed)`
3. Provider returns localized string

**Expected Result**: Localized message (e.g., "The attack missed!" or "¡El ataque falló!")  
**Status**: ⏳ Planned

---

### UC-012: Format Type Effectiveness Message

**Description**: Format message for type effectiveness  
**Actor**: Battle system  
**Preconditions**: `BattleMessageFormatter` uses `ILocalizationProvider`  
**Steps**:

1. Battle system calls `Format(LocalizationKey.TypeSuperEffective)`
2. Formatter uses `ILocalizationProvider.GetString(LocalizationKey.TypeSuperEffective)`
3. Provider returns localized string

**Expected Result**: Localized message (e.g., "It's super effective!" or "¡Es súper efectivo!")  
**Status**: ⏳ Planned

---

## Category 3: Catalog Description Localization

### UC-013: Get Move Description

**Description**: Get localized description for a move  
**Actor**: Game system  
**Preconditions**: Move exists, description localized  
**Steps**:

1. System calls `GetString(LocalizationKey.MoveDescriptionPrefix + moveId)`
2. Provider looks up key (e.g., "move_description_thunderbolt")
3. Provider returns localized description

**Expected Result**: Localized move description  
**Status**: ⏳ Planned

---

### UC-014: Get Ability Description

**Description**: Get localized description for an ability  
**Actor**: Game system  
**Preconditions**: Ability exists, description localized  
**Steps**:

1. System calls `GetString(LocalizationKey.AbilityDescriptionPrefix + abilityId)`
2. Provider looks up key (e.g., "ability_description_intimidate")
3. Provider returns localized description

**Expected Result**: Localized ability description  
**Status**: ⏳ Planned

---

### UC-015: Get Item Description

**Description**: Get localized description for an item  
**Actor**: Game system  
**Preconditions**: Item exists, description localized  
**Steps**:

1. System calls `GetString(LocalizationKey.ItemDescriptionPrefix + itemId)`
2. Provider looks up key (e.g., "item_description_leftovers")
3. Provider returns localized description

**Expected Result**: Localized item description  
**Status**: ⏳ Planned

---

## Category 4: Error Handling

### UC-016: Handle Formatting Error

**Description**: Handle error when format string is invalid  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized, key exists, format string invalid  
**Steps**:

1. System calls `GetString(key, args)` with invalid format string
2. Provider looks up translation
3. Provider attempts to format string
4. Formatting throws `FormatException`
5. Provider catches exception and returns unformatted string

**Expected Result**: Unformatted string returned (graceful degradation)  
**Status**: ⏳ Planned

---

### UC-017: Handle Null Key

**Description**: Handle null or empty key  
**Actor**: Game system  
**Preconditions**: `ILocalizationProvider` initialized  
**Steps**:

1. System calls `GetString(null)` or `GetString("")`
2. Provider checks for null/empty key
3. Provider returns empty string

**Expected Result**: Empty string returned  
**Status**: ⏳ Planned

---

## Category 5: Data Provider Operations

### UC-018: Register Localization Data

**Description**: Register new localization data  
**Actor**: Content system  
**Preconditions**: `LocalizationDataProvider` initialized  
**Steps**:

1. Content system calls `Register(key, translations)` internally
2. Provider stores data in dictionary
3. Data available for lookup

**Expected Result**: Data registered and available  
**Status**: ⏳ Planned

---

### UC-019: Get Localization Data

**Description**: Get localization data for a key  
**Actor**: Game system  
**Preconditions**: `LocalizationDataProvider` initialized, key exists  
**Steps**:

1. System calls `GetData(key)`
2. Provider looks up data in dictionary
3. Provider returns `LocalizationData` object

**Expected Result**: `LocalizationData` object with translations  
**Status**: ⏳ Planned

---

### UC-020: Check if Data Exists

**Description**: Check if localization data exists for a key  
**Actor**: Game system  
**Preconditions**: `LocalizationDataProvider` initialized  
**Steps**:

1. System calls `HasData(key)`
2. Provider checks dictionary for key
3. Provider returns boolean result

**Expected Result**: True if data exists, false otherwise  
**Status**: ⏳ Planned

---

## Related Documents

-   **[Architecture.md](architecture.md)** - Technical specification
-   **[Roadmap.md](roadmap.md)** - Implementation plan
-   **[Testing.md](testing.md)** - Testing strategy
-   **[Code Location.md](code_location.md)** - Where code lives

---

**Last Updated**: 2025-01-XX
