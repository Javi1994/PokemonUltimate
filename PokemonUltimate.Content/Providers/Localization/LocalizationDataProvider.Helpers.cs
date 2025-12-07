using System.Collections.Generic;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Partial class for helper methods used in localization data registration.
    /// Contains key generation and registration helper methods.
    /// </summary>
    public static partial class LocalizationDataProvider
    {
        #region Move Helpers

        /// <summary>
        /// Helper method to register move name translations.
        /// </summary>
        private static void RegisterMoveName(string moveName, string english, string spanish, string french)
        {
            var key = GenerateMoveNameKey(moveName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        /// <summary>
        /// Generates a localization key for a move name.
        /// </summary>
        private static string GenerateMoveNameKey(string moveName)
        {
            if (string.IsNullOrEmpty(moveName))
                return string.Empty;

            var normalized = moveName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"move_name_{normalized}";
        }

        #endregion

        #region Pokemon Helpers

        /// <summary>
        /// Helper method to register Pokemon name translations.
        /// </summary>
        private static void RegisterPokemonName(string pokemonName, string english, string spanish, string french)
        {
            var key = GeneratePokemonNameKey(pokemonName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        /// <summary>
        /// Generates a localization key for a Pokemon name.
        /// </summary>
        private static string GeneratePokemonNameKey(string pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return string.Empty;

            var normalized = pokemonName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"pokemon_name_{normalized}";
        }

        #endregion

        #region Ability Helpers

        /// <summary>
        /// Helper method to register ability name translations.
        /// </summary>
        private static void RegisterAbilityName(string abilityName, string english, string spanish, string french)
        {
            var key = GenerateAbilityNameKey(abilityName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        /// <summary>
        /// Generates a localization key for an ability name.
        /// </summary>
        private static string GenerateAbilityNameKey(string abilityName)
        {
            if (string.IsNullOrEmpty(abilityName))
                return string.Empty;

            var normalized = abilityName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"ability_name_{normalized}";
        }

        #endregion

        #region Item Helpers

        /// <summary>
        /// Helper method to register item name translations.
        /// </summary>
        private static void RegisterItemName(string itemName, string english, string spanish, string french)
        {
            var key = GenerateItemNameKey(itemName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        /// <summary>
        /// Helper method to register item description translations.
        /// </summary>
        private static void RegisterItemDescription(string itemName, string english, string spanish, string french)
        {
            var key = GenerateItemDescriptionKey(itemName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        /// <summary>
        /// Generates a localization key for an item name.
        /// </summary>
        private static string GenerateItemNameKey(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return string.Empty;

            var normalized = itemName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"item_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for an item description.
        /// </summary>
        private static string GenerateItemDescriptionKey(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return string.Empty;

            var normalized = itemName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"item_description_{normalized}";
        }

        #endregion

        #region Weather Helpers

        private static void RegisterWeatherName(string weatherName, string english, string spanish, string french)
        {
            var key = GenerateWeatherNameKey(weatherName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static void RegisterWeatherDescription(string weatherName, string english, string spanish, string french)
        {
            var key = GenerateWeatherDescriptionKey(weatherName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static string GenerateWeatherNameKey(string weatherName)
        {
            if (string.IsNullOrEmpty(weatherName))
                return string.Empty;

            var normalized = weatherName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"weather_name_{normalized}";
        }

        private static string GenerateWeatherDescriptionKey(string weatherName)
        {
            if (string.IsNullOrEmpty(weatherName))
                return string.Empty;

            var normalized = weatherName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"weather_description_{normalized}";
        }

        #endregion

        #region Terrain Helpers

        private static void RegisterTerrainName(string terrainName, string english, string spanish, string french)
        {
            var key = GenerateTerrainNameKey(terrainName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static void RegisterTerrainDescription(string terrainName, string english, string spanish, string french)
        {
            var key = GenerateTerrainDescriptionKey(terrainName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static string GenerateTerrainNameKey(string terrainName)
        {
            if (string.IsNullOrEmpty(terrainName))
                return string.Empty;

            var normalized = terrainName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"terrain_name_{normalized}";
        }

        private static string GenerateTerrainDescriptionKey(string terrainName)
        {
            if (string.IsNullOrEmpty(terrainName))
                return string.Empty;

            var normalized = terrainName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"terrain_description_{normalized}";
        }

        #endregion

        #region Side Condition Helpers

        private static void RegisterSideConditionName(string sideConditionName, string english, string spanish, string french)
        {
            var key = GenerateSideConditionNameKey(sideConditionName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static void RegisterSideConditionDescription(string sideConditionName, string english, string spanish, string french)
        {
            var key = GenerateSideConditionDescriptionKey(sideConditionName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static string GenerateSideConditionNameKey(string sideConditionName)
        {
            if (string.IsNullOrEmpty(sideConditionName))
                return string.Empty;

            var normalized = sideConditionName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"side_condition_name_{normalized}";
        }

        private static string GenerateSideConditionDescriptionKey(string sideConditionName)
        {
            if (string.IsNullOrEmpty(sideConditionName))
                return string.Empty;

            var normalized = sideConditionName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"side_condition_description_{normalized}";
        }

        #endregion

        #region Field Effect Helpers

        private static void RegisterFieldEffectName(string fieldEffectName, string english, string spanish, string french)
        {
            var key = GenerateFieldEffectNameKey(fieldEffectName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static void RegisterFieldEffectDescription(string fieldEffectName, string english, string spanish, string french)
        {
            var key = GenerateFieldEffectDescriptionKey(fieldEffectName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static string GenerateFieldEffectNameKey(string fieldEffectName)
        {
            if (string.IsNullOrEmpty(fieldEffectName))
                return string.Empty;

            var normalized = fieldEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"field_effect_name_{normalized}";
        }

        private static string GenerateFieldEffectDescriptionKey(string fieldEffectName)
        {
            if (string.IsNullOrEmpty(fieldEffectName))
                return string.Empty;

            var normalized = fieldEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"field_effect_description_{normalized}";
        }

        #endregion

        #region Hazard Helpers

        private static void RegisterHazardName(string hazardName, string english, string spanish, string french)
        {
            var key = GenerateHazardNameKey(hazardName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static void RegisterHazardDescription(string hazardName, string english, string spanish, string french)
        {
            var key = GenerateHazardDescriptionKey(hazardName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static string GenerateHazardNameKey(string hazardName)
        {
            if (string.IsNullOrEmpty(hazardName))
                return string.Empty;

            var normalized = hazardName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"hazard_name_{normalized}";
        }

        private static string GenerateHazardDescriptionKey(string hazardName)
        {
            if (string.IsNullOrEmpty(hazardName))
                return string.Empty;

            var normalized = hazardName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"hazard_description_{normalized}";
        }

        #endregion

        #region Status Effect Helpers

        private static void RegisterStatusEffectName(string statusEffectName, string english, string spanish, string french)
        {
            var key = GenerateStatusEffectNameKey(statusEffectName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static void RegisterStatusEffectDescription(string statusEffectName, string english, string spanish, string french)
        {
            var key = GenerateStatusEffectDescriptionKey(statusEffectName);
            Register(key, new Dictionary<string, string>
            {
                { "en", english },
                { "es", spanish },
                { "fr", french }
            });
        }

        private static string GenerateStatusEffectNameKey(string statusEffectName)
        {
            if (string.IsNullOrEmpty(statusEffectName))
                return string.Empty;

            var normalized = statusEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"status_effect_name_{normalized}";
        }

        private static string GenerateStatusEffectDescriptionKey(string statusEffectName)
        {
            if (string.IsNullOrEmpty(statusEffectName))
                return string.Empty;

            var normalized = statusEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"status_effect_description_{normalized}";
        }

        #endregion
    }
}
