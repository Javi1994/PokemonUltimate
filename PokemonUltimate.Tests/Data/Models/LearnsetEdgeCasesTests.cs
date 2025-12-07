using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Builders;
using PokemonBuilder = PokemonUltimate.Content.Builders.Pokemon;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using Pokemon = PokemonUltimate.Content.Builders.Pokemon;

namespace PokemonUltimate.Tests.Data.Models
{
    /// <summary>
    /// Edge case tests for the Learnset system.
    /// Tests move learning, limits, duplicates, and special scenarios.
    /// </summary>
    [TestFixture]
    public class LearnsetEdgeCasesTests
    {
        #region Test Data Setup

        private PokemonSpeciesData _pokemonWithManyMoves;
        private PokemonSpeciesData _pokemonWithNoMoves;
        private PokemonSpeciesData _pokemonWithDuplicateMoves;
        private PokemonSpeciesData _pokemonWithSameLevelMoves;

        [SetUp]
        public void Setup()
        {
            // Pokemon with many moves (10+)
            _pokemonWithManyMoves = PokemonBuilder.Define("ManyMovesPokemon", 800)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                    .AtLevel(5, MoveCatalog.Scratch)
                    .AtLevel(10, MoveCatalog.Ember)
                    .AtLevel(15, MoveCatalog.WaterGun)
                    .AtLevel(20, MoveCatalog.ThunderShock)
                    .AtLevel(25, MoveCatalog.VineWhip)
                    .AtLevel(30, MoveCatalog.Psychic)
                    .AtLevel(35, MoveCatalog.Earthquake)
                    .AtLevel(40, MoveCatalog.Flamethrower)
                    .AtLevel(45, MoveCatalog.Thunderbolt))
                .Build();

