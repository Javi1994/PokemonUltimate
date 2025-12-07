using System.Collections.Generic;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Provides learnset data for Pokemon species.
    /// Centralized data source for Pokemon learnsets (moves they can learn).
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    ///
    /// **Design**: Follows SRP (Single Responsibility Principle) - only responsible for providing learnset data.
    /// Data is organized by generation for maintainability.
    /// </remarks>
    public static class LearnsetProvider
    {
        private static readonly Dictionary<string, LearnsetData> _learnsets = new Dictionary<string, LearnsetData>(System.StringComparer.OrdinalIgnoreCase);

        static LearnsetProvider()
        {
            InitializeGen1();
            // InitializeGen2(); // Uncomment when Gen2 data is added
        }

        /// <summary>
        /// Gets learnset data for a Pokemon by name.
        /// </summary>
        /// <param name="pokemonName">The name of the Pokemon (case-insensitive).</param>
        /// <returns>Learnset data if found, null otherwise.</returns>
        public static LearnsetData GetLearnset(string pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return null;

            _learnsets.TryGetValue(pokemonName, out var learnset);
            return learnset;
        }

        /// <summary>
        /// Checks if learnset data exists for a Pokemon.
        /// </summary>
        /// <param name="pokemonName">The name of the Pokemon (case-insensitive).</param>
        /// <returns>True if data exists, false otherwise.</returns>
        public static bool HasLearnset(string pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return false;

            return _learnsets.ContainsKey(pokemonName);
        }

        #region Generation Data Initialization

        /// <summary>
        /// Initialize Generation 1 Pokemon learnset data.
        /// Uses only moves that exist in MoveCatalog (36 moves currently available).
        /// </summary>
        private static void InitializeGen1()
        {
            // Grass Starter Line - Using available moves only
            Register("Bulbasaur", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(9, MoveCatalog.VineWhip)
                .AtLevel(27, MoveCatalog.RazorLeaf)
                .AtLevel(65, MoveCatalog.SolarBeam));

            Register("Ivysaur", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(9, MoveCatalog.VineWhip)
                .AtLevel(22, MoveCatalog.RazorLeaf)
                .AtLevel(65, MoveCatalog.SolarBeam));

            Register("Venusaur", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(9, MoveCatalog.VineWhip)
                .AtLevel(32, MoveCatalog.RazorLeaf)
                .AtLevel(65, MoveCatalog.SolarBeam));

            // Fire Starter Line - Using available moves only
            Register("Charmander", m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
                .AtLevel(9, MoveCatalog.Ember)
                .AtLevel(38, MoveCatalog.Flamethrower)
                .ByTM(MoveCatalog.FireBlast));

            Register("Charmeleon", m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
                .AtLevel(17, MoveCatalog.Ember)
                .AtLevel(39, MoveCatalog.Flamethrower)
                .ByTM(MoveCatalog.FireBlast));

            Register("Charizard", m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
                .AtLevel(46, MoveCatalog.Flamethrower)
                .ByTM(MoveCatalog.FireBlast, MoveCatalog.Earthquake));

            // Water Starter Line - Using available moves only
            Register("Squirtle", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(13, MoveCatalog.WaterGun)
                .AtLevel(43, MoveCatalog.HydroPump));

            Register("Wartortle", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(13, MoveCatalog.WaterGun)
                .AtLevel(43, MoveCatalog.HydroPump));

            Register("Blastoise", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(13, MoveCatalog.WaterGun)
                .AtLevel(43, MoveCatalog.HydroPump)
                .ByTM(MoveCatalog.Surf));

            // Electric Pokemon - Using available moves only
            Register("Pikachu", m => m
                .StartsWith(MoveCatalog.ThunderShock, MoveCatalog.Growl)
                .AtLevel(11, MoveCatalog.QuickAttack)
                .AtLevel(26, MoveCatalog.Thunderbolt)
                .ByTM(MoveCatalog.Thunder));

            Register("Raichu", m => m
                .StartsWith(MoveCatalog.ThunderShock, MoveCatalog.QuickAttack)
                .AtLevel(1, MoveCatalog.Thunderbolt)
                .ByTM(MoveCatalog.Thunder));

            // Normal Pokemon - Using available moves only
            Register("Eevee", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(22, MoveCatalog.QuickAttack));

            // Psychic Pokemon - Using available moves only
            Register("Mewtwo", m => m
                .StartsWith(MoveCatalog.Confusion)
                .AtLevel(22, MoveCatalog.Psybeam)
                .AtLevel(36, MoveCatalog.Psychic));

            Register("Mew", m => m
                .StartsWith(MoveCatalog.Tackle)
                .AtLevel(40, MoveCatalog.Psychic));

            // Ghost/Poison Line - Using available moves only
            Register("Gastly", m => m
                .StartsWith(MoveCatalog.Lick, MoveCatalog.Hypnosis)
                .AtLevel(25, MoveCatalog.ShadowBall));

            Register("Haunter", m => m
                .StartsWith(MoveCatalog.Lick, MoveCatalog.Hypnosis)
                .AtLevel(25, MoveCatalog.ShadowBall));

            Register("Gengar", m => m
                .StartsWith(MoveCatalog.Lick, MoveCatalog.Hypnosis)
                .AtLevel(25, MoveCatalog.ShadowBall)
                .ByTM(MoveCatalog.SludgeBomb));

            // Rock/Ground Line - Using available moves only
            Register("Geodude", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.DefenseCurl)
                .AtLevel(11, MoveCatalog.RockThrow)
                .AtLevel(29, MoveCatalog.Earthquake)
                .AtLevel(50, MoveCatalog.RockSlide));

            Register("Graveler", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.DefenseCurl)
                .AtLevel(11, MoveCatalog.RockThrow)
                .AtLevel(29, MoveCatalog.Earthquake)
                .AtLevel(50, MoveCatalog.RockSlide));

            Register("Golem", m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.DefenseCurl)
                .AtLevel(11, MoveCatalog.RockThrow)
                .AtLevel(29, MoveCatalog.Earthquake)
                .AtLevel(50, MoveCatalog.RockSlide));
        }

        /// <summary>
        /// Register learnset data for a Pokemon.
        /// </summary>
        private static void Register(string pokemonName, System.Action<LearnsetBuilder> configure)
        {
            var builder = new LearnsetBuilder();
            configure(builder);
            var moves = builder.Build();
            var learnsetData = new LearnsetData(moves);
            _learnsets[pokemonName] = learnsetData;
        }

        #endregion
    }
}
