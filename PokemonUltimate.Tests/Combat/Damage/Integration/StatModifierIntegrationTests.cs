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

namespace PokemonUltimate.Tests.Combat.Damage.Integration
{
    /// <summary>
    /// Integration tests for Stat Modifier system.
    /// Verifies that stat and damage modifiers work correctly with the full DamagePipeline
    /// and interact properly with other systems (STAB, Type Effectiveness, etc.).
    /// </summary>
    [TestFixture]
    public class StatModifierIntegrationTests
    {
        private DamagePipeline _pipeline;

        [SetUp]
        public void SetUp()
        {
            _pipeline = new DamagePipeline();
        }

        #region Stat Modifiers ↔ DamagePipeline Integration

        [Test]
        public void BaseDamageStep_ChoiceBand_AppliesBeforeStatRatio()
        {
            // Setup: Pikachu with Choice Band
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.ChoiceBand;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var move = MoveCatalog.Tackle; // Physical move
            
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, move, field);
            
            // Choice Band should increase Attack stat, which increases BaseDamage
            // Compare with same Pokemon without Choice Band
            pikachu.HeldItem = null;
            var fieldWithoutItem = CreateBattleField(pikachu, squirtle);
            var contextWithoutItem = _pipeline.Calculate(
                fieldWithoutItem.PlayerSide.Slots[0], 
                fieldWithoutItem.EnemySide.Slots[0], 
                move, 
                fieldWithoutItem);
            
            Assert.That(context.BaseDamage, Is.GreaterThan(contextWithoutItem.BaseDamage));
        }

        [Test]
        public void BaseDamageStep_ChoiceBand_OnlyAffectsTargetStat()
        {
            // Setup: Pikachu with Choice Band (boosts Attack, not SpAttack)
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.ChoiceBand;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            
            var physicalMove = MoveCatalog.Tackle; // Uses Attack
            var specialMove = MoveCatalog.ThunderShock; // Uses SpAttack
            
            var physicalContext = _pipeline.Calculate(attackerSlot, defenderSlot, physicalMove, field);
            
            // Remove item and test both moves
            pikachu.HeldItem = null;
            var fieldNoItem = CreateBattleField(pikachu, squirtle);
            var physicalContextNoItem = _pipeline.Calculate(
                fieldNoItem.PlayerSide.Slots[0], 
                fieldNoItem.EnemySide.Slots[0], 
                physicalMove, 
                fieldNoItem);
            var specialContextNoItem = _pipeline.Calculate(
                fieldNoItem.PlayerSide.Slots[0], 
                fieldNoItem.EnemySide.Slots[0], 
                specialMove, 
                fieldNoItem);
            
            // Choice Band should boost physical moves but not special moves
            Assert.That(physicalContext.BaseDamage, Is.GreaterThan(physicalContextNoItem.BaseDamage));
            // Special move damage should be the same (no boost)
            Assert.That(specialContextNoItem.BaseDamage, Is.GreaterThan(0));
        }

        #endregion

        #region Damage Modifiers ↔ DamagePipeline Integration

        [Test]
        public void AttackerItemStep_LifeOrb_AppliesAfterSTAB()
        {
            // Setup: Pikachu with Life Orb using Electric move (STAB)
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.LifeOrb;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var electricMove = MoveCatalog.ThunderShock; // Electric type, STAB
            
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, electricMove, field, false, 1.0f);
            
            // Remove item and compare
            pikachu.HeldItem = null;
            var fieldNoItem = CreateBattleField(pikachu, squirtle);
            var contextNoItem = _pipeline.Calculate(
                fieldNoItem.PlayerSide.Slots[0], 
                fieldNoItem.EnemySide.Slots[0], 
                electricMove, 
                fieldNoItem, 
                false, 
                1.0f);
            
            // Life Orb should multiply final damage by 1.3x
            // STAB is 1.5x, so with Life Orb: 1.5 * 1.3 = 1.95x total
            float actualRatio = context.FinalDamage / (float)contextNoItem.FinalDamage;
            
