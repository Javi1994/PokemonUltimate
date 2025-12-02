using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration
{
    /// <summary>
    /// Integration tests for TargetResolver - verifies targeting works with AI, PlayerInput, and UseMoveAction.
    /// </summary>
    [TestFixture]
    public class TargetResolverIntegrationTests
    {
        private BattleField _field;
        private BattleSlot _playerSlot;
        private BattleSlot _enemySlot;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _playerSlot = _field.PlayerSide.Slots[0];
            _enemySlot = _field.EnemySide.Slots[0];
        }

        #region TargetResolver -> AI Integration

        [Test]
        public async Task RandomAI_UsesTargetResolver_SelectsValidTarget()
        {
            // Arrange
            var ai = new RandomAI();

            // Act
            var action = await ai.GetAction(_field, _playerSlot);

            // Assert
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
            var useMoveAction = (UseMoveAction)action;
            Assert.That(useMoveAction.Target, Is.Not.Null);
            Assert.That(useMoveAction.Target, Is.Not.EqualTo(_playerSlot)); // Should target enemy
        }

        [Test]
        public async Task AlwaysAttackAI_UsesTargetResolver_SelectsFirstValidTarget()
        {
            // Arrange
            var ai = new AlwaysAttackAI();

            // Act
            var action = await ai.GetAction(_field, _playerSlot);

            // Assert
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
            var useMoveAction = (UseMoveAction)action;
            Assert.That(useMoveAction.Target, Is.Not.Null);
            
            // Verify target is valid for the move
            var validTargets = TargetResolver.GetValidTargets(_playerSlot, useMoveAction.Move, _field);
            Assert.That(validTargets, Contains.Item(useMoveAction.Target));
        }

        #endregion

        #region TargetResolver -> PlayerInputProvider Integration

        [Test]
        public async Task PlayerInputProvider_UsesTargetResolver_ForMoveTargeting()
        {
            // Arrange
            var mockView = new MockBattleView();
            var provider = new PlayerInputProvider(mockView);
            mockView.MoveToReturn = _playerSlot.Pokemon.Moves.FirstOrDefault(m => m.HasPP);
            mockView.TargetToReturn = _enemySlot;

            // Act
            var action = await provider.GetAction(_field, _playerSlot);

            // Assert
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
            var useMoveAction = (UseMoveAction)action;
            
            // Verify target was validated by TargetResolver
            var validTargets = TargetResolver.GetValidTargets(_playerSlot, useMoveAction.Move, _field);
            Assert.That(validTargets, Contains.Item(useMoveAction.Target));
        }

        [Test]
        public async Task PlayerInputProvider_SingleTarget_AutoSelects()
        {
            // Arrange
            var mockView = new MockBattleView();
            var provider = new PlayerInputProvider(mockView);
            mockView.MoveToReturn = _playerSlot.Pokemon.Moves.FirstOrDefault(m => m.HasPP);

            // Act
            var action = await provider.GetAction(_field, _playerSlot);

            // Assert - Should auto-select the only valid target
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
            var useMoveAction = (UseMoveAction)action;
            Assert.That(useMoveAction.Target, Is.EqualTo(_enemySlot));
        }

        #endregion

        #region TargetResolver -> UseMoveAction Integration

        [Test]
        public void TargetResolver_SingleEnemyMove_ReturnsEnemySlot()
        {
            // Arrange
            var move = MoveCatalog.Thunderbolt; // SingleEnemy target

            // Act
            var validTargets = TargetResolver.GetValidTargets(_playerSlot, move, _field);

            // Assert
            Assert.That(validTargets, Has.Count.EqualTo(1));
            Assert.That(validTargets[0], Is.EqualTo(_enemySlot));
        }

        [Test]
        public void TargetResolver_SelfMove_ReturnsSelf()
        {
            // Arrange - Find a move that targets self
            var selfMove = _playerSlot.Pokemon.Moves.FirstOrDefault(m => m.Move.TargetScope == TargetScope.Self);
            if (selfMove == null)
            {
                Assert.Inconclusive("No self-targeting move found");
                return;
            }

            // Act
            var validTargets = TargetResolver.GetValidTargets(_playerSlot, selfMove.Move, _field);

            // Assert
            Assert.That(validTargets, Contains.Item(_playerSlot));
            Assert.That(validTargets.Count, Is.EqualTo(1));
        }

        [Test]
        public void TargetResolver_FaintedTarget_Excluded()
        {
            // Arrange
            _enemySlot.Pokemon.CurrentHP = 0; // Faint the enemy
            var move = MoveCatalog.Thunderbolt;

            // Act
            var validTargets = TargetResolver.GetValidTargets(_playerSlot, move, _field);

            // Assert
            Assert.That(validTargets, Is.Empty);
        }

        #endregion

        #region TargetResolver -> Doubles Integration

        [Test]
        public void TargetResolver_DoublesBattle_ReturnsMultipleTargets()
        {
            // Arrange - Doubles battle
            var doublesField = new BattleField();
            doublesField.Initialize(new BattleRules { PlayerSlots = 2, EnemySlots = 2 },
                new[]
                {
                    PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                    PokemonFactory.Create(PokemonCatalog.Charmander, 50)
                },
                new[]
                {
                    PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                    PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
                });

            var playerSlot = doublesField.PlayerSide.Slots[0];
            var move = MoveCatalog.Thunderbolt; // SingleEnemy

            // Act
            var validTargets = TargetResolver.GetValidTargets(playerSlot, move, doublesField);

            // Assert
            Assert.That(validTargets, Has.Count.EqualTo(2)); // Two enemy slots
            Assert.That(validTargets.All(t => t.Side == doublesField.EnemySide), Is.True);
        }

        #endregion
    }

    /// <summary>
    /// Mock battle view for testing PlayerInputProvider.
    /// </summary>
    internal class MockBattleView : IBattleView
    {
        public MoveInstance MoveToReturn { get; set; }
        public BattleSlot TargetToReturn { get; set; }

        public Task ShowMessage(string message) => Task.CompletedTask;
        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;
        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;
        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;
        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;
        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task<BattleActionType> SelectActionType(BattleSlot slot) => Task.FromResult(BattleActionType.Fight);
        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) => Task.FromResult(MoveToReturn ?? moves?.FirstOrDefault());
        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) => Task.FromResult(TargetToReturn ?? validTargets?.FirstOrDefault());
        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) => Task.FromResult(availablePokemon?.FirstOrDefault());
    }
}

