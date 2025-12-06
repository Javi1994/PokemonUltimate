using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Tests.Systems.Core.Registry
{
    /// <summary>
    /// Tests for basic PokemonRegistry operations: Register, Retrieve by Name, Exists, GetAll
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.4: Registry System
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonRegistryTests
    {
        private PokemonRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new PokemonRegistry();
        }

        #region Register and Retrieve by Name

        [Test]
        public void Test_Register_And_Retrieve_By_Name()
        {
            var pika = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            _registry.Register(pika);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.Exists("Pikachu"), Is.True);
                Assert.That(_registry.GetByName("Pikachu"), Is.EqualTo(pika));
            });
        }

        [Test]
        public void Test_GetByName_NonExistent_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _registry.GetByName("MissingNo"));
        }

        [Test]
        public void Test_Name_Is_Case_Sensitive()
        {
            var pika = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            _registry.Register(pika);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.Exists("Pikachu"), Is.True);
                Assert.That(_registry.Exists("pikachu"), Is.False, "Registry should be case sensitive");
                Assert.That(_registry.Exists("PIKACHU"), Is.False, "Registry should be case sensitive");
            });
        }

        [Test]
        public void Test_Exists_Returns_False_For_NonExistent()
        {
            Assert.That(_registry.Exists("NonExistent"), Is.False);
        }

        #endregion

        #region Multiple Pokemon

        [Test]
        public void Test_Register_Multiple_Pokemon()
        {
            var bulbasaur = new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 };
            var charmander = new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 };
            var squirtle = new PokemonSpeciesData { Name = "Squirtle", PokedexNumber = 7 };

            _registry.Register(bulbasaur);
            _registry.Register(charmander);
            _registry.Register(squirtle);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Bulbasaur"), Is.EqualTo(bulbasaur));
                Assert.That(_registry.GetByName("Charmander"), Is.EqualTo(charmander));
                Assert.That(_registry.GetByName("Squirtle"), Is.EqualTo(squirtle));
            });
        }

        #endregion

        #region GetAll Tests

        [Test]
        public void Test_GetAll_Returns_All_Registered_Pokemon()
        {
            var bulbasaur = new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 };
            var charmander = new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 };
            var squirtle = new PokemonSpeciesData { Name = "Squirtle", PokedexNumber = 7 };

            _registry.Register(bulbasaur);
            _registry.Register(charmander);
            _registry.Register(squirtle);

            var allPokemon = _registry.GetAll().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(allPokemon, Has.Count.EqualTo(3));
                Assert.That(allPokemon, Does.Contain(bulbasaur));
                Assert.That(allPokemon, Does.Contain(charmander));
                Assert.That(allPokemon, Does.Contain(squirtle));
            });
        }

        [Test]
        public void Test_GetAll_Returns_Empty_When_No_Pokemon_Registered()
        {
            var allPokemon = _registry.GetAll().ToList();

            Assert.That(allPokemon, Is.Empty);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Test_Overwrite_Duplicate_Name()
        {
            var pika1 = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            var pika2 = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 26 };

            _registry.Register(pika1);
            _registry.Register(pika2); // Should overwrite

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Pikachu"), Is.EqualTo(pika2));
                Assert.That(_registry.GetByName("Pikachu"), Is.Not.EqualTo(pika1));
            });
        }

        [Test]
        public void Test_Register_Null_Throws()
        {
            Assert.Throws<NullReferenceException>(() => _registry.Register(null!));
        }

        #endregion

        #region Variant Query Tests

        [Test]
        public void GetVariantsOf_Should_Return_All_Variants_Of_BaseForm()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            var megaCharizardX = new PokemonSpeciesData
            {
                Name = "Mega Charizard X",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Mega
            };
            var megaCharizardY = new PokemonSpeciesData
            {
                Name = "Mega Charizard Y",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Mega
            };

            _registry.Register(charizard);
            _registry.Register(megaCharizardX);
            _registry.Register(megaCharizardY);

            var variants = _registry.GetVariantsOf(charizard).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(variants, Has.Count.EqualTo(2));
                Assert.That(variants, Contains.Item(megaCharizardX));
                Assert.That(variants, Contains.Item(megaCharizardY));
            });
        }

        [Test]
        public void GetVariantsOf_With_Null_BaseForm_Should_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => _registry.GetVariantsOf(null!));
        }

        [Test]
        public void GetVariantsOf_With_No_Variants_Should_Return_Empty()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            _registry.Register(charizard);

            var variants = _registry.GetVariantsOf(charizard).ToList();

            Assert.That(variants, Is.Empty);
        }

        [Test]
        public void GetMegaVariants_Should_Return_All_Mega_Variants()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            var megaCharizardX = new PokemonSpeciesData
            {
                Name = "Mega Charizard X",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Mega
            };
            var charizardDinamax = new PokemonSpeciesData
            {
                Name = "Charizard Dinamax",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Dinamax
            };

            _registry.Register(charizard);
            _registry.Register(megaCharizardX);
            _registry.Register(charizardDinamax);

            var megaVariants = _registry.GetMegaVariants().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(megaVariants, Has.Count.EqualTo(1));
                Assert.That(megaVariants, Contains.Item(megaCharizardX));
                Assert.That(megaVariants, Does.Not.Contain(charizardDinamax));
            });
        }

        [Test]
        public void GetDinamaxVariants_Should_Return_All_Dinamax_Variants()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            var megaCharizardX = new PokemonSpeciesData
            {
                Name = "Mega Charizard X",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Mega
            };
            var charizardDinamax = new PokemonSpeciesData
            {
                Name = "Charizard Dinamax",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Dinamax
            };

            _registry.Register(charizard);
            _registry.Register(megaCharizardX);
            _registry.Register(charizardDinamax);

            var dinamaxVariants = _registry.GetDinamaxVariants().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(dinamaxVariants, Has.Count.EqualTo(1));
                Assert.That(dinamaxVariants, Contains.Item(charizardDinamax));
                Assert.That(dinamaxVariants, Does.Not.Contain(megaCharizardX));
            });
        }

        [Test]
        public void GetTeraVariants_Should_Return_All_Tera_Variants()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            var charizardTeraFire = new PokemonSpeciesData
            {
                Name = "Charizard Tera Fire",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Tera,
                TeraType = PokemonType.Fire
            };
            var charizardTeraDragon = new PokemonSpeciesData
            {
                Name = "Charizard Tera Dragon",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Tera,
                TeraType = PokemonType.Dragon
            };

            _registry.Register(charizard);
            _registry.Register(charizardTeraFire);
            _registry.Register(charizardTeraDragon);

            var teraVariants = _registry.GetTeraVariants().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(teraVariants, Has.Count.EqualTo(2));
                Assert.That(teraVariants, Contains.Item(charizardTeraFire));
                Assert.That(teraVariants, Contains.Item(charizardTeraDragon));
            });
        }

        [Test]
        public void GetBaseForms_Should_Return_All_Non_Variant_Pokemon()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            var megaCharizardX = new PokemonSpeciesData
            {
                Name = "Mega Charizard X",
                PokedexNumber = 6,
                BaseForm = charizard,
                VariantType = PokemonVariantType.Mega
            };
            var pikachu = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };

            _registry.Register(charizard);
            _registry.Register(megaCharizardX);
            _registry.Register(pikachu);

            var baseForms = _registry.GetBaseForms().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(baseForms, Has.Count.EqualTo(2));
                Assert.That(baseForms, Contains.Item(charizard));
                Assert.That(baseForms, Contains.Item(pikachu));
                Assert.That(baseForms, Does.Not.Contain(megaCharizardX));
            });
        }

        [Test]
        public void GetRegionalVariants_Should_Return_All_Regional_Variants()
        {
            var vulpix = new PokemonSpeciesData { Name = "Vulpix", PokedexNumber = 37 };
            var alolanVulpix = new PokemonSpeciesData
            {
                Name = "Alolan Vulpix",
                PokedexNumber = 37,
                BaseForm = vulpix,
                VariantType = PokemonVariantType.Regional,
                RegionalForm = "Alola"
            };
            var megaCharizardX = new PokemonSpeciesData
            {
                Name = "Mega Charizard X",
                PokedexNumber = 6,
                BaseForm = new PokemonSpeciesData { Name = "Charizard" },
                VariantType = PokemonVariantType.Mega
            };

            _registry.Register(vulpix);
            _registry.Register(alolanVulpix);
            _registry.Register(megaCharizardX);

            var regionalVariants = _registry.GetRegionalVariants().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(regionalVariants, Has.Count.EqualTo(1));
                Assert.That(regionalVariants, Contains.Item(alolanVulpix));
                Assert.That(regionalVariants, Does.Not.Contain(megaCharizardX));
            });
        }

        [Test]
        public void GetRegionalVariantsByRegion_Should_Return_Variants_From_Specific_Region()
        {
            var vulpix = new PokemonSpeciesData { Name = "Vulpix", PokedexNumber = 37 };
            var alolanVulpix = new PokemonSpeciesData
            {
                Name = "Alolan Vulpix",
                PokedexNumber = 37,
                BaseForm = vulpix,
                VariantType = PokemonVariantType.Regional,
                RegionalForm = "Alola"
            };
            var galarianMeowth = new PokemonSpeciesData
            {
                Name = "Galarian Meowth",
                PokedexNumber = 52,
                BaseForm = new PokemonSpeciesData { Name = "Meowth" },
                VariantType = PokemonVariantType.Regional,
                RegionalForm = "Galar"
            };

            _registry.Register(vulpix);
            _registry.Register(alolanVulpix);
            _registry.Register(galarianMeowth);

            var alolanVariants = _registry.GetRegionalVariantsByRegion("Alola").ToList();

            Assert.Multiple(() =>
            {
                Assert.That(alolanVariants, Has.Count.EqualTo(1));
                Assert.That(alolanVariants, Contains.Item(alolanVulpix));
                Assert.That(alolanVariants, Does.Not.Contain(galarianMeowth));
            });
        }

        [Test]
        public void GetVariantsWithGameplayChanges_Should_Return_Only_Variants_With_Changes()
        {
            var staticAbility = PokemonUltimate.Content.Catalogs.Abilities.AbilityCatalog.Static;
            
            var pikachu = new PokemonSpeciesData 
            { 
                Name = "Pikachu", 
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Ability1 = staticAbility
            };
            var megaPikachu = new PokemonSpeciesData
            {
                Name = "Mega Pikachu",
                PokedexNumber = 25,
                BaseForm = pikachu,
                VariantType = PokemonVariantType.Mega,
                BaseStats = new BaseStats(35, 75, 50, 70, 60, 110) // Higher stats
            };
            var pikachuCosmetic = new PokemonSpeciesData
            {
                Name = "Pikachu (Cosmetic)",
                PokedexNumber = 25,
                BaseForm = pikachu,
                VariantType = PokemonVariantType.Cosmetic,
                RegionalForm = "Cosmetic",
                PrimaryType = PokemonType.Electric, // Same
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90), // Same
                Ability1 = staticAbility // Same
            };

            _registry.Register(pikachu);
            _registry.Register(megaPikachu);
            _registry.Register(pikachuCosmetic);

            var variantsWithChanges = _registry.GetVariantsWithGameplayChanges().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(variantsWithChanges, Contains.Item(megaPikachu));
                Assert.That(variantsWithChanges, Does.Not.Contain(pikachuCosmetic));
            });
        }

        [Test]
        public void GetVisualOnlyVariants_Should_Return_Purely_Visual_Variants()
        {
            var staticAbility = PokemonUltimate.Content.Catalogs.Abilities.AbilityCatalog.Static;
            
            var pikachu = new PokemonSpeciesData 
            { 
                Name = "Pikachu", 
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Ability1 = staticAbility
            };
            var megaPikachu = new PokemonSpeciesData
            {
                Name = "Mega Pikachu",
                PokedexNumber = 25,
                BaseForm = pikachu,
                VariantType = PokemonVariantType.Mega,
                BaseStats = new BaseStats(35, 75, 50, 70, 60, 110)
            };
            var pikachuCosmetic = new PokemonSpeciesData
            {
                Name = "Pikachu (Cosmetic)",
                PokedexNumber = 25,
                BaseForm = pikachu,
                VariantType = PokemonVariantType.Cosmetic,
                RegionalForm = "Cosmetic",
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Ability1 = staticAbility
            };

            _registry.Register(pikachu);
            _registry.Register(megaPikachu);
            _registry.Register(pikachuCosmetic);

            var visualOnly = _registry.GetVisualOnlyVariants().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(visualOnly, Contains.Item(pikachuCosmetic));
                Assert.That(visualOnly, Does.Not.Contain(megaPikachu));
            });
        }

        #endregion
    }
}

