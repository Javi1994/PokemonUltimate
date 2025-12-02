using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Haunter in PokemonCatalog.
    /// Verifies correct data, evolution line, and abilities.
    /// </summary>
    [TestFixture]
    public class HaunterTests
    {
        [Test]
        public void Haunter_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Gastly.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Haunter));
                Assert.That(PokemonCatalog.Haunter.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Gengar));
                Assert.That(PokemonCatalog.Haunter.Evolutions[0].HasCondition<PokemonUltimate.Core.Evolution.Conditions.TradeCondition>(), Is.True);
            });
        }

        [Test]
        public void Haunter_IsGhostPoisonType()
        {
            var pokemon = PokemonCatalog.Haunter;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Ghost));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Haunter_HasLevitate()
        {
            Assert.That(PokemonCatalog.Haunter.Ability1.Name, Is.EqualTo("Levitate"));
        }
    }
}

