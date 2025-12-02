using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Damage
{
    /// <summary>
    /// Adapter that converts ItemData to IStatModifier for passive stat/damage modifications.
    /// </summary>
    public class ItemStatModifier : IStatModifier
    {
        private readonly ItemData _itemData;

        /// <summary>
        /// Creates a new item stat modifier adapter.
        /// </summary>
        /// <param name="itemData">The item data to adapt.</param>
        public ItemStatModifier(ItemData itemData)
        {
            _itemData = itemData ?? throw new ArgumentNullException(nameof(itemData), ErrorMessages.ItemCannotBeNull);
        }

        /// <summary>
        /// Gets the stat multiplier from the item.
        /// Handles items like Choice Band (+50% Attack), Assault Vest (+50% SpDef), etc.
        /// </summary>
        public float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Check if this item modifies the requested stat
            if (_itemData.TargetStat == stat && _itemData.StatMultiplier > 0f)
            {
                return _itemData.StatMultiplier;
            }

            return 1.0f;
        }

        /// <summary>
        /// Gets the damage multiplier from the item.
        /// Handles items like Life Orb (+30% damage).
        /// </summary>
        public float GetDamageMultiplier(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            // Check if this item provides a damage multiplier
            if (_itemData.DamageMultiplier > 0f)
            {
                return _itemData.DamageMultiplier;
            }

            return 1.0f;
        }
    }
}

