using System.Collections.Generic;

namespace PokemonUltimate.Core.Interfaces
{
    // Contract for any system that stores and retrieves data by ID.
    public interface IDataRegistry<T> where T : IIdentifiable
    {
        T Get(string id);
        IEnumerable<T> GetAll();
        bool Exists(string id);
        void Register(T item);
    }
}
