using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Earthquake move in MoveCatalog.
    /// Verifies correct data, AoE target scope, power, and type.
    /// </summary>
    [TestFixture]
    public class EarthquakeTests
    {
        [Test]
        public void Earthquake_HitsAllOthers()
        {
            var move = MoveCatalog.Earthquake;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Earthquake"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ground));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllOthers));
                Assert.That(move.Power, Is.EqualTo(100));
            });
        }

        [Test]
        public void Earthquake_HasDamageEffect()
        {
            var move = MoveCatalog.Earthquake;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}

