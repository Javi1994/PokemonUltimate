using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Shadow Ball move in MoveCatalog.
    /// Verifies correct data, power, accuracy, category, and stat-lowering effect.
    /// </summary>
    [TestFixture]
    public class ShadowBallTests
    {
        [Test]
        public void ShadowBall_Data_IsCorrect()
        {
            var move = MoveCatalog.ShadowBall;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Shadow Ball"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ghost));
                Assert.That(move.Power, Is.EqualTo(80));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void ShadowBall_MayLowerSpDef()
        {
            var move = MoveCatalog.ShadowBall;
            var statChange = move.GetEffect<PokemonUltimate.Core.Effects.StatChangeEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(statChange, Is.Not.Null);
                Assert.That(statChange.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(statChange.Stages, Is.EqualTo(-1));
                Assert.That(statChange.ChancePercent, Is.EqualTo(20));
            });
        }
    }
}

