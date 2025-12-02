using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Mewtwo in PokemonCatalog.
    /// Verifies correct data, legendary stats, type, and gender status.
    /// </summary>
    [TestFixture]
    public class MewtwoTests
    {
        [Test]
        public void Mewtwo_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Mewtwo;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Mewtwo"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(150));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Psychic));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(680)); // Legendary BST
                Assert.That(pokemon.BaseStats.SpAttack, Is.EqualTo(154)); // Highest SpAtk
                Assert.That(pokemon.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Mewtwo_IsGenderless()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Mewtwo.IsGenderless, Is.True);
                Assert.That(PokemonCatalog.Mewtwo.GenderRatio, Is.EqualTo(-1f));
            });
        }
    }
}

