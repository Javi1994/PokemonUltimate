using System.Collections.Generic;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Catalogs
{
    // Static catalog of all Pokemon in the game.
    // Provides direct access to Pokemon data and bulk registration to registries.
    public static class PokemonCatalog
    {
        #region Generation 1 Starters

        public static readonly PokemonSpeciesData Bulbasaur = new PokemonSpeciesData
        {
            Name = "Bulbasaur",
            PokedexNumber = 1
        };

        public static readonly PokemonSpeciesData Ivysaur = new PokemonSpeciesData
        {
            Name = "Ivysaur",
            PokedexNumber = 2
        };

        public static readonly PokemonSpeciesData Venusaur = new PokemonSpeciesData
        {
            Name = "Venusaur",
            PokedexNumber = 3
        };

        public static readonly PokemonSpeciesData Charmander = new PokemonSpeciesData
        {
            Name = "Charmander",
            PokedexNumber = 4
        };

        public static readonly PokemonSpeciesData Charmeleon = new PokemonSpeciesData
        {
            Name = "Charmeleon",
            PokedexNumber = 5
        };

        public static readonly PokemonSpeciesData Charizard = new PokemonSpeciesData
        {
            Name = "Charizard",
            PokedexNumber = 6
        };

        public static readonly PokemonSpeciesData Squirtle = new PokemonSpeciesData
        {
            Name = "Squirtle",
            PokedexNumber = 7
        };

        public static readonly PokemonSpeciesData Wartortle = new PokemonSpeciesData
        {
            Name = "Wartortle",
            PokedexNumber = 8
        };

        public static readonly PokemonSpeciesData Blastoise = new PokemonSpeciesData
        {
            Name = "Blastoise",
            PokedexNumber = 9
        };

        #endregion

        #region Popular Pokemon

        public static readonly PokemonSpeciesData Pikachu = new PokemonSpeciesData
        {
            Name = "Pikachu",
            PokedexNumber = 25
        };

        public static readonly PokemonSpeciesData Raichu = new PokemonSpeciesData
        {
            Name = "Raichu",
            PokedexNumber = 26
        };

        public static readonly PokemonSpeciesData Eevee = new PokemonSpeciesData
        {
            Name = "Eevee",
            PokedexNumber = 133
        };

        public static readonly PokemonSpeciesData Snorlax = new PokemonSpeciesData
        {
            Name = "Snorlax",
            PokedexNumber = 143
        };

        public static readonly PokemonSpeciesData Mewtwo = new PokemonSpeciesData
        {
            Name = "Mewtwo",
            PokedexNumber = 150
        };

        public static readonly PokemonSpeciesData Mew = new PokemonSpeciesData
        {
            Name = "Mew",
            PokedexNumber = 151
        };

        #endregion

        #region All Pokemon & Registration

        // All Pokemon defined in this catalog
        public static IEnumerable<PokemonSpeciesData> All
        {
            get
            {
                // Starters
                yield return Bulbasaur;
                yield return Ivysaur;
                yield return Venusaur;
                yield return Charmander;
                yield return Charmeleon;
                yield return Charizard;
                yield return Squirtle;
                yield return Wartortle;
                yield return Blastoise;
                // Popular
                yield return Pikachu;
                yield return Raichu;
                yield return Eevee;
                yield return Snorlax;
                yield return Mewtwo;
                yield return Mew;
            }
        }

        // Register all Pokemon from this catalog into a registry
        public static void RegisterAll(IPokemonRegistry registry)
        {
            foreach (var pokemon in All)
            {
                registry.Register(pokemon);
            }
        }

        // Get count of all Pokemon in catalog
        public static int Count => 15;

        #endregion
    }
}

