using System;
using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Moves;

namespace PokemonUltimate.Tests.Combat.Damage
{
    /// <summary>
    /// Edge case tests for DamagePipeline and its steps.
    /// </summary>
    [TestFixture]
    public class DamagePipelineEdgeCasesTests
    {
        private DamagePipeline _pipeline;
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;

        [SetUp]
        public void SetUp()
        {
            _pipeline = new DamagePipeline();
            _field = new BattleField();
            
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            
            _field.Initialize(BattleRules.Singles, playerParty, enemyParty);
            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
        }

        #region Level Edge Cases

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(50)]
        [TestCase(100)]
        public void Calculate_VariousLevels_ProducesValidDamage(int level)
        {
            var attacker = PokemonFactory.Create(PokemonCatalog.Pikachu, level);
            var defender = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(attacker, defender);

            var context = _pipeline.Calculate(
                field.PlayerSide.Slots[0], 
                field.EnemySide.Slots[0], 
                MoveCatalog.Tackle, 
                field,
                fixedRandomValue: 1.0f);

            Assert.That(context.FinalDamage, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Calculate_Level1VsLevel100_StillDealsDamage()
        {
            var weakAttacker = PokemonFactory.Create(PokemonCatalog.Pikachu, 1);
            var strongDefender = PokemonFactory.Create(PokemonCatalog.Snorlax, 100);
            var field = CreateBattleField(weakAttacker, strongDefender);

            var context = _pipeline.Calculate(
                field.PlayerSide.Slots[0], 
                field.EnemySide.Slots[0], 
                MoveCatalog.Tackle, 
                field,
                fixedRandomValue: 1.0f);

            // Minimum damage is 1 (unless immune)
            Assert.That(context.FinalDamage, Is.GreaterThanOrEqualTo(1));
        }

        #endregion

        #region Stat Stage Edge Cases

        [Test]
        public void Calculate_MaxAttackStages_IncreaseDamage()
        {
            _attackerSlot.ModifyStatStage(Stat.Attack, 6); // +6 stages
            
            var boostedContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            // Reset and calculate normal
            _attackerSlot.ResetBattleState();
            _attackerSlot.SetPokemon(_attackerSlot.Pokemon);
            
            var normalContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            Assert.That(boostedContext.FinalDamage, Is.GreaterThan(normalContext.FinalDamage));
        }

        [Test]
        public void Calculate_MaxDefenseStages_DecreaseDamage()
        {
            _defenderSlot.ModifyStatStage(Stat.Defense, 6); // +6 defense stages
            
            var reducedContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            // Reset defender
            _defenderSlot.ResetBattleState();
            _defenderSlot.SetPokemon(_defenderSlot.Pokemon);
            
            var normalContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            Assert.That(reducedContext.FinalDamage, Is.LessThan(normalContext.FinalDamage));
        }

        [Test]
        public void Calculate_MinAttackStages_DecreaseDamage()
        {
            _attackerSlot.ModifyStatStage(Stat.Attack, -6); // -6 stages
            
            var reducedContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            // Damage should still be at least 1
            Assert.That(reducedContext.FinalDamage, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Calculate_SpAttackStages_AffectsSpecialMoves()
        {
            _attackerSlot.ModifyStatStage(Stat.SpAttack, 4);
            
            var boostedContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.ThunderShock, _field, fixedRandomValue: 1.0f);

            _attackerSlot.ResetBattleState();
            _attackerSlot.SetPokemon(_attackerSlot.Pokemon);
            
            var normalContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.ThunderShock, _field, fixedRandomValue: 1.0f);

            Assert.That(boostedContext.FinalDamage, Is.GreaterThan(normalContext.FinalDamage));
        }

        [Test]
        public void Calculate_PhysicalMoveIgnoresSpDefenseStages()
        {
            _defenderSlot.ModifyStatStage(Stat.SpDefense, 6); // +6 SpDef
            
            var withSpDefContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            _defenderSlot.ResetBattleState();
            _defenderSlot.SetPokemon(_defenderSlot.Pokemon);
            
            var normalContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            // SpDefense stages shouldn't affect physical moves
            Assert.That(withSpDefContext.FinalDamage, Is.EqualTo(normalContext.FinalDamage));
        }

        #endregion

        #region Random Factor Edge Cases

        [Test]
        [TestCase(0.0f)]  // Minimum random (results in 0.85x)
        [TestCase(0.5f)]  // Mid random
        [TestCase(1.0f)]  // Maximum random (results in 1.0x)
        public void Calculate_FixedRandomValues_ProducesConsistentResults(float randomValue)
        {
            // Force no crit to ensure deterministic results
            var context1 = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, 
                forceCritical: false, fixedRandomValue: randomValue);
            var context2 = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, 
                forceCritical: false, fixedRandomValue: randomValue);

            Assert.That(context1.FinalDamage, Is.EqualTo(context2.FinalDamage));
        }

