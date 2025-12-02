using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Edge case tests for StatChangeAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class StatChangeActionEdgeCasesTests
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
                new StatChangeAction(null, null, Stat.Attack, 2));
        }

        [Test]
        public void Constructor_HPStat_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                new StatChangeAction(null, _targetSlot, Stat.HP, 2));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 2);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 2);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_AllStatTypes_CanBeModified()
        {
            var stats = new[]
            {
                Stat.Attack,
                Stat.Defense,
                Stat.SpAttack,
                Stat.SpDefense,
                Stat.Speed,
                Stat.Accuracy,
                Stat.Evasion
            };

            foreach (var stat in stats)
            {
                _targetSlot.ModifyStatStage(stat, 0); // Reset
                var action = new StatChangeAction(null, _targetSlot, stat, 1);
                action.ExecuteLogic(_field);
                
                Assert.That(_targetSlot.GetStatStage(stat), Is.EqualTo(1), 
                    $"Failed to modify {stat}");
            }
        }

        [Test]
        public void ExecuteLogic_MaxStagePlusOne_ClampsToSix()
        {
            _targetSlot.ModifyStatStage(Stat.Attack, 6);

            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 1);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Attack), Is.EqualTo(6));
        }

        [Test]
        public void ExecuteLogic_MinStageMinusOne_ClampsToNegativeSix()
        {
            _targetSlot.ModifyStatStage(Stat.Defense, -6);

            var action = new StatChangeAction(null, _targetSlot, Stat.Defense, -1);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Defense), Is.EqualTo(-6));
        }

        [Test]
        public void ExecuteLogic_LargePositiveChange_ClampsCorrectly()
        {
            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 100);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Attack), Is.EqualTo(6));
        }

        [Test]
        public void ExecuteLogic_LargeNegativeChange_ClampsCorrectly()
        {
            var action = new StatChangeAction(null, _targetSlot, Stat.Defense, -100);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Defense), Is.EqualTo(-6));
        }

        [Test]
        public void ExecuteLogic_MultipleChanges_StackCorrectly()
        {
            var action1 = new StatChangeAction(null, _targetSlot, Stat.Attack, 2);
            var action2 = new StatChangeAction(null, _targetSlot, Stat.Attack, 3);
            
            action1.ExecuteLogic(_field);
            action2.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Attack), Is.EqualTo(5));
        }

        [Test]
        public void ExecuteLogic_FaintedPokemon_StillModifiesStats()
        {
            _pokemon.CurrentHP = 0;
            
            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 2);
            action.ExecuteLogic(_field);

            Assert.That(_targetSlot.GetStatStage(Stat.Attack), Is.EqualTo(2));
        }

        [Test]
        public void ExecuteVisual_ZeroChange_SkipsVisual()
        {
            var view = new MockStatChangeBattleView();

            var action = new StatChangeAction(null, _targetSlot, Stat.Attack, 0);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatChangeCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_EmptySlot_SkipsVisual()
        {
            var view = new MockStatChangeBattleView();
            var emptySlot = new BattleSlot(0);

            var action = new StatChangeAction(null, emptySlot, Stat.Attack, 2);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.StatChangeCalled, Is.False);
        }

        #endregion
    }
}

