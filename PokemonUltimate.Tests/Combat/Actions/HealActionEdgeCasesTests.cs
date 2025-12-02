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
    /// Edge case tests for HealAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class HealActionEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _targetSlot;
        private PokemonInstance _pokemon;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _targetSlot = _field.PlayerSide.Slots[0];
            _pokemon = _targetSlot.Pokemon;
        }

        #region Null Validation Tests

        [Test]
        public void Constructor_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new HealAction(null, null, 50));
        }

        [Test]
        public void Constructor_NegativeAmount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                new HealAction(null, _targetSlot, -1));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var action = new HealAction(null, _targetSlot, 50);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var action = new HealAction(null, _targetSlot, 50);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_FullHP_DoesNotExceedMaxHP()
        {
            _pokemon.CurrentHP = _pokemon.MaxHP;
            int healAmount = 100;

            var action = new HealAction(null, _targetSlot, healAmount);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(_pokemon.MaxHP));
        }

        [Test]
        public void ExecuteLogic_HealingAmountGreaterThanMaxHP_ClampsToMaxHP()
        {
            _pokemon.CurrentHP = 1;
            int healAmount = _pokemon.MaxHP * 2; // Way more than needed

            var action = new HealAction(null, _targetSlot, healAmount);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(_pokemon.MaxHP));
        }

        [Test]
        public void ExecuteLogic_HealingFromZeroToMaxHP_ClampsCorrectly()
        {
            _pokemon.CurrentHP = 0;
            int healAmount = _pokemon.MaxHP * 2;

            var action = new HealAction(null, _targetSlot, healAmount);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(_pokemon.MaxHP));
        }

        [Test]
        public void ExecuteLogic_OneHPBelowMax_HealsToOneHP()
        {
            _pokemon.CurrentHP = _pokemon.MaxHP - 1;
            int healAmount = 1;

            var action = new HealAction(null, _targetSlot, healAmount);
            action.ExecuteLogic(_field);

            Assert.That(_pokemon.CurrentHP, Is.EqualTo(_pokemon.MaxHP));
        }

        [Test]
        public void ExecuteVisual_FullHP_SkipsVisual()
        {
            var view = new MockHealBattleView();
            _pokemon.CurrentHP = _pokemon.MaxHP;

            var action = new HealAction(null, _targetSlot, 0);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.HPBarUpdateCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_ZeroAmount_SkipsVisual()
        {
            var view = new MockHealBattleView();

            var action = new HealAction(null, _targetSlot, 0);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.HPBarUpdateCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_EmptySlot_SkipsVisual()
        {
            var view = new MockHealBattleView();
            var emptySlot = new BattleSlot(0);

            var action = new HealAction(null, emptySlot, 50);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.HPBarUpdateCalled, Is.False);
        }

        #endregion
    }
}

