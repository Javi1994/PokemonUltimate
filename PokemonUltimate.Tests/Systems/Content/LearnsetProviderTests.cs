using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Systems.Content
{
    /// <summary>
    /// Tests for LearnsetProvider - provides learnset data for Pokemon species.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    [TestFixture]
    public class LearnsetProviderTests
    {
        #region GetLearnset Tests

        [Test]
        public void GetLearnset_ExistingPokemon_ReturnsLearnsetData()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.That(learnset, Is.Not.Null);
            Assert.That(learnset.Moves, Is.Not.Empty);
        }

        [Test]
        public void GetLearnset_NonExistentPokemon_ReturnsNull()
        {
            // Arrange
            var pokemonName = "NonExistentPokemon";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.That(learnset, Is.Null);
        }

        [Test]
        public void GetLearnset_NullName_ReturnsNull()
        {
            // Act
            var learnset = LearnsetProvider.GetLearnset(null);

            // Assert
            Assert.That(learnset, Is.Null);
        }

        [Test]
        public void GetLearnset_EmptyName_ReturnsNull()
        {
            // Act
            var learnset = LearnsetProvider.GetLearnset(string.Empty);

            // Assert
            Assert.That(learnset, Is.Null);
        }

        [Test]
        public void GetLearnset_CaseInsensitive_ReturnsData()
        {
            // Arrange
            var pokemonName = "PIKACHU";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.That(learnset, Is.Not.Null);
        }

        #endregion

        #region HasLearnset Tests

        [Test]
        public void HasLearnset_ExistingPokemon_ReturnsTrue()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var hasLearnset = LearnsetProvider.HasLearnset(pokemonName);

            // Assert
            Assert.That(hasLearnset, Is.True);
        }

        [Test]
        public void HasLearnset_NonExistentPokemon_ReturnsFalse()
        {
            // Arrange
            var pokemonName = "NonExistentPokemon";

            // Act
            var hasLearnset = LearnsetProvider.HasLearnset(pokemonName);

            // Assert
            Assert.That(hasLearnset, Is.False);
        }

        [Test]
        public void HasLearnset_NullName_ReturnsFalse()
        {
            // Act
            var hasLearnset = LearnsetProvider.HasLearnset(null);

            // Assert
            Assert.That(hasLearnset, Is.False);
        }

        #endregion

        #region Learnset Quality Tests

        [Test]
        public void GetLearnset_Pikachu_HasStartingMoves()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.That(learnset, Is.Not.Null);
            var startMoves = learnset.Moves.Where(m => m.Method == LearnMethod.Start).ToList();
            Assert.That(startMoves, Is.Not.Empty);
            Assert.That(startMoves.Any(m => m.Move.Name == "Thunder Shock"), Is.True);
        }

        [Test]
        public void GetLearnset_Pikachu_HasLevelUpMoves()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.That(learnset, Is.Not.Null);
            var levelUpMoves = learnset.Moves.Where(m => m.Method == LearnMethod.LevelUp).ToList();
            Assert.That(levelUpMoves, Is.Not.Empty);
        }

        [Test]
        public void GetLearnset_Bulbasaur_HasCorrectMoves()
        {
            // Arrange
            var pokemonName = "Bulbasaur";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(learnset, Is.Not.Null);
                Assert.That(learnset.Moves, Is.Not.Empty);
                // Verify it has starting moves
                var startMoves = learnset.Moves.Where(m => m.Method == LearnMethod.Start).ToList();
                Assert.That(startMoves, Is.Not.Empty);
            });
        }

        [Test]
        public void GetLearnset_ReturnsValidLearnableMoves()
        {
            // Arrange
            var pokemonName = "Pikachu";

            // Act
            var learnset = LearnsetProvider.GetLearnset(pokemonName);

            // Assert
            Assert.That(learnset, Is.Not.Null);
            foreach (var learnableMove in learnset.Moves)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(learnableMove.Move, Is.Not.Null, "Move should not be null");
                    // Verify Method is one of the valid enum values
                    Assert.That(Enum.IsDefined(typeof(LearnMethod), learnableMove.Method), Is.True, "Method should be a valid LearnMethod");
                });
            }
        }

        #endregion
    }
}
