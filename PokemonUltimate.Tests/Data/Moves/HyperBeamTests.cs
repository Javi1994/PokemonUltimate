using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Hyper Beam move in MoveCatalog.
    /// Verifies correct data, high power, special category, and accuracy.
    /// </summary>
    [TestFixture]
    public class HyperBeamTests
    {
        [Test]
        public void HyperBeam_IsStrongSpecial()
        {
            var move = MoveCatalog.HyperBeam;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Hyper Beam"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Power, Is.EqualTo(150));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.Accuracy, Is.EqualTo(90));
            });
        }

        [Test]
        public void HyperBeam_HasDamageEffect()
        {
            var move = MoveCatalog.HyperBeam;
            Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
        }
    }
}
