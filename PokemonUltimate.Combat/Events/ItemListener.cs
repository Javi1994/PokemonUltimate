using System;
using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Constants;
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

        // Dictionary mapping BattleTrigger to ItemTrigger for efficient lookup
        private static readonly Dictionary<BattleTrigger, Core.Enums.ItemTrigger?> TriggerMapping = new Dictionary<BattleTrigger, Core.Enums.ItemTrigger?>
        {
            { BattleTrigger.OnTurnEnd, Core.Enums.ItemTrigger.OnTurnEnd },
            { BattleTrigger.OnDamageTaken, Core.Enums.ItemTrigger.OnContactReceived },
            { BattleTrigger.OnAfterMove, Core.Enums.ItemTrigger.OnDamageDealt },
            { BattleTrigger.OnWeatherChange, null }, // Items don't respond to weather changes
            { BattleTrigger.OnSwitchIn, null }, // Items don't trigger on switch-in
            { BattleTrigger.OnBeforeMove, null } // Items don't trigger before move (except Choice items, deferred)
        };

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
        /// Uses dictionary lookup for efficient trigger mapping.
        /// </summary>
        private bool ShouldRespondToTrigger(BattleTrigger trigger)
        {
            if (TriggerMapping.TryGetValue(trigger, out var itemTrigger))
            {
                // If mapping is null, item doesn't respond to this trigger
                if (itemTrigger == null)
                    return false;

                return _itemData.ListensTo(itemTrigger.Value);
            }

            return false;
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
                healAmount = pokemon.MaxHP / ItemConstants.LeftoversHealDivisor;
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

