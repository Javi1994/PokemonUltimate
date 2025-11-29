using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Generation 1 Pokemon (Kanto region, #001-151).
    /// </summary>
    public static partial class PokemonCatalog
    {
        #region Starters - Grass Line

        public static readonly PokemonSpeciesData Bulbasaur = new PokemonSpeciesData
        {
            Name = "Bulbasaur",
            PokedexNumber = 1,
            PrimaryType = PokemonType.Grass,
            SecondaryType = PokemonType.Poison,
            BaseStats = new BaseStats(45, 49, 49, 65, 65, 45)
        };

        public static readonly PokemonSpeciesData Ivysaur = new PokemonSpeciesData
        {
            Name = "Ivysaur",
            PokedexNumber = 2,
            PrimaryType = PokemonType.Grass,
            SecondaryType = PokemonType.Poison,
            BaseStats = new BaseStats(60, 62, 63, 80, 80, 60)
        };

        public static readonly PokemonSpeciesData Venusaur = new PokemonSpeciesData
        {
            Name = "Venusaur",
            PokedexNumber = 3,
            PrimaryType = PokemonType.Grass,
            SecondaryType = PokemonType.Poison,
            BaseStats = new BaseStats(80, 82, 83, 100, 100, 80)
        };

        #endregion

        #region Starters - Fire Line

        public static readonly PokemonSpeciesData Charmander = new PokemonSpeciesData
        {
            Name = "Charmander",
            PokedexNumber = 4,
            PrimaryType = PokemonType.Fire,
            SecondaryType = null,
            BaseStats = new BaseStats(39, 52, 43, 60, 50, 65)
        };

        public static readonly PokemonSpeciesData Charmeleon = new PokemonSpeciesData
        {
            Name = "Charmeleon",
            PokedexNumber = 5,
            PrimaryType = PokemonType.Fire,
            SecondaryType = null,
            BaseStats = new BaseStats(58, 64, 58, 80, 65, 80)
        };

        public static readonly PokemonSpeciesData Charizard = new PokemonSpeciesData
        {
            Name = "Charizard",
            PokedexNumber = 6,
            PrimaryType = PokemonType.Fire,
            SecondaryType = PokemonType.Flying,
            BaseStats = new BaseStats(78, 84, 78, 109, 85, 100)
        };

        #endregion

        #region Starters - Water Line

        public static readonly PokemonSpeciesData Squirtle = new PokemonSpeciesData
        {
            Name = "Squirtle",
            PokedexNumber = 7,
            PrimaryType = PokemonType.Water,
            SecondaryType = null,
            BaseStats = new BaseStats(44, 48, 65, 50, 64, 43)
        };

        public static readonly PokemonSpeciesData Wartortle = new PokemonSpeciesData
        {
            Name = "Wartortle",
            PokedexNumber = 8,
            PrimaryType = PokemonType.Water,
            SecondaryType = null,
            BaseStats = new BaseStats(59, 63, 80, 65, 80, 58)
        };

        public static readonly PokemonSpeciesData Blastoise = new PokemonSpeciesData
        {
            Name = "Blastoise",
            PokedexNumber = 9,
            PrimaryType = PokemonType.Water,
            SecondaryType = null,
            BaseStats = new BaseStats(79, 83, 100, 85, 105, 78)
        };

        #endregion

        #region Electric

        public static readonly PokemonSpeciesData Pikachu = new PokemonSpeciesData
        {
            Name = "Pikachu",
            PokedexNumber = 25,
            PrimaryType = PokemonType.Electric,
            SecondaryType = null,
            BaseStats = new BaseStats(35, 55, 40, 50, 50, 90)
        };

        public static readonly PokemonSpeciesData Raichu = new PokemonSpeciesData
        {
            Name = "Raichu",
            PokedexNumber = 26,
            PrimaryType = PokemonType.Electric,
            SecondaryType = null,
            BaseStats = new BaseStats(60, 90, 55, 90, 80, 110)
        };

        #endregion

        #region Normal

        public static readonly PokemonSpeciesData Eevee = new PokemonSpeciesData
        {
            Name = "Eevee",
            PokedexNumber = 133,
            PrimaryType = PokemonType.Normal,
            SecondaryType = null,
            BaseStats = new BaseStats(55, 55, 50, 45, 65, 55)
        };

        public static readonly PokemonSpeciesData Snorlax = new PokemonSpeciesData
        {
            Name = "Snorlax",
            PokedexNumber = 143,
            PrimaryType = PokemonType.Normal,
            SecondaryType = null,
            BaseStats = new BaseStats(160, 110, 65, 65, 110, 30)
        };

        #endregion

        #region Legendary/Mythical

        public static readonly PokemonSpeciesData Mewtwo = new PokemonSpeciesData
        {
            Name = "Mewtwo",
            PokedexNumber = 150,
            PrimaryType = PokemonType.Psychic,
            SecondaryType = null,
            BaseStats = new BaseStats(106, 110, 90, 154, 90, 130)
        };

        public static readonly PokemonSpeciesData Mew = new PokemonSpeciesData
        {
            Name = "Mew",
            PokedexNumber = 151,
            PrimaryType = PokemonType.Psychic,
            SecondaryType = null,
            BaseStats = new BaseStats(100, 100, 100, 100, 100, 100)
        };

        #endregion

        #region Registration

        static partial void RegisterGen1()
        {
            // Starters
            _all.Add(Bulbasaur);
            _all.Add(Ivysaur);
            _all.Add(Venusaur);
            _all.Add(Charmander);
            _all.Add(Charmeleon);
            _all.Add(Charizard);
            _all.Add(Squirtle);
            _all.Add(Wartortle);
            _all.Add(Blastoise);
            // Electric
            _all.Add(Pikachu);
            _all.Add(Raichu);
            // Normal
            _all.Add(Eevee);
            _all.Add(Snorlax);
            // Legendary
            _all.Add(Mewtwo);
            _all.Add(Mew);
        }

        #endregion
    }
}

