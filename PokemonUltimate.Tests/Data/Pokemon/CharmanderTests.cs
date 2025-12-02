using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Charmander in PokemonCatalog.
    /// Verifies correct data, learnset, evolution line, and type.
    /// </summary>
    [TestFixture]
    public class CharmanderTests
    {
        [Test]
        public void Charmander_Data_IsCorrect()
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
        public void Charmander_Learnset_IsCorrect()
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
        public void Charmander_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                // Charmander → Charmeleon at level 16
                Assert.That(PokemonCatalog.Charmander.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charmeleon));
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(16));

                // Charmeleon → Charizard at level 36
                Assert.That(PokemonCatalog.Charmeleon.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charizard));
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(36));
            });
        }

        [Test]
        public void Charmander_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Charmander.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

