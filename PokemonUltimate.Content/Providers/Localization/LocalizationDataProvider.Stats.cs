using System.Collections.Generic;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for stat translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeStats()
        {
            Register(LocalizationKey.Stat_HP, new Dictionary<string, string>
            {
                { "en", "HP" },
                { "es", "PS" },
                { "fr", "PV" }
            });

            Register(LocalizationKey.Stat_Attack, new Dictionary<string, string>
            {
                { "en", "Attack" },
                { "es", "Ataque" },
                { "fr", "Attaque" }
            });

            Register(LocalizationKey.Stat_Defense, new Dictionary<string, string>
            {
                { "en", "Defense" },
                { "es", "Defensa" },
                { "fr", "Défense" }
            });

            Register(LocalizationKey.Stat_SpAttack, new Dictionary<string, string>
            {
                { "en", "Sp. Atk" },
                { "es", "At. Esp." },
                { "fr", "Atq. Spé." }
            });

            Register(LocalizationKey.Stat_SpDefense, new Dictionary<string, string>
            {
                { "en", "Sp. Def" },
                { "es", "Def. Esp." },
                { "fr", "Déf. Spé." }
            });

            Register(LocalizationKey.Stat_Speed, new Dictionary<string, string>
            {
                { "en", "Speed" },
                { "es", "Velocidad" },
                { "fr", "Vitesse" }
            });

            Register(LocalizationKey.Stat_Accuracy, new Dictionary<string, string>
            {
                { "en", "Accuracy" },
                { "es", "Precisión" },
                { "fr", "Précision" }
            });

            Register(LocalizationKey.Stat_Evasion, new Dictionary<string, string>
            {
                { "en", "Evasion" },
                { "es", "Evasión" },
                { "fr", "Esquive" }
            });
        }
    }
}
