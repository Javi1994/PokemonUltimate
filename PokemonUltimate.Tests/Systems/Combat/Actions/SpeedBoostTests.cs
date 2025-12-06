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
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Speed Boost ability - raises Speed at end of each turn.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.17: Advanced Abilities
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class SpeedBoostTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private PokemonInstance _user;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var userPokemon = PokemonFactory.Create(PokemonCatalog.Sharpedo, 50);
            userPokemon.SetAbility(AbilityCatalog.SpeedBoost);
            
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { userPokemon },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _user = _userSlot.Pokemon;
        }

        #region OnTurnEnd Tests

        [Test]
        public void OnTurnEnd_WithSpeedBoost_RaisesSpeed()
        {
            // Arrange - Initial Speed stage should be 0
            int initialSpeedStage = _userSlot.GetStatStage(Stat.Speed);
            Assert.That(initialSpeedStage, Is.EqualTo(0), "Initial Speed stage should be 0");

            // Act - Trigger OnTurnEnd
            var triggerProcessor = new BattleTriggerProcessor();
            var turnEndActions = triggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);
            
            // Execute all actions
            var allActions = new Queue<BattleAction>(turnEndActions.ToList());
            while (allActions.Count > 0)
            {
                var currentAction = allActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allActions.Enqueue(subReaction);
                }
            }

            // Assert - Speed should be raised by 1 stage
            int finalSpeedStage = _userSlot.GetStatStage(Stat.Speed);
            Assert.That(finalSpeedStage, Is.EqualTo(initialSpeedStage + 1), "Speed Boost should raise Speed by 1 stage");
        }

        [Test]
        public void OnTurnEnd_MultipleTurns_WithSpeedBoost_Stacks()
        {
            // Arrange
            int initialSpeedStage = _userSlot.GetStatStage(Stat.Speed);
            var triggerProcessor = new BattleTriggerProcessor();

            // Act - Trigger OnTurnEnd multiple times (simulating multiple turns)
            for (int turn = 1; turn <= 3; turn++)
            {
                var turnEndActions = triggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);
                
                // Execute all actions
                var allActions = new Queue<BattleAction>(turnEndActions.ToList());
                while (allActions.Count > 0)
                {
                    var currentAction = allActions.Dequeue();
                    var subReactions = currentAction.ExecuteLogic(_field).ToList();
                    foreach (var subReaction in subReactions)
                    {
                        allActions.Enqueue(subReaction);
                    }
                }

                // Assert - Speed should increase each turn
                int currentSpeedStage = _userSlot.GetStatStage(Stat.Speed);
                Assert.That(currentSpeedStage, Is.EqualTo(initialSpeedStage + turn), 
                    $"Speed should be raised by {turn} stages after {turn} turns");
            }
        }

        [Test]
        public void OnTurnEnd_MaxSpeed_WithSpeedBoost_NoOverflow()
        {
            // Arrange - Set Speed to max (+6 stages)
            _userSlot.ModifyStatStage(Stat.Speed, 6);
            int initialSpeedStage = _userSlot.GetStatStage(Stat.Speed);
            Assert.That(initialSpeedStage, Is.EqualTo(6), "Speed should be at max (+6)");

            // Act - Trigger OnTurnEnd
            var triggerProcessor = new BattleTriggerProcessor();
            var turnEndActions = triggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);
            
            // Execute all actions
            var allActions = new Queue<BattleAction>(turnEndActions.ToList());
            while (allActions.Count > 0)
            {
                var currentAction = allActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allActions.Enqueue(subReaction);
                }
            }

            // Assert - Speed should remain at max (no overflow)
            int finalSpeedStage = _userSlot.GetStatStage(Stat.Speed);
            Assert.That(finalSpeedStage, Is.EqualTo(6), "Speed should remain at max (+6), no overflow");
        }

        #endregion
    }
}

