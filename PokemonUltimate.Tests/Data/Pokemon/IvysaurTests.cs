using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Ivysaur in PokemonCatalog.
    /// Verifies correct data, evolution line, and gender ratio.
    /// </summary>
    [TestFixture]
    public class IvysaurTests
    {
        [Test]
        public void Ivysaur_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Bulbasaur.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Ivysaur));
                Assert.That(PokemonCatalog.Ivysaur.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Venusaur));
            });
        }

        [Test]
        public void Ivysaur_IsGrassPoisonType()
        {
            var pokemon = PokemonCatalog.Ivysaur;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Grass));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Ivysaur_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Ivysaur.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

