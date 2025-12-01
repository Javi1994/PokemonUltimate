using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Content.Catalogs.Abilities
{
    /// <summary>
    /// Central catalog of all Pokemon abilities.
    /// </summary>
    public static partial class AbilityCatalog
    {
        private static readonly List<AbilityData> _all = new List<AbilityData>();

        /// <summary>
        /// Gets all registered abilities.
        /// </summary>
        public static IReadOnlyList<AbilityData> All => _all;

        /// <summary>
        /// Gets an ability by name.
        /// </summary>
        public static AbilityData GetByName(string name)
        {
            return _all.Find(a => a.Name == name);
        }

        /// <summary>
        /// Gets an ability by ID.
        /// </summary>
        public static AbilityData GetById(string id)
        {
            return _all.Find(a => a.Id == id);
        }

        static AbilityCatalog()
        {
            RegisterGen3Abilities();
            RegisterAdditionalAbilities();
        }

        static partial void RegisterGen3Abilities();
        static partial void RegisterAdditionalAbilities();
    }
}

