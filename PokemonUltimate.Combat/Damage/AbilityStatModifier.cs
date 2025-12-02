using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Damage
{
    /// <summary>
    /// Adapter that converts AbilityData to IStatModifier for passive stat/damage modifications.
    /// </summary>
    public class AbilityStatModifier : IStatModifier
    {
        private readonly AbilityData _abilityData;

        /// <summary>
        /// Creates a new ability stat modifier adapter.
        /// </summary>
        /// <param name="abilityData">The ability data to adapt.</param>
        public AbilityStatModifier(AbilityData abilityData)
        {
            _abilityData = abilityData ?? throw new ArgumentNullException(nameof(abilityData), ErrorMessages.AbilityCannotBeNull);
        }

        /// <summary>
        /// Gets the stat multiplier from the ability.
        /// Currently no abilities provide passive stat multipliers (future: Huge Power, Pure Power).
        /// </summary>
        public float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Future: Check for passive stat multiplier abilities
            // For now, return 1.0f (no modification)
            return 1.0f;
        }

        /// <summary>
        /// Gets the damage multiplier from the ability.
        /// Handles abilities like Blaze, Torrent, Overgrow that boost damage when HP is low.
        /// </summary>
        public float GetDamageMultiplier(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            // Check for HP threshold abilities (Blaze, Torrent, Overgrow)
            if (_abilityData.HPThreshold > 0f && _abilityData.AffectedType.HasValue)
            {
                var pokemon = context.Attacker.Pokemon;
                float hpPercent = (float)pokemon.CurrentHP / pokemon.MaxHP;

                // Check if HP is below threshold and move type matches
                if (hpPercent <= _abilityData.HPThreshold && 
                    context.Move.Type == _abilityData.AffectedType.Value)
                {
                    return _abilityData.Multiplier;
                }
            }

            return 1.0f;
        }
    }
}

