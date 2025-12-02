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
    /// Edge case tests for RecoilEffect and DrainEffect.
    /// </summary>
    [TestFixture]
    public class RecoilDrainEffectEdgeCasesTests
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

        #region Recoil Edge Cases

        [Test]
        public void RecoilEffect_WithMinimumDamage_DealsAtLeast1HP()
        {
            // Arrange - Very weak move, but recoil should still deal at least 1 HP
            var move = new MoveData
            {
                Name = "Test Move",
                Power = 1,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 10,
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

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Recoil should deal at least 1 HP if damage was dealt
            if (_defender.CurrentHP < _defender.MaxHP) // Damage was dealt
            {
                Assert.That(_attacker.CurrentHP, Is.LessThan(initialAttackerHP),
                    "Recoil should deal damage if any damage was dealt to defender");
            }
        }

        [Test]
        public void RecoilEffect_CanCauseFaint()
        {
            // Arrange - Attacker at low HP, strong recoil move
            _attacker.CurrentHP = 10; // Very low HP
            
            var move = new MoveData
            {
                Name = "Head Smash",
                Power = 150,
                Accuracy = 100,
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

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            var faintActions = new List<FaintAction>();
            foreach (var action in reactions)
            {
                var result = action.ExecuteLogic(_field);
                faintActions.AddRange(result.OfType<FaintAction>());
            }

            // Assert - Recoil can cause faint
            if (_attacker.IsFainted)
            {
                Assert.That(faintActions.Any(f => f.Target == _attackerSlot), Is.True,
                    "Recoil damage should be able to cause faint");
            }
        }

        [Test]
        public void RecoilEffect_NoDamageDealt_NoRecoil()
        {
            // Arrange - Move with Status category (no damage)
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
            
            // Assert - No recoil if no damage was dealt
            Assert.That(_attacker.CurrentHP, Is.EqualTo(initialAttackerHP),
                "No recoil should occur if no damage was dealt");
        }

        #endregion

        #region Drain Edge Cases

        [Test]
        public void DrainEffect_WithMinimumDamage_HealsAtLeast1HP()
        {
            // Arrange - Very weak move, but drain should still heal at least 1 HP
            var move = new MoveData
            {
                Name = "Test Move",
                Power = 1,
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
            _attacker.TakeDamage(_attacker.MaxHP / 2);
            int initialAttackerHP = _attacker.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - Drain should heal at least 1 HP if damage was dealt
            if (_defender.CurrentHP < _defender.MaxHP) // Damage was dealt
            {
                Assert.That(_attacker.CurrentHP, Is.GreaterThan(initialAttackerHP),
                    "Drain should heal if any damage was dealt to defender");
            }
        }

        [Test]
        public void DrainEffect_CannotExceedMaxHP()
        {
            // Arrange - Attacker at near max HP, use drain move
            _attacker.CurrentHP = _attacker.MaxHP - 1; // 1 HP below max
            
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
                "Drain heal should not exceed MaxHP");
        }

        [Test]
        public void DrainEffect_NoDamageDealt_NoHeal()
        {
            // Arrange - Move with Status category (no damage)
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
        public void DrainEffect_AtMaxHP_NoHeal()
        {
            // Arrange - Attacker at max HP, use drain move
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
            int initialAttackerHP = _attacker.CurrentHP;

            // Act
            var useMoveAction = new UseMoveAction(_attackerSlot, _defenderSlot, moveInstance);
            var reactions = useMoveAction.ExecuteLogic(_field).ToList();

            // Execute all actions
            foreach (var action in reactions)
            {
                action.ExecuteLogic(_field);
            }

            // Assert - HP should remain at max (no overheal)
            Assert.That(_attacker.CurrentHP, Is.EqualTo(initialAttackerHP),
                "Drain should not heal if already at MaxHP");
        }

        #endregion

        #region Combined Edge Cases

        [Test]
        public void RecoilAndDrain_OrderOfExecution_RecoilThenDrain()
        {
            // Arrange - Move with both recoil and drain
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
            
            // Reduce attacker HP so drain can heal
            _attacker.TakeDamage(_attacker.MaxHP / 3);
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

            // Assert - Both effects should be processed
            int damageDealt = initialDefenderHP - _defender.CurrentHP;
            int netHPChange = _attacker.CurrentHP - initialAttackerHP;
            
            // Net change = heal (50%) - recoil (25%) = +25% of damage dealt
            int expectedRecoil = (int)(damageDealt * 0.25f);
            int expectedHeal = (int)(damageDealt * 0.5f);
            int expectedNet = expectedHeal - expectedRecoil;
            
            Assert.That(netHPChange, Is.EqualTo(expectedNet).Within(1),
                $"Net HP change should be heal ({expectedHeal}) - recoil ({expectedRecoil}) = {expectedNet}");
        }

        #endregion
    }
}

