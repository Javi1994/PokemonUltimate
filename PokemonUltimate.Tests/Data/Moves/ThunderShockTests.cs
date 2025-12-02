using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Thunder Shock move in MoveCatalog.
    /// Verifies correct data, power, accuracy, and effects.
    /// </summary>
    [TestFixture]
    public class ThunderShockTests
    {
        [Test]
        public void ThunderShock_Data_IsCorrect()
        {
            var move = MoveCatalog.ThunderShock;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Thunder Shock"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(move.Power, Is.EqualTo(40));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.DamageEffect>(), Is.True);
            });
        }

        [Test]
        public void ThunderShock_CanParalyze()
        {
            var move = MoveCatalog.ThunderShock;
            var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
                Assert.That(status.ChancePercent, Is.EqualTo(10));
            });
        }
    }
}

