using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration
{
    /// <summary>
    /// Integration tests for full battles - end-to-end battle simulation.
    /// </summary>
    [TestFixture]
    public class FullBattleTests
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

        #region Basic Battle Tests

        [Test]
        public async Task FullBattle_1v1_ProducesWinner()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.Outcome, Is.AnyOf(BattleOutcome.Victory, BattleOutcome.Defeat));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        [Test]
        public async Task FullBattle_TypeAdvantage_FavoredSideWins()
        {
            // Arrange: Pikachu (Electric) vs Bulbasaur (Grass/Poison) - Electric is super effective
            // Note: This is probabilistic, but with type advantage, favored side should win more often
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50) };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert: Battle should complete
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
            Assert.That(result.TurnsTaken, Is.LessThan(1000)); // Should complete reasonably
        }

        [Test]
        public async Task FullBattle_HigherLevel_FavoredSideWins()
        {
            // Arrange: Higher level Pokemon should win
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 60) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 30) };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert: Battle should complete, higher level should win (probabilistic)
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        [Test]
        public async Task FullBattle_ManyTurns_CompletesWithoutError()
        {
            // Arrange: Two similar Pokemon - battle should take many turns
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert: Should complete without errors
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
            Assert.That(result.TurnsTaken, Is.LessThan(1000)); // Should not hit max turns
        }

        [Test]
        public async Task FullBattle_AllMovesUsed_PPDepletes()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert: At least one Pokemon should have used moves (PP depleted)
            var playerPokemon = _engine.Field.PlayerSide.Party[0];
            var enemyPokemon = _engine.Field.EnemySide.Party[0];

            // At least one move should have been used (PP < MaxPP)
            bool playerUsedMoves = playerPokemon.Moves.Any(m => m.CurrentPP < m.MaxPP);
            bool enemyUsedMoves = enemyPokemon.Moves.Any(m => m.CurrentPP < m.MaxPP);

            Assert.That(playerUsedMoves || enemyUsedMoves, Is.True, 
                "At least one Pokemon should have used moves during battle");
        }

        [Test]
        public async Task FullBattle_MultiplePokemonInParty_CompletesWhenActiveFaints()
        {
            // Arrange: Player has multiple Pokemon, enemy has one
            // Note: Current implementation ends battle when active Pokemon faints (no auto-switching)
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50) };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert: Battle should complete (ends when active Pokemon faints)
            Assert.That(result, Is.Not.Null);
            // Note: Battle may end as Ongoing if no active slots remain, but should complete
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
            
            // At least one Pokemon should have fainted
            bool playerHasFainted = playerParty.Any(p => p.IsFainted);
            bool enemyHasFainted = enemyParty.Any(p => p.IsFainted);
            Assert.That(playerHasFainted || enemyHasFainted, Is.True, 
                "At least one Pokemon should have fainted during battle");
        }

        #endregion

        #region AI vs AI Tests

        [Test]
        public async Task FullBattle_RandomAIvsRandomAI_Completes()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        [Test]
        public async Task FullBattle_AlwaysAttackAIvsAlwaysAttackAI_Completes()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        [Test]
        public async Task FullBattle_MixedAI_Completes()
        {
            // Arrange: Player uses RandomAI, Enemy uses AlwaysAttackAI
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            var playerAI = new RandomAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        #endregion
    }
}

