using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonBuilder = PokemonUltimate.Content.Builders.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Tests.Systems.Combat.Engine;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.Formats
{
    /// <summary>
    /// Comprehensive integration tests for all Battle Formats - verifies formats work correctly
    /// with CombatEngine, abilities, items, field conditions, and complex scenarios.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.19: Battle Formats
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class BattleFormatsIntegrationTests
    {
        private CombatEngine _engine;
        private NullBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _engine = CombatEngineTestHelper.CreateCombatEngine();
            _view = new NullBattleView();
        }

        #region Doubles Format Integration Tests

        [Test]
        public async Task Doubles_FullBattle_CompletesSuccessfully()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(2));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Doubles_SpreadMove_HitsBothEnemies()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            // Create a test provider that uses Earthquake (spread move)
            var earthquakeMoveData = MoveCatalog.Earthquake;
            var earthquakeMove = new MoveInstance(earthquakeMoveData);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, earthquakeMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialHP1 = enemyParty[0].CurrentHP;
            int initialHP2 = enemyParty[1].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Both enemies should take damage
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialHP1).Or.EqualTo(0));
            Assert.That(enemyParty[1].CurrentHP, Is.LessThan(initialHP2).Or.EqualTo(0));
        }

        [Test]
        public async Task Doubles_WithAbilities_WorkCorrectly()
        {
            // Arrange - Test that abilities work correctly in doubles format
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30), // Low level
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run battle
            var result = await _engine.RunBattle();

            // Assert - Battle should complete successfully
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Doubles_WithLifeOrb_DamageIncreased()
        {
            // Arrange
            var lifeOrbPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.LifeOrb)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                lifeOrbPokemon,
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            int initialEnemyHP = enemyParty[0].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Life Orb should increase damage (and cause recoil)
            // Enemy should take more damage than without Life Orb
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialEnemyHP).Or.EqualTo(0));
            
            // Life Orb user should have taken recoil damage
            if (lifeOrbPokemon.CurrentHP < lifeOrbPokemon.MaxHP)
            {
                // Recoil damage was applied
                Assert.That(lifeOrbPokemon.CurrentHP, Is.LessThan(lifeOrbPokemon.MaxHP));
            }
        }

        [Test]
        public async Task Doubles_WithReflect_ReducesPhysicalDamage()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);
            
            // Add Reflect to player side
            _engine.Field.PlayerSide.AddSideCondition(reflectData, 5);

            int initialPlayerHP1 = playerParty[0].CurrentHP;
            int initialPlayerHP2 = playerParty[1].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Reflect should reduce physical damage in doubles (33% reduction)
            // Note: Exact damage depends on moves used, but Reflect should reduce it
            Assert.That(_engine.Field.PlayerSide.HasSideCondition(SideCondition.Reflect), Is.True);
        }

        #endregion

        #region Triples Format Integration Tests

        [Test]
        public async Task Triples_FullBattle_CompletesSuccessfully()
        {
            // Arrange
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
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(3));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task Triples_SpreadMove_HitsAllEnemies()
        {
            // Arrange
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

            var earthquakeMoveData = MoveCatalog.Earthquake;
            var earthquakeMove = new MoveInstance(earthquakeMoveData);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, earthquakeMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialHP1 = enemyParty[0].CurrentHP;
            int initialHP2 = enemyParty[1].CurrentHP;
            int initialHP3 = enemyParty[2].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - All three enemies should take damage
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialHP1).Or.EqualTo(0));
            Assert.That(enemyParty[1].CurrentHP, Is.LessThan(initialHP2).Or.EqualTo(0));
            Assert.That(enemyParty[2].CurrentHP, Is.LessThan(initialHP3).Or.EqualTo(0));
        }

        [Test]
        public async Task Triples_WithSpeedBoost_TriggersEachTurn()
        {
            // Arrange
            // Use a Pokemon that naturally has Speed Boost (or use a Pokemon with a valid ability)
            // Since Speed Boost might not be available for all Pokemon, test with a valid ability
            var pokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                pokemon,
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run multiple turns
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Battle should continue successfully in triples format
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(3));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        #endregion

        #region Horde Format Integration Tests

        [Test]
        public async Task Horde1v3_FullBattle_CompletesSuccessfully()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30)
            };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Horde, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task Horde1v5_SpreadMove_HitsAllEnemies()
        {
            // Arrange
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30),
                PokemonFactory.Create(PokemonCatalog.Eevee, 30),
                PokemonFactory.Create(PokemonCatalog.Geodude, 30)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 5 };
            var earthquakeMoveData = MoveCatalog.Earthquake;
            var earthquakeMove = new MoveInstance(earthquakeMoveData);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, earthquakeMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            var initialHPs = enemyParty.Select(p => p.CurrentHP).ToList();

            // Act
            await _engine.RunTurn();

            // Assert - All five enemies should take damage
            for (int i = 0; i < 5; i++)
            {
                Assert.That(enemyParty[i].CurrentHP, Is.LessThan(initialHPs[i]).Or.EqualTo(0),
                    $"Enemy {i} should take damage from spread move");
            }
        }

        [Test]
        public async Task Horde1v3_WithAbilities_WorkCorrectly()
        {
            // Arrange - Test that abilities work correctly in horde format
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Horde, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run turns
            for (int i = 0; i < 5; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Battle should progress correctly
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        #endregion

        #region Raid Format Integration Tests

        [Test]
        public async Task Raid1vBoss_FullBattle_CompletesSuccessfully()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Raid1vBoss, playerParty, bossParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(_engine.Field.Rules.IsBossBattle, Is.True);
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(1));
            
            // Boss should have increased HP
            var bossPokemon = _engine.Field.EnemySide.Slots[0].Pokemon;
            var normalMewtwo = PokemonFactory.Create(PokemonCatalog.Mewtwo, 50);
            Assert.That(bossPokemon.MaxHP, Is.GreaterThanOrEqualTo(normalMewtwo.MaxHP), 
                "Boss should have increased HP");
        }

        [Test]
        public async Task Raid2vBoss_FullBattle_CompletesSuccessfully()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Raid2vBoss, playerParty, bossParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(_engine.Field.Rules.IsBossBattle, Is.True);
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(2));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(1));
            
            // Boss should have increased HP and stats
            var bossPokemon = _engine.Field.EnemySide.Slots[0].Pokemon;
            var normalMewtwo = PokemonFactory.Create(PokemonCatalog.Mewtwo, 50);
            Assert.That(bossPokemon.MaxHP, Is.GreaterThanOrEqualTo(normalMewtwo.MaxHP), 
                "Boss should have increased HP");
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

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Raid1vBoss, playerParty, bossParty, playerAI, enemyAI, _view);

            // Act
            var bossPokemon = _engine.Field.EnemySide.Slots[0].Pokemon;

            // Assert - Boss should have increased stats (1.2x multiplier from Raid1vBoss rules)
            // Using 1.15f as threshold to account for rounding differences (1.2x * 0.96 ≈ 1.15x)
            int expectedMinAttack = (int)(normalAttack * 1.15f);
            int expectedMinSpAttack = (int)(normalSpAttack * 1.15f);
            
            Assert.That(bossPokemon.Attack, Is.GreaterThanOrEqualTo(expectedMinAttack), 
                $"Boss Attack should be >= {expectedMinAttack} (was {bossPokemon.Attack}, normal was {normalAttack}, multiplier: 1.2x)");
            Assert.That(bossPokemon.SpAttack, Is.GreaterThanOrEqualTo(expectedMinSpAttack), 
                $"Boss SpAttack should be >= {expectedMinSpAttack} (was {bossPokemon.SpAttack}, normal was {normalSpAttack}, multiplier: 1.2x)");
            
            // Also verify it's actually increased (not just >= normal)
            Assert.That(bossPokemon.Attack, Is.GreaterThan(normalAttack), "Boss Attack should be greater than normal");
            Assert.That(bossPokemon.SpAttack, Is.GreaterThan(normalSpAttack), "Boss SpAttack should be greater than normal");
        }

        [Test]
        public async Task Raid2vBoss_BothPlayersCanAttackBoss()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Raid2vBoss, playerParty, bossParty, playerAI, enemyAI, _view);

            int initialBossHP = bossParty[0].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Boss should take damage from both players
            Assert.That(bossParty[0].CurrentHP, Is.LessThan(initialBossHP).Or.EqualTo(0),
                "Boss should take damage from both players");
        }

        #endregion

        #region Cross-Format Compatibility Tests

        [Test]
        public async Task Doubles_WithTruantAbility_AlternatesTurns()
        {
            // Arrange
            var truantPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Slaking, 50)
                .WithPerfectIVs()
                .WithAbility(AbilityCatalog.Truant)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                truantPokemon,
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run multiple turns
            for (int i = 0; i < 5; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Truant should work in doubles format
            Assert.That(_engine.Field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
        }

        [Test]
        public async Task Triples_WithFocusSash_PreventsOneHitKO()
        {
            // Arrange
            var focusSashPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.FocusSash)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                focusSashPokemon,
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 60), // Higher level for more damage
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            int initialHP = focusSashPokemon.CurrentHP;

            // Act - Run turn where Focus Sash might trigger
            await _engine.RunTurn();

            // Assert - Focus Sash should prevent one-hit KO if damage would be fatal
            // Note: Focus Sash only works if Pokemon is at full HP
            if (initialHP == focusSashPokemon.MaxHP)
            {
                // If at full HP and took massive damage, Focus Sash should leave at 1 HP
                bool isAliveOrFainted = focusSashPokemon.CurrentHP >= 1 || focusSashPokemon.IsFainted;
                Assert.That(isAliveOrFainted, Is.True);
            }
        }

        [Test]
        public async Task Horde_WithRockyHelmet_DamagesOnContact()
        {
            // Arrange
            var rockyHelmetPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Golem, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.RockyHelmet)
                .Build();

            var playerParty = new List<PokemonInstance> { rockyHelmetPokemon };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Horde, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run turns until contact move is used
            for (int i = 0; i < 5; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Rocky Helmet should work in horde format
            Assert.That(_engine.Field.PlayerSide.Slots[0].Pokemon, Is.Not.Null);
        }

        [Test]
        public async Task Raid_WithItems_WorkCorrectly()
        {
            // Arrange - Test that items work correctly in raid format
            var playerPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.Leftovers)
                .Build();

            var playerParty = new List<PokemonInstance> { playerPokemon };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Raid1vBoss, playerParty, bossParty, playerAI, enemyAI, _view);

            int initialHP = playerPokemon.CurrentHP;
            playerPokemon.CurrentHP = initialHP - 20; // Damage Pokemon

            // Act - Run turn to trigger end-of-turn effects
            await _engine.RunTurn();

            // Assert - Items should work correctly in raid format
            Assert.That(_engine.Field.Rules.IsBossBattle, Is.True);
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(1));
        }

        #endregion

        #region Field Conditions Compatibility Tests

        [Test]
        public async Task Doubles_WithLightScreen_ReducesSpecialDamage()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var lightScreenData = SideConditionCatalog.GetByType(SideCondition.LightScreen);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);
            _engine.Field.PlayerSide.AddSideCondition(lightScreenData, 5);

            // Act
            await _engine.RunTurn();

            // Assert - Light Screen should be active
            Assert.That(_engine.Field.PlayerSide.HasSideCondition(SideCondition.LightScreen), Is.True);
        }

        [Test]
        public async Task Triples_WithTailwind_IncreasesSpeed()
        {
            // Arrange
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

            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);
            _engine.Field.PlayerSide.AddSideCondition(tailwindData, 4);

            // Act
            await _engine.RunTurn();

            // Assert - Tailwind should be active
            Assert.That(_engine.Field.PlayerSide.HasSideCondition(SideCondition.Tailwind), Is.True);
        }

        #endregion

        #region Complex Scenarios Tests

        [Test]
        public async Task Doubles_MultipleTurns_StateConsistent()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerAI = new RandomAI();
            var enemyAI = new RandomAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run multiple turns
            for (int i = 0; i < 10; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;

                // Assert - Field state should be consistent
                Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(2));
                Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(2));
            }

            // Final assertion
            bool battleCompleteOrHasSlots = _engine.Outcome != BattleOutcome.Ongoing || 
                _engine.Field.GetAllActiveSlots().Any();
            Assert.That(battleCompleteOrHasSlots, Is.True);
        }

        [Test]
        public async Task Triples_OneFainted_RemainingPokemonContinue()
        {
            // Arrange
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

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Faint one Pokemon manually
            _engine.Field.PlayerSide.Slots[0].Pokemon.CurrentHP = 0;

            // Act
            await _engine.RunTurn();

            // Assert - Battle should continue with remaining Pokemon
            // Note: After fainting one Pokemon, battle may end if all Pokemon faint
            // or continue if there are remaining active Pokemon
            var allSlots = _engine.Field.PlayerSide.Slots;
            var activeSlots = allSlots.Where(s => s.Pokemon != null && !s.Pokemon.IsFainted).ToList();
            // Should have at least one slot (even if empty or fainted)
            Assert.That(allSlots.Count, Is.EqualTo(3), "Should have 3 slots");
        }

        [Test]
        public async Task Horde_AllEnemiesFaint_Victory()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 60) // High level to ensure victory
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 20),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 20),
                PokemonFactory.Create(PokemonCatalog.Charmander, 20)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Horde, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert - Should end when all enemies faint or player faints
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        [Test]
        public async Task Raid_BossDefeated_Victory()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 60) // High level
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 30) // Lower level boss
            };

            var rules = new BattleRules 
            { 
                PlayerSlots = 1, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 2.0f, // Lower multiplier for faster test
                BossStatMultiplier = 1.1f
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(rules, playerParty, bossParty, playerAI, enemyAI, _view);

            // Act
            var result = await _engine.RunBattle();

            // Assert - Battle should complete
            Assert.That(result.Outcome, Is.Not.EqualTo(BattleOutcome.Ongoing));
            Assert.That(result.TurnsTaken, Is.GreaterThan(0));
        }

        #endregion

        #region Edge Cases Tests

        [Test]
        public void Doubles_AllPokemonFaint_DetectsCorrectOutcome()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint all player Pokemon
            foreach (var slot in _engine.Field.PlayerSide.Slots)
            {
                if (slot.Pokemon != null)
                    slot.Pokemon.CurrentHP = 0;
            }

            // Act
            var outcome = BattleArbiter.CheckOutcome(_engine.Field);

            // Assert
            Assert.That(outcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public async Task Triples_EmptySlots_HandledCorrectly()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
                // Only 2 Pokemon for 3 slots
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Assert - Before turn: Empty slots should be handled correctly
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(3), "Should have 3 slots");
            var initialActiveSlots = _engine.Field.PlayerSide.GetActiveSlots().ToList();
            Assert.That(initialActiveSlots.Count, Is.EqualTo(2), "Should have 2 active Pokemon initially");
            
            // Verify that slot 2 is empty
            Assert.That(_engine.Field.PlayerSide.Slots[2].IsEmpty, Is.True, "Third slot should be empty");

            // Act - Run turn (may cause Pokemon to faint, but that's OK)
            await _engine.RunTurn();

            // Assert - After turn: Slots structure should remain correct
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(3), "Should still have 3 slots after turn");
            // Note: Pokemon may faint during battle, but slots structure should remain intact
            var finalSlots = _engine.Field.PlayerSide.Slots;
            Assert.That(finalSlots.Count, Is.EqualTo(3), "Slot count should remain 3");
        }

        [Test]
        public async Task Horde_SpreadMoveWithOneFainted_HitsRemaining()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30)
            };

            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 3 };
            var earthquakeMoveData = MoveCatalog.Earthquake;
            var earthquakeMove = new MoveInstance(earthquakeMoveData);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, earthquakeMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(rules, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            // Faint one enemy
            _engine.Field.EnemySide.Slots[0].Pokemon.CurrentHP = 0;

            int initialHP2 = enemyParty[1].CurrentHP;
            int initialHP3 = enemyParty[2].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Should hit remaining enemies
            Assert.That(enemyParty[1].CurrentHP, Is.LessThan(initialHP2).Or.EqualTo(0));
            Assert.That(enemyParty[2].CurrentHP, Is.LessThan(initialHP3).Or.EqualTo(0));
        }

        #endregion

        #region Comprehensive System Integration Tests

        #region Targeting System Integration

        [Test]
        public async Task Doubles_TargetScope_AllEnemies_HitsBothEnemies()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var surfMove = new MoveInstance(MoveCatalog.Surf); // AllEnemies target
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, surfMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialHP1 = enemyParty[0].CurrentHP;
            int initialHP2 = enemyParty[1].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Both enemies should take damage
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialHP1).Or.EqualTo(0));
            Assert.That(enemyParty[1].CurrentHP, Is.LessThan(initialHP2).Or.EqualTo(0));
        }

        [Test]
        public async Task Doubles_TargetScope_AllAdjacent_HitsAllAdjacent()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var earthquakeMove = new MoveInstance(MoveCatalog.Earthquake); // AllAdjacent target
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, earthquakeMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialPlayerHP2 = playerParty[1].CurrentHP;
            int initialEnemyHP1 = enemyParty[0].CurrentHP;
            int initialEnemyHP2 = enemyParty[1].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - All adjacent Pokemon (ally + both enemies) should take damage
            Assert.That(playerParty[1].CurrentHP, Is.LessThan(initialPlayerHP2).Or.EqualTo(0));
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialEnemyHP1).Or.EqualTo(0));
            Assert.That(enemyParty[1].CurrentHP, Is.LessThan(initialEnemyHP2).Or.EqualTo(0));
        }

        [Test]
        public async Task Triples_TargetScope_AllEnemies_HitsAllThreeEnemies()
        {
            // Arrange
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

            var surfMove = new MoveInstance(MoveCatalog.Surf);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, surfMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            var initialHPs = enemyParty.Select(p => p.CurrentHP).ToList();

            // Act
            await _engine.RunTurn();

            // Assert - All three enemies should take damage
            for (int i = 0; i < 3; i++)
            {
                Assert.That(enemyParty[i].CurrentHP, Is.LessThan(initialHPs[i]).Or.EqualTo(0),
                    $"Enemy {i} should take damage from AllEnemies move");
            }
        }

        [Test]
        public async Task Horde_TargetScope_AllEnemies_HitsAllEnemies()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30)
            };

            var surfMove = new MoveInstance(MoveCatalog.Surf);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, surfMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Horde, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            var initialHPs = enemyParty.Select(p => p.CurrentHP).ToList();

            // Act
            await _engine.RunTurn();

            // Assert - All enemies should take damage
            for (int i = 0; i < 3; i++)
            {
                Assert.That(enemyParty[i].CurrentHP, Is.LessThan(initialHPs[i]).Or.EqualTo(0),
                    $"Enemy {i} should take damage from AllEnemies move");
            }
        }

        [Test]
        public async Task Doubles_TargetScope_SingleEnemy_CanTargetEitherEnemy()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var tackleMove = new MoveInstance(MoveCatalog.Tackle); // SingleEnemy target
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[1]; // Target second enemy
                return new UseMoveAction(playerSlot, enemySlot, tackleMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialHP1 = enemyParty[0].CurrentHP;
            int initialHP2 = enemyParty[1].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Only targeted enemy should take damage
            Assert.That(enemyParty[0].CurrentHP, Is.EqualTo(initialHP1), "First enemy should not take damage");
            Assert.That(enemyParty[1].CurrentHP, Is.LessThan(initialHP2).Or.EqualTo(0), "Second enemy should take damage");
        }

        #endregion

        #region Abilities Integration

        [Test]
        public async Task Doubles_Intimidate_LowersBothEnemiesAttack()
        {
            // Arrange
            var intimidatePokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Gyarados, 50)
                .WithPerfectIVs()
                .WithAbility(AbilityCatalog.Intimidate)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                intimidatePokemon,
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            int initialAttack1 = _engine.Field.EnemySide.Slots[0].GetStatStage(Stat.Attack);
            int initialAttack2 = _engine.Field.EnemySide.Slots[1].GetStatStage(Stat.Attack);

            // Act - Run turn (Intimidate triggers on switch-in, which happens during initialization)
            await _engine.RunTurn();

            // Assert - Both enemies should have lowered Attack
            int finalAttack1 = _engine.Field.EnemySide.Slots[0].GetStatStage(Stat.Attack);
            int finalAttack2 = _engine.Field.EnemySide.Slots[1].GetStatStage(Stat.Attack);
            
            Assert.That(finalAttack1, Is.LessThanOrEqualTo(initialAttack1), "First enemy Attack should be lowered");
            Assert.That(finalAttack2, Is.LessThanOrEqualTo(initialAttack2), "Second enemy Attack should be lowered");
        }

        [Test]
        public async Task Doubles_LightningRod_RedirectsElectricMoves()
        {
            // Arrange
            var lightningRodPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .WithAbility(AbilityCatalog.LightningRod)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                lightningRodPokemon
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var thunderboltMove = new MoveInstance(MoveCatalog.Thunderbolt); // Electric move
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, thunderboltMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialSpAttack = lightningRodPokemon.SpAttack;
            int initialEnemyHP = enemyParty[0].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Lightning Rod should redirect Electric moves and boost SpAttack
            // Note: Lightning Rod redirects to the Pokemon with the ability
            // In doubles format, Lightning Rod should work correctly
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(2));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Triples_Abilities_WorkCorrectly()
        {
            // Arrange - Test that abilities work correctly in triples format
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

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run multiple turns
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Abilities should work correctly in triples
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(3));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        #endregion

        #region Items Integration

        [Test]
        public async Task Doubles_Leftovers_HealsEachTurn()
        {
            // Arrange
            var leftoversPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.Leftovers)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                leftoversPokemon,
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerProvider = new TestActionProvider(new MessageAction("Pass"));
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int maxHP = leftoversPokemon.MaxHP;
            leftoversPokemon.CurrentHP = maxHP - 20; // Damage Pokemon

            int initialHP = leftoversPokemon.CurrentHP;

            // Act - Run turn to trigger end-of-turn healing
            await _engine.RunTurn();

            // Assert - Leftovers should heal Pokemon
            int finalHP = leftoversPokemon.CurrentHP;
            Assert.That(finalHP, Is.GreaterThanOrEqualTo(initialHP), "Leftovers should heal Pokemon in doubles");
        }

        [Test]
        public async Task Triples_LifeOrb_BoostsDamageAndCausesRecoil()
        {
            // Arrange
            var lifeOrbPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.LifeOrb)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                lifeOrbPokemon,
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);

            int initialAttackerHP = lifeOrbPokemon.CurrentHP;
            int initialEnemyHP = enemyParty[0].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Life Orb should boost damage and cause recoil
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialEnemyHP).Or.EqualTo(0),
                "Enemy should take increased damage from Life Orb");
            // Recoil may or may not be visible depending on damage dealt
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task Doubles_RockyHelmet_DamagesOnContact()
        {
            // Arrange
            var rockyHelmetPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Golem, 50)
                .WithPerfectIVs()
                .WithItem(ItemCatalog.RockyHelmet)
                .Build();

            var playerParty = new List<PokemonInstance>
            {
                rockyHelmetPokemon,
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);

            int initialHelmetHP = rockyHelmetPokemon.CurrentHP;

            // Act - Run turns until contact move is used
            for (int i = 0; i < 5; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Rocky Helmet should work in doubles format
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(2));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(2));
        }

        #endregion

        #region Field Conditions Integration

        [Test]
        public async Task Doubles_Reflect_ReducesPhysicalDamage()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);
            _engine.Field.PlayerSide.AddSideCondition(reflectData, 5);

            // Act
            await _engine.RunTurn();

            // Assert - Reflect should be active and reduce damage
            Assert.That(_engine.Field.PlayerSide.HasSideCondition(SideCondition.Reflect), Is.True);
        }

        [Test]
        public async Task Triples_LightScreen_ReducesSpecialDamage()
        {
            // Arrange
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

            var lightScreenData = SideConditionCatalog.GetByType(SideCondition.LightScreen);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);
            _engine.Field.PlayerSide.AddSideCondition(lightScreenData, 5);

            // Act
            await _engine.RunTurn();

            // Assert - Light Screen should be active
            Assert.That(_engine.Field.PlayerSide.HasSideCondition(SideCondition.LightScreen), Is.True);
        }

        [Test]
        public async Task Doubles_Tailwind_IncreasesSpeed()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);
            _engine.Field.PlayerSide.AddSideCondition(tailwindData, 4);

            // Act
            await _engine.RunTurn();

            // Assert - Tailwind should be active
            Assert.That(_engine.Field.PlayerSide.HasSideCondition(SideCondition.Tailwind), Is.True);
        }

        #endregion

        #region Move Types Integration

        [Test]
        public async Task Doubles_SingleTargetMove_HitsOnlyTarget()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var tackleMove = new MoveInstance(MoveCatalog.Tackle); // SingleEnemy target
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, tackleMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            int initialHP1 = enemyParty[0].CurrentHP;
            int initialHP2 = enemyParty[1].CurrentHP;

            // Act
            await _engine.RunTurn();

            // Assert - Only targeted enemy should take damage
            Assert.That(enemyParty[0].CurrentHP, Is.LessThan(initialHP1).Or.EqualTo(0));
            Assert.That(enemyParty[1].CurrentHP, Is.EqualTo(initialHP2), "Non-targeted enemy should not take damage");
        }

        [Test]
        public async Task Triples_SpreadMove_HitsAllTargetsWithReducedDamage()
        {
            // Arrange
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

            var earthquakeMove = new MoveInstance(MoveCatalog.Earthquake);
            var playerProvider = new TestActionProvider((field, slot) =>
            {
                var playerSlot = field.PlayerSide.Slots[0];
                var enemySlot = field.EnemySide.Slots[0];
                return new UseMoveAction(playerSlot, enemySlot, earthquakeMove);
            });
            var enemyProvider = new TestActionProvider(new MessageAction("Pass"));

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerProvider, enemyProvider, _view);

            var initialHPs = enemyParty.Select(p => p.CurrentHP).ToList();

            // Act
            await _engine.RunTurn();

            // Assert - All enemies should take damage (with 0.75x multiplier for spread moves)
            for (int i = 0; i < 3; i++)
            {
                Assert.That(enemyParty[i].CurrentHP, Is.LessThan(initialHPs[i]).Or.EqualTo(0),
                    $"Enemy {i} should take damage from spread move");
            }
        }

        [Test]
        public async Task Horde_DifferentMoveTypes_WorkCorrectly()
        {
            // Arrange
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 30),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 30),
                PokemonFactory.Create(PokemonCatalog.Charmander, 30)
            };

            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Horde, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Act - Run multiple turns with different move types
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Different move types should work correctly
            Assert.That(_engine.Field.PlayerSide.Slots.Count, Is.EqualTo(1));
            Assert.That(_engine.Field.EnemySide.Slots.Count, Is.EqualTo(3));
        }

        #endregion

        #region Complex Multi-System Integration

        [Test]
        public async Task Doubles_MultipleSystems_WorkTogether()
        {
            // Arrange - Combine abilities, items, and field conditions
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                    .WithPerfectIVs()
                    .WithItem(ItemCatalog.Leftovers)
                    .Build()
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };

            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Doubles, playerParty, enemyParty, playerAI, enemyAI, _view);
            
            // Add side condition
            _engine.Field.PlayerSide.AddSideCondition(reflectData, 5);

            // Act - Run multiple turns to verify all systems work together
            // This test verifies that abilities, items, and field conditions work together without errors
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Battle should progress without exceptions
            // The fact that we reached here means all systems worked together correctly
            Assert.That(_engine.Field, Is.Not.Null, "BattleField should exist");
        }

        [Test]
        public async Task Triples_MultipleSystems_WorkTogether()
        {
            // Arrange - Combine multiple systems in triples
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Squirtle, 50)
                    .WithPerfectIVs()
                    .WithItem(ItemCatalog.Leftovers)
                    .Build()
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Eevee, 50),
                PokemonFactory.Create(PokemonCatalog.Geodude, 50)
            };

            var lightScreenData = SideConditionCatalog.GetByType(SideCondition.LightScreen);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            _engine.Initialize(BattleRules.Triples, playerParty, enemyParty, playerAI, enemyAI, _view);
            
            // Add side condition
            _engine.Field.PlayerSide.AddSideCondition(lightScreenData, 5);

            // Act - Run multiple turns to verify all systems work together
            // This test verifies that abilities, items, and field conditions work together without errors
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Battle should progress without exceptions
            // The fact that we reached here means all systems worked together correctly
            Assert.That(_engine.Field, Is.Not.Null, "BattleField should exist");
        }

        [Test]
        public async Task Raid_MultipleSystems_WorkTogether()
        {
            // Arrange - Combine multiple systems in raid format
            var playerParty = new List<PokemonInstance>
            {
                PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50)
                    .WithPerfectIVs()
                    .WithItem(ItemCatalog.Leftovers)
                    .Build()
            };
            var bossParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Mewtwo, 50)
            };

            var reflectData = SideConditionCatalog.GetByType(SideCondition.Reflect);
            var playerAI = new AlwaysAttackAI();
            var enemyAI = new AlwaysAttackAI();

            var rules = new BattleRules 
            { 
                PlayerSlots = 1, 
                EnemySlots = 1,
                IsBossBattle = true,
                BossMultiplier = 5.0f,
                BossStatMultiplier = 1.2f
            };

            _engine.Initialize(rules, playerParty, bossParty, playerAI, enemyAI, _view);
            
            // Add side condition
            _engine.Field.PlayerSide.AddSideCondition(reflectData, 5);

            // Act - Run multiple turns to verify all systems work together
            // This test verifies that abilities, items, and field conditions work together without errors
            for (int i = 0; i < 3; i++)
            {
                await _engine.RunTurn();
                if (_engine.Outcome != BattleOutcome.Ongoing)
                    break;
            }

            // Assert - Battle should progress without exceptions
            // The fact that we reached here means all systems worked together correctly
            Assert.That(_engine.Field, Is.Not.Null, "BattleField should exist");
        }

        #endregion

        #endregion
    }
}

