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
    /// Tests for HealEffectProcessor - processes HealEffect (heals user by percentage of max HP).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class HealEffectProcessorTests
    {
        private HealEffectProcessor _processor;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            _processor = new HealEffectProcessor();
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
        public void Process_HealEffect_AddsHealAction()
        {
            // Arrange
            var effect = new HealEffect { HealPercent = 50 };
            int maxHP = _userSlot.Pokemon.MaxHP;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_actions, Has.Count.EqualTo(1));
            Assert.That(_actions[0], Is.InstanceOf<HealAction>());
            var healAction = (HealAction)_actions[0];
            Assert.That(healAction.Target, Is.EqualTo(_userSlot));
            Assert.That(healAction.User, Is.EqualTo(_userSlot));
            Assert.That(healAction.Amount, Is.EqualTo(maxHP / 2)); // 50% of max HP
        }

        [Test]
        public void Process_HealEffect100Percent_HealsFullMaxHP()
        {
            // Arrange
            var effect = new HealEffect { HealPercent = 100 };
            int maxHP = _userSlot.Pokemon.MaxHP;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            var healAction = (HealAction)_actions[0];
            Assert.That(healAction.Amount, Is.EqualTo(maxHP));
        }

        [Test]
        public void Process_HealEffect25Percent_HealsQuarterMaxHP()
        {
            // Arrange
            var effect = new HealEffect { HealPercent = 25 };
            int maxHP = _userSlot.Pokemon.MaxHP;

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            var healAction = (HealAction)_actions[0];
            Assert.That(healAction.Amount, Is.EqualTo(maxHP / 4));
        }

        [Test]
        public void Process_NonHealEffect_DoesNotAddAction()
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
