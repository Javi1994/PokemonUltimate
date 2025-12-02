using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for ForceSwitchEffect behavior.
    /// </summary>
    [TestFixture]
    public class ForceSwitchEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new ForceSwitchEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.ForceSwitch));
            Assert.That(effect.DealsDamage, Is.False);
            Assert.That(effect.RandomReplacement, Is.True);
        }
        
        [Test]
        public void Constructor_WithDamage_SetsDealsDamage()
        {
            var effect = new ForceSwitchEffect(dealsDamage: true);
            
            Assert.That(effect.DealsDamage, Is.True);
        }
        
        [Test]
        public void Description_WithDamage_DescribesDamageAndSwitch()
        {
            var effect = new ForceSwitchEffect(dealsDamage: true);
            
            Assert.That(effect.Description, Does.Contain("damage"));
            Assert.That(effect.Description, Does.Contain("switch"));
        }
    }
}

