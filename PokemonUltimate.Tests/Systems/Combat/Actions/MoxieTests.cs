using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Moxie ability - raises Attack after knocking out opponent.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.17: Advanced Abilities
    /// **Documentation**: See `docs/features/2-combat-system/2.17-advanced-abilities/README.md`
    /// </remarks>
    [TestFixture]
    public class MoxieTests
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
            var userPokemon = PokemonFactory.Create(PokemonCatalog.Gyarados, 50);
            userPokemon.SetAbility(AbilityCatalog.Moxie);
            // Ensure Pokemon has at least one move
            if (userPokemon.Moves.Count == 0)
            {
                userPokemon.Moves.Add(new MoveInstance(MoveCatalog.Tackle));
            }
            
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { userPokemon },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _user = _userSlot.Pokemon;
            _target = _targetSlot.Pokemon;
            
            _moveInstance = _user.Moves[0];
        }

        #region OnAfterMove Tests

        [Test]
        public void ExecuteLogic_KnockingOutOpponent_WithMoxie_RaisesAttack()
        {
            // Arrange - Damage target to near fainting
            int initialAttackStage = _userSlot.GetStatStage(Stat.Attack);
            _target.CurrentHP = 1; // Almost fainted

            // Act - Use move that will KO opponent
            var action = new UseMoveAction(_userSlot, _targetSlot, _moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions first to apply damage and KO opponent
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(_field).ToList();
            }

            // Verify target fainted before checking Moxie
            Assert.That(_target.IsFainted, Is.True, "Target should be fainted for Moxie to activate");

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

            // Manually trigger OnAfterMove AFTER damage has been applied and target is fainted
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

            // Assert - Moxie should have raised Attack
            int finalAttackStage = _userSlot.GetStatStage(Stat.Attack);
            Assert.That(finalAttackStage, Is.GreaterThan(initialAttackStage), "Moxie should raise Attack after KO");
            Assert.That(finalAttackStage, Is.EqualTo(initialAttackStage + 1), "Attack should be raised by 1 stage");
        }

        [Test]
        public void ExecuteLogic_NotKnockingOutOpponent_WithMoxie_NoStatChange()
        {
            // Arrange - Target has plenty of HP
            int initialAttackStage = _userSlot.GetStatStage(Stat.Attack);
            _target.CurrentHP = _target.MaxHP; // Full HP

            // Act - Use move
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

            // Assert - Moxie should NOT raise Attack if no KO
            int finalAttackStage = _userSlot.GetStatStage(Stat.Attack);
            Assert.That(finalAttackStage, Is.EqualTo(initialAttackStage), "Moxie should not raise Attack if no KO");
        }

        [Test]
        public void ExecuteLogic_WithoutMoxie_NoStatChange()
        {
            // Arrange - Remove Moxie ability
            _user.SetAbility(null);
            int initialAttackStage = _userSlot.GetStatStage(Stat.Attack);
            _target.CurrentHP = 1; // Almost fainted

            // Act - Use move that will KO opponent
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

            // Assert - No stat change without Moxie
            int finalAttackStage = _userSlot.GetStatStage(Stat.Attack);
            Assert.That(finalAttackStage, Is.EqualTo(initialAttackStage), "No stat change without Moxie");
        }

        #endregion
    }
}

