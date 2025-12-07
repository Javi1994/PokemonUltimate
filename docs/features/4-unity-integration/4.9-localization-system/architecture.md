# Sub-Feature 4.9: Localization System - Architecture

> **Technical specification for the localization system**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Localization System provides centralized text management and translation support for multiple languages. It follows the same provider pattern as `PokedexDataProvider` and `LearnsetProvider` for consistency.

**Current Status**: ✅ **Combat System Fully Localized**. All content names translated (200+ keys). All combat messages use `LocalizationProvider`. Descriptions extension methods ready, translations pending.

**Design Principles**:

-   **Separation of Concerns**: Logic uses keys, View handles translation
-   **Provider Pattern**: Centralized data providers (consistent with existing pattern)
-   **SOLID Principles**: SRP (LocalizationProvider), OCP (extensible), DIP (ILocalizationProvider)
-   **No Hardcoded Strings**: All text comes from providers
-   **Type Safety**: Use constants for keys instead of magic strings

## Architecture

### Component Structure

```
PokemonUltimate.Core/
└── Localization/
    ├── ILocalizationProvider.cs          # Interface for localization
    ├── LocalizationKey.cs                # Constants for message keys
    └── LocalizationData.cs                # Data structure for translations

PokemonUltimate.Content/
└── Providers/
    ├── LocalizationProvider.cs           # Default implementation
    └── LocalizationDataProvider.cs        # Data provider (similar to PokedexDataProvider)

PokemonUltimate.Unity/ (Future)
└── Localization/
    └── UnityLocalizationManager.cs       # Unity-specific implementation
```

## Core Components

### 1. ILocalizationProvider Interface

**Location**: `PokemonUltimate.Core/Localization/ILocalizationProvider.cs`

```csharp
/// <summary>
/// Interface for providing localized strings.
/// </summary>
/// <remarks>
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.9: Localization System
/// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
/// </remarks>
public interface ILocalizationProvider
{
    /// <summary>
    /// Gets a localized string by key.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <param name="args">Optional arguments for string formatting.</param>
    /// <returns>The localized string, or the key if not found.</returns>
    string GetString(string key, params object[] args);

    /// <summary>
    /// Checks if a localization key exists.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    bool HasString(string key);

    /// <summary>
    /// Gets or sets the current language code (e.g., "en", "es", "fr").
    /// </summary>
    string CurrentLanguage { get; set; }

    /// <summary>
    /// Gets all available language codes.
    /// </summary>
    /// <returns>Collection of available language codes.</returns>
    IEnumerable<string> GetAvailableLanguages();
}
```

**Design Decisions**:

-   Uses `string` for keys (not enum) for flexibility and extensibility
-   Returns key if translation not found (fail-safe behavior)
-   Supports string formatting with placeholders (`{0}`, `{1}`, etc.)
-   Language codes follow ISO 639-1 standard (2-letter codes)

### 2. LocalizationKey Constants

**Location**: `PokemonUltimate.Core/Localization/LocalizationKey.cs`

```csharp
/// <summary>
/// Constants for localization keys.
/// Organized by category for maintainability.
/// </summary>
/// <remarks>
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.9: Localization System
/// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
/// </remarks>
public static class LocalizationKey
{
    #region Battle Messages

    public const string BattleUsedMove = "battle_used_move";
    public const string BattleFlinched = "battle_flinched";
    public const string BattleMissed = "battle_missed";
    public const string BattleProtected = "battle_protected";
    public const string BattleNoPP = "battle_no_pp";
    public const string BattleAsleep = "battle_asleep";
    public const string BattleFrozen = "battle_frozen";
    public const string BattleParalyzed = "battle_paralyzed";
    // ... more battle keys

    #endregion

    #region Type Effectiveness

    public const string TypeNoEffect = "type_no_effect";
    public const string TypeSuperEffective = "type_super_effective";
    public const string TypeSuperEffective4x = "type_super_effective_4x";
    public const string TypeNotVeryEffective = "type_not_very_effective";
    public const string TypeNotVeryEffective025x = "type_not_very_effective_025x";

    #endregion

    #region Status Effects

    public const string StatusBurnDamage = "status_burn_damage";
    public const string StatusPoisonDamage = "status_poison_damage";
    // ... more status keys

    #endregion

    #region Weather & Terrain

    public const string WeatherSandstormDamage = "weather_sandstorm_damage";
    public const string WeatherHailDamage = "weather_hail_damage";
    public const string TerrainHealing = "terrain_healing";
    // ... more weather/terrain keys

    #endregion

    #region Abilities & Items

    public const string AbilityActivated = "ability_activated";
    public const string ItemActivated = "item_activated";
    // ... more ability/item keys

    #endregion

    #region Move Descriptions

    public const string MoveDescriptionPrefix = "move_description_";
    // Format: move_description_{moveId} (e.g., "move_description_thunderbolt")

    #endregion

    #region Ability Descriptions

    public const string AbilityDescriptionPrefix = "ability_description_";
    // Format: ability_description_{abilityId} (e.g., "ability_description_intimidate")

    #endregion

    #region Item Descriptions

    public const string ItemDescriptionPrefix = "item_description_";
    // Format: item_description_{itemId} (e.g., "item_description_leftovers")

    #endregion

    #region Pokedex Data

    public const string PokedexDescriptionPrefix = "pokedex_description_";
    // Format: pokedex_description_{pokemonName} (e.g., "pokedex_description_pikachu")

    #endregion
}
```

