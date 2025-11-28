using System.Collections.Generic;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    // Specialized registry for Pokemon that supports lookup by Name or Pokedex Number.
    public class PokemonRegistry : GameDataRegistry<PokemonSpeciesData>, IPokemonRegistry
    {
        private readonly Dictionary<int, PokemonSpeciesData> _byPokedexNumber = new Dictionary<int, PokemonSpeciesData>();

        public new void Register(PokemonSpeciesData item)
        {
            base.Register(item);
            if (item.PokedexNumber > 0)
            {
                _byPokedexNumber[item.PokedexNumber] = item;
            }
        }

        // Retrieve by Name (delegates to base Get since Name is the ID)
        public PokemonSpeciesData GetByName(string name)
        {
            return Get(name);
        }

        // Retrieve by Pokedex Number
        public PokemonSpeciesData GetByPokedexNumber(int number)
        {
            if (_byPokedexNumber.TryGetValue(number, out var item))
            {
                return item;
            }
            throw new KeyNotFoundException($"Pokemon with Pokedex Number {number} not found.");
        }

        // Check if exists by Pokedex Number
        public bool ExistsByPokedexNumber(int number)
        {
            return _byPokedexNumber.ContainsKey(number);
        }
    }
}
