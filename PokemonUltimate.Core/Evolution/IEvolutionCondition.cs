using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Evolution
{
    /// <summary>
    /// Base interface for all evolution conditions.
    /// Follows the Composition Pattern - evolutions are composed of multiple conditions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Evolution System
    /// **Documentation**: See `docs/features/1-game-data/1.11-evolution-system/README.md`
    /// </remarks>
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

        /// <summary>
        /// Checks if this condition is met by the given Pokemon instance.
        /// </summary>
        bool IsMet(PokemonInstance pokemon);
    }
}

