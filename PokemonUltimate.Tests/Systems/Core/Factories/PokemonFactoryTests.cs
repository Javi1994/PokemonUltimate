using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    /// <summary>
    /// Tests for PokemonFactory - static factory for quick Pokemon creation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.3: PokemonInstance
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonFactoryTests
    {
        private PokemonSpeciesData _pikachu;
        private PokemonSpeciesData _genderlessSpecies;
        private PokemonSpeciesData _maleOnlySpecies;
        private PokemonSpeciesData _femaleOnlySpecies;

        [SetUp]
        public void SetUp()
        {
            // Reset random for consistent tests
            PokemonFactory.SetSeed(12345);

            _pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                GenderRatio = 50f, // 50% male
                Learnset = new System.Collections.Generic.List<LearnableMove>
                {
                    new LearnableMove(CreateMove("Tackle"), LearnMethod.Start, 1),
                    new LearnableMove(CreateMove("Thunder Shock"), LearnMethod.Start, 1),
                    new LearnableMove(CreateMove("Thunderbolt"), LearnMethod.LevelUp, 26),
                    new LearnableMove(CreateMove("Thunder"), LearnMethod.LevelUp, 50)
                }
            };

            _genderlessSpecies = new PokemonSpeciesData
            {
                Name = "Magnemite",
                PokedexNumber = 81,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(25, 35, 70, 95, 55, 45),
                GenderRatio = -1f // Genderless
            };

            _maleOnlySpecies = new PokemonSpeciesData
            {
                Name = "Tauros",
                PokedexNumber = 128,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(75, 100, 95, 40, 70, 110),
                GenderRatio = 100f // Male only
            };

            _femaleOnlySpecies = new PokemonSpeciesData
            {
                Name = "Chansey",
                PokedexNumber = 113,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(250, 5, 5, 35, 105, 50),
                GenderRatio = 0f // Female only
            };
        }

        [TearDown]
        public void TearDown()
        {
            PokemonFactory.ResetRandom();
        }

        private MoveData CreateMove(string name)
        {
            return new MoveData { Name = name, MaxPP = 10 };
        }

        #region Basic Creation Tests

        [Test]
        public void Create_Should_Return_Instance_With_Correct_Species()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 25);

            Assert.That(pokemon.Species, Is.EqualTo(_pikachu));
        }

        [Test]
        public void Create_Should_Return_Instance_With_Correct_Level()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 50);

            Assert.That(pokemon.Level, Is.EqualTo(50));
        }

        [Test]
        public void Create_Should_Throw_For_Null_Species()
        {
            Assert.That(() => PokemonFactory.Create(null, 25), Throws.ArgumentNullException);
        }

        [Test]
        public void Create_Should_Throw_For_Level_Zero()
        {
            Assert.That(() => PokemonFactory.Create(_pikachu, 0), Throws.ArgumentException);
        }

        [Test]
        public void Create_Should_Throw_For_Level_Over_100()
        {
            Assert.That(() => PokemonFactory.Create(_pikachu, 101), Throws.ArgumentException);
        }

        #endregion

        #region Stat Calculation Tests

        [Test]
        public void Create_Should_Calculate_HP_Correctly()
        {
            // Pikachu base HP = 35, Level 50, IV=31, EV=252
            // ((2*35 + 31 + 63) * 50 / 100) + 50 + 10 = 82 + 60 = 142
            var pokemon = Pokemon.Create(_pikachu, 50)
                .WithNature(Nature.Hardy)
                .WithPerfectIVs()
                .Build();

            Assert.That(pokemon.MaxHP, Is.EqualTo(142));
        }

        [Test]
        public void Create_Should_Calculate_Attack_With_Nature()
        {
            // Pikachu base Attack = 55, Level 50, Adamant (+Atk), IV=31, EV=252
            // ((2*55 + 31 + 63) * 50 / 100) + 5 = 102 + 5 = 107, * 1.1 = 117
            var pokemon = Pokemon.Create(_pikachu, 50)
                .WithNature(Nature.Adamant)
                .WithPerfectIVs()
                .Build();

            Assert.That(pokemon.Attack, Is.EqualTo(117));
        }

        [Test]
        public void Create_Should_Calculate_SpAttack_Reduced_By_Nature()
        {
            // Pikachu base SpAttack = 50, Level 50, Adamant (-SpAtk), IV=31, EV=252
            // ((2*50 + 31 + 63) * 50 / 100) + 5 = 97 + 5 = 102, * 0.9 = 91
            var pokemon = Pokemon.Create(_pikachu, 50)
                .WithNature(Nature.Adamant)
                .WithPerfectIVs()
                .WithEVs(0, 0, 0, 252, 0, 0) // SpAttack EVs = 252
                .Build();

            Assert.That(pokemon.SpAttack, Is.EqualTo(91));
        }

        [Test]
        public void Create_Should_Start_At_Full_HP()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 50);

            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        #endregion

        #region Move Selection Tests

        [Test]
        public void Create_Should_Select_Moves_From_Learnset()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 25);

            Assert.That(pokemon.Moves, Has.Count.GreaterThan(0));
        }

        [Test]
        public void Create_Should_Select_Starting_Moves_At_Level_1()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 1);

            Assert.That(pokemon.Moves.Select(m => m.Move.Name), 
                Contains.Item("Tackle").And.Contains("Thunder Shock"));
        }

        [Test]
        public void Create_Should_Include_Level_Up_Moves()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 30);

            Assert.That(pokemon.Moves.Select(m => m.Move.Name), Contains.Item("Thunderbolt"));
        }

        [Test]
        public void Create_Should_Not_Include_Higher_Level_Moves()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 30);

            Assert.That(pokemon.Moves.Select(m => m.Move.Name), Does.Not.Contain("Thunder"));
        }

        [Test]
        public void Create_Should_Limit_To_4_Moves()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 100);

            Assert.That(pokemon.Moves, Has.Count.LessThanOrEqualTo(4));
        }

        [Test]
        public void Create_Should_Prioritize_Higher_Level_Moves()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 60);

            // Should have Thunder (50), Thunderbolt (26) before the level 1 moves
            var moveNames = pokemon.Moves.Select(m => m.Move.Name).ToList();
            Assert.That(moveNames[0], Is.EqualTo("Thunder"));
            Assert.That(moveNames[1], Is.EqualTo("Thunderbolt"));
        }

        [Test]
        public void Create_Should_Handle_Empty_Learnset()
        {
            var noMovesSpecies = new PokemonSpeciesData
            {
                Name = "NoMoves",
                BaseStats = new BaseStats(100, 100, 100, 100, 100, 100)
            };

            var pokemon = PokemonFactory.Create(noMovesSpecies, 50, Nature.Hardy, Gender.Male);

            // When Pokemon has no learnset, it should have Tackle as default move to prevent infinite battles
            Assert.That(pokemon.Moves, Is.Not.Empty);
            Assert.That(pokemon.Moves.Count, Is.EqualTo(1));
            Assert.That(pokemon.Moves[0].Move.Name, Is.EqualTo("Tackle"));
        }

        #endregion

        #region Gender Tests

        [Test]
        public void Create_Should_Set_Gender_For_Genderless_Species()
        {
            var pokemon = PokemonFactory.Create(_genderlessSpecies, 25);

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Genderless));
        }

        [Test]
        public void Create_Should_Set_Gender_For_Male_Only_Species()
        {
            var pokemon = PokemonFactory.Create(_maleOnlySpecies, 25);

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Male));
        }

        [Test]
        public void Create_Should_Set_Gender_For_Female_Only_Species()
        {
            var pokemon = PokemonFactory.Create(_femaleOnlySpecies, 25);

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
        }

        [Test]
        public void Create_WithGender_Should_Throw_For_Wrong_Gender_On_Genderless()
        {
            Assert.That(
                () => PokemonFactory.Create(_genderlessSpecies, 25, Nature.Hardy, Gender.Male),
                Throws.ArgumentException);
        }

        [Test]
        public void Create_WithGender_Should_Throw_For_Female_On_Male_Only()
        {
            Assert.That(
                () => PokemonFactory.Create(_maleOnlySpecies, 25, Nature.Hardy, Gender.Female),
                Throws.ArgumentException);
        }

        [Test]
        public void Create_WithGender_Should_Throw_For_Male_On_Female_Only()
        {
            Assert.That(
                () => PokemonFactory.Create(_femaleOnlySpecies, 25, Nature.Hardy, Gender.Male),
                Throws.ArgumentException);
        }

        [Test]
        public void Create_WithGender_Should_Throw_For_Genderless_On_Gendered_Species()
        {
            Assert.That(
                () => PokemonFactory.Create(_pikachu, 25, Nature.Hardy, Gender.Genderless),
                Throws.ArgumentException);
        }

        [Test]
        public void Create_WithGender_Should_Accept_Valid_Gender()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 25, Nature.Hardy, Gender.Female);

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
        }

        #endregion

        #region Nature Tests

        [Test]
        public void Create_WithNature_Should_Set_Nature()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 25, Nature.Jolly);

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Jolly));
        }

        [Test]
        public void Create_Should_Assign_Random_Nature()
        {
            PokemonFactory.SetSeed(99999);
            var pokemon1 = PokemonFactory.Create(_pikachu, 25);
            
            PokemonFactory.SetSeed(11111);
            var pokemon2 = PokemonFactory.Create(_pikachu, 25);

            // Different seeds should (likely) give different natures
            // This test might occasionally fail if the same nature is rolled
            // but with deterministic seeds it should be consistent
            Assert.That(pokemon1.Nature, Is.Not.EqualTo(pokemon2.Nature));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Create_Should_Generate_Unique_InstanceIds()
        {
            var pokemon1 = PokemonFactory.Create(_pikachu, 25);
            var pokemon2 = PokemonFactory.Create(_pikachu, 25);

            Assert.That(pokemon1.InstanceId, Is.Not.EqualTo(pokemon2.InstanceId));
        }

        [Test]
        public void Create_Should_Initialize_Battle_State()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 25);

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.None));
            Assert.That(pokemon.VolatileStatus, Is.EqualTo(VolatileStatus.None));
            Assert.That(pokemon.IsFainted, Is.False);
        }

        [Test]
        public void Create_Should_Initialize_Stat_Stages_To_Zero()
        {
            var pokemon = PokemonFactory.Create(_pikachu, 25);

            Assert.That(pokemon.StatStages[Stat.Attack], Is.EqualTo(0));
            Assert.That(pokemon.StatStages[Stat.Speed], Is.EqualTo(0));
        }

        #endregion
    }
}