**Design Decisions**:

-   Uses snake_case for consistency with existing codebase
-   Prefixes for dynamic content (moves, abilities, items, Pokemon)
-   Organized by category for maintainability
-   Keys are constants (compile-time checked)

### 3. LocalizationData Structure

**Location**: `PokemonUltimate.Core/Localization/LocalizationData.cs`

```csharp
/// <summary>
/// Data structure for storing translations for a single key.
/// </summary>
/// <remarks>
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.9: Localization System
/// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
/// </remarks>
public class LocalizationData
{
    /// <summary>
    /// The localization key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Dictionary mapping language codes to translated strings.
    /// </summary>
    public Dictionary<string, string> Translations { get; set; }

    public LocalizationData()
    {
        Translations = new Dictionary<string, string>();
    }

    public LocalizationData(string key, Dictionary<string, string> translations)
    {
        Key = key;
        Translations = translations ?? new Dictionary<string, string>();
    }
}
```

**Design Decisions**:

-   Simple data structure (no behavior)
-   Dictionary for O(1) lookup performance
-   Immutable-friendly (can be made readonly if needed)

### 4. LocalizationDataProvider ✅ **REFACTORED INTO PARTIAL FILES**

**Location**: `PokemonUltimate.Content/Providers/Localization/` (split across 17 partial files)

**File Structure**:
- `LocalizationDataProvider.cs` - Core class with public API
- `LocalizationDataProvider.BattleMessages.cs` - Battle message translations
- `LocalizationDataProvider.Moves.cs` - Move name translations
- `LocalizationDataProvider.Pokemon.cs` - Pokemon name translations
- `LocalizationDataProvider.Abilities.cs` - Ability name translations
- `LocalizationDataProvider.Items.cs` - Item name and description translations
- `LocalizationDataProvider.Weather.cs` - Weather name and description translations
- `LocalizationDataProvider.Terrain.cs` - Terrain name and description translations
- `LocalizationDataProvider.SideConditions.cs` - Side condition translations
- `LocalizationDataProvider.FieldEffects.cs` - Field effect translations
- `LocalizationDataProvider.Hazards.cs` - Hazard translations
- `LocalizationDataProvider.StatusEffects.cs` - Status effect translations
- `LocalizationDataProvider.Types.cs` - Type and type effectiveness translations
- `LocalizationDataProvider.Natures.cs` - Nature translations
- `LocalizationDataProvider.Stats.cs` - Stat translations
- `LocalizationDataProvider.Helpers.cs` - Helper methods for key generation and registration

