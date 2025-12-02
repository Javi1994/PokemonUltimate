using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Blastoise in PokemonCatalog.
    /// Verifies correct data, types, stats, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class BlastoiseTests
    {
        [Test]
        public void Blastoise_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Squirtle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Wartortle));
                Assert.That(PokemonCatalog.Wartortle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Blastoise));
                Assert.That(PokemonCatalog.Blastoise.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Blastoise_IsWaterType()
        {
            var pokemon = PokemonCatalog.Blastoise;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(pokemon.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Blastoise_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Blastoise.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

