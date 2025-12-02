using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Combat.Queue
{
    /// <summary>
    /// Tests for BattleQueue - the action processing system.
    /// </summary>
    [TestFixture]
    public class BattleQueueTests
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

        #region Basic Queue Operations

        [Test]
        public void Constructor_CreatesEmptyQueue()
        {
            var queue = new BattleQueue();

            Assert.That(queue.Count, Is.EqualTo(0));
            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        public void Enqueue_SingleAction_IncreasesCount()
        {
            var action = new MessageAction("Test");

            _queue.Enqueue(action);

            Assert.That(_queue.Count, Is.EqualTo(1));
            Assert.That(_queue.IsEmpty, Is.False);
        }

        [Test]
        public void Enqueue_MultipleActions_IncreasesCount()
        {
            _queue.Enqueue(new MessageAction("One"));
            _queue.Enqueue(new MessageAction("Two"));
            _queue.Enqueue(new MessageAction("Three"));

            Assert.That(_queue.Count, Is.EqualTo(3));
        }

        [Test]
        public void EnqueueRange_MultipleActions_AddsAll()
        {
            var actions = new List<BattleAction>
            {
                new MessageAction("One"),
                new MessageAction("Two"),
                new MessageAction("Three")
            };

            _queue.EnqueueRange(actions);

            Assert.That(_queue.Count, Is.EqualTo(3));
        }

        [Test]
        public void EnqueueRange_EmptyList_DoesNothing()
        {
            _queue.EnqueueRange(new List<BattleAction>());

            Assert.That(_queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void Clear_RemovesAllActions()
        {
            _queue.Enqueue(new MessageAction("One"));
            _queue.Enqueue(new MessageAction("Two"));

            _queue.Clear();

            Assert.That(_queue.Count, Is.EqualTo(0));
            Assert.That(_queue.IsEmpty, Is.True);
        }

        #endregion

        #region ProcessQueue Tests

        [Test]
        public async Task ProcessQueue_EmptyQueue_CompletesImmediately()
        {
            await _queue.ProcessQueue(_field, _view);

            Assert.That(_queue.IsEmpty, Is.True);
        }

        [Test]
        public async Task ProcessQueue_SingleAction_ExecutesLogic()
        {
            var tracker = new ExecutionTracker();
            var action = new TrackingAction(tracker);
            _queue.Enqueue(action);

            await _queue.ProcessQueue(_field, _view);

            Assert.That(tracker.LogicExecuted, Is.True);
        }

        [Test]
        public async Task ProcessQueue_SingleAction_ExecutesVisual()
        {
            var tracker = new ExecutionTracker();
            var action = new TrackingAction(tracker);
            _queue.Enqueue(action);

            await _queue.ProcessQueue(_field, _view);

            Assert.That(tracker.VisualExecuted, Is.True);
        }

        [Test]
        public async Task ProcessQueue_SingleAction_ExecutesLogicBeforeVisual()
        {
            var tracker = new ExecutionTracker();
            var action = new TrackingAction(tracker);
            _queue.Enqueue(action);

            await _queue.ProcessQueue(_field, _view);

            Assert.That(tracker.LogicExecutedFirst, Is.True);
        }

        [Test]
        public async Task ProcessQueue_MultipleActions_ExecutesInOrder()
        {
            var executionOrder = new List<int>();
            _queue.Enqueue(new OrderTrackingAction(1, executionOrder));
            _queue.Enqueue(new OrderTrackingAction(2, executionOrder));
            _queue.Enqueue(new OrderTrackingAction(3, executionOrder));

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public async Task ProcessQueue_EmptiesQueue()
        {
            _queue.Enqueue(new MessageAction("One"));
            _queue.Enqueue(new MessageAction("Two"));

            await _queue.ProcessQueue(_field, _view);

            Assert.That(_queue.IsEmpty, Is.True);
        }

        #endregion

        #region Reaction Actions Tests

        [Test]
        public async Task ProcessQueue_ActionReturnsReactions_ReactionsExecutedImmediately()
        {
            var executionOrder = new List<int>();
            var reaction = new OrderTrackingAction(2, executionOrder);
            var mainAction = new ActionWithReaction(1, executionOrder, reaction);
            var afterAction = new OrderTrackingAction(3, executionOrder);

            _queue.Enqueue(mainAction);
            _queue.Enqueue(afterAction);

            await _queue.ProcessQueue(_field, _view);

            // Reaction (2) should execute before afterAction (3)
            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public async Task ProcessQueue_MultipleReactions_AllExecuted()
        {
            var executionOrder = new List<int>();
            var reactions = new List<BattleAction>
            {
                new OrderTrackingAction(2, executionOrder),
                new OrderTrackingAction(3, executionOrder)
            };
            var mainAction = new ActionWithMultipleReactions(1, executionOrder, reactions);

            _queue.Enqueue(mainAction);

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public async Task ProcessQueue_NestedReactions_ExecutedInCorrectOrder()
        {
            var executionOrder = new List<int>();

            // Action 3 is a reaction to Action 2, which is a reaction to Action 1
            var action3 = new OrderTrackingAction(3, executionOrder);
            var action2 = new ActionWithReaction(2, executionOrder, action3);
            var action1 = new ActionWithReaction(1, executionOrder, action2);

            _queue.Enqueue(action1);

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public async Task InsertAtFront_AddsBeforeExisting()
        {
            var executionOrder = new List<int>();
            _queue.Enqueue(new OrderTrackingAction(2, executionOrder));

            _queue.InsertAtFront(new[] { new OrderTrackingAction(1, executionOrder) });

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public async Task InsertAtFront_MultipleActions_MaintainsOrder()
        {
            var executionOrder = new List<int>();
            _queue.Enqueue(new OrderTrackingAction(3, executionOrder));

            _queue.InsertAtFront(new BattleAction[]
            {
                new OrderTrackingAction(1, executionOrder),
                new OrderTrackingAction(2, executionOrder)
            });

            await _queue.ProcessQueue(_field, _view);

            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        #endregion

        #region Safety Tests

        [Test]
        public void ProcessQueue_InfiniteLoop_ThrowsAfterLimit()
        {
            var infiniteAction = new InfiniteReactionAction();
            _queue.Enqueue(infiniteAction);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _queue.ProcessQueue(_field, _view));
        }

        [Test]
        public async Task ProcessQueue_ManyActionsUnderLimit_Succeeds()
        {
            // Add 500 actions - should succeed
            for (int i = 0; i < 500; i++)
            {
                _queue.Enqueue(new MessageAction($"Action {i}"));
            }

            await _queue.ProcessQueue(_field, _view);

            Assert.That(_queue.IsEmpty, Is.True);
        }

        #endregion

        #region Helper Classes

        private class ExecutionTracker
        {
            public bool LogicExecuted { get; set; }
            public bool VisualExecuted { get; set; }
            public bool LogicExecutedFirst { get; set; }
            private bool _logicTime;

            public void MarkLogic()
            {
                LogicExecuted = true;
                _logicTime = true;
            }

            public void MarkVisual()
            {
                LogicExecutedFirst = _logicTime && !VisualExecuted;
                VisualExecuted = true;
            }
        }

        private class TrackingAction : BattleAction
        {
            private readonly ExecutionTracker _tracker;

            public TrackingAction(ExecutionTracker tracker) : base(null)
            {
                _tracker = tracker;
            }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                _tracker.MarkLogic();
                return Enumerable.Empty<BattleAction>();
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                _tracker.MarkVisual();
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

        private class ActionWithMultipleReactions : BattleAction
        {
            private readonly int _order;
            private readonly List<int> _executionOrder;
            private readonly IEnumerable<BattleAction> _reactions;

            public ActionWithMultipleReactions(int order, List<int> executionOrder, IEnumerable<BattleAction> reactions) : base(null)
            {
                _order = order;
                _executionOrder = executionOrder;
                _reactions = reactions;
            }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                _executionOrder.Add(_order);
                return _reactions;
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                return Task.CompletedTask;
            }
        }

        private class InfiniteReactionAction : BattleAction
        {
            public InfiniteReactionAction() : base(null) { }

            public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
            {
                // Always returns itself as a reaction - infinite loop!
                return new[] { new InfiniteReactionAction() };
            }

            public override Task ExecuteVisual(IBattleView view)
            {
                return Task.CompletedTask;
            }
        }

        #endregion
    }
}

