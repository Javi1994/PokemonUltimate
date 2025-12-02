using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Tests.Systems.Core.Registry
{
    /// <summary>
    /// Tests for basic PokemonRegistry operations: Register, Retrieve by Name, Exists, GetAll
    /// </summary>
    [TestFixture]
    public class PokemonRegistryTests
    {
        private PokemonRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new PokemonRegistry();
        }

        #region Register and Retrieve by Name

        [Test]
        public void Test_Register_And_Retrieve_By_Name()
        {
            var pika = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            _registry.Register(pika);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.Exists("Pikachu"), Is.True);
                Assert.That(_registry.GetByName("Pikachu"), Is.EqualTo(pika));
            });
        }

        [Test]
        public void Test_GetByName_NonExistent_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _registry.GetByName("MissingNo"));
        }

        [Test]
        public void Test_Name_Is_Case_Sensitive()
        {
            var pika = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            _registry.Register(pika);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.Exists("Pikachu"), Is.True);
                Assert.That(_registry.Exists("pikachu"), Is.False, "Registry should be case sensitive");
                Assert.That(_registry.Exists("PIKACHU"), Is.False, "Registry should be case sensitive");
            });
        }

        [Test]
        public void Test_Exists_Returns_False_For_NonExistent()
        {
            Assert.That(_registry.Exists("NonExistent"), Is.False);
        }

        #endregion

        #region Multiple Pokemon

        [Test]
        public void Test_Register_Multiple_Pokemon()
        {
            var bulbasaur = new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 };
            var charmander = new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 };
            var squirtle = new PokemonSpeciesData { Name = "Squirtle", PokedexNumber = 7 };

            _registry.Register(bulbasaur);
            _registry.Register(charmander);
            _registry.Register(squirtle);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Bulbasaur"), Is.EqualTo(bulbasaur));
                Assert.That(_registry.GetByName("Charmander"), Is.EqualTo(charmander));
                Assert.That(_registry.GetByName("Squirtle"), Is.EqualTo(squirtle));
            });
        }

        #endregion

        #region GetAll Tests

        [Test]
        public void Test_GetAll_Returns_All_Registered_Pokemon()
        {
            var bulbasaur = new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 };
            var charmander = new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 };
            var squirtle = new PokemonSpeciesData { Name = "Squirtle", PokedexNumber = 7 };

            _registry.Register(bulbasaur);
            _registry.Register(charmander);
            _registry.Register(squirtle);

            var allPokemon = _registry.GetAll().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(allPokemon, Has.Count.EqualTo(3));
                Assert.That(allPokemon, Does.Contain(bulbasaur));
                Assert.That(allPokemon, Does.Contain(charmander));
                Assert.That(allPokemon, Does.Contain(squirtle));
            });
        }

        [Test]
        public void Test_GetAll_Returns_Empty_When_No_Pokemon_Registered()
        {
            var allPokemon = _registry.GetAll().ToList();

            Assert.That(allPokemon, Is.Empty);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Test_Overwrite_Duplicate_Name()
        {
            var pika1 = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            var pika2 = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 26 };

            _registry.Register(pika1);
            _registry.Register(pika2); // Should overwrite

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Pikachu"), Is.EqualTo(pika2));
                Assert.That(_registry.GetByName("Pikachu"), Is.Not.EqualTo(pika1));
            });
        }

        [Test]
        public void Test_Register_Null_Throws()
        {
            Assert.Throws<NullReferenceException>(() => _registry.Register(null!));
        }

        #endregion
    }
}

