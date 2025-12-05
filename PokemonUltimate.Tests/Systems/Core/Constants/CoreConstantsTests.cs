using NUnit.Framework;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Tests.Systems.Core.Constants
{
    /// <summary>
    /// Tests for CoreConstants - centralized constants for Core module.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    [TestFixture]
    public class CoreConstantsTests
    {
        #region Shiny Constants Tests

        [Test]
        public void ShinyOdds_Should_Be_4096()
        {
            // Assert
            Assert.That(CoreConstants.ShinyOdds, Is.EqualTo(4096));
        }

        #endregion

        #region Friendship Constants Tests

        [Test]
        public void DefaultWildFriendship_Should_Be_70()
        {
            // Assert
            Assert.That(CoreConstants.DefaultWildFriendship, Is.EqualTo(70));
        }

        [Test]
        public void HatchedFriendship_Should_Be_120()
        {
            // Assert
            Assert.That(CoreConstants.HatchedFriendship, Is.EqualTo(120));
        }

        [Test]
        public void HighFriendshipThreshold_Should_Be_220()
        {
            // Assert
            Assert.That(CoreConstants.HighFriendshipThreshold, Is.EqualTo(220));
        }

        [Test]
        public void MaxFriendship_Should_Be_255()
        {
            // Assert
            Assert.That(CoreConstants.MaxFriendship, Is.EqualTo(255));
        }

        #endregion

        #region IV and EV Constants Tests

        [Test]
        public void MaxIV_Should_Be_31()
        {
            // Assert
            Assert.That(CoreConstants.MaxIV, Is.EqualTo(31));
        }

        [Test]
        public void MaxEV_Should_Be_252()
        {
            // Assert
            Assert.That(CoreConstants.MaxEV, Is.EqualTo(252));
        }

        [Test]
        public void MaxTotalEV_Should_Be_510()
        {
            // Assert
            Assert.That(CoreConstants.MaxTotalEV, Is.EqualTo(510));
        }

        [Test]
        public void DefaultIV_Should_Be_MaxIV()
        {
            // Assert
            Assert.That(CoreConstants.DefaultIV, Is.EqualTo(CoreConstants.MaxIV));
        }

        [Test]
        public void DefaultEV_Should_Be_MaxEV()
        {
            // Assert
            Assert.That(CoreConstants.DefaultEV, Is.EqualTo(CoreConstants.MaxEV));
        }

        #endregion

        #region Stat Stage Constants Tests

        [Test]
        public void MinStatStage_Should_Be_Negative6()
        {
            // Assert
            Assert.That(CoreConstants.MinStatStage, Is.EqualTo(-6));
        }

        [Test]
        public void MaxStatStage_Should_Be_6()
        {
            // Assert
            Assert.That(CoreConstants.MaxStatStage, Is.EqualTo(6));
        }

        #endregion

        #region Stat Formula Constants Tests

        [Test]
        public void StatFormulaBase_Should_Be_2()
        {
            // Assert
            Assert.That(CoreConstants.StatFormulaBase, Is.EqualTo(2));
        }

        [Test]
        public void StatFormulaDivisor_Should_Be_100()
        {
            // Assert
            Assert.That(CoreConstants.StatFormulaDivisor, Is.EqualTo(100));
        }

        [Test]
        public void StatFormulaBonus_Should_Be_5()
        {
            // Assert
            Assert.That(CoreConstants.StatFormulaBonus, Is.EqualTo(5));
        }

        [Test]
        public void HPFormulaBonus_Should_Be_10()
        {
            // Assert
            Assert.That(CoreConstants.HPFormulaBonus, Is.EqualTo(10));
        }

        [Test]
        public void EVBonusDivisor_Should_Be_4()
        {
            // Assert
            Assert.That(CoreConstants.EVBonusDivisor, Is.EqualTo(4));
        }

        #endregion
    }
}
