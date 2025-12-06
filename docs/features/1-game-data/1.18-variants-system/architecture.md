# Sub-Feature 1.18: Variants System - Architecture

> Complete technical specification for implementing Mega Evolutions, Dinamax, and Terracristalización as separate Pokemon species with different stats.

**Sub-Feature Number**: 1.18  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

Instead of temporary transformations during battle, **Mega Evolutions**, **Dinamax**, and **Terracristalización** will be implemented as **separate Pokemon species** with their own `PokemonSpeciesData` entries. Each variant has:
- Unique name (e.g., "Mega Charizard X", "Charizard Dinamax", "Charizard Tera Fire")
- Different base stats
- Potentially different types
- Potentially different abilities
- Own learnset (can inherit from base or be unique)

**Design Philosophy**: Each variant is a distinct Pokemon that can be obtained/encountered separately, not a transformation of the base form.

---

## 1. Variant Types

### 1.1 Mega Evolutions

**Characteristics**:
- Permanent form (not temporary)
- Usually higher BST (Base Stat Total) than base form
- May change types (e.g., Mega Charizard X: Fire/Dragon)
- Usually has a unique ability
- Some Pokemon have multiple Mega forms (X and Y variants)

**Examples**:
- `Charizard` → `Mega Charizard X` (Fire/Dragon, Tough Claws)
- `Charizard` → `Mega Charizard Y` (Fire/Flying, Drought)
- `Venusaur` → `Mega Venusaur` (Grass/Poison, Thick Fat)
- `Blastoise` → `Mega Blastoise` (Water, Mega Launcher)

**Naming Convention**: `"Mega {BaseName}"` or `"Mega {BaseName} {Variant}"` (e.g., "Mega Charizard X")

---

### 1.2 Dinamax

**Characteristics**:
- Permanent form (not temporary)
- Significantly increased HP (typically 2x base HP)
- Other stats remain similar to base form
- May have access to special "Max Moves"
- Gigantamax is a special variant with unique appearance and Max Move

**Examples**:
- `Charizard` → `Charizard Dinamax` (156 HP vs 78 base)
- `Charizard` → `Charizard Gigantamax` (special appearance + unique Max Move)

**Naming Convention**: `"{BaseName} Dinamax"` or `"{BaseName} Gigantamax"`

---

### 1.3 Terracristalización

**Characteristics**:
- Permanent form (not temporary)
- Changes to mono-type (Tera type)
- Same base stats as base form
- May have stat boost when Terracristalizado
- Multiple variants per base form (one per Tera type)

**Examples**:
- `Charizard` → `Charizard Tera Fire` (mono-Fire)
- `Charizard` → `Charizard Tera Dragon` (mono-Dragon)
- `Pikachu` → `Pikachu Tera Flying` (mono-Flying)

**Naming Convention**: `"{BaseName} Tera {Type}"`

---

### 1.4 Regional Forms

**Characteristics**:
- Permanent form (not temporary)
- Can have different types, stats, abilities, or be purely visual
- Examples include Alolan, Galarian, Hisuian, Paldean forms
- Some regional forms are purely cosmetic (same stats/types/abilities)
- Others have significant gameplay differences

**Examples with Gameplay Changes**:
- `Vulpix` → `Alolan Vulpix` (Fire → Ice type, different stats)
- `Meowth` → `Galarian Meowth` (Normal → Steel type)
- `Zorua` → `Hisuian Zorua` (Dark → Normal/Ghost)

**Examples Purely Visual**:
- `Pikachu` → `Pikachu (Cosmetic)` (same stats/types/abilities, visual only)
- Some Spinda forms (visual patterns only)

**Naming Convention**: `"{Region} {BaseName}"` or `"{BaseName} ({Region})"` (e.g., "Alolan Vulpix", "Galarian Meowth")

**Regional Identifiers**: "Alola", "Galar", "Hisui", "Paldea", "Cosmetic", etc.

