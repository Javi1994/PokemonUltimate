using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Strategies.Move.Definition;

namespace PokemonUltimate.Core.Strategies.Move
{
    /// <summary>
    /// Move selector that uses different strategies to select moves from a Pokemon's learnset.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.12: Factories & Calculators
    /// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
    /// </remarks>
    public class MoveSelector : IMoveSelector
    {
        private readonly IMoveSelectionStrategy _strategy;

        /// <summary>
        /// Creates a move selector with a specific strategy.
        /// </summary>
        public MoveSelector(IMoveSelectionStrategy strategy)
        {
            _strategy = strategy ?? throw new System.ArgumentNullException(nameof(strategy));
        }

        /// <summary>
        /// Creates a move selector with the default strategy (highest level first).
        /// </summary>
        public static MoveSelector CreateDefault()
        {
            return new MoveSelector(new DefaultMoveStrategy());
        }

        /// <summary>
        /// Creates a move selector with random selection strategy.
        /// </summary>
        public static MoveSelector CreateRandom(IRandomProvider randomProvider)
        {
            return new MoveSelector(new RandomMoveStrategy(randomProvider));
        }

        /// <summary>
        /// Creates a move selector with STAB priority strategy.
        /// </summary>
        public static MoveSelector CreateStab(PokemonType primaryType, PokemonType? secondaryType = null)
        {
            return new MoveSelector(new StabMoveStrategy(primaryType, secondaryType));
        }

        /// <summary>
        /// Creates a move selector with power priority strategy.
        /// </summary>
        public static MoveSelector CreatePower()
        {
            return new MoveSelector(new PowerMoveStrategy());
        }

        /// <summary>
        /// Creates a move selector with optimal selection strategy.
        /// </summary>
        public static MoveSelector CreateOptimal(PokemonType primaryType, PokemonType? secondaryType, IRandomProvider randomProvider)
        {
            return new MoveSelector(new OptimalMoveStrategy(primaryType, secondaryType, randomProvider));
        }

        public IEnumerable<LearnableMove> SelectMoves(IReadOnlyList<LearnableMove> availableMoves, int count)
        {
            if (availableMoves == null || availableMoves.Count == 0)
                return new List<LearnableMove>();

            // If count is 0 or negative, return empty list
            if (count <= 0)
                return new List<LearnableMove>();

            // Clamp count to valid range: max 4 moves, but not more than available
            int maxCount = System.Math.Min(4, availableMoves.Count);
            if (count > maxCount)
                count = maxCount;
            else if (count < 1)
                count = 1;

            return _strategy.SelectMoves(availableMoves, count);
        }
    }
}
