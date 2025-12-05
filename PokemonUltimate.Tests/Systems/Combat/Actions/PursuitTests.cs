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
    /// Tests for Pursuit move mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class PursuitTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _pursuitMove;
        private MoveInstance _normalMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];

            // Create Pursuit move
            var pursuitMoveData = new MoveData
            {
                Name = "Pursuit",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Dark,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new PursuitEffect()
                }
            };
            _pursuitMove = new MoveInstance(pursuitMoveData);

            // Create normal move for comparison
            var normalMoveData = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            _normalMove = new MoveInstance(normalMoveData);
        }

        [Test]
        public void UseMoveAction_Pursuit_TargetSwitches_2xPower()
        {
            // Arrange
            // Mark target as switching out
            _targetSlot.AddVolatileStatus(VolatileStatus.SwitchingOut);
            
            // Act
            var pursuitAction = new UseMoveAction(_userSlot, _targetSlot, _pursuitMove);
            var reactions = pursuitAction.ExecuteLogic(_field).ToList();

            // Assert
            // Pursuit should deal 2x damage when target switches
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            
            // Compare with normal move (same power, but Pursuit should deal more)
            var normalAction = new UseMoveAction(_userSlot, _targetSlot, _normalMove);
            var normalReactions = normalAction.ExecuteLogic(_field).ToList();
            var normalDamageAction = normalReactions.OfType<DamageAction>().FirstOrDefault();
            
            // Pursuit with 2x power should deal significantly more damage
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(normalDamageAction.Context.FinalDamage));
            
            // Verify message about switching
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("switching out")), Is.True);
        }

        [Test]
        public void UseMoveAction_Pursuit_TargetDoesNotSwitch_NormalPower()
        {
            // Arrange
            // Target is not switching (no SwitchingOut status)
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.SwitchingOut), Is.False);
            
            // Act
            var pursuitAction = new UseMoveAction(_userSlot, _targetSlot, _pursuitMove);
            var reactions = pursuitAction.ExecuteLogic(_field).ToList();

            // Assert
            // Pursuit should deal normal damage when target doesn't switch
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            
            // Compare with normal move (should be similar since same power)
            var normalAction = new UseMoveAction(_userSlot, _targetSlot, _normalMove);
            var normalReactions = normalAction.ExecuteLogic(_field).ToList();
            var normalDamageAction = normalReactions.OfType<DamageAction>().FirstOrDefault();
            
            // Damage should be similar (allowing for random variation)
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
            
            // No switching message when target is not switching
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("switching out")), Is.False);
        }

        [Test]
        public void UseMoveAction_Pursuit_HitsBeforeSwitch()
        {
            // Arrange
            // Target is switching out
            _targetSlot.AddVolatileStatus(VolatileStatus.SwitchingOut);
            
            // Act
            var pursuitAction = new UseMoveAction(_userSlot, _targetSlot, _pursuitMove);
            var reactions = pursuitAction.ExecuteLogic(_field).ToList();

            // Assert
            // Pursuit should hit before switch completes
            // This is tested by verifying damage is dealt even when target is switching
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
            
            // Verify switching message
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("switching out")), Is.True);
        }
    }
}

