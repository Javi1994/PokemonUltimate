using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Alakazam in PokemonCatalog.
    /// Verifies correct data, stats, evolution line, abilities, and learnset.
    /// </summary>
    [TestFixture]
    public class AlakazamTests
    {
        [Test]
        public void Alakazam_HasExtremeSpAttack()
        {
            var pokemon = PokemonCatalog.Alakazam;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.BaseStats.SpAttack, Is.EqualTo(135)); // Extremely high SpAtk
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(120)); // Very fast
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(500));
                Assert.That(pokemon.BaseStats.Defense, Is.EqualTo(45)); // Very frail physically
            });
        }

        [Test]
        public void Alakazam_EvolutionLine_IsCorrect()
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
        public void Alakazam_Line_HasSynchronize()
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

