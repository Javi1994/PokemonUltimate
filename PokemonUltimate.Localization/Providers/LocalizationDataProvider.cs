using System;
using System.Collections.Generic;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Data;

namespace PokemonUltimate.Localization.Providers
{
    /// <summary>
    /// Provides localization data for all game text.
    /// Centralized data source for translations.
    /// Similar to PokedexDataProvider pattern.
    ///
    /// This is a partial class split across multiple files:
    /// - LocalizationDataProvider.cs: Core class with public API
    /// - LocalizationDataProvider.BattleMessages.cs: Battle message translations
    /// - LocalizationDataProvider.Moves.cs: Move name translations
    /// - LocalizationDataProvider.Pokemon.cs: Pokemon name translations
    /// - LocalizationDataProvider.Abilities.cs: Ability name translations
    /// - LocalizationDataProvider.Items.cs: Item name and description translations
    /// - LocalizationDataProvider.Weather.cs: Weather name and description translations
    /// - LocalizationDataProvider.Terrain.cs: Terrain name and description translations
    /// - LocalizationDataProvider.SideConditions.cs: Side condition translations
    /// - LocalizationDataProvider.FieldEffects.cs: Field effect translations
    /// - LocalizationDataProvider.Hazards.cs: Hazard translations
    /// - LocalizationDataProvider.StatusEffects.cs: Status effect translations
    /// - LocalizationDataProvider.Types.cs: Type and type effectiveness translations
    /// - LocalizationDataProvider.Natures.cs: Nature translations
    /// - LocalizationDataProvider.Stats.cs: Stat translations
    /// - LocalizationDataProvider.Party.cs: Party management message translations
    /// - LocalizationDataProvider.Helpers.cs: Helper methods for key generation and registration
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static partial class LocalizationDataProvider
    {
        private static readonly Dictionary<string, LocalizationData> _data =
            new Dictionary<string, LocalizationData>(StringComparer.OrdinalIgnoreCase);

        static LocalizationDataProvider()
        {
            InitializeBattleMessages();
            InitializeTypeEffectiveness();
            InitializeStatusEffects();
            InitializeWeatherTerrain();
            InitializeAbilitiesItems();
            InitializeMoveNames();
            InitializePokemonNames();
            InitializeAbilityNames();
            InitializeItemNames();
            InitializeItemDescriptions();
            InitializeNatures();
            InitializeMoveCategories();
            InitializeStats();
            InitializeWeatherNames();
            InitializeTerrainNames();
            InitializeSideConditionNames();
            InitializeFieldEffectNames();
            InitializeHazardNames();
            InitializeStatusEffectNames();
            InitializeParty();
            InitializeUI();
        }

        /// <summary>
        /// Gets localization data for a key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <returns>LocalizationData if found, null otherwise.</returns>
        public static LocalizationData GetData(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            _data.TryGetValue(key, out var data);
            return data;
        }

        /// <summary>
        /// Checks if data exists for a key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <returns>True if data exists, false otherwise.</returns>
        public static bool HasData(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Gets all registered localization data.
        /// </summary>
        /// <returns>Collection of all LocalizationData.</returns>
        internal static IEnumerable<LocalizationData> GetAllData()
        {
            return _data.Values;
        }

        /// <summary>
        /// Registers localization data.
        /// </summary>
        private static void Register(string key, Dictionary<string, string> translations)
        {
            var data = new LocalizationData(key, translations);
            _data[key] = data;
        }
    }
}
