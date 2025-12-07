using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Core.Strategies.Move
{
    /// <summary>
    /// Interface for selecting moves from a Pokemon's learnset.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public interface IMoveSelector
    {
        /// <summary>
        /// Selects moves from the available learnset moves.
        /// </summary>
        /// <param name="availableMoves">List of moves available at the Pokemon's level</param>
        /// <param name="count">Number of moves to select (1-4)</param>
        /// <returns>Selected moves</returns>
        IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count);
    }
}
