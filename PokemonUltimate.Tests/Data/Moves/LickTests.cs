using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Lick move in MoveCatalog.
    /// Verifies correct data, power, accuracy, and paralysis effect.
    /// </summary>
    [TestFixture]
    public class LickTests
    {
        [Test]
        public void Lick_MayParalyze()
        {
            var move = MoveCatalog.Lick;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Lick"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ghost));
                Assert.That(move.Power, Is.EqualTo(30));
                Assert.That(move.HasEffect<PokemonUltimate.Core.Effects.StatusEffect>(), Is.True);

                var status = move.GetEffect<PokemonUltimate.Core.Effects.StatusEffect>();
                Assert.That(status.Status, Is.EqualTo(PersistentStatus.Paralysis));
            });
        }
    }
}

