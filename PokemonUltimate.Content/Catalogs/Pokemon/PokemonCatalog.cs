// Pokemon names and game data referenced here are trademarks of Nintendo/Game Freak/The Pokemon Company.
// This is a non-commercial fan project for educational purposes only. See LEGAL.md for details.

using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    /// <summary>
    /// Static catalog of all Pokemon in the game.
    /// Organized by generation using partial classes.
    /// Each generation file (PokemonCatalog.Gen1.cs, etc.) adds its Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// 
    /// Pokemon names are trademarks of Nintendo/Game Freak. Non-commercial fan project.
    /// </remarks>
    public static partial class PokemonCatalog
    {
        private static List<PokemonSpeciesData> _all;

        /// <summary>
        /// All Pokemon defined in this catalog (lazy initialized).
        /// </summary>
        public static IEnumerable<PokemonSpeciesData> All
        {
            get
            {
                if (_all == null) InitializeAll();
                return _all;
            }
        }

        /// <summary>
        /// Count of all Pokemon in catalog.
        /// </summary>
        public static int Count
        {
            get
            {
                if (_all == null) InitializeAll();
                return _all.Count;
            }
        }

        /// <summary>
        /// Register all Pokemon from this catalog into a registry.
        /// </summary>
        public static void RegisterAll(IPokemonRegistry registry)
        {
            foreach (var pokemon in All)
            {
                registry.Register(pokemon);
            }
        }

        /// <summary>
        /// Initialize all Pokemon. Each partial class implements its Register method.
        /// Add new RegisterGenX() calls here when adding new generations.
        /// </summary>
        private static void InitializeAll()
        {
            _all = new List<PokemonSpeciesData>();
            
            // Register each generation - add new lines here when adding generations
            RegisterGen1();
            RegisterGen3();
            RegisterGen4();
            RegisterGen5();
            // RegisterGen2();  // Uncomment when Gen2 is added
            // RegisterCustom(); // For custom Pokemon
        }

        // Partial methods - implemented in separate files
        static partial void RegisterGen1();
        static partial void RegisterGen3();
        static partial void RegisterGen4();
        static partial void RegisterGen5();
        // static partial void RegisterGen2();
        // static partial void RegisterCustom();
    }
}