```csharp
/// <summary>
/// Provides localization data for all game text.
/// Centralized data source for translations.
/// Similar to PokedexDataProvider pattern.
/// 
/// This is a partial class split across multiple files for maintainability.
/// </summary>
/// <remarks>
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.9: Localization System
/// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
/// </remarks>
public static partial class LocalizationDataProvider
{
    private static readonly Dictionary<string, LocalizationData> _data =
        new Dictionary<string, LocalizationData>(StringComparer.OrdinalIgnoreCase);

    static LocalizationDataProvider()
    {
        InitializeBattleMessages();
        InitializeTypeEffectiveness();
        InitializeStatusEffects();
        InitializeWeatherTerrain();
        InitializeAbilitiesItems();
        InitializeMoveNames();
        InitializePokemonNames();
        InitializeAbilityNames();
        InitializeItemNames();
        InitializeItemDescriptions();
        InitializeNatures();
        InitializeMoveCategories();
        InitializeStats();
        InitializeWeatherNames();
        InitializeTerrainNames();
        InitializeSideConditionNames();
        InitializeFieldEffectNames();
        InitializeHazardNames();
        InitializeStatusEffectNames();
        InitializeUI();
        // InitializeMoveDescriptions(); // When moves are migrated
        // InitializeAbilityDescriptions(); // When abilities are migrated
        // InitializePokedexData(); // When Pokedex is migrated
    }

    /// <summary>
    /// Gets localization data for a key.
    /// </summary>
    public static LocalizationData GetData(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        _data.TryGetValue(key, out var data);
        return data;
    }

    /// <summary>
    /// Checks if data exists for a key.
    /// </summary>
    public static bool HasData(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        return _data.ContainsKey(key);
    }

    /// <summary>
    /// Registers localization data.
    /// </summary>
    private static void Register(string key, Dictionary<string, string> translations)
    {
        var data = new LocalizationData(key, translations);
        _data[key] = data;
    }

    #region Initialization Methods

    private static void InitializeBattleMessages()
    {
        Register(LocalizationKey.BattleUsedMove, new Dictionary<string, string>
        {
            { "en", "{0} used {1}!" },
            { "es", "¡{0} usó {1}!" },
            { "fr", "{0} a utilisé {1}!" }
        });

        Register(LocalizationKey.BattleMissed, new Dictionary<string, string>
        {
            { "en", "The attack missed!" },
            { "es", "¡El ataque falló!" },
            { "fr", "L'attaque a échoué!" }
        });

        // ... more battle messages
    }

    private static void InitializeTypeEffectiveness()
    {
        Register(LocalizationKey.TypeNoEffect, new Dictionary<string, string>
        {
            { "en", "It has no effect..." },
            { "es", "No tuvo efecto..." },
            { "fr", "Cela n'a aucun effet..." }
        });

        // ... more type effectiveness messages
    }

    // ... more initialization methods

    #endregion
}
```

**Design Decisions**:

-   Static partial class (similar to `PokedexDataProvider`, split for maintainability)
-   Case-insensitive key lookup
-   Organized initialization methods by category (split across partial files)
-   Easy to extend with new languages
-   **Refactored into 17 partial files** following the same pattern as `MoveCatalog` and other catalogs

### 5. LocalizationProvider Implementation

**Location**: `PokemonUltimate.Content/Providers/LocalizationProvider.cs`

```csharp
/// <summary>
/// Default implementation of ILocalizationProvider.
/// Uses LocalizationDataProvider for data.
/// </summary>
/// <remarks>
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.9: Localization System
/// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
/// </remarks>
public class LocalizationProvider : ILocalizationProvider
{
    private string _currentLanguage = "en";

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(ErrorMessages.LanguageCodeCannotBeEmpty, nameof(value));
            _currentLanguage = value;
        }
    }

    public string GetString(string key, params object[] args)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        var data = LocalizationDataProvider.GetData(key);
        if (data == null)
            return key; // Return key if not found (fail-safe)

        if (!data.Translations.TryGetValue(_currentLanguage, out var translation))
        {
            // Fallback to English if current language not available
            if (!data.Translations.TryGetValue("en", out translation))
                return key; // Return key if English not available
        }

        if (args == null || args.Length == 0)
            return translation;

        try
        {
            return string.Format(translation, args);
        }
        catch (FormatException)
        {
            // Return unformatted string if formatting fails
            return translation;
        }
    }

    public bool HasString(string key)
    {
        return LocalizationDataProvider.HasData(key);
    }

    public IEnumerable<string> GetAvailableLanguages()
    {
        var languages = new HashSet<string>();
        // Collect all languages from all registered data
        // Implementation details...
        return languages;
    }
}
```

**Design Decisions**:

-   Default language is English ("en")
-   Fallback to English if current language not available
-   Returns key if translation not found (fail-safe)
-   Handles formatting errors gracefully
-   Validates language code on set

## Integration Points

### 1. BattleMessageFormatter Integration ✅ **COMPLETE**

**Previous**: Used `GameMessages` constants directly  
**Current**: Uses `ILocalizationProvider` with `LocalizationKey` constants ✅

```csharp
// ❌ Before (Deprecated)
public string FormatMoveUsed(PokemonInstance pokemon, MoveData move)
{
    return $"{pokemon.DisplayName} used {move.Name}!";
}

// ✅ After (Current Implementation)
public string FormatMoveUsed(PokemonInstance pokemon, MoveData move)
{
    return _localizationProvider.GetString(
        LocalizationKey.BattleUsedMove,
        pokemon.Species.GetDisplayName(_localizationProvider),
        move.GetDisplayName(_localizationProvider)
    );
}
```

**Changes Completed** ✅:

-   ✅ Added `ILocalizationProvider` dependency to `BattleMessageFormatter`
-   ✅ Updated constructor to accept `ILocalizationProvider` (with default parameter)
-   ✅ Replaced all hardcoded strings with `LocalizationKey` constants
-   ✅ Updated all `Format` calls to use localization
-   ✅ **All Combat actions now use `LocalizationProvider`**

