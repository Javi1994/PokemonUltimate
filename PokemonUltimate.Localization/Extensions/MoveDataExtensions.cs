using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Localization.Extensions
{
    /// <summary>
    /// Extension methods for MoveData localization.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/CONTENT_LOCALIZATION_DESIGN.md`
    /// **Note**: These extension methods are in the Localization project to avoid circular dependencies.
    /// </remarks>
    public static class MoveDataExtensions
    {
        /// <summary>
        /// Gets the localized display name for this move.
        /// Falls back to the original name if translation not found.
        /// </summary>
        /// <param name="move">The move data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized name, or the original name if translation not available.</returns>
        public static string GetDisplayName(this MoveData move, ILocalizationProvider localizationProvider)
        {
            if (move == null)
                return string.Empty;

            if (localizationProvider == null)
                return move.Name;

            var key = GenerateMoveNameKey(move.Name);
            var translatedName = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original name
            if (translatedName == key)
                return move.Name;

            return translatedName;
        }

        /// <summary>
        /// Gets the localized description for this move.
        /// Falls back to the original description if translation not found.
        /// </summary>
        /// <param name="move">The move data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized description, or the original description if translation not available.</returns>
        public static string GetDescription(this MoveData move, ILocalizationProvider localizationProvider)
        {
            if (move == null)
                return string.Empty;

            if (localizationProvider == null)
                return move.Description;

            var key = GenerateMoveDescriptionKey(move.Name);
            var translatedDescription = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original description
            if (translatedDescription == key)
                return move.Description;

            return translatedDescription;
        }

        /// <summary>
        /// Generates a localization key for a move name.
        /// Format: "move_name_{moveName}" (lowercase, spaces replaced with underscores).
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

        /// <summary>
        /// Generates a localization key for a move description.
        /// Format: "move_description_{moveName}" (lowercase, spaces replaced with underscores).
        /// </summary>
        private static string GenerateMoveDescriptionKey(string moveName)
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

            return $"move_description_{normalized}";
        }
    }
}
