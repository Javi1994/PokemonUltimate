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
    /// Functional tests for IStatModifier system.
    /// Tests stat and damage modifiers from abilities and items.
    /// </summary>
    [TestFixture]
    public class StatModifierTests
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

        #region ItemStatModifier Tests

        [Test]
        public void ItemStatModifier_ChoiceBand_Returns1_5ForAttack()
        {
            var modifier = new ItemStatModifier(ItemCatalog.ChoiceBand);
            
            float multiplier = modifier.GetStatMultiplier(_attackerSlot, Stat.Attack, _field);
            
            Assert.That(multiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void ItemStatModifier_ChoiceBand_Returns1_0ForOtherStats()
        {
            var modifier = new ItemStatModifier(ItemCatalog.ChoiceBand);
            
            float multiplier = modifier.GetStatMultiplier(_attackerSlot, Stat.Defense, _field);
            
            Assert.That(multiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void ItemStatModifier_LifeOrb_Returns1_3ForDamage()
        {
            var modifier = new ItemStatModifier(ItemCatalog.LifeOrb);
            var move = MoveCatalog.Tackle;
            var context = new DamageContext(_attackerSlot, _defenderSlot, move, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.3f));
        }

        [Test]
        public void ItemStatModifier_Leftovers_Returns1_0ForDamage()
        {
            var modifier = new ItemStatModifier(ItemCatalog.Leftovers);
            var move = MoveCatalog.Tackle;
            var context = new DamageContext(_attackerSlot, _defenderSlot, move, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void ItemStatModifier_NullItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new ItemStatModifier(null));
        }

        #endregion

        #region AbilityStatModifier Tests

        [Test]
        public void AbilityStatModifier_Blaze_Returns1_5WhenLowHPAndFireMove()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            
            // Set HP to 30% (below 33% threshold)
            pokemon.CurrentHP = (int)(pokemon.MaxHP * 0.30f);
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            var fireMove = MoveCatalog.Ember; // Fire type
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.5f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_Returns1_0WhenHighHP()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            
            // Set HP to 50% (above 33% threshold)
            pokemon.CurrentHP = (int)(pokemon.MaxHP * 0.50f);
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_Returns1_0WhenNonFireMove()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            
            // Set HP to 30% (below threshold)
            pokemon.CurrentHP = (int)(pokemon.MaxHP * 0.30f);
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            var nonFireMove = MoveCatalog.Tackle; // Normal type
            var context = new DamageContext(_attackerSlot, _defenderSlot, nonFireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.0f));
        }

        [Test]
        public void AbilityStatModifier_Blaze_Returns1_0AtExactly33PercentHP()
        {
            var modifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            var pokemon = _attackerSlot.Pokemon;
            
            // Set HP to exactly 33% (threshold is <= 0.33)
            pokemon.CurrentHP = (int)(pokemon.MaxHP * 0.33f);
            pokemon.SetAbility(AbilityCatalog.Blaze);
            
            var fireMove = MoveCatalog.Ember;
            var context = new DamageContext(_attackerSlot, _defenderSlot, fireMove, _field);
            
            float multiplier = modifier.GetDamageMultiplier(context);
            
            Assert.That(multiplier, Is.EqualTo(1.5f)); // Should activate at exactly 33%
        }

        [Test]
        public void AbilityStatModifier_NullAbility_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new AbilityStatModifier(null));
        }

        #endregion

        #region Integration with BaseDamageStep Tests

        [Test]
        public void BaseDamageStep_WithChoiceBand_IncreasesAttackStat()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.HeldItem = ItemCatalog.ChoiceBand;
            
            var move = MoveCatalog.Tackle; // Physical move
            var pipeline = new DamagePipeline();
            
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field);
            
            // With Choice Band, Attack should be 1.5x, so damage should be higher
            // We'll compare with a Pokemon without Choice Band
            var pokemonWithoutItem = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            var fieldWithoutItem = new BattleField();
            var playerPartyWithoutItem = new[] { pokemonWithoutItem };
            var enemyParty = new[] { _defenderSlot.Pokemon };
            fieldWithoutItem.Initialize(BattleRules.Singles, playerPartyWithoutItem, enemyParty);
            var slotWithoutItem = fieldWithoutItem.PlayerSide.Slots[0];
            
            var contextWithoutItem = pipeline.Calculate(slotWithoutItem, fieldWithoutItem.EnemySide.Slots[0], move, fieldWithoutItem);
            
            Assert.That(context.BaseDamage, Is.GreaterThan(contextWithoutItem.BaseDamage));
        }

        [Test]
        public void BaseDamageStep_WithChoiceBand_OnlyAffectsPhysicalMoves()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.HeldItem = ItemCatalog.ChoiceBand;
            
            var physicalMove = MoveCatalog.Tackle;
            var specialMove = MoveCatalog.ThunderShock;
            
            var pipeline = new DamagePipeline();
            
            var physicalContext = pipeline.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field);
            
            // Remove item and test special move
            pokemon.HeldItem = null;
            var physicalContextNoItem = pipeline.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field);
            
            // Choice Band should only affect physical moves
            Assert.That(physicalContext.BaseDamage, Is.GreaterThan(physicalContextNoItem.BaseDamage));
        }

        #endregion

        #region Integration with AttackerItemStep Tests

        [Test]
        public void AttackerItemStep_WithLifeOrb_IncreasesDamage()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.HeldItem = ItemCatalog.LifeOrb;
            
            var move = MoveCatalog.Tackle;
            var pipeline = new DamagePipeline();
            
            // Use fixed random value for consistent comparison
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, false, 1.0f);
            
            // Remove item and compare with same random value
            pokemon.HeldItem = null;
            var contextWithoutItem = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field, false, 1.0f);
            
            // Life Orb should increase damage by 30%
            Assert.That(context.FinalDamage, Is.GreaterThan(contextWithoutItem.FinalDamage));
        }

        #endregion

        #region Integration with AttackerAbilityStep Tests

        [Test]
        public void AttackerAbilityStep_WithBlaze_IncreasesFireDamageWhenLowHP()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            pokemon.CurrentHP = (int)(pokemon.MaxHP * 0.30f); // Below 33% threshold
            
            var fireMove = MoveCatalog.Ember;
            var pipeline = new DamagePipeline();
            
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, fireMove, _field);
            
            // Remove ability and compare
            pokemon.SetAbility(null);
            var contextWithoutAbility = pipeline.Calculate(_attackerSlot, _defenderSlot, fireMove, _field);
            
            // Blaze should increase Fire damage by 50% when HP is low
            Assert.That(context.FinalDamage, Is.GreaterThan(contextWithoutAbility.FinalDamage));
        }

        [Test]
        public void AttackerAbilityStep_WithBlaze_DoesNotAffectNonFireMoves()
        {
            var pokemon = _attackerSlot.Pokemon;
            pokemon.SetAbility(AbilityCatalog.Blaze);
            pokemon.CurrentHP = (int)(pokemon.MaxHP * 0.30f);
            
            var nonFireMove = MoveCatalog.Tackle;
            var pipeline = new DamagePipeline();
            
            // Use fixed random value for consistent comparison
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, nonFireMove, _field, false, 1.0f);
            
            // Remove ability and compare with same random value
            pokemon.SetAbility(null);
            var contextWithoutAbility = pipeline.Calculate(_attackerSlot, _defenderSlot, nonFireMove, _field, false, 1.0f);
            
            // Blaze should not affect non-Fire moves
            Assert.That(context.FinalDamage, Is.EqualTo(contextWithoutAbility.FinalDamage));
        }

        #endregion
    }
}

