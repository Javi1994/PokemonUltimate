using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects.Definition;

namespace PokemonUltimate.Combat.Handlers.Definition
{
    /// <summary>
    /// Interfaz unificada para handlers de efectos de movimientos.
    /// Permite verificación de comportamientos, procesamiento de efectos y modificación de valores.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public interface IMoveEffectHandler
    {
        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        Type EffectType { get; }

        /// <summary>
        /// Verifica si el efecto puede aplicarse en las condiciones dadas.
        /// </summary>
        /// <param name="effect">El efecto a verificar. No puede ser null.</param>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>True si el efecto puede aplicarse, false en caso contrario.</returns>
        bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field);

        /// <summary>
        /// Procesa el efecto y genera las acciones correspondientes.
        /// </summary>
        /// <param name="effect">El efecto a procesar. No puede ser null.</param>
        /// <param name="user">El slot del usuario. No puede ser null.</param>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="move">Los datos del movimiento. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="damageDealt">El daño infligido por el movimiento (para efectos que dependen del daño).</param>
        /// <returns>Lista de acciones generadas por el efecto.</returns>
        List<Actions.BattleAction> Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt);

        /// <summary>
        /// Modifica un valor basado en el comportamiento del efecto.
        /// </summary>
        /// <param name="effect">El efecto que puede modificar el valor.</param>
        /// <param name="originalValue">El valor original.</param>
        /// <param name="valueType">El tipo de valor ("damage", "recoil", "drain", etc.).</param>
        /// <param name="user">El slot del usuario. Puede ser null.</param>
        /// <param name="target">El slot objetivo. Puede ser null.</param>
        /// <returns>El valor modificado si aplica, o null si no se aplica modificación.</returns>
        int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null);
    }
}
