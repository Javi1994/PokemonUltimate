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
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Formats
{
    /// <summary>
    /// Tests for Horde Battles (1vs2, 1vs3, 1vs5).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.19: Battle Formats
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class HordeTests
    {

        #region Horde 1v2 Tests

        [Test]
        public void Horde1v2_Initialize_CreatesCorrectSlots()
        {
            // Arrange & Act
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 2 };
            field.Initialize(rules, playerParty, enemyParty);

            // Assert
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(2));
            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[1].Pokemon, Is.Not.Null);
        }

        [Test]
        public void Horde1v2_SpreadMove_HitsBothEnemies()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 2 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var enemySlot1 = field.EnemySide.Slots[0];
            var enemySlot2 = field.EnemySide.Slots[1];

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

            // Act
            var action = new UseMoveAction(userSlot, enemySlot1, earthquakeInstance);
            var reactions = action.ExecuteLogic(field).ToList();

            // Assert
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(2), "Spread move should hit both enemies");

            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(field);
            }

            Assert.That(enemySlot1.Pokemon.CurrentHP, Is.LessThan(initialHP1));
            Assert.That(enemySlot2.Pokemon.CurrentHP, Is.LessThan(initialHP2));
        }

        [Test]
        public void Horde1v2_SingleTarget_SelectsSpecificEnemy()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 2 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var targetSlot = field.EnemySide.Slots[0];
            var otherSlot = field.EnemySide.Slots[1];

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
            int otherHP = otherSlot.Pokemon.CurrentHP;

            // Act
            var action = new UseMoveAction(userSlot, targetSlot, tackleInstance);
            var reactions = action.ExecuteLogic(field).ToList();

            // Assert
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(1), "Single target move should hit only selected enemy");

            damageActions[0].ExecuteLogic(field);

            Assert.That(targetSlot.Pokemon.CurrentHP, Is.LessThan(initialHP));
            Assert.That(otherSlot.Pokemon.CurrentHP, Is.EqualTo(otherHP), "Other enemy should not be hit");
        }

        #endregion

        #region Horde 1v3 Tests

        [Test]
        public void Horde1v3_Initialize_CreatesCorrectSlots()
        {
            // Arrange & Act
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };

            field.Initialize(BattleRules.Horde, playerParty, enemyParty);

            // Assert
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(3));
            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[1].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[2].Pokemon, Is.Not.Null);
        }

        [Test]
        public void Horde1v3_SpreadMove_HitsAllEnemies()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };

            field.Initialize(BattleRules.Horde, playerParty, enemyParty);

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
        public void Horde1v3_OneFainted_HitsRemaining()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };

            field.Initialize(BattleRules.Horde, playerParty, enemyParty);

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

        #region Horde 1v5 Tests

        [Test]
        public void Horde1v5_Initialize_CreatesCorrectSlots()
        {
            // Arrange & Act
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 5 };
            field.Initialize(rules, playerParty, enemyParty);

            // Assert
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(5));
            for (int i = 0; i < 5; i++)
            {
                Assert.That(field.EnemySide.Slots[i].Pokemon, Is.Not.Null, $"Enemy slot {i} should have Pokemon");
            }
        }

        [Test]
        public void Horde1v5_SpreadMove_HitsAllEnemies()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 5 };
            field.Initialize(rules, playerParty, enemyParty);

            var userSlot = field.PlayerSide.Slots[0];
            var enemySlots = field.EnemySide.Slots.ToList();

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

            var initialHPs = enemySlots.Select(s => s.Pokemon.CurrentHP).ToList();

            // Act
            var action = new UseMoveAction(userSlot, enemySlots[0], earthquakeInstance);
            var reactions = action.ExecuteLogic(field).ToList();

            // Assert
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions, Has.Count.EqualTo(5), "Spread move should hit all five enemies");

            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(field);
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.That(enemySlots[i].Pokemon.CurrentHP, Is.LessThan(initialHPs[i]), $"Enemy {i} should take damage");
            }
        }

        #endregion
    }
}

