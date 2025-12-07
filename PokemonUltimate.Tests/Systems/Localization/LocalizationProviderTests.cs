using NUnit.Framework;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Tests for ILocalizationProvider interface compliance.
    /// Uses a mock implementation to test interface behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/testing.md`
    /// </remarks>
    [TestFixture]
    public class LocalizationProviderTests
    {
        private MockLocalizationProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new MockLocalizationProvider();
        }

        [Test]
        public void GetString_ValidKey_ReturnsTranslatedString()
        {
            // Arrange
            _provider.SetupTranslation("test_key", "Test translation");
            _provider.CurrentLanguage = "en";

            // Act
            var result = _provider.GetString("test_key");

            // Assert
            Assert.That(result, Is.EqualTo("Test translation"));
        }

        [Test]
        public void GetString_ValidKeyWithArgs_ReturnsFormattedString()
        {
            // Arrange
            _provider.SetupTranslation("test_key", "{0} used {1}!");
            _provider.CurrentLanguage = "en";

            // Act
            var result = _provider.GetString("test_key", "Pikachu", "Thunderbolt");

            // Assert
            Assert.That(result, Is.EqualTo("Pikachu used Thunderbolt!"));
        }

        [Test]
        public void HasString_ExistingKey_ReturnsTrue()
        {
            // Arrange
            _provider.SetupTranslation("test_key", "Test");

            // Act
            var result = _provider.HasString("test_key");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasString_NonExistentKey_ReturnsFalse()
        {
            // Act
            var result = _provider.HasString("non_existent_key");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CurrentLanguage_SetValidLanguage_UpdatesLanguage()
        {
            // Arrange
            _provider.CurrentLanguage = "en";

            // Act
            _provider.CurrentLanguage = "es";

            // Assert
            Assert.That(_provider.CurrentLanguage, Is.EqualTo("es"));
        }

        [Test]
        public void GetAvailableLanguages_ReturnsAllLanguages()
        {
            // Arrange
            _provider.SetupTranslation("key1", "Test1");
            _provider.SetupTranslation("key2", "Test2");

            // Act
            var languages = _provider.GetAvailableLanguages();

            // Assert
            Assert.That(languages, Is.Not.Null);
            Assert.That(languages, Contains.Item("en"));
        }

        /// <summary>
        /// Mock implementation of ILocalizationProvider for testing.
        /// </summary>
        private class MockLocalizationProvider : ILocalizationProvider
        {
            private readonly System.Collections.Generic.Dictionary<string, string> _translations =
                new System.Collections.Generic.Dictionary<string, string>();
            public string CurrentLanguage { get; set; } = "en";

            public void SetupTranslation(string key, string translation)
            {
                _translations[key] = translation;
            }

            public string GetString(string key, params object[] args)
            {
                if (!_translations.TryGetValue(key, out var translation))
                    return key;

                if (args == null || args.Length == 0)
                    return translation;

                return string.Format(translation, args);
            }

            public bool HasString(string key)
            {
                return _translations.ContainsKey(key);
            }

            public System.Collections.Generic.IEnumerable<string> GetAvailableLanguages()
            {
                return new[] { "en" };
            }
        }
    }
}
