using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration.System
{
    /// <summary>
    /// Integration tests for BattleArbiter - verifies battle outcome detection integrates with CombatEngine.
    /// </summary>
    [TestFixture]
    public class BattleArbiterIntegrationTests
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

        #region BattleArbiter -> CombatEngine Integration

        [Test]
        public void CombatEngine_PlayerDefeated_DetectsDefeat()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint player Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP = 0;

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public void CombatEngine_EnemyDefeated_DetectsVictory()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint enemy Pokemon
            _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP = 0;

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Victory));
        }

        [Test]
        public void CombatEngine_BothDefeated_DetectsDraw()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint both Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP = 0;
            _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP = 0;

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Draw));
        }

        [Test]
        public void CombatEngine_BothAlive_ReturnsOngoing()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing));
        }

        #endregion

        #region BattleArbiter -> FaintAction Integration

        [Test]
        public void CombatEngine_FaintAction_UpdatesOutcome()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint enemy through action
            var faintAction = new FaintAction(_engine.Field.PlayerSide.Slots[0], _engine.Field.EnemySide.Slots[0]);
            _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP = 0;
            faintAction.ExecuteLogic(_engine.Field);

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Victory));
        }

        #endregion

        #region BattleArbiter -> Multiple Pokemon Integration

        [Test]
        public void BattleArbiter_MultiplePokemonParty_ChecksAll()
        {
            // Arrange - Multiple Pokemon in party
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint all player Pokemon
            foreach (var pokemon in playerParty)
            {
                pokemon.CurrentHP = 0;
            }

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        #endregion
    }
}

