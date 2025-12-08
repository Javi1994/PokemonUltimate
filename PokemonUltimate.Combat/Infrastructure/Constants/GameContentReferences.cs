using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Infrastructure.Constants
{
    /// <summary>
    /// Referencias seguras a contenido del juego usando propiedades estáticas del catálogo.
    /// Evita referencias hardcodeadas y proporciona validación en tiempo de compilación.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public static class GameContentReferences
    {
        #region Items

        /// <summary>
        /// Focus Sash - Prevents OHKO when at full HP (consumed on use).
        /// </summary>
        public static ItemData FocusSash => ItemCatalog.FocusSash;

        /// <summary>
        /// Leftovers - Restores 1/16 max HP at end of each turn.
        /// </summary>
        public static ItemData Leftovers => ItemCatalog.Leftovers;

        /// <summary>
        /// Life Orb - Boosts damage by 30% but causes 10% recoil.
        /// </summary>
        public static ItemData LifeOrb => ItemCatalog.LifeOrb;

        /// <summary>
        /// Black Sludge - Restores HP for Poison types, damages others.
        /// </summary>
        public static ItemData BlackSludge => ItemCatalog.BlackSludge;

        /// <summary>
        /// Choice Band - Boosts Attack by 50% but locks to one move.
        /// </summary>
        public static ItemData ChoiceBand => ItemCatalog.ChoiceBand;

        /// <summary>
        /// Choice Specs - Boosts Special Attack by 50% but locks to one move.
        /// </summary>
        public static ItemData ChoiceSpecs => ItemCatalog.ChoiceSpecs;

        /// <summary>
        /// Choice Scarf - Boosts Speed by 50% but locks to one move.
        /// </summary>
        public static ItemData ChoiceScarf => ItemCatalog.ChoiceScarf;

        #endregion

        #region Abilities

        /// <summary>
        /// Sturdy - Prevents OHKO when at full HP.
        /// </summary>
        public static AbilityData Sturdy => AbilityCatalog.Sturdy;

        /// <summary>
        /// Contrary - Reverses stat changes.
        /// </summary>
        //TODO: Implement stat change reversal logic
        //public static AbilityData Contrary => AbilityCatalog.Contrary;

        /// <summary>
        /// Intimidate - Lowers opposing Pokemon's Attack on switch-in.
        /// </summary>
        public static AbilityData Intimidate => AbilityCatalog.Intimidate;

        /// <summary>
        /// Clear Body - Prevents stats from being lowered.
        /// </summary>
        public static AbilityData ClearBody => AbilityCatalog.ClearBody;

        /// <summary>
        /// Speed Boost - Raises Speed at the end of each turn.
        /// </summary>
        public static AbilityData SpeedBoost => AbilityCatalog.SpeedBoost;

        #endregion
    }
}
