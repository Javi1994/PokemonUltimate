using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Psychic move in MoveCatalog.
    /// Verifies correct data, power, accuracy, and stat-lowering effect.
    /// </summary>
    [TestFixture]
    public class PsychicTests
    {
        [Test]
        public void Psychic_MayLowerSpDef()
        {
            var move = MoveCatalog.Psychic;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Psychic"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.StatChangeEffect>(), Is.True);

                var effect = move.GetEffect<PokemonUltimate.Core.Effects.StatChangeEffect>();
                Assert.That(effect.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(effect.Stages, Is.EqualTo(-1));
                Assert.That(effect.ChancePercent, Is.EqualTo(10));
            });
        }
    }
}

