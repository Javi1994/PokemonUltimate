using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Gastly in PokemonCatalog.
    /// Verifies correct data, types, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class GastlyTests
    {
        [Test]
        public void Gastly_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Gastly;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Gastly"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(92));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Ghost));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(310));
            });
        }

        [Test]
        public void Gastly_EvolutionLine_IsCorrect()
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
        public void Gastly_Line_HasLevitate()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Gastly.Ability1.Name, Is.EqualTo("Levitate"));
                Assert.That(PokemonCatalog.Haunter.Ability1.Name, Is.EqualTo("Levitate"));
            });
        }
    }
}

