using System.Linq;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler para verificación de aplicación de cambio de Pokemon.
    /// Valida si un cambio puede realizarse y maneja el estado de switching.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class SwitchApplicationHandler : IApplicationHandler
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

            // Side must exist
            if (slot.Side == null)
                return false;

            // Slot can be empty (for filling empty slots) or have a Pokemon (for switching)
            // Empty slots are valid when filling them with a new Pokemon
            // Non-empty slots are valid when switching Pokemon

            // Additional validation: new Pokemon must not be fainted
            if (newPokemon.IsFainted)
                return false;

            // Additional validation: new Pokemon must not already be in an active slot
            var side = slot.Side;
            if (side != null)
            {
                var activePokemon = side.Slots
                    .Where(s => !s.IsEmpty && s.Pokemon != null)
                    .Select(s => s.Pokemon)
                    .ToHashSet();

                if (activePokemon.Contains(newPokemon))
                    return false; // Pokemon is already active in another slot
            }

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
