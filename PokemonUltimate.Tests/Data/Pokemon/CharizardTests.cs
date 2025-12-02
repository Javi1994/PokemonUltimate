using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;

namespace PokemonUltimate.Tests.Data.Pokemon
{
    /// <summary>
    /// Tests for Charizard in PokemonCatalog.
    /// Verifies correct data, types, stats, and evolution line.
    /// </summary>
    [TestFixture]
    public class CharizardTests
    {
        [Test]
        public void Charizard_IsFireFlyingType()
        {
            var pokemon = PokemonCatalog.Charizard;

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(pokemon.HasType(PokemonType.Fire), Is.True);
                Assert.That(pokemon.HasType(PokemonType.Flying), Is.True);
                Assert.That(pokemon.HasType(PokemonType.Dragon), Is.False);
                Assert.That(pokemon.BaseStats.Total, Is.EqualTo(534));
            });
        }

        [Test]
        public void Charizard_CannotEvolve()
        {
            Assert.That(PokemonCatalog.Charizard.CanEvolve, Is.False);
        }

        [Test]
        public void Charizard_EvolutionLine_IsCorrect()
        {
            Assert.Multiple(() =>
            {
                // Charmander → Charmeleon at level 16
                Assert.That(PokemonCatalog.Charmander.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charmeleon));
                Assert.That(PokemonCatalog.Charmander.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(16));

                // Charmeleon → Charizard at level 36
                Assert.That(PokemonCatalog.Charmeleon.CanEvolve, Is.True);
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].Target, Is.EqualTo(PokemonCatalog.Charizard));
                Assert.That(PokemonCatalog.Charmeleon.Evolutions[0].GetCondition<PokemonUltimate.Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(36));
            });
        }

        [Test]
        public void Charizard_HasStarterGenderRatio()
        {
            Assert.That(PokemonCatalog.Charizard.GenderRatio, Is.EqualTo(87.5f));
        }
    }
}

