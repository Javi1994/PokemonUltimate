using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Registry;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Catalogs.Pokemon
{
    /// <summary>
    /// General tests for PokemonCatalog: All enumeration, Count, RegisterAll, and global validations.
    /// Generation-specific tests are in PokemonCatalogGen1Tests.cs, etc.
    /// </summary>
    public class PokemonCatalogTests
    {
        #region All Enumeration Tests

        [Test]
        public void Test_All_Returns_Correct_Count()
        {
            var allPokemon = PokemonCatalog.All.ToList();

            Assert.That(allPokemon, Has.Count.EqualTo(PokemonCatalog.Count));
        }

        [Test]
        public void Test_Count_Is_Positive()
        {
            Assert.That(PokemonCatalog.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Test_All_Pokemon_Have_Unique_Names()
        {
            var allPokemon = PokemonCatalog.All.ToList();
            var names = allPokemon.Select(p => p.Name).ToList();

            Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count), 
                "All Pokemon should have unique names");
        }

        [Test]
        public void Test_All_Pokemon_Have_Unique_Pokedex_Numbers()
        {
            var allPokemon = PokemonCatalog.All.ToList();
            var numbers = allPokemon.Select(p => p.PokedexNumber).ToList();

            Assert.That(numbers.Distinct().Count(), Is.EqualTo(numbers.Count), 
                "All Pokemon should have unique Pokedex numbers");
        }

        #endregion

        #region Global Validation Tests

        [Test]
        public void Test_All_Pokemon_Have_Valid_PrimaryType()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.That(Enum.IsDefined(typeof(PokemonType), pokemon.PrimaryType), Is.True,
                    $"{pokemon.Name} should have a valid PrimaryType");
            }
        }

        [Test]
        public void Test_All_Pokemon_Have_Valid_BaseStats()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(pokemon.BaseStats, Is.Not.Null, 
                        $"{pokemon.Name} should have BaseStats");
                    Assert.That(pokemon.BaseStats.Total, Is.GreaterThan(0), 
                        $"{pokemon.Name} should have positive BST");
                    Assert.That(pokemon.BaseStats.HP, Is.GreaterThan(0), 
                        $"{pokemon.Name} should have positive HP");
                });
            }
        }

        [Test]
        public void Test_All_Pokemon_Have_NonEmpty_Names()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.That(string.IsNullOrEmpty(pokemon.Name), Is.False,
                    "All Pokemon should have non-empty names");
            }
        }

        [Test]
        public void Test_All_Pokemon_Have_Positive_PokedexNumber()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.That(pokemon.PokedexNumber, Is.GreaterThan(0),
                    $"{pokemon.Name} should have positive Pokedex number");
            }
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
        public void Test_RegisterAll_All_Pokemon_Retrievable_By_Name()
        {
            var registry = new PokemonRegistry();
            PokemonCatalog.RegisterAll(registry);

            foreach (var pokemon in PokemonCatalog.All)
            {
                var retrieved = registry.GetByName(pokemon.Name);
                Assert.That(retrieved, Is.SameAs(pokemon),
                    $"{pokemon.Name} should be retrievable by name");
            }
        }

        [Test]
        public void Test_RegisterAll_All_Pokemon_Retrievable_By_Number()
        {
            var registry = new PokemonRegistry();
            PokemonCatalog.RegisterAll(registry);

            foreach (var pokemon in PokemonCatalog.All)
            {
                var retrieved = registry.GetByPokedexNumber(pokemon.PokedexNumber);
                Assert.That(retrieved, Is.SameAs(pokemon),
                    $"{pokemon.Name} should be retrievable by Pokedex number");
            }
        }

        #endregion
    }
}

