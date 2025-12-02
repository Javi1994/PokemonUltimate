using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for BaseStats class: constructors, properties, and Total calculation.
    /// </summary>
    [TestFixture]
    public class BaseStatsTests
    {
        #region Constructor Tests

        [Test]
        public void Test_Default_Constructor()
        {
            var stats = new BaseStats();

            Assert.Multiple(() =>
            {
                Assert.That(stats.HP, Is.EqualTo(0));
                Assert.That(stats.Attack, Is.EqualTo(0));
                Assert.That(stats.Defense, Is.EqualTo(0));
                Assert.That(stats.SpAttack, Is.EqualTo(0));
                Assert.That(stats.SpDefense, Is.EqualTo(0));
                Assert.That(stats.Speed, Is.EqualTo(0));
            });
        }

        [Test]
        public void Test_Parameterized_Constructor()
        {
            var stats = new BaseStats(100, 90, 80, 70, 60, 50);

            Assert.Multiple(() =>
            {
                Assert.That(stats.HP, Is.EqualTo(100));
                Assert.That(stats.Attack, Is.EqualTo(90));
                Assert.That(stats.Defense, Is.EqualTo(80));
                Assert.That(stats.SpAttack, Is.EqualTo(70));
                Assert.That(stats.SpDefense, Is.EqualTo(60));
                Assert.That(stats.Speed, Is.EqualTo(50));
            });
        }

        #endregion

        #region Total Calculation Tests

        [Test]
        public void Test_Total_Calculation()
        {
            var stats = new BaseStats(100, 90, 80, 70, 60, 50);

            Assert.That(stats.Total, Is.EqualTo(450));
        }

        [Test]
        public void Test_Total_With_Zero_Stats()
        {
            var stats = new BaseStats();

            Assert.That(stats.Total, Is.EqualTo(0));
        }

        [Test]
        public void Test_Total_With_Same_Values()
        {
            var stats = new BaseStats(100, 100, 100, 100, 100, 100);

            Assert.That(stats.Total, Is.EqualTo(600)); // Mew's BST
        }

        #endregion

        #region Real Pokemon Stats Tests

        [Test]
        public void Test_Pikachu_Stats()
        {
            var stats = new BaseStats(35, 55, 40, 50, 50, 90);

            Assert.Multiple(() =>
            {
                Assert.That(stats.HP, Is.EqualTo(35));
                Assert.That(stats.Attack, Is.EqualTo(55));
                Assert.That(stats.Defense, Is.EqualTo(40));
                Assert.That(stats.SpAttack, Is.EqualTo(50));
                Assert.That(stats.SpDefense, Is.EqualTo(50));
                Assert.That(stats.Speed, Is.EqualTo(90));
                Assert.That(stats.Total, Is.EqualTo(320));
            });
        }

        [Test]
        public void Test_Charizard_Stats()
        {
            var stats = new BaseStats(78, 84, 78, 109, 85, 100);

            Assert.That(stats.Total, Is.EqualTo(534));
        }

        [Test]
        public void Test_Mewtwo_Stats()
        {
            var stats = new BaseStats(106, 110, 90, 154, 90, 130);

            Assert.That(stats.Total, Is.EqualTo(680)); // Legendary BST
        }

        [Test]
        public void Test_Snorlax_Stats()
        {
            var stats = new BaseStats(160, 110, 65, 65, 110, 30);

            Assert.Multiple(() =>
            {
                Assert.That(stats.HP, Is.EqualTo(160)); // Highest HP
                Assert.That(stats.Speed, Is.EqualTo(30)); // Very slow
                Assert.That(stats.Total, Is.EqualTo(540));
            });
        }

        #endregion

        #region Property Modification Tests

        [Test]
        public void Test_Properties_Are_Mutable()
        {
            var stats = new BaseStats();

            stats.HP = 100;
            stats.Attack = 90;
            stats.Defense = 80;
            stats.SpAttack = 70;
            stats.SpDefense = 60;
            stats.Speed = 50;

            Assert.Multiple(() =>
            {
                Assert.That(stats.HP, Is.EqualTo(100));
                Assert.That(stats.Total, Is.EqualTo(450));
            });
        }

        [Test]
        public void Test_Total_Updates_After_Modification()
        {
            var stats = new BaseStats(50, 50, 50, 50, 50, 50);
            Assert.That(stats.Total, Is.EqualTo(300));

            stats.HP = 100;
            Assert.That(stats.Total, Is.EqualTo(350)); // +50 HP
        }

        #endregion
    }
}

