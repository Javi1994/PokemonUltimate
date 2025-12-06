using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Statistics
{
    /// <summary>
    /// Functional tests for BattleStatisticsCollector - statistics collection system.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleStatisticsCollectorTests
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
        public void Constructor_InitializesStatistics_StatisticsIsNotNull()
        {
            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats, Is.Not.Null);
        }

        [Test]
        public void GetStatistics_AfterConstruction_ReturnsEmptyStatistics()
        {
            // Act
            var stats = _collector.GetStatistics();

            // Assert
            Assert.That(stats.PlayerWins, Is.EqualTo(0));
            Assert.That(stats.EnemyWins, Is.EqualTo(0));
            Assert.That(stats.MoveUsageStats.Count, Is.EqualTo(0));
        }

        [Test]
        public void OnActionExecuted_WithMessageAction_TracksActionType()
        {
            // Arrange
            var action = new MessageAction("Test message");
            var reactions = Enumerable.Empty<BattleAction>();

            // Act
            _collector.OnActionExecuted(action, _field, reactions);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.ActionTypeStats.ContainsKey("MessageAction"), Is.True);
        }

        [Test]
        public void OnBattleStart_NotifiesObserver_StatisticsInitialized()
        {
            // Act
            _collector.OnBattleStart(_field);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats, Is.Not.Null);
        }

        [Test]
        public void OnBattleEnd_WithVictory_TracksOutcome()
        {
            // Act
            _collector.OnBattleEnd(BattleOutcome.Victory, _field);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.PlayerWins, Is.EqualTo(1));
        }

        [Test]
        public void OnBattleEnd_WithDefeat_TracksOutcome()
        {
            // Act
            _collector.OnBattleEnd(BattleOutcome.Defeat, _field);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.EnemyWins, Is.EqualTo(1));
        }

        [Test]
        public void OnTurnStart_IncrementsTurnCount()
        {
            // Act
            _collector.OnTurnStart(1, _field);
            _collector.OnTurnStart(2, _field);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.TotalTurns, Is.EqualTo(2));
        }

        [Test]
        public void Reset_ClearsAllStatistics_StatisticsReset()
        {
            // Arrange
            _collector.OnBattleEnd(BattleOutcome.Victory, _field);
            _collector.OnTurnStart(1, _field);
            var action = new MessageAction("Test");
            _collector.OnActionExecuted(action, _field, Enumerable.Empty<BattleAction>());

            // Act
            _collector.Reset();

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.PlayerWins, Is.EqualTo(0));
            Assert.That(stats.TotalTurns, Is.EqualTo(0));
            Assert.That(stats.ActionTypeStats.Count, Is.EqualTo(0));
        }

        [Test]
        public void RegisterTracker_CustomTracker_IsCalled()
        {
            // Arrange
            var customTracker = new TestStatisticsTracker();
            _collector.RegisterTracker(customTracker);
            var action = new MessageAction("Test");

            // Act
            _collector.OnActionExecuted(action, _field, Enumerable.Empty<BattleAction>());

            // Assert
            Assert.That(customTracker.WasCalled, Is.True);
        }
    }

    /// <summary>
    /// Test tracker for testing tracker registration.
    /// </summary>
    internal class TestStatisticsTracker : IStatisticsTracker
    {
        public bool WasCalled { get; private set; }

        public void TrackAction(BattleAction action, BattleField field, IEnumerable<BattleAction> reactions, BattleStatistics stats)
        {
            WasCalled = true;
        }
    }
}

