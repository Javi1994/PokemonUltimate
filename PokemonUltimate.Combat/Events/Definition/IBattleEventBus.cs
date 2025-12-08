namespace PokemonUltimate.Combat.Events.Definition
{
    /// <summary>
    /// Interface for subscribing to battle events (statistics/logging).
    /// </summary>
    public interface IBattleEventSubscriber
    {
        /// <summary>
        /// Called when a battle event is published.
        /// </summary>
        /// <param name="event">The event that occurred.</param>
        void OnBattleEvent(BattleEvent @event);
    }

    /// <summary>
    /// Interface for battle event bus that manages battle events for statistics/logging.
    /// Processors handle abilities/items directly, so this bus only handles event publishing for observers.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.20: Statistics System
    /// **Documentation**: See `docs/features/2-combat-system/2.20-statistics-system/architecture.md`
    /// </remarks>
    public interface IBattleEventBus
    {
        // ===== BattleEvent Events (for statistics/logging) =====

        /// <summary>
        /// Publishes a battle event (for statistics/logging).
        /// </summary>
        /// <param name="event">The event to publish.</param>
        void PublishEvent(BattleEvent @event);

        /// <summary>
        /// Subscribes a component to receive battle events (statistics/logging).
        /// </summary>
        /// <param name="subscriber">The subscriber to add.</param>
        void Subscribe(IBattleEventSubscriber subscriber);

        /// <summary>
        /// Unsubscribes a component from receiving battle events.
        /// </summary>
        /// <param name="subscriber">The subscriber to remove.</param>
        void Unsubscribe(IBattleEventSubscriber subscriber);
    }
}
