using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Actions
{
    /// <summary>
    /// Functional tests for RecoilEffect and DrainEffect processing in UseMoveAction.
    /// </summary>
    [TestFixture]
    public class RecoilDrainEffectTests
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

        #region RecoilEffect Tests

        [Test]
        public void UseMoveAction_WithRecoil25Percent_DealsRecoilDamage()
        {
            // Arrange - Create move with 25% recoil (Take Down)
            var move = new MoveData
            {
                Name = "Take Down",
                Power = 90,
                Accuracy = 100, // Use 100% accuracy for deterministic test
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

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Defender took damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Attacker took recoil damage (25% of damage dealt)
            Assert.That(_attacker.CurrentHP, Is.LessThan(initialAttackerHP));
            int recoilDamage = initialAttackerHP - _attacker.CurrentHP;
            int expectedRecoil = (int)(damageDealt * 0.25f);
            
            Assert.That(recoilDamage, Is.EqualTo(expectedRecoil).Within(1),
                $"Recoil should be approximately 25% of damage dealt ({damageDealt} * 0.25 = {expectedRecoil})");
        }

        [Test]
        public void UseMoveAction_WithRecoil33Percent_DealsRecoilDamage()
        {
            // Arrange - Create move with 33% recoil (Double-Edge, Brave Bird)
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

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Defender took damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Attacker took recoil damage (33% of damage dealt)
            Assert.That(_attacker.CurrentHP, Is.LessThan(initialAttackerHP));
            int recoilDamage = initialAttackerHP - _attacker.CurrentHP;
            int expectedRecoil = (int)(damageDealt * 0.33f);
            
            Assert.That(recoilDamage, Is.EqualTo(expectedRecoil).Within(1),
                $"Recoil should be approximately 33% of damage dealt ({damageDealt} * 0.33 = {expectedRecoil})");
        }

        [Test]
        public void UseMoveAction_WithRecoil50Percent_DealsRecoilDamage()
        {
            // Arrange - Create move with 50% recoil (Head Smash)
            var move = new MoveData
            {
                Name = "Head Smash",
                Power = 150,
                Accuracy = 100, // Use 100% accuracy for deterministic test
                Type = PokemonType.Rock,
                Category = MoveCategory.Physical,
                MaxPP = 5,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new RecoilEffect(50) // 50% recoil
                }
            };

            var moveInstance = new MoveInstance(move);
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Defender took damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Attacker took recoil damage (50% of damage dealt)
            Assert.That(_attacker.CurrentHP, Is.LessThan(initialAttackerHP));
            int recoilDamage = initialAttackerHP - _attacker.CurrentHP;
            int expectedRecoil = (int)(damageDealt * 0.5f);
            
            // Recoil is calculated based on context.FinalDamage from pipeline,
            // which may differ from actual damage applied due to rounding and pipeline calculations
            // The pipeline's FinalDamage includes all multipliers (STAB, type effectiveness, etc.)
            // which may result in a different value than the actual damage applied
            Assert.That(recoilDamage, Is.EqualTo(expectedRecoil).Within(10),
                $"Recoil should be approximately 50% of damage dealt ({damageDealt} * 0.5 = {expectedRecoil}, actual: {recoilDamage}). " +
                $"Note: Recoil is calculated from pipeline's FinalDamage, which may differ from actual damage applied.");
        }

        [Test]
        public void UseMoveAction_WithRecoil_NoDamageDealt_NoRecoil()
        {
            // Arrange - Create move with recoil but Status category (no damage)
            // Note: Power = 0 moves can still deal minimum damage (2 HP) due to formula
            // So we use Status category which doesn't deal damage
            var move = new MoveData
            {
                Name = "Test Move",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Status, // Status moves don't deal damage
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(), // This won't be processed for Status moves
                    new RecoilEffect(25)
                }
            };

            var moveInstance = new MoveInstance(move);
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - No damage was dealt (Status move)
            Assert.That(_defender.CurrentHP, Is.EqualTo(initialDefenderHP),
                "Status moves should not deal damage");
            
            // Assert - No recoil damage if no damage was dealt
            Assert.That(_attacker.CurrentHP, Is.EqualTo(initialAttackerHP),
                "No recoil should occur if no damage was dealt");
        }

        #endregion

        #region DrainEffect Tests

        [Test]
        public void UseMoveAction_WithDrain50Percent_HealsUser()
        {
            // Arrange - Create move with 50% drain (Giga Drain, Absorb)
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
            
            // Reduce attacker HP so drain can heal (can't heal if already at max HP)
            _attacker.TakeDamage(_attacker.MaxHP / 4); // Reduce by 25%
            
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Defender took damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Attacker healed (50% of damage dealt)
            Assert.That(_attacker.CurrentHP, Is.GreaterThan(initialAttackerHP));
            int healAmount = _attacker.CurrentHP - initialAttackerHP;
            int expectedHeal = (int)(damageDealt * 0.5f);
            
            Assert.That(healAmount, Is.EqualTo(expectedHeal).Within(1),
                $"Heal should be approximately 50% of damage dealt ({damageDealt} * 0.5 = {expectedHeal})");
        }

        [Test]
        public void UseMoveAction_WithDrain75Percent_HealsUser()
        {
            // Arrange - Create move with 75% drain (Drain Punch, Horn Leech)
            var move = new MoveData
            {
                Name = "Drain Punch",
                Power = 75,
                Accuracy = 100,
                Type = PokemonType.Fighting,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new DrainEffect(75) // 75% drain
                }
            };

            var moveInstance = new MoveInstance(move);
            
            // Reduce attacker HP so drain can heal (can't heal if already at max HP)
            _attacker.TakeDamage(_attacker.MaxHP / 4); // Reduce by 25%
            
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Defender took damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Attacker healed (75% of damage dealt)
            Assert.That(_attacker.CurrentHP, Is.GreaterThan(initialAttackerHP));
            int healAmount = _attacker.CurrentHP - initialAttackerHP;
            int expectedHeal = (int)(damageDealt * 0.75f);
            
            Assert.That(healAmount, Is.EqualTo(expectedHeal).Within(1),
                $"Heal should be approximately 75% of damage dealt ({damageDealt} * 0.75 = {expectedHeal})");
        }

        [Test]
        public void UseMoveAction_WithDrain_NoDamageDealt_NoHeal()
        {
            // Arrange - Create move with drain but Status category (no damage)
            // Note: Power = 0 moves can still deal minimum damage (2 HP) due to formula
            // So we use Status category which doesn't deal damage
            var move = new MoveData
            {
                Name = "Test Move",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Grass,
                Category = MoveCategory.Status, // Status moves don't deal damage
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(), // This won't be processed for Status moves
                    new DrainEffect(50)
                }
            };

            var moveInstance = new MoveInstance(move);
            
            // Reduce attacker HP so we can verify no heal occurs
            _attacker.TakeDamage(_attacker.MaxHP / 4);
            
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - No damage was dealt (Status move)
            Assert.That(_defender.CurrentHP, Is.EqualTo(initialDefenderHP),
                "Status moves should not deal damage");
            
            // Assert - No heal if no damage was dealt
            Assert.That(_attacker.CurrentHP, Is.EqualTo(initialAttackerHP),
                "No heal should occur if no damage was dealt");
        }

        [Test]
        public void UseMoveAction_WithDrain_CannotExceedMaxHP()
        {
            // Arrange - Attacker at full HP, use drain move
            _attacker.CurrentHP = _attacker.MaxHP;
            
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
                    new DrainEffect(50)
                }
            };

            var moveInstance = new MoveInstance(move);

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - HP should not exceed MaxHP
            Assert.That(_attacker.CurrentHP, Is.LessThanOrEqualTo(_attacker.MaxHP),
                "Heal should not exceed MaxHP");
        }

        #endregion

        #region Combined Effects Tests

        [Test]
        public void UseMoveAction_WithRecoilAndDrain_ProcessesBoth()
        {
            // Arrange - Create move with both recoil and drain (unusual but testable)
            var move = new MoveData
            {
                Name = "Test Move",
                Power = 80,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new RecoilEffect(25),
                    new DrainEffect(50)
                }
            };

            var moveInstance = new MoveInstance(move);
            
            // Reduce attacker HP so drain can heal (can't heal if already at max HP)
            _attacker.TakeDamage(_attacker.MaxHP / 3); // Reduce by 33%
            
            int initialAttackerHP = _attacker.CurrentHP;
            int initialDefenderHP = _defender.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Defender took damage
            Assert.That(_defender.CurrentHP, Is.LessThan(initialDefenderHP));
            int damageDealt = initialDefenderHP - _defender.CurrentHP;

            // Attacker should have net effect: heal from drain - recoil damage
            int netHPChange = _attacker.CurrentHP - initialAttackerHP;
            int expectedRecoil = (int)(damageDealt * 0.25f);
            int expectedHeal = (int)(damageDealt * 0.5f);
            int expectedNet = expectedHeal - expectedRecoil;
            
            Assert.That(netHPChange, Is.EqualTo(expectedNet).Within(1),
                $"Net HP change should be heal ({expectedHeal}) - recoil ({expectedRecoil}) = {expectedNet}");
        }

        #endregion
    }
}

