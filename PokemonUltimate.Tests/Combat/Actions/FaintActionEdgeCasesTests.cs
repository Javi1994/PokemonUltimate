using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Edge case tests for FaintAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class FaintActionEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _faintedSlot;
        private PokemonInstance _pokemon;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _faintedSlot = _field.PlayerSide.Slots[0];
            _pokemon = _faintedSlot.Pokemon;
            _pokemon.CurrentHP = 0;
        }

        #region Null Validation Tests

        [Test]
        public void Constructor_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new FaintAction(null, null));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var action = new FaintAction(null, _faintedSlot);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var action = new FaintAction(null, _faintedSlot);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_AlreadyFainted_HandlesGracefully()
        {
            Assert.That(_pokemon.IsFainted, Is.True);

            var action = new FaintAction(null, _faintedSlot);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_pokemon.IsFainted, Is.True);
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_EmptySlot_ReturnsEmpty()
        {
            var emptySlot = new BattleSlot(0);
            var action = new FaintAction(null, emptySlot);

            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteVisual_EmptySlot_SkipsAnimation()
        {
            var view = new MockFaintBattleView();
            var emptySlot = new BattleSlot(0);
            var action = new FaintAction(null, emptySlot);

            action.ExecuteVisual(view).Wait();

            Assert.That(view.FaintAnimationCalled, Is.False);
        }

        [Test]
        public void ExecuteLogic_NullUser_HandlesGracefully()
        {
            var action = new FaintAction(null, _faintedSlot);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(reactions, Is.Empty);
        }

        #endregion
    }
}

