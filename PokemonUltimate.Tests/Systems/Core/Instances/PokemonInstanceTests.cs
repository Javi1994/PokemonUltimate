using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using Evolution = PokemonUltimate.Core.Evolution.Evolution;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    [TestFixture]
    public class PokemonInstanceTests
    {
        private PokemonSpeciesData _testSpecies;
        private List<MoveInstance> _testMoves;

        [SetUp]
        public void SetUp()
        {
            _testSpecies = new PokemonSpeciesData
            {
                Name = "TestMon",
                PokedexNumber = 1,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(100, 100, 100, 100, 100, 100)
            };

            var testMove = new MoveData
            {
                Name = "Test Move",
                Type = PokemonType.Normal,
                MaxPP = 10
            };
            _testMoves = new List<MoveInstance> { new MoveInstance(testMove) };
        }

        private PokemonInstance CreateTestInstance(
            int level = 50,
            int maxHP = 200,
            int attack = 100,
            int defense = 100,
            int spAttack = 100,
            int spDefense = 100,
            int speed = 100,
            Nature nature = Nature.Hardy,
            Gender gender = Gender.Male)
        {
            return new PokemonInstance(
                _testSpecies, level, maxHP, attack, defense, 
                spAttack, spDefense, speed, nature, gender, _testMoves);
        }

        #region Identity Tests

        [Test]
        public void Constructor_Should_Set_Species()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.Species, Is.EqualTo(_testSpecies));
        }

        [Test]
        public void Constructor_Should_Generate_InstanceId()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.InstanceId, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void InstanceId_Should_Be_Unique()
        {
            var instance1 = CreateTestInstance();
            var instance2 = CreateTestInstance();

            Assert.That(instance1.InstanceId, Is.Not.EqualTo(instance2.InstanceId));
        }

        [Test]
        public void DisplayName_Should_Return_Species_Name_When_No_Nickname()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.DisplayName, Is.EqualTo("TestMon"));
        }

        [Test]
        public void DisplayName_Should_Return_Nickname_When_Set()
        {
            var instance = CreateTestInstance();
            instance.Nickname = "Sparky";

            Assert.That(instance.DisplayName, Is.EqualTo("Sparky"));
        }

        #endregion

        #region Stats Tests

        [Test]
        public void Constructor_Should_Set_Stats()
        {
            var instance = CreateTestInstance(
                maxHP: 200, attack: 150, defense: 120,
                spAttack: 130, spDefense: 110, speed: 140);

            Assert.That(instance.MaxHP, Is.EqualTo(200));
            Assert.That(instance.Attack, Is.EqualTo(150));
            Assert.That(instance.Defense, Is.EqualTo(120));
            Assert.That(instance.SpAttack, Is.EqualTo(130));
            Assert.That(instance.SpDefense, Is.EqualTo(110));
            Assert.That(instance.Speed, Is.EqualTo(140));
        }

        [Test]
        public void Constructor_Should_Set_CurrentHP_To_MaxHP()
        {
            var instance = CreateTestInstance(maxHP: 200);

            Assert.That(instance.CurrentHP, Is.EqualTo(200));
        }

        [Test]
        public void Constructor_Should_Set_Level()
        {
            var instance = CreateTestInstance(level: 75);

            Assert.That(instance.Level, Is.EqualTo(75));
        }

        #endregion

        #region Personal Characteristics Tests

        [Test]
        public void Constructor_Should_Set_Nature()
        {
            var instance = CreateTestInstance(nature: Nature.Adamant);

            Assert.That(instance.Nature, Is.EqualTo(Nature.Adamant));
        }

        [Test]
        public void Constructor_Should_Set_Gender()
        {
            var instance = CreateTestInstance(gender: Gender.Female);

            Assert.That(instance.Gender, Is.EqualTo(Gender.Female));
        }

        #endregion

        #region IsFainted Tests

        [Test]
        public void IsFainted_Should_Return_False_When_HP_Above_Zero()
        {
            var instance = CreateTestInstance(maxHP: 100);

            Assert.That(instance.IsFainted, Is.False);
        }

        [Test]
        public void IsFainted_Should_Return_True_When_HP_Zero()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 0;

            Assert.That(instance.IsFainted, Is.True);
        }

        [Test]
        public void IsFainted_Should_Return_True_When_HP_Negative()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = -10;

            Assert.That(instance.IsFainted, Is.True);
        }

        #endregion

        #region HPPercentage Tests

        [Test]
        public void HPPercentage_Should_Return_1_When_Full()
        {
            var instance = CreateTestInstance(maxHP: 100);

            Assert.That(instance.HPPercentage, Is.EqualTo(1f));
        }

        [Test]
        public void HPPercentage_Should_Return_0_5_When_Half()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 50;

            Assert.That(instance.HPPercentage, Is.EqualTo(0.5f));
        }

        [Test]
        public void HPPercentage_Should_Return_0_When_Fainted()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 0;

            Assert.That(instance.HPPercentage, Is.EqualTo(0f));
        }

        #endregion

        #region Status Tests

        [Test]
        public void HasStatus_Should_Return_False_Initially()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.HasStatus, Is.False);
        }

        [Test]
        public void HasStatus_Should_Return_True_When_Status_Set()
        {
            var instance = CreateTestInstance();
            instance.Status = PersistentStatus.Burn;

            Assert.That(instance.HasStatus, Is.True);
        }

        [Test]
        public void Status_Should_Be_None_Initially()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.Status, Is.EqualTo(PersistentStatus.None));
        }

        [Test]
        public void VolatileStatus_Should_Be_None_Initially()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        #endregion

        #region Stat Stages Tests

        [Test]
        public void StatStages_Should_Be_Zero_Initially()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.StatStages[Stat.Attack], Is.EqualTo(0));
            Assert.That(instance.StatStages[Stat.Defense], Is.EqualTo(0));
            Assert.That(instance.StatStages[Stat.Speed], Is.EqualTo(0));
        }

        [Test]
        public void ModifyStatStage_Should_Change_Stage()
        {
            var instance = CreateTestInstance();

            instance.ModifyStatStage(Stat.Attack, 2);

            Assert.That(instance.StatStages[Stat.Attack], Is.EqualTo(2));
        }

        [Test]
        public void ModifyStatStage_Should_Return_Actual_Change()
        {
            var instance = CreateTestInstance();

            int change = instance.ModifyStatStage(Stat.Attack, 2);

            Assert.That(change, Is.EqualTo(2));
        }

        [Test]
        public void ModifyStatStage_Should_Clamp_At_Plus6()
        {
            var instance = CreateTestInstance();
            instance.ModifyStatStage(Stat.Attack, 6);

            int change = instance.ModifyStatStage(Stat.Attack, 2);

            Assert.That(instance.StatStages[Stat.Attack], Is.EqualTo(6));
            Assert.That(change, Is.EqualTo(0));
        }

        [Test]
        public void ModifyStatStage_Should_Clamp_At_Minus6()
        {
            var instance = CreateTestInstance();
            instance.ModifyStatStage(Stat.Defense, -6);

            int change = instance.ModifyStatStage(Stat.Defense, -2);

            Assert.That(instance.StatStages[Stat.Defense], Is.EqualTo(-6));
            Assert.That(change, Is.EqualTo(0));
        }

        [Test]
        public void GetEffectiveStat_Should_Apply_Stage_Multiplier()
        {
            var instance = CreateTestInstance(attack: 100);
            instance.ModifyStatStage(Stat.Attack, 2);

            int effective = instance.GetEffectiveStat(Stat.Attack);

            Assert.That(effective, Is.EqualTo(200)); // +2 = x2
        }

        #endregion

        #region TakeDamage Tests

        [Test]
        public void TakeDamage_Should_Reduce_HP()
        {
            var instance = CreateTestInstance(maxHP: 100);

            instance.TakeDamage(30);

            Assert.That(instance.CurrentHP, Is.EqualTo(70));
        }

        [Test]
        public void TakeDamage_Should_Return_Actual_Damage()
        {
            var instance = CreateTestInstance(maxHP: 100);

            int damage = instance.TakeDamage(30);

            Assert.That(damage, Is.EqualTo(30));
        }

        [Test]
        public void TakeDamage_Should_Not_Go_Below_Zero()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 20;

            instance.TakeDamage(50);

            Assert.That(instance.CurrentHP, Is.EqualTo(0));
        }

        [Test]
        public void TakeDamage_Should_Return_Capped_Damage_When_Overkill()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 20;

            int damage = instance.TakeDamage(50);

            Assert.That(damage, Is.EqualTo(20));
        }

        [Test]
        public void TakeDamage_Should_Throw_For_Negative_Amount()
        {
            var instance = CreateTestInstance();

            Assert.That(() => instance.TakeDamage(-10), Throws.ArgumentException);
        }

        #endregion

        #region Heal Tests

        [Test]
        public void Heal_Should_Increase_HP()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 50;

            instance.Heal(30);

            Assert.That(instance.CurrentHP, Is.EqualTo(80));
        }

        [Test]
        public void Heal_Should_Return_Actual_Heal()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 50;

            int healed = instance.Heal(30);

            Assert.That(healed, Is.EqualTo(30));
        }

        [Test]
        public void Heal_Should_Not_Exceed_MaxHP()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 80;

            instance.Heal(50);

            Assert.That(instance.CurrentHP, Is.EqualTo(100));
        }

        [Test]
        public void Heal_Should_Return_Capped_Heal_When_Overheal()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 80;

            int healed = instance.Heal(50);

            Assert.That(healed, Is.EqualTo(20));
        }

        [Test]
        public void Heal_Should_Throw_For_Negative_Amount()
        {
            var instance = CreateTestInstance();

            Assert.That(() => instance.Heal(-10), Throws.ArgumentException);
        }

        #endregion

        #region ResetBattleState Tests

        [Test]
        public void ResetBattleState_Should_Clear_StatStages()
        {
            var instance = CreateTestInstance();
            instance.ModifyStatStage(Stat.Attack, 3);
            instance.ModifyStatStage(Stat.Defense, -2);

            instance.ResetBattleState();

            Assert.That(instance.StatStages[Stat.Attack], Is.EqualTo(0));
            Assert.That(instance.StatStages[Stat.Defense], Is.EqualTo(0));
        }

        [Test]
        public void ResetBattleState_Should_Clear_VolatileStatus()
        {
            var instance = CreateTestInstance();
            instance.VolatileStatus = VolatileStatus.Confusion;

            instance.ResetBattleState();

            Assert.That(instance.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public void ResetBattleState_Should_Not_Clear_PersistentStatus()
        {
            var instance = CreateTestInstance();
            instance.Status = PersistentStatus.Burn;

            instance.ResetBattleState();

            Assert.That(instance.Status, Is.EqualTo(PersistentStatus.Burn));
        }

        #endregion

        #region FullHeal Tests

        [Test]
        public void FullHeal_Should_Restore_HP()
        {
            var instance = CreateTestInstance(maxHP: 100);
            instance.CurrentHP = 20;

            instance.FullHeal();

            Assert.That(instance.CurrentHP, Is.EqualTo(100));
        }

        [Test]
        public void FullHeal_Should_Clear_PersistentStatus()
        {
            var instance = CreateTestInstance();
            instance.Status = PersistentStatus.Poison;

            instance.FullHeal();

            Assert.That(instance.Status, Is.EqualTo(PersistentStatus.None));
        }

        [Test]
        public void FullHeal_Should_Clear_VolatileStatus()
        {
            var instance = CreateTestInstance();
            instance.VolatileStatus = VolatileStatus.Confusion;

            instance.FullHeal();

            Assert.That(instance.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        [Test]
        public void FullHeal_Should_Restore_Move_PP()
        {
            var instance = CreateTestInstance();
            instance.Moves[0].Use();
            instance.Moves[0].Use();

            instance.FullHeal();

            Assert.That(instance.Moves[0].CurrentPP, Is.EqualTo(instance.Moves[0].MaxPP));
        }

        #endregion

        #region Moves Tests

        [Test]
        public void Constructor_Should_Set_Moves()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.Moves, Has.Count.EqualTo(1));
            Assert.That(instance.Moves[0].Move.Name, Is.EqualTo("Test Move"));
        }

        #endregion

        #region Friendship Tests

        [Test]
        public void Friendship_Should_Default_To_70()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.Friendship, Is.EqualTo(70));
        }

        [Test]
        public void HasHighFriendship_Should_Return_True_When_220_Or_Above()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 220;

            Assert.That(instance.HasHighFriendship, Is.True);
        }

        [Test]
        public void HasHighFriendship_Should_Return_False_When_Below_220()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 219;

            Assert.That(instance.HasHighFriendship, Is.False);
        }

        [Test]
        public void HasMaxFriendship_Should_Return_True_At_255()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 255;

            Assert.That(instance.HasMaxFriendship, Is.True);
        }

        [Test]
        public void IncreaseFriendship_Should_Increase_Value()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 100;

            instance.IncreaseFriendship(20);

            Assert.That(instance.Friendship, Is.EqualTo(120));
        }

        [Test]
        public void IncreaseFriendship_Should_Cap_At_255()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 250;

            instance.IncreaseFriendship(20);

            Assert.That(instance.Friendship, Is.EqualTo(255));
        }

        [Test]
        public void IncreaseFriendship_Should_Return_Actual_Change()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 250;

            int change = instance.IncreaseFriendship(20);

            Assert.That(change, Is.EqualTo(5));
        }

        [Test]
        public void DecreaseFriendship_Should_Decrease_Value()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 100;

            instance.DecreaseFriendship(20);

            Assert.That(instance.Friendship, Is.EqualTo(80));
        }

        [Test]
        public void DecreaseFriendship_Should_Not_Go_Below_Zero()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 10;

            instance.DecreaseFriendship(20);

            Assert.That(instance.Friendship, Is.EqualTo(0));
        }

        [Test]
        public void DecreaseFriendship_Should_Return_Actual_Change()
        {
            var instance = CreateTestInstance();
            instance.Friendship = 10;

            int change = instance.DecreaseFriendship(20);

            Assert.That(change, Is.EqualTo(10));
        }

        #endregion

        #region Shiny Tests

        [Test]
        public void IsShiny_Should_Default_To_False()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.IsShiny, Is.False);
        }

        #endregion

        #region Level Up Tests

        [Test]
        public void CanLevelUp_Should_Return_False_When_Not_Enough_Exp()
        {
            var instance = CreateTestInstance(level: 10);
            instance.CurrentExp = 0;

            Assert.That(instance.CanLevelUp(), Is.False);
        }

        [Test]
        public void CanLevelUp_Should_Return_True_When_Enough_Exp()
        {
            var instance = CreateTestInstance(level: 10);
            instance.CurrentExp = 11 * 11 * 11; // Exp for level 11

            Assert.That(instance.CanLevelUp(), Is.True);
        }

        [Test]
        public void CanLevelUp_Should_Return_False_At_Level_100()
        {
            var instance = CreateTestInstance(level: 100);
            instance.CurrentExp = 999999999;

            Assert.That(instance.CanLevelUp(), Is.False);
        }

        [Test]
        public void GetExpForNextLevel_Should_Return_Correct_Value()
        {
            var instance = CreateTestInstance(level: 10);
            
            // Level 11 = 11^3 = 1331
            Assert.That(instance.GetExpForNextLevel(), Is.EqualTo(1331));
        }

        [Test]
        public void GetExpToNextLevel_Should_Return_Remaining()
        {
            var instance = CreateTestInstance(level: 10);
            instance.CurrentExp = 1000;

            // Need 1331 for level 11, have 1000, need 331 more
            Assert.That(instance.GetExpToNextLevel(), Is.EqualTo(331));
        }

        [Test]
        public void LevelUp_Should_Increase_Level()
        {
            var instance = CreateTestInstance(level: 10);

            instance.LevelUp();

            Assert.That(instance.Level, Is.EqualTo(11));
        }

        [Test]
        public void LevelUp_Should_Increase_Stats()
        {
            // Create with level 10 and correct stats for that level
            int level10HP = StatCalculator.CalculateHP(100, 10);
            var instance = CreateTestInstance(level: 10, maxHP: level10HP);
            int oldMaxHP = instance.MaxHP;

            instance.LevelUp();

            // After level up, RecalculateStats will use Species.BaseStats (100) at level 11
            int expectedHP = StatCalculator.CalculateHP(100, 11);
            Assert.That(instance.MaxHP, Is.EqualTo(expectedHP));
            Assert.That(instance.MaxHP, Is.GreaterThan(oldMaxHP));
        }

        [Test]
        public void LevelUp_Should_Increase_Current_HP_Proportionally()
        {
            int level10HP = StatCalculator.CalculateHP(100, 10);
            var instance = CreateTestInstance(level: 10, maxHP: level10HP);
            instance.CurrentHP = level10HP;

            instance.LevelUp();

            int level11HP = StatCalculator.CalculateHP(100, 11);
            int hpGain = level11HP - level10HP;
            Assert.That(instance.CurrentHP, Is.EqualTo(level10HP + hpGain));
        }

        [Test]
        public void LevelUp_Should_Return_False_At_Max_Level()
        {
            var instance = CreateTestInstance(level: 100);

            bool result = instance.LevelUp();

            Assert.That(result, Is.False);
            Assert.That(instance.Level, Is.EqualTo(100));
        }

        [Test]
        public void AddExperience_Should_Level_Up_When_Enough()
        {
            var instance = CreateTestInstance(level: 1);
            instance.CurrentExp = 0;

            // Add enough for level 2 (8 exp)
            int levelsGained = instance.AddExperience(10);

            Assert.That(instance.Level, Is.EqualTo(2));
            Assert.That(levelsGained, Is.EqualTo(1));
        }

        [Test]
        public void AddExperience_Should_Return_Multiple_Levels()
        {
            var instance = CreateTestInstance(level: 1);
            instance.CurrentExp = 0;

            // Add enough for level 5 (125 exp)
            int levelsGained = instance.AddExperience(200);

            Assert.That(instance.Level, Is.GreaterThanOrEqualTo(5));
            Assert.That(levelsGained, Is.GreaterThanOrEqualTo(4));
        }

        [Test]
        public void LevelUpTo_Should_Reach_Target_Level()
        {
            var instance = CreateTestInstance(level: 10);

            instance.LevelUpTo(50);

            Assert.That(instance.Level, Is.EqualTo(50));
        }

        [Test]
        public void LevelUpTo_Should_Return_Levels_Gained()
        {
            var instance = CreateTestInstance(level: 10);

            int gained = instance.LevelUpTo(15);

            Assert.That(gained, Is.EqualTo(5));
        }

        [Test]
        public void LevelUpTo_Should_Do_Nothing_If_Target_Lower()
        {
            var instance = CreateTestInstance(level: 50);

            int gained = instance.LevelUpTo(10);

            Assert.That(gained, Is.EqualTo(0));
            Assert.That(instance.Level, Is.EqualTo(50));
        }

        #endregion

        #region Move Learning Tests

        [Test]
        public void TryLearnMove_Should_Add_Move()
        {
            var instance = CreateTestInstance();
            instance.Moves.Clear();
            var newMove = new MoveData { Name = "New Move", MaxPP = 10 };

            bool result = instance.TryLearnMove(newMove);

            Assert.That(result, Is.True);
            Assert.That(instance.Moves, Has.Count.EqualTo(1));
        }

        [Test]
        public void TryLearnMove_Should_Fail_When_Full()
        {
            var instance = CreateTestInstance();
            // Add 3 more moves to make 4 total
            for (int i = 0; i < 3; i++)
                instance.TryLearnMove(new MoveData { Name = $"Move{i}", MaxPP = 10 });

            var fifthMove = new MoveData { Name = "Fifth Move", MaxPP = 10 };
            bool result = instance.TryLearnMove(fifthMove);

            Assert.That(result, Is.False);
            Assert.That(instance.Moves, Has.Count.EqualTo(4));
        }

        [Test]
        public void TryLearnMove_Should_Fail_If_Already_Known()
        {
            var instance = CreateTestInstance();
            var existingMove = instance.Moves[0].Move;

            bool result = instance.TryLearnMove(existingMove);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReplaceMove_Should_Replace_At_Index()
        {
            var instance = CreateTestInstance();
            var newMove = new MoveData { Name = "Replacement", MaxPP = 15 };

            bool result = instance.ReplaceMove(0, newMove);

            Assert.That(result, Is.True);
            Assert.That(instance.Moves[0].Move.Name, Is.EqualTo("Replacement"));
        }

        [Test]
        public void ReplaceMove_Should_Fail_For_Invalid_Index()
        {
            var instance = CreateTestInstance();
            var newMove = new MoveData { Name = "Replacement", MaxPP = 15 };

            bool result = instance.ReplaceMove(99, newMove);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ForgetMove_Should_Remove_At_Index()
        {
            var instance = CreateTestInstance();
            int initialCount = instance.Moves.Count;

            bool result = instance.ForgetMove(0);

            Assert.That(result, Is.True);
            Assert.That(instance.Moves, Has.Count.EqualTo(initialCount - 1));
        }

        [Test]
        public void ForgetMove_Should_Fail_For_Invalid_Index()
        {
            var instance = CreateTestInstance();

            bool result = instance.ForgetMove(99);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Evolution Tests

        private PokemonSpeciesData _evolvedSpecies;
        private PokemonUltimate.Core.Evolution.Evolution _levelEvolution;
        private PokemonUltimate.Core.Evolution.Evolution _friendshipEvolution;

        private void SetupEvolutionData()
        {
            _evolvedSpecies = new PokemonSpeciesData
            {
                Name = "EvolvedMon",
                PokedexNumber = 2,
                PrimaryType = PokemonType.Normal,
                BaseStats = new BaseStats(120, 120, 120, 120, 120, 120)
            };

            _levelEvolution = new PokemonUltimate.Core.Evolution.Evolution
            {
                Target = _evolvedSpecies,
                Conditions = new List<PokemonUltimate.Core.Evolution.IEvolutionCondition>
                {
                    new LevelCondition(16)
                }
            };

            _friendshipEvolution = new PokemonUltimate.Core.Evolution.Evolution
            {
                Target = _evolvedSpecies,
                Conditions = new List<PokemonUltimate.Core.Evolution.IEvolutionCondition>
                {
                    new FriendshipCondition(220)
                }
            };
        }

        [Test]
        public void CanEvolve_Should_Return_False_When_No_Evolutions()
        {
            var instance = CreateTestInstance(level: 50);

            Assert.That(instance.CanEvolve(), Is.False);
        }

        [Test]
        public void CanEvolve_Should_Return_False_When_Level_Not_Met()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 10);

            Assert.That(instance.CanEvolve(), Is.False);
        }

        [Test]
        public void CanEvolve_Should_Return_True_When_Level_Met()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 16);

            Assert.That(instance.CanEvolve(), Is.True);
        }

        [Test]
        public void GetAvailableEvolution_Should_Return_Null_When_No_Evolutions()
        {
            var instance = CreateTestInstance();

            Assert.That(instance.GetAvailableEvolution(), Is.Null);
        }

        [Test]
        public void GetAvailableEvolution_Should_Return_Evolution_When_Available()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 20);

            var evolution = instance.GetAvailableEvolution();

            Assert.That(evolution, Is.Not.Null);
            Assert.That(evolution.Target, Is.EqualTo(_evolvedSpecies));
        }

        [Test]
        public void Evolve_Should_Change_Species()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 20);

            bool result = instance.Evolve(_evolvedSpecies);

            Assert.That(result, Is.True);
            Assert.That(instance.Species, Is.EqualTo(_evolvedSpecies));
        }

        [Test]
        public void Evolve_Should_Recalculate_Stats()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            
            // Use stats that match the base stats
            int level20HP = StatCalculator.CalculateHP(100, 20);
            var instance = CreateTestInstance(level: 20, maxHP: level20HP);
            int oldMaxHP = instance.MaxHP;

            instance.Evolve(_evolvedSpecies);

            // Evolved species has higher base stats (120 vs 100)
            int evolvedHP = StatCalculator.CalculateHP(120, 20);
            Assert.That(instance.MaxHP, Is.EqualTo(evolvedHP));
            Assert.That(instance.MaxHP, Is.GreaterThan(oldMaxHP));
        }

        [Test]
        public void Evolve_Should_Maintain_HP_Percentage()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 20);
            instance.CurrentHP = instance.MaxHP / 2; // 50% HP
            int halfHP = instance.CurrentHP;

            instance.Evolve(_evolvedSpecies);

            // Should still be approximately 50% HP
            float hpPercent = (float)instance.CurrentHP / instance.MaxHP;
            Assert.That(hpPercent, Is.EqualTo(0.5f).Within(0.1f));
        }

        [Test]
        public void Evolve_Should_Preserve_Nickname()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 20);
            instance.Nickname = "Sparky";

            instance.Evolve(_evolvedSpecies);

            Assert.That(instance.Nickname, Is.EqualTo("Sparky"));
        }

        [Test]
        public void Evolve_Should_Preserve_Moves()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 20);
            int moveCount = instance.Moves.Count;

            instance.Evolve(_evolvedSpecies);

            Assert.That(instance.Moves, Has.Count.EqualTo(moveCount));
        }

        [Test]
        public void TryEvolve_Should_Return_Target_On_Success()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 20);

            var result = instance.TryEvolve();

            Assert.That(result, Is.EqualTo(_evolvedSpecies));
            Assert.That(instance.Species, Is.EqualTo(_evolvedSpecies));
        }

        [Test]
        public void TryEvolve_Should_Return_Null_On_Failure()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _levelEvolution };
            var instance = CreateTestInstance(level: 10);

            var result = instance.TryEvolve();

            Assert.That(result, Is.Null);
            Assert.That(instance.Species, Is.EqualTo(_testSpecies));
        }

        [Test]
        public void Friendship_Evolution_Should_Work()
        {
            SetupEvolutionData();
            _testSpecies.Evolutions = new List<PokemonUltimate.Core.Evolution.Evolution> { _friendshipEvolution };
            var instance = CreateTestInstance(level: 10);

            // Low friendship - can't evolve
            instance.Friendship = 100;
            Assert.That(instance.CanEvolve(), Is.False);

            // High friendship - can evolve
            instance.Friendship = 220;
            Assert.That(instance.CanEvolve(), Is.True);
        }

        #endregion
    }
}

