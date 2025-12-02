using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration.Damage
{
    /// <summary>
    /// Integration tests for DamagePipeline - verifies damage calculation integrates with UseMoveAction and DamageAction.
    /// </summary>
    [TestFixture]
    public class DamagePipelineIntegrationTests
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

        #region UseMoveAction -> DamagePipeline Integration

        [Test]
        public void UseMoveAction_DamageMove_UsesDamagePipeline()
        {
            // Arrange
            var moveInstance = _attacker.Moves.FirstOrDefault(m => m.Move.Power > 0);
            if (moveInstance == null)
            {
                Assert.Inconclusive("No damaging move found");
                return;
            }

            int initialHP = _defender.CurrentHP;
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);

            // Act
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Assert
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
        }

        [Test]
        public void UseMoveAction_DamageMove_AppliesDamageCorrectly()
        {
            // Arrange
            var moveInstance = _attacker.Moves.FirstOrDefault(m => m.Move.Power > 0);
            if (moveInstance == null)
            {
                Assert.Inconclusive("No damaging move found");
                return;
            }

            int initialHP = _defender.CurrentHP;
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);

            // Act
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            damageAction?.ExecuteLogic(_field);

            // Assert
            Assert.That(_defender.CurrentHP, Is.LessThan(initialHP));
        }

        #endregion

        #region DamagePipeline -> DamageAction Integration

        [Test]
        public void DamagePipeline_CalculatesDamage_DamageActionUsesResult()
        {
            // Arrange
            var move = MoveCatalog.Thunderbolt;
            var pipeline = new DamagePipeline();

            // Act
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field);
            var damageAction = new DamageAction(_attackerSlot, _defenderSlot, context);

            // Assert
            Assert.That(context, Is.Not.Null);
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
            Assert.That(damageAction.Context, Is.EqualTo(context));
        }

        [Test]
        public void DamagePipeline_TypeEffectiveness_ReflectedInDamage()
        {
            // Arrange - Electric vs Water should be super effective
            var electricMove = MoveCatalog.Thunderbolt;
            var waterPokemon = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            _defenderSlot.SetPokemon(waterPokemon);

            var pipeline = new DamagePipeline();

            // Act
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, electricMove, _field);

            // Assert
            Assert.That(context.TypeEffectiveness, Is.GreaterThan(1.0f)); // Super effective
            Assert.That(context.FinalDamage, Is.GreaterThan(0));
        }

        [Test]
        public void DamagePipeline_STAB_AppliedCorrectly()
        {
            // Arrange - Pikachu using Electric move should have STAB
            var electricMove = MoveCatalog.Thunderbolt;
            var pipeline = new DamagePipeline();

            // Act
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, electricMove, _field);

            // Assert
            Assert.That(context.IsStab, Is.True);
            // STAB should increase damage (multiplier > 1.0)
            Assert.That(context.Multiplier, Is.GreaterThan(1.0f));
        }

        #endregion

        #region DamagePipeline Steps Integration

        [Test]
        public void DamagePipeline_AllSteps_ExecuteInOrder()
        {
            // Arrange
            var move = MoveCatalog.Thunderbolt;
            var pipeline = new DamagePipeline();

            // Act
            var context = pipeline.Calculate(_attackerSlot, _defenderSlot, move, _field);

            // Assert - Verify all steps have been applied
            Assert.That(context.BaseDamage, Is.GreaterThan(0));
            Assert.That(context.Multiplier, Is.GreaterThan(0));
            Assert.That(context.TypeEffectiveness, Is.GreaterThan(0));
            Assert.That(context.RandomFactor, Is.GreaterThanOrEqualTo(0.85f));
            Assert.That(context.RandomFactor, Is.LessThanOrEqualTo(1.0f));
        }

        [Test]
        public void DamagePipeline_FixedDamage_DragonRage_AlwaysDeals40()
        {
            // Arrange - Dragon Rage always deals exactly 40 HP damage
            var gyarados = PokemonFactory.Create(PokemonCatalog.Gyarados, 50);
            var weakDefender = PokemonFactory.Create(PokemonCatalog.Magikarp, 50);
            var strongDefender = PokemonFactory.Create(PokemonCatalog.Snorlax, 50);
            
            var fieldWeak = new BattleField();
            fieldWeak.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { gyarados },
                new[] { weakDefender });
            
            var fieldStrong = new BattleField();
            fieldStrong.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { gyarados },
                new[] { strongDefender });

            var dragonRage = MoveCatalog.DragonRage;
            var pipeline = new DamagePipeline();

            // Act
            var contextWeak = pipeline.Calculate(
                fieldWeak.PlayerSide.Slots[0],
                fieldWeak.EnemySide.Slots[0],
                dragonRage,
                fieldWeak,
                fixedRandomValue: 1.0f);
            
            var contextStrong = pipeline.Calculate(
                fieldStrong.PlayerSide.Slots[0],
                fieldStrong.EnemySide.Slots[0],
                dragonRage,
                fieldStrong,
                fixedRandomValue: 1.0f);

            // Assert - Fixed damage should be exactly 40 regardless of stats
            Assert.That(contextWeak.FinalDamage, Is.EqualTo(40), "Dragon Rage should deal exactly 40 HP to weak Pokemon");
            Assert.That(contextStrong.FinalDamage, Is.EqualTo(40), "Dragon Rage should deal exactly 40 HP to strong Pokemon");
        }

        #endregion
    }
}

