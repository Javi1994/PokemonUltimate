using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Bulbasaur in PokemonCatalog.
    /// Verifies correct data, types, stats, evolution, and learnset.
    /// </summary>
    [TestFixture]
    public class BulbasaurTests
    {
        [Test]
        public void Bulbasaur_Data_IsCorrect()
        {
            var pokemon = PokemonCatalog.Bulbasaur;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Bulbasaur"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(1));
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Grass));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Poison));
                Assert.That(pokemon.IsDualType, Is.True);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(318));
            });
        }

        [Test]
        public void Bulbasaur_EvolvesToIvysaur()
        {
            var pokemon = PokemonCatalog.Bulbasaur;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.CanEvolve, Is.True);
                Assert.That(pokemon.Evolutions, Has.Count.EqualTo(1));
                Assert.That(pokemon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Ivysaur));
                Assert.That(pokemon.Evolutions[0].GetCondition<LevelCondition>().MinLevel, Is.EqualTo(16));
            });
        }

        [Test]
        public void Bulbasaur_IsWeakerThanVenusaur()
        {
            Assert.That(PokemonCatalog.Venusaur.BaseStats.Total,
                Is.GreaterThan(PokemonCatalog.Bulbasaur.BaseStats.Total));
        }

        [Test]
        public void Bulbasaur_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Bulbasaur.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

