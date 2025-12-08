using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler unificado para inversión de cambios de estadísticas (Contrary).
    /// Puede verificar comportamientos (usado en Actions) y modificar valores.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class StatChangeReversalHandler : IAbilityEffectHandler
    {
        /// <summary>
        /// El trigger que activa este handler (None porque es pasivo).
        /// </summary>
        public AbilityTrigger Trigger => AbilityTrigger.None;

        /// <summary>
        /// El efecto de habilidad que maneja este handler (None porque es verificación/modificación).
        /// </summary>
        public AbilityEffect Effect => AbilityEffect.None;

        /// <summary>
        /// Verifica si el Pokemon invierte cambios de estadísticas (tiene la habilidad Contrary).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Contrary, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            return pokemon?.Ability?.Id == "contrary";
        }

        /// <summary>
        /// Procesa el efecto (no genera acciones, solo modifica valores).
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista vacía (no genera acciones).</returns>
        public System.Collections.Generic.List<PokemonUltimate.Combat.Actions.BattleAction> Process(AbilityData ability, PokemonUltimate.Combat.Field.BattleSlot slot, PokemonUltimate.Combat.Field.BattleField field)
        {
            // Contrary no genera acciones, solo modifica valores
            return new System.Collections.Generic.List<PokemonUltimate.Combat.Actions.BattleAction>();
        }

        /// <summary>
        /// Invierte el cambio de estadísticas si el Pokemon tiene Contrary.
        /// </summary>
        /// <param name="pokemon">El Pokemon que recibe el cambio de stats. Puede ser null.</param>
        /// <param name="statChange">El cambio de estadística original (positivo o negativo).</param>
        /// <returns>El cambio invertido si tiene Contrary, o el cambio original en caso contrario.</returns>
        public int ReverseStatChange(PokemonInstance pokemon, int statChange)
        {
            if (HasBehavior(pokemon))
                return -statChange;

            return statChange;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (invierte cambios de stats).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento. No puede ser null.</param>
        /// <param name="originalValue">El valor original (cambio de stat).</param>
        /// <param name="valueType">El tipo de valor ("statChange").</param>
        /// <returns>El cambio invertido si aplica, o null si no se aplica modificación.</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            if (valueType != "statChange")
                return null;

            if (!HasBehavior(pokemon))
                return null;

            return -originalValue;
        }
    }
}
