using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Immutable data structure containing learnset information for a Pokemon species.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    public class LearnsetData
    {
        /// <summary>
        /// List of moves this Pokemon can learn.
        /// </summary>
        public List<LearnableMove> Moves { get; }

        /// <summary>
        /// Initializes a new instance of LearnsetData.
        /// </summary>
        /// <param name="moves">The list of learnable moves.</param>
        public LearnsetData(List<LearnableMove> moves)
        {
            Moves = moves ?? new List<LearnableMove>();
        }
    }
}
