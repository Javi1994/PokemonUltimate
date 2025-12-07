using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.Core.Domain.Evolution
{
    /// <summary>
    /// Represents a possible evolution for a Pokemon species.
    /// Contains the target species and the conditions required.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Evolution System
    /// **Documentation**: See `docs/features/1-game-data/1.11-evolution-system/README.md`
    /// </remarks>
    public class Evolution
    {
        /// <summary>
        /// The Pokemon species this evolution leads to.
        /// </summary>
        public PokemonSpeciesData Target { get; set; }

        /// <summary>
        /// All conditions that must be met for this evolution.
        /// All conditions are AND-ed together.
        /// </summary>
        public List<IEvolutionCondition> Conditions { get; set; } = new List<IEvolutionCondition>();

        /// <summary>
        /// Human-readable description of all conditions.
        /// </summary>
        public string Description => string.Join(" + ", Conditions.Select(c => c.Description));

        /// <summary>
        /// Check if this evolution has a specific condition type.
        /// </summary>
        public bool HasCondition<T>() where T : IEvolutionCondition
        {
            return Conditions.Any(c => c is T);
        }

        /// <summary>
        /// Get a specific condition type (or null if not found).
        /// </summary>
        public T GetCondition<T>() where T : class, IEvolutionCondition
        {
            return Conditions.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Checks if a specific Pokemon instance meets all conditions for this evolution.
        /// </summary>
        public bool CanEvolve(PokemonInstance pokemon)
        {
            if (pokemon == null || Conditions.Count == 0)
                return false;

            return Conditions.All(c => c.IsMet(pokemon));
        }
    }
}

