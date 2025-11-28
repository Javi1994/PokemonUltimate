using System.Collections.Generic;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Interfaces
{
    // Contract for a registry that handles Move-specific lookups.
    // Supports lookup by Name (string) and filtering by Type/Category.
    public interface IMoveRegistry : IDataRegistry<MoveData>
    {
        // Retrieve move by its unique name (e.g., "Thunderbolt")
        MoveData GetByName(string name);

        // Get all moves of a specific type (e.g., all Fire moves)
        IEnumerable<MoveData> GetByType(PokemonType type);

        // Get all moves of a specific category (Physical, Special, Status)
        IEnumerable<MoveData> GetByCategory(MoveCategory category);
    }
}

