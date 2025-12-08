using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Combat.Handlers.Definition
{
    /// <summary>
    /// Interface para handlers de habilidades que necesitan información del atacante (efectos de contacto).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public interface IContactAbilityHandler
    {
        /// <summary>
        /// Procesa el efecto de la habilidad con información del atacante.
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad (defensor). No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="attacker">El slot del atacante que hizo contacto. No puede ser null.</param>
        /// <returns>Lista de acciones de batalla a ejecutar. Nunca null, puede estar vacía.</returns>
        List<BattleAction> ProcessWithAttacker(AbilityData ability, BattleSlot slot, BattleField field, BattleSlot attacker);
    }
}
