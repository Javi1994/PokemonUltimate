using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Hydro Pump move in MoveCatalog.
    /// Verifies correct data, high power, low accuracy, and type.
    /// </summary>
    [TestFixture]
    public class HydroPumpTests
    {
        [Test]
        public void HydroPump_HasHighPowerLowAccuracy()
        {
            var move = MoveCatalog.HydroPump;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Hydro Pump"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(move.Power, Is.EqualTo(110));
                Assert.That(move.Accuracy, Is.EqualTo(80));
            });
        }

        [Test]
        public void HydroPump_HasDamageEffect()
        {
            var move = MoveCatalog.HydroPump;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}
