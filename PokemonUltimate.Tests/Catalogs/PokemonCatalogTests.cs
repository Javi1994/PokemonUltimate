using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs
{
    /// <summary>
    /// Tests for PokemonCatalog: static access, types, stats, All enumeration, and RegisterAll.
    /// </summary>
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
                Assert.That(pikachu.PrimaryType, Is.EqualTo(PokemonType.Electric));
                Assert.That(pikachu.SecondaryType, Is.Null);
                Assert.That(pikachu.BaseStats.Total, Is.EqualTo(320));
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

        #region Type Tests

        [Test]
        public void Test_Starters_Have_Correct_Types()
        {
            Assert.Multiple(() =>
            {
                // Grass starter is Grass/Poison
                Assert.That(PokemonCatalog.Bulbasaur.PrimaryType, Is.EqualTo(PokemonType.Grass));
                Assert.That(PokemonCatalog.Bulbasaur.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(PokemonCatalog.Bulbasaur.IsDualType, Is.True);
                
                // Fire starter is pure Fire
                Assert.That(PokemonCatalog.Charmander.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(PokemonCatalog.Charmander.SecondaryType, Is.Null);
                Assert.That(PokemonCatalog.Charmander.IsDualType, Is.False);
                
                // Water starter is pure Water
                Assert.That(PokemonCatalog.Squirtle.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(PokemonCatalog.Squirtle.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Test_Charizard_Is_Fire_Flying()
        {
            var charizard = PokemonCatalog.Charizard;

            Assert.Multiple(() =>
            {
                Assert.That(charizard.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(charizard.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(charizard.HasType(PokemonType.Fire), Is.True);
                Assert.That(charizard.HasType(PokemonType.Flying), Is.True);
                Assert.That(charizard.HasType(PokemonType.Dragon), Is.False);
            });
        }

        [Test]
        public void Test_Legendary_Types()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Mewtwo.PrimaryType, Is.EqualTo(PokemonType.Psychic));
                Assert.That(PokemonCatalog.Mewtwo.SecondaryType, Is.Null);
                Assert.That(PokemonCatalog.Mew.PrimaryType, Is.EqualTo(PokemonType.Psychic));
            });
        }

        [Test]
        public void Test_All_Pokemon_Have_Valid_PrimaryType()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.That(Enum.IsDefined(typeof(PokemonType), pokemon.PrimaryType), Is.True,
                    $"{pokemon.Name} should have a valid PrimaryType");
            }
        }

        #endregion

        #region BaseStats Tests

        [Test]
        public void Test_All_Pokemon_Have_Valid_Stats()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(pokemon.BaseStats, Is.Not.Null, $"{pokemon.Name} should have BaseStats");
                    Assert.That(pokemon.BaseStats.Total, Is.GreaterThan(0), $"{pokemon.Name} should have positive BST");
                    Assert.That(pokemon.BaseStats.HP, Is.GreaterThan(0), $"{pokemon.Name} should have positive HP");
                });
            }
        }

        [Test]
        public void Test_Starter_Evolution_Stats_Increase()
        {
            Assert.Multiple(() =>
            {
                // BST should increase with evolution
                Assert.That(PokemonCatalog.Ivysaur.BaseStats.Total, 
                    Is.GreaterThan(PokemonCatalog.Bulbasaur.BaseStats.Total));
                Assert.That(PokemonCatalog.Venusaur.BaseStats.Total, 
                    Is.GreaterThan(PokemonCatalog.Ivysaur.BaseStats.Total));
                
                Assert.That(PokemonCatalog.Charmeleon.BaseStats.Total, 
                    Is.GreaterThan(PokemonCatalog.Charmander.BaseStats.Total));
                Assert.That(PokemonCatalog.Charizard.BaseStats.Total, 
                    Is.GreaterThan(PokemonCatalog.Charmeleon.BaseStats.Total));
            });
        }

        [Test]
        public void Test_Pikachu_Has_Correct_Stats()
        {
            var stats = PokemonCatalog.Pikachu.BaseStats;

            Assert.Multiple(() =>
            {
                Assert.That(stats.HP, Is.EqualTo(35));
                Assert.That(stats.Attack, Is.EqualTo(55));
                Assert.That(stats.Defense, Is.EqualTo(40));
                Assert.That(stats.SpAttack, Is.EqualTo(50));
                Assert.That(stats.SpDefense, Is.EqualTo(50));
                Assert.That(stats.Speed, Is.EqualTo(90));
            });
        }

        [Test]
        public void Test_Legendary_Have_High_BST()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Mewtwo.BaseStats.Total, Is.EqualTo(680));
                Assert.That(PokemonCatalog.Mew.BaseStats.Total, Is.EqualTo(600));
            });
        }

        [Test]
        public void Test_Snorlax_Has_Extreme_HP()
        {
            var snorlax = PokemonCatalog.Snorlax;

            Assert.Multiple(() =>
            {
                Assert.That(snorlax.BaseStats.HP, Is.EqualTo(160));
                Assert.That(snorlax.BaseStats.Speed, Is.EqualTo(30)); // Very slow
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
