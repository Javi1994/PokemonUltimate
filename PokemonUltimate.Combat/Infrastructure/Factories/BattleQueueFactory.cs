using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Infrastructure.Factories.Definition;

namespace PokemonUltimate.Combat.Infrastructure.Factories
{
    /// <summary>
    /// Factory for creating BattleQueue instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleQueueFactory : IBattleQueueFactory
    {
        /// <summary>
        /// Creates a new BattleQueue instance.
        /// </summary>
        /// <returns>A new BattleQueue.</returns>
        public BattleQueueService Create()
        {
            return new BattleQueueService();
        }
    }
}
