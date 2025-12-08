using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Handlers.Abilities
{
    /// <summary>
    /// Handler genérico para habilidades que activan efectos al recibir contacto (Static, Rough Skin, Iron Barbs, etc.).
    /// Maneja tanto ChanceToStatusOnContact como DamageOnContact.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class ContactAbilityHandler : IAbilityEffectHandler, IContactAbilityHandler
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// El trigger que activa este handler.
        /// </summary>
        public AbilityTrigger Trigger => AbilityTrigger.OnContactReceived;

        /// <summary>
        /// El efecto de habilidad que maneja este handler.
        /// Nota: Este handler maneja múltiples efectos, pero se registra manualmente por trigger+efecto.
        /// </summary>
        public AbilityEffect Effect => AbilityEffect.ChanceToStatusOnContact; // Valor por defecto, pero maneja ambos

        /// <summary>
        /// Crea un nuevo ContactAbilityHandler.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        public ContactAbilityHandler(IRandomProvider randomProvider = null)
        {
            _randomProvider = randomProvider ?? new Infrastructure.Providers.RandomProvider();
        }

        /// <summary>
        /// Verifica si el Pokemon tiene una habilidad de contacto.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene una habilidad de contacto, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            return pokemon?.Ability != null &&
                   pokemon.Ability.ListensTo(AbilityTrigger.OnContactReceived) &&
                   (pokemon.Ability.Effect == AbilityEffect.ChanceToStatusOnContact ||
                    pokemon.Ability.Effect == AbilityEffect.DamageOnContact);
        }

        /// <summary>
        /// Procesa el efecto de la habilidad (sin información del atacante).
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad. No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Lista vacía (se requiere ProcessWithAttacker para efectos de contacto).</returns>
        public List<BattleAction> Process(AbilityData ability, BattleSlot slot, BattleField field)
        {
            // Para efectos de contacto, se requiere información del atacante
            return new List<BattleAction>();
        }

        /// <summary>
        /// Procesa el efecto de la habilidad con información del atacante.
        /// </summary>
        /// <param name="ability">La habilidad data. No puede ser null.</param>
        /// <param name="slot">El slot del Pokemon con esta habilidad (defensor). No puede ser null.</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <param name="attacker">El slot del atacante que hizo contacto. No puede ser null.</param>
        /// <returns>Lista de acciones generadas.</returns>
        public List<BattleAction> ProcessWithAttacker(AbilityData ability, BattleSlot slot, BattleField field, BattleSlot attacker)
        {
            var actions = new List<BattleAction>();

            if (attacker == null || !attacker.IsActive() || attacker.HasFainted)
                return actions;

            // Procesar según el tipo de efecto
            switch (ability.Effect)
            {
                case AbilityEffect.ChanceToStatusOnContact:
                    actions.AddRange(ProcessChanceToStatusOnContact(ability, slot, attacker, field));
                    break;

                case AbilityEffect.DamageOnContact:
                    actions.AddRange(ProcessDamageOnContact(ability, slot, attacker, field));
                    break;
            }

            return actions;
        }

        /// <summary>
        /// Procesa ChanceToStatusOnContact (Static, Poison Point, Flame Body).
        /// </summary>
        private List<BattleAction> ProcessChanceToStatusOnContact(AbilityData ability, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.StatusEffect == null)
                return actions;

            // Verificar si el atacante ya tiene un status
            if (attacker.Pokemon.Status != PersistentStatus.None)
                return actions;

            // Calcular probabilidad (default 30%)
            float chance = ability.EffectChance > 0 ? ability.EffectChance : 0.30f;

            // Verificar probabilidad
            if (_randomProvider.NextDouble() >= chance)
                return actions;

            // Mensaje de activación de habilidad
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, defender.Pokemon.DisplayName, abilityName)));

            // Aplicar status al atacante
            actions.Add(new ApplyStatusAction(defender, attacker, ability.StatusEffect.Value));

            return actions;
        }

        /// <summary>
        /// Procesa DamageOnContact (Rough Skin, Iron Barbs).
        /// </summary>
        private List<BattleAction> ProcessDamageOnContact(AbilityData ability, BattleSlot defender, BattleSlot attacker, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Calcular daño (default 1/8 de Max HP para Rough Skin)
            float damageMultiplier = ability.Multiplier > 0 ? ability.Multiplier : 0.125f;
            int damage = (int)(attacker.Pokemon.MaxHP * damageMultiplier);
            if (damage < 1)
                damage = 1;

            // Mensaje de activación de habilidad
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, defender.Pokemon.DisplayName, abilityName)));

            // Aplicar daño de contacto
            actions.Add(new ContactDamageAction(attacker, damage, abilityName));

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento (no aplica para efectos de contacto).
        /// </summary>
        /// <param name="pokemon">El Pokemon con el comportamiento.</param>
        /// <param name="originalValue">El valor original.</param>
        /// <param name="valueType">El tipo de valor.</param>
        /// <returns>Null (no modifica valores directamente).</returns>
        public int? ModifyValue(PokemonInstance pokemon, int originalValue, string valueType)
        {
            return null; // Los efectos de contacto no modifican valores directamente, generan acciones
        }
    }
}