### 2. GameMessages Migration ✅ **COMPLETE**

**Status**: ✅ **Fully Migrated in Combat System**

**Completed Phases**:

-   ✅ Phase 1: Added `LocalizationKey` constants for all `GameMessages`
-   ✅ Phase 2: Added translations to `LocalizationDataProvider` (split into partial files)
-   ✅ Phase 3: Updated all Combat code to use `ILocalizationProvider` instead of `GameMessages`
-   ✅ Phase 3.5: Replaced all hardcoded item/ability/hazard name comparisons with catalog lookups

**Current State**:

-   ✅ **Combat System**: All messages use `LocalizationProvider` (no `GameMessages` usage)
-   ✅ **Core**: `GameMessages` only used in `TypeEffectiveness.GetEffectivenessDescription()` (not used in production)
-   ✅ **LocalizationDataProvider**: Split into 17 partial files for maintainability
-   ✅ **Extension Methods**: All content types have `GetLocalizedName()` / `GetDisplayName()` methods

### 3. Catalog Descriptions Migration

**Move Descriptions**: `MoveCatalog` → `LocalizationDataProvider`  
**Ability Descriptions**: `AbilityCatalog` → `LocalizationDataProvider`  
**Item Descriptions**: `ItemCatalog` → `LocalizationDataProvider`  
**Pokedex Descriptions**: `PokedexDataProvider` → `LocalizationDataProvider` (or keep separate)

**Strategy**: Use prefix pattern for dynamic content:

-   `move_description_{moveId}`
-   `ability_description_{abilityId}`
-   `item_description_{itemId}`
-   `pokedex_description_{pokemonName}`

## Language Support

### Supported Languages (Initial)

-   **English (en)**: Default language
-   **Spanish (es)**: Full support
-   **French (fr)**: Full support

### Adding New Languages

1. Add language code to `LocalizationDataProvider` initialization methods
2. Add translations for all keys
3. Language automatically available via `ILocalizationProvider`

### Language Code Format

-   Follows ISO 639-1 standard (2-letter codes)
-   Examples: "en", "es", "fr", "de", "ja", "zh"

## Error Handling

### Missing Translations

-   **Behavior**: Returns key if translation not found
-   **Fallback**: Falls back to English if current language not available
-   **Rationale**: Fail-safe behavior prevents crashes

### Invalid Formatting

-   **Behavior**: Returns unformatted string if formatting fails
-   **Rationale**: Prevents crashes from malformed format strings

### Invalid Language Code

-   **Behavior**: Throws `ArgumentException` when setting invalid language code
-   **Rationale**: Fail-fast for invalid configuration

## Performance Considerations

### Lookup Performance

-   **Dictionary Lookup**: O(1) average case
-   **Case-Insensitive**: Uses `StringComparer.OrdinalIgnoreCase`
-   **Caching**: No caching needed (static data)

### Memory Usage

-   **Static Data**: All translations loaded at startup
-   **Memory Impact**: Minimal (text data is small)
-   **Future**: Could lazy-load languages if needed

## Testing Strategy

See [`testing.md`](testing.md) for complete testing strategy.

**Key Test Areas**:

-   `ILocalizationProvider` interface compliance
-   `LocalizationProvider` implementation
-   `LocalizationDataProvider` data integrity
-   Fallback behavior
-   Formatting with placeholders
-   Language switching
-   Missing key handling

## Related Features

-   **[Feature 1.14: Enums & Constants](../1-game-data/1.14-enums-constants/)** - `GameMessages` class (deprecated, only used in non-production code)
-   **[Feature 1.19: Pokedex Fields](../1-game-data/1.19-pokedex-fields/)** - Pokedex descriptions
-   **[Feature 2: Combat System](../2-combat-system/)** - ✅ **Fully localized**: All combat messages use `LocalizationProvider`
-   **[Feature 2.1: Battle Foundation](../2-combat-system/2.1-battle-foundation/)** - `BattleMessageFormatter` integration ✅
-   **[Feature 4.3: IBattleView Implementation](4.3-ibattleview-implementation/)** - Where localization is used

## Related Documents

-   **[README.md](README.md)** - Overview and problem statement
-   **[Roadmap.md](roadmap.md)** - Implementation phases
-   **[Use Cases.md](use_cases.md)** - All scenarios
-   **[Testing.md](testing.md)** - Testing strategy
-   **[Code Location.md](code_location.md)** - Where code lives

---

**Last Updated**: 2025-01-XX  
**Migration Complete**: ✅ All Combat system messages now use `LocalizationProvider`  
**LocalizationDataProvider**: ✅ Refactored into 17 partial files for maintainability
