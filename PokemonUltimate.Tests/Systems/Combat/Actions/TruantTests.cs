using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Truant ability - blocks moves every other turn.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.17: Advanced Abilities
    /// **Documentation**: See `docs/features/2-combat-system/2.17-advanced-abilities/README.md`
    /// </remarks>
    [TestFixture]
    public class TruantTests
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
            var userPokemon = PokemonFactory.Create(PokemonCatalog.Slakoth, 50);
            userPokemon.SetAbility(AbilityCatalog.Truant);
            // Ensure Pokemon has at least one move
            if (userPokemon.Moves.Count == 0)
            {
                userPokemon.Moves.Add(new MoveInstance(MoveCatalog.Tackle));
            }
            
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { userPokemon },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _user = _userSlot.Pokemon;
            _target = _targetSlot.Pokemon;
            
            _moveInstance = _user.Moves[0];
        }

        #region OnBeforeMove Tests

        [Test]
        public void ExecuteLogic_FirstTurn_WithTruant_AllowsMove()
        {
            // Arrange - First turn (odd turn)
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            int initialPP = _moveInstance.CurrentPP;

            // Act
            var reactions = action.ExecuteLogic(_field).ToList();

            // Assert - Move should execute normally
            Assert.That(reactions.Count, Is.GreaterThan(0));
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("used")), Is.True);
            Assert.That(_moveInstance.CurrentPP, Is.EqualTo(initialPP - 1), "PP should be consumed");
        }

        [Test]
        public void ExecuteLogic_SecondTurn_WithTruant_BlocksMove()
        {
            // Arrange - First turn (allows move)
            var action1 = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            action1.ExecuteLogic(_field).ToList(); // Execute first turn
            
            // Reset move PP for second turn
            _moveInstance = new MoveInstance(_moveInstance.Move);
            int initialPP = _moveInstance.CurrentPP;

            // Act - Second turn (even turn - should be blocked)
            var action2 = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action2.ExecuteLogic(_field).ToList();

            // Assert - Move should be blocked
            Assert.That(reactions.Count, Is.GreaterThan(0));
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("loafing around")), Is.True);
            Assert.That(_moveInstance.CurrentPP, Is.EqualTo(initialPP), "PP should NOT be consumed when blocked");
        }

        [Test]
        public void ExecuteLogic_ThirdTurn_WithTruant_AllowsMove()
        {
            // Arrange - Execute two turns to get to third turn
            var action1 = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            action1.ExecuteLogic(_field).ToList(); // Turn 1 - allows
            
            _moveInstance = new MoveInstance(_moveInstance.Move);
            var action2 = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            action2.ExecuteLogic(_field).ToList(); // Turn 2 - blocks
            
            // Reset for turn 3
            _moveInstance = new MoveInstance(_moveInstance.Move);
            int initialPP = _moveInstance.CurrentPP;

            // Act - Third turn (odd turn - should allow)
            var action3 = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action3.ExecuteLogic(_field).ToList();

            // Assert - Move should execute normally
            Assert.That(reactions.Count, Is.GreaterThan(0));
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("used")), Is.True);
            Assert.That(_moveInstance.CurrentPP, Is.EqualTo(initialPP - 1), "PP should be consumed");
        }

        [Test]
        public void ExecuteLogic_WithoutTruant_AlwaysAllowsMove()
        {
            // Arrange - Remove Truant ability
            _user.SetAbility(null);
            int initialPP = _moveInstance.CurrentPP;

            // Act
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Assert - Move should always execute
            Assert.That(reactions.Count, Is.GreaterThan(0));
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("used")), Is.True);
            Assert.That(_moveInstance.CurrentPP, Is.EqualTo(initialPP - 1), "PP should be consumed");
        }

        #endregion
    }
}

