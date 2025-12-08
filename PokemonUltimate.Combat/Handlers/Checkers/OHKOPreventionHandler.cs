using System;
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

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler unificado para prevención de OHKO (Focus Sash, Sturdy).
    /// Puede verificar comportamientos (usado en Actions) y procesar efectos (usado en Steps).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class OHKOPreventionHandler : IAbilityEffectHandler, IItemEffectHandler
    {
        /// <summary>
        /// El trigger que activa este handler (None porque es principalmente verificación).
        /// </summary>
        public AbilityTrigger Trigger => AbilityTrigger.OnWouldFaint;

        /// <summary>
        /// El efecto de habilidad que maneja este handler.
        /// </summary>
        public AbilityEffect Effect => AbilityEffect.SurviveFatalHit;

        /// <summary>
        /// El trigger de item que activa este handler.
        /// </summary>
        ItemTrigger IItemEffectHandler.Trigger => ItemTrigger.OnWouldFaint;

        /// <summary>
        /// Verifica si puede manejar este item (Focus Sash).
        /// </summary>
        /// <param name="item">El item data a verificar. No puede ser null.</param>
        /// <returns>True si es Focus Sash, false en caso contrario.</returns>
        public bool CanHandle(ItemData item)
        {
            if (item == null)
                return false;

            // Verificar si es Focus Sash por ID o nombre
            return item.Id == "focus-sash" || item.Name.Equals("Focus Sash", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el Pokemon puede prevenir OHKO (tiene Focus Sash o Sturdy y está a HP completo).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon puede prevenir OHKO, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return false;

            // Verificar Focus Sash (item)
            if (pokemon.HeldItem?.Id == "focus-sash")
            {
                return pokemon.CurrentHP >= pokemon.MaxHP; // Solo funciona a HP completo
            }

            // Verificar Sturdy (habilidad)
            if (pokemon.Ability?.Id == "sturdy")
            {
                return pokemon.CurrentHP >= pokemon.MaxHP; // Solo funciona a HP completo
            }

            return false;
        }

        /// <summary>
        /// Procesa el efecto de prevención de OHKO para habilidades (Sturdy).
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista de acciones generadas (mensajes de activación).</returns>
        public List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!HasBehavior(slot.Pokemon))
                return actions;

            // Mensaje de activación de habilidad
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, slot.Pokemon.DisplayName, abilityName)));
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.EnduredHit, slot.Pokemon.DisplayName)));

            return actions;
        }

        /// <summary>
        /// Procesa el efecto de prevención de OHKO para items (Focus Sash).
        /// </summary>
        /// <param name="item">El item data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista de acciones generadas (mensajes y consumo del item).</returns>
        List<BattleAction> IItemEffectHandler.Process(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (!HasBehavior(slot.Pokemon))
                return actions;

            // Consumir el item
            slot.Pokemon.HeldItem = null;

            // Mensajes de activación del item
            var provider = LocalizationService.Instance;
            var itemName = item.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.ItemActivated, slot.Pokemon.DisplayName, itemName)));
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.HeldOnUsingItem, slot.Pokemon.DisplayName, itemName)));

            return actions;
        }

        /// <summary>
        /// Obtiene el tipo de prevención de OHKO (Item o Ability).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>El tipo de prevención, o None si no puede prevenir OHKO.</returns>
        public OHKOPreventionType GetPreventionType(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return OHKOPreventionType.None;

            // Verificar Focus Sash primero (tiene prioridad)
            if (pokemon.HeldItem?.Id == "focus-sash" && pokemon.CurrentHP >= pokemon.MaxHP)
                return OHKOPreventionType.Item;

            // Verificar Sturdy
            if (pokemon.Ability?.Id == "sturdy" && pokemon.CurrentHP >= pokemon.MaxHP)
                return OHKOPreventionType.Ability;

            return OHKOPreventionType.None;
        }

        /// <summary>
        /// Calcula el daño modificado después de prevenir OHKO.
        /// Deja al Pokemon con 1 HP si puede prevenir el OHKO.
        /// </summary>
        /// <param name="pokemon">El Pokemon que recibe el daño. No puede ser null.</param>
        /// <param name="originalDamage">El daño original que se aplicaría.</param>
        /// <returns>El daño modificado (deja 1 HP) si puede prevenir OHKO, o el daño original en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si pokemon es null.</exception>
        public int CalculateModifiedDamage(PokemonInstance pokemon, int originalDamage)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            if (!HasBehavior(pokemon))
                return originalDamage;

            // Reducir daño para dejar al Pokemon con 1 HP
            return Math.Max(0, pokemon.CurrentHP - 1);
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (modifica daño para prevenir OHKO).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento. No puede ser null.</param>
        /// <param name="originalValue">El valor original (daño).</param>
        /// <param name="valueType">El tipo de valor ("damage").</param>
        /// <returns>El daño modificado si aplica, o null si no se aplica modificación.</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            if (valueType != "damage")
                return null;

            if (!HasBehavior(pokemon))
                return null;

            // Verificar si el daño causaría desmayo
            if (pokemon.CurrentHP <= originalValue)
            {
                return CalculateModifiedDamage(pokemon, originalValue);
            }

            return null;
        }
    }

    /// <summary>
    /// Tipo de prevención de OHKO.
    /// </summary>
    public enum OHKOPreventionType
    {
        /// <summary>
        /// No puede prevenir OHKO.
        /// </summary>
        None,

        /// <summary>
        /// Previene OHKO mediante item (Focus Sash).
        /// </summary>
        Item,

        /// <summary>
        /// Previene OHKO mediante habilidad (Sturdy).
        /// </summary>
        Ability
    }
}
