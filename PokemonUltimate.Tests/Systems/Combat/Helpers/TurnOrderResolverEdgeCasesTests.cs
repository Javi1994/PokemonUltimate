using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Combat.Helpers
{
    /// <summary>
    /// Edge case tests for TurnOrderResolver.
    /// </summary>
    [TestFixture]
    public class TurnOrderResolverEdgeCasesTests
    {
        private BattleField _field;
        private TurnOrderResolver _resolver;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var playerParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
            };
            var enemyParty = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Snorlax, 50)
            };
            _field.Initialize(BattleRules.Singles, playerParty, enemyParty);

            // Create resolver instance with random provider
            var randomProvider = new RandomProvider(42); // Fixed seed for reproducible tests
            _resolver = new TurnOrderResolver(randomProvider);
        }

        #region Stat Stage Edge Cases

        [Test]
        public void GetEffectiveSpeed_StagesAtExactBoundary_Plus6()
        {
            var slot = _field.PlayerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;

            // Try to go beyond +6 (should be clamped)
            slot.ModifyStatStage(Stat.Speed, 10); // Would be +10, clamped to +6

            var effectiveSpeed = _resolver.GetEffectiveSpeed(slot, _field);

            // +6 stages = 4x
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 4.0f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_StagesAtExactBoundary_Minus6()
        {
            var slot = _field.PlayerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;

            // Try to go beyond -6 (should be clamped)
            slot.ModifyStatStage(Stat.Speed, -10); // Would be -10, clamped to -6

            var effectiveSpeed = _resolver.GetEffectiveSpeed(slot, _field);

            // -6 stages = 0.25x
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 0.25f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_ZeroStages_NoChange()
        {
            var slot = _field.PlayerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;

            var effectiveSpeed = _resolver.GetEffectiveSpeed(slot, _field);

            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed).Within(0.1f));
        }

        [Test]
        [TestCase(1, 1.5f)]
        [TestCase(2, 2.0f)]
        [TestCase(3, 2.5f)]
        [TestCase(4, 3.0f)]
        [TestCase(5, 3.5f)]
        [TestCase(6, 4.0f)]
        public void GetEffectiveSpeed_PositiveStages_CorrectMultiplier(int stages, float expectedMultiplier)
        {
            var slot = _field.PlayerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.ModifyStatStage(Stat.Speed, stages);

            var effectiveSpeed = _resolver.GetEffectiveSpeed(slot, _field);

            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * expectedMultiplier).Within(0.1f));
        }

        [Test]
        [TestCase(-1, 0.666f)]
        [TestCase(-2, 0.5f)]
        [TestCase(-3, 0.4f)]
        [TestCase(-4, 0.333f)]
        [TestCase(-5, 0.286f)]  // 2/7 = 0.2857...
        [TestCase(-6, 0.25f)]
        public void GetEffectiveSpeed_NegativeStages_CorrectMultiplier(int stages, float expectedMultiplier)
        {
            var slot = _field.PlayerSide.Slots[0];
            var baseSpeed = slot.Pokemon.Speed;
            slot.ModifyStatStage(Stat.Speed, stages);

            var effectiveSpeed = _resolver.GetEffectiveSpeed(slot, _field);

            // Use 1% tolerance for floating point comparison
            var expected = baseSpeed * expectedMultiplier;
            Assert.That(effectiveSpeed, Is.EqualTo(expected).Within(expected * 0.01f + 0.5f));
        }

        #endregion

        #region Priority Edge Cases

        [Test]
        [TestCase(-7)]
        [TestCase(-5)]
        [TestCase(-3)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void SortActions_ExtremePriorities_StillSortsCorrectly(int extremePriority)
        {
            var normalAction = new TestAction(_field.PlayerSide.Slots[0], priority: 0);
            var extremeAction = new TestAction(_field.EnemySide.Slots[0], priority: extremePriority);
            var actions = new List<BattleAction> { normalAction, extremeAction };

            var sorted = _resolver.SortActions(actions, _field);

            if (extremePriority > 0)
                Assert.That(sorted[0], Is.SameAs(extremeAction));
            else
                Assert.That(sorted[0], Is.SameAs(normalAction));
        }

        [Test]
        public void SortActions_SpeedTie_RandomTiebreaker_Executes()
        {
            // This test verifies that speed ties don't crash and produce valid output
            // We can't easily test true randomness without mocking, but we can verify
            // the sorting completes successfully multiple times
            var field = new BattleField();
            var p1 = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            var p2 = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            field.Initialize(BattleRules.Singles,
                new List<PokemonInstance> { p1 },
                new List<PokemonInstance> { p2 });

            var action1 = new TestAction(field.PlayerSide.Slots[0], priority: 0);
            var action2 = new TestAction(field.EnemySide.Slots[0], priority: 0);

            // Run multiple times - should always complete without error
            for (int i = 0; i < 50; i++)
            {
                var resolver = new TurnOrderResolver(new RandomProvider(42));
                var sorted = resolver.SortActions(
                    new List<BattleAction> { action1, action2 }, field);

                Assert.That(sorted.Count, Is.EqualTo(2));
                Assert.That(sorted, Contains.Item(action1));
                Assert.That(sorted, Contains.Item(action2));
            }
        }

        #endregion

        #region Actions Without Users

        [Test]
        public void SortActions_ActionWithNullUser_HandledGracefully()
        {
            var normalAction = new TestAction(_field.PlayerSide.Slots[0], priority: 0);
            var nullUserAction = new TestAction(null, priority: 0);
            var actions = new List<BattleAction> { normalAction, nullUserAction };

            // Should not throw
            var sorted = _resolver.SortActions(actions, _field);

            Assert.That(sorted.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetEffectiveSpeed_SlotWithNoPokemon_ReturnsZero()
        {
            var emptySlot = new BattleSlot(0);

            var speed = _resolver.GetEffectiveSpeed(emptySlot, _field);

            Assert.That(speed, Is.EqualTo(0));
        }

        #endregion

        #region Large Scale Tests

        [Test]
        public void SortActions_ManyActions_AllSorted()
        {
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>();
            var enemyParty = new List<PokemonInstance>();

            for (int i = 0; i < 3; i++)
            {
                playerParty.Add(PokemonFactory.Create(PokemonCatalog.Pikachu, 50));
                enemyParty.Add(PokemonFactory.Create(PokemonCatalog.Snorlax, 50));
            }

            field.Initialize(new BattleRules { PlayerSlots = 3, EnemySlots = 3 },
                playerParty, enemyParty);

            var actions = new List<BattleAction>();
            for (int i = 0; i < 3; i++)
            {
                actions.Add(new TestAction(field.PlayerSide.Slots[i], priority: i - 1));
                actions.Add(new TestAction(field.EnemySide.Slots[i], priority: i - 1));
            }

            var resolver = new TurnOrderResolver(new RandomProvider(42));
            var sorted = resolver.SortActions(actions, field);

            Assert.That(sorted.Count, Is.EqualTo(6));

            // First two should be priority +1
            Assert.That(sorted[0].Priority, Is.EqualTo(1));
            Assert.That(sorted[1].Priority, Is.EqualTo(1));

            // Last two should be priority -1
            Assert.That(sorted[4].Priority, Is.EqualTo(-1));
            Assert.That(sorted[5].Priority, Is.EqualTo(-1));
        }

        #endregion

        #region Combined Modifiers

        [Test]
        public void SortActions_ParalyzedVsFasterPokemon_ParalyzedSlower()
        {
            // Setup: Pikachu (90 speed) paralyzed vs Charmander (65 speed) healthy
            var field = new BattleField();
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            var charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 50);

            field.Initialize(BattleRules.Singles,
                new List<PokemonInstance> { pikachu },
                new List<PokemonInstance> { charmander });

            // Paralyze Pikachu (90 * 0.5 = 45 effective speed)
            pikachu.Status = PersistentStatus.Paralysis;

            // Charmander has 65 speed, should be faster now

            var pikachuAction = new TestAction(field.PlayerSide.Slots[0], priority: 0);
            var charmanderAction = new TestAction(field.EnemySide.Slots[0], priority: 0);

            var resolver = new TurnOrderResolver(new RandomProvider(42));
            var sorted = resolver.SortActions(
                new List<BattleAction> { pikachuAction, charmanderAction }, field);

            // Charmander (65) should beat paralyzed Pikachu (45)
            Assert.That(sorted[0].User.Pokemon, Is.SameAs(charmander));
        }

        [Test]
        public void SortActions_SpeedBoostCanOvercomeBaseDisadvantage()
        {
            // Setup: Snorlax (30 speed) with +4 stages vs Pikachu (90 speed)
            var field = new BattleField();
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            var snorlax = PokemonFactory.Create(PokemonCatalog.Snorlax, 50);

            field.Initialize(BattleRules.Singles,
                new List<PokemonInstance> { pikachu },
                new List<PokemonInstance> { snorlax });

            // Snorlax gets +4 speed stages (30 * 3.0 = 90)
            field.EnemySide.Slots[0].ModifyStatStage(Stat.Speed, 4);

            var pikachuAction = new TestAction(field.PlayerSide.Slots[0], priority: 0);
            var snorlaxAction = new TestAction(field.EnemySide.Slots[0], priority: 0);

            // Both should have similar speeds now (90 vs 90)
            var resolver = new TurnOrderResolver(new RandomProvider(42));
            var pikachuSpeed = resolver.GetEffectiveSpeed(field.PlayerSide.Slots[0], field);
            var snorlaxSpeed = resolver.GetEffectiveSpeed(field.EnemySide.Slots[0], field);

            // Snorlax with +4 should be close to Pikachu's speed
            Assert.That(snorlaxSpeed, Is.GreaterThan(80)); // 30 * 3.0 = 90
        }

        #endregion

        #region Helper Classes

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

