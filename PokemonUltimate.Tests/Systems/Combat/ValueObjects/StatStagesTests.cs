using System;
using NUnit.Framework;
using PokemonUltimate.Combat.ValueObjects;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Combat.ValueObjects
{
    /// <summary>
    /// Tests for StatStages - Value Object representing stat stages for a Pokemon in battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    [TestFixture]
    public class StatStagesTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_Default_AllStatsAtZero()
        {
            // Act
            var stages = new StatStages();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(stages.GetStage(Stat.Attack), Is.EqualTo(0));
                Assert.That(stages.GetStage(Stat.Defense), Is.EqualTo(0));
                Assert.That(stages.GetStage(Stat.SpAttack), Is.EqualTo(0));
                Assert.That(stages.GetStage(Stat.SpDefense), Is.EqualTo(0));
                Assert.That(stages.GetStage(Stat.Speed), Is.EqualTo(0));
                Assert.That(stages.GetStage(Stat.Accuracy), Is.EqualTo(0));
                Assert.That(stages.GetStage(Stat.Evasion), Is.EqualTo(0));
            });
        }

        #endregion

        #region GetStage Tests

        [Test]
        public void GetStage_HP_ReturnsZero()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            var result = stages.GetStage(Stat.HP);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetStage_UnknownStat_ReturnsZero()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            // Using a stat that doesn't exist in the dictionary
            var result = stages.GetStage((Stat)999);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion

        #region ModifyStage Tests

        [Test]
        public void ModifyStage_PositiveChange_IncreasesStage()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            var newStages = stages.ModifyStage(Stat.Attack, 2, out var actualChange);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Attack), Is.EqualTo(2));
                Assert.That(actualChange, Is.EqualTo(2));
            });
        }

        [Test]
        public void ModifyStage_NegativeChange_DecreasesStage()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            var newStages = stages.ModifyStage(Stat.Defense, -1, out var actualChange);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Defense), Is.EqualTo(-1));
                Assert.That(actualChange, Is.EqualTo(-1));
            });
        }

        [Test]
        public void ModifyStage_ExceedsMax_ClampsToMax()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            var newStages = stages.ModifyStage(Stat.Attack, 10, out var actualChange);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Attack), Is.EqualTo(6));
                Assert.That(actualChange, Is.EqualTo(6));
            });
        }

        [Test]
        public void ModifyStage_BelowMin_ClampsToMin()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            var newStages = stages.ModifyStage(Stat.Defense, -10, out var actualChange);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Defense), Is.EqualTo(-6));
                Assert.That(actualChange, Is.EqualTo(-6));
            });
        }

        [Test]
        public void ModifyStage_AlreadyAtMax_NoChange()
        {
            // Arrange
            var stages = new StatStages();
            stages = stages.ModifyStage(Stat.Attack, 6, out _);

            // Act
            var newStages = stages.ModifyStage(Stat.Attack, 1, out var actualChange);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Attack), Is.EqualTo(6));
                Assert.That(actualChange, Is.EqualTo(0));
            });
        }

        [Test]
        public void ModifyStage_AlreadyAtMin_NoChange()
        {
            // Arrange
            var stages = new StatStages();
            stages = stages.ModifyStage(Stat.Defense, -6, out _);

            // Act
            var newStages = stages.ModifyStage(Stat.Defense, -1, out var actualChange);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Defense), Is.EqualTo(-6));
                Assert.That(actualChange, Is.EqualTo(0));
            });
        }

        [Test]
        public void ModifyStage_HP_ThrowsArgumentException()
        {
            // Arrange
            var stages = new StatStages();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                stages.ModifyStage(Stat.HP, 1, out _));
        }

        [Test]
        public void ModifyStage_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new StatStages();

            // Act
            var modified = original.ModifyStage(Stat.Attack, 3, out _);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.GetStage(Stat.Attack), Is.EqualTo(0));
                Assert.That(modified.GetStage(Stat.Attack), Is.EqualTo(3));
            });
        }

        [Test]
        public void ModifyStage_MultipleStats_OnlyModifiesTarget()
        {
            // Arrange
            var stages = new StatStages();

            // Act
            var newStages = stages.ModifyStage(Stat.Attack, 2, out _);
            newStages = newStages.ModifyStage(Stat.Speed, -1, out _);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newStages.GetStage(Stat.Attack), Is.EqualTo(2));
                Assert.That(newStages.GetStage(Stat.Speed), Is.EqualTo(-1));
                Assert.That(newStages.GetStage(Stat.Defense), Is.EqualTo(0));
            });
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_WithModifiedStages_ResetsAllToZero()
        {
            // Arrange
            var stages = new StatStages();
            stages = stages.ModifyStage(Stat.Attack, 3, out _);
            stages = stages.ModifyStage(Stat.Defense, -2, out _);
            stages = stages.ModifyStage(Stat.Speed, 1, out _);

            // Act
            var reset = stages.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(reset.GetStage(Stat.Attack), Is.EqualTo(0));
                Assert.That(reset.GetStage(Stat.Defense), Is.EqualTo(0));
                Assert.That(reset.GetStage(Stat.Speed), Is.EqualTo(0));
            });
        }

        [Test]
        public void Reset_Immutable_OriginalUnchanged()
        {
            // Arrange
            var original = new StatStages();
            original = original.ModifyStage(Stat.Attack, 3, out _);

            // Act
            var reset = original.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.GetStage(Stat.Attack), Is.EqualTo(3));
                Assert.That(reset.GetStage(Stat.Attack), Is.EqualTo(0));
            });
        }

        #endregion

        #region Copy Tests

        [Test]
        public void Copy_CreatesIndependentInstance()
        {
            // Arrange
            var original = new StatStages();
            original = original.ModifyStage(Stat.Attack, 2, out _);

            // Act
            var copy = original.Copy();
            var modifiedCopy = copy.ModifyStage(Stat.Attack, 1, out _);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.GetStage(Stat.Attack), Is.EqualTo(2));
                Assert.That(copy.GetStage(Stat.Attack), Is.EqualTo(2));
                Assert.That(modifiedCopy.GetStage(Stat.Attack), Is.EqualTo(3));
            });
        }

        #endregion
    }
}
