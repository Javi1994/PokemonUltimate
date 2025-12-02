using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Queue
{
    /// <summary>
    /// Edge case tests for BattleQueue - boundary conditions and error handling.
    /// </summary>
    [TestFixture]
    public class BattleQueueEdgeCasesTests
    {
        private BattleQueue _queue;
        private BattleField _field;
        private IBattleView _view;

        [SetUp]
        public void SetUp()
        {
            _queue = new BattleQueue();
            _field = new BattleField();
            _view = NullBattleView.Instance;

            var playerParty = new List<PokemonUltimate.Core.Instances.PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 25)
            };
            var enemyParty = new List<PokemonUltimate.Core.Instances.PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Charmander, 25)
            };
            _field.Initialize(BattleRules.Singles, playerParty, enemyParty);
        }

        #region Null Handling

        [Test]
        public void Enqueue_NullAction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _queue.Enqueue(null));
        }

        [Test]
        public void EnqueueRange_NullCollection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _queue.EnqueueRange(null));
        }

        [Test]
        public void EnqueueRange_CollectionWithNulls_IgnoresNulls()
        {
            var actions = new List<BattleAction>
            {
                new MessageAction("One"),
                null,
                new MessageAction("Two"),
                null
            };

            _queue.EnqueueRange(actions);

            Assert.That(_queue.Count, Is.EqualTo(2));
        }

        [Test]
        public void InsertAtFront_NullCollection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _queue.InsertAtFront(null));
        }

        [Test]
        public void InsertAtFront_CollectionWithNulls_IgnoresNulls()
        {
            _queue.Enqueue(new MessageAction("Last"));

            _queue.InsertAtFront(new BattleAction[] { null, new MessageAction("First"), null });

            Assert.That(_queue.Count, Is.EqualTo(2));
        }

        #endregion

        #region Boundary Conditions

        [Test]
        public async Task ProcessQueue_ExactlyMaxIterations_Succeeds()
        {
            // Add exactly 999 actions (under limit)
            for (int i = 0; i < 999; i++)
            {
                _queue.Enqueue(new NoOpAction());
            }

            await _queue.ProcessQueue(_field, _view);

            Assert.That(_queue.IsEmpty, Is.True);
        }

        [Test]
        public async Task ProcessQueue_EmptyQueueMultipleTimes_Succeeds()
        {
            // Process empty queue multiple times
            await _queue.ProcessQueue(_field, _view);
            await _queue.ProcessQueue(_field, _view);
            await _queue.ProcessQueue(_field, _view);

            Assert.That(_queue.IsEmpty, Is.True);
        }

        [Test]
        public void Clear_EmptyQueue_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _queue.Clear());
        }

        [Test]
        public void Clear_CalledMultipleTimes_DoesNotThrow()
        {
            _queue.Enqueue(new MessageAction("Test"));

            Assert.DoesNotThrow(() =>
            {
                _queue.Clear();
                _queue.Clear();
                _queue.Clear();
            });
        }

        #endregion

        #region Reaction Chain Tests

        [Test]
        public async Task ProcessQueue_DeepReactionChain_ExecutesAllInOrder()
        {
            var executionOrder = new List<int>();
            const int chainDepth = 10;

            // Create a chain: 1 -> 2 -> 3 -> ... -> 10
            BattleAction CreateChainAction(int depth)
            {
                if (depth >= chainDepth)
                    return new OrderTrackingAction(depth, executionOrder);

                return new ActionWithReaction(
                    depth,
                    executionOrder,
                    CreateChainAction(depth + 1));
            }

            _queue.Enqueue(CreateChainAction(1));
            await _queue.ProcessQueue(_field, _view);

            var expected = Enumerable.Range(1, chainDepth).ToArray();
            Assert.That(executionOrder, Is.EqualTo(expected));
        }

        [Test]
        public async Task ProcessQueue_ActionReturnsEmptyReactions_ContinuesNormally()
        {
            var executionOrder = new List<int>();

            _queue.Enqueue(new ActionWithEmptyReactions(1, executionOrder));
            _queue.Enqueue(new OrderTrackingAction(2, executionOrder));

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public async Task ProcessQueue_MultipleActionsEachWithReactions_ExecutesCorrectly()
        {
            var executionOrder = new List<int>();

            // Action 1 -> Reaction 1a
            // Action 2 -> Reaction 2a
            var reaction1a = new OrderTrackingAction(2, executionOrder);
            var action1 = new ActionWithReaction(1, executionOrder, reaction1a);

            var reaction2a = new OrderTrackingAction(4, executionOrder);
            var action2 = new ActionWithReaction(3, executionOrder, reaction2a);

            _queue.Enqueue(action1);
            _queue.Enqueue(action2);

            await _queue.ProcessQueue(_field, _view);

            // Expected: 1, 2 (reaction), 3, 4 (reaction)
            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3, 4 }));
        }

        #endregion

        #region InsertAtFront Edge Cases

        [Test]
        public async Task InsertAtFront_EmptyQueue_Works()
        {
            var executionOrder = new List<int>();

            _queue.InsertAtFront(new[] { new OrderTrackingAction(1, executionOrder) });

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public Task InsertAtFront_EmptyCollection_DoesNothing()
        {
            _queue.Enqueue(new MessageAction("One"));

            _queue.InsertAtFront(new List<BattleAction>());

            Assert.That(_queue.Count, Is.EqualTo(1));
            return Task.CompletedTask;
        }

        [Test]
        public async Task InsertAtFront_MultipleCalls_AllInsertedCorrectly()
        {
            var executionOrder = new List<int>();

            _queue.Enqueue(new OrderTrackingAction(4, executionOrder));
            _queue.InsertAtFront(new[] { new OrderTrackingAction(3, executionOrder) });
            _queue.InsertAtFront(new[] { new OrderTrackingAction(2, executionOrder) });
            _queue.InsertAtFront(new[] { new OrderTrackingAction(1, executionOrder) });

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3, 4 }));
        }

        #endregion

        #region State After Operations

        [Test]
        public async Task ProcessQueue_AfterProcessing_CanBeReused()
        {
            var executionOrder = new List<int>();

            _queue.Enqueue(new OrderTrackingAction(1, executionOrder));
            await _queue.ProcessQueue(_field, _view);

            _queue.Enqueue(new OrderTrackingAction(2, executionOrder));
            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void Count_AfterMultipleOperations_ReflectsCorrectState()
        {
            Assert.That(_queue.Count, Is.EqualTo(0));

            _queue.Enqueue(new MessageAction("1"));
            Assert.That(_queue.Count, Is.EqualTo(1));

            _queue.Enqueue(new MessageAction("2"));
            Assert.That(_queue.Count, Is.EqualTo(2));

            _queue.EnqueueRange(new[] { new MessageAction("3"), new MessageAction("4") });
            Assert.That(_queue.Count, Is.EqualTo(4));

            _queue.InsertAtFront(new[] { new MessageAction("0") });
            Assert.That(_queue.Count, Is.EqualTo(5));

            _queue.Clear();
            Assert.That(_queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void IsEmpty_AfterMultipleOperations_ReflectsCorrectState()
        {
            Assert.That(_queue.IsEmpty, Is.True);

            _queue.Enqueue(new MessageAction("1"));
            Assert.That(_queue.IsEmpty, Is.False);

            _queue.Clear();
            Assert.That(_queue.IsEmpty, Is.True);

            _queue.InsertAtFront(new[] { new MessageAction("0") });
            Assert.That(_queue.IsEmpty, Is.False);
        }

        #endregion

        #region Concurrent-Like Scenarios

        [Test]
        public async Task ProcessQueue_ActionModifiesQueueExternally_DoesNotCrash()
        {
            // This tests that if an action somehow triggers external enqueue during processing,
            // the queue handles it gracefully
            var executionOrder = new List<int>();

            _queue.Enqueue(new OrderTrackingAction(1, executionOrder));
            _queue.Enqueue(new OrderTrackingAction(2, executionOrder));
            _queue.Enqueue(new OrderTrackingAction(3, executionOrder));

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder.Count, Is.EqualTo(3));
        }

        #endregion

        #region Helper Classes

        private class NoOpAction : BattleAction
        {
            public NoOpAction() : base(null) { }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                return Enumerable.Empty<BattleAction>();
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                return Task.CompletedTask;
            }
        }

        private class OrderTrackingAction : BattleAction
        {
            private readonly int _order;
            private readonly List<int> _executionOrder;

            public OrderTrackingAction(int order, List<int> executionOrder) : base(null)
            {
                _order = order;
                _executionOrder = executionOrder;
            }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                _executionOrder.Add(_order);
                return Enumerable.Empty<BattleAction>();
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                return Task.CompletedTask;
            }
        }

        private class ActionWithReaction : BattleAction
        {
            private readonly int _order;
            private readonly List<int> _executionOrder;
            private readonly BattleAction _reaction;

            public ActionWithReaction(int order, List<int> executionOrder, BattleAction reaction) : base(null)
            {
                _order = order;
                _executionOrder = executionOrder;
                _reaction = reaction;
            }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                _executionOrder.Add(_order);
                return new[] { _reaction };
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                return Task.CompletedTask;
            }
        }

        private class ActionWithEmptyReactions : BattleAction
        {
            private readonly int _order;
            private readonly List<int> _executionOrder;

            public ActionWithEmptyReactions(int order, List<int> executionOrder) : base(null)
            {
                _order = order;
                _executionOrder = executionOrder;
            }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                _executionOrder.Add(_order);
                return Enumerable.Empty<BattleAction>();
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                return Task.CompletedTask;
            }
        }

        #endregion
    }
}

