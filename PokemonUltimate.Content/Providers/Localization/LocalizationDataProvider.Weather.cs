using System.Collections.Generic;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for weather name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeWeatherTerrain()
        {
            Register(LocalizationKey.WeatherSandstormDamage, new Dictionary<string, string>
            {
                { "en", "{0} is buffeted by the sandstorm!" },
                { "es", "¡{0} es azotado por la tormenta de arena!" },
                { "fr", "{0} est frappé par la tempête de sable!" }
            });

            Register(LocalizationKey.WeatherHailDamage, new Dictionary<string, string>
            {
                { "en", "{0} is pelted by hail!" },
                { "es", "¡{0} es golpeado por el granizo!" },
                { "fr", "{0} est frappé par la grêle!" }
            });

            Register(LocalizationKey.TerrainHealing, new Dictionary<string, string>
            {
                { "en", "{0} is healed by the {1}!" },
                { "es", "¡{0} fue curado por el {1}!" },
                { "fr", "{0} est soigné par le {1}!" }
            });
        }

        private static void InitializeWeatherNames()
        {
            // Weather names
            RegisterWeatherName("Rain", "Rain", "Lluvia", "Pluie");
            RegisterWeatherName("Harsh Sunlight", "Harsh Sunlight", "Sol Abrasador", "Soleil Brûlant");
            RegisterWeatherName("Sandstorm", "Sandstorm", "Tormenta de Arena", "Tempête de Sable");
            RegisterWeatherName("Hail", "Hail", "Granizo", "Grêle");
            RegisterWeatherName("Snow", "Snow", "Nieve", "Neige");
            RegisterWeatherName("Heavy Rain", "Heavy Rain", "Lluvia Intensa", "Pluie Intense");
            RegisterWeatherName("Extremely Harsh Sunlight", "Extremely Harsh Sunlight", "Sol Extremadamente Abrasador", "Soleil Extrêmement Brûlant");
            RegisterWeatherName("Strong Winds", "Strong Winds", "Vientos Fuertes", "Vents Violents");
            RegisterWeatherName("Fog", "Fog", "Niebla", "Brouillard");

            // Weather descriptions
            RegisterWeatherDescription("Rain", "Rain is falling. Water moves are boosted and Fire moves are weakened.",
                "Está lloviendo. Los movimientos de Agua se potencian y los de Fuego se debilitan.",
                "Il pleut. Les capacités Eau sont renforcées et les capacités Feu sont affaiblies.");
            RegisterWeatherDescription("Harsh Sunlight", "The sunlight is harsh. Fire moves are boosted and Water moves are weakened.",
                "El sol es abrasador. Los movimientos de Fuego se potencian y los de Agua se debilitan.",
                "Le soleil est brûlant. Les capacités Feu sont renforcées et les capacités Eau sont affaiblies.");
            RegisterWeatherDescription("Sandstorm", "A sandstorm is raging. Non-Rock/Ground/Steel types take damage each turn.",
                "Una tormenta de arena está arrasando. Los tipos que no sean Roca/Tierra/Acero reciben daño cada turno.",
                "Une tempête de sable fait rage. Les types non Roche/Sol/Acier subissent des dégâts à chaque tour.");
            RegisterWeatherDescription("Hail", "It is hailing. Non-Ice types take damage each turn.",
                "Está granizando. Los tipos que no sean Hielo reciben daño cada turno.",
                "Il grêle. Les types non Glace subissent des dégâts à chaque tour.");
            RegisterWeatherDescription("Snow", "It is snowing. Ice-type Defense is boosted.",
                "Está nevando. La Defensa de los tipos Hielo se potencia.",
                "Il neige. La Défense des types Glace est renforcée.");
            RegisterWeatherDescription("Heavy Rain", "Heavy rain is falling! Fire-type moves are completely nullified.",
                "¡Está cayendo una lluvia intensa! Los movimientos de tipo Fuego son completamente anulados.",
                "Une pluie intense tombe ! Les capacités de type Feu sont complètement annulées.");
            RegisterWeatherDescription("Extremely Harsh Sunlight", "The sunlight is extremely harsh! Water-type moves are completely nullified.",
                "¡El sol es extremadamente abrasador! Los movimientos de tipo Agua son completamente anulados.",
                "Le soleil est extrêmement brûlant ! Les capacités de type Eau sont complètement annulées.");
            RegisterWeatherDescription("Strong Winds", "Mysterious strong winds are blowing! Flying-type weaknesses are reduced.",
                "¡Soplan vientos fuertes misteriosos! Las debilidades de tipo Volador se reducen.",
                "Des vents violents mystérieux soufflent ! Les faiblesses de type Vol sont réduites.");
            RegisterWeatherDescription("Fog", "The fog is deep. Move accuracy is reduced.",
                "La niebla es espesa. La precisión de los movimientos se reduce.",
                "Le brouillard est épais. La précision des capacités est réduite.");
        }
    }
}
