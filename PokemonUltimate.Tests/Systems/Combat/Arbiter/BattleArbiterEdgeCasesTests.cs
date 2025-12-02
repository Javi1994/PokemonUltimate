using System;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Arbiter
{
    /// <summary>
    /// Edge case tests for BattleArbiter - boundary conditions and invalid states.
    /// </summary>
    [TestFixture]
    public class BattleArbiterEdgeCasesTests
    {
        private BattleField _field;
        private BattleRules _rules;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
        }

        [Test]
        public void CheckOutcome_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => BattleArbiter.CheckOutcome(null));
        }

        [Test]
        public void CheckOutcome_PlayerHasMultiplePokemon_OneFainted_ReturnsOngoing()
        {
            // Arrange
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            playerParty[0].CurrentHP = 0; // First fainted
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing));
        }

        [Test]
        public void CheckOutcome_EnemyHasMultiplePokemon_OneFainted_ReturnsOngoing()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            enemyParty[0].CurrentHP = 0; // First fainted
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing));
        }

        [Test]
        public void CheckOutcome_AllPlayerPokemonFainted_ReturnsDefeat()
        {
            // Arrange
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            playerParty[0].CurrentHP = 0;
            playerParty[1].CurrentHP = 0; // All fainted
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public void CheckOutcome_AllEnemyPokemonFainted_ReturnsVictory()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            enemyParty[0].CurrentHP = 0;
            enemyParty[1].CurrentHP = 0; // All fainted
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Victory));
        }

        [Test]
        public void CheckOutcome_BothSidesAllFainted_ReturnsDraw()
        {
            // Arrange
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            playerParty[0].CurrentHP = 0;
            playerParty[1].CurrentHP = 0;
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            enemyParty[0].CurrentHP = 0;
            enemyParty[1].CurrentHP = 0;
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Draw));
        }

        [Test]
        public void CheckOutcome_DoubleBattle_OneSideDefeated_ReturnsCorrectOutcome()
        {
            // Arrange
            _rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            enemyParty[0].CurrentHP = 0;
            enemyParty[1].CurrentHP = 0; // All enemy fainted
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Victory));
        }

        [Test]
        public void CheckOutcome_EmptyParty_ReturnsOngoing()
        {
            // Arrange
            // Note: Initialize throws if party is empty, so we test with minimal party
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Clear slots but party still has Pokemon
            _field.PlayerSide.Slots[0].ClearSlot();
            _field.EnemySide.Slots[0].ClearSlot();

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert - Party still has Pokemon available, so Ongoing
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing));
        }
    }
}

