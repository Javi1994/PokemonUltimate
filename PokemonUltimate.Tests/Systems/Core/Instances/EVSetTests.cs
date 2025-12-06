using NUnit.Framework;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Tests for EVSet class: constructors, properties, validation, and helper methods.
    /// </summary>
    [TestFixture]
    public class EVSetTests
    {
        #region Constructor Tests

        [Test]
        public void DefaultConstructor_OptimalDistribution()
        {
            var evs = new EVSet();

            Assert.Multiple(() =>
            {
                Assert.That(evs.HP, Is.EqualTo(CoreConstants.MaxEV));
                Assert.That(evs.Attack, Is.EqualTo(CoreConstants.MaxEV));
                Assert.That(evs.Defense, Is.EqualTo(4));
                Assert.That(evs.SpAttack, Is.EqualTo(0));
                Assert.That(evs.SpDefense, Is.EqualTo(0));
                Assert.That(evs.Speed, Is.EqualTo(0));
                Assert.That(evs.IsValid, Is.True);
                Assert.That(evs.Total, Is.EqualTo(508));
            });
        }

        [Test]
        public void ParameterizedConstructor_ValuesSetCorrectly()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);

            Assert.Multiple(() =>
            {
                Assert.That(evs.HP, Is.EqualTo(252));
                Assert.That(evs.Attack, Is.EqualTo(252));
                Assert.That(evs.Defense, Is.EqualTo(4));
                Assert.That(evs.SpAttack, Is.EqualTo(0));
                Assert.That(evs.SpDefense, Is.EqualTo(0));
                Assert.That(evs.Speed, Is.EqualTo(0));
            });
        }

        #endregion

        #region Validation Tests

        [Test]
        public void ValidEVs_IsValidReturnsTrue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.IsValid, Is.True);
        }

        [Test]
        public void MaximumEVs_IsValidReturnsTrue()
        {
            // Maximum valid distribution: 252/252/4/0/0/0 = 508
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.IsValid, Is.True);
        }

        [Test]
        public void ZeroEVs_IsValidReturnsTrue()
        {
            var evs = new EVSet(0, 0, 0, 0, 0, 0);
            Assert.That(evs.IsValid, Is.True);
        }

        [Test]
        public void EVBelowMinimum_ThrowsArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => new EVSet(-1, 0, 0, 0, 0, 0));
        }

        [Test]
        public void EVAboveMaximum_ThrowsArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => new EVSet(253, 0, 0, 0, 0, 0));
        }

        [Test]
        public void TotalEVsExceeds510_ThrowsArgumentException()
        {
            // 252 + 252 + 7 = 511 > 510
            Assert.Throws<System.ArgumentException>(() => new EVSet(252, 252, 7, 0, 0, 0));
        }

        [Test]
        public void TotalEVsExactly510_IsValid()
        {
            // 252 + 252 + 4 + 2 = 510 (max allowed)
            var evs = new EVSet(252, 252, 4, 2, 0, 0);
            Assert.That(evs.IsValid, Is.True);
            Assert.That(evs.Total, Is.EqualTo(510));
        }

        #endregion

        #region Total Calculation Tests

        [Test]
        public void Total_SumOfAllEVs()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.Total, Is.EqualTo(508));
        }

        [Test]
        public void MaximumEVs_TotalIs508()
        {
            // Maximum valid distribution: 252/252/4/0/0/0 = 508
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.Total, Is.EqualTo(508));
        }

        [Test]
        public void ZeroEVs_TotalIsZero()
        {
            var evs = new EVSet(0, 0, 0, 0, 0, 0);
            Assert.That(evs.Total, Is.EqualTo(0));
        }

        #endregion

        #region GetEV Tests

        [Test]
        public void GetEV_HP_ReturnsCorrectValue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.HP), Is.EqualTo(252));
        }

        [Test]
        public void GetEV_Attack_ReturnsCorrectValue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.Attack), Is.EqualTo(252));
        }

        [Test]
        public void GetEV_Defense_ReturnsCorrectValue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.Defense), Is.EqualTo(4));
        }

        [Test]
        public void GetEV_SpAttack_ReturnsCorrectValue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.SpAttack), Is.EqualTo(0));
        }

        [Test]
        public void GetEV_SpDefense_ReturnsCorrectValue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.SpDefense), Is.EqualTo(0));
        }

        [Test]
        public void GetEV_Speed_ReturnsCorrectValue()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.Speed), Is.EqualTo(0));
        }

        [Test]
        public void GetEV_Accuracy_ReturnsZero()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.Accuracy), Is.EqualTo(0));
        }

        [Test]
        public void GetEV_Evasion_ReturnsZero()
        {
            var evs = new EVSet(252, 252, 4, 0, 0, 0);
            Assert.That(evs.GetEV(Stat.Evasion), Is.EqualTo(0));
        }

        #endregion

        #region Static Factory Methods Tests

        [Test]
        public void Maximum_ReturnsOptimalDistribution()
        {
            var evs = EVSet.Maximum();

            Assert.Multiple(() =>
            {
                Assert.That(evs.HP, Is.EqualTo(252));
                Assert.That(evs.Attack, Is.EqualTo(252));
                Assert.That(evs.Defense, Is.EqualTo(4));
                Assert.That(evs.SpAttack, Is.EqualTo(0));
                Assert.That(evs.SpDefense, Is.EqualTo(0));
                Assert.That(evs.Speed, Is.EqualTo(0));
                Assert.That(evs.Total, Is.EqualTo(508));
            });
        }

        [Test]
        public void Zero_ReturnsAllZero()
        {
            var evs = EVSet.Zero();

            Assert.Multiple(() =>
            {
                Assert.That(evs.HP, Is.EqualTo(0));
                Assert.That(evs.Attack, Is.EqualTo(0));
                Assert.That(evs.Defense, Is.EqualTo(0));
                Assert.That(evs.SpAttack, Is.EqualTo(0));
                Assert.That(evs.SpDefense, Is.EqualTo(0));
                Assert.That(evs.Speed, Is.EqualTo(0));
                Assert.That(evs.Total, Is.EqualTo(0));
            });
        }

        #endregion
    }
}

