using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Defense Curl move in MoveCatalog.
    /// Verifies correct data, status category, and defense-raising effect.
    /// </summary>
    [TestFixture]
    public class DefenseCurlTests
    {
        [Test]
        public void DefenseCurl_RaisesDefense()
        {
            var move = MoveCatalog.DefenseCurl;

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.StatChangeEffect>(), Is.True);

                var effect = move.GetEffect<PokemonUltimate.Core.Effects.StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.Defense));
                Assert.That(effect.Stages, Is.EqualTo(1));
            });
        }
    }
}

