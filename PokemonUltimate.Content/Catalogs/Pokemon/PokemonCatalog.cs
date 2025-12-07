// Pokemon names and game data referenced here are trademarks of Nintendo/Game Freak/The Pokemon Company.
// This is a non-commercial fan project for educational purposes only. See LEGAL.md for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Registry.Definition;

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
        // Generation ranges
        private const int Gen1Start = 1;
        private const int Gen1End = 151;
        private const int Gen2Start = 152;
        private const int Gen2End = 251;
        private const int Gen3Start = 252;
        private const int Gen3End = 386;
        private const int Gen4Start = 387;
        private const int Gen4End = 493;
        private const int Gen5Start = 494;
        private const int Gen5End = 649;

        private static readonly object _lockObject = new object();
        private static volatile List<PokemonSpeciesData> _all;

        /// <summary>
        /// All Pokemon defined in this catalog (lazy initialized, thread-safe).
        /// Returns a read-only collection for better performance and immutability.
        /// </summary>
        public static IReadOnlyList<PokemonSpeciesData> All
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
        /// Count of all Pokemon in catalog.
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
        /// Gets all Pokemon from Generation 1 (Pokedex numbers 1-151).
        /// </summary>
        /// <returns>All Generation 1 Pokemon.</returns>
        public static IEnumerable<PokemonSpeciesData> GetAllGen1()
        {
            return All.Where(p => p.PokedexNumber >= Gen1Start && p.PokedexNumber <= Gen1End);
        }

        /// <summary>
        /// Gets a Pokemon by its Pokedex number.
        /// </summary>
        /// <param name="pokedexNumber">The Pokedex number to search for.</param>
        /// <returns>The Pokemon with the specified Pokedex number, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown when pokedexNumber is less than 1.</exception>
        public static PokemonSpeciesData GetByPokedexNumber(int pokedexNumber)
        {
            if (pokedexNumber < 1)
                throw new ArgumentException("Pokedex number must be greater than 0", nameof(pokedexNumber));

            return All.FirstOrDefault(p => p.PokedexNumber == pokedexNumber);
        }

        /// <summary>
        /// Gets all Pokemon of a specific type (primary or secondary).
        /// </summary>
        /// <param name="type">The Pokemon type to filter by.</param>
        /// <returns>All Pokemon that have the specified type as primary or secondary.</returns>
        public static IEnumerable<PokemonSpeciesData> GetAllByType(PokemonType type)
        {
            return All.Where(p => p.PrimaryType == type || p.SecondaryType == type);
        }

        /// <summary>
        /// Register all Pokemon from this catalog into a registry.
        /// </summary>
        /// <param name="registry">The registry to register Pokemon to.</param>
        /// <exception cref="ArgumentNullException">Thrown when registry is null.</exception>
        public static void RegisterAll(IPokemonRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            foreach (var pokemon in All)
            {
                registry.Register(pokemon);
            }
        }

        /// <summary>
        /// Initialize all Pokemon. Each partial class implements its Register method.
        /// Add new RegisterGenX() calls here when adding new generations.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when catalog integrity validation fails.</exception>
        private static void InitializeAll()
        {
            _all = new List<PokemonSpeciesData>();

            try
            {
                // Register each generation - add new lines here when adding generations
                RegisterGen1();
                RegisterGen3();
                RegisterGen4();
                RegisterGen5();
                // RegisterGen2();  // Uncomment when Gen2 is added
                // RegisterCustom(); // For custom Pokemon

                ValidateCatalogIntegrity();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to initialize Pokemon catalog: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates catalog integrity: checks for duplicate Pokedex numbers, names, and validates references.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
        private static void ValidateCatalogIntegrity()
        {
            // Validate duplicate Pokedex numbers
            var duplicateNumbers = _all
                .GroupBy(p => p.PokedexNumber)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNumbers.Any())
            {
                throw new InvalidOperationException(
                    $"Duplicate Pokedex numbers found: {string.Join(", ", duplicateNumbers)}");
            }

            // Validate duplicate names
            var duplicateNames = _all
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                throw new InvalidOperationException(
                    $"Duplicate Pokemon names found: {string.Join(", ", duplicateNames)}");
            }

            // Validate evolution references
            ValidateEvolutionReferences();

            // Validate learnset references
            ValidateLearnsetReferences();
        }

        /// <summary>
        /// Validates that all evolution targets exist in the catalog.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when invalid evolution references are found.</exception>
        private static void ValidateEvolutionReferences()
        {
            var allPokemonNames = new HashSet<string>(
                _all.Select(p => p.Name),
                StringComparer.OrdinalIgnoreCase);

            foreach (var pokemon in _all)
            {
                foreach (var evolution in pokemon.Evolutions)
                {
                    if (evolution.Target == null)
                    {
                        throw new InvalidOperationException(
                            $"Pokemon {pokemon.Name} has null evolution target");
                    }

                    if (!allPokemonNames.Contains(evolution.Target.Name))
                    {
                        throw new InvalidOperationException(
                            $"Pokemon {pokemon.Name} references evolution target '{evolution.Target.Name}' " +
                            $"which is not in the catalog");
                    }
                }
            }
        }

        /// <summary>
        /// Validates that all moves in learnsets exist in MoveCatalog.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when invalid move references are found.</exception>
        private static void ValidateLearnsetReferences()
        {
            var allMoveNames = new HashSet<string>(
                Catalogs.Moves.MoveCatalog.All.Select(m => m.Name),
                StringComparer.OrdinalIgnoreCase);

            foreach (var pokemon in _all)
            {
                foreach (var learnableMove in pokemon.Learnset)
                {
                    if (learnableMove.Move == null)
                    {
                        throw new InvalidOperationException(
                            $"Pokemon {pokemon.Name} has null move in learnset");
                    }

                    if (!allMoveNames.Contains(learnableMove.Move.Name))
                    {
                        throw new InvalidOperationException(
                            $"Pokemon {pokemon.Name} references move '{learnableMove.Move.Name}' " +
                            $"which is not in MoveCatalog");
                    }
                }
            }
        }

        // Partial methods - implemented in separate files
        /// <summary>
        /// Registers all Generation 1 Pokemon to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGen1();

        /// <summary>
        /// Registers all Generation 3 Pokemon to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGen3();

        /// <summary>
        /// Registers all Generation 4 Pokemon to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGen4();

        /// <summary>
        /// Registers all Generation 5 Pokemon to the catalog.
        /// Called automatically during catalog initialization.
        /// </summary>
        static partial void RegisterGen5();

        // static partial void RegisterGen2();
        // static partial void RegisterCustom();
    }
}

