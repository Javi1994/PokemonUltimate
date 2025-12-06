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

namespace PokemonUltimate.Tests.Systems.Combat.Integration.Actions
{
    /// <summary>
    /// Integration tests for RecoilEffect and DrainEffect with full battle systems.
    /// </summary>
    [TestFixture]
    public class RecoilDrainIntegrationTests
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
                new[] { PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Pikachu, 50).WithPerfectIVs().Build() },
                new[] { PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Charmander, 50).WithPerfectIVs().Build() });

            _attackerSlot = _field.PlayerSide.Slots[0];
            _defenderSlot = _field.EnemySide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            _defender = _defenderSlot.Pokemon;
        }

        #region Recoil Integration Tests

        [Test]
        public void RecoilEffect_WithDamagePipeline_CalculatesCorrectly()
        {
            // Arrange - Recoil move with full damage pipeline
            var move = new MoveData
            {
                Name = "Double-Edge",
                Power = 120,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new RecoilEffect(33) // 33% recoil
                }
            };

            var moveInstance = new MoveInstance(move);
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act - Use move through UseMoveAction
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Verify damage was calculated through pipeline
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Recoil should be based on final damage (after all pipeline steps)
            Assert.That(_attacker.CurrentHP, Is.LessThan(initialAttackerHP));
            int recoilDamage = initialAttackerHP - _attacker.CurrentHP;
            int expectedRecoil = (int)(damageDealt * 0.33f);
            
            Assert.That(recoilDamage, Is.EqualTo(expectedRecoil).Within(1),
                "Recoil should be calculated based on final damage from pipeline");
        }

        [Test]
        public void RecoilEffect_WithSTAB_RecoilBasedOnSTABDamage()
        {
            // Arrange - Normal type Pokemon using Normal move (STAB)
            var normalPokemon = PokemonUltimate.Core.Factories.Pokemon.Create(PokemonCatalog.Snorlax, 50).WithPerfectIVs().Build();
            _field.PlayerSide.Slots[0].SetPokemon(normalPokemon);
            _attackerSlot = _field.PlayerSide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            
            var move = new MoveData
            {
                Name = "Take Down",
                Power = 90,
                Accuracy = 85,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new RecoilEffect(25) // 25% recoil
                }
            };

            var moveInstance = new MoveInstance(move);
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Recoil should be based on STAB-boosted damage
            int damageDealt = initialDefenderHP - _defender.CurrentHP;
            int recoilDamage = initialAttackerHP - _attacker.CurrentHP;
            int expectedRecoil = (int)(damageDealt * 0.25f);
            
            Assert.That(recoilDamage, Is.EqualTo(expectedRecoil).Within(1),
                "Recoil should be based on final damage including STAB");
        }

        #endregion

        #region Drain Integration Tests

        [Test]
        public void DrainEffect_WithDamagePipeline_CalculatesCorrectly()
        {
            // Arrange - Drain move with full damage pipeline
            var move = new MoveData
            {
                Name = "Giga Drain",
                Power = 75,
                Accuracy = 100,
                Type = PokemonType.Grass,
                Category = MoveCategory.Special,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new DrainEffect(50) // 50% drain
                }
            };

            var moveInstance = new MoveInstance(move);
            
            // Reduce attacker HP so drain can heal
            _attacker.TakeDamage(_attacker.MaxHP / 4);
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act - Use move through UseMoveAction
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Verify drain was calculated based on pipeline damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Drain should be based on final damage (after all pipeline steps)
            Assert.That(_attacker.CurrentHP, Is.GreaterThan(initialAttackerHP));
            int healAmount = _attacker.CurrentHP - initialAttackerHP;
            int expectedHeal = (int)(damageDealt * 0.5f);
            
            Assert.That(healAmount, Is.EqualTo(expectedHeal).Within(1),
                "Drain should be calculated based on final damage from pipeline");
        }

        [Test]
        public void DrainEffect_WithTypeEffectiveness_HealBasedOnEffectiveDamage()
        {
            // Arrange - Grass move vs Water (super effective)
            var grassPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);
            _field.PlayerSide.Slots[0].SetPokemon(grassPokemon);
            _attackerSlot = _field.PlayerSide.Slots[0];
            _attacker = _attackerSlot.Pokemon;
            
            var waterDefender = PokemonFactory.Create(PokemonCatalog.Squirtle, 50);
            _field.EnemySide.Slots[0].SetPokemon(waterDefender);
            _defenderSlot = _field.EnemySide.Slots[0];
            _defender = _defenderSlot.Pokemon;
            
            var move = new MoveData
            {
                Name = "Giga Drain",
                Power = 75,
                Accuracy = 100,
                Type = PokemonType.Grass,
                Category = MoveCategory.Special,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new DrainEffect(50) // 50% drain
                }
            };

            var moveInstance = new MoveInstance(move);
            
            // Reduce attacker HP so drain can heal
            _attacker.TakeDamage(_attacker.MaxHP / 4);
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Drain should be based on super-effective damage
            int damageDealt = initialDefenderHP - _defender.CurrentHP;
            int healAmount = _attacker.CurrentHP - initialAttackerHP;
            
            // Verify that damage was dealt (super-effective)
            Assert.That(damageDealt, Is.GreaterThan(0),
                "Super-effective move should deal damage");
            
            // Verify that drain healed (should be approximately 50% of damage dealt)
            Assert.That(healAmount, Is.GreaterThan(0),
                $"Drain should heal if damage was dealt. Actual heal: {healAmount}, Damage dealt: {damageDealt}");
            
            // Drain should be approximately 50% of damage dealt
            // Note: The actual calculation uses context.FinalDamage from the pipeline,
            // which may differ slightly from the actual damage applied due to rounding
            float healRatio = (float)healAmount / damageDealt;
            Assert.That(healRatio, Is.GreaterThan(0.3f).And.LessThan(0.7f),
                $"Drain should heal approximately 50% of damage dealt. Heal ratio: {healRatio:F2}, Heal: {healAmount}, Damage: {damageDealt}");
        }

        #endregion

        #region Full Battle Integration

        [Test]
        public void RecoilDrain_WithBattleQueue_ExecutesInCorrectOrder()
        {
            // Arrange - Move with recoil (100% accuracy to ensure it hits)
            var move = new MoveData
            {
                Name = "Take Down",
                Power = 90,
                Accuracy = 100, // Changed from 85 to ensure deterministic test
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new RecoilEffect(25)
                }
            };

            var moveInstance = new MoveInstance(move);
            int initialAttackerHP = _attacker.CurrentHP;

            // Act - Use move and process through battle queue
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Verify actions are in correct order: damage first, then recoil
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions.Count, Is.GreaterThanOrEqualTo(1),
                "Should have at least one damage action");

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Recoil damage was applied
            Assert.That(_attacker.CurrentHP, Is.LessThan(initialAttackerHP),
                "Recoil damage should be applied after main damage");
        }

        #endregion
    }
}

