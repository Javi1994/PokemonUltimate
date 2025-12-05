using NUnit.Framework;
using PokemonUltimate.Combat.Logging;

namespace PokemonUltimate.Tests.Systems.Combat.Logging
{
    /// <summary>
    /// Tests for BattleLogger - default implementation of IBattleLogger.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleLoggerTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_CreatesInstance()
        {
            // Act
            var logger = new BattleLogger();

            // Assert
            Assert.That(logger, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithContext_CreatesInstance()
        {
            // Act
            var logger = new BattleLogger("TestContext");

            // Assert
            Assert.That(logger, Is.Not.Null);
        }

        #endregion

        #region LogDebug Tests

        [Test]
        public void LogDebug_WithMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogDebug("Test debug message"));
        }

        [Test]
        public void LogDebug_NullMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogDebug(null));
        }

        #endregion

        #region LogInfo Tests

        [Test]
        public void LogInfo_WithMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogInfo("Test info message"));
        }

        [Test]
        public void LogInfo_NullMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogInfo(null));
        }

        #endregion

        #region LogWarning Tests

        [Test]
        public void LogWarning_WithMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogWarning("Test warning message"));
        }

        [Test]
        public void LogWarning_NullMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogWarning(null));
        }

        #endregion

        #region LogError Tests

        [Test]
        public void LogError_WithMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogError("Test error message"));
        }

        [Test]
        public void LogError_NullMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogError(null));
        }

        #endregion

        #region LogBattleEvent Tests

        [Test]
        public void LogBattleEvent_WithEventTypeAndMessage_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogBattleEvent("TurnStart", "Turn 1 started"));
        }

        [Test]
        public void LogBattleEvent_NullParameters_DoesNotThrow()
        {
            // Arrange
            var logger = new BattleLogger();

            // Act & Assert
            Assert.DoesNotThrow(() => logger.LogBattleEvent(null, null));
        }

        #endregion
    }
}
