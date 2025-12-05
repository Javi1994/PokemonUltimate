using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Effects.Strategies;

namespace PokemonUltimate.Tests.Systems.Core.Effects.Strategies
{
    /// <summary>
    /// Tests for MoveRestrictionDescriptionRegistry - move restriction description registry.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class MoveRestrictionDescriptionRegistryTests
    {
        #region GetDescription Tests

        [Test]
        public void GetDescription_Encore_ReturnsCorrectDescription()
        {
            // Arrange
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Encore, 3);

            // Act
            var description = MoveRestrictionDescriptionRegistry.GetDescription(effect.RestrictionType, effect.Duration);

            // Assert
            Assert.That(description, Does.Contain("repeat last move"));
            Assert.That(description, Does.Contain("3"));
        }

        [Test]
        public void GetDescription_Taunt_ReturnsCorrectDescription()
        {
            // Arrange
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Taunt, 3);

            // Act
            var description = MoveRestrictionDescriptionRegistry.GetDescription(effect.RestrictionType, effect.Duration);

            // Assert
            Assert.That(description, Does.Contain("damaging moves"));
            Assert.That(description, Does.Contain("3"));
        }

        [Test]
        public void GetDescription_Torment_ReturnsCorrectDescription()
        {
            // Arrange
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Torment, 0);

            // Act
            var description = MoveRestrictionDescriptionRegistry.GetDescription(effect.RestrictionType, effect.Duration);

            // Assert
            Assert.That(description, Does.Contain("consecutively"));
        }

        [Test]
        public void GetDescription_Imprison_ReturnsCorrectDescription()
        {
            // Arrange
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Imprison, 0);

            // Act
            var description = MoveRestrictionDescriptionRegistry.GetDescription(effect.RestrictionType, effect.Duration);

            // Assert
            Assert.That(description, Does.Contain("cannot use moves"));
        }

        [Test]
        public void GetDescription_AllTypes_ReturnNonEmptyDescriptions()
        {
            // Act & Assert - Verify all restriction types return valid descriptions
            foreach (MoveRestrictionType type in System.Enum.GetValues(typeof(MoveRestrictionType)))
            {
                var description = MoveRestrictionDescriptionRegistry.GetDescription(type, 3);
                Assert.That(description, Is.Not.Null);
                Assert.That(description, Is.Not.Empty, $"Description for {type} should not be empty");
            }
        }

        #endregion
    }
}
