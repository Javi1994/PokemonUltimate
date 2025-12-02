using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Squirtle in PokemonCatalog.
    /// Verifies correct data, type, evolution line, and gender ratio.
    /// </summary>
    [TestFixture]
    public class SquirtleTests
    {
        [Test]
        public void Squirtle_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Squirtle;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Squirtle"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(7));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(pokemon.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Squirtle_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Squirtle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Wartortle));
                Assert.That(PokemonCatalog.Wartortle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Blastoise));
                Assert.That(PokemonCatalog.Blastoise.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Squirtle_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Squirtle.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

