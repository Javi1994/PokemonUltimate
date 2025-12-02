using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Venusaur in PokemonCatalog.
    /// Verifies correct data, types, stats, and evolution status.
    /// </summary>
    [TestFixture]
    public class VenusaurTests
    {
        [Test]
        public void Venusaur_IsStrongerThanBulbasaur()
        {
            Assert.That(PokemonCatalog.Venusaur.BaseStats.Total,
                Is.GreaterThan(PokemonCatalog.Bulbasaur.BaseStats.Total));
        }

        [Test]
        public void Venusaur_CannotEvolve()
        {
            Assert.That(PokemonCatalog.Venusaur.CanEvolve, Is.False);
        }

        [Test]
        public void Venusaur_IsGrassPoisonType()
        {
            var pokemon = PokemonCatalog.Venusaur;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Grass));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Venusaur_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Venusaur.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

