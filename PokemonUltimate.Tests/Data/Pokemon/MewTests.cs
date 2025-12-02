using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Mew in PokemonCatalog.
    /// Verifies correct data, balanced stats, type, and gender status.
    /// </summary>
    [TestFixture]
    public class MewTests
    {
        [Test]
        public void Mew_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Mew;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Mew"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(151));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Psychic));
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(600));
                // Mew has perfectly balanced stats
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(100));
                Assert.That(pokemon.BaseStats.Attack, Is.EqualTo(100));
                Assert.That(pokemon.BaseStats.Defense, Is.EqualTo(100));
                Assert.That(pokemon.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Mew_IsGenderless()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Mew.IsGenderless, Is.True);
                Assert.That(PokemonCatalog.Mew.GenderRatio, Is.EqualTo(-1f));
            });
        }
    }
}

