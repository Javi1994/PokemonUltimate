using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Specialized registry for Pokemon that supports lookup by Name, Pokedex Number, and Type.
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

        /// <summary>
        /// Get all Pokemon of a specific type (primary or secondary).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetByType(PokemonType type)
        {
            return GetAll().Where(p => p.HasType(type));
        }

        /// <summary>
        /// Get all Pokemon within a Pokedex number range (inclusive).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetByPokedexRange(int start, int end)
        {
            if (start > end)
                return Enumerable.Empty<PokemonSpeciesData>();
                
            return GetAll().Where(p => p.PokedexNumber >= start && p.PokedexNumber <= end);
        }

        /// <summary>
        /// Get all dual-type Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetDualType()
        {
            return GetAll().Where(p => p.IsDualType);
        }

        /// <summary>
        /// Get all single-type Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetMonoType()
        {
            return GetAll().Where(p => !p.IsDualType);
        }

        /// <summary>
        /// Get all Pokemon that can evolve.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetEvolvable()
        {
            return GetAll().Where(p => p.CanEvolve);
        }

        /// <summary>
        /// Get all final evolution forms (cannot evolve further).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetFinalForms()
        {
            return GetAll().Where(p => !p.CanEvolve);
        }
    }
}
