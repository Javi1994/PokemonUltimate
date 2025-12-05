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
    /// Tests for StatChangeEffectProcessor - processes StatChangeEffect (modifies stat stages).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class StatChangeEffectProcessorTests
    {
        private StatChangeEffectProcessor _processor;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            var randomProvider = new RandomProvider(42); // Fixed seed for reproducible tests
            _processor = new StatChangeEffectProcessor(randomProvider);
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
        public void Process_StatChangeEffectWith100PercentChance_AddsStatChangeAction()
        {
            // Arrange
            var effect = new StatChangeEffect(Stat.Attack, 2, targetSelf: false, chancePercent: 100);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<StatChangeAction>());
            var statAction = (StatChangeAction)_actions[0];
            Assert.That(statAction.Target, Is.EqualTo(_targetSlot));
            Assert.That(statAction.Stat, Is.EqualTo(Stat.Attack));
            Assert.That(statAction.Change, Is.EqualTo(2));
        }

        [Test]
        public void Process_StatChangeEffectTargetSelf_AppliesToUser()
        {
            // Arrange
            var effect = new StatChangeEffect(Stat.Attack, 2, targetSelf: true, chancePercent: 100);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            var statAction = (StatChangeAction)_actions[0];
            Assert.That(statAction.Target, Is.EqualTo(_userSlot));
        }

        [Test]
        public void Process_StatChangeEffectWithLowChance_MayNotAddAction()
        {
            // Arrange
            var effect = new StatChangeEffect(Stat.Attack, 2, chancePercent: 0);
            var randomProvider = new RandomProvider(42); // Fixed seed
            var processor = new StatChangeEffectProcessor(randomProvider);

            // Act
            processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert - With 0% chance, should not add action
            Assert.That(_actions, Is.Empty);
        }

        [Test]
        public void Process_NonStatChangeEffect_DoesNotAddAction()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Is.Empty);
        }

        #endregion
    }
}
