using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Factories
{
    /// <summary>
    /// Comprehensive edge case tests for type effectiveness.
    /// These tests verify critical battle mechanics using real Pokemon matchups.
    /// </summary>
    [TestFixture]
    public class TypeEffectivenessEdgeCasesTests
    {
        #region All Immunities (9 total in Gen6+)

        [Test]
        [Description("Normal-type moves have no effect on Ghost-types")]
        public void Immunity_Normal_Vs_Ghost()
        {
            // Example: Hyper Beam vs Gengar
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Ghost);
            Assert.That(eff, Is.EqualTo(0.0f));
            Assert.That(TypeEffectiveness.IsImmune(eff), Is.True);
        }

        [Test]
        [Description("Ghost-type moves have no effect on Normal-types")]
        public void Immunity_Ghost_Vs_Normal()
        {
            // Example: Shadow Ball vs Snorlax
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Normal);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Fighting-type moves have no effect on Ghost-types")]
        public void Immunity_Fighting_Vs_Ghost()
        {
            // Example: Close Combat vs Gengar
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Ghost);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Electric-type moves have no effect on Ground-types")]
        public void Immunity_Electric_Vs_Ground()
        {
            // Example: Thunderbolt vs Garchomp
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Electric, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Ground-type moves have no effect on Flying-types")]
        public void Immunity_Ground_Vs_Flying()
        {
            // Example: Earthquake vs Charizard
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Poison-type moves have no effect on Steel-types")]
        public void Immunity_Poison_Vs_Steel()
        {
            // Example: Sludge Bomb vs Metagross
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Steel);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Psychic-type moves have no effect on Dark-types")]
        public void Immunity_Psychic_Vs_Dark()
        {
            // Example: Psychic vs Tyranitar
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Psychic, PokemonType.Dark);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Dragon-type moves have no effect on Fairy-types")]
        public void Immunity_Dragon_Vs_Fairy()
        {
            // Example: Draco Meteor vs Clefable
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Dragon, PokemonType.Fairy);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Normal and Fighting-type moves have no effect on Ghost-types")]
        public void All_Physical_Immunities_Vs_Ghost()
        {
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Ghost), Is.EqualTo(0.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Ghost), Is.EqualTo(0.0f));
        }

        #endregion

        #region 4x Super Effective (Critical Matchups)

        [Test]
        [Description("Ice vs Dragonite (Dragon/Flying) = 4x")]
        public void FourX_Ice_Vs_Dragonite()
        {
            // Dragonite is Dragon/Flying, Ice is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ice, PokemonType.Dragon, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Rock vs Charizard (Fire/Flying) = 4x")]
        public void FourX_Rock_Vs_Charizard()
        {
            // Charizard is Fire/Flying, Rock is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Rock, PokemonType.Fire, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Ground vs Magnezone (Electric/Steel) = 4x")]
        public void FourX_Ground_Vs_Magnezone()
        {
            // Magnezone is Electric/Steel, Ground is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Electric, PokemonType.Steel);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Fire vs Ferrothorn (Grass/Steel) = 4x")]
        public void FourX_Fire_Vs_Ferrothorn()
        {
            // Ferrothorn is Grass/Steel, Fire is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass, PokemonType.Steel);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Fighting vs Tyranitar (Rock/Dark) = 4x")]
        public void FourX_Fighting_Vs_Tyranitar()
        {
            // Tyranitar is Rock/Dark, Fighting is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Rock, PokemonType.Dark);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Ice vs Garchomp (Dragon/Ground) = 4x")]
        public void FourX_Ice_Vs_Garchomp()
        {
            // Garchomp is Dragon/Ground, Ice is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ice, PokemonType.Dragon, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Water vs Rhyperior (Ground/Rock) = 4x")]
        public void FourX_Water_Vs_Rhyperior()
        {
            // Rhyperior is Ground/Rock, Water is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Water, PokemonType.Ground, PokemonType.Rock);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        [Test]
        [Description("Grass vs Quagsire (Water/Ground) = 4x")]
        public void FourX_Grass_Vs_Quagsire()
        {
            // Quagsire is Water/Ground, Grass is 2x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Water, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(4.0f));
        }

        #endregion

        #region 0.25x Resistances (Double Resistance)

        [Test]
        [Description("Fighting vs Toxicroak (Poison/Fighting) - Poison resists Fighting")]
        public void HalfX_Fighting_Vs_Toxicroak()
        {
            // Toxicroak is Poison/Fighting
            // Fighting vs Poison = 0.5x, Fighting vs Fighting = 1.0x
            // Combined = 0.5x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Poison, PokemonType.Fighting);
            Assert.That(eff, Is.EqualTo(0.5f));
        }

        [Test]
        [Description("Grass vs Crobat (Poison/Flying) = 0.25x")]
        public void QuarterX_Grass_Vs_Crobat()
        {
            // Crobat is Poison/Flying, Grass is 0.5x vs both
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Poison, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(0.25f));
        }

        [Test]
        [Description("Fire vs Heatran (Fire/Steel) - Fire resists but Steel is weak")]
        public void Neutral_Fire_Vs_Heatran()
        {
            // Heatran is Fire/Steel
            // Fire vs Fire = 0.5x (resisted)
            // Fire vs Steel = 2.0x (super effective!)
            // Combined = 0.5 * 2.0 = 1.0x (neutral)
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Fire, PokemonType.Steel);
            Assert.That(eff, Is.EqualTo(1.0f));
        }

        [Test]
        [Description("Bug vs Venomoth (Bug/Poison) - Poison resists Bug")]
        public void HalfX_Bug_Vs_Venomoth()
        {
            // Bug vs Bug = 1.0x (neutral)
            // Bug vs Poison = 0.5x (resisted)
            // Combined = 0.5x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Bug, PokemonType.Bug, PokemonType.Poison);
            Assert.That(eff, Is.EqualTo(0.5f));
        }

        [Test]
        [Description("Fire vs Reshiram (Dragon/Fire) = 0.25x - double resist")]
        public void QuarterX_Fire_Vs_Reshiram()
        {
            // Fire vs Dragon = 0.5x, Fire vs Fire = 0.5x
            // Combined = 0.25x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Dragon, PokemonType.Fire);
            Assert.That(eff, Is.EqualTo(0.25f));
        }

        [Test]
        [Description("Water vs Kingdra (Water/Dragon) = 0.25x - double resist")]
        public void QuarterX_Water_Vs_Kingdra()
        {
            // Water vs Water = 0.5x, Water vs Dragon = 0.5x
            // Combined = 0.25x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Water, PokemonType.Water, PokemonType.Dragon);
            Assert.That(eff, Is.EqualTo(0.25f));
        }

        #endregion

        #region Immunity Overrides Super Effective

        [Test]
        [Description("Ground vs Charizard: Immunity overrides Fire weakness")]
        public void Immunity_Overrides_Weakness_Charizard()
        {
            // Charizard (Fire/Flying): Ground is 2x vs Fire but 0x vs Flying = 0x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Fire, PokemonType.Flying);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Electric vs Swampert: Immunity from Ground type")]
        public void Immunity_Overrides_Weakness_Swampert()
        {
            // Swampert (Water/Ground): Electric is 2x vs Water but 0x vs Ground = 0x
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Electric, PokemonType.Water, PokemonType.Ground);
            Assert.That(eff, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Fighting vs Sableye: Both types immune")]
        public void Double_Immunity_Sableye()
        {
            // Sableye (Dark/Ghost): Fighting is 2x vs Dark but 0x vs Ghost = 0x
            // Normal is 0x vs Ghost = 0x
            float fighting = TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Dark, PokemonType.Ghost);
            float normal = TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Dark, PokemonType.Ghost);
            
            Assert.That(fighting, Is.EqualTo(0.0f));
            Assert.That(normal, Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Poison vs Aegislash (Steel/Ghost): Immune")]
        public void Immunity_Aegislash()
        {
            // Aegislash (Steel/Ghost): Immune to Poison (Steel) and Normal/Fighting (Ghost)
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Steel, PokemonType.Ghost), Is.EqualTo(0.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Steel, PokemonType.Ghost), Is.EqualTo(0.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Steel, PokemonType.Ghost), Is.EqualTo(0.0f));
        }

        #endregion

        #region Real Pokemon Matchups (Competitive)

        [Test]
        [Description("Landorus-T (Ground/Flying) key matchups")]
        public void Matchups_Landorus()
        {
            // Ground/Flying type
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ice, PokemonType.Ground, PokemonType.Flying), Is.EqualTo(4.0f), "Ice 4x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Water, PokemonType.Ground, PokemonType.Flying), Is.EqualTo(2.0f), "Water 2x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Electric, PokemonType.Ground, PokemonType.Flying), Is.EqualTo(0.0f), "Electric immune");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Ground, PokemonType.Flying), Is.EqualTo(0.0f), "Ground immune");
        }

        [Test]
        [Description("Scizor (Bug/Steel) key matchups")]
        public void Matchups_Scizor()
        {
            // Bug/Steel type - Only weakness is Fire (4x)
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Bug, PokemonType.Steel), Is.EqualTo(4.0f), "Fire 4x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Bug, PokemonType.Steel), Is.EqualTo(0.0f), "Poison immune");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Bug, PokemonType.Steel), Is.EqualTo(0.25f), "Grass 0.25x");
        }

        [Test]
        [Description("Rotom-Wash (Electric/Water) key matchups")]
        public void Matchups_RotomWash()
        {
            // Electric/Water type
            // Grass is 2x vs Water, 1x vs Electric = 2x total
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Electric, PokemonType.Water), Is.EqualTo(2.0f), "Grass 2x");
            // Ground is 2x vs Electric, 1x vs Water = 2x total (not neutral!)
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Electric, PokemonType.Water), Is.EqualTo(2.0f), "Ground 2x");
            // Flying is 0.5x vs Electric, 1x vs Water = 0.5x total
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Flying, PokemonType.Electric, PokemonType.Water), Is.EqualTo(0.5f), "Flying 0.5x");
            // Steel is 0.5x vs Electric, 0.5x vs Water = 0.25x total
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Steel, PokemonType.Electric, PokemonType.Water), Is.EqualTo(0.25f), "Steel 0.25x");
        }

        [Test]
        [Description("Gengar (Ghost/Poison) key matchups")]
        public void Matchups_Gengar()
        {
            // Ghost/Poison type
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Psychic, PokemonType.Ghost, PokemonType.Poison), Is.EqualTo(2.0f), "Psychic 2x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Ghost, PokemonType.Poison), Is.EqualTo(2.0f), "Ghost 2x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Ghost, PokemonType.Poison), Is.EqualTo(2.0f), "Ground 2x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Dark, PokemonType.Ghost, PokemonType.Poison), Is.EqualTo(2.0f), "Dark 2x");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Ghost, PokemonType.Poison), Is.EqualTo(0.0f), "Normal immune");
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Ghost, PokemonType.Poison), Is.EqualTo(0.0f), "Fighting immune");
        }

        [Test]
        [Description("Fairy type defensive matchups")]
        public void Matchups_Fairy_Defense()
        {
            // Fairy is weak to Poison and Steel
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Fairy), Is.EqualTo(2.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Steel, PokemonType.Fairy), Is.EqualTo(2.0f));
            
            // Fairy resists Fighting, Bug, Dark
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Fairy), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Bug, PokemonType.Fairy), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Dark, PokemonType.Fairy), Is.EqualTo(0.5f));
            
            // Fairy is immune to Dragon
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Dragon, PokemonType.Fairy), Is.EqualTo(0.0f));
        }

        #endregion

        #region Steel Type (Many Resistances)

        [Test]
        [Description("Steel type has 10 resistances")]
        public void Steel_Resistances()
        {
            var steelResists = new[] 
            {
                PokemonType.Normal, PokemonType.Grass, PokemonType.Ice,
                PokemonType.Flying, PokemonType.Psychic, PokemonType.Bug,
                PokemonType.Rock, PokemonType.Dragon, PokemonType.Steel, PokemonType.Fairy
            };

            foreach (var type in steelResists)
            {
                float eff = TypeEffectiveness.GetEffectiveness(type, PokemonType.Steel);
                Assert.That(eff, Is.EqualTo(0.5f), $"{type} should be resisted by Steel");
            }
        }

        [Test]
        [Description("Steel type has 1 immunity")]
        public void Steel_Immunity()
        {
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Steel), Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Steel type weaknesses")]
        public void Steel_Weaknesses()
        {
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Steel), Is.EqualTo(2.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Steel), Is.EqualTo(2.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ground, PokemonType.Steel), Is.EqualTo(2.0f));
        }

        #endregion

        #region Ghost Type (Unique Properties)

        [Test]
        [Description("Ghost has 2 immunities")]
        public void Ghost_Immunities()
        {
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Normal, PokemonType.Ghost), Is.EqualTo(0.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fighting, PokemonType.Ghost), Is.EqualTo(0.0f));
        }

        [Test]
        [Description("Ghost resists Poison and Bug")]
        public void Ghost_Resistances()
        {
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Poison, PokemonType.Ghost), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Bug, PokemonType.Ghost), Is.EqualTo(0.5f));
        }

        [Test]
        [Description("Ghost is weak to Ghost and Dark")]
        public void Ghost_Weaknesses()
        {
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Ghost), Is.EqualTo(2.0f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Dark, PokemonType.Ghost), Is.EqualTo(2.0f));
        }

        #endregion

        #region Special Edge Cases

        [Test]
        [Description("Same type attacking same type")]
        public void SameType_Matchups()
        {
            // Most types resist themselves
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Fire), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Water, PokemonType.Water), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Electric, PokemonType.Electric), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Grass, PokemonType.Grass), Is.EqualTo(0.5f));
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Dragon, PokemonType.Dragon), Is.EqualTo(2.0f)); // Exception!
            Assert.That(TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Ghost), Is.EqualTo(2.0f)); // Exception!
        }

        [Test]
        [Description("Dragon is super effective against itself")]
        public void Dragon_Self_Super_Effective()
        {
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Dragon, PokemonType.Dragon);
            Assert.That(eff, Is.EqualTo(2.0f));
        }

        [Test]
        [Description("Normal type has no super effective matchups")]
        public void Normal_NoSuperEffective()
        {
            var superEffective = TypeEffectiveness.GetSuperEffectiveAgainst(PokemonType.Normal);
            Assert.That(superEffective.Count, Is.EqualTo(0));
        }

        [Test]
        [Description("Ice type - Great offense, poor defense")]
        public void Ice_OffenseVsDefense()
        {
            // Ice is super effective against 4 types (Dragon, Flying, Grass, Ground)
            var superEffective = TypeEffectiveness.GetSuperEffectiveAgainst(PokemonType.Ice);
            Assert.That(superEffective.Count, Is.EqualTo(4));
            
            // But only resists itself
            var resisted = TypeEffectiveness.GetResistedBy(PokemonType.Ice);
            Assert.That(resisted, Does.Contain(PokemonType.Ice));
        }

        [Test]
        [Description("Null secondary type is handled correctly")]
        public void NullSecondaryType_HandledCorrectly()
        {
            // Single type Pokemon (null secondary type)
            float withNull = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass, null);
            float withoutNull = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass);
            
            Assert.That(withNull, Is.EqualTo(withoutNull));
            Assert.That(withNull, Is.EqualTo(2.0f));
        }

        #endregion

        #region STAB Edge Cases

        [Test]
        [Description("STAB with dual type (both types match)")]
        public void STAB_DualType_BothMatch()
        {
            // If a Pokemon is Fire/Fighting and uses Fire Punch
            float stab1 = TypeEffectiveness.GetSTABMultiplier(PokemonType.Fire, PokemonType.Fire, PokemonType.Fighting);
            float stab2 = TypeEffectiveness.GetSTABMultiplier(PokemonType.Fighting, PokemonType.Fire, PokemonType.Fighting);
            
            Assert.That(stab1, Is.EqualTo(1.5f));
            Assert.That(stab2, Is.EqualTo(1.5f));
        }

        [Test]
        [Description("STAB is only 1.5x, never stacks")]
        public void STAB_NeverStacks()
        {
            // Even if move type matches both Pokemon types, it's still only 1.5x
            // (This would be a strange case, like a hypothetical Fire/Fire type)
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Fire, PokemonType.Fire, PokemonType.Fire);
            Assert.That(stab, Is.EqualTo(1.5f));
        }

        [Test]
        [Description("No STAB for coverage moves")]
        public void NoSTAB_CoverageMoves()
        {
            // Charizard (Fire/Flying) using Thunder Punch (Electric)
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Electric, PokemonType.Fire, PokemonType.Flying);
            Assert.That(stab, Is.EqualTo(1.0f));
        }

        #endregion

        #region Combined Damage Calculation Examples

        [Test]
        [Description("Charizard Fire Blast vs Ferrothorn: STAB + 4x = 6x base")]
        public void Combined_CharizardVsFerrothorn()
        {
            // Charizard (Fire/Flying) uses Fire Blast (Fire) vs Ferrothorn (Grass/Steel)
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Fire, PokemonType.Fire, PokemonType.Flying);
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Fire, PokemonType.Grass, PokemonType.Steel);
            float combined = stab * eff;
            
            Assert.That(stab, Is.EqualTo(1.5f));
            Assert.That(eff, Is.EqualTo(4.0f));
            Assert.That(combined, Is.EqualTo(6.0f));
        }

        [Test]
        [Description("Weavile Ice Punch vs Garchomp: STAB + 4x = 6x base")]
        public void Combined_WeavileVsGarchomp()
        {
            // Weavile (Dark/Ice) uses Ice Punch (Ice) vs Garchomp (Dragon/Ground)
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Ice, PokemonType.Dark, PokemonType.Ice);
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ice, PokemonType.Dragon, PokemonType.Ground);
            float combined = stab * eff;
            
            Assert.That(stab, Is.EqualTo(1.5f));
            Assert.That(eff, Is.EqualTo(4.0f));
            Assert.That(combined, Is.EqualTo(6.0f));
        }

        [Test]
        [Description("Gengar Shadow Ball vs Alakazam: STAB + 2x = 3x base")]
        public void Combined_GengarVsAlakazam()
        {
            // Gengar (Ghost/Poison) uses Shadow Ball (Ghost) vs Alakazam (Psychic)
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Ghost, PokemonType.Ghost, PokemonType.Poison);
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Psychic);
            float combined = stab * eff;
            
            Assert.That(stab, Is.EqualTo(1.5f));
            Assert.That(eff, Is.EqualTo(2.0f));
            Assert.That(combined, Is.EqualTo(3.0f));
        }

        [Test]
        [Description("No damage when immune, regardless of STAB")]
        public void Combined_ImmuneCancelsEverything()
        {
            // Gengar uses Shadow Ball vs Snorlax (Normal)
            float stab = TypeEffectiveness.GetSTABMultiplier(PokemonType.Ghost, PokemonType.Ghost, PokemonType.Poison);
            float eff = TypeEffectiveness.GetEffectiveness(PokemonType.Ghost, PokemonType.Normal);
            float combined = stab * eff;
            
            Assert.That(stab, Is.EqualTo(1.5f));
            Assert.That(eff, Is.EqualTo(0.0f));
            Assert.That(combined, Is.EqualTo(0.0f)); // 1.5 * 0 = 0
        }

        #endregion
    }
}

