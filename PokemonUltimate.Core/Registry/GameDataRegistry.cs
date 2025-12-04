using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Generic storage for game data, accessible by ID.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.17: Registry System
    /// **Documentation**: See `docs/features/1-game-data/1.17-registry-system/README.md`
    /// </remarks>
    public class GameDataRegistry<T> : IDataRegistry<T> where T : class, IIdentifiable
    {
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();

        /// <summary>
        /// Register a single item.
        /// </summary>
        public void Register(T item)
        {
            _items[item.Id] = item;
        }

        /// <summary>
        /// Register multiple items at once.
        /// </summary>
        public void RegisterAll(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Register(item);
            }
        }

        /// <summary>
        /// Get an item by ID. Throws if not found.
        /// </summary>
        public T Get(string id)
        {
            if (_items.TryGetValue(id, out var item))
            {
                return item;
            }
            throw new KeyNotFoundException(ErrorMessages.Format(ErrorMessages.ItemNotFound, id));
        }

        /// <summary>
        /// Get an item by ID. Returns null if not found.
        /// </summary>
        public T GetById(string id)
        {
            _items.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Try to get an item by ID.
        /// </summary>
        public bool TryGet(string id, out T item)
        {
            return _items.TryGetValue(id, out item);
        }

        /// <summary>
        /// Get all registered items.
        /// </summary>
        public IEnumerable<T> GetAll()
        {
            return _items.Values;
        }

        /// <summary>
        /// Get all registered items (alias for GetAll).
        /// </summary>
        public IEnumerable<T> All => _items.Values;

        /// <summary>
        /// Check if an item with the given ID exists.
        /// </summary>
        public bool Exists(string id)
        {
            return _items.ContainsKey(id);
        }

        /// <summary>
        /// Check if an item with the given ID exists (alias for Exists).
        /// </summary>
        public bool Contains(string id)
        {
            return _items.ContainsKey(id);
        }

        /// <summary>
        /// Get the number of registered items.
        /// </summary>
        public int Count => _items.Count;
    }
}
