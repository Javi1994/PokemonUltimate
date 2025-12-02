using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Edge case tests for UseMoveAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class UseMoveActionEdgeCasesTests
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

        #region Null Validation Tests

        [Test]
        public void Constructor_NullUser_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new UseMoveAction(null, _targetSlot, _moveInstance));
        }

        [Test]
        public void Constructor_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new UseMoveAction(_userSlot, null, _moveInstance));
        }

        [Test]
        public void Constructor_NullMoveInstance_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new UseMoveAction(_userSlot, _targetSlot, null));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_Freeze_DoesNotExecuteMove()
        {
            _user.Status = PersistentStatus.Freeze;

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<MessageAction>());
            var message = (MessageAction)reactions[0];
            Assert.That(message.Message, Does.Contain("frozen"));
        }

        [Test]
        public void ExecuteLogic_EmptyUserSlot_ReturnsEmpty()
        {
            var emptySlot = new BattleSlot(0);
            var action = new UseMoveAction(emptySlot, _targetSlot, _moveInstance);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_EmptyTargetSlot_ReturnsEmpty()
        {
            var emptySlot = new BattleSlot(0);
            var action = new UseMoveAction(_userSlot, emptySlot, _moveInstance);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_FaintedUser_DoesNotExecute()
        {
            _user.CurrentHP = 0;

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_FaintedTarget_StillExecutesMove()
        {
            _target.CurrentHP = 0;

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Move should still execute (damage will be 0, but move is used)
            Assert.That(reactions, Is.Not.Empty);
            Assert.That(_moveInstance.CurrentPP, Is.LessThan(_moveInstance.MaxPP));
        }

        [Test]
        public void ExecuteLogic_Paralysis_25PercentChanceToFail()
        {
            _user.Status = PersistentStatus.Paralysis;
            int successes = 0;
            int failures = 0;

            // Run multiple times to test probability
            for (int i = 0; i < 100; i++)
            {
                _user.Status = PersistentStatus.Paralysis;
                var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
                var reactions = action.ExecuteLogic(_field).ToList();

                if (reactions.Count == 1 && reactions[0] is MessageAction msg && 
                    msg.Message.Contains("paralyzed"))
                {
                    failures++;
                }
                else
                {
                    successes++;
                }

                // Restore PP for next iteration
                _moveInstance.RestoreFully();
            }

            // Should have roughly 25% failures (allowing for variance)
            Assert.That(failures, Is.GreaterThan(10)); // At least 10% failures
            Assert.That(failures, Is.LessThan(40)); // Less than 40% failures
        }

        [Test]
        public void ExecuteLogic_Flinch_ConsumesFlinchStatus()
        {
            _userSlot.AddVolatileStatus(VolatileStatus.Flinch);

            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            action.ExecuteLogic(_field);

            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Flinch), Is.False);
        }

        [Test]
        public void ExecuteLogic_NoPP_DoesNotDeductPP()
        {
            while (_moveInstance.HasPP)
            {
                _moveInstance.Use();
            }

            int ppBefore = _moveInstance.CurrentPP;
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            action.ExecuteLogic(_field);

            Assert.That(_moveInstance.CurrentPP, Is.EqualTo(ppBefore));
        }

        [Test]
        public void ExecuteVisual_EmptyUserSlot_SkipsVisual()
        {
            var view = new MockUseMoveBattleView();
            var emptySlot = new BattleSlot(0);
            var action = new UseMoveAction(emptySlot, _targetSlot, _moveInstance);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.MoveAnimationCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_EmptyTargetSlot_SkipsVisual()
        {
            var view = new MockUseMoveBattleView();
            var emptySlot = new BattleSlot(0);
            var action = new UseMoveAction(_userSlot, emptySlot, _moveInstance);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.MoveAnimationCalled, Is.False);
        }

        #endregion
    }
}

