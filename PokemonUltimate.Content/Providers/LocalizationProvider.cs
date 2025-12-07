using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Default implementation of ILocalizationProvider.
    /// Uses LocalizationDataProvider for data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public class LocalizationProvider : ILocalizationProvider
    {
        private string _currentLanguage = "en";

        /// <summary>
        /// Gets or sets the current language code (e.g., "en", "es", "fr").
        /// </summary>
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(ErrorMessages.LanguageCodeCannotBeEmpty, nameof(value));
                _currentLanguage = value;
            }
        }

        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="args">Optional arguments for string formatting.</param>
        /// <returns>The localized string, or the key if not found.</returns>
        public string GetString(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            var data = LocalizationDataProvider.GetData(key);
            if (data == null)
                return key; // Return key if not found (fail-safe)

            if (!data.Translations.TryGetValue(_currentLanguage, out var translation))
            {
                // Fallback to English if current language not available
                if (!data.Translations.TryGetValue("en", out translation))
                    return key; // Return key if English not available
            }

            if (args == null || args.Length == 0)
                return translation;

            try
            {
                return string.Format(translation, args);
            }
            catch (FormatException)
            {
                // Return unformatted string if formatting fails
                return translation;
            }
        }

        /// <summary>
        /// Checks if a localization key exists.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        public bool HasString(string key)
        {
            return LocalizationDataProvider.HasData(key);
        }

        /// <summary>
        /// Gets all available language codes.
        /// </summary>
        /// <returns>Collection of available language codes.</returns>
        public IEnumerable<string> GetAvailableLanguages()
        {
            var languages = new HashSet<string>();
            // Collect all languages from all registered data
            foreach (var data in LocalizationDataProvider.GetAllData())
            {
                foreach (var lang in data.Translations.Keys)
                {
                    languages.Add(lang);
                }
            }
            return languages;
        }
    }
}
