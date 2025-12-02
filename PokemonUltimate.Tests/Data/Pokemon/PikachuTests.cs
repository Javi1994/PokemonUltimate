using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Pikachu in PokemonCatalog.
    /// Verifies correct data, learnset, evolution, and abilities.
    /// </summary>
    [TestFixture]
    public class PikachuTests
    {
        [Test]
        public void Pikachu_Data_IsCorrect()
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
        public void Pikachu_Learnset_IncludesElectricMoves()
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
        public void Pikachu_Evolves_WithThunderStone()
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
        public void Pikachu_IsWeakerThanRaichu()
        {
            Assert.That(PokemonCatalog.Raichu.BaseStats.Total,
                Is.GreaterThan(PokemonCatalog.Pikachu.BaseStats.Total));
        }

        [Test]
        public void Pikachu_HasDefaultGenderRatio()
        {
            Assert.That(PokemonCatalog.Pikachu.GenderRatio, Is.EqualTo(50f));
        }
    }
}

