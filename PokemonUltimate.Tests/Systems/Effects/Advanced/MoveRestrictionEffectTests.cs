using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for MoveRestrictionEffect behavior.
    /// </summary>
    [TestFixture]
    public class MoveRestrictionEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultDuration()
        {
            var effect = new MoveRestrictionEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.MoveRestriction));
            Assert.That(effect.Duration, Is.EqualTo(3));
        }
        
        [Test]
        public void Constructor_Encore_ForcesRepeat()
        {
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Encore, 3);
            
            Assert.That(effect.RestrictionType, Is.EqualTo(MoveRestrictionType.Encore));
        }
        
        [Test]
        public void Constructor_Taunt_BlocksStatus()
        {
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Taunt, 3);
            
            Assert.That(effect.Description, Does.Contain("damaging"));
        }
        
        [Test]
        public void Constructor_Imprison_AffectsSelf()
        {
            var effect = new MoveRestrictionEffect(MoveRestrictionType.Imprison)
            {
                TargetSelf = true
            };
            
            Assert.That(effect.TargetSelf, Is.True);
        }
    }
}

