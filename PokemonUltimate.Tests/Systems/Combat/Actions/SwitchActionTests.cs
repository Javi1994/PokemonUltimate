using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for SwitchAction - switches a Pokemon in battle.
    /// </summary>
    [TestFixture]
    public class SwitchActionTests
    {
        private BattleField _field;
        private BattleSide _playerSide;
        private BattleSlot _activeSlot;
        private PokemonInstance _activePokemon;
        private PokemonInstance _benchPokemon;

        [SetUp]
        public void SetUp()
        {
            _activePokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            _benchPokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 50);

            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { _activePokemon, _benchPokemon },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });

            _playerSide = _field.PlayerSide;
            _activeSlot = _playerSide.Slots[0];
            // _activePokemon should be in slot 0, _benchPokemon should be in party[1]
            Assert.That(_activeSlot.Pokemon, Is.EqualTo(_activePokemon));
        }

        #region ExecuteLogic Tests

        [Test]
        public void ExecuteLogic_SwapsPokemon()
        {
            Assert.That(_activeSlot.Pokemon, Is.EqualTo(_activePokemon));
            Assert.That(_playerSide.Party, Contains.Item(_benchPokemon));
            Assert.That(_playerSide.Party, Contains.Item(_activePokemon));

            var action = new SwitchAction(_activeSlot, _benchPokemon);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_activeSlot.Pokemon, Is.EqualTo(_benchPokemon));
            // Both Pokemon should still be in party
            Assert.That(_playerSide.Party, Contains.Item(_activePokemon));
            Assert.That(_playerSide.Party, Contains.Item(_benchPokemon));
        }

        [Test]
        public void ExecuteLogic_ResetsBattleState()
        {
            // Set some battle state
            _activeSlot.ModifyStatStage(Stat.Attack, 2);
            _activePokemon.Status = PersistentStatus.Burn;

            var action = new SwitchAction(_activeSlot, _benchPokemon);
            action.ExecuteLogic(_field);

            // Battle state should be reset for the new Pokemon
            Assert.That(_activeSlot.GetStatStage(Stat.Attack), Is.EqualTo(0));
            // Note: Status persists, only volatile status and stat stages reset
        }

        [Test]
        public void ExecuteLogic_EmptySlot_DoesNothing()
        {
            var emptySlot = new BattleSlot(0);
            var action = new SwitchAction(emptySlot, _benchPokemon);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void SwitchAction_Priority_IsSix()
        {
            var action = new SwitchAction(_activeSlot, _benchPokemon);

            Assert.That(action.Priority, Is.EqualTo(6));
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsSwitchAnimations()
        {
            var view = new MockSwitchBattleView();
            var action = new SwitchAction(_activeSlot, _benchPokemon);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.SwitchOutCalled, Is.True);
            Assert.That(view.SwitchOutSlot, Is.EqualTo(_activeSlot));
            Assert.That(view.SwitchInCalled, Is.True);
            Assert.That(view.SwitchInSlot, Is.EqualTo(_activeSlot));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for switch testing.
    /// </summary>
    internal class MockSwitchBattleView : IBattleView
    {
        public bool SwitchOutCalled { get; private set; }
        public BattleSlot SwitchOutSlot { get; private set; }
        public bool SwitchInCalled { get; private set; }
        public BattleSlot SwitchInSlot { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task UpdateHPBar(BattleSlot slot) => Task.CompletedTask;

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;

        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;

        public Task PlaySwitchOutAnimation(BattleSlot slot)
        {
            SwitchOutCalled = true;
            SwitchOutSlot = slot;
            return Task.CompletedTask;
        }

        public Task PlaySwitchInAnimation(BattleSlot slot)
        {
            SwitchInCalled = true;
            SwitchInSlot = slot;
            return Task.CompletedTask;
        }
        public Task<BattleActionType> SelectActionType(BattleSlot slot) => Task.FromResult(BattleActionType.Fight);
        public Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves) => Task.FromResult(moves?.FirstOrDefault());
        public Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets) => Task.FromResult(validTargets?.FirstOrDefault());
        public Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon) => Task.FromResult(availablePokemon?.FirstOrDefault());
    }
}

