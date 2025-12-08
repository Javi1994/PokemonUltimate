using System.Collections.Generic;
using PokemonUltimate.Localization.Constants;

namespace PokemonUltimate.Localization.Providers
{
    /// <summary>
    /// Partial class for battle message translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeBattleMessages()
        {
            Register(LocalizationKey.BattleUsedMove, new Dictionary<string, string>
            {
                { "en", "{0} used {1}!" },
                { "es", "¡{0} usó {1}!" },
                { "fr", "{0} a utilisé {1}!" }
            });

            Register(LocalizationKey.BattleMissed, new Dictionary<string, string>
            {
                { "en", "The attack missed!" },
                { "es", "¡El ataque falló!" },
                { "fr", "L'attaque a échoué!" }
            });

            Register(LocalizationKey.BattleFlinched, new Dictionary<string, string>
            {
                { "en", "{0} flinched and couldn't move!" },
                { "es", "¡{0} retrocedió y no pudo moverse!" },
                { "fr", "{0} a reculé et n'a pas pu bouger!" }
            });

            Register(LocalizationKey.BattleProtected, new Dictionary<string, string>
            {
                { "en", "{0} protected itself!" },
                { "es", "¡{0} se protegió!" },
                { "fr", "{0} s'est protégé!" }
            });

            Register(LocalizationKey.BattleNoPP, new Dictionary<string, string>
            {
                { "en", "{0} has no PP left!" },
                { "es", "¡{0} no tiene más PP!" },
                { "fr", "{0} n'a plus de PP!" }
            });

            Register(LocalizationKey.BattleAsleep, new Dictionary<string, string>
            {
                { "en", "{0} is fast asleep." },
                { "es", "{0} está profundamente dormido." },
                { "fr", "{0} dort profondément." }
            });

            Register(LocalizationKey.BattleFrozen, new Dictionary<string, string>
            {
                { "en", "{0} is frozen solid!" },
                { "es", "¡{0} está congelado!" },
                { "fr", "{0} est complètement gelé!" }
            });

            Register(LocalizationKey.BattleParalyzed, new Dictionary<string, string>
            {
                { "en", "{0} is paralyzed! It can't move!" },
                { "es", "¡{0} está paralizado! ¡No puede moverse!" },
                { "fr", "{0} est paralysé! Il ne peut pas bouger!" }
            });

            Register(LocalizationKey.BattleSwitchingOut, new Dictionary<string, string>
            {
                { "en", "{0} is switching out!" },
                { "es", "¡{0} está cambiando!" },
                { "fr", "{0} change!" }
            });

            Register(LocalizationKey.MoveProtectFailed, new Dictionary<string, string>
            {
                { "en", "{0} avoided the attack!" },
                { "es", "¡{0} evitó el ataque!" },
                { "fr", "{0} a évité l'attaque!" }
            });

            Register(LocalizationKey.MoveCountered, new Dictionary<string, string>
            {
                { "en", "{0} countered the attack!" },
                { "es", "¡{0} contraatacó!" },
                { "fr", "{0} a contre-attaqué!" }
            });

            Register(LocalizationKey.MoveFocusing, new Dictionary<string, string>
            {
                { "en", "{0} is tightening its focus!" },
                { "es", "¡{0} está concentrándose!" },
                { "fr", "{0} se concentre!" }
            });

            Register(LocalizationKey.MoveFocusLost, new Dictionary<string, string>
            {
                { "en", "{0} lost its focus!" },
                { "es", "¡{0} perdió la concentración!" },
                { "fr", "{0} a perdu sa concentration!" }
            });

            Register(LocalizationKey.MoveSemiInvulnerable, new Dictionary<string, string>
            {
                { "en", "{0} {1}!" },
                { "es", "¡{0} {1}!" },
                { "fr", "{0} {1}!" }
            });

            Register(LocalizationKey.HitsExactly, new Dictionary<string, string>
            {
                { "en", "Hits {0} times." },
                { "es", "Golpea {0} veces." },
                { "fr", "Touche {0} fois." }
            });

            Register(LocalizationKey.HitsRange, new Dictionary<string, string>
            {
                { "en", "Hits {0}-{1} times." },
                { "es", "Golpea {0}-{1} veces." },
                { "fr", "Touche {0}-{1} fois." }
            });

            Register(LocalizationKey.MoveFailed, new Dictionary<string, string>
            {
                { "en", "{0} can't use {1}!" },
                { "es", "¡{0} no puede usar {1}!" },
                { "fr", "{0} ne peut pas utiliser {1}!" }
            });
        }
    }
}
