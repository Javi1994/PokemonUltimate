using System.Collections.Generic;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Static catalog of all Pokemon in the game.
    /// Provides direct access to Pokemon data and bulk registration to registries.
    /// Stats are official Gen 1 values.
    /// </summary>
    public static class PokemonCatalog
    {
        #region Generation 1 Starters - Grass Line

        public static readonly PokemonSpeciesData Bulbasaur = new PokemonSpeciesData
        {
            Name = "Bulbasaur",
            PokedexNumber = 1,
            PrimaryType = PokemonType.Grass,
            SecondaryType = PokemonType.Poison,
            BaseStats = new BaseStats(45, 49, 49, 65, 65, 45) // BST: 318
        };

        public static readonly PokemonSpeciesData Ivysaur = new PokemonSpeciesData
        {
            Name = "Ivysaur",
            PokedexNumber = 2,
            PrimaryType = PokemonType.Grass,
            SecondaryType = PokemonType.Poison,
            BaseStats = new BaseStats(60, 62, 63, 80, 80, 60) // BST: 405
        };

        public static readonly PokemonSpeciesData Venusaur = new PokemonSpeciesData
        {
            Name = "Venusaur",
            PokedexNumber = 3,
            PrimaryType = PokemonType.Grass,
            SecondaryType = PokemonType.Poison,
            BaseStats = new BaseStats(80, 82, 83, 100, 100, 80) // BST: 525
        };

        #endregion

        #region Generation 1 Starters - Fire Line

        public static readonly PokemonSpeciesData Charmander = new PokemonSpeciesData
        {
            Name = "Charmander",
            PokedexNumber = 4,
            PrimaryType = PokemonType.Fire,
            SecondaryType = null,
            BaseStats = new BaseStats(39, 52, 43, 60, 50, 65) // BST: 309
        };

        public static readonly PokemonSpeciesData Charmeleon = new PokemonSpeciesData
        {
            Name = "Charmeleon",
            PokedexNumber = 5,
            PrimaryType = PokemonType.Fire,
            SecondaryType = null,
            BaseStats = new BaseStats(58, 64, 58, 80, 65, 80) // BST: 405
        };

        public static readonly PokemonSpeciesData Charizard = new PokemonSpeciesData
        {
            Name = "Charizard",
            PokedexNumber = 6,
            PrimaryType = PokemonType.Fire,
            SecondaryType = PokemonType.Flying,
            BaseStats = new BaseStats(78, 84, 78, 109, 85, 100) // BST: 534
        };

        #endregion

        #region Generation 1 Starters - Water Line

        public static readonly PokemonSpeciesData Squirtle = new PokemonSpeciesData
        {
            Name = "Squirtle",
            PokedexNumber = 7,
            PrimaryType = PokemonType.Water,
            SecondaryType = null,
            BaseStats = new BaseStats(44, 48, 65, 50, 64, 43) // BST: 314
        };

        public static readonly PokemonSpeciesData Wartortle = new PokemonSpeciesData
        {
            Name = "Wartortle",
            PokedexNumber = 8,
            PrimaryType = PokemonType.Water,
            SecondaryType = null,
            BaseStats = new BaseStats(59, 63, 80, 65, 80, 58) // BST: 405
        };

        public static readonly PokemonSpeciesData Blastoise = new PokemonSpeciesData
        {
            Name = "Blastoise",
            PokedexNumber = 9,
            PrimaryType = PokemonType.Water,
            SecondaryType = null,
            BaseStats = new BaseStats(79, 83, 100, 85, 105, 78) // BST: 530
        };

        #endregion

        #region Electric Pokemon

        public static readonly PokemonSpeciesData Pikachu = new PokemonSpeciesData
        {
            Name = "Pikachu",
            PokedexNumber = 25,
            PrimaryType = PokemonType.Electric,
            SecondaryType = null,
            BaseStats = new BaseStats(35, 55, 40, 50, 50, 90) // BST: 320
        };

        public static readonly PokemonSpeciesData Raichu = new PokemonSpeciesData
        {
            Name = "Raichu",
            PokedexNumber = 26,
            PrimaryType = PokemonType.Electric,
            SecondaryType = null,
            BaseStats = new BaseStats(60, 90, 55, 90, 80, 110) // BST: 485
        };

        #endregion

        #region Normal Pokemon

        public static readonly PokemonSpeciesData Eevee = new PokemonSpeciesData
        {
            Name = "Eevee",
            PokedexNumber = 133,
            PrimaryType = PokemonType.Normal,
            SecondaryType = null,
            BaseStats = new BaseStats(55, 55, 50, 45, 65, 55) // BST: 325
        };

        public static readonly PokemonSpeciesData Snorlax = new PokemonSpeciesData
        {
            Name = "Snorlax",
            PokedexNumber = 143,
            PrimaryType = PokemonType.Normal,
            SecondaryType = null,
            BaseStats = new BaseStats(160, 110, 65, 65, 110, 30) // BST: 540
        };

        #endregion

        #region Legendary/Mythical

        public static readonly PokemonSpeciesData Mewtwo = new PokemonSpeciesData
        {
            Name = "Mewtwo",
            PokedexNumber = 150,
            PrimaryType = PokemonType.Psychic,
            SecondaryType = null,
            BaseStats = new BaseStats(106, 110, 90, 154, 90, 130) // BST: 680
        };

        public static readonly PokemonSpeciesData Mew = new PokemonSpeciesData
        {
            Name = "Mew",
            PokedexNumber = 151,
            PrimaryType = PokemonType.Psychic,
            SecondaryType = null,
            BaseStats = new BaseStats(100, 100, 100, 100, 100, 100) // BST: 600
        };

        #endregion

        #region All Pokemon & Registration

        /// <summary>
        /// All Pokemon defined in this catalog.
        /// </summary>
        public static IEnumerable<PokemonSpeciesData> All
        {
            get
            {
                // Grass Starters
                yield return Bulbasaur;
                yield return Ivysaur;
                yield return Venusaur;
                // Fire Starters
                yield return Charmander;
                yield return Charmeleon;
                yield return Charizard;
                // Water Starters
                yield return Squirtle;
                yield return Wartortle;
                yield return Blastoise;
                // Electric
                yield return Pikachu;
                yield return Raichu;
                // Normal
                yield return Eevee;
                yield return Snorlax;
                // Legendary
                yield return Mewtwo;
                yield return Mew;
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
        /// Count of all Pokemon in catalog.
        /// </summary>
        public static int Count => 15;

        #endregion
    }
}
