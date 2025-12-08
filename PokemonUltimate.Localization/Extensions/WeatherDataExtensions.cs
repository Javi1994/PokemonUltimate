using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Localization.Extensions
{
    /// <summary>
    /// Extension methods for WeatherData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class WeatherDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this weather.
        /// </summary>
        /// <param name="weather">The weather data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetDisplayName(this WeatherData weather, ILocalizationProvider localizationProvider = null)
        {
            if (weather == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateWeatherNameKey(weather.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return weather.Name;
        }

        /// <summary>
        /// Gets the localized description for this weather.
        /// </summary>
        /// <param name="weather">The weather data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetDescription(this WeatherData weather, ILocalizationProvider localizationProvider = null)
        {
            if (weather == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateWeatherDescriptionKey(weather.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return weather.Description;
        }

        /// <summary>
        /// Generates a localization key for a weather name.
        /// </summary>
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

        /// <summary>
        /// Generates a localization key for a weather description.
        /// </summary>
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
    }
}
