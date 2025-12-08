using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Handlers.Definition
{
    /// <summary>
    /// Interface unificada para handlers que procesan efectos de habilidades.
    /// Puede verificar comportamientos (usado en Actions) y procesar efectos (usado en Steps).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public interface IAbilityEffectHandler
    {
        /// <summary>
        /// El trigger que activa este handler (None si es solo verificación).
        /// </summary>
        AbilityTrigger Trigger { get; }

        /// <summary>
        /// El efecto de habilidad que maneja este handler (None si es solo verificación).
        /// </summary>
        AbilityEffect Effect { get; }

        /// <summary>
        /// Verifica si el Pokemon tiene el comportamiento que maneja este handler.
        /// Usado por Actions para verificar condiciones.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene el comportamiento, false en caso contrario.</returns>
        bool HasBehavior(PokemonInstance pokemon);

        /// <summary>
        /// Procesa el efecto de la habilidad y genera acciones.
        /// Usado por Steps cuando se activa el trigger.
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista de acciones de batalla a ejecutar. Nunca null, puede estar vacía.</returns>
        List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field);

        /// <summary>
        /// Modifica un valor basado en el comportamiento (opcional).
        /// Usado por Actions para modificar daño, stats, etc.
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento. No puede ser null.</param>
        /// <param name="originalValue">El valor original a modificar.</param>
        /// <param name="valueType">El tipo de valor ("damage", "statChange", etc.).</param>
        /// <returns>El valor modificado, o null si no se aplica modificación.</returns>
        int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType);
    }
}
