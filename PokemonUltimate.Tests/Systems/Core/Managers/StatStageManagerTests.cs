using System;
using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Managers;

namespace PokemonUltimate.Tests.Systems.Core.Managers
{
    /// <summary>
    /// Tests for StatStageManager - stat stage management in battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class StatStageManagerTests
    {
        private IStatStageManager _manager;

        [SetUp]
        public void SetUp()
        {
            _manager = StatStageManager.Default;
        }

        #region CreateInitialStages Tests

        [Test]
        public void CreateInitialStages_ReturnsDictionaryWithAllStatsAtZero()
        {
            // Act
            var stages = _manager.CreateInitialStages();

            // Assert
            Assert.That(stages, Is.Not.Null);
            Assert.That(stages[Stat.Attack], Is.EqualTo(0));
            Assert.That(stages[Stat.Defense], Is.EqualTo(0));
            Assert.That(stages[Stat.SpAttack], Is.EqualTo(0));
            Assert.That(stages[Stat.SpDefense], Is.EqualTo(0));
            Assert.That(stages[Stat.Speed], Is.EqualTo(0));
            Assert.That(stages[Stat.Accuracy], Is.EqualTo(0));
            Assert.That(stages[Stat.Evasion], Is.EqualTo(0));
        }

        #endregion

        #region ModifyStage Tests

        [Test]
        public void ModifyStage_PositiveChange_IncreasesStage()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();

            // Act
            var actualChange = _manager.ModifyStage(stages, Stat.Attack, 2);

            // Assert
            Assert.That(actualChange, Is.EqualTo(2));
            Assert.That(stages[Stat.Attack], Is.EqualTo(2));
        }

        [Test]
        public void ModifyStage_NegativeChange_DecreasesStage()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();
            stages[Stat.Attack] = 3;

            // Act
            var actualChange = _manager.ModifyStage(stages, Stat.Attack, -2);

            // Assert
            Assert.That(actualChange, Is.EqualTo(-2));
            Assert.That(stages[Stat.Attack], Is.EqualTo(1));
        }

        [Test]
        public void ModifyStage_ExceedsMaxStage_ClampsToMax()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();
            stages[Stat.Attack] = CoreConstants.MaxStatStage - 1; // 5

            // Act
            var actualChange = _manager.ModifyStage(stages, Stat.Attack, 5);

            // Assert
            // From 5 to 6 (max), so change is 1
            Assert.That(actualChange, Is.EqualTo(1));
            Assert.That(stages[Stat.Attack], Is.EqualTo(CoreConstants.MaxStatStage));
        }

        [Test]
        public void ModifyStage_BelowMinStage_ClampsToMin()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();
            stages[Stat.Attack] = CoreConstants.MinStatStage + 1; // -5

            // Act
            var actualChange = _manager.ModifyStage(stages, Stat.Attack, -5);

            // Assert
            // From -5 to -6 (min), so change is -1
            Assert.That(actualChange, Is.EqualTo(-1));
            Assert.That(stages[Stat.Attack], Is.EqualTo(CoreConstants.MinStatStage));
        }

        [Test]
        public void ModifyStage_HPStat_ThrowsArgumentException()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _manager.ModifyStage(stages, Stat.HP, 1));
        }

        [Test]
        public void ModifyStage_NullStages_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _manager.ModifyStage(null, Stat.Attack, 1));
        }

        [Test]
        public void ModifyStage_StatNotInDictionary_ReturnsZero()
        {
            // Arrange
            var stages = new Dictionary<Stat, int>();

            // Act
            var actualChange = _manager.ModifyStage(stages, Stat.Attack, 1);

            // Assert
            Assert.That(actualChange, Is.EqualTo(0));
        }

        #endregion

        #region ResetStages Tests

        [Test]
        public void ResetStages_SetsAllStatsToZero()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();
            stages[Stat.Attack] = 3;
            stages[Stat.Defense] = -2;
            stages[Stat.Speed] = 6;

            // Act
            _manager.ResetStages(stages);

            // Assert
            Assert.That(stages[Stat.Attack], Is.EqualTo(0));
            Assert.That(stages[Stat.Defense], Is.EqualTo(0));
            Assert.That(stages[Stat.SpAttack], Is.EqualTo(0));
            Assert.That(stages[Stat.SpDefense], Is.EqualTo(0));
            Assert.That(stages[Stat.Speed], Is.EqualTo(0));
            Assert.That(stages[Stat.Accuracy], Is.EqualTo(0));
            Assert.That(stages[Stat.Evasion], Is.EqualTo(0));
        }

        [Test]
        public void ResetStages_NullStages_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _manager.ResetStages(null));
        }

        #endregion

        #region GetStage Tests

        [Test]
        public void GetStage_ExistingStat_ReturnsStage()
        {
            // Arrange
            var stages = _manager.CreateInitialStages();
            stages[Stat.Attack] = 3;

            // Act
            var stage = _manager.GetStage(stages, Stat.Attack);

            // Assert
            Assert.That(stage, Is.EqualTo(3));
        }

        [Test]
        public void GetStage_NonExistentStat_ReturnsZero()
        {
            // Arrange
            var stages = new Dictionary<Stat, int>();

            // Act
            var stage = _manager.GetStage(stages, Stat.Attack);

            // Assert
            Assert.That(stage, Is.EqualTo(0));
        }

        [Test]
        public void GetStage_NullStages_ReturnsZero()
        {
            // Act
            var stage = _manager.GetStage(null, Stat.Attack);

            // Assert
            Assert.That(stage, Is.EqualTo(0));
        }

        #endregion

        #region IsValidStage Tests

        [Test]
        public void IsValidStage_ValidStage_ReturnsTrue()
        {
            // Act & Assert
            Assert.That(_manager.IsValidStage(0), Is.True);
            Assert.That(_manager.IsValidStage(3), Is.True);
            Assert.That(_manager.IsValidStage(-3), Is.True);
            Assert.That(_manager.IsValidStage(CoreConstants.MaxStatStage), Is.True);
            Assert.That(_manager.IsValidStage(CoreConstants.MinStatStage), Is.True);
        }

        [Test]
        public void IsValidStage_InvalidStage_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(_manager.IsValidStage(7), Is.False);
            Assert.That(_manager.IsValidStage(-7), Is.False);
            Assert.That(_manager.IsValidStage(10), Is.False);
            Assert.That(_manager.IsValidStage(-10), Is.False);
        }

        #endregion
    }
}
