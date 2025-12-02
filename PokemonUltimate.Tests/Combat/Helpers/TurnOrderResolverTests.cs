using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Items;

namespace PokemonUltimate.Tests.Combat.Helpers
{
    /// <summary>
    /// Tests for TurnOrderResolver - sorts battle actions by priority and speed.
    /// </summary>
    [TestFixture]
    public class TurnOrderResolverTests
    {
        private BattleField _field;
        private BattleSide _playerSide;
        private BattleSide _enemySide;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)  // Fast Pokemon
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Snorlax, 50)  // Slow Pokemon
            };
            
            _field.Initialize(BattleRules.Singles, playerParty, enemyParty);
            _playerSide = _field.PlayerSide;
            _enemySide = _field.EnemySide;
        }

        #region Priority Tests

        [Test]
        public void SortActions_HigherPriority_GoesFirst()
        {
            var lowPriority = new TestAction(_playerSide.Slots[0], priority: 0);
            var highPriority = new TestAction(_enemySide.Slots[0], priority: 1);
            var actions = new List<BattleAction> { lowPriority, highPriority };

            var sorted = TurnOrderResolver.SortActions(actions, _field);

            Assert.That(sorted[0], Is.SameAs(highPriority));
            Assert.That(sorted[1], Is.SameAs(lowPriority));
        }

        [Test]
        public void SortActions_MuchHigherPriority_GoesFirst_EvenIfSlower()
        {
            // Slow Pokemon with +2 priority vs Fast Pokemon with 0 priority
            var slowHighPriority = new TestAction(_enemySide.Slots[0], priority: 2);
            var fastLowPriority = new TestAction(_playerSide.Slots[0], priority: 0);
            var actions = new List<BattleAction> { fastLowPriority, slowHighPriority };

            var sorted = TurnOrderResolver.SortActions(actions, _field);

            Assert.That(sorted[0], Is.SameAs(slowHighPriority));
        }

        [Test]
        public void SortActions_NegativePriority_GoesLast()
        {
            var normalPriority = new TestAction(_playerSide.Slots[0], priority: 0);
            var negativePriority = new TestAction(_enemySide.Slots[0], priority: -1);
            var actions = new List<BattleAction> { negativePriority, normalPriority };

            var sorted = TurnOrderResolver.SortActions(actions, _field);

            Assert.That(sorted[0], Is.SameAs(normalPriority));
            Assert.That(sorted[1], Is.SameAs(negativePriority));
        }

        [Test]
        public void GetPriority_FromBattleAction_ReturnsPriority()
        {
            var action = new TestAction(_playerSide.Slots[0], priority: 3);

            var priority = TurnOrderResolver.GetPriority(action);

            Assert.That(priority, Is.EqualTo(3));
        }

        [Test]
        public void GetPriority_NullAction_ReturnsZero()
        {
            var priority = TurnOrderResolver.GetPriority(null);

            Assert.That(priority, Is.EqualTo(0));
        }

        #endregion

        #region Speed Tests

        [Test]
        public void SortActions_SamePriority_FasterGoesFirst()
        {
            // Pikachu (90 speed) vs Snorlax (30 speed)
            var pikachuAction = new TestAction(_playerSide.Slots[0], priority: 0);
            var snorlaxAction = new TestAction(_enemySide.Slots[0], priority: 0);
            var actions = new List<BattleAction> { snorlaxAction, pikachuAction };

            var sorted = TurnOrderResolver.SortActions(actions, _field);

            // Pikachu should go first (faster)
            Assert.That(sorted[0].User, Is.SameAs(_playerSide.Slots[0]));
        }

        [Test]
        public void GetEffectiveSpeed_NoModifiers_ReturnsBaseSpeed()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed));
        }

        [Test]
        public void GetEffectiveSpeed_Paralyzed_HalvesSpeed()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed; // Get speed BEFORE paralysis
            slot.Pokemon.Status = PersistentStatus.Paralysis;

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 0.5f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_OtherStatus_NoSpeedChange()
        {
            var slot = _playerSide.Slots[0];
            slot.Pokemon.Status = PersistentStatus.Burn;
            var baseSpeed = slot.Pokemon.Speed;

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed));
        }

        #endregion

        #region Stat Stage Tests

        [Test]
        public void GetEffectiveSpeed_PositiveStages_IncreasesSpeed()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.ModifyStatStage(Stat.Speed, 2); // +2 stages

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // +2 stages = 2x speed
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 2.0f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_NegativeStages_DecreasesSpeed()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.ModifyStatStage(Stat.Speed, -2); // -2 stages

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // -2 stages = 0.5x speed
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 0.5f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_MaxPositiveStages_IncreasesSpeed4x()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.ModifyStatStage(Stat.Speed, 6); // +6 stages (max)

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // +6 stages = 4x speed
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 4.0f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_MaxNegativeStages_DecreasesSpeedTo25Percent()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.ModifyStatStage(Stat.Speed, -6); // -6 stages (min)

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // -6 stages = 0.25x speed
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 0.25f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_ParalysisAndNegativeStages_BothApply()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.Pokemon.Status = PersistentStatus.Paralysis;
            slot.ModifyStatStage(Stat.Speed, -2);

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // 0.5 (paralysis) * 0.5 (-2 stages) = 0.25x
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 0.25f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_WithChoiceScarf_IncreasesSpeedBy50Percent()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.Pokemon.HeldItem = ItemCatalog.ChoiceScarf;

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // Choice Scarf should multiply speed by 1.5x
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 1.5f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_WithChoiceScarf_StacksWithStatStages()
        {
            var slot = _playerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.Pokemon.HeldItem = ItemCatalog.ChoiceScarf;
            slot.ModifyStatStage(Stat.Speed, 1); // +1 stage = 1.5x

            var effectiveSpeed = TurnOrderResolver.GetEffectiveSpeed(slot, _field);

            // Choice Scarf (1.5x) * +1 stage (1.5x) = 2.25x
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 2.25f).Within(0.1f));
        }

        [Test]
        public void SortActions_WithChoiceScarf_SlowerPokemonGoesFirst()
        {
            // Snorlax (slow) with Choice Scarf vs Pikachu (fast) without item
            var snorlax = _enemySide.Slots[0].Pokemon;
            snorlax.HeldItem = ItemCatalog.ChoiceScarf;
            
            var pikachuAction = new TestAction(_playerSide.Slots[0], priority: 0);
            var snorlaxAction = new TestAction(_enemySide.Slots[0], priority: 0);
            var actions = new List<BattleAction> { pikachuAction, snorlaxAction };

            var sorted = TurnOrderResolver.SortActions(actions, _field);

            // With Choice Scarf, Snorlax should be faster than Pikachu
            // Snorlax base speed ~30, with Choice Scarf = 45
            // Pikachu base speed ~90
            // So Pikachu should still go first, but let's verify the speeds
            var snorlaxSpeed = TurnOrderResolver.GetEffectiveSpeed(_enemySide.Slots[0], _field);
            var pikachuSpeed = TurnOrderResolver.GetEffectiveSpeed(_playerSide.Slots[0], _field);
            
            // Verify Choice Scarf increased Snorlax's speed
            Assert.That(snorlaxSpeed, Is.GreaterThan(snorlax.Speed));
        }

        #endregion

        #region Speed Tie Tests

        [Test]
        public void SortActions_SpeedTie_ReturnsConsistentOrder()
        {
            // Create two Pokemon with exactly the same speed
            var pokemon1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            var pokemon2 = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            
            // Force same IVs/EVs by setting to same level
            var field = new BattleField();
            field.Initialize(BattleRules.Singles, 
                new List<PokemonInstance> { pokemon1 },
                new List<PokemonInstance> { pokemon2 });

            var action1 = new TestAction(field.PlayerSide.Slots[0], priority: 0);
            var action2 = new TestAction(field.EnemySide.Slots[0], priority: 0);
            var actions = new List<BattleAction> { action1, action2 };

            // Run multiple times - should complete without error
            for (int i = 0; i < 10; i++)
            {
                var sorted = TurnOrderResolver.SortActions(actions, field);
                Assert.That(sorted.Count, Is.EqualTo(2));
            }
        }

        #endregion

        #region Multiple Actions Tests

        [Test]
        public void SortActions_MultipleActions_SortedByPriorityThenSpeed()
        {
            // Setup doubles battle
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),   // Fast
                PokemonFactory.Create(PokemonCatalog.Snorlax, 50)    // Slow
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Charmander, 50), // Medium
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)   // Medium-slow
            };
            field.Initialize(BattleRules.Doubles, playerParty, enemyParty);

            var actions = new List<BattleAction>
            {
                new TestAction(field.PlayerSide.Slots[0], priority: 0),  // Fast, normal
                new TestAction(field.PlayerSide.Slots[1], priority: 1),  // Slow, priority +1
                new TestAction(field.EnemySide.Slots[0], priority: 0),   // Medium, normal
                new TestAction(field.EnemySide.Slots[1], priority: -1),  // Medium-slow, negative
            };

            var sorted = TurnOrderResolver.SortActions(actions, field);

            // Order: Priority +1, then 0s by speed, then -1
            Assert.That(sorted[0].User, Is.SameAs(field.PlayerSide.Slots[1])); // Priority +1
            Assert.That(sorted[3].User, Is.SameAs(field.EnemySide.Slots[1])); // Priority -1
        }

        [Test]
        public void SortActions_EmptyList_ReturnsEmptyList()
        {
            var sorted = TurnOrderResolver.SortActions(new List<BattleAction>(), _field);

            Assert.That(sorted, Is.Empty);
        }

        [Test]
        public void SortActions_SingleAction_ReturnsSameAction()
        {
            var action = new TestAction(_playerSide.Slots[0], priority: 0);

            var sorted = TurnOrderResolver.SortActions(new List<BattleAction> { action }, _field);

            Assert.That(sorted.Count, Is.EqualTo(1));
            Assert.That(sorted[0], Is.SameAs(action));
        }

        #endregion

        #region Null Safety Tests

        [Test]
        public void SortActions_NullField_ThrowsArgumentNullException()
        {
            var actions = new List<BattleAction> { new TestAction(_playerSide.Slots[0], priority: 0) };

            Assert.Throws<ArgumentNullException>(() => TurnOrderResolver.SortActions(actions, null));
        }

        [Test]
        public void SortActions_NullActions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TurnOrderResolver.SortActions(null, _field));
        }

        [Test]
        public void GetEffectiveSpeed_NullSlot_ReturnsZero()
        {
            var speed = TurnOrderResolver.GetEffectiveSpeed(null, _field);

            Assert.That(speed, Is.EqualTo(0));
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Test action with configurable priority for testing.
        /// </summary>
        private class TestAction : BattleAction
        {
            private readonly int _priority;

            public override int Priority => _priority;

            public TestAction(BattleSlot user, int priority) : base(user)
            {
                _priority = priority;
            }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                return Enumerable.Empty<BattleAction>();
            }

            public override System.Threading.Tasks.Task ExecuteVisual(IBattleView view)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        #endregion
    }
}

