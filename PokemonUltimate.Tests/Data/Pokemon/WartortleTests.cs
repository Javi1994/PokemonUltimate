using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Wartortle in PokemonCatalog.
    /// Verifies correct data, evolution line, and gender ratio.
    /// </summary>
    [TestFixture]
    public class WartortleTests
    {
        [Test]
        public void Wartortle_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Squirtle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Wartortle));
                Assert.That(PokemonCatalog.Wartortle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Blastoise));
            });
        }

        [Test]
        public void Wartortle_IsWaterType()
        {
            var pokemon = PokemonCatalog.Wartortle;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(pokemon.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Wartortle_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Wartortle.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

