using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    // Note: AbilityData must be imported from PokemonUltimate.Core.Blueprints
    /// <summary>
    /// Fluent builder for creating PokemonSpeciesData instances.
    /// Usage: Pokemon.Define("Pikachu", 25).Type(Electric).Stats(...).Build()
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class PokemonBuilder
    {
        private readonly PokemonSpeciesData _pokemon;
        private readonly List<Core.Evolution.Evolution> _evolutions = new List<Core.Evolution.Evolution>();
        private List<LearnableMove> _learnset = new List<LearnableMove>();

        private PokemonBuilder(string name, int pokedexNumber)
        {
            _pokemon = new PokemonSpeciesData
            {
                Name = name,
                PokedexNumber = pokedexNumber
            };
        }

        /// <summary>
        /// Start defining a new Pokemon.
        /// </summary>
        public static PokemonBuilder Define(string name, int pokedexNumber)
        {
            return new PokemonBuilder(name, pokedexNumber);
        }

        /// <summary>
        /// Set the Pokemon's primary type (mono-type).
        /// </summary>
        public PokemonBuilder Type(PokemonType primary)
        {
            _pokemon.PrimaryType = primary;
            _pokemon.SecondaryType = null;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's types (dual-type).
        /// </summary>
        public PokemonBuilder Types(PokemonType primary, PokemonType secondary)
        {
            _pokemon.PrimaryType = primary;
            _pokemon.SecondaryType = secondary;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's base stats.
        /// </summary>
        public PokemonBuilder Stats(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            _pokemon.BaseStats = new BaseStats(hp, attack, defense, spAttack, spDefense, speed);
            return this;
        }

        /// <summary>
        /// Set the Pokemon's gender ratio (percentage chance of being male).
        /// Use Genderless(), MaleOnly(), or FemaleOnly() for special cases.
        /// </summary>
        public PokemonBuilder GenderRatio(float malePercentage)
        {
            _pokemon.GenderRatio = malePercentage;
            return this;
        }

        /// <summary>
        /// Mark this Pokemon as genderless (Magnemite, Ditto, etc.).
        /// </summary>
        public PokemonBuilder Genderless()
        {
            _pokemon.GenderRatio = -1f;
            return this;
        }

        /// <summary>
        /// Mark this Pokemon as male-only (Tauros, Hitmonlee, etc.).
        /// </summary>
        public PokemonBuilder MaleOnly()
        {
            _pokemon.GenderRatio = 100f;
            return this;
        }

        /// <summary>
        /// Mark this Pokemon as female-only (Chansey, Jynx, etc.).
        /// </summary>
        public PokemonBuilder FemaleOnly()
        {
            _pokemon.GenderRatio = 0f;
            return this;
        }

        #region Abilities

        /// <summary>
        /// Set the Pokemon's primary ability.
        /// </summary>
        public PokemonBuilder Ability(AbilityData ability)
        {
            _pokemon.Ability1 = ability;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's abilities (primary and optional secondary).
        /// </summary>
        public PokemonBuilder Abilities(AbilityData primary, AbilityData secondary = null)
        {
            _pokemon.Ability1 = primary;
            _pokemon.Ability2 = secondary;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's hidden ability.
        /// </summary>
        public PokemonBuilder HiddenAbility(AbilityData ability)
        {
            _pokemon.HiddenAbility = ability;
            return this;
        }

        #endregion

        /// <summary>
        /// Define the Pokemon's learnset using a fluent builder.
        /// </summary>
        public PokemonBuilder Moves(Action<LearnsetBuilder> configure)
        {
            var builder = new LearnsetBuilder();
            configure(builder);
            _learnset = builder.Build();
            return this;
        }

        /// <summary>
        /// Define an evolution for this Pokemon.
        /// </summary>
        public PokemonBuilder EvolvesTo(PokemonSpeciesData target, Action<EvolutionBuilder> configure)
        {
            var builder = new EvolutionBuilder(target);
            configure(builder);
            _evolutions.Add(builder.Build());
            return this;
        }

        #region Gameplay Fields

        /// <summary>
        /// Set the base experience yield when this Pokemon is defeated.
        /// Used in Gen 3+ EXP formula: (BaseExp * Level * WildMultiplier) / (7 * Participants)
        /// </summary>
        public PokemonBuilder BaseExp(int baseExp)
        {
            _pokemon.BaseExperienceYield = baseExp;
            return this;
        }

        /// <summary>
        /// Set the catch rate (3-255). Lower values = harder to catch.
        /// Typical values: 45 (common), 3 (legendaries), 255 (guaranteed catch)
        /// </summary>
        public PokemonBuilder CatchRate(int catchRate)
        {
            _pokemon.CatchRate = catchRate;
            return this;
        }

        /// <summary>
        /// Set the base friendship value (0-255) when Pokemon is first obtained.
        /// Default: 70 (wild Pokemon), 120 (hatched from egg), 0 (some legendaries)
        /// </summary>
        public PokemonBuilder BaseFriendship(int baseFriendship)
        {
            _pokemon.BaseFriendship = baseFriendship;
            return this;
        }

        /// <summary>
        /// Set the experience growth rate curve.
        /// </summary>
        public PokemonBuilder GrowthRate(Core.Enums.GrowthRate growthRate)
        {
            _pokemon.GrowthRate = growthRate;
            return this;
        }

        #endregion

        #region Pokedex Fields

        /// <summary>
        /// Set the Pokedex entry description text.
        /// </summary>
        public PokemonBuilder Description(string description)
        {
            _pokemon.Description = description;
            return this;
        }

        /// <summary>
        /// Set the Pokemon classification (e.g., "Flame Pokemon", "Mouse Pokemon").
        /// </summary>
        public PokemonBuilder Category(string category)
        {
            _pokemon.Category = category;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's height in meters.
        /// </summary>
        public PokemonBuilder Height(float height)
        {
            _pokemon.Height = height;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's weight in kilograms.
        /// </summary>
        public PokemonBuilder Weight(float weight)
        {
            _pokemon.Weight = weight;
            return this;
        }

        /// <summary>
        /// Set the Pokemon color classification.
        /// </summary>
        public PokemonBuilder Color(Core.Enums.PokemonColor color)
        {
            _pokemon.Color = color;
            return this;
        }

        /// <summary>
        /// Set the Pokemon shape classification.
        /// </summary>
        public PokemonBuilder Shape(Core.Enums.PokemonShape shape)
        {
            _pokemon.Shape = shape;
            return this;
        }

        /// <summary>
        /// Set the Pokemon habitat classification.
        /// </summary>
        public PokemonBuilder Habitat(Core.Enums.PokemonHabitat habitat)
        {
            _pokemon.Habitat = habitat;
            return this;
        }

        #endregion

        #region Variant Methods

        /// <summary>
        /// Mark this Pokemon as a Mega Evolution variant of the specified base form.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form this variant is based on.</param>
        /// <param name="variant">Optional variant identifier (e.g., "X", "Y") for Pokemon with multiple Mega forms.</param>
        /// <remarks>
        /// **Feature**: 1: Game Data
        /// **Sub-Feature**: 1.18: Variants System
        /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
        /// </remarks>
        public PokemonBuilder AsMegaVariant(PokemonSpeciesData baseForm, string variant = null)
        {
            if (baseForm == null)
                throw new ArgumentNullException(nameof(baseForm), ErrorMessages.PokemonCannotBeNull);

            _pokemon.BaseForm = baseForm;
            _pokemon.VariantType = PokemonVariantType.Mega;
            _pokemon.TeraType = null;
            _pokemon.RegionalForm = string.Empty;

            // Note: The bidirectional relationship (adding to baseForm.Variants) 
            // should be handled by the caller or a separate relationship manager
            // to maintain Single Responsibility Principle

            return this;
        }

        /// <summary>
        /// Mark this Pokemon as a Dinamax variant of the specified base form.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form this variant is based on.</param>
        /// <remarks>
        /// **Feature**: 1: Game Data
        /// **Sub-Feature**: 1.18: Variants System
        /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
        /// </remarks>
        public PokemonBuilder AsDinamaxVariant(PokemonSpeciesData baseForm)
        {
            if (baseForm == null)
                throw new ArgumentNullException(nameof(baseForm), ErrorMessages.PokemonCannotBeNull);

            _pokemon.BaseForm = baseForm;
            _pokemon.VariantType = PokemonVariantType.Dinamax;
            _pokemon.TeraType = null;
            _pokemon.RegionalForm = string.Empty;

            // Note: The bidirectional relationship (adding to baseForm.Variants) 
            // should be handled by the caller or a separate relationship manager
            // to maintain Single Responsibility Principle

            return this;
        }

        /// <summary>
        /// Mark this Pokemon as a Terracristalización variant of the specified base form.
        /// </summary>
        /// <param name="baseForm">The base Pokemon form this variant is based on.</param>
        /// <param name="teraType">The Tera type for this variant (mono-type).</param>
        /// <remarks>
        /// **Feature**: 1: Game Data
        /// **Sub-Feature**: 1.18: Variants System
        /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
        /// </remarks>
        public PokemonBuilder AsTeraVariant(PokemonSpeciesData baseForm, PokemonType teraType)
        {
            if (baseForm == null)
                throw new ArgumentNullException(nameof(baseForm), ErrorMessages.PokemonCannotBeNull);

            _pokemon.BaseForm = baseForm;
            _pokemon.VariantType = PokemonVariantType.Tera;
            _pokemon.TeraType = teraType;
            _pokemon.RegionalForm = string.Empty;

            // Note: The bidirectional relationship (adding to baseForm.Variants) 
            // should be handled by the caller or a separate relationship manager
            // to maintain Single Responsibility Principle

            return this;
        }

        /// <summary>
        /// Mark this Pokemon as a Regional form variant of the specified base form.
        /// Regional forms can have different types, stats, abilities, or be purely visual.
        /// Examples: Alolan Vulpix (Ice type), Galarian Meowth (Steel type), Alolan Raichu (Electric/Psychic)
        /// </summary>
        /// <param name="baseForm">The base Pokemon form this variant is based on.</param>
        /// <param name="region">The region identifier (e.g., "Alola", "Galar", "Hisui", "Paldea").</param>
        /// <remarks>
        /// **Feature**: 1: Game Data
        /// **Sub-Feature**: 1.18: Variants System
        /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
        /// </remarks>
        public PokemonBuilder AsRegionalVariant(PokemonSpeciesData baseForm, string region)
        {
            if (baseForm == null)
                throw new ArgumentNullException(nameof(baseForm), ErrorMessages.PokemonCannotBeNull);
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region cannot be null or empty", nameof(region));

            _pokemon.BaseForm = baseForm;
            _pokemon.VariantType = PokemonVariantType.Regional;
            _pokemon.RegionalForm = region;
            _pokemon.TeraType = null;

            // Note: The bidirectional relationship (adding to baseForm.Variants) 
            // should be handled by the caller or a separate relationship manager
            // to maintain Single Responsibility Principle

            return this;
        }

        #endregion

        /// <summary>
        /// Build the final PokemonSpeciesData instance.
        /// Establishes bidirectional relationship if this is a variant.
        /// </summary>
        public PokemonSpeciesData Build()
        {
            _pokemon.Learnset = _learnset;
            _pokemon.Evolutions = _evolutions;
            
            // Establish bidirectional relationship for variants (SRP: Build handles finalization)
            if (_pokemon.IsVariant && _pokemon.BaseForm != null)
            {
                EstablishVariantRelationship(_pokemon, _pokemon.BaseForm);
            }
            
            return _pokemon;
        }

        /// <summary>
        /// Establishes the bidirectional relationship between a variant and its base form.
        /// This method encapsulates the relationship logic to maintain Single Responsibility Principle.
        /// </summary>
        private static void EstablishVariantRelationship(PokemonSpeciesData variant, PokemonSpeciesData baseForm)
        {
            if (variant == null || baseForm == null)
                return;

            if (!baseForm.Variants.Contains(variant))
            {
                baseForm.Variants.Add(variant);
            }
        }
    }

    /// <summary>
    /// Static entry point for the Pokemon builder.
    /// </summary>
    public static class Pokemon
    {
        /// <summary>
        /// Start defining a new Pokemon species.
        /// </summary>
        public static PokemonBuilder Define(string name, int pokedexNumber)
        {
            return PokemonBuilder.Define(name, pokedexNumber);
        }
    }
}

