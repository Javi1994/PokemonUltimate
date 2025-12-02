using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Golem in PokemonCatalog.
    /// Verifies correct data, types, stats, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class GolemTests
    {
        [Test]
        public void Golem_HasHighDefense()
        {
            var pokemon = PokemonCatalog.Golem;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.BaseStats.Defense, Is.EqualTo(130)); // Very high Defense
                Assert.That(pokemon.BaseStats.Attack, Is.EqualTo(120)); // High Attack
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(495));
            });
        }

        [Test]
        public void Golem_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Geodude.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Geodude.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Graveler));
                Assert.That(PokemonCatalog.Geodude.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(25));

                Assert.That(PokemonCatalog.Graveler.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Graveler.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Golem));
                Assert.That(PokemonCatalog.Graveler.Evolutions[0].HasCondition<PokemonUltimate.Core.Evolution.Conditions.TradeCondition>(), Is.True);

                Assert.That(PokemonCatalog.Golem.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Golem_IsRockGroundType()
        {
            var pokemon = PokemonCatalog.Golem;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Rock));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Ground));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Geodude_Line_HasSturdy()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Geodude.GetAllAbilities().Any(a => a.Name == "Sturdy"), Is.True);
                Assert.That(PokemonCatalog.Graveler.GetAllAbilities().Any(a => a.Name == "Sturdy"), Is.True);
                Assert.That(PokemonCatalog.Golem.GetAllAbilities().Any(a => a.Name == "Sturdy"), Is.True);
            });
        }
    }
}

