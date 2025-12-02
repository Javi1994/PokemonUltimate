using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Eevee in PokemonCatalog.
    /// Verifies correct data, type, and gender ratio.
    /// </summary>
    [TestFixture]
    public class EeveeTests
    {
        [Test]
        public void Eevee_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Eevee;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(133));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Normal));
            });
        }

        [Test]
        public void Eevee_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Eevee.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

