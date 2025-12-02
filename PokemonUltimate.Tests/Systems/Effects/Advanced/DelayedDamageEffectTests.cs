using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for DelayedDamageEffect behavior.
    /// </summary>
    [TestFixture]
    public class DelayedDamageEffectTests
    {
        [Test]
        public void Constructor_Default_TwoTurnDelay()
        {
            var effect = new DelayedDamageEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.DelayedDamage));
            Assert.That(effect.TurnsDelay, Is.EqualTo(2));
            Assert.That(effect.IsHealing, Is.False);
        }
        
        [Test]
        public void Constructor_FutureSight_TargetsSlot()
        {
            var effect = new DelayedDamageEffect(2, false)
            {
                DamageType = PokemonType.Psychic,
                UsesCasterStats = true,
                BypassesProtect = true
            };
            
            Assert.That(effect.TargetsSlot, Is.True);
            Assert.That(effect.DamageType, Is.EqualTo(PokemonType.Psychic));
        }
        
        [Test]
        public void Constructor_Wish_Heals()
        {
            var effect = new DelayedDamageEffect(1, true)
            {
                HealFraction = 0.5f
            };
            
            Assert.That(effect.IsHealing, Is.True);
            Assert.That(effect.TurnsDelay, Is.EqualTo(1));
            Assert.That(effect.HealFraction, Is.EqualTo(0.5f));
        }
        
        [Test]
        public void Description_Damage_DescribesDamage()
        {
            var effect = new DelayedDamageEffect(2, false);
            
            Assert.That(effect.Description, Does.Contain("Damages"));
        }
        
        [Test]
        public void Description_Healing_DescribesHealing()
        {
            var effect = new DelayedDamageEffect(1, true);
            
            Assert.That(effect.Description, Does.Contain("Heals"));
        }
    }
}

