using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Kadabra in PokemonCatalog.
    /// Verifies correct data, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class KadabraTests
    {
        [Test]
        public void Kadabra_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Abra.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Kadabra));
                Assert.That(PokemonCatalog.Kadabra.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Alakazam));
                Assert.That(PokemonCatalog.Kadabra.Evolutions[0].HasCondition<PokemonUltimate.Core.Evolution.Conditions.TradeCondition>(), Is.True);
            });
        }

        [Test]
        public void Kadabra_IsPsychicType()
        {
            var pokemon = PokemonCatalog.Kadabra;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Psychic));
                Assert.That(pokemon.SecondaryType, Is.Null);
            });
        }

        [Test]
        public void Kadabra_HasSynchronize()
        {
            Assert.That(PokemonCatalog.Kadabra.GetAllAbilities().Any(a => a.Name == "Synchronize"), Is.True);
        }
    }
}

