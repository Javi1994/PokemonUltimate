

namespace PokemonUltimate.Combat.Factories
{
    /// <summary>
    /// Interface for creating BattleQueue instances.
    /// Enables dependency injection and testability.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IBattleQueueFactory
    {
        /// <summary>
        /// Creates a new BattleQueue instance.
        /// </summary>
        /// <returns>A new BattleQueue.</returns>
        BattleQueue Create();
    }
}
