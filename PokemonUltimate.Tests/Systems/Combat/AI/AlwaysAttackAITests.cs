using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.AI
{
    /// <summary>
    /// Tests for AlwaysAttackAI - always uses first available move.
    /// </summary>
    [TestFixture]
    public class AlwaysAttackAITests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private AlwaysAttackAI _ai;

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
            _ai = new AlwaysAttackAI();
        }

        #region GetAction Tests

        [Test]
        public async Task GetAction_ReturnsUseMoveAction()
        {
            var action = await _ai.GetAction(_field, _userSlot);

            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
        }

        [Test]
        public async Task GetAction_AlwaysUsesFirstMove()
        {
            var firstMove = _user.Moves.FirstOrDefault(m => m.HasPP);
            
            if (firstMove != null)
            {
                var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;

                Assert.That(action, Is.Not.Null);
                Assert.That(action.MoveInstance, Is.EqualTo(firstMove));
            }
        }

        [Test]
        public async Task GetAction_SkipsMovesWithoutPP()
        {
            // Deplete PP from first move
            var firstMove = _user.Moves[0];
            while (firstMove.HasPP)
            {
                firstMove.Use();
            }

            // AI should use next available move
            var secondMove = _user.Moves.FirstOrDefault(m => m.HasPP);
            if (secondMove != null)
            {
                var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;
                Assert.That(action.MoveInstance, Is.EqualTo(secondMove));
            }
        }

        [Test]
        public async Task GetAction_SelectsValidTarget()
        {
            var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;

            Assert.That(action, Is.Not.Null);
            Assert.That(action.Target, Is.Not.Null);
        }

        [Test]
        public async Task GetAction_MultipleCalls_ReturnsSameMove()
        {
            var firstAction = await _ai.GetAction(_field, _userSlot) as UseMoveAction;
            var secondAction = await _ai.GetAction(_field, _userSlot) as UseMoveAction;

            Assert.That(firstAction, Is.Not.Null);
            Assert.That(secondAction, Is.Not.Null);
            Assert.That(firstAction.MoveInstance, Is.EqualTo(secondAction.MoveInstance));
        }

        #endregion

        #region Null Validation Tests

        [Test]
        public void GetAction_NullField_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _ai.GetAction(null, _userSlot));
        }

        [Test]
        public void GetAction_NullSlot_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _ai.GetAction(_field, null));
        }

        #endregion

        #region Edge Cases

        [Test]
        public async Task GetAction_NoMovesWithPP_ReturnsNull()
        {
            // Deplete all PP from all moves
            foreach (var move in _user.Moves)
            {
                while (move.HasPP)
                {
                    move.Use();
                }
            }

            var action = await _ai.GetAction(_field, _userSlot);

            Assert.That(action, Is.Null);
        }

        [Test]
        public async Task GetAction_EmptySlot_ReturnsNull()
        {
            _userSlot.ClearSlot();

            var action = await _ai.GetAction(_field, _userSlot);

            Assert.That(action, Is.Null);
        }

        [Test]
        public async Task GetAction_FaintedPokemon_ReturnsNull()
        {
            _user.TakeDamage(_user.MaxHP);

            var action = await _ai.GetAction(_field, _userSlot);

            Assert.That(action, Is.Null);
        }

        #endregion
    }
}

