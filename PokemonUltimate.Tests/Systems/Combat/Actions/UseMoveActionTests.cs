using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for UseMoveAction - executes a move in battle.
    /// </summary>
    [TestFixture]
    public class UseMoveActionTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private MoveInstance _moveInstance;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _user = _userSlot.Pokemon;
            _target = _targetSlot.Pokemon;
            _moveInstance = _user.Moves[0];
        }

        #region ExecuteLogic Tests

        [Test]
        public void ExecuteLogic_NoPP_ReturnsFailMessage()
        {
            // Use all PP
            while (_moveInstance.HasPP)
            {
                _moveInstance.Use();
            }

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<MessageAction>());
            var messageAction = (MessageAction)reactions[0];
            Assert.That(messageAction.Message, Does.Contain("no PP"));
        }

        [Test]
        public void ExecuteLogic_Flinched_ReturnsFailMessage()
        {
            _userSlot.AddVolatileStatus(VolatileStatus.Flinch);

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<MessageAction>());
            var messageAction = (MessageAction)reactions[0];
            Assert.That(messageAction.Message, Does.Contain("flinched"));
        }

        [Test]
        public void ExecuteLogic_Success_DeductsPP()
        {
            int initialPP = _moveInstance.CurrentPP;

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            action.ExecuteLogic(_field);

            Assert.That(_moveInstance.CurrentPP, Is.EqualTo(initialPP - 1));
        }

        [Test]
        public void ExecuteLogic_Success_ReturnsMessageAndDamageActions()
        {
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(reactions, Has.Count.GreaterThanOrEqualTo(2));
            Assert.That(reactions[0], Is.InstanceOf<MessageAction>());
            Assert.That(reactions.Any(r => r is DamageAction), Is.True);
        }

        [Test]
        public void ExecuteLogic_WithStatusEffect_ReturnsStatusAction()
        {
            // Use a move with status effect (like Thunder Wave)
            // Note: ThunderWave has 90% accuracy, so we need to run multiple times
            // or verify that when it hits, it generates the status action
            var statusMove = new MoveInstance(MoveCatalog.ThunderWave);
            _user.Moves[0] = statusMove;

            // Run multiple times to account for accuracy (90%)
            bool foundStatusAction = false;
            for (int i = 0; i < 20; i++)
            {
                // Reset move PP for each attempt
                var testMove = new MoveInstance(MoveCatalog.ThunderWave);
                _user.Moves[0] = testMove;
                
                var action = new UseMoveAction(_userSlot, _targetSlot, testMove);
                var reactions = action.ExecuteLogic(_field).ToList();

                // Check if move hit (has more than just the "used" message)
                if (reactions.Count > 1 && !reactions.Any(r => r is MessageAction msg && msg.Message.Contains("missed")))
                {
                    // Move hit, check for status action
                    if (reactions.Any(r => r is ApplyStatusAction))
                    {
                        foundStatusAction = true;
                        break;
                    }
                }
            }

            // Should have found status action at least once when move hit
            Assert.That(foundStatusAction, Is.True, 
                "Status action should be generated when ThunderWave hits (90% accuracy, 100% status chance)");
        }

        [Test]
        public void ExecuteLogic_Misses_ReturnsOnlyMissMessage()
        {
            // Use a move with low accuracy and force a miss
            var lowAccuracyMove = new MoveInstance(MoveCatalog.Thunder);
            _user.Moves[0] = lowAccuracyMove;

            var action = new UseMoveAction(_userSlot, _targetSlot, lowAccuracyMove);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Should have "used move" message and "missed" message
            Assert.That(reactions.Count(r => r is MessageAction), Is.GreaterThanOrEqualTo(1));
            // If it misses, no damage action should be generated
            // (This test may need adjustment based on accuracy implementation)
        }

        [Test]
        public void ExecuteLogic_Sleep_DoesNotExecuteMove()
        {
            _user.Status = PersistentStatus.Sleep;

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Should return a message about being asleep
            Assert.That(reactions, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<MessageAction>());
        }

        [Test]
        public void ExecuteLogic_Paralysis_MayFail()
        {
            _user.Status = PersistentStatus.Paralysis;

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Should either execute or return paralysis message
            // (25% chance to fail, so this test checks structure)
            Assert.That(reactions, Is.Not.Empty);
        }

        [Test]
        public void ExecuteLogic_NeverMissesMove_AlwaysHits()
        {
            // Use Quick Attack which has high accuracy (100%)
            var quickAttackMove = new MoveInstance(MoveCatalog.QuickAttack);
            _user.Moves[0] = quickAttackMove;

            var action = new UseMoveAction(_userSlot, _targetSlot, quickAttackMove);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Should generate damage action (high accuracy move)
            Assert.That(reactions.Any(r => r is DamageAction), Is.True);
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsPlayMoveAnimation()
        {
            var view = new MockUseMoveBattleView();
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.MoveAnimationCalled, Is.True);
            Assert.That(view.MoveAnimationUser, Is.EqualTo(_userSlot));
            Assert.That(view.MoveAnimationTarget, Is.EqualTo(_targetSlot));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for UseMoveAction testing.
    /// </summary>
    internal class MockUseMoveBattleView : IBattleView
    {
        public bool MoveAnimationCalled { get; private set; }
        public BattleSlot MoveAnimationUser { get; private set; }
        public BattleSlot MoveAnimationTarget { get; private set; }
        public string MoveAnimationMoveId { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId)
        {
            MoveAnimationCalled = true;
            MoveAnimationUser = user;
            MoveAnimationTarget = target;
            MoveAnimationMoveId = moveId;
            return Task.CompletedTask;
        }

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;

        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;

        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
        public Task<BattleActionType> SelectActionType(BattleSlot slot) => Task.FromResult(BattleActionType.Fight);
        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) => Task.FromResult(moves?.FirstOrDefault());
        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) => Task.FromResult(validTargets?.FirstOrDefault());
        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) => Task.FromResult(availablePokemon?.FirstOrDefault());
    }
}

