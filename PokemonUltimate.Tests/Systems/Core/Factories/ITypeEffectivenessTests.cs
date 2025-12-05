using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    /// <summary>
    /// Tests for ITypeEffectiveness interface implementation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    [TestFixture]
    public class ITypeEffectivenessTests
    {
        private ITypeEffectiveness _typeEffectiveness;

        [SetUp]
        public void SetUp()
        {
            _typeEffectiveness = TypeEffectiveness.Default;
        }

        #region Interface Implementation Tests

        [Test]
        public void TypeEffectiveness_ImplementsITypeEffectiveness()
        {
            // Assert
            Assert.That(_typeEffectiveness, Is.InstanceOf<ITypeEffectiveness>());
        }

        [Test]
        public void GetEffectiveness_ThroughInterface_ReturnsCorrectValue()
        {
            // Act
            var effectiveness = _typeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass);

            // Assert
            Assert.That(effectiveness, Is.GreaterThan(1.0f)); // Fire is super effective against Grass
        }

        [Test]
        public void GetSTABMultiplier_ThroughInterface_ReturnsCorrectValue()
        {
            // Act
            var stab = _typeEffectiveness.GetSTABMultiplier(PokemonType.Fire, PokemonType.Fire, null);

            // Assert
            Assert.That(stab, Is.EqualTo(1.5f)); // STAB multiplier
        }

        [Test]
        public void IsSuperEffective_ThroughInterface_ReturnsCorrectValue()
        {
            // Act
            var isSuperEffective = _typeEffectiveness.IsSuperEffective(2.0f);

            // Assert
            Assert.That(isSuperEffective, Is.True);
        }

        #endregion
    }
}
