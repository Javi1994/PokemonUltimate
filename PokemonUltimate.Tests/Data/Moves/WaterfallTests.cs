using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Waterfall move in MoveCatalog.
    /// Verifies correct data, physical category, power, and flinch effect.
    /// </summary>
    [TestFixture]
    public class WaterfallTests
    {
        [Test]
        public void Waterfall_MayFlinch()
        {
            var move = MoveCatalog.Waterfall;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Waterfall"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(move.Power, Is.EqualTo(80));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.FlinchEffect>(), Is.True);

                var effect = move.GetEffect<PokemonUltimate.Core.Effects.FlinchEffect>();
                Assert.That(effect.ChancePercent, Is.EqualTo(20));
            });
        }
    }
}

