# Feature Naming in Code

> **MANDATORY: Guidelines for reflecting standardized feature nomenclature in source code.**

**⚠️ PRIORITY: HIGH** - This is a mandatory practice for all code in the project.

**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document defines how to reference features and sub-features in source code to maintain consistency with the documentation structure. **This practice is mandatory** - all public classes and key internal classes MUST include feature references in their XML documentation comments.

**⚠️ CRITICAL RULES**:
1. **NEVER write code without reading existing code first** - See `project_guidelines.md` Section 5.7
2. **ALL code must reference its feature** - No exceptions for public APIs
3. **Match existing patterns** - Follow the style and structure of existing code

## Approach

We use a **multi-layered approach** to reflect feature nomenclature in code:

1. **XML Documentation Comments** - Primary method (appears in IntelliSense)
2. **Code Regions** - Optional organization for large files
3. **Custom Attributes** - Optional for static analysis (future use)

---

## 1. XML Documentation Comments (Primary Method)

### Format

Add feature references to XML documentation comments using `<remarks>` or `<seealso>` tags:

```csharp
/// <summary>
/// [Class description]
/// </summary>
/// <remarks>
/// **Feature**: [Feature Number]: [Feature Name]
/// **Sub-Feature**: [Sub-Feature Number]: [Sub-Feature Name] (if applicable)
/// **Documentation**: See `docs/features/[N]-[feature-name]/architecture.md`
/// </remarks>
/// <seealso cref="[Related class]"/>
public class ClassName
{
    // ...
}
```

### Examples

#### Feature 1: Pokemon Data

```csharp
namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Blueprint for a Pokemon species (immutable data).
    /// Pokemon can be retrieved by Name (unique string) or PokedexNumber (unique int).
    /// This is the "Species" data - shared by all Pokemon of the same kind.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.1: PokemonSpeciesData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    public class PokemonSpeciesData : IIdentifiable
    {
        // ...
    }
}
```

#### Feature 2: Combat System

```csharp
namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Main controller for battle execution.
    /// Orchestrates the full battle loop, turn execution, and outcome detection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class CombatEngine
    {
        // ...
    }
}
```

#### Feature 3: Content Expansion

```csharp
namespace PokemonUltimate.Content.Catalogs
{
    /// <summary>
    /// Static catalog of all Pokemon species data.
    /// Use partial classes to organize by generation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/architecture.md`
    /// </remarks>
    public static partial class PokemonCatalog
    {
        // ...
    }
}
```

### When to Add Feature References

**MANDATORY - Always add feature references to:**
- ✅ **ALL public classes and interfaces** - No exceptions
- ✅ **ALL public methods** that are part of a feature's public API
- ✅ **ALL key internal classes** that represent core feature functionality
- ✅ **ALL classes** that are part of a feature's main API surface
- ✅ **ALL new code** - Every class you create must have feature reference
- ✅ **ALL modified code** - When modifying existing classes, ensure feature reference exists

**Recommended (but not mandatory) for:**
- Internal helper classes (when modified or created)
- Private classes (when part of feature's core functionality)
- Extension methods (when they extend feature APIs)

**⚠️ Rule**: If you write code, it MUST have a feature reference. No code is exempt from this requirement.

---

## 2. Code Regions (Optional Organization)

For large files that span multiple features or sub-features, use regions to organize:

```csharp
namespace PokemonUltimate.Combat.Actions
{
    #region Feature 2.5: Combat Actions - Base Classes
    
    /// <summary>
    /// Base class for all battle actions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public abstract class BattleAction
    {
        // ...
    }
    
    #endregion
    
    #region Feature 2.5: Combat Actions - Damage Actions
    
    /// <summary>
    /// Action representing damage dealt to a Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class DamageAction : BattleAction
    {
        // ...
    }
    
    #endregion
}
```

**Use regions sparingly** - only when a file is large (>500 lines) and contains multiple distinct feature areas.

---

## 3. Custom Attributes (Future Use - Optional)

For static analysis or documentation generation, we can create custom attributes:

```csharp
namespace PokemonUltimate.Core.Attributes
{
    /// <summary>
    /// Marks a class, method, or namespace as belonging to a specific feature.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Namespace)]
    public class FeatureAttribute : Attribute
    {
        public int FeatureNumber { get; }
        public string FeatureName { get; }
        public int? SubFeatureNumber { get; }
        public string SubFeatureName { get; }
        public string DocumentationPath { get; }
        
