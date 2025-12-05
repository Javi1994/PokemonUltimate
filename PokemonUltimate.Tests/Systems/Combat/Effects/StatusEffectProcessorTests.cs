using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Effects;
using PokemonUltimate.Combat.Providers;
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
    /// Tests for StatusEffectProcessor - processes StatusEffect (applies status conditions).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class StatusEffectProcessorTests
    {
        private StatusEffectProcessor _processor;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            var randomProvider = new RandomProvider(42); // Fixed seed for reproducible tests
            _processor = new StatusEffectProcessor(randomProvider);
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
        public void Process_StatusEffectWith100PercentChance_AddsApplyStatusAction()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis, chancePercent: 100);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<ApplyStatusAction>());
            var statusAction = (ApplyStatusAction)_actions[0];
            Assert.That(statusAction.Target, Is.EqualTo(_targetSlot));
            Assert.That(statusAction.Status, Is.EqualTo(PersistentStatus.Paralysis));
        }

        [Test]
        public void Process_StatusEffectTargetSelf_AppliesToUser()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Sleep, chancePercent: 100)
            {
                TargetSelf = true
            };

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            var statusAction = (ApplyStatusAction)_actions[0];
            Assert.That(statusAction.Target, Is.EqualTo(_userSlot));
        }

        [Test]
        public void Process_StatusEffectWithLowChance_MayNotAddAction()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis, chancePercent: 0);
            var randomProvider = new RandomProvider(42); // Fixed seed
            var processor = new StatusEffectProcessor(randomProvider);

            // Act
            processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert - With 0% chance, should not add action
            // Note: This depends on the random value, but with 0% chance it should never trigger
            Assert.That(_actions, Is.Empty);
        }

        [Test]
        public void Process_NonStatusEffect_DoesNotAddAction()
        {
            // Arrange
            var effect = new StatChangeEffect(Stat.Attack, 1);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Is.Empty);
        }

        #endregion
    }
}
