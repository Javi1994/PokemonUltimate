using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Data;

namespace PokemonUltimate.Tests.Catalogs
{
    // Tests for PokemonCatalog: static access, All enumeration, and RegisterAll
    public class PokemonCatalogTests
    {
        #region Direct Access Tests

        [Test]
        public void Test_Pikachu_Has_Correct_Data()
        {
            var pikachu = PokemonCatalog.Pikachu;

            Assert.Multiple(() =>
            {
                Assert.That(pikachu.Name, Is.EqualTo("Pikachu"));
                Assert.That(pikachu.PokedexNumber, Is.EqualTo(25));
            });
        }

        [Test]
        public void Test_Starters_Have_Correct_Pokedex_Numbers()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Bulbasaur.PokedexNumber, Is.EqualTo(1));
                Assert.That(PokemonCatalog.Charmander.PokedexNumber, Is.EqualTo(4));
                Assert.That(PokemonCatalog.Squirtle.PokedexNumber, Is.EqualTo(7));
            });
        }

        [Test]
        public void Test_Evolutions_Have_Sequential_Numbers()
        {
            Assert.Multiple(() =>
            {
                // Bulbasaur line
                Assert.That(PokemonCatalog.Bulbasaur.PokedexNumber, Is.EqualTo(1));
                Assert.That(PokemonCatalog.Ivysaur.PokedexNumber, Is.EqualTo(2));
                Assert.That(PokemonCatalog.Venusaur.PokedexNumber, Is.EqualTo(3));
                
                // Charmander line
                Assert.That(PokemonCatalog.Charmander.PokedexNumber, Is.EqualTo(4));
                Assert.That(PokemonCatalog.Charmeleon.PokedexNumber, Is.EqualTo(5));
                Assert.That(PokemonCatalog.Charizard.PokedexNumber, Is.EqualTo(6));
                
                // Squirtle line
                Assert.That(PokemonCatalog.Squirtle.PokedexNumber, Is.EqualTo(7));
                Assert.That(PokemonCatalog.Wartortle.PokedexNumber, Is.EqualTo(8));
                Assert.That(PokemonCatalog.Blastoise.PokedexNumber, Is.EqualTo(9));
            });
        }

        [Test]
        public void Test_Legendary_Pokemon_Data()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Mewtwo.Name, Is.EqualTo("Mewtwo"));
                Assert.That(PokemonCatalog.Mewtwo.PokedexNumber, Is.EqualTo(150));
                Assert.That(PokemonCatalog.Mew.Name, Is.EqualTo("Mew"));
                Assert.That(PokemonCatalog.Mew.PokedexNumber, Is.EqualTo(151));
            });
        }

        #endregion

        #region All Enumeration Tests

        [Test]
        public void Test_All_Returns_Correct_Count()
        {
            var allPokemon = PokemonCatalog.All.ToList();

            Assert.That(allPokemon, Has.Count.EqualTo(PokemonCatalog.Count));
        }

        [Test]
        public void Test_All_Contains_All_Starters()
        {
            var allPokemon = PokemonCatalog.All.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(allPokemon, Does.Contain(PokemonCatalog.Bulbasaur));
                Assert.That(allPokemon, Does.Contain(PokemonCatalog.Charmander));
                Assert.That(allPokemon, Does.Contain(PokemonCatalog.Squirtle));
            });
        }

        [Test]
        public void Test_All_Pokemon_Have_Unique_Names()
        {
            var allPokemon = PokemonCatalog.All.ToList();
            var names = allPokemon.Select(p => p.Name).ToList();

            Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count), "All Pokemon should have unique names");
        }

        [Test]
        public void Test_All_Pokemon_Have_Unique_Pokedex_Numbers()
        {
            var allPokemon = PokemonCatalog.All.ToList();
            var numbers = allPokemon.Select(p => p.PokedexNumber).ToList();

            Assert.That(numbers.Distinct().Count(), Is.EqualTo(numbers.Count), "All Pokemon should have unique Pokedex numbers");
        }

        #endregion

        #region RegisterAll Tests

        [Test]
        public void Test_RegisterAll_Populates_Registry()
        {
            var registry = new PokemonRegistry();

            PokemonCatalog.RegisterAll(registry);

            Assert.That(registry.GetAll().Count(), Is.EqualTo(PokemonCatalog.Count));
        }

        [Test]
        public void Test_RegisterAll_Pokemon_Are_Retrievable_By_Name()
        {
            var registry = new PokemonRegistry();
            PokemonCatalog.RegisterAll(registry);

            Assert.Multiple(() =>
            {
                Assert.That(registry.GetByName("Pikachu"), Is.EqualTo(PokemonCatalog.Pikachu));
                Assert.That(registry.GetByName("Charizard"), Is.EqualTo(PokemonCatalog.Charizard));
                Assert.That(registry.GetByName("Mewtwo"), Is.EqualTo(PokemonCatalog.Mewtwo));
            });
        }

        [Test]
        public void Test_RegisterAll_Pokemon_Are_Retrievable_By_Number()
        {
            var registry = new PokemonRegistry();
            PokemonCatalog.RegisterAll(registry);

            Assert.Multiple(() =>
            {
                Assert.That(registry.GetByPokedexNumber(25), Is.EqualTo(PokemonCatalog.Pikachu));
                Assert.That(registry.GetByPokedexNumber(6), Is.EqualTo(PokemonCatalog.Charizard));
                Assert.That(registry.GetByPokedexNumber(150), Is.EqualTo(PokemonCatalog.Mewtwo));
            });
        }

        #endregion
    }
}

