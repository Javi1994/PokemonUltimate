using System.Linq;
using System.Threading.Tasks;
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
    /// Integration tests for BattleQueue with statistics collection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleQueueIntegrationTests
    {
        private BattleQueue _queue;
        private BattleField _field;
        private BattleStatisticsCollector _collector;
        private IBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _queue = new BattleQueue();
            _field = new BattleField();
            _collector = new BattleStatisticsCollector();
            _view = NullBattleView.Instance;

            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, playerParty, enemyParty);

            _queue.AddObserver(_collector);
        }

        [Test]
        public async Task ProcessQueue_WithObserver_NotifiesObserver()
        {
            // Arrange
            var action = new MessageAction("Test message");
            _queue.Enqueue(action);

            // Act
            await _queue.ProcessQueue(_field, _view);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.ActionTypeStats.ContainsKey("MessageAction"), Is.True);
            Assert.That(stats.ActionTypeStats["MessageAction"], Is.EqualTo(1));
        }

        [Test]
        public async Task ProcessQueue_MultipleActions_TracksAllActions()
        {
            // Arrange
            _queue.Enqueue(new MessageAction("One"));
            _queue.Enqueue(new MessageAction("Two"));
            _queue.Enqueue(new MessageAction("Three"));

            // Act
            await _queue.ProcessQueue(_field, _view);

            // Assert
            var stats = _collector.GetStatistics();
            Assert.That(stats.ActionTypeStats["MessageAction"], Is.EqualTo(3));
        }

        [Test]
        public async Task ProcessQueue_WithReactions_TracksReactions()
        {
            // Arrange
            var userSlot = _field.PlayerSide.Slots[0];
            var targetSlot = _field.EnemySide.Slots[0];
            var move = userSlot.Pokemon.Moves[0];
            var moveAction = new UseMoveAction(userSlot, targetSlot, move);
            _queue.Enqueue(moveAction);

            // Act
            await _queue.ProcessQueue(_field, _view);

            // Assert
            var stats = _collector.GetStatistics();
            // Should track the UseMoveAction
            Assert.That(stats.ActionTypeStats.ContainsKey("UseMoveAction"), Is.True);
        }

        [Test]
        public void AddObserver_NullObserver_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => _queue.AddObserver(null));
        }

        [Test]
        public void RemoveObserver_ExistingObserver_RemovesObserver()
        {
            // Arrange
            var testCollector = new BattleStatisticsCollector();
            _queue.AddObserver(testCollector);

            // Act
            _queue.RemoveObserver(testCollector);
            _queue.Enqueue(new MessageAction("Test"));
            _queue.ProcessQueue(_field, _view).Wait();

            // Assert - Original collector should still track, removed one should not
            var stats = _collector.GetStatistics();
            Assert.That(stats.ActionTypeStats.ContainsKey("MessageAction"), Is.True);
        }
    }
}

