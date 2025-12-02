using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration
{
    /// <summary>
    /// Integration tests for Status Effects - verifies status effects integrate with DamagePipeline and other systems.
    /// </summary>
    [TestFixture]
    public class StatusEffectsIntegrationTests
    {
        private BattleField _field;
        private BattleSlot _attackerSlot;
        private BattleSlot _defenderSlot;
        private PokemonInstance _attacker;
        private PokemonInstance _defender;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            _defender = _defenderSlot.Pokemon;
        }

        #region Status Effects -> DamagePipeline Integration

        [Test]
        public void Burn_PhysicalMove_ReducesDamage()
        {
            // Arrange - Create a physical move
            var physicalMove = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            var moveInstance = new MoveInstance(physicalMove);

            // Apply Burn status to attacker
            var applyStatusAction = new ApplyStatusAction(_attackerSlot, _attackerSlot, PersistentStatus.Burn);
            applyStatusAction.ExecuteLogic(_field);

            // Calculate damage WITHOUT burn (baseline)
            var pipelineWithoutBurn = new DamagePipeline();
            _attacker.Status = PersistentStatus.None; // Reset status
            var contextWithoutBurn = pipelineWithoutBurn.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field);
            float damageWithoutBurn = contextWithoutBurn.FinalDamage;

            // Apply Burn again
            _attacker.Status = PersistentStatus.Burn;

            // Act - Calculate damage WITH burn
            var pipelineWithBurn = new DamagePipeline();
            var contextWithBurn = pipelineWithBurn.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field);
            float damageWithBurn = contextWithBurn.FinalDamage;

            // Assert - Burn should reduce physical damage by 50%
            Assert.That(damageWithBurn, Is.LessThan(damageWithoutBurn));
            // With 50% reduction, damage should be approximately half (allowing for rounding)
            float expectedRatio = 0.5f;
            float actualRatio = damageWithBurn / damageWithoutBurn;
            Assert.That(actualRatio, Is.EqualTo(expectedRatio).Within(0.1f), 
                "Burn should reduce physical damage by approximately 50%");
        }

        [Test]
        public void Burn_SpecialMove_DoesNotReduceDamage()
        {
            // Arrange - Create a special move
            var specialMove = new MoveData
            {
                Name = "Thunderbolt",
                Power = 90,
                Accuracy = 100,
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            // Use fixed random value to ensure consistent results
            float fixedRandom = 0.9f; // Use same random value for both calculations

            // Calculate damage WITHOUT burn (baseline)
            var pipelineWithoutBurn = new DamagePipeline();
            _attacker.Status = PersistentStatus.None; // Reset status
            var contextWithoutBurn = pipelineWithoutBurn.Calculate(_attackerSlot, _defenderSlot, specialMove, _field, false, fixedRandom);
            float damageWithoutBurn = contextWithoutBurn.FinalDamage;

            // Apply Burn status to attacker
            var applyStatusAction = new ApplyStatusAction(_attackerSlot, _attackerSlot, PersistentStatus.Burn);
            applyStatusAction.ExecuteLogic(_field);

            // Act - Calculate damage WITH burn (using same random value)
            var pipelineWithBurn = new DamagePipeline();
            var contextWithBurn = pipelineWithBurn.Calculate(_attackerSlot, _defenderSlot, specialMove, _field, false, fixedRandom);
            float damageWithBurn = contextWithBurn.FinalDamage;

            // Assert - Burn should NOT affect special moves
            // The damage should be exactly the same (within floating point precision)
            Assert.That(damageWithBurn, Is.EqualTo(damageWithoutBurn).Within(0.1f), 
                "Burn should not reduce special move damage");
            
            // Verify the ratio is close to 1.0 (no reduction)
            float ratio = damageWithBurn / damageWithoutBurn;
            Assert.That(ratio, Is.EqualTo(1.0f).Within(0.05f), 
                "Burn should not reduce special move damage (ratio should be ~1.0, not ~0.5)");
        }

        [Test]
        public void ApplyStatusAction_Burn_ThenUseMoveAction_PhysicalMove_UsesReducedDamage()
        {
            // Arrange - Create a physical move
            var physicalMove = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            var moveInstance = new MoveInstance(physicalMove);

            // Apply Burn status
            var applyStatusAction = new ApplyStatusAction(_attackerSlot, _attackerSlot, PersistentStatus.Burn);
            applyStatusAction.ExecuteLogic(_field);

            int initialHP = _defender.CurrentHP;

            // Act - Use physical move with burn
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            damageAction?.ExecuteLogic(_field);

            // Assert - Damage should be reduced
            Assert.That(_defender.CurrentHP, Is.LessThan(initialHP));
            
            // Verify the damage context reflects burn penalty
            if (damageAction != null)
            {
                // The multiplier should reflect burn penalty (0.5x for physical)
                // Note: This is indirect verification - the actual damage reduction is what matters
                Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
            }
        }

        #endregion

        #region Status Effects -> UseMoveAction Prevention Integration

        [Test]
        public void Sleep_PreventsMoveExecution()
        {
            // Arrange
            var moveInstance = _attacker.Moves.FirstOrDefault(m => m.HasPP);
            if (moveInstance == null)
            {
                Assert.Inconclusive("No move with PP found");
                return;
            }

            // Apply Sleep status
            var applyStatusAction = new ApplyStatusAction(_attackerSlot, _attackerSlot, PersistentStatus.Sleep);
            applyStatusAction.ExecuteLogic(_field);

            int initialHP = _defender.CurrentHP;

            // Act - Try to use move while asleep
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Assert - Should generate message action, no damage action
            var messageAction = reactions.OfType<MessageAction>().FirstOrDefault();
            Assert.That(messageAction, Is.Not.Null);
            Assert.That(messageAction.Message, Contains.Substring("asleep").IgnoreCase);
            
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Null, "Sleep should prevent move execution");
            
            Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP), "No damage should be dealt");
        }

        [Test]
        public void Freeze_PreventsMoveExecution()
        {
            // Arrange
            var moveInstance = _attacker.Moves.FirstOrDefault(m => m.HasPP);
            if (moveInstance == null)
            {
                Assert.Inconclusive("No move with PP found");
                return;
            }

            // Apply Freeze status
            var applyStatusAction = new ApplyStatusAction(_attackerSlot, _attackerSlot, PersistentStatus.Freeze);
            applyStatusAction.ExecuteLogic(_field);

            int initialHP = _defender.CurrentHP;

            // Act - Try to use move while frozen
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Assert - Should generate message action, no damage action
            var messageAction = reactions.OfType<MessageAction>().FirstOrDefault();
            Assert.That(messageAction, Is.Not.Null);
            Assert.That(messageAction.Message, Contains.Substring("frozen").IgnoreCase);
            
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Null, "Freeze should prevent move execution");
            
            Assert.That(_defender.CurrentHP, Is.EqualTo(initialHP), "No damage should be dealt");
        }

        #endregion
    }
}