        public FeatureAttribute(
            int featureNumber,
            string featureName,
            string documentationPath,
            int? subFeatureNumber = null,
            string subFeatureName = null)
        {
            FeatureNumber = featureNumber;
            FeatureName = featureName;
            SubFeatureNumber = subFeatureNumber;
            SubFeatureName = subFeatureName;
            DocumentationPath = documentationPath;
        }
    }
}
```

**Usage** (optional, for future tooling):

```csharp
[Feature(
    featureNumber: 2,
    featureName: "Combat System",
    documentationPath: "docs/features/2-combat-system/architecture.md",
    subFeatureNumber: 2.6,
    subFeatureName: "Combat Engine")]
public class CombatEngine
{
    // ...
}
```

**Note**: Attributes are optional and not required. XML documentation comments are sufficient for most use cases.

---

## 4. Namespace Organization

Namespaces already reflect feature organization:

- **Feature 1**: `PokemonUltimate.Core.Blueprints`, `PokemonUltimate.Core.Instances`
- **Feature 2**: `PokemonUltimate.Combat.*`
- **Feature 3**: `PokemonUltimate.Content.*`

**No changes needed** - namespaces already align with features.

---

## 5. File Headers (Optional)

For consistency, you can add a file header comment:

```csharp
// Feature: 2: Combat System
// Sub-Feature: 2.6: Combat Engine
// Documentation: docs/features/2-combat-system/2.6-combat-engine/architecture.md

using System;
// ...
```

**Note**: File headers are optional. XML documentation comments are preferred as they appear in IntelliSense.

---

## Implementation Priority

### Phase 1: XML Documentation Comments (Recommended)
- ✅ Add `<remarks>` sections to all public classes
- ✅ Reference feature numbers and names
- ✅ Link to documentation paths
- **Effort**: Low (adds ~3 lines per class)
- **Benefit**: High (appears in IntelliSense, helps developers)

### Phase 2: Code Regions (Optional)
- ⏳ Add regions to large files (>500 lines)
- ⏳ Group by feature/sub-feature
- **Effort**: Medium (requires file review)
- **Benefit**: Medium (improves readability)

### Phase 3: Custom Attributes (Future)
- ⏳ Create `FeatureAttribute` class
- ⏳ Add attributes to key classes
- ⏳ Build tooling to extract feature information
- **Effort**: High (requires tooling)
- **Benefit**: Low-Medium (enables static analysis)

---

## Examples by Feature

### Feature 1: Pokemon Data

```csharp
/// <summary>
/// Blueprint for a Pokemon species (immutable data).
/// </summary>
/// <remarks>
/// **Feature**: 1: Pokemon Data
/// **Sub-Feature**: 1.1: PokemonSpeciesData (Blueprint)
/// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
/// </remarks>
public class PokemonSpeciesData : IIdentifiable { }

/// <summary>
/// Mutable runtime instance of a Pokemon.
/// </summary>
/// <remarks>
/// **Feature**: 1: Pokemon Data
/// **Sub-Feature**: 1.2: PokemonInstance (Runtime)
/// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
/// </remarks>
public class PokemonInstance { }
```

### Feature 2: Combat System

```csharp
/// <summary>
/// Main controller for battle execution.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.6: Combat Engine
/// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
/// </remarks>
public class CombatEngine { }

/// <summary>
/// Modular damage calculation pipeline.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.4: Damage Calculation Pipeline
/// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
/// </remarks>
public class DamagePipeline { }
```

### Feature 3: Content Expansion

```csharp
/// <summary>
/// Static catalog of all Pokemon species data.
/// </summary>
/// <remarks>
/// **Feature**: 3: Content Expansion
/// **Sub-Feature**: 3.1: Pokemon Expansion
/// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/architecture.md`
/// </remarks>
public static partial class PokemonCatalog { }

/// <summary>
/// Static catalog of all move data.
/// </summary>
/// <remarks>
/// **Feature**: 3: Content Expansion
/// **Sub-Feature**: 3.2: Move Expansion
/// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
/// </remarks>
public static partial class MoveCatalog { }
```

---

## Related Documents

- **[Features Master List](../../features_master_list.md)** - Feature numbering standards
- **[Feature Documentation Standard](../../feature_documentation_standard.md)** - Documentation structure
- **[Project Guidelines](project_guidelines.md)** - General coding standards

---

**Last Updated**: 2025-01-XX

