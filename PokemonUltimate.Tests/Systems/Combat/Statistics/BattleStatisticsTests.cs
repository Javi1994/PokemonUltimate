using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat.Statistics;

namespace PokemonUltimate.Tests.Systems.Combat.Statistics
{
    /// <summary>
    /// Functional tests for BattleStatistics - statistics data model.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    [TestFixture]
    public class BattleStatisticsTests
    {
        private BattleStatistics _statistics;

        [SetUp]
        public void SetUp()
        {
            _statistics = new BattleStatistics();
        }

        [Test]
        public void Constructor_InitializesAllCollections_CollectionsAreEmpty()
        {
            // Assert
            Assert.That(_statistics.PlayerWins, Is.EqualTo(0));
            Assert.That(_statistics.EnemyWins, Is.EqualTo(0));
            Assert.That(_statistics.Draws, Is.EqualTo(0));
            Assert.That(_statistics.MoveUsageStats, Is.Not.Null);
            Assert.That(_statistics.MoveUsageStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.StatusEffectStats, Is.Not.Null);
            Assert.That(_statistics.StatusEffectStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.VolatileStatusStats, Is.Not.Null);
            Assert.That(_statistics.VolatileStatusStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.DamageStats, Is.Not.Null);
            Assert.That(_statistics.DamageStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.CriticalHits, Is.EqualTo(0));
            Assert.That(_statistics.Misses, Is.EqualTo(0));
            Assert.That(_statistics.WeatherChanges, Is.Not.Null);
            Assert.That(_statistics.WeatherChanges.Count, Is.EqualTo(0));
            Assert.That(_statistics.TerrainChanges, Is.Not.Null);
            Assert.That(_statistics.TerrainChanges.Count, Is.EqualTo(0));
            Assert.That(_statistics.SideConditionStats, Is.Not.Null);
            Assert.That(_statistics.SideConditionStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.HazardStats, Is.Not.Null);
            Assert.That(_statistics.HazardStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.StatChangeStats, Is.Not.Null);
            Assert.That(_statistics.StatChangeStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.HealingStats, Is.Not.Null);
            Assert.That(_statistics.HealingStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.ActionTypeStats, Is.Not.Null);
            Assert.That(_statistics.ActionTypeStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.EffectGenerationStats, Is.Not.Null);
            Assert.That(_statistics.EffectGenerationStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.TotalTurns, Is.EqualTo(0));
            Assert.That(_statistics.TurnDurations, Is.Not.Null);
            Assert.That(_statistics.TurnDurations.Count, Is.EqualTo(0));
            Assert.That(_statistics.AbilityActivationStats, Is.Not.Null);
            Assert.That(_statistics.AbilityActivationStats.Count, Is.EqualTo(0));
            Assert.That(_statistics.ItemActivationStats, Is.Not.Null);
            Assert.That(_statistics.ItemActivationStats.Count, Is.EqualTo(0));
        }

        [Test]
        public void MoveUsageStats_AddEntry_EntryAdded()
        {
            // Arrange
            var pokemonName = "Pikachu";
            var moveName = "Thunderbolt";

            // Act
            if (!_statistics.MoveUsageStats.ContainsKey(pokemonName))
                _statistics.MoveUsageStats[pokemonName] = new Dictionary<string, int>();
            
            _statistics.MoveUsageStats[pokemonName][moveName] = 1;

            // Assert
            Assert.That(_statistics.MoveUsageStats.ContainsKey(pokemonName), Is.True);
            Assert.That(_statistics.MoveUsageStats[pokemonName].ContainsKey(moveName), Is.True);
            Assert.That(_statistics.MoveUsageStats[pokemonName][moveName], Is.EqualTo(1));
        }

        [Test]
        public void DamageStats_AddDamage_DamageAdded()
        {
            // Arrange
            var pokemonName = "Pikachu";
            var damage = 50;

            // Act
            if (!_statistics.DamageStats.ContainsKey(pokemonName))
                _statistics.DamageStats[pokemonName] = new List<int>();
            
            _statistics.DamageStats[pokemonName].Add(damage);

            // Assert
            Assert.That(_statistics.DamageStats.ContainsKey(pokemonName), Is.True);
            Assert.That(_statistics.DamageStats[pokemonName].Count, Is.EqualTo(1));
            Assert.That(_statistics.DamageStats[pokemonName][0], Is.EqualTo(damage));
        }

        [Test]
        public void CriticalHits_Increment_Incremented()
        {
            // Act
            _statistics.CriticalHits++;

            // Assert
            Assert.That(_statistics.CriticalHits, Is.EqualTo(1));
        }

        [Test]
        public void Misses_Increment_Incremented()
        {
            // Act
            _statistics.Misses++;

            // Assert
            Assert.That(_statistics.Misses, Is.EqualTo(1));
        }
    }
}

