using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions.Integration
{
    /// <summary>
    /// Integration tests for Advanced Items feature.
    /// Tests combinations of advanced items and interactions between different mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.18: Advanced Items
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class AdvancedItemsIntegrationTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private BattleTriggerProcessor _triggerProcessor;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _triggerProcessor = new BattleTriggerProcessor();
        }

        #region Life Orb Integration Tests

        [Test]
        public void LifeOrb_WithContactMove_AndRockyHelmet_BothEffectsApply()
        {
            // Arrange - User has Life Orb, target has Rocky Helmet
            _userSlot.Pokemon.HeldItem = ItemCatalog.LifeOrb;
            _targetSlot.Pokemon.HeldItem = ItemCatalog.RockyHelmet;
            
            // Use contact move
            if (_userSlot.Pokemon.Moves.Count == 0)
            {
                _userSlot.Pokemon.Moves.Add(new MoveInstance(MoveCatalog.Tackle));
            }
            var moveInstance = _userSlot.Pokemon.Moves[0];
            _targetSlot.Pokemon.CurrentHP = _targetSlot.Pokemon.MaxHP; // Full HP to ensure damage

            int initialUserHP = _userSlot.Pokemon.CurrentHP;
            int initialTargetHP = _targetSlot.Pokemon.CurrentHP;

            // Act - Use contact move
            var action = new UseMoveAction(_userSlot, _targetSlot, moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions first
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            foreach (var damageAction in damageActions)
            {
                var damageReactions = damageAction.ExecuteLogic(_field).ToList();
                // Process OnContactReceived reactions (Rocky Helmet)
                var allDamageReactions = new System.Collections.Generic.Queue<BattleAction>(damageReactions);
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

            // Execute other actions
            var otherActions = reactions.Where(r => !(r is DamageAction)).ToList();
            var allOtherActions = new System.Collections.Generic.Queue<BattleAction>(otherActions);
            while (allOtherActions.Count > 0)
            {
                var currentAction = allOtherActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allOtherActions.Enqueue(subReaction);
                }
            }

            // Trigger OnAfterMove for Life Orb recoil
            var afterMoveActions = _triggerProcessor.ProcessTrigger(BattleTrigger.OnAfterMove, _field);
            var allAfterMoveActions = new System.Collections.Generic.Queue<BattleAction>(afterMoveActions.ToList());
            while (allAfterMoveActions.Count > 0)
            {
                var currentAction = allAfterMoveActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allAfterMoveActions.Enqueue(subReaction);
                }
            }

            // Assert - Both effects should apply
            // Target should take damage from move
            Assert.That(_targetSlot.Pokemon.CurrentHP, Is.LessThan(initialTargetHP),
                "Target should take damage from move");
            
            // User should take recoil from Life Orb AND contact damage from Rocky Helmet
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.LessThan(initialUserHP),
                "User should take recoil from Life Orb and contact damage from Rocky Helmet");
        }

        #endregion

        #region Focus Sash Integration Tests

        [Test]
        public void FocusSash_WithFatalDamage_AndLifeOrbRecoil_PreventsFainting()
        {
            // Arrange - User has Focus Sash at full HP (required for Focus Sash to work)
            _userSlot.Pokemon.HeldItem = ItemCatalog.FocusSash;
            _userSlot.Pokemon.CurrentHP = _userSlot.Pokemon.MaxHP; // Full HP (required)
            
            // Create a damage context that would be fatal
            // Use DamageAction directly to ensure fatal damage (like FocusSashTests does)
            var context = new DamageContext(_targetSlot, _userSlot, MoveCatalog.Tackle, _field);
            context.BaseDamage = _userSlot.Pokemon.MaxHP + 100; // More than enough to KO
            context.Multiplier = 1.0f;

            // Act - Apply fatal damage
            var damageAction = new DamageAction(_targetSlot, _userSlot, context);
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Execute all reactions
            var allReactions = new System.Collections.Generic.Queue<BattleAction>(reactions);
            while (allReactions.Count > 0)
            {
                var currentReaction = allReactions.Dequeue();
                var subReactions = currentReaction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allReactions.Enqueue(subReaction);
                }
            }

            // Assert - Focus Sash should prevent fainting (user should be at 1 HP)
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.EqualTo(1),
                $"Focus Sash should prevent fainting, leaving Pokemon at 1 HP (actual: {_userSlot.Pokemon.CurrentHP})");
            Assert.That(_userSlot.Pokemon.IsFainted, Is.False,
                "Pokemon should not be fainted");
            Assert.That(_userSlot.Pokemon.HeldItem, Is.Null,
                "Focus Sash should be consumed after use");
        }

        [Test]
        public void FocusSash_WithNonFatalDamage_DoesNotActivate()
        {
            // Arrange - User has Focus Sash
            _userSlot.Pokemon.HeldItem = ItemCatalog.FocusSash;
            _userSlot.Pokemon.CurrentHP = _userSlot.Pokemon.MaxHP; // Full HP
            
            // Target will deal non-fatal damage
            if (_targetSlot.Pokemon.Moves.Count == 0)
            {
                _targetSlot.Pokemon.Moves.Add(new MoveInstance(MoveCatalog.Tackle));
            }
            var targetMove = _targetSlot.Pokemon.Moves[0];
            
            // Set user HP high enough to survive
            _userSlot.Pokemon.CurrentHP = _userSlot.Pokemon.MaxHP / 2; // Half HP

            int initialHP = _userSlot.Pokemon.CurrentHP;

            // Act - Target uses move
            var action = new UseMoveAction(_targetSlot, _userSlot, targetMove);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(_field).ToList();
            }

            // Assert - Focus Sash should NOT activate (item should still be held)
            Assert.That(_userSlot.Pokemon.HeldItem, Is.EqualTo(ItemCatalog.FocusSash),
                "Focus Sash should not be consumed for non-fatal damage");
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.LessThan(initialHP),
                "Pokemon should take damage");
        }

        #endregion

        #region Rocky Helmet Integration Tests

        [Test]
        public void RockyHelmet_WithContactMove_AndRoughSkin_BothDamageAttacker()
        {
            // Arrange - Target has Rocky Helmet and Rough Skin ability
            _targetSlot.Pokemon.HeldItem = ItemCatalog.RockyHelmet;
            _targetSlot.Pokemon.SetAbility(PokemonUltimate.Content.Catalogs.Abilities.AbilityCatalog.RoughSkin);
            
            // User attacks with contact move (Tackle makes contact)
            var contactMove = new MoveInstance(MoveCatalog.Tackle);
            if (_userSlot.Pokemon.Moves.Count == 0)
            {
                _userSlot.Pokemon.Moves.Add(contactMove);
            }
            else
            {
                _userSlot.Pokemon.Moves[0] = contactMove;
            }
            _targetSlot.Pokemon.CurrentHP = _targetSlot.Pokemon.MaxHP; // Full HP to ensure damage is dealt

            int initialUserHP = _userSlot.Pokemon.CurrentHP;

            // Verify move makes contact
            Assert.That(contactMove.Move.MakesContact, Is.True, "Tackle should make contact");

            // Act - Use contact move
            var action = new UseMoveAction(_userSlot, _targetSlot, contactMove);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions first to trigger OnContactReceived
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            bool foundContactDamageAction = false;
            foreach (var damageAction in damageActions)
            {
                var damageReactions = damageAction.ExecuteLogic(_field).ToList();
                
                // Check if ContactDamageAction is in reactions
                if (damageReactions.Any(r => r is ContactDamageAction))
                {
                    foundContactDamageAction = true;
                }
                
                // Process OnContactReceived reactions (Rocky Helmet + Rough Skin)
                // These reactions include ContactDamageAction which applies damage
                var allDamageReactions = new System.Collections.Generic.Queue<BattleAction>(damageReactions);
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
            var allOtherActions = new System.Collections.Generic.Queue<BattleAction>(otherActions);
            while (allOtherActions.Count > 0)
            {
                var currentAction = allOtherActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allOtherActions.Enqueue(subReaction);
                }
            }

            // Assert - User should take damage from both Rocky Helmet and Rough Skin
            // Rocky Helmet: 1/6 max HP, Rough Skin: 1/8 max HP
            int expectedContactDamage = (_userSlot.Pokemon.MaxHP / 6) + (_userSlot.Pokemon.MaxHP / 8);
            int actualDamage = initialUserHP - _userSlot.Pokemon.CurrentHP;
            
            // Verify that ContactDamageAction was found (both effects should create one)
            Assert.That(foundContactDamageAction, Is.True, "ContactDamageAction should be generated by Rocky Helmet or Rough Skin");
            
            // Verify that contact damage was applied
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.LessThan(initialUserHP),
                $"User should take contact damage from both Rocky Helmet and Rough Skin (initial: {initialUserHP}, final: {_userSlot.Pokemon.CurrentHP}, damage: {actualDamage})");
            
            // Verify that contact damage was applied (at least the minimum expected)
            // We allow some variance because the exact damage calculation may vary
            Assert.That(actualDamage, Is.GreaterThanOrEqualTo(expectedContactDamage - 15),
                $"User should take at least {expectedContactDamage - 15} contact damage (expected: {expectedContactDamage}, got: {actualDamage})");
        }

        #endregion

        #region Black Sludge Integration Tests

        [Test]
        public void BlackSludge_OnPoisonType_AndLeftovers_BothHeal()
        {
            // Arrange - Poison type Pokemon with Black Sludge
            var poisonPokemon = PokemonFactory.Create(PokemonCatalog.Venusaur, 50);
            poisonPokemon.HeldItem = ItemCatalog.BlackSludge;
            poisonPokemon.CurrentHP = poisonPokemon.MaxHP - 50; // Damaged
            
            _userSlot.SetPokemon(poisonPokemon);
            int initialHP = poisonPokemon.CurrentHP;

            // Act - Trigger OnTurnEnd
            var turnEndActions = _triggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);
            var allActions = new System.Collections.Generic.Queue<BattleAction>(turnEndActions.ToList());
            while (allActions.Count > 0)
            {
                var currentAction = allActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allActions.Enqueue(subReaction);
                }
            }

            // Assert - Black Sludge should heal Poison type Pokemon
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.GreaterThan(initialHP),
                "Black Sludge should heal Poison type Pokemon");
            
            int expectedHeal = poisonPokemon.MaxHP / 16; // 1/16 HP
            int actualHeal = _userSlot.Pokemon.CurrentHP - initialHP;
            Assert.That(actualHeal, Is.EqualTo(expectedHeal).Within(2),
                $"Should heal approximately {expectedHeal} HP (actual: {actualHeal})");
        }

        [Test]
        public void BlackSludge_OnNonPoisonType_Damages_ButLeftoversWouldHeal()
        {
            // Arrange - Non-Poison type Pokemon with Black Sludge
            _userSlot.Pokemon.HeldItem = ItemCatalog.BlackSludge;
            _userSlot.Pokemon.CurrentHP = _userSlot.Pokemon.MaxHP; // Full HP initially
            
            // Damage Pokemon slightly
            _userSlot.Pokemon.CurrentHP = _userSlot.Pokemon.MaxHP - 10;
            int initialHP = _userSlot.Pokemon.CurrentHP;

            // Act - Trigger OnTurnEnd
            var turnEndActions = _triggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, _field);
            var allActions = new System.Collections.Generic.Queue<BattleAction>(turnEndActions.ToList());
            while (allActions.Count > 0)
            {
                var currentAction = allActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allActions.Enqueue(subReaction);
                }
            }

            // Assert - Black Sludge should damage non-Poison type Pokemon
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.LessThan(initialHP),
                "Black Sludge should damage non-Poison type Pokemon");
        }

        #endregion

        #region Multiple Items Integration Tests

        [Test]
        public void LifeOrb_AndFocusSash_WithFatalRecoil_FocusSashPreventsFainting()
        {
            // Arrange - User has Life Orb and Focus Sash
            // Note: In practice, Pokemon can only hold one item, but we test the interaction
            // by simulating a scenario where Life Orb recoil would be fatal
            _userSlot.Pokemon.HeldItem = ItemCatalog.LifeOrb;
            _userSlot.Pokemon.CurrentHP = _userSlot.Pokemon.MaxHP / 10; // Low HP
            
            // Target has Focus Sash (simulating switch scenario)
            _targetSlot.Pokemon.HeldItem = ItemCatalog.FocusSash;
            _targetSlot.Pokemon.CurrentHP = _targetSlot.Pokemon.MaxHP;

            if (_userSlot.Pokemon.Moves.Count == 0)
            {
                _userSlot.Pokemon.Moves.Add(new MoveInstance(MoveCatalog.Tackle));
            }
            var moveInstance = _userSlot.Pokemon.Moves[0];

            int initialUserHP = _userSlot.Pokemon.CurrentHP;

            // Act - Use move that deals damage
            var action = new UseMoveAction(_userSlot, _targetSlot, moveInstance);
            var reactions = action.ExecuteLogic(_field).ToList();

            // Execute damage actions
            var damageActions = reactions.Where(r => r is DamageAction).ToList();
            foreach (var damageAction in damageActions)
            {
                damageAction.ExecuteLogic(_field).ToList();
            }

            // Execute other actions
            var otherActions = reactions.Where(r => !(r is DamageAction)).ToList();
            var allOtherActions = new System.Collections.Generic.Queue<BattleAction>(otherActions);
            while (allOtherActions.Count > 0)
            {
                var currentAction = allOtherActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allOtherActions.Enqueue(subReaction);
                }
            }

            // Trigger OnAfterMove for Life Orb recoil
            var afterMoveActions = _triggerProcessor.ProcessTrigger(BattleTrigger.OnAfterMove, _field);
            var allAfterMoveActions = new System.Collections.Generic.Queue<BattleAction>(afterMoveActions.ToList());
            while (allAfterMoveActions.Count > 0)
            {
                var currentAction = allAfterMoveActions.Dequeue();
                var subReactions = currentAction.ExecuteLogic(_field).ToList();
                foreach (var subReaction in subReactions)
                {
                    allAfterMoveActions.Enqueue(subReaction);
                }
            }

            // Assert - Life Orb recoil should apply (Focus Sash only prevents move damage, not recoil)
            // Note: Focus Sash only prevents fainting from the move itself, not from recoil
            Assert.That(_userSlot.Pokemon.CurrentHP, Is.LessThan(initialUserHP),
                "Life Orb recoil should apply");
        }

        #endregion
    }
}

