using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for SelfDestructEffect behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.2: MoveData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class SelfDestructEffectTests
    {
        [Test]
        public void Constructor_Default_IsExplosion()
        {
            var effect = new SelfDestructEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.SelfDestruct));
            Assert.That(effect.Type, Is.EqualTo(SelfDestructType.Explosion));
            Assert.That(effect.DealsDamage, Is.True);
        }
        
        [Test]
        public void Constructor_Memento_DoesNotDealDamage()
        {
            var effect = new SelfDestructEffect(SelfDestructType.Memento);
            
            Assert.That(effect.DealsDamage, Is.False);
        }
        
        [Test]
        public void Constructor_HealingWish_HealsReplacement()
        {
            var effect = new SelfDestructEffect(SelfDestructType.HealingWish);
            
            Assert.That(effect.HealsReplacement, Is.True);
            Assert.That(effect.DealsDamage, Is.False);
        }
        
        [Test]
        public void Constructor_LunarDance_RestoresPP()
        {
            var effect = new SelfDestructEffect(SelfDestructType.LunarDance);
            
            Assert.That(effect.RestoresPP, Is.True);
            Assert.That(effect.HealsReplacement, Is.True);
        }
        
        [Test]
        public void Constructor_FinalGambit_DamageEqualsHP()
        {
            var effect = new SelfDestructEffect(SelfDestructType.FinalGambit);
            
            Assert.That(effect.DamageEqualsHP, Is.True);
            Assert.That(effect.DealsDamage, Is.True);
        }
    }
}

