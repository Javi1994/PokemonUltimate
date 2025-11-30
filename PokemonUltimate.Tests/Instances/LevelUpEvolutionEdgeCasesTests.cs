using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Instances
{
    /// <summary>
    /// Tests for edge cases in Level Up and Evolution systems.
    /// These are critical systems that must be bulletproof.
    /// </summary>
    [TestFixture]
    public class LevelUpEvolutionEdgeCasesTests
    {
        private PokemonSpeciesData _testSpecies;
        private PokemonSpeciesData _evolvedSpecies;
        private PokemonSpeciesData _finalSpecies;
        private MoveData _level5Move;
        private MoveData _level10Move;
        private MoveData _level15Move;
        private MoveData _level20Move;

        [SetUp]
        public void SetUp()
        {
            // Create test moves at different levels
            _level5Move = new MoveData { Name = "Level5Move", Type = PokemonType.Normal, MaxPP = 10 };
            _level10Move = new MoveData { Name = "Level10Move", Type = PokemonType.Normal, MaxPP = 10 };
            _level15Move = new MoveData { Name = "Level15Move", Type = PokemonType.Normal, MaxPP = 10 };
            _level20Move = new MoveData { Name = "Level20Move", Type = PokemonType.Normal, MaxPP = 10 };

            // Create final evolution first (no evolutions)
            _finalSpecies = new PokemonSpeciesData
            {
                Name = "FinalForm",
                PokedexNumber = 3,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(100, 100, 100, 100, 100, 100)
            };

            // Create middle evolution
            _evolvedSpecies = new PokemonSpeciesData
            {
                Name = "EvolvedForm",
                PokedexNumber = 2,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(80, 80, 80, 80, 80, 80),
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = _finalSpecies,
                        Conditions = new List<Core.Evolution.IEvolutionCondition>
                        {
                            new LevelCondition(36)
                        }
                    }
                }
            };

            // Create base species with moves and evolutions
            _testSpecies = new PokemonSpeciesData
            {
                Name = "TestMon",
                PokedexNumber = 1,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(50, 50, 50, 50, 50, 50),
                Learnset = new List<LearnableMove>
                {
                    new LearnableMove(_level5Move, LearnMethod.LevelUp, 5),
                    new LearnableMove(_level10Move, LearnMethod.LevelUp, 10),
                    new LearnableMove(_level15Move, LearnMethod.LevelUp, 15),
                    new LearnableMove(_level20Move, LearnMethod.LevelUp, 20)
                },
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = _evolvedSpecies,
                        Conditions = new List<Core.Evolution.IEvolutionCondition>
                        {
                            new LevelCondition(16)
                        }
                    }
                }
            };
        }

        #region Level Up Edge Cases

        [Test]
        public void Level100_Cannot_Level_Up()
        {
            var pokemon = CreatePokemon(level: 100);

            Assert.That(pokemon.LevelUp(), Is.False);
            Assert.That(pokemon.Level, Is.EqualTo(100));
        }

        [Test]
        public void Level100_AddExperience_Does_Nothing()
        {
            var pokemon = CreatePokemon(level: 100);
            pokemon.CurrentExp = 1000000;

            int levelsGained = pokemon.AddExperience(999999);

            Assert.That(levelsGained, Is.EqualTo(0));
            Assert.That(pokemon.Level, Is.EqualTo(100));
        }

        [Test]
        public void LevelUpTo_Above_100_Caps_At_100()
        {
            var pokemon = CreatePokemon(level: 95);

            int levelsGained = pokemon.LevelUpTo(100);

            Assert.That(pokemon.Level, Is.EqualTo(100));
            Assert.That(levelsGained, Is.EqualTo(5));
        }

        [Test]
        public void LevelUpTo_Same_Level_Returns_Zero()
        {
            var pokemon = CreatePokemon(level: 50);

            int levelsGained = pokemon.LevelUpTo(50);

            Assert.That(levelsGained, Is.EqualTo(0));
        }

        [Test]
        public void LevelUpTo_Lower_Level_Returns_Zero()
        {
            var pokemon = CreatePokemon(level: 50);

            int levelsGained = pokemon.LevelUpTo(30);

            Assert.That(levelsGained, Is.EqualTo(0));
            Assert.That(pokemon.Level, Is.EqualTo(50));
        }

        [Test]
        public void Negative_Experience_Throws()
        {
            var pokemon = CreatePokemon(level: 10);

            Assert.That(() => pokemon.AddExperience(-100), Throws.ArgumentException);
        }

        [Test]
        public void Stats_Increase_Each_Level()
        {
            var pokemon = CreatePokemon(level: 10);
            var previousHP = pokemon.MaxHP;

            for (int i = 0; i < 5; i++)
            {
                pokemon.LevelUp();
                Assert.That(pokemon.MaxHP, Is.GreaterThan(previousHP));
                previousHP = pokemon.MaxHP;
            }
        }

        [Test]
        public void CurrentHP_Increases_With_MaxHP_On_LevelUp()
        {
            var pokemon = CreatePokemon(level: 10);
            pokemon.CurrentHP = pokemon.MaxHP; // Full HP
            int oldMaxHP = pokemon.MaxHP;

            pokemon.LevelUp();

            int hpGain = pokemon.MaxHP - oldMaxHP;
            Assert.That(hpGain, Is.GreaterThan(0));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        [Test]
        public void Damaged_Pokemon_Maintains_HP_Ratio_On_LevelUp()
        {
            var pokemon = CreatePokemon(level: 10);
            pokemon.CurrentHP = pokemon.MaxHP / 2; // 50% HP

            pokemon.LevelUp();

            // Should still be approximately 50%
            float ratio = (float)pokemon.CurrentHP / pokemon.MaxHP;
            Assert.That(ratio, Is.EqualTo(0.5f).Within(0.1f));
        }

        #endregion

        #region Multi-Level Move Learning

        [Test]
        public void GetPendingMoves_Returns_Moves_From_Skipped_Levels()
        {
            var pokemon = CreatePokemon(level: 1);

            // Jump from level 1 to level 20
            pokemon.LevelUpTo(20);

            var pendingMoves = pokemon.GetPendingMoves();

            // Should include all moves from levels 5, 10, 15, 20
            Assert.That(pendingMoves.Count, Is.EqualTo(4));
        }

        [Test]
        public void GetPendingMoves_Excludes_Already_Known_Moves()
        {
            var pokemon = CreatePokemon(level: 1);
            pokemon.TryLearnMove(_level5Move); // Already know level 5 move

            pokemon.LevelUpTo(20);
            var pendingMoves = pokemon.GetPendingMoves();

            // Should NOT include level 5 move since we already know it
            Assert.That(pendingMoves.Count, Is.EqualTo(3));
            Assert.That(pendingMoves.Any(m => m.Move == _level5Move), Is.False);
        }

        [Test]
        public void TryLearnAllPendingMoves_Learns_Multiple_Moves()
        {
            var pokemon = CreatePokemon(level: 1);
            pokemon.Moves.Clear(); // Start with no moves

            pokemon.LevelUpTo(15);
            var learned = pokemon.TryLearnAllPendingMoves();

            // Should learn 3 moves (level 5, 10, 15)
            Assert.That(learned.Count, Is.EqualTo(3));
            Assert.That(pokemon.Moves.Count, Is.EqualTo(3));
        }

        [Test]
        public void TryLearnAllPendingMoves_Stops_At_4_Moves()
        {
            var pokemon = CreatePokemon(level: 1);
            pokemon.Moves.Clear();

            pokemon.LevelUpTo(20);
            var learned = pokemon.TryLearnAllPendingMoves();

            // Should only learn 4 moves even though 4 are available
            Assert.That(pokemon.Moves.Count, Is.EqualTo(4));
        }

        [Test]
        public void ClearPendingMoves_Resets_Check_Level()
        {
            var pokemon = CreatePokemon(level: 1);
            pokemon.LevelUpTo(10);

            var pending1 = pokemon.GetPendingMoves();
            Assert.That(pending1.Count, Is.EqualTo(2)); // Level 5 and 10

            pokemon.ClearPendingMoves();

            var pending2 = pokemon.GetPendingMoves();
            Assert.That(pending2.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddExperience_MultiLevel_Tracks_Correctly()
        {
            var pokemon = CreatePokemon(level: 1);
            pokemon.CurrentExp = 0;

            // Add massive experience (enough for level 20+)
            int levelsGained = pokemon.AddExperience(10000);

            Assert.That(levelsGained, Is.GreaterThan(10));
            Assert.That(pokemon.Level, Is.GreaterThan(10));
        }

        #endregion

        #region Evolution Edge Cases

        [Test]
        public void CanEvolve_False_Before_Required_Level()
        {
            var pokemon = CreatePokemon(level: 15);

            Assert.That(pokemon.CanEvolve(), Is.False);
        }

        [Test]
        public void CanEvolve_True_At_Exact_Level()
        {
            var pokemon = CreatePokemon(level: 16);

            Assert.That(pokemon.CanEvolve(), Is.True);
        }

        [Test]
        public void CanEvolve_True_Above_Required_Level()
        {
            var pokemon = CreatePokemon(level: 50);

            Assert.That(pokemon.CanEvolve(), Is.True);
        }

        [Test]
        public void TryEvolve_Returns_Null_When_Cannot_Evolve()
        {
            var pokemon = CreatePokemon(level: 10);

            var result = pokemon.TryEvolve();

            Assert.That(result, Is.Null);
            Assert.That(pokemon.Species, Is.EqualTo(_testSpecies));
        }

        [Test]
        public void TryEvolve_Changes_Species_When_Can_Evolve()
        {
            var pokemon = CreatePokemon(level: 16);

            var result = pokemon.TryEvolve();

            Assert.That(result, Is.EqualTo(_evolvedSpecies));
            Assert.That(pokemon.Species, Is.EqualTo(_evolvedSpecies));
        }

        [Test]
        public void Evolution_Preserves_Level()
        {
            var pokemon = CreatePokemon(level: 20);
            pokemon.LevelUp(); // Level 21

            pokemon.TryEvolve();

            Assert.That(pokemon.Level, Is.EqualTo(21));
        }

        [Test]
        public void Evolution_Preserves_Nature()
        {
            var pokemon = CreatePokemon(level: 16);

            pokemon.TryEvolve();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Hardy));
        }

        [Test]
        public void Evolution_Preserves_Nickname()
        {
            var pokemon = CreatePokemon(level: 16);
            pokemon.Nickname = "CustomName";

            pokemon.TryEvolve();

            Assert.That(pokemon.Nickname, Is.EqualTo("CustomName"));
        }

        [Test]
        public void Evolution_Preserves_Friendship()
        {
            var pokemon = CreatePokemon(level: 16);
            pokemon.Friendship = 200;

            pokemon.TryEvolve();

            Assert.That(pokemon.Friendship, Is.EqualTo(200));
        }

        [Test]
        public void Evolution_Preserves_Moves()
        {
            var pokemon = CreatePokemon(level: 16);
            pokemon.TryLearnMove(_level5Move);
            int moveCount = pokemon.Moves.Count;

            pokemon.TryEvolve();

            Assert.That(pokemon.Moves.Count, Is.EqualTo(moveCount));
            Assert.That(pokemon.Moves.Any(m => m.Move == _level5Move), Is.True);
        }

        [Test]
        public void Evolution_Recalculates_Stats()
        {
            var pokemon = CreatePokemon(level: 16);
            int oldMaxHP = pokemon.MaxHP;

            pokemon.TryEvolve();

            // Evolved form has higher base stats
            Assert.That(pokemon.MaxHP, Is.GreaterThan(oldMaxHP));
        }

        [Test]
        public void Evolution_Maintains_HP_Percentage()
        {
            var pokemon = CreatePokemon(level: 16);
            pokemon.CurrentHP = pokemon.MaxHP / 2; // 50%

            pokemon.TryEvolve();

            float ratio = (float)pokemon.CurrentHP / pokemon.MaxHP;
            Assert.That(ratio, Is.EqualTo(0.5f).Within(0.1f));
        }

        [Test]
        public void Fainted_Pokemon_Stays_Fainted_After_Evolution()
        {
            var pokemon = CreatePokemon(level: 16);
            pokemon.CurrentHP = 0;

            pokemon.TryEvolve();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(0));
            Assert.That(pokemon.IsFainted, Is.True);
        }

        [Test]
        public void Evolved_Pokemon_Can_Evolve_Again()
        {
            var pokemon = CreatePokemon(level: 36);
            pokemon.TryEvolve(); // First evolution

            Assert.That(pokemon.Species, Is.EqualTo(_evolvedSpecies));
            Assert.That(pokemon.CanEvolve(), Is.True);

            pokemon.TryEvolve(); // Second evolution
            Assert.That(pokemon.Species, Is.EqualTo(_finalSpecies));
        }

        [Test]
        public void GetPossibleEvolutions_Returns_All_Targets()
        {
            var pokemon = CreatePokemon(level: 1);

            var possible = pokemon.GetPossibleEvolutions();

            Assert.That(possible.Count, Is.EqualTo(1));
            Assert.That(possible[0], Is.EqualTo(_evolvedSpecies));
        }

        [Test]
        public void Evolve_Rejects_Invalid_Target_With_Exception()
        {
            var pokemon = CreatePokemon(level: 16);
            var invalidTarget = new PokemonSpeciesData { Name = "Invalid" };

            Assert.Throws<ArgumentException>(() => pokemon.Evolve(invalidTarget));
            Assert.That(pokemon.Species, Is.EqualTo(_testSpecies));
        }

        #endregion

        #region Item Evolution Tests

        [Test]
        public void Item_Evolution_Works_With_Correct_Item()
        {
            var raichu = new PokemonSpeciesData { Name = "Raichu", BaseStats = new BaseStats(60, 90, 55, 90, 80, 110) };
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = raichu,
                        Conditions = new List<Core.Evolution.IEvolutionCondition> { new ItemCondition("Thunder Stone") }
                    }
                }
            };

            var pokemon = CreatePokemonFromSpecies(pikachu, 25);

            Assert.That(pokemon.CanEvolve(), Is.False); // Can't auto-evolve
            Assert.That(pokemon.CanEvolveWithItem("Thunder Stone"), Is.True);
            Assert.That(pokemon.CanEvolveWithItem("Fire Stone"), Is.False);

            var result = pokemon.EvolveWithItem("Thunder Stone");
            Assert.That(result, Is.EqualTo(raichu));
            Assert.That(pokemon.Species, Is.EqualTo(raichu));
        }

        [Test]
        public void Item_Evolution_Fails_With_Wrong_Item()
        {
            var raichu = new PokemonSpeciesData { Name = "Raichu", BaseStats = new BaseStats(60, 90, 55, 90, 80, 110) };
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = raichu,
                        Conditions = new List<Core.Evolution.IEvolutionCondition> { new ItemCondition("Thunder Stone") }
                    }
                }
            };

            var pokemon = CreatePokemonFromSpecies(pikachu, 25);

            var result = pokemon.EvolveWithItem("Fire Stone");
            Assert.That(result, Is.Null);
            Assert.That(pokemon.Species, Is.EqualTo(pikachu));
        }

        #endregion

        #region Trade Evolution Tests

        [Test]
        public void Trade_Evolution_Works()
        {
            var gengar = new PokemonSpeciesData { Name = "Gengar", BaseStats = new BaseStats(60, 65, 60, 130, 75, 110) };
            var haunter = new PokemonSpeciesData
            {
                Name = "Haunter",
                BaseStats = new BaseStats(45, 50, 45, 115, 55, 95),
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = gengar,
                        Conditions = new List<Core.Evolution.IEvolutionCondition> { new TradeCondition() }
                    }
                }
            };

            var pokemon = CreatePokemonFromSpecies(haunter, 30);

            Assert.That(pokemon.CanEvolve(), Is.False); // Can't auto-evolve
            Assert.That(pokemon.CanEvolveByTrade(), Is.True);

            var result = pokemon.EvolveByTrade();
            Assert.That(result, Is.EqualTo(gengar));
        }

        #endregion

        #region Friendship Evolution Tests

        [Test]
        public void Friendship_Evolution_Works_When_Happy()
        {
            var espeon = new PokemonSpeciesData { Name = "Espeon", BaseStats = new BaseStats(65, 65, 60, 130, 95, 110) };
            var eevee = new PokemonSpeciesData
            {
                Name = "Eevee",
                BaseStats = new BaseStats(55, 55, 50, 45, 65, 55),
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = espeon,
                        Conditions = new List<Core.Evolution.IEvolutionCondition> { new FriendshipCondition(220) }
                    }
                }
            };

            var pokemon = CreatePokemonFromSpecies(eevee, 20);
            pokemon.Friendship = 100;

            Assert.That(pokemon.CanEvolve(), Is.False);

            pokemon.Friendship = 220;
            Assert.That(pokemon.CanEvolve(), Is.True);

            var result = pokemon.TryEvolve();
            Assert.That(result, Is.EqualTo(espeon));
        }

        #endregion

        #region Knows Move Evolution Tests

        [Test]
        public void KnowsMove_Evolution_Works()
        {
            var ancientPower = new MoveData { Name = "Ancient Power", Type = PokemonType.Rock, MaxPP = 5 };
            var mamoswine = new PokemonSpeciesData { Name = "Mamoswine", BaseStats = new BaseStats(110, 130, 80, 70, 60, 80) };
            var piloswine = new PokemonSpeciesData
            {
                Name = "Piloswine",
                BaseStats = new BaseStats(100, 100, 80, 60, 60, 50),
                Evolutions = new List<Core.Evolution.Evolution>
                {
                    new Core.Evolution.Evolution
                    {
                        Target = mamoswine,
                        Conditions = new List<Core.Evolution.IEvolutionCondition> { new KnowsMoveCondition(ancientPower) }
                    }
                }
            };

            var pokemon = CreatePokemonFromSpecies(piloswine, 40);
            Assert.That(pokemon.CanEvolve(), Is.False);

            pokemon.TryLearnMove(ancientPower);
            Assert.That(pokemon.CanEvolve(), Is.True);

            var result = pokemon.TryEvolve();
            Assert.That(result, Is.EqualTo(mamoswine));
        }

        #endregion

        #region Helper Methods

        private PokemonInstance CreatePokemon(int level)
        {
            return new PokemonInstance(
                _testSpecies, level,
                StatCalculator.CalculateHP(50, level),
                StatCalculator.CalculateStat(50, level, Nature.Hardy, Stat.Attack),
                StatCalculator.CalculateStat(50, level, Nature.Hardy, Stat.Defense),
                StatCalculator.CalculateStat(50, level, Nature.Hardy, Stat.SpAttack),
                StatCalculator.CalculateStat(50, level, Nature.Hardy, Stat.SpDefense),
                StatCalculator.CalculateStat(50, level, Nature.Hardy, Stat.Speed),
                Nature.Hardy, Gender.Male, new List<MoveInstance>());
        }

        private PokemonInstance CreatePokemonFromSpecies(PokemonSpeciesData species, int level)
        {
            return new PokemonInstance(
                species, level,
                StatCalculator.CalculateHP(species.BaseStats.HP, level),
                StatCalculator.CalculateStat(species.BaseStats.Attack, level, Nature.Hardy, Stat.Attack),
                StatCalculator.CalculateStat(species.BaseStats.Defense, level, Nature.Hardy, Stat.Defense),
                StatCalculator.CalculateStat(species.BaseStats.SpAttack, level, Nature.Hardy, Stat.SpAttack),
                StatCalculator.CalculateStat(species.BaseStats.SpDefense, level, Nature.Hardy, Stat.SpDefense),
                StatCalculator.CalculateStat(species.BaseStats.Speed, level, Nature.Hardy, Stat.Speed),
                Nature.Hardy, Gender.Male, new List<MoveInstance>());
        }

        #endregion
    }
}

