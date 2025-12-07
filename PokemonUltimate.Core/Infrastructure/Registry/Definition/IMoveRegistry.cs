using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Infrastructure.Registry.Definition
{
    /// <summary>
    /// Contract for a registry that handles Move-specific lookups.
    /// Supports lookup by Name (string) and filtering by Type/Category.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.17: Registry System
    /// **Documentation**: See `docs/features/1-game-data/1.17-registry-system/README.md`
    /// </remarks>
    public interface IMoveRegistry : IDataRegistry<MoveData>
    {
        /// <summary>
        /// Retrieve move by its unique name (e.g., "Thunderbolt")
        /// </summary>
        MoveData GetByName(string name);

        /// <summary>
        /// Get all moves of a specific type (e.g., all Fire moves)
        /// </summary>
        IEnumerable<MoveData> GetByType(PokemonType type);

        /// <summary>
        /// Get all moves of a specific category (Physical, Special, Status)
        /// </summary>
        IEnumerable<MoveData> GetByCategory(MoveCategory category);
    }
}

