using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Contract for a registry that handles Move-specific lookups.
    /// Supports lookup by Name (string) and filtering by Type/Category.
    /// </summary>
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

