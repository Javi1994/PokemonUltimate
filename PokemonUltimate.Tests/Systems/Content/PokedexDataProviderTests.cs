using NUnit.Framework;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Content
{
    /// <summary>
    /// Tests for PokedexDataProvider - provides Pokedex data for Pokemon species.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.19: Pokedex Fields
    /// **Documentation**: See `docs/features/1-game-data/1.19-pokedex-fields/README.md`
    /// </remarks>
    [TestFixture]
    public class PokedexDataProviderTests
    {
        #region GetData Tests

        [Test]
        public void GetData_ExistingPokemon_ReturnsPokedexData()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var data = PokedexDataProvider.GetData(pokemonName);

            // Assert
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Description, Is.Not.Empty);
            Assert.That(data.Category, Is.Not.Empty);
            Assert.That(data.Height, Is.GreaterThan(0));
            Assert.That(data.Weight, Is.GreaterThan(0));
            Assert.That(data.Color, Is.Not.EqualTo(PokemonColor.Unknown));
            Assert.That(data.Shape, Is.Not.EqualTo(PokemonShape.Unknown));
            Assert.That(data.Habitat, Is.Not.EqualTo(PokemonHabitat.Unknown));
        }

        [Test]
        public void GetData_NonExistentPokemon_ReturnsNull()
        {
            // Arrange
            var pokemonName = "NonExistentPokemon";

            // Act
            var data = PokedexDataProvider.GetData(pokemonName);

            // Assert
            Assert.That(data, Is.Null);
        }

        [Test]
        public void GetData_NullName_ReturnsNull()
        {
            // Act
            var data = PokedexDataProvider.GetData(null);

            // Assert
            Assert.That(data, Is.Null);
        }

        [Test]
        public void GetData_EmptyName_ReturnsNull()
        {
            // Act
            var data = PokedexDataProvider.GetData(string.Empty);

            // Assert
            Assert.That(data, Is.Null);
        }

        [Test]
        public void GetData_CaseInsensitive_ReturnsData()
        {
            // Arrange
            var pokemonName = "PIKACHU";

            // Act
            var data = PokedexDataProvider.GetData(pokemonName);

            // Assert
            Assert.That(data, Is.Not.Null);
        }

        #endregion

        #region HasData Tests

        [Test]
        public void HasData_ExistingPokemon_ReturnsTrue()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var hasData = PokedexDataProvider.HasData(pokemonName);

            // Assert
            Assert.That(hasData, Is.True);
        }

        [Test]
        public void HasData_NonExistentPokemon_ReturnsFalse()
        {
            // Arrange
            var pokemonName = "NonExistentPokemon";

            // Act
            var hasData = PokedexDataProvider.HasData(pokemonName);

            // Assert
            Assert.That(hasData, Is.False);
        }

        [Test]
        public void HasData_NullName_ReturnsFalse()
        {
            // Act
            var hasData = PokedexDataProvider.HasData(null);

            // Assert
            Assert.That(hasData, Is.False);
        }

        #endregion

        #region Data Quality Tests

        [Test]
        public void GetData_Pikachu_HasCorrectValues()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var data = PokedexDataProvider.GetData(pokemonName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(data.Description, Is.Not.Empty);
                Assert.That(data.Category, Is.EqualTo("Mouse Pokemon"));
                Assert.That(data.Height, Is.EqualTo(0.4f));
                Assert.That(data.Weight, Is.EqualTo(6.0f));
                Assert.That(data.Color, Is.EqualTo(PokemonColor.Yellow));
                Assert.That(data.Shape, Is.EqualTo(PokemonShape.Quadruped));
                Assert.That(data.Habitat, Is.EqualTo(PokemonHabitat.Forest));
            });
        }

        [Test]
        public void GetData_Bulbasaur_HasCorrectValues()
        {
            // Arrange
            var pokemonName = "Bulbasaur";

            // Act
            var data = PokedexDataProvider.GetData(pokemonName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(data, Is.Not.Null);
                Assert.That(data.Description, Is.Not.Empty);
                Assert.That(data.Category, Is.EqualTo("Seed Pokemon"));
                Assert.That(data.Height, Is.EqualTo(0.7f));
                Assert.That(data.Weight, Is.EqualTo(6.9f));
                Assert.That(data.Color, Is.EqualTo(PokemonColor.Green));
            });
        }

        #endregion
    }
}
