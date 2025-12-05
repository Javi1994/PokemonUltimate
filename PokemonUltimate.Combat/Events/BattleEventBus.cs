using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;

namespace PokemonUltimate.Combat.Events
{
    /// <summary>
    /// Default implementation of IBattleEventBus.
    /// Manages subscriptions and publishes events to all subscribed listeners.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    public class BattleEventBus : IBattleEventBus
    {
        private readonly Dictionary<BattleTrigger, List<IBattleListener>> _subscriptions;

        /// <summary>
        /// Creates a new battle event bus.
        /// </summary>
        public BattleEventBus()
        {
            _subscriptions = new Dictionary<BattleTrigger, List<IBattleListener>>();
        }

        /// <summary>
        /// Subscribes a listener to a specific battle trigger.
        /// </summary>
        public void Subscribe(BattleTrigger trigger, IBattleListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            if (!_subscriptions.TryGetValue(trigger, out var listeners))
            {
                listeners = new List<IBattleListener>();
                _subscriptions[trigger] = listeners;
            }

            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        /// <summary>
        /// Unsubscribes a listener from a specific battle trigger.
        /// </summary>
        public void Unsubscribe(BattleTrigger trigger, IBattleListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            if (_subscriptions.TryGetValue(trigger, out var listeners))
            {
                listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Unsubscribes a listener from all triggers.
        /// </summary>
        public void UnsubscribeAll(IBattleListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            foreach (var listeners in _subscriptions.Values)
            {
                listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Publishes a battle trigger event and returns actions from all subscribed listeners.
        /// </summary>
        public IEnumerable<BattleAction> Publish(BattleTrigger trigger, BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (!_subscriptions.TryGetValue(trigger, out var listeners))
                return Enumerable.Empty<BattleAction>();

            var allActions = new List<BattleAction>();

            foreach (var listener in listeners.ToList()) // ToList to avoid modification during iteration
            {
                try
                {
                    // Get the slot that owns this listener (if applicable)
                    // For now, we'll need to pass the slot through the listener or use a different approach
                    // This is a simplified version - in a full implementation, we'd need to track listener-slot associations
                    var listenerActions = listener.OnTrigger(trigger, null, field);
                    allActions.AddRange(listenerActions);
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other listeners
                    // In production, this would use a logger
                    System.Diagnostics.Debug.WriteLine($"Error processing listener for trigger {trigger}: {ex.Message}");
                }
            }

            return allActions;
        }
    }
}
