using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Fire Blast move in MoveCatalog.
    /// Verifies correct data, high power, low accuracy, and burn effect.
    /// </summary>
    [TestFixture]
    public class FireBlastTests
    {
        [Test]
        public void FireBlast_HasHighPowerLowAccuracy()
        {
            var move = MoveCatalog.FireBlast;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Fire Blast"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(move.Power, Is.EqualTo(110));
                Assert.That(move.Accuracy, Is.EqualTo(85));
            });
        }

        [Test]
        public void FireBlast_CanBurn()
        {
            var move = MoveCatalog.FireBlast;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Burn));
            });
        }
    }
}

