using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Statistics
{
    /// <summary>
    /// Edge cases tests for BattleStatisticsCollector.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleStatisticsEdgeCasesTests
    {
        private BattleStatisticsCollector _collector;
        private BattleField _field;

        [SetUp]
        public void SetUp()
        {
            _collector = new BattleStatisticsCollector();
            _field = new BattleField();
            
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, playerParty, enemyParty);
        }

        [Test]
        public void OnActionExecuted_NullAction_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _collector.OnActionExecuted(null, _field, Enumerable.Empty<BattleAction>()));
        }

        [Test]
        public void OnActionExecuted_NullField_DoesNotThrow()
        {
            // Arrange
            var action = new MessageAction("Test");

            // Act & Assert
            Assert.DoesNotThrow(() => _collector.OnActionExecuted(action, null, Enumerable.Empty<BattleAction>()));
        }

        [Test]
        public void OnActionExecuted_NullReactions_HandlesGracefully()
        {
            // Arrange
            var action = new MessageAction("Test");

            // Act
            _collector.OnActionExecuted(action, _field, null);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.ActionTypeStats.ContainsKey("MessageAction"), Is.True);
        }

        [Test]
        public void OnTurnStart_NullField_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _collector.OnTurnStart(1, null));
        }

        [Test]
        public void OnTurnEnd_NullField_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _collector.OnTurnEnd(1, null));
        }

        [Test]
        public void OnBattleStart_NullField_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _collector.OnBattleStart(null));
        }

        [Test]
        public void OnBattleEnd_NullField_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _collector.OnBattleEnd(BattleOutcome.Victory, null));
        }

        [Test]
        public void RegisterTracker_NullTracker_DoesNotAdd()
        {
            // Arrange
            var initialTrackerCount = _collector.GetStatistics().ActionTypeStats.Count;

            // Act
            _collector.RegisterTracker(null);
            var action = new MessageAction("Test");
            _collector.OnActionExecuted(action, _field, Enumerable.Empty<BattleAction>());

            // Assert - Should still work normally
            var stats = _collector.GetStatistics();
            Assert.That(stats.ActionTypeStats.ContainsKey("MessageAction"), Is.True);
        }

        [Test]
        public void Reset_MultipleTimes_StatisticsRemainEmpty()
        {
            // Arrange
            _collector.OnBattleEnd(BattleOutcome.Victory, _field);
            _collector.OnTurnStart(1, _field);
            var action = new MessageAction("Test");
            _collector.OnActionExecuted(action, _field, Enumerable.Empty<BattleAction>());

            // Act
            _collector.Reset();
            _collector.Reset();
            _collector.Reset();

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.PlayerWins, Is.EqualTo(0));
            Assert.That(stats.TotalTurns, Is.EqualTo(0));
            Assert.That(stats.ActionTypeStats.Count, Is.EqualTo(0));
        }

        [Test]
        public void OnBattleStart_ResetsStatistics()
        {
            // Arrange
            _collector.OnBattleEnd(BattleOutcome.Victory, _field);
            var stats1 = _collector.GetStatistics();
            Assert.That(stats1.PlayerWins, Is.EqualTo(1));

            // Act
            _collector.OnBattleStart(_field);

            // Assert
            var stats2 = _collector.GetStatistics();
            Assert.That(stats2.PlayerWins, Is.EqualTo(0));
        }

        [Test]
        public void MultipleObservers_SameAction_TracksCorrectly()
        {
            // Arrange
            var collector2 = new BattleStatisticsCollector();
            var queue = new BattleQueue();
            queue.AddObserver(_collector);
            queue.AddObserver(collector2);
            var action = new MessageAction("Test");
            queue.Enqueue(action);

            // Act
            queue.ProcessQueue(_field, NullBattleView.Instance).Wait();

            // Assert
            var stats1 = _collector.GetStatistics();
            var stats2 = collector2.GetStatistics();
            Assert.That(stats1.ActionTypeStats["MessageAction"], Is.EqualTo(1));
            Assert.That(stats2.ActionTypeStats["MessageAction"], Is.EqualTo(1));
        }

        [Test]
        public void GetStatistics_MultipleCalls_ReturnsSameInstance()
        {
            // Act
            var stats1 = _collector.GetStatistics();
            var stats2 = _collector.GetStatistics();

            // Assert
            Assert.That(stats1, Is.SameAs(stats2));
        }

        [Test]
        public void OnBattleEnd_Draw_TracksDraw()
        {
            // Act
            _collector.OnBattleEnd(BattleOutcome.Draw, _field);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.Draws, Is.EqualTo(1));
            Assert.That(stats.PlayerWins, Is.EqualTo(0));
            Assert.That(stats.EnemyWins, Is.EqualTo(0));
        }

        [Test]
        public void OnTurnStart_MultipleTurns_TracksTotalTurns()
        {
            // Act
            _collector.OnTurnStart(1, _field);
            _collector.OnTurnStart(2, _field);
            _collector.OnTurnStart(3, _field);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.TotalTurns, Is.EqualTo(3));
        }
    }
}

