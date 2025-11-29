using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs.Pokemon
{
    /// <summary>
    /// Tests specific to Generation 1 Pokemon in PokemonCatalog.
    /// Verifies correct data for each Pokemon defined in PokemonCatalog.Gen1.cs.
    /// </summary>
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
        public void Test_Venusaur_Is_Stronger_Than_Bulbasaur()
        {
            Assert.That(PokemonCatalog.Venusaur.BaseStats.Total, 
                Is.GreaterThan(PokemonCatalog.Bulbasaur.BaseStats.Total));
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
            });
        }

        #endregion
    }
}

