# Feature 1: Game Data - Testing

> Comprehensive testing strategy for game data structures (blueprints and instances).

**Feature Number**: 1  
**Feature Name**: Game Data (formerly "Pokemon Data")  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

This document defines how to organize and create tests for **all game data structures** organized by sub-features:

-   **Grupo A: Core Entity Data** (1.1-1.4) - Pokemon, Moves, Abilities, Items
-   **Grupo B: Field & Status Data** (1.5-1.10) - Status Effects, Weather, Terrain, Hazards, Side Conditions, Field Effects
-   **Grupo C: Supporting Systems** (1.11-1.12) - Evolution System, Type Effectiveness
-   **Grupo D: Infrastructure** (1.13-1.14, 1.16-1.17) - Interfaces, Enums, Constants, Factories, Registries (Note: Builders moved to Feature 3.9)
-   **Grupo E: Planned Features** (1.18-1.19) - Variants System, Pokedex Fields

**See**: [Sub-Features Overview](README.md) for complete sub-feature list.

---

## Testing Philosophy

Following the existing test structure (`docs/ai/testing_structure_definition.md`):

1. **Blueprints/** - Tests de estructura (CÓMO son los datos)
2. **Data/** - Tests de contenido (QUÉ contienen los datos)
3. **Systems/** - Tests de comportamiento (CÓMO funcionan las instancias)

---

## Test Organization Structure

**Principle**: Extend existing test files instead of creating new ones with "New" in the name.

**Organized by Sub-Features**:

```
PokemonUltimate.Tests/
│
├── Blueprints/                              # Tests de estructura de datos (Sub-Features 1.1-1.10)
│   │
│   ├── PokemonSpeciesDataTests.cs          # 1.1: Pokemon Data - ✅ EXTEND
│   ├── MoveDataTests.cs                     # 1.2: Move Data
│   ├── AbilityDataTests.cs                  # 1.3: Ability Data
│   ├── ItemDataTests.cs                     # 1.4: Item Data
│   ├── StatusEffectDataTests.cs             # 1.5: Status Effect Data
│   ├── WeatherDataTests.cs                  # 1.6: Weather Data
│   ├── TerrainDataTests.cs                  # 1.7: Terrain Data
│   ├── HazardDataTests.cs                   # 1.8: Hazard Data
│   ├── SideConditionDataTests.cs            # 1.9: Side Condition Data
│   └── FieldEffectDataTests.cs              # 1.10: Field Effect Data
│
├── Data/                                    # Tests de contenido específico
│   │
│   ├── Pokemon/                             # 1.1: Pokemon Data
│   │   ├── PikachuTests.cs                  # ✅ EXTEND - Add new fields validation
│   │   ├── CharizardTests.cs                # ✅ EXTEND - Add new fields validation
│   │   └── [26 Pokemon tests...]            # ✅ EXTEND - Add new fields to each
│   │
│   ├── Moves/                               # 1.2: Move Data
│   │   └── [Move]Tests.cs                   # Individual move tests
│   │
│   ├── Abilities/                          # 1.3: Ability Data
│   │   └── [Ability]Tests.cs                # Individual ability tests
│   │
│   ├── Items/                               # 1.4: Item Data
│   │   └── [Item]Tests.cs                   # Individual item tests
│   │
│   ├── Catalogs/                            # Tests generales de catálogos
│   │   ├── PokemonCatalogTests.cs           # 1.1: Pokemon Data
│   │   ├── PokemonCatalogValidationTests.cs # 1.1: Pokemon Data - ✅ EXTEND
│   │   ├── MoveCatalogTests.cs              # 1.2: Move Data
│   │   ├── AbilityCatalogTests.cs           # 1.3: Ability Data
│   │   └── ItemCatalogTests.cs              # 1.4: Item Data
│   │
│   ├── Builders/                            # Tests de builders (Moved to Feature 3.9)
│   │   ├── PokemonBuilderTests.cs           # 1.1 + 1.15: ✅ EXTEND
│   │   ├── MoveBuilderTests.cs              # 1.2 + 1.15
│   │   ├── AbilityBuilderTests.cs           # 1.3 + 1.15
│   │   ├── ItemBuilderTests.cs              # 1.4 + 1.15
│   │   └── BuilderEdgeCasesTests.cs         # Moved to Feature 3.9: Builders
│   │
│   ├── Enums/                               # 1.14: Enums & Constants
│   │   └── [Enum]Tests.cs                   # Enum validation tests
│   │
│   └── Constants/                           # 1.14: Enums & Constants
│       └── [Constants]Tests.cs              # Constants validation tests
│
└── Systems/                                 # Tests de comportamiento
    │
    └── Core/
        │
        ├── Instances/                        # 1.1-1.2: Instance tests
        │   ├── PokemonInstanceTests.cs      # 1.1: ✅ EXTEND
        │   └── MoveInstanceTests.cs         # 1.2: Move instances
        │
        ├── Evolution/                       # 1.11: Evolution System
        │   ├── EvolutionTests.cs
        │   ├── LevelConditionTests.cs
        │   ├── ItemConditionTests.cs
        │   └── [Other condition tests...]
        │
        ├── Effects/                         # 1.2: Move Effects
        │   ├── DamageEffectTests.cs
        │   ├── StatusEffectTests.cs
        │   └── [Other effect tests...]
        │
        ├── Factories/                       # 1.16: Factories & Calculators
        │   ├── StatCalculatorTests.cs        # 1.16: Stat calculation
        │   ├── PokemonFactoryTests.cs        # 1.1 + 1.16: ✅ EXTEND
        │   ├── TypeEffectivenessTests.cs     # 1.12: Type Effectiveness
        │
        └── Blueprints/                      # 1.14: Enums & Constants (NatureData)
            └── NatureDataTests.cs            # 1.14: Nature modifier tables
        │
        └── Registry/                        # 1.17: Registry System
            ├── GameDataRegistryTests.cs      # 1.17: Generic registry
            ├── PokemonRegistryTests.cs        # 1.1 + 1.17: Pokemon registry
            └── MoveRegistryTests.cs          # 1.2 + 1.17: Move registry
```

---

## Test Categories by Priority

### Phase 1: Critical Fields Tests (HIGH Priority)

**Fields**: `BaseExperienceYield`, `CatchRate`, `BaseFriendship`, `GrowthRate`

#### 1.1 Blueprint Structure Tests

**File**: `Blueprints/PokemonSpeciesDataTests.cs` (EXTEND)

**Purpose**: Add tests for new fields to existing file

**Location**: Add new region `#region Gameplay Fields Tests` before `#region Helpers`

**Tests**:

```csharp
[TestFixture]
public class PokemonSpeciesDataEdgeCasesTests
{
    #region BaseExperienceYield Tests

    [Test]
    public void BaseExperienceYield_Default_IsZero()
    {
        var pokemon = new PokemonSpeciesData();
        Assert.That(pokemon.BaseExperienceYield, Is.EqualTo(0));
    }

    [Test]
    [TestCase(1)]      // Minimum
    [TestCase(255)]    // Typical max
    [TestCase(390)]    // Legendary (Mewtwo)
    public void BaseExperienceYield_Accepts_ValidRange(int expYield)
    {
        var pokemon = new PokemonSpeciesData { BaseExperienceYield = expYield };
        Assert.That(pokemon.BaseExperienceYield, Is.EqualTo(expYield));
    }

    [Test]
    public void BaseExperienceYield_Negative_ThrowsException()
    {
        var pokemon = new PokemonSpeciesData();
        Assert.Throws<ArgumentException>(() =>
            pokemon.BaseExperienceYield = -1);
    }

    #endregion

    #region CatchRate Tests

    [Test]
    public void CatchRate_Default_IsZero()
    {
        var pokemon = new PokemonSpeciesData();
        Assert.That(pokemon.CatchRate, Is.EqualTo(0));
    }

    [Test]
    [TestCase(3)]      // Legendary (Mewtwo)
    [TestCase(45)]     // Common (Pikachu)
    [TestCase(255)]    // Maximum (Caterpie)
    public void CatchRate_Accepts_ValidRange(int catchRate)
    {
        var pokemon = new PokemonSpeciesData { CatchRate = catchRate };
        Assert.That(pokemon.CatchRate, Is.EqualTo(catchRate));
    }

    #endregion

    #region BaseFriendship Tests

    [Test]
    public void BaseFriendship_Default_IsSeventy()
    {
        var pokemon = new PokemonSpeciesData();
        Assert.That(pokemon.BaseFriendship, Is.EqualTo(70));
    }

    [Test]
    [TestCase(0)]      // Some legendaries
    [TestCase(70)]    // Wild Pokemon (default)
    [TestCase(120)]   // Starters/Hatched
    [TestCase(255)]   // Maximum
    public void BaseFriendship_Accepts_ValidRange(int friendship)
    {
        var pokemon = new PokemonSpeciesData { BaseFriendship = friendship };
        Assert.That(pokemon.BaseFriendship, Is.EqualTo(friendship));
    }

    [Test]
    public void BaseFriendship_OutOfRange_ThrowsException()
    {
        var pokemon = new PokemonSpeciesData();
        Assert.Throws<ArgumentException>(() =>
            pokemon.BaseFriendship = 256);
    }

    #endregion

    #region GrowthRate Tests

    [Test]
    public void GrowthRate_Default_IsMediumFast()
    {
        var pokemon = new PokemonSpeciesData();
        Assert.That(pokemon.GrowthRate, Is.EqualTo(GrowthRate.MediumFast));
    }

    [Test]
    [TestCase(GrowthRate.Fast)]
    [TestCase(GrowthRate.MediumFast)]
    [TestCase(GrowthRate.MediumSlow)]
    [TestCase(GrowthRate.Slow)]
    [TestCase(GrowthRate.Erratic)]
    [TestCase(GrowthRate.Fluctuating)]
    public void GrowthRate_Accepts_AllValues(GrowthRate growthRate)
    {
        var pokemon = new PokemonSpeciesData { GrowthRate = growthRate };
        Assert.That(pokemon.GrowthRate, Is.EqualTo(growthRate));
    }

    #endregion
}
```

#### 1.2 Builder Tests

**File**: `Data/Builders/PokemonBuilderTests.cs` (EXTEND)

**Purpose**: Add builder method tests to existing file

**Location**: Add new region `#region Gameplay Fields Builder Tests` at the end

**Tests**:

```csharp
[TestFixture]
public class PokemonBuilderNewFieldsTests
{
    [Test]
    public void BaseExperienceYield_Sets_Correctly()
    {
        var pokemon = Pokemon.Define("Pikachu", 25)
            .BaseExperienceYield(112)
            .Build();

        Assert.That(pokemon.BaseExperienceYield, Is.EqualTo(112));
    }

    [Test]
    public void CatchRate_Sets_Correctly()
    {
        var pokemon = Pokemon.Define("Pikachu", 25)
            .CatchRate(190)
            .Build();

        Assert.That(pokemon.CatchRate, Is.EqualTo(190));
    }

    [Test]
    public void BaseFriendship_Sets_Correctly()
    {
        var pokemon = Pokemon.Define("Pikachu", 25)
            .BaseFriendship(70)
            .Build();

        Assert.That(pokemon.BaseFriendship, Is.EqualTo(70));
    }

    [Test]
    public void GrowthRate_Sets_Correctly()
    {
        var pokemon = Pokemon.Define("Pikachu", 25)
            .GrowthRate(GrowthRate.MediumFast)
            .Build();

        Assert.That(pokemon.GrowthRate, Is.EqualTo(GrowthRate.MediumFast));
    }

    [Test]
    public void Builder_Chains_AllNewFields()
    {
        var pokemon = Pokemon.Define("Pikachu", 25)
            .BaseExperienceYield(112)
            .CatchRate(190)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumFast)
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(pokemon.BaseExperienceYield, Is.EqualTo(112));
            Assert.That(pokemon.CatchRate, Is.EqualTo(190));
            Assert.That(pokemon.BaseFriendship, Is.EqualTo(70));
            Assert.That(pokemon.GrowthRate, Is.EqualTo(GrowthRate.MediumFast));
        });
    }
}
```

#### 1.3 Catalog Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs` (EXTEND)

**Purpose**: Add gameplay fields validation to existing validation file

**Location**: Add new region `#region Gameplay Fields Validation` at the end

**Tests**:

```csharp
[TestFixture]
public class PokemonCatalogGameplayFieldsTests
{
    [Test]
    public void AllPokemon_HaveBaseExperienceYield()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.BaseExperienceYield, Is.GreaterThan(0),
                $"{pokemon.Name} should have BaseExperienceYield > 0");
        }
    }

    [Test]
    public void AllPokemon_HaveCatchRate()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.CatchRate, Is.InRange(3, 255),
                $"{pokemon.Name} should have CatchRate between 3 and 255");
        }
    }

    [Test]
    public void AllPokemon_HaveBaseFriendship()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.BaseFriendship, Is.InRange(0, 255),
                $"{pokemon.Name} should have BaseFriendship between 0 and 255");
        }
    }

    [Test]
    public void AllPokemon_HaveGrowthRate()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(Enum.IsDefined(typeof(GrowthRate), pokemon.GrowthRate),
                $"{pokemon.Name} should have valid GrowthRate");
        }
    }

    [Test]
    [Description("Pikachu BaseExperienceYield should be 112")]
    public void Pikachu_BaseExperienceYield_Is112()
    {
        Assert.That(PokemonCatalog.Pikachu.BaseExperienceYield, Is.EqualTo(112));
    }

    [Test]
    [Description("Mewtwo BaseExperienceYield should be 306")]
    public void Mewtwo_BaseExperienceYield_Is306()
    {
        Assert.That(PokemonCatalog.Mewtwo.BaseExperienceYield, Is.EqualTo(306));
    }

    [Test]
    [Description("Pikachu CatchRate should be 190")]
    public void Pikachu_CatchRate_Is190()
    {
        Assert.That(PokemonCatalog.Pikachu.CatchRate, Is.EqualTo(190));
    }

    [Test]
    [Description("Mewtwo CatchRate should be 3")]
    public void Mewtwo_CatchRate_Is3()
    {
        Assert.That(PokemonCatalog.Mewtwo.CatchRate, Is.EqualTo(3));
    }

    [Test]
    [Description("Starters should have BaseFriendship 120")]
    public void Starters_HaveBaseFriendship120()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PokemonCatalog.Bulbasaur.BaseFriendship, Is.EqualTo(120));
            Assert.That(PokemonCatalog.Charmander.BaseFriendship, Is.EqualTo(120));
            Assert.That(PokemonCatalog.Squirtle.BaseFriendship, Is.EqualTo(120));
        });
    }

    [Test]
    [Description("Mewtwo should have BaseFriendship 0")]
    public void Mewtwo_HasBaseFriendship0()
    {
        Assert.That(PokemonCatalog.Mewtwo.BaseFriendship, Is.EqualTo(0));
    }
}
```

#### 1.4 Individual Pokemon Tests (Extend Existing)

**File**: `Data/Pokemon/PikachuTests.cs` (EXTEND)

**Purpose**: Add gameplay fields validation to existing Pokemon test files

**Location**: Add new test method to existing file

**Tests**:

```csharp
// Add to existing PikachuTests.cs
[Test]
public void Pikachu_HasAllGameplayFields()
{
    var pikachu = PokemonCatalog.Pikachu;

    Assert.Multiple(() =>
    {
        Assert.That(pikachu.BaseExperienceYield, Is.EqualTo(112), "BaseExperienceYield");
        Assert.That(pikachu.CatchRate, Is.EqualTo(190), "CatchRate");
        Assert.That(pikachu.BaseFriendship, Is.EqualTo(70), "BaseFriendship");
        Assert.That(pikachu.GrowthRate, Is.EqualTo(GrowthRate.MediumFast), "GrowthRate");
    });
}
```

**Note**: Add similar tests to ALL existing `[Pokemon]Tests.cs` files (26 files).

---

### Phase 2: Pokedex Fields Tests (MEDIUM Priority)

**Fields**: `Description`, `Category`, `Height`, `Weight`, `Color`, `Shape`, `Habitat`

#### 2.1 Blueprint Structure Tests

**File**: `Blueprints/PokemonSpeciesDataTests.cs` (EXTEND)

**Location**: Add new region `#region Pokedex Fields Tests` before `#region Helpers`

**Tests**:

```csharp
#region Pokedex Fields Tests

[Test]
public void Description_Default_IsEmpty()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Description, Is.EqualTo(string.Empty));
}

[Test]
public void Category_Default_IsEmpty()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Category, Is.EqualTo(string.Empty));
}

[Test]
public void Height_Default_IsZero()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Height, Is.EqualTo(0f));
}

[Test]
public void Weight_Default_IsZero()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Weight, Is.EqualTo(0f));
}

[Test]
public void Color_Default_IsUnknown()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Color, Is.EqualTo(PokemonColor.Unknown));
}

[Test]
public void Shape_Default_IsUnknown()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Shape, Is.EqualTo(PokemonShape.Unknown));
}

[Test]
public void Habitat_Default_IsUnknown()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Habitat, Is.EqualTo(PokemonHabitat.Unknown));
}

[Test]
[TestCase(0.1f)]   // Smallest Pokemon
[TestCase(1.7f)]   // Charizard
[TestCase(20.0f)]  // Large Pokemon
public void Height_Accepts_ValidRange(float height)
{
    var pokemon = new PokemonSpeciesData { Height = height };
    Assert.That(pokemon.Height, Is.EqualTo(height));
}

[Test]
[TestCase(0.1f)]   // Smallest Pokemon
[TestCase(90.5f)]  // Charizard
[TestCase(1000f)] // Heavy Pokemon
public void Weight_Accepts_ValidRange(float weight)
{
    var pokemon = new PokemonSpeciesData { Weight = weight };
    Assert.That(pokemon.Weight, Is.EqualTo(weight));
}

[Test]
[TestCase(PokemonColor.Red)]
[TestCase(PokemonColor.Yellow)]
[TestCase(PokemonColor.Blue)]
public void Color_Accepts_AllValues(PokemonColor color)
{
    var pokemon = new PokemonSpeciesData { Color = color };
    Assert.That(pokemon.Color, Is.EqualTo(color));
}

#endregion
```

#### 2.2 Builder Tests

**File**: `Data/Builders/PokemonBuilderTests.cs` (EXTEND)

**Location**: Add new region `#region Pokedex Fields Builder Tests` at the end

**Tests**:

```csharp
[Test]
public void PokedexFields_Sets_Correctly()
{
    var pokemon = Pokemon.Define("Charizard", 6)
        .Description("It can melt boulders. It causes forest fires by blowing flames.")
        .Category("Flame Pokemon")
        .Height(1.7f)
        .Weight(90.5f)
        .Color(PokemonColor.Red)
        .Shape(PokemonShape.Upright)
        .Habitat(PokemonHabitat.Mountain)
        .Build();

    Assert.Multiple(() =>
    {
        Assert.That(pokemon.Description, Is.Not.Empty);
        Assert.That(pokemon.Category, Is.EqualTo("Flame Pokemon"));
        Assert.That(pokemon.Height, Is.EqualTo(1.7f));
        Assert.That(pokemon.Weight, Is.EqualTo(90.5f));
        Assert.That(pokemon.Color, Is.EqualTo(PokemonColor.Red));
        Assert.That(pokemon.Shape, Is.EqualTo(PokemonShape.Upright));
        Assert.That(pokemon.Habitat, Is.EqualTo(PokemonHabitat.Mountain));
    });
}
```

#### 2.3 Catalog Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs` (EXTEND)

**Location**: Add new region `#region Pokedex Fields Validation` at the end

**Tests**:

```csharp
[TestFixture]
public class PokemonCatalogPokedexFieldsTests
{
    [Test]
    public void AllPokemon_HaveDescription()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Description, Is.Not.Empty,
                $"{pokemon.Name} should have Description");
        }
    }

    [Test]
    public void AllPokemon_HaveCategory()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Category, Is.Not.Empty,
                $"{pokemon.Name} should have Category");
        }
    }

    [Test]
    public void AllPokemon_HaveHeight()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Height, Is.GreaterThan(0f),
                $"{pokemon.Name} should have Height > 0");
        }
    }

    [Test]
    public void AllPokemon_HaveWeight()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Weight, Is.GreaterThan(0f),
                $"{pokemon.Name} should have Weight > 0");
        }
    }

    [Test]
    public void AllPokemon_HaveColor()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Color, Is.Not.EqualTo(PokemonColor.Unknown),
                $"{pokemon.Name} should have Color set");
        }
    }

    [Test]
    public void AllPokemon_HaveShape()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Shape, Is.Not.EqualTo(PokemonShape.Unknown),
                $"{pokemon.Name} should have Shape set");
        }
    }

    [Test]
    public void AllPokemon_HaveHabitat()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            Assert.That(pokemon.Habitat, Is.Not.EqualTo(PokemonHabitat.Unknown),
                $"{pokemon.Name} should have Habitat set");
        }
    }

    [Test]
    [Description("Charizard Height should be 1.7m")]
    public void Charizard_Height_Is1_7()
    {
        Assert.That(PokemonCatalog.Charizard.Height, Is.EqualTo(1.7f));
    }

    [Test]
    [Description("Charizard Weight should be 90.5kg")]
    public void Charizard_Weight_Is90_5()
    {
        Assert.That(PokemonCatalog.Charizard.Weight, Is.EqualTo(90.5f));
    }
}
```

---

### Phase 3: Variants System Tests (MEDIUM Priority)

**Fields**: `BaseForm`, `VariantType`, `TeraType`, `Variants`

#### 3.1 Blueprint Structure Tests

**File**: `Blueprints/PokemonSpeciesDataTests.cs` (EXTEND)

**Location**: Add new region `#region Variants System Tests` before `#region Helpers`

**Tests**:

```csharp
#region Variants System Tests

[Test]
public void BaseForm_Default_IsNull()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.BaseForm, Is.Null);
}

[Test]
public void VariantType_Default_IsNone()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.VariantType, Is.EqualTo(PokemonVariantType.None));
}

[Test]
public void TeraType_Default_IsNull()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.TeraType, Is.Null);
}

[Test]
public void Variants_Default_IsEmpty()
{
    var pokemon = new PokemonSpeciesData();
    Assert.That(pokemon.Variants, Is.Not.Null.And.Empty);
}

[Test]
public void IsVariant_ReturnsTrue_WhenVariantTypeIsNotNone()
{
    var baseForm = new PokemonSpeciesData { Name = "Charizard" };
    var mega = new PokemonSpeciesData
    {
        Name = "Mega Charizard X",
        BaseForm = baseForm,
        VariantType = PokemonVariantType.Mega
    };

    Assert.That(mega.IsVariant, Is.True);
}

[Test]
public void IsVariant_ReturnsFalse_WhenBaseForm()
{
    var pokemon = new PokemonSpeciesData { Name = "Charizard" };
    Assert.That(pokemon.IsVariant, Is.False);
}

#endregion
```

#### 3.2 Builder Tests

**File**: `Data/Builders/PokemonBuilderTests.cs` (EXTEND)

**Location**: Add new region `#region Variants Builder Tests` at the end

**Tests**:

```csharp
[TestFixture]
public class PokemonBuilderVariantsTests
{
    [Test]
    public void AsMegaVariant_Sets_Correctly()
    {
        var baseForm = Pokemon.Define("Charizard", 6).Build();
        var mega = Pokemon.Define("Mega Charizard X", 6)
            .AsMegaVariant(baseForm, "X")
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(mega.BaseForm, Is.EqualTo(baseForm));
            Assert.That(mega.VariantType, Is.EqualTo(PokemonVariantType.Mega));
        });
    }

    [Test]
    public void AsDinamaxVariant_Sets_Correctly()
    {
        var baseForm = Pokemon.Define("Charizard", 6).Build();
        var dinamax = Pokemon.Define("Charizard Dinamax", 6)
            .AsDinamaxVariant(baseForm)
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(dinamax.BaseForm, Is.EqualTo(baseForm));
            Assert.That(dinamax.VariantType, Is.EqualTo(PokemonVariantType.Dinamax));
        });
    }

    [Test]
    public void AsGigantamaxVariant_Sets_Correctly()
    {
        var baseForm = Pokemon.Define("Charizard", 6).Build();
        var gmax = Pokemon.Define("Charizard Gigantamax", 6)
            .AsDinamaxVariant(baseForm, isGigantamax: true)
            .Build();

        Assert.That(gmax.VariantType, Is.EqualTo(PokemonVariantType.Gigantamax));
    }

    [Test]
    public void AsTeraVariant_Sets_Correctly()
    {
        var baseForm = Pokemon.Define("Charizard", 6).Build();
        var tera = Pokemon.Define("Charizard Tera Fire", 6)
            .AsTeraVariant(baseForm, PokemonType.Fire)
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(tera.BaseForm, Is.EqualTo(baseForm));
            Assert.That(tera.VariantType, Is.EqualTo(PokemonVariantType.Tera));
            Assert.That(tera.TeraType, Is.EqualTo(PokemonType.Fire));
        });
    }
}
```

#### 3.3 Catalog Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs` (EXTEND)

**Location**: Add new region `#region Variants System Validation` at the end

**Tests**:

```csharp
[TestFixture]
public class PokemonCatalogVariantsTests
{
    [Test]
    public void AllVariants_HaveBaseForm()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            if (pokemon.IsVariant)
            {
                Assert.That(pokemon.BaseForm, Is.Not.Null,
                    $"{pokemon.Name} is a variant but has no BaseForm");
            }
        }
    }

    [Test]
    public void AllVariants_HaveVariantType()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            if (pokemon.IsVariant)
            {
                Assert.That(pokemon.VariantType, Is.Not.EqualTo(PokemonVariantType.None),
                    $"{pokemon.Name} is a variant but VariantType is None");
            }
        }
    }

    [Test]
    public void TeraVariants_HaveTeraType()
    {
        foreach (var pokemon in PokemonCatalog.All)
        {
            if (pokemon.VariantType == PokemonVariantType.Tera)
            {
                Assert.That(pokemon.TeraType, Is.Not.Null,
                    $"{pokemon.Name} is Tera variant but has no TeraType");
            }
        }
    }

    [Test]
    public void BaseForms_HaveVariantsList()
    {
        // Test that base forms have their variants listed
        // (when variants are added to catalog)
    }
}
```

---

### Phase 4: Instance Tests (HIGH Priority)

**Purpose**: Verify PokemonInstance uses new fields correctly

#### 4.1 Instance Field Usage Tests

**File**: `Systems/Core/Instances/PokemonInstanceTests.cs` (EXTEND)

**Location**: Add new region `#region Species Fields Usage Tests` at the end

**Tests**:

```csharp
[TestFixture]
public class PokemonInstanceNewFieldsTests
{
    [Test]
    public void Instance_UsesSpeciesBaseFriendship()
    {
        var species = new PokemonSpeciesData
        {
            Name = "TestMon",
            BaseFriendship = 120
        };

        var instance = PokemonFactory.CreateInstance(species, level: 5);

        Assert.That(instance.Friendship, Is.EqualTo(120));
    }

    [Test]
    public void Instance_UsesSpeciesGrowthRate()
    {
        var species = new PokemonSpeciesData
        {
            Name = "TestMon",
            GrowthRate = GrowthRate.Slow
        };

        var instance = PokemonFactory.CreateInstance(species, level: 5);

        // Verify EXP calculation uses correct growth rate
        // (when EXP system is implemented)
    }
}
```

---

## Implementation Strategy

### Step 1: Extend Blueprint Tests

**File**: `Blueprints/PokemonSpeciesDataTests.cs`

1. Add new region `#region Gameplay Fields Tests` before `#region Helpers`

    - Default values tests
    - Valid range tests
    - Edge cases

2. Add new region `#region Pokedex Fields Tests` before `#region Helpers`

    - Default values tests
    - Valid range tests

3. Add new region `#region Variants System Tests` before `#region Helpers`
    - Default values tests
    - Variant relationships tests

### Step 2: Extend Builder Tests

**File**: `Data/Builders/PokemonBuilderTests.cs`

1. Add new region `#region Gameplay Fields Builder Tests` at the end

    - Builder methods for EXP, Catch, Friendship, GrowthRate
    - Chaining tests

2. Add new region `#region Pokedex Fields Builder Tests` at the end

    - Builder methods for Description, Category, Height, Weight, Color, Shape, Habitat

3. Add new region `#region Variants Builder Tests` at the end
    - Builder methods for AsMegaVariant, AsDinamaxVariant, AsTeraVariant

### Step 3: Extend Catalog Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

1. Add new region `#region Gameplay Fields Validation` at the end

    - All Pokemon have BaseExperienceYield > 0
    - All Pokemon have CatchRate in valid range
    - All Pokemon have BaseFriendship in valid range
    - All Pokemon have valid GrowthRate
    - Specific Pokemon validation (Pikachu EXP = 112, etc.)

2. Add new region `#region Pokedex Fields Validation` at the end

    - All Pokemon have Description
    - All Pokemon have Category
    - All Pokemon have Height > 0
    - All Pokemon have Weight > 0
    - All Pokemon have Color set
    - All Pokemon have Shape set
    - All Pokemon have Habitat set

3. Add new region `#region Variants System Validation` at the end
    - All variants have BaseForm
    - All variants have VariantType
    - Tera variants have TeraType

### Step 4: Extend Individual Pokemon Tests

**Files**: `Data/Pokemon/[Pokemon]Tests.cs` (all 26 files)

Add new test method to each existing file:

```csharp
[Test]
public void [Pokemon]_HasAllGameplayFields()
{
    // Validate EXP, Catch, Friendship, GrowthRate
}

[Test]
public void [Pokemon]_HasAllPokedexFields()
{
    // Validate Description, Category, Height, Weight, Color, Shape, Habitat
}
```

**Note**: Update all 26 existing Pokemon test files.

### Step 5: Extend Instance Tests

**File**: `Systems/Core/Instances/PokemonInstanceTests.cs`

Add new region `#region Species Fields Usage Tests` at the end:

-   Instance uses Species.BaseFriendship
-   Instance uses Species.GrowthRate (when EXP system implemented)
-   Field propagation from species to instance

---

## Test Naming Conventions

Follow existing patterns:

-   **Functional Tests**: `MethodName_Scenario_ExpectedResult`
-   **Edge Cases**: `FieldName_EdgeCase_ExpectedResult`
-   **Validation**: `AllPokemon_HaveFieldName` or `PokemonName_FieldName_IsValue`

---

## Checklist for Implementation

### Phase 1: Critical Fields

-   [ ] **Extend** `Blueprints/PokemonSpeciesDataTests.cs` - Add Gameplay Fields region
-   [ ] **Extend** `Data/Builders/PokemonBuilderTests.cs` - Add Gameplay Fields Builder region
-   [ ] **Extend** `Data/Catalogs/PokemonCatalogValidationTests.cs` - Add Gameplay Fields Validation region
-   [ ] **Extend** `Data/Pokemon/[Pokemon]Tests.cs` (all 26 files) - Add HasAllGameplayFields test

### Phase 2: Pokedex Fields

-   [ ] **Extend** `Blueprints/PokemonSpeciesDataTests.cs` - Add Pokedex Fields region
-   [ ] **Extend** `Data/Builders/PokemonBuilderTests.cs` - Add Pokedex Fields Builder region
-   [ ] **Extend** `Data/Catalogs/PokemonCatalogValidationTests.cs` - Add Pokedex Fields Validation region
-   [ ] **Extend** `Data/Pokemon/[Pokemon]Tests.cs` (all 26 files) - Add HasAllPokedexFields test

### Phase 3: Variants System

-   [ ] **Extend** `Blueprints/PokemonSpeciesDataTests.cs` - Add Variants System region
-   [ ] **Extend** `Data/Builders/PokemonBuilderTests.cs` - Add Variants Builder region
-   [ ] **Extend** `Data/Catalogs/PokemonCatalogValidationTests.cs` - Add Variants Validation region

### Phase 4: Instance Tests

-   [ ] **Extend** `Systems/Core/Instances/PokemonInstanceTests.cs` - Add Species Fields Usage region

---

## Additional Tests for Comprehensive Coverage

Beyond the basic field tests, here are additional tests to ensure data integrity and consistency:

### 1. Learnset Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

**Add region**: `#region Learnset Validation`

**Tests**:

```csharp
[Test]
public void AllPokemon_HaveAtLeastOneMove()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(pokemon.Learnset, Is.Not.Empty,
            $"{pokemon.Name} should have at least one move");
    }
}

[Test]
public void AllPokemon_HaveStartingMoves()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        var startingMoves = pokemon.GetStartingMoves().ToList();
        Assert.That(startingMoves, Is.Not.Empty,
            $"{pokemon.Name} should have at least one starting move");
    }
}

[Test]
public void AllPokemon_HaveSTABMoves()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        var stabMoves = pokemon.Learnset
            .Where(m => m.Move.Type == pokemon.PrimaryType ||
                       (pokemon.SecondaryType.HasValue && m.Move.Type == pokemon.SecondaryType.Value))
            .ToList();

        Assert.That(stabMoves, Is.Not.Empty,
            $"{pokemon.Name} should have at least one STAB move");
    }
}

[Test]
public void AllPokemon_HaveLevelUpMoves()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        var levelUpMoves = pokemon.Learnset
            .Where(m => m.Method == LearnMethod.LevelUp)
            .ToList();

        Assert.That(levelUpMoves, Is.Not.Empty,
            $"{pokemon.Name} should have at least one level-up move");
    }
}

[Test]
public void LearnsetMoves_AreUnique()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        var moveNames = pokemon.Learnset.Select(m => m.Move.Name).ToList();
        var uniqueMoves = moveNames.Distinct().ToList();

        Assert.That(uniqueMoves.Count, Is.EqualTo(moveNames.Count),
            $"{pokemon.Name} has duplicate moves in learnset");
    }
}

[Test]
public void LevelUpMoves_AreInAscendingOrder()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        var levelUpMoves = pokemon.Learnset
            .Where(m => m.Method == LearnMethod.LevelUp && m.Level.HasValue)
            .OrderBy(m => m.Level.Value)
            .ToList();

        for (int i = 1; i < levelUpMoves.Count; i++)
        {
            Assert.That(levelUpMoves[i].Level.Value, Is.GreaterThanOrEqualTo(levelUpMoves[i-1].Level.Value),
                $"{pokemon.Name} has level-up moves out of order");
        }
    }
}
```

### 2. Ability Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

**Add region**: `#region Ability Validation`

**Tests**:

```csharp
[Test]
public void AllPokemon_HaveAtLeastOneAbility()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(pokemon.Ability1, Is.Not.Null,
            $"{pokemon.Name} should have at least Ability1");
    }
}

[Test]
public void AllPokemon_HaveUniqueAbilities()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        var abilities = new List<AbilityData>();
        if (pokemon.Ability1 != null) abilities.Add(pokemon.Ability1);
        if (pokemon.Ability2 != null) abilities.Add(pokemon.Ability2);
        if (pokemon.HiddenAbility != null) abilities.Add(pokemon.HiddenAbility);

        var uniqueAbilities = abilities.Distinct().ToList();
        Assert.That(uniqueAbilities.Count, Is.EqualTo(abilities.Count),
            $"{pokemon.Name} has duplicate abilities");
    }
}

[Test]
public void Ability2_IsDifferentFromAbility1()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.Ability2 != null)
        {
            Assert.That(pokemon.Ability2, Is.Not.EqualTo(pokemon.Ability1),
                $"{pokemon.Name} Ability2 should be different from Ability1");
        }
    }
}

[Test]
public void HiddenAbility_IsDifferentFromRegularAbilities()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.HiddenAbility != null)
        {
            Assert.That(pokemon.HiddenAbility, Is.Not.EqualTo(pokemon.Ability1),
                $"{pokemon.Name} HiddenAbility should be different from Ability1");

            if (pokemon.Ability2 != null)
            {
                Assert.That(pokemon.HiddenAbility, Is.Not.EqualTo(pokemon.Ability2),
                    $"{pokemon.Name} HiddenAbility should be different from Ability2");
            }
        }
    }
}
```

### 3. Evolution Chain Consistency Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

**Add region**: `#region Evolution Chain Consistency`

**Tests**:

```csharp
[Test]
public void EvolutionChains_HaveConsistentGrowthRate()
{
    // All Pokemon in an evolution chain should have the same GrowthRate
    var evolutionChains = GetEvolutionChains();

    foreach (var chain in evolutionChains)
    {
        var growthRates = chain.Select(p => p.GrowthRate).Distinct().ToList();
        Assert.That(growthRates.Count, Is.EqualTo(1),
            $"Evolution chain starting with {chain.First().Name} should have consistent GrowthRate");
    }
}

[Test]
public void EvolutionChains_HaveConsistentBaseFriendship()
{
    // Most evolution chains have consistent BaseFriendship (except special cases)
    var evolutionChains = GetEvolutionChains();

    foreach (var chain in evolutionChains)
    {
        // Starters have 120, most others have 70
        var friendships = chain.Select(p => p.BaseFriendship).Distinct().ToList();
        // Allow some variation (e.g., starters vs wild)
        Assert.That(friendships.Count, Is.LessThanOrEqualTo(2),
            $"Evolution chain starting with {chain.First().Name} should have consistent BaseFriendship");
    }
}

[Test]
public void Evolutions_TargetExistsInCatalog()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        foreach (var evolution in pokemon.Evolutions)
        {
            var targetExists = PokemonCatalog.All.Any(p => p.Name == evolution.Target.Name);
            Assert.That(targetExists, Is.True,
                $"{pokemon.Name} evolves to {evolution.Target.Name} which doesn't exist in catalog");
        }
    }
}

[Test]
public void Evolutions_DoNotCreateCircularReferences()
{
    // A Pokemon should not evolve into itself or create cycles
    foreach (var pokemon in PokemonCatalog.All)
    {
        var visited = new HashSet<string>();
        var current = pokemon;

        while (current != null && current.CanEvolve)
        {
            Assert.That(visited.Contains(current.Name), Is.False,
                $"Circular evolution detected involving {current.Name}");

            visited.Add(current.Name);
            current = current.Evolutions.FirstOrDefault()?.Target;
        }
    }
}

[Test]
public void Evolutions_HaveValidConditions()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        foreach (var evolution in pokemon.Evolutions)
        {
            Assert.That(evolution.Conditions, Is.Not.Empty,
                $"{pokemon.Name} evolution to {evolution.Target.Name} should have at least one condition");
        }
    }
}

private List<List<PokemonSpeciesData>> GetEvolutionChains()
{
    // Helper to group Pokemon into evolution chains
    var chains = new List<List<PokemonSpeciesData>>();
    var processed = new HashSet<string>();

    foreach (var pokemon in PokemonCatalog.All)
    {
        if (processed.Contains(pokemon.Name)) continue;

        var chain = new List<PokemonSpeciesData> { pokemon };
        processed.Add(pokemon.Name);

        // Follow evolution chain forward
        var current = pokemon;
        while (current.CanEvolve)
        {
            var evolution = current.Evolutions.FirstOrDefault();
            if (evolution != null && !processed.Contains(evolution.Target.Name))
            {
                chain.Add(evolution.Target);
                processed.Add(evolution.Target.Name);
                current = evolution.Target;
            }
            else
            {
                break;
            }
        }

        chains.Add(chain);
    }

    return chains;
}
```

### 4. Data Consistency Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

**Add region**: `#region Data Consistency`

**Tests**:

```csharp
[Test]
public void Legendaries_HaveLowCatchRate()
{
    var legendaries = new[] { "Mewtwo", "Mew" };

    foreach (var legendaryName in legendaries)
    {
        var legendary = PokemonCatalog.All.FirstOrDefault(p => p.Name == legendaryName);
        if (legendary != null)
        {
            Assert.That(legendary.CatchRate, Is.LessThanOrEqualTo(45),
                $"{legendaryName} should have low catch rate (legendary)");
        }
    }
}

[Test]
public void Legendaries_HaveHighBST()
{
    var legendaries = new[] { "Mewtwo", "Mew" };

    foreach (var legendaryName in legendaries)
    {
        var legendary = PokemonCatalog.All.FirstOrDefault(p => p.Name == legendaryName);
        if (legendary != null)
        {
            Assert.That(legendary.BaseStats.Total, Is.GreaterThanOrEqualTo(580),
                $"{legendaryName} should have high BST (legendary)");
        }
    }
}

[Test]
public void Starters_HaveHighBaseFriendship()
{
    var starters = new[] { "Bulbasaur", "Charmander", "Squirtle" };

    foreach (var starterName in starters)
    {
        var starter = PokemonCatalog.All.FirstOrDefault(p => p.Name == starterName);
        if (starter != null)
        {
            Assert.That(starter.BaseFriendship, Is.EqualTo(120),
                $"{starterName} should have BaseFriendship 120 (starter)");
        }
    }
}

[Test]
public void GenderlessPokemon_CannotHaveGenderRatio()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.IsGenderless)
        {
            Assert.That(pokemon.GenderRatio, Is.LessThan(0),
                $"{pokemon.Name} is genderless but GenderRatio is not negative");
        }
    }
}

[Test]
public void PokedexNumbers_AreSequential()
{
    var numbers = PokemonCatalog.All
        .Select(p => p.PokedexNumber)
        .OrderBy(n => n)
        .ToList();

    // Check for gaps (allowing for missing Pokemon)
    for (int i = 1; i < numbers.Count; i++)
    {
        Assert.That(numbers[i], Is.GreaterThan(numbers[i-1]),
            "Pokedex numbers should be in ascending order");
    }
}

[Test]
public void Pokemon_WithSameName_HaveSamePokedexNumber()
{
    // This should never happen, but verify
    var grouped = PokemonCatalog.All
        .GroupBy(p => p.Name)
        .Where(g => g.Count() > 1)
        .ToList();

    Assert.That(grouped, Is.Empty, "Pokemon with same name found");
}
```

### 5. Variants System Validation Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

**Add region**: `#region Variants System Validation`

**Tests**:

```csharp
[Test]
public void Variants_HaveDifferentStatsThanBaseForm()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.IsVariant && pokemon.BaseForm != null)
        {
            // Variants should have different stats (at least HP should be different for Dinamax)
            if (pokemon.VariantType == PokemonVariantType.Dinamax ||
                pokemon.VariantType == PokemonVariantType.Gigantamax)
            {
                Assert.That(pokemon.BaseStats.HP, Is.GreaterThan(pokemon.BaseForm.BaseStats.HP),
                    $"{pokemon.Name} Dinamax should have higher HP than base form");
            }
            else
            {
                // Mega and Tera should have different stats
                Assert.That(pokemon.BaseStats.Total, Is.Not.EqualTo(pokemon.BaseForm.BaseStats.Total),
                    $"{pokemon.Name} variant should have different stats than base form");
            }
        }
    }
}

[Test]
public void Variants_BaseFormExistsInCatalog()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.IsVariant && pokemon.BaseForm != null)
        {
            var baseFormExists = PokemonCatalog.All.Any(p => p.Name == pokemon.BaseForm.Name);
            Assert.That(baseFormExists, Is.True,
                $"{pokemon.Name} variant references base form {pokemon.BaseForm.Name} which doesn't exist");
        }
    }
}

[Test]
public void Variants_CannotHaveVariants()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.IsVariant)
        {
            Assert.That(pokemon.Variants, Is.Empty,
                $"{pokemon.Name} is a variant and should not have variants");
        }
    }
}

[Test]
public void TeraVariants_HaveTeraType()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (pokemon.VariantType == PokemonVariantType.Tera)
        {
            Assert.That(pokemon.TeraType, Is.Not.Null,
                $"{pokemon.Name} Tera variant should have TeraType set");
        }
    }
}

[Test]
public void BaseForms_HaveVariantsListed()
{
    // When variants are added, base forms should reference them
    foreach (var pokemon in PokemonCatalog.All)
    {
        if (!pokemon.IsVariant && pokemon.Variants.Any())
        {
            foreach (var variant in pokemon.Variants)
            {
                Assert.That(variant.BaseForm, Is.EqualTo(pokemon),
                    $"{variant.Name} should reference {pokemon.Name} as BaseForm");
            }
        }
    }
}
```

### 6. Builder Validation Tests

**File**: `Data/Builders/PokemonBuilderTests.cs`

**Add region**: `#region Builder Validation Tests`

**Tests**:

```csharp
[Test]
public void Builder_ValidatesCatchRateRange()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .CatchRate(-1)
            .Build());

    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .CatchRate(256)
            .Build());
}

[Test]
public void Builder_ValidatesBaseFriendshipRange()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .BaseFriendship(-1)
            .Build());

    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .BaseFriendship(256)
            .Build());
}

[Test]
public void Builder_ValidatesBaseExperienceYieldRange()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .BaseExperienceYield(-1)
            .Build());
}

[Test]
public void Builder_ValidatesHeightRange()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .Height(-1f)
            .Build());
}

[Test]
public void Builder_ValidatesWeightRange()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 1)
            .Weight(-1f)
            .Build());
}

[Test]
public void Builder_RequiresName()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("", 1)
            .Build());
}

[Test]
public void Builder_RequiresPokedexNumber()
{
    Assert.Throws<ArgumentException>(() =>
        Pokemon.Define("Test", 0)
            .Build());
}
```

### 7. Instance Field Propagation Tests

**File**: `Systems/Core/Instances/PokemonInstanceTests.cs`

**Add region**: `#region Species Fields Usage Tests`

**Tests**:

```csharp
[Test]
public void Instance_UsesSpeciesBaseFriendship()
{
    var species = new PokemonSpeciesData
    {
        Name = "TestMon",
        BaseFriendship = 120
    };

    var instance = PokemonFactory.CreateInstance(species, level: 5);

    Assert.That(instance.Friendship, Is.EqualTo(120));
}

[Test]
public void Instance_UsesSpeciesGrowthRate()
{
    var species = new PokemonSpeciesData
    {
        Name = "TestMon",
        GrowthRate = GrowthRate.Slow
    };

    var instance = PokemonFactory.CreateInstance(species, level: 5);

    // Verify EXP calculation uses correct growth rate
    // (when EXP system is implemented)
    Assert.That(instance.Species.GrowthRate, Is.EqualTo(GrowthRate.Slow));
}

[Test]
public void Instance_CanAccessAllSpeciesFields()
{
    var species = new PokemonSpeciesData
    {
        Name = "TestMon",
        BaseExperienceYield = 100,
        CatchRate = 45,
        BaseFriendship = 70,
        GrowthRate = GrowthRate.MediumFast,
        Description = "Test description",
        Category = "Test Pokemon",
        Height = 1.0f,
        Weight = 10.0f
    };

    var instance = PokemonFactory.CreateInstance(species, level: 5);

    Assert.Multiple(() =>
    {
        Assert.That(instance.Species.BaseExperienceYield, Is.EqualTo(100));
        Assert.That(instance.Species.CatchRate, Is.EqualTo(45));
        Assert.That(instance.Species.BaseFriendship, Is.EqualTo(70));
        Assert.That(instance.Species.GrowthRate, Is.EqualTo(GrowthRate.MediumFast));
        Assert.That(instance.Species.Description, Is.EqualTo("Test description"));
    });
}
```

### 8. Pokedex Data Quality Tests

**File**: `Data/Catalogs/PokemonCatalogValidationTests.cs`

**Add region**: `#region Pokedex Data Quality`

**Tests**:

```csharp
[Test]
public void Descriptions_AreNotEmpty()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(pokemon.Description, Is.Not.Empty,
            $"{pokemon.Name} should have a description");
    }
}

[Test]
public void Descriptions_HaveMinimumLength()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(pokemon.Description.Length, Is.GreaterThan(10),
            $"{pokemon.Name} description should be at least 10 characters");
    }
}

[Test]
public void Categories_FollowPattern()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(pokemon.Category, Does.EndWith("Pokemon"),
            $"{pokemon.Name} category should end with 'Pokemon'");
    }
}

[Test]
public void Heights_AreRealistic()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        // Heights should be between 0.1m and 20m (reasonable range)
        Assert.That(pokemon.Height, Is.InRange(0.1f, 20.0f),
            $"{pokemon.Name} height should be realistic");
    }
}

[Test]
public void Weights_AreRealistic()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        // Weights should be between 0.1kg and 1000kg (reasonable range)
        Assert.That(pokemon.Weight, Is.InRange(0.1f, 1000.0f),
            $"{pokemon.Name} weight should be realistic");
    }
}

[Test]
public void Colors_AreValid()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(Enum.IsDefined(typeof(PokemonColor), pokemon.Color),
            $"{pokemon.Name} should have valid Color");
    }
}

[Test]
public void Shapes_AreValid()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(Enum.IsDefined(typeof(PokemonShape), pokemon.Shape),
            $"{pokemon.Name} should have valid Shape");
    }
}

[Test]
public void Habitats_AreValid()
{
    foreach (var pokemon in PokemonCatalog.All)
    {
        Assert.That(Enum.IsDefined(typeof(PokemonHabitat), pokemon.Habitat),
            $"{pokemon.Name} should have valid Habitat");
    }
}
```

---

## Summary

**Test Organization by Sub-Feature**:

### Grupo A: Core Entity Data (1.1-1.4)

1. **1.1: Pokemon Data**

    - `Blueprints/PokemonSpeciesDataTests.cs` - Extend with new regions for all new fields
    - `Data/Builders/PokemonBuilderTests.cs` - Extend with builder method tests + validation tests
    - `Data/Catalogs/PokemonCatalogValidationTests.cs` - Comprehensive validation
    - `Data/Pokemon/[Pokemon]Tests.cs` - Extend all 26 files with field validation tests
    - `Systems/Core/Instances/PokemonInstanceTests.cs` - Extend with instance field usage tests

2. **1.2: Move Data**

    - `Blueprints/MoveDataTests.cs` - Move blueprint structure tests
    - `Systems/Core/Instances/MoveInstanceTests.cs` - Move instance behavior tests
    - `Systems/Effects/[Effect]Tests.cs` - All 22 move effect tests
    - `Data/Builders/MoveBuilderTests.cs` - Move builder tests

3. **1.3: Ability Data**

    - `Blueprints/AbilityDataTests.cs` - Ability blueprint tests
    - `Data/Builders/AbilityBuilderTests.cs` - Ability builder tests

4. **1.4: Item Data**
    - `Blueprints/ItemDataTests.cs` - Item blueprint tests
    - `Data/Builders/ItemBuilderTests.cs` - Item builder tests

### Grupo B: Field & Status Data (1.5-1.6)

5. **1.5: Status Effect Data**

    - `Blueprints/StatusEffectDataTests.cs` - Status effect blueprint tests
    - `Data/Builders/StatusEffectBuilderTests.cs` - Status builder tests

6. **1.6: Field Conditions Data**
    - `Blueprints/[Weather/Terrain/Hazard/etc]DataTests.cs` - Field condition blueprint tests
    - `Data/Builders/[Weather/Terrain/etc]BuilderTests.cs` - Field condition builder tests

### Grupo C: Supporting Systems (1.7-1.8)

7. **1.11: Evolution System**

    - `Systems/Core/Evolution/EvolutionTests.cs` - Evolution path tests
    - `Systems/Core/Evolution/[Condition]Tests.cs` - All 6 evolution condition tests

8. **1.12: Type Effectiveness Table**
    - `Systems/Core/Factories/TypeEffectivenessTests.cs` - Type effectiveness calculation tests

### Grupo D: Infrastructure (1.13-1.17)

9. **1.13: Interfaces Base**

    - `Blueprints/IIdentifiableTests.cs` - Interface tests

10. **1.14: Enums & Constants**

    - `Data/Enums/[Enum]Tests.cs` - Enum validation tests
    - `Data/Constants/[Constants]Tests.cs` - Constants validation tests
    - `Blueprints/NatureDataTests.cs` - Nature modifier table tests

11. **Builders** (Moved to Feature 3.9)

    - See **[Feature 3.9: Builders](../3-content-expansion/3.9-builders/)** for builder tests

12. **1.15: Factories & Calculators**

    - `Systems/Core/Factories/StatCalculatorTests.cs` - Stat calculation tests
    - `Systems/Core/Factories/PokemonFactoryTests.cs` - Factory tests

13. **1.16: Registry System**
    - `Systems/Core/Registry/GameDataRegistryTests.cs` - Generic registry tests
    - `Systems/Core/Registry/PokemonRegistryTests.cs` - Pokemon registry tests
    - `Systems/Core/Registry/MoveRegistryTests.cs` - Move registry tests

### Grupo E: Planned Features (1.18-1.19)

14. **1.18: Variants System** ⏳ Planned

    -   Variants validation tests (when implemented)

15. **1.19: Pokedex Fields** ⏳ Planned
    -   Pokedex fields validation tests (when implemented)

**Additional Test Categories** (Sub-Feature 1.1):

-   ✅ **Learnset Validation** - Completeness, STAB moves, ordering
-   ✅ **Ability Validation** - Presence, uniqueness
-   ✅ **Evolution Consistency** - GrowthRate, BaseFriendship, circular references
-   ✅ **Data Consistency** - Legendary patterns, starter patterns, gender rules
-   ✅ **Variants Validation** - Relationships, stats differences, type consistency
-   ✅ **Builder Validation** - Range validation, required fields
-   ✅ **Instance Propagation** - Field usage from species
-   ✅ **Pokedex Quality** - Description length, category patterns, realistic values

**Approach**:

-   ✅ **Extend existing test files** - No "New" files, just add regions/methods
-   ✅ **Use regions** - Organize new tests in `#region` blocks
-   ✅ **Follow existing patterns** - Same naming conventions and structure
-   ✅ **Validate against official data** - Use real Pokemon values from games
-   ✅ **Comprehensive coverage** - Test relationships, consistency, and quality
-   ✅ **Organize by sub-feature** - Tests grouped by Feature 1 sub-features

**Priority**:

1. Critical fields (Sub-Feature 1.1: EXP, Catch, Friendship, GrowthRate)
2. Pokedex fields (Sub-Feature 1.19: Description, Category, Height, Weight, Color, Shape, Habitat)
3. Variants system (Sub-Feature 1.18: BaseForm, VariantType, TeraType, Variants)
4. Instance tests (Sub-Feature 1.1: field usage)
5. **Additional validation** (Sub-Feature 1.1: Learnset, Abilities, Consistency, Quality)

**Key Principle**: **Extend, don't create new files**. All tests go into existing files using regions for organization, grouped by sub-features.

---

## Related Documents

-   **[Architecture](architecture.md)** - What to test (data structure specification)
-   **[Use Cases](use_cases.md)** - Scenarios to verify
-   **[Roadmap](roadmap.md)** - Fields to test as they're implemented
-   **[Code Location](code_location.md)** - Where tests live
-   **[Test Structure Standard](../../ai/testing_structure_definition.md)** - Standard test organization

---

**Last Updated**: 2025-01-XX
