using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using System.Linq;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Catalogs.Pokemon
{
    /// <summary>
    /// Tests specific to Generation 1 Pokemon in PokemonCatalog.
    /// Verifies correct data for each Pokemon defined in PokemonCatalog.Gen1.cs.
    /// </summary>
    [TestFixture]
    public class PokemonCatalogGen1Tests
    {
        #region Starter Lines - Grass

        [Test]
        public void Test_Bulbasaur_Data()
        {
            var pokemon = PokemonCatalog.Bulbasaur;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Bulbasaur"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(1));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Grass));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(318));
            });
        }

        [Test]
        public void Test_Bulbasaur_EvolvesTo_Ivysaur()
        {
            var pokemon = PokemonCatalog.Bulbasaur;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.CanEvolve, Is.True);
                Assert.That(pokemon.Evolutions, Has.Count.EqualTo(1));
                Assert.That(pokemon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Ivysaur));
                Assert.That(pokemon.Evolutions[0].GetCondition<LevelCondition>().MinLevel, Is.EqualTo(16));
            });
        }

        [Test]
        public void Test_Venusaur_Is_Stronger_Than_Bulbasaur()
        {
            Assert.That(PokemonCatalog.Venusaur.BaseStats.Total, 
                Is.GreaterThan(PokemonCatalog.Bulbasaur.BaseStats.Total));
        }

        [Test]
        public void Test_Venusaur_Cannot_Evolve()
        {
            Assert.That(PokemonCatalog.Venusaur.CanEvolve, Is.False);
        }

        #endregion

        #region Starter Lines - Fire

        [Test]
        public void Test_Charmander_Data()
        {
            var pokemon = PokemonCatalog.Charmander;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Charmander"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(4));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.IsDualType, Is.False);
            });
        }

        [Test]
        public void Test_Charmander_Learnset()
        {
            var pokemon = PokemonCatalog.Charmander;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Learnset, Is.Not.Empty);
                Assert.That(pokemon.GetStartingMoves().Count(), Is.EqualTo(2));
                Assert.That(pokemon.CanLearn(MoveCatalog.Scratch), Is.True);
                Assert.That(pokemon.CanLearn(MoveCatalog.Ember), Is.True);
                Assert.That(pokemon.CanLearn(MoveCatalog.Flamethrower), Is.True);
            });
        }

        [Test]
        public void Test_Charmander_Evolution_Line()
        {
            Assert.Multiple(() =>
            {
                // Charmander → Charmeleon at level 16
                Assert.That(PokemonCatalog.Charmander.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charmeleon));
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].GetCondition<LevelCondition>().MinLevel, Is.EqualTo(16));

                // Charmeleon → Charizard at level 36
                Assert.That(PokemonCatalog.Charmeleon.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charizard));
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].GetCondition<LevelCondition>().MinLevel, Is.EqualTo(36));
            });
        }

        [Test]
        public void Test_Charizard_Is_Fire_Flying()
        {
            var pokemon = PokemonCatalog.Charizard;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(pokemon.HasType(PokemonType.Fire), Is.True);
                Assert.That(pokemon.HasType(PokemonType.Flying), Is.True);
                Assert.That(pokemon.HasType(PokemonType.Dragon), Is.False);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(534));
            });
        }

        #endregion

        #region Starter Lines - Water

        [Test]
        public void Test_Squirtle_Data()
        {
            var pokemon = PokemonCatalog.Squirtle;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Squirtle"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(7));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(pokemon.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Test_Squirtle_Evolution_Line()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Squirtle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Wartortle));
                Assert.That(PokemonCatalog.Wartortle.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Blastoise));
                Assert.That(PokemonCatalog.Blastoise.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Test_Evolution_Lines_Have_Sequential_Numbers()
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

        #endregion

        #region Electric Pokemon

        [Test]
        public void Test_Pikachu_Data()
        {
            var pokemon = PokemonCatalog.Pikachu;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Pikachu"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(25));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Electric));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(35));
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(90));
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(320));
            });
        }

        [Test]
        public void Test_Pikachu_Learnset()
        {
            var pokemon = PokemonCatalog.Pikachu;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.CanLearn(MoveCatalog.ThunderShock), Is.True);
                Assert.That(pokemon.CanLearn(MoveCatalog.Thunderbolt), Is.True);
                Assert.That(pokemon.CanLearn(MoveCatalog.Thunder), Is.True);
            });
        }

        [Test]
        public void Test_Pikachu_Evolves_With_Thunder_Stone()
        {
            var pokemon = PokemonCatalog.Pikachu;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.CanEvolve, Is.True);
                Assert.That(pokemon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Raichu));
                Assert.That(pokemon.Evolutions[0].HasCondition<ItemCondition>(), Is.True);
                Assert.That(pokemon.Evolutions[0].GetCondition<ItemCondition>().ItemName, Is.EqualTo("Thunder Stone"));
            });
        }

        [Test]
        public void Test_Raichu_Is_Stronger_Than_Pikachu()
        {
            Assert.That(PokemonCatalog.Raichu.BaseStats.Total, 
                Is.GreaterThan(PokemonCatalog.Pikachu.BaseStats.Total));
        }

        #endregion

        #region Normal Pokemon

        [Test]
        public void Test_Snorlax_Has_Extreme_Stats()
        {
            var pokemon = PokemonCatalog.Snorlax;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Normal));
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(160)); // Highest HP among Gen 1
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(30)); // Very slow
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(540));
            });
        }

        [Test]
        public void Test_Eevee_Data()
        {
            var pokemon = PokemonCatalog.Eevee;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(133));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Normal));
            });
        }

        #endregion

        #region Legendary/Mythical

        [Test]
        public void Test_Mewtwo_Data()
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
        public void Test_Mew_Data()
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

        #endregion

        #region Gender Ratio Tests

        [Test]
        public void Test_Starters_Have_87_5_Percent_Male_Ratio()
        {
            Assert.Multiple(() =>
            {
                // Grass line
                Assert.That(PokemonCatalog.Bulbasaur.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Ivysaur.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Venusaur.GenderRatio, Is.EqualTo(87.5f));
                
                // Fire line
                Assert.That(PokemonCatalog.Charmander.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Charmeleon.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Charizard.GenderRatio, Is.EqualTo(87.5f));
                
                // Water line
                Assert.That(PokemonCatalog.Squirtle.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Wartortle.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Blastoise.GenderRatio, Is.EqualTo(87.5f));
            });
        }

        [Test]
        public void Test_Starters_Have_Both_Genders()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Bulbasaur.HasBothGenders, Is.True);
                Assert.That(PokemonCatalog.Charmander.HasBothGenders, Is.True);
                Assert.That(PokemonCatalog.Squirtle.HasBothGenders, Is.True);
            });
        }

        [Test]
        public void Test_Eevee_And_Snorlax_Have_87_5_Percent_Male_Ratio()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Eevee.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(PokemonCatalog.Snorlax.GenderRatio, Is.EqualTo(87.5f));
            });
        }

        [Test]
        public void Test_Pikachu_And_Raichu_Have_Default_50_Percent_Ratio()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Pikachu.GenderRatio, Is.EqualTo(50f));
                Assert.That(PokemonCatalog.Raichu.GenderRatio, Is.EqualTo(50f));
            });
        }

        [Test]
        public void Test_Legendaries_Are_Genderless()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Mewtwo.IsGenderless, Is.True);
                Assert.That(PokemonCatalog.Mewtwo.GenderRatio, Is.EqualTo(-1f));
                
                Assert.That(PokemonCatalog.Mew.IsGenderless, Is.True);
                Assert.That(PokemonCatalog.Mew.GenderRatio, Is.EqualTo(-1f));
            });
        }

        #endregion
    }
}
