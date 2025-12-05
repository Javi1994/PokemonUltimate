using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Core.Factories.MoveSelection.Strategies
{
    /// <summary>
    /// Strategy interface for move selection algorithms.
    /// </summary>
    public interface IMoveSelectionStrategy
    {
        /// <summary>
        /// Selects moves from available moves using the strategy's algorithm.
        /// </summary>
        IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count);
    }
}
