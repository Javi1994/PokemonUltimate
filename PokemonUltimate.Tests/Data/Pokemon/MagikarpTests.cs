using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Magikarp in PokemonCatalog.
    /// Verifies correct data, low stats, evolution to Gyarados, and type.
    /// </summary>
    [TestFixture]
    public class MagikarpTests
    {
        [Test]
        public void Magikarp_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Magikarp;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Magikarp"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(129));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(200)); // Very low BST
                Assert.That(pokemon.BaseStats.Attack, Is.EqualTo(10)); // Extremely low Attack
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(80)); // But decent Speed
            });
        }

        [Test]
        public void Magikarp_EvolvesToGyarados()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Magikarp.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Magikarp.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Gyarados));
                Assert.That(PokemonCatalog.Magikarp.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(20));
                Assert.That(PokemonCatalog.Gyarados.CanEvolve, Is.False);
            });
        }
    }
}

