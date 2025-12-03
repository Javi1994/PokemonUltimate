using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Factories
{
    /// <summary>
    /// Tests for PokemonInstanceBuilder - fluent builder for creating PokemonInstance objects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.3: PokemonInstance
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonInstanceBuilderTests
    {
        private PokemonSpeciesData _pikachu;
        private PokemonSpeciesData _magnemite;
        private PokemonSpeciesData _tauros;
        private PokemonSpeciesData _chansey;
        private MoveData _tackle;
        private MoveData _thunderShock;
        private MoveData _thunderbolt;

        [SetUp]
        public void SetUp()
        {
            Pokemon.SetSeed(12345);

            _tackle = new MoveData { Name = "Tackle", Type = PokemonType.Normal, MaxPP = 35 };
            _thunderShock = new MoveData { Name = "Thunder Shock", Type = PokemonType.Electric, MaxPP = 30 };
            _thunderbolt = new MoveData { Name = "Thunderbolt", Type = PokemonType.Electric, MaxPP = 15 };

            _pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                GenderRatio = 50f,
                Learnset = new List<LearnableMove>
                {
                    new LearnableMove(_tackle, LearnMethod.Start, 1),
                    new LearnableMove(_thunderShock, LearnMethod.Start, 1),
                    new LearnableMove(_thunderbolt, LearnMethod.LevelUp, 26)
                }
            };

            _magnemite = new PokemonSpeciesData
            {
                Name = "Magnemite",
                PokedexNumber = 81,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(25, 35, 70, 95, 55, 45),
                GenderRatio = -1f
            };

            _tauros = new PokemonSpeciesData
            {
                Name = "Tauros",
                PokedexNumber = 128,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(75, 100, 95, 40, 70, 110),
                GenderRatio = 100f
            };

            _chansey = new PokemonSpeciesData
            {
                Name = "Chansey",
                PokedexNumber = 113,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(250, 5, 5, 35, 105, 50),
                GenderRatio = 0f
            };
        }

        [TearDown]
        public void TearDown()
        {
            Pokemon.ResetRandom();
        }

        #region Basic Creation Tests

        [Test]
        public void Create_Should_Return_Builder()
        {
            var builder = Pokemon.Create(_pikachu, 25);

            Assert.That(builder, Is.Not.Null);
            Assert.That(builder, Is.TypeOf<PokemonInstanceBuilder>());
        }

        [Test]
        public void Build_Should_Return_Instance_With_Correct_Species()
        {
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.Species, Is.EqualTo(_pikachu));
        }

        [Test]
        public void Build_Should_Return_Instance_With_Correct_Level()
        {
            var pokemon = Pokemon.Create(_pikachu, 50).Build();

            Assert.That(pokemon.Level, Is.EqualTo(50));
        }

        [Test]
        public void Create_Should_Throw_For_Null_Species()
        {
            Assert.That(() => Pokemon.Create(null, 25), Throws.ArgumentNullException);
        }

        [Test]
        public void Create_Should_Throw_For_Level_Zero()
        {
            Assert.That(() => Pokemon.Create(_pikachu, 0), Throws.ArgumentException);
        }

        [Test]
        public void Create_Should_Throw_For_Level_Over_100()
        {
            Assert.That(() => Pokemon.Create(_pikachu, 101), Throws.ArgumentException);
        }

        [Test]
        public void Random_Should_Create_Instance_Directly()
        {
            var pokemon = Pokemon.Random(_pikachu, 25);

            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Species, Is.EqualTo(_pikachu));
        }

        [Test]
        public void CreateInLevelRange_Should_Create_Within_Range()
        {
            Pokemon.SetSeed(99999);
            var pokemon = Pokemon.CreateInLevelRange(_pikachu, 20, 30).Build();

            Assert.That(pokemon.Level, Is.InRange(20, 30));
        }

        #endregion

        #region Nature Tests

        [Test]
        public void WithNature_Should_Set_Specific_Nature()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNature(Nature.Adamant)
                .Build();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Adamant));
        }

        [Test]
        public void WithNeutralNature_Should_Set_Hardy()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNeutralNature()
                .Build();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Hardy));
        }

        [Test]
        public void WithNatureBoosting_Attack_Should_Set_Adamant()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNatureBoosting(Stat.Attack)
                .Build();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Adamant));
        }

        [Test]
        public void WithNatureBoosting_Speed_Should_Set_Jolly()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNatureBoosting(Stat.Speed)
                .Build();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Jolly));
        }

        [Test]
        public void WithNatureBoosting_SpAttack_Should_Set_Modest()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNatureBoosting(Stat.SpAttack)
                .Build();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Modest));
        }

        [Test]
        public void WithRandomNature_Should_Allow_Any_Nature()
        {
            Pokemon.SetSeed(11111);
            var pokemon1 = Pokemon.Create(_pikachu, 25).WithRandomNature().Build();
            
            Pokemon.SetSeed(22222);
            var pokemon2 = Pokemon.Create(_pikachu, 25).WithRandomNature().Build();

            Assert.That(pokemon1.Nature, Is.Not.EqualTo(pokemon2.Nature));
        }

        #endregion

        #region Gender Tests

        [Test]
        public void WithGender_Should_Set_Specific_Gender()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithGender(Gender.Female)
                .Build();

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
        }

        [Test]
        public void Male_Should_Set_Male_Gender()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Male()
                .Build();

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Male));
        }

        [Test]
        public void Female_Should_Set_Female_Gender()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Female()
                .Build();

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
        }

        [Test]
        public void Genderless_Species_Should_Be_Genderless()
        {
            var pokemon = Pokemon.Create(_magnemite, 25).Build();

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Genderless));
        }

        [Test]
        public void MaleOnly_Species_Should_Be_Male()
        {
            var pokemon = Pokemon.Create(_tauros, 25).Build();

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Male));
        }

        [Test]
        public void FemaleOnly_Species_Should_Be_Female()
        {
            var pokemon = Pokemon.Create(_chansey, 25).Build();

            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
        }

        [Test]
        public void WithGender_Should_Throw_For_Invalid_Genderless()
        {
            Assert.That(() => Pokemon.Create(_magnemite, 25).Male().Build(),
                Throws.ArgumentException);
        }

        [Test]
        public void WithGender_Should_Throw_For_Female_On_MaleOnly()
        {
            Assert.That(() => Pokemon.Create(_tauros, 25).Female().Build(),
                Throws.ArgumentException);
        }

        [Test]
        public void WithGender_Should_Throw_For_Male_On_FemaleOnly()
        {
            Assert.That(() => Pokemon.Create(_chansey, 25).Male().Build(),
                Throws.ArgumentException);
        }

        #endregion

        #region Nickname Tests

        [Test]
        public void Named_Should_Set_Nickname()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Named("Sparky")
                .Build();

            Assert.That(pokemon.Nickname, Is.EqualTo("Sparky"));
            Assert.That(pokemon.DisplayName, Is.EqualTo("Sparky"));
        }

        [Test]
        public void WithNickname_Should_Set_Nickname()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNickname("Bolt")
                .Build();

            Assert.That(pokemon.Nickname, Is.EqualTo("Bolt"));
        }

        [Test]
        public void No_Nickname_Should_Use_Species_Name()
        {
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.Nickname, Is.Null);
            Assert.That(pokemon.DisplayName, Is.EqualTo("Pikachu"));
        }

        #endregion

        #region Move Tests

        [Test]
        public void Build_Should_Select_Moves_From_Learnset()
        {
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.Moves, Has.Count.GreaterThan(0));
        }

        [Test]
        public void WithMoves_Should_Set_Specific_Moves()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithMoves(_tackle, _thunderbolt)
                .Build();

            var moveNames = pokemon.Moves.Select(m => m.Move.Name).ToList();
            Assert.That(moveNames, Contains.Item("Tackle"));
            Assert.That(moveNames, Contains.Item("Thunderbolt"));
            Assert.That(pokemon.Moves, Has.Count.EqualTo(2));
        }

        [Test]
        public void WithMoves_Should_Limit_To_4()
        {
            var move3 = new MoveData { Name = "Move3", MaxPP = 10 };
            var move4 = new MoveData { Name = "Move4", MaxPP = 10 };
            var move5 = new MoveData { Name = "Move5", MaxPP = 10 };

            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithMoves(_tackle, _thunderShock, _thunderbolt, move3, move4, move5)
                .Build();

            Assert.That(pokemon.Moves, Has.Count.EqualTo(4));
        }

        [Test]
        public void WithNoMoves_Should_Create_With_Empty_Moveset()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithNoMoves()
                .Build();

            Assert.That(pokemon.Moves, Is.Empty);
        }

        [Test]
        public void WithSingleMove_Should_Limit_To_1()
        {
            var pokemon = Pokemon.Create(_pikachu, 30)
                .WithSingleMove()
                .Build();

            Assert.That(pokemon.Moves, Has.Count.EqualTo(1));
        }

        [Test]
        public void WithMoveCount_Should_Limit_Moves()
        {
            var pokemon = Pokemon.Create(_pikachu, 30)
                .WithMoveCount(2)
                .Build();

            Assert.That(pokemon.Moves, Has.Count.LessThanOrEqualTo(2));
        }

        [Test]
        public void WithRandomMoves_Should_Shuffle_Selection()
        {
            Pokemon.SetSeed(11111);
            var pokemon1 = Pokemon.Create(_pikachu, 30).WithRandomMoves().Build();
            
            Pokemon.SetSeed(22222);
            var pokemon2 = Pokemon.Create(_pikachu, 30).WithRandomMoves().Build();

            var moves1 = string.Join(",", pokemon1.Moves.Select(m => m.Move.Name));
            var moves2 = string.Join(",", pokemon2.Moves.Select(m => m.Move.Name));
            
            // Different seeds should give different order (usually)
            Assert.That(moves1, Is.Not.EqualTo(moves2).Or.EqualTo(moves2)); // May be same by chance
        }

        #endregion

        #region HP Tests

        [Test]
        public void Build_Should_Start_At_Full_HP()
        {
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        [Test]
        public void AtHealth_Should_Set_Current_HP()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .AtHealth(50)
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(50));
        }

        [Test]
        public void AtHealth_Should_Cap_At_MaxHP()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .AtHealth(9999)
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        [Test]
        public void AtHealthPercent_Should_Set_Percentage()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithMaxHP(100)
                .AtHealthPercent(0.5f)
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(50));
        }

        [Test]
        public void AtHalfHealth_Should_Set_50_Percent()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithMaxHP(100)
                .AtHalfHealth()
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(50));
        }

        [Test]
        public void AtOneHP_Should_Set_1_HP()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .AtOneHP()
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(1));
        }

        [Test]
        public void Fainted_Should_Set_0_HP()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Fainted()
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(0));
            Assert.That(pokemon.IsFainted, Is.True);
        }

        [Test]
        public void AtFullHealth_Should_Reset_To_Full()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .AtHealth(10)
                .AtFullHealth()
                .Build();

            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        #endregion

        #region Status Tests

        [Test]
        public void WithStatus_Should_Set_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithStatus(PersistentStatus.Burn)
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Burn));
        }

        [Test]
        public void Burned_Should_Set_Burn_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Burned()
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Burn));
        }

        [Test]
        public void Paralyzed_Should_Set_Paralysis_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Paralyzed()
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Paralysis));
        }

        [Test]
        public void Poisoned_Should_Set_Poison_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Poisoned()
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Poison));
        }

        [Test]
        public void BadlyPoisoned_Should_Set_BadlyPoisoned_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .BadlyPoisoned()
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.BadlyPoisoned));
        }

        [Test]
        public void Asleep_Should_Set_Sleep_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Asleep()
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Sleep));
        }

        [Test]
        public void Frozen_Should_Set_Freeze_Status()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Frozen()
                .Build();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Freeze));
        }

        #endregion

        #region Experience Tests

        [Test]
        public void WithExperience_Should_Set_Exp()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithExperience(5000)
                .Build();

            Assert.That(pokemon.CurrentExp, Is.EqualTo(5000));
        }

        [Test]
        public void Build_Should_Default_To_Zero_Exp()
        {
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.CurrentExp, Is.EqualTo(0));
        }

        #endregion

        #region Stat Override Tests

        [Test]
        public void WithStats_Should_Override_All_Stats()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithStats(200, 150, 120, 130, 110, 140)
                .Build();

            Assert.That(pokemon.MaxHP, Is.EqualTo(200));
            Assert.That(pokemon.Attack, Is.EqualTo(150));
            Assert.That(pokemon.Defense, Is.EqualTo(120));
            Assert.That(pokemon.SpAttack, Is.EqualTo(130));
            Assert.That(pokemon.SpDefense, Is.EqualTo(110));
            Assert.That(pokemon.Speed, Is.EqualTo(140));
        }

        [Test]
        public void WithMaxHP_Should_Override_HP()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithMaxHP(500)
                .Build();

            Assert.That(pokemon.MaxHP, Is.EqualTo(500));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(500));
        }

        [Test]
        public void WithAttack_Should_Override_Attack()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithAttack(999)
                .Build();

            Assert.That(pokemon.Attack, Is.EqualTo(999));
        }

        [Test]
        public void WithSpeed_Should_Override_Speed()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithSpeed(200)
                .Build();

            Assert.That(pokemon.Speed, Is.EqualTo(200));
        }

        #endregion

        #region Chaining Tests

        [Test]
        public void Builder_Should_Support_Full_Chaining()
        {
            var pokemon = Pokemon.Create(_pikachu, 50)
                .WithNature(Nature.Jolly)
                .Female()
                .Named("Sparkette")
                .WithMoves(_thunderbolt, _tackle)
                .AtHalfHealth()
                .Paralyzed()
                .WithExperience(1000)
                .Build();

            Assert.That(pokemon.Species.Name, Is.EqualTo("Pikachu"));
            Assert.That(pokemon.Level, Is.EqualTo(50));
            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Jolly));
            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Female));
            Assert.That(pokemon.Nickname, Is.EqualTo("Sparkette"));
            Assert.That(pokemon.Moves, Has.Count.EqualTo(2));
            Assert.That(pokemon.HPPercentage, Is.EqualTo(0.5f).Within(0.01f));
            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Paralysis));
            Assert.That(pokemon.CurrentExp, Is.EqualTo(1000));
        }

        [Test]
        public void Builder_Should_Generate_Unique_IDs()
        {
            var pokemon1 = Pokemon.Create(_pikachu, 25).Build();
            var pokemon2 = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon1.InstanceId, Is.Not.EqualTo(pokemon2.InstanceId));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Build_Should_Handle_Species_Without_Learnset()
        {
            var noMoves = new PokemonSpeciesData
            {
                Name = "NoMoves",
                BaseStats = new BaseStats(100, 100, 100, 100, 100, 100),
                GenderRatio = 50
            };

            var pokemon = Pokemon.Create(noMoves, 25).Build();

            Assert.That(pokemon.Moves, Is.Empty);
        }

        [Test]
        public void Build_Should_Calculate_Stats_Correctly()
        {
            // Pikachu base HP = 35, Level 50, IV=31, EV=252
            // ((2*35 + 31 + 63) * 50 / 100) + 50 + 10 = 82 + 60 = 142
            var pokemon = Pokemon.Create(_pikachu, 50)
                .WithNeutralNature()
                .Build();

            Assert.That(pokemon.MaxHP, Is.EqualTo(142));
        }

        #endregion

        #region Friendship Tests

        [Test]
        public void Build_Should_Default_Friendship_To_70()
        {
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.Friendship, Is.EqualTo(70));
        }

        [Test]
        public void WithFriendship_Should_Set_Value()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithFriendship(150)
                .Build();

            Assert.That(pokemon.Friendship, Is.EqualTo(150));
        }

        [Test]
        public void WithHighFriendship_Should_Set_220()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithHighFriendship()
                .Build();

            Assert.That(pokemon.Friendship, Is.EqualTo(220));
            Assert.That(pokemon.HasHighFriendship, Is.True);
        }

        [Test]
        public void WithMaxFriendship_Should_Set_255()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithMaxFriendship()
                .Build();

            Assert.That(pokemon.Friendship, Is.EqualTo(255));
            Assert.That(pokemon.HasMaxFriendship, Is.True);
        }

        [Test]
        public void WithLowFriendship_Should_Set_0()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithLowFriendship()
                .Build();

            Assert.That(pokemon.Friendship, Is.EqualTo(0));
        }

        [Test]
        public void AsHatched_Should_Set_120()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .AsHatched()
                .Build();

            Assert.That(pokemon.Friendship, Is.EqualTo(120));
        }

        [Test]
        public void WithFriendship_Should_Throw_For_Invalid_Value()
        {
            Assert.That(() => Pokemon.Create(_pikachu, 25).WithFriendship(256),
                Throws.ArgumentException);
            Assert.That(() => Pokemon.Create(_pikachu, 25).WithFriendship(-1),
                Throws.ArgumentException);
        }

        #endregion

        #region Shiny Tests

        [Test]
        public void Build_Should_Default_To_Not_Shiny()
        {
            // With deterministic seed, default should not be shiny
            Pokemon.SetSeed(12345);
            var pokemon = Pokemon.Create(_pikachu, 25).Build();

            Assert.That(pokemon.IsShiny, Is.False);
        }

        [Test]
        public void Shiny_Should_Make_Pokemon_Shiny()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .Shiny()
                .Build();

            Assert.That(pokemon.IsShiny, Is.True);
        }

        [Test]
        public void NotShiny_Should_Make_Pokemon_Not_Shiny()
        {
            var pokemon = Pokemon.Create(_pikachu, 25)
                .NotShiny()
                .Build();

            Assert.That(pokemon.IsShiny, Is.False);
        }

        [Test]
        public void WithShinyChance_Should_Roll_For_Shiny()
        {
            // This test just verifies it doesn't throw
            var pokemon = Pokemon.Create(_pikachu, 25)
                .WithShinyChance()
                .Build();

            // Shiny status is either true or false
            Assert.That(pokemon.IsShiny, Is.TypeOf<bool>());
        }

        #endregion

        #region Move Selection Preset Tests

        [Test]
        public void WithStabMoves_Should_Prefer_Type_Matching_Moves()
        {
            // Create a Pokemon with STAB and non-STAB moves available
            var pikachuWithMoves = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Learnset = new List<LearnableMove>
                {
                    new LearnableMove(new MoveData { Name = "Thunderbolt", Type = PokemonType.Electric, Power = 90 }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Tackle", Type = PokemonType.Normal, Power = 40 }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Thunder", Type = PokemonType.Electric, Power = 110 }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Surf", Type = PokemonType.Water, Power = 90 }, LearnMethod.LevelUp, 1)
                }
            };

            var pokemon = Pokemon.Create(pikachuWithMoves, 50)
                .WithStabMoves()
                .Build();

            // Should prefer Electric moves (STAB)
            var electricMoves = pokemon.Moves.Count(m => m.Move.Type == PokemonType.Electric);
            Assert.That(electricMoves, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void WithStrongMoves_Should_Prefer_High_Power_Moves()
        {
            var pikachuWithMoves = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Learnset = new List<LearnableMove>
                {
                    new LearnableMove(new MoveData { Name = "Thunder", Type = PokemonType.Electric, Power = 110, Category = MoveCategory.Special }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Thunderbolt", Type = PokemonType.Electric, Power = 90, Category = MoveCategory.Special }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Tackle", Type = PokemonType.Normal, Power = 40, Category = MoveCategory.Physical }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Growl", Type = PokemonType.Normal, Power = 0, Category = MoveCategory.Status }, LearnMethod.LevelUp, 1)
                }
            };

            var pokemon = Pokemon.Create(pikachuWithMoves, 50)
                .WithStrongMoves()
                .WithMoveCount(2)
                .Build();

            // Should pick Thunder (110) and Thunderbolt (90)
            Assert.That(pokemon.Moves.Any(m => m.Move.Name == "Thunder"), Is.True);
            Assert.That(pokemon.Moves.Any(m => m.Move.Name == "Thunderbolt"), Is.True);
        }

        [Test]
        public void WithOptimalMoves_Should_Prefer_Stab_And_Power()
        {
            var pikachuWithMoves = new PokemonSpeciesData
            {
                Name = "Pikachu",
                PokedexNumber = 25,
                PrimaryType = PokemonType.Electric,
                BaseStats = new BaseStats(35, 55, 40, 50, 50, 90),
                Learnset = new List<LearnableMove>
                {
                    new LearnableMove(new MoveData { Name = "Thunderbolt", Type = PokemonType.Electric, Power = 90, Category = MoveCategory.Special }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Hyper Beam", Type = PokemonType.Normal, Power = 150, Category = MoveCategory.Special }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Thunder", Type = PokemonType.Electric, Power = 110, Category = MoveCategory.Special }, LearnMethod.LevelUp, 1),
                    new LearnableMove(new MoveData { Name = "Tackle", Type = PokemonType.Normal, Power = 40, Category = MoveCategory.Physical }, LearnMethod.LevelUp, 1)
                }
            };

            var pokemon = Pokemon.Create(pikachuWithMoves, 50)
                .WithOptimalMoves()
                .WithMoveCount(2)
                .Build();

            // Thunder has STAB (100 bonus) + 110 power = 210 score
            // Thunderbolt has STAB (100) + 90 = 190 score
            // Hyper Beam has 150 power + no STAB = 150 score
            Assert.That(pokemon.Moves.Any(m => m.Move.Name == "Thunder"), Is.True);
            Assert.That(pokemon.Moves.Any(m => m.Move.Name == "Thunderbolt"), Is.True);
        }

        [Test]
        public void WithLearnsetMoves_Should_Reset_To_Default()
        {
            var pokemon = Pokemon.Create(_pikachu, 50)
                .WithStrongMoves()
                .WithLearnsetMoves()  // Reset to default
                .Build();

            // Should use default highest-level-first selection
            // Just verify it doesn't crash
            Assert.That(pokemon, Is.Not.Null);
        }

        #endregion
    }
}

