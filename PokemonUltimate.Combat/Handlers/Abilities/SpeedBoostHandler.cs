using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Handlers.Abilities
{
    /// <summary>
    /// Handler para la habilidad Speed Boost que aumenta Speed al final de cada turno.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class SpeedBoostHandler : IAbilityEffectHandler
    {
        /// <summary>
        /// El trigger que activa este handler.
        /// </summary>
        public AbilityTrigger Trigger => AbilityTrigger.OnTurnEnd;

        /// <summary>
        /// El efecto de habilidad que maneja este handler.
        /// </summary>
        public AbilityEffect Effect => AbilityEffect.RaiseOwnStat;

        /// <summary>
        /// Verifica si el Pokemon tiene Speed Boost.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Speed Boost, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            // Speed Boost se identifica por su efecto RaiseOwnStat en OnTurnEnd
            // La verificación específica se hace en Process basándose en la ability data
            return pokemon?.Ability != null &&
                   pokemon.Ability.Effect == AbilityEffect.RaiseOwnStat &&
                   pokemon.Ability.ListensTo(AbilityTrigger.OnTurnEnd);
        }

        /// <summary>
        /// Procesa el efecto de Speed Boost y genera acciones.
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista de acciones generadas.</returns>
        public List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.TargetStat == null)
                return actions;

            var pokemon = slot.Pokemon;
            var currentStatStage = slot.GetStatStage(ability.TargetStat.Value);

            // Verificar si el stat ya está al máximo (+6 stages)
            if (currentStatStage >= 6)
                return actions;

            // Mensaje de activación de habilidad
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, pokemon.DisplayName, abilityName)));

            // Aumentar el stat propio
            actions.Add(new StatChangeAction(slot, slot, ability.TargetStat.Value, ability.StatStages));

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (no aplica para Speed Boost).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento.</param>
        /// <param name="originalValue">El valor original.</param>
        /// <param name="valueType">El tipo de valor.</param>
        /// <returns>Null (no modifica valores).</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            return null; // Speed Boost no modifica valores directamente, genera acciones
        }
    }
}
