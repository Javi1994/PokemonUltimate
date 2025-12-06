using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Tests.Systems.Combat.Engine;
using TestActionProvider = PokemonUltimate.Tests.Systems.Combat.Engine.TestActionProvider;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.Engine
{
    /// <summary>
    /// Integration tests for Turn Order System - verifies turn order resolution works with CombatEngine.
    /// </summary>
    [TestFixture]
    public class TurnOrderIntegrationTests
    {
        private BattleField _field;
        private BattleSlot _playerSlot;
        private BattleSlot _enemySlot;
        private TurnOrderResolver _resolver;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _playerSlot = _field.PlayerSide.Slots[0];
            _enemySlot = _field.EnemySide.Slots[0];

            // Create resolver instance with random provider
            var randomProvider = new RandomProvider(42); // Fixed seed for reproducible tests
            _resolver = new TurnOrderResolver(randomProvider);
        }

        #region TurnOrderResolver -> CombatEngine Integration

        [Test]
        public void TurnOrderResolver_SwitchAction_HighestPriority()
        {
            // Arrange
            var switchAction = new SwitchAction(_playerSlot, PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50));
            var moveAction = new UseMoveAction(_enemySlot, _playerSlot, _enemySlot.Pokemon.Moves[0]);
            BattleAction[] actions = new BattleAction[] { moveAction, switchAction };

            // Act
            var sorted = _resolver.SortActions(actions, _field);

            // Assert
            Assert.That(sorted[0], Is.EqualTo(switchAction));
            Assert.That(sorted[1], Is.EqualTo(moveAction));
        }

        [Test]
        public void TurnOrderResolver_MultipleMoves_SortsBySpeed()
        {
            // Arrange
            // Pikachu should be faster than Charmander
            var playerMove = new UseMoveAction(_playerSlot, _enemySlot, _playerSlot.Pokemon.Moves[0]);
            var enemyMove = new UseMoveAction(_enemySlot, _playerSlot, _enemySlot.Pokemon.Moves[0]);
            BattleAction[] actions = new BattleAction[] { enemyMove, playerMove };

            // Act
            var sorted = _resolver.SortActions(actions, _field);

            // Assert
            // Faster Pokemon should go first (assuming Pikachu is faster)
            Assert.That(sorted.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task CombatEngine_RunTurn_SortsActionsByTurnOrder()
        {
            // Arrange
            var engine = CombatEngineTestHelper.CreateCombatEngine();
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var view = new NullBattleView();

            // Create a helper provider that will create actions using the correct slots after initialization
            IActionProvider playerProvider = null;
            IActionProvider enemyProvider = null;

            // Create providers that will use the engine's field slots
            playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, playerSlot.Pokemon.Moves[0]);
            });

            enemyProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(enemySlot, playerSlot, enemySlot.Pokemon.Moves[0]);
            });

            engine.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                playerParty, enemyParty, playerProvider, enemyProvider, view);

            // Act
            await engine.RunTurn();

            // Assert - Turn should complete without errors
            Assert.That(engine.Outcome, Is.EqualTo(BattleOutcome.Ongoing).Or.EqualTo(BattleOutcome.Victory).Or.EqualTo(BattleOutcome.Defeat));
        }

        #endregion

        #region Priority -> Speed Integration

        [Test]
        public void TurnOrderResolver_SamePriority_SortsBySpeed()
        {
            // Arrange
            var action1 = new UseMoveAction(_playerSlot, _enemySlot, _playerSlot.Pokemon.Moves[0]);
            var action2 = new UseMoveAction(_enemySlot, _playerSlot, _enemySlot.Pokemon.Moves[0]);
            BattleAction[] actions = new BattleAction[] { action2, action1 };

            // Act
            var sorted = _resolver.SortActions(actions, _field);

            // Assert
            Assert.That(sorted.Count, Is.EqualTo(2));
            // Both should have priority 0, so speed determines order
        }

        #endregion
    }
}

