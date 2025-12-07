using System.Collections.Generic;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for Pokemon type and type effectiveness translations.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        private static void InitializeTypeEffectiveness()
        {
            Register(LocalizationKey.TypeNoEffect, new Dictionary<string, string>
            {
                { "en", "It has no effect..." },
                { "es", "No tuvo efecto..." },
                { "fr", "Cela n'a aucun effet..." }
            });

            InitializePokemonTypes();

            Register(LocalizationKey.TypeSuperEffective, new Dictionary<string, string>
            {
                { "en", "It's super effective!" },
                { "es", "¡Es súper efectivo!" },
                { "fr", "C'est super efficace!" }
            });

            Register(LocalizationKey.TypeSuperEffective4x, new Dictionary<string, string>
            {
                { "en", "It's super effective! (4x)" },
                { "es", "¡Es súper efectivo! (4x)" },
                { "fr", "C'est super efficace! (4x)" }
            });

            Register(LocalizationKey.TypeNotVeryEffective, new Dictionary<string, string>
            {
                { "en", "It's not very effective..." },
                { "es", "No es muy efectivo..." },
                { "fr", "Ce n'est pas très efficace..." }
            });

            Register(LocalizationKey.TypeNotVeryEffective025x, new Dictionary<string, string>
            {
                { "en", "It's not very effective... (0.25x)" },
                { "es", "No es muy efectivo... (0.25x)" },
                { "fr", "Ce n'est pas très efficace... (0.25x)" }
            });
        }

        private static void InitializePokemonTypes()
        {
            Register(LocalizationKey.Type_Normal, new Dictionary<string, string>
            {
                { "en", "Normal" },
                { "es", "Normal" },
                { "fr", "Normal" }
            });

            Register(LocalizationKey.Type_Fire, new Dictionary<string, string>
            {
                { "en", "Fire" },
                { "es", "Fuego" },
                { "fr", "Feu" }
            });

            Register(LocalizationKey.Type_Water, new Dictionary<string, string>
            {
                { "en", "Water" },
                { "es", "Agua" },
                { "fr", "Eau" }
            });

            Register(LocalizationKey.Type_Grass, new Dictionary<string, string>
            {
                { "en", "Grass" },
                { "es", "Planta" },
                { "fr", "Plante" }
            });

            Register(LocalizationKey.Type_Electric, new Dictionary<string, string>
            {
                { "en", "Electric" },
                { "es", "Eléctrico" },
                { "fr", "Électrik" }
            });

            Register(LocalizationKey.Type_Ice, new Dictionary<string, string>
            {
                { "en", "Ice" },
                { "es", "Hielo" },
                { "fr", "Glace" }
            });

            Register(LocalizationKey.Type_Fighting, new Dictionary<string, string>
            {
                { "en", "Fighting" },
                { "es", "Lucha" },
                { "fr", "Combat" }
            });

            Register(LocalizationKey.Type_Poison, new Dictionary<string, string>
            {
                { "en", "Poison" },
                { "es", "Veneno" },
                { "fr", "Poison" }
            });

            Register(LocalizationKey.Type_Ground, new Dictionary<string, string>
            {
                { "en", "Ground" },
                { "es", "Tierra" },
                { "fr", "Sol" }
            });

            Register(LocalizationKey.Type_Flying, new Dictionary<string, string>
            {
                { "en", "Flying" },
                { "es", "Volador" },
                { "fr", "Vol" }
            });

            Register(LocalizationKey.Type_Psychic, new Dictionary<string, string>
            {
                { "en", "Psychic" },
                { "es", "Psíquico" },
                { "fr", "Psy" }
            });

            Register(LocalizationKey.Type_Bug, new Dictionary<string, string>
            {
                { "en", "Bug" },
                { "es", "Bicho" },
                { "fr", "Insecte" }
            });

            Register(LocalizationKey.Type_Rock, new Dictionary<string, string>
            {
                { "en", "Rock" },
                { "es", "Roca" },
                { "fr", "Roche" }
            });

            Register(LocalizationKey.Type_Ghost, new Dictionary<string, string>
            {
                { "en", "Ghost" },
                { "es", "Fantasma" },
                { "fr", "Spectre" }
            });

            Register(LocalizationKey.Type_Dragon, new Dictionary<string, string>
            {
                { "en", "Dragon" },
                { "es", "Dragón" },
                { "fr", "Dragon" }
            });

            Register(LocalizationKey.Type_Dark, new Dictionary<string, string>
            {
                { "en", "Dark" },
                { "es", "Siniestro" },
                { "fr", "Ténèbres" }
            });

            Register(LocalizationKey.Type_Steel, new Dictionary<string, string>
            {
                { "en", "Steel" },
                { "es", "Acero" },
                { "fr", "Acier" }
            });

            Register(LocalizationKey.Type_Fairy, new Dictionary<string, string>
            {
                { "en", "Fairy" },
                { "es", "Hada" },
                { "fr", "Fée" }
            });
        }
    }
}
