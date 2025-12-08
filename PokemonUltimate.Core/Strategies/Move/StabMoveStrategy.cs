using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Strategies.Move.Definition;

namespace PokemonUltimate.Core.Strategies.Move
{
    /// <summary>
    /// Strategy that prioritizes STAB (Same Type Attack Bonus) moves.
    /// </summary>
    public class StabMoveStrategy : IMoveSelectionStrategy
    {
        private readonly PokemonType _primaryType;
        private readonly PokemonType? _secondaryType;

        public StabMoveStrategy(PokemonType primaryType, PokemonType? secondaryType = null)
        {
            _primaryType = primaryType;
            _secondaryType = secondaryType;
        }

        public IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count)
        {
            var scored = availableMoves.Select(m => new
            {
                Move = m,
                Score = CalculateStabScore(m.Move)
            });

            return scored
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Move.Move.Power)
                .ThenBy(x => x.Move.Move.Name)
                .Take(count)
                .Select(x => x.Move);
        }

        private int CalculateStabScore(MoveData move)
        {
            bool isStab = move.Type == _primaryType || move.Type == _secondaryType;
            return isStab ? 100 : 0; // Significant STAB bonus
        }
    }
}
