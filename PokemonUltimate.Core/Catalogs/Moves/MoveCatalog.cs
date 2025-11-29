using System.Collections.Generic;
using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Static catalog of all Moves in the game.
    /// Organized by type using partial classes.
    /// Each type file (MoveCatalog.Fire.cs, etc.) adds its Moves.
    /// </summary>
    public static partial class MoveCatalog
    {
        private static List<MoveData> _all;

        /// <summary>
        /// All Moves defined in this catalog (lazy initialized).
        /// </summary>
        public static IEnumerable<MoveData> All
        {
            get
            {
                if (_all == null) InitializeAll();
                return _all;
            }
        }

        /// <summary>
        /// Count of all Moves in catalog.
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
        /// Register all Moves from this catalog into a registry.
        /// </summary>
        public static void RegisterAll(IMoveRegistry registry)
        {
            foreach (var move in All)
            {
                registry.Register(move);
            }
        }

        /// <summary>
        /// Initialize all Moves. Each partial class implements its Register method.
        /// Add new RegisterTypeX() calls here when adding new types.
        /// </summary>
        private static void InitializeAll()
        {
            _all = new List<MoveData>();
            
            // Register each type - add new lines here when adding types
            RegisterNormal();
            RegisterFire();
            RegisterWater();
            RegisterGrass();
            RegisterElectric();
            RegisterGround();
            RegisterPsychic();
        }

        // Partial methods - implemented in separate files
        static partial void RegisterNormal();
        static partial void RegisterFire();
        static partial void RegisterWater();
        static partial void RegisterGrass();
        static partial void RegisterElectric();
        static partial void RegisterGround();
        static partial void RegisterPsychic();
    }
}

