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

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Tests for DamageAction - applies damage to a target Pokemon.
    /// </summary>
    [TestFixture]
    public class DamageActionTests
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
            var playerSide = new BattleSide(1, isPlayer: true);
            var enemySide = new BattleSide(1, isPlayer: false);
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            _defender = _defenderSlot.Pokemon;
            _move = MoveCatalog.Thunderbolt;
        }

        #region ExecuteLogic Tests

        [Test]
        public void ExecuteLogic_DealsDamage_ReducesHP()
        {
            int initialHP = _defender.CurrentHP;
            int damage = 50;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP - damage));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_TriggersFaint_WhenHPReachesZero()
        {
            _defender.CurrentHP = 30;
            int damage = 50;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field).ToList();

            Assert.That(_defender.CurrentHP, Is.EqualTo(0));
            Assert.That(_defender.IsFainted, Is.True);
            Assert.That(reactions, Has.Count.EqualTo(1));
            Assert.That(reactions[0], Is.InstanceOf<FaintAction>());
        }

        [Test]
        public void ExecuteLogic_DoesNotOverkill()
        {
            _defender.CurrentHP = 20;
            int damage = 100;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            action.ExecuteLogic(_field);

            Assert.That(_defender.CurrentHP, Is.EqualTo(0));
            Assert.That(_defender.IsFainted, Is.True);
        }

        [Test]
        public void ExecuteLogic_ZeroDamage_DoesNothing()
        {
            int initialHP = _defender.CurrentHP;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 0;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP));
            Assert.That(reactions, Is.Empty);
        }

        [Test]
        public void ExecuteLogic_ImmuneMove_DealsNoDamage()
        {
            int initialHP = _defender.CurrentHP;

            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 50;
            context.Multiplier = 1.0f;
            context.TypeEffectiveness = 0f; // Immune

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            var reactions = action.ExecuteLogic(_field);

            Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP));
            Assert.That(reactions, Is.Empty);
        }

        #endregion

        #region ExecuteVisual Tests

        [Test]
        public void ExecuteVisual_CallsPlayDamageAnimation()
        {
            var view = new MockBattleView();
            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 50;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.DamageAnimationCalled, Is.True);
            Assert.That(view.DamageAnimationSlot, Is.EqualTo(_defenderSlot));
        }

        [Test]
        public void ExecuteVisual_CallsUpdateHPBar()
        {
            var view = new MockBattleView();
            var context = new DamageContext(_attackerSlot, _defenderSlot, _move, _field);
            context.BaseDamage = 50;
            context.Multiplier = 1.0f;

            var action = new DamageAction(_attackerSlot, _defenderSlot, context);
            action.ExecuteVisual(view).Wait();

            Assert.That(view.HPBarUpdateCalled, Is.True);
            Assert.That(view.HPBarUpdateSlot, Is.EqualTo(_defenderSlot));
        }

        #endregion
    }

    /// <summary>
    /// Mock implementation of IBattleView for testing.
    /// </summary>
    internal class MockBattleView : IBattleView
    {
        public bool DamageAnimationCalled { get; private set; }
        public BattleSlot DamageAnimationSlot { get; private set; }
        public bool HPBarUpdateCalled { get; private set; }
        public BattleSlot HPBarUpdateSlot { get; private set; }

        public Task ShowMessage(string message) => Task.CompletedTask;

        public Task PlayDamageAnimation(BattleSlot slot)
        {
            DamageAnimationCalled = true;
            DamageAnimationSlot = slot;
            return Task.CompletedTask;
        }

        public Task UpdateHPBar(BattleSlot slot)
        {
            HPBarUpdateCalled = true;
            HPBarUpdateSlot = slot;
            return Task.CompletedTask;
        }

        public Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId) => Task.CompletedTask;

        public Task PlayFaintAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlayStatusAnimation(BattleSlot slot, string statusName) => Task.CompletedTask;

        public Task ShowStatChange(BattleSlot slot, string statName, int stages) => Task.CompletedTask;

        public Task PlaySwitchOutAnimation(BattleSlot slot) => Task.CompletedTask;

        public Task PlaySwitchInAnimation(BattleSlot slot) => Task.CompletedTask;
    }
}

