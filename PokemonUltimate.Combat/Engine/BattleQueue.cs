using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Constants;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Manages the sequential processing of battle actions.
    /// Actions are processed in order: Logic (instant) -> Visual (async) -> Reactions (inserted at front).
    /// Supports observers for statistics collection, logging, and debugging.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.2: Action Queue
    /// **Documentation**: See `docs/features/2-combat-system/2.2-action-queue/architecture.md`
    /// </remarks>
    public class BattleQueue
    {
        private readonly LinkedList<BattleAction> _queue;
        private readonly List<IBattleActionObserver> _observers = new List<IBattleActionObserver>();

        /// <summary>
        /// The number of actions currently in the queue.
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// True if the queue is empty.
        /// </summary>
        public bool IsEmpty => _queue.Count == 0;

        /// <summary>
        /// Creates a new empty battle queue.
        /// </summary>
        public BattleQueue()
        {
            _queue = new LinkedList<BattleAction>();
        }

        /// <summary>
        /// Adds an action to the end of the queue.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <exception cref="ArgumentNullException">If action is null.</exception>
        public void Enqueue(BattleAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _queue.AddLast(action);
        }

        /// <summary>
        /// Adds multiple actions to the end of the queue.
        /// </summary>
        /// <param name="actions">The actions to add.</param>
        /// <exception cref="ArgumentNullException">If actions is null.</exception>
        public void EnqueueRange(IEnumerable<BattleAction> actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            foreach (var action in actions)
            {
                if (action != null)
                {
                    _queue.AddLast(action);
                }
            }
        }

        /// <summary>
        /// Inserts actions at the front of the queue.
        /// Used for reaction actions that should execute immediately.
        /// </summary>
        /// <param name="actions">The actions to insert.</param>
        /// <exception cref="ArgumentNullException">If actions is null.</exception>
        public void InsertAtFront(IEnumerable<BattleAction> actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            // Insert in reverse order to maintain order at front
            var actionList = actions.Where(a => a != null).ToList();
            for (int i = actionList.Count - 1; i >= 0; i--)
            {
                _queue.AddFirst(actionList[i]);
            }
        }

        /// <summary>
        /// Removes all actions from the queue.
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Adds an observer to receive notifications when actions are executed.
        /// </summary>
        /// <param name="observer">The observer to add.</param>
        /// <exception cref="ArgumentNullException">If observer is null.</exception>
        public void AddObserver(IBattleActionObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        /// <summary>
        /// Removes an observer from receiving notifications.
        /// </summary>
        /// <param name="observer">The observer to remove.</param>
        public void RemoveObserver(IBattleActionObserver observer)
        {
            if (observer != null)
            {
                _observers.Remove(observer);
            }
        }

        /// <summary>
        /// Gets all registered observers.
        /// </summary>
        /// <returns>A read-only list of observers.</returns>
        public IReadOnlyList<IBattleActionObserver> GetObservers()
        {
            return _observers.ToList().AsReadOnly();
        }

        /// <summary>
        /// Processes all actions in the queue until empty.
        /// Each action's Logic is executed first, then Visual.
        /// Reaction actions are inserted at the front of the queue.
        /// </summary>
        /// <param name="field">The battlefield for logic execution.</param>
        /// <param name="view">The view for visual execution.</param>
        /// <exception cref="InvalidOperationException">If more than 1000 iterations occur (infinite loop protection).</exception>
        public async Task ProcessQueue(BattleField field, IBattleView view)
        {
            int iterationCount = 0;

            while (_queue.Count > 0)
            {
                if (iterationCount++ > BattleConstants.MaxQueueIterations)
                {
                    throw new InvalidOperationException(ErrorMessages.BattleQueueInfiniteLoop);
                }

                // Dequeue the first action
                var action = _queue.First.Value;
                _queue.RemoveFirst();

                // Phase 1: Execute Logic (instant)
                var reactions = action.ExecuteLogic(field);

                // Notify observers about action execution
                foreach (var observer in _observers)
                {
                    observer.OnActionExecuted(action, field, reactions ?? Enumerable.Empty<BattleAction>());
                }

                // Phase 2: Execute Visual (async)
                await action.ExecuteVisual(view);

                // Phase 3: Insert reactions at front (they execute next)
                if (reactions != null)
                {
                    InsertAtFront(reactions);
                }
            }
        }
    }
}

