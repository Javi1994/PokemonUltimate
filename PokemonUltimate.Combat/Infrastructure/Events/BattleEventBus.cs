using System.Collections.Generic;
using System.Linq;

namespace PokemonUltimate.Combat.Infrastructure.Events
{
    /// <summary>
    /// Unified event bus for battle events (statistics/logging).
    /// Processors handle abilities/items directly, so this bus only handles event publishing for observers.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    public class BattleEventBus : IBattleEventBus
    {
        // BattleEvent subscriptions (for statistics/logging)
        private readonly List<IBattleEventSubscriber> _eventSubscribers = new List<IBattleEventSubscriber>();

        // ===== BattleEvent Methods (for statistics/logging) =====

        /// <summary>
        /// Subscribes a component to receive battle events (statistics/logging).
        /// </summary>
        /// <param name="subscriber">The subscriber to add.</param>
        public void Subscribe(IBattleEventSubscriber subscriber)
        {
            if (subscriber != null && !_eventSubscribers.Contains(subscriber))
            {
                _eventSubscribers.Add(subscriber);
            }
        }

        /// <summary>
        /// Unsubscribes a component from receiving battle events.
        /// </summary>
        /// <param name="subscriber">The subscriber to remove.</param>
        public void Unsubscribe(IBattleEventSubscriber subscriber)
        {
            if (subscriber != null)
            {
                _eventSubscribers.Remove(subscriber);
            }
        }

        /// <summary>
        /// Publishes a battle event to all subscribers.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        public void PublishEvent(BattleEvent @event)
        {
            if (@event == null)
                return;

            // Create a copy of subscribers list to avoid modification during iteration
            var subscribersCopy = _eventSubscribers.ToList();

            foreach (var subscriber in subscribersCopy)
            {
                try
                {
                    subscriber.OnBattleEvent(@event);
                }
                catch
                {
                    // Ignore errors from subscribers to prevent one bad subscriber from breaking the system
                }
            }
        }
    }
}
