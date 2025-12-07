# Sub-Feature 4.9: Localization System - Code Location

> **Where localization code lives in the codebase**

**Sub-Feature Number**: 4.9  
**Parent Feature**: Feature 4: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Code Organization

### Core Infrastructure

**Location**: `PokemonUltimate.Core/Localization/`

```
PokemonUltimate.Core/
└── Localization/
    ├── ILocalizationProvider.cs          # Interface for localization
    ├── LocalizationKey.cs                # Constants for message keys
    └── LocalizationData.cs                # Data structure for translations
```

**Files**:

-   `ILocalizationProvider.cs` - Interface definition
-   `LocalizationKey.cs` - Constants class with all localization keys
-   `LocalizationData.cs` - Data structure for storing translations

---

### Content Providers

**Location**: `PokemonUltimate.Content/Providers/`

```
PokemonUltimate.Content/
└── Providers/
    ├── LocalizationProvider.cs           # Default implementation
    └── LocalizationDataProvider.cs        # Data provider (static class)
```

**Files**:

-   `LocalizationProvider.cs` - Default implementation of `ILocalizationProvider`
-   `LocalizationDataProvider.cs` - Static data provider (similar to `PokedexDataProvider`)

---

### Combat Integration

**Location**: `PokemonUltimate.Combat/Messages/`

```
PokemonUltimate.Combat/
└── Messages/
    ├── IBattleMessageFormatter.cs        # Interface (updated)
    └── BattleMessageFormatter.cs           # Implementation (updated)
```

**Files**:

-   `IBattleMessageFormatter.cs` - May need updates to accept `ILocalizationProvider`
-   `BattleMessageFormatter.cs` - Updated to use `ILocalizationProvider` instead of `GameMessages`

---

### Tests

**Location**: `PokemonUltimate.Tests/Systems/Localization/`

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

**Files**:

-   `LocalizationProviderTests.cs` - Functional tests for `LocalizationProvider`
-   `LocalizationProviderEdgeCasesTests.cs` - Edge cases for `LocalizationProvider`
-   `LocalizationDataProviderTests.cs` - Functional tests for `LocalizationDataProvider`
-   `LocalizationDataProviderEdgeCasesTests.cs` - Edge cases for `LocalizationDataProvider`
-   `Integration/BattleMessageFormatterLocalizationTests.cs` - Integration tests
-   `Integration/CatalogLocalizationTests.cs` - Catalog integration tests

---

### Unity Integration (Future)

**Location**: `PokemonUltimate.Unity/Localization/` (Future)

```
PokemonUltimate.Unity/
└── Localization/
    └── UnityLocalizationManager.cs       # Unity-specific implementation
```

**Files**:

-   `UnityLocalizationManager.cs` - Unity-specific localization manager (future work)

---

## File Reference Table

| Component         | File Path                                                       | Purpose                     |
| ----------------- | --------------------------------------------------------------- | --------------------------- |
| **Interface**     | `PokemonUltimate.Core/Localization/ILocalizationProvider.cs`    | Core interface              |
| **Keys**          | `PokemonUltimate.Core/Localization/LocalizationKey.cs`          | Constants for keys          |
| **Data**          | `PokemonUltimate.Core/Localization/LocalizationData.cs`         | Data structure              |
| **Provider**      | `PokemonUltimate.Content/Providers/LocalizationProvider.cs`     | Default implementation      |
| **Data Provider** | `PokemonUltimate.Content/Providers/LocalizationDataProvider.cs` | Static data source          |
| **Formatter**     | `PokemonUltimate.Combat/Messages/BattleMessageFormatter.cs`     | Updated to use localization |
| **Tests**         | `PokemonUltimate.Tests/Systems/Localization/`                   | All tests                   |

---

## Namespace Organization

### Core Namespaces

```csharp
namespace PokemonUltimate.Core.Localization
{
    public interface ILocalizationProvider { }
    public static class LocalizationKey { }
    public class LocalizationData { }
}
```

