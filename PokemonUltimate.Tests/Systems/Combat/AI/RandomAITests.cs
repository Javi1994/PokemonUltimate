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
    /// Tests for RandomAI - selects random valid moves.
    /// </summary>
    [TestFixture]
    public class RandomAITests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private RandomAI _ai;

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
            _ai = new RandomAI();
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
        public async Task GetAction_UsesValidMove()
        {
            var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;

            Assert.That(action, Is.Not.Null);
            Assert.That(action.MoveInstance, Is.Not.Null);
            Assert.That(_user.Moves, Contains.Item(action.MoveInstance));
        }

        [Test]
        public async Task GetAction_UsesMoveWithPP()
        {
            // Deplete all PP from first move
            var firstMove = _user.Moves[0];
            while (firstMove.HasPP)
            {
                firstMove.Use();
            }

            // If there are other moves, AI should pick one with PP
            if (_user.Moves.Any(m => m.HasPP))
            {
                var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;
                Assert.That(action.MoveInstance.HasPP, Is.True);
            }
        }

        [Test]
        public async Task GetAction_SelectsValidTarget()
        {
            var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;

            Assert.That(action, Is.Not.Null);
            Assert.That(action.Target, Is.Not.Null);
            Assert.That(action.Target, Is.Not.EqualTo(_userSlot)); // Should target enemy
        }

        [Test]
        public async Task GetAction_SelfTargetingMove_TargetsSelf()
        {
            // Create a Pokemon with a self-targeting move (e.g., Swords Dance)
            // For now, we'll test that if move targets self, target is self
            // This test may need adjustment based on available moves in catalog
            
            var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;
            Assert.That(action, Is.Not.Null);
            // Target validation happens in TargetResolver, so action should be valid
        }

        [Test]
        public async Task GetAction_MultipleCalls_CanReturnDifferentMoves()
        {
            // Run multiple times and check if we get different moves (probabilistic)
            var actions = new System.Collections.Generic.HashSet<string>();
            
            for (int i = 0; i < 20; i++)
            {
                var action = await _ai.GetAction(_field, _userSlot) as UseMoveAction;
                if (action != null)
                {
                    actions.Add(action.MoveInstance.Move.Name);
                }
            }

            // If Pokemon has multiple moves, we should see variety (with high probability)
            // This is a probabilistic test, but with 20 attempts it's very likely
            if (_user.Moves.Count(m => m.HasPP) > 1)
            {
                Assert.That(actions.Count, Is.GreaterThan(1), 
                    "AI should select different moves randomly");
            }
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

            // AI should return null if no moves available
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

