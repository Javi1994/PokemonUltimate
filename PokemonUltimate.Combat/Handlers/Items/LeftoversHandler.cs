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

namespace PokemonUltimate.Combat.Handlers.Items
{
    /// <summary>
    /// Handler para items que curan al final de cada turno (Leftovers, Black Sludge, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class LeftoversHandler : IItemEffectHandler
    {
        /// <summary>
        /// El trigger que activa este handler.
        /// </summary>
        public ItemTrigger Trigger => ItemTrigger.OnTurnEnd;

        /// <summary>
        /// Verifica si puede manejar este item.
        /// </summary>
        /// <param name="item">El item data a verificar. No puede ser null.</param>
        /// <returns>True si el item cura al final del turno, false en caso contrario.</returns>
        public bool CanHandle(ItemData item)
        {
            if (item == null)
                return false;

            // Maneja items que curan al final del turno
            return item.ListensTo(ItemTrigger.OnTurnEnd) && item.HealAmount > 0;
        }

        /// <summary>
        /// Verifica si el Pokemon tiene un item que cura al final del turno.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene un item que cura, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            return pokemon?.HeldItem != null && CanHandle(pokemon.HeldItem);
        }

        /// <summary>
        /// Procesa el efecto del item y genera acciones de curación.
        /// </summary>
        /// <param name="item">El item data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista de acciones generadas.</returns>
        public List<BattleAction> Process(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            // No curar si ya está a HP completo
            if (pokemon.CurrentHP >= pokemon.MaxHP)
                return actions;

            // Calcular cantidad de curación
            int healAmount = CalculateHealAmount(item, pokemon);

            // Mensaje de activación del item
            var provider = LocalizationService.Instance;
            var itemName = item.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.ItemActivated, pokemon.DisplayName, itemName)));

            // Acción de curación
            actions.Add(new HealAction(slot, slot, healAmount));

            return actions;
        }

        /// <summary>
        /// Calcula la cantidad de curación para un item.
        /// </summary>
        /// <param name="item">El item data.</param>
        /// <param name="pokemon">El Pokemon que será curado.</param>
        /// <returns>La cantidad de curación.</returns>
        private int CalculateHealAmount(ItemData item, PokemonInstance pokemon)
        {
            if (item.HealAmount > 0)
                return item.HealAmount;

            // Por defecto: estilo Leftovers (1/16)
            return Math.Max(1, pokemon.MaxHP / 16);
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (no aplica para Leftovers).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento.</param>
        /// <param name="originalValue">El valor original.</param>
        /// <param name="valueType">El tipo de valor.</param>
        /// <returns>Null (no modifica valores directamente).</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            return null; // Leftovers no modifica valores directamente, genera acciones
        }
    }
}
