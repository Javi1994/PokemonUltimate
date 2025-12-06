using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Static ability - chance to paralyze on contact.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.17: Advanced Abilities
    /// **Documentation**: See `docs/features/2-combat-system/2.17-advanced-abilities/README.md`
    /// </remarks>
    [TestFixture]
    public class StaticTests
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
            
            // Set Static ability on target (defender)
            _target.SetAbility(AbilityCatalog.Static);
            
            // Use a contact move (Tackle makes contact)
            _contactMove = new MoveInstance(MoveCatalog.Tackle);
            _user.Moves[0] = _contactMove;
        }

        #region OnContactReceived Tests

        [Test]
        public void ExecuteLogic_ContactMove_WithStatic_MayParalyzeAttacker()
        {
            // Arrange - Ensure attacker has no status
            _user.Status = PersistentStatus.None;
            _target.CurrentHP = _target.MaxHP; // Full HP to ensure damage is dealt

            // Act - Use contact move multiple times to test probability
            bool paralyzed = false;
            for (int i = 0; i < 50; i++) // Test multiple times due to 30% chance
            {
                _user.Status = PersistentStatus.None; // Reset status
                _target.CurrentHP = _target.MaxHP; // Reset HP
                
                var action = new UseMoveAction(_userSlot, _targetSlot, new MoveInstance(MoveCatalog.Tackle));
                var reactions = action.ExecuteLogic(_field).ToList();

                // Execute damage actions first to trigger OnContactReceived
                var damageActions = reactions.Where(r => r is DamageAction).ToList();
                foreach (var damageAction in damageActions)
                {
                    var damageReactions = damageAction.ExecuteLogic(_field).ToList();
                    // Execute OnContactReceived reactions (Static, Rough Skin, Rocky Helmet)
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

                if (_user.Status == PersistentStatus.Paralysis)
                {
                    paralyzed = true;
                    break;
                }
            }

            // Assert - Static should have a chance to paralyze
            // Note: With 30% chance over 50 attempts, probability of never paralyzing is very low
            Assert.That(paralyzed, Is.True, "Static should have chance to paralyze attacker");
        }

        [Test]
        public void ExecuteLogic_NonContactMove_WithStatic_NoParalysis()
        {
            // Arrange - Use non-contact move (Thunderbolt doesn't make contact)
            _user.Status = PersistentStatus.None;
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

            // Assert - Non-contact moves shouldn't trigger Static
            Assert.That(_user.Status, Is.EqualTo(PersistentStatus.None), "Non-contact moves shouldn't trigger Static");
        }

        [Test]
        public void ExecuteLogic_ContactMove_AttackerAlreadyParalyzed_NoStatusChange()
        {
            // Arrange - Attacker already has status
            _user.Status = PersistentStatus.Paralysis;
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

            // Assert - Status should remain unchanged
            Assert.That(_user.Status, Is.EqualTo(PersistentStatus.Paralysis), "Status should remain unchanged if already paralyzed");
        }

        [Test]
        public void ExecuteLogic_WithoutStatic_NoParalysis()
        {
            // Arrange - Remove Static ability
            _target.SetAbility(null);
            _user.Status = PersistentStatus.None;
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

            // Assert - No paralysis without Static
            Assert.That(_user.Status, Is.EqualTo(PersistentStatus.None), "No paralysis without Static");
        }

        #endregion
    }
}

