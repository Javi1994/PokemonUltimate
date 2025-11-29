namespace PokemonUltimate.Core.Evolution
{
    /// <summary>
    /// Base interface for all evolution conditions.
    /// Follows the Composition Pattern - evolutions are composed of multiple conditions.
    /// </summary>
    public interface IEvolutionCondition
    {
        /// <summary>
        /// The type of this condition.
        /// </summary>
        EvolutionConditionType ConditionType { get; }

        /// <summary>
        /// Human-readable description of this condition.
        /// </summary>
        string Description { get; }

        // Future: bool IsMet(PokemonInstance pokemon, EvolutionContext context);
    }
}

