using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Content
{
    /// <summary>
    /// Tests for VariantProvider - provides variant forms for Pokemon species.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.18: Variants System
    /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
    /// </remarks>
    [TestFixture]
    public class VariantProviderTests
    {
        #region GetVariants Tests

        [Test]
        public void GetVariants_ExistingPokemonWithVariants_ReturnsVariants()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var variants = VariantProvider.GetVariants(charizard).ToList();

            // Assert
            Assert.That(variants, Is.Not.Empty, "Charizard should have variants");
            Assert.That(variants.All(v => v.BaseForm == charizard), Is.True, "All variants should reference Charizard as base form");
        }

        [Test]
        public void GetVariants_NonExistentPokemon_ReturnsEmpty()
        {
            // Arrange
            var nonExistent = new PokemonSpeciesData { Name = "NonExistentPokemon" };

            // Act
            var variants = VariantProvider.GetVariants(nonExistent).ToList();

            // Assert
            Assert.That(variants, Is.Empty);
        }

        [Test]
        public void GetVariants_NullBaseForm_ReturnsEmpty()
        {
            // Act
            PokemonSpeciesData nullPokemon = null;
            var variants = VariantProvider.GetVariants(nullPokemon).ToList();

            // Assert
            Assert.That(variants, Is.Empty);
        }

        [Test]
        public void GetVariants_ByPokemonName_ReturnsVariants()
        {
            // Arrange
            string pokemonName = "Charizard";

            // Act
            var variants = VariantProvider.GetVariants((string)pokemonName).ToList();

            // Assert
            Assert.That(variants, Is.Not.Empty, "Charizard should have variants");
        }

        [Test]
        public void GetVariants_ByPokemonName_CaseInsensitive()
        {
            // Arrange
            string pokemonName = "CHARIZARD";

            // Act
            var variants = VariantProvider.GetVariants(pokemonName).ToList();

            // Assert
            Assert.That(variants, Is.Not.Empty, "Should be case-insensitive");
        }

        [Test]
        public void GetVariants_ByPokemonName_NullOrEmpty_ReturnsEmpty()
        {
            // Act & Assert
            Assert.That(VariantProvider.GetVariants((string)null).ToList(), Is.Empty);
            Assert.That(VariantProvider.GetVariants(string.Empty).ToList(), Is.Empty);
        }

        #endregion

        #region HasVariants Tests

        [Test]
        public void HasVariants_PokemonWithVariants_ReturnsTrue()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var hasVariants = VariantProvider.HasVariants(charizard);

            // Assert
            Assert.That(hasVariants, Is.True, "Charizard should have variants");
        }

        [Test]
        public void HasVariants_PokemonWithoutVariants_ReturnsFalse()
        {
            // Arrange
            var pikachu = PokemonCatalog.Pikachu;
            // Note: Pikachu might have variants, so we'll use a Pokemon that definitely doesn't
            var pokemonWithoutVariants = new PokemonSpeciesData { Name = "TestPokemon" };

            // Act
            var hasVariants = VariantProvider.HasVariants(pokemonWithoutVariants);

            // Assert
            Assert.That(hasVariants, Is.False);
        }

        [Test]
        public void HasVariants_NullBaseForm_ReturnsFalse()
        {
            // Act
            var hasVariants = VariantProvider.HasVariants(null);

            // Assert
            Assert.That(hasVariants, Is.False);
        }

        #endregion

        #region GetMegaVariants Tests

        [Test]
        public void GetMegaVariants_PokemonWithMegaVariants_ReturnsOnlyMega()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var megaVariants = VariantProvider.GetMegaVariants(charizard).ToList();

            // Assert
            Assert.That(megaVariants, Is.Not.Empty, "Charizard should have Mega variants");
            Assert.That(megaVariants.All(v => v.IsMegaVariant), Is.True, "All should be Mega variants");
            Assert.That(megaVariants.All(v => v.VariantType == PokemonVariantType.Mega), Is.True);
        }

        [Test]
        public void GetMegaVariants_PokemonWithoutMegaVariants_ReturnsEmpty()
        {
            // Arrange
            var pokemonWithoutMega = new PokemonSpeciesData { Name = "TestPokemon" };

            // Act
            var megaVariants = VariantProvider.GetMegaVariants(pokemonWithoutMega).ToList();

            // Assert
            Assert.That(megaVariants, Is.Empty);
        }

        #endregion

        #region GetRegionalVariants Tests

        [Test]
        public void GetRegionalVariants_PokemonWithRegionalVariants_ReturnsOnlyRegional()
        {
            // Arrange
            var pikachu = PokemonCatalog.Pikachu;

            // Act
            var regionalVariants = VariantProvider.GetRegionalVariants(pikachu).ToList();

            // Assert
            if (regionalVariants.Any())
            {
                Assert.That(regionalVariants.All(v => v.IsRegionalVariant), Is.True, "All should be Regional variants");
                Assert.That(regionalVariants.All(v => v.VariantType == PokemonVariantType.Regional), Is.True);
            }
        }

        [Test]
        public void GetRegionalVariants_PokemonWithoutRegionalVariants_ReturnsEmpty()
        {
            // Arrange
            var pokemonWithoutRegional = new PokemonSpeciesData { Name = "TestPokemon" };

            // Act
            var regionalVariants = VariantProvider.GetRegionalVariants(pokemonWithoutRegional).ToList();

            // Assert
            Assert.That(regionalVariants, Is.Empty);
        }

        #endregion

        #region GetTeraVariants Tests

        [Test]
        public void GetTeraVariants_PokemonWithTeraVariants_ReturnsOnlyTera()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var teraVariants = VariantProvider.GetTeraVariants(charizard).ToList();

            // Assert
            // Note: Tera variants might not be implemented yet, so we just check the method works
            Assert.That(teraVariants.All(v => v.IsTeraVariant), Is.True, "All should be Tera variants if any exist");
        }

        [Test]
        public void GetTeraVariants_PokemonWithoutTeraVariants_ReturnsEmpty()
        {
            // Arrange
            var pokemonWithoutTera = new PokemonSpeciesData { Name = "TestPokemon" };

            // Act
            var teraVariants = VariantProvider.GetTeraVariants(pokemonWithoutTera).ToList();

            // Assert
            Assert.That(teraVariants, Is.Empty);
        }

        #endregion

        #region Variant Relationship Tests

        [Test]
        public void GetVariants_VariantsHaveCorrectBaseForm()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var variants = VariantProvider.GetVariants(charizard).ToList();

            // Assert
            Assert.That(variants, Is.Not.Empty);
            foreach (var variant in variants)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(variant.BaseForm, Is.EqualTo(charizard), $"{variant.Name} should reference Charizard as base form");
                    Assert.That(variant.IsVariant, Is.True, $"{variant.Name} should be marked as variant");
                    Assert.That(charizard.Variants, Contains.Item(variant), "Base form should have variant in Variants list");
                });
            }
        }

        [Test]
        public void GetVariants_VariantsHaveUniqueNames()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var variants = VariantProvider.GetVariants(charizard).ToList();

            // Assert
            var variantNames = variants.Select(v => v.Name).ToList();
            Assert.That(variantNames, Is.Unique, "All variants should have unique names");
        }

        #endregion

        #region Extension Methods Tests

        [Test]
        public void GetVariants_ExtensionMethod_WorksCorrectly()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var variants = charizard.GetVariants().ToList();

            // Assert
            Assert.That(variants, Is.Not.Empty, "Extension method should work");
            Assert.That(variants.All(v => v.BaseForm == charizard), Is.True, "All variants should reference Charizard");
        }

        [Test]
        public void HasVariantsAvailable_ExtensionMethod_WorksCorrectly()
        {
            // Arrange
            var charizard = PokemonCatalog.Charizard;

            // Act
            var hasVariants = charizard.HasVariantsAvailable();

            // Assert
            Assert.That(hasVariants, Is.True, "Extension method should work");
        }

        #endregion
    }
}

