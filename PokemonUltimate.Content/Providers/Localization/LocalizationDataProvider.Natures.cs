using System.Collections.Generic;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for nature translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeNatures()
        {
            Register(LocalizationKey.Nature_Hardy, new Dictionary<string, string>
            {
                { "en", "Hardy" },
                { "es", "Fuerte" },
                { "fr", "Hardy" }
            });

            Register(LocalizationKey.Nature_Docile, new Dictionary<string, string>
            {
                { "en", "Docile" },
                { "es", "Dócil" },
                { "fr", "Docile" }
            });

            Register(LocalizationKey.Nature_Serious, new Dictionary<string, string>
            {
                { "en", "Serious" },
                { "es", "Serio" },
                { "fr", "Sérieux" }
            });

            Register(LocalizationKey.Nature_Bashful, new Dictionary<string, string>
            {
                { "en", "Bashful" },
                { "es", "Tímido" },
                { "fr", "Timide" }
            });

            Register(LocalizationKey.Nature_Quirky, new Dictionary<string, string>
            {
                { "en", "Quirky" },
                { "es", "Raro" },
                { "fr", "Bizarre" }
            });

            Register(LocalizationKey.Nature_Lonely, new Dictionary<string, string>
            {
                { "en", "Lonely" },
                { "es", "Huraño" },
                { "fr", "Solo" }
            });

            Register(LocalizationKey.Nature_Brave, new Dictionary<string, string>
            {
                { "en", "Brave" },
                { "es", "Audaz" },
                { "fr", "Brave" }
            });

            Register(LocalizationKey.Nature_Adamant, new Dictionary<string, string>
            {
                { "en", "Adamant" },
                { "es", "Firme" },
                { "fr", "Rigide" }
            });

            Register(LocalizationKey.Nature_Naughty, new Dictionary<string, string>
            {
                { "en", "Naughty" },
                { "es", "Pícaro" },
                { "fr", "Mauvais" }
            });

            Register(LocalizationKey.Nature_Bold, new Dictionary<string, string>
            {
                { "en", "Bold" },
                { "es", "Osado" },
                { "fr", "Assuré" }
            });

            Register(LocalizationKey.Nature_Relaxed, new Dictionary<string, string>
            {
                { "en", "Relaxed" },
                { "es", "Plácido" },
                { "fr", "Relâché" }
            });

            Register(LocalizationKey.Nature_Impish, new Dictionary<string, string>
            {
                { "en", "Impish" },
                { "es", "Agitado" },
                { "fr", "Malin" }
            });

            Register(LocalizationKey.Nature_Lax, new Dictionary<string, string>
            {
                { "en", "Lax" },
                { "es", "Flojo" },
                { "fr", "Lâche" }
            });

            Register(LocalizationKey.Nature_Timid, new Dictionary<string, string>
            {
                { "en", "Timid" },
                { "es", "Miedoso" },
                { "fr", "Timide" }
            });

            Register(LocalizationKey.Nature_Hasty, new Dictionary<string, string>
            {
                { "en", "Hasty" },
                { "es", "Activo" },
                { "fr", "Pressé" }
            });

            Register(LocalizationKey.Nature_Jolly, new Dictionary<string, string>
            {
                { "en", "Jolly" },
                { "es", "Alegre" },
                { "fr", "Jovial" }
            });

            Register(LocalizationKey.Nature_Naive, new Dictionary<string, string>
            {
                { "en", "Naive" },
                { "es", "Ingenuo" },
                { "fr", "Naïf" }
            });

            Register(LocalizationKey.Nature_Modest, new Dictionary<string, string>
            {
                { "en", "Modest" },
                { "es", "Modesto" },
                { "fr", "Modeste" }
            });

            Register(LocalizationKey.Nature_Mild, new Dictionary<string, string>
            {
                { "en", "Mild" },
                { "es", "Afable" },
                { "fr", "Gentil" }
            });

            Register(LocalizationKey.Nature_Quiet, new Dictionary<string, string>
            {
                { "en", "Quiet" },
                { "es", "Manso" },
                { "fr", "Discret" }
            });

            Register(LocalizationKey.Nature_Rash, new Dictionary<string, string>
            {
                { "en", "Rash" },
                { "es", "Alocado" },
                { "fr", "Foufou" }
            });

            Register(LocalizationKey.Nature_Calm, new Dictionary<string, string>
            {
                { "en", "Calm" },
                { "es", "Calmado" },
                { "fr", "Calme" }
            });

            Register(LocalizationKey.Nature_Gentle, new Dictionary<string, string>
            {
                { "en", "Gentle" },
                { "es", "Amable" },
                { "fr", "Gentil" }
            });

            Register(LocalizationKey.Nature_Sassy, new Dictionary<string, string>
            {
                { "en", "Sassy" },
                { "es", "Grosero" },
                { "fr", "Malpoli" }
            });

            Register(LocalizationKey.Nature_Careful, new Dictionary<string, string>
            {
                { "en", "Careful" },
                { "es", "Cauto" },
                { "fr", "Prudent" }
            });
        }
    }
}
