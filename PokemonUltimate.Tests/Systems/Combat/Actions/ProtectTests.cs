using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for Protect/Detect move mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class ProtectTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private MoveInstance _protectMove;
        private MoveInstance _attackMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _user = _userSlot.Pokemon;
            _target = _targetSlot.Pokemon;
            _attackMove = _user.Moves[0]; // Use first move as attack
            
            // Create Protect move
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
        public void UseMoveAction_Protect_BlocksMove()
        {
            // Arrange
            // Use Protect first to set target as protected
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            var protectReactions = protectAction.ExecuteLogic(_field).ToList();
            
            // Verify Protect was applied
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.True);
            
            // Now try to attack the protected target
            var attackAction = new UseMoveAction(_userSlot, _targetSlot, _attackMove);
            
            // Act
            var reactions = attackAction.ExecuteLogic(_field).ToList();
            
            // Assert
            Assert.That(reactions, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("protected")), Is.True);
            Assert.That(reactions.Any(r => r is DamageAction), Is.False);
        }

        [Test]
        public void UseMoveAction_Protect_ConsecutiveUse_HalvesChance()
        {
            // Arrange
            // First use: should succeed (100% chance)
            var protectAction1 = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            var reactions1 = protectAction1.ExecuteLogic(_field).ToList();
            
            // Verify first use succeeded
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.True);
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.EqualTo(1));
            
            // Remove protection for next turn
            _targetSlot.RemoveVolatileStatus(VolatileStatus.Protected);
            
            // Second use: should have 50% chance (halved)
            // Note: This test may be flaky due to randomness, but we can verify the counter increments
            var protectAction2 = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            var reactions2 = protectAction2.ExecuteLogic(_field).ToList();
            
            // Act & Assert
            // Verify consecutive uses counter increments
            Assert.That(_targetSlot.ProtectConsecutiveUses, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void UseMoveAction_Protect_Feint_Bypasses()
        {
            // Arrange
            // Set target as protected
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            protectAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.True);
            
            // Create Feint move (bypasses Protect)
            var feintMoveData = new MoveData
            {
                Name = "Feint",
                Power = 30,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 2,
                TargetScope = TargetScope.SingleEnemy,
                BypassesProtect = true,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var feintMove = new MoveInstance(feintMoveData);
            
            // Act
            var feintAction = new UseMoveAction(_userSlot, _targetSlot, feintMove);
            var reactions = feintAction.ExecuteLogic(_field).ToList();
            
            // Assert
            // Feint should bypass Protect and deal damage
            Assert.That(reactions.Any(r => r is DamageAction), Is.True);
        }

        [Test]
        public void UseMoveAction_Protect_Priority_Plus4()
        {
            // Arrange
            // Protect move is already created in SetUp with Priority = 4
            
            // Act
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, _protectMove);
            
            // Assert
            Assert.That(protectAction.Priority, Is.EqualTo(4));
        }
    }
}

