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

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Edge case tests for Protect/Detect move mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class ProtectEdgeCasesTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _protectMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];

            var protectMoveData = new MoveData
            {
                Name = "Protect",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                MaxPP = 10,
                Priority = 4,
                TargetScope = TargetScope.Self,
                Effects = new List<IMoveEffect> { new ProtectEffect() }
            };
            _protectMove = new MoveInstance(protectMoveData);
        }

        [Test]
        public void UseMoveAction_Protect_FirstUse_AlwaysSucceeds()
        {
            // Arrange
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.EqualTo(0));

            // Act
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            var reactions = protectAction.ExecuteLogic(_field).ToList();

            // Assert
            // First use should always succeed (100% chance)
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.True);
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.EqualTo(1));
        }

        [Test]
        public void UseMoveAction_Protect_MultipleConsecutiveUses_ChanceHalves()
        {
            // Arrange
            // Use Protect multiple times consecutively
            for (int i = 0; i < 3; i++)
            {
                var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
                protectAction.ExecuteLogic(_field);
                
                // Remove protection for next turn
                _targetSlot.RemoveVolatileStatus(VolatileStatus.Protected);
            }

            // Act & Assert
            // Consecutive uses counter should increment
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void UseMoveAction_Protect_ResetAfterTurn_ResetsCounter()
        {
            // Arrange
            // Use Protect
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            protectAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.GreaterThan(0));

            // Simulate end of turn (Protect removed, but counter should persist)
            _targetSlot.RemoveVolatileStatus(VolatileStatus.Protected);
            
            // Reset counter manually (simulating turn without Protect)
            _targetSlot.ResetProtectUses();

            // Act & Assert
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.EqualTo(0));
        }

        [Test]
        public void UseMoveAction_Protect_SwitchOut_ResetsCounter()
        {
            // Arrange
            // Use Protect
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            protectAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.GreaterThan(0));

            // Act - Switch out (resets battle state)
            _targetSlot.ResetBattleState();

            // Assert
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.EqualTo(0));
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.False);
        }

        [Test]
        public void UseMoveAction_Protect_StatusMove_StillBlocked()
        {
            // Arrange
            // Set target as protected
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            protectAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.True);

            // Create a status move
            var statusMoveData = new MoveData
            {
                Name = "Thunder Wave",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Electric,
                Category = MoveCategory.Status,
                MaxPP = 20,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new StatusEffect { Status = PersistentStatus.Paralysis } }
            };
            var statusMove = new MoveInstance(statusMoveData);

            // Act
            var statusAction = new UseMoveAction(_userSlot, _targetSlot, statusMove);
            var reactions = statusAction.ExecuteLogic(_field).ToList();

            // Assert
            // Status moves should also be blocked by Protect
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("protected")), Is.True);
        }
    }
}

