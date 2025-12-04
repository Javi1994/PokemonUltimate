using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Contract for any system that stores and retrieves data by ID.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.17: Registry System
    /// **Documentation**: See `docs/features/1-game-data/1.17-registry-system/README.md`
    /// </remarks>
    public interface IDataRegistry<T> where T : IIdentifiable
    {
        /// <summary>
        /// Get an item by ID. Throws if not found.
        /// </summary>
        T Get(string id);

        /// <summary>
        /// Get an item by ID. Returns null if not found.
        /// </summary>
        T GetById(string id);

        /// <summary>
        /// Try to get an item by ID.
        /// </summary>
        bool TryGet(string id, out T item);

        /// <summary>
        /// Get all registered items.
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get all registered items (property alias).
        /// </summary>
        IEnumerable<T> All { get; }

        /// <summary>
        /// Check if an item with the given ID exists.
        /// </summary>
        bool Exists(string id);

        /// <summary>
        /// Check if an item with the given ID exists (alias for Exists).
        /// </summary>
        bool Contains(string id);

        /// <summary>
        /// Get the number of registered items.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Register a single item.
        /// </summary>
        void Register(T item);

        /// <summary>
        /// Register multiple items at once.
        /// </summary>
        void RegisterAll(IEnumerable<T> items);
    }
}
