using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Handlers.Definition
{
    /// <summary>
    /// Interface para handlers de items que necesitan información del atacante (efectos de contacto).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public interface IContactItemHandler
    {
        /// <summary>
        /// Procesa el efecto del item con información del atacante.
        /// </summary>
        /// <param name="item">El item data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item (defensor). No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="attacker">El slot del atacante que hizo contacto. No puede ser null.</param>
        /// <returns>Lista de acciones de batalla a ejecutar. Nunca null, puede estar vacía.</returns>
        List<BattleAction> ProcessWithAttacker(ItemData item, BattleSlot slot, BattleField field, BattleSlot attacker);
    }
}
