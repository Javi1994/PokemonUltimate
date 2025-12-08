using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de aplicación de cambio de Pokemon.
    /// Valida si un cambio puede realizarse y maneja el estado de switching.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class SwitchApplicationChecker
    {
        /// <summary>
        /// Verifica si un cambio de Pokemon puede realizarse.
        /// </summary>
        /// <param name="slot">El slot donde se realizará el cambio. No puede ser null.</param>
        /// <param name="newPokemon">El nuevo Pokemon. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>True si el cambio puede realizarse, false en caso contrario.</returns>
        public bool CanSwitch(BattleSlot slot, PokemonInstance newPokemon, BattleField field)
        {
            if (slot == null || newPokemon == null || field == null)
                return false;

            // Slot must not be empty (must have a Pokemon to switch out)
            if (slot.IsEmpty)
                return false;

            // Side must exist
            if (slot.Side == null)
                return false;

            return true;
        }

        /// <summary>
        /// Prepara el slot para el cambio marcando el Pokemon como switching out.
        /// Esto es necesario para efectos como Pursuit.
        /// </summary>
        /// <param name="slot">El slot que cambiará. No puede ser null.</param>
        public void PrepareSwitchOut(BattleSlot slot)
        {
            if (slot == null || slot.IsEmpty)
                return;

            // Mark Pokemon as switching out (for Pursuit detection)
            slot.AddVolatileStatus(VolatileStatus.SwitchingOut);
        }
    }
}
