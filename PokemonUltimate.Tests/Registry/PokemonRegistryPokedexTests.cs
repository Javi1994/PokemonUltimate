using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Tests.Registry
{
    /// <summary>
    /// Tests for PokemonRegistry Pokedex Number operations: GetByPokedexNumber, ExistsByPokedexNumber
    /// </summary>
    [TestFixture]
    public class PokemonRegistryPokedexTests
    {
        private PokemonRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new PokemonRegistry();
        }

        #region GetByPokedexNumber Tests

        [Test]
        public void Test_Register_And_Retrieve_By_PokedexNumber()
        {
            var pika = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            _registry.Register(pika);

            Assert.That(_registry.GetByPokedexNumber(25), Is.EqualTo(pika));
        }

        [Test]
        public void Test_GetByPokedexNumber_NonExistent_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _registry.GetByPokedexNumber(999));
        }

        [Test]
        public void Test_GetByPokedexNumber_Multiple_Pokemon()
        {
            var bulbasaur = new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 };
            var charmander = new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 };
            var squirtle = new PokemonSpeciesData { Name = "Squirtle", PokedexNumber = 7 };

            _registry.Register(bulbasaur);
            _registry.Register(charmander);
            _registry.Register(squirtle);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByPokedexNumber(1), Is.EqualTo(bulbasaur));
                Assert.That(_registry.GetByPokedexNumber(4), Is.EqualTo(charmander));
                Assert.That(_registry.GetByPokedexNumber(7), Is.EqualTo(squirtle));
            });
        }

        #endregion

        #region ExistsByPokedexNumber Tests

        [Test]
        public void Test_ExistsByPokedexNumber_Returns_True_When_Exists()
        {
            var pika = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            _registry.Register(pika);

            Assert.That(_registry.ExistsByPokedexNumber(25), Is.True);
        }

        [Test]
        public void Test_ExistsByPokedexNumber_Returns_False_When_Not_Exists()
        {
            Assert.That(_registry.ExistsByPokedexNumber(999), Is.False);
        }

        [Test]
        public void Test_ExistsByPokedexNumber_Multiple_Pokemon()
        {
            _registry.Register(new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 });
            _registry.Register(new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 });

            Assert.Multiple(() =>
            {
                Assert.That(_registry.ExistsByPokedexNumber(1), Is.True);
                Assert.That(_registry.ExistsByPokedexNumber(4), Is.True);
                Assert.That(_registry.ExistsByPokedexNumber(7), Is.False); // Squirtle not registered
            });
        }

        #endregion

        #region Dual Lookup Tests

        [Test]
        public void Test_Same_Pokemon_Retrieved_By_Name_And_Number()
        {
            var charizard = new PokemonSpeciesData { Name = "Charizard", PokedexNumber = 6 };
            _registry.Register(charizard);

            var byName = _registry.GetByName("Charizard");
            var byNumber = _registry.GetByPokedexNumber(6);

            Assert.That(byName, Is.SameAs(byNumber), "Both lookups should return the same instance");
        }

        [Test]
        public void Test_All_Starters_Dual_Lookup()
        {
            var bulbasaur = new PokemonSpeciesData { Name = "Bulbasaur", PokedexNumber = 1 };
            var charmander = new PokemonSpeciesData { Name = "Charmander", PokedexNumber = 4 };
            var squirtle = new PokemonSpeciesData { Name = "Squirtle", PokedexNumber = 7 };

            _registry.Register(bulbasaur);
            _registry.Register(charmander);
            _registry.Register(squirtle);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Bulbasaur"), Is.SameAs(_registry.GetByPokedexNumber(1)));
                Assert.That(_registry.GetByName("Charmander"), Is.SameAs(_registry.GetByPokedexNumber(4)));
                Assert.That(_registry.GetByName("Squirtle"), Is.SameAs(_registry.GetByPokedexNumber(7)));
            });
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Test_Pokemon_With_Zero_PokedexNumber_Not_Indexed_By_Number()
        {
            var mystery = new PokemonSpeciesData { Name = "MissingNo", PokedexNumber = 0 };
            _registry.Register(mystery);

            Assert.Multiple(() =>
            {
                // Should be retrievable by name
                Assert.That(_registry.GetByName("MissingNo"), Is.EqualTo(mystery));
                // Should NOT be indexed by number (0 is invalid)
                Assert.That(_registry.ExistsByPokedexNumber(0), Is.False);
            });
        }

        [Test]
        public void Test_Negative_PokedexNumber_Not_Indexed()
        {
            var glitch = new PokemonSpeciesData { Name = "GlitchMon", PokedexNumber = -1 };
            _registry.Register(glitch);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("GlitchMon"), Is.EqualTo(glitch));
                Assert.That(_registry.ExistsByPokedexNumber(-1), Is.False);
            });
        }

        [Test]
        public void Test_Overwrite_Updates_Pokedex_Index()
        {
            var pika1 = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 25 };
            var pika2 = new PokemonSpeciesData { Name = "Pikachu", PokedexNumber = 26 };

            _registry.Register(pika1);
            _registry.Register(pika2);

            Assert.Multiple(() =>
            {
                // New number should work
                Assert.That(_registry.ExistsByPokedexNumber(26), Is.True);
                Assert.That(_registry.GetByPokedexNumber(26), Is.EqualTo(pika2));
                // Old number still points to old object (known limitation)
                // This is a design decision - we don't clean up old indices
            });
        }

        #endregion
    }
}

