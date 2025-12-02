using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for RevengeEffect behavior.
    /// </summary>
    [TestFixture]
    public class RevengeEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new RevengeEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Revenge));
            Assert.That(effect.DamageMultiplier, Is.EqualTo(2.0f));
            Assert.That(effect.RequiresHit, Is.True);
        }
        
        [Test]
        public void Constructor_Counter_CountersPhysical()
        {
            var effect = new RevengeEffect(MoveCategory.Physical, 2.0f);
            
            Assert.That(effect.CountersCategory, Is.EqualTo(MoveCategory.Physical));
        }
        
        [Test]
        public void Constructor_MirrorCoat_CountersSpecial()
        {
            var effect = new RevengeEffect(MoveCategory.Special, 2.0f);
            
            Assert.That(effect.CountersCategory, Is.EqualTo(MoveCategory.Special));
        }
        
        [Test]
        public void Constructor_MetalBurst_CountersBothWithLowerMultiplier()
        {
            var effect = new RevengeEffect(null, 1.5f);
            
            Assert.That(effect.CountersCategory, Is.Null);
            Assert.That(effect.DamageMultiplier, Is.EqualTo(1.5f));
        }
        
        [Test]
        public void Description_Physical_DescribesPhysical()
        {
            var effect = new RevengeEffect(MoveCategory.Physical, 2.0f);
            
            Assert.That(effect.Description, Does.Contain("Physical"));
        }
    }
}

