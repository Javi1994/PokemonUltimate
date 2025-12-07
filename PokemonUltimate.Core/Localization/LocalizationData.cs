using System.Collections.Generic;

namespace PokemonUltimate.Core.Localization
{
    /// <summary>
    /// Data structure for storing translations for a single key.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public class LocalizationData
    {
        /// <summary>
        /// The localization key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Dictionary mapping language codes to translated strings.
        /// </summary>
        public Dictionary<string, string> Translations { get; set; }

        /// <summary>
        /// Initializes a new instance of LocalizationData with empty translations.
        /// </summary>
        public LocalizationData()
        {
            Translations = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of LocalizationData with the specified key and translations.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="translations">Dictionary of language codes to translated strings.</param>
        public LocalizationData(string key, Dictionary<string, string> translations)
        {
            Key = key;
            Translations = translations ?? new Dictionary<string, string>();
        }
    }
}
