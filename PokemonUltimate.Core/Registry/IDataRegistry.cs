using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Contract for any system that stores and retrieves data by ID.
    /// </summary>
    public interface IDataRegistry<T> where T : IIdentifiable
    {
        T Get(string id);
        IEnumerable<T> GetAll();
        bool Exists(string id);
        void Register(T item);
    }
}

