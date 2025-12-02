using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for NatureData static class: stat modifiers for each nature.
    /// </summary>
    [TestFixture]
    public class NatureDataTests
    {
        #region Neutral Natures

        [Test]
        [TestCase(Nature.Hardy)]
        [TestCase(Nature.Docile)]
        [TestCase(Nature.Serious)]
        [TestCase(Nature.Bashful)]
        [TestCase(Nature.Quirky)]
        public void Test_Neutral_Natures_Have_No_Stat_Changes(Nature nature)
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.IsNeutral(nature), Is.True);
                Assert.That(NatureData.GetIncreasedStat(nature), Is.Null);
                Assert.That(NatureData.GetDecreasedStat(nature), Is.Null);
            });
        }

        [Test]
        [TestCase(Nature.Hardy)]
        public void Test_Neutral_Nature_Returns_1_For_All_Stats(Nature nature)
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetStatMultiplier(nature, Stat.Attack), Is.EqualTo(1.0f));
                Assert.That(NatureData.GetStatMultiplier(nature, Stat.Defense), Is.EqualTo(1.0f));
                Assert.That(NatureData.GetStatMultiplier(nature, Stat.Speed), Is.EqualTo(1.0f));
                Assert.That(NatureData.GetStatMultiplier(nature, Stat.SpAttack), Is.EqualTo(1.0f));
                Assert.That(NatureData.GetStatMultiplier(nature, Stat.SpDefense), Is.EqualTo(1.0f));
            });
        }

        #endregion

        #region Attack Boosting Natures

        [Test]
        public void Test_Adamant_Boosts_Attack_Reduces_SpAttack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.IsNeutral(Nature.Adamant), Is.False);
                Assert.That(NatureData.GetIncreasedStat(Nature.Adamant), Is.EqualTo(Stat.Attack));
                Assert.That(NatureData.GetDecreasedStat(Nature.Adamant), Is.EqualTo(Stat.SpAttack));
                Assert.That(NatureData.GetStatMultiplier(Nature.Adamant, Stat.Attack), Is.EqualTo(1.1f));
                Assert.That(NatureData.GetStatMultiplier(Nature.Adamant, Stat.SpAttack), Is.EqualTo(0.9f));
                Assert.That(NatureData.GetStatMultiplier(Nature.Adamant, Stat.Defense), Is.EqualTo(1.0f));
            });
        }

        [Test]
        public void Test_Brave_Boosts_Attack_Reduces_Speed()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Brave), Is.EqualTo(Stat.Attack));
                Assert.That(NatureData.GetDecreasedStat(Nature.Brave), Is.EqualTo(Stat.Speed));
            });
        }

        [Test]
        public void Test_Lonely_Boosts_Attack_Reduces_Defense()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Lonely), Is.EqualTo(Stat.Attack));
                Assert.That(NatureData.GetDecreasedStat(Nature.Lonely), Is.EqualTo(Stat.Defense));
            });
        }

        [Test]
        public void Test_Naughty_Boosts_Attack_Reduces_SpDefense()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Naughty), Is.EqualTo(Stat.Attack));
                Assert.That(NatureData.GetDecreasedStat(Nature.Naughty), Is.EqualTo(Stat.SpDefense));
            });
        }

        #endregion

        #region Speed Boosting Natures

        [Test]
        public void Test_Jolly_Boosts_Speed_Reduces_SpAttack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Jolly), Is.EqualTo(Stat.Speed));
                Assert.That(NatureData.GetDecreasedStat(Nature.Jolly), Is.EqualTo(Stat.SpAttack));
                Assert.That(NatureData.GetStatMultiplier(Nature.Jolly, Stat.Speed), Is.EqualTo(1.1f));
                Assert.That(NatureData.GetStatMultiplier(Nature.Jolly, Stat.SpAttack), Is.EqualTo(0.9f));
            });
        }

        [Test]
        public void Test_Timid_Boosts_Speed_Reduces_Attack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Timid), Is.EqualTo(Stat.Speed));
                Assert.That(NatureData.GetDecreasedStat(Nature.Timid), Is.EqualTo(Stat.Attack));
            });
        }

        #endregion

        #region Special Attack Boosting Natures

        [Test]
        public void Test_Modest_Boosts_SpAttack_Reduces_Attack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Modest), Is.EqualTo(Stat.SpAttack));
                Assert.That(NatureData.GetDecreasedStat(Nature.Modest), Is.EqualTo(Stat.Attack));
                Assert.That(NatureData.GetStatMultiplier(Nature.Modest, Stat.SpAttack), Is.EqualTo(1.1f));
                Assert.That(NatureData.GetStatMultiplier(Nature.Modest, Stat.Attack), Is.EqualTo(0.9f));
            });
        }

        #endregion

        #region Special Defense Boosting Natures

        [Test]
        public void Test_Calm_Boosts_SpDefense_Reduces_Attack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Calm), Is.EqualTo(Stat.SpDefense));
                Assert.That(NatureData.GetDecreasedStat(Nature.Calm), Is.EqualTo(Stat.Attack));
            });
        }

        [Test]
        public void Test_Careful_Boosts_SpDefense_Reduces_SpAttack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Careful), Is.EqualTo(Stat.SpDefense));
                Assert.That(NatureData.GetDecreasedStat(Nature.Careful), Is.EqualTo(Stat.SpAttack));
            });
        }

        #endregion

        #region Defense Boosting Natures

        [Test]
        public void Test_Bold_Boosts_Defense_Reduces_Attack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Bold), Is.EqualTo(Stat.Defense));
                Assert.That(NatureData.GetDecreasedStat(Nature.Bold), Is.EqualTo(Stat.Attack));
            });
        }

        [Test]
        public void Test_Impish_Boosts_Defense_Reduces_SpAttack()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.GetIncreasedStat(Nature.Impish), Is.EqualTo(Stat.Defense));
                Assert.That(NatureData.GetDecreasedStat(Nature.Impish), Is.EqualTo(Stat.SpAttack));
            });
        }

        #endregion

        #region All 25 Natures Count

        [Test]
        public void Test_There_Are_25_Natures()
        {
            var natures = System.Enum.GetValues(typeof(Nature));
            Assert.That(natures.Length, Is.EqualTo(25));
        }

        [Test]
        public void Test_5_Neutral_20_Non_Neutral()
        {
            int neutralCount = 0;
            int nonNeutralCount = 0;

            foreach (Nature nature in System.Enum.GetValues(typeof(Nature)))
            {
                if (NatureData.IsNeutral(nature))
                    neutralCount++;
                else
                    nonNeutralCount++;
            }

            Assert.Multiple(() =>
            {
                Assert.That(neutralCount, Is.EqualTo(5));
                Assert.That(nonNeutralCount, Is.EqualTo(20));
            });
        }

        #endregion

        #region Multiplier Constants

        [Test]
        public void Test_Multiplier_Constants()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NatureData.BoostMultiplier, Is.EqualTo(1.1f));
                Assert.That(NatureData.ReduceMultiplier, Is.EqualTo(0.9f));
                Assert.That(NatureData.NeutralMultiplier, Is.EqualTo(1.0f));
            });
        }

        #endregion
    }
}

