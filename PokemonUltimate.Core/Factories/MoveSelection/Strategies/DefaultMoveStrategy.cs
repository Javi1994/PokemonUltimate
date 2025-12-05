using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Core.Factories.MoveSelection.Strategies
{
    /// <summary>
    /// Default strategy that selects moves by highest level first, then alphabetically.
    /// </summary>
    public class DefaultMoveStrategy : IMoveSelectionStrategy
    {
        public IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count)
        {
            return availableMoves
                .OrderByDescending(m => m.Level)
                .ThenBy(m => m.Move.Name)
                .Take(count);
        }
    }
}