---

## 2. Required Fields

### 2.1 PokemonSpeciesData Extensions

```csharp
public class PokemonSpeciesData : IIdentifiable {
    // ... existing fields ...
    
    // Variant fields (NEW)
    public PokemonSpeciesData BaseForm { get; set; }  // Reference to base Pokemon (null if base form)
    public PokemonVariantType? VariantType { get; set; }  // Mega, Dinamax, Tera, Regional (null if base form)
    public PokemonType? TeraType { get; set; }  // Tera type for Terracristalización variants
    public string RegionalForm { get; set; }  // Regional identifier for Regional variants (e.g., "Alola", "Galar")
    public List<PokemonSpeciesData> Variants { get; set; }  // List of all variant forms
    
    // Computed properties
    public bool IsVariant => BaseForm != null;
    public bool IsBaseForm => BaseForm == null;
    public bool IsMegaVariant => VariantType == PokemonVariantType.Mega;
    public bool IsDinamaxVariant => VariantType == PokemonVariantType.Dinamax;
    public bool IsTeraVariant => VariantType == PokemonVariantType.Tera;
    public bool IsRegionalVariant => VariantType == PokemonVariantType.Regional;
    
    // Gameplay change detection
    public bool HasGameplayChanges { get; }  // Returns true if variant has different stats/types/abilities
}
```

### 2.2 PokemonVariantType Enum

```csharp
public enum PokemonVariantType {
    Mega,        // Stat/type/ability changes
    Dinamax,     // HP increase (2x)
    Tera,         // Type change (mono-type)
    Regional     // Regional forms (may or may not have gameplay changes)
}
```

---

## 3. Builder Support

### 3.1 PokemonBuilder Extensions

```csharp
public class PokemonBuilder {
    // ... existing methods ...
    
    // Variant methods (NEW)
    public PokemonBuilder AsMegaVariant(PokemonSpeciesData baseForm, string variant = null);
    public PokemonBuilder AsDinamaxVariant(PokemonSpeciesData baseForm);
    public PokemonBuilder AsTeraVariant(PokemonSpeciesData baseForm, PokemonType teraType);
    public PokemonBuilder AsRegionalVariant(PokemonSpeciesData baseForm, string region);
}
```

### 3.2 Usage Examples

```csharp
// Base form
var Charizard = Pokemon.Define("Charizard", 6)
    .Types(PokemonType.Fire, PokemonType.Flying)
    .Stats(78, 84, 78, 109, 85, 100)
    .Build();

// Mega Charizard X
var MegaCharizardX = Pokemon.Define("Mega Charizard X", 6)
    .Types(PokemonType.Fire, PokemonType.Dragon)  // Type change!
    .Stats(78, 130, 111, 130, 85, 100)  // Higher BST
    .Ability(AbilityCatalog.ToughClaws)
    .AsMegaVariant(Charizard, "X")
    .Build();

// Charizard Dinamax
var CharizardDinamax = Pokemon.Define("Charizard Dinamax", 6)
    .Stats(156, 84, 78, 109, 85, 100)  // HP doubled
    .AsDinamaxVariant(Charizard)
    .Build();

// Charizard Tera Fire
var CharizardTeraFire = Pokemon.Define("Charizard Tera Fire", 6)
    .Type(PokemonType.Fire)  // Mono-type
    .Stats(78, 84, 78, 109, 85, 100)  // Same stats
    .AsTeraVariant(Charizard, PokemonType.Fire)
    .Build();

// Alolan Vulpix (Regional form with gameplay changes)
var Vulpix = Pokemon.Define("Vulpix", 37)
    .Type(PokemonType.Fire)
    .Stats(38, 41, 40, 50, 65, 65)
    .Build();

var AlolanVulpix = Pokemon.Define("Alolan Vulpix", 37)
    .Type(PokemonType.Ice)  // Different type!
    .Stats(38, 41, 40, 50, 65, 65)  // Same stats
    .AsRegionalVariant(Vulpix, "Alola")
    .Build();

// Pikachu Cosmetic (Regional form, purely visual)
var PikachuCosmetic = Pokemon.Define("Pikachu (Cosmetic)", 25)
    .Type(PokemonType.Electric)  // Same type
    .Stats(35, 55, 40, 50, 50, 90)  // Same stats
    .AsRegionalVariant(Pikachu, "Cosmetic")
    .Build();
```

