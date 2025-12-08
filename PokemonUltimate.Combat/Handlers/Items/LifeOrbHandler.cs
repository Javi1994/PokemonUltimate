using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Handler para items que causan daño de retroceso después de infligir daño (Life Orb, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class LifeOrbHandler : IItemEffectHandler
    {
        /// <summary>
        /// El trigger que activa este handler.
        /// </summary>
        public ItemTrigger Trigger => ItemTrigger.OnDamageDealt;

        /// <summary>
        /// Verifica si puede manejar este item.
        /// </summary>
        /// <param name="item">El item data a verificar. No puede ser null.</param>
        /// <returns>True si el item causa daño de retroceso, false en caso contrario.</returns>
        public bool CanHandle(ItemData item)
        {
            if (item == null)
                return false;

            // Maneja items que causan daño de retroceso después de infligir daño
            return item.ListensTo(ItemTrigger.OnDamageDealt) && item.RecoilPercent > 0;
        }

        /// <summary>
        /// Verifica si el Pokemon tiene un item que causa daño de retroceso.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene un item con retroceso, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            return pokemon?.HeldItem != null && CanHandle(pokemon.HeldItem);
        }

        /// <summary>
        /// Procesa el efecto del item y genera acciones de daño de retroceso.
        /// </summary>
        /// <param name="item">El item data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con este item. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista de acciones generadas.</returns>
        public List<BattleAction> Process(ItemData item, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Verificar si se infligió daño este turno
            var opposingSide = field.GetOppositeSide(slot.Side);
            bool damageWasDealt = opposingSide.GetActiveSlots()
                .Any(s => s.PhysicalDamageTakenThisTurn > 0 || s.SpecialDamageTakenThisTurn > 0);

            if (!damageWasDealt)
                return actions;

            // Calcular daño de retroceso (típicamente 10% del HP máximo para Life Orb)
            int recoilDamage = CalculateRecoilDamage(item, slot.Pokemon);
            if (recoilDamage <= 0)
                return actions;

            // Mensaje de activación del item
            var provider = LocalizationService.Instance;
            var itemName = item.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.ItemActivated, slot.Pokemon.DisplayName, itemName)));

            // Aplicar daño de retroceso directamente
            slot.Pokemon.TakeDamage(recoilDamage);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.HurtByRecoil, slot.Pokemon.DisplayName)));

            return actions;
        }

        /// <summary>
        /// Calcula el daño de retroceso para un item.
        /// </summary>
        /// <param name="item">El item data.</param>
        /// <param name="pokemon">El Pokemon que recibirá el retroceso.</param>
        /// <returns>La cantidad de daño de retroceso.</returns>
        private int CalculateRecoilDamage(ItemData item, PokemonInstance pokemon)
        {
            if (item.RecoilPercent > 0)
            {
                // Calcular basado en porcentaje del HP máximo
                int recoilDamage = (int)(pokemon.MaxHP * item.RecoilPercent);
                return Math.Max(1, recoilDamage); // Mínimo 1 de daño
            }

            // Por defecto: estilo Life Orb (10% del HP máximo)
            return Math.Max(1, pokemon.MaxHP / 10);
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (no aplica para Life Orb en este contexto).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento.</param>
        /// <param name="originalValue">El valor original.</param>
        /// <param name="valueType">El tipo de valor.</param>
        /// <returns>Null (no modifica valores directamente en este contexto).</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            return null; // Life Orb genera acciones de daño, no modifica valores directamente aquí
        }
    }
}
