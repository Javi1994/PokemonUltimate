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
    /// Tests for FlinchEffectProcessor - processes FlinchEffect (applies flinch status to target).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class FlinchEffectProcessorTests
    {
        private FlinchEffectProcessor _processor;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;
        private List<BattleAction> _actions;

        [SetUp]
        public void SetUp()
        {
            var randomProvider = new RandomProvider(42); // Fixed seed for reproducible tests
            _processor = new FlinchEffectProcessor(randomProvider);
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
        public void Process_FlinchEffectWith100PercentChance_AppliesFlinch()
        {
            // Arrange
            var effect = new FlinchEffect { ChancePercent = 100 };
            bool hadFlinchBefore = _targetSlot.HasVolatileStatus(VolatileStatus.Flinch);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Flinch), Is.True);
            Assert.That(_actions, Is.Empty); // Flinch is applied directly, no action added
        }

        [Test]
        public void Process_FlinchEffectWithLowChance_MayNotApplyFlinch()
        {
            // Arrange
            var effect = new FlinchEffect { ChancePercent = 0 };
            var randomProvider = new RandomProvider(42); // Fixed seed
            var processor = new FlinchEffectProcessor(randomProvider);

            // Act
            processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert - With 0% chance, should not apply flinch
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Flinch), Is.False);
        }

        [Test]
        public void Process_NonFlinchEffect_DoesNotApplyFlinch()
        {
            // Arrange
            var effect = new StatusEffect(PersistentStatus.Paralysis);

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Flinch), Is.False);
        }

        [Test]
        public void Process_FlinchEffect_DoesNotAddActions()
        {
            // Arrange
            var effect = new FlinchEffect { ChancePercent = 100 };

            // Act
            _processor.Process(effect, _userSlot, _targetSlot, _move, _field, 0, _actions);

            // Assert - Flinch is applied directly to slot, no action needed
            Assert.That(_actions, Is.Empty);
        }

        #endregion
    }
}
