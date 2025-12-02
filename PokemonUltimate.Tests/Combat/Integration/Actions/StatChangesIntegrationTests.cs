using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration.Actions
{
    /// <summary>
    /// Integration tests for Stat Changes - verifies stat changes integrate with DamagePipeline and TurnOrderResolver.
    /// </summary>
    [TestFixture]
    public class StatChangesIntegrationTests
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

        #region Stat Changes -> DamagePipeline Integration

        [Test]
        public void StatChangeAction_AttackStage_ThenPhysicalMove_IncreasesDamage()
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

            // Calculate damage WITHOUT stat change (baseline) - use fixed random for consistency
            var pipelineWithoutStatChange = new DamagePipeline();
            var contextWithoutStatChange = pipelineWithoutStatChange.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field, fixedRandomValue: 1.0f);
            float damageWithoutStatChange = contextWithoutStatChange.FinalDamage;

            // Apply Attack +2 stat change
            var statChangeAction = new StatChangeAction(_attackerSlot, _attackerSlot, Stat.Attack, 2);
            statChangeAction.ExecuteLogic(_field);

            // Act - Calculate damage WITH stat change - use same fixed random
            var pipelineWithStatChange = new DamagePipeline();
            var contextWithStatChange = pipelineWithStatChange.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field, fixedRandomValue: 1.0f);
            float damageWithStatChange = contextWithStatChange.FinalDamage;

            // Assert - Attack +2 should increase damage
            Assert.That(damageWithStatChange, Is.GreaterThan(damageWithoutStatChange));
            
            // Attack +2 = (2+2)/2 = 2.0x multiplier for attack stat
            // However, final damage includes other factors (STAB, type effectiveness, random factor)
            // So the ratio won't be exactly 2.0x, but should be significantly higher
            float actualRatio = damageWithStatChange / damageWithoutStatChange;
            Assert.That(actualRatio, Is.GreaterThan(1.5f), 
                "Attack +2 should significantly increase physical damage (ratio > 1.5x)");
            Assert.That(actualRatio, Is.LessThan(2.5f), 
                "Attack +2 should not exceed reasonable damage increase (ratio < 2.5x)");
        }

        [Test]
        public void StatChangeAction_DefenseStage_ThenPhysicalMove_ReducesDamage()
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

            // Calculate damage WITHOUT stat change (baseline)
            var pipelineWithoutStatChange = new DamagePipeline();
            var contextWithoutStatChange = pipelineWithoutStatChange.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field);
            float damageWithoutStatChange = contextWithoutStatChange.FinalDamage;

            // Apply Defense +2 stat change to defender
            var statChangeAction = new StatChangeAction(_defenderSlot, _defenderSlot, Stat.Defense, 2);
            statChangeAction.ExecuteLogic(_field);

            // Act - Calculate damage WITH stat change
            var pipelineWithStatChange = new DamagePipeline();
            var contextWithStatChange = pipelineWithStatChange.Calculate(_attackerSlot, _defenderSlot, physicalMove, _field);
            float damageWithStatChange = contextWithStatChange.FinalDamage;

            // Assert - Defense +2 should reduce damage
            Assert.That(damageWithStatChange, Is.LessThan(damageWithoutStatChange));
            
            // Defense +2 = (2+2)/2 = 2.0x multiplier for defense stat
            // However, final damage includes other factors (STAB, type effectiveness, random factor)
            // So the ratio won't be exactly 0.5x, but should be significantly lower
            float actualRatio = damageWithStatChange / damageWithoutStatChange;
            Assert.That(actualRatio, Is.LessThan(0.7f), 
                "Defense +2 should significantly reduce physical damage (ratio < 0.7x)");
            Assert.That(actualRatio, Is.GreaterThan(0.3f), 
                "Defense +2 should not reduce damage too much (ratio > 0.3x)");
        }

        [Test]
        public void StatChangeAction_SpAttackStage_ThenSpecialMove_IncreasesDamage()
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

            // Calculate damage WITHOUT stat change (baseline) - use fixed random for consistency
            var pipelineWithoutStatChange = new DamagePipeline();
            var contextWithoutStatChange = pipelineWithoutStatChange.Calculate(_attackerSlot, _defenderSlot, specialMove, _field, fixedRandomValue: 1.0f);
            float damageWithoutStatChange = contextWithoutStatChange.FinalDamage;

            // Apply SpAttack +2 stat change
            var statChangeAction = new StatChangeAction(_attackerSlot, _attackerSlot, Stat.SpAttack, 2);
            statChangeAction.ExecuteLogic(_field);

            // Act - Calculate damage WITH stat change - use same fixed random
            var pipelineWithStatChange = new DamagePipeline();
            var contextWithStatChange = pipelineWithStatChange.Calculate(_attackerSlot, _defenderSlot, specialMove, _field, fixedRandomValue: 1.0f);
            float damageWithStatChange = contextWithStatChange.FinalDamage;

            // Assert - SpAttack +2 should increase damage
            Assert.That(damageWithStatChange, Is.GreaterThan(damageWithoutStatChange));
            
            // SpAttack +2 = (2+2)/2 = 2.0x multiplier for SpAttack stat
            // However, final damage includes other factors (STAB, type effectiveness, random factor)
            // So the ratio won't be exactly 2.0x, but should be significantly higher
            float actualRatio = damageWithStatChange / damageWithoutStatChange;
            Assert.That(actualRatio, Is.GreaterThan(1.5f), 
                "SpAttack +2 should significantly increase special damage (ratio > 1.5x)");
            Assert.That(actualRatio, Is.LessThan(2.5f), 
                "SpAttack +2 should not exceed reasonable damage increase (ratio < 2.5x)");
        }

        [Test]
        public void StatChangeAction_ThenUseMoveAction_UsesModifiedDamage()
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

            // Apply Attack +2 stat change
            var statChangeAction = new StatChangeAction(_attackerSlot, _attackerSlot, Stat.Attack, 2);
            statChangeAction.ExecuteLogic(_field);

            int initialHP = _defender.CurrentHP;

            // Act - Use move with stat change
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            damageAction?.ExecuteLogic(_field);

            // Assert - Damage should be increased
            Assert.That(_defender.CurrentHP, Is.LessThan(initialHP));
            
            // Verify damage is higher than it would be without stat change
            // (indirect verification - the actual damage increase is what matters)
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
        }

        #endregion

        #region Stat Changes -> TurnOrderResolver Integration

        [Test]
        public void StatChangeAction_SpeedStage_ThenTurnOrderResolver_AffectsOrder()
        {
            // Arrange - Two Pokemon with different base speeds
            // Pikachu (base speed ~90) vs Charmander (base speed ~65)
            // Pikachu should normally go first
            
            var pikachuSlot = _attackerSlot;
            var charmanderSlot = _defenderSlot;
            
            // Get initial speed order
            float pikachuSpeedBefore = TurnOrderResolver.GetEffectiveSpeed(pikachuSlot, _field);
            float charmanderSpeedBefore = TurnOrderResolver.GetEffectiveSpeed(charmanderSlot, _field);
            
            // Pikachu should be faster initially
            Assert.That(pikachuSpeedBefore, Is.GreaterThan(charmanderSpeedBefore), 
                "Pikachu should be faster than Charmander initially");

            // Apply Speed -2 to Pikachu (Paralysis-like effect)
            var statChangeAction = new StatChangeAction(pikachuSlot, pikachuSlot, Stat.Speed, -2);
            statChangeAction.ExecuteLogic(_field);

            // Act - Get speed after stat change
            float pikachuSpeedAfter = TurnOrderResolver.GetEffectiveSpeed(pikachuSlot, _field);
            float charmanderSpeedAfter = TurnOrderResolver.GetEffectiveSpeed(charmanderSlot, _field);

            // Assert - Pikachu's speed should be reduced
            Assert.That(pikachuSpeedAfter, Is.LessThan(pikachuSpeedBefore));
            
            // Speed -2 = 2/(2-(-2)) = 2/4 = 0.5x multiplier
            float expectedRatio = 0.5f;
            float actualRatio = pikachuSpeedAfter / pikachuSpeedBefore;
            Assert.That(actualRatio, Is.EqualTo(expectedRatio).Within(0.1f), 
                "Speed -2 should approximately halve speed");
        }

        [Test]
        public void StatChangeAction_SpeedStage_ThenCombatEngine_AffectsTurnOrder()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            // Create action providers that return actions
            var playerAction = new MessageAction("Player move");
            var enemyAction = new MessageAction("Enemy move");
            var playerProvider = new TestActionProvider(playerAction);
            var enemyProvider = new TestActionProvider(enemyAction);
            
            engine.Initialize(rules, playerParty, enemyParty, playerProvider, enemyProvider, view);

            // Get initial turn order (Pikachu should go first)
            var actionsBefore = new[] { playerAction, enemyAction };
            var sortedBefore = TurnOrderResolver.SortActions(actionsBefore, engine.Field);
            
            // Apply Speed -2 to player (Pikachu)
            var statChangeAction = new StatChangeAction(
                engine.Field.PlayerSide.Slots[0], 
                engine.Field.PlayerSide.Slots[0], 
                Stat.Speed, 
                -2);
            statChangeAction.ExecuteLogic(engine.Field);

            // Act - Get turn order after stat change
            var actionsAfter = new[] { playerAction, enemyAction };
            var sortedAfter = TurnOrderResolver.SortActions(actionsAfter, engine.Field);

            // Assert - Enemy (Charmander) should now go first due to speed reduction
            // Note: This is probabilistic due to random tiebreaker, but with significant speed difference,
            // Charmander should go first most of the time
            // We verify that speed was reduced, which affects the ordering
            float playerSpeedAfter = TurnOrderResolver.GetEffectiveSpeed(engine.Field.PlayerSide.Slots[0], engine.Field);
            float enemySpeedAfter = TurnOrderResolver.GetEffectiveSpeed(engine.Field.EnemySide.Slots[0], engine.Field);
            
            Assert.That(playerSpeedAfter, Is.LessThan(enemySpeedAfter), 
                "After Speed -2, enemy should be faster than player");
        }

        #endregion
    }
}

