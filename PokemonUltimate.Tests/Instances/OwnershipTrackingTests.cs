using System;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Instances
{
    /// <summary>
    /// Functional tests for Ownership & Tracking fields in PokemonInstance.
    /// </summary>
    [TestFixture]
    public class OwnershipTrackingTests
    {
        private PokemonSpeciesData _testSpecies;

        [SetUp]
        public void SetUp()
        {
            _testSpecies = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats { HP = 35, Attack = 55, Defense = 30, SpAttack = 50, SpDefense = 40, Speed = 90 }
            };
        }

        #region OriginalTrainer Tests

        [Test]
        public void PokemonInstance_OriginalTrainer_Default_IsNull()
        {
            // Arrange & Act
            var pokemon = CreateTestPokemon();

            // Assert
            Assert.That(pokemon.OriginalTrainer, Is.Null);
        }

        [Test]
        public void PokemonInstance_OriginalTrainer_CanBeSet()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.OriginalTrainer = "Ash";

            // Assert
            Assert.That(pokemon.OriginalTrainer, Is.EqualTo("Ash"));
        }

        [Test]
        public void PokemonInstance_OriginalTrainer_CanBeSetToNull()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            pokemon.OriginalTrainer = "Ash";

            // Act
            pokemon.OriginalTrainer = null;

            // Assert
            Assert.That(pokemon.OriginalTrainer, Is.Null);
        }

        #endregion

        #region TrainerId Tests

        [Test]
        public void PokemonInstance_TrainerId_Default_IsNull()
        {
            // Arrange & Act
            var pokemon = CreateTestPokemon();

            // Assert
            Assert.That(pokemon.TrainerId, Is.Null);
        }

        [Test]
        public void PokemonInstance_TrainerId_CanBeSet()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.TrainerId = 12345;

            // Assert
            Assert.That(pokemon.TrainerId, Is.EqualTo(12345));
        }

        [Test]
        public void PokemonInstance_TrainerId_CanBeSetToNull()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            pokemon.TrainerId = 12345;

            // Act
            pokemon.TrainerId = null;

            // Assert
            Assert.That(pokemon.TrainerId, Is.Null);
        }

        #endregion

        #region MetLevel Tests

        [Test]
        public void PokemonInstance_MetLevel_Default_IsNull()
        {
            // Arrange & Act
            var pokemon = CreateTestPokemon();

            // Assert
            Assert.That(pokemon.MetLevel, Is.Null);
        }

        [Test]
        public void PokemonInstance_MetLevel_CanBeSet()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.MetLevel = 5;

            // Assert
            Assert.That(pokemon.MetLevel, Is.EqualTo(5));
        }

        [Test]
        public void PokemonInstance_MetLevel_CanBeSetToNull()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            pokemon.MetLevel = 5;

            // Act
            pokemon.MetLevel = null;

            // Assert
            Assert.That(pokemon.MetLevel, Is.Null);
        }

        [Test]
        public void PokemonInstance_MetLevel_CanBeSetToMinimum()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.MetLevel = 1;

            // Assert
            Assert.That(pokemon.MetLevel, Is.EqualTo(1));
        }

        [Test]
        public void PokemonInstance_MetLevel_CanBeSetToMaximum()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.MetLevel = 100;

            // Assert
            Assert.That(pokemon.MetLevel, Is.EqualTo(100));
        }

        #endregion

        #region MetLocation Tests

        [Test]
        public void PokemonInstance_MetLocation_Default_IsNull()
        {
            // Arrange & Act
            var pokemon = CreateTestPokemon();

            // Assert
            Assert.That(pokemon.MetLocation, Is.Null);
        }

        [Test]
        public void PokemonInstance_MetLocation_CanBeSet()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.MetLocation = "Route 1";

            // Assert
            Assert.That(pokemon.MetLocation, Is.EqualTo("Route 1"));
        }

        [Test]
        public void PokemonInstance_MetLocation_CanBeSetToNull()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            pokemon.MetLocation = "Route 1";

            // Act
            pokemon.MetLocation = null;

            // Assert
            Assert.That(pokemon.MetLocation, Is.Null);
        }

        [Test]
        public void PokemonInstance_MetLocation_CanBeSetToEmptyString()
        {
            // Arrange
            var pokemon = CreateTestPokemon();

            // Act
            pokemon.MetLocation = string.Empty;

            // Assert
            Assert.That(pokemon.MetLocation, Is.EqualTo(string.Empty));
        }

        #endregion

        #region MetDate Tests

        [Test]
        public void PokemonInstance_MetDate_Default_IsNull()
        {
            // Arrange & Act
            var pokemon = CreateTestPokemon();

            // Assert
            Assert.That(pokemon.MetDate, Is.Null);
        }

        [Test]
        public void PokemonInstance_MetDate_CanBeSet()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            var date = new DateTime(2024, 1, 15);

            // Act
            pokemon.MetDate = date;

            // Assert
            Assert.That(pokemon.MetDate, Is.EqualTo(date));
        }

        [Test]
        public void PokemonInstance_MetDate_CanBeSetToNull()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            pokemon.MetDate = new DateTime(2024, 1, 15);

            // Act
            pokemon.MetDate = null;

            // Assert
            Assert.That(pokemon.MetDate, Is.Null);
        }

        [Test]
        public void PokemonInstance_MetDate_CanBeSetToCurrentDate()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            var currentDate = DateTime.Now;

            // Act
            pokemon.MetDate = currentDate;

            // Assert
            Assert.That(pokemon.MetDate, Is.EqualTo(currentDate));
        }

        #endregion

        #region Combined Ownership Tests

        [Test]
        public void PokemonInstance_AllOwnershipFields_CanBeSetTogether()
        {
            // Arrange
            var pokemon = CreateTestPokemon();
            var date = new DateTime(2024, 1, 15);

            // Act
            pokemon.OriginalTrainer = "Ash";
            pokemon.TrainerId = 12345;
            pokemon.MetLevel = 5;
            pokemon.MetLocation = "Route 1";
            pokemon.MetDate = date;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(pokemon.OriginalTrainer, Is.EqualTo("Ash"));
                Assert.That(pokemon.TrainerId, Is.EqualTo(12345));
                Assert.That(pokemon.MetLevel, Is.EqualTo(5));
                Assert.That(pokemon.MetLocation, Is.EqualTo("Route 1"));
                Assert.That(pokemon.MetDate, Is.EqualTo(date));
            });
        }

        #endregion

        #region Helper Methods

        private PokemonInstance CreateTestPokemon()
        {
            return new PokemonInstance(
                _testSpecies,
                level: 50,
                maxHP: 100,
                attack: 50,
                defense: 50,
                spAttack: 50,
                spDefense: 50,
                speed: 50,
                nature: Nature.Hardy,
                gender: Gender.Male,
                moves: null);
        }

        #endregion
    }
}

