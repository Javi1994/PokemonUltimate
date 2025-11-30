using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using Pokemon = PokemonUltimate.Content.Builders.Pokemon;

namespace PokemonUltimate.Tests.Evolution
{
    /// <summary>
    /// Edge case tests for evolution chains and complex evolution scenarios.
    /// Tests multi-stage evolutions, branching paths, and edge conditions.
    /// </summary>
    [TestFixture]
    public class EvolutionChainsEdgeCasesTests
    {
        #region Test Pokemon Setup

        // Three-stage evolution chain
        private PokemonSpeciesData _stage3Pokemon;
        private PokemonSpeciesData _stage2Pokemon;
        private PokemonSpeciesData _stage1Pokemon;

        // Branching evolution (like Eevee)
        private PokemonSpeciesData _branchBase;
        private PokemonSpeciesData _branchEvo1;
        private PokemonSpeciesData _branchEvo2;
        private PokemonSpeciesData _branchEvo3;

        // No evolution Pokemon
        private PokemonSpeciesData _noEvoPokemon;

        // Multi-condition evolution
        private PokemonSpeciesData _multiCondBase;
        private PokemonSpeciesData _multiCondEvo;

        [SetUp]
        public void Setup()
        {
            // Stage 3 (final form) - no evolutions
            _stage3Pokemon = Pokemon.Define("TestFinalForm", 999)
                .Type(PokemonType.Fire)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            // Stage 2 - evolves to Stage 3 at level 36
            _stage2Pokemon = Pokemon.Define("TestMiddleForm", 998)
                .Type(PokemonType.Fire)
                .Stats(80, 80, 80, 80, 80, 80)
                .EvolvesTo(_stage3Pokemon, e => e.AtLevel(36))
                .Build();

            // Stage 1 - evolves to Stage 2 at level 16
            _stage1Pokemon = Pokemon.Define("TestBasicForm", 997)
                .Type(PokemonType.Fire)
                .Stats(50, 50, 50, 50, 50, 50)
                .EvolvesTo(_stage2Pokemon, e => e.AtLevel(16))
                .Build();

            // Branching evolutions (like Eevee)
            _branchEvo1 = Pokemon.Define("TestEvoFire", 996)
                .Type(PokemonType.Fire)
                .Stats(90, 90, 90, 90, 90, 90)
                .Build();

            _branchEvo2 = Pokemon.Define("TestEvoWater", 995)
                .Type(PokemonType.Water)
                .Stats(90, 90, 90, 90, 90, 90)
                .Build();

            _branchEvo3 = Pokemon.Define("TestEvoElectric", 994)
                .Type(PokemonType.Electric)
                .Stats(90, 90, 90, 90, 90, 90)
                .Build();

            _branchBase = Pokemon.Define("TestBranchBase", 993)
                .Type(PokemonType.Normal)
                .Stats(55, 55, 55, 55, 55, 55)
                .EvolvesTo(_branchEvo1, e => e.WithItem("Fire Stone"))
                .EvolvesTo(_branchEvo2, e => e.WithItem("Water Stone"))
                .EvolvesTo(_branchEvo3, e => e.WithItem("Thunder Stone"))
                .Build();

            // No evolution Pokemon (legendary-like)
            _noEvoPokemon = Pokemon.Define("TestLegendary", 992)
                .Type(PokemonType.Psychic)
                .Stats(150, 150, 150, 150, 150, 150)
                .Build();

            // Multi-condition evolution
            _multiCondEvo = Pokemon.Define("TestMultiCondEvo", 991)
                .Type(PokemonType.Psychic)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            _multiCondBase = Pokemon.Define("TestMultiCondBase", 990)
                .Type(PokemonType.Normal)
                .Stats(60, 60, 60, 60, 60, 60)
                .EvolvesTo(_multiCondEvo, e => e
                    .AtLevel(20)
                    .WithFriendship())
                .Build();
        }

        #endregion

        #region Three-Stage Evolution Chain Tests

