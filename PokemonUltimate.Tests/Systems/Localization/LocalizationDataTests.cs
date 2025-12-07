using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Tests for LocalizationData - data structure for storing translations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/testing.md`
    /// </remarks>
    [TestFixture]
    public class LocalizationDataTests
    {
        [Test]
        public void Constructor_Default_InitializesEmptyTranslations()
        {
            // Arrange & Act
            var data = new LocalizationData();

            // Assert
            Assert.That(data.Key, Is.Null);
            Assert.That(data.Translations, Is.Not.Null);
            Assert.That(data.Translations.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithParameters_SetsKeyAndTranslations()
        {
            // Arrange
            var key = "test_key";
            var translations = new Dictionary<string, string>
            {
                { "en", "English text" },
                { "es", "Texto en español" }
            };

            // Act
            var data = new LocalizationData(key, translations);

            // Assert
            Assert.That(data.Key, Is.EqualTo(key));
            Assert.That(data.Translations, Is.EqualTo(translations));
            Assert.That(data.Translations.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WithNullTranslations_InitializesEmptyDictionary()
        {
            // Arrange
            var key = "test_key";

            // Act
            var data = new LocalizationData(key, null);

            // Assert
            Assert.That(data.Key, Is.EqualTo(key));
            Assert.That(data.Translations, Is.Not.Null);
            Assert.That(data.Translations.Count, Is.EqualTo(0));
        }
    }
}
