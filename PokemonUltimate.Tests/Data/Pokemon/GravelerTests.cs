using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Graveler in PokemonCatalog.
    /// Verifies correct data, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class GravelerTests
    {
        [Test]
        public void Graveler_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Geodude.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Graveler));
                Assert.That(PokemonCatalog.Graveler.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Golem));
                Assert.That(PokemonCatalog.Graveler.Evolutions[0].HasCondition<PokemonUltimate.Core.Evolution.Conditions.TradeCondition>(), Is.True);
            });
        }

        [Test]
        public void Graveler_IsRockGroundType()
        {
            var pokemon = PokemonCatalog.Graveler;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Rock));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Ground));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Graveler_HasSturdy()
        {
            Assert.That(PokemonCatalog.Graveler.GetAllAbilities().Any(a => a.Name == "Sturdy"), Is.True);
        }
    }
}

