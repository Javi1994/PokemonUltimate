using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Teleport move in MoveCatalog.
    /// Verifies correct data, status category, and no effects (switching move deferred).
    /// </summary>
    [TestFixture]
    public class TeleportTests
    {
        [Test]
        public void Teleport_HasNoEffects()
        {
            var move = MoveCatalog.Teleport;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Teleport"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Psychic));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Effects.Count, Is.EqualTo(0)); // Switching move, no effects (deferred)
            });
        }
    }
}