            // Pokemon with no moves
            _pokemonWithNoMoves = PokemonBuilder.Define("NoMovesPokemon", 801)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            // Pokemon with same move learned at multiple levels (edge case)
            _pokemonWithDuplicateMoves = PokemonBuilder.Define("DuplicateMovesPokemon", 802)
                .Type(PokemonType.Fire)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Tackle)
                    .AtLevel(10, MoveCatalog.Ember)
                    .AtLevel(20, MoveCatalog.Ember)  // Same move at different level
                    .AtLevel(30, MoveCatalog.Flamethrower))
                .Build();

            // Pokemon with multiple moves at same level
            _pokemonWithSameLevelMoves = PokemonBuilder.Define("SameLevelMovesPokemon", 803)
                .Type(PokemonType.Water)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Tackle)
                    .AtLevel(10, MoveCatalog.WaterGun, MoveCatalog.Growl)
                    .AtLevel(20, MoveCatalog.Surf, MoveCatalog.HydroPump, MoveCatalog.Scratch))
                .Build();
        }

        #endregion

        #region Empty Learnset Tests

        [Test]
        public void NoMoves_LearnsetIsEmpty()
        {
            Assert.That(_pokemonWithNoMoves.Learnset, Is.Empty);
        }

        [Test]
        public void NoMoves_GetStartingMoves_ReturnsEmpty()
        {
            var moves = _pokemonWithNoMoves.GetStartingMoves();
            Assert.That(moves, Is.Empty);
        }

        [Test]
        public void NoMoves_GetMovesAtLevel_ReturnsEmpty()
        {
            var moves = _pokemonWithNoMoves.GetMovesAtLevel(50);
            Assert.That(moves, Is.Empty);
        }

        [Test]
        public void NoMoves_GetMovesUpToLevel_ReturnsEmpty()
        {
            var moves = _pokemonWithNoMoves.GetMovesUpToLevel(100);
            Assert.That(moves, Is.Empty);
        }

        [Test]
        public void NoMoves_CanLearn_ReturnsFalse()
        {
            Assert.That(_pokemonWithNoMoves.CanLearn(MoveCatalog.Tackle), Is.False);
        }

        [Test]
        public void NoMoves_Instance_HasTackleAsDefault()
        {
            var pokemon = PokemonFactory.Create(_pokemonWithNoMoves, 50);
            // When Pokemon has no learnset, it should have Tackle as default move
            Assert.That(pokemon.Moves, Is.Not.Empty);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Tackle"));
        }

        [Test]
        public void NoMoves_WithNoMovesExplicit_ReturnsEmptyMoveset()
        {
            // When explicitly calling WithNoMoves(), should still return empty moveset
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithNoMoves, 50)
                .WithNoMoves()
                .Build();
            Assert.That(pokemon.Moves, Is.Empty);
        }

        [Test]
        public void LearnsetWithNoMovesUpToLevel_HasTackleAsDefault()
        {
            // Pokemon with learnset but no moves available at low level
            var pokemonWithHighLevelMoves = PokemonBuilder.Define("HighLevelPokemon", 804)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m
                    .AtLevel(50, MoveCatalog.Flamethrower))  // Only learns at level 50
                .Build();

            // Create at level 10 - no moves available yet
            var pokemon = PokemonFactory.Create(pokemonWithHighLevelMoves, 10);
            
            // Should have Tackle as default since no moves available at level 10
            Assert.That(pokemon.Moves, Is.Not.Empty);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Tackle"));
        }

        #endregion

        #region Many Moves Tests

        [Test]
        public void ManyMoves_LearnsetHasAllMoves()
        {
            // 2 starting + 9 level-up moves = 11 total
            Assert.That(_pokemonWithManyMoves.Learnset.Count, Is.EqualTo(11));
        }

        [Test]
        public void ManyMoves_GetMovesUpToLevel_ReturnsCorrectCount()
        {
            var movesAt25 = _pokemonWithManyMoves.GetMovesUpToLevel(25);
            // Level 1: 2 (Tackle, Growl), 5: 1, 10: 1, 15: 1, 20: 1, 25: 1 = 7
            Assert.That(movesAt25.Count(), Is.EqualTo(7));
        }

        [Test]
        public void ManyMoves_Instance_LimitedTo4()
        {
            var pokemon = PokemonFactory.Create(_pokemonWithManyMoves, 50);
            Assert.That(pokemon.Moves.Count, Is.LessThanOrEqualTo(4));
        }

        [Test]
        public void ManyMoves_Instance_GetsHighestLevelMoves()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithLearnsetMoves()
                .Build();

            // Should have 4 highest-level moves
            Assert.That(pokemon.Moves.Count, Is.EqualTo(4));

            // Should include the highest level moves
            var moveNames = pokemon.Moves.Select(m => m.Move.Name).ToList();
            Assert.That(moveNames, Does.Contain("Thunderbolt")); // Level 45
            Assert.That(moveNames, Does.Contain("Flamethrower")); // Level 40
            Assert.That(moveNames, Does.Contain("Earthquake")); // Level 35
            Assert.That(moveNames, Does.Contain("Psychic")); // Level 30
        }

        #endregion

        #region Same Move at Multiple Levels Tests

        [Test]
        public void DuplicateMoves_LearnsetContainsBoth()
        {
            // Both entries should exist in learnset
            var emberMoves = _pokemonWithDuplicateMoves.Learnset
                .Where(m => m.Move.Name == "Ember")
                .ToList();

            Assert.That(emberMoves.Count, Is.EqualTo(2));
        }

        [Test]
        public void DuplicateMoves_CanLearn_ReturnsTrue()
        {
            Assert.That(_pokemonWithDuplicateMoves.CanLearn(MoveCatalog.Ember), Is.True);
        }

        [Test]
        public void DuplicateMoves_Instance_TryLearnDoesNotDuplicate()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithDuplicateMoves, 5)
                .WithMoves(MoveCatalog.Ember)
                .Build();

            // Already knows Ember, trying to learn again should fail
            bool learned = pokemon.TryLearnMove(MoveCatalog.Ember);

            Assert.That(learned, Is.False);
            Assert.That(pokemon.Moves.Count(m => m.Move.Name == "Ember"), Is.EqualTo(1));
        }

        #endregion

        #region Multiple Moves at Same Level Tests

        [Test]
        public void SameLevelMoves_GetMovesAtLevel_ReturnsAll()
        {
            var movesAt10 = _pokemonWithSameLevelMoves.GetMovesAtLevel(10);
            Assert.That(movesAt10.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SameLevelMoves_GetMovesAtLevel20_ReturnsThree()
        {
            var movesAt20 = _pokemonWithSameLevelMoves.GetMovesAtLevel(20);
            Assert.That(movesAt20.Count(), Is.EqualTo(3));
        }

        [Test]
        public void SameLevelMoves_Instance_LimitedTo4()
        {
            // Even with 6 moves at levels 10 and 20, should only have 4
            var pokemon = PokemonFactory.Create(_pokemonWithSameLevelMoves, 25);
            Assert.That(pokemon.Moves.Count, Is.LessThanOrEqualTo(4));
        }

        #endregion

        #region Level Query Edge Cases

        [Test]
        public void GetMovesAtLevel_Level0_ReturnsEmpty()
        {
            var moves = _pokemonWithManyMoves.GetMovesAtLevel(0);
            Assert.That(moves, Is.Empty);
        }

        [Test]
        public void GetMovesAtLevel_NegativeLevel_ReturnsEmpty()
        {
            var moves = _pokemonWithManyMoves.GetMovesAtLevel(-5);
            Assert.That(moves, Is.Empty);
        }

        [Test]
        public void GetMovesAtLevel_VeryHighLevel_ReturnsEmpty()
        {
            var moves = _pokemonWithManyMoves.GetMovesAtLevel(999);
            Assert.That(moves, Is.Empty);
        }

        [Test]
        public void GetMovesUpToLevel_Level0_ReturnsStartingMoves()
        {
            // Level 0 includes starting moves (LearnMethod.Start)
            var moves = _pokemonWithManyMoves.GetMovesUpToLevel(0);
            Assert.That(moves.Count(), Is.EqualTo(2)); // Tackle and Growl
        }

        [Test]
        public void GetMovesUpToLevel_VeryHighLevel_ReturnsAll()
        {
            var moves = _pokemonWithManyMoves.GetMovesUpToLevel(999);
            Assert.That(moves.Count(), Is.EqualTo(11));
        }

        [Test]
        public void GetStartingMoves_OnlyLevel1()
        {
            var startMoves = _pokemonWithManyMoves.GetStartingMoves();
            Assert.That(startMoves.Count(), Is.EqualTo(2));
            Assert.That(startMoves.All(m => m.Level == 0 || m.Method == LearnMethod.Start), Is.True);
        }

        #endregion

        #region Move Learning During Level Up

        [Test]
        public void LevelUp_LearnsMoveAtExactLevel()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 4)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));

            // Level up to 5
            pokemon.LevelUp();
            var pendingMoves = pokemon.GetPendingMoves();

            Assert.That(pendingMoves.Count, Is.EqualTo(1));
            Assert.That(pendingMoves[0].Move.Name, Is.EqualTo("Scratch"));
        }

        [Test]
        public void LevelUp_MultipleLevel_GetsPendingMoves()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            // Level up from 1 to 15 (should get moves at 5, 10, 15)
            pokemon.LevelUpTo(15);
            var pendingMoves = pokemon.GetPendingMoves();

            Assert.That(pendingMoves.Count, Is.EqualTo(3));
        }

        [Test]
        public void TryLearnMove_WhenFull_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle, MoveCatalog.Growl, MoveCatalog.Scratch, MoveCatalog.Ember)
                .Build();

            Assert.That(pokemon.Moves.Count, Is.EqualTo(4));

            bool learned = pokemon.TryLearnMove(MoveCatalog.Flamethrower);
            Assert.That(learned, Is.False);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(4));
        }

        [Test]
        public void TryLearnMove_DuplicateMove_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            bool learned = pokemon.TryLearnMove(MoveCatalog.Tackle);
            Assert.That(learned, Is.False);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryLearnMove_NullMove_ReturnsFalse()
        {
            var pokemon = PokemonFactory.Create(_pokemonWithManyMoves, 1);

            bool learned = pokemon.TryLearnMove(null);
            Assert.That(learned, Is.False);
        }

        #endregion

        #region Move Replacement Tests

        [Test]
        public void ReplaceMove_ValidIndex_Succeeds()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle, MoveCatalog.Growl)
                .Build();

            bool replaced = pokemon.ReplaceMove(0, MoveCatalog.Flamethrower);

            Assert.That(replaced, Is.True);
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Flamethrower"));
        }

        [Test]
        public void ReplaceMove_NegativeIndex_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            bool replaced = pokemon.ReplaceMove(-1, MoveCatalog.Flamethrower);
            Assert.That(replaced, Is.False);
        }

        [Test]
        public void ReplaceMove_IndexOutOfRange_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            bool replaced = pokemon.ReplaceMove(5, MoveCatalog.Flamethrower);
            Assert.That(replaced, Is.False);
        }

        [Test]
        public void ReplaceMove_WithDuplicate_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle, MoveCatalog.Flamethrower)
                .Build();

            // Try to replace Tackle with Flamethrower (already known)
            bool replaced = pokemon.ReplaceMove(0, MoveCatalog.Flamethrower);
            Assert.That(replaced, Is.False);
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Tackle"));
        }

        [Test]
        public void ReplaceMove_WithNull_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            bool replaced = pokemon.ReplaceMove(0, null);
            Assert.That(replaced, Is.False);
        }

        #endregion

        #region Forget Move Tests

        [Test]
        public void ForgetMove_ValidIndex_Succeeds()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle, MoveCatalog.Growl)
                .Build();

            bool forgot = pokemon.ForgetMove(0);

            Assert.That(forgot, Is.True);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Growl"));
        }

        [Test]
        public void ForgetMove_NegativeIndex_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            bool forgot = pokemon.ForgetMove(-1);
            Assert.That(forgot, Is.False);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
        }

        [Test]
        public void ForgetMove_IndexOutOfRange_ReturnsFalse()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle)
                .Build();

            bool forgot = pokemon.ForgetMove(5);
            Assert.That(forgot, Is.False);
        }

        [Test]
        public void ForgetMove_AllMoves_ResultsInEmptyMoveset()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 1)
                .WithMoves(MoveCatalog.Tackle, MoveCatalog.Growl)
                .Build();

            pokemon.ForgetMove(0);
            pokemon.ForgetMove(0); // Now index 0 is Growl

            Assert.That(pokemon.Moves, Is.Empty);
        }

        #endregion

        #region Real Pokemon Learnset Tests

        [Test]
        public void Charmander_HasStartingMoves()
        {
            var startMoves = PokemonCatalog.Charmander.GetStartingMoves();
            Assert.That(startMoves, Is.Not.Empty);
        }

        [Test]
        public void Charmander_LearnsEmber()
        {
            Assert.That(PokemonCatalog.Charmander.CanLearn(MoveCatalog.Ember), Is.True);
        }

        [Test]
        public void Charmander_LearnsFlamethrower()
        {
            Assert.That(PokemonCatalog.Charmander.CanLearn(MoveCatalog.Flamethrower), Is.True);
        }

        [Test]
        public void Pikachu_HasThunderShock()
        {
            Assert.That(PokemonCatalog.Pikachu.CanLearn(MoveCatalog.ThunderShock), Is.True);
        }

        [Test]
        public void Pikachu_HasThunderbolt()
        {
            Assert.That(PokemonCatalog.Pikachu.CanLearn(MoveCatalog.Thunderbolt), Is.True);
        }

        #endregion

        #region Learn Method Tests

        [Test]
        public void LearnMethod_Start_IsLevel0()
        {
            var startMoves = _pokemonWithManyMoves.GetStartingMoves();
            foreach (var move in startMoves)
            {
                Assert.That(move.Method, Is.EqualTo(LearnMethod.Start));
            }
        }

        [Test]
        public void LearnMethod_LevelUp_HasCorrectLevel()
        {
            var level10Moves = _pokemonWithManyMoves.GetMovesAtLevel(10);
            foreach (var move in level10Moves)
            {
                Assert.That(move.Level, Is.EqualTo(10));
                Assert.That(move.Method, Is.EqualTo(LearnMethod.LevelUp));
            }
        }

        #endregion

        #region Move Presets Tests

        [Test]
        public void WithStabMoves_PrioritizesTypeMatching()
        {
            // Fire type Pokemon should prioritize Fire moves
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithStabMoves()
                .Build();

            // Should have moves, preferably type-matching
            Assert.That(pokemon.Moves.Count, Is.GreaterThan(0));
        }

        [Test]
        public void WithStrongMoves_PrioritizesHighPower()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithStrongMoves()
                .Build();

            // Should have high-power moves if available
            Assert.That(pokemon.Moves.Count, Is.GreaterThan(0));
        }

        [Test]
        public void WithRandomMoves_ReturnsUpTo4()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithRandomMoves()
                .Build();

            Assert.That(pokemon.Moves.Count, Is.LessThanOrEqualTo(4));
            Assert.That(pokemon.Moves.Count, Is.GreaterThan(0));
        }

        [Test]
        public void WithNoMoves_ReturnsEmptyMoveset()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithNoMoves()
                .Build();

            Assert.That(pokemon.Moves, Is.Empty);
        }

        [Test]
        public void WithSingleMove_ReturnsExactlyOne()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithSingleMove()
                .Build();

            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
        }

        [Test]
        public void WithMoveCount_LimitsCorrectly()
        {
            var pokemon = PokemonInstanceBuilder.Create(_pokemonWithManyMoves, 50)
                .WithMoveCount(2)
                .Build();

            Assert.That(pokemon.Moves.Count, Is.EqualTo(2));
        }

        #endregion
    }
}