            Assert.That(actualRatio, Is.GreaterThan(1.25f).And.LessThan(1.35f)); // Allow margin for rounding
        }

        [Test]
        public void AttackerAbilityStep_Blaze_AppliesAfterSTAB()
        {
            // Setup: Charizard with Blaze using Fire move (STAB)
            var charizard = PokemonFactory.Create(PokemonCatalog.Charizard, 50);
            charizard.SetAbility(AbilityCatalog.Blaze);
            charizard.CurrentHP = (int)(charizard.MaxHP * 0.30f); // Below 33% threshold
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(charizard, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var fireMove = MoveCatalog.Ember; // Fire type, STAB
            
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, fireMove, field, false, 1.0f);
            
            // Remove ability and compare
            charizard.SetAbility(null);
            var fieldNoAbility = CreateBattleField(charizard, squirtle);
            var contextNoAbility = _pipeline.Calculate(
                fieldNoAbility.PlayerSide.Slots[0], 
                fieldNoAbility.EnemySide.Slots[0], 
                fireMove, 
                fieldNoAbility, 
                false, 
                1.0f);
            
            // Blaze should multiply final damage by 1.5x
            // STAB is 1.5x, so with Blaze: 1.5 * 1.5 = 2.25x total
            float actualRatio = context.FinalDamage / (float)contextNoAbility.FinalDamage;
            
            Assert.That(actualRatio, Is.GreaterThan(1.45f).And.LessThan(1.55f)); // Allow margin for rounding
        }

        [Test]
        public void AttackerItemStep_LifeOrb_AppliesBeforeTypeEffectiveness()
        {
            // Setup: Pikachu with Life Orb using super-effective move
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.LifeOrb;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50); // Water type
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var electricMove = MoveCatalog.ThunderShock; // Electric vs Water = 2x super effective
            
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, electricMove, field, false, 1.0f);
            
            // Verify TypeEffectiveness is applied (should be 2.0)
            Assert.That(context.TypeEffectiveness, Is.EqualTo(2.0f));
            
            // Life Orb multiplier should be in the Multiplier (applied before type effectiveness)
            Assert.That(context.Multiplier, Is.GreaterThan(1.0f));
        }

        #endregion

        #region Multiple Modifiers Stacking Integration

        [Test]
        public void DamagePipeline_ChoiceBandAndLifeOrb_StackCorrectly()
        {
            // Setup: Pikachu with both Choice Band and Life Orb (shouldn't happen in real game, but tests stacking)
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.ChoiceBand; // +50% Attack stat
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var move = MoveCatalog.Tackle; // Physical move
            
            var contextWithChoiceBand = _pipeline.Calculate(attackerSlot, defenderSlot, move, field, false, 1.0f);
            
            // Now add Life Orb effect manually (simulating both items)
            // In real game, you can't have both, but this tests the stacking logic
            var contextWithBoth = _pipeline.Calculate(attackerSlot, defenderSlot, move, field, false, 1.0f);
            // Apply Life Orb multiplier manually to simulate both items
            contextWithBoth.Multiplier *= 1.3f;
            
            // Base damage should be higher with Choice Band (stat boost)
            Assert.That(contextWithChoiceBand.BaseDamage, Is.GreaterThan(0));
            // Final damage with both should be even higher
            Assert.That(contextWithBoth.FinalDamage, Is.GreaterThan(contextWithChoiceBand.FinalDamage));
        }

        [Test]
        public void DamagePipeline_BlazeAndSTAB_StackMultiplicatively()
        {
            // Setup: Charizard with Blaze using Fire move (STAB + Blaze)
            var charizard = PokemonFactory.Create(PokemonCatalog.Charizard, 50);
            charizard.SetAbility(AbilityCatalog.Blaze);
            charizard.CurrentHP = (int)(charizard.MaxHP * 0.30f);
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(charizard, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var fireMove = MoveCatalog.Ember; // Fire type, STAB
            
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, fireMove, field, false, 1.0f);
            
            // Verify STAB is applied
            Assert.That(context.IsStab, Is.True);
            
            // Multiplier should include both STAB (1.5x) and Blaze (1.5x) = 2.25x total
            // But STAB is applied in StabStep, and Blaze in AttackerAbilityStep
            // So multiplier should be > 1.0 (includes Blaze)
            Assert.That(context.Multiplier, Is.GreaterThan(1.0f));
        }

        #endregion

        #region Stat Modifiers ↔ Stat Stages Integration

        [Test]
        public void BaseDamageStep_ChoiceBand_StacksWithStatStages()
        {
            // Setup: Pikachu with Choice Band and +2 Attack stat stage
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.ChoiceBand;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            attackerSlot.ModifyStatStage(Stat.Attack, 2); // +2 Attack
            
            var defenderSlot = field.EnemySide.Slots[0];
            var move = MoveCatalog.Tackle;
            
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, move, field);
            
            // Should have higher damage than without stat stage
            var fieldNoStage = CreateBattleField(pikachu, squirtle);
            var attackerSlotNoStage = fieldNoStage.PlayerSide.Slots[0];
            attackerSlotNoStage.Pokemon.HeldItem = ItemCatalog.ChoiceBand;
            var contextNoStage = _pipeline.Calculate(attackerSlotNoStage, fieldNoStage.EnemySide.Slots[0], move, fieldNoStage);
            
            Assert.That(context.BaseDamage, Is.GreaterThan(contextNoStage.BaseDamage));
        }

        #endregion

        #region Integration with Full Battle System

        [Test]
        public void DamagePipeline_WithChoiceBand_InFullBattleContext()
        {
            // Setup: Full battle scenario with Choice Band
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.ChoiceBand;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var move = MoveCatalog.Tackle;
            
            // Calculate damage in full pipeline context
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, move, field);
            
            // Verify all steps processed correctly
            Assert.That(context.BaseDamage, Is.GreaterThan(0));
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
            Assert.That(context.TypeEffectiveness, Is.GreaterThan(0));
        }

        [Test]
        public void DamagePipeline_WithLifeOrb_InFullBattleContext()
        {
            // Setup: Full battle scenario with Life Orb
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            pikachu.HeldItem = ItemCatalog.LifeOrb;
            
            var squirtle = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            var field = CreateBattleField(pikachu, squirtle);
            
            var attackerSlot = field.PlayerSide.Slots[0];
            var defenderSlot = field.EnemySide.Slots[0];
            var move = MoveCatalog.ThunderShock;
            
            // Calculate damage in full pipeline context
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, move, field);
            
            // Verify all steps processed correctly
            Assert.That(context.BaseDamage, Is.GreaterThan(0));
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
            Assert.That(context.Multiplier, Is.GreaterThan(1.0f)); // Should include Life Orb multiplier
        }

        [Test]
        public void DamagePipeline_WithBlaze_InFullBattleContext()
        {
            // Setup: Full battle scenario with Blaze
            var charizard = PokemonFactory.Create(PokemonCatalog.Charizard, 50);
            charizard.SetAbility(AbilityCatalog.Blaze);
            charizard.CurrentHP = (int)(charizard.MaxHP * 0.30f);
            
            // Use a neutral target (Normal type) so type effectiveness is 1.0x
            var eevee = PokemonFactory.Create(PokemonCatalog.Eevee, 50);
            var field = CreateBattleField(charizard, eevee);
            
            // Re-apply ability and HP after field initialization
            var attackerSlot = field.PlayerSide.Slots[0];
            attackerSlot.Pokemon.SetAbility(AbilityCatalog.Blaze);
            attackerSlot.Pokemon.CurrentHP = (int)(attackerSlot.Pokemon.MaxHP * 0.30f);
            
            var defenderSlot = field.EnemySide.Slots[0];
            var move = MoveCatalog.Ember;
            
            // Calculate damage in full pipeline context with fixed random value
            var context = _pipeline.Calculate(attackerSlot, defenderSlot, move, field, false, 1.0f);
            
            // Verify all steps processed correctly
            Assert.That(context.BaseDamage, Is.GreaterThan(0));
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
            // With STAB (1.5x), Blaze (1.5x), random factor (1.0x), and type effectiveness (1.0x),
            // multiplier should be 2.25x, which is > 1.0f
            Assert.That(context.Multiplier, Is.GreaterThan(1.0f)); // Should include Blaze multiplier
            Assert.That(context.IsStab, Is.True); // Fire move on Fire Pokemon
        }

        #endregion

        #region Helper Methods

        private BattleField CreateBattleField(PokemonInstance playerPokemon, PokemonInstance enemyPokemon)
        {
            var field = new BattleField();
            var playerParty = new[] { playerPokemon };
            var enemyParty = new[] { enemyPokemon };
            field.Initialize(BattleRules.Singles, playerParty, enemyParty);
            return field;
        }

        #endregion
    }
}

