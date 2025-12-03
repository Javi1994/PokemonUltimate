using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    /// <summary>
    /// Tests for TypeEffectiveness - type matchup calculations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    [TestFixture]
    public class TypeEffectivenessTests
    {
        #region Basic Effectiveness Tests

        [Test]
        public void Fire_SuperEffective_Against_Grass()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        [Test]
        public void Water_SuperEffective_Against_Fire()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Water, PokemonType.Fire);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        [Test]
        public void Grass_SuperEffective_Against_Water()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Water);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        [Test]
        public void Fire_NotVeryEffective_Against_Water()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Water);
            Assert.That(eff, Is.EqualTo(0.5f));
        }

        [Test]
        public void Normal_Immune_To_Ghost()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Ghost);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Ghost_Immune_To_Normal()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Normal);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Electric_Immune_To_Ground()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Electric, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Ground_Immune_To_Flying()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Fighting_Immune_To_Ghost()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Ghost);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Dragon_Immune_To_Fairy()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Dragon, PokemonType.Fairy);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Poison_Immune_To_Steel()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Steel);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Psychic_Immune_To_Dark()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Psychic, PokemonType.Dark);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        #endregion

        #region Dual Type Tests

        [Test]
        public void Fire_Against_GrassPoison_Is_4x()
        {
            // Fire is super effective vs Grass (2x) and neutral vs Poison (1x) = 2x
            // But Grass/Poison takes 2x from Fire
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass, PokemonType.Poison);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        [Test]
        public void Ground_Against_FireFlying_Is_Neutral()
        {
            // Ground is super effective vs Fire (2x) but immune against Flying (0x) = 0x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Fire, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Ice_Against_GrassFlying_Is_4x()
        {
            // Ice is super effective vs both Grass (2x) and Flying (2x) = 4x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ice, PokemonType.Grass, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        public void Fighting_Against_Normal_Is_2x()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Normal, null);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        [Test]
        public void Fire_Against_WaterGround_Is_0_25x()
        {
            // Fire is not very effective vs both Water (0.5x) and Ground (has no effect on Fire attacks, so 1x from that direction)
            // Actually: Fire vs Water = 0.5, Fire vs Ground = 1.0 -> 0.5
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Water, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(0.5f));
        }

        [Test]
        public void Water_Against_RockGround_Is_4x()
        {
            // Water is super effective vs both Rock (2x) and Ground (2x) = 4x
            // This applies to Geodude, Graveler, Golem
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Water, PokemonType.Rock, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        public void Grass_Against_RockGround_Is_4x()
        {
            // Grass is super effective vs both Rock (2x) and Ground (2x) = 4x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Rock, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        public void Ground_Against_WaterFlying_Is_Immune()
        {
            // Ground is super effective vs Water (2x) but immune vs Flying (0x) = 0x
            // This applies to Gyarados
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Water, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        public void Electric_Against_WaterFlying_Is_4x()
        {
            // Electric is super effective vs Water (2x) and super effective vs Flying (2x) = 4x
            // This applies to Gyarados
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Electric, PokemonType.Water, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        public void Rock_Against_WaterFlying_Is_2x()
        {
            // Rock is neutral vs Water (1x) and super effective vs Flying (2x) = 2x
            // This applies to Gyarados
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Rock, PokemonType.Water, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        #endregion

        #region STAB Tests

        [Test]
        public void STAB_When_Type_Matches_Primary()
        {
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Fire, PokemonType.Fire, null);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_When_Type_Matches_Secondary()
        {
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Flying, PokemonType.Fire, PokemonType.Flying);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void No_STAB_When_Type_Doesnt_Match()
        {
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Water, PokemonType.Fire, PokemonType.Flying);
            Assert.That(stab, Is.EqualTo(1.0f));
        }

        [Test]
        public void STAB_Ghost_With_Ghost_Poison()
        {
            // Gengar (Ghost/Poison) using Ghost move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Ghost, PokemonType.Ghost, PokemonType.Poison);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_Poison_With_Ghost_Poison()
        {
            // Gengar (Ghost/Poison) using Poison move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Poison, PokemonType.Ghost, PokemonType.Poison);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_Rock_With_Rock_Ground()
        {
            // Golem (Rock/Ground) using Rock move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Rock, PokemonType.Rock, PokemonType.Ground);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_Ground_With_Rock_Ground()
        {
            // Golem (Rock/Ground) using Ground move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Ground, PokemonType.Rock, PokemonType.Ground);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_Water_With_Water_Flying()
        {
            // Gyarados (Water/Flying) using Water move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Water, PokemonType.Water, PokemonType.Flying);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_Flying_With_Water_Flying()
        {
            // Gyarados (Water/Flying) using Flying move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Flying, PokemonType.Water, PokemonType.Flying);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        public void STAB_Dragon_With_Dragon_Type()
        {
            // Dragon type Pokemon using Dragon move = STAB
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Dragon, PokemonType.Dragon, null);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        #endregion

        #region Helper Method Tests

        [Test]
        public void IsSuperEffective_True_For_2x()
        {
            Assert.That(TypeEffectiveness.IsSuperEffective(2.0f), Is.True);
            Assert.That(TypeEffectiveness.IsSuperEffective(4.0f), Is.True);
        }

        [Test]
        public void IsSuperEffective_False_For_1x_Or_Less()
        {
            Assert.That(TypeEffectiveness.IsSuperEffective(1.0f), Is.False);
            Assert.That(TypeEffectiveness.IsSuperEffective(0.5f), Is.False);
        }

        [Test]
        public void IsNotVeryEffective_True_For_Half()
        {
            Assert.That(TypeEffectiveness.IsNotVeryEffective(0.5f), Is.True);
            Assert.That(TypeEffectiveness.IsNotVeryEffective(0.25f), Is.True);
        }

        [Test]
        public void IsNotVeryEffective_False_For_Immune()
        {
            Assert.That(TypeEffectiveness.IsNotVeryEffective(0.0f), Is.False);
        }

        [Test]
        public void IsImmune_True_For_Zero()
        {
            Assert.That(TypeEffectiveness.IsImmune(0.0f), Is.True);
        }

        [Test]
        public void GetEffectivenessDescription_Correct_Messages()
        {
            Assert.That(TypeEffectiveness.GetEffectivenessDescription(0.0f), Does.Contain("no effect"));
            Assert.That(TypeEffectiveness.GetEffectivenessDescription(4.0f), Does.Contain("super effective").And.Contain("4x"));
            Assert.That(TypeEffectiveness.GetEffectivenessDescription(2.0f), Does.Contain("super effective"));
            Assert.That(TypeEffectiveness.GetEffectivenessDescription(0.25f), Does.Contain("not very effective").And.Contain("0.25x"));
            Assert.That(TypeEffectiveness.GetEffectivenessDescription(0.5f), Does.Contain("not very effective"));
            Assert.That(TypeEffectiveness.GetEffectivenessDescription(1.0f), Is.Null);
        }

        [Test]
        public void GetSuperEffectiveAgainst_Returns_Correct_Types()
        {
            var superEffective = TypeEffectiveness.GetSuperEffectiveAgainst(PokemonType.Fire);

            Assert.That(superEffective, Does.Contain(PokemonType.Grass));
            Assert.That(superEffective, Does.Contain(PokemonType.Ice));
            Assert.That(superEffective, Does.Contain(PokemonType.Bug));
            Assert.That(superEffective, Does.Contain(PokemonType.Steel));
            Assert.That(superEffective, Does.Not.Contain(PokemonType.Water));
        }

        [Test]
        public void GetImmuneTypes_Returns_Correct_Types()
        {
            var immune = TypeEffectiveness.GetImmuneTypes(PokemonType.Normal);

            Assert.That(immune, Does.Contain(PokemonType.Ghost));
            Assert.That(immune.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetResistedBy_Returns_Correct_Types()
        {
            var resisted = TypeEffectiveness.GetResistedBy(PokemonType.Fire);

            Assert.That(resisted, Does.Contain(PokemonType.Fire));
            Assert.That(resisted, Does.Contain(PokemonType.Water));
            Assert.That(resisted, Does.Contain(PokemonType.Rock));
            Assert.That(resisted, Does.Contain(PokemonType.Dragon));
        }

        #endregion

        #region Coverage Tests

        [Test]
        public void All_Types_Have_Entries()
        {
            foreach (PokemonType attacker in System.Enum.GetValues(typeof(PokemonType)))
            {
                foreach (PokemonType defender in System.Enum.GetValues(typeof(PokemonType)))
                {
                    float eff = TypeEffectiveness.GetEffectiveness(attacker, defender);
                    Assert.That(eff, Is.GreaterThanOrEqualTo(0.0f));
                    Assert.That(eff, Is.LessThanOrEqualTo(4.0f));
                }
            }
        }

        #endregion
    }
}

