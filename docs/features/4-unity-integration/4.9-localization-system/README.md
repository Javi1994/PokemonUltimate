# Sub-Feature 4.9: Localization System

> Centralized text management and translation system for multi-language support.

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Localization System provides centralized text management and translation support for multiple languages. It follows the same provider pattern as `PokedexDataProvider` and `LearnsetProvider` for consistency.

**Current Status**: ✅ Combat System Fully Localized (Phases 1-3 Complete + Full Combat Migration)

**Completed Phases**:

-   ✅ Phase 1: Core Localization Infrastructure
-   ✅ Phase 2: Content Localization Provider
-   ✅ Phase 3: BattleMessageFormatter Integration
-   ✅ **Phase 3.5: Full Combat System Migration** - All `GameMessages` replaced with `LocalizationProvider`

**Remaining Phases**:

-   ⏳ Phase 4: Catalog Descriptions Migration (Move/Ability/Item descriptions)
-   ⏳ Phase 5: Unity Integration

**Content Localization**:

**Names** (✅ Complete):

-   ✅ Move Names: 36 moves translated (EN, ES, FR)
-   ✅ Pokemon Names: 26 Pokemon translated (EN, ES, FR)
-   ✅ Ability Names: 43 abilities translated (EN, ES, FR)
-   ✅ Item Names: 23 items translated (EN, ES, FR)
-   ✅ Weather Names: 9 weather types translated (EN, ES, FR)
-   ✅ Terrain Names: 4 terrain types translated (EN, ES, FR)
-   ✅ Side Condition Names: 10 side conditions translated (EN, ES, FR)
-   ✅ Field Effect Names: 8 field effects translated (EN, ES, FR)
-   ✅ Hazard Names: 4 hazards translated (EN, ES, FR)
-   ✅ Status Effect Names: All status effects translated (EN, ES, FR)

**Descriptions** (⏳ Pending):

-   ⏳ Move Descriptions: Extension methods ready, translations pending
-   ⏳ Ability Descriptions: Extension methods ready, translations pending
-   ⏳ Item Descriptions: Extension methods ready, translations pending

**Total Translated**: 
- 128 content names (36 moves + 26 Pokemon + 43 abilities + 23 items)
- 35 battle messages (move execution, status effects, weather, terrain, hazards, etc.)
- All type effectiveness messages
- All stat and nature names
- All move categories

**Total Localization Keys**: ~200+ keys covering all combat messages and content names

## Problem Statement

**Previous State**: Text was hardcoded in English throughout the codebase:

-   ~~`GameMessages.cs`~~ - **DEPRECATED**: Battle messages now use `LocalizationProvider` ✅
-   `BattleMessageFormatter.cs` - **UPDATED**: Now uses `ILocalizationProvider` ✅
-   All Combat actions - **UPDATED**: Now use `LocalizationProvider` ✅
-   Move descriptions in `MoveCatalog` - Extension methods ready, translations pending ⏳
-   Ability descriptions in `AbilityCatalog` - Extension methods ready, translations pending ⏳
-   Item descriptions in `ItemCatalog` - Extension methods ready, translations pending ⏳
-   Pokedex data in `PokedexDataProvider` - English descriptions (future work)

**Current Status**:

-   ✅ **Combat System Fully Localized**: All battle messages use `LocalizationProvider`
-   ✅ **Multi-language Support**: English, Spanish, French supported
-   ✅ **No Hardcoded Messages**: All combat messages use localization keys
-   ⏳ **Content Descriptions**: Extension methods ready, translations pending

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

### Before (Deprecated - Hardcoded)

```csharp
// ❌ OLD WAY - No longer used in Combat
// In BattleMessageFormatter.cs
return $"{pokemon.DisplayName} used {move.Name}!";

// In GameMessages.cs
public const string MoveMissed = "The attack missed!";

// In ItemListener.cs
yield return new MessageAction(string.Format(GameMessages.ItemActivated, pokemon.DisplayName, itemName));
```

### After (Current - Localized)

```csharp
// ✅ CURRENT WAY - Used throughout Combat system
// In BattleMessageFormatter.cs
return _localizationProvider.GetString(
    LocalizationKey.BattleUsedMove,
    pokemon.DisplayName,
    move.GetDisplayName(_localizationProvider)
);

// In ItemListener.cs
var provider = LocalizationManager.Instance;
var itemName = _itemData.GetLocalizedName(provider);
yield return new MessageAction(provider.GetString(LocalizationKey.ItemActivated, pokemon.DisplayName, itemName));

// In LocalizationDataProvider.cs (split across partial files)
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

## Documentation

| Document                              | Purpose                             |
| ------------------------------------- | ----------------------------------- |
| **[Architecture](architecture.md)**   | Technical specification and design  |
| **[Roadmap](roadmap.md)**             | Implementation phases and plan      |
| **[Use Cases](use_cases.md)**         | All scenarios for localization      |
| **[Testing](testing.md)**             | Testing strategy and test structure |
| **[Code Location](code_location.md)** | Where code lives in the codebase    |
| **[Migration Summary](MIGRATION_SUMMARY.md)** | ✅ Complete migration summary of Combat system |

## Related Features

-   **[Feature 1.14: Enums & Constants](../1-game-data/1.14-enums-constants/)** - Current `GameMessages` class
-   **[Feature 1.19: Pokedex Fields](../1-game-data/1.19-pokedex-fields/)** - Pokedex descriptions
-   **[Feature 3.1: Pokemon Expansion](../3-content-expansion/3.1-pokemon-expansion/)** - Pokemon descriptions
-   **[Feature 3.2: Move Expansion](../3-content-expansion/3.2-move-expansion/)** - Move descriptions
-   **[Feature 4.3: IBattleView Implementation](4.3-ibattleview-implementation/)** - Where localization is used

## Quick Usage Guide

### How to Set Language to Spanish

**In Unity**:

1. Add `LocalizationManager` component to a GameObject in your scene
2. Set `Default Language` to `"es"` in the Inspector
3. Configure `BattleManager` → `Language Code` = `"es"`

**In Code**:

```csharp
var provider = new LocalizationProvider();
provider.CurrentLanguage = "es"; // Spanish
```

**See**: [`HOW_TO_USE.md`](HOW_TO_USE.md) for complete usage guide.

## Related Documents

-   **[HOW_TO_USE.md](HOW_TO_USE.md)** ⭐ - **How to use the localization system**
-   **[STANDARDIZATION_GUIDE.md](STANDARDIZATION_GUIDE.md)** ⭐⭐ - **Guía para estandarizar el uso del sistema** (NUEVO)
-   **[Architecture.md](architecture.md)** - Technical specification
-   **[Roadmap.md](roadmap.md)** - Implementation phases
-   **[Use Cases.md](use_cases.md)** - All scenarios
-   **[Testing.md](testing.md)** - Testing strategy
-   **[Code Location.md](code_location.md)** - Where code lives
-   **[CONTENT_LOCALIZATION_DESIGN.md](CONTENT_LOCALIZATION_DESIGN.md)** - Content localization design
-   **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Implementation summary
-   **[Parent Architecture](../architecture.md)** - Localization strategy section
-   **[Parent Roadmap](../roadmap.md)** - Implementation phases
-   **[Parent Code Location](../code_location.md)** - Where localization code will live

---

**Last Updated**: 2025-01-XX  
**Migration Complete**: ✅ All Combat system messages now use `LocalizationProvider`
