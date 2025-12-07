using NUnit.Framework;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Tests.Systems.Localization
{
    /// <summary>
    /// Tests for LocalizationKey - constants for localization keys.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/testing.md`
    /// </remarks>
    [TestFixture]
    public class LocalizationKeyTests
    {
        [Test]
        public void BattleUsedMove_IsNotNull()
        {
            // Assert
            Assert.That(LocalizationKey.BattleUsedMove, Is.Not.Null);
            Assert.That(LocalizationKey.BattleUsedMove, Is.Not.Empty);
        }

        [Test]
        public void BattleMissed_IsNotNull()
        {
            // Assert
            Assert.That(LocalizationKey.BattleMissed, Is.Not.Null);
            Assert.That(LocalizationKey.BattleMissed, Is.Not.Empty);
        }

        [Test]
        public void TypeNoEffect_IsNotNull()
        {
            // Assert
            Assert.That(LocalizationKey.TypeNoEffect, Is.Not.Null);
            Assert.That(LocalizationKey.TypeNoEffect, Is.Not.Empty);
        }

        [Test]
        public void TypeSuperEffective_IsNotNull()
        {
            // Assert
            Assert.That(LocalizationKey.TypeSuperEffective, Is.Not.Null);
            Assert.That(LocalizationKey.TypeSuperEffective, Is.Not.Empty);
        }

        [Test]
        public void Keys_AreUnique()
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
                LocalizationKey.BattleParalyzed,
                LocalizationKey.TypeNoEffect,
                LocalizationKey.TypeSuperEffective,
                LocalizationKey.TypeSuperEffective4x,
                LocalizationKey.TypeNotVeryEffective,
                LocalizationKey.TypeNotVeryEffective025x
            };

            // Act & Assert
            for (int i = 0; i < keys.Length; i++)
            {
                for (int j = i + 1; j < keys.Length; j++)
                {
                    Assert.That(keys[i], Is.Not.EqualTo(keys[j]),
                        $"Keys at indices {i} and {j} should be unique");
                }
            }
        }
    }
}
