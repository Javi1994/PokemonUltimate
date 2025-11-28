using System.Collections.Generic;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    // Generic storage for game data, accessible by ID.
    public class GameDataRegistry<T> : IDataRegistry<T> where T : class, IIdentifiable
    {
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();

        public void Register(T item)
        {
            _items[item.Id] = item;
        }

        public T Get(string id)
        {
            if (_items.TryGetValue(id, out var item))
            {
                return item;
            }
            throw new KeyNotFoundException($"Item with ID {id} not found.");
        }

        public IEnumerable<T> GetAll()
        {
            return _items.Values;
        }

        public bool Exists(string id)
        {
            return _items.ContainsKey(id);
        }
    }
}
