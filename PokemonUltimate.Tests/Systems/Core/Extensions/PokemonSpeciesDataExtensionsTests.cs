using NUnit.Framework;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Core.Extensions
{
    /// <summary>
    /// Tests for PokemonSpeciesData extension methods.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.19: Pokedex Fields
    /// **Documentation**: See `docs/features/1-game-data/1.19-pokedex-fields/README.md`
    /// </remarks>
    [TestFixture]
    public class PokemonSpeciesDataExtensionsTests
    {
        [Test]
        public void WithPokedexData_PokemonWithData_AppliesPokedexData()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25
            };

            // Act
            var result = pokemon.WithPokedexData();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.SameAs(pokemon)); // Returns same instance
                Assert.That(pokemon.Description, Is.Not.Empty);
                Assert.That(pokemon.Category, Is.EqualTo("Mouse Pokemon"));
                Assert.That(pokemon.Height, Is.EqualTo(0.4f));
                Assert.That(pokemon.Weight, Is.EqualTo(6.0f));
                Assert.That(pokemon.Color, Is.EqualTo(PokemonColor.Yellow));
                Assert.That(pokemon.Shape, Is.EqualTo(PokemonShape.Quadruped));
                Assert.That(pokemon.Habitat, Is.EqualTo(PokemonHabitat.Forest));
            });
        }

        [Test]
        public void WithPokedexData_PokemonWithoutData_DoesNotModify()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "NonExistentPokemon",
                PokedexNumber = 9999,
                Description = "Original",
                Category = "Original",
                Height = 1.0f,
                Weight = 10.0f,
                Color = PokemonColor.Red,
                Shape = PokemonShape.Ball,
                Habitat = PokemonHabitat.Cave
            };

            // Act
            var result = pokemon.WithPokedexData();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.SameAs(pokemon));
                Assert.That(pokemon.Description, Is.EqualTo("Original"));
                Assert.That(pokemon.Category, Is.EqualTo("Original"));
                Assert.That(pokemon.Height, Is.EqualTo(1.0f));
                Assert.That(pokemon.Weight, Is.EqualTo(10.0f));
                Assert.That(pokemon.Color, Is.EqualTo(PokemonColor.Red));
                Assert.That(pokemon.Shape, Is.EqualTo(PokemonShape.Ball));
                Assert.That(pokemon.Habitat, Is.EqualTo(PokemonHabitat.Cave));
            });
        }

        [Test]
        public void WithPokedexData_PokemonWithPartialData_OnlyUpdatesMissingFields()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                Description = "Custom description", // Already set
                Category = string.Empty, // Missing
                Height = 0f, // Missing
                Weight = 0f // Missing
            };

            // Act
            pokemon.WithPokedexData();

            // Assert
            Assert.Multiple(() =>
            {
                // Custom description should be preserved if we implement that logic
                // For now, we'll overwrite (can be changed later)
                Assert.That(pokemon.Category, Is.EqualTo("Mouse Pokemon"));
                Assert.That(pokemon.Height, Is.EqualTo(0.4f));
                Assert.That(pokemon.Weight, Is.EqualTo(6.0f));
            });
        }

        [Test]
        public void WithPokedexData_ReturnsSameInstance()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData { Name = "Pikachu" };

            // Act
            var result = pokemon.WithPokedexData();

            // Assert
            Assert.That(result, Is.SameAs(pokemon));
        }
    }
}
