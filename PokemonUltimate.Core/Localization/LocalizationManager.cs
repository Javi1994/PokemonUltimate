using System;

namespace PokemonUltimate.Core.Localization
{
    /// <summary>
    /// Static singleton manager for localization across all projects.
    /// Provides centralized access to ILocalizationProvider.
    ///
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/HOW_TO_USE.md`
    ///
    /// **Note**: This class requires an ILocalizationProvider instance to be set via SetProvider().
    /// Projects should call Initialize() with a concrete provider implementation.
    /// </summary>
    public static class LocalizationManager
    {
        private static ILocalizationProvider _instance;
        private static readonly object _lock = new object();
        private static string _defaultLanguage = "en";

        /// <summary>
        /// Gets or sets the default language code for new instances.
        /// </summary>
        public static string DefaultLanguage
        {
            get => _defaultLanguage;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Language code cannot be null or empty", nameof(value));
                _defaultLanguage = value;
            }
        }

        /// <summary>
        /// Gets the singleton localization provider instance.
        /// Throws InvalidOperationException if not initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">If provider has not been initialized.</exception>
        public static ILocalizationProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(
                        "LocalizationManager has not been initialized. Call LocalizationManager.Initialize(provider) first.");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initializes the localization manager with a provider instance.
        /// </summary>
        /// <param name="provider">The localization provider instance. Cannot be null.</param>
        /// <param name="languageCode">Optional language code (defaults to "en").</param>
        /// <exception cref="ArgumentNullException">If provider is null.</exception>
        public static void Initialize(ILocalizationProvider provider, string languageCode = "en")
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (_lock)
            {
                _instance = provider;
                _instance.CurrentLanguage = languageCode;
                _defaultLanguage = languageCode;
            }
        }

        /// <summary>
        /// Sets the provider instance (for advanced usage).
        /// </summary>
        /// <param name="provider">The localization provider instance. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If provider is null.</exception>
        public static void SetProvider(ILocalizationProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (_lock)
            {
                _instance = provider;
            }
        }

        /// <summary>
        /// Sets the current language for the singleton instance.
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "en", "es", "fr")</param>
        public static void SetLanguage(string languageCode)
        {
            Instance.CurrentLanguage = languageCode;
        }

        /// <summary>
        /// Sets language to Spanish.
        /// </summary>
        public static void SetSpanish()
        {
            SetLanguage("es");
        }

        /// <summary>
        /// Sets language to English.
        /// </summary>
        public static void SetEnglish()
        {
            SetLanguage("en");
        }

        /// <summary>
        /// Sets language to French.
        /// </summary>
        public static void SetFrench()
        {
            SetLanguage("fr");
        }

        /// <summary>
        /// Gets the current language code.
        /// </summary>
        public static string CurrentLanguage => Instance.CurrentLanguage;

        /// <summary>
        /// Resets the singleton instance (useful for testing).
        /// </summary>
        internal static void Reset()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }
    }
}
