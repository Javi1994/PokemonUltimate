using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Growl move in MoveCatalog.
    /// Verifies correct data, status category, and stat-lowering effect.
    /// </summary>
    [TestFixture]
    public class GrowlTests
    {
        [Test]
        public void Growl_LowersAttack()
        {
            var move = MoveCatalog.Growl;

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.StatChangeEffect>(), Is.True);

                var effect = move.GetEffect<PokemonUltimate.Core.Effects.StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(effect.Stages, Is.EqualTo(-1));
            });
        }

        [Test]
        public void Growl_TargetsAllEnemies()
        {
            var move = MoveCatalog.Growl;
            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
        }
    }
}
