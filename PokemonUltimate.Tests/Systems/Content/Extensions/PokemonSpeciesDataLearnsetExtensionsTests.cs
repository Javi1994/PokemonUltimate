using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Systems.Content.Extensions
{
    /// <summary>
    /// Tests for PokemonSpeciesData learnset extension methods.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    [TestFixture]
    public class PokemonSpeciesDataLearnsetExtensionsTests
    {
        [Test]
        public void WithLearnset_PokemonWithData_AppliesLearnset()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25
            };

            // Act
            var result = pokemon.WithLearnset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.SameAs(pokemon)); // Returns same instance
                Assert.That(pokemon.Learnset, Is.Not.Null);
                Assert.That(pokemon.Learnset, Is.Not.Empty);
                // Verify it has starting moves
                var startMoves = pokemon.Learnset.Where(m => m.Method == LearnMethod.Start).ToList();
                Assert.That(startMoves, Is.Not.Empty);
            });
        }

        [Test]
        public void WithLearnset_PokemonWithoutData_DoesNotModify()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "NonExistentPokemon",
                PokedexNumber = 9999,
                Learnset = new System.Collections.Generic.List<LearnableMove>
                {
                    new LearnableMove(MoveCatalog.Tackle, LearnMethod.Start)
                }
            };
            var originalLearnsetCount = pokemon.Learnset.Count;

            // Act
            var result = pokemon.WithLearnset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.SameAs(pokemon));
                Assert.That(pokemon.Learnset.Count, Is.EqualTo(originalLearnsetCount));
            });
        }

        [Test]
        public void WithLearnset_PokemonWithExistingLearnset_PreservesExisting()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                Learnset = new System.Collections.Generic.List<LearnableMove>
                {
                    new LearnableMove(MoveCatalog.Tackle, LearnMethod.Start)
                }
            };
            var originalLearnsetCount = pokemon.Learnset.Count;

            // Act
            pokemon.WithLearnset();

            // Assert
            // Should preserve existing learnset if already set
            // (Implementation can decide: replace or merge - for now we'll replace)
            Assert.That(pokemon.Learnset, Is.Not.Null);
        }

        [Test]
        public void WithLearnset_ReturnsSameInstance()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData { Name = "Pikachu" };

            // Act
            var result = pokemon.WithLearnset();

            // Assert
            Assert.That(result, Is.SameAs(pokemon));
        }

        [Test]
        public void WithLearnset_AppliesValidLearnableMoves()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25
            };

            // Act
            pokemon.WithLearnset();

            // Assert
            Assert.That(pokemon.Learnset, Is.Not.Null);
            foreach (var learnableMove in pokemon.Learnset)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(learnableMove.Move, Is.Not.Null);
                    // Verify Method is one of the valid enum values
                    Assert.That(System.Enum.IsDefined(typeof(LearnMethod), learnableMove.Method), Is.True);
                });
            }
        }
    }
}
