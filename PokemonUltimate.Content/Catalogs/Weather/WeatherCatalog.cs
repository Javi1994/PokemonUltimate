using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Weather
{
    /// <summary>
    /// Central catalog of all weather condition definitions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.6: Content Organization
    /// **Documentation**: See `docs/features/3-content-expansion/3.6-content-organization/README.md`
    /// </remarks>
    public static class WeatherCatalog
    {
        private static readonly List<WeatherData> _all = new List<WeatherData>();

        /// <summary>
        /// Gets all registered weather conditions.
        /// </summary>
        public static IReadOnlyList<WeatherData> All => _all;

        #region Standard Weather (5 turns)

        /// <summary>
        /// Rain: Boosts Water 1.5x, weakens Fire 0.5x.
        /// Enables Thunder and Hurricane to have 100% accuracy.
        /// </summary>
        public static readonly WeatherData Rain = WeatherEffect.Define("Rain")
            .Description("Rain is falling. Water moves are boosted and Fire moves are weakened.")
            .Type(Core.Enums.Weather.Rain)
            .Duration(5)
            .Boosts(PokemonType.Water)
            .Weakens(PokemonType.Fire)
            .PerfectAccuracy("Thunder", "Hurricane")
            .AbilitiesDoubleSpeed("Swift Swim")
            .AbilitiesHeal("Rain Dish", "Dry Skin")
            .Build();

        /// <summary>
        /// Harsh Sunlight: Boosts Fire 1.5x, weakens Water 0.5x.
        /// Enables Solar Beam/Blade to skip charge turn.
        /// </summary>
        public static readonly WeatherData Sun = WeatherEffect.Define("Harsh Sunlight")
            .Description("The sunlight is harsh. Fire moves are boosted and Water moves are weakened.")
            .Type(Core.Enums.Weather.Sun)
            .Duration(5)
            .Boosts(PokemonType.Fire)
            .Weakens(PokemonType.Water)
            .InstantCharge("Solar Beam", "Solar Blade")
            .AbilitiesDoubleSpeed("Chlorophyll")
            .AbilitiesHeal("Solar Power") // Note: Solar Power boosts SpAtk but deals damage
            .Build();

        /// <summary>
        /// Sandstorm: Deals 1/16 damage to non-Rock/Ground/Steel.
        /// Boosts Rock-type SpDef by 1.5x.
        /// </summary>
        public static readonly WeatherData Sandstorm = WeatherEffect.Define("Sandstorm")
            .Description("A sandstorm is raging. Non-Rock/Ground/Steel types take damage each turn.")
            .Type(Core.Enums.Weather.Sandstorm)
            .Duration(5)
            .DealsDamagePerTurn(1f / 16f, PokemonType.Rock, PokemonType.Ground, PokemonType.Steel)
            .BoostsStat(Stat.SpDefense, 1.5f, PokemonType.Rock)
            .AbilitiesImmuneToDamage("Sand Veil", "Sand Rush", "Sand Force", "Overcoat", "Magic Guard")
            .AbilitiesDoubleSpeed("Sand Rush")
            .Build();

        /// <summary>
        /// Hail: Deals 1/16 damage to non-Ice types.
        /// Enables Blizzard to have 100% accuracy.
        /// </summary>
        public static readonly WeatherData Hail = WeatherEffect.Define("Hail")
            .Description("It is hailing. Non-Ice types take damage each turn.")
            .Type(Core.Enums.Weather.Hail)
            .Duration(5)
            .DealsDamagePerTurn(1f / 16f, PokemonType.Ice)
            .PerfectAccuracy("Blizzard")
            .AbilitiesImmuneToDamage("Ice Body", "Snow Cloak", "Overcoat", "Magic Guard")
            .AbilitiesHeal("Ice Body")
            .Build();

        /// <summary>
        /// Snow (Gen 9): Boosts Ice-type Defense by 1.5x, no damage.
        /// Enables Blizzard to have 100% accuracy.
        /// </summary>
        public static readonly WeatherData Snow = WeatherEffect.Define("Snow")
            .Description("It is snowing. Ice-type Defense is boosted.")
            .Type(Core.Enums.Weather.Snow)
            .Duration(5)
            .BoostsStat(Stat.Defense, 1.5f, PokemonType.Ice)
            .PerfectAccuracy("Blizzard")
            .AbilitiesDoubleSpeed("Slush Rush")
            .Build();

        #endregion

        #region Primal Weather (Indefinite, cannot be overwritten)

        /// <summary>
        /// Heavy Rain (Primordial Sea): Water boosted, Fire NULLIFIED.
        /// Cannot be overwritten by regular weather.
        /// </summary>
        public static readonly WeatherData HeavyRain = WeatherEffect.Define("Heavy Rain")
            .Description("Heavy rain is falling! Fire-type moves are completely nullified.")
            .Type(Core.Enums.Weather.HeavyRain)
            .Indefinite()
            .Primal()
            .Boosts(PokemonType.Water)
            .Nullifies(PokemonType.Fire)
            .PerfectAccuracy("Thunder", "Hurricane")
            .Build();

        /// <summary>
        /// Extremely Harsh Sunlight (Desolate Land): Fire boosted, Water NULLIFIED.
        /// Cannot be overwritten by regular weather.
        /// </summary>
        public static readonly WeatherData ExtremelyHarshSunlight = WeatherEffect.Define("Extremely Harsh Sunlight")
            .Description("The sunlight is extremely harsh! Water-type moves are completely nullified.")
            .Type(Core.Enums.Weather.ExtremelyHarshSunlight)
            .Indefinite()
            .Primal()
            .Boosts(PokemonType.Fire)
            .Nullifies(PokemonType.Water)
            .InstantCharge("Solar Beam", "Solar Blade")
            .Build();

        /// <summary>
        /// Strong Winds (Delta Stream): Reduces super-effective damage to Flying.
        /// Cannot be overwritten by regular weather.
        /// </summary>
        public static readonly WeatherData StrongWinds = WeatherEffect.Define("Strong Winds")
            .Description("Mysterious strong winds are blowing! Flying-type weaknesses are reduced.")
            .Type(Core.Enums.Weather.StrongWinds)
            .Indefinite()
            .Primal()
            .Build();

        #endregion

        #region Special Weather

        /// <summary>
        /// Fog (Gen 4): Reduces accuracy of all moves.
        /// </summary>
        public static readonly WeatherData Fog = WeatherEffect.Define("Fog")
            .Description("The fog is deep. Move accuracy is reduced.")
            .Type(Core.Enums.Weather.Fog)
            .Indefinite() // Only in specific areas, removed by Defog
            .Build();

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets weather data by weather enum.
        /// </summary>
        public static WeatherData GetByWeather(Core.Enums.Weather weather)
        {
            switch (weather)
            {
                case Core.Enums.Weather.Rain: return Rain;
                case Core.Enums.Weather.Sun: return Sun;
                case Core.Enums.Weather.Sandstorm: return Sandstorm;
                case Core.Enums.Weather.Hail: return Hail;
                case Core.Enums.Weather.Snow: return Snow;
                case Core.Enums.Weather.HeavyRain: return HeavyRain;
                case Core.Enums.Weather.ExtremelyHarshSunlight: return ExtremelyHarshSunlight;
                case Core.Enums.Weather.StrongWinds: return StrongWinds;
                case Core.Enums.Weather.Fog: return Fog;
                default: return null;
            }
        }

        /// <summary>
        /// Gets weather data by name.
        /// </summary>
        public static WeatherData GetByName(string name)
        {
            return _all.Find(w => w.Name == name);
        }

        /// <summary>
        /// Gets all weather conditions that deal damage.
        /// </summary>
        public static IEnumerable<WeatherData> GetDamagingWeather()
        {
            foreach (var weather in _all)
            {
                if (weather.DealsDamage) yield return weather;
            }
        }

        /// <summary>
        /// Gets all primal weather conditions.
        /// </summary>
        public static IEnumerable<WeatherData> GetPrimalWeather()
        {
            foreach (var weather in _all)
            {
                if (weather.IsPrimal) yield return weather;
            }
        }

        #endregion

        #region Static Constructor

        static WeatherCatalog()
        {
            // Standard weather
            _all.Add(Rain);
            _all.Add(Sun);
            _all.Add(Sandstorm);
            _all.Add(Hail);
            _all.Add(Snow);

            // Primal weather
            _all.Add(HeavyRain);
            _all.Add(ExtremelyHarshSunlight);
            _all.Add(StrongWinds);

            // Special weather
            _all.Add(Fog);
        }

        #endregion
    }
}

