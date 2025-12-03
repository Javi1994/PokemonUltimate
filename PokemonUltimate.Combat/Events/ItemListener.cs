using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Events
{
    /// <summary>
    /// Adapts ItemData to IBattleListener interface.
    /// Converts item triggers to battle actions based on item configuration.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.9: Abilities & Items
    /// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
    /// </remarks>
    public class ItemListener : IBattleListener
    {
        private readonly ItemData _itemData;

        /// <summary>
        /// Creates a new item listener for the given item data.
        /// </summary>
        /// <param name="itemData">The item data to listen with. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If itemData is null.</exception>
        public ItemListener(ItemData itemData)
        {
            _itemData = itemData ?? throw new ArgumentNullException(nameof(itemData), ErrorMessages.ItemCannotBeNull);
        }

        /// <summary>
        /// Processes battle triggers and generates actions based on item configuration.
        /// </summary>
        public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field)
        {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), ErrorMessages.SlotCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Check if this item listens to this trigger
            if (!ShouldRespondToTrigger(trigger))
                yield break;

            // Process based on item type
            foreach (var action in ProcessItemEffect(trigger, holder, field))
            {
                yield return action;
            }
        }

        /// <summary>
        /// Checks if this item should respond to the given trigger.
        /// </summary>
        private bool ShouldRespondToTrigger(BattleTrigger trigger)
        {
            switch (trigger)
            {
                case BattleTrigger.OnTurnEnd:
                    return _itemData.ListensTo(ItemTrigger.OnTurnEnd);
                case BattleTrigger.OnDamageTaken:
                    return _itemData.ListensTo(ItemTrigger.OnContactReceived);
                case BattleTrigger.OnAfterMove:
                    return _itemData.ListensTo(ItemTrigger.OnDamageDealt);
                case BattleTrigger.OnWeatherChange:
                    return false; // Items don't respond to weather changes
                case BattleTrigger.OnSwitchIn:
                    return false; // Items don't trigger on switch-in
                case BattleTrigger.OnBeforeMove:
                    return false; // Items don't trigger before move (except Choice items, deferred)
                default:
                    return false;
            }
        }

        /// <summary>
        /// Processes item effects and returns actions.
        /// </summary>
        private IEnumerable<BattleAction> ProcessItemEffect(BattleTrigger trigger, BattleSlot holder, BattleField field)
        {
            // End-of-turn healing (Leftovers, Black Sludge)
            if (trigger == BattleTrigger.OnTurnEnd && _itemData.ListensTo(ItemTrigger.OnTurnEnd))
            {
                foreach (var action in ProcessEndOfTurnHealing(holder))
                    yield return action;
            }
        }

        /// <summary>
        /// Processes end-of-turn healing items (Leftovers, Black Sludge).
        /// </summary>
        private IEnumerable<BattleAction> ProcessEndOfTurnHealing(BattleSlot holder)
        {
            var pokemon = holder.Pokemon;
            
            // Don't heal if already at full HP
            if (pokemon.CurrentHP >= pokemon.MaxHP)
                yield break;

            // Calculate heal amount
            int healAmount;
            if (_itemData.HealAmount > 0)
            {
                // Fixed amount healing
                healAmount = _itemData.HealAmount;
            }
            else if (_itemData.HealAmount < 0)
            {
                // Percentage-based healing (stored as negative value)
                // HealAmount = -(percent * 100), so percent = -HealAmount / 100
                float percent = -_itemData.HealAmount / 100f;
                healAmount = (int)(pokemon.MaxHP * percent);
            }
            else
            {
                // Default: Leftovers-style healing (1/16)
                healAmount = pokemon.MaxHP / 16;
            }

            // Minimum 1 HP healed
            if (healAmount < 1)
                healAmount = 1;

            // Message
            yield return new MessageAction(string.Format(GameMessages.ItemActivated, pokemon.DisplayName, _itemData.Name));

            // Healing action
            yield return new HealAction(holder, holder, healAmount);
        }
    }
}

