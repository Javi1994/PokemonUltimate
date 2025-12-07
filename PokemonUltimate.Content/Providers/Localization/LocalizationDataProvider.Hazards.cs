using System.Collections.Generic;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for hazard name and description translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeHazardNames()
        {
            // Hazard names
            RegisterHazardName("Stealth Rock", "Stealth Rock", "Trampa Rocas", "Piège de Roches");
            RegisterHazardName("Spikes", "Spikes", "Púas", "Picots");
            RegisterHazardName("Toxic Spikes", "Toxic Spikes", "Púas Tóxicas", "Picots Toxiques");
            RegisterHazardName("Sticky Web", "Sticky Web", "Telaraña Pegajosa", "Toile Gluante");

            // Hazard descriptions
            RegisterHazardDescription("Stealth Rock", "Pointed stones float around the opposing team, damaging Pokemon that switch in.",
                "Piedras afiladas flotan alrededor del equipo rival, dañando a los Pokémon que entran.",
                "Des pierres pointues flottent autour de l'équipe adverse, blessant les Pokémon qui entrent.");
            RegisterHazardDescription("Spikes", "Sharp spikes hurt Pokemon that switch in.",
                "Púas afiladas lastiman a los Pokémon que entran.",
                "Des picots acérés blessent les Pokémon qui entrent.");
            RegisterHazardDescription("Toxic Spikes", "Poison spikes that poison Pokemon that switch in.",
                "Púas venenosas que envenenan a los Pokémon que entran.",
                "Des picots empoisonnés qui empoisonnent les Pokémon qui entrent.");
            RegisterHazardDescription("Sticky Web", "A sticky web that lowers the Speed of Pokemon that switch in.",
                "Una telaraña pegajosa que reduce la Velocidad de los Pokémon que entran.",
                "Une toile gluante qui réduit la Vitesse des Pokémon qui entrent.");

            // Hazard damage/effect messages (using {0} for Pokemon name, {1} for hazard name)
            Register(LocalizationKey.HazardSpikesDamage, new Dictionary<string, string>
            {
                { "en", "{0} is hurt by {1}!" },
                { "es", "¡{0} se lastimó por {1}!" },
                { "fr", "{0} est blessé par {1}!" }
            });

            Register(LocalizationKey.HazardStealthRockDamage, new Dictionary<string, string>
            {
                { "en", "{0} is hurt by {1}!" },
                { "es", "¡{0} se lastimó por {1}!" },
                { "fr", "{0} est blessé par {1}!" }
            });

            Register(LocalizationKey.HazardToxicSpikesAbsorbed, new Dictionary<string, string>
            {
                { "en", "{0} absorbed the {1}!" },
                { "es", "¡{0} absorbió las {1}!" },
                { "fr", "{0} a absorbé les {1}!" }
            });

            Register(LocalizationKey.HazardToxicSpikesStatus, new Dictionary<string, string>
            {
                { "en", "{0} was poisoned by {1}!" },
                { "es", "¡{0} fue envenenado por {1}!" },
                { "fr", "{0} a été empoisonné par {1}!" }
            });

            Register(LocalizationKey.HazardStickyWebSpeed, new Dictionary<string, string>
            {
                { "en", "{0} was caught in a {1}!" },
                { "es", "¡{0} quedó atrapado en una {1}!" },
                { "fr", "{0} a été pris dans une {1}!" }
            });
        }
    }
}
