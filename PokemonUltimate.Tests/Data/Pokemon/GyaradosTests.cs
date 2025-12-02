using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Gyarados in PokemonCatalog.
    /// Verifies correct data, types, stats, evolution from Magikarp, and abilities.
    /// </summary>
    [TestFixture]
    public class GyaradosTests
    {
        [Test]
        public void Gyarados_IsWaterFlyingType()
        {
            var pokemon = PokemonCatalog.Gyarados;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Water));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(pokemon.HasType(PokemonType.Water), Is.True);
                Assert.That(pokemon.HasType(PokemonType.Flying), Is.True);
            });
        }

        [Test]
        public void Gyarados_IsMuchStrongerThanMagikarp()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Gyarados.BaseStats.Total, Is.EqualTo(540));
                Assert.That(PokemonCatalog.Gyarados.BaseStats.Total,
                    Is.GreaterThan(PokemonCatalog.Magikarp.BaseStats.Total * 2));
                Assert.That(PokemonCatalog.Gyarados.BaseStats.Attack, Is.EqualTo(125)); // Very high Attack
            });
        }

        [Test]
        public void Gyarados_EvolutionFromMagikarp_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                Assert.That(PokemonCatalog.Magikarp.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Magikarp.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Gyarados));
                Assert.That(PokemonCatalog.Magikarp.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(20));
                Assert.That(PokemonCatalog.Gyarados.CanEvolve, Is.False);
            });
        }

        [Test]
        public void Gyarados_HasIntimidate()
        {
            Assert.That(PokemonCatalog.Gyarados.Ability1.Name, Is.EqualTo("Intimidate"));
        }
    }
}

