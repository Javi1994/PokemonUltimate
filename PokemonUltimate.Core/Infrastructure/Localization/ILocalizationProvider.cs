using System.Collections.Generic;

namespace PokemonUltimate.Core.Infrastructure.Localization
{
    /// <summary>
    /// Interface for providing localized strings.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="args">Optional arguments for string formatting.</param>
        /// <returns>The localized string, or the key if not found.</returns>
        string GetString(string key, params object[] args);

        /// <summary>
        /// Checks if a localization key exists.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        bool HasString(string key);

        /// <summary>
        /// Gets or sets the current language code (e.g., "en", "es", "fr").
        /// </summary>
        string CurrentLanguage { get; set; }

        /// <summary>
        /// Gets all available language codes.
        /// </summary>
        /// <returns>Collection of available language codes.</returns>
        IEnumerable<string> GetAvailableLanguages();
    }
}
