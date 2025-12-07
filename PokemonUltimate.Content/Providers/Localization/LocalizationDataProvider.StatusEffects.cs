using System.Collections.Generic;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for status effect translations (persistent and volatile).
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeStatusEffects()
        {
            Register(LocalizationKey.StatusBurnDamage, new Dictionary<string, string>
            {
                { "en", "{0} is hurt by its burn!" },
                { "es", "¡{0} se lastimó por su quemadura!" },
                { "fr", "{0} est blessé par sa brûlure!" }
            });

            InitializePersistentStatusNames();
            InitializeVolatileStatusNames();

            Register(LocalizationKey.StatusPoisonDamage, new Dictionary<string, string>
            {
                { "en", "{0} is hurt by poison!" },
                { "es", "¡{0} se lastimó por el veneno!" },
                { "fr", "{0} est blessé par le poison!" }
            });

            InitializePersistentStatusNames();
            InitializeVolatileStatusNames();
        }

        private static void InitializePersistentStatusNames()
        {
            Register(LocalizationKey.Status_None, new Dictionary<string, string>
            {
                { "en", "None" },
                { "es", "Ninguno" },
                { "fr", "Aucun" }
            });

            Register(LocalizationKey.Status_Burn, new Dictionary<string, string>
            {
                { "en", "Burn" },
                { "es", "Quemadura" },
                { "fr", "Brûlure" }
            });

            Register(LocalizationKey.Status_Paralysis, new Dictionary<string, string>
            {
                { "en", "Paralysis" },
                { "es", "Parálisis" },
                { "fr", "Paralysie" }
            });

            Register(LocalizationKey.Status_Sleep, new Dictionary<string, string>
            {
                { "en", "Sleep" },
                { "es", "Sueño" },
                { "fr", "Sommeil" }
            });

            Register(LocalizationKey.Status_Poison, new Dictionary<string, string>
            {
                { "en", "Poison" },
                { "es", "Veneno" },
                { "fr", "Poison" }
            });

            Register(LocalizationKey.Status_BadlyPoisoned, new Dictionary<string, string>
            {
                { "en", "Badly Poisoned" },
                { "es", "Envenenado Grave" },
                { "fr", "Gravement Empoisonné" }
            });

            Register(LocalizationKey.Status_Freeze, new Dictionary<string, string>
            {
                { "en", "Freeze" },
                { "es", "Congelación" },
                { "fr", "Gel" }
            });
        }

        private static void InitializeVolatileStatusNames()
        {
            Register(LocalizationKey.VolatileStatus_None, new Dictionary<string, string>
            {
                { "en", "None" },
                { "es", "Ninguno" },
                { "fr", "Aucun" }
            });

            Register(LocalizationKey.VolatileStatus_Confusion, new Dictionary<string, string>
            {
                { "en", "Confusion" },
                { "es", "Confusión" },
                { "fr", "Confusion" }
            });

            Register(LocalizationKey.VolatileStatus_Flinch, new Dictionary<string, string>
            {
                { "en", "Flinch" },
                { "es", "Retroceso" },
                { "fr", "Recul" }
            });

            Register(LocalizationKey.VolatileStatus_LeechSeed, new Dictionary<string, string>
            {
                { "en", "Leech Seed" },
                { "es", "Drenadoras" },
                { "fr", "Vampigraine" }
            });

            Register(LocalizationKey.VolatileStatus_Attract, new Dictionary<string, string>
            {
                { "en", "Attract" },
                { "es", "Atracción" },
                { "fr", "Attraction" }
            });

            Register(LocalizationKey.VolatileStatus_Curse, new Dictionary<string, string>
            {
                { "en", "Curse" },
                { "es", "Maldición" },
                { "fr", "Malédiction" }
            });

            Register(LocalizationKey.VolatileStatus_Encore, new Dictionary<string, string>
            {
                { "en", "Encore" },
                { "es", "Danza Amiga" },
                { "fr", "Encore" }
            });

            Register(LocalizationKey.VolatileStatus_Taunt, new Dictionary<string, string>
            {
                { "en", "Taunt" },
                { "es", "Mofa" },
                { "fr", "Provoc" }
            });

            Register(LocalizationKey.VolatileStatus_Torment, new Dictionary<string, string>
            {
                { "en", "Torment" },
                { "es", "Tormento" },
                { "fr", "Tourmente" }
            });

            Register(LocalizationKey.VolatileStatus_Disable, new Dictionary<string, string>
            {
                { "en", "Disable" },
                { "es", "Anulación" },
                { "fr", "Entrave" }
            });

            Register(LocalizationKey.VolatileStatus_SemiInvulnerable, new Dictionary<string, string>
            {
                { "en", "Semi-Invulnerable" },
                { "es", "Semi-Invulnerable" },
                { "fr", "Semi-Invulnérable" }
            });

            Register(LocalizationKey.VolatileStatus_Charging, new Dictionary<string, string>
            {
                { "en", "Charging" },
                { "es", "Cargando" },
                { "fr", "Charge" }
            });

            Register(LocalizationKey.VolatileStatus_Protected, new Dictionary<string, string>
            {
                { "en", "Protected" },
                { "es", "Protegido" },
                { "fr", "Protégé" }
            });

            Register(LocalizationKey.VolatileStatus_SwitchingOut, new Dictionary<string, string>
            {
                { "en", "Switching Out" },
                { "es", "Cambiando" },
                { "fr", "Changement" }
            });

            Register(LocalizationKey.VolatileStatus_Focusing, new Dictionary<string, string>
            {
                { "en", "Focusing" },
                { "es", "Concentrando" },
                { "fr", "Concentration" }
            });

            Register(LocalizationKey.VolatileStatus_FollowMe, new Dictionary<string, string>
            {
                { "en", "Follow Me" },
                { "es", "Señuelo" },
                { "fr", "Ralliement" }
            });

            Register(LocalizationKey.VolatileStatus_RagePowder, new Dictionary<string, string>
            {
                { "en", "Rage Powder" },
                { "es", "Polvo Ira" },
                { "fr", "Poudre Fureur" }
            });
        }
    }
}
