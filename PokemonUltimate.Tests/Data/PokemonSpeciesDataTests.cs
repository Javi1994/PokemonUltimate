using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Data
{
    /// <summary>
    /// Tests for PokemonSpeciesData model: property values, defaults, types, stats, and IIdentifiable implementation.
    /// </summary>
    public class PokemonSpeciesDataTests
    {
        #region Basic Property Tests

        [Test]
        public void Test_Properties_Are_Set_Correctly()
        {
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric
            };

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Pikachu"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(25));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Electric));
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
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Normal)); // Default enum value
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.BaseStats, Is.Not.Null);
            });
        }

        #endregion

        #region Type Tests

        [Test]
        public void Test_MonoType_Pokemon()
        {
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PrimaryType = PokemonType.Electric,
                SecondaryType = null
            };

            Assert.Multiple(() =>
            {
                Assert.That(pikachu.PrimaryType, Is.EqualTo(PokemonType.Electric));
                Assert.That(pikachu.SecondaryType, Is.Null);
                Assert.That(pikachu.IsDualType, Is.False);
            });
        }

        [Test]
        public void Test_DualType_Pokemon()
        {
            var charizard = new PokemonSpeciesData
            {
                Name = "Charizard",
                PrimaryType = PokemonType.Fire,
                SecondaryType = PokemonType.Flying
            };

            Assert.Multiple(() =>
            {
                Assert.That(charizard.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(charizard.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(charizard.IsDualType, Is.True);
            });
        }

        [Test]
        public void Test_HasType_Returns_True_For_PrimaryType()
        {
            var pokemon = new PokemonSpeciesData
            {
                PrimaryType = PokemonType.Fire,
                SecondaryType = PokemonType.Flying
            };

            Assert.That(pokemon.HasType(PokemonType.Fire), Is.True);
        }

        [Test]
        public void Test_HasType_Returns_True_For_SecondaryType()
        {
            var pokemon = new PokemonSpeciesData
            {
                PrimaryType = PokemonType.Fire,
                SecondaryType = PokemonType.Flying
            };

            Assert.That(pokemon.HasType(PokemonType.Flying), Is.True);
        }

        [Test]
        public void Test_HasType_Returns_False_For_Unrelated_Type()
        {
            var pokemon = new PokemonSpeciesData
            {
                PrimaryType = PokemonType.Fire,
                SecondaryType = PokemonType.Flying
            };

            Assert.That(pokemon.HasType(PokemonType.Water), Is.False);
        }

        [Test]
        public void Test_HasType_Works_For_MonoType()
        {
            var pokemon = new PokemonSpeciesData
            {
                PrimaryType = PokemonType.Electric,
                SecondaryType = null
            };

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.HasType(PokemonType.Electric), Is.True);
                Assert.That(pokemon.HasType(PokemonType.Normal), Is.False);
            });
        }

        #endregion

        #region BaseStats Tests

        [Test]
        public void Test_BaseStats_Default_Is_Not_Null()
        {
            var pokemon = new PokemonSpeciesData();

            Assert.That(pokemon.BaseStats, Is.Not.Null);
        }

        [Test]
        public void Test_BaseStats_Are_Set_Correctly()
        {
            var pokemon = new PokemonSpeciesData
            {
                Name = "Pikachu",
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90)
            };

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(35));
                Assert.That(pokemon.BaseStats.Attack, Is.EqualTo(55));
                Assert.That(pokemon.BaseStats.Defense, Is.EqualTo(40));
                Assert.That(pokemon.BaseStats.SpAttack, Is.EqualTo(50));
                Assert.That(pokemon.BaseStats.SpDefense, Is.EqualTo(50));
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(90));
            });
        }

        [Test]
        public void Test_BaseStats_Total_Calculated_Correctly()
        {
            var stats = new BaseStats(35, 55, 40, 50, 50, 90);

            Assert.That(stats.Total, Is.EqualTo(320)); // Pikachu's BST
        }

        [Test]
        public void Test_BaseStats_Total_For_Legendary()
        {
            var mewtwoStats = new BaseStats(106, 110, 90, 154, 90, 130);

            Assert.That(mewtwoStats.Total, Is.EqualTo(680)); // Mewtwo's BST
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

        #region Full Pokemon Examples

        [Test]
        public void Test_Complete_Pokemon_Definition()
        {
            var charizard = new PokemonSpeciesData
            {
                Name = "Charizard",
                PokedexNumber = 6,
                PrimaryType = PokemonType.Fire,
                SecondaryType = PokemonType.Flying,
                BaseStats = new BaseStats(78, 84, 78, 109, 85, 100)
            };

            Assert.Multiple(() =>
            {
                Assert.That(charizard.Name, Is.EqualTo("Charizard"));
                Assert.That(charizard.PokedexNumber, Is.EqualTo(6));
                Assert.That(charizard.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(charizard.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(charizard.IsDualType, Is.True);
                Assert.That(charizard.HasType(PokemonType.Fire), Is.True);
                Assert.That(charizard.HasType(PokemonType.Flying), Is.True);
                Assert.That(charizard.HasType(PokemonType.Dragon), Is.False);
                Assert.That(charizard.BaseStats.Total, Is.EqualTo(534));
            });
        }

        [Test]
        [TestCase("Bulbasaur", 1, PokemonType.Grass, PokemonType.Poison, 318)]
        [TestCase("Charmander", 4, PokemonType.Fire, null, 309)]
        [TestCase("Squirtle", 7, PokemonType.Water, null, 314)]
        [TestCase("Pikachu", 25, PokemonType.Electric, null, 320)]
        public void Test_Various_Pokemon(string name, int dexNum, PokemonType primary, PokemonType? secondary, int bst)
        {
            var pokemon = new PokemonSpeciesData
            {
                Name = name,
                PokedexNumber = dexNum,
                PrimaryType = primary,
                SecondaryType = secondary,
                BaseStats = CreateStatsWithTotal(bst)
            };

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo(name));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(dexNum));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(primary));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(secondary));
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(bst));
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

        #region Helpers

        private static BaseStats CreateStatsWithTotal(int targetTotal)
        {
            // Creates balanced stats that sum to approximately the target
            int perStat = targetTotal / 6;
            int remainder = targetTotal % 6;
            return new BaseStats(perStat + remainder, perStat, perStat, perStat, perStat, perStat);
        }

        #endregion
    }
}
