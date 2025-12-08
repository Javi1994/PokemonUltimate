using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Strategies.Move.Definition;

namespace PokemonUltimate.Core.Strategies.Move
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
