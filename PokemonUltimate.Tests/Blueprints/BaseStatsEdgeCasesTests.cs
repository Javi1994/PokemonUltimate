using System;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Edge case tests for BaseStats.
    /// Tests boundary values, comparisons, and stat queries.
    /// </summary>
    [TestFixture]
    public class BaseStatsEdgeCasesTests
    {
        #region Boundary Values

        [Test]
        public void BaseStats_AllZero_TotalIsZero()
        {
            var stats = new BaseStats(0, 0, 0, 0, 0, 0);
            Assert.That(stats.Total, Is.EqualTo(0));
        }

        [Test]
        public void BaseStats_AllMax255_TotalIs1530()
        {
            var stats = new BaseStats(255, 255, 255, 255, 255, 255);
            Assert.That(stats.Total, Is.EqualTo(1530));
        }

        [Test]
        public void BaseStats_NegativeValues_Allowed()
        {
            // Negative values are technically allowed (no validation)
            var stats = new BaseStats(-10, -20, -30, -40, -50, -60);
            Assert.That(stats.Total, Is.EqualTo(-210));
        }

        [Test]
        public void BaseStats_VeryHighValues_Allowed()
        {
            // Values above 255 are allowed (no validation in base class)
            var stats = new BaseStats(999, 999, 999, 999, 999, 999);
            Assert.That(stats.Total, Is.EqualTo(5994));
        }

        [Test]
        public void BaseStats_SingleStatHigh_OthersZero()
        {
            var stats = new BaseStats(255, 0, 0, 0, 0, 0);
            Assert.That(stats.Total, Is.EqualTo(255));
            Assert.That(stats.HP, Is.EqualTo(255));
        }

        #endregion

        #region Real Pokemon BST Ranges

        [Test]
        [Description("Sunkern has the lowest BST at 180")]
        public void BaseStats_LowestBST_Sunkern180()
        {
            var sunkern = new BaseStats(30, 30, 30, 30, 30, 30);
            Assert.That(sunkern.Total, Is.EqualTo(180));
        }

        [Test]
        [Description("Eternatus Eternamax has the highest BST at 1125")]
        public void BaseStats_HighestBST_Eternamax1125()
        {
            var eternamax = new BaseStats(255, 115, 250, 125, 250, 130);
            Assert.That(eternamax.Total, Is.EqualTo(1125));
        }

        [Test]
        [Description("Standard legendary BST is 680")]
        public void BaseStats_LegendaryBST_680()
        {
            // Mewtwo: 106+110+90+154+90+130 = 680
            var mewtwo = new BaseStats(106, 110, 90, 154, 90, 130);
            Assert.That(mewtwo.Total, Is.EqualTo(680));
        }

        [Test]
        [Description("Mythical 100-all BST is 600")]
        public void BaseStats_MythicalBST_600()
        {
            var mew = new BaseStats(100, 100, 100, 100, 100, 100);
            Assert.That(mew.Total, Is.EqualTo(600));
        }

        #endregion

        #region Stat Queries by Enum

        [Test]
        public void GetStat_HP_ReturnsCorrectValue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.HP), Is.EqualTo(100));
        }

        [Test]
        public void GetStat_Attack_ReturnsCorrectValue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.Attack), Is.EqualTo(80));
        }

        [Test]
        public void GetStat_Defense_ReturnsCorrectValue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.Defense), Is.EqualTo(70));
        }

        [Test]
        public void GetStat_SpAttack_ReturnsCorrectValue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.SpAttack), Is.EqualTo(90));
        }

        [Test]
        public void GetStat_SpDefense_ReturnsCorrectValue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.SpDefense), Is.EqualTo(85));
        }

        [Test]
        public void GetStat_Speed_ReturnsCorrectValue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.Speed), Is.EqualTo(95));
        }

        [Test]
        public void GetStat_Accuracy_ReturnsZero()
        {
            // Accuracy is not a base stat
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.Accuracy), Is.EqualTo(0));
        }

        [Test]
        public void GetStat_Evasion_ReturnsZero()
        {
            // Evasion is not a base stat
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.GetStat(Stat.Evasion), Is.EqualTo(0));
        }

        #endregion

        #region Highest/Lowest Stat

        [Test]
        public void HighestStat_ReturnsCorrectStat()
        {
            // Speed is highest at 130
            var stats = new BaseStats(80, 100, 90, 110, 85, 130);
            Assert.That(stats.HighestStat, Is.EqualTo(Stat.Speed));
            Assert.That(stats.HighestStatValue, Is.EqualTo(130));
        }

        [Test]
        public void LowestStat_ReturnsCorrectStat()
        {
            // HP is lowest at 80
            var stats = new BaseStats(80, 100, 90, 110, 85, 130);
            Assert.That(stats.LowestStat, Is.EqualTo(Stat.HP));
            Assert.That(stats.LowestStatValue, Is.EqualTo(80));
        }

        [Test]
        public void HighestStat_WhenTied_ReturnsFirst()
        {
            // Attack and SpAttack both 100
            var stats = new BaseStats(80, 100, 90, 100, 85, 95);
            // Should return Attack (first in order)
            Assert.That(stats.HighestStatValue, Is.EqualTo(100));
        }

        [Test]
        public void LowestStat_WhenTied_ReturnsFirst()
        {
            // HP and Defense both 80
            var stats = new BaseStats(80, 100, 80, 100, 85, 95);
            Assert.That(stats.LowestStatValue, Is.EqualTo(80));
        }

        [Test]
        public void HighestStat_AllEqual_ReturnsHP()
        {
            var stats = new BaseStats(100, 100, 100, 100, 100, 100);
            Assert.That(stats.HighestStat, Is.EqualTo(Stat.HP));
            Assert.That(stats.HighestStatValue, Is.EqualTo(100));
        }

        #endregion

        #region Stat Validation

        [Test]
        public void IsValid_AllPositive_ReturnsTrue()
        {
            var stats = new BaseStats(100, 80, 70, 90, 85, 95);
            Assert.That(stats.IsValid, Is.True);
        }

        [Test]
        public void IsValid_HasZeroStat_ReturnsFalse()
        {
            var stats = new BaseStats(100, 0, 70, 90, 85, 95);
            Assert.That(stats.IsValid, Is.False);
        }

        [Test]
        public void IsValid_HasNegativeStat_ReturnsFalse()
        {
            var stats = new BaseStats(100, -10, 70, 90, 85, 95);
            Assert.That(stats.IsValid, Is.False);
        }

        [Test]
        public void IsValid_AllZero_ReturnsFalse()
        {
            var stats = new BaseStats(0, 0, 0, 0, 0, 0);
            Assert.That(stats.IsValid, Is.False);
        }

        [Test]
        public void IsValid_ExceedsMax255_ReturnsFalse()
        {
            var stats = new BaseStats(300, 80, 70, 90, 85, 95);
            Assert.That(stats.IsValid, Is.False);
        }

        #endregion

        #region Stat Categories

        [Test]
        public void PhysicalStats_ReturnsAttackAndDefense()
        {
            var stats = new BaseStats(100, 120, 80, 90, 85, 95);
            Assert.That(stats.PhysicalAttack, Is.EqualTo(120));
            Assert.That(stats.PhysicalDefense, Is.EqualTo(80));
        }

        [Test]
        public void SpecialStats_ReturnsSpAttackAndSpDefense()
        {
            var stats = new BaseStats(100, 120, 80, 90, 85, 95);
            Assert.That(stats.SpecialAttack, Is.EqualTo(90));
            Assert.That(stats.SpecialDefense, Is.EqualTo(85));
        }

        [Test]
        public void IsPhysicalAttacker_HigherAttack_ReturnsTrue()
        {
            var stats = new BaseStats(100, 120, 80, 90, 85, 95);
            Assert.That(stats.IsPhysicalAttacker, Is.True);
        }

        [Test]
        public void IsSpecialAttacker_HigherSpAttack_ReturnsTrue()
        {
            var stats = new BaseStats(100, 80, 80, 120, 85, 95);
            Assert.That(stats.IsSpecialAttacker, Is.True);
        }

        [Test]
        public void IsMixedAttacker_EqualAttacks_ReturnsTrue()
        {
            var stats = new BaseStats(100, 100, 80, 100, 85, 95);
            Assert.That(stats.IsMixedAttacker, Is.True);
        }

        [Test]
        public void DefensiveTotal_ReturnsSumOfDefenses()
        {
            var stats = new BaseStats(100, 80, 90, 70, 110, 95);
            // HP + Defense + SpDefense = 100 + 90 + 110 = 300
            Assert.That(stats.DefensiveTotal, Is.EqualTo(300));
        }

        [Test]
        public void OffensiveTotal_ReturnsSumOfAttacks()
        {
            var stats = new BaseStats(100, 80, 90, 70, 110, 95);
            // Attack + SpAttack = 80 + 70 = 150
            Assert.That(stats.OffensiveTotal, Is.EqualTo(150));
        }

        #endregion

        #region Default Constructor

        [Test]
        public void DefaultConstructor_AllZero()
        {
            var stats = new BaseStats();
            Assert.That(stats.HP, Is.EqualTo(0));
            Assert.That(stats.Attack, Is.EqualTo(0));
            Assert.That(stats.Defense, Is.EqualTo(0));
            Assert.That(stats.SpAttack, Is.EqualTo(0));
            Assert.That(stats.SpDefense, Is.EqualTo(0));
            Assert.That(stats.Speed, Is.EqualTo(0));
            Assert.That(stats.Total, Is.EqualTo(0));
        }

        [Test]
        public void SetterModification_UpdatesTotal()
        {
            var stats = new BaseStats();
            stats.HP = 100;
            Assert.That(stats.Total, Is.EqualTo(100));
            
            stats.Attack = 50;
            Assert.That(stats.Total, Is.EqualTo(150));
        }

        #endregion
    }
}

