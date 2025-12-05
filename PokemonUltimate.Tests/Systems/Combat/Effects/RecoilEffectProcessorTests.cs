using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Effects;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Effects
{
    /// <summary>
    /// Tests for RecoilEffectProcessor - processes RecoilEffect (applies recoil damage to user).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class RecoilEffectProcessorTests
    {
        private RecoilEffectProcessor _processor;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            var damageContextFactory = new DamageContextFactory();
            _processor = new RecoilEffectProcessor(damageContextFactory);
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _move = MoveCatalog.Thunderbolt;
            _actions = new List<BattleAction>();
        }

        #region Process Tests

        [Test]
        public void Process_RecoilEffectWithDamage_AddsDamageAction()
        {
            // Arrange
            var effect = new RecoilEffect { RecoilPercent = 33 };
            const int damageDealt = 30;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<DamageAction>());
            var damageAction = (DamageAction)_actions[0];
            Assert.That(damageAction.Target, Is.EqualTo(_userSlot));
            Assert.That(damageAction.User, Is.EqualTo(_userSlot));
        }

        [Test]
        public void Process_RecoilEffectWithNoDamage_DoesNotAddAction()
        {
            // Arrange
            var effect = new RecoilEffect { RecoilPercent = 33 };
            const int damageDealt = 0;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Is.Empty);
        }

        [Test]
        public void Process_RecoilEffectCalculatesCorrectRecoil()
        {
            // Arrange
            var effect = new RecoilEffect { RecoilPercent = 33 };
            const int damageDealt = 30; // 33% of 30 = 10

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            var damageAction = (DamageAction)_actions[0];
            var context = damageAction.Context;
            Assert.That(context.BaseDamage, Is.GreaterThanOrEqualTo(1)); // At least 1 HP
        }

        [Test]
        public void Process_RecoilEffectMinimumDamage_AtLeastOneHP()
        {
            // Arrange
            var effect = new RecoilEffect { RecoilPercent = 1 };
            const int damageDealt = 1; // 1% of 1 = 0.01, should round to at least 1

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            var damageAction = (DamageAction)_actions[0];
            var context = damageAction.Context;
            Assert.That(context.BaseDamage, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Process_NonRecoilEffect_DoesNotAddAction()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 10, _actions);

            // Assert
            Assert.That(_actions, Is.Empty);
        }

        #endregion
    }
}
