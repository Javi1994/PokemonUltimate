// Move names and game data referenced here are trademarks of Nintendo/Game Freak/The Pokemon Company.
// This is a non-commercial fan project for educational purposes only. See LEGAL.md for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Registry.Definition;

namespace PokemonUltimate.Content.Catalogs.Moves
{
    /// <summary>
    /// Static catalog of all Moves in the game.
    /// Organized by type using partial classes.
    /// Each type file (MoveCatalog.Fire.cs, etc.) adds its Moves.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.2: Move Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.2-move-expansion/architecture.md`
    ///
    /// Move names are trademarks of Nintendo/Game Freak. Non-commercial fan project.
    /// </remarks>
    public static partial class MoveCatalog
    {
        private static readonly object _lockObject = new object();
        private static volatile List<MoveData> _all;

        /// <summary>
        /// All Moves defined in this catalog (lazy initialized, thread-safe).
        /// Returns a read-only collection for better performance and immutability.
        /// </summary>
        public static IReadOnlyList<MoveData> All
        {
            get
            {
                if (_all == null)
                {
                    lock (_lockObject)
                    {
                        if (_all == null)
                        {
                            InitializeAll();
                        }
                    }
                }
                return _all.AsReadOnly();
            }
        }

        /// <summary>
        /// Count of all Moves in catalog.
        /// </summary>
        public static int Count
        {
            get
            {
                if (_all == null)
                {
                    lock (_lockObject)
                    {
                        if (_all == null)
                        {
                            InitializeAll();
                        }
                    }
                }
                return _all.Count;
            }
        }

        /// <summary>
        /// Gets all moves of a specific type.
        /// </summary>
        /// <param name="type">The move type to filter by.</param>
        /// <returns>All moves of the specified type.</returns>
        public static IEnumerable<MoveData> GetAllByType(PokemonType type)
        {
            return All.Where(m => m.Type == type);
        }

        /// <summary>
        /// Gets a move by name (case-insensitive).
        /// </summary>
        /// <param name="name">The move name to search for.</param>
        /// <returns>The move with the specified name, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown when name is null, empty, or whitespace.</exception>
        public static MoveData GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Move name cannot be null or empty", nameof(name));

            return All.FirstOrDefault(m =>
                string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Register all Moves from this catalog into a registry.
        /// </summary>
        /// <param name="registry">The registry to register Moves to.</param>
        /// <exception cref="ArgumentNullException">Thrown when registry is null.</exception>
        public static void RegisterAll(IMoveRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

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
            RegisterGhost();
            RegisterRock();
            RegisterFlying();
            RegisterPoison();
            RegisterDragon();
        }

        // Partial methods - implemented in separate files
        /// <summary>
        /// Registers all Normal-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterNormal();

        /// <summary>
        /// Registers all Fire-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterFire();

        /// <summary>
        /// Registers all Water-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterWater();

        /// <summary>
        /// Registers all Grass-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGrass();

        /// <summary>
        /// Registers all Electric-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterElectric();

        /// <summary>
        /// Registers all Ground-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGround();

        /// <summary>
        /// Registers all Psychic-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterPsychic();

        /// <summary>
        /// Registers all Ghost-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGhost();

        /// <summary>
        /// Registers all Rock-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterRock();

        /// <summary>
        /// Registers all Flying-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterFlying();

        /// <summary>
        /// Registers all Poison-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterPoison();

        /// <summary>
        /// Registers all Dragon-type moves to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterDragon();
    }
}

