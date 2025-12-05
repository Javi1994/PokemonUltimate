using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Helpers.TargetRedirectionResolvers;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Helpers
{
    /// <summary>
    /// Tests for LightningRodResolver - resolves Lightning Rod and Storm Drain redirection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class LightningRodResolverTests
    {
        private LightningRodResolver _resolver;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _electricMove;
        private MoveData _waterMove;
        private MoveData _normalMove;

        [SetUp]
        public void SetUp()
        {
            _resolver = new LightningRodResolver();
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _electricMove = MoveCatalog.Thunderbolt;
            _waterMove = MoveCatalog.WaterGun;
            _normalMove = MoveCatalog.Tackle;
        }

        #region ResolveRedirection Tests

        [Test]
        public void ResolveRedirection_NoLightningRod_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _electricMove, _field);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveRedirection_NonElectricMove_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _normalMove, _field);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveRedirection_MultipleTargets_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot, _userSlot };

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _electricMove, _field);

            // Assert - Only redirects single-target moves
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveRedirection_NullParameters_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };

            // Act & Assert
            Assert.That(_resolver.ResolveRedirection(null, originalTargets, _electricMove, _field), Is.Null);
            Assert.That(_resolver.ResolveRedirection(_userSlot, null, _electricMove, _field), Is.Null);
            Assert.That(_resolver.ResolveRedirection(_userSlot, originalTargets, null, _field), Is.Null);
            Assert.That(_resolver.ResolveRedirection(_userSlot, originalTargets, _electricMove, null), Is.Null);
        }

        [Test]
        public void ResolveRedirection_WaterMove_ChecksStormDrain()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _waterMove, _field);

            // Assert - Should check for Storm Drain, but none present
            Assert.That(result, Is.Null);
        }

        #endregion
    }
}
