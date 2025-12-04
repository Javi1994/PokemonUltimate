using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Blueprint for a Pokemon species (immutable data).
    /// Pokemon can be retrieved by Name (unique string) or PokedexNumber (unique int).
    /// This is the "Species" data - shared by all Pokemon of the same kind.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/README.md`
    /// </remarks>
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
        /// Gender ratio: percentage chance of being male (0-100).
        /// -1 means genderless (Magnemite, Ditto, etc.).
        /// Common values: 50 (equal), 87.5 (starters), 0 (female only), 100 (male only).
        /// </summary>
        public float GenderRatio { get; set; } = 50f;

        #region Abilities

        /// <summary>
        /// Primary ability (most Pokemon have this).
        /// </summary>
        public AbilityData Ability1 { get; set; }

        /// <summary>
        /// Secondary ability (optional, some Pokemon don't have one).
        /// </summary>
        public AbilityData Ability2 { get; set; }

        /// <summary>
        /// Hidden ability (rare, obtained through special means).
        /// </summary>
        public AbilityData HiddenAbility { get; set; }

        #endregion

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
        /// Returns true if this Pokemon is genderless.
        /// </summary>
        public bool IsGenderless => GenderRatio < 0;

        /// <summary>
        /// Returns true if this Pokemon can only be male.
        /// </summary>
        public bool IsMaleOnly => GenderRatio >= 100;

        /// <summary>
        /// Returns true if this Pokemon can only be female.
        /// </summary>
        public bool IsFemaleOnly => GenderRatio == 0;

        /// <summary>
        /// Returns true if this Pokemon can be either gender.
        /// </summary>
        public bool HasBothGenders => GenderRatio > 0 && GenderRatio < 100;

        /// <summary>
        /// Returns true if this Pokemon has a secondary ability.
        /// </summary>
        public bool HasSecondaryAbility => Ability2 != null;

        /// <summary>
        /// Returns true if this Pokemon has a hidden ability.
        /// </summary>
        public bool HasHiddenAbility => HiddenAbility != null;

        /// <summary>
        /// Gets all possible abilities for this Pokemon.
        /// </summary>
        public IEnumerable<AbilityData> GetAllAbilities()
        {
            if (Ability1 != null) yield return Ability1;
            if (Ability2 != null) yield return Ability2;
            if (HiddenAbility != null) yield return HiddenAbility;
        }

        /// <summary>
        /// Gets a random non-hidden ability for this Pokemon.
        /// </summary>
        public AbilityData GetRandomAbility(Random random = null)
        {
            random = random ?? new Random();
            if (Ability2 != null && random.Next(2) == 1)
                return Ability2;
            return Ability1;
        }

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
            if (move == null)
                throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);
            
            return Learnset.Any(m => m.Move == move);
        }

        #endregion
    }
}

