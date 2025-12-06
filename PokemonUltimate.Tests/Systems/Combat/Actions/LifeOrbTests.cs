using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Life Orb item - recoil after dealing damage.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.18: Advanced Items
    /// **Documentation**: See `docs/features/2-combat-system/2.18-advanced-items/README.md`
    /// </remarks>
    [TestFixture]
    public class LifeOrbTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private MoveInstance _moveInstance;

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
            
            // Set Life Orb item
            _user.HeldItem = ItemCatalog.LifeOrb;
            _moveInstance = _user.Moves[0];
        }

        #region OnAfterMove Tests

        [Test]
        public void ExecuteLogic_DealingDamage_WithLifeOrb_CausesRecoil()
        {
            // Arrange
            int initialUserHP = _user.CurrentHP;
            _target.CurrentHP = _target.MaxHP; // Full HP to ensure damage is dealt

            // Act - Use move
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions first to update damage trackers
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(_field).ToList();
            }

            // Execute other actions (messages, etc.)
            var otherActions = reactions.Where(r => !(r is DamageAction)).ToList();
            var allOtherActions = new Queue<BattleAction>(otherActions);
            while (allOtherActions.Count > 0)
            {
                var currentAction = allOtherActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allOtherActions.Enqueue(subReaction);
                }
            }

            // Manually trigger OnAfterMove AFTER damage has been executed
            var triggerProcessor = new BattleTriggerProcessor();
            var afterMoveActions = triggerProcessor.ProcessTrigger(BattleTrigger.OnAfterMove, _field);
            var allAfterMoveActions = new Queue<BattleAction>(afterMoveActions);
            while (allAfterMoveActions.Count > 0)
            {
                var currentAction = allAfterMoveActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allAfterMoveActions.Enqueue(subReaction);
                }
            }

            // Assert - Life Orb should cause recoil
            Assert.That(_user.CurrentHP, Is.LessThan(initialUserHP), "Life Orb should cause recoil");
            int expectedRecoil = _user.MaxHP / 10; // 10% of max HP
            Assert.That(_user.CurrentHP, Is.EqualTo(initialUserHP - expectedRecoil), $"Should deal {expectedRecoil} recoil damage");
        }

        [Test]
        public void ExecuteLogic_StatusMove_WithLifeOrb_NoRecoil()
        {
            // Arrange - Use status move (no damage)
            int initialUserHP = _user.CurrentHP;
            var statusMove = new MoveInstance(MoveCatalog.ThunderWave);
            if (_user.Moves.Count > 0)
            {
                _user.Moves[0] = statusMove;
            }
            else
            {
                _user.Moves.Add(statusMove);
            }

            // Act
            var action = new UseMoveAction(_userSlot, _targetSlot, statusMove);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute all actions except OnAfterMove first
            // OnAfterMove actions are at the end of the list
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            var nonAfterMoveActions = reactions.Take(reactions.Count - reactions.Count(r => r is MessageAction msg && msg.Message.Contains("Life Orb"))).ToList();

            // Execute damage actions first (if any) - status moves have none
            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(_field).ToList();
            }

            // Execute other actions (messages, status effects, etc.)
            var allNonAfterMoveActions = new Queue<BattleAction>(nonAfterMoveActions);
            while (allNonAfterMoveActions.Count > 0)
            {
                var currentAction = allNonAfterMoveActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    if (!(subReaction is MessageAction msg && msg.Message.Contains("Life Orb")))
                    {
                        allNonAfterMoveActions.Enqueue(subReaction);
                    }
                }
            }

            // Now manually trigger OnAfterMove AFTER damage actions have been executed
            // Since no damage was dealt (status move), Life Orb shouldn't activate
            var triggerProcessor = new BattleTriggerProcessor();
            var afterMoveActions = triggerProcessor.ProcessTrigger(BattleTrigger.OnAfterMove, _field);
            var allAfterMoveActions = new Queue<BattleAction>(afterMoveActions);
            while (allAfterMoveActions.Count > 0)
            {
                var currentAction = allAfterMoveActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allAfterMoveActions.Enqueue(subReaction);
                }
            }

            // Assert - Status moves shouldn't cause recoil (no damage was dealt)
            Assert.That(_user.CurrentHP, Is.EqualTo(initialUserHP), "Status moves shouldn't cause Life Orb recoil");
        }

        [Test]
        public void ExecuteLogic_WithoutLifeOrb_NoRecoil()
        {
            // Arrange - Remove Life Orb
            _user.HeldItem = null;
            int initialUserHP = _user.CurrentHP;
            _target.CurrentHP = _target.MaxHP;

            // Act
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Process all reactions
            foreach (var reaction in reactions)
            {
                if (reaction is DamageAction damageAction)
                {
                    damageAction.ExecuteLogic(_field).ToList();
                }
            }

            // Assert - No recoil without Life Orb
            // Note: User HP might change due to other effects, but not due to Life Orb recoil
            // We'll check that there's no message about Life Orb recoil
            bool hasLifeOrbMessage = reactions.Any(r => 
                r is MessageAction msg && msg.Message.Contains("Life Orb"));
            Assert.That(hasLifeOrbMessage, Is.False, "No Life Orb message without Life Orb");
        }

        #endregion
    }
}

