using System;
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
    /// Edge case tests for SwitchAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class SwitchActionEdgeCasesTests
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
        }

        #region Null Validation Tests

        [Test]
        public void Constructor_NullSlot_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new SwitchAction(null, _benchPokemon));
        }

        [Test]
        public void Constructor_NullNewPokemon_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new SwitchAction(_activeSlot, null));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var action = new SwitchAction(_activeSlot, _benchPokemon);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var action = new SwitchAction(_activeSlot, _benchPokemon);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_ClearsVolatileStatus()
        {
            _activeSlot.AddVolatileStatus(VolatileStatus.Flinch);
            _activeSlot.AddVolatileStatus(VolatileStatus.Confusion);

            var action = new SwitchAction(_activeSlot, _benchPokemon);
            action.ExecuteLogic(_field);

            Assert.That(_activeSlot.HasVolatileStatus(VolatileStatus.Flinch), Is.False);
            Assert.That(_activeSlot.HasVolatileStatus(VolatileStatus.Confusion), Is.False);
        }

        [Test]
        public void ExecuteLogic_ResetsAllStatStages()
        {
            _activeSlot.ModifyStatStage(Stat.Attack, 3);
            _activeSlot.ModifyStatStage(Stat.Defense, -2);
            _activeSlot.ModifyStatStage(Stat.Speed, 1);
            _activeSlot.ModifyStatStage(Stat.Accuracy, -1);
            _activeSlot.ModifyStatStage(Stat.Evasion, 2);

            var action = new SwitchAction(_activeSlot, _benchPokemon);
            action.ExecuteLogic(_field);

            Assert.That(_activeSlot.GetStatStage(Stat.Attack), Is.EqualTo(0));
            Assert.That(_activeSlot.GetStatStage(Stat.Defense), Is.EqualTo(0));
            Assert.That(_activeSlot.GetStatStage(Stat.Speed), Is.EqualTo(0));
            Assert.That(_activeSlot.GetStatStage(Stat.Accuracy), Is.EqualTo(0));
            Assert.That(_activeSlot.GetStatStage(Stat.Evasion), Is.EqualTo(0));
        }

        [Test]
        public void ExecuteLogic_PreservesPersistentStatus()
        {
            _activePokemon.Status = PersistentStatus.Burn;

            var action = new SwitchAction(_activeSlot, _benchPokemon);
            action.ExecuteLogic(_field);

            // Status should persist on the old Pokemon (not on the new one)
            // The new Pokemon should have no status
            Assert.That(_activeSlot.Pokemon.Status, Is.EqualTo(PersistentStatus.None));
        }

        [Test]
        public void ExecuteLogic_EmptySlot_ReturnsEmpty()
        {
            var emptySlot = new BattleSlot(0);
            var action = new SwitchAction(emptySlot, _benchPokemon);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteVisual_EmptySlot_SkipsAnimations()
        {
            var view = new MockSwitchBattleView();
            var emptySlot = new BattleSlot(0);
            var action = new SwitchAction(emptySlot, _benchPokemon);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.SwitchOutCalled, Is.False);
            Assert.That(view.SwitchInCalled, Is.False);
        }

        [Test]
        public void Priority_IsSix()
        {
            var action = new SwitchAction(_activeSlot, _benchPokemon);

            Assert.That(action.Priority, Is.EqualTo(6));
        }

        [Test]
        public void CanBeBlocked_IsFalse()
        {
            var action = new SwitchAction(_activeSlot, _benchPokemon);

            Assert.That(action.CanBeBlocked, Is.False);
        }

        #endregion
    }
}

