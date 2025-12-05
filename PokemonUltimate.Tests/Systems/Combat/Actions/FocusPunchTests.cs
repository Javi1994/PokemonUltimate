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
    /// Tests for Focus Punch move mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class FocusPunchTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _focusPunchMove;
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

            // Create Focus Punch move
            var focusPunchMoveData = new MoveData
            {
                Name = "Focus Punch",
                Power = 150,
                Accuracy = 100,
                Type = PokemonType.Fighting,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = -3,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new FocusPunchEffect()
                }
            };
            _focusPunchMove = new MoveInstance(focusPunchMoveData);

            // Create attack move for hitting user
            _attackMove = _targetSlot.Pokemon.Moves[0];
        }

        [Test]
        public void UseMoveAction_FocusPunch_NotHit_Succeeds()
        {
            // Arrange
            // User is not hit before moving
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Focusing), Is.False);
            
            // Act
            var focusPunchAction = new UseMoveAction(_userSlot, _targetSlot, _focusPunchMove);
            var reactions = focusPunchAction.ExecuteLogic(_field).ToList();

            // Assert
            // Focus Punch should succeed and deal damage
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
            
            // Verify PP was deducted
            Assert.That(_focusPunchMove.CurrentPP, Is.LessThan(_focusPunchMove.MaxPP));
        }

        [Test]
        public void UseMoveAction_FocusPunch_HitBeforeMoving_Fails()
        {
            // Arrange
            // User tightens focus (marked as focusing)
            _userSlot.AddVolatileStatus(VolatileStatus.Focusing);
            
            // User is hit before moving (record damage)
            var attackAction = new UseMoveAction(_targetSlot, _userSlot, _attackMove);
            var attackReactions = attackAction.ExecuteLogic(_field).ToList();
            var damageAction = attackReactions.OfType<DamageAction>().FirstOrDefault();
            if (damageAction != null)
            {
                damageAction.ExecuteLogic(_field);
            }
            
            // Act - Try to use Focus Punch
            var focusPunchAction = new UseMoveAction(_userSlot, _targetSlot, _focusPunchMove);
            var reactions = focusPunchAction.ExecuteLogic(_field).ToList();

            // Assert
            // Focus Punch should fail (no damage dealt)
            var focusPunchDamage = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(focusPunchDamage, Is.Null);
            
            // Verify PP was still deducted
            Assert.That(_focusPunchMove.CurrentPP, Is.LessThan(_focusPunchMove.MaxPP));
            
            // Verify failure message
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("lost its focus")), Is.True);
        }

        [Test]
        public void UseMoveAction_FocusPunch_Fails_StillDeductsPP()
        {
            // Arrange
            int initialPP = _focusPunchMove.CurrentPP;
            
            // User is hit before moving
            _userSlot.AddVolatileStatus(VolatileStatus.Focusing);
            var attackAction = new UseMoveAction(_targetSlot, _userSlot, _attackMove);
            var attackReactions = attackAction.ExecuteLogic(_field).ToList();
            var damageAction = attackReactions.OfType<DamageAction>().FirstOrDefault();
            if (damageAction != null)
            {
                damageAction.ExecuteLogic(_field);
            }
            
            // Act - Try to use Focus Punch (should fail)
            var focusPunchAction = new UseMoveAction(_userSlot, _targetSlot, _focusPunchMove);
            focusPunchAction.ExecuteLogic(_field);

            // Assert
            // PP should still be deducted even though move failed
            Assert.That(_focusPunchMove.CurrentPP, Is.EqualTo(initialPP - 1));
        }
    }
}

