using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Core.Combat.Actions;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Core.Combat
{
    /// <summary>
    /// Manages the sequential processing of battle actions.
    /// Actions are processed in order: Logic (instant) -> Visual (async) -> Reactions (inserted at front).
    /// </summary>
    public class BattleQueue
    {
        private const int MaxIterations = 1000;
        
        private readonly LinkedList<BattleAction> _queue;

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
                if (iterationCount++ > MaxIterations)
                {
                    throw new InvalidOperationException(ErrorMessages.BattleQueueInfiniteLoop);
                }

                // Dequeue the first action
                var action = _queue.First.Value;
                _queue.RemoveFirst();

                // Phase 1: Execute Logic (instant)
                var reactions = action.ExecuteLogic(field);

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

