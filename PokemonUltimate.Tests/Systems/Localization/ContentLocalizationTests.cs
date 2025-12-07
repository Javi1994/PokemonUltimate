using NUnit.Framework;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Tests for content localization (Move and Pokemon names).
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/CONTENT_LOCALIZATION_DESIGN.md`
    /// </remarks>
    [TestFixture]
    public class ContentLocalizationTests
    {
        private ILocalizationProvider _localizationProvider;

        [SetUp]
        public void SetUp()
        {
            _localizationProvider = new LocalizationProvider();
        }

        [Test]
        public void MoveData_GetDisplayName_English_ReturnsEnglishName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";
            var move = new MoveData
            {
                Name = "Thunderbolt"
            };

            // Act
            var result = move.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Thunderbolt"));
        }

        [Test]
        public void MoveData_GetDisplayName_Spanish_ReturnsSpanishName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";
            var move = new MoveData
            {
                Name = "Thunderbolt"
            };

            // Act
            var result = move.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Rayo"));
        }

        [Test]
        public void MoveData_GetDisplayName_French_ReturnsFrenchName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "fr";
            var move = new MoveData
            {
                Name = "Thunderbolt"
            };

            // Act
            var result = move.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Tonnerre"));
        }

        [Test]
        public void MoveData_GetDisplayName_NonExistentMove_FallsBackToOriginalName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";
            var move = new MoveData
            {
                Name = "NonExistentMove"
            };

            // Act
            var result = move.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("NonExistentMove"));
        }

        [Test]
        public void MoveData_GetDisplayName_NullProvider_ReturnsOriginalName()
        {
            // Arrange
            var move = new MoveData
            {
                Name = "Thunderbolt"
            };

            // Act
            var result = move.GetDisplayName(null);

            // Assert
            Assert.That(result, Is.EqualTo("Thunderbolt"));
        }

        [Test]
        public void PokemonSpeciesData_GetDisplayName_English_ReturnsEnglishName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu"
            };

            // Act
            var result = pokemon.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Pikachu"));
        }

        [Test]
        public void PokemonSpeciesData_GetDisplayName_Spanish_ReturnsSpanishName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";
            var pokemon = new PokemonSpeciesData
            {
                Name = "Charizard"
            };

            // Act
            var result = pokemon.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Charizard")); // Same in Spanish
        }

        [Test]
        public void PokemonSpeciesData_GetDisplayName_French_ReturnsFrenchName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "fr";
            var pokemon = new PokemonSpeciesData
            {
                Name = "Bulbasaur"
            };

            // Act
            var result = pokemon.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Bulbizarre"));
        }

        [Test]
        public void PokemonSpeciesData_GetDisplayName_NonExistentPokemon_FallsBackToOriginalName()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";
            var pokemon = new PokemonSpeciesData
            {
                Name = "NonExistentPokemon"
            };

            // Act
            var result = pokemon.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("NonExistentPokemon"));
        }

        [Test]
        public void PokemonSpeciesData_GetDisplayName_NullProvider_ReturnsOriginalName()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu"
            };

            // Act
            var result = pokemon.GetDisplayName(null);

            // Assert
            Assert.That(result, Is.EqualTo("Pikachu"));
        }

        [Test]
        public void MoveData_GetDisplayName_MoveWithSpaces_HandlesCorrectly()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";
            var move = new MoveData
            {
                Name = "Thunder Wave"
            };

            // Act
            var result = move.GetDisplayName(_localizationProvider);

            // Assert
            Assert.That(result, Is.EqualTo("Onda Trueno"));
        }
    }
}
