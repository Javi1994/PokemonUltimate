using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Data.Moves
{
    /// <summary>
    /// Tests for Razor Leaf move in MoveCatalog.
    /// Verifies correct data, high crit ratio, power, and type.
    /// </summary>
    [TestFixture]
    public class RazorLeafTests
    {
        [Test]
        public void RazorLeaf_HasHighCrit()
        {
            var move = MoveCatalog.RazorLeaf;
            var damage = move.GetEffect<PokemonUltimate.Core.Effects.DamageEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Razor Leaf"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Grass));
                Assert.That(damage.CritStages, Is.EqualTo(1));
            });
        }
    }
}

