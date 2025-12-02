using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Solar Beam move in MoveCatalog.
    /// Verifies correct data, high power, special category, and type.
    /// </summary>
    [TestFixture]
    public class SolarBeamTests
    {
        [Test]
        public void SolarBeam_IsPowerful()
        {
            var move = MoveCatalog.SolarBeam;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Solar Beam"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Grass));
                Assert.That(move.Power, Is.EqualTo(120));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            });
        }

        [Test]
        public void SolarBeam_HasDamageEffect()
        {
            var move = MoveCatalog.SolarBeam;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

