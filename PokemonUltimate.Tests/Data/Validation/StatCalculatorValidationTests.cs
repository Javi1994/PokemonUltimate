using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Data.Validation
{
    /// <summary>
    /// Validation tests that verify our stat calculations match official Pokemon games.
    /// Formula: Gen3+ with IV=31, EV=252 for all stats (max competitive).
    /// 
    /// HP Formula: floor((2 * Base + IV + floor(EV/4)) * Level / 100) + Level + 10
    /// Stat Formula: floor((floor((2 * Base + IV + floor(EV/4)) * Level / 100) + 5) * NatureModifier)
    /// </summary>
    [TestFixture]
    public class StatCalculatorValidationTests
    {
        #region Pikachu (Base: 35/55/40/50/50/90)

        [Test]
        [Description("Pikachu Lv50, Hardy nature, 31 IVs, 252 EVs")]
        public void Pikachu_Level50_MaxIVsEVs_Hardy()
        {
            // Base stats: HP=35, Atk=55, Def=40, SpA=50, SpD=50, Spe=90
            // HP: floor((2*35 + 31 + 63) * 50 / 100) + 50 + 10 = 82 + 60 = 142
            // Atk: floor((2*55 + 31 + 63) * 50 / 100) + 5 = 102 + 5 = 107
            // Def: floor((2*40 + 31 + 63) * 50 / 100) + 5 = 87 + 5 = 92
            // SpA: floor((2*50 + 31 + 63) * 50 / 100) + 5 = 97 + 5 = 102
            // SpD: floor((2*50 + 31 + 63) * 50 / 100) + 5 = 97 + 5 = 102
            // Spe: floor((2*90 + 31 + 63) * 50 / 100) + 5 = 137 + 5 = 142
            int hp = StatCalculator.CalculateHP(35, 50);
            int atk = StatCalculator.CalculateStat(55, 50, Nature.Hardy, Stat.Attack);
            int def = StatCalculator.CalculateStat(40, 50, Nature.Hardy, Stat.Defense);
            int spa = StatCalculator.CalculateStat(50, 50, Nature.Hardy, Stat.SpAttack);
            int spd = StatCalculator.CalculateStat(50, 50, Nature.Hardy, Stat.SpDefense);
            int spe = StatCalculator.CalculateStat(90, 50, Nature.Hardy, Stat.Speed);

            Assert.That(hp, Is.EqualTo(142), "Pikachu HP");
            Assert.That(atk, Is.EqualTo(107), "Pikachu Attack");
            Assert.That(def, Is.EqualTo(92), "Pikachu Defense");
            Assert.That(spa, Is.EqualTo(102), "Pikachu SpAtk");
            Assert.That(spd, Is.EqualTo(102), "Pikachu SpDef");
            Assert.That(spe, Is.EqualTo(142), "Pikachu Speed");
        }

        [Test]
        [Description("Pikachu Lv50, Jolly nature (+Spe, -SpA)")]
        public void Pikachu_Level50_Jolly()
        {
            // Spe: 142 * 1.1 = 156.2 -> 156
            // SpA: 102 * 0.9 = 91.8 -> 91
            int spe = StatCalculator.CalculateStat(90, 50, Nature.Jolly, Stat.Speed);
            int spa = StatCalculator.CalculateStat(50, 50, Nature.Jolly, Stat.SpAttack);

            Assert.That(spe, Is.EqualTo(156), "Pikachu Speed (Jolly +10%)");
            Assert.That(spa, Is.EqualTo(91), "Pikachu SpAtk (Jolly -10%)");
        }

        [Test]
        [Description("Pikachu Lv100, Hardy nature")]
        public void Pikachu_Level100_Hardy()
        {
            // HP: floor((2*35 + 31 + 63) * 100 / 100) + 100 + 10 = 164 + 110 = 274
            // Atk: floor((2*55 + 31 + 63) * 100 / 100) + 5 = 204 + 5 = 209
            // Spe: floor((2*90 + 31 + 63) * 100 / 100) + 5 = 274 + 5 = 279
            int hp = StatCalculator.CalculateHP(35, 100);
            int atk = StatCalculator.CalculateStat(55, 100, Nature.Hardy, Stat.Attack);
            int spe = StatCalculator.CalculateStat(90, 100, Nature.Hardy, Stat.Speed);

            Assert.That(hp, Is.EqualTo(274), "Pikachu HP Lv100");
            Assert.That(atk, Is.EqualTo(209), "Pikachu Attack Lv100");
            Assert.That(spe, Is.EqualTo(279), "Pikachu Speed Lv100");
        }

        #endregion

        #region Charizard (Base: 78/84/78/109/85/100)

        [Test]
        [Description("Charizard Lv50, Timid nature (+Spe, -Atk)")]
        public void Charizard_Level50_Timid()
        {
            // Base: HP=78, Atk=84, Def=78, SpA=109, SpD=85, Spe=100
            // HP: floor(250 * 0.5) + 60 = 125 + 60 = 185
            // Atk: (131 + 5) * 0.9 = 136 * 0.9 = 122.4 -> 122
            // SpA: floor(312 * 0.5) + 5 = 156 + 5 = 161
            // Spe: (147 + 5) * 1.1 = 152 * 1.1 = 167.2 -> 167
            int hp = StatCalculator.CalculateHP(78, 50);
            int atk = StatCalculator.CalculateStat(84, 50, Nature.Timid, Stat.Attack);
            int spa = StatCalculator.CalculateStat(109, 50, Nature.Timid, Stat.SpAttack);
            int spe = StatCalculator.CalculateStat(100, 50, Nature.Timid, Stat.Speed);

            Assert.That(hp, Is.EqualTo(185), "Charizard HP");
            Assert.That(atk, Is.EqualTo(122), "Charizard Attack (Timid -10%)");
            Assert.That(spa, Is.EqualTo(161), "Charizard SpAtk (neutral)");
            Assert.That(spe, Is.EqualTo(167), "Charizard Speed (Timid +10%)");
        }

        [Test]
        [Description("Charizard Lv100, Modest nature (+SpA, -Atk)")]
        public void Charizard_Level100_Modest()
        {
            // HP: 250 + 110 = 360
            // SpA: (312 + 5) * 1.1 = 317 * 1.1 = 348.7 -> 348
            // Atk: (262 + 5) * 0.9 = 267 * 0.9 = 240.3 -> 240
            int hp = StatCalculator.CalculateHP(78, 100);
            int spa = StatCalculator.CalculateStat(109, 100, Nature.Modest, Stat.SpAttack);
            int atk = StatCalculator.CalculateStat(84, 100, Nature.Modest, Stat.Attack);

            Assert.That(hp, Is.EqualTo(360), "Charizard HP Lv100");
            Assert.That(spa, Is.EqualTo(348), "Charizard SpAtk Lv100 (Modest +10%)");
            Assert.That(atk, Is.EqualTo(240), "Charizard Attack Lv100 (Modest -10%)");
        }

        #endregion

        #region Blastoise (Base: 79/83/100/85/105/78)

        [Test]
        [Description("Blastoise Lv50, Bold nature (+Def, -Atk)")]
        public void Blastoise_Level50_Bold()
        {
            // Base: HP=79, Atk=83, Def=100, SpA=85, SpD=105, Spe=78
            // HP: floor(252 * 0.5) + 60 = 126 + 60 = 186
            // Def: (147 + 5) * 1.1 = 152 * 1.1 = 167.2 -> 167
            // Atk: (130 + 5) * 0.9 = 135 * 0.9 = 121.5 -> 121
            // SpD: floor(304 * 0.5) + 5 = 152 + 5 = 157
            int hp = StatCalculator.CalculateHP(79, 50);
            int def = StatCalculator.CalculateStat(100, 50, Nature.Bold, Stat.Defense);
            int atk = StatCalculator.CalculateStat(83, 50, Nature.Bold, Stat.Attack);
            int spd = StatCalculator.CalculateStat(105, 50, Nature.Bold, Stat.SpDefense);

            Assert.That(hp, Is.EqualTo(186), "Blastoise HP");
            Assert.That(def, Is.EqualTo(167), "Blastoise Defense (Bold +10%)");
            Assert.That(atk, Is.EqualTo(121), "Blastoise Attack (Bold -10%)");
            Assert.That(spd, Is.EqualTo(157), "Blastoise SpDef (neutral)");
        }

        #endregion

        #region Mewtwo (Base: 106/110/90/154/90/130) - Legendary Stats

        [Test]
        [Description("Mewtwo Lv50, Timid nature")]
        public void Mewtwo_Level50_Timid()
        {
            // Base: HP=106, Atk=110, Def=90, SpA=154, SpD=90, Spe=130
            // HP: floor(306 * 0.5) + 60 = 153 + 60 = 213
            // SpA: floor(402 * 0.5) + 5 = 201 + 5 = 206 (neutral)
            // Spe: (177 + 5) * 1.1 = 182 * 1.1 = 200.2 -> 200
            // Atk: (157 + 5) * 0.9 = 162 * 0.9 = 145.8 -> 145
            int hp = StatCalculator.CalculateHP(106, 50);
            int spa = StatCalculator.CalculateStat(154, 50, Nature.Timid, Stat.SpAttack);
            int spe = StatCalculator.CalculateStat(130, 50, Nature.Timid, Stat.Speed);
            int atk = StatCalculator.CalculateStat(110, 50, Nature.Timid, Stat.Attack);

            Assert.That(hp, Is.EqualTo(213), "Mewtwo HP");
            Assert.That(spa, Is.EqualTo(206), "Mewtwo SpAtk (neutral)");
            Assert.That(spe, Is.EqualTo(200), "Mewtwo Speed (Timid +10%)");
            Assert.That(atk, Is.EqualTo(145), "Mewtwo Attack (Timid -10%)");
        }

        [Test]
        [Description("Mewtwo Lv100, Modest nature - Max SpA Pokemon")]
        public void Mewtwo_Level100_Modest()
        {
            // SpA: (402 + 5) * 1.1 = 407 * 1.1 = 447.7 -> 447
            int spa = StatCalculator.CalculateStat(154, 100, Nature.Modest, Stat.SpAttack);

            // One of the highest SpAtk in the game
            Assert.That(spa, Is.EqualTo(447), "Mewtwo SpAtk Lv100 (Modest +10%)");
        }

        #endregion

        #region Snorlax (Base: 160/110/65/65/110/30) - Tank Stats

        [Test]
        [Description("Snorlax Lv50, Careful nature (+SpD, -SpA)")]
        public void Snorlax_Level50_Careful()
        {
            // Base: HP=160, Atk=110, Def=65, SpA=65, SpD=110, Spe=30
            // HP: floor(414 * 0.5) + 60 = 207 + 60 = 267
            // Atk: floor(314 * 0.5) + 5 = 157 + 5 = 162 (neutral)
            // SpD: 162 * 1.1 = 178.2 -> 178 (Careful +10%)
            // Spe: floor(154 * 0.5) + 5 = 77 + 5 = 82 (neutral)
            int hp = StatCalculator.CalculateHP(160, 50);
            int atk = StatCalculator.CalculateStat(110, 50, Nature.Careful, Stat.Attack);
            int spd = StatCalculator.CalculateStat(110, 50, Nature.Careful, Stat.SpDefense);
            int spe = StatCalculator.CalculateStat(30, 50, Nature.Careful, Stat.Speed);

            Assert.That(hp, Is.EqualTo(267), "Snorlax HP (massive)");
            Assert.That(atk, Is.EqualTo(162), "Snorlax Attack (neutral)");
            Assert.That(spd, Is.EqualTo(178), "Snorlax SpDef (Careful +10%)");
            Assert.That(spe, Is.EqualTo(82), "Snorlax Speed (slow)");
        }

        [Test]
        [Description("Snorlax Lv100 - Highest HP Pokemon in our catalog")]
        public void Snorlax_Level100_HP()
        {
            // HP: 414 + 110 = 524
            int hp = StatCalculator.CalculateHP(160, 100);

            Assert.That(hp, Is.EqualTo(524), "Snorlax HP Lv100 (one of highest in game)");
        }

        #endregion

        #region Alakazam (Base: 55/50/45/135/95/120) - Glass Cannon

        [Test]
        [Description("Alakazam Lv50, Timid nature")]
        public void Alakazam_Level50_Timid()
        {
            // Base: HP=55, Atk=50, Def=45, SpA=135, SpD=95, Spe=120
            // HP: floor(204 * 0.5) + 60 = 102 + 60 = 162
            // Def: floor(184 * 0.5) + 5 = 92 + 5 = 97 (neutral)
            // SpA: floor(364 * 0.5) + 5 = 182 + 5 = 187 (neutral)
            // Spe: (167 + 5) * 1.1 = 172 * 1.1 = 189.2 -> 189
            int hp = StatCalculator.CalculateHP(55, 50);
            int def = StatCalculator.CalculateStat(45, 50, Nature.Timid, Stat.Defense);
            int spa = StatCalculator.CalculateStat(135, 50, Nature.Timid, Stat.SpAttack);
            int spe = StatCalculator.CalculateStat(120, 50, Nature.Timid, Stat.Speed);

            Assert.That(hp, Is.EqualTo(162), "Alakazam HP (frail)");
            Assert.That(def, Is.EqualTo(97), "Alakazam Defense (very frail)");
            Assert.That(spa, Is.EqualTo(187), "Alakazam SpAtk (massive)");
            Assert.That(spe, Is.EqualTo(189), "Alakazam Speed (Timid +10%)");
        }

        #endregion

        #region Edge Cases - Extreme Base Stats

        [Test]
        [Description("Pokemon with 1 base stat (theoretical minimum)")]
        public void MinBaseStat_Level50()
        {
            // Shedinja has 1 HP (but uses different mechanics)
            // HP: floor((2*1 + 31 + 63) * 0.5) + 60 = floor(96 * 0.5) + 60 = 48 + 60 = 108
            // Stat: floor((2*1 + 31 + 63) * 0.5) + 5 = 48 + 5 = 53
            int hp = StatCalculator.CalculateHP(1, 50, 31, 252);
            int stat = StatCalculator.CalculateStat(1, 50, Nature.Hardy, Stat.Attack);

            Assert.That(hp, Is.EqualTo(108), "Even 1 base HP gives decent HP with max IVs/EVs");
            Assert.That(stat, Is.EqualTo(53), "Even 1 base stat gives decent stat with max IVs/EVs");
        }

        [Test]
        [Description("Pokemon with 255 base stat (theoretical maximum)")]
        public void MaxBaseStat_Level100()
        {
            // No Pokemon has 255 base stat, but testing the formula
            // HP: (2*255 + 31 + 63) + 110 = 604 + 110 = 714
            // Stat: 604 + 5 = 609
            int hp = StatCalculator.CalculateHP(255, 100);
            int stat = StatCalculator.CalculateStat(255, 100, Nature.Hardy, Stat.Attack);

            Assert.That(hp, Is.EqualTo(714), "Max possible HP");
            Assert.That(stat, Is.EqualTo(609), "Max possible stat (neutral)");
        }

        [Test]
        [Description("Shuckle - Highest Def/SpD (230), Lowest everything else")]
        public void Shuckle_Level50_Impish()
        {
            // Base: HP=20, Atk=10, Def=230, SpA=10, SpD=230, Spe=5
            // HP: floor(134 * 0.5) + 60 = 67 + 60 = 127
            // Def: (277 + 5) * 1.1 = 282 * 1.1 = 310.2 -> 310
            // Spe: floor(104 * 0.5) + 5 = 52 + 5 = 57 (neutral, Impish doesn't affect Speed)
            int hp = StatCalculator.CalculateHP(20, 50);
            int def = StatCalculator.CalculateStat(230, 50, Nature.Impish, Stat.Defense);
            int spe = StatCalculator.CalculateStat(5, 50, Nature.Impish, Stat.Speed);

            Assert.That(hp, Is.EqualTo(127), "Shuckle HP (low)");
            Assert.That(def, Is.EqualTo(310), "Shuckle Defense (Impish +10%, highest possible)");
            Assert.That(spe, Is.EqualTo(57), "Shuckle Speed (one of lowest)");
        }

        #endregion

        #region Level 1 and Level 5 (Early Game)

        [Test]
        [Description("Pikachu Lv5 - Early game stats")]
        public void Pikachu_Level5_Hardy()
        {
            // HP: floor(164 * 0.05) + 5 + 10 = 8 + 15 = 23
            // Atk: floor(204 * 0.05) + 5 = 10 + 5 = 15
            // Spe: floor(274 * 0.05) + 5 = 13 + 5 = 18
            int hp = StatCalculator.CalculateHP(35, 5);
            int atk = StatCalculator.CalculateStat(55, 5, Nature.Hardy, Stat.Attack);
            int spe = StatCalculator.CalculateStat(90, 5, Nature.Hardy, Stat.Speed);

            Assert.That(hp, Is.EqualTo(23), "Pikachu HP Lv5");
            Assert.That(atk, Is.EqualTo(15), "Pikachu Attack Lv5");
            Assert.That(spe, Is.EqualTo(18), "Pikachu Speed Lv5");
        }

        [Test]
        [Description("Pikachu Lv1 - Minimum level")]
        public void Pikachu_Level1_Hardy()
        {
            // HP: floor(164 * 0.01) + 1 + 10 = 1 + 11 = 12
            // Atk: floor(204 * 0.01) + 5 = 2 + 5 = 7
            int hp = StatCalculator.CalculateHP(35, 1);
            int atk = StatCalculator.CalculateStat(55, 1, Nature.Hardy, Stat.Attack);

            Assert.That(hp, Is.EqualTo(12), "Pikachu HP Lv1");
            Assert.That(atk, Is.EqualTo(7), "Pikachu Attack Lv1");
        }

        #endregion

        #region Zero IVs/EVs Comparison (for reference)

        [Test]
        [Description("Comparison: Max vs Zero IVs/EVs shows significant difference")]
        public void IVsEVs_Make_Significant_Difference()
        {
            // Charizard SpAtk at Lv50
            // withMax: floor((2*109 + 31 + 63) * 0.5) + 5 = 156 + 5 = 161
            // withZero: floor((2*109 + 0 + 0) * 0.5) + 5 = 109 + 5 = 114
            int withMax = StatCalculator.CalculateStat(109, 50, Nature.Hardy, Stat.SpAttack, 31, 252);
            int withZero = StatCalculator.CalculateStat(109, 50, Nature.Hardy, Stat.SpAttack, 0, 0);

            // The difference is substantial (about 40% more with max IVs/EVs)
            Assert.That(withMax, Is.EqualTo(161), "Charizard SpAtk with max IVs/EVs");
            Assert.That(withZero, Is.EqualTo(114), "Charizard SpAtk with zero IVs/EVs");
            Assert.That(withMax, Is.GreaterThan(withZero * 1.4f), "Max should be 40%+ higher");
        }

        #endregion

        #region Nature Effects Verification

        [Test]
        [Description("All 5 neutral natures give same result")]
        public void AllNeutralNatures_GiveSameResult()
        {
            var neutralNatures = new[] { Nature.Hardy, Nature.Docile, Nature.Serious, Nature.Bashful, Nature.Quirky };
            int expected = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack);

            foreach (var nature in neutralNatures)
            {
                int result = StatCalculator.CalculateStat(100, 50, nature, Stat.Attack);
                Assert.That(result, Is.EqualTo(expected), $"{nature} should be neutral");
            }
        }

        [Test]
        [Description("Adamant vs Modest - Attack-focused vs SpAtk-focused")]
        public void Adamant_vs_Modest_Comparison()
        {
            // Base 100 for both Attack and SpAtk
            // Neutral stat: floor(294 * 0.5) + 5 = 147 + 5 = 152
            // +10%: 152 * 1.1 = 167.2 -> 167
            // -10%: 152 * 0.9 = 136.8 -> 136
            int adamantAtk = StatCalculator.CalculateStat(100, 50, Nature.Adamant, Stat.Attack);
            int adamantSpA = StatCalculator.CalculateStat(100, 50, Nature.Adamant, Stat.SpAttack);
            int modestAtk = StatCalculator.CalculateStat(100, 50, Nature.Modest, Stat.Attack);
            int modestSpA = StatCalculator.CalculateStat(100, 50, Nature.Modest, Stat.SpAttack);

            // Adamant: +Atk, -SpA
            Assert.That(adamantAtk, Is.EqualTo(167), "Adamant boosts Attack");
            Assert.That(adamantSpA, Is.EqualTo(136), "Adamant reduces SpAtk");
            
            // Modest: +SpA, -Atk
            Assert.That(modestAtk, Is.EqualTo(136), "Modest reduces Attack");
            Assert.That(modestSpA, Is.EqualTo(167), "Modest boosts SpAtk");
        }

        #endregion
    }
}
