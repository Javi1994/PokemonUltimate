using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Specialized registry for Pokemon that supports lookup by Name or Pokedex Number.
    /// </summary>
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

        /// <summary>
        /// Retrieve by Name (delegates to base Get since Name is the ID)
        /// </summary>
        public PokemonSpeciesData GetByName(string name)
        {
            return Get(name);
        }

        /// <summary>
        /// Retrieve by Pokedex Number
        /// </summary>
        public PokemonSpeciesData GetByPokedexNumber(int number)
        {
            if (_byPokedexNumber.TryGetValue(number, out var item))
            {
                return item;
            }
            throw new KeyNotFoundException($"Pokemon with Pokedex Number {number} not found.");
        }

        /// <summary>
        /// Check if exists by Pokedex Number
        /// </summary>
        public bool ExistsByPokedexNumber(int number)
        {
            return _byPokedexNumber.ContainsKey(number);
        }
    }
}

