using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Providers;
using PokemonUltimate.Core.Infrastructure.Providers.Definition;

namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Blueprint for a Pokemon species (immutable data).
    /// Pokemon can be retrieved by Name (unique string) or PokedexNumber (unique int).
    /// This is the "Species" data - shared by all Pokemon of the same kind.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/README.md`
    /// </remarks>
    public class PokemonSpeciesData : IIdentifiable
    {
        /// <summary>
        /// Unique identifier - the Pokemon's name (e.g., "Pikachu", "Charizard").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// National Pokedex number (e.g., 25 for Pikachu, 6 for Charizard).
        /// </summary>
        public int PokedexNumber { get; set; }

        /// <summary>
        /// Primary type of the Pokemon (every Pokemon has one).
        /// </summary>
        public PokemonType PrimaryType { get; set; }

        /// <summary>
        /// Secondary type of the Pokemon (optional, null if mono-type).
        /// </summary>
        public PokemonType? SecondaryType { get; set; }

        /// <summary>
        /// Base stats used for calculating actual stats.
        /// </summary>
        public BaseStats BaseStats { get; set; } = new BaseStats();

        /// <summary>
        /// Moves this Pokemon can learn.
        /// </summary>
        public List<LearnableMove> Learnset { get; set; } = new List<LearnableMove>();

        /// <summary>
        /// Possible evolutions for this Pokemon.
        /// </summary>
        public List<Domain.Evolution.Evolution> Evolutions { get; set; } = new List<Domain.Evolution.Evolution>();

        /// <summary>
        /// Gender ratio: percentage chance of being male (0-100).
        /// -1 means genderless (Magnemite, Ditto, etc.).
        /// Common values: 50 (equal), 87.5 (starters), 0 (female only), 100 (male only).
        /// </summary>
        public float GenderRatio { get; set; } = 50f;

        #region Abilities

        /// <summary>
        /// Primary ability (most Pokemon have this).
        /// </summary>
        public AbilityData Ability1 { get; set; }

        /// <summary>
        /// Secondary ability (optional, some Pokemon don't have one).
        /// </summary>
        public AbilityData Ability2 { get; set; }

        /// <summary>
        /// Hidden ability (rare, obtained through special means).
        /// </summary>
        public AbilityData HiddenAbility { get; set; }

        #endregion

        #region Gameplay Fields

        /// <summary>
        /// Base experience yield when this Pokemon is defeated.
        /// Used in Gen 3+ EXP formula: (BaseExp * Level * WildMultiplier) / (7 * Participants)
        /// Typical range: 50-300 (legendaries can be 300+)
        /// </summary>
        public int BaseExperienceYield { get; set; } = 0;

        /// <summary>
        /// Catch rate (3-255). Lower values = harder to catch.
        /// Typical values: 45 (common), 3 (legendaries), 255 (guaranteed catch)
        /// </summary>
        public int CatchRate { get; set; } = 45;

        /// <summary>
        /// Base friendship value (0-255) when Pokemon is first obtained.
        /// Default: 70 (wild Pokemon), 120 (hatched from egg), 0 (some legendaries)
        /// </summary>
        public int BaseFriendship { get; set; } = 70;

        /// <summary>
        /// Experience growth rate curve for this Pokemon.
        /// Determines EXP needed per level.
        /// </summary>
        public GrowthRate GrowthRate { get; set; } = GrowthRate.MediumFast;

        #endregion

        #region Breeding Fields

        /// <summary>
        /// Egg groups this Pokemon belongs to (determines breeding compatibility).
        /// Most Pokemon have one egg group, some have two (e.g., Pikachu is Field/Fairy).
        /// Pokemon can breed if they share at least one egg group (unless one is Ditto).
        /// </summary>
        public List<EggGroup> EggGroups { get; set; } = new List<EggGroup>();

        /// <summary>
        /// Number of egg cycles required to hatch an egg of this Pokemon species.
        /// Range: 1-40 (typical: 5-20, legendaries: 40, baby Pokemon: 5-10).
        /// Each cycle = 255 steps in the original games.
        /// </summary>
        public int EggCycles { get; set; } = 20;

        #endregion

        #region Pokedex Fields

        /// <summary>
        /// Pokedex entry description text.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Pokemon classification (e.g., "Flame Pokemon", "Mouse Pokemon").
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Height in meters.
        /// </summary>
        public float Height { get; set; } = 0f;

        /// <summary>
        /// Weight in kilograms.
        /// </summary>
        public float Weight { get; set; } = 0f;

        /// <summary>
        /// Pokemon color classification.
        /// </summary>
        public PokemonColor Color { get; set; } = PokemonColor.Unknown;

        /// <summary>
        /// Pokemon shape classification.
        /// </summary>
        public PokemonShape Shape { get; set; } = PokemonShape.Unknown;

        /// <summary>
        /// Pokemon habitat classification.
        /// </summary>
        public PokemonHabitat Habitat { get; set; } = PokemonHabitat.Unknown;

        #endregion

        #region Variant Fields

        /// <summary>
        /// Reference to the base Pokemon form (null if this is a base form).
        /// Variants are implemented as separate Pokemon species with different stats.
        /// </summary>
        /// <remarks>
        /// **Feature**: 1: Game Data
        /// **Sub-Feature**: 1.18: Variants System
        /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
        /// </remarks>
        public PokemonSpeciesData BaseForm { get; set; }

        /// <summary>
        /// Type of variant (Mega, Dinamax, Tera). Null if this is a base form.
        /// </summary>
        public PokemonVariantType? VariantType { get; set; }

        /// <summary>
        /// Tera type for Terracristalización variants. Only set when VariantType is Tera.
        /// </summary>
        public PokemonType? TeraType { get; set; }

        /// <summary>
        /// Regional identifier for Regional form variants (e.g., "Alola", "Galar", "Hisui", "Paldea").
        /// Only set when VariantType is Regional.
        /// </summary>
        public string RegionalForm { get; set; } = string.Empty;

        /// <summary>
        /// List of all variant forms of this Pokemon. Populated automatically when variants are registered.
        /// </summary>
        public List<PokemonSpeciesData> Variants { get; set; } = new List<PokemonSpeciesData>();

        #endregion

        /// <summary>
        /// IIdentifiable implementation - Name serves as the unique ID.
        /// </summary>
        public string Id => Name;

        /// <summary>
        /// Returns true if this Pokemon has two types.
        /// </summary>
        public bool IsDualType => SecondaryType.HasValue;

        /// <summary>
        /// Returns true if this Pokemon can evolve.
        /// </summary>
        public bool CanEvolve => Evolutions.Count > 0;

        /// <summary>
        /// Returns true if this Pokemon is genderless.
        /// </summary>
        public bool IsGenderless => GenderRatio < 0;

        /// <summary>
        /// Returns true if this Pokemon can only be male.
        /// </summary>
        public bool IsMaleOnly => GenderRatio >= 100;

        /// <summary>
        /// Returns true if this Pokemon can only be female.
        /// </summary>
        public bool IsFemaleOnly => GenderRatio == 0;

        /// <summary>
        /// Returns true if this Pokemon can be either gender.
        /// </summary>
        public bool HasBothGenders => GenderRatio > 0 && GenderRatio < 100;

        /// <summary>
        /// Returns true if this Pokemon has a secondary ability.
        /// </summary>
        public bool HasSecondaryAbility => Ability2 != null;

        /// <summary>
        /// Returns true if this Pokemon has a hidden ability.
        /// </summary>
        public bool HasHiddenAbility => HiddenAbility != null;

        /// <summary>
        /// Returns true if this Pokemon is a variant form (has a BaseForm).
        /// </summary>
        public bool IsVariant => BaseForm != null;

        /// <summary>
        /// Returns true if this Pokemon is a base form (not a variant).
        /// </summary>
        public bool IsBaseForm => BaseForm == null;

        /// <summary>
        /// Returns true if this Pokemon is a Mega Evolution variant.
        /// </summary>
        public bool IsMegaVariant => VariantType == PokemonVariantType.Mega;

        /// <summary>
        /// Returns true if this Pokemon is a Dinamax variant.
        /// </summary>
        public bool IsDinamaxVariant => VariantType == PokemonVariantType.Dinamax;

        /// <summary>
        /// Returns true if this Pokemon is a Terracristalización variant.
        /// </summary>
        public bool IsTeraVariant => VariantType == PokemonVariantType.Tera;

        /// <summary>
        /// Returns true if this Pokemon is a Regional form variant.
        /// </summary>
        public bool IsRegionalVariant => VariantType == PokemonVariantType.Regional;

        /// <summary>
        /// Returns true if this Pokemon is a Cosmetic variant (e.g., Pikachu Libre).
        /// </summary>
        public bool IsCosmeticVariant => VariantType == PokemonVariantType.Cosmetic;

        /// <summary>
        /// Returns true if this variant has gameplay changes (stats, types, abilities).
        /// Returns false for purely visual variants (some regional forms).
        /// </summary>
        public bool HasGameplayChanges
        {
            get
            {
                if (!IsVariant || BaseForm == null) return false;

                // Check for stat differences
                if (BaseStats.HP != BaseForm.BaseStats.HP ||
                    BaseStats.Attack != BaseForm.BaseStats.Attack ||
                    BaseStats.Defense != BaseForm.BaseStats.Defense ||
                    BaseStats.SpAttack != BaseForm.BaseStats.SpAttack ||
                    BaseStats.SpDefense != BaseForm.BaseStats.SpDefense ||
                    BaseStats.Speed != BaseForm.BaseStats.Speed) return true;

                // Check for type differences
                if (PrimaryType != BaseForm.PrimaryType || SecondaryType != BaseForm.SecondaryType) return true;

                // Check for ability differences (consider Ability1, Ability2, HiddenAbility)
                if (Ability1?.Name != BaseForm.Ability1?.Name ||
                    Ability2?.Name != BaseForm.Ability2?.Name ||
                    HiddenAbility?.Name != BaseForm.HiddenAbility?.Name) return true;

                // Tera variants always have a gameplay change (type change)
                if (IsTeraVariant) return true;

                // Dinamax variants always have a gameplay change (HP increase)
                if (IsDinamaxVariant) return true;

                // Mega variants always have gameplay changes (stats, types, abilities)
                if (IsMegaVariant) return true;

                return false;
            }
        }

        /// <summary>
        /// Returns true if this Pokemon has any variant forms.
        /// </summary>
        public bool HasVariants => Variants.Count > 0;

        /// <summary>
        /// Returns the number of variant forms this Pokemon has.
        /// </summary>
        public int VariantCount => Variants.Count;

        /// <summary>
        /// Gets all Mega Evolution variants of this Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> MegaVariants => Variants.Where(v => v.IsMegaVariant);

        /// <summary>
        /// Gets all Dinamax variants of this Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> DinamaxVariants => Variants.Where(v => v.IsDinamaxVariant);

        /// <summary>
        /// Gets all Terracristalización variants of this Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> TeraVariants => Variants.Where(v => v.IsTeraVariant);

        /// <summary>
        /// Gets all Regional form variants of this Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> RegionalVariants => Variants.Where(v => v.IsRegionalVariant);

        /// <summary>
        /// Gets all Cosmetic variants of this Pokemon.
        /// </summary>
        public IEnumerable<PokemonSpeciesData> CosmeticVariants => Variants.Where(v => v.IsCosmeticVariant);

        /// <summary>
        /// Gets all variants that have gameplay changes (stats, types, abilities).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> VariantsWithGameplayChanges => Variants.Where(v => v.HasGameplayChanges);

        /// <summary>
        /// Gets all variants that are purely visual (no gameplay changes).
        /// </summary>
        public IEnumerable<PokemonSpeciesData> VisualOnlyVariants => Variants.Where(v => !v.HasGameplayChanges);

        /// <summary>
        /// Gets all possible abilities for this Pokemon.
        /// </summary>
        public IEnumerable<AbilityData> GetAllAbilities()
        {
            if (Ability1 != null) yield return Ability1;
            if (Ability2 != null) yield return Ability2;
            if (HiddenAbility != null) yield return HiddenAbility;
        }

        /// <summary>
        /// Gets a random non-hidden ability for this Pokemon.
        /// </summary>
        /// <param name="randomProvider">Optional random provider. If null, creates a new RandomProvider.</param>
        public AbilityData GetRandomAbility(IRandomProvider randomProvider = null)
        {
            randomProvider = randomProvider ?? new RandomProvider();
            if (Ability2 != null && randomProvider.Next(2) == 1)
                return Ability2;
            return Ability1;
        }

        /// <summary>
        /// Checks if this Pokemon has a specific type (primary or secondary).
        /// </summary>
        public bool HasType(PokemonType type)
        {
            return PrimaryType == type || SecondaryType == type;
        }

        #region Learnset Helpers

        /// <summary>
        /// Get moves available at level 1 (starting moves).
        /// </summary>
        public IEnumerable<LearnableMove> GetStartingMoves()
        {
            return Learnset.Where(m => m.Method == LearnMethod.Start);
        }

        /// <summary>
        /// Get moves learned at a specific level.
        /// </summary>
        public IEnumerable<LearnableMove> GetMovesAtLevel(int level)
        {
            return Learnset.Where(m => m.Method == LearnMethod.LevelUp && m.Level == level);
        }

        /// <summary>
        /// Get all moves learnable up to and including a specific level.
        /// </summary>
        public IEnumerable<LearnableMove> GetMovesUpToLevel(int level)
        {
            return Learnset.Where(m =>
                m.Method == LearnMethod.Start ||
                (m.Method == LearnMethod.LevelUp && m.Level <= level));
        }

        /// <summary>
        /// Check if this Pokemon can learn a specific move (by any method).
        /// </summary>
        public bool CanLearn(MoveData move)
        {
            if (move == null)
                throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);

            return Learnset.Any(m => m.Move == move);
        }

        #endregion

        #region Breeding Helpers

        /// <summary>
        /// Checks if this Pokemon can breed with another Pokemon species.
        /// Two Pokemon can breed if:
        /// - They share at least one egg group (unless one is Ditto)
        /// - Neither has Undiscovered egg group
        /// - If one is Ditto, the other must not be Undiscovered
        /// </summary>
        /// <param name="other">The other Pokemon species to check compatibility with.</param>
        /// <returns>True if the two Pokemon can breed together.</returns>
        public bool CanBreedWith(PokemonSpeciesData other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), ErrorMessages.PokemonCannotBeNull);

            // Undiscovered cannot breed
            if (EggGroups.Contains(EggGroup.Undiscovered) ||
                other.EggGroups.Contains(EggGroup.Undiscovered))
            {
                return false;
            }

            // Ditto can breed with any (except Undiscovered, already checked)
            if (EggGroups.Contains(EggGroup.Ditto) ||
                other.EggGroups.Contains(EggGroup.Ditto))
            {
                return true;
            }

            // Check if they share at least one egg group
            return EggGroups.Intersect(other.EggGroups).Any();
        }

        /// <summary>
        /// Returns true if this Pokemon belongs to a specific egg group.
        /// </summary>
        public bool IsInEggGroup(EggGroup eggGroup)
        {
            return EggGroups.Contains(eggGroup);
        }

        /// <summary>
        /// Returns true if this Pokemon cannot breed (has Undiscovered egg group).
        /// </summary>
        public bool CannotBreed => EggGroups.Contains(EggGroup.Undiscovered);

        #endregion
    }
}

