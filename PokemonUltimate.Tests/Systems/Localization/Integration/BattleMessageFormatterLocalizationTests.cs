using NUnit.Framework;
using PokemonUltimate.Combat.Messages;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization.Integration
{
    /// <summary>
    /// Integration tests for BattleMessageFormatter with LocalizationProvider.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/testing.md`
    /// </remarks>
    [TestFixture]
    public class BattleMessageFormatterLocalizationTests
    {
        private ILocalizationProvider _localizationProvider;
        private BattleMessageFormatter _formatter;
        private PokemonInstance _pokemon;
        private MoveData _move;

        [SetUp]
        public void SetUp()
        {
            _localizationProvider = new LocalizationProvider();
            _formatter = new BattleMessageFormatter(_localizationProvider);

            // Create test Pokemon and Move
            var species = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 30, 50, 40, 90)
            };
            _pokemon = PokemonFactory.Create(species, 50);

            _move = new MoveData
            {
                Name = "Thunderbolt",
                Type = PokemonType.Electric
            };
        }

        [Test]
        public void BattleMessageFormatter_UsesLocalizationProvider_FormatsCorrectly()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";

            // Act
            var result = _formatter.FormatMoveUsed(_pokemon, _move);

            // Assert
            Assert.That(result, Is.EqualTo("Pikachu used Thunderbolt!"));
        }

        [Test]
        public void BattleMessageFormatter_FormatMoveUsed_ReturnsLocalizedMessage()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";

            // Act
            var result = _formatter.FormatMoveUsed(_pokemon, _move);

            // Assert
            Assert.That(result, Is.EqualTo("¡Pikachu usó Thunderbolt!"));
        }

        [Test]
        public void BattleMessageFormatter_FormatMoveUsedDuringSwitch_ReturnsLocalizedMessage()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";
            var charizardSpecies = new PokemonSpeciesData
            {
                Name = "Charizard",
                PrimaryType = PokemonType.Fire,
                BaseStats = new BaseStats(78, 84, 78, 109, 85, 100)
            };
            var switchingPokemon = PokemonFactory.Create(charizardSpecies, 50);

            // Act
            var result = _formatter.FormatMoveUsedDuringSwitch(switchingPokemon, _pokemon, _move);

            // Assert
            Assert.That(result, Contains.Substring("Charizard is switching out!"));
            Assert.That(result, Contains.Substring("Pikachu used Thunderbolt!"));
        }

        [Test]
        public void BattleMessageFormatter_Format_WithLocalizationKey_ReturnsLocalizedMessage()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";

            // Act
            var result = _formatter.Format(LocalizationKey.BattleMissed);

            // Assert
            Assert.That(result, Is.EqualTo("The attack missed!"));
        }

        [Test]
        public void BattleMessageFormatter_Format_WithLocalizationKeyAndArgs_ReturnsFormattedMessage()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "en";

            // Act
            var result = _formatter.Format(LocalizationKey.BattleFlinched, "Pikachu");

            // Assert
            Assert.That(result, Is.EqualTo("Pikachu flinched and couldn't move!"));
        }

        [Test]
        public void BattleMessageFormatter_Format_WithLocalizationKeySpanish_ReturnsSpanishMessage()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "es";

            // Act
            var result = _formatter.Format(LocalizationKey.BattleMissed);

            // Assert
            Assert.That(result, Is.EqualTo("¡El ataque falló!"));
        }

        [Test]
        public void BattleMessageFormatter_Format_WithLocalizationKeyFrench_ReturnsFrenchMessage()
        {
            // Arrange
            _localizationProvider.CurrentLanguage = "fr";

            // Act
            var result = _formatter.Format(LocalizationKey.BattleMissed);

            // Assert
            Assert.That(result, Is.EqualTo("L'attaque a échoué!"));
        }

        [Test]
        public void BattleMessageFormatter_Constructor_WithNullLocalizationProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => new BattleMessageFormatter(null));
        }
    }
}
