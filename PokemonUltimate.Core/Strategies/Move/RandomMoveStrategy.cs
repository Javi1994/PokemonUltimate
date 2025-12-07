using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Providers.Definition;

namespace PokemonUltimate.Core.Strategies.Move
{
    /// <summary>
    /// Strategy that selects moves randomly from available moves.
    /// </summary>
    public class RandomMoveStrategy : IMoveSelectionStrategy
    {
        private readonly IRandomProvider _randomProvider;

        public RandomMoveStrategy(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new System.ArgumentNullException(nameof(randomProvider));
        }

        public IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count)
        {
            return availableMoves
                .OrderBy(x => _randomProvider.Next(int.MaxValue))
                .Take(count);
        }
    }
}
