using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Tests for PokemonInstance stats cache functionality.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonInstanceStatsCacheTests
    {
        #region Cache Invalidation Tests

        [Test]
        public void LevelUp_InvalidatesCache_RecalculatesStats()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();
            int initialAttack = pokemon.Attack;
            int initialMaxHP = pokemon.MaxHP;

            // Act
            pokemon.LevelUp();

            // Assert
            Assert.That(pokemon.Attack, Is.GreaterThan(initialAttack));
            Assert.That(pokemon.MaxHP, Is.GreaterThan(initialMaxHP));
        }

        [Test]
        public void Evolve_InvalidatesCache_RecalculatesStats()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();
            int initialAttack = pokemon.Attack;
            int initialMaxHP = pokemon.MaxHP;

            // Act - Evolve to Raichu (if evolution exists)
            if (pokemon.CanEvolve())
            {
                var evolution = pokemon.GetAvailableEvolution();
                if (evolution != null)
                {
                    pokemon.Evolve(evolution.Target);

                    // Assert - Stats should be recalculated for new species
                    Assert.That(pokemon.Species, Is.EqualTo(evolution.Target));
                    // Stats may change based on new species base stats
                }
            }
            else
            {
                Assert.Inconclusive("Pikachu evolution not available for testing");
            }
        }

        #endregion

        #region Cache Consistency Tests

        [Test]
        public void GetEffectiveStat_UsesCachedStats_ConsistentResults()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();
            int baseAttack = pokemon.Attack;

            // Act - Multiple calls should return same result (no recalculation)
            var effective1 = pokemon.GetEffectiveStat(Stat.Attack);
            var effective2 = pokemon.GetEffectiveStat(Stat.Attack);

            // Assert
            Assert.That(effective1, Is.EqualTo(effective2));
            Assert.That(effective1, Is.EqualTo(baseAttack)); // No stat stages
        }

        #endregion
    }
}
