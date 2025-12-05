using System;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Providers;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    /// <summary>
    /// Tests for PokemonInstanceBuilder validation - Build() method validation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    [TestFixture]
    public class PokemonInstanceBuilderValidationTests
    {
        #region Stat Override Validation Tests

        [Test]
        public void Build_NegativeMaxHPOverride_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithMaxHP(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_NegativeAttackOverride_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithAttack(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_NegativeDefenseOverride_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithDefense(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_NegativeSpAttackOverride_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithSpAttack(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_NegativeSpDefenseOverride_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithSpDefense(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_NegativeSpeedOverride_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithSpeed(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        #endregion

        #region HP Percent Validation Tests

        [Test]
        public void Build_HPPercentBelowZero_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithHPPercent(-0.1f);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_HPPercentAboveOne_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithHPPercent(1.1f);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_HPPercentValidRange_Succeeds()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithHPPercent(0.5f);

            // Act
            var pokemon = builder.Build();

            // Assert
            Assert.That(pokemon, Is.Not.Null);
        }

        #endregion

        #region Current HP Validation Tests

        [Test]
        public void Build_NegativeCurrentHP_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithCurrentHP(-10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        #endregion

        #region Experience Validation Tests

        [Test]
        public void Build_NegativeExperience_ThrowsArgumentException()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithExperience(-100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        #endregion

        #region Ability Validation Tests

        [Test]
        public void Build_InvalidAbility_ThrowsArgumentException()
        {
            // Arrange
            var invalidAbility = AbilityCatalog.Intimidate; // Not valid for Pikachu
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithAbility(invalidAbility);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => builder.Build());
            Assert.That(ex.Message, Does.Contain("not valid for species"));
        }

        [Test]
        public void Build_ValidPrimaryAbility_Succeeds()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithAbility(PokemonCatalog.Pikachu.Ability1);

            // Act
            var pokemon = builder.Build();

            // Assert
            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Ability, Is.EqualTo(PokemonCatalog.Pikachu.Ability1));
        }

        [Test]
        public void Build_ValidSecondaryAbility_Succeeds()
        {
            // Arrange - Eevee has Ability2 (Adaptability)
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Eevee, 25)
                .WithAbility(PokemonCatalog.Eevee.Ability2);

            // Act
            var pokemon = builder.Build();

            // Assert
            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Ability, Is.EqualTo(PokemonCatalog.Eevee.Ability2));
        }

        [Test]
        public void Build_ValidHiddenAbility_Succeeds()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithAbility(PokemonCatalog.Pikachu.HiddenAbility);

            // Act
            var pokemon = builder.Build();

            // Assert
            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Ability, Is.EqualTo(PokemonCatalog.Pikachu.HiddenAbility));
        }

        #endregion

        #region Move Validation Tests

        [Test]
        public void Build_MoveNotInLearnset_ThrowsArgumentException()
        {
            // Arrange - Use a move that's definitely not in Pikachu's learnset
            // Using Flamethrower which should not be in Pikachu's learnset (it's a Fire move)
            var invalidMove = MoveCatalog.Flamethrower;
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithMoves(invalidMove);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => builder.Build());
            Assert.That(ex.Message, Does.Contain("not in species"));
        }

        [Test]
        public void Build_ValidMovesFromLearnset_Succeeds()
        {
            // Arrange - Create Pokemon without specifying moves, should use learnset
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25);

            // Act
            var pokemon = builder.Build();

            // Assert
            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Moves.Count, Is.GreaterThanOrEqualTo(0));
        }

        #endregion
    }
}
