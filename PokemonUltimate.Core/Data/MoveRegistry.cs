using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    // Specialized registry for Moves that supports lookup by Name and filtering by Type/Category.
    public class MoveRegistry : GameDataRegistry<MoveData>, IMoveRegistry
    {
        // Retrieve by Name (delegates to base Get since Name is the ID)
        public MoveData GetByName(string name)
        {
            return Get(name);
        }

        // Get all moves of a specific type
        public IEnumerable<MoveData> GetByType(PokemonType type)
        {
            return GetAll().Where(m => m.Type == type);
        }

        // Get all moves of a specific category
        public IEnumerable<MoveData> GetByCategory(MoveCategory category)
        {
            return GetAll().Where(m => m.Category == category);
        }
    }
}

