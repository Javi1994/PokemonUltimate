using NUnit.Framework;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Tests for IVSet class: constructors, properties, validation, and helper methods.
    /// </summary>
    [TestFixture]
    public class IVSetTests
    {
        #region Constructor Tests

        [Test]
        public void DefaultConstructor_AllValuesSetToMaximum()
        {
            var ivs = new IVSet();

            Assert.Multiple(() =>
            {
                Assert.That(ivs.HP, Is.EqualTo(CoreConstants.MaxIV));
                Assert.That(ivs.Attack, Is.EqualTo(CoreConstants.MaxIV));
                Assert.That(ivs.Defense, Is.EqualTo(CoreConstants.MaxIV));
                Assert.That(ivs.SpAttack, Is.EqualTo(CoreConstants.MaxIV));
                Assert.That(ivs.SpDefense, Is.EqualTo(CoreConstants.MaxIV));
                Assert.That(ivs.Speed, Is.EqualTo(CoreConstants.MaxIV));
                Assert.That(ivs.IsValid, Is.True);
            });
        }

        [Test]
        public void ParameterizedConstructor_ValuesSetCorrectly()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);

            Assert.Multiple(() =>
            {
                Assert.That(ivs.HP, Is.EqualTo(31));
                Assert.That(ivs.Attack, Is.EqualTo(30));
                Assert.That(ivs.Defense, Is.EqualTo(25));
                Assert.That(ivs.SpAttack, Is.EqualTo(20));
                Assert.That(ivs.SpDefense, Is.EqualTo(15));
                Assert.That(ivs.Speed, Is.EqualTo(10));
            });
        }

        #endregion

        #region Validation Tests

        [Test]
        public void ValidIVs_IsValidReturnsTrue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.IsValid, Is.True);
        }

        [Test]
        public void PerfectIVs_IsValidReturnsTrue()
        {
            var ivs = new IVSet(31, 31, 31, 31, 31, 31);
            Assert.That(ivs.IsValid, Is.True);
        }

        [Test]
        public void ZeroIVs_IsValidReturnsTrue()
        {
            var ivs = new IVSet(0, 0, 0, 0, 0, 0);
            Assert.That(ivs.IsValid, Is.True);
        }

        [Test]
        public void IVBelowMinimum_ThrowsArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => new IVSet(-1, 0, 0, 0, 0, 0));
        }

        [Test]
        public void IVAboveMaximum_ThrowsArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => new IVSet(32, 0, 0, 0, 0, 0));
        }

        #endregion

        #region Total Calculation Tests

        [Test]
        public void Total_SumOfAllIVs()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.Total, Is.EqualTo(131));
        }

        [Test]
        public void PerfectIVs_TotalIs186()
        {
            var ivs = new IVSet(31, 31, 31, 31, 31, 31);
            Assert.That(ivs.Total, Is.EqualTo(186));
        }

        [Test]
        public void ZeroIVs_TotalIsZero()
        {
            var ivs = new IVSet(0, 0, 0, 0, 0, 0);
            Assert.That(ivs.Total, Is.EqualTo(0));
        }

        #endregion

        #region GetIV Tests

        [Test]
        public void GetIV_HP_ReturnsCorrectValue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.HP), Is.EqualTo(31));
        }

        [Test]
        public void GetIV_Attack_ReturnsCorrectValue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.Attack), Is.EqualTo(30));
        }

        [Test]
        public void GetIV_Defense_ReturnsCorrectValue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.Defense), Is.EqualTo(25));
        }

        [Test]
        public void GetIV_SpAttack_ReturnsCorrectValue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.SpAttack), Is.EqualTo(20));
        }

        [Test]
        public void GetIV_SpDefense_ReturnsCorrectValue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.SpDefense), Is.EqualTo(15));
        }

        [Test]
        public void GetIV_Speed_ReturnsCorrectValue()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.Speed), Is.EqualTo(10));
        }

        [Test]
        public void GetIV_Accuracy_ReturnsZero()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.Accuracy), Is.EqualTo(0));
        }

        [Test]
        public void GetIV_Evasion_ReturnsZero()
        {
            var ivs = new IVSet(31, 30, 25, 20, 15, 10);
            Assert.That(ivs.GetIV(Stat.Evasion), Is.EqualTo(0));
        }

        #endregion

        #region Static Factory Methods Tests

        [Test]
        public void Perfect_ReturnsAll31()
        {
            var ivs = IVSet.Perfect();

            Assert.Multiple(() =>
            {
                Assert.That(ivs.HP, Is.EqualTo(31));
                Assert.That(ivs.Attack, Is.EqualTo(31));
                Assert.That(ivs.Defense, Is.EqualTo(31));
                Assert.That(ivs.SpAttack, Is.EqualTo(31));
                Assert.That(ivs.SpDefense, Is.EqualTo(31));
                Assert.That(ivs.Speed, Is.EqualTo(31));
                Assert.That(ivs.Total, Is.EqualTo(186));
            });
        }

        [Test]
        public void Zero_ReturnsAllZero()
        {
            var ivs = IVSet.Zero();

            Assert.Multiple(() =>
            {
                Assert.That(ivs.HP, Is.EqualTo(0));
                Assert.That(ivs.Attack, Is.EqualTo(0));
                Assert.That(ivs.Defense, Is.EqualTo(0));
                Assert.That(ivs.SpAttack, Is.EqualTo(0));
                Assert.That(ivs.SpDefense, Is.EqualTo(0));
                Assert.That(ivs.Speed, Is.EqualTo(0));
                Assert.That(ivs.Total, Is.EqualTo(0));
            });
        }

        #endregion
    }
}

