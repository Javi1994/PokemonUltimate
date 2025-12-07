using System.Collections.Generic;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Infrastructure.Factories
{
    /// <summary>
    /// Interface for creating BattleField instances.
    /// Enables dependency injection and testability.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IBattleFieldFactory
    {
        /// <summary>
        /// Creates a new BattleField and initializes it with the provided rules and parties.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <returns>A new initialized BattleField.</returns>
        BattleField Create(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty);
    }
}
