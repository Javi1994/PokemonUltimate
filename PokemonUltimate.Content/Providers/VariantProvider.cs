using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Provides variant forms for Pokemon species.
    /// Centralized provider for creating and managing Pokemon variants (Mega, Dinamax, Tera, Regional, Cosmetic).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.18: Variants System
    /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
    ///
    /// **Design**: Follows SRP (Single Responsibility Principle) - only responsible for providing variant data.
    /// Variants are created and registered here, then can be accessed via the Pokemon base form's Variants property.
    /// </remarks>
    public static class VariantProvider
    {
        private static readonly Dictionary<string, List<PokemonSpeciesData>> _variantsByBaseName =
            new Dictionary<string, List<PokemonSpeciesData>>(System.StringComparer.OrdinalIgnoreCase);

        static VariantProvider()
        {
            InitializeGen1Variants();
            // InitializeGen2Variants(); // Uncomment when Gen2 variants are added
        }

        /// <summary>
        /// Gets all variant forms for a Pokemon base form.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form.</param>
        /// <returns>List of variant forms, or empty list if none exist.</returns>
        public static IEnumerable<PokemonSpeciesData> GetVariants(PokemonSpeciesData baseForm)
        {
            if (baseForm == null)
                return Enumerable.Empty<PokemonSpeciesData>();

            if (_variantsByBaseName.TryGetValue(baseForm.Name, out var variants))
                return variants;

            return Enumerable.Empty<PokemonSpeciesData>();
        }

        /// <summary>
        /// Gets all variant forms for a Pokemon by name.
        /// </summary>
        /// <param name="basePokemonName">The name of the base Pokemon (case-insensitive).</param>
        /// <returns>List of variant forms, or empty list if none exist.</returns>
        public static IEnumerable<PokemonSpeciesData> GetVariants(string basePokemonName)
        {
            if (string.IsNullOrEmpty(basePokemonName))
                return Enumerable.Empty<PokemonSpeciesData>();

            if (_variantsByBaseName.TryGetValue(basePokemonName, out var variants))
                return variants;

            return Enumerable.Empty<PokemonSpeciesData>();
        }

        /// <summary>
        /// Checks if a Pokemon has any variant forms.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form.</param>
        /// <returns>True if variants exist, false otherwise.</returns>
        public static bool HasVariants(PokemonSpeciesData baseForm)
        {
            if (baseForm == null)
                return false;

            return _variantsByBaseName.ContainsKey(baseForm.Name) &&
                   _variantsByBaseName[baseForm.Name].Count > 0;
        }

        /// <summary>
        /// Gets all Mega Evolution variants for a Pokemon.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form.</param>
        /// <returns>List of Mega Evolution variants.</returns>
        public static IEnumerable<PokemonSpeciesData> GetMegaVariants(PokemonSpeciesData baseForm)
        {
            return GetVariants(baseForm).Where(v => v.IsMegaVariant);
        }

        /// <summary>
        /// Gets all Regional form variants for a Pokemon.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form.</param>
        /// <returns>List of Regional form variants.</returns>
        public static IEnumerable<PokemonSpeciesData> GetRegionalVariants(PokemonSpeciesData baseForm)
        {
            return GetVariants(baseForm).Where(v => v.IsRegionalVariant);
        }

        /// <summary>
        /// Gets all Terracristalización variants for a Pokemon.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form.</param>
        /// <returns>List of Terracristalización variants.</returns>
        public static IEnumerable<PokemonSpeciesData> GetTeraVariants(PokemonSpeciesData baseForm)
        {
            return GetVariants(baseForm).Where(v => v.IsTeraVariant);
        }

        /// <summary>
        /// Registers a variant form for a base Pokemon.
        /// This method establishes the bidirectional relationship automatically.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form.</param>
        /// <param name="variant">The variant form to register.</param>
        private static void RegisterVariant(PokemonSpeciesData baseForm, PokemonSpeciesData variant)
        {
            if (baseForm == null || variant == null)
                return;

            if (!_variantsByBaseName.ContainsKey(baseForm.Name))
                _variantsByBaseName[baseForm.Name] = new List<PokemonSpeciesData>();

            if (!_variantsByBaseName[baseForm.Name].Contains(variant))
            {
                _variantsByBaseName[baseForm.Name].Add(variant);
                // Also add to base form's Variants list (bidirectional relationship)
                if (!baseForm.Variants.Contains(variant))
                    baseForm.Variants.Add(variant);
            }
        }

        #region Generation Variant Initialization

        /// <summary>
        /// Initialize Generation 1 Pokemon variants.
        /// </summary>
        private static void InitializeGen1Variants()
        {
            // Example: Charizard Mega Evolutions
            if (PokemonCatalog.Charizard != null)
            {
                // Mega Charizard Y (using existing ability)
                var megaCharizardY = Pokemon.Define("Mega Charizard Y", 6)
                    .Types(PokemonType.Fire, PokemonType.Flying)
                    .Stats(78, 104, 78, 159, 115, 100)
                    .Ability(AbilityCatalog.Drought)
                    .AsMegaVariant(PokemonCatalog.Charizard, "Y")
                    .Build();
                RegisterVariant(PokemonCatalog.Charizard, megaCharizardY);

                // Charizard Dinamax
                var charizardDinamax = Pokemon.Define("Charizard Dinamax", 6)
                    .Types(PokemonType.Fire, PokemonType.Flying)
                    .Stats(156, 84, 78, 109, 85, 100) // HP doubled
                    .AsDinamaxVariant(PokemonCatalog.Charizard)
                    .Build();
                RegisterVariant(PokemonCatalog.Charizard, charizardDinamax);
            }

            // Example: Venusaur Mega Evolution
            if (PokemonCatalog.Venusaur != null)
            {
                var megaVenusaur = Pokemon.Define("Mega Venusaur", 3)
                    .Types(PokemonType.Grass, PokemonType.Poison)
                    .Stats(80, 100, 123, 122, 120, 80)
                    .Ability(AbilityCatalog.ThickFat)
                    .AsMegaVariant(PokemonCatalog.Venusaur)
                    .Build();
                RegisterVariant(PokemonCatalog.Venusaur, megaVenusaur);
            }

            // Example: Pikachu Cosmetic Variants (using Regional variant type for now)
            if (PokemonCatalog.Pikachu != null)
            {
                var pikachuLibre = Pokemon.Define("Pikachu Libre", 25)
                    .Type(PokemonType.Electric)
                    .Stats(35, 55, 40, 50, 50, 90) // Same stats as base
                    .AsRegionalVariant(PokemonCatalog.Pikachu, "Libre")
                    .Build();
                RegisterVariant(PokemonCatalog.Pikachu, pikachuLibre);
            }

            // Add more Gen 1 variants here as needed
        }

        #endregion
    }
}

