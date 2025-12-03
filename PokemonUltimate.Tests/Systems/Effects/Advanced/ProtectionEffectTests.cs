using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for ProtectionEffect behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.2: MoveData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class ProtectionEffectTests
    {
        [Test]
        public void Constructor_Default_SetsFullProtection()
        {
            var effect = new ProtectionEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.Protection));
            Assert.That(effect.Type, Is.EqualTo(ProtectionType.Full));
            Assert.That(effect.SingleTargetOnly, Is.False);
        }
        
        [Test]
        public void Constructor_WithType_SetsType()
        {
            var effect = new ProtectionEffect(ProtectionType.WideGuard);
            
            Assert.That(effect.Type, Is.EqualTo(ProtectionType.WideGuard));
        }
        
        [Test]
        public void Description_Full_DescribesProtection()
        {
            var effect = new ProtectionEffect(ProtectionType.Full);
            
            Assert.That(effect.Description, Does.Contain("Protects"));
        }
        
        [Test]
        public void Description_WideGuard_DescribesSpreadMoves()
        {
            var effect = new ProtectionEffect(ProtectionType.WideGuard);
            
            Assert.That(effect.Description, Does.Contain("spread"));
        }
        
        [Test]
        public void ContactPenalty_KingsShield_LowersAttack()
        {
            var effect = new ProtectionEffect(ProtectionType.KingsShield)
            {
                ContactEffect = ContactPenalty.StatDrop,
                ContactStatDrop = Stat.Attack,
                ContactStatStages = -2
            };
            
            Assert.That(effect.ContactEffect, Is.EqualTo(ContactPenalty.StatDrop));
            Assert.That(effect.ContactStatDrop, Is.EqualTo(Stat.Attack));
            Assert.That(effect.ContactStatStages, Is.EqualTo(-2));
        }
    }
}

