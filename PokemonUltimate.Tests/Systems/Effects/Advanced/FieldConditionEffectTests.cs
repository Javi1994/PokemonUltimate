using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for FieldConditionEffect behavior.
    /// </summary>
    [TestFixture]
    public class FieldConditionEffectTests
    {
        [Test]
        public void Constructor_Default_SetsDefaultValues()
        {
            var effect = new FieldConditionEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.FieldCondition));
            Assert.That(effect.RemovesCondition, Is.False);
        }
        
        [Test]
        public void Weather_RainDance_SetsRain()
        {
            var effect = new FieldConditionEffect
            {
                ConditionType = FieldConditionType.Weather,
                WeatherToSet = Weather.Rain
            };
            
            Assert.That(effect.WeatherToSet, Is.EqualTo(Weather.Rain));
        }
        
        [Test]
        public void Hazard_StealthRock_SetsHazard()
        {
            var effect = new FieldConditionEffect
            {
                ConditionType = FieldConditionType.Hazard,
                HazardToSet = HazardType.StealthRock,
                TargetsUserSide = false
            };
            
            Assert.That(effect.HazardToSet, Is.EqualTo(HazardType.StealthRock));
            Assert.That(effect.TargetsUserSide, Is.False);
        }
        
        [Test]
        public void Description_Weather_DescribesWeather()
        {
            var effect = new FieldConditionEffect
            {
                ConditionType = FieldConditionType.Weather,
                WeatherToSet = Weather.Sun
            };
            
            Assert.That(effect.Description, Does.Contain("Sun"));
        }
        
        [Test]
        public void RemovesCondition_Defog_RemovesHazards()
        {
            var effect = new FieldConditionEffect
            {
                RemovesCondition = true,
                ConditionsToRemove = new[] { FieldConditionType.Hazard }
            };
            
            Assert.That(effect.RemovesCondition, Is.True);
            Assert.That(effect.Description, Does.Contain("Removes"));
        }
    }
}

