using System;
using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Tests for edge cases in Stats and Moves systems.
    /// These are critical systems that must be bulletproof.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.3: PokemonInstance
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class StatsAndMovesEdgeCasesTests
    {
        private PokemonSpeciesData _testSpecies;
        private MoveData _testMove;

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

            _testMove = new MoveData
            {
                Name = "Test Move",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 80,
                Accuracy = 100,
                MaxPP = 15
            };
        }

        #region StatCalculator Edge Cases

        [Test]
        public void CalculateHP_Level1_Returns_Minimum()
        {
            // Level 1 with base 1 HP should still be positive
            int hp = StatCalculator.CalculateHP(1, 1, 0, 0);
            Assert.That(hp, Is.GreaterThan(0));
        }

        [Test]
        public void CalculateHP_Level100_MaxStats()
        {
            // Maximum possible HP calculation
            int hp = StatCalculator.CalculateHP(255, 100, 31, 252);
            Assert.That(hp, Is.GreaterThan(500)); // Very high HP
        }

        [Test]
        public void CalculateHP_ZeroBaseStat_StillPositive()
        {
            // Even with 0 base HP, formula adds level + 10
            int hp = StatCalculator.CalculateHP(0, 50, 0, 0);
            Assert.That(hp, Is.EqualTo(60)); // 50 + 10
        }

        [Test]
        public void CalculateStat_Level1_Returns_Minimum()
        {
            int stat = StatCalculator.CalculateStat(1, 1, Nature.Hardy, Stat.Attack, 0, 0);
            Assert.That(stat, Is.GreaterThan(0));
        }

        [Test]
        public void CalculateStat_Extreme_Nature_Effects()
        {
            int neutral = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack);
            int boosted = StatCalculator.CalculateStat(100, 50, Nature.Adamant, Stat.Attack);
            int reduced = StatCalculator.CalculateStat(100, 50, Nature.Modest, Stat.Attack);

            Assert.That(boosted, Is.GreaterThan(neutral));
            Assert.That(reduced, Is.LessThan(neutral));
            Assert.That((float)boosted / neutral, Is.EqualTo(1.1f).Within(0.01f));
            Assert.That((float)reduced / neutral, Is.EqualTo(0.9f).Within(0.01f));
        }

        [Test]
        public void GetStageMultiplier_Boundary_Values()
        {
            // Test exact boundary values
            Assert.That(StatCalculator.GetStageMultiplier(-6), Is.EqualTo(0.25f));
            Assert.That(StatCalculator.GetStageMultiplier(-5), Is.EqualTo(2f / 7f).Within(0.01f));
            Assert.That(StatCalculator.GetStageMultiplier(0), Is.EqualTo(1f));
            Assert.That(StatCalculator.GetStageMultiplier(5), Is.EqualTo(3.5f));
            Assert.That(StatCalculator.GetStageMultiplier(6), Is.EqualTo(4f));
        }

        [Test]
        public void GetStageMultiplier_OutOfRange_Clamps()
        {
            // Values beyond -6/+6 should clamp
            Assert.That(StatCalculator.GetStageMultiplier(-100), Is.EqualTo(0.25f));
            Assert.That(StatCalculator.GetStageMultiplier(100), Is.EqualTo(4f));
        }

        [Test]
        public void GetAccuracyStageMultiplier_Different_From_Regular()
        {
            // Accuracy uses 3-based formula, not 2-based
            float accMult = StatCalculator.GetAccuracyStageMultiplier(1);
            float statMult = StatCalculator.GetStageMultiplier(1);

            Assert.That(accMult, Is.Not.EqualTo(statMult));
            Assert.That(accMult, Is.EqualTo(4f / 3f).Within(0.01f));
        }

        [Test]
        public void GetEffectiveStat_ZeroStat_Returns_Zero()
        {
            int result = StatCalculator.GetEffectiveStat(0, 6);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetEffectiveStat_MaxStage_Quadruples()
        {
            int result = StatCalculator.GetEffectiveStat(100, 6);
            Assert.That(result, Is.EqualTo(400));
        }

        [Test]
        public void GetEffectiveStat_MinStage_Quarters()
        {
            int result = StatCalculator.GetEffectiveStat(100, -6);
            Assert.That(result, Is.EqualTo(25));
        }

        [Test]
        public void CalculateHP_Negative_BaseStat_Throws()
        {
            Assert.That(() => StatCalculator.CalculateHP(-1, 50), Throws.ArgumentException);
        }

        [Test]
        public void CalculateHP_Invalid_Level_Throws()
        {
            Assert.That(() => StatCalculator.CalculateHP(100, 0), Throws.ArgumentException);
            Assert.That(() => StatCalculator.CalculateHP(100, 101), Throws.ArgumentException);
        }

        [Test]
        public void CalculateHP_Invalid_IV_Throws()
        {
            Assert.That(() => StatCalculator.CalculateHP(100, 50, -1, 0), Throws.ArgumentException);
            Assert.That(() => StatCalculator.CalculateHP(100, 50, 32, 0), Throws.ArgumentException);
        }

        [Test]
        public void CalculateHP_Invalid_EV_Throws()
        {
            Assert.That(() => StatCalculator.CalculateHP(100, 50, 0, -1), Throws.ArgumentException);
            Assert.That(() => StatCalculator.CalculateHP(100, 50, 0, 253), Throws.ArgumentException);
        }

        [Test]
        public void Experience_Level1_Is_1()
        {
            Assert.That(StatCalculator.GetExpForLevel(1), Is.EqualTo(1));
        }

        [Test]
        public void Experience_Level100_Is_Million()
        {
            Assert.That(StatCalculator.GetExpForLevel(100), Is.EqualTo(1000000));
        }

        [Test]
        public void Experience_Growth_Is_Cubic()
        {
            // Medium Fast: Level^3
            for (int level = 1; level <= 10; level++)
            {
                int expected = level * level * level;
                Assert.That(StatCalculator.GetExpForLevel(level), Is.EqualTo(expected));
            }
        }

        [Test]
        public void GetLevelForExp_Returns_Correct_Level()
        {
            // Test various exp values
            Assert.That(StatCalculator.GetLevelForExp(0), Is.EqualTo(1));
            Assert.That(StatCalculator.GetLevelForExp(7), Is.EqualTo(1)); // Just below 8
            Assert.That(StatCalculator.GetLevelForExp(8), Is.EqualTo(2)); // Exactly level 2
            Assert.That(StatCalculator.GetLevelForExp(999999), Is.EqualTo(99)); // Just below 100
            Assert.That(StatCalculator.GetLevelForExp(1000000), Is.EqualTo(100));
            Assert.That(StatCalculator.GetLevelForExp(9999999), Is.EqualTo(100)); // Way over
        }

        #endregion

        #region MoveInstance Edge Cases

        [Test]
        public void MoveInstance_StartsAtFullPP()
        {
            var move = new MoveInstance(_testMove);
            Assert.That(move.CurrentPP, Is.EqualTo(move.MaxPP));
            Assert.That(move.HasPP, Is.True);
        }

        [Test]
        public void MoveInstance_UseDecrementsPP()
        {
            var move = new MoveInstance(_testMove);
            int startPP = move.CurrentPP;

            bool result = move.Use();

            Assert.That(result, Is.True);
            Assert.That(move.CurrentPP, Is.EqualTo(startPP - 1));
        }

        [Test]
        public void MoveInstance_UseAtZeroPP_ReturnsFalse()
        {
            var move = new MoveInstance(_testMove);

            // Use all PP
            while (move.CurrentPP > 0)
                move.Use();

            bool result = move.Use();

            Assert.That(result, Is.False);
            Assert.That(move.CurrentPP, Is.EqualTo(0));
            Assert.That(move.HasPP, Is.False);
        }

        [Test]
        public void MoveInstance_UseAllPP_Tracking()
        {
            var move = new MoveInstance(_testMove);
            int useCount = 0;

            while (move.Use())
                useCount++;

            Assert.That(useCount, Is.EqualTo(_testMove.MaxPP));
            Assert.That(move.CurrentPP, Is.EqualTo(0));
        }

        [Test]
        public void MoveInstance_Restore_CapsAtMax()
        {
            var move = new MoveInstance(_testMove);
            move.Use();
            move.Use();

            move.Restore(999);

            Assert.That(move.CurrentPP, Is.EqualTo(move.MaxPP));
        }

        [Test]
        public void MoveInstance_Restore_PartialAmount()
        {
            var move = new MoveInstance(_testMove);
            for (int i = 0; i < 5; i++) move.Use();

            move.Restore(3);

            Assert.That(move.CurrentPP, Is.EqualTo(move.MaxPP - 2)); // 5 used, 3 restored = 2 down
        }

        [Test]
        public void MoveInstance_Restore_NegativeThrows()
        {
            var move = new MoveInstance(_testMove);

            Assert.That(() => move.Restore(-1), Throws.ArgumentException);
        }

        [Test]
        public void MoveInstance_RestoreFully_FromZero()
        {
            var move = new MoveInstance(_testMove);
            while (move.Use()) { }

            move.RestoreFully();

            Assert.That(move.CurrentPP, Is.EqualTo(move.MaxPP));
        }

        [Test]
        public void MoveInstance_NullMove_Throws()
        {
            Assert.That(() => new MoveInstance(null), Throws.ArgumentNullException);
        }

        [Test]
        public void MoveInstance_ZeroPP_Move()
        {
            var zeroPPMove = new MoveData { Name = "Struggle", MaxPP = 0 };
            var move = new MoveInstance(zeroPPMove);

            Assert.That(move.MaxPP, Is.EqualTo(0));
            Assert.That(move.CurrentPP, Is.EqualTo(0));
            Assert.That(move.HasPP, Is.False);
            Assert.That(move.Use(), Is.False);
        }

        [Test]
        public void MoveInstance_HighPP_Move()
        {
            var highPPMove = new MoveData { Name = "Test", MaxPP = 40 };
            var move = new MoveInstance(highPPMove);

            Assert.That(move.MaxPP, Is.EqualTo(40));
            Assert.That(move.CurrentPP, Is.EqualTo(40));
        }

        #endregion

        #region PokemonInstance Stat Edge Cases

        [Test]
        public void GetEffectiveStat_AllStats()
        {
            var pokemon = CreatePokemon(50);

            // Test all combat stats
            Assert.That(pokemon.GetEffectiveStat(Stat.Attack), Is.GreaterThan(0));
            Assert.That(pokemon.GetEffectiveStat(Stat.Defense), Is.GreaterThan(0));
            Assert.That(pokemon.GetEffectiveStat(Stat.SpAttack), Is.GreaterThan(0));
            Assert.That(pokemon.GetEffectiveStat(Stat.SpDefense), Is.GreaterThan(0));
            Assert.That(pokemon.GetEffectiveStat(Stat.Speed), Is.GreaterThan(0));
        }

        [Test]
        public void GetEffectiveStat_AccuracyEvasion_Base100()
        {
            var pokemon = CreatePokemon(50);

            Assert.That(pokemon.GetEffectiveStat(Stat.Accuracy), Is.EqualTo(100));
            Assert.That(pokemon.GetEffectiveStat(Stat.Evasion), Is.EqualTo(100));
        }

        [Test]
        public void ModifyStatStage_Clamps_AtMax()
        {
            var pokemon = CreatePokemon(50);

            int change = pokemon.ModifyStatStage(Stat.Attack, 10); // Try +10 from 0

            Assert.That(pokemon.StatStages[Stat.Attack], Is.EqualTo(6));
            Assert.That(change, Is.EqualTo(6)); // Only changed by 6
        }

        [Test]
        public void ModifyStatStage_Clamps_AtMin()
        {
            var pokemon = CreatePokemon(50);

            int change = pokemon.ModifyStatStage(Stat.Defense, -10); // Try -10 from 0

            Assert.That(pokemon.StatStages[Stat.Defense], Is.EqualTo(-6));
            Assert.That(change, Is.EqualTo(-6)); // Only changed by -6
        }

        [Test]
        public void ModifyStatStage_AtMax_ReturnsZero()
        {
            var pokemon = CreatePokemon(50);
            pokemon.ModifyStatStage(Stat.Attack, 6); // Max out

            int change = pokemon.ModifyStatStage(Stat.Attack, 1);

            Assert.That(change, Is.EqualTo(0));
            Assert.That(pokemon.StatStages[Stat.Attack], Is.EqualTo(6));
        }

        [Test]
        public void ModifyStatStage_AtMin_ReturnsZero()
        {
            var pokemon = CreatePokemon(50);
            pokemon.ModifyStatStage(Stat.Defense, -6); // Min out

            int change = pokemon.ModifyStatStage(Stat.Defense, -1);

            Assert.That(change, Is.EqualTo(0));
            Assert.That(pokemon.StatStages[Stat.Defense], Is.EqualTo(-6));
        }

        [Test]
        public void ModifyStatStage_HP_ThrowsArgumentException()
        {
            var pokemon = CreatePokemon(50);

            // HP stat stage modification should throw an exception
            Assert.Throws<ArgumentException>(() => pokemon.ModifyStatStage(Stat.HP, 2));
        }

        [Test]
        public void GetEffectiveStat_WithStages()
        {
            var pokemon = CreatePokemon(50);
            int baseAttack = pokemon.Attack;

            pokemon.ModifyStatStage(Stat.Attack, 2); // +2 = 2x

            int effective = pokemon.GetEffectiveStat(Stat.Attack);
            Assert.That(effective, Is.EqualTo(baseAttack * 2));
        }

        [Test]
        public void GetEffectiveStat_Accuracy_WithStages()
        {
            var pokemon = CreatePokemon(50);

            pokemon.ModifyStatStage(Stat.Accuracy, 1); // +1

            int effective = pokemon.GetEffectiveStat(Stat.Accuracy);
            Assert.That(effective, Is.EqualTo(133)); // 100 * (4/3) = 133
        }

        [Test]
        public void ResetBattleState_ClearsAllStages()
        {
            var pokemon = CreatePokemon(50);
            pokemon.ModifyStatStage(Stat.Attack, 3);
            pokemon.ModifyStatStage(Stat.Defense, -2);
            pokemon.ModifyStatStage(Stat.Speed, 1);

            pokemon.ResetBattleState();

            Assert.That(pokemon.StatStages[Stat.Attack], Is.EqualTo(0));
            Assert.That(pokemon.StatStages[Stat.Defense], Is.EqualTo(0));
            Assert.That(pokemon.StatStages[Stat.Speed], Is.EqualTo(0));
        }

        [Test]
        public void ResetBattleState_KeepsPersistentStatus()
        {
            var pokemon = CreatePokemon(50);
            pokemon.Status = PersistentStatus.Burn;

            pokemon.ResetBattleState();

            Assert.That(pokemon.Status, Is.EqualTo(PersistentStatus.Burn));
        }

        [Test]
        public void ResetBattleState_ClearsVolatileStatus()
        {
            var pokemon = CreatePokemon(50);
            pokemon.VolatileStatus = VolatileStatus.Confusion;

            pokemon.ResetBattleState();

            Assert.That(pokemon.VolatileStatus, Is.EqualTo(VolatileStatus.None));
        }

        #endregion

        #region Pokemon HP Edge Cases

        [Test]
        public void TakeDamage_ExactlyMaxHP_Faints()
        {
            var pokemon = CreatePokemon(50);
            int maxHP = pokemon.MaxHP;

            int damage = pokemon.TakeDamage(maxHP);

            Assert.That(damage, Is.EqualTo(maxHP));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(0));
            Assert.That(pokemon.IsFainted, Is.True);
        }

        [Test]
        public void TakeDamage_MoreThanMaxHP_CapsAtCurrent()
        {
            var pokemon = CreatePokemon(50);
            int currentHP = pokemon.CurrentHP;

            int damage = pokemon.TakeDamage(9999999);

            Assert.That(damage, Is.EqualTo(currentHP));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(0));
        }

        [Test]
        public void TakeDamage_OneFainted_ReturnsZero()
        {
            var pokemon = CreatePokemon(50);
            pokemon.TakeDamage(pokemon.MaxHP); // Faint

            int damage = pokemon.TakeDamage(100);

            Assert.That(damage, Is.EqualTo(0));
        }

        [Test]
        public void Heal_AtFullHP_ReturnsZero()
        {
            var pokemon = CreatePokemon(50);

            int healed = pokemon.Heal(100);

            Assert.That(healed, Is.EqualTo(0));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        [Test]
        public void Heal_FromZero_Works()
        {
            var pokemon = CreatePokemon(50);
            pokemon.TakeDamage(pokemon.MaxHP);

            int healed = pokemon.Heal(50);

            Assert.That(healed, Is.EqualTo(50));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(50));
            Assert.That(pokemon.IsFainted, Is.False);
        }

        [Test]
        public void Heal_OverMax_Caps()
        {
            var pokemon = CreatePokemon(50);
            pokemon.TakeDamage(10);
            int missing = pokemon.MaxHP - pokemon.CurrentHP;

            int healed = pokemon.Heal(9999);

            Assert.That(healed, Is.EqualTo(missing));
            Assert.That(pokemon.CurrentHP, Is.EqualTo(pokemon.MaxHP));
        }

        [Test]
        public void HPPercentage_AtFull_Is100()
        {
            var pokemon = CreatePokemon(50);

            Assert.That(pokemon.HPPercentage, Is.EqualTo(1.0f));
        }

        [Test]
        public void HPPercentage_AtHalf_Is50()
        {
            var pokemon = CreatePokemon(50);
            pokemon.TakeDamage(pokemon.MaxHP / 2);

            Assert.That(pokemon.HPPercentage, Is.EqualTo(0.5f).Within(0.01f));
        }

        [Test]
        public void HPPercentage_AtZero_Is0()
        {
            var pokemon = CreatePokemon(50);
            pokemon.TakeDamage(pokemon.MaxHP);

            Assert.That(pokemon.HPPercentage, Is.EqualTo(0f));
        }

        #endregion

        #region Nature Verification

        [Test]
        public void AllNatures_Have_Multipliers()
        {
            foreach (Nature nature in Enum.GetValues(typeof(Nature)))
            {
                // Each nature should return valid multipliers for each stat
                foreach (Stat stat in new[] { Stat.Attack, Stat.Defense, Stat.SpAttack, Stat.SpDefense, Stat.Speed })
                {
                    float mult = NatureData.GetStatMultiplier(nature, stat);
                    Assert.That(mult, Is.GreaterThanOrEqualTo(0.9f));
                    Assert.That(mult, Is.LessThanOrEqualTo(1.1f));
                }
            }
        }

        [Test]
        public void NeutralNatures_NoEffect()
        {
            var neutralNatures = new[] { Nature.Hardy, Nature.Docile, Nature.Serious, Nature.Bashful, Nature.Quirky };

            foreach (var nature in neutralNatures)
            {
                Assert.That(NatureData.IsNeutral(nature), Is.True);

                foreach (Stat stat in new[] { Stat.Attack, Stat.Defense, Stat.SpAttack, Stat.SpDefense, Stat.Speed })
                {
                    Assert.That(NatureData.GetStatMultiplier(nature, stat), Is.EqualTo(1.0f));
                }
            }
        }

        [Test]
        public void NonNeutralNatures_HaveBoostAndReduce()
        {
            var nonNeutralNatures = new[] { Nature.Adamant, Nature.Jolly, Nature.Modest, Nature.Timid, Nature.Bold };

            foreach (var nature in nonNeutralNatures)
            {
                Assert.That(NatureData.IsNeutral(nature), Is.False);

                var increased = NatureData.GetIncreasedStat(nature);
                var decreased = NatureData.GetDecreasedStat(nature);

                Assert.That(increased, Is.Not.Null);
                Assert.That(decreased, Is.Not.Null);
                Assert.That(increased, Is.Not.EqualTo(decreased));
            }
        }

        #endregion

        #region Status Effect on Stats

        [Test]
        public void Burn_Halves_Attack()
        {
            var pokemon = CreatePokemon(50);
            int normalAttack = pokemon.GetEffectiveStat(Stat.Attack);

            pokemon.Status = PersistentStatus.Burn;
            int burnedAttack = pokemon.GetEffectiveStat(Stat.Attack);

            Assert.That(burnedAttack, Is.EqualTo(normalAttack / 2));
        }

        [Test]
        public void Burn_Does_Not_Affect_SpAttack()
        {
            var pokemon = CreatePokemon(50);
            int normalSpAttack = pokemon.GetEffectiveStat(Stat.SpAttack);

            pokemon.Status = PersistentStatus.Burn;
            int burnedSpAttack = pokemon.GetEffectiveStat(Stat.SpAttack);

            Assert.That(burnedSpAttack, Is.EqualTo(normalSpAttack));
        }

        [Test]
        public void Paralysis_Quarters_Speed()
        {
            var pokemon = CreatePokemon(50);
            int normalSpeed = pokemon.GetEffectiveStat(Stat.Speed);

            pokemon.Status = PersistentStatus.Paralysis;
            int paralyzedSpeed = pokemon.GetEffectiveStat(Stat.Speed);

            Assert.That(paralyzedSpeed, Is.EqualTo(normalSpeed / 4));
        }

        [Test]
        public void Paralysis_Does_Not_Affect_Attack()
        {
            var pokemon = CreatePokemon(50);
            int normalAttack = pokemon.GetEffectiveStat(Stat.Attack);

            pokemon.Status = PersistentStatus.Paralysis;
            int paralyzedAttack = pokemon.GetEffectiveStat(Stat.Attack);

            Assert.That(paralyzedAttack, Is.EqualTo(normalAttack));
        }

        [Test]
        public void GetEffectiveStatRaw_Ignores_Status()
        {
            var pokemon = CreatePokemon(50);
            int normalAttack = pokemon.GetEffectiveStatRaw(Stat.Attack);

            pokemon.Status = PersistentStatus.Burn;
            int rawAttack = pokemon.GetEffectiveStatRaw(Stat.Attack);

            Assert.That(rawAttack, Is.EqualTo(normalAttack));
        }

        [Test]
        public void Status_And_Stage_Stack()
        {
            var pokemon = CreatePokemon(50);
            int baseSpeed = pokemon.Speed;

            pokemon.ModifyStatStage(Stat.Speed, 2); // 2x
            pokemon.Status = PersistentStatus.Paralysis; // /4

            int effectiveSpeed = pokemon.GetEffectiveStat(Stat.Speed);
            // Base * 2 (stage) / 4 (paralysis) = base / 2
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed / 2));
        }

        #endregion

        #region PP Ups Tests

        [Test]
        public void MoveInstance_PPUps_Default_Zero()
        {
            var move = new MoveInstance(_testMove);
            Assert.That(move.PPUps, Is.EqualTo(0));
            Assert.That(move.CanApplyPPUp, Is.True);
        }

        [Test]
        public void MoveInstance_PPUps_Increase_MaxPP()
        {
            var move = new MoveInstance(_testMove);
            int basePP = move.MaxPP;

            move.ApplyPPUp();

            // 15 + (15 * 0.2 * 1) = 15 + 3 = 18
            Assert.That(move.MaxPP, Is.EqualTo(18));
            Assert.That(move.PPUps, Is.EqualTo(1));
        }

        [Test]
        public void MoveInstance_PPUps_Max_Is_3()
        {
            var move = new MoveInstance(_testMove);

            move.ApplyPPUp();
            move.ApplyPPUp();
            move.ApplyPPUp();

            Assert.That(move.PPUps, Is.EqualTo(3));
            Assert.That(move.CanApplyPPUp, Is.False);
            Assert.That(move.IsMaxPP, Is.True);

            bool result = move.ApplyPPUp();
            Assert.That(result, Is.False);
        }

        [Test]
        public void MoveInstance_PPMax_MaxesOut()
        {
            var move = new MoveInstance(_testMove);

            int applied = move.ApplyPPMax();

            Assert.That(applied, Is.EqualTo(3));
            Assert.That(move.IsMaxPP, Is.True);
            // 15 + (15 * 0.2 * 3) = 15 + 9 = 24
            Assert.That(move.MaxPP, Is.EqualTo(24));
        }

        [Test]
        public void MoveInstance_PPUp_Also_Restores_Gained_PP()
        {
            var move = new MoveInstance(_testMove);
            for (int i = 0; i < 5; i++) move.Use(); // Use 5 PP, 10 remaining

            move.ApplyPPUp();

            // MaxPP is now 18, CurrentPP should be 10 + 3 = 13
            Assert.That(move.CurrentPP, Is.EqualTo(13));
        }

        [Test]
        public void MoveInstance_Use_Multiple_PP()
        {
            var move = new MoveInstance(_testMove);

            bool result = move.Use(3);

            Assert.That(result, Is.True);
            Assert.That(move.CurrentPP, Is.EqualTo(12));
        }

        [Test]
        public void MoveInstance_Use_Multiple_PP_Fails_If_Not_Enough()
        {
            var move = new MoveInstance(_testMove);
            while (move.CurrentPP > 5) move.Use();

            bool result = move.Use(10);

            Assert.That(result, Is.False);
        }

        [Test]
        public void MoveInstance_Constructor_With_PPUps()
        {
            var move = new MoveInstance(_testMove, 2);

            Assert.That(move.PPUps, Is.EqualTo(2));
            // 15 + (15 * 0.2 * 2) = 15 + 6 = 21
            Assert.That(move.MaxPP, Is.EqualTo(21));
            Assert.That(move.CurrentPP, Is.EqualTo(21));
        }

        [Test]
        public void MoveInstance_Constructor_PPUps_Clamps()
        {
            var move = new MoveInstance(_testMove, 10);

            Assert.That(move.PPUps, Is.EqualTo(3)); // Clamped to max
        }

        #endregion

        #region Helper Methods

        private PokemonInstance CreatePokemon(int level)
        {
            return new PokemonInstance(
                _testSpecies, level,
                StatCalculator.CalculateHP(100, level),
                StatCalculator.CalculateStat(100, level, Nature.Hardy, Stat.Attack),
                StatCalculator.CalculateStat(100, level, Nature.Hardy, Stat.Defense),
                StatCalculator.CalculateStat(100, level, Nature.Hardy, Stat.SpAttack),
                StatCalculator.CalculateStat(100, level, Nature.Hardy, Stat.SpDefense),
                StatCalculator.CalculateStat(100, level, Nature.Hardy, Stat.Speed),
                Nature.Hardy, Gender.Male, new List<MoveInstance>());
        }

        #endregion
    }
}

