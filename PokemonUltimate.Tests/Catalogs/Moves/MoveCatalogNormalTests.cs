using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs.Moves
{
    /// <summary>
    /// Tests for Normal-type moves in MoveCatalog.
    /// </summary>
    public class MoveCatalogNormalTests
    {
        [Test]
        public void Test_Tackle_Data()
        {
            var move = MoveCatalog.Tackle;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Tackle"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
            });
        }

        [Test]
        public void Test_QuickAttack_Has_Priority()
        {
            var move = MoveCatalog.QuickAttack;

            Assert.Multiple(() =>
            {
                Assert.That(move.Priority, Is.EqualTo(1));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        [Test]
        public void Test_HyperBeam_Is_Strong_Special()
        {
            var move = MoveCatalog.HyperBeam;

            Assert.Multiple(() =>
            {
                Assert.That(move.Power, Is.EqualTo(150));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.Accuracy, Is.EqualTo(90));
            });
        }

        [Test]
        public void Test_Growl_Lowers_Attack()
        {
            var move = MoveCatalog.Growl;

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.HasEffect<StatChangeEffect>(), Is.True);
                
                var effect = move.GetEffect<StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(effect.Stages, Is.EqualTo(-1));
            });
        }
    }
}