---

## 4. VariantProvider

### 4.1 Overview

The `VariantProvider` is a centralized provider for managing Pokemon variants, following the same pattern as `PokedexDataProvider` and `LearnsetProvider`. It provides:

- Centralized variant creation and registration
- Easy querying of variants for any Pokemon
- Automatic bidirectional relationship establishment
- Organized by generation for maintainability

### 4.2 Location

**Namespace**: `PokemonUltimate.Content.Providers`  
**File**: `PokemonUltimate.Content/Providers/VariantProvider.cs`

### 4.3 API

```csharp
public static class VariantProvider
{
    // Get all variants for a Pokemon
    public static IEnumerable<PokemonSpeciesData> GetVariants(PokemonSpeciesData baseForm);
    public static IEnumerable<PokemonSpeciesData> GetVariants(string basePokemonName);
    
    // Check if Pokemon has variants
    public static bool HasVariants(PokemonSpeciesData baseForm);
    
    // Get variants by type
    public static IEnumerable<PokemonSpeciesData> GetMegaVariants(PokemonSpeciesData baseForm);
    public static IEnumerable<PokemonSpeciesData> GetRegionalVariants(PokemonSpeciesData baseForm);
    public static IEnumerable<PokemonSpeciesData> GetTeraVariants(PokemonSpeciesData baseForm);
}
```

### 4.4 Extension Methods

For convenience, extension methods are provided:

```csharp
// In PokemonSpeciesDataExtensions.cs
public static IEnumerable<PokemonSpeciesData> GetVariants(this PokemonSpeciesData pokemon);
public static bool HasVariantsAvailable(this PokemonSpeciesData pokemon);
```

### 4.5 Usage Examples

```csharp
// Option 1: Direct provider access
var variants = VariantProvider.GetVariants(PokemonCatalog.Charizard);
var hasVariants = VariantProvider.HasVariants(PokemonCatalog.Charizard);
var megaVariants = VariantProvider.GetMegaVariants(PokemonCatalog.Charizard);

// Option 2: Extension methods (more convenient)
var variants = PokemonCatalog.Charizard.GetVariants();
var hasVariants = PokemonCatalog.Charizard.HasVariantsAvailable();

// Option 3: Direct property access (after variants are registered)
var variants = PokemonCatalog.Charizard.Variants;
var hasVariants = PokemonCatalog.Charizard.HasVariants;
```

### 4.6 Adding New Variants

To add new variants, edit `InitializeGen1Variants()` (or appropriate generation method) in `VariantProvider.cs`:

```csharp
private static void InitializeGen1Variants()
{
    if (PokemonCatalog.Charizard != null)
    {
        var megaCharizardX = Pokemon.Define("Mega Charizard X", 6)
            .Types(PokemonType.Fire, PokemonType.Dragon)
            .Stats(78, 130, 111, 130, 85, 100)
            .Ability(AbilityCatalog.ToughClaws)
            .AsMegaVariant(PokemonCatalog.Charizard, "X")
            .Build();
        RegisterVariant(PokemonCatalog.Charizard, megaCharizardX);
    }
}
```

---

## 5. Learnset Inheritance

Variants can inherit moves from base form or have unique learnsets:

