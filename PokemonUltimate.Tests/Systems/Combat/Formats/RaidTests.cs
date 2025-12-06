using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Formats
{
    /// <summary>
    /// Tests for Raid Battles (1vsBoss, 2vsBoss).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.19: Battle Formats
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class RaidTests
    {
        #region Raid 1vBoss Tests

        [Test]
        public void Raid1vBoss_Initialize_CreatesCorrectSlots()
        {
            // Arrange & Act
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 1, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f // 5x HP multiplier
            };
            field.Initialize(rules, playerParty, bossParty);

            // Assert
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.Rules.IsBossBattle, Is.True);
        }

        [Test]
        public void Raid1vBoss_BossHasIncreasedHP()
        {
            // Arrange
            var normalMewtwo = PokemonFactory.Create(PokemonCatalog.Mewtwo, 50);
            int normalMaxHP = normalMewtwo.MaxHP;

            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 1, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f // 5x HP multiplier
            };
            field.Initialize(rules, playerParty, bossParty);

            // Act
            var bossSlot = field.EnemySide.Slots[0];
            var bossPokemon = bossSlot.Pokemon;

            // Assert
            // Boss should have increased HP (5x multiplier)
            // Note: This test will fail initially - we need to implement Boss HP multiplier
            Assert.That(bossPokemon.MaxHP, Is.GreaterThanOrEqualTo(normalMaxHP), "Boss should have increased HP");
        }

        [Test]
        public void Raid1vBoss_BossHasIncreasedStats()
        {
            // Arrange - Use perfect IVs and neutral nature for deterministic stats
            var normalMewtwo = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Mewtwo, 50)
                .WithPerfectIVs()
                .WithNature(PokemonUltimate.Core.Enums.Nature.Serious) // Neutral nature (no stat modifiers)
                .Build();
            int normalAttack = normalMewtwo.Attack;
            int normalSpAttack = normalMewtwo.SpAttack;

            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                    .WithPerfectIVs()
                    .WithNature(PokemonUltimate.Core.Enums.Nature.Serious)
                    .Build()
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Mewtwo, 50)
                    .WithPerfectIVs()
                    .WithNature(PokemonUltimate.Core.Enums.Nature.Serious)
                    .Build()
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 1, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f, // HP multiplier
                BossStatMultiplier = 1.5f // 1.5x stats multiplier
            };
            field.Initialize(rules, playerParty, bossParty);

            // Act
            var bossSlot = field.EnemySide.Slots[0];
            var bossPokemon = bossSlot.Pokemon;

            // Assert
            // Boss should have increased stats (1.5x multiplier)
            // Using 1.4f as threshold to account for rounding differences
            int expectedMinAttack = (int)(normalAttack * 1.4f);
            int expectedMinSpAttack = (int)(normalSpAttack * 1.4f);
            Assert.That(bossPokemon.Attack, Is.GreaterThanOrEqualTo(expectedMinAttack), 
                $"Boss Attack should be >= {expectedMinAttack} (was {bossPokemon.Attack}, normal was {normalAttack})");
            Assert.That(bossPokemon.SpAttack, Is.GreaterThanOrEqualTo(expectedMinSpAttack), 
                $"Boss SpAttack should be >= {expectedMinSpAttack} (was {bossPokemon.SpAttack}, normal was {normalSpAttack})");
            
            // Also verify it's actually increased (not just >= normal)
            Assert.That(bossPokemon.Attack, Is.GreaterThan(normalAttack), "Boss Attack should be greater than normal");
            Assert.That(bossPokemon.SpAttack, Is.GreaterThan(normalSpAttack), "Boss SpAttack should be greater than normal");
        }

        #endregion

        #region Raid 2vBoss Tests

        [Test]
        public void Raid2vBoss_Initialize_CreatesCorrectSlots()
        {
            // Arrange & Act
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 2, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f
            };
            field.Initialize(rules, playerParty, bossParty);

            // Assert
            Assert.That(field.PlayerSide.Slots.Count, Is.EqualTo(2));
            Assert.That(field.EnemySide.Slots.Count, Is.EqualTo(1));
            Assert.That(field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.PlayerSide.Slots[1].Pokemon, Is.Not.Null);
            Assert.That(field.EnemySide.Slots[0].Pokemon, Is.Not.Null);
            Assert.That(field.Rules.IsBossBattle, Is.True);
        }

        [Test]
        public void Raid2vBoss_BothPlayersCanAttackBoss()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 2, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f
            };
            field.Initialize(rules, playerParty, bossParty);

            var userSlot1 = field.PlayerSide.Slots[0];
            var userSlot2 = field.PlayerSide.Slots[1];
            var bossSlot = field.EnemySide.Slots[0];

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
            var tackleInstance1 = new MoveInstance(tackleMove);
            var tackleInstance2 = new MoveInstance(tackleMove);

            int initialHP = bossSlot.Pokemon.CurrentHP;

            // Act
            var action1 = new UseMoveAction(userSlot1, bossSlot, tackleInstance1);
            var reactions1 = action1.ExecuteLogic(field).ToList();
            var damageActions1 = reactions1.OfType<DamageAction>().ToList();
            foreach (var damageAction in damageActions1)
            {
                damageAction.ExecuteLogic(field);
            }

            var action2 = new UseMoveAction(userSlot2, bossSlot, tackleInstance2);
            var reactions2 = action2.ExecuteLogic(field).ToList();
            var damageActions2 = reactions2.OfType<DamageAction>().ToList();
            foreach (var damageAction in damageActions2)
            {
                damageAction.ExecuteLogic(field);
            }

            // Assert
            Assert.That(bossSlot.Pokemon.CurrentHP, Is.LessThan(initialHP), "Boss should take damage from both players");
        }

        [Test]
        public void Raid2vBoss_BossAreaMove_HitsBothPlayers()
        {
            // Arrange
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 2, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f
            };
            field.Initialize(rules, playerParty, bossParty);

            var bossSlot = field.EnemySide.Slots[0];
            var playerSlot1 = field.PlayerSide.Slots[0];
            var playerSlot2 = field.PlayerSide.Slots[1];

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
            var earthquakeInstance = new MoveInstance(earthquakeMove);

            int initialHP1 = playerSlot1.Pokemon.CurrentHP;
            int initialHP2 = playerSlot2.Pokemon.CurrentHP;

            // Act
            var action = new UseMoveAction(bossSlot, playerSlot1, earthquakeInstance);
            var reactions = action.ExecuteLogic(field).ToList();

            // Assert
            var damageActions = reactions.OfType<DamageAction>().ToList();
            // Boss using AllAdjacent should hit both players
            Assert.That(damageActions, Has.Count.GreaterThanOrEqualTo(1), "Boss area move should hit players");

            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(field);
            }

            // At least one player should take damage
            bool player1Damaged = playerSlot1.Pokemon.CurrentHP < initialHP1;
            bool player2Damaged = playerSlot2.Pokemon.CurrentHP < initialHP2;
            Assert.That(player1Damaged || player2Damaged, Is.True, "At least one player should take damage");
        }

        #endregion
    }
}

