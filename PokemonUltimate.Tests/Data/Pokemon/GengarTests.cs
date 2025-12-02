using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Gengar in PokemonCatalog.
    /// Verifies correct data, types, stats, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class GengarTests
    {
        [Test]
        public void Gengar_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Gengar;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Gengar"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(94));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Ghost));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Gengar_IsStrongerThanGastly()
        {
            Assert.That(PokemonCatalog.Gengar.BaseStats.Total,
                Is.GreaterThan(PokemonCatalog.Gastly.BaseStats.Total));
            Assert.That(PokemonCatalog.Gengar.BaseStats.SpAttack, Is.EqualTo(130)); // Very high SpAtk
        }

        [Test]
        public void Gengar_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Gastly.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Gastly.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Haunter));
                Assert.That(PokemonCatalog.Gastly.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(25));

                Assert.That(PokemonCatalog.Haunter.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Haunter.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Gengar));
                Assert.That(PokemonCatalog.Haunter.Evolutions[0].HasCondition<PokemonUltimate.Core.Evolution.Conditions.TradeCondition>(), Is.True);

                Assert.That(PokemonCatalog.Gengar.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Gengar_Line_HasLevitate()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Gastly.Ability1.Name, Is.EqualTo("Levitate"));
                Assert.That(PokemonCatalog.Haunter.Ability1.Name, Is.EqualTo("Levitate"));
            });
        }
    }
}

