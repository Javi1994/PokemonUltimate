using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Charmeleon in PokemonCatalog.
    /// Verifies correct data, evolution line, and gender ratio.
    /// </summary>
    [TestFixture]
    public class CharmeleonTests
    {
        [Test]
        public void Charmeleon_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charmeleon));
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charizard));
            });
        }

        [Test]
        public void Charmeleon_IsFireType()
        {
            var pokemon = PokemonCatalog.Charmeleon;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(pokemon.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Charmeleon_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Charmeleon.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

