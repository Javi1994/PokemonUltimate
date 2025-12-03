using System;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Arbiter
{
    /// <summary>
    /// Functional tests for BattleArbiter - victory/defeat detection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleArbiterTests
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
        public void CheckOutcome_BothAlive_ReturnsOngoing()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing));
        }

        [Test]
        public void CheckOutcome_EnemyDefeated_ReturnsVictory()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyPokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 50);
            enemyPokemon.CurrentHP = 0; // Fainted
            var enemyParty = new[] { enemyPokemon };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Victory));
        }

        [Test]
        public void CheckOutcome_PlayerDefeated_ReturnsDefeat()
        {
            // Arrange
            var playerPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            playerPokemon.CurrentHP = 0; // Fainted
            var playerParty = new[] { playerPokemon };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public void CheckOutcome_BothDefeated_ReturnsDraw()
        {
            // Arrange
            var playerPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            playerPokemon.CurrentHP = 0; // Fainted
            var playerParty = new[] { playerPokemon };
            var enemyPokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 50);
            enemyPokemon.CurrentHP = 0; // Fainted
            var enemyParty = new[] { enemyPokemon };
            _field.Initialize(_rules, playerParty, enemyParty);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Draw));
        }

        [Test]
        public void CheckOutcome_EmptySlots_ReturnsOngoing()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            _field.Initialize(_rules, playerParty, enemyParty);
            
            // Clear slots (simulating no Pokemon sent out yet)
            _field.PlayerSide.Slots[0].ClearSlot();
            _field.EnemySide.Slots[0].ClearSlot();

            // Act
            var outcome = BattleArbiter.CheckOutcome(_field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing));
        }

        [Test]
        public void CheckOutcome_NullField_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => BattleArbiter.CheckOutcome(null));
        }
    }
}

