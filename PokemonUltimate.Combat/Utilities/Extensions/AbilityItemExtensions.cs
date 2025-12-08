using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Utilities.Extensions
{
    /// <summary>
    /// Extension methods para verificar comportamientos de items y habilidades.
    /// Proporciona métodos de conveniencia para verificaciones comunes sin hardcoding.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public static class AbilityItemExtensions
    {
        /// <summary>
        /// Verifica si el Pokemon tiene un item específico por ID.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <param name="item">El item a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene el item, false en caso contrario.</returns>
        public static bool HasItem(this PokemonInstance pokemon, ItemData item)
        {
            if (pokemon == null || item == null)
                return false;

            return pokemon.HeldItem?.Id == item.Id;
        }

        /// <summary>
        /// Verifica si el Pokemon tiene una habilidad específica por ID.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <param name="ability">La habilidad a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene la habilidad, false en caso contrario.</returns>
        public static bool HasAbility(this PokemonInstance pokemon, AbilityData ability)
        {
            if (pokemon == null || ability == null)
                return false;

            return pokemon.Ability?.Id == ability.Id;
        }

        /// <summary>
        /// Verifica si el Pokemon tiene Focus Sash.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Focus Sash, false en caso contrario.</returns>
        public static bool HasFocusSash(this PokemonInstance pokemon)
        {
            return pokemon.HasItem(GameContentReferences.FocusSash);
        }

        /// <summary>
        /// Verifica si el Pokemon tiene la habilidad Sturdy.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Sturdy, false en caso contrario.</returns>
        public static bool HasSturdy(this PokemonInstance pokemon)
        {
            return pokemon.HasAbility(GameContentReferences.Sturdy);
        }

        /// <summary>
        /// Verifica si el Pokemon tiene la habilidad Contrary.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Contrary, false en caso contrario.</returns>
        public static bool HasContrary(this PokemonInstance pokemon)
        {
            //TODO: return pokemon.HasAbility(GameContentReferences.Contrary);
            return false;
        }
    }
}
