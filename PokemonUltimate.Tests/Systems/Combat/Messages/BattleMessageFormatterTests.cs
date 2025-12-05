using System;
using NUnit.Framework;
using PokemonUltimate.Combat.Messages;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Messages
{
    /// <summary>
    /// Tests for BattleMessageFormatter - formats battle messages.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleMessageFormatterTests
    {
        private BattleMessageFormatter _formatter;
        private PokemonInstance _pokemon;
        private MoveData _move;

        [SetUp]
        public void SetUp()
        {
            _formatter = new BattleMessageFormatter();
            _pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            _move = MoveCatalog.Thunderbolt;
        }

        #region FormatMoveUsed Tests

        [Test]
        public void FormatMoveUsed_ValidParameters_ReturnsFormattedMessage()
        {
            // Act
            var message = _formatter.FormatMoveUsed(_pokemon, _move);

            // Assert
            Assert.That(message, Is.EqualTo($"{_pokemon.DisplayName} used {_move.Name}!"));
        }

        [Test]
        public void FormatMoveUsed_NullPokemon_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _formatter.FormatMoveUsed(null, _move));
        }

        [Test]
        public void FormatMoveUsed_NullMove_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _formatter.FormatMoveUsed(_pokemon, null));
        }

        #endregion

        #region FormatMoveUsedDuringSwitch Tests

        [Test]
        public void FormatMoveUsedDuringSwitch_ValidParameters_ReturnsFormattedMessage()
        {
            // Arrange
            var switchingPokemon = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);

            // Act
            var message = _formatter.FormatMoveUsedDuringSwitch(switchingPokemon, _pokemon, _move);

            // Assert
            Assert.That(message, Does.Contain(switchingPokemon.DisplayName));
            Assert.That(message, Does.Contain("switching out"));
            Assert.That(message, Does.Contain(_pokemon.DisplayName));
            Assert.That(message, Does.Contain(_move.Name));
        }

        [Test]
        public void FormatMoveUsedDuringSwitch_NullSwitchingPokemon_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _formatter.FormatMoveUsedDuringSwitch(null, _pokemon, _move));
        }

        [Test]
        public void FormatMoveUsedDuringSwitch_NullAttackingPokemon_ThrowsArgumentNullException()
        {
            // Arrange
            var switchingPokemon = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _formatter.FormatMoveUsedDuringSwitch(switchingPokemon, null, _move));
        }

        [Test]
        public void FormatMoveUsedDuringSwitch_NullMove_ThrowsArgumentNullException()
        {
            // Arrange
            var switchingPokemon = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _formatter.FormatMoveUsedDuringSwitch(switchingPokemon, _pokemon, null));
        }

        #endregion

        #region Format Tests

        [Test]
        public void Format_SimpleTemplate_ReturnsTemplate()
        {
            // Arrange
            const string template = "Test message";

            // Act
            var message = _formatter.Format(template);

            // Assert
            Assert.That(message, Is.EqualTo(template));
        }

        [Test]
        public void Format_TemplateWithArgs_FormatsCorrectly()
        {
            // Arrange
            const string template = "Pokemon {0} used {1}!";

            // Act
            var message = _formatter.Format(template, "Pikachu", "Thunderbolt");

            // Assert
            Assert.That(message, Is.EqualTo("Pokemon Pikachu used Thunderbolt!"));
        }

        [Test]
        public void Format_NullTemplate_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _formatter.Format(null));
        }

        [Test]
        public void Format_TemplateWithNullArgs_ReturnsTemplate()
        {
            // Arrange
            const string template = "Test message";

            // Act
            var message = _formatter.Format(template, null);

            // Assert
            Assert.That(message, Is.EqualTo(template));
        }

        [Test]
        public void Format_TemplateWithEmptyArgs_ReturnsTemplate()
        {
            // Arrange
            const string template = "Test message";

            // Act
            var message = _formatter.Format(template, new object[0]);

            // Assert
            Assert.That(message, Is.EqualTo(template));
        }

        #endregion
    }
}
