using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Providers
{
    /// <summary>
    /// Edge case tests for PlayerInputProvider.
    /// Tests boundary conditions, invalid states, and error handling.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PlayerInputProviderEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private MockBattleView _mockView;
        private PlayerInputProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _mockView = new MockBattleView();
            _provider = new PlayerInputProvider(_mockView);
        }

        #region Invalid Action Type Tests

        [Test]
        public void GetAction_InvalidActionType_ThrowsArgumentException()
        {
            // Arrange
            _mockView.ActionTypeToReturn = (BattleActionType)999; // Invalid enum value

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _provider.GetAction(_field, _userSlot));
        }

        [Test]
        public void GetAction_ItemActionType_ThrowsNotImplementedException()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Item;

            // Act & Assert
            Assert.ThrowsAsync<NotImplementedException>(async () =>
                await _provider.GetAction(_field, _userSlot));
        }

        [Test]
        public void GetAction_RunActionType_ThrowsNotImplementedException()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Run;

            // Act & Assert
            Assert.ThrowsAsync<NotImplementedException>(async () =>
                await _provider.GetAction(_field, _userSlot));
        }

        #endregion

        #region Empty Collections Tests

        [Test]
        public async Task GetAction_FightAction_EmptyMovesList_ReturnsNull()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            // Remove all moves (simulate Pokemon with no moves)
            var pokemon = _userSlot.Pokemon;
            // Note: Pokemon always has moves, so we'll deplete all PP instead
            foreach (var move in pokemon.Moves)
            {
                while (move.HasPP)
                {
                    move.Use();
                }
            }

            // Act
            var action = await _provider.GetAction(_field, _userSlot);

            // Assert
            Assert.That(action, Is.Null);
        }

        [Test]
        public async Task GetAction_SwitchAction_EmptyParty_ReturnsNull()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Switch;
            // Party only has the active Pokemon, so no switches available

            // Act
            var action = await _provider.GetAction(_field, _userSlot);

            // Assert
            Assert.That(action, Is.Null);
        }

        #endregion

        #region Boundary Tests

        [Test]
        public async Task GetAction_FightAction_OneMoveWithPP_Works()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            
            // Deplete PP from all moves except one
            var pokemon = _userSlot.Pokemon;
            var moves = pokemon.Moves.ToList();
            for (int i = 1; i < moves.Count; i++)
            {
                while (moves[i].HasPP)
                {
                    moves[i].Use();
                }
            }

            var availableMove = moves[0];
            _mockView.MoveToReturn = availableMove;
            _mockView.TargetToReturn = _field.EnemySide.Slots[0];

            // Act
            var action = await _provider.GetAction(_field, _userSlot);

            // Assert
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<UseMoveAction>());
        }

        [Test]
        public async Task GetAction_SwitchAction_OneAvailablePokemon_Works()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Switch;
            
            var secondPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);
            var newParty = new List<PokemonInstance>(_field.PlayerSide.Party) { secondPokemon };
            _field.PlayerSide.SetParty(newParty);
            _mockView.PokemonToReturn = secondPokemon;

            // Act
            var action = await _provider.GetAction(_field, _userSlot);

            // Assert
            Assert.That(action, Is.Not.Null);
            Assert.That(action, Is.InstanceOf<SwitchAction>());
        }

        #endregion

        #region State Validation Tests

        [Test]
        public async Task GetAction_SlotBecomesEmpty_DuringBattle_ReturnsNull()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            _userSlot.ClearSlot(); // Slot becomes empty

            // Act
            var action = await _provider.GetAction(_field, _userSlot);

            // Assert
            Assert.That(action, Is.Null);
        }

        [Test]
        public async Task GetAction_PokemonFaints_DuringBattle_ReturnsNull()
        {
            // Arrange
            _mockView.ActionTypeToReturn = BattleActionType.Fight;
            _userSlot.Pokemon.TakeDamage(_userSlot.Pokemon.MaxHP); // Pokemon faints

            // Act
            var action = await _provider.GetAction(_field, _userSlot);

            // Assert
            Assert.That(action, Is.Null);
        }

        #endregion

        #region Null Safety Tests

        [Test]
        public void GetAction_NullField_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _provider.GetAction(null, _userSlot));
        }

        [Test]
        public void GetAction_NullSlot_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _provider.GetAction(_field, null));
        }

        [Test]
        public void Constructor_NullView_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new PlayerInputProvider(null));
        }

        #endregion
    }
}

