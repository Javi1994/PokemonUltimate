using System;
using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Effects;
using PokemonUltimate.Combat.Factories;
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
    /// Tests for MoveEffectProcessorRegistry - registry for move effect processors.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class MoveEffectProcessorRegistryTests
    {
        private MoveEffectProcessorRegistry _registry;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            var randomProvider = new RandomProvider(42);
            var damageContextFactory = new DamageContextFactory();
            _registry = new MoveEffectProcessorRegistry(randomProvider, damageContextFactory);
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _move = MoveCatalog.Thunderbolt;
            _actions = new List<BattleAction>();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_NullRandomProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var damageContextFactory = new DamageContextFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MoveEffectProcessorRegistry(null, damageContextFactory));
        }

        [Test]
        public void Constructor_NullDamageContextFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var randomProvider = new RandomProvider(42);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MoveEffectProcessorRegistry(randomProvider, null));
        }

        #endregion

        #region Process Tests

        [Test]
        public void Process_StatusEffect_ProcessesCorrectly()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis, chancePercent: 100);

            // Act
            var result = _registry.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<ApplyStatusAction>());
        }

        [Test]
        public void Process_StatChangeEffect_ProcessesCorrectly()
        {
            // Arrange
            var effect = new StatChangeEffect(Stat.Attack, 2, chancePercent: 100);

            // Act
            var result = _registry.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<StatChangeAction>());
        }

        [Test]
        public void Process_UnknownEffectType_ReturnsFalse()
        {
            // Arrange
            var unknownEffect = new UnknownTestEffect();

            // Act
            var result = _registry.Process(unknownEffect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_actions, Is.Empty);
        }

        [Test]
        public void Process_NullEffect_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Process(null, _userSlot, _targetSlot, _move, _field, 0, _actions));
        }

        [Test]
        public void Process_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Process(effect, null, _targetSlot, _move, _field, 0, _actions));
        }

        [Test]
        public void Process_NullTarget_ThrowsArgumentNullException()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Process(effect, _userSlot, null, _move, _field, 0, _actions));
        }

        [Test]
        public void Process_NullMove_ThrowsArgumentNullException()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Process(effect, _userSlot, _targetSlot, null, _field, 0, _actions));
        }

        [Test]
        public void Process_NullField_ThrowsArgumentNullException()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Process(effect, _userSlot, _targetSlot, _move, null, 0, _actions));
        }

        [Test]
        public void Process_NullActions_ThrowsArgumentNullException()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Process(effect, _userSlot, _targetSlot, _move, _field, 0, null));
        }

        #endregion

        #region Register Tests

        [Test]
        public void Register_CustomProcessor_ProcessesCorrectly()
        {
            // Arrange
            var customEffect = new UnknownTestEffect();
            var customProcessor = new TestEffectProcessor();
            _registry.Register(typeof(UnknownTestEffect), customProcessor);

            // Act
            var result = _registry.Process(customEffect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_actions, Has.Count.EqualTo(1));
        }

        [Test]
        public void Register_NullEffectType_ThrowsArgumentNullException()
        {
            // Arrange
            var processor = new TestEffectProcessor();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Register(null, processor));
        }

        [Test]
        public void Register_NullProcessor_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _registry.Register(typeof(StatusEffect), null));
        }

        #endregion

        #region Helper Classes

        private class UnknownTestEffect : IMoveEffect
        {
            public EffectType EffectType => EffectType.Status;
            public string Description => "Unknown test effect";
        }

        private class TestEffectProcessor : IMoveEffectProcessor
        {
            public void Process(
                IMoveEffect effect,
                BattleSlot user,
                BattleSlot target,
                MoveData move,
                BattleField field,
                int damageDealt,
                List<BattleAction> actions)
            {
                // Simple test processor that adds a message action
                actions.Add(new MessageAction("Test effect processed"));
            }
        }

        #endregion
    }
}
