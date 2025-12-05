using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using static PokemonUltimate.Tests.Systems.Combat.Engine.TestActionProvider;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Functional tests for CombatEngine - battle orchestration.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class CombatEngineTests
    {
        private CombatEngine _engine;
        private BattleRules _rules;
        private NullBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _engine = CombatEngineTestHelper.CreateCombatEngine();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _view = new NullBattleView();
        }


        [Test]
        public void Initialize_SetsUpFieldCorrectly()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act
            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Assert
            Assert.IsNotNull(_engine.Field);
            Assert.IsNotNull(_engine.Queue);
            Assert.That(_engine.Outcome, Is.EqualTo(BattleOutcome.Ongoing));
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(1));
        }

        [Test]
        public void Initialize_AssignsActionProvidersToSlots()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act
            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Assert
            Assert.That(_engine.Field.PlayerSide.Slots[0].ActionProvider, Is.EqualTo(playerProvider));
            Assert.That(_engine.Field.EnemySide.Slots[0].ActionProvider, Is.EqualTo(enemyProvider));
        }

        [Test]
        public async Task RunTurn_CollectsActionsFromBothSides()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAction = new MessageAction("Player move");
            var enemyAction = new MessageAction("Enemy move");
            var playerProvider = new TestActionProvider(playerAction);
            var enemyProvider = new TestActionProvider(enemyAction);

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            await _engine.RunTurn();

            // Assert
            // Queue should be empty after processing
            Assert.That(_engine.Queue.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RunTurn_ProcessesAllActions()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            await _engine.RunTurn();

            // Assert
            // Actions should be processed (queue empty)
            Assert.That(_engine.Queue.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RunBattle_EndsWhenEnemyDefeated_ReturnsVictory()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyPokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 50);
            var enemyParty = new[] { enemyPokemon };

            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Manually faint enemy Pokemon
            _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP = 0;

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BattleOutcome.Victory));
            Assert.That(_engine.Outcome, Is.EqualTo(BattleOutcome.Victory));
        }

        [Test]
        public async Task RunBattle_EndsWhenPlayerDefeated_ReturnsDefeat()
        {
            // Arrange
            var playerPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            var playerParty = new[] { playerPokemon };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };

            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Manually faint player Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP = 0;

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BattleOutcome.Defeat));
            Assert.That(_engine.Outcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public void Initialize_NullRules_ThrowsArgumentNullException()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _engine.Initialize(null, playerParty, enemyParty, playerProvider, enemyProvider, _view));
        }

        [Test]
        public void RunTurn_NotInitialized_ThrowsInvalidOperationException()
        {
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _engine.RunTurn());
        }

        [Test]
        public void RunBattle_NotInitialized_ThrowsInvalidOperationException()
        {
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _engine.RunBattle());
        }

    }
}

