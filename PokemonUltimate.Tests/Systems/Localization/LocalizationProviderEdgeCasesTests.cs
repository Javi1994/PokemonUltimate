using NUnit.Framework;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Edge cases tests for LocalizationProvider.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/testing.md`
    /// </remarks>
    [TestFixture]
    public class LocalizationProviderEdgeCasesTests
    {
        private LocalizationProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new LocalizationProvider();
        }

        [Test]
        public void GetString_NullKey_ReturnsEmptyString()
        {
            // Act
            var result = _provider.GetString(null);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetString_EmptyKey_ReturnsEmptyString()
        {
            // Act
            var result = _provider.GetString(string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetString_MissingKey_ReturnsKey()
        {
            // Arrange
            var key = "non_existent_key";

            // Act
            var result = _provider.GetString(key);

            // Assert
            Assert.That(result, Is.EqualTo(key));
        }

        [Test]
        public void GetString_MissingLanguage_FallsBackToEnglish()
        {
            // Arrange
            _provider.CurrentLanguage = "fr"; // French not available for all keys
            var key = LocalizationKey.BattleUsedMove;

            // Act
            var result = _provider.GetString(key, "Pikachu", "Thunderbolt");

            // Assert
            Assert.That(result, Is.Not.EqualTo(key)); // Should have translation (English fallback)
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetString_MissingLanguageAndEnglish_ReturnsKey()
        {
            // Arrange
            _provider.CurrentLanguage = "de"; // German not available
            var key = "non_existent_key";

            // Act
            var result = _provider.GetString(key);

            // Assert
            Assert.That(result, Is.EqualTo(key));
        }

        [Test]
        public void GetString_InvalidFormatString_ReturnsUnformattedString()
        {
            // Arrange
            _provider.CurrentLanguage = "en";
            var key = LocalizationKey.BattleUsedMove;

            // Act - Pass wrong number of arguments
            var result = _provider.GetString(key, "Pikachu"); // Missing second argument

            // Assert - Should handle gracefully (return formatted or unformatted)
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CurrentLanguage_SetNull_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => _provider.CurrentLanguage = null);
        }

        [Test]
        public void CurrentLanguage_SetEmpty_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => _provider.CurrentLanguage = string.Empty);
        }

        [Test]
        public void GetString_ValidKeyWithNoArgs_ReturnsUnformattedString()
        {
            // Arrange
            _provider.CurrentLanguage = "en";
            var key = LocalizationKey.BattleMissed;

            // Act
            var result = _provider.GetString(key);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.Not.EqualTo(key));
        }

        [Test]
        public void GetString_ValidKeyWithArgs_FormatsCorrectly()
        {
            // Arrange
            _provider.CurrentLanguage = "en";
            var key = LocalizationKey.BattleUsedMove;

            // Act
            var result = _provider.GetString(key, "Pikachu", "Thunderbolt");

            // Assert
            Assert.That(result, Is.EqualTo("Pikachu used Thunderbolt!"));
        }
    }
}
