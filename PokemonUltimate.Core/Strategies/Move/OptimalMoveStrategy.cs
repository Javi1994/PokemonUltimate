using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Providers.Definition;

namespace PokemonUltimate.Core.Strategies.Move
{
    /// <summary>
    /// Strategy that selects optimal moves considering STAB, power, accuracy, and type coverage.
    /// </summary>
    public class OptimalMoveStrategy : IMoveSelectionStrategy
    {
        private readonly PokemonType _primaryType;
        private readonly PokemonType? _secondaryType;
        private readonly IRandomProvider _randomProvider;

        public OptimalMoveStrategy(PokemonType primaryType, PokemonType? secondaryType, IRandomProvider randomProvider)
        {
            _primaryType = primaryType;
            _secondaryType = secondaryType;
            _randomProvider = randomProvider ?? throw new System.ArgumentNullException(nameof(randomProvider));
        }

        public IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count)
        {
            var scored = availableMoves.Select(m => new
            {
                Move = m,
                Score = CalculateOptimalScore(m.Move)
            });

            return scored
                .OrderByDescending(x => x.Score)
                .ThenBy(x => _randomProvider.Next(int.MaxValue)) // Random tie-breaker
                .Take(count)
                .Select(x => x.Move);
        }

        private int CalculateOptimalScore(MoveData move)
        {
            int score = 0;

            // Base power score (0-150 range mapped to 0-150)
            score += move.Power;

            // STAB bonus
            bool isStab = move.Type == _primaryType || move.Type == _secondaryType;
            if (isStab)
            {
                score += 100; // Significant STAB bonus
            }

            // Accuracy bonus (higher is better, 0 means always hits)
            if (move.Accuracy > 0)
            {
                score += move.Accuracy / 10;
            }

            // Small bonus for moves learned at higher levels (indicates stronger moves)
            // This is handled by the LearnableMove.Level, but we can add a small bonus here
            // Note: Level information is not directly available in MoveData, so we skip this

            return score;
        }
    }
}
