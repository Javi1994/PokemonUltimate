using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Strategies.Move.Definition;

namespace PokemonUltimate.Core.Strategies.Move
{
    /// <summary>
    /// Strategy that prioritizes moves with highest base power.
    /// </summary>
    public class PowerMoveStrategy : IMoveSelectionStrategy
    {
        public IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count)
        {
            return availableMoves
                .OrderByDescending(m => m.Move.Power)
                .ThenByDescending(m => m.Move.Accuracy)
                .ThenBy(m => m.Move.Name)
                .Take(count);
        }
    }
}
