using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for VolatileStatusEffect behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.2: MoveData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class VolatileStatusEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new VolatileStatusEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.VolatileStatus));
            Assert.That(effect.ChancePercent, Is.EqualTo(100));
            Assert.That(effect.TargetSelf, Is.False);
            Assert.That(effect.Duration, Is.EqualTo(0));
        }
        
        [Test]
        public void Constructor_WithParameters_SetsValues()
        {
            var effect = new VolatileStatusEffect(VolatileStatus.Confusion, 50);
            
            Assert.That(effect.Status, Is.EqualTo(VolatileStatus.Confusion));
            Assert.That(effect.ChancePercent, Is.EqualTo(50));
        }
        
        [Test]
        public void Description_Confusion_DescribesEffect()
        {
            var effect = new VolatileStatusEffect(VolatileStatus.Confusion, 100);
            
            Assert.That(effect.Description, Does.Contain("Confusion"));
            Assert.That(effect.Description, Does.Contain("100%"));
        }
    }
}