```csharp
// Option 1: Inherit all moves from base
var MegaCharizardX = Pokemon.Define("Mega Charizard X", 6)
    .AsMegaVariant(Charizard, "X")
    .InheritLearnset()  // Inherit all moves from Charizard
    .Build();

// Option 2: Custom learnset (override)
var MegaCharizardX = Pokemon.Define("Mega Charizard X", 6)
    .AsMegaVariant(Charizard, "X")
    .Moves(m => m
        .StartsWith(MoveCatalog.DragonClaw)  // Unique move
        .AtLevel(50, MoveCatalog.FireBlast))
    .Build();
```

---

## 6. Validation Rules

1. **Base Form Validation**: BaseForm must be non-null for variants
2. **Variant Type Validation**: VariantType must match variant type (Mega/Dinamax/Tera/Regional)
3. **Tera Type Validation**: TeraType must be set for Tera variants
4. **Regional Form Validation**: RegionalForm must be non-empty for Regional variants
5. **Name Validation**: Variant names must follow naming conventions
6. **BST Validation**: Mega variants typically have higher BST than base form
7. **HP Validation**: Dinamax variants have 2x HP
8. **Gameplay Changes Detection**: System automatically detects if variants have gameplay changes (stats/types/abilities differ from base form)

---

## 6. Registry Integration

Variants are stored in the same PokemonRegistry as base forms:

```csharp
var registry = new PokemonRegistry();
registry.Register(Charizard);
registry.Register(MegaCharizardX);
registry.Register(CharizardDinamax);
registry.Register(CharizardTeraFire);

// Query variants
var variants = registry.GetVariantsOf(Charizard);  // Returns all variants
var megaForms = registry.GetMegaVariants();  // Returns all Mega forms
var regionalForms = registry.GetRegionalVariants();  // Returns all Regional forms
var alolanForms = registry.GetRegionalVariantsByRegion("Alola");  // Returns Alolan forms
var gameplayVariants = registry.GetVariantsWithGameplayChanges();  // Variants with stat/type/ability changes
var visualOnly = registry.GetVisualOnlyVariants();  // Purely visual variants
```

---

## Status

✅ **COMPLETE** - This sub-feature is fully implemented and tested.

**Implementation Summary**:
- ✅ PokemonVariantType enum created (Mega, Dinamax, Tera, Regional, Cosmetic)
- ✅ Variant fields added to PokemonSpeciesData (BaseForm, VariantType, TeraType, RegionalForm, Variants)
- ✅ Computed properties (IsVariant, IsBaseForm, IsMegaVariant, IsDinamaxVariant, IsTeraVariant, IsRegionalVariant, IsCosmeticVariant, HasVariants, VariantCount, MegaVariants, DinamaxVariants, TeraVariants, RegionalVariants, CosmeticVariants, VariantsWithGameplayChanges, VisualOnlyVariants)
- ✅ HasGameplayChanges property for detecting variants with gameplay differences
- ✅ Variant builder methods implemented (AsMegaVariant, AsDinamaxVariant, AsTeraVariant, AsRegionalVariant, AsCosmeticVariant)
- ✅ Variant registry query methods implemented (GetVariantsOf, GetMegaVariants, GetDinamaxVariants, GetTeraVariants, GetRegionalVariants, GetRegionalVariantsByRegion, GetVariantsWithGameplayChanges, GetVisualOnlyVariants)
- ✅ VariantProvider implemented for centralized variant management (`PokemonUltimate.Content/Providers/VariantProvider.cs`)
- ✅ Extension methods for easy variant access (`.GetVariants()`, `.HasVariantsAvailable()`)
- ✅ Automatic bidirectional relationship establishment when variants are created
- ✅ 63+ tests passing (functional + edge cases + VariantProvider tests + registry tests)

**Related Documents**:
- **[Parent Architecture](../architecture.md#114-variants-system)** - Feature 1 architecture
- **[Parent Roadmap](../roadmap.md)** - Implementation timeline
- **[Parent Use Cases](../use_cases.md)** - Variant scenarios

---

**Last Updated**: 2025-01-XX

