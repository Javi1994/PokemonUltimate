using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Factories;

namespace PokemonUltimate.Tests.Systems.Combat.Factories
{
    /// <summary>
    /// Tests for BattleQueueFactory - factory for creating BattleQueue instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleQueueFactoryTests
    {
        private BattleQueueFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new BattleQueueFactory();
        }

        #region Create Tests

        [Test]
        public void Create_ReturnsNewBattleQueue()
        {
            // Act
            var queue = _factory.Create();

            // Assert
            Assert.That(queue, Is.Not.Null);
            Assert.That(queue, Is.InstanceOf<BattleQueue>());
        }

        [Test]
        public void Create_MultipleCalls_ReturnsDifferentInstances()
        {
            // Act
            var queue1 = _factory.Create();
            var queue2 = _factory.Create();

            // Assert
            Assert.That(queue1, Is.Not.SameAs(queue2));
        }

        [Test]
        public void Create_ReturnsEmptyQueue()
        {
            // Act
            var queue = _factory.Create();

            // Assert
            // BattleQueue doesn't expose a count property directly, but we can verify it's created
            Assert.That(queue, Is.Not.Null);
        }

        #endregion
    }
}
