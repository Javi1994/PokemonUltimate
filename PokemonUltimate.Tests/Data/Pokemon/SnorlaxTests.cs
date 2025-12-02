using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Snorlax in PokemonCatalog.
    /// Verifies correct data, extreme stats, and type.
    /// </summary>
    [TestFixture]
    public class SnorlaxTests
    {
        [Test]
        public void Snorlax_HasExtremeStats()
        {
            var pokemon = PokemonCatalog.Snorlax;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Normal));
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(160)); // Highest HP among Gen 1
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(30)); // Very slow
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(540));
            });
        }

        [Test]
        public void Snorlax_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Snorlax.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

