using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories.MoveSelection;
using PokemonUltimate.Core.Providers;

namespace PokemonUltimate.Tests.Systems.Core.Factories.MoveSelection
{
    /// <summary>
    /// Tests for MoveSelector - move selection strategies.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    [TestFixture]
    public class MoveSelectorTests
    {
        private List<LearnableMove> _availableMoves;

        [SetUp]
        public void SetUp()
        {
            _availableMoves = new List<LearnableMove>
            {
                new LearnableMove(new MoveData { Name = "Tackle", Type = PokemonType.Normal, Power = 40 }, LearnMethod.LevelUp, 1),
                new LearnableMove(new MoveData { Name = "Thunderbolt", Type = PokemonType.Electric, Power = 90 }, LearnMethod.LevelUp, 20),
                new LearnableMove(new MoveData { Name = "Quick Attack", Type = PokemonType.Normal, Power = 40 }, LearnMethod.LevelUp, 10),
                new LearnableMove(new MoveData { Name = "Thunder", Type = PokemonType.Electric, Power = 110 }, LearnMethod.LevelUp, 30),
                new LearnableMove(new MoveData { Name = "Spark", Type = PokemonType.Electric, Power = 65 }, LearnMethod.LevelUp, 15)
            };
        }

        #region Default Strategy Tests

        [Test]
        public void CreateDefault_SelectsMovesByHighestLevel()
        {
            // Arrange
            var selector = MoveSelector.CreateDefault();

            // Act
            var selected = selector.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(3));
            Assert.That(selected[0].Move.Name, Is.EqualTo("Thunder")); // Level 30
            Assert.That(selected[1].Move.Name, Is.EqualTo("Thunderbolt")); // Level 20
            Assert.That(selected[2].Move.Name, Is.EqualTo("Spark")); // Level 15
        }

        #endregion

        #region Random Strategy Tests

        [Test]
        public void CreateRandom_SelectsRandomMoves()
        {
            // Arrange
            var randomProvider = new RandomProvider(42);
            var selector = MoveSelector.CreateRandom(randomProvider);

            // Act
            var selected = selector.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(3));
            Assert.That(selected.All(m => _availableMoves.Contains(m)), Is.True);
        }

        [Test]
        public void CreateRandom_SameSeed_ProducesSameSelection()
        {
            // Arrange
            var selector1 = MoveSelector.CreateRandom(new RandomProvider(42));
            var selector2 = MoveSelector.CreateRandom(new RandomProvider(42));

            // Act
            var selected1 = selector1.SelectMoves(_availableMoves, 3).ToList();
            var selected2 = selector2.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected1.Select(m => m.Move.Name).ToList(),
                Is.EqualTo(selected2.Select(m => m.Move.Name).ToList()));
        }

        #endregion

        #region STAB Strategy Tests

        [Test]
        public void CreateStab_PrioritizesStabMoves()
        {
            // Arrange
            var selector = MoveSelector.CreateStab(PokemonType.Electric);

            // Act
            var selected = selector.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(3));
            // All selected moves should be Electric type (STAB)
            Assert.That(selected.All(m => m.Move.Type == PokemonType.Electric), Is.True);
        }

        [Test]
        public void CreateStab_WithSecondaryType_ConsidersBothTypes()
        {
            // Arrange
            var selector = MoveSelector.CreateStab(PokemonType.Electric, PokemonType.Normal);

            // Act
            var selected = selector.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(3));
            // Selected moves should be Electric or Normal type (STAB)
            Assert.That(selected.All(m => m.Move.Type == PokemonType.Electric || m.Move.Type == PokemonType.Normal), Is.True);
        }

        #endregion

        #region Power Strategy Tests

        [Test]
        public void CreatePower_PrioritizesHighestPowerMoves()
        {
            // Arrange
            var selector = MoveSelector.CreatePower();

            // Act
            var selected = selector.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(3));
            Assert.That(selected[0].Move.Name, Is.EqualTo("Thunder")); // Power 110
            Assert.That(selected[1].Move.Name, Is.EqualTo("Thunderbolt")); // Power 90
            Assert.That(selected[2].Move.Name, Is.EqualTo("Spark")); // Power 65
        }

        #endregion

        #region Optimal Strategy Tests

        [Test]
        public void CreateOptimal_SelectsOptimalMoves()
        {
            // Arrange
            var randomProvider = new RandomProvider(42);
            var selector = MoveSelector.CreateOptimal(PokemonType.Electric, null, randomProvider);

            // Act
            var selected = selector.SelectMoves(_availableMoves, 3).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(3));
            // Should prioritize STAB moves (Electric) and high power
            Assert.That(selected[0].Move.Type, Is.EqualTo(PokemonType.Electric));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void SelectMoves_EmptyList_ReturnsEmpty()
        {
            // Arrange
            var selector = MoveSelector.CreateDefault();
            var emptyList = new List<LearnableMove>();

            // Act
            var selected = selector.SelectMoves(emptyList, 4).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(0));
        }

        [Test]
        public void SelectMoves_CountGreaterThanAvailable_ReturnsMax4Moves()
        {
            // Arrange
            var selector = MoveSelector.CreateDefault();
            // _availableMoves has 5 moves, but max is 4

            // Act
            var selected = selector.SelectMoves(_availableMoves, 10).ToList();

            // Assert
            // Should return maximum 4 moves (Pokemon limit), not all 5 available
            Assert.That(selected.Count, Is.EqualTo(4));
            Assert.That(selected.Count, Is.LessThanOrEqualTo(_availableMoves.Count));
        }

        [Test]
        public void SelectMoves_CountZero_ReturnsEmpty()
        {
            // Arrange
            var selector = MoveSelector.CreateDefault();

            // Act
            var selected = selector.SelectMoves(_availableMoves, 0).ToList();

            // Assert
            Assert.That(selected.Count, Is.EqualTo(0));
        }

        [Test]
        public void SelectMoves_CountNegative_ReturnsEmpty()
        {
            // Arrange
            var selector = MoveSelector.CreateDefault();

            // Act
            var selected = selector.SelectMoves(_availableMoves, -1).ToList();

            // Assert
            // Negative count should return empty list
            Assert.That(selected.Count, Is.EqualTo(0));
        }

        #endregion
    }
}
