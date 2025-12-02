using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    [TestFixture]
    public class StatCalculatorTests
    {
        #region Constants Tests

        [Test]
        public void MaxIV_Should_Be_31()
        {
            Assert.That(StatCalculator.MaxIV, Is.EqualTo(31));
        }

        [Test]
        public void MaxEV_Should_Be_252()
        {
            Assert.That(StatCalculator.MaxEV, Is.EqualTo(252));
        }

        [Test]
        public void DefaultIV_Should_Be_MaxIV()
        {
            Assert.That(StatCalculator.DefaultIV, Is.EqualTo(StatCalculator.MaxIV));
        }

        [Test]
        public void DefaultEV_Should_Be_MaxEV()
        {
            Assert.That(StatCalculator.DefaultEV, Is.EqualTo(StatCalculator.MaxEV));
        }

        #endregion

        #region CalculateHP Tests (With Max IVs/EVs)

        [Test]
        public void CalculateHP_With_Max_IVs_EVs_Level50()
        {
            // Base 100, Level 50, IV=31, EV=252
            // ((2*100 + 31 + 63) * 50 / 100) + 50 + 10 = (294 * 50 / 100) + 60 = 147 + 60 = 207
            int result = StatCalculator.CalculateHP(100, 50);

            Assert.That(result, Is.EqualTo(207));
        }

        [Test]
        public void CalculateHP_With_Max_IVs_EVs_Level100()
        {
            // Base 100, Level 100, IV=31, EV=252
            // ((2*100 + 31 + 63) * 100 / 100) + 100 + 10 = 294 + 110 = 404
            int result = StatCalculator.CalculateHP(100, 100);

            Assert.That(result, Is.EqualTo(404));
        }

        [Test]
        public void CalculateHP_Pikachu_Level50_Max()
        {
            // Pikachu base HP = 35, Level 50, Max IVs/EVs
            // ((2*35 + 31 + 63) * 50 / 100) + 50 + 10 = (164 * 50 / 100) + 60 = 82 + 60 = 142
            int result = StatCalculator.CalculateHP(35, 50);

            Assert.That(result, Is.EqualTo(142));
        }

        [Test]
        public void CalculateHP_With_Zero_IVs_EVs()
        {
            // Base 100, Level 50, IV=0, EV=0
            // ((2*100 + 0 + 0) * 50 / 100) + 50 + 10 = 100 + 60 = 160
            int result = StatCalculator.CalculateHP(100, 50, 0, 0);

            Assert.That(result, Is.EqualTo(160));
        }

        [Test]
        public void CalculateHP_Should_Throw_For_Negative_BaseHP()
        {
            Assert.That(() => StatCalculator.CalculateHP(-1, 50), Throws.ArgumentException);
        }

        [Test]
        public void CalculateHP_Should_Throw_For_Invalid_Level()
        {
            Assert.That(() => StatCalculator.CalculateHP(100, 0), Throws.ArgumentException);
            Assert.That(() => StatCalculator.CalculateHP(100, 101), Throws.ArgumentException);
        }

        [Test]
        public void CalculateHP_Should_Throw_For_Invalid_IV()
        {
            Assert.That(() => StatCalculator.CalculateHP(100, 50, -1, 0), Throws.ArgumentException);
            Assert.That(() => StatCalculator.CalculateHP(100, 50, 32, 0), Throws.ArgumentException);
        }

        [Test]
        public void CalculateHP_Should_Throw_For_Invalid_EV()
        {
            Assert.That(() => StatCalculator.CalculateHP(100, 50, 0, -1), Throws.ArgumentException);
            Assert.That(() => StatCalculator.CalculateHP(100, 50, 0, 253), Throws.ArgumentException);
        }

        #endregion

        #region CalculateStat Tests (With Max IVs/EVs)

        [Test]
        public void CalculateStat_Neutral_Nature_Level50()
        {
            // Base 100, Level 50, Hardy, IV=31, EV=252
            // ((2*100 + 31 + 63) * 50 / 100) + 5 = 147 + 5 = 152
            int result = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack);

            Assert.That(result, Is.EqualTo(152));
        }

        [Test]
        public void CalculateStat_Boosting_Nature_Adds_10_Percent()
        {
            // Base 100, Level 50, Adamant (+Atk), IV=31, EV=252
            // ((2*100 + 31 + 63) * 50 / 100) + 5 = 152, * 1.1 = 167
            int result = StatCalculator.CalculateStat(100, 50, Nature.Adamant, Stat.Attack);

            Assert.That(result, Is.EqualTo(167));
        }

        [Test]
        public void CalculateStat_Reducing_Nature_Subtracts_10_Percent()
        {
            // Base 100, Level 50, Adamant (-SpAtk), IV=31, EV=252
            // 152 * 0.9 = 136
            int result = StatCalculator.CalculateStat(100, 50, Nature.Adamant, Stat.SpAttack);

            Assert.That(result, Is.EqualTo(136));
        }

        [Test]
        public void CalculateStat_Level100_Max_IVs_EVs()
        {
            // Base 100, Level 100, Hardy, IV=31, EV=252
            // ((2*100 + 31 + 63) * 100 / 100) + 5 = 294 + 5 = 299
            int result = StatCalculator.CalculateStat(100, 100, Nature.Hardy, Stat.Attack);

            Assert.That(result, Is.EqualTo(299));
        }

        [Test]
        public void CalculateStat_With_Zero_IVs_EVs()
        {
            // Base 100, Level 50, Hardy, IV=0, EV=0
            // ((2*100 + 0 + 0) * 50 / 100) + 5 = 100 + 5 = 105
            int result = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack, 0, 0);

            Assert.That(result, Is.EqualTo(105));
        }

        [Test]
        public void CalculateStat_Timid_Boosts_Speed_Reduces_Attack()
        {
            int speed = StatCalculator.CalculateStat(100, 50, Nature.Timid, Stat.Speed);
            int attack = StatCalculator.CalculateStat(100, 50, Nature.Timid, Stat.Attack);
            int defense = StatCalculator.CalculateStat(100, 50, Nature.Timid, Stat.Defense);

            Assert.That(speed, Is.EqualTo(167));   // Boosted
            Assert.That(attack, Is.EqualTo(136));  // Reduced
            Assert.That(defense, Is.EqualTo(152)); // Neutral
        }

        [Test]
        public void CalculateStat_Should_Throw_For_Negative_BaseStat()
        {
            Assert.That(() => StatCalculator.CalculateStat(-1, 50, Nature.Hardy, Stat.Attack),
                Throws.ArgumentException);
        }

        #endregion

        #region GetStageMultiplier Tests

        [Test]
        public void GetStageMultiplier_Stage0_Returns_1()
        {
            float result = StatCalculator.GetStageMultiplier(0);
            Assert.That(result, Is.EqualTo(1f));
        }

        [Test]
        public void GetStageMultiplier_Stage1_Returns_1_5()
        {
            float result = StatCalculator.GetStageMultiplier(1);
            Assert.That(result, Is.EqualTo(1.5f));
        }

        [Test]
        public void GetStageMultiplier_Stage2_Returns_2()
        {
            float result = StatCalculator.GetStageMultiplier(2);
            Assert.That(result, Is.EqualTo(2f));
        }

        [Test]
        public void GetStageMultiplier_Stage6_Returns_4()
        {
            float result = StatCalculator.GetStageMultiplier(6);
            Assert.That(result, Is.EqualTo(4f));
        }

        [Test]
        public void GetStageMultiplier_StageMinus1_Returns_0_67()
        {
            float result = StatCalculator.GetStageMultiplier(-1);
            Assert.That(result, Is.EqualTo(2f / 3f).Within(0.01f));
        }

        [Test]
        public void GetStageMultiplier_StageMinus6_Returns_0_25()
        {
            float result = StatCalculator.GetStageMultiplier(-6);
            Assert.That(result, Is.EqualTo(0.25f));
        }

        [Test]
        public void GetStageMultiplier_Clamps_Above_6()
        {
            float result = StatCalculator.GetStageMultiplier(10);
            Assert.That(result, Is.EqualTo(4f));
        }

        [Test]
        public void GetStageMultiplier_Clamps_Below_Minus6()
        {
            float result = StatCalculator.GetStageMultiplier(-10);
            Assert.That(result, Is.EqualTo(0.25f));
        }

        #endregion

        #region GetEffectiveStat Tests

        [Test]
        public void GetEffectiveStat_Stage0_Returns_Same()
        {
            int result = StatCalculator.GetEffectiveStat(100, 0);
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void GetEffectiveStat_Stage2_Doubles()
        {
            int result = StatCalculator.GetEffectiveStat(100, 2);
            Assert.That(result, Is.EqualTo(200));
        }

        [Test]
        public void GetEffectiveStat_StageMinus2_Halves()
        {
            int result = StatCalculator.GetEffectiveStat(100, -2);
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public void GetEffectiveStat_Throws_For_Negative_Stat()
        {
            Assert.That(() => StatCalculator.GetEffectiveStat(-1, 0), Throws.ArgumentException);
        }

        #endregion

        #region GetAccuracyStageMultiplier Tests

        [Test]
        public void GetAccuracyStageMultiplier_Stage0_Returns_1()
        {
            float result = StatCalculator.GetAccuracyStageMultiplier(0);
            Assert.That(result, Is.EqualTo(1f));
        }

        [Test]
        public void GetAccuracyStageMultiplier_Stage1_Returns_1_33()
        {
            float result = StatCalculator.GetAccuracyStageMultiplier(1);
            Assert.That(result, Is.EqualTo(4f / 3f).Within(0.01f));
        }

        [Test]
        public void GetAccuracyStageMultiplier_Stage6_Returns_3()
        {
            float result = StatCalculator.GetAccuracyStageMultiplier(6);
            Assert.That(result, Is.EqualTo(3f));
        }

        [Test]
        public void GetAccuracyStageMultiplier_StageMinus1_Returns_0_75()
        {
            float result = StatCalculator.GetAccuracyStageMultiplier(-1);
            Assert.That(result, Is.EqualTo(0.75f));
        }

        #endregion

        #region Experience Tests

        [Test]
        public void GetExpForLevel_Level1_Returns_1()
        {
            int exp = StatCalculator.GetExpForLevel(1);
            Assert.That(exp, Is.EqualTo(1));
        }

        [Test]
        public void GetExpForLevel_Level50_Returns_125000()
        {
            // 50^3 = 125,000
            int exp = StatCalculator.GetExpForLevel(50);
            Assert.That(exp, Is.EqualTo(125000));
        }

        [Test]
        public void GetExpForLevel_Level100_Returns_1000000()
        {
            // 100^3 = 1,000,000
            int exp = StatCalculator.GetExpForLevel(100);
            Assert.That(exp, Is.EqualTo(1000000));
        }

        [Test]
        public void GetExpForLevel_Throws_For_Invalid_Level()
        {
            Assert.That(() => StatCalculator.GetExpForLevel(0), Throws.ArgumentException);
            Assert.That(() => StatCalculator.GetExpForLevel(101), Throws.ArgumentException);
        }

        [Test]
        public void GetExpToNextLevel_Level1_Returns_Difference()
        {
            // Level 2 exp - Level 1 exp = 8 - 1 = 7
            int exp = StatCalculator.GetExpToNextLevel(1);
            Assert.That(exp, Is.EqualTo(7));
        }

        [Test]
        public void GetExpToNextLevel_Level99_Returns_Difference()
        {
            // Level 100 - Level 99 = 1,000,000 - 970,299 = 29,701
            int exp = StatCalculator.GetExpToNextLevel(99);
            Assert.That(exp, Is.EqualTo(29701));
        }

        [Test]
        public void GetExpToNextLevel_Level100_Returns_0()
        {
            int exp = StatCalculator.GetExpToNextLevel(100);
            Assert.That(exp, Is.EqualTo(0));
        }

        [Test]
        public void GetLevelForExp_Returns_Correct_Level()
        {
            Assert.That(StatCalculator.GetLevelForExp(0), Is.EqualTo(1));
            Assert.That(StatCalculator.GetLevelForExp(1), Is.EqualTo(1));
            Assert.That(StatCalculator.GetLevelForExp(8), Is.EqualTo(2));
            Assert.That(StatCalculator.GetLevelForExp(125000), Is.EqualTo(50));
            Assert.That(StatCalculator.GetLevelForExp(1000000), Is.EqualTo(100));
            Assert.That(StatCalculator.GetLevelForExp(999999999), Is.EqualTo(100));
        }

        [Test]
        public void GetLevelForExp_Throws_For_Negative()
        {
            Assert.That(() => StatCalculator.GetLevelForExp(-1), Throws.ArgumentException);
        }

        #endregion

        #region Comparison Tests (With vs Without IVs/EVs)

        [Test]
        public void Stats_With_Max_IVs_EVs_Are_Higher_Than_Without()
        {
            int hpWithout = StatCalculator.CalculateHP(100, 50, 0, 0);
            int hpWith = StatCalculator.CalculateHP(100, 50);

            int atkWithout = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack, 0, 0);
            int atkWith = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack);

            Assert.That(hpWith, Is.GreaterThan(hpWithout));
            Assert.That(atkWith, Is.GreaterThan(atkWithout));

            // Specific values
            Assert.That(hpWithout, Is.EqualTo(160));
            Assert.That(hpWith, Is.EqualTo(207));
            Assert.That(atkWithout, Is.EqualTo(105));
            Assert.That(atkWith, Is.EqualTo(152));
        }

        #endregion
    }
}

