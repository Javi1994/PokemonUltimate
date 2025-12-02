using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Abra in PokemonCatalog.
    /// Verifies correct data, stats, evolution line, abilities, and learnset.
    /// </summary>
    [TestFixture]
    public class AbraTests
    {
        [Test]
        public void Abra_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Abra;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Abra"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(63));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Psychic));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(310));
                Assert.That(pokemon.BaseStats.SpAttack, Is.EqualTo(105)); // High SpAtk even at base
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(25)); // Very frail
            });
        }

        [Test]
        public void Abra_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Abra.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Abra.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Kadabra));
                Assert.That(PokemonCatalog.Abra.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(16));

                Assert.That(PokemonCatalog.Kadabra.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Kadabra.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Alakazam));
                Assert.That(PokemonCatalog.Kadabra.Evolutions[0].HasCondition<PokemonUltimate.Core.Evolution.Conditions.TradeCondition>(), Is.True);

                Assert.That(PokemonCatalog.Alakazam.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Abra_Line_HasSynchronize()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Abra.GetAllAbilities().Any(a => a.Name == "Synchronize"), Is.True);
                Assert.That(PokemonCatalog.Kadabra.GetAllAbilities().Any(a => a.Name == "Synchronize"), Is.True);
                Assert.That(PokemonCatalog.Alakazam.GetAllAbilities().Any(a => a.Name == "Synchronize"), Is.True);
            });
        }

        [Test]
        public void Abra_Learnset_IncludesTeleport()
        {
            var pokemon = PokemonCatalog.Abra;
            Assert.That(pokemon.CanLearn(MoveCatalog.Teleport), Is.True);
        }
    }
}

