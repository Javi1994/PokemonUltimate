using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    /// <summary>
    /// Blueprint for a Pokemon species (immutable data).
    /// Pokemon can be retrieved by Name (unique string) or PokedexNumber (unique int).
    /// This is the "Species" data - shared by all Pokemon of the same kind.
    /// </summary>
    public class PokemonSpeciesData : IIdentifiable
    {
        /// <summary>
        /// Unique identifier - the Pokemon's name (e.g., "Pikachu", "Charizard").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// National Pokedex number (e.g., 25 for Pikachu, 6 for Charizard).
        /// </summary>
        public int PokedexNumber { get; set; }

        /// <summary>
        /// Primary type of the Pokemon (every Pokemon has one).
        /// </summary>
        public PokemonType PrimaryType { get; set; }

        /// <summary>
        /// Secondary type of the Pokemon (optional, null if mono-type).
        /// </summary>
        public PokemonType? SecondaryType { get; set; }

        /// <summary>
        /// Base stats used for calculating actual stats.
        /// </summary>
        public BaseStats BaseStats { get; set; } = new BaseStats();

        /// <summary>
        /// Moves this Pokemon can learn.
        /// </summary>
        public List<LearnableMove> Learnset { get; set; } = new List<LearnableMove>();

        /// <summary>
        /// Possible evolutions for this Pokemon.
        /// </summary>
        public List<Evolution.Evolution> Evolutions { get; set; } = new List<Evolution.Evolution>();

        /// <summary>
        /// IIdentifiable implementation - Name serves as the unique ID.
        /// </summary>
        public string Id => Name;

        /// <summary>
        /// Returns true if this Pokemon has two types.
        /// </summary>
        public bool IsDualType => SecondaryType.HasValue;

        /// <summary>
        /// Returns true if this Pokemon can evolve.
        /// </summary>
        public bool CanEvolve => Evolutions.Count > 0;

        /// <summary>
        /// Checks if this Pokemon has a specific type (primary or secondary).
        /// </summary>
        public bool HasType(PokemonType type)
        {
            return PrimaryType == type || SecondaryType == type;
        }

        #region Learnset Helpers

        /// <summary>
        /// Get moves available at level 1 (starting moves).
        /// </summary>
        public IEnumerable<LearnableMove> GetStartingMoves()
        {
            return Learnset.Where(m => m.Method == LearnMethod.Start);
        }

        /// <summary>
        /// Get moves learned at a specific level.
        /// </summary>
        public IEnumerable<LearnableMove> GetMovesAtLevel(int level)
        {
            return Learnset.Where(m => m.Method == LearnMethod.LevelUp && m.Level == level);
        }

        /// <summary>
        /// Get all moves learnable up to and including a specific level.
        /// </summary>
        public IEnumerable<LearnableMove> GetMovesUpToLevel(int level)
        {
            return Learnset.Where(m =>
                m.Method == LearnMethod.Start ||
                (m.Method == LearnMethod.LevelUp && m.Level <= level));
        }

        /// <summary>
        /// Check if this Pokemon can learn a specific move (by any method).
        /// </summary>
        public bool CanLearn(MoveData move)
        {
            return Learnset.Any(m => m.Move == move);
        }

        #endregion
    }
}
