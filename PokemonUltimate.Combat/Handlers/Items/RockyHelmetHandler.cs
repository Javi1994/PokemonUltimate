using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Handlers.Items
{
    /// <summary>
    /// Handler para Rocky Helmet que daña al atacante cuando hace contacto.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class RockyHelmetHandler : IItemEffectHandler, IContactItemHandler
    {
        /// <summary>
        /// El trigger que activa este handler.
        /// </summary>
        public ItemTrigger Trigger => ItemTrigger.OnContactReceived;

        /// <summary>
        /// Verifica si puede manejar este item.
        /// </summary>
        /// <param name="item">El item data a verificar. No puede ser null.</param>
        /// <returns>True si es Rocky Helmet, false en caso contrario.</returns>
        public bool CanHandle(ItemData item)
        {
            return item != null && item.Id == "rocky-helmet";
        }

        /// <summary>
        /// Verifica si el Pokemon tiene Rocky Helmet.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Rocky Helmet, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            return pokemon?.HeldItem != null && CanHandle(pokemon.HeldItem);
        }

        /// <summary>
        /// Procesa el efecto del item (sin información del atacante).
        /// </summary>
        /// <param name="item">El item data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista vacía (se requiere ProcessWithAttacker para efectos de contacto).</returns>
        public List<BattleAction> Process(ItemData item, BattleSlot slot, BattleField field)
        {
            // Para efectos de contacto, se requiere información del atacante
            return new List<BattleAction>();
        }

        /// <summary>
        /// Procesa el efecto del item con información del atacante.
        /// </summary>
        /// <param name="item">El item data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item (defensor). No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="attacker">El slot del atacante que hizo contacto. No puede ser null.</param>
        /// <returns>Lista de acciones generadas.</returns>
        public List<BattleAction> ProcessWithAttacker(ItemData item, BattleSlot slot, BattleField field, BattleSlot attacker)
        {
            var actions = new List<BattleAction>();

            if (attacker == null || attacker.IsEmpty || attacker.HasFainted)
                return actions;

            // Rocky Helmet daña 1/6 del Max HP del atacante
            int damage = attacker.Pokemon.MaxHP / 6;
            if (damage < 1)
                damage = 1;

            // Mensaje de activación del item
            var provider = LocalizationService.Instance;
            var itemName = item.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.ItemActivated, slot.Pokemon.DisplayName, itemName)));

            // Aplicar daño de contacto
            actions.Add(new ContactDamageAction(attacker, damage, itemName));

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (no aplica para Rocky Helmet).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento.</param>
        /// <param name="originalValue">El valor original.</param>
        /// <param name="valueType">El tipo de valor.</param>
        /// <returns>Null (no modifica valores directamente).</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            return null; // Rocky Helmet no modifica valores directamente, genera acciones
        }
    }
}