### Content Namespaces

```csharp
namespace PokemonUltimate.Content.Providers
{
    public class LocalizationProvider : ILocalizationProvider { }
    public static class LocalizationDataProvider { }
}
```

### Combat Namespaces

```csharp
namespace PokemonUltimate.Combat.Messages
{
    public interface IBattleMessageFormatter { }
    public class BattleMessageFormatter : IBattleMessageFormatter { }
}
```

---

## Related Files

### Files That Use Localization ✅ **ALL UPDATED**

**Combat System** (✅ All files updated):

-   ✅ `PokemonUltimate.Combat/Actions/UseMoveAction.cs` - Uses `LocalizationProvider` via `BattleMessageFormatter`
-   ✅ `PokemonUltimate.Combat/Actions/DamageAction.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Actions/ContactDamageAction.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Engine/EndOfTurnProcessor.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Engine/EntryHazardProcessor.cs` - Uses `LocalizationProvider` with localized hazard names
-   ✅ `PokemonUltimate.Combat/Events/AbilityListener.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Events/ItemListener.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Effects/ProtectEffectProcessor.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Effects/CounterEffectProcessor.cs` - Uses `LocalizationProvider` directly
-   ✅ `PokemonUltimate.Combat/Statistics/Trackers/DamageTracker.cs` - Uses `LocalizationProvider` for message detection

**Content Catalogs** (Future):

-   `PokemonUltimate.Content/Catalogs/Moves/MoveCatalog.cs` - Move descriptions
-   `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.cs` - Ability descriptions
-   `PokemonUltimate.Content/Catalogs/Items/ItemCatalog.cs` - Item descriptions

---

## Migration Impact

### Migration Status ✅

**Phase 3: BattleMessageFormatter Integration** ✅ **COMPLETE**:

-   ✅ `PokemonUltimate.Combat/Messages/BattleMessageFormatter.cs` - Uses `ILocalizationProvider`
-   ✅ `PokemonUltimate.Combat/Messages/IBattleMessageFormatter.cs` - No changes needed
-   ✅ All call sites of `BattleMessageFormatter` - Use default or pass `ILocalizationProvider`
-   ✅ **All Combat actions** - Now use `LocalizationProvider` directly or via `BattleMessageFormatter`

**Phase 4: Catalog Migration** ⏳ **EXTENSION METHODS READY**:

-   ✅ Extension methods created for all content types
-   ⏳ `PokemonUltimate.Content/Catalogs/Moves/MoveCatalog.cs` - Extension ready, translations pending
-   ⏳ `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.cs` - Extension ready, translations pending
-   ⏳ `PokemonUltimate.Content/Catalogs/Items/ItemCatalog.cs` - Extension ready, translations pending

---

## Code Examples

### Creating LocalizationProvider

```csharp
// In test setup or dependency injection
var localizationProvider = new LocalizationProvider();
localizationProvider.CurrentLanguage = "en";
```

### Using LocalizationProvider

```csharp
// In BattleMessageFormatter
public class BattleMessageFormatter : IBattleMessageFormatter
{
    private readonly ILocalizationProvider _localizationProvider;

    public BattleMessageFormatter(ILocalizationProvider localizationProvider)
    {
        _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
    }

    public string FormatMoveUsed(PokemonInstance pokemon, MoveData move)
    {
        return _localizationProvider.GetString(
            LocalizationKey.BattleUsedMove,
            pokemon.DisplayName,
            move.Name
        );
    }
}
```

---

## Related Documents

-   **[Architecture.md](architecture.md)** - Technical specification
-   **[Roadmap.md](roadmap.md)** - Implementation plan
-   **[Use Cases.md](use_cases.md)** - All scenarios
-   **[Testing.md](testing.md)** - Testing strategy

---

**Last Updated**: 2025-01-XX  
**Migration Complete**: ✅ All Combat system files updated to use `LocalizationProvider`
