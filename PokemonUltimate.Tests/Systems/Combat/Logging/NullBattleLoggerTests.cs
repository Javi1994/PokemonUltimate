using NUnit.Framework;
using PokemonUltimate.Combat.Logging;

namespace PokemonUltimate.Tests.Systems.Combat.Logging
{
    /// <summary>
    /// Tests for NullBattleLogger - null object implementation that discards all log messages.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class NullBattleLoggerTests
    {
        private NullBattleLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new NullBattleLogger();
        }

        #region LogDebug Tests

        [Test]
        public void LogDebug_WithMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogDebug("Test debug message"));
        }

        [Test]
        public void LogDebug_NullMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogDebug(null));
        }

        #endregion

        #region LogInfo Tests

        [Test]
        public void LogInfo_WithMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogInfo("Test info message"));
        }

        [Test]
        public void LogInfo_NullMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogInfo(null));
        }

        #endregion

        #region LogWarning Tests

        [Test]
        public void LogWarning_WithMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogWarning("Test warning message"));
        }

        [Test]
        public void LogWarning_NullMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogWarning(null));
        }

        #endregion

        #region LogError Tests

        [Test]
        public void LogError_WithMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogError("Test error message"));
        }

        [Test]
        public void LogError_NullMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogError(null));
        }

        #endregion

        #region LogBattleEvent Tests

        [Test]
        public void LogBattleEvent_WithEventTypeAndMessage_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogBattleEvent("TurnStart", "Turn 1 started"));
        }

        [Test]
        public void LogBattleEvent_NullParameters_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _logger.LogBattleEvent(null, null));
        }

        #endregion
    }
}
