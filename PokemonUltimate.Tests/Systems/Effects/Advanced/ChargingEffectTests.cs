using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for ChargingEffect behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.2: MoveData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class ChargingEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new ChargingEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Charging));
            Assert.That(effect.SemiInvulnerable, Is.False);
            Assert.That(effect.IsRechargeMove, Is.False);
        }
        
        [Test]
        public void SemiInvulnerable_Fly_IsFlying()
        {
            var effect = new ChargingEffect
            {
                SemiInvulnerable = true,
                InvulnerableState = SemiInvulnerableState.Flying,
                ChargeMessage = "{0} flew up high!"
            };
            
            Assert.That(effect.SemiInvulnerable, Is.True);
            Assert.That(effect.InvulnerableState, Is.EqualTo(SemiInvulnerableState.Flying));
        }
        
        [Test]
        public void SkipChargeInWeather_SolarBeam_SkipsInSun()
        {
            var effect = new ChargingEffect
            {
                SkipChargeInWeather = Weather.Sun
            };
            
            Assert.That(effect.SkipChargeInWeather, Is.EqualTo(Weather.Sun));
        }
        
        [Test]
        public void Description_RechargeMove_DescribesRecharge()
        {
            var effect = new ChargingEffect { IsRechargeMove = true };
            
            Assert.That(effect.Description, Does.Contain("recharge"));
        }
    }
}

