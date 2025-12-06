using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Rocky Helmet item - damages attacker on contact.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.18: Advanced Items
    /// **Documentation**: See `docs/features/2-combat-system/2.18-advanced-items/README.md`
    /// </remarks>
    [TestFixture]
    public class RockyHelmetTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private MoveInstance _contactMove;

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
            
            // Set Rocky Helmet on target
            _target.HeldItem = ItemCatalog.RockyHelmet;
            
            // Use a contact move
            _contactMove = new MoveInstance(MoveCatalog.Tackle);
            _user.Moves[0] = _contactMove;
        }

        #region OnContactReceived Tests

        [Test]
        public void ExecuteLogic_ContactMove_WithRockyHelmet_DamagesAttacker()
        {
            // Arrange
            int initialUserHP = _user.CurrentHP;
            _target.CurrentHP = _target.MaxHP; // Full HP to ensure damage is dealt

            // Act - Use contact move
            var action = new UseMoveAction(_userSlot, _targetSlot, new MoveInstance(MoveCatalog.Tackle));
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions first to trigger OnContactReceived
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            foreach (var damageAction in damageActions)
            {
                var damageReactions = damageAction.ExecuteLogic(_field).ToList();
                // Execute OnContactReceived reactions (Rocky Helmet)
                var allDamageReactions = new Queue<BattleAction>(damageReactions);
                while (allDamageReactions.Count > 0)
                {
                    var currentReaction = allDamageReactions.Dequeue();
                    var subReactions = currentReaction.ExecuteLogic(_field).ToList();
                    foreach (var subReaction in subReactions)
                    {
                        allDamageReactions.Enqueue(subReaction);
                    }
                }
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

            // Assert - Rocky Helmet should damage attacker
            Assert.That(_user.CurrentHP, Is.LessThan(initialUserHP), "Rocky Helmet should damage attacker");
            int expectedDamage = _user.MaxHP / 6; // 1/6 of max HP
            Assert.That(_user.CurrentHP, Is.EqualTo(initialUserHP - expectedDamage), $"Should deal {expectedDamage} damage");
        }

        [Test]
        public void ExecuteLogic_NonContactMove_WithRockyHelmet_NoDamage()
        {
            // Arrange - Use non-contact move
            int initialUserHP = _user.CurrentHP;
            var nonContactMove = new MoveInstance(MoveCatalog.Thunderbolt);
            _user.Moves[0] = nonContactMove;

            // Act
            var action = new UseMoveAction(_userSlot, _targetSlot, nonContactMove);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Process all reactions
            foreach (var reaction in reactions)
            {
                if (reaction is DamageAction damageAction)
                {
                    damageAction.ExecuteLogic(_field).ToList();
                }
            }

            // Assert - Non-contact moves shouldn't trigger Rocky Helmet
            Assert.That(_user.CurrentHP, Is.EqualTo(initialUserHP), "Non-contact moves shouldn't trigger Rocky Helmet");
        }

        [Test]
        public void ExecuteLogic_WithoutRockyHelmet_NoContactDamage()
        {
            // Arrange - Remove Rocky Helmet
            _target.HeldItem = null;
            int initialUserHP = _user.CurrentHP;
            _target.CurrentHP = _target.MaxHP;

            // Act
            var action = new UseMoveAction(_userSlot, _targetSlot, new MoveInstance(MoveCatalog.Tackle));
            var reactions = action.ExecuteLogic(_field).ToList();

            // Process all reactions
            foreach (var reaction in reactions)
            {
                if (reaction is DamageAction damageAction)
                {
                    damageAction.ExecuteLogic(_field).ToList();
                }
            }

            // Assert - No contact damage without Rocky Helmet
            Assert.That(_user.CurrentHP, Is.EqualTo(initialUserHP), "No contact damage without Rocky Helmet");
        }

        #endregion
    }
}

