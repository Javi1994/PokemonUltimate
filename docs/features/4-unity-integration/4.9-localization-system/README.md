# Sub-Feature 4.9: Localization System

> Centralized text management and translation system for multi-language support.

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Localization System provides centralized text management and translation support for multiple languages. It follows the same provider pattern as `PokedexDataProvider` and `LearnsetProvider` for consistency.

**Current Status**: ⏳ Planned

## Problem Statement

Currently, text is hardcoded in English throughout the codebase:

-   `GameMessages.cs` - Battle messages in English
-   `BattleMessageFormatter.cs` - Formatted messages in English
-   Move descriptions in `MoveCatalog` - English descriptions
-   Ability descriptions in `AbilityCatalog` - English descriptions
-   Item descriptions in `ItemCatalog` - English descriptions
-   Pokedex data in `PokedexDataProvider` - English descriptions

**Issues**:

-   No support for multiple languages
-   Text changes require code changes
-   Hard to maintain and update translations
-   Violates separation of concerns (logic mixed with presentation)

## Solution: Localization Provider System

Following the same pattern as `PokedexDataProvider` and `LearnsetProvider`:

### Architecture

```
PokemonUltimate.Core/
└── Localization/
    ├── ILocalizationProvider.cs          # Interface for localization
    ├── LocalizationKey.cs                # Enum/Constants for message keys
    └── LocalizationData.cs                # Data structure for translations

PokemonUltimate.Content/
└── Providers/
    ├── LocalizationProvider.cs           # Default implementation
    └── LocalizationDataProvider.cs        # Data provider (similar to PokedexDataProvider)

PokemonUltimate.Unity/ (Future)
└── Localization/
    └── UnityLocalizationManager.cs       # Unity-specific implementation
```

### Key Design Principles

1. **Separation of Concerns**: Logic uses keys, View handles translation
2. **Provider Pattern**: Centralized data providers (consistent with existing pattern)
3. **SOLID Principles**: SRP (LocalizationProvider), OCP (extensible), DIP (ILocalizationProvider)
4. **No Hardcoded Strings**: All text comes from providers
5. **Type Safety**: Use constants/enums for keys instead of magic strings

## Implementation Plan

### Phase 1: Core Localization Infrastructure (Feature 1.20)

**Location**: `PokemonUltimate.Core/Localization/`

1. **Create `ILocalizationProvider` interface**

    ```csharp
    public interface ILocalizationProvider
    {
        string GetString(string key, params object[] args);
        bool HasString(string key);
        string CurrentLanguage { get; set; }
    }
    ```

2. **Create `LocalizationKey` constants class**

    ```csharp
    public static class LocalizationKey
    {
        // Battle Messages
        public const string BattleUsedMove = "battle_used_move";
        public const string BattleFlinched = "battle_flinched";
        public const string BattleMissed = "battle_missed";
        // ... more keys
    }
    ```

3. **Create `LocalizationData` structure**
    ```csharp
    public class LocalizationData
    {
        public string Key { get; set; }
        public Dictionary<string, string> Translations { get; set; } // Language -> Text
    }
    ```

### Phase 2: Content Localization Provider (Feature 3.10)

**Location**: `PokemonUltimate.Content/Providers/`

1. **Create `LocalizationDataProvider`**

    - Similar to `PokedexDataProvider`
    - Centralized storage for all translations
    - Organized by category (battle, moves, abilities, items, pokedex)

2. **Create `LocalizationProvider` implementation**

    - Implements `ILocalizationProvider`
    - Uses `LocalizationDataProvider` for data
    - Handles string formatting with placeholders

3. **Migrate existing texts**
    - Move `GameMessages` strings to `LocalizationDataProvider`
    - Update `BattleMessageFormatter` to use keys
    - Update catalog descriptions to use keys

### Phase 3: Unity Integration (Feature 4.9)

**Location**: `PokemonUltimate.Unity/Localization/`

1. **Create `UnityLocalizationManager`**

    - Unity-specific implementation
    - Can load from JSON/CSV/Unity Localization Package
    - Handles runtime language switching

2. **Update `UnityBattleView`**
    - Uses `ILocalizationProvider` for all messages
    - Formats messages before displaying

## Usage Examples

### Before (Current - Hardcoded)

```csharp
// In BattleMessageFormatter.cs
return $"{pokemon.DisplayName} used {move.Name}!";

// In GameMessages.cs
public const string MoveMissed = "The attack missed!";
```

### After (With Localization)

```csharp
// In BattleMessageFormatter.cs
return _localizationProvider.GetString(
    LocalizationKey.BattleUsedMove,
    pokemon.DisplayName,
    move.Name
);

// In LocalizationDataProvider.cs
Register(LocalizationKey.BattleUsedMove, new Dictionary<string, string>
{
    { "en", "{0} used {1}!" },
    { "es", "¡{0} usó {1}!" },
    { "fr", "{0} a utilisé {1}!" }
});
```

## Text Categories

1. **Battle Messages** (`battle_*`)

    - Move usage, status effects, stat changes, etc.

2. **Move Descriptions** (`move_*`)

    - Move descriptions from `MoveCatalog`

3. **Ability Descriptions** (`ability_*`)

    - Ability descriptions from `AbilityCatalog`

4. **Item Descriptions** (`item_*`)

    - Item descriptions from `ItemCatalog`

5. **Pokedex Data** (`pokedex_*`)

    - Pokemon descriptions, categories from `PokedexDataProvider`

6. **UI Messages** (`ui_*`)
    - Menu text, button labels, etc.

## Benefits

-   ✅ **Multi-language Support**: Easy to add new languages
-   ✅ **Centralized Management**: All text in one place
-   ✅ **Consistent Pattern**: Same as `PokedexDataProvider` and `LearnsetProvider`
-   ✅ **Type Safety**: Constants instead of magic strings
-   ✅ **Separation of Concerns**: Logic doesn't know about language
-   ✅ **Easy Updates**: Change translations without code changes
-   ✅ **SOLID Principles**: Follows SRP, OCP, DIP

## Related Features

-   **[Feature 1.14: Enums & Constants](../1-game-data/1.14-enums-constants/)** - Current `GameMessages` class
-   **[Feature 1.19: Pokedex Fields](../1-game-data/1.19-pokedex-fields/)** - Pokedex descriptions
-   **[Feature 3.1: Pokemon Expansion](../3-content-expansion/3.1-pokemon-expansion/)** - Pokemon descriptions
-   **[Feature 3.2: Move Expansion](../3-content-expansion/3.2-move-expansion/)** - Move descriptions
-   **[Feature 4.3: IBattleView Implementation](4.3-ibattleview-implementation/)** - Where localization is used

## Related Documents

-   **[Parent Architecture](../architecture.md)** - Localization strategy section
-   **[Parent Roadmap](../roadmap.md)** - Implementation phases
-   **[Parent Code Location](../code_location.md)** - Where localization code will live

---

**Last Updated**: 2025-01-XX
