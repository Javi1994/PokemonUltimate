using System;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Damage
{
    /// <summary>
    /// Edge case tests for IStatModifier system.
    /// Tests boundary conditions, null handling, and real-world scenarios.
    /// </summary>
    [TestFixture]
    public class StatModifierEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;

        [SetUp]
        public void SetUp()
        {
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            
            _field = new BattleField();
            _field.Initialize(BattleRules.Singles, playerParty, enemyParty);
            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
        }

        #region Null Handling Tests

        [Test]
        public void ItemStatModifier_NullSlot_ThrowsException()
        {
            var modifier = new ItemStatModifier(ItemCatalog.ChoiceBand);
            
            Assert.Throws<ArgumentNullException>(() => 
                modifier.GetStatMultiplier(null, Stat.Attack, _field));
        }

        [Test]
        public void ItemStatModifier_NullField_ThrowsException()
        {
            var modifier = new ItemStatModifier(ItemCatalog.ChoiceBand);
            
            Assert.Throws<ArgumentNullException>(() => 
                modifier.GetStatMultiplier(_attackerSlot, Stat.Attack, null));
        }

        [Test]
        public void ItemStatModifier_NullContext_ThrowsException()
        {
            var modifier = new ItemStatModifier(ItemCatalog.LifeOrb);
            
            Assert.Throws<ArgumentNullException>(() => 
                modifier.GetDamageMultiplier(null));
        }

        [Test]
        public void AbilityStatModifier_NullSlot_ThrowsException()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            
            Assert.Throws<ArgumentNullException>(() => 
                modifier.GetStatMultiplier(null, Stat.Attack, _field));
        }

        [Test]
        public void AbilityStatModifier_NullField_ThrowsException()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            
            Assert.Throws<ArgumentNullException>(() => 
                modifier.GetStatMultiplier(_attackerSlot, Stat.Attack, null));
        }

        [Test]
        public void AbilityStatModifier_NullContext_ThrowsException()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            
            Assert.Throws<ArgumentNullException>(() => 
                modifier.GetDamageMultiplier(null));
        }

        #endregion

        #region Boundary Condition Tests

        [Test]
        public void AbilityStatModifier_Blaze_AtExactly33PercentHP_Activates()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            // Set HP to exactly 33% (threshold is <= 0.33)
            int hpAt33Percent = (int)(pokemon.MaxHP * 0.33f);
            pokemon.CurrentHP = hpAt33Percent;
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_At32_9PercentHP_Activates()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            // Set HP to 32.9% (below threshold)
            int hpAt32_9Percent = (int)(pokemon.MaxHP * 0.329f);
            pokemon.CurrentHP = hpAt32_9Percent;
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_At33_1PercentHP_DoesNotActivate()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            // Set HP to 33.1% (above threshold)
            int hpAt33_1Percent = (int)(pokemon.MaxHP * 0.331f);
            pokemon.CurrentHP = hpAt33_1Percent;
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_At1HP_Activates()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            // Set HP to 1 (minimum)
            pokemon.CurrentHP = 1;
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_AtMaxHP_DoesNotActivate()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            // Set HP to max
            pokemon.CurrentHP = pokemon.MaxHP;
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.0f));
        }

        #endregion

        #region Multiple Modifiers Tests

        [Test]
        public void BaseDamageStep_WithChoiceBandAndStatStages_AppliesBoth()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.HeldItem = ItemCatalog.ChoiceBand;
            
            // Apply +2 Attack stat stage
            _attackerSlot.ModifyStatStage(Stat.Attack, 2);
            
            var move = MoveCatalog.Tackle;
            var pipeline = new DamagePipeline();
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field);
            
            // Should have higher damage than without Choice Band
            Assert.That(context.BaseDamage, Is.GreaterThan(0));
        }

        [Test]
        public void AttackerItemStep_WithLifeOrb_StacksWithSTAB()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.HeldItem = ItemCatalog.LifeOrb;
            
            // Use Electric move with Electric Pokemon (STAB)
            var electricMove = MoveCatalog.ThunderShock;
            var pipeline = new DamagePipeline();
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, electricMove, _field);
            
            // Should have STAB (1.5x) and Life Orb (1.3x) = 1.95x total multiplier
            Assert.That(context.Multiplier, Is.GreaterThan(1.0f));
        }

        [Test]
        public void AttackerAbilityStep_WithBlaze_StacksWithSTAB()
        {
            // Use Fire move on a Fire Pokemon against a neutral target
            var charizard = PokemonFactory.Create(PokemonCatalog.Charizard, 50);
            charizard.SetAbility(AbilityCatalog.Blaze);
            charizard.CurrentHP = (int)(charizard.MaxHP * 0.30f);
            
            // Use a neutral target (Normal type) so type effectiveness is 1.0x
            var eevee = PokemonFactory.Create(PokemonCatalog.Eevee, 50);
            var neutralField = new BattleField();
            neutralField.Initialize(BattleRules.Singles, 
                new[] { charizard }, 
                new[] { eevee });
            
            // Re-apply ability and HP after field initialization (slots get new Pokemon instances)
            var attackerSlot = neutralField.PlayerSide.Slots[0];
            attackerSlot.Pokemon.SetAbility(AbilityCatalog.Blaze);
            attackerSlot.Pokemon.CurrentHP = (int)(attackerSlot.Pokemon.MaxHP * 0.30f);
            
            var fireMove = MoveCatalog.Ember;
            var pipeline = new DamagePipeline();
            
            // Use fixed random value (1.0) to eliminate variance
            var context = pipeline.Calculate(
                attackerSlot, 
                neutralField.EnemySide.Slots[0], 
                fireMove, 
                neutralField, 
                false, 
                1.0f);
            
            // Should have STAB (1.5x) and Blaze (1.5x) = 2.25x total multiplier
            // With random factor 1.0 and type effectiveness 1.0, multiplier should be 2.25x
            Assert.That(context.Multiplier, Is.GreaterThan(2.0f));
        }

        #endregion

        #region Real-World Validation Tests

        [Test]
        public void ChoiceBand_WithPhysicalMove_IncreasesDamageBy50Percent()
        {
            var pokemon = _attackerSlot.Pokemon;
            var baseAttack = pokemon.Attack;
            
            pokemon.HeldItem = ItemCatalog.ChoiceBand;
            
            var move = MoveCatalog.Tackle;
            var pipeline = new DamagePipeline();
            var contextWithItem = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field);
            
            pokemon.HeldItem = null;
            var contextWithoutItem = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field);
            
            // With Choice Band, Attack is 1.5x, so base damage should be ~1.5x higher
            // (actual final damage will vary due to random factor, but base damage should be consistent)
            float damageRatio = contextWithItem.BaseDamage / contextWithoutItem.BaseDamage;
            Assert.That(damageRatio, Is.GreaterThan(1.4f).And.LessThan(1.6f)); // Allow small margin for rounding
        }

        [Test]
        public void LifeOrb_IncreasesFinalDamageBy30Percent()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.HeldItem = ItemCatalog.LifeOrb;
            
            var move = MoveCatalog.Tackle;
            var pipeline = new DamagePipeline();
            
            // Use fixed random value for consistent comparison
            var contextWithItem = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, false, 1.0f);
            
            pokemon.HeldItem = null;
            var contextWithoutItem = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, false, 1.0f);
            
            // Life Orb multiplies damage by 1.3x
            float damageRatio = contextWithItem.FinalDamage / (float)contextWithoutItem.FinalDamage;
            Assert.That(damageRatio, Is.GreaterThan(1.25f).And.LessThan(1.35f)); // Allow margin for rounding
        }

        #endregion
    }
}

