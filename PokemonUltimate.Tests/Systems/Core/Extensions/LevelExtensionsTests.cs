using NUnit.Framework;
using PokemonUltimate.Core.Extensions;

namespace PokemonUltimate.Tests.Systems.Core.Extensions
{
    /// <summary>
    /// Tests for LevelExtensions - level validation extension methods.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    [TestFixture]
    public class LevelExtensionsTests
    {
        #region IsValidLevel Tests

        [Test]
        public void IsValidLevel_ValidLevel_ReturnsTrue()
        {
            // Act & Assert
            Assert.That(1.IsValidLevel(), Is.True);
            Assert.That(50.IsValidLevel(), Is.True);
            Assert.That(100.IsValidLevel(), Is.True);
        }

        [Test]
        public void IsValidLevel_LevelBelowOne_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(0.IsValidLevel(), Is.False);
            Assert.That((-1).IsValidLevel(), Is.False);
        }

        [Test]
        public void IsValidLevel_LevelAbove100_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(101.IsValidLevel(), Is.False);
            Assert.That(200.IsValidLevel(), Is.False);
        }

        #endregion
    }
}
