using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for BindingEffect behavior.
    /// </summary>
    [TestFixture]
    public class BindingEffectTests
    {
        [Test]
        public void Constructor_Default_SetsStandardValues()
        {
            var effect = new BindingEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Binding));
            Assert.That(effect.MinTurns, Is.EqualTo(4));
            Assert.That(effect.MaxTurns, Is.EqualTo(5));
            Assert.That(effect.DamagePerTurn, Is.EqualTo(0.125f));
            Assert.That(effect.PreventsSwitch, Is.True);
        }
        
        [Test]
        public void Constructor_WithBindType_SetsType()
        {
            var effect = new BindingEffect("wrapped", 0.125f);
            
            Assert.That(effect.BindType, Is.EqualTo("wrapped"));
        }
        
        [Test]
        public void EnhancedDamage_BindingBand_IsHigher()
        {
            var effect = new BindingEffect();
            
            Assert.That(effect.EnhancedDamagePerTurn, Is.GreaterThan(effect.DamagePerTurn));
        }
    }
}

