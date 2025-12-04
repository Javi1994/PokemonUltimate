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

## 2. Required Fields

### 2.1 PokemonSpeciesData Extensions

```csharp
public class PokemonSpeciesData : IIdentifiable {
    // ... existing fields ...
    
    // Variant fields (NEW)
    public PokemonSpeciesData BaseForm { get; set; }  // Reference to base Pokemon (null if base form)
    public PokemonVariantType? VariantType { get; set; }  // Mega, Dinamax, Tera (null if base form)
    public PokemonType? TeraType { get; set; }  // Tera type for Terracristalización variants
    public List<PokemonSpeciesData> Variants { get; set; }  // List of all variant forms
    
    // Computed properties
    public bool IsVariant => BaseForm != null;
    public bool IsBaseForm => BaseForm == null;
    public bool IsMegaVariant => VariantType == PokemonVariantType.Mega;
    public bool IsDinamaxVariant => VariantType == PokemonVariantType.Dinamax;
    public bool IsTeraVariant => VariantType == PokemonVariantType.Tera;
}
```

### 2.2 PokemonVariantType Enum

```csharp
public enum PokemonVariantType {
    Mega,
    Dinamax,
    Tera
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
```

---

## 4. Learnset Inheritance

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

## 5. Validation Rules

1. **Base Form Validation**: BaseForm must be non-null for variants
2. **Variant Type Validation**: VariantType must match variant type (Mega/Dinamax/Tera)
3. **Tera Type Validation**: TeraType must be set for Tera variants
4. **Name Validation**: Variant names must follow naming conventions
5. **BST Validation**: Mega variants typically have higher BST than base form
6. **HP Validation**: Dinamax variants have 2x HP

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
```

---

## Status

⏳ **PLANNED** - This sub-feature is planned but not yet implemented.

**Related Documents**:
- **[Parent Architecture](../architecture.md#114-variants-system)** - Feature 1 architecture
- **[Parent Roadmap](../roadmap.md)** - Implementation timeline
- **[Parent Use Cases](../use_cases.md)** - Variant scenarios

---

**Last Updated**: 2025-01-XX

