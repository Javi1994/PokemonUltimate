using NUnit.Framework;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Tests for LocalizationDataProvider - provides localization data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/testing.md`
    /// </remarks>
    [TestFixture]
    public class LocalizationDataProviderTests
    {
        [Test]
        public void GetData_ExistingKey_ReturnsLocalizationData()
        {
            // Arrange
            var key = LocalizationKey.BattleUsedMove;

            // Act
            var data = LocalizationDataProvider.GetData(key);

            // Assert
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Key, Is.EqualTo(key));
            Assert.That(data.Translations, Is.Not.Null);
            Assert.That(data.Translations.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GetData_ExistingKey_HasEnglishTranslation()
        {
            // Arrange
            var key = LocalizationKey.BattleUsedMove;

            // Act
            var data = LocalizationDataProvider.GetData(key);

            // Assert
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Translations.ContainsKey("en"), Is.True);
            Assert.That(data.Translations["en"], Is.Not.Empty);
        }

        [Test]
        public void GetData_NonExistentKey_ReturnsNull()
        {
            // Arrange
            var key = "non_existent_key";

            // Act
            var data = LocalizationDataProvider.GetData(key);

            // Assert
            Assert.That(data, Is.Null);
        }

        [Test]
        public void GetData_NullKey_ReturnsNull()
        {
            // Act
            var data = LocalizationDataProvider.GetData(null);

            // Assert
            Assert.That(data, Is.Null);
        }

        [Test]
        public void GetData_EmptyKey_ReturnsNull()
        {
            // Act
            var data = LocalizationDataProvider.GetData(string.Empty);

            // Assert
            Assert.That(data, Is.Null);
        }

        [Test]
        public void HasData_ExistingKey_ReturnsTrue()
        {
            // Arrange
            var key = LocalizationKey.BattleUsedMove;

            // Act
            var result = LocalizationDataProvider.HasData(key);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasData_NonExistentKey_ReturnsFalse()
        {
            // Arrange
            var key = "non_existent_key";

            // Act
            var result = LocalizationDataProvider.HasData(key);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasData_NullKey_ReturnsFalse()
        {
            // Act
            var result = LocalizationDataProvider.HasData(null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasData_EmptyKey_ReturnsFalse()
        {
            // Act
            var result = LocalizationDataProvider.HasData(string.Empty);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetData_BattleKeys_AllExist()
        {
            // Arrange
            var keys = new[]
            {
                LocalizationKey.BattleUsedMove,
                LocalizationKey.BattleMissed,
                LocalizationKey.BattleFlinched,
                LocalizationKey.BattleProtected,
                LocalizationKey.BattleNoPP,
                LocalizationKey.BattleAsleep,
                LocalizationKey.BattleFrozen,
                LocalizationKey.BattleParalyzed
            };

            // Act & Assert
            foreach (var key in keys)
            {
                var data = LocalizationDataProvider.GetData(key);
                Assert.That(data, Is.Not.Null, $"Key {key} should exist");
                Assert.That(data.Translations.ContainsKey("en"), Is.True, $"Key {key} should have English translation");
            }
        }
    }
}
