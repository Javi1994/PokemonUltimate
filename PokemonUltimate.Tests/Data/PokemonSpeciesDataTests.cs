using PokemonUltimate.Core.Data;

namespace PokemonUltimate.Tests.Data
{
    // Tests for PokemonSpeciesData model: property values, defaults, and IIdentifiable implementation
    public class PokemonSpeciesDataTests
    {
        #region Property Tests

        [Test]
        public void Test_Properties_Are_Set_Correctly()
        {
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25
            };

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Pikachu"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(25));
            });
        }

        [Test]
        public void Test_Default_Values()
        {
            var pokemon = new PokemonSpeciesData();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo(string.Empty));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(0));
            });
        }

        #endregion

        #region IIdentifiable Tests

        [Test]
        public void Test_Id_Returns_Name()
        {
            var pokemon = new PokemonSpeciesData { Name = "Charizard" };

            Assert.That(pokemon.Id, Is.EqualTo("Charizard"));
        }

        [Test]
        public void Test_Id_Returns_Empty_When_Name_Is_Default()
        {
            var pokemon = new PokemonSpeciesData();

            Assert.That(pokemon.Id, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Test_Id_Updates_When_Name_Changes()
        {
            var pokemon = new PokemonSpeciesData { Name = "Charmander" };
            Assert.That(pokemon.Id, Is.EqualTo("Charmander"));

            pokemon.Name = "Charmeleon";
            Assert.That(pokemon.Id, Is.EqualTo("Charmeleon"));
        }

        #endregion

        #region Various Pokemon Tests

        [Test]
        [TestCase("Bulbasaur", 1)]
        [TestCase("Ivysaur", 2)]
        [TestCase("Venusaur", 3)]
        [TestCase("Charmander", 4)]
        [TestCase("Charmeleon", 5)]
        [TestCase("Charizard", 6)]
        [TestCase("Squirtle", 7)]
        [TestCase("Wartortle", 8)]
        [TestCase("Blastoise", 9)]
        [TestCase("Pikachu", 25)]
        [TestCase("Mew", 151)]
        public void Test_Pokemon_With_Various_Pokedex_Numbers(string name, int pokedexNumber)
        {
            var pokemon = new PokemonSpeciesData
            {
                Name = name,
                PokedexNumber = pokedexNumber
            };

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo(name));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(pokedexNumber));
                Assert.That(pokemon.Id, Is.EqualTo(name));
            });
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Test_Name_With_Spaces()
        {
            var pokemon = new PokemonSpeciesData { Name = "Mr. Mime" };

            Assert.That(pokemon.Name, Is.EqualTo("Mr. Mime"));
            Assert.That(pokemon.Id, Is.EqualTo("Mr. Mime"));
        }

        [Test]
        public void Test_Name_With_Special_Characters()
        {
            var pokemon = new PokemonSpeciesData { Name = "Nidoran♀" };

            Assert.That(pokemon.Name, Is.EqualTo("Nidoran♀"));
        }

        [Test]
        public void Test_High_Pokedex_Number()
        {
            // Testing for Gen 9 Pokemon numbers
            var pokemon = new PokemonSpeciesData
            {
                Name = "Koraidon",
                PokedexNumber = 1007
            };

            Assert.That(pokemon.PokedexNumber, Is.EqualTo(1007));
        }

        #endregion
    }
}

