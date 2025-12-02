using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat
{
    /// <summary>
    /// Edge case tests for CombatEngine - boundary conditions and invalid states.
    /// </summary>
    [TestFixture]
    public class CombatEngineEdgeCasesTests
    {
        private CombatEngine _engine;
        private BattleRules _rules;
        private NullBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _engine = new CombatEngine();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _view = new NullBattleView();
        }

        [Test]
        public void Initialize_NullPlayerParty_ThrowsArgumentNullException()
        {
            // Arrange
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _engine.Initialize(_rules, null, enemyParty, playerProvider, enemyProvider, _view));
        }

        [Test]
        public void Initialize_NullEnemyParty_ThrowsArgumentNullException()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _engine.Initialize(_rules, playerParty, null, playerProvider, enemyProvider, _view));
        }

        [Test]
        public void Initialize_NullPlayerProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _engine.Initialize(_rules, playerParty, enemyParty, null, enemyProvider, _view));
        }

        [Test]
        public void Initialize_NullEnemyProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, null, _view));
        }

        [Test]
        public void Initialize_NullView_ThrowsArgumentNullException()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, null));
        }

        [Test]
        public async Task RunTurn_NoActiveSlots_DoesNotCrash()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Clear all slots
            _engine.Field.PlayerSide.Slots[0].ClearSlot();
            _engine.Field.EnemySide.Slots[0].ClearSlot();

            // Act & Assert - Should not throw
            await _engine.RunTurn();
            Assert.That(_engine.Queue.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RunTurn_NullActionFromProvider_HandledGracefully()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider((BattleAction)null);
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act & Assert - Should not throw
            await _engine.RunTurn();
        }

        [Test]
        public async Task RunTurn_ProviderReturnsNull_HandledGracefully()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            // Create provider that returns null
            var nullProvider = new TestActionProvider((BattleAction)null);
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, nullProvider, enemyProvider, _view);

            // Act & Assert - Should not throw
            await _engine.RunTurn();
        }

        [Test]
        public async Task RunBattle_MaxTurnsReached_StopsLoop()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert - Should stop at max turns (1000) or when battle ends
            Assert.IsTrue(result.TurnsTaken <= 1000);
        }

        [Test]
        public async Task RunBattle_ImmediateVictory_ReturnsQuickly()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyPokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 50);
            enemyPokemon.CurrentHP = 0; // Already fainted
            var enemyParty = new[] { enemyPokemon };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BattleOutcome.Victory));
            // RunBattle executes one turn before checking outcome
            Assert.That(result.TurnsTaken, Is.EqualTo(1));
        }

        [Test]
        public async Task RunBattle_ImmediateDefeat_ReturnsQuickly()
        {
            // Arrange
            var playerPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            playerPokemon.CurrentHP = 0; // Already fainted
            var playerParty = new[] { playerPokemon };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BattleOutcome.Defeat));
            // RunBattle executes one turn before checking outcome
            Assert.That(result.TurnsTaken, Is.EqualTo(1));
        }

        [Test]
        public async Task RunBattle_DoubleBattle_ProcessesBothSides()
        {
            // Arrange
            _rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            await _engine.RunTurn();

            // Assert - Should collect actions from all 4 slots
            // Queue should be empty after processing
            Assert.That(_engine.Queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void Initialize_AssignsProvidersToAllSlots()
        {
            // Arrange
            _rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var playerProvider = new TestActionProvider("Player action");
            var enemyProvider = new TestActionProvider("Enemy action");

            // Act
            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Assert
            Assert.That(_engine.Field.PlayerSide.Slots[0].ActionProvider, Is.EqualTo(playerProvider));
            Assert.That(_engine.Field.PlayerSide.Slots[1].ActionProvider, Is.EqualTo(playerProvider));
            Assert.That(_engine.Field.EnemySide.Slots[0].ActionProvider, Is.EqualTo(enemyProvider));
            Assert.That(_engine.Field.EnemySide.Slots[1].ActionProvider, Is.EqualTo(enemyProvider));
        }
    }
}

