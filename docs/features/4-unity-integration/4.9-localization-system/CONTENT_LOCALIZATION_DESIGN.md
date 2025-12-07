# Content Localization Design - Moves & Pokemon Names

> **Design document for localizing move and Pokemon names**

## Overview

This document describes how to localize content names (Moves, Pokemon, Abilities, Items) while maintaining the English names as internal IDs.

## Problem Statement

Currently:

-   `MoveData.Name` = "Thunderbolt" (hardcoded English)
-   `PokemonSpeciesData.Name` = "Pikachu" (hardcoded English)
-   These names are used as IDs throughout the codebase
-   No way to display translated names to players

**Requirements**:

1. Keep English names as internal IDs (for code compatibility)
2. Support translated display names
3. Maintain backward compatibility
4. Easy to add new languages

## Solution: Display Name Extension Pattern

### Approach

1. **Keep English names as IDs**: `MoveData.Name` and `PokemonSpeciesData.Name` remain unchanged
2. **Add localization keys**: Use pattern `move_name_{moveId}` and `pokemon_name_{pokemonId}`
3. **Add extension methods**: `GetDisplayName()` that returns translated name
4. **Fallback to English**: If translation not found, return original name

### Architecture

```
MoveData.Name = "Thunderbolt" (ID, never changes)
MoveData.GetDisplayName(localizationProvider) = "Thunderbolt" (en) / "Rayo" (es) / "Tonnerre" (fr)

PokemonSpeciesData.Name = "Pikachu" (ID, never changes)
PokemonSpeciesData.GetDisplayName(localizationProvider) = "Pikachu" (en) / "Pikachu" (es) / "Pikachu" (fr)
```

### Localization Key Pattern

**Moves**:

-   Key format: `move_name_{moveId}` (lowercase, spaces replaced with underscores)
-   Example: `move_name_thunderbolt`, `move_name_flamethrower`

**Pokemon**:

-   Key format: `pokemon_name_{pokemonId}` (lowercase, spaces replaced with underscores)
-   Example: `pokemon_name_pikachu`, `pokemon_name_charizard`

**Move Descriptions**:

-   Key format: `move_description_{moveId}`
-   Example: `move_description_thunderbolt`

**Pokemon Descriptions** (Pokedex):

-   Key format: `pokedex_description_{pokemonId}`
-   Example: `pokedex_description_pikachu`

## Implementation Plan

### Phase 1: Extension Methods

**Location**: `PokemonUltimate.Core/Extensions/`

1. Create `MoveDataExtensions.cs`

    - `GetDisplayName(ILocalizationProvider)` method
    - `GetDescription(ILocalizationProvider)` method

2. Create `PokemonSpeciesDataExtensions.cs`
    - `GetDisplayName(ILocalizationProvider)` method

### Phase 2: Localization Data

**Location**: `PokemonUltimate.Content/Providers/LocalizationDataProvider.cs`

1. Add initialization methods:

    - `InitializeMoveNames()` - All move names
    - `InitializePokemonNames()` - All Pokemon names
    - `InitializeMoveDescriptions()` - All move descriptions

2. Use helper method to generate keys:
    ```csharp
    private static string GenerateMoveKey(string moveName) =>
        $"move_name_{moveName.ToLowerInvariant().Replace(" ", "_")}";
    ```

### Phase 3: Integration

**Location**: Various (UI, Battle messages, etc.)

1. Update `BattleMessageFormatter` to use `GetDisplayName()`
2. Update UI components to use `GetDisplayName()`
3. Keep backward compatibility (original `Name` property still works)

## Key Generation Rules

### Move Names

-   Convert to lowercase
-   Replace spaces with underscores
-   Remove special characters (keep alphanumeric and underscores)
-   Examples:
    -   "Thunderbolt" → `move_name_thunderbolt`
    -   "Fire Blast" → `move_name_fire_blast`
    -   "X-Scissor" → `move_name_x_scissor`

### Pokemon Names

-   Convert to lowercase
-   Replace spaces with underscores
-   Remove special characters
-   Examples:
    -   "Pikachu" → `pokemon_name_pikachu`
    -   "Mr. Mime" → `pokemon_name_mr_mime`
    -   "Farfetch'd" → `pokemon_name_farfetchd`

## Example Usage

### Before (Current)

```csharp
var move = MoveCatalog.Thunderbolt;
var message = $"{pokemon.Name} used {move.Name}!";
// Result: "Pikachu used Thunderbolt!"
```

### After (With Localization)

```csharp
var move = MoveCatalog.Thunderbolt;
var message = $"{pokemon.GetDisplayName(_localizationProvider)} used {move.GetDisplayName(_localizationProvider)}!";
// Result (en): "Pikachu used Thunderbolt!"
// Result (es): "Pikachu usó Rayo!"
// Result (fr): "Pikachu a utilisé Tonnerre!"
```

## Data Structure

### LocalizationDataProvider Structure

```csharp
private static void InitializeMoveNames()
{
    Register("move_name_thunderbolt", new Dictionary<string, string>
    {
        { "en", "Thunderbolt" },
        { "es", "Rayo" },
        { "fr", "Tonnerre" }
    });

    Register("move_name_flamethrower", new Dictionary<string, string>
    {
        { "en", "Flamethrower" },
        { "es", "Lanzallamas" },
        { "fr", "Lance-Flammes" }
    });

    // ... more moves
}
```

## Benefits

1. **Backward Compatibility**: Original `Name` property still works
2. **Type Safety**: Extension methods provide compile-time checking
3. **Easy Migration**: Can be done incrementally
4. **Performance**: Lookup is O(1) with dictionary
5. **Maintainability**: All translations in one place

## Migration Strategy

1. **Phase 1**: Add extension methods (no breaking changes)
2. **Phase 2**: Add translations to `LocalizationDataProvider`
3. **Phase 3**: Update UI/battle messages to use `GetDisplayName()` incrementally
4. **Phase 4**: Keep `Name` for internal use, `GetDisplayName()` for display

## Related Documents

-   **[Architecture.md](architecture.md)** - Core localization architecture
-   **[Roadmap.md](roadmap.md)** - Implementation phases

---

**Last Updated**: 2025-01-XX