        [Test]
        public void ThreeStageChain_Stage1_HasOneEvolution()
        {
            Assert.That(_stage1Pokemon.Evolutions.Count, Is.EqualTo(1));
            Assert.That(_stage1Pokemon.Evolutions[0].Target, Is.EqualTo(_stage2Pokemon));
        }

        [Test]
        public void ThreeStageChain_Stage2_HasOneEvolution()
        {
            Assert.That(_stage2Pokemon.Evolutions.Count, Is.EqualTo(1));
            Assert.That(_stage2Pokemon.Evolutions[0].Target, Is.EqualTo(_stage3Pokemon));
        }

        [Test]
        public void ThreeStageChain_Stage3_HasNoEvolutions()
        {
            Assert.That(_stage3Pokemon.CanEvolve, Is.False);
            Assert.That(_stage3Pokemon.Evolutions, Is.Empty);
        }

        [Test]
        public void ThreeStageChain_CanEvolveFromStage1ToStage2()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 16);
            
            Assert.That(pokemon.CanEvolve(), Is.True);
            var evolved = pokemon.TryEvolve();
            Assert.That(evolved, Is.EqualTo(_stage2Pokemon));
            Assert.That(pokemon.Species, Is.EqualTo(_stage2Pokemon));
        }

        [Test]
        public void ThreeStageChain_CannotSkipStage()
        {
            // Stage 1 at level 36 should evolve to Stage 2, not Stage 3
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 36);
            
            var availableEvolutions = pokemon.GetAvailableEvolutions();
            Assert.That(availableEvolutions.Count, Is.EqualTo(1));
            Assert.That(availableEvolutions[0].Target, Is.EqualTo(_stage2Pokemon));
        }

        [Test]
        public void ThreeStageChain_FullEvolutionChain()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 16);
            
            // First evolution: Stage 1 -> Stage 2
            Assert.That(pokemon.CanEvolve(), Is.True);
            pokemon.TryEvolve();
            Assert.That(pokemon.Species, Is.EqualTo(_stage2Pokemon));
            
            // At level 16, can't evolve to Stage 3 yet
            Assert.That(pokemon.CanEvolve(), Is.False);
            
            // Level up to 36
            pokemon.LevelUpTo(36);
            
            // Second evolution: Stage 2 -> Stage 3
            Assert.That(pokemon.CanEvolve(), Is.True);
            pokemon.TryEvolve();
            Assert.That(pokemon.Species, Is.EqualTo(_stage3Pokemon));
            
            // Final form - no more evolutions
            Assert.That(pokemon.CanEvolve(), Is.False);
        }

        [Test]
        public void ThreeStageChain_StatsIncreaseWithEachEvolution()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 36);
            int stage1HP = pokemon.MaxHP;
            
            pokemon.TryEvolve();
            int stage2HP = pokemon.MaxHP;
            Assert.That(stage2HP, Is.GreaterThan(stage1HP));
            
            pokemon.TryEvolve();
            int stage3HP = pokemon.MaxHP;
            Assert.That(stage3HP, Is.GreaterThan(stage2HP));
        }

        #endregion

        #region Branching Evolution Tests (Eevee-like)

        [Test]
        public void BranchingEvolution_BaseHasMultipleEvolutions()
        {
            Assert.That(_branchBase.Evolutions.Count, Is.EqualTo(3));
        }

        [Test]
        public void BranchingEvolution_CanGetAllPossibleEvolutions()
        {
            var pokemon = PokemonFactory.Create(_branchBase, 1);
            var possibleEvolutions = pokemon.GetPossibleEvolutions();
            
            Assert.That(possibleEvolutions.Count, Is.EqualTo(3));
            Assert.That(possibleEvolutions, Does.Contain(_branchEvo1));
            Assert.That(possibleEvolutions, Does.Contain(_branchEvo2));
            Assert.That(possibleEvolutions, Does.Contain(_branchEvo3));
        }

        [Test]
        public void BranchingEvolution_CannotAutoEvolve()
        {
            // Item evolutions require explicit item use
            var pokemon = PokemonFactory.Create(_branchBase, 50);
            
            Assert.That(pokemon.CanEvolve(), Is.False);
            Assert.That(pokemon.TryEvolve(), Is.Null);
        }

        [Test]
        public void BranchingEvolution_CanEvolveWithFireStone()
        {
            var pokemon = PokemonFactory.Create(_branchBase, 1);
            
            Assert.That(pokemon.CanEvolveWithItem("Fire Stone"), Is.True);
            Assert.That(pokemon.CanEvolveWithItem("Water Stone"), Is.True);
            Assert.That(pokemon.CanEvolveWithItem("Thunder Stone"), Is.True);
            
            var evolved = pokemon.EvolveWithItem("Fire Stone");
            Assert.That(evolved, Is.EqualTo(_branchEvo1));
            Assert.That(pokemon.Species.PrimaryType, Is.EqualTo(PokemonType.Fire));
        }

        [Test]
        public void BranchingEvolution_DifferentStonesGiveDifferentResults()
        {
            var pokemon1 = PokemonFactory.Create(_branchBase, 1);
            var pokemon2 = PokemonFactory.Create(_branchBase, 1);
            var pokemon3 = PokemonFactory.Create(_branchBase, 1);
            
            pokemon1.EvolveWithItem("Fire Stone");
            pokemon2.EvolveWithItem("Water Stone");
            pokemon3.EvolveWithItem("Thunder Stone");
            
            Assert.That(pokemon1.Species, Is.EqualTo(_branchEvo1));
            Assert.That(pokemon2.Species, Is.EqualTo(_branchEvo2));
            Assert.That(pokemon3.Species, Is.EqualTo(_branchEvo3));
            
            // All different
            Assert.That(pokemon1.Species, Is.Not.EqualTo(pokemon2.Species));
            Assert.That(pokemon2.Species, Is.Not.EqualTo(pokemon3.Species));
        }

        [Test]
        public void BranchingEvolution_WrongItemDoesNothing()
        {
            var pokemon = PokemonFactory.Create(_branchBase, 1);
            
            Assert.That(pokemon.CanEvolveWithItem("Moon Stone"), Is.False);
            var result = pokemon.EvolveWithItem("Moon Stone");
            
            Assert.That(result, Is.Null);
            Assert.That(pokemon.Species, Is.EqualTo(_branchBase));
        }

        [Test]
        public void BranchingEvolution_CannotEvolveAgainAfterBranching()
        {
            var pokemon = PokemonFactory.Create(_branchBase, 1);
            pokemon.EvolveWithItem("Fire Stone");
            
            // Evolved forms have no further evolutions
            Assert.That(pokemon.CanEvolve(), Is.False);
            Assert.That(pokemon.GetPossibleEvolutions(), Is.Empty);
        }

        #endregion

        #region No Evolution Pokemon Tests

        [Test]
        public void NoEvolution_CanEvolveFalse()
        {
            Assert.That(_noEvoPokemon.CanEvolve, Is.False);
        }

        [Test]
        public void NoEvolution_EvolutionsListEmpty()
        {
            Assert.That(_noEvoPokemon.Evolutions, Is.Empty);
        }

        [Test]
        public void NoEvolution_InstanceCannotEvolve()
        {
            var pokemon = PokemonFactory.Create(_noEvoPokemon, 100);
            
            Assert.That(pokemon.CanEvolve(), Is.False);
            Assert.That(pokemon.TryEvolve(), Is.Null);
            Assert.That(pokemon.GetAvailableEvolutions(), Is.Empty);
            Assert.That(pokemon.GetPossibleEvolutions(), Is.Empty);
        }

        [Test]
        public void NoEvolution_CannotEvolveWithItem()
        {
            var pokemon = PokemonFactory.Create(_noEvoPokemon, 100);
            
            Assert.That(pokemon.CanEvolveWithItem("Fire Stone"), Is.False);
            Assert.That(pokemon.EvolveWithItem("Fire Stone"), Is.Null);
        }

        [Test]
        public void NoEvolution_CannotEvolveByTrade()
        {
            var pokemon = PokemonFactory.Create(_noEvoPokemon, 100);
            
            Assert.That(pokemon.CanEvolveByTrade(), Is.False);
            Assert.That(pokemon.EvolveByTrade(), Is.Null);
        }

        #endregion

        #region Multi-Condition Evolution Tests

        [Test]
        public void MultiCondition_NeedsAllConditions()
        {
            // Level 20 but no friendship
            var pokemon1 = PokemonInstanceBuilder.Create(_multiCondBase, 20)
                .WithFriendship(50)
                .Build();
            Assert.That(pokemon1.CanEvolve(), Is.False);
            
            // High friendship but level 15
            var pokemon2 = PokemonInstanceBuilder.Create(_multiCondBase, 15)
                .WithFriendship(220)
                .Build();
            Assert.That(pokemon2.CanEvolve(), Is.False);
        }

        [Test]
        public void MultiCondition_EvolvesWhenAllMet()
        {
            var pokemon = PokemonInstanceBuilder.Create(_multiCondBase, 20)
                .WithFriendship(220)
                .Build();
            
            Assert.That(pokemon.CanEvolve(), Is.True);
            var evolved = pokemon.TryEvolve();
            Assert.That(evolved, Is.EqualTo(_multiCondEvo));
        }

        [Test]
        public void MultiCondition_ExceedingConditionsStillWorks()
        {
            // Level 50 with max friendship (exceeds requirements)
            var pokemon = PokemonInstanceBuilder.Create(_multiCondBase, 50)
                .WithFriendship(255)
                .Build();
            
            Assert.That(pokemon.CanEvolve(), Is.True);
        }

        #endregion

        #region Evolution Level Edge Cases

        [Test]
        public void Evolution_AtExactLevel()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 16);
            Assert.That(pokemon.CanEvolve(), Is.True);
        }

        [Test]
        public void Evolution_OneLevelBelow_CannotEvolve()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 15);
            Assert.That(pokemon.CanEvolve(), Is.False);
        }

        [Test]
        public void Evolution_HighAboveLevel_CanEvolve()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 100);
            Assert.That(pokemon.CanEvolve(), Is.True);
        }

        [Test]
        public void Evolution_Level1Pokemon_WithLevel1Evolution()
        {
            // Create a Pokemon that evolves at level 1 (edge case)
            var evo = Pokemon.Define("Level1Evo", 899)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();
            
            var base_ = Pokemon.Define("Level1Base", 898)
                .Type(PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .EvolvesTo(evo, e => e.AtLevel(1))
                .Build();
            
            var pokemon = PokemonFactory.Create(base_, 1);
            Assert.That(pokemon.CanEvolve(), Is.True);
        }

        [Test]
        public void Evolution_AtMaxLevel()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 100);
            
            // Can still evolve even at max level
            Assert.That(pokemon.CanEvolve(), Is.True);
            pokemon.TryEvolve();
            Assert.That(pokemon.Species, Is.EqualTo(_stage2Pokemon));
            Assert.That(pokemon.Level, Is.EqualTo(100));
        }

        #endregion

        #region Real Pokemon Evolution Tests (Using Catalog)

        [Test]
        public void RealPokemon_CharmanderChain()
        {
            var charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 16);
            
            Assert.That(charmander.CanEvolve(), Is.True);
            charmander.TryEvolve();
            Assert.That(charmander.Species.Name, Is.EqualTo("Charmeleon"));
            
            // Level up to 36
            charmander.LevelUpTo(36);
            Assert.That(charmander.CanEvolve(), Is.True);
            charmander.TryEvolve();
            Assert.That(charmander.Species.Name, Is.EqualTo("Charizard"));
            
            // Final form
            Assert.That(charmander.CanEvolve(), Is.False);
        }

        [Test]
        public void RealPokemon_PikachuThunderStone()
        {
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            
            // Can't auto-evolve
            Assert.That(pikachu.CanEvolve(), Is.False);
            
            // Can evolve with Thunder Stone
            Assert.That(pikachu.CanEvolveWithItem("Thunder Stone"), Is.True);
            pikachu.EvolveWithItem("Thunder Stone");
            Assert.That(pikachu.Species.Name, Is.EqualTo("Raichu"));
        }

        [Test]
        public void RealPokemon_PikachuWrongStone()
        {
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
            
            Assert.That(pikachu.CanEvolveWithItem("Fire Stone"), Is.False);
            Assert.That(pikachu.EvolveWithItem("Fire Stone"), Is.Null);
            Assert.That(pikachu.Species.Name, Is.EqualTo("Pikachu"));
        }

        #endregion

        #region Evolution State Preservation Tests

        [Test]
        public void Evolution_PreservesNickname()
        {
            var pokemon = PokemonInstanceBuilder.Create(_stage1Pokemon, 16)
                .Named("Flamey")
                .Build();
            
            pokemon.TryEvolve();
            Assert.That(pokemon.Nickname, Is.EqualTo("Flamey"));
        }

        [Test]
        public void Evolution_PreservesFriendship()
        {
            var pokemon = PokemonInstanceBuilder.Create(_stage1Pokemon, 16)
                .WithFriendship(200)
                .Build();
            
            pokemon.TryEvolve();
            Assert.That(pokemon.Friendship, Is.EqualTo(200));
        }

        [Test]
        public void Evolution_PreservesMoves()
        {
            var pokemon = PokemonInstanceBuilder.Create(_stage1Pokemon, 16)
                .WithMoves(MoveCatalog.Ember, MoveCatalog.Scratch)
                .Build();
            
            pokemon.TryEvolve();
            Assert.That(pokemon.Moves.Count, Is.EqualTo(2));
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Ember"));
        }

        [Test]
        public void Evolution_PreservesExperience()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 16);
            pokemon.CurrentExp = 5000;
            
            pokemon.TryEvolve();
            Assert.That(pokemon.CurrentExp, Is.EqualTo(5000));
        }

        [Test]
        public void Evolution_PreservesGenderAndNature()
        {
            var pokemon = PokemonInstanceBuilder.Create(_stage1Pokemon, 16)
                .WithNature(Nature.Jolly)
                .Female()
                .Build();
            
            pokemon.TryEvolve();
            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Jolly));
            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
        }

        [Test]
        public void Evolution_PreservesShinyStatus()
        {
            var pokemon = PokemonInstanceBuilder.Create(_stage1Pokemon, 16)
                .Shiny()
                .Build();
            
            pokemon.TryEvolve();
            Assert.That(pokemon.IsShiny, Is.True);
        }

        #endregion

        #region Evolve Method Validation

        [Test]
        public void Evolve_ToInvalidTarget_ReturnsFalse()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 50);
            
            // Try to evolve directly to Stage 3 (skipping Stage 2)
            bool result = pokemon.Evolve(_stage3Pokemon);
            
            Assert.That(result, Is.False);
            Assert.That(pokemon.Species, Is.EqualTo(_stage1Pokemon));
        }

        [Test]
        public void Evolve_ToNullTarget_ReturnsFalse()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 50);
            
            bool result = pokemon.Evolve(null);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evolve_ToUnrelatedSpecies_ReturnsFalse()
        {
            var pokemon = PokemonFactory.Create(_stage1Pokemon, 50);
            
            // Try to evolve to a completely unrelated Pokemon
            bool result = pokemon.Evolve(_noEvoPokemon);
            
            Assert.That(result, Is.False);
            Assert.That(pokemon.Species, Is.EqualTo(_stage1Pokemon));
        }

        #endregion
    }
}

