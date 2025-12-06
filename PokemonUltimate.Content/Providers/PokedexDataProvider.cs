using System.Collections.Generic;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Provides Pokedex data for Pokemon species.
    /// Centralized data source for Pokedex fields (Description, Category, Height, Weight, Color, Shape, Habitat).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.19: Pokedex Fields
    /// **Documentation**: See `docs/features/1-game-data/1.19-pokedex-fields/README.md`
    ///
    /// **Design**: Follows SRP (Single Responsibility Principle) - only responsible for providing Pokedex data.
    /// Data is organized by generation for maintainability.
    /// </remarks>
    public static class PokedexDataProvider
    {
        private static readonly Dictionary<string, PokedexData> _data = new Dictionary<string, PokedexData>(System.StringComparer.OrdinalIgnoreCase);

        static PokedexDataProvider()
        {
            InitializeGen1();
            // InitializeGen2(); // Uncomment when Gen2 data is added
        }

        /// <summary>
        /// Gets Pokedex data for a Pokemon by name.
        /// </summary>
        /// <param name="pokemonName">The name of the Pokemon (case-insensitive).</param>
        /// <returns>Pokedex data if found, null otherwise.</returns>
        public static PokedexData GetData(string pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return null;

            _data.TryGetValue(pokemonName, out var data);
            return data;
        }

        /// <summary>
        /// Checks if Pokedex data exists for a Pokemon.
        /// </summary>
        /// <param name="pokemonName">The name of the Pokemon (case-insensitive).</param>
        /// <returns>True if data exists, false otherwise.</returns>
        public static bool HasData(string pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return false;

            return _data.ContainsKey(pokemonName);
        }

        #region Generation Data Initialization

        /// <summary>
        /// Initialize Generation 1 Pokemon Pokedex data.
        /// </summary>
        private static void InitializeGen1()
        {
            // Grass Starter Line
            Register("Bulbasaur", "A strange seed was planted on its back at birth. The plant sprouts and grows with this Pokemon.",
                "Seed Pokemon", 0.7f, 6.9f, PokemonColor.Green, PokemonShape.Quadruped, PokemonHabitat.Grassland);

            Register("Ivysaur", "When the bulb on its back grows large, it appears to lose the ability to stand on its hind legs.",
                "Seed Pokemon", 1.0f, 13.0f, PokemonColor.Green, PokemonShape.Quadruped, PokemonHabitat.Grassland);

            Register("Venusaur", "The plant blooms when it absorbs solar energy. It stays on the move to seek sunlight.",
                "Seed Pokemon", 2.0f, 100.0f, PokemonColor.Green, PokemonShape.Quadruped, PokemonHabitat.Grassland);

            // Fire Starter Line
            Register("Charmander", "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.",
                "Lizard Pokemon", 0.6f, 8.5f, PokemonColor.Red, PokemonShape.Quadruped, PokemonHabitat.Mountain);

            Register("Charmeleon", "When it swings its burning tail, it elevates the air temperature to unbearably high levels.",
                "Flame Pokemon", 1.1f, 19.0f, PokemonColor.Red, PokemonShape.Quadruped, PokemonHabitat.Mountain);

            Register("Charizard", "Spits fire that is hot enough to melt boulders. Known to cause forest fires unintentionally.",
                "Flame Pokemon", 1.7f, 90.5f, PokemonColor.Red, PokemonShape.Wings, PokemonHabitat.Mountain);

            // Water Starter Line
            Register("Squirtle", "When it retracts its long neck into its shell, it squirts out water with vigorous force.",
                "Tiny Turtle Pokemon", 0.5f, 9.0f, PokemonColor.Blue, PokemonShape.Quadruped, PokemonHabitat.WatersEdge);

            Register("Wartortle", "Often hides in water to stalk unwary prey. For fast swimming, it moves its ears to maintain balance.",
                "Turtle Pokemon", 1.0f, 22.5f, PokemonColor.Blue, PokemonShape.Quadruped, PokemonHabitat.WatersEdge);

            Register("Blastoise", "Once it takes aim at its enemy, it blasts out water with even more force than a fire hose.",
                "Shellfish Pokemon", 1.6f, 85.5f, PokemonColor.Blue, PokemonShape.Quadruped, PokemonHabitat.WatersEdge);

            // Electric Pokemon
            Register("Pikachu", "When it releases pent-up energy in a burst, the power is equal to a lightning bolt.",
                "Mouse Pokemon", 0.4f, 6.0f, PokemonColor.Yellow, PokemonShape.Quadruped, PokemonHabitat.Forest);

            Register("Raichu", "Its long tail serves as a ground to protect itself from its own high-voltage power.",
                "Mouse Pokemon", 0.8f, 30.0f, PokemonColor.Yellow, PokemonShape.Quadruped, PokemonHabitat.Forest);

            // Normal Pokemon
            Register("Eevee", "Its genetic code is irregular. It may mutate if it is exposed to radiation from element stones.",
                "Evolution Pokemon", 0.3f, 6.5f, PokemonColor.Brown, PokemonShape.Quadruped, PokemonHabitat.Urban);

            // Psychic Pokemon
            Register("Mewtwo", "A Pokemon created by genetic manipulation. However, even though the scientific power of humans created this Pokemon's body, they failed to endow Mewtwo with a compassionate heart.",
                "Genetic Pokemon", 2.0f, 122.0f, PokemonColor.Violet, PokemonShape.Upright, PokemonHabitat.Rare);

            Register("Mew", "So rare that it is still said to be a mirage by many experts. Only a few people have seen it worldwide.",
                "New Species Pokemon", 0.4f, 4.0f, PokemonColor.Pink, PokemonShape.Quadruped, PokemonHabitat.Rare);

            // Ghost/Poison Line
            Register("Gastly", "Born from gases, anyone would faint if engulfed by its gaseous body, which contains poison.",
                "Gas Pokemon", 1.3f, 0.1f, PokemonColor.Violet, PokemonShape.Blob, PokemonHabitat.Cave);

            Register("Haunter", "By licking, it saps the victim's life. It causes shaking that won't stop until the victim's demise.",
                "Gas Pokemon", 1.6f, 0.1f, PokemonColor.Violet, PokemonShape.Blob, PokemonHabitat.Cave);

            Register("Gengar", "On the night of a full moon, if shadows of Pokemon dancing are seen, it's Gengar's doing.",
                "Shadow Pokemon", 1.5f, 40.5f, PokemonColor.Violet, PokemonShape.Humanoid, PokemonHabitat.Cave);

            // Rock/Ground Line
            Register("Geodude", "Found in fields and mountains. Mistaking them for boulders, people often step or trip on them.",
                "Rock Pokemon", 0.4f, 20.0f, PokemonColor.Brown, PokemonShape.Blob, PokemonHabitat.Mountain);

            Register("Graveler", "Rolls down slopes to move. It rolls over any obstacle without slowing or changing its direction.",
                "Rock Pokemon", 1.0f, 105.0f, PokemonColor.Brown, PokemonShape.Blob, PokemonHabitat.Mountain);

            Register("Golem", "Its boulder-like body is extremely hard. It can easily withstand dynamite blasts without damage.",
                "Megaton Pokemon", 1.4f, 300.0f, PokemonColor.Brown, PokemonShape.Blob, PokemonHabitat.Mountain);
        }

        /// <summary>
        /// Register Pokedex data for a Pokemon.
        /// </summary>
        private static void Register(
            string pokemonName,
            string description,
            string category,
            float height,
            float weight,
            PokemonColor color,
            PokemonShape shape,
            PokemonHabitat habitat)
        {
            var data = new PokedexData(description, category, height, weight, color, shape, habitat);
            _data[pokemonName] = data;
        }

        #endregion
    }
}
