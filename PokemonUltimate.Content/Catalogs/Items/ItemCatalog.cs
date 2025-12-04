using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Content.Catalogs.Items
{
    /// <summary>
    /// Central catalog of all items.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.3: Item Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.3-item-expansion/README.md`
    /// </remarks>
    public static partial class ItemCatalog
    {
        private static readonly List<ItemData> _all = new List<ItemData>();

        /// <summary>
        /// Gets all registered items.
        /// </summary>
        public static IReadOnlyList<ItemData> All => _all;

        /// <summary>
        /// Gets an item by name.
        /// </summary>
        public static ItemData GetByName(string name)
        {
            return _all.Find(i => i.Name == name);
        }

        /// <summary>
        /// Gets an item by ID.
        /// </summary>
        public static ItemData GetById(string id)
        {
            return _all.Find(i => i.Id == id);
        }

        static ItemCatalog()
        {
            RegisterHeldItems();
            RegisterBerries();
        }

        static partial void RegisterHeldItems();
        static partial void RegisterBerries();
    }
}

