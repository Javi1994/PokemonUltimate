# Sub-Feature 1.18: Variants System

> Mega Evolutions, Dinamax, and Terracristalización as separate Pokemon species.

**Sub-Feature Number**: 1.18  
**Parent Feature**: [Feature 1: Game Data](../README.md)  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## Overview

Instead of temporary transformations during battle, **Mega Evolutions**, **Dinamax**, and **Terracristalización** are implemented as **separate Pokemon species** with their own `PokemonSpeciesData` entries.

**Key Characteristics**:
- Each variant is a distinct `PokemonSpeciesData` entry
- Variants have different base stats, types, and abilities
- Variants are linked to base form via `BaseForm` field
- Variants can be obtained/encountered separately

## Current Status

- ✅ **Complete**: Variant system fields (BaseForm, VariantType, TeraType, RegionalForm, Variants)
- ✅ **Complete**: PokemonVariantType enum (Mega, Dinamax, Tera, Regional, Cosmetic)
- ✅ **Complete**: Variant builder methods (AsMegaVariant, AsDinamaxVariant, AsTeraVariant, AsRegionalVariant, AsCosmeticVariant)
- ✅ **Complete**: Variant registry query methods (GetVariantsOf, GetMegaVariants, etc.)
- ✅ **Complete**: VariantProvider for centralized variant management
- ✅ **Complete**: Extension methods for easy variant access (.GetVariants(), .HasVariantsAvailable())
- ✅ **Complete**: Computed properties on PokemonSpeciesData (HasVariants, MegaVariants, etc.)
- ✅ **Complete**: Variant relationship validation and tests (63+ tests passing)

## Variant Types

- **Mega Evolutions**: Permanent stat changes, type changes, ability changes
- **Dinamax**: Massive HP increase, special moves
- **Terracristalización**: Type change, stat boost
- **Regional Forms**: Can have different types, stats, abilities, or be purely visual (Alola, Galar, Hisui, etc.)

## Robustness Features

The system is designed to handle all types of variants robustly:

1. **Variants with Gameplay Changes**:
   - Mega Evolutions (stat/type/ability changes)
   - Dinamax (HP increase)
   - Terracristalización (type changes)
   - Regional forms with different stats/types/abilities

2. **Purely Visual Variants**:
   - Regional forms with same stats/types/abilities (cosmetic only)
   - Automatically detected via `HasGameplayChanges` property

3. **Query Capabilities**:
   - Get all variants of a base form (via `VariantProvider` or `pokemon.Variants`)
   - Get variants by type (Mega, Dinamax, Tera, Regional, Cosmetic)
   - Get regional variants by region (Alola, Galar, etc.)
   - Get variants with gameplay changes vs visual-only variants
   - Extension methods for easy access: `pokemon.GetVariants()`, `pokemon.HasVariantsAvailable()`

4. **VariantProvider**:
   - Centralized provider for variant management (`VariantProvider`)
   - Automatic variant registration and relationship establishment
   - Easy querying: `VariantProvider.GetVariants(pokemon)`, `VariantProvider.GetMegaVariants(pokemon)`
   - Follows same pattern as `PokedexDataProvider` and `LearnsetProvider`

## Related Sub-Features

- **[1.1: Pokemon Data](../1.1-pokemon-data/)** - Variants are PokemonSpeciesData entries

## Documentation

- **[Architecture](architecture.md)** - Complete specification of variants system
- **[Parent Architecture](../architecture.md#114-variants-system)** - Technical specification
- **[Parent Code Location](../code_location.md#118-variants-system-complete)** - Code organization

## VariantProvider

The `VariantProvider` (`PokemonUltimate.Content/Providers/VariantProvider.cs`) provides centralized variant management:

- **Centralized Creation**: All variants are created and registered in `VariantProvider`
- **Easy Querying**: `VariantProvider.GetVariants(pokemon)` or `pokemon.GetVariants()`
- **Automatic Registration**: Variants are automatically added to base form's `Variants` list
- **Extension Methods**: Convenient methods like `pokemon.HasVariantsAvailable()`

**Usage**:
```csharp
// Get all variants for a Pokemon
var variants = VariantProvider.GetVariants(PokemonCatalog.Charizard);
// Or using extension method
var variants = PokemonCatalog.Charizard.GetVariants();

// Check if Pokemon has variants
if (VariantProvider.HasVariants(PokemonCatalog.Charizard)) { ... }
// Or using extension method
if (PokemonCatalog.Charizard.HasVariantsAvailable()) { ... }
```

See [Architecture](architecture.md#4-variantprovider) for complete details.

---

**Last Updated**: 2025-01-XX

