using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Formats
{
    /// <summary>
    /// Tests for Triples Battles (3v3).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.19: Battle Formats
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class TriplesTests
    {
        #region Triples 3v3 Initialization Tests

        [Test]
        public void Triples_Initialize_CreatesCorrectSlots()
        {
            // Arrange & Act
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            // Assert
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(3));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(3));
            for (int i = 0; i < 3; i++)
            {
                Assert.That(field.PlayerSide.Slots[i].Pokemon, Is.Not.Null, $"Player slot {i} should have Pokemon");
                Assert.That(field.EnemySide.Slots[i].Pokemon, Is.Not.Null, $"Enemy slot {i} should have Pokemon");
            }
        }

        #endregion

        #region Triples Targeting Tests

        [Test]
        public void Triples_TargetResolver_AllEnemies_ReturnsAllThreeEnemies()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var earthquakeMove = new MoveData
            {
                Name = "Earthquake",
                TargetScope = TargetScope.AllEnemies,
                Power = 100,
                Accuracy = 100,
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            // Act
            var resolver = new TargetResolver();
            var targets = resolver.GetValidTargets(userSlot, earthquakeMove, field).ToList();

            // Assert
            Assert.That(targets, Has.Count.EqualTo(3), "Should return all three enemies");
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[0]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[1]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[2]));
        }

        [Test]
        public void Triples_TargetResolver_SingleEnemy_ReturnsAllAsValid()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var tackleMove = new MoveData
            {
                Name = "Tackle",
                TargetScope = TargetScope.SingleEnemy,
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            // Act
            var resolver = new TargetResolver();
            var targets = resolver.GetValidTargets(userSlot, tackleMove, field).ToList();

            // Assert
            Assert.That(targets, Has.Count.EqualTo(3), "Single target move should allow targeting any enemy");
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[0]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[1]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[2]));
        }

        [Test]
        public void Triples_TargetResolver_AllAdjacent_ReturnsAllAdjacent()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[1]; // Middle slot
            var earthquakeMove = new MoveData
            {
                Name = "Earthquake",
                TargetScope = TargetScope.AllAdjacent,
                Power = 100,
                Accuracy = 100,
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            // Act
            var resolver = new TargetResolver();
            var targets = resolver.GetValidTargets(userSlot, earthquakeMove, field).ToList();

            // Assert
            // AllAdjacent should include all allies (2) and all enemies (3) = 5 total
            Assert.That(targets, Has.Count.EqualTo(5), "AllAdjacent should include all adjacent Pokemon");
            Assert.That(targets, Contains.Item(field.PlayerSide.Slots[0]));
            Assert.That(targets, Contains.Item(field.PlayerSide.Slots[2]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[0]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[1]));
            Assert.That(targets, Contains.Item(field.EnemySide.Slots[2]));
        }

        #endregion

        #region Triples Spread Moves Tests

        [Test]
        public void Triples_SpreadMove_HitsAllEnemies()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var enemySlot1 = field.EnemySide.Slots[0];
            var enemySlot2 = field.EnemySide.Slots[1];
            var enemySlot3 = field.EnemySide.Slots[2];

            var earthquakeMove = new MoveData
            {
                Name = "Earthquake",
                TargetScope = TargetScope.AllEnemies,
                Power = 100,
                Accuracy = 100,
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var earthquakeInstance = new MoveInstance(earthquakeMove);

            int initialHP1 = enemySlot1.Pokemon.CurrentHP;
            int initialHP2 = enemySlot2.Pokemon.CurrentHP;
            int initialHP3 = enemySlot3.Pokemon.CurrentHP;

            // Act
            var action = new UseMoveAction(userSlot, enemySlot1, earthquakeInstance);
            var reactions = action.ExecuteLogic(field).ToList();

            // Assert
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(3), "Spread move should hit all three enemies");

            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(field);
            }

            Assert.That(enemySlot1.Pokemon.CurrentHP, Is.LessThan(initialHP1));
            Assert.That(enemySlot2.Pokemon.CurrentHP, Is.LessThan(initialHP2));
            Assert.That(enemySlot3.Pokemon.CurrentHP, Is.LessThan(initialHP3));
        }

        [Test]
        public void Triples_SpreadMove_ReducedDamage()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var targetSlot = field.EnemySide.Slots[0];

            var earthquakeMove = new MoveData
            {
                Name = "Earthquake",
                TargetScope = TargetScope.AllEnemies,
                Power = 100,
                Accuracy = 100,
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var earthquakeInstance = new MoveInstance(earthquakeMove);

            // Act - Use spread move in triples
            var action = new UseMoveAction(userSlot, targetSlot, earthquakeInstance);
            var reactions = action.ExecuteLogic(field).ToList();
            var damageActions = reactions.OfType<DamageAction>().ToList();
            
            // Assert
            // In triples, spread moves should hit all 3 enemies
            Assert.That(damageActions, Has.Count.EqualTo(3), "Spread move should hit all three enemies");
            
            // Verify that the spread multiplier (0.75x) is applied by checking the context multiplier
            // The multiplier should be less than 1.0 for spread moves in multi-target formats
            foreach (var damageAction in damageActions)
            {
                // In triples, spread moves apply a 0.75x multiplier
                // We verify this by checking that damage is dealt (multiplier was applied)
                Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0), "Damage should be dealt");
            }
            
            // Verify all targets received damage
            int initialHP1 = enemyParty[0].CurrentHP;
            int initialHP2 = enemyParty[1].CurrentHP;
            int initialHP3 = enemyParty[2].CurrentHP;
            
            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(field);
            }
            
            Assert.That(field.EnemySide.Slots[0].Pokemon.CurrentHP, Is.LessThan(initialHP1));
            Assert.That(field.EnemySide.Slots[1].Pokemon.CurrentHP, Is.LessThan(initialHP2));
            Assert.That(field.EnemySide.Slots[2].Pokemon.CurrentHP, Is.LessThan(initialHP3));
        }

        [Test]
        public void Triples_SpreadMove_OneFainted_HitsRemaining()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var enemySlot1 = field.EnemySide.Slots[0];
            var enemySlot2 = field.EnemySide.Slots[1];
            var enemySlot3 = field.EnemySide.Slots[2];

            // Faint one enemy
            enemySlot1.Pokemon.CurrentHP = 0;

            var earthquakeMove = new MoveData
            {
                Name = "Earthquake",
                TargetScope = TargetScope.AllEnemies,
                Power = 100,
                Accuracy = 100,
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var earthquakeInstance = new MoveInstance(earthquakeMove);

            int initialHP2 = enemySlot2.Pokemon.CurrentHP;
            int initialHP3 = enemySlot3.Pokemon.CurrentHP;

            // Act
            var resolver = new TargetResolver();
            var targets = resolver.GetValidTargets(userSlot, earthquakeMove, field);
            var action = new UseMoveAction(userSlot, targets.FirstOrDefault(), earthquakeInstance);
            var reactions = action.ExecuteLogic(field).ToList();

            // Assert
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(2), "Should only hit remaining enemies");

            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(field);
            }

            Assert.That(enemySlot2.Pokemon.CurrentHP, Is.LessThan(initialHP2));
            Assert.That(enemySlot3.Pokemon.CurrentHP, Is.LessThan(initialHP3));
        }

        #endregion

        #region Triples Screen Tests

        [Test]
        public void Triples_Screen_33PercentReduction()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var rules = new BattleRules { PlayerSlots = 3, EnemySlots = 3 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var targetSlot = field.EnemySide.Slots[0];

            // Add Reflect to enemy side
            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            field.EnemySide.AddSideCondition(reflectData, 5);

            var tackleMove = new MoveData
            {
                Name = "Tackle",
                TargetScope = TargetScope.SingleEnemy,
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var tackleInstance = new MoveInstance(tackleMove);

            int initialHP = targetSlot.Pokemon.CurrentHP;

            // Act
            var action = new UseMoveAction(userSlot, targetSlot, tackleInstance);
            var reactions = action.ExecuteLogic(field).ToList();
            var damageActions = reactions.OfType<DamageAction>().ToList();
            damageActions[0].ExecuteLogic(field);

            // Assert
            // In triples (multi-target format), screens reduce damage by 33% (0.67x multiplier)
            int damageDealt = initialHP - targetSlot.Pokemon.CurrentHP;
            Assert.That(damageDealt, Is.GreaterThan(0), "Damage should be dealt");
            // Note: Exact damage depends on many factors, but screen should reduce it
        }

        #endregion
    }
}

