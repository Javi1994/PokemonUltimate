using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Effects;
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
    /// Tests for DrainEffectProcessor - processes DrainEffect (heals user by percentage of damage dealt).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class DrainEffectProcessorTests
    {
        private DrainEffectProcessor _processor;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            _processor = new DrainEffectProcessor();
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
        public void Process_DrainEffectWithDamage_AddsHealAction()
        {
            // Arrange
            var effect = new DrainEffect { DrainPercent = 50 };
            const int damageDealt = 20;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<HealAction>());
            var healAction = (HealAction)_actions[0];
            Assert.That(healAction.Target, Is.EqualTo(_userSlot));
            Assert.That(healAction.User, Is.EqualTo(_userSlot));
        }

        [Test]
        public void Process_DrainEffectWithNoDamage_DoesNotAddAction()
        {
            // Arrange
            var effect = new DrainEffect { DrainPercent = 50 };
            const int damageDealt = 0;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Is.Empty);
        }

        [Test]
        public void Process_DrainEffectCalculatesCorrectHeal()
        {
            // Arrange
            var effect = new DrainEffect { DrainPercent = 50 };
            const int damageDealt = 20; // 50% of 20 = 10

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            var healAction = (HealAction)_actions[0];
            Assert.That(healAction.Amount, Is.EqualTo(10));
        }

        [Test]
        public void Process_DrainEffectMinimumHeal_AtLeastOneHP()
        {
            // Arrange
            var effect = new DrainEffect { DrainPercent = 1 };
            const int damageDealt = 1; // 1% of 1 = 0.01, should round to at least 1

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, damageDealt, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            var healAction = (HealAction)_actions[0];
            Assert.That(healAction.Amount, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Process_NonDrainEffect_DoesNotAddAction()
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
