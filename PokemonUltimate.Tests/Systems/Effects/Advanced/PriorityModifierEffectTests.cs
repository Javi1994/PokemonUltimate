using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Effects.Advanced
{
    /// <summary>
    /// Tests for PriorityModifierEffect behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.2: MoveData (Blueprint)
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PriorityModifierEffectTests
    {
        [Test]
        public void Constructor_Default_AlwaysCondition()
        {
            var effect = new PriorityModifierEffect();
            
            Assert.That(effect.EffectType, Is.EqualTo(EffectType.PriorityModifier));
            Assert.That(effect.Condition, Is.EqualTo(PriorityCondition.Always));
            Assert.That(effect.PriorityChange, Is.EqualTo(1));
        }
        
        [Test]
        public void TerrainBased_GrassyGlide_RequiresTerrain()
        {
            var effect = new PriorityModifierEffect
            {
                Condition = PriorityCondition.TerrainBased,
                RequiredTerrain = Terrain.Grassy,
                PriorityChange = 1
            };
            
            Assert.That(effect.RequiredTerrain, Is.EqualTo(Terrain.Grassy));
        }
        
        [Test]
        public void FullHP_GaleWings_RequiresFullHP()
        {
            var effect = new PriorityModifierEffect
            {
                Condition = PriorityCondition.FullHP,
                HPThreshold = 1.0f,
                PriorityChange = 1
            };
            
            Assert.That(effect.HPThreshold, Is.EqualTo(1.0f));
        }
        
        [Test]
        public void Description_Always_ShowsPriority()
        {
            var effect = new PriorityModifierEffect { PriorityChange = 2 };
            
            Assert.That(effect.Description, Does.Contain("+2"));
        }
    }
}

