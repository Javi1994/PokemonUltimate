using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Splash move in MoveCatalog.
    /// Verifies correct data, status category, and no effects (does nothing).
    /// </summary>
    [TestFixture]
    public class SplashTests
    {
        [Test]
        public void Splash_HasNoEffects()
        {
            var move = MoveCatalog.Splash;

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Splash"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Normal));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Effects.Count, Is.EqualTo(0)); // Does nothing
            });
        }
    }
}

