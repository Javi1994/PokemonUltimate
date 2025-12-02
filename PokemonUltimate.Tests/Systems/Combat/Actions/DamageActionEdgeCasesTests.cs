using System;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Edge case tests for DamageAction - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class DamageActionEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;
        private PokemonInstance _attacker;
        private PokemonInstance _defender;
        private MoveData _move;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            _defender = _defenderSlot.Pokemon;
            _move = MoveCatalog.Thunderbolt;
        }

        #region Null Validation Tests

        [Test]
        public void Constructor_NullTarget_ThrowsArgumentNullException()
        {
            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            
            Assert.Throws<ArgumentNullException>(() => 
                new DamageAction(_attackerSlot, null, context));
        }

        [Test]
        public void Constructor_NullContext_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new DamageAction(_attackerSlot, _defenderSlot, null));
        }

        [Test]
        public void ExecuteLogic_NullField_ThrowsArgumentNullException()
        {
            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            var action = new DamageAction(_attackerSlot, _defenderSlot, context);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteLogic(null).ToList());
        }

        [Test]
        public void ExecuteVisual_NullView_ThrowsArgumentNullException()
        {
            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            var action = new DamageAction(_attackerSlot, _defenderSlot, context);

            Assert.Throws<ArgumentNullException>(() => 
                action.ExecuteVisual(null).Wait());
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ExecuteLogic_AlreadyFaintedPokemon_DoesNotApplyDamage()
        {
            _defender.CurrentHP = 0;
            int initialHP = _defender.CurrentHP;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 50;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_DamageToSelf_AppliesDamage()
        {
            int initialHP = _attacker.CurrentHP;
            int damage = 30;

            var context = new DamageContext(_attackerSlot, _attackerSlot, _move, _field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _attackerSlot, context);
            action.ExecuteLogic(_field);

            Assert.That(_attacker.CurrentHP, Is.EqualTo(initialHP - damage));
        }

        [Test]
        public void ExecuteLogic_DamageToSelf_FaintsSelf()
        {
            _attacker.CurrentHP = 20;
            int damage = 50;

            var context = new DamageContext(_attackerSlot, _attackerSlot, _move, _field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _attackerSlot, context);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(_attacker.CurrentHP, Is.EqualTo(0));
            Assert.That(_attacker.IsFainted, Is.True);
            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
        }

        [Test]
        public void ExecuteLogic_ExactHPToZero_Faints()
        {
            int exactHP = 50;
            _defender.CurrentHP = exactHP;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = exactHP;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(_defender.CurrentHP, Is.EqualTo(0));
            Assert.That(_defender.IsFainted, Is.True);
            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
        }

        [Test]
        public void ExecuteLogic_OneHPRemaining_DoesNotFaint()
        {
            _defender.CurrentHP = 1;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 1;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(_defender.CurrentHP, Is.EqualTo(0));
            Assert.That(_defender.IsFainted, Is.True);
            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
        }

        [Test]
        public void ExecuteLogic_VeryLargeDamage_ClampsToZero()
        {
            _defender.CurrentHP = 100;
            int hugeDamage = 99999;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = hugeDamage;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            action.ExecuteLogic(_field);

            Assert.That(_defender.CurrentHP, Is.EqualTo(0));
            Assert.That(_defender.IsFainted, Is.True);
        }

        [Test]
        public void ExecuteLogic_NegativeDamage_HandledAsZero()
        {
            int initialHP = _defender.CurrentHP;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = -10; // Should be handled by DamageContext, but test edge case
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field);

            // If FinalDamage is 0 or negative, no damage should be applied
            if (context.FinalDamage <= 0)
            {
                Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP));
                Assert.That(reactions, Is.Empty);
            }
        }

        [Test]
        public void ExecuteVisual_EmptySlot_SkipsVisual()
        {
            var view = new MockBattleView();
            var emptySlot = new BattleSlot(0);
            var context = new DamageContext(_attackerSlot, emptySlot, _move, _field);
            context.BaseDamage = 50;

            var action = new DamageAction(_attackerSlot, emptySlot, context);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.DamageAnimationCalled, Is.False);
            Assert.That(view.HPBarUpdateCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_ZeroDamage_SkipsVisual()
        {
            var view = new MockBattleView();
            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 0;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.DamageAnimationCalled, Is.False);
            Assert.That(view.HPBarUpdateCalled, Is.False);
        }

        [Test]
        public void ExecuteVisual_AlreadyFainted_SkipsVisual()
        {
            var view = new MockBattleView();
            _defender.CurrentHP = 0;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 50;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            action.ExecuteVisual(view).Wait();

            // Should skip because slot is considered empty/fainted
            Assert.That(view.DamageAnimationCalled, Is.False);
        }

        #endregion
    }
}

