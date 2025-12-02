using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Raichu in PokemonCatalog.
    /// Verifies correct data, stats comparison with Pikachu, and evolution status.
    /// </summary>
    [TestFixture]
    public class RaichuTests
    {
        [Test]
        public void Raichu_IsStrongerThanPikachu()
        {
            Assert.That(PokemonCatalog.Raichu.BaseStats.Total,
                Is.GreaterThan(PokemonCatalog.Pikachu.BaseStats.Total));
        }

        [Test]
        public void Raichu_HasDefaultGenderRatio()
        {
            Assert.That(PokemonCatalog.Raichu.GenderRatio, Is.EqualTo(50f));
        }

        [Test]
        public void Raichu_CannotEvolve()
        {
            Assert.That(PokemonCatalog.Raichu.CanEvolve, Is.False);
        }
    }
}

