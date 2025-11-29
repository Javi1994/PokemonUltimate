using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Specialized registry for Moves that supports lookup by Name and filtering by Type/Category.
    /// </summary>
    public class MoveRegistry : GameDataRegistry<MoveData>, IMoveRegistry
    {
        /// <summary>
        /// Retrieve by Name (delegates to base Get since Name is the ID)
        /// </summary>
        public MoveData GetByName(string name)
        {
            return Get(name);
        }

        /// <summary>
        /// Get all moves of a specific type
        /// </summary>
        public IEnumerable<MoveData> GetByType(PokemonType type)
        {
            return GetAll().Where(m => m.Type == type);
        }

        /// <summary>
        /// Get all moves of a specific category
        /// </summary>
        public IEnumerable<MoveData> GetByCategory(MoveCategory category)
        {
            return GetAll().Where(m => m.Category == category);
        }
    }
}