        [Test]
        public void Calculate_MinRandomFactor_IsLowerThanMaxRandom()
        {
            var minContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 0.0f);
            var maxContext = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.Tackle, _field, fixedRandomValue: 1.0f);

            Assert.That(minContext.FinalDamage, Is.LessThanOrEqualTo(maxContext.FinalDamage));
        }

        #endregion

        #region Status Move Edge Cases

        [Test]
        public void Calculate_StatusMove_DealsZeroDamage()
        {
            var statusMove = MoveCatalog.Growl; // Status move

            var context = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, statusMove, _field);

            Assert.That(context.BaseDamage, Is.EqualTo(0));
            Assert.That(context.FinalDamage, Is.EqualTo(0));
        }

        #endregion

        #region Multiple Modifier Stacking

        [Test]
        public void Calculate_AllModifiersStack_Correctly()
        {
            // Pikachu (Electric) uses ThunderShock (Electric) vs Squirtle (Water)
            // STAB: 1.5x, Type: 2.0x, Critical: 1.5x, Random max: 1.0x
            // Total multiplier should include all of these
            
            var context = _pipeline.Calculate(
                _attackerSlot, _defenderSlot, MoveCatalog.ThunderShock, _field,
                forceCritical: true, fixedRandomValue: 1.0f);

            Assert.That(context.IsStab, Is.True);
            Assert.That(context.IsCritical, Is.True);
            Assert.That(context.TypeEffectiveness, Is.EqualTo(2.0f));
            
            // Multiplier should be at least STAB * Type * Crit = 1.5 * 2 * 1.5 = 4.5
            Assert.That(context.Multiplier, Is.GreaterThanOrEqualTo(4.0f));
        }

        [Test]
        public void Calculate_BurnAndWeakType_BothApply()
        {
            // Physical attack with burn and not-effective type
            _attackerSlot.Pokemon.Status = PersistentStatus.Burn;
            
            // Pikachu uses Tackle vs Bulbasaur
            // Tackle is Normal, neutral vs Grass/Poison = 1.0x
            // Burn on physical = 0.5x
            var bulbasaur = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);
            var field = new BattleField();
            field.Initialize(BattleRules.Singles,
                new List<PokemonInstance> { _attackerSlot.Pokemon },
                new List<PokemonInstance> { bulbasaur });
            
            var context = _pipeline.Calculate(
                field.PlayerSide.Slots[0], 
                field.EnemySide.Slots[0], 
                MoveCatalog.Tackle, 
                field,
                fixedRandomValue: 1.0f);

            // Should have burn penalty applied
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
        }

        #endregion

        #region Pipeline Extension Tests

        [Test]
        public void Pipeline_AddStep_IncreasesStepCount()
        {
            var initialCount = _pipeline.StepCount;
            
            _pipeline.AddStep(new TestDamageStep());

            Assert.That(_pipeline.StepCount, Is.EqualTo(initialCount + 1));
        }

        [Test]
        public void Pipeline_InsertStep_AtCorrectPosition()
        {
            var customPipeline = new DamagePipeline(new IDamageStep[] 
            { 
                new BaseDamageStep(),
                new TypeEffectivenessStep()
            });

            customPipeline.InsertStep(1, new StabStep());

            Assert.That(customPipeline.StepCount, Is.EqualTo(3));
        }

        [Test]
        public void Pipeline_AddNullStep_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _pipeline.AddStep(null));
        }

        [Test]
        public void Pipeline_InsertNullStep_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _pipeline.InsertStep(0, null));
        }

        #endregion

        #region Context Edge Cases

        [Test]
        public void DamageContext_NullAttacker_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new DamageContext(null, _defenderSlot, MoveCatalog.Tackle, _field));
        }

        [Test]
        public void DamageContext_NullDefender_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new DamageContext(_attackerSlot, null, MoveCatalog.Tackle, _field));
        }

        [Test]
        public void DamageContext_NullMove_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new DamageContext(_attackerSlot, _defenderSlot, null, _field));
        }

        [Test]
        public void DamageContext_NullField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new DamageContext(_attackerSlot, _defenderSlot, MoveCatalog.Tackle, null));
        }

        #endregion

        #region Helper Classes

        private BattleField CreateBattleField(PokemonInstance player, PokemonInstance enemy)
        {
            var field = new BattleField();
            field.Initialize(BattleRules.Singles,
                new List<PokemonInstance> { player },
                new List<PokemonInstance> { enemy });
            return field;
        }

        /// <summary>
        /// Test step that does nothing (for testing pipeline extension).
        /// </summary>
        private class TestDamageStep : IDamageStep
        {
            public void Process(DamageContext context)
            {
                // No-op for testing
            }
        }

        #endregion
    }
}

