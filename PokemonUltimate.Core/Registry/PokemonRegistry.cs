using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Specialized registry for Pokemon that supports lookup by Name, Pokedex Number, and Type.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.17: Registry System
    /// **Documentation**: See `docs/features/1-game-data/1.17-registry-system/README.md`
    /// </remarks>
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
            throw new KeyNotFoundException(ErrorMessages.Format(ErrorMessages.PokemonNotFoundByPokedex, number));
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

        /// <summary>
        /// Get all variant forms of the specified base Pokemon.
        /// </summary>
        /// <remarks>
        /// **Feature**: 1: Game Data
        /// **Sub-Feature**: 1.18: Variants System
        /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
        /// </remarks>
        public IEnumerable<PokemonSpeciesData> GetVariantsOf(PokemonSpeciesData baseForm)
        {
            if (baseForm == null)
                throw new ArgumentNullException(nameof(baseForm), ErrorMessages.PokemonCannotBeNull);

            return GetAll().Where(p => p.BaseForm == baseForm);
        }

        /// <summary>
        /// Get all Mega Evolution variants.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetMegaVariants()
        {
            return GetAll().Where(p => p.IsMegaVariant);
        }

        /// <summary>
        /// Get all Dinamax variants.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetDinamaxVariants()
        {
            return GetAll().Where(p => p.IsDinamaxVariant);
        }

        /// <summary>
        /// Get all Terracristalización variants.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetTeraVariants()
        {
            return GetAll().Where(p => p.IsTeraVariant);
        }

        /// <summary>
        /// Get all base forms (non-variant Pokemon).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetBaseForms()
        {
            return GetAll().Where(p => p.IsBaseForm);
        }

        /// <summary>
        /// Get all Regional form variants.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetRegionalVariants()
        {
            return GetAll().Where(p => p.IsRegionalVariant);
        }

        /// <summary>
        /// Get all Regional form variants from a specific region.
        /// </summary>
        /// <param name="region">The region identifier (e.g., "Alola", "Galar", "Hisui").</param>
        public IEnumerable<PokemonSpeciesData> GetRegionalVariantsByRegion(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region cannot be null or empty", nameof(region));

            return GetAll().Where(p => p.IsRegionalVariant && 
                                      p.RegionalForm.Equals(region, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all variants that have gameplay changes (stats, types, abilities).
        /// Excludes purely visual variants.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetVariantsWithGameplayChanges()
        {
            return GetAll().Where(p => p.HasGameplayChanges);
        }

        /// <summary>
        /// Get all purely visual variants (no gameplay changes).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> GetVisualOnlyVariants()
        {
            return GetAll().Where(p => p.IsVariant && !p.HasGameplayChanges);
        }
    }
}
