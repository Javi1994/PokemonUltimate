using System;
using NUnit.Framework;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Tests.Systems.Core.Constants
{
    /// <summary>
    /// Tests for CoreValidators - centralized validation methods.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    [TestFixture]
    public class CoreValidatorsTests
    {
        #region ValidateLevel Tests

        [Test]
        public void ValidateLevel_ValidLevel_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => CoreValidators.ValidateLevel(1));
            Assert.DoesNotThrow(() => CoreValidators.ValidateLevel(50));
            Assert.DoesNotThrow(() => CoreValidators.ValidateLevel(100));
        }

        [Test]
        public void ValidateLevel_LevelBelowOne_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateLevel(0));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateLevel(-1));
        }

        [Test]
        public void ValidateLevel_LevelAbove100_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateLevel(101));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateLevel(200));
        }

        #endregion

        #region ValidateFriendship Tests

        [Test]
        public void ValidateFriendship_ValidFriendship_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => CoreValidators.ValidateFriendship(0));
            Assert.DoesNotThrow(() => CoreValidators.ValidateFriendship(128));
            Assert.DoesNotThrow(() => CoreValidators.ValidateFriendship(255));
        }

        [Test]
        public void ValidateFriendship_FriendshipBelowZero_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateFriendship(-1));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateFriendship(-10));
        }

        [Test]
        public void ValidateFriendship_FriendshipAbove255_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateFriendship(256));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateFriendship(300));
        }

        #endregion

        #region ValidateStatStage Tests

        [Test]
        public void ValidateStatStage_ValidStage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => CoreValidators.ValidateStatStage(-6));
            Assert.DoesNotThrow(() => CoreValidators.ValidateStatStage(0));
            Assert.DoesNotThrow(() => CoreValidators.ValidateStatStage(6));
        }

        [Test]
        public void ValidateStatStage_StageBelowMin_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateStatStage(-7));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateStatStage(-10));
        }

        [Test]
        public void ValidateStatStage_StageAboveMax_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateStatStage(7));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateStatStage(10));
        }

        #endregion

        #region ValidateIV Tests

        [Test]
        public void ValidateIV_ValidIV_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => CoreValidators.ValidateIV(0));
            Assert.DoesNotThrow(() => CoreValidators.ValidateIV(16));
            Assert.DoesNotThrow(() => CoreValidators.ValidateIV(31));
        }

        [Test]
        public void ValidateIV_IVBelowZero_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateIV(-1));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateIV(-10));
        }

        [Test]
        public void ValidateIV_IVAboveMax_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateIV(32));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateIV(100));
        }

        #endregion

        #region ValidateEV Tests

        [Test]
        public void ValidateEV_ValidEV_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => CoreValidators.ValidateEV(0));
            Assert.DoesNotThrow(() => CoreValidators.ValidateEV(128));
            Assert.DoesNotThrow(() => CoreValidators.ValidateEV(252));
        }

        [Test]
        public void ValidateEV_EVBelowZero_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateEV(-1));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateEV(-10));
        }

        [Test]
        public void ValidateEV_EVAboveMax_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateEV(253));
            Assert.Throws<ArgumentException>(() => CoreValidators.ValidateEV(300));
        }

        #endregion
    }
}
