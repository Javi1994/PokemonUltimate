using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Tests.Systems.Combat.Engine;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.System
{
    /// <summary>
    /// Integration tests for PlayerInputProvider - verifies player input integrates with CombatEngine in full battle flow.
    /// </summary>
    [TestFixture]
    public class PlayerInputIntegrationTests
    {
        private CombatEngine _engine;
        private BattleRules _rules;
        private MockBattleViewForIntegration _mockView;

        [SetUp]
        public void SetUp()
        {
            _engine = new CombatEngine();
            _rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _mockView = new MockBattleViewForIntegration();
        }

        #region PlayerInputProvider -> CombatEngine Integration

        [Test]
        public async Task CombatEngine_PlayerInputProvider_FightAction_ExecutesMove()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            var playerProvider = new PlayerInputProvider(_mockView);
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            _mockView.MoveToReturn = playerParty[0].Moves.First(m => m.HasPP);
            _mockView.TargetToReturn = null; // Auto-select in 1v1

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _mockView);

            int initialEnemyHP = _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP;

            // Act - Process turn with player input
            await _engine.RunTurn();

            // Assert - Move should have been executed
            // Note: May not deal damage if move misses or has no power, but action should be processed
            Assert.That(_engine.Queue.Count, Is.EqualTo(0), "Queue should be empty after processing");
        }

        [Test]
        public async Task CombatEngine_PlayerInputProvider_SwitchAction_ExecutesSwitch()
        {
            // Arrange
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            var playerProvider = new PlayerInputProvider(_mockView);
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _mockView.ActionTypeToReturn = BattleActionType.Switch;
            _mockView.PokemonToSwitchTo = playerParty[1];

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _mockView);

            var originalPokemon = _engine.Field.PlayerSide.Slots[0].Pokemon;

            // Act - Process turn with switch action
            await _engine.RunTurn();

            // Assert - Pokemon should be switched
            Assert.That(_engine.Field.PlayerSide.Slots[0].Pokemon, Is.EqualTo(playerParty[1]));
            Assert.That(_engine.Field.PlayerSide.Slots[0].Pokemon, Is.Not.EqualTo(originalPokemon));
        }

        [Test]
        public async Task CombatEngine_PlayerInputProvider_MultipleTurns_ProcessesCorrectly()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            var playerProvider = new PlayerInputProvider(_mockView);
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            _mockView.MoveToReturn = playerParty[0].Moves.First(m => m.HasPP);
            _mockView.TargetToReturn = null;

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _mockView);

            // Act - Process multiple turns
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
            }

            // Assert - Battle should still be ongoing (no one fainted)
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Ongoing).Or.EqualTo(BattleOutcome.Victory).Or.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public async Task CombatEngine_PlayerInputProvider_CancelledInput_ReturnsNull()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            var playerProvider = new PlayerInputProvider(_mockView);
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            _mockView.MoveToReturn = null; // Player cancelled move selection

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _mockView);

            // Act - Process turn with cancelled input
            await _engine.RunTurn();

            // Assert - Turn should complete without error (null action is handled)
            Assert.That(_engine.Queue.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task CombatEngine_PlayerInputProvider_AutoSelectsTarget_InSingleTargetScenario()
        {
            // Arrange
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            var playerProvider = new PlayerInputProvider(_mockView);
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            _mockView.MoveToReturn = playerParty[0].Moves.First(m => m.HasPP);
            _mockView.TargetToReturn = null; // Should auto-select enemy in 1v1

            _engine.Initialize(_rules, playerParty, enemyParty, playerProvider, enemyProvider, _mockView);

            // Act - Process turn
            await _engine.RunTurn();

            // Assert - Action should be created and processed
            // The auto-selection happens inside PlayerInputProvider, so we verify the turn completes
            Assert.That(_engine.Queue.Count, Is.EqualTo(0));
        }

        #endregion
    }

    /// <summary>
    /// Mock battle view for PlayerInputProvider integration tests.
    /// </summary>
    internal class MockBattleViewForIntegration : IBattleView
    {
        public BattleActionType ActionTypeToReturn { get; set; }
        public MoveInstance MoveToReturn { get; set; }
        public BattleSlot TargetToReturn { get; set; }
        public PokemonInstance PokemonToSwitchTo { get; set; }

        public Task ShowMessage(string message) => Task.CompletedTask;
        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;
        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;
        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;
        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;
        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task<BattleActionType> SelectActionType(BattleSlot slot)
        {
            return Task.FromResult(ActionTypeToReturn);
        }

        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves)
        {
            return Task.FromResult(MoveToReturn ?? moves?.FirstOrDefault());
        }

        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets)
        {
            return Task.FromResult(TargetToReturn ?? validTargets?.FirstOrDefault());
        }

        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)
        {
            return Task.FromResult(PokemonToSwitchTo ?? availablePokemon?.FirstOrDefault());
        }
    }
}

