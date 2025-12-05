using NUnit.Framework;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Extensions;

namespace PokemonUltimate.Tests.Systems.Core.Extensions
{
    /// <summary>
    /// Tests for FriendshipExtensions - friendship value extension methods.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    [TestFixture]
    public class FriendshipExtensionsTests
    {
        #region ClampFriendship Tests

        [Test]
        public void ClampFriendship_ValidValue_ReturnsSameValue()
        {
            // Act & Assert
            Assert.That(0.ClampFriendship(), Is.EqualTo(0));
            Assert.That(128.ClampFriendship(), Is.EqualTo(128));
            Assert.That(255.ClampFriendship(), Is.EqualTo(255));
        }

        [Test]
        public void ClampFriendship_BelowZero_ClampsToZero()
        {
            // Act & Assert
            Assert.That((-1).ClampFriendship(), Is.EqualTo(0));
            Assert.That((-10).ClampFriendship(), Is.EqualTo(0));
        }

        [Test]
        public void ClampFriendship_AboveMax_ClampsToMax()
        {
            // Act & Assert
            Assert.That(256.ClampFriendship(), Is.EqualTo(CoreConstants.MaxFriendship));
            Assert.That(300.ClampFriendship(), Is.EqualTo(CoreConstants.MaxFriendship));
        }

        #endregion
    }
}
