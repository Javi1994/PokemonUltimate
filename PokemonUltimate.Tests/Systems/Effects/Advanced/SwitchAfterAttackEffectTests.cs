using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for SwitchAfterAttackEffect behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.2: MoveData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class SwitchAfterAttackEffectTests
    {
        [Test]
        public void Constructor_Default_DealsDamage()
        {
            var effect = new SwitchAfterAttackEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.SwitchAfterAttack));
            Assert.That(effect.DealsDamage, Is.True);
            Assert.That(effect.MandatorySwitch, Is.True);
        }
        
        [Test]
        public void Constructor_WithoutDamage_OnlySwitches()
        {
            var effect = new SwitchAfterAttackEffect(dealsDamage: false);
            
            Assert.That(effect.DealsDamage, Is.False);
        }
        
        [Test]
        public void StatChanges_PartingShot_LowersStats()
        {
            var effect = new SwitchAfterAttackEffect(dealsDamage: false)
            {
                StatChanges = new[]
                {
                    new StatChangeEffect(Stat.Attack, -1),
                    new StatChangeEffect(Stat.SpAttack, -1)
                }
            };
            
            Assert.That(effect.StatChanges.Length, Is.EqualTo(2));
        }
    }
}

