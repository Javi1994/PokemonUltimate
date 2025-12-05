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
    /// Tests for FollowMeResolver - resolves Follow Me and Rage Powder redirection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    [TestFixture]
    public class FollowMeResolverTests
    {
        private FollowMeResolver _resolver;
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveData _move;

        [SetUp]
        public void SetUp()
        {
            _resolver = new FollowMeResolver();
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles,
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _move = MoveCatalog.Thunderbolt;
        }

        #region ResolveRedirection Tests

        [Test]
        public void ResolveRedirection_NoFollowMe_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _move, _field);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveRedirection_MultipleTargets_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot, _userSlot };
            _targetSlot.AddVolatileStatus(VolatileStatus.FollowMe);

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _move, _field);

            // Assert - Only redirects single-target moves
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveRedirection_NullParameters_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };

            // Act & Assert
            Assert.That(_resolver.ResolveRedirection(null, originalTargets, _move, _field), Is.Null);
            Assert.That(_resolver.ResolveRedirection(_userSlot, null, _move, _field), Is.Null);
            Assert.That(_resolver.ResolveRedirection(_userSlot, originalTargets, null, _field), Is.Null);
            Assert.That(_resolver.ResolveRedirection(_userSlot, originalTargets, _move, null), Is.Null);
        }

        [Test]
        public void ResolveRedirection_TargetAlreadyHasFollowMe_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };
            _targetSlot.AddVolatileStatus(VolatileStatus.FollowMe);

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _move, _field);

            // Assert - Follow Me doesn't redirect moves already targeting the user
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveRedirection_WithRagePowder_ReturnsNull()
        {
            // Arrange
            var originalTargets = new List<BattleSlot> { _targetSlot };
            _targetSlot.AddVolatileStatus(VolatileStatus.RagePowder);

            // Act
            var result = _resolver.ResolveRedirection(_userSlot, originalTargets, _move, _field);

            // Assert - Similar to Follow Me, doesn't redirect if already targeting
            Assert.That(result, Is.Null);
        }

        #endregion
    }
}
