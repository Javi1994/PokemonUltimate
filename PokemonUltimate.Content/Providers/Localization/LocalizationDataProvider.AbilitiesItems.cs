using System.Collections.Generic;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for ability and item activation message translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeAbilitiesItems()
        {
            Register(LocalizationKey.AbilityActivated, new Dictionary<string, string>
            {
                { "en", "{0}'s {1}!" },
                { "es", "¡{0} de {1}!" },
                { "fr", "{0} de {1}!" }
            });

            Register(LocalizationKey.ItemActivated, new Dictionary<string, string>
            {
                { "en", "{0}'s {1}!" },
                { "es", "¡{0} de {1}!" },
                { "fr", "{0} de {1}!" }
            });

            Register(LocalizationKey.TruantLoafing, new Dictionary<string, string>
            {
                { "en", "{0} is loafing around!" },
                { "es", "¡{0} está holgazaneando!" },
                { "fr", "{0} flâne!" }
            });

            Register(LocalizationKey.HurtByItem, new Dictionary<string, string>
            {
                { "en", "{0} was hurt by {1}!" },
                { "es", "¡{0} se lastimó por {1}!" },
                { "fr", "{0} a été blessé par {1}!" }
            });

            Register(LocalizationKey.HurtByRecoil, new Dictionary<string, string>
            {
                { "en", "{0} was hurt by recoil!" },
                { "es", "¡{0} se lastimó por el retroceso!" },
                { "fr", "{0} a été blessé par le recul!" }
            });

            Register(LocalizationKey.HurtByContact, new Dictionary<string, string>
            {
                { "en", "{0} was hurt by {1}!" },
                { "es", "¡{0} se lastimó por {1}!" },
                { "fr", "{0} a été blessé par {1}!" }
            });

            Register(LocalizationKey.HeldOnUsingItem, new Dictionary<string, string>
            {
                { "en", "{0} held on using its {1}!" },
                { "es", "¡{0} aguantó usando su {1}!" },
                { "fr", "{0} a tenu bon grâce à son {1}!" }
            });

            Register(LocalizationKey.EnduredHit, new Dictionary<string, string>
            {
                { "en", "{0} endured the hit!" },
                { "es", "¡{0} aguantó el golpe!" },
                { "fr", "{0} a enduré le coup!" }
            });
        }
    }
}
