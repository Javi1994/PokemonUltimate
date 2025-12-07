using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Edge cases tests for content localization (Move, Pokemon, Ability, Item names).
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/CONTENT_LOCALIZATION_DESIGN.md`
    /// </remarks>
    [TestFixture]
    public class ContentLocalizationEdgeCasesTests
    {
        private ILocalizationProvider _localizationProvider;

        [SetUp]
        public void SetUp()
        {
            _localizationProvider = new LocalizationProvider();
        }

        [Test]
        public void MoveData_GetDisplayName_NullMove_ReturnsEmptyString()
        {
            // Arrange
            MoveData move = null;

            // Act
            var result = move.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void PokemonSpeciesData_GetDisplayName_NullSpecies_ReturnsEmptyString()
        {
            // Arrange
            PokemonSpeciesData species = null;

            // Act
            var result = species.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void AbilityData_GetDisplayName_NullAbility_ReturnsEmptyString()
        {
            // Arrange
            AbilityData ability = null;

            // Act
            var result = ability.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ItemData_GetDisplayName_NullItem_ReturnsEmptyString()
        {
            // Arrange
            ItemData item = null;

            // Act
            var result = item.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void AbilityData_GetDisplayName_WithSpaces_HandlesCorrectly()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";
            var ability = AbilityCatalog.SpeedBoost;

            // Act
            var result = ability.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Impulso"));
        }

        [Test]
        public void ItemData_GetDisplayName_WithSpaces_HandlesCorrectly()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";
            var item = ItemCatalog.ChoiceBand;

            // Act
            var result = item.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Cinta Elegida"));
        }
    }
}
