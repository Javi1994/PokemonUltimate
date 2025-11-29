using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Evolution
{
    /// <summary>
    /// Represents a possible evolution for a Pokemon species.
    /// Contains the target species and the conditions required.
    /// </summary>
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
    }
}

